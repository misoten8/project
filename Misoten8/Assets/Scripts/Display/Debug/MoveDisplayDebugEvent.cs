using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// MoveDisplayDebugEvent クラス
/// </summary>
public class MoveDisplayDebugEvent : DisplayDebugEventBase
{
	[SerializeField]
	private MoveDisplay _moveDisplay;

	public override void OnAwake()
	{
		MoveEvents e = _moveDisplay._events;

		debugEvents
			.eventList = new List<DisplayDebugger.DebugEvents.Element>()
			{
				new DisplayDebugger.DebugEvents.Element(e.onBattleReady, nameof(e.onBattleReady)),
				new DisplayDebugger.DebugEvents.Element(e.onBattleStart, nameof(e.onBattleStart)),
				new DisplayDebugger.DebugEvents.Element(e.onDanceGaugeMax, nameof(e.onDanceGaugeMax)),
				new DisplayDebugger.DebugEvents.Element(e.onBattleEnd, nameof(e.onBattleEnd)),
			};
	}
}