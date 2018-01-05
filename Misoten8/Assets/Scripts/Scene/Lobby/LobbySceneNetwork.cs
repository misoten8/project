using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

//TODO:カスタムプロパティが正常に値が入っているか確認する
/// <summary>
/// ロビーシーン管理クラスで利用する通信処理
/// </summary>
/// <remarks>
/// シーン管理クラスと通信処理は分離させます
/// </remarks>
public class LobbySceneNetwork : Photon.MonoBehaviour
{
	[SerializeField]
	private LobbyScene _lobbyScene;

	[SerializeField]
	private bool _offlineMode = false;

	private LobbyScene.State _currentState = LobbyScene.State.Start;

	/// <summary>
	/// シーン切り替え
	/// </summary>
	[PunRPC]
	public void CallBackSwitchLobbyScene(LobbyScene.SceneType nextScene)
	{
		_lobbyScene.CallBackSwitch(nextScene);
	}

	private void Start () 
	{
		StartCoroutine(BeginConnect());
	}

	/// <summary>
	/// 接続処理
	/// </summary>
	private IEnumerator BeginConnect()
	{
		// UIの切り替えが完了するまで待機する
		while(DisplayManager.IsSwitching)
		{
			yield return null;
		}

		if (_offlineMode)
		{
			PhotonNetwork.offlineMode = true;
			PhotonNetwork.player.CustomProperties = Define.defaultRoomPropaties;
			_currentState = LobbyScene.State.Offline;
		}
		else
		{
			PhotonNetwork.offlineMode = false;
		}

		if (!PhotonNetwork.connected)
		{
			_currentState = LobbyScene.State.ConnectingLobby;

			// マスターサーバーへ接続  
			PhotonNetwork.ConnectUsingSettings("v0.1");
		}
		else
		{
			if (PhotonNetwork.countOfRooms == 0)
			{
				// ルーム作成
				RoomOptions roomOptions = new RoomOptions
				{
					IsVisible = true,
					IsOpen = true,
					MaxPlayers = Define.PLAYER_NUM_MAX,
					CustomRoomProperties = Define.defaultRoomPropaties,
					CustomRoomPropertiesForLobby = new string[] { "CustomProperties" }
				};
				// ルームの作成
				PhotonNetwork.CreateRoom("Battle Room", roomOptions, new TypedLobby());
				_currentState = LobbyScene.State.CreatingRoom;
			}
			else
			{
				// ルーム入室
				PhotonNetwork.JoinRoom("Battle Room");
				_currentState = LobbyScene.State.JoingRoom;
			}
		}
		yield return null;
	}

	/// <summary>  
	/// マスターサーバーのロビー入室時  
	/// </summary>  
	private void OnJoinedLobby()
	{
		if (PhotonNetwork.inRoom)
		{
			_currentState = LobbyScene.State.WaitMember;
			Debug.Log("既にルームに入室しています");
			// カスタムプロパティの初期化
			PhotonNetwork.SetPlayerCustomProperties(Define.defaultRoomPropaties);
			return;
		}

		if(PhotonNetwork.countOfRooms == 0)
		{
			// ルーム作成
			RoomOptions roomOptions = new RoomOptions
			{
				IsVisible = true,
				IsOpen = true,
				MaxPlayers = Define.PLAYER_NUM_MAX,
				CustomRoomProperties = Define.defaultRoomPropaties,
				CustomRoomPropertiesForLobby = new string[] { "CustomProperties" }
			};
			// ルームの作成
			PhotonNetwork.CreateRoom("Battle Room", roomOptions, new TypedLobby());
			_currentState = LobbyScene.State.CreatingRoom;
		}
		else
		{
			// ルーム入室
			PhotonNetwork.JoinRoom("Battle Room");
			_currentState = LobbyScene.State.JoingRoom;
		}
	}

	void OnPhotonCreateRoomFailed(object[] codeAndMsg)
	{
		Debug.LogWarning("既にルームが作成されているので、ルームの作成が出来ませんでした。" +
			"\n作成されているルームに入室します");

		// ルーム入室
		PhotonNetwork.JoinRoom("Battle Room");
		_currentState = LobbyScene.State.JoingRoom;
	}

