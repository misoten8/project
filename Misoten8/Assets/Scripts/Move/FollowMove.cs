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
    private float _mobInterval = 0.0f;

	/// <summary>
	/// 移動を停止する目標座標までの距離
	/// </summary>
	[SerializeField]
	private float _toStopDistance = 0.0f;

	//private float _angle = 0.2f;
	//private float _angle2 = -0.2f;

	/// <summary>
	/// 移動目標座標
	/// </summary>
	private Vector3 _targetPosition;

	/// <summary>
	/// ターン中かどうか
	/// </summary>
	private bool _isTurn = false;
	/// <summary>
	/// ターン開始時のオイラー角
	/// </summary>
	private float _startEulerAngle = 0.0f;
	/// <summary>
	/// ターン終了時のオイラー角
	/// </summary>
	private float _endEulerAngle = 0.0f;
	/// <summary>
	/// ターン完了までの残り時間
	/// </summary>
	/// <remarks>
	/// ターン時間は1秒
	/// </remarks>
	private float _limitTime = _TURN_TIME;
	/// <summary>
	/// ターン時間
	/// </summary>
	private const float _TURN_TIME = 1.0f;
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

		// ターン処理
		Turn(isStopped);

		// フラグが変化した場合のみアニメーションに変更を通知する
		if (isStopped != _animator.GetBool(_animIdIsStop))
		{
			_animator.SetBool(_animIdIsStop, isStopped);

			// ターン開始
			if(isStopped)
			{		
				_startEulerAngle = transform.eulerAngles.y;
				if (_player.Dance.IsPlaying)
				{
					Vector3 diff = _player.transform.position - transform.position;
					_endEulerAngle = Mathf.Atan2(diff.x, diff.z) * Mathf.Rad2Deg;
				}
				else
				{
					_endEulerAngle = _player.transform.eulerAngles.y;
				}
				_isTurn = true;
				_limitTime = _TURN_TIME;
				_agent.updateRotation = false;
			}
			else
			{
				_agent.updateRotation = true;
			}
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
			angleRight = _player?.RankAngleRight ?? 1.0f,
			offsetZ = _player?.Dance.IsPlaying ?? false ? -3.0f : 0.0f;// ダンス視聴中なら隊列を後ろに下げる

		return new Vector3(
		Mathf.Sin(_target.eulerAngles.y * Mathf.Deg2Rad + angleLeft - ((followIndex % 2) * angleRight)) * 
		(-_mobInterval * (followIndex % cutNum + 1)) +
		Mathf.Sin(_target.eulerAngles.y * Mathf.Deg2Rad) * 
		((-_mobInterval) * (followIndex / cutNum)),
		0.0f,
		(Mathf.Cos(_target.eulerAngles.y * Mathf.Deg2Rad + angleLeft - ((followIndex % 2) * angleRight)) * 
		(-_mobInterval * (followIndex % cutNum + 1)) +
		Mathf.Cos(_target.eulerAngles.y * Mathf.Deg2Rad) * 
		((-_mobInterval) * (followIndex / cutNum))) +
		offsetZ);
	}

	/// <summary>
	/// ターン処理
	/// </summary>
	private void Turn(bool isStopped)
	{
		if (_isTurn)
		{
			// 停止中のみターンする
			if (isStopped)
			{
				_limitTime -= Time.deltaTime;
				_limitTime = Mathf.Max(_limitTime, 0.0f);
				transform.eulerAngles = new Vector3(transform.eulerAngles.x, Mathf.LerpAngle(_startEulerAngle, _endEulerAngle, 1.0f - _limitTime / _TURN_TIME), transform.eulerAngles.z);

				if (_limitTime <= 0.0f)
				{
					// ターン終了
					_isTurn = false;
				}
			}
			else
			{
				// ターン終了
				_isTurn = false;
			}
		}
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