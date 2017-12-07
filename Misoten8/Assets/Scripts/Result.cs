using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Result クラス
/// 製作者：実川
/// </summary>
public class Result : MonoBehaviour
{
	void Start ()
	{
		
	}
	
	void Update ()
	{
		
	}

	private void OnGUI()
	{
		GUI.Label(new Rect(new Vector2(0,0), new Vector2(300, 200)), "Result Scene");
	}

	public void TransScene()
	{
		SceneManager.LoadScene("Title");
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
