using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ResultScoreUI クラス
/// 製作者：実川
/// </summary>
public class ResultScoreUI : MonoBehaviour 
{
	[SerializeField]
	private Define.PlayerType _playerType;

	[SerializeField]
	private Text _text;

	void Start ()
	{
		_text.text = "Player" + _playerType.ToString() + ":" + ResultScore.scoreArray[(int)_playerType].ToString();
	}
}