using System;
using UnityEngine;
using UniRx;
using WiimoteApi;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// ダンス クラス
/// 製作者：実川
/// </summary>
public class Dance : MonoBehaviour
{
	public enum DanceResultState : byte
	{
		None = 0,
		/// <summary>
		/// 個人のダンスでクリア
		/// </summary>
		Clear,
		/// <summary>
		/// 個人のダンスでクリア
		/// </summary>
		Miss,
		/// <summary>
		/// ダンスバトルで勝利
		/// </summary>
		Win,
		/// <summary>
		/// ダンスバトルで敗北
		/// </summary>
		Lose,
		/// <summary>
		/// ダンスバトルで引き分け
		/// </summary>
		Draw,
		/// <summary>
		/// ダンスを中断
		/// </summary>
		Cansel,
		Max
	}

	public enum Phase
	{
		/// <summary>
		/// 躍っていない状態
		/// </summary>
		None,
		/// <summary>
		/// ダンス準備～ダンス開始まで
		/// </summary>
		Start,
		/// <summary>
		/// ダンス開始～ダンス終了まで
		/// </summary>
		Play,
		/// <summary>
		/// ダンス終了～通常モード遷移準備まで
		/// </summary>
		Finish,
		/// <summary>
		/// 通常モード遷移準備～通常モードまで
		/// </summary>
		End,
		/// <summary>
		/// 乱入モード～ダンス準備まで
		/// </summary>
		/// <remarks>
		/// 現在の仕様ではフェーズがNone or Startの状態で乱入できる
		/// </remarks>
		Penetration,
	}

	public Define.PlayerType PlayerType
	{
		get { return _player.Type; }
	}

	/// <summary>
	/// ダンス中かどうか
	/// </summary>
	/// <remarks>
	/// None以外のフェーズがダンスモード
	/// </remarks>
	public bool IsPlaying
	{
		get { return _isPlaing; }
	}

	public bool IsRequestShake
	{
		get { return _isRequestShake; }
	}

	public Player Player
	{
		get { return _player; }
	}

	/// <summary>
	/// ダンスの状態
	/// </summary>
	public Phase DancePhase
	{
		get { return _phase; }
	}

	/// <summary>
	/// ダンス終了時のファン増減数
	/// </summary>
	public int ChangeFanCountAtComplateDance
	{
		get { return _changeFanCountAtComplateDance; }
	}

	/// <summary>
	/// 対戦相手のリスト
	/// </summary>
	public List<Define.PlayerType> BattleTargetList
	{
		get { return _battleTargetList; }
	}

	/// <summary>
	/// 対戦相手のリスト
	/// </summary>
	private List<Define.PlayerType> _battleTargetList = new List<Define.PlayerType>();

	[SerializeField]
	private Player _player;

	/// <summary>
	/// ダンスの効果範囲の当たり判定
	/// </summary>
	[SerializeField]
	private SphereCollider _danceCollider;

	[SerializeField]
	private MeshRenderer _danceFloor;

	private playercamera _playercamera;

	private DanceResultState _state = DanceResultState.None;

	private Phase _phase = Phase.None;

	public int DancePoint
	{
		get { return _dancePoint; }
	}

	private int _dancePoint = 0;

	private bool _isRequestShake = false;

	/// <summary>
	/// 乱入している or されているかどうか
	/// </summary>
	private bool _isPenetrated = false;

	/// <summary>
	/// ダンス中かどうか
	/// </summary>
	private bool _isPlaing = false;

	/// <summary>
	/// 各リクエスト事の持続時間
	/// </summary>
	private float[] _requestTime = new float[PlayerManager.REQUEST_COUNT];

	/// <summary>
	/// ステップ実行のコルーチンインスタンス
	/// </summary>
	private Coroutine _coroutine = null;

	private int _changeFanCountAtComplateDance;

	/// <summary>
	/// このダンスがダンスバトルかどうか
	/// </summary>
	private bool _isDanceBattle = false;

	/// <summary>
	/// ダンスバトルの結果を受け取ったかどうか
	/// </summary>
	private bool _isReceiveDanceBattleResult = false;

	/// <summary>
	/// playerに呼び出してもらう
	/// </summary>
	public void OnAwake(playercamera playerCamera)
	{
		_playercamera = playerCamera;

		_danceCollider.enabled = true;

		_dancePoint = 0;
	}

