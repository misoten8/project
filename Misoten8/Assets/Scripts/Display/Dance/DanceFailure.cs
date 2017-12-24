using System.Collections;
using System.Collections.Generic;
using TextFx;
using UnityEngine;

/// <summary>
/// DanceFailure クラス
/// </summary>
public class DanceFailure : UIBase
{
	private Player _localPlayer;

	private MobManager _mobManager;

	private TextFxUGUI _textFx;

	private int startFunCount;

	public override void OnAwake(ISceneCache cache, IEvents displayEvents)
	{
		base.OnAwake(cache, displayEvents);

		var events = displayEvents as DanceEvents;
		if (events == null)
			Debug.LogWarning("DanceEventsが取得できませんでした");

		var battleSceneCache = cache as BattleSceneCache;
		if (battleSceneCache == null)
			Debug.LogWarning("BattleSceneCacheが取得できませんでした");

		_localPlayer = battleSceneCache.playerManager?.GetLocalPlayer();
		if (_localPlayer == null)
			Debug.LogWarning("localPlayerが取得できませんでした");

		_mobManager = battleSceneCache.mobManager;
		if (_mobManager == null)
			Debug.LogWarning("mobManagerが取得できませんでした");

		_textFx = uiObjects[0] as TextFxUGUI;
		if (_textFx == null)
			Debug.LogWarning("_textFxが取得できませんでした");

		if (events != null)
		{
			events.onDanceStart += () => startFunCount = _mobManager.GetFunCount(_localPlayer.Type);
			events.onDanceFailled += () =>
			{
				if (_mobManager == null)
				{
					_textFx.SetText("Failure...");
				}
				else
				{
					_textFx.SetText("Failure...");
					//TODO:ダンスバトル時はファンが減るため、表示する
					//int diff = _mobManager.GetFunCount(_localPlayer.Type);
					//_textFx.SetText("Failure...\n-" + diff.ToString() + "...");
				}
				_textFx.AnimationManager.PlayAnimation();
			};
		}
	}
}