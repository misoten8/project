using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Define
{
	/// <summary>
	/// プレイヤーの人数(実況も含む)
	/// </summary>
	public const int PLAYER_NUM_MAX = 4;

	/// <summary>
	/// メーターUIの表示数(プレイヤー数 + 無所属)
	/// </summary>
	public const int METER_NUM_MAX = PLAYER_NUM_MAX + 1;

	public const int FAN_POINT_EASY = 100;

	public const int FAN_POINT_NORMAL = 200;

	public const int FAN_POINT_HARD = 300;

	public const int FAN_SCORE_EASY = 10;

	public const int FAN_SCORE_NORMAL = 50;

	public const int FAN_SCORE_HARD = 100;

	/// <summary>
	/// ファンポイント最大値(この値によって各プレイヤーに割り振られるスコアポイントが決まる)
	/// </summary>
	public static readonly int[] FanPointArray = new int[(int)FanLevel.Max]
	{
		FAN_POINT_EASY,
		FAN_POINT_NORMAL,
		FAN_POINT_HARD
	};

	/// <summary>
	/// 各プレイヤーに対する好感度に応じて与えられるスコアポイント
	/// </summary>
	public static readonly int[] FanScoreArray = new int[(int)FanLevel.Max]
	{
		FAN_SCORE_EASY,
		FAN_SCORE_NORMAL,
		FAN_SCORE_HARD
	};

	/// <summary>
	/// ルームカスタムプロパティ
	/// </summary>
	/// <remarks>
	/// 転送サイズの軽減やタイプミスを考慮して列挙型を定義
	/// </remarks>
	public enum RoomPropaties : byte
	{
		/// <summary>
		/// nullを返します
		/// </summary>
		None = 0,
		/// <summary>
		/// ロビーシーンが読み込み完了したかどうか(bool)
		/// </summary>
		IsLobbySceneLoaded,
		/// <summary>
		/// ルームでのマッチングが完了し、バトルを開始できる状態かどうか(bool)
		/// </summary>
		RoomMatchingComplete,
		/// <summary>
		/// バトルシーンが読み込み完了したかどうか(bool)
		/// </summary>
		IsBattleSceneLoaded,
	}

	/// <summary>
	/// ファン化難易度
	/// </summary>
	public enum FanLevel
	{
		Easy = 0,
		Normal,
		Hard,
		/// <summary>
		/// 最大数
		/// </summary>
		Max
	}

	public enum PlayerType
	{
		/// <summary>
		/// 無所属
		/// </summary>
		None = 0,
		/// <summary>
		/// Player1
		/// </summary>
		First,
		/// <summary>
		/// Player2
		/// </summary>
		Second,
		/// <summary>
		/// Player3
		/// </summary>
		Third,
		/// <summary>
		/// Player4
		/// </summary>
		Fourth,
		/// <summary>
		/// 実況
		/// </summary>
		Camera
	}

	/// <summary>
	/// ルーム作成時に設定されるカスタムプロパティ
	/// </summary>
	public static readonly ExitGames.Client.Photon.Hashtable defaultRoomPropaties = new ExitGames.Client.Photon.Hashtable
	{
		{ RoomPropaties.IsLobbySceneLoaded, false },
		{ RoomPropaties.RoomMatchingComplete, false },
		{ RoomPropaties.IsBattleSceneLoaded, false },
	};

	/// <summary>
	/// プレイヤーの色
	/// </summary>
	public static readonly Color[] playerColor = new Color[]
	{
		Color.gray,
		Color.red,
		Color.blue,
		Color.green,
		Color.yellow
	};
}