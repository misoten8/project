﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using WiimoteApi;

/// <summary>
/// シーン基底クラス
/// </summary>
public abstract class SceneBase<T> : MonoBehaviour where T : SceneBase<T>
{
	/// <summary>
	/// メインシーンの種類
	/// </summary>
	public enum SceneType : byte
	{
		None = 0,
		Title,
		Lobby,
		Battle,
		Result
	}

	/// <summary>
	/// このオブジェクトが存在するシーン
	/// </summary>
	public Scene FromScene
	{
		get { return gameObject.scene; }
	}

	/// <summary>
	/// 外部シーンが利用できるデータキャッシュ
	/// </summary>
	public abstract ISceneCache SceneCache
	{
		get;
	}

	/// <summary>
	/// メインシーンタイプとシーンの紐付けマップ
	/// </summary>
	protected static readonly Dictionary<SceneType, string> SCENE_MAP = 
		new Dictionary<SceneType, string>
	{
		{ SceneType.Title, "Title" },
		{ SceneType.Lobby, "Lobby" },
		{ SceneType.Battle, "Battle" },
		{ SceneType.Result, "Result" }
	};

	/// <summary>
	/// シーン生成時に最初に表示されるディスプレイ
	/// </summary>
	[SerializeField]
	protected DisplayManager.DisplayType firstUsingDisplay;

	/// <summary>
	/// シーン遷移中かどうか
	/// </summary>
	protected bool duringTransScene = false;

	/// <summary>
	/// シーン切り替え
	/// </summary>
	public virtual void Switch(SceneType nextScene)
	{
		if (duringTransScene)
			return;

		StartCoroutine(SwitchAsync(nextScene));
	}

	/// <summary>
	/// 派生クラスのインスタンスを取得
	/// </summary>
	protected abstract T GetOverrideInstance();

	/// <summary>
	/// 非同期シーン切り替え
	/// </summary>
	protected IEnumerator SwitchAsync(SceneType nextScene)
	{
		duringTransScene = true;

		shakeparameter.ResetShakeParameter();

		DisplayManager.OnSceneEnd();

		// ディスプレイ解放待ち
		while (DisplayManager.IsSwitching)
			yield return null;

		SceneManager.LoadSceneAsync(SCENE_MAP[nextScene], LoadSceneMode.Single);

		shakeparameter.ResetShakeParameter();
	}

	private void Awake()
	{
		// シェイクカウンタの初期化
		shakeparameter.ResetShakeParameter();

		// シーン情報を渡す
		DisplayManager.OnSceneStart(GetOverrideInstance());

		// ディスプレイの表示
		DisplayManager.Switch(firstUsingDisplay);
	}

	/// <summary>
	/// アプリケーション終了時実行イベント
	/// </summary>
	void OnApplicationQuit()
	{
		Debug.Log("ルームから退出しました");

		if (PhotonNetwork.room == null)
			return;

        // Wiiリモコン終了処理
        if (WiimoteManager.Wiimotes[0] != null)
        {
            WiimoteManager.Cleanup(WiimoteManager.Wiimotes[0]);
            WiimoteManager.Wiimotes[0] = null;
        }
		// ルーム退室  
		PhotonNetwork.LeaveRoom();
		// ネットワーク切断
		PhotonNetwork.Disconnect();
	}
}
