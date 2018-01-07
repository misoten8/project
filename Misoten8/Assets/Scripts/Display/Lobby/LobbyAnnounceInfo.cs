using TextFx;
using Misoten8Utility;
using UnityEngine;

/// <summary>
/// LobbyAnnounceInfo クラス
/// </summary>
public class LobbyAnnounceInfo : UIBase
{
	/// <summary>
	/// 待機状態
	/// </summary>
	private enum WaitState
	{
		/// <summary>
		/// ネットワークへの接続が開始されるまで待機
		/// </summary>
		BeginConnect,
		/// <summary>
		/// 接続状態が変わるまで待機
		/// </summary>
		ChangeConnectState,
		/// <summary>
		/// アニメーションタイマーがループ期間を超えるまで待機
		/// </summary>
		OverAnimLoopDuration
	}

	private WaitState _state = WaitState.BeginConnect;

	private TextFxTextMeshProUGUI _message;

	private LobbyNetworkParameters _networkParameters;

	/// <summary>
	/// アニメーションがループするまでの期間(時間)
	/// </summary>
	private float _animLoopDuration;

	/// <summary>
	/// 接続状態が変化する前のアニメーション時間
	/// </summary>
	private float _animTimerBeforeChangeConnectState;

	/// <summary>
	/// 前描画時の接続状況
	/// </summary>
	private LobbyNetworkParameters.ConnectState _prevConnectState;

	public override void OnAwake(ISceneCache cache, IEvents displayEvents)
	{
		base.OnAwake(cache, displayEvents);
		var events = displayEvents as LobbyEvents;
		if (events.IsEmpty())
			return;

		_message = uiObjects[0] as TextFxTextMeshProUGUI;
		if (_message.IsEmpty())
			return;

		var sceneCache = cache as LobbySceneCache;
		if (sceneCache.IsEmpty())
			return;

		_networkParameters = sceneCache.lobbyNetworkParameters;
		if (_networkParameters.IsEmpty())
			return;

		_prevConnectState = _networkParameters.CurrentState;

		_animLoopDuration = GetAnimLoopDuration();

		events.onBeginConnect += () =>
		{
			_state = WaitState.ChangeConnectState;
			OnDrawUpdate();
		};
	}

	/// <summary>
	/// 接続状態が前回と変更されていた場合描画する
	/// </summary>
	public override bool IsDrawUpdate()
	{
		float animTimer = _message.AnimationManager.AnimationTimer;

		switch (_state)
		{
			case WaitState.BeginConnect:
				return false;

			case WaitState.ChangeConnectState:
				if (_prevConnectState != _networkParameters.CurrentState)
				{
					_state = WaitState.OverAnimLoopDuration;
					_prevConnectState = _networkParameters.CurrentState;
					_animTimerBeforeChangeConnectState = animTimer;
				}
				return false;

			case WaitState.OverAnimLoopDuration:
				float diff = _animTimerBeforeChangeConnectState % _animLoopDuration;
				float waitTime = _animLoopDuration + _animTimerBeforeChangeConnectState - diff;

				if (animTimer >= waitTime)
				{
					_state = WaitState.ChangeConnectState;
					_prevConnectState = _networkParameters.CurrentState;
					_animTimerBeforeChangeConnectState = 0.0f;
					return true;
				}
				return false;

			default:
				Debug.LogWarning("値が不正です");
				return false;
		}
	}

	public override void OnDrawUpdate()
	{
		// メッセージの設定
		_message.SetText(LobbyNetworkParameters.MessageMap[_networkParameters.CurrentState]);

		// メッセージのスクロールアニメーション
		_message.AnimationManager.PlayAnimation();
	}

	/// <summary>
	/// 取得するまでがあまりにも長いため関数化
	/// </summary>
	private float GetAnimLoopDuration()
	{
		float duration = 10.0f;

		var anim = _message.AnimationManager.GetAnimation(0);
		if(anim.IsEmpty())
			return duration;

		var action = anim.GetAction(0);
		if (action.IsEmpty())
			return duration;

		var targetValue = action.m_duration_progression.Values[0];
		if (targetValue.IsEmpty())
			return duration;

		duration = targetValue;

		return duration;
	}
}