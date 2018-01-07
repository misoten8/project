using UnityEngine;
using System.Collections;
using Misoten8Utility;

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

	private LobbyNetworkParameters _networkParameters;

	/// <summary>
	/// バトル開始の準備ができたかどうか
	/// </summary>
	public bool IsReady()
	{
		//TODO:バトル開始の条件式は後々変更する
		if (PhotonNetwork.inRoom)
		{
			return true;
		}
		// 以下の条件式が正しい(調整するかも)
		//if (_networkParameters.OfflineMode)
		//{
		//	return true;
		//}
		//if (PhotonNetwork.playerList.Length == Define.PLAYER_NUM_MAX)
		//{
		//	return true;
		//}
		Debug.LogWarning("まだゲーム開始の準備ができていません");
		return false;
	}

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
		_networkParameters = _lobbyScene.LobbyNetworkCustomizer;
		if (_networkParameters.IsEmpty())
			return;

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

		var events = DisplayManager.GetInstanceDisplayEvents<LobbyEvents>();

		if (_lobbyScene.LobbyNetworkCustomizer.OfflineMode)
		{
			PhotonNetwork.offlineMode = true;
			_networkParameters.CurrentState = LobbyNetworkParameters.ConnectState.Offline;
			events?.onBeginConnect?.Invoke();
		}
		else
		{
			PhotonNetwork.offlineMode = false;
		}

		if (!PhotonNetwork.connected)
		{
			_networkParameters.CurrentState = LobbyNetworkParameters.ConnectState.ConnectingLobby;

			// マスターサーバーへ接続  
			PhotonNetwork.ConnectUsingSettings("v0.1");

			events?.onBeginConnect?.Invoke();
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
					MaxPlayers = Define.PLAYER_NUM_MAX
				};
				// ルームの作成
				PhotonNetwork.CreateRoom("Battle Room", roomOptions, new TypedLobby());
				if (!_lobbyScene.LobbyNetworkCustomizer.OfflineMode)
				{
					_networkParameters.CurrentState = LobbyNetworkParameters.ConnectState.CreatingRoom;
					events?.onBeginConnect?.Invoke();
				}
			}
			else
			{
				// ルーム入室
				PhotonNetwork.JoinRoom("Battle Room");
				_networkParameters.CurrentState = LobbyNetworkParameters.ConnectState.JoingRoom;
				events?.onBeginConnect?.Invoke();
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
			_networkParameters.CurrentState = LobbyNetworkParameters.ConnectState.WaitMember;
			Debug.Log("既にルームに入室しています");
			return;
		}

		// ルームが既に作成されているかどうか
		if(PhotonNetwork.countOfRooms == 0)
		{
			// ルーム作成
			RoomOptions roomOptions = new RoomOptions
			{
				IsVisible = true,
				IsOpen = true,
				MaxPlayers = Define.PLAYER_NUM_MAX,
			};
			// ルームの作成
			PhotonNetwork.CreateRoom("Battle Room", roomOptions, new TypedLobby());
			_networkParameters.CurrentState = LobbyNetworkParameters.ConnectState.CreatingRoom;
		}
		else
		{
			// ルーム入室
			PhotonNetwork.JoinRoom("Battle Room");
			_networkParameters.CurrentState = LobbyNetworkParameters.ConnectState.JoingRoom;
		}
	}

	/// <summary>
	/// ルーム作成失敗時
	/// </summary>
	void OnPhotonCreateRoomFailed(object[] codeAndMsg)
	{
		Debug.LogWarning("既にルームが作成されているので、ルームの作成が出来ませんでした。" +
			"\n作成されているルームに入室します");

		// ルーム入室
		PhotonNetwork.JoinRoom("Battle Room");
		_networkParameters.CurrentState = LobbyNetworkParameters.ConnectState.JoingRoom;
	}

	/// <summary>  
	/// ルーム参加時  
	/// </summary>  
	private void OnJoinedRoom()
	{
		if(!_networkParameters.OfflineMode)
			_networkParameters.CurrentState = LobbyNetworkParameters.ConnectState.WaitMember;
		Debug.Log("ルームに入室しました あなたはplayer" + PhotonNetwork.player.ID.ToString() + "で\n" + 
			"あなたは" + (PhotonNetwork.isMasterClient ? "マスタークライアント" : "一般クライアント") + "です");

		var events = DisplayManager.GetInstanceDisplayEvents<LobbyEvents>();

		// プレイヤー入室UIを表示
		if (_lobbyScene.LobbyNetworkCustomizer.OfflineMode)
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
			_networkParameters.CurrentState = LobbyNetworkParameters.ConnectState.Ready;
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
			_networkParameters.CurrentState = LobbyNetworkParameters.ConnectState.WaitMember;
		}
	}

	/// <summary>
	/// 定義のみ
	/// </summary>
	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) { }
}