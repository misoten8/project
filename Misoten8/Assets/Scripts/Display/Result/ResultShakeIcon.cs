using System.Collections;
using System.Collections.Generic;
using Misoten8Utility;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ResultShakeIcon クラス
/// </summary>
public class ResultShakeIcon : UIBase
{
	[SerializeField]
	private Animator _animator;

	private int _borderShakeCount = Define.SCENE_TRANCE_VALUE;
	private Image _image;
	private float _drawValue = 0.0f;
	private string _animNameSlideUp = "SlideUp";

	public override void OnAwake(ISceneCache cache, IEvents displayEvents)
	{
		base.OnAwake(cache, displayEvents);
		_image = uiObjects[0] as Image;

		var events = displayEvents as ResultEvents;
		if (events.IsEmpty())
			return;

		events.onTransTitleReady += () => _animator.CrossFade(_animNameSlideUp, 1.0f, 0);
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