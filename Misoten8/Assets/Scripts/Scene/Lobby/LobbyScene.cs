using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ロビーシーン管理クラス
/// </summary>
[RequireComponent(typeof(LobbySceneCache))]
public class LobbyScene : SceneBase<LobbyScene>
{
	public LobbyNetworkParameters LobbyNetworkCustomizer
	{
		get { return _lobbyNetworkCustomizer; }
	}

	/// <summary>
	/// 外部シーンが利用できるデータキャッシュ
	/// </summary>
	public override ISceneCache SceneCache
	{
		get { return _sceneCache; }
	}

	[SerializeField]
	private LobbySceneNetwork _lobbySceneNetwork;

	[SerializeField]
	private LobbyNetworkParameters _lobbyNetworkCustomizer;

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
			if (_lobbySceneNetwork.IsReady())
			{
				Switch(SceneType.Battle);
				return;
			}
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
		AudioManager.PauseBGM();

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