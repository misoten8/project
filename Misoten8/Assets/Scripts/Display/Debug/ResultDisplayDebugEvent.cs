using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ResultDisplayDebugEvent クラス
/// </summary>
public class ResultDisplayDebugEvent : DisplayDebugEventBase
{
	[SerializeField]
	private ResultDisplay _resultDisplay;

	public override void OnAwake()
	{
		ResultEvents e = _resultDisplay._events;

		debugEvents
			.eventList = new List<DisplayDebugger.DebugEvents.Element>()
			{

			};
	}
}