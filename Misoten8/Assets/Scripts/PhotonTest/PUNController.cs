using UnityEngine;
using UnityEngine.UI;

// TODO:ルームを一つにして、一番早いユーザーをオーナーにする
public class PUNController : Photon.MonoBehaviour
{
	// 現在のステート表示用テキスト  
	[SerializeField] private Text _currentStateText;

	// ルームの親オブジェクト  
	[SerializeField] private GameObject _room;
	// 退室ボタン  
	[SerializeField] private GameObject _left;


	void Start()
	{
		//マスターサーバーへ接続  
		PhotonNetwork.offlineMode = false;
		PhotonNetwork.ConnectUsingSettings("v0.1");
		// 退室ボタン非表示  
		_left.SetActive(false);
		// 退室処理登録  
		_left.GetComponent<Button>().onClick.AddListener(LeftRoom);
	}

	/// <summary>  
	/// マスターサーバーのロビー入室時  
	/// </summary>  
	void OnJoinedLobby()
	{

		Debug.Log("joined lobby");
		_currentStateText.text = "Lobby";

	}

	/// <summary>  
	/// ルーム参加時  
	/// </summary>  
	void OnJoinedRoom()
	{
		Debug.Log("joined room");
		_currentStateText.text = "" + PhotonNetwork.room.Name;
		// ルーム一覧非表示  
		_room.SetActive(false);
		// 退室ボタン表示  
		_left.SetActive(true);
	}

	/// <summary>  
	/// ルーム退室時  
	/// </summary>  
	void OnLeftRoom()
	{
		Debug.Log("left room");
		_currentStateText.text = "Lobby:" + PhotonNetwork.lobby.Name;
		// ルーム一覧表示  
		_room.SetActive(true);
		// 退室ボタン非表示  
		_left.SetActive(false);

	}

	/// <summary>  
	/// 他のユーザーのルーム退室時  
	/// </summary>  
	/// <param name="otherPlayer">Other player.</param>  
	void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
	{
		Debug.Log("ID:" + otherPlayer.ID + "left room");
		// 退室ログ表示  
		GetComponent<InRoomChat>().messages.Add("player" + otherPlayer.ID + "さんが退室しました");
	}

	/// <summary>  
	/// 他ユーザーがルームに接続した時  
	/// </summary>  
	/// <param name="newPlayer">New player.</param>  
	void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
	{
		// 入室ログ表示  
		Debug.Log("Joined otherPlayer ");
		GetComponent<InRoomChat>().messages.Add("player" + newPlayer.ID + "さんが入室しました");

	}


	/// <summary>  
	/// ルーム退室  
	/// </summary>  
	public void LeftRoom()
	{
		// ルーム退室  
		PhotonNetwork.LeaveRoom();
		// ログ削除  
		GetComponent<InRoomChat>().messages.Clear();
	}
}