using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// LobbyDisplayDebugEvent クラス
/// </summary>
public class LobbyDisplayDebugEvent : DisplayDebugEventBase
{
	[SerializeField]
	private LobbyDisplay _lobbyDisplay;

	public override void OnAwake()
	{
		LobbyEvents e = _lobbyDisplay._events;

		debugEvents
			.eventList = new List<DisplayDebugger.DebugEvents.Element>()
			{

			};
	}
}