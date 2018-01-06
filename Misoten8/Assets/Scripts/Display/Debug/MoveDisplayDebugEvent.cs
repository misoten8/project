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

			};
	}
}