using System.Collections;

/// <summary>
/// ディスプレイインターフェイス
/// </summary>
public interface IDisplay
{
	/// <summary>
	/// ディスプレイ切り替えアニメーションが再生中かどうか
	/// </summary>
	bool IsSwitchAnimPlaying { get; }

	/// <summary>
	/// ディスプレイイベントの定義インターフェイス
	/// </summary>
	IEvents DisplayEvents { get; }

	/// <summary>
	/// ディスプレイ生成時に呼ばれるイベント
	/// </summary>
	void OnAwake(ISceneCache cache);

	/// <summary>
	/// ディスプレイ遷移開始時に呼ばれるイベント
	/// </summary>
	IEnumerator OnSwitchFadeIn();

	/// <summary>
	/// ディスプレイ遷移開始時に呼ばれるイベント
	/// </summary>
	IEnumerator OnSwitchFadeOut();

	/// <summary>
	/// ディスプレイ消去時に呼ばれるイベント
	/// </summary>
	void OnDelete();
}
