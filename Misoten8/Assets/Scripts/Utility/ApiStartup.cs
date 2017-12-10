using UnityEngine;
using Misoten8Utility;

/// <summary>
/// アプリケーション起動時実行クラス
/// </summary>
public class ApiStartup : MonoBehaviour 
{
	/// <summary>
	/// アプリケーション起動時実行イベント
	/// </summary>
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void OnApiStartup()
	{
		Resources.LoadAll("Prefabs/ApiStartupObjects")?.Foreach(e => Instantiate(e));
	}
}