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
	private string _animNameSlideDown = "SlideDown";

	public override void OnAwake(ISceneCache cache, IEvents displayEvents)
	{
		base.OnAwake(cache, displayEvents);
		_image = uiObjects[0] as Image;

		var events = displayEvents as ResultEvents;
		if (events.IsEmpty())
			return;

		events.onTransTitleReady += () => _animator.CrossFade(_animNameSlideDown, 1.0f, 0);
	}
}