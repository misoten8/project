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

	[SerializeField]
	private LobbySceneCache _sceneCache;

	/// <summary>
	/// 派生クラスのインスタンスを取得
	/// </summary>
	protected override LobbyScene GetOverrideInstance()
	{
		return this;
	}

	void Start()
	{
		AudioManager.Play(BGMType.Lobby);
	}

	void Update () 
	{
		if (shakeparameter.IsOverWithValue(2))
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