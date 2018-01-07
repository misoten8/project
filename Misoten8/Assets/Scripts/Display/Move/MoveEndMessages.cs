using Misoten8Utility;
using TextFx;

/// <summary>
/// MoveEndMessages クラス
/// </summary>
public class MoveEndMessages : UIBase
{
	private TextFxTextMeshProUGUI _message;

	public override void OnAwake(ISceneCache cache, IEvents displayEvents)
	{
		base.OnAwake(cache, displayEvents);
		var events = displayEvents as MoveEvents;
		if (events.IsEmpty())
			return;

		_message = uiObjects[0] as TextFxTextMeshProUGUI;
		if (_message.IsEmpty())
			return;

		events.onBattleEnd += () => _message.AnimationManager.PlayAnimation();
	}
}