using UnityEngine;
using WiimoteApi;

//TODO:プレイヤーをネットワーク対応する
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

	[SerializeField]
	private Rigidbody _rb;

	[SerializeField]
	private float _power;

	[SerializeField]
	private float _rotatePower;

	[SerializeField]
	private Animator _animator;

	[SerializeField]
	private Dance _dance;

	private PlayerManager _playerManager;

	private MobManager _mobManager;

	private cameramanager _cameramanager;

	/// <summary>
	/// PhotonNetwork.Instantiate によって GameObject(とその子供)が生成された際に呼び出されます。
	/// </summary>
	/// <remarks>
	/// PhotonMessageInfoパラメータは「誰が」「いつ」作成したかを提供します。
	/// (「いつ」は PhotonNetworking.time に基づきます。)
	/// </remarks>
	public override void OnPhotonInstantiate(PhotonMessageInfo info)
	{
		var caches = GameObject.Find("SystemObjects/BattleManager").GetComponent<PlayerGenerator>().Caches;
		_playerManager = caches.playerManager;
		_mobManager = caches.mobManager;
		_cameramanager = caches.cameramanager;

		// プレイヤーを管理クラスに登録
		_playerManager.SetPlayer(this);

		_type = (Define.PlayerType)(int)photonView.instantiationData[0];
		_playerColor = Define.playerColor[(int)_type];
		_dance.OnAwake();

		Debug.Log("生成受信データ player ID : " + ((int)photonView.instantiationData[0]).ToString() + "\n クライアントID : " + PhotonNetwork.player.ID.ToString());
		// プレイヤー自身だけに実行される処理
		if ((int)photonView.instantiationData[0] == PhotonNetwork.player.ID)
		{
			_cameramanager.SetFollowTarget(transform);
			_cameramanager.SetLookAtTarget(transform);
		}
	}

	void Update()
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
			if (Input.GetKeyDown("k") || WiimoteManager.GetButton(0, ButtonData.WMBUTTON_TWO))
				photonView.RPC("DanceBegin", PhotonTargets.AllViaServer);
		}
		else
		{
			if (Input.GetKeyDown("k") || WiimoteManager.GetButton(0, ButtonData.WMBUTTON_ONE))
				photonView.RPC("DanceCancel", PhotonTargets.AllViaServer);
		}

		// 移動量の減衰
		_rb.velocity -= _rb.velocity * 0.1f;
	}

	/// <summary>
	/// ダンス開始
	/// </summary>
	[PunRPC]
	public void DanceBegin()
	{
		if (!photonView.isMine)
			return;

		_dance.Begin();
	}

	/// <summary>
	/// ダンスキャンセル
	/// </summary>
	[PunRPC]
	public void DanceCancel()
	{
		if (!photonView.isMine)
			return;

		_dance.Cancel();
	}

	/// <summary>
	/// 定義のみ
	/// </summary>
	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) { }
}
