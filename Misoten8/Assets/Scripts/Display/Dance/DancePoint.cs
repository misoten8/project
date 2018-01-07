using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// DancePoint クラス
/// </summary>
public class DancePoint : UIBase 
{
	private Image _gauge;
	private Image _clearText;
	private Image _clearGauge;
	private float _drawValue = 0.0f;
	private Dance _dance = null;

	public override void OnAwake(ISceneCache cache, IEvents displayEvents)
	{
		base.OnAwake(cache, displayEvents);
		var events = displayEvents as DanceEvents;
		if (events == null)
			Debug.LogWarning("DanceEventsが取得できませんでした");

		_gauge = uiObjects[2] as Image;
		if (_gauge == null)
			Debug.LogWarning("gaugeが取得できませんでした");

		_clearText = uiObjects[1] as Image;
		if (_clearText == null)
			Debug.LogWarning("clearTextが取得できませんでした");

		_clearGauge = uiObjects[3] as Image;
		if (_clearGauge == null)
			Debug.LogWarning("clearGaugeが取得できませんでした");

		var sceneCahce = cache as BattleSceneCache;
		if(sceneCahce == null)
			Debug.LogWarning("sceneCahceが取得できませんでした");

		_dance = sceneCahce.playerManager?.GetLocalPlayer()?.Dance;
		_gauge.fillAmount = _drawValue;

		if (events != null)
		{
			events.onDanceStart += () => _gauge.fillAmount = _drawValue;
			events.onRequestNolmaComplate += () =>
			{
				_clearText.enabled = true;
				_clearGauge.enabled = true;
			};
		}
	}

	public override bool IsDrawUpdate()
	{
		if(_dance == null)
		{
			return false;
		}
		float value = (float)Mathf.Min(_dance.DancePoint, PlayerManager.SHAKE_NORMA) / (float)PlayerManager.SHAKE_NORMA;
		if (_drawValue != value)
		{
			_drawValue = value;
			return true;
		}
		return false;
	}

	public override void OnDrawUpdate()
	{
		_gauge.fillAmount = _drawValue;
	}
}