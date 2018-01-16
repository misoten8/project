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
		string text = StramingAssetsReader.Read("configFile.txt");
		if (text != null)
		{
			var config = JsonUtility.FromJson<Config>(text);
			if (config != null)
			{
				Define.config = config;
				Debug.Log("offlineMode:" + Define.config.offlineMode.ToString() + "\nplayerNum:" + Define.config.playerNum.ToString());
			}
			else
			{
				Debug.LogError("解析エラー");
			}
		}
		else
		{
			Debug.LogError("読み込みエラー");
		}

		Resources.LoadAll("Prefabs/ApiStartupObjects")?.Foreach(e => Instantiate(e));
	}
}