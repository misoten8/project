using TextFx;
using Misoten8Utility;

/// <summary>
/// MoveStartMessages クラス
/// </summary>
public class MoveStartMessages : UIBase 
{
	private TextFxTextMeshProUGUI _ready;
	private TextFxTextMeshProUGUI _start;

	public override void OnAwake(ISceneCache cache, IEvents displayEvents)
	{
		base.OnAwake(cache, displayEvents);
		var events = displayEvents as MoveEvents;
		if (events.IsEmpty())
			return;

		_ready = uiObjects[0] as TextFxTextMeshProUGUI;
		if (_ready.IsEmpty())
			return;

		_start = uiObjects[1] as TextFxTextMeshProUGUI;
		if (_start.IsEmpty())
			return;

		events.onBattleReady += () => _ready.AnimationManager.PlayAnimation();
		events.onBattleStart += () => _start.AnimationManager.PlayAnimation();
	}
}