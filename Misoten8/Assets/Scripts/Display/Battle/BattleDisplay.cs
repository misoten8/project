/// <summary>
/// バトルディスプレイクラス
/// </summary>
public class BattleDisplay : DisplayBase
{
	/// <summary>
	/// ディスプレイイベントの定義インターフェイス
	/// </summary>
	public override IEvents DisplayEvents
	{
		get { return _events; }
	}

	/// <summary>
	/// UIオブジェクト呼び出しイベントクラス
	/// </summary>
	public BattleEvents _events = new BattleEvents();
}