using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

/// <summary>
/// ディスプレイ管理クラス
/// </summary>
/// <remarks>
/// ディスプレイの切り替え及び、ディスプレイのイベントの取得はこのクラスを参照してください
/// シーンクラスに一部処理を依存しています
/// </remarks>
public class DisplayManager : SingletonMonoBehaviour<DisplayManager>
{
	/// <summary>
	/// 現在選択されているディスプレイの種類
	/// </summary>
	public static DisplayType CurrentDisplayType
	{
		get { return Instance._currentDisplayType; }
	}

	private DisplayType _currentDisplayType = DisplayType.None;

	/// <summary>
	/// ディスプレイ切り替え中かどうか
	/// </summary>
	public static bool IsSwitching
	{
		get { return Instance._isSwitching || Instance._switchStack != 0; }
	}

	private bool _isSwitching = false;

	/// <summary>
	/// ディスプレイの種類
	/// </summary>
	public enum DisplayType
	{
		/// <summary>
		/// ディスプレイなし
		/// </summary>
		None,
		Title,
		Lobby,
		Battle,
		Move,
		Dance,
		Result
	}

	/// <summary>
	/// フェードインアニメーション終了時実行イベント
	/// </summary>
	public event Action onFadedIn;

	/// <summary>
	/// フェードアウトアニメーション終了時実行イベント
	/// </summary>
	public event Action onFadedOut;

	/// <summary>
	/// ディスプレイタイプとシーンの紐付けマップ
	/// </summary>
	private static readonly Dictionary<DisplayType, string> _DISPLAY_MAP =
		new Dictionary<DisplayType, string>
	{
		{ DisplayType.None, "None" },
		{ DisplayType.Title, "TitleDisplay" },
		{ DisplayType.Lobby, "LobbyDisplay" },
		{ DisplayType.Move, "MoveDisplay" },
		{ DisplayType.Dance, "DanceDisplay" },
		{ DisplayType.Result, "ResultDisplay" }
	};

	/// <summary>
	/// 現在表示されているディスプレイ
	/// </summary>
	private IDisplay _currentdisplay;

	/// <summary>
	/// 現在表示されているシーンのキャッシュ
	/// </summary>
	private ISceneCache _currentSceneCache;

	/// <summary>
	/// シーン切り替え予約数
	/// </summary>
	private uint _switchStack = 0;

	/// <summary>
	/// シーン開始時実行イベント
	/// </summary>
	public static void OnSceneStart<T>(SceneBase<T> sceneBase) where T : SceneBase<T>
	{
		Instance._currentSceneCache = sceneBase.SceneCache;
	}

	/// <summary>
	/// シーン遷移時実行イベント
	/// </summary>
	public static void OnSceneEnd()
	{
		// ディスプレイ解放処理
		Switch(DisplayType.None);
	}

	/// <summary>
	/// ディスプレイの切り替え処理
	/// ディスプレイ遷移中に呼び出した場合、遷移後に切り替えを開始する
	/// </summary>
	public static void Switch(DisplayType type)
	{
		if (Instance._switchStack > 0)
		{
			Debug.LogWarning("ディスプレイ遷移の予約は最大2つまでです");
			return;
		}

		Instance.StartCoroutine(Instance._SwitchDisplay(type));
	}

	/// <summary>
	/// ディスプレイイベント継承クラスを取得する
	/// </summary>
	public static T GetInstanceDisplayEvents<T>() where T : class, IEvents
	{
		T events = Instance._currentdisplay.DisplayEvents as T;

		return events != null ? events : default(T);
	}

	public static bool IsEmpty()
	{
		return Instance == null;
	}

