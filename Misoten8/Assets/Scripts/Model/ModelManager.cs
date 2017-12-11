using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// モデルのキャッシュを管理するクラス
/// </summary>
/// <remarks>
/// ２つ以上のシーンで登場したり、動的にインスタンスする場合に有効です
/// </remarks>
public class ModelManager : SingletonMonoBehaviour<ModelManager> 
{
	/// <summary>
	/// モデルの種類
	/// </summary>
	public enum ModelType
	{
		None = 0,
		Player1,
		Player2,
		Player3,
		/// <summary>
		/// 実況モデル
		/// </summary>
		Nav,
		Mob1
	}

	private Dictionary<ModelType, GameObject> _modelCaches = new Dictionary<ModelType, GameObject>();

	/// <summary>
	/// モデルのファイルパス紐付けマップ
	/// </summary>
	private static readonly Dictionary<ModelType, string> _MODEL_DIRECTORY = new Dictionary<ModelType, string>
	{
		{ ModelType.Player1, "Models/Player1" },
		{ ModelType.Player2, "Models/player2" },
		{ ModelType.Player3, "Models/player3" },
		{ ModelType.Mob1, "Prefabs/Players/NPC" }
	};

	/// <summary>
	/// 指定したモデルのキャッシュを取得する
	/// </summary>
	public static GameObject GetCache(ModelType type)
	{
		return Instance?._modelCaches[type];
	}

	/// <summary>
	/// モデルのリソース読み込み
	/// </summary>
	public IEnumerator Load()
	{
		foreach(var map in _MODEL_DIRECTORY)
		{
			ResourceRequest resReq = Resources.LoadAsync<GameObject>(map.Value);

			// 読み込み待ち
			while (!resReq.isDone)
				yield return null;

			GameObject cache = resReq?.asset as GameObject;

			if (cache == null)
			{
				Debug.LogWarning("モデルファイルの読み込みに失敗しました。ファイルパス：" + map.Value);
				continue;
			}

			Instance?._modelCaches.Add(map.Key, cache);

			yield return null;
		}
		Debug.Log("モデル読み込み完了");
	}

	void Start () 
	{
		StartCoroutine(Load());
	}
}