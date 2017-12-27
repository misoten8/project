using UnityEngine.UI;
using UnityEngine;

/// <summary>
/// MoveGauge クラス
/// 製作者：実川
/// </summary>
public class MoveGauge : UIBase
{
	private int _borderShakeCount = PlayerManager.DANCE_START_SHAKE_COUNT;
	private Image _positiveGauge;
	private Image _negativeGauge;
	private Image _maxText;
	private float _drawValue = 0.0f;

	public override void OnAwake(ISceneCache cache, IEvents displayEvents)
	{
		base.OnAwake(cache, displayEvents);
		var events = displayEvents as MoveEvents;
		if (events == null)
			Debug.LogWarning("MoveEventsが取得できませんでした");

		_positiveGauge = uiObjects[3] as Image;
		if (_positiveGauge == null)
			Debug.LogWarning("positiveGaugeが取得できませんでした");

		_negativeGauge = uiObjects[2] as Image;
		if (_negativeGauge == null)
			Debug.LogWarning("negativeGaugeが取得できませんでした");

		_maxText = uiObjects[1] as Image;
		if (_maxText == null)
			Debug.LogWarning("maxTextが取得できませんでした");

		_positiveGauge.fillAmount = _drawValue;
		_negativeGauge.fillAmount = _drawValue;
		//TODO:減少時はネガティブゲージを使用する
		_negativeGauge.enabled = false;

		if (events != null)
		{
			events.onDanceGaugeMax += () =>
			{
				_maxText.enabled = true;
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
		_positiveGauge.fillAmount = _drawValue;
		_negativeGauge.fillAmount = _drawValue;
	}
}