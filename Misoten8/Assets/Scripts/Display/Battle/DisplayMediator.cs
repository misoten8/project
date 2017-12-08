using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// DisplayMediator クラス
/// 外部、内部でのやり取りを仲介する　mediatorパターン
/// このスクリプトがアタッチされたオブジェクトをプレハブ化する
/// 製作者：実川
/// </summary>
public class DisplayMediator : MonoBehaviour
{
	public Text DancePoint
	{
		get { return _dancePoint; }
	}

	[Header("以下は外部から参照するフィールド群")]
	[SerializeField]
	private Text _dancePoint;

	public Text DanceSuccess
	{
		get { return _danceSuccess; }
	}

	[SerializeField]
	private Text _danceSuccess;

	public Text DanceFailure
	{
		get { return _danceFailure; }
	}

	[SerializeField]
	private Text _danceFailure;

	public Text DanceShake
	{
		get { return _danceShake; }
	}

	[SerializeField]
	private Text _danceShake;

	public Text DanceStop
	{
		get { return _danceStop; }
	}

	[SerializeField]
	private Text _danceStop;


	// 以下は子要素が使用するフィールド群


	public BattleTime BattleTime
	{
		get { return _battleTime; }
	}

	[Header("以下は内部から参照するフィールド群")]
	[SerializeField]
	private BattleTime _battleTime;

	public InfluencePower InfluencePower
	{
		get { return _influencePower; }
	}

	[SerializeField]
	private InfluencePower _influencePower;

	public Score Score
	{
		get { return _score; }
	}

	[SerializeField]
	private Score _score;
}
