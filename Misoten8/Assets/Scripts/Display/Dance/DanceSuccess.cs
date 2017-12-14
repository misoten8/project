using UnityEngine;

/// <summary>
/// DanceSuccess クラス
/// </summary>
public class DanceSuccess : UIBase
{
	private Player _localPlayer;

	private MobManager _mobManager;

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

		if (events != null)
		{
			events.onDanceStart += () => uiObjects[0].color = Color.clear;
			events.onDanceSuccess += () =>
			{
				uiObjects[0].color = Color.white;
				if(_mobManager == null)
				{
					// 文字編集
				}
				else
				{
					_mobManager.GetFunCountDiff(_localPlayer.Type);
				}
			};
			events.onDanceEnd += () => uiObjects[0].color = Color.clear;
		}
	}
}