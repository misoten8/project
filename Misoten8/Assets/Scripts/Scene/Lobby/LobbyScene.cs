using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ロビーシーン管理クラス
/// </summary>
[RequireComponent(typeof(LobbySceneCache))]
public class LobbyScene : SceneBase<LobbyScene>
{
	[SerializeField]
	private LobbySceneNetwork _lobbySceneNetwork;

	/// <summary>
	/// 外部シーンが利用できるデータキャッシュ
	/// </summary>
	public override ISceneCache SceneCache
	{
		get { return _sceneCache; }
	}

	/// <summary>
	/// メッセージの種類とメッセージの紐付けマップ
	/// </summary>
	public static readonly Dictionary<State, string> MessageMap = new Dictionary<State, string>
	{
		{ State.Start, "" },
		{ State.ConnectingLobby, "ネットワークに接続中です" },
		{ State.CreatingRoom, "ルームを作成しています" },
		{ State.JoingRoom, "ルームに入室しています" },
		{ State.WaitMember, "メンバーが揃うまで待機します(デバッグ時は開始できます)" },
		{ State.Ready, "メンバーが揃いました、ボタンを押してゲームを開始してください" },
		{ State.Offline, "オフラインモード" },
	};

	/// <summary>
	/// メッセージの種類
	/// </summary>
	public enum State
	{
		Start = 0,
		ConnectingLobby,
		CreatingRoom,
		JoingRoom,
		WaitMember,
		Ready,
		Offline,
		Max
	}

	[SerializeField]
	private LobbySceneCache _sceneCache;

	/// <summary>
	/// 派生クラスのインスタンスを取得
	/// </summary>
	protected override LobbyScene GetOverrideInstance()
	{
		return this;
	}
    private void Start()
    {
        AudioManager.PlayBGM("タイトル");
    }
    void Update () 
	{
		if (shakeparameter.IsOverWithValue(Define.SCENE_TRANCE_VALUE))
		{
			if (PhotonNetwork.inRoom)
			{
				Switch(SceneType.Battle);
				return;
			}
			Debug.LogWarning("まだゲーム開始の準備ができていません");
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
        AudioManager.PlaySE("決定１");

        // シーン遷移処理呼び出し
        _lobbySceneNetwork.photonView.RPC("CallBackSwitchLobbyScene", PhotonTargets.AllViaServer, (byte)nextScene);
	}

	/// <summary>
	/// 使用しないでください
	/// </summary>
	public void CallBackSwitch(SceneType nextScene)
	{
		StartCoroutine(SwitchAsync(nextScene));
	}
}