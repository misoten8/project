/// <summary>
/// ダンスディスプレイクラス
/// </summary>
public class DanceDisplay : DisplayBase
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
	public DanceEvents _events = new DanceEvents();
}