	/// <summary>
	/// ディスプレイ切替の非同期ステップ実行処理
	/// </summary>
	private IEnumerator _SwitchDisplay(DisplayType type)
	{
		_switchStack++;

		// ディスプレイ切り替え待ち
		while (_isSwitching)
			yield return null;

		// ディスプレイ切り替え開始
		_isSwitching = true;

		// ディスプレイ解放
		yield return StartCoroutine(SwitchFadeOut(_currentDisplayType));

		// ディスプレイ読み込み
		yield return StartCoroutine(SwitchFadeIn(type));

		_switchStack--;

		// ディスプレイ切り替え終了
		_isSwitching = false;
	}

	/// <summary>
	/// ディスプレイ切り替え開始処理
	/// </summary>
	private IEnumerator SwitchFadeOut(DisplayType DeleteDisplayType)
	{
		// 解放するディスプレイが無い場合、処理を中断する
		if (DeleteDisplayType == DisplayType.None)
			yield break;

		// 解放するディスプレイシーンのアニメーション再生
		yield return StartCoroutine(_currentdisplay.OnSwitchFadeOut());

		onFadedOut?.Invoke();
		onFadedOut = null;

		_currentdisplay.OnDelete();

		// ディスプレイシーン解放
		AsyncOperation asyncOp = SceneManager.UnloadSceneAsync(_DISPLAY_MAP[DeleteDisplayType]);

		// ディスプレイシーン解放待ち
		while (asyncOp.progress < 0.9f)
			yield return null;

		_currentDisplayType = DisplayType.None;
	}

	/// <summary>
	/// ディスプレイ切り替え終了処理
	/// </summary>
	private IEnumerator SwitchFadeIn(DisplayType LoadDisplayType)
	{
		// 読み込むディスプレイが無い場合、処理を中断する
		if (LoadDisplayType == DisplayType.None)
			yield break;

		// ディスプレイシーン読み込み
		AsyncOperation asyncOp = SceneManager.LoadSceneAsync(_DISPLAY_MAP[LoadDisplayType], LoadSceneMode.Additive);

		// ディスプレイシーン読み込み待ち
		while (!asyncOp.isDone)
			yield return null;

		// ディスプレイシーンの整理(このタイミングで_currentdisplay変更)
		yield return StartCoroutine(FindDisplayAndCleanUpScene(SceneManager.GetSceneByName(_DISPLAY_MAP[LoadDisplayType])));

		// ディスプレイの初期化
		_currentdisplay.OnAwake(_currentSceneCache);

		// ディスプレイ開始アニメーションの再生
		yield return StartCoroutine(_currentdisplay.OnSwitchFadeIn());

		onFadedIn?.Invoke();
		onFadedIn = null;

		_currentDisplayType = LoadDisplayType;
	}

	/// <summary>
	/// ディスプレイシーン内の不要なオブジェクトを全て消去し、ディスプレイクラスを取得する
	/// </summary>
	private IEnumerator FindDisplayAndCleanUpScene(Scene scene)
	{
		// ルートオブジェクト取得
		GameObject[] goList = scene.GetRootGameObjects();
		//TODO:IsNullOrEmpty作成
		//プロジェクト結合時にUtilityクラスにコレクション型のIsNullOrEmptyメソッドを拡張メソッド形式で作成する
		if (goList == null)
		{
			yield break;
		}
		if (goList.Length == 0)
		{
			yield break;
		}

		DisplayBase display;

		// ディスプレイオブジェクト取得処理
		foreach (var go in goList)
		{
			display = go.GetComponentInChildren<DisplayBase>();

			// displayを含んでいる場合(goがCanvas)
			if (display != null)
			{
				// ディスプレイオブジェクトの取得
				Instance._currentdisplay = display;
				display.gameObject.SetActive(false);

				// Canvas内のディスプレイオブジェクト以外の消去
				foreach (Transform child in go.transform)
				{
					if (child != display.transform)
					{
						Destroy(child.gameObject);
					}
				}
			}
			else
			{
				// ディスプレイオブジェクトと関係無いオブジェクトの消去
				Destroy(go);
			}
		}
		yield return null;
	}
}
