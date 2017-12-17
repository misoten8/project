﻿using System;
using UnityEngine;
using UniRx;
using WiimoteApi;
using System.Linq;
using System.Collections;

//TODO:乱入に対応する
/// <summary>
/// ダンス クラス
/// 製作者：実川
/// </summary>
public class Dance : MonoBehaviour
{
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

	public bool IsSuccess
	{
		get { return _isSuccess; }
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
	/// ダンス終了時実行イベント
	/// bool型引数 -> このダンスが中断されたかどうか
	/// </summary>
	public event Action<bool, bool> onEndDance;


	[SerializeField]
	private Player _player;

	/// <summary>
	/// ダンスの効果範囲の当たり判定
	/// </summary>
	[SerializeField]
	private SphereCollider _danceCollider;

	[SerializeField]
	private DanceUI _danceUI;

	[SerializeField]
	private MeshRenderer _danceFloor;

	private playercamera _playercamera;

	private Phase _phase = Phase.None;

	private int _dancePoint = 100;

	private bool _isSuccess = false;

	private bool _isRequestShake = false;

	/// <summary>
	/// 乱入している or されているかどうか
	/// </summary>
	private bool _isPenetrated = false;

	/// <summary>
	/// 処理中かどうか
	/// </summary>
	//private bool _isTransing = false;

	/// <summary>
	/// ダンス中かどうか
	/// </summary>
	private bool _isPlaing = false;

	/// <summary>
	/// 各リクエスト事の持続時間
	/// </summary>
	private float[] _requestTime = new float[PlayerManager.REQUEST_COUNT];

	/// <summary>
	/// playerに呼び出してもらう
	/// </summary>
	public void OnAwake(cameramanager playerCamera)
	{
		_playercamera = playerCamera;

		_danceCollider.enabled = true;
		_danceUI.OnAwake();
		_danceUI.NotActive();
		_dancePoint = 0;
	}

	void Update()
	{
		switch (_phase)
		{
			case Phase.Play:
				if (Input.GetKeyDown("return") || WiimoteManager.GetSwing(0))
				{
					ChangeFanPoint(_isRequestShake ? 1 : -1);
					ParticleManager.Play(_isRequestShake ? "DanceNowClear" : "DanceNowFailed", new Vector3(), transform);
				}
				_danceUI.SetPointUpdate(_dancePoint);
				break;
		}
	}

	/// <summary>
	/// ダンス開始
	/// </summary>
	public void Begin()
	{
		//if (Player.photonView.isMine)
		{
			shakeparameter.ResetShakeParameter();

			// ダンスの振付時間を乱数で決定する
			_requestTime = _requestTime.Select(e => UnityEngine.Random.Range(PlayerManager.DANCE_TIME, PlayerManager.DANCE_TIME * PlayerManager.LEAN_COEFFICIENT)).ToArray();

			// 合計
			float sum = _requestTime.Sum();

			// 正規化
			_requestTime = _requestTime.Select(e => PlayerManager.DANCE_TIME * (e / sum)).ToArray();

			_isTransing = false;
			_isSuccess = false;
			_dancePoint = 0;
			_danceUI.Active();
			_danceFloor.enabled = true;
			_isPlaing = true;
			_player.Animator.SetBool("PlayDance", true);

			_playercamera?.SetCameraMode(playercamera.CAMERAMODE.DANCE_INTRO);
			StartCoroutine("StepDo");
		}
	}

	/// <summary>
	/// ダンス終了
	/// </summary>
	public void End()
	{
		//if (Player.photonView.isMine)
		{
			onEndDance?.Invoke(false, IsSuccess);
			onEndDance = null;
			_danceUI.SetResult(IsSuccess);
			_isTransing = true;
			shakeparameter.ResetShakeParameter();
			StopCoroutine("StepDo");
			Observable
				.Timer(TimeSpan.FromSeconds(3))
				.Subscribe(_ =>
				{
					if (Player.photonView.isMine)
						shakeparameter.ResetShakeParameter();

					_isPlaing = false;
					_isTransing = false;
					_danceUI.NotActive();
					_danceFloor.enabled = false;
					// スコアを設定する
					_dancePoint = 0;
					_player.Animator.SetBool("PlayDance", false);
					_playercamera?.SetCameraMode(playercamera.CAMERAMODE.NORMAL);
				});
		}
	}

	/// <summary>
	/// ダンスを中断する
	/// </summary>
	public void Cancel()
	{
		//if (Player.photonView.isMine)
		{
			if (_phase == Phase.None)
				return;

			shakeparameter.ResetShakeParameter();

			onEndDance?.Invoke(true, IsSuccess);
			onEndDance = null;
			_danceUI.NotActive();
			_danceFloor.enabled = false;
			// スコアを設定する
			_dancePoint = 0;
			_player.Animator.SetBool("PlayDance", false);
			_playercamera?.SetCameraMode(playercamera.CAMERAMODE.NORMAL);
			StopCoroutine("StepDo");
			PhaseNone();
		}
	}

	/// <summary>
	/// ダンス乱入
	/// </summary>
	/// <returns>乱入できたかどうか</returns>
	public bool ToPenetration()
	{
		if (_isPenetrated)
			return false;
		if (_phase == Phase.None)
		{
			_isPenetrated = true;
			return true;
		}
		if (_phase == Phase.Start)
		{
			_isPenetrated = true;
			return true;
		}
		return false;
	}

	private void PhaseNone()
	{
		_phase = Phase.None;

		_isPlaing = false;
	}

	private void PhaseStart()
	{
		_phase = Phase.Start;

		shakeparameter.ResetShakeParameter();

		// ダンスの振付時間を乱数で決定する
		_requestTime = _requestTime.Select(e => UnityEngine.Random.Range(PlayerManager.DANCE_TIME, PlayerManager.DANCE_TIME * PlayerManager.LEAN_COEFFICIENT)).ToArray();

		// 合計
		float sum = _requestTime.Sum();

		// 正規化
		_requestTime = _requestTime.Select(e => PlayerManager.DANCE_TIME * (e / sum)).ToArray();

		_isSuccess = false;
		_dancePoint = 0;
		_danceUI.Active();
		_danceFloor.enabled = true;
		_isPlaing = true;
		_player.Animator.SetBool("PlayDance", true);

		_playercamera?.SetCameraMode(cameramanager.CAMERAMODE.DANCE_INTRO);
	}

	private void PhasePlay()
	{
		_phase = Phase.Play;
	}

	private void PhaseFinish()
	{
		_phase = Phase.Finish;
		onEndDance?.Invoke(false, IsSuccess);
		onEndDance = null;
		_danceUI.SetResult(IsSuccess);
		shakeparameter.ResetShakeParameter();
	}

	private void PhaseEnd()
	{
		_phase = Phase.End;

		if (Player.photonView.isMine)
			shakeparameter.ResetShakeParameter();

		_danceUI.NotActive();
		_danceFloor.enabled = false;
		// スコアを設定する
		_dancePoint = 0;
		_player.Animator.SetBool("PlayDance", false);
		_playercamera?.SetCameraMode(cameramanager.CAMERAMODE.NORMAL);
	}

	private void PhasePenetration()
	{
		_phase = Phase.Penetration;
	}

	private void ChangeFanPoint(int addValue)
	{
		_dancePoint += addValue;
		_danceUI.SetPointColor(addValue > 0 ? new Color(0.0f, 1.0f, 0.0f) : new Color(1.0f, 0.0f, 0.0f));
		if (_dancePoint >= PlayerManager.SHAKE_NORMA)
		{
			_isSuccess = true;
			_danceUI.SetPointColor(new Color(0.0f, 0.0f, 1.0f));
		}
	}

	/// <summary>
	/// ダンス要求リクエストのステップ事に処理を実行する
	/// </summary>
	private IEnumerator StepDo()
	{
		PhaseStart();
		
		yield return new WaitForSeconds(1.0f);

		PhasePlay();

		for (int callCount = 0; callCount < PlayerManager.REQUEST_COUNT; callCount++)
		{
			_isRequestShake = !_isRequestShake;
			_danceUI.SetRequestShake(_isRequestShake);
			yield return new WaitForSeconds(_requestTime[callCount]);
		}

		PhaseFinish();

		yield return new WaitForSeconds(3.0f);

		PhaseEnd();

		yield return null;

		PhaseNone();
	}
}
