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
	/// プレイヤーの人数(実況無し)
	/// </summary>
	public const int PLAYER_NUM_MAX_CHARACTOR_ONLY = PLAYER_NUM_MAX - 1;

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
	/// シーン遷移に必要なシェイク数(プレス数)
	/// </summary>
    public const int SCENE_TRANCE_VALUE = 3;
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

	public enum PlayerType : byte
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

	/// <summary>
	/// プレイヤー名取得マップ
	/// </summary>
	public static readonly Dictionary<PlayerType, string> playerNameMap = new Dictionary<PlayerType, string>
	{
		{ PlayerType.First, "Player1" },
		{ PlayerType.Second, "Player2" },
		{ PlayerType.Third, "Player3" },
		{ PlayerType.Camera, "Nav" },
	};

	/// <summary>
	/// バトルに参加したプレイヤーの数
	/// </summary>
	public static int JoinBattlePlayerNum
	{
		get { return _joinBattlePlayerNum; }
		set
		{
			_joinBattlePlayerNum = value;
#if DEBUG
			Debug.Log("バトルに参加したプレイヤーの数が設定されました。設定数:" + _joinBattlePlayerNum.ToString());
#endif
		}
	}

	private static int _joinBattlePlayerNum = 0;

	/// <summary>
	/// このマシンが使用するプレイヤーの種類
	/// </summary>
	public static PlayerType LocalPlayerType
	{
		get { return _localPlayerType; }
		set
		{
			_localPlayerType = value;
#if DEBUG
			Debug.Log("このマシンが使用するプレイヤーの種類が設定されました。プレイヤータイプ:" + _localPlayerType.ToString());
#endif
		}
	}

	private static PlayerType _localPlayerType = PlayerType.None;

	/// <summary>
	/// バトル終了時のスコアマップ
	/// </summary>
	public static Dictionary<PlayerType, int> ResultScoreMap
	{
		get { return _resultScoreMap; }
	}

	private static Dictionary<PlayerType, int> _resultScoreMap = new Dictionary<PlayerType, int>
	{
		{ PlayerType.First, 1 },
		{ PlayerType.Second, 2 },
		{ PlayerType.Third, 3 },
	};

	/// <summary>
	/// プレイヤーIDからプレイヤーのタイプに変換する
	/// </summary>
	public static PlayerType ConvertToPlayerType(int playerId)
	{
		//TODO:内部のロジックはロビーの設定に合わせて変更する
		switch (playerId)
		{
			case 0:
				return PlayerType.None;
			case 1:
				return PlayerType.First;
			case 2:
				return PlayerType.Second;
			case 3:
				return PlayerType.Third;
			default:
				Debug.LogWarning("Define.ConvertToPlayerTypeにて不正なplayerIdが指定されました\n代用としてNoneを返しました");
				return PlayerType.None;
		}
	}
}