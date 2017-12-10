/// <summary>
/// タイトルディスプレイクラス
/// </summary>
public class TitleDisplay : DisplayBase 
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
	public TitleEvents _events = new TitleEvents();
}