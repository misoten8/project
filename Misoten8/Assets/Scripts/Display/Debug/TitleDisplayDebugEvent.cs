using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// TitleDisplayDebugEvent クラス
/// </summary>
public class TitleDisplayDebugEvent : DisplayDebugEventBase
{
	[SerializeField]
	private TitleDisplay _titleDisplay;

	public override void OnAwake()
	{
		TitleEvents e = _titleDisplay._events;

		debugEvents
			.eventList = new List<DisplayDebugger.DebugEvents.Element>()
			{

			};
	}
}