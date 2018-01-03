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

	private PlayerManager _playerManager = null;

	private Player _player = null;

	private Mob _mob = null;
    private int cutNum = 10;      // 列を区切る番号

	/// <summary>
	/// モブ同士の位置間隔
	/// </summary>
	[SerializeField]
    private float _mobInterval = 0.025f;

	/// <summary>
	/// 移動を停止する目標座標までの距離
	/// </summary>
	[SerializeField]
	private float _toStopDistance = 0.25f;

	//private float _angle = 0.2f;
	//private float _angle2 = -0.2f;

	/// <summary>
	/// 移動目標座標
	/// </summary>
	private Vector3 _targetPosition;

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
		_playerManager = _mob.PlayerManager;
		_player = _playerManager.GetPlayer(_mob.FllowTarget);
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

		// 一致しない場合再取得する
		if(_mob.FllowTarget != _player?.Type)
		{
			_player = _playerManager.GetPlayer(_mob.FllowTarget);
		}

		// 隊列座標を求める
		Vector3 targetPosition = _target.position + GetPlayerOffset(_mob._followInex);

		// 座標が変化した場合のみナビメッシュに変更を通知する
		if(_targetPosition != targetPosition)
		{
			_targetPosition = targetPosition;

			// ナビメッシュの移動目標座標を設定する
			_agent.SetDestination(_targetPosition);
		}

		float distance = Vector3.Distance(_targetPosition, transform.position);

		// 距離を設定
		_animator.SetFloat(_animIdDistance, distance);

		// 距離判定
		bool isStopped = distance < _toStopDistance ? true : false;

		// フラグが変化した場合のみアニメーションに変更を通知する
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
		float
			angleLeft = _player?.RankAngleLeft ?? 0.5f,
			angleRight = _player?.RankAngleRight ?? 1.0f;

		return new Vector3(
		Mathf.Sin(_target.eulerAngles.y * Mathf.Deg2Rad + angleLeft - ((followIndex % 2) * angleRight)) * 
		(-_mobInterval * (followIndex % cutNum + 1)) +
		Mathf.Sin(_target.eulerAngles.y * Mathf.Deg2Rad) * 
		((-_mobInterval) * (followIndex / cutNum)),
		0.0f,
		Mathf.Cos(_target.eulerAngles.y * Mathf.Deg2Rad + angleLeft - ((followIndex % 2) * angleRight)) * 
		(-_mobInterval * (followIndex % cutNum + 1)) +
		Mathf.Cos(_target.eulerAngles.y * Mathf.Deg2Rad) * 
		((-_mobInterval) * (followIndex / cutNum)));
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