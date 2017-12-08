using UnityEngine;

/// <summary>
/// シングルトンクラス
/// 製作者：実川
/// </summary>
public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : SingletonMonoBehaviour<T>
{
	public static T Instance
	{
		get { return _instance; }
	}

	private static T _instance = null;

	private void Awake()
	{
		if (_instance == null)
		{
			_instance = this as T;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}
}