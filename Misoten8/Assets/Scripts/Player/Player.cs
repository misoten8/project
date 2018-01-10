﻿using UnityEngine;
using WiimoteApi;
using System.Collections;

/// <summary>
/// Player クラス
/// 製作者：実川
/// </summary>
public class Player : Photon.PunBehaviour
{
	public Define.PlayerType Type
	{
		get { return _type; }
	}

	private Define.PlayerType _type;

	public Color PlayerColor
	{
		get { return _playerColor; }
	}

	private Color _playerColor;

	/// <summary>
	/// アニメーション処理
	/// </summary>
	public Animator Animator
	{
		get { return _animator; }
	}
	[SerializeField]
	private Animator _animator;

	/// <summary>
	/// このプレイヤーがクライアント自身かどうか(PhotonView.isMineを使用しないこと)
	/// </summary>
	/// <remarks>
	/// PhotonView.isMineを利用しないのは、このプレイヤーの初期化処理で所有権の変更を行うため、
	/// 変更されるまでタイムラグがあり、isMineである確証が得られないため。
	/// </remarks>
	public bool IsMine
	{
		get { return _isMine; }
	}

	private bool _isMine = false;

	/// <summary>
	/// プレイヤーのモデルオブジェクト
	/// </summary>
	/// <remarks>
	/// Player1,Player2等のプレハブと同等のオブジェクトを受け取る事ができます
	/// </remarks>
	public GameObject Model
	{
		get { return _model; }
	}

	private GameObject _model = null;

	public Dance Dance
	{
		get { return _dance; }
	}

	/// <summary>
	/// ファンの人数
	/// </summary>
	public int FanCount
	{
		get { return _fanCount; }
		set { _fanCount = value; }
	}

	private int _fanCount = 0;

	public float RankAngleLeft
	{
		get { return _rankAngleLeft; }
		set { _rankAngleLeft = value; }
	}

	private float _rankAngleLeft = 0.5f;

	public float RankAngleRight
	{
		get { return _rankAngleRight; }
		set { _rankAngleRight = value; }
	}

	private float _rankAngleRight = 1.0f;

	public float RankPosOffsetZ
	{
		get { return _rankPosOffsetZ; }
		set { _rankPosOffsetZ = value; }
	}

	[SerializeField]
	private float _rankPosOffsetZ = 0.0f;

	public MobManager MobManager
	{
		get { return _mobManager; }
	}

	private MobManager _mobManager;

	public BattleScene BattleScene
	{
		get { return _battleScene; }
	}

	private BattleScene _battleScene;

	[SerializeField]
	private float _rotatePower;

	[SerializeField]
	private Dance _dance;

	[SerializeField]
	private Transform _modelPlaceObject;

	[SerializeField]
	private PlayerBillboard _billboard;

	private PlayerManager _playerManager;

	private playercamera _playercamera;

	private bool canPlayDance = true;

    public Transform TargetForward
    {
        get { return _targetForward; }
    }
    [SerializeField]
    private Transform _targetForward;

	public Transform TargetBack
	{
		get { return _targetBack; }
	}
	[SerializeField]
	private Transform _targetBack;

	private string _animNameMoveState = "MoveState";