	void Update()
	{
		if(!Player.BattleScene.IsBattleTime)
		{
			if(_coroutine != null)
			{
				StopCoroutine(_coroutine);
				_coroutine = null;
				if (_phase != Phase.End)
				{
					PhaseEnd();
				}
			}
			return;
		}

		switch (_phase)
		{
			case Phase.Play:
				if (Player.IsMine)
				{
					if (Input.GetKeyDown(KeyCode.Return) || WiimoteManager.GetSwing(0))
					{
						Player.photonView.RPC("DanceShake", PhotonTargets.All, (byte)PlayerType);
					}
				}
				break;
		}
	}

	/// <summary>
	/// 振った時の処理
	/// </summary>
	public void Shake()
	{
		if(Player.IsMine)
		{
			ChangeFanPoint(_isRequestShake ? 1 : -1);
		}
		ParticleManager.Play(_isRequestShake ? "DanceNowClear" : "DanceNowFailed", new Vector3(), transform);
	}

	/// <summary>
	/// ダンス開始
	/// </summary>
	public void Begin()
	{
		_coroutine = StartCoroutine(StepDo());
	}
	/// <summary>
	/// 1vs1ダンスバトル開始
	/// </summary>
	public void Begin(Define.PlayerType target)
	{
		_battleTargetList.Clear();
		_battleTargetList.Add(target);
		_coroutine = StartCoroutine(OnlineStepDo());
	}
	/// <summary>
	/// 全員でのダンスバトル開始
	/// </summary>
	public void Begin(Define.PlayerType target1, Define.PlayerType target2)
	{
		_battleTargetList.Clear();
		_battleTargetList.Add(target1);
		_battleTargetList.Add(target2);
		_coroutine = StartCoroutine(OnlineStepDo());
	}

	/// <summary>
	/// ダンスバトルの結果
	/// </summary>
	public void DanceBattleResult(int battleResultState, int changeFunScore)
	{
		if (Player.IsMine)
		{
			// フィニッシュした事を通知
		}

		// ダンスバトルの結果を受け取った事を設定
		_isReceiveDanceBattleResult = true;
	}

	private void PhaseNone()
	{
		_phase = Phase.None;

		_isPlaing = false;
	}

	private void PhaseStart()
	{
		_phase = Phase.Start;
		// 追従してきたモブ全員のダンス開始イベントを呼び出す
		_player.MobManager
			.Mobs.Where(e => e.FllowTarget == PlayerType)
			.Select(e =>
			{
				e.OnBeginDance();
				return e;
			})
			.Count();

		// ダンスの振付時間を乱数で決定する
		_requestTime = _requestTime.Select(e => UnityEngine.Random.Range(PlayerManager.DANCE_TIME, PlayerManager.DANCE_TIME * PlayerManager.LEAN_COEFFICIENT)).ToArray();

		// 合計
		float sum = _requestTime.Sum();

		// 正規化
		_requestTime = _requestTime.Select(e => PlayerManager.DANCE_TIME * (e / sum)).ToArray();

		_state = DanceResultState.None;
		_dancePoint = 0;
		_danceFloor.enabled = true;
		_isPlaing = true;
		
		if (Player.IsMine)
		{
			DisplayManager.GetInstanceDisplayEvents<DanceEvents>()?.onDanceStart?.Invoke();
			_playercamera?.SetCameraMode(playercamera.CAMERAMODE.DANCE_INTRO);
		}
		// 隊列の角度の設定
		Player.RankAngleLeft = 2.0f;
		Player.RankAngleRight = 4.0f;
		Player.RankPosOffsetZ = -5.0f;
	}

	private void PhasePlay()
	{
		_phase = Phase.Play;

		if (Player.IsMine)
		{
			shakeparameter.ResetShakeParameter();
			shakeparameter.SetActive(true);
			AudioManager.PlaySE("Lets_dance_3");
		}

		_player.Animator.SetBool("PlayDance", true);
	}

