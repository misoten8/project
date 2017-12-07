using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤー生成クラス
/// マスタークライアントにのみ存在する
/// </summary>
public class PlayerGenerator : Photon.MonoBehaviour
{
	/// <summary>
	///　初期化時に渡すキャッシュクラス
	/// </summary>
	public struct PlayerCaches
	{
		public MobManager mobManager;
		public PlayerManager playerManager;
		public cameramanager cameramanager;
	}

	public PlayerCaches Caches
	{
		get { return _caches; }
	}

	private PlayerCaches _caches;

	private void OnEnable()
	{
		_caches.mobManager = GetComponent<MobManager>();
		_caches.playerManager = GetComponent<PlayerManager>();
		_caches.cameramanager = GameObject.Find("Cameras/cameramanager").GetComponent<cameramanager>();

		if (!PhotonNetwork.isMasterClient)
			return;

		// 参加ユーザー分生成する
		foreach (var photonPlayer in PhotonNetwork.playerList)
		{
			object[] data = new object[] { photonPlayer.ID };
			Player player = PhotonNetwork.InstantiateSceneObject("Prefabs/Player", Vector3.zero, Quaternion.identity, 0, data).GetComponent<Player>();
			player.photonView.TransferOwnership(photonPlayer);
		}
	}

	/// <summary>
	/// 定義のみ
	/// </summary>
	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) { }
}