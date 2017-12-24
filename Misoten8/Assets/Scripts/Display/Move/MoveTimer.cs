using System.Collections;
using System.Collections.Generic;
using TextFx;
using UnityEngine;

/// <summary>
/// MoveTimer クラス
/// 製作者：実川
/// </summary>
public class MoveTimer : UIBase
{
	private BattleTime _battleTime = null;
	private float _currentTime = 0.0f;
	private TextFxUGUI _textFx;

	public override void OnAwake(ISceneCache cache, IEvents displayEvents)
	{
		base.OnAwake(cache, displayEvents);
		var sceneCache = cache as BattleSceneCache;
		if (sceneCache == null)
			Debug.LogWarning("BattleSceneCacheが取得できませんでした");

		var events = displayEvents as DanceEvents;
		if (events == null)
			Debug.LogWarning("DanceEventsが取得できませんでした");

		_textFx = uiObjects[0] as TextFxUGUI;
		if (_textFx == null)
			Debug.LogWarning("_textFxが取得できませんでした");

		_battleTime = sceneCache.battleTime;
		if(_battleTime == null)
			Debug.LogWarning("BattleTimeが取得できませんでした");

		_textFx.SetText("Time Limit:" + _battleTime.CurrentTime.ToString("F0"));
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
		_textFx.SetText("Time Limit:" + _currentTime.ToString("F0"));
	}
}