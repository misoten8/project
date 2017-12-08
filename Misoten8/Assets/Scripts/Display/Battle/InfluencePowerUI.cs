using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// InfluencePowerUI クラス
/// 製作者：実川
/// </summary>
public class InfluencePowerUI : MonoBehaviour
{
	[SerializeField]
	private Image _image;

	[SerializeField]
	private DisplayMediator _displayFacade;

	[SerializeField]
	private Define.PlayerType _playerType;

	void Update()
	{
		_image.fillAmount = _displayFacade.InfluencePower.GetValue(_playerType);
	}
}
