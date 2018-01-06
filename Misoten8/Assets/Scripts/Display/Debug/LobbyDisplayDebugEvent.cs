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
				new DisplayDebugger.DebugEvents.Element(e.onPlayer1Online, nameof(e.onPlayer1Online)),
				new DisplayDebugger.DebugEvents.Element(e.onPlayer2Online, nameof(e.onPlayer2Online)),
				new DisplayDebugger.DebugEvents.Element(e.onPlayer3Online, nameof(e.onPlayer3Online)),
				new DisplayDebugger.DebugEvents.Element(e.onNavOnline, nameof(e.onNavOnline)),
				new DisplayDebugger.DebugEvents.Element(e.onPlayer1Offline, nameof(e.onPlayer1Offline)),
				new DisplayDebugger.DebugEvents.Element(e.onPlayer2Offline, nameof(e.onPlayer2Offline)),
				new DisplayDebugger.DebugEvents.Element(e.onPlayer3Offline, nameof(e.onPlayer3Offline)),
				new DisplayDebugger.DebugEvents.Element(e.onNavOffline, nameof(e.onNavOffline)),
				new DisplayDebugger.DebugEvents.Element(e.onBeginConnect, nameof(e.onBeginConnect)),
			};
	}
}