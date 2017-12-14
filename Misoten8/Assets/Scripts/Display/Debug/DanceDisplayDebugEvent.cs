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
				new DisplayDebugger.DebugEvents.Element(e.onRequestSuccess, nameof(e.onRequestSuccess)),
				new DisplayDebugger.DebugEvents.Element(e.onRequestFailled, nameof(e.onRequestFailled)),
				new DisplayDebugger.DebugEvents.Element(e.onPenetrationPlayer, nameof(e.onPenetrationPlayer))
			};
	}
}