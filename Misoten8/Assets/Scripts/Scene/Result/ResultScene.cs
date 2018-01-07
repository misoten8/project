using System.Collections;
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
    private void Start()
    {
        AudioManager.PlayBGM("リザルト");

		DisplayManager.Instance.onFadedIn += () =>
		{
			StartCoroutine(StepDo());
		};
    }
    [SerializeField]
	private ResultSceneCache _sceneCache;

	/// <summary>
	/// タイトル遷移が可能かどうか
	/// </summary>
	private bool _isTransTitle = false;

	/// <summary>
	/// 派生クラスのインスタンスを取得
	/// </summary>
	protected override ResultScene GetOverrideInstance()
	{
		return this;
	}

	void Update ()
	{
		if (!_isTransTitle)
			return;

		if (shakeparameter.IsOverWithValue(Define.SCENE_TRANCE_VALUE))
		{
            AudioManager.PlaySE("決定１");
            Switch(SceneType.Title);
		}
	}

	public void TransScene()
	{
		SceneManager.LoadScene("Title");
	}

	private IEnumerator StepDo()
	{
		shakeparameter.SetActive(false);

		var events = DisplayManager.GetInstanceDisplayEvents<ResultEvents>();

		events?.onPlayWinnerPanel?.Invoke();

		yield return new WaitForSeconds(3.0f);

		events?.onOpneScorePanel?.Invoke();

		yield return new WaitForSeconds(3.0f);

		// 入力操作受付開始
		_isTransTitle = true;
		shakeparameter.SetActive(true);
		events?.onTransTitleReady?.Invoke();

		yield return null;
	}
}
