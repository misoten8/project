using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// パーティクル管理
/// 製作者：実川
/// </summary>
public class ParticleManager : SingletonMonoBehaviour<ParticleManager>
{
	/// <summary>
	/// パーティクルプレハブリスト
	/// </summary>
	List<GameObject> m_prefabList = new List<GameObject>();

	/// <summary>
	/// パーティクルインスタンスリスト
	/// </summary>
	List<GameObject> m_instanceList = new List<GameObject>();

	void Start()
	{
		Load();
	}

	/// <summary>
	/// 使用するパーティクルプレハブを事前読み込みを行う
	/// </summary>
	public static void Load()
	{
		GameObject[] array = Resources.LoadAll<GameObject>("Prefabs/Particles");

		foreach (GameObject element in array)
		{
			Instance.m_prefabList.Add(element);
		}
	}

	/// <summary>
	/// パーティクルを再生する
	/// </summary>
	/// <param name="fileName">パーティクルプレハブ名</param>
	/// <param name="pos">座標</param>
	/// <param name="parent">親</param>
	public static GameObject Play(string fileName, Vector3 pos = new Vector3(), Transform parent = null)
	{
		foreach (GameObject element in Instance.m_prefabList)
		{
			if (element.name != fileName)
				continue;

			GameObject instance;

			if (parent == null)
			{
				// ワールド座標
				instance = Instantiate(element);
				instance.transform.position = pos;
			}
			else
			{
				// 相対座標
				instance = Instantiate(element, parent);
				instance.transform.localPosition = pos;
			}

			Instance.m_instanceList.Add(instance);
			return instance;
		}
		return null;
	}

	/// <summary>
	/// 現在インスタンスされているパーティクルを全て消去します。
	/// </summary>
	public static void ResetAll()
	{
		if (Instance.m_instanceList.Count == 0)
			return;

		foreach (GameObject element in Instance.m_instanceList)
		{
			Destroy(element);
		}
		Instance.m_instanceList.Clear();
	}
}
