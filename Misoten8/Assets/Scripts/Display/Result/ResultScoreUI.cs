using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// スコア表示UI
/// </summary>
public class ResultScoreUI : UIBase
{
	[SerializeField]
	private Define.PlayerType _playerType;

	private Text _text;

	public override void OnAwake(ISceneCache cache, IEvents displayEvents)
	{
		base.OnAwake(cache, displayEvents);
		_text = uiObjects[0] as Text;
		_text.text = "Player" + _playerType.ToString() + ":" + ResultScore.scoreArray[(int)_playerType].ToString();
	}
}