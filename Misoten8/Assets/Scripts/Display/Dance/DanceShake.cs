using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// DanceShake クラス
/// 製作者：実川
/// </summary>
public class DanceShake : UIBase
{
	public override void OnAwake(ISceneCache cache, IEvents displayEvents)
	{
		base.OnAwake(cache, displayEvents);
		var events = displayEvents as DanceEvents;

		if (events != null)
		{
			events.onDanceStart += () => uiObjects[0].color = UnityEngine.Color.clear;
			events.onRequestSuccess += () => uiObjects[0].color = UnityEngine.Color.white;
		}
	}
}