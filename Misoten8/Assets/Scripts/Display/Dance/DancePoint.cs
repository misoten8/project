using UnityEngine.UI;
/// <summary>
/// DancePoint クラス
/// </summary>
public class DancePoint : UIBase 
{
	private Image _image;

	public override void OnAwake(ISceneCache cache, IEvents displayEvents)
	{
		base.OnAwake(cache, displayEvents);
		var events = displayEvents as DanceEvents;
		
		if (events != null)
		{
			events.onDanceStart += () => uiObjects[0].color = UnityEngine.Color.white;
		}
	}
}