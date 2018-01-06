using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 通信情報管理クラス
/// </summary>
public class LobbyNetworkParameters : MonoBehaviour 
{
	/// <summary>
	/// メッセージの種類とメッセージの紐付けマップ
	/// </summary>
	public static readonly Dictionary<ConnectState, string> MessageMap = new Dictionary<ConnectState, string>
	{
		{ ConnectState.Start, "ロビー" },
		{ ConnectState.ConnectingLobby, "セツゾクチュウ" },
		{ ConnectState.CreatingRoom, "ルームをつくっています" },
		{ ConnectState.JoingRoom, "ルームにはいっています" },
		{ ConnectState.WaitMember, "ほかのひとをまっています" },
		{ ConnectState.Ready, "じゅんびができました!リモコンをふってスタート!" },
		{ ConnectState.Offline, "オフラインモード!リモコンをふってスタート!" },
	};

	/// <summary>
	/// 接続メッセージの種類
	/// </summary>
	public enum ConnectState
	{
		Start = 0,
		ConnectingLobby,
		CreatingRoom,
		JoingRoom,
		WaitMember,
		Ready,
		Offline,
		Max
	}

	/// <summary>
	/// オフラインモードかどうか
	/// </summary>
	public bool OfflineMode
	{
		get { return _offlineMode; }
	}

	/// <summary>
	/// 現在の接続状況
	/// </summary>
	public ConnectState CurrentState
	{
		get { return _currentState; }
		set { _currentState = value; }
	}

	[SerializeField]
	private bool _offlineMode = false;

	[SerializeField]
	private ConnectState _currentState = ConnectState.Start;
}