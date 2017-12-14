using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// DanceFailure クラス
/// </summary>
public class DanceFailure : UIBase
{
	public override void OnAwake(ISceneCache cache, IEvents displayEvents)
	{
		base.OnAwake(cache, displayEvents);
		var events = displayEvents as DanceEvents;

		if (events != null)
		{
			events.onDanceStart += () => uiObjects[0].color = UnityEngine.Color.clear;
			events.onDanceFailled += () => uiObjects[0].color = UnityEngine.Color.white;
		}
	}
}