using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// シェイク値表示UI
/// </summary>
public class ShakeIcon : UIBase 
{
	[SerializeField]
	private int _borderShakeCount;
	private Image _image;
	private float _drawValue = 0.0f;

	public override void OnAwake(ISceneCache cache, IEvents displayEvents)
	{
		base.OnAwake(cache, displayEvents);
		_image = uiObjects[0] as Image;
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
		_image.fillAmount = _drawValue;
	}
}