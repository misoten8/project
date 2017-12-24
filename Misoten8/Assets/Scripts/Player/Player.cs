using UnityEngine;
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

	[SerializeField]
	private Rigidbody _rb;

	[SerializeField]
	private float _power;

	[SerializeField]
	private float _rotatePower;

	[SerializeField]
	private Dance _dance;

	[SerializeField]
	private Transform _modelPlaceObject;

	private PlayerManager _playerManager;

	private MobManager _mobManager;

	private playercamera _playercamera;


	private bool canPlayDance = true;

    // ファン追従用オブジェ
    public Transform TargetObj
    {
        get { return _targetObj; }
    }
    [SerializeField]
    private Transform _targetObj;

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
		_playerManager = caches.playerManager;
		_mobManager = caches.mobManager;
		_playercamera = caches.playercamera;
        _targetObj.localPosition = new Vector3( _targetObj.localPosition.x, _targetObj.localPosition.y,-_targetObj.localPosition.z);
		// プレイヤーを管理クラスに登録
		_playerManager.SetPlayer(this);

		_type = (Define.PlayerType)(int)photonView.instantiationData[0];
        WiimoteManager.Wiimotes[0].SetLED((int)_type);
		_playerColor = Define.playerColor[(int)_type];
		_dance.OnAwake(_playercamera);
		Debug.Log("生成受信データ player ID : " + ((int)photonView.instantiationData[0]).ToString() + "\n クライアントID : " + PhotonNetwork.player.ID.ToString());
		
		// モデルの設定
		_model = Instantiate(ModelManager.GetCache(PlayerManager.MODEL_MAP[_type]));
		_model.transform.SetParent(_modelPlaceObject);
		_animator = _model.GetComponent<Animator>();
		var playerAnimEvent = _model.GetComponent<PlayerAnimEvent>();
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
		if (!photonView.isMine)
			return;

		if (!_dance.IsPlaying)
		{
			if (Input.GetKey("up") || WiimoteManager.GetButton(0, ButtonData.WMBUTTON_RIGHT))
				_rb.AddForce(transform.forward * _power);
			if (Input.GetKey("left") || WiimoteManager.GetButton(0, ButtonData.WMBUTTON_UP))
				transform.Rotate(Vector3.up, -_rotatePower);
			if (Input.GetKey("right") || WiimoteManager.GetButton(0, ButtonData.WMBUTTON_DOWN))
				transform.Rotate(Vector3.up, _rotatePower);
			if (Input.GetKey("down") || WiimoteManager.GetButton(0, ButtonData.WMBUTTON_LEFT))
				_rb.AddForce(-transform.forward * _power);
			if (shakeparameter.IsOverWithValue(2))
			{
                _playercamera.SetDollyPosition(transform);//ドリーの位置設定
                photonView.RPC("DanceBegin", PhotonTargets.AllViaServer, (byte)_type);
				shakeparameter.ResetShakeParameter();
			}
		}

		Vector3 velocity = _rb.velocity;

		_animator.SetFloat("Velocity", Mathf.Abs(velocity.x) + Mathf.Abs(velocity.z));

		// 移動量の減衰
		_rb.velocity -= velocity * 0.1f;

    }

	/// <summary>
	/// ダンス開始
	/// </summary>
	[PunRPC]
	public void DanceBegin(byte playerType)
	{
		if (_type == (Define.PlayerType)playerType)
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
		if (_type == (Define.PlayerType)playerType)
		{
			_dance.Shake();
		}
	}

	/// <summary>
	/// 定義のみ
	/// </summary>
	private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) { }
}
