using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤー管理 クラス
/// プレイヤー全体に対するイベント等の発信や、変数の保管等を行う
/// 製作者：実川
/// </summary>
public class PlayerManager : Photon.MonoBehaviour
{
	/// <summary>
	/// ダンス時間
	/// </summary>
	public const float DANCE_TIME = 30.0f;

	/// <summary>
	/// ダンス開始するまでの間隔
	/// </summary>
	public const float DANCE_START_INTERVAL = 20.0f;

	/// <summary>
	/// 一回のダンスで発生するリクエスト回数
	/// </summary>
	public const int REQUEST_COUNT = 3;

	/// <summary>
	/// プレイヤーキャッシュリスト
	/// </summary>
	public List<Player> Players
	{
		get { return _players; }
	}

	private List<Player> _players = new List<Player>();

	/// <summary>
	/// プレイヤーを追加する
	/// </summary>
	public void SetPlayer(Player addPlayer)
	{
		_players.Add(addPlayer);
	}

	/// <summary>
	/// プレイヤーを取得する
	/// </summary>
	public Player GetPlayer(Define.PlayerType playerType)
	{
		if (playerType == Define.PlayerType.None)
			return null;
		
		return _players.First(e => e.Type == playerType);
	}

	/// <summary>
	/// 定義のみ
	/// </summary>
	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) { }
}