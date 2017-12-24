using System.Collections;
using System.Collections.Generic;
using TextFx;
using UnityEngine;

/// <summary>
/// DanceStop クラス
/// </summary>
public class DanceStop : UIBase
{
	private TextFxUGUI _textFx;

	public override void OnAwake(ISceneCache cache, IEvents displayEvents)
	{
		base.OnAwake(cache, displayEvents);
		var events = displayEvents as DanceEvents;
		if (events == null)
			Debug.LogWarning("DanceEventsが取得できませんでした");

		_textFx = uiObjects[0] as TextFxUGUI;
		if (_textFx == null)
			Debug.LogWarning("_textFxが取得できませんでした");

		if (events != null)
		{
			events.onRequestShake += () => _textFx.enabled = false;
			events.onRequestStop += () =>
			{
				_textFx.enabled = true;
				_textFx.AnimationManager.PlayAnimation();
			};
			events.onDanceFinished += () => _textFx.enabled = false;
		};
	}
}