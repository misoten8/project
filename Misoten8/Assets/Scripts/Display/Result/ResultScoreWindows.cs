using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Misoten8Utility;
using TMPro;

/// <summary>
/// ResultScoreWindows クラス
/// </summary>
public class ResultScoreWindows : UIBase
{
	[SerializeField]
	private List<Animator> _panelPlayer = new List<Animator>();

	private List<TextMeshProUGUI> _fanValues = new List<TextMeshProUGUI>();

	private string _animNameSlideUp = "SlideUp";

	private ResultRanking _resultRanking = null;

	public override void OnAwake(ISceneCache cache, IEvents displayEvents)
	{
		base.OnAwake(cache, displayEvents);

		var sceneCache = cache as ResultSceneCache;
		if (sceneCache.IsEmpty())
			return;

		_resultRanking = sceneCache.resultRanking;
		if (_resultRanking.IsEmpty())
			return;

		var events = displayEvents as ResultEvents;
		if (events.IsEmpty())
			return;

		for(int i = 0; i < uiObjects.Length; i++)
		{
			var fan = uiObjects[i] as TextMeshProUGUI;

			if (fan.IsEmpty())
				return;

			_fanValues.Add(fan);
		}

		if (_fanValues.Count != Define.PLAYER_NUM_MAX - 1)
		{
			Debug.LogWarning("要素数が不正です");
			return;
		}
			
		events.onOpneScorePanel += () =>
		{
			for(int i = 0; i < Define.JoinBattlePlayerNum; i++)
			{
				_panelPlayer[i].CrossFade(_animNameSlideUp, 1.0f, 0);
				_fanValues[i].SetText(ResultScore.scoreArray[i].ToString() + "にん");

				RectTransform rectTransform = _panelPlayer[i].gameObject.GetComponent<RectTransform>();
				float offset = _resultRanking.GetPlayerRank((Define.PlayerType)(i + 1)) * 0.1f;
				rectTransform.pivot = new Vector2(rectTransform.pivot.x, rectTransform.pivot.y + offset);
			}
		};
	}
}