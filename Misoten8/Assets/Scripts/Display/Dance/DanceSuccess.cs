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
			events.onDanceStart += () => startFunCount = _mobManager?.GetFunCount(_localPlayer.Type) ?? 0;
			events.onDanceSuccess += () =>
			{
				if (_mobManager == null)
				{
					_textFx.SetText("Success!!");
				}
				else
				{
					_textFx.SetText("Success!!");
					//TODO:クリア時に何人増えたか表示する
					//int diff = _mobManager.GetFunCount(_localPlayer.Type) - startFunCount;
					//_textFx.SetText("Success!!\n+" + diff.ToString() + "!");
				}
				_textFx.AnimationManager.PlayAnimation();
			};
		}
	}
}