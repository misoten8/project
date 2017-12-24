using TextFx;
using UnityEngine;

/// <summary>
/// DanceSuccess クラス
/// </summary>
public class DanceSuccess : UIBase
{
	private Player _localPlayer;

	private MobManager _mobManager;

	private TextFxUGUI _textFx;

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
			events.onDanceSuccess += () =>
			{
				_textFx.AnimationManager.PlayAnimation();
				if (_mobManager == null)
				{
					// 文字編集
				}
				else
				{
					_mobManager.GetFunCountDiff(_localPlayer.Type);
				}
			};
		}
	}
}