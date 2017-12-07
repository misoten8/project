using System.Collections;
using UnityEngine;
using Misoten8Utility;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// モブキャラ生成 クラス
/// 製作者：実川
/// </summary>
public class MobGenerator : Photon.MonoBehaviour
{
	/// <summary>
	/// スコア
	/// </summary>
	[SerializeField]
	private Score _score;

	/// <summary>
	/// 人のプレハブ
	/// </summary>
	[SerializeField]
	private GameObject[] _peplePrefab;

	/// <summary>
	/// 生成数
	/// </summary>
	[SerializeField]
	private int _createNum;

	/// <summary>
	/// 1フレームで生成する数
	/// </summary>
	[SerializeField]
	private int _frameCreateNum;

	/// <summary>
	/// 生成範囲
	/// </summary>
	[SerializeField]
	private Vector2 _rangeSize;

	/// <summary>
	///　初期化時に渡すキャッシュクラス
	/// </summary>
	public struct MobCaches
	{
		public MobManager mobManager;
		public PlayerManager playerManager;
	}

	public MobCaches Caches
	{
		get { return _caches; }
	}

	private MobCaches _caches;

	private void OnEnable()
	{
		_caches.mobManager = GetComponent<MobManager>();
		_caches.playerManager = GetComponent<PlayerManager>();

		if (!PhotonNetwork.isMasterClient)
			return;

		StartCoroutine(Enumerator());
	}

	private void Create()
	{
		PhotonNetwork.InstantiateSceneObject(
			"Prefabs/Mobs/" + _peplePrefab[Random.Range(0, 3)].name, 
			transform.position + new Vector3(Random.Range(-_rangeSize.x, _rangeSize.x),
			0, 
			Random.Range(-_rangeSize.y, _rangeSize.y)), 
			Quaternion.identity, 
			0, 
			null);
	}

	private IEnumerator Enumerator()
	{
		if (_frameCreateNum.IsOutRange(0, _createNum))
		{
			Debug.LogWarning("1フレームで生成する数が異常です。処理を中断します\n　生成数：" + _frameCreateNum.ToString());
			yield break;
		}

		for (int i = 0; i < _createNum; i += _frameCreateNum)
		{
			_frameCreateNum.Loop(x => Create());
			yield return null;
		}
	}

	/// <summary>
	/// 定義のみ
	/// </summary>
	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) { }
}
