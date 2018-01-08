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
	private BattleSceneNetwork _network;

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
	/// 現在がバトル中かどうか
	/// </summary>
	/// <remarks>
	/// このパラメータでバトルシーンのオブジェクトの動作を制御する
	/// </remarks>
	public bool IsBattleTime
	{
		get { return _isBattleTime; }
	}

	private bool _isBattleTime = false;

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

		// クライアント全員にタイムアップ処理を通知する
		_network.photonView.RPC("FinishBattleScene", PhotonTargets.AllViaServer);
	}

	/// <summary>
	/// 生成クラスをアクティブにする
	/// </summary>
	public void StartupGenerator()
	{
		_mobGenerator.enabled = true;
		_playerGenerator.enabled = true;
		DisplayManager.GetInstanceDisplayEvents<MoveEvents>()?.onBattleReady?.Invoke();

		if (PhotonNetwork.isMasterClient)
			StartCoroutine(DelayBegin());
	}

	/// <summary>
	/// バトル開始する
	/// </summary>
	public void Begin()
	{
		_battleTime.enabled = true;
		DisplayManager.GetInstanceDisplayEvents<MoveEvents>()?.onBattleStart?.Invoke();
		AudioManager.PlayBGM("bgm_main_kari");
		_isBattleTime = true;
	}

	/// <summary>
	/// タイムアップ
	/// </summary>
	/// <remarks>
	/// ゲーム終了演出 + プレイヤー操作停止(ダンスはキャンセルする)
	/// </remarks>
	public void Finish()
	{
		_battleTime.enabled = false;
		_isBattleTime = false;
		DisplayManager.GetInstanceDisplayEvents<MoveEvents>()?.onBattleEnd?.Invoke();
		DisplayManager.GetInstanceDisplayEvents<DanceEvents>()?.onBattleEnd?.Invoke();

		if (PhotonNetwork.isMasterClient)
			StartCoroutine(DelayEnd());
	}

	/// <summary>
	/// リザルトにスコアを設定する
	/// </summary>
	public void End(int joinBattlePlayerNum, int[] scoreArray)
	{
		for(int i = 0; i < joinBattlePlayerNum; i++)
		{
			Define.ResultScoreMap[Define.ConvertToPlayerType(i + 1)] = scoreArray[i];
		}

		AudioManager.PauseBGM();

		// ルームから退出する
		PhotonNetwork.LeaveRoom();
		StartCoroutine(SwitchAsync(SceneType.Result));
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
		_network.photonView.RPC("StartupGeneratorBattleScene", PhotonTargets.AllViaServer);
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
			_network.photonView.RPC("LoadedBattleSceneSendToMasterClient", PhotonTargets.MasterClient, (byte)PhotonNetwork.player.ID);
			yield return new WaitForSeconds(0.5f);
		} while (true);
	}

	/// <summary>
	/// バトル開始の通知を遅延させる
	/// </summary>
	/// <remarks>
	/// マスタークライアントが呼び出す
	/// </remarks>
	private IEnumerator DelayBegin()
	{
		//TODO:モブ等の生成が終わってからバトルを開始する
		yield return new WaitForSeconds(3.0f);

		// クライアント全員にバトル開始を通知する
		_network.photonView.RPC("BeginGameBattleScene", PhotonTargets.AllViaServer);
	}

	/// <summary>
	/// バトル終了処理の通知を遅延させる
	/// </summary>
	/// <remarks>
	/// マスタークライアントが呼び出す
	/// </remarks>
	private IEnumerator DelayEnd()
	{
		yield return new WaitForSeconds(3.0f);

		int[] resultScore = Define.ResultScoreMap.Select(e => _score.GetScore(e.Key)).ToArray();
		int playerNum = Define.JoinBattlePlayerNum;
		PhotonTargets target = PhotonTargets.AllViaServer;

		// クライアント全員にバトル終了処理を通知する
		_network.photonView.RPC("EndBattleScene", target, playerNum, resultScore);
	}
}
