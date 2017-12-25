﻿using UnityEngine;
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

	/// <summary>
	/// メッセージの種類とメッセージの紐付けマップ
	/// </summary>
	private static readonly Dictionary<State, string> _messageMap = new Dictionary<State, string>
	{
		{ State.Start, "" },
		{ State.ConnectingLobby, "ネットワークに接続中です" },
		{ State.CreatingRoom, "ルームを作成しています" },
		{ State.JoingRoom, "ルームに入室しています" },
		{ State.WaitMember, "メンバーが揃うまで待機します(デバッグ時は開始できます)" },
		{ State.Ready, "メンバーが揃いました、ボタンを押してゲームを開始してください" }
	};

	/// <summary>
	/// メッセージの種類
	/// </summary>
	private enum State
	{
		Start = 0,
		ConnectingLobby,
		CreatingRoom,
		JoingRoom,
		WaitMember,
		Ready,
		Max
	}

	private State _currentState = State.Start;

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
		if (_offlineMode)
		{
			PhotonNetwork.offlineMode = true;
			PhotonNetwork.player.CustomProperties = Define.defaultRoomPropaties;
			_currentState = State.WaitMember;
		}
		else
		{
			PhotonNetwork.offlineMode = false;
		}

		if (!PhotonNetwork.connected)
		{
			_currentState = State.ConnectingLobby;

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
				_currentState = State.CreatingRoom;
			}
			else
			{
				// ルーム入室
				PhotonNetwork.JoinRoom("Battle Room");
				_currentState = State.JoingRoom;
			}
		}
	}

	/// <summary>  
	/// マスターサーバーのロビー入室時  
	/// </summary>  
	private void OnJoinedLobby()
	{
		if (PhotonNetwork.inRoom)
		{
			_currentState = State.WaitMember;
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
			_currentState = State.CreatingRoom;
		}
		else
		{
			// ルーム入室
			PhotonNetwork.JoinRoom("Battle Room");
			_currentState = State.JoingRoom;
		}
	}

	void OnPhotonCreateRoomFailed(object[] codeAndMsg)
	{
		Debug.LogWarning("既にルームが作成されているので、ルームの作成が出来ませんでした。" +
			"\n作成されているルームに入室します");

		// ルーム入室
		PhotonNetwork.JoinRoom("Battle Room");
		_currentState = State.JoingRoom;
	}

	/// <summary>  
	/// ルーム参加時  
	/// </summary>  
	private void OnJoinedRoom()
	{
		_currentState = State.WaitMember;
		Debug.Log("ルームに入室しました あなたはplayer" + PhotonNetwork.player.ID.ToString() + "です");
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

		if(PhotonNetwork.room?.PlayerCount == Define.PLAYER_NUM_MAX)
		{
			_currentState = State.Ready;
		}
	}

	/// <summary>  
	/// 他のユーザーのルーム退室時  
	/// </summary>  
	private void OnPhotonPlayerDisconnected(PhotonPlayer leavePlayer)
	{
		Debug.Log("player" + leavePlayer.ID.ToString() + "が退室しました");

		if (PhotonNetwork.room?.PlayerCount != Define.PLAYER_NUM_MAX)
		{
			_currentState = State.WaitMember;
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
			message = _messageMap[_currentState] + "\n" +
				"現在のルーム接続人数：" + PhotonNetwork.room?.PlayerCount.ToString() + "人\n" +
				"このルームの最大接続人数：" + PhotonNetwork.room?.MaxPlayers.ToString() + "人\n" +
				"あなたは" + (PhotonNetwork.isMasterClient ? "マスタークライアント" : "一般クライアント") + "です";
		}
		else
		{
			message = "オフラインモードです\n" +
				"いつでもゲームを開始できます";
		}
		// UI表示
		//GUI.Box(rect, message);
	}

	public void OnPhotonPlayerPropertiesChanged(object[] i_playerAndUpdatedProps)
	{
		var player = i_playerAndUpdatedProps[0] as PhotonPlayer;
		var properties = i_playerAndUpdatedProps[1] as ExitGames.Client.Photon.Hashtable;

		Debug.Log("誰かのプロパティが変化しました！");
		PhotonNetwork.playerList
			.Where(e => e.ID == player.ID)
			.Select(e =>
			{
				Debug.Log("プレイヤー" + player.ID.ToString() + "のプロパティが変化しました");
				e.SetCustomProperties(properties);
				return default(IEnumerable);
			});
	}
}