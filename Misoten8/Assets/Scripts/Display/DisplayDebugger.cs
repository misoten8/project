using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// ディスプレイ単体駆動補助クラス
/// </summary>
/// <remarks>
/// ディスプレイシーン単体でのテストはこのクラスを使用してください
/// </remarks>
public class DisplayDebugger : MonoBehaviour
{
	/// <summary>
	/// 呼び出し対象ディスプレイ
	/// </summary>
	[SerializeField]
	private DisplayBase _display;

	/// <summary>
	/// 引き渡し用シーンキャッシュ
	/// </summary>
	[SerializeField]
	private SceneCacheBase _sceneCache;

	[SerializeField]
	private DisplayDebugEventBase _debugEvents;

	private void Start()
	{
		// デバッグモードかどうか
		if(!DisplayManager.IsEmpty())
		{
			Destroy(gameObject);
			return;
		}

		Debug.Log("ディスプレイデバッガー起動");

		// ディスプレイ初期化処理呼び出し
		_display?.OnAwake(_sceneCache);
		// フェードインアニメーション再生
		_display?.OnSwitchFadeIn();
		// デバッグ用イベントクラスの初期化
		_debugEvents?.OnAwake();
	}

	private void OnGUI()
	{
		int counter = 0;
		_debugEvents?.DebugEvents.eventList.ForEach(e => 
		{
			// イベント実行用ボタンUI表示
			if (GUI.Button(new Rect(new Vector2(0, counter * 20), new Vector2(300, 20)), e.name))
			{
				// イベント実行
				e.displayEvent?.Invoke();
			}
			counter++;
		});
	}

	/// <summary>
	/// デバッグでのみ使用するイベントクラス
	/// </summary>
	public class DebugEvents
	{
		/// <summary>
		/// イベントの単体要素クラス
		/// </summary>
		public class Element
		{
			/// <summary>
			/// ディスプレイイベント
			/// </summary>
			public Action displayEvent;

			/// <summary>
			/// イベント名
			/// </summary>
			public String name;

			/// <summary>
			/// コンストラクタ
			/// </summary>
			public Element(Action DisplayEvent, String Name)
			{
				displayEvent = DisplayEvent;
				name = Name;
			}
		}

		/// <summary>
		/// デバッグ用イベントクラスのリスト
		/// </summary>
		public List<Element> eventList = new List<Element>();
	}
}