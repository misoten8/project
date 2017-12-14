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
	/// 使用するモデルの紐付けマップ
	/// </summary>
	public static readonly Dictionary<Define.FanLevel, ModelManager.ModelType> MODEL_MAP = new Dictionary<Define.FanLevel, ModelManager.ModelType>
	{
		{ Define.FanLevel.Easy, ModelManager.ModelType.Mob1 },
		{ Define.FanLevel.Normal, ModelManager.ModelType.Mob1 },
		{ Define.FanLevel.Hard, ModelManager.ModelType.Mob1 }
	};

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

	private int[] _funCount = new int[Define.PLAYER_NUM_MAX + 1] { 0, 0, 0, 0, 0 };
	// ダンスによって変化した人数
	private int[] _funCountDiff = new int[Define.PLAYER_NUM_MAX + 1] { 0, 0, 0, 0, 0 };

	private List<int> _fanChangeStackID = new List<int>();
	private List<Define.PlayerType> _fanChangeStackType = new List<Define.PlayerType>();

	private List<int> _followChangeStackID = new List<int>();
	private List<Define.PlayerType> _followChangeStackType = new List<Define.PlayerType>();

	private void Start()
	{
		Debug.Log("owner ID:" + photonView.ownerId.ToString());
		_onScoreChange = () =>
		{
			_isScoreChange = true;
		};
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
				Debug.Log(_followChangeStackType.Count.ToString() + "人のモブの追従変更情報が送信されるドン！");
				photonView.RPC("SendFollowChanges", PhotonTargets.AllViaServer, _followChangeStackType.ToArray(), _followChangeStackID.ToArray());
				_followChangeStackType.Clear();
				_followChangeStackID.Clear();
			}
			return;
		}

		if (_fanChangeStackType.Count > 0)
		{
			Debug.Log(_fanChangeStackType.Count.ToString() + "人のモブのファン変更情報が送信されるドン！");
			photonView.RPC("SendFanChanges", PhotonTargets.AllViaServer, _fanChangeStackType.ToArray(), _fanChangeStackID.ToArray());
			_fanChangeStackType.Clear();
			_fanChangeStackID.Clear();
		}
	}

	/// <summary>
	/// プレイヤーごとのファンの人数を取得
	/// </summary>
	public int GetFunCount(Define.PlayerType playerType)
	{
		return _funCount[(int)playerType];
	}

	/// <summary>
	/// 直前ダンスによって変化したファンの人数
	/// </summary>
	public int GetFunCountDiff(Define.PlayerType playerType)
	{
		return _funCountDiff[(int)playerType];
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
			return;

		int max = fanTargets.Count();
		for (int i = 0; i < max; i++)
		{
			_mobs.First(e => e.photonView.viewID == photonViewIDs[i]).SetFunType(fanTargets[i]);
		}

		// 前回の人数を取得
		_funCountDiff = _funCount;

		// ファンの人数の更新
		int targetType = 0;
		_funCount.Foreach(e =>
		{
			e = _mobs.Where(_e => _e.FunType == (Define.PlayerType)targetType).Count();

			// 差分を算出
			_funCountDiff[targetType] = e - _funCountDiff[targetType];

			// スコアに反映
			if ((Define.PlayerType)targetType != Define.PlayerType.None)
				_score.SetScore((Define.PlayerType)targetType, e);

			targetType ++;
		});

		_isScoreChange = false;
	}

	/// <summary>
	/// 追従変更履歴を一括送信
	/// </summary>
	[PunRPC]
	private void SendFollowChanges(Define.PlayerType[] followTargets, int[] photonViewIDs)
	{
		if (followTargets.Count() != photonViewIDs.Count())
			return;

		int max = followTargets.Count();
		for (int i = 0; i < max; i++)
		{
			_mobs.First(e => e.photonView.viewID == photonViewIDs[i]).SetFollowType(followTargets[i]);
		}
	}

	/// <summary>
	/// 定義のみ
	/// </summary>
	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) { }
}