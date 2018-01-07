using System;

/// <summary>
/// ロビーディスプレイのUIオブジェクト呼び出しイベントクラス
/// </summary>
public class LobbyEvents : EventsBase
{
	/// <summary>
	/// プレイヤー1オンライン時実行イベント
	/// </summary>
	public Action onPlayer1Online;
	/// <summary>
	/// プレイヤー2オンライン時実行イベント
	/// </summary>
	public Action onPlayer2Online;
	/// <summary>
	/// プレイヤー3オンライン時実行イベント
	/// </summary>
	public Action onPlayer3Online;
	/// <summary>
	/// 実況ナビオンライン時実行イベント
	/// </summary>
	public Action onNavOnline;
	/// <summary>
	/// プレイヤー1オフライン時実行イベント
	/// </summary>
	public Action onPlayer1Offline;
	/// <summary>
	/// プレイヤー2オフライン時実行イベント
	/// </summary>
	public Action onPlayer2Offline;
	/// <summary>
	/// プレイヤー3オフライン時実行イベント
	/// </summary>
	public Action onPlayer3Offline;
	/// <summary>
	/// 実況ナビオフライン時実行イベント
	/// </summary>
	public Action onNavOffline;
	/// <summary>
	/// ネットワーク接続開始時実行イベント
	/// </summary>
	/// <remarks>
	/// オフラインモード時にも呼ばれる
	/// </remarks>
	public Action onBeginConnect;
}