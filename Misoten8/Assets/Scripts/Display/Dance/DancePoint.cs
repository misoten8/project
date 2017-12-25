using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// DancePoint クラス
/// </summary>
public class DancePoint : UIBase 
{
	private int _borderShakeCount = PlayerManager.SHAKE_NORMA;
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

		//var sceneCahce = cache as BattleSceneCache;
		//sceneCahce.playerManager.GetPlayer( Define.PlayerType);

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
		float value = Mathf.Min(shakeparameter.GetShakeParameter(), _borderShakeCount) / _borderShakeCount;
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