	private void PhaseFinish()
	{
		_phase = Phase.Finish;
		var events = DisplayManager.GetInstanceDisplayEvents<DanceEvents>();
		events?.onDanceFinished?.Invoke();

		// ダンス終了後の状態を設定する
		SetDanceResultState();

		// 追従対処のモブ全員のダンス終了イベントを実行する
		_changeFanCountAtComplateDance = _player.MobManager
			.Mobs.Where(e => e.FllowTarget == PlayerType)
			.Select(e => 
			{
				e.OnEndDance(_state);
				return e;
			})
			.Count();

		if (Player.IsMine)
		{
			shakeparameter.ResetShakeParameter();
			shakeparameter.SetActive(false);
			switch (_state)
			{
				case DanceResultState.Clear:
				case DanceResultState.Win:
					events?.onDanceSuccess?.Invoke();
					AudioManager.PlaySE("ダンス成功");
					AudioManager.PlaySE("モブ歓声＿ダンス成功");
					break;
				case DanceResultState.Miss:
				case DanceResultState.Lose:
					events?.onDanceFailled?.Invoke();
					AudioManager.PlaySE("ダンス失敗");
					AudioManager.PlaySE("モブ歓声＿ダンス失敗");
					break;

			}
		}
	}

	//変化したファン情報の同期は、勝者が行う
	/// <summary>
	/// ダンスバトルで使用するフィニッシュフェーズ
	/// </summary>
	private void PhaseFinishOnline()
	{
		_phase = Phase.Finish;
		var events = DisplayManager.GetInstanceDisplayEvents<DanceEvents>();
		events?.onDanceFinished?.Invoke();

		// ダンス終了後の状態を設定する
		SetDanceResultState();

		// 追従対処のモブ全員のダンス終了イベントを実行する
		_changeFanCountAtComplateDance = _player.MobManager
			.Mobs.Where(e => e.FllowTarget == PlayerType)
			.Select(e =>
			{
				e.OnEndDance(_state);
				return e;
			})
			.Count();

		if (Player.IsMine)
		{
			shakeparameter.ResetShakeParameter();
			shakeparameter.SetActive(false);
			switch (_state)
			{
				case DanceResultState.Clear:
				case DanceResultState.Win:
					events?.onDanceSuccess?.Invoke();
					AudioManager.PlaySE("ダンス成功");
					AudioManager.PlaySE("モブ歓声＿ダンス成功");
					break;
				case DanceResultState.Miss:
				case DanceResultState.Lose:
					events?.onDanceFailled?.Invoke();
					AudioManager.PlaySE("ダンス失敗");
					AudioManager.PlaySE("モブ歓声＿ダンス失敗");
					break;

			}
		}
	}

	private void PhaseEnd()
	{
		_phase = Phase.End;

		if (Player.IsMine)
		{
			DisplayManager.GetInstanceDisplayEvents<DanceEvents>()?.onDanceEnd?.Invoke();
			_playercamera?.SetCameraMode(playercamera.CAMERAMODE.NORMAL);
			DisplayManager.Switch(DisplayManager.DisplayType.Move);
			shakeparameter.SetActive(true);
		}

		// 隊列の角度の設定
		Player.RankAngleLeft = 0.5f;
		Player.RankAngleRight = 1.5f;
		Player.RankPosOffsetZ = 0.0f;

		_danceFloor.enabled = false;
		// スコアを設定する
		_dancePoint = 0;
		_player.Animator.SetBool("PlayDance", false);
	}

	private void PhasePenetration()
	{
		_phase = Phase.Penetration;
	}

	private void ChangeFanPoint(int addValue)
	{
		if (_dancePoint >= PlayerManager.SHAKE_NORMA)
		{
			return;
		}
		_dancePoint += addValue;
		_dancePoint = Math.Max(_dancePoint, 0);

		if (!Player.IsMine)
			return;
		if (_dancePoint >= PlayerManager.SHAKE_NORMA)
		{
			DisplayManager.GetInstanceDisplayEvents<DanceEvents>()?.onRequestNolmaComplate?.Invoke();
		}
	}

	/// <summary>
	/// ダンス終了後の状態を設定する
	/// </summary>
	/// <remarks>
	/// 個人でのダンスで使用する
	/// </remarks>
	private void SetDanceResultState()
	{
		// 通常ダンス
		if (_dancePoint >= PlayerManager.SHAKE_NORMA)
		{
			_state = DanceResultState.Clear;
		}
		else
		{
			_state = DanceResultState.Miss;
		}
	}

