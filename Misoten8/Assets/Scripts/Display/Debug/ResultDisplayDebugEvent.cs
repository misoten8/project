using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ResultDisplayDebugEvent クラス
/// </summary>
public class ResultDisplayDebugEvent : DisplayDebugEventBase
{
	[SerializeField]
	private ResultDisplay _resultDisplay;

	[SerializeField]
	private int _joinBattlePlayerNum;

	public override void OnAwake()
	{
		Define.JoinBattlePlayerNum = _joinBattlePlayerNum;

		ResultEvents e = _resultDisplay._events;

		debugEvents
			.eventList = new List<DisplayDebugger.DebugEvents.Element>()
			{
				new DisplayDebugger.DebugEvents.Element(e.onPlayWinnerPanel, nameof(e.onPlayWinnerPanel)),
				new DisplayDebugger.DebugEvents.Element(e.onOpneScorePanel, nameof(e.onOpneScorePanel)),
				new DisplayDebugger.DebugEvents.Element(e.onTransTitleReady, nameof(e.onTransTitleReady))
			};
	}
}