using UnityEngine;
using WiimoteApi;

/// <summary>
/// タイトルシーン管理クラス
/// </summary>
[RequireComponent(typeof(TitleSceneCache))]
public class TitleScene : SceneBase<TitleScene>
{
	/// <summary>
	/// 外部シーンが利用できるデータキャッシュ
	/// </summary>
	public override ISceneCache SceneCache
	{
		get { return _sceneCache; }
	}

	[SerializeField]
	private TitleSceneCache _sceneCache;

	/// <summary>
	/// 派生クラスのインスタンスを取得
	/// </summary>
	protected override TitleScene GetOverrideInstance()
	{
		return this;
	}

	void Update()
	{
		if (shakeparameter.IsCompareWithValue(2))
		{
			Switch(SceneType.Lobby);
		}
	}

	private void OnGUI()
	{
		GUI.Label(new Rect(new Vector2(0, 0), new Vector2(300, 200)), "Title Scene");
	}
}