	private void BattleResultCast()
	{
		// ランキングを求める(この処理は分離)
		List<DanceBattleRankingEx> sort = new List<DanceBattleRankingEx>();

		sort.Add(new DanceBattleRankingEx(PlayerType, DancePoint));
		foreach (var type in _battleTargetList)
		{
			int dancePoint = _player.PlayerManager.GetPlayer(type).Dance.DancePoint;
			sort.Add(new DanceBattleRankingEx(type, dancePoint));
		}
		sort = sort.OrderByDescending(e => e.dancePoint).ToList();

		// ランキングを設定
		for (int i = 0; i < sort.Count; i++)
		{
			sort[i].rank = i;
		}

		// スコアが重複していた場合、順位の高い方に合わせる

		// 引き分けかどうか

		//変化したファン情報の同期は、勝者が行う

		//二つのpun関数を用意
		//1:バトル結果送信


		//2:ファン変更処理送信
		
	}

	private void OnTriggerStay(Collider other)
	{
		if (other.tag != "DanceRange")
			return;

		if (IsPlaying)
			return;

		Dance playerDance = other.gameObject.GetComponent<Dance>();

		// 既にターゲットリストに登録されているかどうか
		if (_battleTargetList.Count(e => e == playerDance.PlayerType) != 0)
			return;

		// ターゲットに追加
		_battleTargetList.Add(playerDance.PlayerType);
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.tag != "DanceRange")
			return;

		if (IsPlaying)
			return;

		Dance playerDance = other.gameObject.GetComponent<Dance>();

		// ターゲットリストに未登録かどうか
		if (_battleTargetList.Count(e => e == playerDance.PlayerType) == 0)
			return;

		// ターゲットから除外する
		_battleTargetList.Remove(playerDance.PlayerType);
	}

	/// <summary>
	/// ダンス要求リクエストのステップ事に処理を実行する
	/// </summary>
	private IEnumerator StepDo()
	{
		PhaseStart();
		
		yield return new WaitForSeconds(5.0f);

		PhasePlay();

		for (int callCount = 0; callCount < PlayerManager.REQUEST_COUNT; callCount++)
		{
			_isRequestShake = !_isRequestShake;
			if(Player.IsMine)
			{
				if(_isRequestShake)
				{
					DisplayManager.GetInstanceDisplayEvents<DanceEvents>()?.onRequestShake?.Invoke();
				}
				else
				{
					DisplayManager.GetInstanceDisplayEvents<DanceEvents>()?.onRequestStop?.Invoke();
				}
			}
			yield return new WaitForSeconds(_requestTime[callCount]);
		}

		PhaseFinish();

		yield return new WaitForSeconds(3.0f);

		PhaseEnd();

		yield return null;

		PhaseNone();
	}

	//TODO:ダンスバトル用のPhaseFinishを非同期にしたStepDoを作成する
	private IEnumerator OnlineStepDo()
	{
		PhaseStart();

		yield return new WaitForSeconds(5.0f);

		PhasePlay();

		for (int callCount = 0; callCount < PlayerManager.REQUEST_COUNT; callCount++)
		{
			_isRequestShake = !_isRequestShake;
			if (Player.IsMine)
			{
				if (_isRequestShake)
				{
					DisplayManager.GetInstanceDisplayEvents<DanceEvents>()?.onRequestShake?.Invoke();
				}
				else
				{
					DisplayManager.GetInstanceDisplayEvents<DanceEvents>()?.onRequestStop?.Invoke();
				}
			}
			yield return new WaitForSeconds(_requestTime[callCount]);
		}

		// ここで代表一人がバトル結果の同期処理を送信する


		// バトル結果が通知されるまで待機
		while (!_isReceiveDanceBattleResult)
			yield return null;

		PhaseFinishOnline();

		yield return new WaitForSeconds(3.0f);

		PhaseEnd();

		yield return null;

		PhaseNone();
	}

	internal class DanceBattleRankingEx
	{
		public Define.PlayerType playerType;
		public int dancePoint;

		// ソート後に入力される
		public int rank;

		public DanceBattleRankingEx(Define.PlayerType playerType, int score)
		{
			this.playerType = playerType;
			this.dancePoint = score;
		}
	}
}
