using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using WiimoteApi;

/// <summary>
/// Title クラス
/// 製作者：実川
/// </summary>
public class Title : MonoBehaviour
{
	void Update()
	{
		if (Input.GetKeyDown("return") || WiimoteManager.GetButton(0, ButtonData.WMBUTTON_TWO))
		{
			TransScene();
		}
	}

	private void OnGUI()
	{
		GUI.Label(new Rect(new Vector2(0, 0), new Vector2(300, 200)), "Title Scene");
	}

	public void TransScene()
	{
		SceneManager.LoadScene("Lobby");
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
