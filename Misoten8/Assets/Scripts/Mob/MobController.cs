using UnityEngine;

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

	/// <summary>
	/// 現在の移動処理
	/// </summary>
	//private IMove _currentMove;

	void Start()
	{
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
					_followMove.OnStart(_mob.PlayerManager.GetPlayer(_mob.FllowTarget)?.transform);
				}
				else
				{
					_followMove.OnStart(_mob.funPlayer.transform);
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
				_followMove.OnStart(target.transform);
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

}