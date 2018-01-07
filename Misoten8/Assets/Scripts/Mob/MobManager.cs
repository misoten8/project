using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Misoten8Utility;

/// <summary>
/// MobManager クラス
/// いずれ、ダンスの当たり判定等もここで行うようにする
/// 製作者：実川
/// </summary>
public class MobManager : Photon.MonoBehaviour
{
	/// <summary>
	/// スコア変化時に通知する
	/// </summary>
	public Action OnScoreChange
	{
		get { return _onScoreChange; }
	}

	private Action _onScoreChange;

	/// <summary>
	/// モブキャラのリスト
	/// </summary>
	public List<Mob> Mobs
	{
		get { return _mobs; }
	}

	private List<Mob> _mobs = new List<Mob>();

	[SerializeField]
	private Score _score;

	private bool _isScoreChange;

	private int[] _funCount = new int[Define.PLAYER_NUM_MAX + 1] {0, 0, 0, 0, 0};

	private List<int> _fanChangeStackID = new List<int>();
	private List<Define.PlayerType> _fanChangeStackType = new List<Define.PlayerType>();

	private List<int> _followChangeStackID = new List<int>();
	private List<Define.PlayerType> _followChangeStackType = new List<Define.PlayerType>();

	private PlayerManager _playerManager = null;

	private void Start()
	{
		Debug.Log("owner ID:" + photonView.ownerId.ToString());
		_onScoreChange = () => 
		{
			_isScoreChange = true;
		};

		_playerManager = GetComponent<PlayerManager>();

		if (_playerManager == null)
		{
			Debug.LogWarning("PlayerManagerが取得できませんでした");
		}
	}

	private void Update()
	{
		if (_mobs.Count == 0)
			return;

		// 一括で設定する
		if (!_isScoreChange)
		{
			if (_followChangeStackType.Count > 0)
			{
				photonView.RPC("SendFollowChanges", PhotonTargets.AllViaServer, _followChangeStackType.ToArray(), _followChangeStackID.ToArray());
				_followChangeStackType.Clear();
				_followChangeStackID.Clear();
			}
			return;
		}

		if (_fanChangeStackType.Count > 0)
		{
			photonView.RPC("SendFanChanges", PhotonTargets.AllViaServer, _fanChangeStackType.ToArray(), _fanChangeStackID.ToArray());
			_fanChangeStackType.Clear();
			_fanChangeStackID.Clear();
		}
	}

	public ModelManager.ModelType GetRandomModelType()
	{
		int number = UnityEngine.Random.Range(0, 2);
		if (number == 0)
			return ModelManager.ModelType.Mob1;
		if (number == 1)
			return ModelManager.ModelType.Mob2;
		return ModelManager.ModelType.Mob1;
	}

	public int GetFunCount(Define.PlayerType playerType)
	{	
		return _funCount[(int)playerType];
	}

	public void SetMob(Mob mob)
	{
		_mobs.Add(mob);
	}

	/// <summary>
	/// 指定座標の範囲内に存在するモブを渡す
	/// </summary>
	public Mob[] FindNearMobs(Vector3 worldPos, float range)
	{
		return _mobs.Where(e => Vector3.Distance(e.transform.position, worldPos) < range)?.ToArray();
	}

	/// <summary>
	/// ファン変更処理をスタックする
	/// </summary>
	public void FanChangeStack(Define.PlayerType fanTarget, int photonViewID)
	{
		_fanChangeStackType.Add(fanTarget);
		_fanChangeStackID.Add(photonViewID);
	}

	/// <summary>
	/// 追従変更処理をスタックする
	/// </summary>
	public void FollowChangeStack(Define.PlayerType followTarget, int photonViewID)
	{
		_followChangeStackType.Add(followTarget);
		_followChangeStackID.Add(photonViewID);
	}

	/// <summary>
	/// ファン変更履歴を一括送信
	/// </summary>
	[PunRPC]
	private void SendFanChanges(Define.PlayerType[] fanTargets, int[] photonViewIDs)
	{
		if (fanTargets.Count() != photonViewIDs.Count())
		{
			Debug.LogWarning("二つの配列のサイズが一致していません。\n正常にデータを受信できませんでした");
			return;
		}

		int max = fanTargets.Count();
		for (int i = 0; i < max; i++)
		{
			_mobs.First(e => e.photonView.viewID == photonViewIDs[i]).SetFunType(fanTargets[i]);
		}

		// ファン情報を再設定
		foreach (var player in _playerManager.Players)
		{
			int count = 0;

			// 配置番号の更新とファン人数の更新
			player.FanCount = _mobs
				// 指定されたプレイヤーのファンのみを選別
				.Where(e => e.FunType == player.Type)
				// 配置番号を昇順に並べ替える
				.OrderBy(e => e._followInex)
				// 配置番号の設定
				.Select(e =>
				{
					e._followInex = count;
					count++;
					return e;
				})
				.Count();

			// スコアを更新
			_score.SetScore(player.Type, player.FanCount);
		}

		_isScoreChange = false;
	}

	/// <summary>
	/// 追従変更履歴を一括送信
	/// </summary>
	[PunRPC]
	private void SendFollowChanges(Define.PlayerType[] followTargets, int[] photonViewIDs)
	{
		if (followTargets.Count() != photonViewIDs.Count())
		{
			Debug.LogWarning("二つの配列のサイズが一致していません。\n正常にデータを受信できませんでした");
			return;
		}

		int max = followTargets.Count();
		for (int i = 0; i < max; i++)
		{
			_mobs.First(e => e.photonView.viewID == photonViewIDs[i]).SetFollowType(followTargets[i]);
		}

		foreach(var followType in new Define.PlayerType[]
		{
			Define.PlayerType.First,
			Define.PlayerType.Second,
			Define.PlayerType.Third
		})
		{
			int count = 0;

			// 配置番号の更新
			int result = _mobs
				// 指定されたプレイヤーのファンのみを選別
				.Where(e => e.FllowTarget == followType)
				// 配置番号を昇順に並べ替える
				.OrderBy(e => e._followInex)
				// 配置番号の設定
				.Select(e =>
				{
					e._followInex = count;
					count++;
					return e;
				})
				.Count();
		}
	}

	/// <summary>
	/// 定義のみ
	/// </summary>
	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) { }
}