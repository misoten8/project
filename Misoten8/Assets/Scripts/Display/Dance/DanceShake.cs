using System.Collections;
using System.Collections.Generic;
using TextFx;
using UnityEngine;

/// <summary>
/// DanceShake クラス
/// 製作者：実川
/// </summary>
public class DanceShake : UIBase
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

		events.onRequestShake += () =>
		{
			_textFx.enabled = true;
			_textFx.AnimationManager.PlayAnimation();
		};
		events.onRequestStop += () => _textFx.enabled = false;
		events.onDanceEnd += () => _textFx.enabled = false;
	}
}