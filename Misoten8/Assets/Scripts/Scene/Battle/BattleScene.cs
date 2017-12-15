using System.Collections;
using UnityEngine;

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
	/// 派生クラスのインスタンスを取得
	/// </summary>
	protected override BattleScene GetOverrideInstance()
	{
		return this;
	}

	private void Start()
	{
		StartCoroutine(DelayInstance());
		AudioManager.PlayBGM("DJ Striden - Lights [Dream Trance]");
	}

	private void OnGUI()
	{
		GUI.Label(new Rect(new Vector2(0, 0), new Vector2(300, 200)), "Battle Scene");
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

	//TODO:バトルシーン開始時にカメラで街を見渡す処理を挟む
	//参加プレイヤー全員のシーン遷移が完了するまで、プレイヤーオブジェクトが生成されないため。
	/// <summary>
	/// 生成コルーチン
	/// </summary>
	private IEnumerator DelayInstance()
	{
		PhotonNetwork.player.CustomProperties[Define.RoomPropaties.IsBattleSceneLoaded] = true;

		if (!PhotonNetwork.isMasterClient)
			yield break;

		// 全員が遷移を完了するまで、待機する
		while (IsWaiting())
			yield return null;

		// クライアント全員の生成クラスをアクティブにする
		_battleSceneNetwork.photonView.RPC("StartupGeneratorBattleScene", PhotonTargets.AllViaServer);
	}

	/// <summary>
	/// 生成クラスをアクティブにする
	/// </summary>
	public void StartupGenerator()
	{
		_mobGenerator.enabled = true;
		_playerGenerator.enabled = true;
		_battleTime.enabled = true;
	}

	private bool IsWaiting()
	{
		foreach (var player in PhotonNetwork.playerList)
		{
			bool? isBattleSceneLoaded = player.CustomProperties[Define.RoomPropaties.IsBattleSceneLoaded] as bool?;
			if (isBattleSceneLoaded == null)
				continue;

			if (isBattleSceneLoaded == true)
				continue;

			// 待機
			return true;
		}
		// 待機終了
		return false;
		// 全員がシーン遷移が完了したかどうかのチェックする
	}
}
