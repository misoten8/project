using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// DanceDisplayDebugEvent クラス
/// </summary>
public class DanceDisplayDebugEvent : DisplayDebugEventBase
{
	[SerializeField]
	private DanceDisplay _danceDisplay;

	public override void OnAwake()
	{
		DanceEvents e = _danceDisplay._events;

		debugEvents
			.eventList = new List<DisplayDebugger.DebugEvents.Element>()
			{
				new DisplayDebugger.DebugEvents.Element(e.onDanceStart, nameof(e.onDanceStart)),
				new DisplayDebugger.DebugEvents.Element(e.onDanceEnd, nameof(e.onDanceEnd)),
				new DisplayDebugger.DebugEvents.Element(e.onDanceSuccess, nameof(e.onDanceSuccess)),
				new DisplayDebugger.DebugEvents.Element(e.onDanceFailled, nameof(e.onDanceFailled)),
				new DisplayDebugger.DebugEvents.Element(e.onRequestShake, nameof(e.onRequestShake)),
				new DisplayDebugger.DebugEvents.Element(e.onRequestStop, nameof(e.onRequestStop)),
				new DisplayDebugger.DebugEvents.Element(e.onRequestNolmaComplate, nameof(e.onRequestNolmaComplate))
			};
	}
}