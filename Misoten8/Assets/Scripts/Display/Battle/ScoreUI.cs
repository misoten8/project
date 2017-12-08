using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ScoreUI クラス
/// 製作者：実川
/// </summary>
public class ScoreUI : MonoBehaviour
{
	[SerializeField]
	private Define.PlayerType _playerType;

	[SerializeField]
	private DisplayMediator _displayFacade;

	[SerializeField]
	private Text _text;

	private Score _score = null;

	void Start ()
	{
		_score = _displayFacade.Score;
		_text.text = _score.GetScore(_playerType).ToString();
	}
	
	void Update ()
	{
		_text.text = "Player" + _playerType.ToString() + ":" + _score.GetScore(_playerType).ToString();
	}
}
