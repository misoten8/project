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

		events.onDanceStart += () => uiObjects[0].color = UnityEngine.Color.clear;
		events.onRequestShake += () => uiObjects[0].color = UnityEngine.Color.white;
		events.onRequestStop += () => uiObjects[0].color = UnityEngine.Color.clear;
		events.onRequestFailled += () => uiObjects[0].color = UnityEngine.Color.clear;
	}
}