	/// <summary>  
	/// ルーム参加時  
	/// </summary>  
	private void OnJoinedRoom()
	{
		_currentState = LobbyScene.State.WaitMember;
		Debug.Log("ルームに入室しました あなたはplayer" + PhotonNetwork.player.ID.ToString() + "です");

		var events = DisplayManager.GetInstanceDisplayEvents<LobbyEvents>();

		// プレイヤー入室UIを表示
		if (_offlineMode)
		{
			if(PhotonNetwork.player.ID == 1)
				events?.onPlayer1Online?.Invoke();
		}
		else
		{
			foreach(var player in PhotonNetwork.playerList)
			{
				switch (player.ID)
				{
					case 1:
						events?.onPlayer1Online?.Invoke();
						break;
					case 2:
						events?.onPlayer2Online?.Invoke();
						break;
					case 3:
						events?.onPlayer3Online?.Invoke();
						break;
				}
			}
		}
		// カスタムプロパティの初期化
		PhotonNetwork.SetPlayerCustomProperties(Define.defaultRoomPropaties);
	}

	void OnPhotonJoinRoomFailed(object[] codeAndMsg)
	{
		Debug.LogWarning("エラーコード:" + ((int)codeAndMsg[0]).ToString() + "\nデバッグメッセージ:" + ((string)codeAndMsg[1]).ToString());
	}

	/// <summary>  
	/// 他ユーザーがルームに接続した時  
	/// </summary>   
	private void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
	{
		// 入室ログ表示  
		Debug.Log("player" + newPlayer.ID.ToString() + "が入室しました");

		// プレイヤー入室UIを表示
		var events = DisplayManager.GetInstanceDisplayEvents<LobbyEvents>();
		switch (newPlayer.ID)
		{
			case 1:
				events?.onPlayer1Online?.Invoke();
				break;
			case 2:
				events?.onPlayer2Online?.Invoke();
				break;
			case 3:
				events?.onPlayer3Online?.Invoke();
				break;
		}

		if (PhotonNetwork.room?.PlayerCount == Define.PLAYER_NUM_MAX)
		{
			_currentState = LobbyScene.State.Ready;
		}
	}

	/// <summary>  
	/// 他のユーザーのルーム退室時  
	/// </summary>  
	private void OnPhotonPlayerDisconnected(PhotonPlayer leavePlayer)
	{
		Debug.Log("player" + leavePlayer.ID.ToString() + "が退室しました");

		// プレイヤー入室UIを表示
		var events = DisplayManager.GetInstanceDisplayEvents<LobbyEvents>();
		switch (leavePlayer.ID)
		{
			case 1:
				events?.onPlayer1Offline?.Invoke();
				break;
			case 2:
				events?.onPlayer2Offline?.Invoke();
				break;
			case 3:
				events?.onPlayer3Offline?.Invoke();
				break;
		}

		if (PhotonNetwork.room?.PlayerCount != Define.PLAYER_NUM_MAX)
		{
			_currentState = LobbyScene.State.WaitMember;
		}
	}

	/// <summary>
	/// 定義のみ
	/// </summary>
	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) { }

	private void OnGUI()
	{
		var boxSize = new Vector2(400.0f, 120.0f);
		var rect = new Rect(new Vector2(Screen.width * 0.5f - boxSize.x * 0.5f, Screen.height * 0.8f - boxSize.y * 0.5f), boxSize);
		string message = "";
		if (!_offlineMode)
		{
			message = LobbyScene.MessageMap[_currentState] + "\n" +
				"現在のルーム接続人数：" + PhotonNetwork.room?.PlayerCount.ToString() + "人\n" +
				"このルームの最大接続人数：" + PhotonNetwork.room?.MaxPlayers.ToString() + "人\n" +
				"あなたは" + (PhotonNetwork.isMasterClient ? "マスタークライアント" : "一般クライアント") + "です";
		}
		else
		{
			message = LobbyScene.MessageMap[_currentState] +
				"\nいつでもゲームを開始できます";
		}
		// UI表示
		GUI.Box(rect, message);
	}
}