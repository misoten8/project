using System.Collections;
using System.Collections.Generic;
using TextFx;
using TMPro;
using UnityEngine;
using Misoten8Utility;

/// <summary>
/// MoveTimer クラス
/// 製作者：実川
/// </summary>
public class MoveTimer : UIBase
{
	private BattleTime _battleTime = null;
	private float _currentTime = 0.0f;
	private TextMeshProUGUI _textMeshPro;

	public override void OnAwake(ISceneCache cache, IEvents displayEvents)
	{
		base.OnAwake(cache, displayEvents);
		var sceneCache = cache as BattleSceneCache;
		sceneCache.IsEmpty();

		var events = displayEvents as MoveEvents;
		events.IsEmpty();

		_textMeshPro = uiObjects[0] as TextMeshProUGUI;
		_textMeshPro.IsEmpty();

		_battleTime = sceneCache.battleTime;
		_battleTime.IsEmpty();

		OnDrawUpdate();
	}

	public override bool IsDrawUpdate()
	{
		float value = _battleTime.CurrentTime;
		if (_currentTime != value)
		{
			_currentTime = value;
			return true;
		}
		return false;
	}

	public override void OnDrawUpdate()
	{
		int minute = (int)_battleTime.CurrentTime / 60;
		int second = (int)_battleTime.CurrentTime % 60;
		string time = minute.ToString() + ":" + (second < 10 ? "0" + second.ToString() : second.ToString());

		_textMeshPro.SetText(time);
	}
}