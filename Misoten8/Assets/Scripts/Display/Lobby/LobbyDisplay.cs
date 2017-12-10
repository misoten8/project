/// <summary>
/// ロビーディスプレイクラス
/// </summary>
public class LobbyDisplay : DisplayBase
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
	public LobbyEvents _events = new LobbyEvents();
}