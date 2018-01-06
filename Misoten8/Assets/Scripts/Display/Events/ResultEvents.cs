using System;

/// <summary>
/// リザルトディスプレイのUIオブジェクト呼び出しイベントクラス
/// </summary>
public class ResultEvents : EventsBase
{
	/// <summary>
	/// 勝利者パネルアニメーション再生時実行イベント
	/// </summary>
	public Action onPlayWinnerPanel;
	/// <summary>
	/// スコアパネルオープン時実行イベント
	/// </summary>
	public Action onOpneScorePanel;
	/// <summary>
	/// タイトル遷移可能時実行イベント
	/// </summary>
	public Action onTransTitleReady;
}