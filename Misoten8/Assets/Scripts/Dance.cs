using System;
using UnityEngine;
using UniRx;
using WiimoteApi;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// ダンス クラス
/// 製作者：実川
/// </summary>
public class Dance : MonoBehaviour
{
	public Define.PlayerType PlayerType
	{
		get { return _player.Type; }
	}

	/// <summary>
	/// ダンス中かどうか
	/// </summary>
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

	[SerializeField]
	private Player _player;

	/// <summary>
	/// ダンス終了時実行イベント
	/// bool型引数 -> このダンスが中断されたかどうか
	/// </summary>
	public event Action<bool, bool> onEndDance;

	/// <summary>
	/// ダンスの効果範囲の当たり判定
	/// </summary>
	[SerializeField]
	private SphereCollider _danceCollider;

	[SerializeField]
	private DanceUI _danceUI;

	[SerializeField]
	private MeshRenderer _danceFloor;

	private cameramanager _cameramanager;

	private int _dancePoint = 100;

	private bool _isSuccess = false;

	private bool _isRequestShake = false;

	/// <summary>
	/// 処理中かどうか
	/// </summary>
	private bool _isTransing = false;

	/// <summary>
	/// ダンス中かどうか
	/// </summary>
	private bool _isPlaing = false;

	// wiiリモコン
	private int _wmNum;

	/// <summary>
	/// 各リクエスト事の持続時間
	/// </summary>
	private float[] _requestTime = new float[PlayerManager.REQUEST_COUNT];

	/// <summary>
	/// playerに呼び出してもらう
	/// </summary>
	public void OnAwake()
	{
		_cameramanager = GameObject.Find("Cameras/cameramanager").GetComponent<cameramanager>();
		_danceCollider.enabled = true;
		_danceUI.OnAwake();
		_danceUI.NotActive();
		_dancePoint = 0;

		_wmNum = (int)_player.Type - 1;
	}

	void Update()
	{
		if (IsPlaying)
		{
			if (_isTransing)
				return;

			if (Input.GetKeyDown("return") || WiimoteManager.GetSwing(_wmNum))
			{
				ChangeFanPoint(_isRequestShake ? 1 : -1);
				ParticleManager.Play(_isRequestShake ? "DanceNowClear" : "DanceNowFailed", new Vector3(), transform);
			}
			_danceUI.SetPointUpdate(_dancePoint);
		}
	}

	/// <summary>
	/// ダンス開始
	/// </summary>
	public void Begin()
	{
		// ダンスの振付時間を乱数で決定する
		_requestTime = _requestTime.Select(e => UnityEngine.Random.Range(PlayerManager.DANCE_TIME, PlayerManager.DANCE_TIME * 3)).ToArray();

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
		_cameramanager?.ChangeCameraMode();
		StartCoroutine("StepDo");
	}

	/// <summary>
	/// ダンス終了
	/// </summary>
	public void End()
	{
		onEndDance?.Invoke(false, IsSuccess);
		onEndDance = null;
		_danceUI.SetResult(IsSuccess);
		_isTransing = true;
		_isPlaing = false;
		StopCoroutine("StepDo");
		Observable
			.Timer(TimeSpan.FromSeconds(3))
			.Subscribe(_ =>
			{
				_isTransing = false;
				_danceUI.NotActive();
				_danceFloor.enabled = false;
				// スコアを設定する
				_dancePoint = 0;
				_cameramanager?.ChangeCameraMode();
			});
	}

	/// <summary>
	/// ダンスを中断する
	/// </summary>
	public void Cancel()
	{
		if (_isTransing)
			return;

		onEndDance?.Invoke(true, IsSuccess);
		onEndDance = null;
		_isPlaing = false;
		_isTransing = false;
		_danceUI.NotActive();
		_danceFloor.enabled = false;
		// スコアを設定する
		_dancePoint = 0;
		_cameramanager?.ChangeCameraMode();
		StopCoroutine("StepDo");
	}

	private void ChangeFanPoint(int addValue)
	{
		_dancePoint += addValue;
		_danceUI.SetPointColor(addValue > 0 ? new Color(0.0f, 1.0f, 0.0f) : new Color(1.0f, 0.0f, 0.0f));
		if (_dancePoint >= 30)
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
		yield return new WaitForSeconds(1.0f);
		_isRequestShake = true;
		_danceUI.SetRequestShake(_isRequestShake);

		yield return new WaitForSeconds(_requestTime[0]);
		_isRequestShake = false;
		_danceUI.SetRequestShake(_isRequestShake);

		yield return new WaitForSeconds(_requestTime[1]);
		_isRequestShake = true;
		_danceUI.SetRequestShake(_isRequestShake);

		yield return new WaitForSeconds(_requestTime[2]);
		End();

		yield return null;
	}
}
