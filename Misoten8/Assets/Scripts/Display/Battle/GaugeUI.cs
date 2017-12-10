using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// GaugeUI クラス
/// 製作者：実川
/// </summary>
public class GaugeUI : MonoBehaviour 
{
	private enum ColorType
	{
		Blue = 0,
		Red,
		Max
	}

	[SerializeField]
	private DisplayMediator _displayFacade;

	[SerializeField]
	private Image[] _gauges = new Image[(int)ColorType.Max];

	[SerializeField]
	private Image _frame;

	private ColorType _colorType = ColorType.Blue;

	void Update () 
	{
		_gauges[(int)_colorType].fillAmount = Mathf.Min(shakeparameter.GetShakeParameter(), 3) / 3.0f;
	}

	public void CanDance()
	{
		_colorType = ColorType.Blue;
		_gauges[(int)ColorType.Red].fillAmount = 0.0f;
	}

	public void CanNotDance()
	{
		_colorType = ColorType.Red;
		_gauges[(int)ColorType.Blue].fillAmount = 0.0f;
	}

	public void SetActive(bool isActive)
	{
		if(isActive)
		{
			_frame.enabled = true;
			_gauges[(int)ColorType.Blue].enabled = true;
			_gauges[(int)ColorType.Red].enabled = true;
		}
		else
		{
			_frame.enabled = false;
			_gauges[(int)ColorType.Blue].enabled = false;
			_gauges[(int)ColorType.Red].enabled = false;
		}
	}
}