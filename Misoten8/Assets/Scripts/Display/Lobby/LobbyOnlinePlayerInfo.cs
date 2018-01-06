using System.Collections;
using System.Collections.Generic;
using TextFx;
using UnityEngine;
using Misoten8Utility;

/// <summary>
/// LobbyOnlinePlayerInfo クラス
/// </summary>
public class LobbyOnlinePlayerInfo : UIBase
{
	private TextFxUGUI _player1;
	private TextFxUGUI _player2;
	private TextFxUGUI _player3;
	private TextFxUGUI _nav;

	public override void OnAwake(ISceneCache cache, IEvents displayEvents)
	{
		base.OnAwake(cache, displayEvents);
		var events = displayEvents as LobbyEvents;
		if (events.IsEmpty())
			return;

		_player1 = uiObjects[0] as TextFxUGUI;
		if (_player1.IsEmpty())
			return;

		_player2 = uiObjects[1] as TextFxUGUI;
		if (_player2.IsEmpty())
			return;

		_player3 = uiObjects[2] as TextFxUGUI;
		if (_player3.IsEmpty())
			return;

		_nav = uiObjects[3] as TextFxUGUI;
		if (_nav.IsEmpty())
			return;

		events.onPlayer1Online += () => _player1.AnimationManager.PlayAnimation();
		events.onPlayer2Online += () => _player2.AnimationManager.PlayAnimation();
		events.onPlayer3Online += () => _player3.AnimationManager.PlayAnimation();
		events.onNavOnline += () => _nav.AnimationManager.PlayAnimation();
		events.onPlayer1Offline += () => _player1.AnimationManager.PlayAnimation(0, 3);
		events.onPlayer2Offline += () => _player2.AnimationManager.PlayAnimation(0, 3);
		events.onPlayer3Offline += () => _player3.AnimationManager.PlayAnimation(0, 3);
		events.onNavOffline += () => _nav.AnimationManager.PlayAnimation(0, 3);
	}
}