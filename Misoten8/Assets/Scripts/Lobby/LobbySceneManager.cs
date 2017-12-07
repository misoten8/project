using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// LobbySceneManager クラス
/// 製作者：実川
/// </summary>
public class LobbySceneManager : Photon.MonoBehaviour 
{
	void Update () 
	{
		if(Input.GetKeyDown("return"))
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