using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Battle クラス
/// 製作者：実川
/// </summary>
public class Battle : Photon.MonoBehaviour
{
	[SerializeField]
	private Score _score;

	[SerializeField]
	private MobGenerator _mobGenerator;

	[SerializeField]
	private PlayerGenerator _playerGenerator;

	[SerializeField]
	private BattleTime _battleTime;

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
	/// シーン遷移する
	/// </summary>
	[PunRPC]
	public void TransScene()
	{
		ResultScore.scoreArray[(int)Define.PlayerType.First] = _score.GetScore(Define.PlayerType.First);
		ResultScore.scoreArray[(int)Define.PlayerType.Second] = _score.GetScore(Define.PlayerType.Second);
		ResultScore.scoreArray[(int)Define.PlayerType.Third] = _score.GetScore(Define.PlayerType.Third);
		ResultScore.scoreArray[(int)Define.PlayerType.Fourth] = _score.GetScore(Define.PlayerType.Fourth);

		AudioManager.PauseBGM();

		// ルームから退出する
		PhotonNetwork.LeaveRoom();
		SceneManager.LoadScene("Result");
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
		photonView.RPC("StartupGenerator", PhotonTargets.AllViaServer);
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

	/// <summary>
	/// 生成クラスをアクティブにする
	/// </summary>
	[PunRPC]
	private void StartupGenerator()
	{
		_mobGenerator.enabled = true;
		_playerGenerator.enabled = true;
		_battleTime.enabled = true;
	}

	/// <summary>
	/// 定義のみ
	/// </summary>
	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) { }

	public void OnPhotonPlayerPropertiesChanged(object[] i_playerAndUpdatedProps)
	{
		var player = i_playerAndUpdatedProps[0] as PhotonPlayer;
		var properties = i_playerAndUpdatedProps[1] as ExitGames.Client.Photon.Hashtable;

		PhotonNetwork.playerList
			.Where(e => e.ID == player.ID)
			.Select(e =>
			{

				e.SetCustomProperties(properties);
				return default(IEnumerable);
			});
	}

	/// <summary>
	/// アプリケーション終了時実行イベント
	/// </summary>
	void OnApplicationQuit()
	{
		Debug.Log("ルームから退出しました");
		// ルーム退室  
		PhotonNetwork.LeaveRoom();
		// ネットワーク切断
		PhotonNetwork.Disconnect();
	}
}
