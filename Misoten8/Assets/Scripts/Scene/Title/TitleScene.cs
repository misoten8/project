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
    private void Start()
    {
		Define.JoinBattlePlayerNum = 0;
        AudioManager.PlayBGM("タイトル");
    }
    void Update()
	{
		if (shakeparameter.IsOverWithValue(Define.SCENE_TRANCE_VALUE))
		{
            AudioManager.PlaySE("決定１");
            Switch(SceneType.Lobby);
		}
	}
}
