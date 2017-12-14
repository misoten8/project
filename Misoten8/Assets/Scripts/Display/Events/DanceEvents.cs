using System;

/// <summary>
/// ダンスディスプレイのUIオブジェクト呼び出しイベントクラス
/// </summary>
public class DanceEvents : EventsBase
{
	/// <summary>
	/// ダンス開始時実行イベント
	/// </summary>
	public Action onDanceStart;
	/// <summary>
	/// ダンス終了時実行イベント
	/// </summary>
	public Action onDanceEnd;
	/// <summary>
	/// ダンス成功時実行イベント
	/// </summary>
	public Action onDanceSuccess;
	/// <summary>
	/// ダンス失敗時実行イベント
	/// </summary>
	public Action onDanceFailled;
	/// <summary>
	/// リクエスト成功時実行イベント
	/// </summary>
	public Action onRequestSuccess;
	/// <summary>
	/// リクエスト失敗時実行イベント
	/// </summary>
	public Action onRequestFailled;
	/// <summary>
	/// プレイヤー乱入時実行イベント
	/// </summary>
	public Action onPenetrationPlayer;
}