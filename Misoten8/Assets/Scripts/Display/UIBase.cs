using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UIオブジェクトのスクリプト制御クラス
/// </summary>
[DisallowMultipleComponent]
public class UIBase : MonoBehaviour
{
	/// <summary>
	/// UIオブジェクトの初期ローカル座標
	/// </summary>
	public Vector3 AnchorPos
	{
		get { return anchorPos; }
	}

	protected Vector3 anchorPos;

	/// <summary>
	/// 遷移アニメーションを再生するかどうか
	/// </summary>
	public bool PlaySwitchAnim
	{
		get { return playSwitchAnim; }
	}

	[SerializeField]
	protected bool playSwitchAnim = true;

	/// <summary>
	/// スクリプト制御を行うUIオブジェクト(アタッチ必須)
	/// </summary>
	[SerializeField, Header("スクリプト制御を行うUIオブジェクトリスト(アタッチ必須)")]
	protected Graphic[] uiObjects;

	/// <summary>
	/// ディスプレイ生成時に呼ばれるイベント
	/// </summary>
	virtual public void OnAwake(ISceneCache cache, IEvents displayEvents)
	{
		anchorPos = transform.localPosition;
	}

	/// <summary>
	/// 描画更新判定処理
	/// </summary>
	virtual public bool IsDrawUpdate()
	{
		return false;
	}

	/// <summary>
	/// UIの描画更新処理
	/// </summary>
	virtual public void OnDrawUpdate() { }

	private void Awake()
	{
		//TODO:処理負荷によってオブジェクト非表示より描画が優先される可能性あり(要検証)
		// 初期化時にUIオブジェクトを非表示にする
		//gameObject.SetActive(false);
	}
}
