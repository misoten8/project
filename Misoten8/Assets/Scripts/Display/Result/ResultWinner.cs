using System.Collections;
using System.Collections.Generic;
using Misoten8Utility;
using TMPro;
using TextFx;
using UnityEngine;

/// <summary>
/// ResultWinner クラス
/// </summary>
public class ResultWinner : UIBase
{
	private TextFxTextMeshProUGUI _item;

	private TextFxTextMeshProUGUI _playerName;

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

		_item = uiObjects[0] as TextFxTextMeshProUGUI;
		if (_item.IsEmpty())
			return;

		_playerName = uiObjects[1] as TextFxTextMeshProUGUI;
		if (_playerName.IsEmpty())
			return;

		events.onPlayWinnerPanel += () =>
		{
			_item.AnimationManager.PlayAnimation();
			_playerName.SetText(Define.playerNameMap[_resultRanking.GetWinner()]);
			StartCoroutine(PlayerName());
		};
	}

	public IEnumerator PlayerName()
	{
		yield return new WaitForSeconds(1.0f);

		_playerName.AnimationManager.PlayAnimation();

		yield return null;
	}
}