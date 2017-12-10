using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// ディスプレイ基底クラス
/// </summary>
[RequireComponent(typeof(DisplaySwitchAnim))]
public abstract class DisplayBase : MonoBehaviour, IDisplay
{
	/// <summary>
	/// ディスプレイ切り替えアニメーションが再生中かどうか
	/// </summary>
	public bool IsSwitchAnimPlaying
	{
		get { return isSwitchAnimPlaying; }
	}

	/// <summary>
	/// ディスプレイイベントの定義インターフェイス
	/// </summary>
	public virtual IEvents DisplayEvents
	{
		get { return null; }
	}
		
	/// <summary>
	/// ディスプレイ切り替えアニメーションが再生中かどうか
	/// </summary>
	protected bool isSwitchAnimPlaying;

	/// <summary>
	/// OnAwake が呼ばれたかどうか
	/// </summary>
	protected bool isCallOnAwake = false;

	/// <summary>
	/// UIオブジェクトのリスト
	/// </summary>
	[SerializeField]
	protected List<UIBase> uiList = new List<UIBase>();

	/// <summary>
	/// ディスプレイ切り替えアニメーション
	/// </summary>
	[SerializeField, HideInInspector]
	protected DisplaySwitchAnim switchAnim;

	/// <summary>
	/// ディスプレイ生成時に呼ばれるイベント
	/// </summary>
	public virtual void OnAwake(ISceneCache cache) 
	{
		gameObject.SetActive(true);
		// キャッシュを各UIオブジェクトに渡す(イベントクラスは渡さない)
		uiList.ForEach(e => e.OnAwake(cache, null));
		isCallOnAwake = true;
		switchAnim.OnAwake (uiList);
	}

	/// <summary>
	/// ディスプレイ遷移開始時に呼ばれるイベント
	/// </summary>
	public virtual IEnumerator OnSwitchFadeIn() 
	{
		switchAnim.OnPlayFadeIn ();
		while (switchAnim.IsPlaying)
			yield return null;
	}

	/// <summary>
	/// ディスプレイ遷移開始時に呼ばれるイベント
	/// </summary>
	public virtual IEnumerator OnSwitchFadeOut()
	{ 
		switchAnim.OnPlayFadeOut ();
		while (switchAnim.IsPlaying)
			yield return null;
	}

	/// <summary>
	/// ディスプレイ消去時に呼ばれるイベント
	/// </summary>
	public virtual void OnDelete() { }
}