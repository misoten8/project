using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
///リザルトシーン管理クラス
/// </summary>
[RequireComponent(typeof(ResultSceneCache))]
public class ResultScene : SceneBase<ResultScene>
{
	/// <summary>
	/// 外部シーンが利用できるデータキャッシュ
	/// </summary>
	public override ISceneCache SceneCache
	{
		get { return _sceneCache; }
	}

	[SerializeField]
	private ResultSceneCache _sceneCache;

	/// <summary>
	/// 派生クラスのインスタンスを取得
	/// </summary>
	protected override ResultScene GetOverrideInstance()
	{
		return this;
	}

	void Update ()
	{
		if (shakeparameter.IsCompareWithValue(2))
		{
			Switch(SceneType.Title);
		}
	}

	private void OnGUI()
	{
		GUI.Label(new Rect(new Vector2(0,0), new Vector2(300, 200)), "Result Scene");
	}

	public void TransScene()
	{
		SceneManager.LoadScene("Title");
	}
}
