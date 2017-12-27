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

    private Transform _target = null;

	private Animator _animator = null;

	private NavMeshAgent _agent = null;

	private Mob _mob = null;
    private int cutNum = 10;      // 列を区切る番号

	[SerializeField]
    private float _mobInterval = 0.5f;

	private float _angle = 0.2f;
	private float _angle2 = -0.2f;
	private Vector3 _targetPosition;
    //public int fanNum = 0;
    //public float testRot = 0.0f;

	private static int _animIdIsStop = Animator.StringToHash("IsStop");
	private static int _animIdDistance = Animator.StringToHash("Distance");

	/// <summary>
	/// 初期化処理
	/// </summary>
	public void OnStart(Transform target, Animator animator, NavMeshAgent agent, Mob mob)
    {
        _target = target;
        enabled = true;
		_animator = animator;
		_mob = mob;
		_agent = agent;
		_mobInterval = 2.0f;
		
	}

    void OnDisable()
    {
        _target = null;
		_animator = null;
	}

    void Update()
    {
        if (_target == null)
        {
            Debug.Log("追従対象が見つからない為、非アクティブになります");
            enabled = false;
            return;
        }

		_targetPosition = _target.position + GetPlayerOffset(_mob._followInex);

		// ナビメッシュの移動目標座標を設定する
		_agent.SetDestination(_targetPosition);

		float distance = Vector3.Distance(_targetPosition, transform.position);
		bool isStopped = false;

		// 距離を設定
		_animator.SetFloat(_animIdDistance, distance);

		// 一定の距離より近づいた場合true
		isStopped = distance < 0.5f ? true : false;

		// フラグが変化した場合にみアニメーションに変更を通知する
		if(isStopped != _animator.GetBool(_animIdIsStop))
		{
			_animator.SetBool(_animIdIsStop, isStopped);
		}

		// 遷移チェック
		_onTransCheck?.Invoke();
    }

	/// <summary>
	/// 追従番号に応じたプレイヤーの相対座標を取得する
	/// </summary>
	private Vector3 GetPlayerOffset(int followIndex)
	{
		return new Vector3(
		Mathf.Sin(_target.eulerAngles.y * Mathf.Deg2Rad + _angle - ((followIndex % 2) * _angle2)) * (-_mobInterval * (followIndex % cutNum + 1)) +
		Mathf.Sin(_target.eulerAngles.y * Mathf.Deg2Rad) * ((_mobInterval) * (followIndex / cutNum)),
		0.0f,
		Mathf.Cos(_target.eulerAngles.y * Mathf.Deg2Rad + _angle - ((followIndex % 2) * _angle2)) * (-_mobInterval * (followIndex % cutNum + 1)) +
		Mathf.Cos(_target.eulerAngles.y * Mathf.Deg2Rad) * ((_mobInterval) * (followIndex / cutNum)));
	}

	// Gizmo描画
	void OnDrawGizmos()
	{
		if (!enabled)
			return;

		Gizmos.color = Color.green;
		Gizmos.DrawSphere(_targetPosition, 0.5f);
	}
}