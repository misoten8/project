using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ディスプレイ切り替えアニメーションクラス
/// </summary>
public class DisplaySwitchAnim : MonoBehaviour
{
	public enum AnimType
	{
		/// <summary>
		/// 移動しない
		/// </summary>
		None,
		/// <summary>
		/// 外側から中心に向かって各UIオブジェクトが移動する
		/// </summary>
		CircleIn,
		/// <summary>
		/// 中心から外側に向かって各UIオブジェクトが移動する
		/// </summary>
		CircleOut,
		/// <summary>
		/// 左方向で外側から中心へ各UIオブジェクトが移動する
		/// </summary>
		SlideLeftIn,
		/// <summary>
		/// 左方向で中心から外側へ各UIオブジェクトが移動する
		/// </summary>
		SlideLeftOut
	}

	/// <summary>
	/// アニメーション再生中かどうか
	/// </summary>
	public bool IsPlaying
	{
		get { return _isPlaying; }
	}

	private bool _isPlaying = false;

	/// <summary>
	/// フェードインアニメーション再生時間
	/// </summary>
	[SerializeField]
	private float _fadeInAnimTime = 1.0f;

	/// <summary>
	/// フェードアウトアニメーション再生時間
	/// </summary>
	[SerializeField]
	private float _fadeOutAnimTime = 1.0f;

	/// <summary>
	/// フェードインで使用する再生アニメーションの種類
	/// </summary>
	[SerializeField]
	private AnimType _fadeInAnim = AnimType.None;

	/// <summary>
	/// フェードアウトで使用する再生アニメーションの種類
	/// </summary>
	[SerializeField]
	private AnimType _fadeOutAnim = AnimType.None;

	/// <summary>
	/// アニメーションメソッドとアニメーションタイプを紐付けるマップ
	/// </summary>
	private Dictionary<AnimType, Action<float, Vector3>> _animationMap;

	/// <summary>
	/// アニメーションを再生させるUIオブジェクトリスト
	/// </summary>
	private List<UIBase> _playAnimUiList = null;

	/// <summary>
	/// アニメーション再生経過時間
	/// </summary>
	private float _playingAnimElapsedTime = 0.0f;

	/// <summary>
	/// アニメーション再生時間
	/// </summary>
	private float _animTime = 0.0f;

	/// <summary>
	/// ディスプレイ中心座標
	/// </summary>
	private Vector3 _anchorPos = new Vector3();

	/// <summary>
	/// アニメーション再生処理
	/// </summary>
	private Action<float, Vector3> _animPlayer;

	/// <summary>
	/// 初期化処理
	/// </summary>
	public void OnAwake(List<UIBase> uiList)
	{
		_playAnimUiList = uiList.FindAll(e => e.PlaySwitchAnim);
		_animationMap = new Dictionary<AnimType, Action<float, Vector3>>
		{
			{ AnimType.None, _Empty },
			{ AnimType.CircleIn, _CircleIn },
			{ AnimType.CircleOut, _CircleOut },
			{ AnimType.SlideLeftIn, _SlideLeftIn },
			{ AnimType.SlideLeftOut, _SlideLeftOut }
		};
	}

	/// <summary>
	/// アニメーション再生処理
	/// </summary>
	public void OnPlayFadeIn()
	{
		_OnPlay(_fadeInAnim);
		_animTime = _fadeInAnimTime;
	}

	/// <summary>
	/// アニメーション再生処理
	/// </summary>
	public void OnPlayFadeOut()
	{
		_OnPlay(_fadeOutAnim);
		_animTime = _fadeOutAnimTime;
	}

	private void Update()
	{
		if (!_isPlaying)
			return;

		_playingAnimElapsedTime += Time.deltaTime;
		_animPlayer?.Invoke(Mathf.Min(_playingAnimElapsedTime / _animTime, 1.0f), _anchorPos);

		if (_playingAnimElapsedTime > _animTime)
			_isPlaying = false;
	}

	/// <summary>
	/// 共通のアニメーション再生処理
	/// </summary>
	private void _OnPlay(AnimType animType)
	{
		_isPlaying = true;
		_playingAnimElapsedTime = 0.0f;
		_animPlayer = _animationMap[animType];
		_animPlayer?.Invoke(0.0f, _anchorPos);
	}

	private void _Empty(float rate, Vector3 anchorPos) { }

	private void _CircleIn(float rate, Vector3 anchorPos)
	{
		Vector3 direction, distance;
		_playAnimUiList.ForEach(e =>
		{
			direction = Vector3.Normalize(e.AnchorPos - anchorPos);
			distance = new Vector3(direction.x * (Screen.width * 3.0f), direction.y * (Screen.height * 3.0f), 0.0f);
			e.transform.localPosition = Vector3.Lerp(e.AnchorPos + distance, e.AnchorPos, rate);
		});
	}

	private void _CircleOut(float rate, Vector3 anchorPos)
	{
		Vector3 direction, distance;
		_playAnimUiList.ForEach(e =>
		{
			direction = Vector3.Normalize(e.AnchorPos - anchorPos);
			distance = new Vector3(direction.x * (Screen.width * 3.0f), direction.y * (Screen.height * 3.0f), 0.0f);
			e.transform.localPosition = Vector3.Lerp(e.AnchorPos, e.AnchorPos + distance, rate);
		});
	}

	private void _SlideLeftIn(float rate, Vector3 anchorPos)
	{
		transform.localPosition = Vector3.Slerp(anchorPos + new Vector3(2000.0f, 0.0f, 0.0f), anchorPos, rate);
	}

	private void _SlideLeftOut(float rate, Vector3 anchorPos)
	{
		transform.localPosition = Vector3.Slerp(anchorPos, anchorPos - new Vector3(2000.0f, 0.0f, 0.0f), rate);
	}
}
