using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// リクエスト投影UIオブジェクト
/// </summary>
public class DanceRequester : UIBase
{
	private Image _arrow;
	private Image _cross;

	public override void OnAwake(ISceneCache cache, IEvents displayEvents)
	{
		base.OnAwake(cache, displayEvents);

		var events = displayEvents as DanceEvents;
		if (events == null)
			Debug.LogWarning("DanceEventsが取得できませんでした");

		_arrow = uiObjects[3] as Image;
		if (_arrow == null)
			Debug.LogWarning("arrowが取得できませんでした");

		_cross = uiObjects[4] as Image;
		if (_cross == null)
			Debug.LogWarning("crossが取得できませんでした");

		events.onDanceStart += () =>
		{
			_arrow.enabled = false;
			_cross.enabled = false;
		};
		events.onRequestShake += () =>
		{
			_arrow.enabled = true;
			_cross.enabled = false;
		};
		events.onRequestStop += () =>
		{
			_arrow.enabled = false;
			_cross.enabled = true;
		};
		events.onDanceEnd += () =>
		{
			_arrow.enabled = false;
			_cross.enabled = false;
		};
	}
}