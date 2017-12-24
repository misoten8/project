using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// DanceStop クラス
/// </summary>
public class DanceStop : UIBase
{
	public override void OnAwake(ISceneCache cache, IEvents displayEvents)
	{
		base.OnAwake(cache, displayEvents);
		var events = displayEvents as DanceEvents;

		if (events != null)
		{
			events.onDanceStart += () => uiObjects[0].color = UnityEngine.Color.clear;
			events.onRequestShake += () => uiObjects[0].color = UnityEngine.Color.clear;
			events.onRequestStop += () => uiObjects[0].color = UnityEngine.Color.white;
			events.onRequestFailled += () => uiObjects[0].color = UnityEngine.Color.clear;
		}
	}
}