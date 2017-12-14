using System;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 追従移動 クラス
/// Rigitbodyを使用せず、直接座標を設定します
/// 製作者：実川
/// </summary>
public class FollowMove : MonoBehaviour, IMove
{
	/// <summary>
	/// 遷移条件判定イベント
	/// FixedUpdateのタイミングで呼ばれます
	/// </summary>
	public Action OnTransCheck
	{
		set { _onTransCheck += value; }
	}

	private Action _onTransCheck;

	/// <summary>
	/// 移動する速度
	/// </summary>
	[SerializeField]
	private float _velocity;

	/// <summary>
	/// 移動を中止する距離
	/// </summary>
	[SerializeField]
	private float _stopDistance;

	/// <summary>
	/// 速度の減少を開始する距離
	/// </summary>
	[SerializeField]
	private float _slowDistance;

	private Transform _target = null;

    private NavMeshAgent _agent = null;
	
    /// <summary>
	/// 初期化処理
	/// </summary>
	public void OnStart(Transform target)
	{
		_target = target;
		enabled = true;
        _agent = GetComponent<NavMeshAgent>();
        _agent.enabled = true;
	}

	void OnDisable()
	{
		_target = null;
        _agent = null;
	}

	void Update()
	{
		if (_target == null)
		{
			Debug.Log("追従対象が見つからない為、非アクティブになります");
			enabled = false;
			return;
		}

        // 目標座標設定
        Vector3 goal = _target.position;
        goal.x += -Mathf.Sin(( _target.rotation.y - 180.0f ) * Mathf.PI) * 4.0f;
        goal.z += -Mathf.Cos(( _target.rotation.y - 180.0f ) * Mathf.PI) * 4.0f;
        _agent.SetDestination(goal);
		
        // 遷移チェック
		_onTransCheck?.Invoke();
	}
}