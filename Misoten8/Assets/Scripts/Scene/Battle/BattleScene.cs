using System.Collections;
using System.Linq;
using UnityEngine;
using Misoten8Utility;

/// <summary>
/// バトルシーン管理クラス
/// </summary>
[RequireComponent(typeof(BattleSceneCache))]
public class BattleScene : SceneBase<BattleScene>
{
	[SerializeField]
	private Score _score;

	[SerializeField]
	private MobGenerator _mobGenerator;

	[SerializeField]
	private PlayerGenerator _playerGenerator;

	[SerializeField]
	private BattleTime _battleTime;

	[SerializeField]
	private BattleSceneNetwork _battleSceneNetwork;

	/// <summary>
	/// 外部シーンが利用できるデータキャッシュ
	/// </summary>
	public override ISceneCache SceneCache
	{
		get { return _sceneCache; }
	}

	[SerializeField]
	private BattleSceneCache _sceneCache;

	/// <summary>
	/// 一般プレイヤー分存在する
	/// マスタークライアントのみが使用する
	/// </summary>
	private bool[] _isBattleSceneLoaded = null;

	/// <summary>
	/// 派生クラスのインスタンスを取得
	/// </summary>
	protected override BattleScene GetOverrideInstance()
	{
		return this;
	}

	private void Start()
	{
		_isBattleSceneLoaded = new bool[PhotonNetwork.otherPlayers.Length];
		_isBattleSceneLoaded?.Foreach(e => e = false);
		
		if (PhotonNetwork.isMasterClient)
		{
			StartCoroutine(DelayInstance());
		}
		else
		{
			StartCoroutine(RepeatNotification());
		}
		AudioManager.PlayBGM("gronx");
	}

	/// <summary>
	/// シーン切り替え
	/// </summary>
	/// <remarks>
	/// 通信処理を挟みます
	/// </remarks>
	public override void Switch(SceneType nextScene)
	{
		if (duringTransScene)
			return;

		duringTransScene = true;

		DisplayManager.GetInstanceDisplayEvents<MoveEvents>()?.onBattleEnd?.Invoke();
		DisplayManager.GetInstanceDisplayEvents<DanceEvents>()?.onBattleEnd?.Invoke();

		// シーン遷移処理呼び出し
		_battleSceneNetwork.photonView.RPC("CallBackSwitchBattleScene", PhotonTargets.AllViaServer, (byte)nextScene);
	}

	/// <summary>
	/// 使用しないでください
	/// </summary>
	public void CallBackSwitch(SceneType nextScene)
	{
		ResultScore.scoreArray[(int)Define.PlayerType.First] = _score.GetScore(Define.PlayerType.First);
		ResultScore.scoreArray[(int)Define.PlayerType.Second] = _score.GetScore(Define.PlayerType.Second);
		ResultScore.scoreArray[(int)Define.PlayerType.Third] = _score.GetScore(Define.PlayerType.Third);
		ResultScore.scoreArray[(int)Define.PlayerType.Fourth] = _score.GetScore(Define.PlayerType.Fourth);

		AudioManager.PauseBGM();

		// ルームから退出する
		PhotonNetwork.LeaveRoom();
		StartCoroutine(SwitchAsync(nextScene));
	}

	/// <summary>
	/// 生成コルーチン
	/// </summary>
	/// <remarks>
	/// マスタークライアントのみが送信する
	/// </remarks>
	private IEnumerator DelayInstance()
	{
		// 全員が遷移を完了するまで、待機する
		while (IsWaiting())
			yield return null;

		Debug.Log("参加プレイヤー：" + PhotonNetwork.room.PlayerCount.ToString());

		// ディスプレイの読み込みが完了するまで待機する
		while (DisplayManager.IsSwitching)
			yield return null;

		// クライアント全員の生成クラスをアクティブにする
		_battleSceneNetwork.photonView.RPC("StartupGeneratorBattleScene", PhotonTargets.AllViaServer);
	}

	/// <summary>
	/// 一定間隔で通知する
	/// </summary>
	/// <remarks>
	/// 一般クライアントが送信する
	/// </remarks>
	private IEnumerator RepeatNotification()
	{
		do
		{
			_battleSceneNetwork.photonView.RPC("LoadedBattleSceneSendToMasterClient", PhotonTargets.MasterClient, (byte)PhotonNetwork.player.ID);
			yield return new WaitForSeconds(0.5f);
		} while (true);	
	}

	/// <summary>
	/// 生成クラスをアクティブにする
	/// </summary>
	public void StartupGenerator()
	{
		_mobGenerator.enabled = true;
		_playerGenerator.enabled = true;
		_battleTime.enabled = true;
		DisplayManager.GetInstanceDisplayEvents<MoveEvents>()?.onBattleStart?.Invoke();
	}

	/// <summary>
	/// フラグを受け取る
	/// </summary>
	public void LoadedBattleSceneSendToMasterClient(byte playerID)
	{
		int? index = PhotonNetwork.otherPlayers?.Index(e => e.ID == playerID);
		if (_isBattleSceneLoaded != null)
		{
			_isBattleSceneLoaded[index ?? 0] = true;
		}
	}

	private bool IsWaiting()
	{
		return !_isBattleSceneLoaded.All(e => e == true);
	}
}
