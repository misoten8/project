using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// ResultRanking クラス
/// 製作者：実川
/// </summary>
public class ResultRanking : MonoBehaviour 
{
	private Dictionary<Define.PlayerType, int> _rankMap = new Dictionary<Define.PlayerType, int>();

	/// <summary>
	/// 指定したプレイヤーのランキングを取得する
	/// </summary>
	public int GetPlayerRank(Define.PlayerType type)
	{
		int rank = 0;
		if(_rankMap.TryGetValue(type, out rank))
		{
			return rank;
		}
		Debug.LogWarning("GetPlayerRankに指定されたプレイヤーの種類が不正です");
		return rank;
	}

	/// <summary>
	/// 勝者を取得する
	/// </summary>
	public Define.PlayerType GetWinner()
	{
        if (_rankMap.Count == 0)
            return Define.PlayerType.First;

		return _rankMap.First(e => e.Value == 1).Key;
	}

	void Start () 
	{
		// スコアでソートを行う
		List<RankingEx> sort = new List<RankingEx>();

		for(int i = 0; i < Define.JoinBattlePlayerNum; i++)
		{
			var type = Define.ConvertToPlayerType(i + 1);
			sort.Add(new RankingEx(type, Define.ResultScoreMap[type]));	
		}
		sort = sort.OrderByDescending(e => e.score).ToList();

		// ランキングを登録
		for(int i = 0; i < Define.JoinBattlePlayerNum; i++)
		{
			_rankMap.Add(sort[i].playerType, i + 1);
		}
	}

	internal class RankingEx
	{
		public Define.PlayerType playerType;
		public int score;

		public RankingEx(Define.PlayerType playerType, int score)
		{
			this.playerType = playerType;
			this.score = score;
		}
	}
}