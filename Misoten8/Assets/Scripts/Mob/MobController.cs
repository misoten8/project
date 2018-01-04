using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// モブキャラ操作 クラス
/// 移動処理等を行います
/// 製作者：実川
/// </summary>
public class MobController : MonoBehaviour
{
	[SerializeField]
	private Mob _mob;

	[SerializeField]
	private FollowMove _followMove;

	[SerializeField]
	private WanderMove _wanderMove;

    [SerializeField]
    private NavMeshAgent _agent;

	private Animator _animator = null;
	/// <summary>
	/// 現在の移動処理
	/// </summary>
	//private IMove _currentMove;

	public void OnStart(Animator anim)
	{
		_animator = anim;
		
		_agent.updatePosition = false;

		// モブ再生イベントで実行する処理を追加
		_mob.onMoveMob += () =>
		{
			if (_mob.FllowTarget == Define.PlayerType.None)
			{
				_wanderMove.OnStart();
			}
			else
			{
				if (_mob.FunType == Define.PlayerType.None)
				{
					_followMove.OnStart(_mob.PlayerManager.GetPlayer(_mob.FllowTarget)?.transform, _animator, _agent, _mob);
				}
				else
				{
					_followMove.OnStart(_mob.funPlayer.transform, _animator, _agent, _mob);
				}
				_wanderMove.enabled = false;
			}
		};

		// モブ停止イベントで実行する処理を追加
		_mob.onDanceWatchMob += () =>
		{
			_followMove.enabled = false;
			_wanderMove.enabled = false;
		};

		// 追従対象プレイヤー変更イベント
		_mob.onChangeFllowPlayer += () =>
		{
			Player target = _mob.PlayerManager.GetPlayer(_mob.FllowTarget);

			if(target != null)
			{
				_followMove.OnStart(target.transform, _animator, _agent, _mob);
				_wanderMove.enabled = false;
			}
			else
			{
				_wanderMove.OnStart();
				_followMove.enabled = false;
			}
		};

		_followMove.OnTransCheck = () =>
		{
			// 条件式

			// 実行処理(別のイベント変数に定義するべき)
			//_followMove.enabled = false;
		};

		// 最初は徘徊移動モードにする
		_wanderMove.OnStart();
	}

    void Update()
    {
		// 初期化したかどうか
		if (_animator == null)
			return;

		// ナビメッシュ上の目標座標を現在の座標に合わせる
		_agent.nextPosition = transform.position;
	}
}