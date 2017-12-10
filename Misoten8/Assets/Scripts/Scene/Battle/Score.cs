using UnityEngine;
using System.Linq;
using System;

/// <summary>
/// Score クラス
/// 製作者：実川
/// </summary>
public class Score : MonoBehaviour
{
	public Action OnScoreUpdate;

	[SerializeField]
	private InfluencePower _ip;

	private int[] _valueArray = new int[Define.METER_NUM_MAX] { 0, 0, 0, 0, 0 };

	private void Update()
	{
		OnScoreUpdate?.Invoke();

		// 影響力の設定
	}

	/// <summary>
	/// 指定したプレイヤーのスコアを変更する
	/// </summary>
	public void SetScore(Define.PlayerType playerType, int value)
	{
		if ((int)playerType < 0 || (int)playerType >= Define.METER_NUM_MAX)
		{
			Debug.LogError("プレイヤー番号が不正です");
			return;
		}

		// スコアを設定する
		_valueArray[(int)playerType] = value;

		// 影響力を設定する
		_ip.SetPlayerScoreArray(_valueArray.Select(x => (float)x).ToArray());
	}

	/// <summary>
	/// 指定したプレイヤーのスコアを返す
	/// </summary>
	public int GetScore(Define.PlayerType playerType)
	{
		if ((int)playerType < 0 || (int)playerType >= Define.METER_NUM_MAX)
		{
			Debug.LogError("プレイヤー番号が不正です");
			return 0;
		}

		return _valueArray[(int)playerType];
	}
}
