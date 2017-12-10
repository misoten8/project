using UnityEngine;
using UnityEngine.SceneManagement;
using WiimoteApi;

/// <summary>
/// LobbySceneManager クラス
/// 製作者：実川
/// </summary>
public class LobbyScene : Photon.MonoBehaviour 
{
	void Update () 
	{
		if (Input.GetKeyDown("return") || WiimoteManager.GetButton(0, ButtonData.WMBUTTON_TWO))
		{
			if (PhotonNetwork.inRoom)
			{
				photonView.RPC("LoadBattleScene", PhotonTargets.AllViaServer);
				return;
			}
			Debug.LogWarning("まだゲーム開始の準備ができていません");
		}
	}

	[PunRPC]
	public void LoadBattleScene()
	{
		SceneManager.LoadScene("Battle");
	}

	/// <summary>
	/// 定義のみ
	/// </summary>
	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) { }
}