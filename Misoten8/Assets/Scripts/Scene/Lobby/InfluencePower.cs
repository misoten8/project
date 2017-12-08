using UnityEngine;
using System.Linq;

/// <summary>
/// プレイヤーの影響力（注目度）を設定、格納するクラス
/// Scoreから影響力に変換する
/// 製作者：実川
/// </summary>
public class InfluencePower : MonoBehaviour
{
	/// <summary>
	/// 影響力リスト合計で値が1.0になるようにする
	/// </summary>
	public float[] ValueArray
	{
		get { return _influencePowerArray; }
	}

	private float[] _influencePowerArray = new float[Define.METER_NUM_MAX] { 0.2f, 0.2f, 0.2f, 0.2f, 0.2f };

	/// <summary>
	/// スコアの配列から、影響力に変換する
	/// </summary>
	public void SetPlayerScoreArray(float[] allMember)
	{
		for(int i = 0; i < Define.METER_NUM_MAX; i++)
		{
			_influencePowerArray[i] = allMember[i] / allMember.Sum();
		}
	}

	/// <summary>
	/// 影響力（注目度）を要素ごとに取得する
	/// </summary>
	public float GetValue(Define.PlayerType playerType)
	{
		return _influencePowerArray[(int)playerType];
	}
}
