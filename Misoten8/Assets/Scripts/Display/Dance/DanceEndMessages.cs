using TextFx;
using Misoten8Utility;

/// <summary>
/// DanceEndMessages クラス
/// </summary>
public class DanceEndMessages : UIBase
{
	private TextFxTextMeshProUGUI _message;

	public override void OnAwake(ISceneCache cache, IEvents displayEvents)
	{
		base.OnAwake(cache, displayEvents);
		var events = displayEvents as DanceEvents;
		if (events.IsEmpty())
			return;

		_message = uiObjects[0] as TextFxTextMeshProUGUI;
		if (_message.IsEmpty())
			return;

		events.onBattleEnd += () => _message.AnimationManager.PlayAnimation();
	}
}