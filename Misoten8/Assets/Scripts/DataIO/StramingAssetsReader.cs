using UnityEngine;

/// <summary>
/// StramingAssetsからファイルデータを読み込むクラス
/// StramingAssetsは読み込み専用のため、読み込み用の処理のみ実装
/// </summary>
public class StramingAssetsReader
{
	/// <summary>
	/// StreamingAssetsからの読み込み、WWWを使用
	/// </summary>
	/// <param name="fileName">拡張子付きのファイル名</param>
	/// <returns></returns>
	public static string Read(string fileName)
	{
		string fullPath = StramingAssetsReader.GetFilePath() + fileName;
		WWW www = new WWW(fullPath);
		//終わるまで待機。非同期で読み込む際はyieldを使用
		while (!www.isDone)
		{ }
		string readText = www.text;
		//必要ならBOMなしに変換
		if (HasBomWithText(www.bytes))
			readText = GetDeletedBomText(www.text);
		return readText;
	}

	/// <summary>
	/// BOM有無の判定をする関数
	/// </summary>
	/// <param name="bytes">byte型に変換した文字列</param>
	/// <returns>BOMがあるかどうか</returns>
	static bool HasBomWithText(byte[] bytes)
	{
		return bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF;
	}

	/// <summary>
	/// BOMを消す関数
	/// </summary>
	/// <param name="text"></param>
	/// <returns>BOMを消去した文字列</returns>
	static string GetDeletedBomText(string text)
	{
		return text.Remove(0, 1);
	}

	/// <summary>
	/// プラットフォーム毎のStreamingAssetsのパスを返す
	/// </summary>
	/// <returns>パス</returns>
	static string GetFilePath()
	{
#if UNITY_EDITOR
        return "file:///" + Application.dataPath + "/StreamingAssets/";
#elif UNITY_IPHONE || UNITY_ANDROID
      return "jar:file://" + Application.dataPath + "!/assets" + "/";
#elif UNITY_WINDOWS
		return "file:///" + Application.dataPath + "/StreamingAssets/";
#endif
	}
}