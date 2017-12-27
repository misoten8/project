using System.Linq;
using System.Collections.Generic;

/// <summary>
/// プレイヤー管理 クラス
/// プレイヤー全体に対するイベント等の発信や、変数の保管等を行う
/// </summary>
public class PlayerManager : Photon.MonoBehaviour
{
	/// <summary>
	/// 使用するモデルの紐付けマップ
	/// </summary>
	public static readonly Dictionary<Define.PlayerType, ModelManager.ModelType> MODEL_MAP = new Dictionary<Define.PlayerType, ModelManager.ModelType>
	{
		{ Define.PlayerType.First, ModelManager.ModelType.Player1 },
		{ Define.PlayerType.Second, ModelManager.ModelType.Player2 },
		{ Define.PlayerType.Third, ModelManager.ModelType.Player3 }
	};

	/// <summary>
	/// ダンス時間
	/// </summary>
	public const float DANCE_TIME = 20.0f;

	/// <summary>
	/// 一回のダンスで発生するリクエスト回数
	/// </summary>
	public const int REQUEST_COUNT = 10;

	/// <summary>
	/// リクエスト秒数の偏り係数(数値が高い程偏る)
	/// </summary>
	public const int LEAN_COEFFICIENT = 3;

	/// <summary>
	/// ダンス成功条件数
	/// </summary>
	public const int SHAKE_NORMA = 10;

	/// <summary>
	/// ダンス開始に必要なシェイク数
	/// </summary>
	public const int DANCE_START_SHAKE_COUNT = 20;

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
		
		return _players?.First(e => e.Type == playerType);
	}

	/// <summary>
	/// ローカル（自身）のプレイヤーを取得する
	/// </summary>
	public Player GetLocalPlayer()
	{
		return _players?.First(e => e.photonView.owner.ID == PhotonNetwork.player.ID);
	}

	/// <summary>
	/// 定義のみ
	/// </summary>
	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) { }
}