	/// <summary>
	/// PhotonNetwork.Instantiate によって GameObject(とその子供)が生成された際に呼び出されます。
	/// </summary>
	/// <remarks>
	/// PhotonMessageInfoパラメータは「誰が」「いつ」作成したかを提供します。
	/// (「いつ」は PhotonNetworking.time に基づきます。)
	/// </remarks>
	public override void OnPhotonInstantiate(PhotonMessageInfo info)
	{
		_isMine = (int)photonView.instantiationData[0] == PhotonNetwork.player.ID;

		var caches = GameObject.Find("SystemObjects/BattleManager").GetComponent<PlayerGenerator>().Caches;
		_battleScene = caches.battleScene;
		_playerManager = caches.playerManager;
		_mobManager = caches.mobManager;
		_playercamera = caches.playercamera;

		// プレイヤーを管理クラスに登録
		_playerManager.SetPlayer(this);

		_type = Define.ConvertToPlayerType((int)photonView.instantiationData[0]);
        
		_playerColor = Define.playerColor[(int)_type];
		_dance.OnAwake(_playercamera);
		Define.JoinBattlePlayerNum++;
		_billboard.OnAwake(_playercamera.CameraBrain?.transform, this);

		Debug.Log("生成受信データ player ID : " + ((int)photonView.instantiationData[0]).ToString() + "\n クライアントID : " + PhotonNetwork.player.ID.ToString());
		
		// モデルの設定
		_model = Instantiate(ModelManager.GetCache(PlayerManager.MODEL_MAP[_type]));
		_model.transform.SetParent(_modelPlaceObject);
		_animator.avatar = _model.GetComponent<Animator>().avatar;
		_animator.runtimeAnimatorController = _model.GetComponent<Animator>().runtimeAnimatorController;
		var playerAnimEvent = GetComponent<PlayerAnimEvent>();
		if(playerAnimEvent == null)
		{
			Debug.LogWarning("プレイヤーのアニメーションイベントクラスを取得できませんでした。");
		}
		else
		{
			playerAnimEvent.Player = this;
		}
		// プレイヤー自身だけに実行される処理
		if (_isMine)
		{
			WiimoteManager.SetLED((int)_type);
			_playercamera.SetFollowTarget(transform);
			_playercamera.SetLookAtTarget(transform);
			StartCoroutine(WaitOnFrame());
		}
	}

	private IEnumerator WaitOnFrame()
	{
		yield return null;
		// カメラのプレイヤー設定処理と重なると動作しないため、１フレーム遅らせる
		_playercamera.SetCameraMode(playercamera.CAMERAMODE.NORMAL);
	}

	private void Update()
	{
		if (!_battleScene.IsBattleTime)
		{
			var currentAnimState = _animator.GetInteger(_animNameMoveState);
			if (currentAnimState != 0)
				_animator.SetInteger(_animNameMoveState, 0);
			return;
		}

		if (!photonView.isMine)
			return;

		if (!_dance.IsPlaying)
		{
			int moveState = 0;

			if (Input.GetKey(KeyCode.UpArrow) || WiimoteManager.GetButton(0, ButtonData.WMBUTTON_RIGHT))
				moveState = 1;
			if (Input.GetKey(KeyCode.LeftArrow) || WiimoteManager.GetButton(0, ButtonData.WMBUTTON_UP))
				transform.Rotate(Vector3.up, -_rotatePower);
			if (Input.GetKey(KeyCode.RightArrow) || WiimoteManager.GetButton(0, ButtonData.WMBUTTON_DOWN))
				transform.Rotate(Vector3.up, _rotatePower);
			if (Input.GetKey(KeyCode.DownArrow) || WiimoteManager.GetButton(0, ButtonData.WMBUTTON_LEFT))
				moveState = 2;
			if (shakeparameter.IsOverWithValue(PlayerManager.DANCE_START_SHAKE_COUNT))
			{
				moveState = 0;
				if (_animator.GetInteger(_animNameMoveState) != moveState)
				{
					_animator.SetInteger(_animNameMoveState, moveState);
				}
				DisplayManager.GetInstanceDisplayEvents<MoveEvents>()?.onDanceGaugeMax?.Invoke();

				_playercamera.SetDollyPosition(transform);//ドリーの位置設定
                photonView.RPC("DanceBegin", PhotonTargets.AllViaServer, (byte)_type);
				shakeparameter.ResetShakeParameter();
				DisplayManager.Switch(DisplayManager.DisplayType.Dance);
			}
			else
			{
				if (_animator.GetInteger(_animNameMoveState) != moveState)
				{
					_animator.SetInteger(_animNameMoveState, moveState);
				}
			}
		}
    }

	/// <summary>
	/// ダンス開始
	/// </summary>
	[PunRPC]
	public void DanceBegin(byte playerType)
	{
		if (_type == Define.ConvertToPlayerType(playerType))
		{
			_dance.Begin();
		}
	}

	/// <summary>
	/// 振った時の処理
	/// </summary>
	[PunRPC]
	public void DanceShake(byte playerType)
	{
		if (_type == Define.ConvertToPlayerType(playerType))
		{
			_dance.Shake();
		}
	}

	/// <summary>
	/// 定義のみ
	/// </summary>
	private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) { }
}
