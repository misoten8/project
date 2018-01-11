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
		shakeparameter.SetActive(false);
		AudioManager.PlayBGM("リザルト");
        DisplayManager.Instance.onFadedIn += () =>
		{
			StartCoroutine(StepDo());
		};

    }
    [SerializeField]
	private ResultSceneCache _sceneCache;

    [SerializeField]
    private resultcamera _resultCamera;

    [SerializeField]
    private ResultRanking _resultRanking;

	/// <summary>
	/// 派生クラスのインスタンスを取得
	/// </summary>
	protected override ResultScene GetOverrideInstance()
	{
		return this;
	}

	public void TransScene()
	{
		SceneManager.LoadScene("Title");
	}

	private IEnumerator StepDo()
	{
		var events = DisplayManager.GetInstanceDisplayEvents<ResultEvents>();

		events?.onPlayWinnerPanel?.Invoke();

		yield return new WaitForSeconds(3.0f);
        _resultCamera.SetCameraMode(resultcamera.CAMERAMODE.RESULTS_ANNOUNCE2);
        _resultCamera.SetAnnounceTarget(null);

		yield return new WaitForSeconds(3.0f);

		events?.onOpneScorePanel?.Invoke();

		yield return new WaitForSeconds(6.0f);

		events?.onTransTitleReady?.Invoke();

		yield return new WaitForSeconds(3.0f);

		Switch(SceneType.Title);
	}
}
