﻿using System.Linq;
using UnityEngine;
using Misoten8Utility;
using System;
using UnityEngine.AI;

/// <summary>
/// モブキャラ クラス
/// 製作者：実川
/// </summary>
public class Mob : Photon.PunBehaviour
{
	/// <summary>
	/// 合計を1.0にしたファンポイント
	/// </summary>
	public float[] InterpolationFanPointArray
	{
		get { return _fanPointArray.Select(x => x / Define.FanPointArray[(int)_fanLevel]).ToArray(); }
	}

	private float[] _fanPointArray = new float[Define.METER_NUM_MAX] { 0, 0, 0, 0, 0 };

	/// <summary>
	/// 一押しのプレイヤー(無所属の場合もある)
	/// </summary>
	public Define.PlayerType FunType
	{
		get { return _funType; }
	}

	[SerializeField]
	private Define.PlayerType _funType = Define.PlayerType.None;

	/// <summary>
	/// 現在モブキャラが推しているプレイヤー
	/// </summary>
	public Player funPlayer
	{
		get { return _funPlayer; }
	}

	private Player _funPlayer = null;

	/// <summary>
	/// モブが移動状態になった時に呼ぶイベント
	/// </summary>
	public event Action onMoveMob;

	/// <summary>
	/// プレイヤー追従対象が変化した時に呼ぶイベント
	/// </summary>
	public event Action onChangeFllowPlayer;

	[SerializeField]
	private Define.FanLevel _fanLevel;

	[SerializeField]
	private MobController _mobController;

	[SerializeField]
	private Animator _animator;

	[SerializeField]
	private Transform _modelPlaceObject;

    // フォロワーになった順番
    // TODO:ここにフォロワーのインデックス番号
    public int _followInex = 0;

	/// <summary>
	/// マテリアルを設定する対象メッシュ
	/// </summary>
	private ModelSkinnidMeshs _modelSkinnidMeshs;

	/// <summary>
	/// モブ管理クラス
	/// </summary>
	public MobManager MobManager
	{
		get { return _mobManager; }
	}

	private MobManager _mobManager;

	/// <summary>
	/// プレイヤー管理クラス
	/// </summary>
	public PlayerManager PlayerManager
	{
		get { return _playerManager; }
	}

	private PlayerManager _playerManager;

	/// <summary>
	/// 追従する対象プレイヤー
	/// </summary>
	public Define.PlayerType FllowTarget
	{
		get { return _fllowTarget; }
	}

	private Define.PlayerType _fllowTarget = Define.PlayerType.None;

	public NavMeshAgent NavMeshAgent
	{
		get { return _navMeshAgent; }
	}

	[SerializeField]
	private NavMeshAgent _navMeshAgent;

	/// <summary>
	/// ダンス視聴中エフェクト
	/// </summary>
	private GameObject _danceNowEffect;

	/// <summary>
	/// 既にダンスを視聴中かどうか
	/// </summary>
    public bool IsViewingInDance
    {
        get { return _isViewingInDance; }
    }

	private bool _isViewingInDance;

	/// <summary>
	/// 追従変更処理が実行されたかどうか
	/// </summary>
	private bool _isPlayChangeFollowTraget = true;

	/// <summary>
	/// 目標地点の変更をスタックしているかどうか
	/// </summary>
	public bool IsSetMarkerStack
	{
		get { return _isSetMarkerStack; }
		set { _isSetMarkerStack = value; }
	}

	private bool _isSetMarkerStack = false;

	/// <summary>
	/// PhotonNetwork.Instantiate によって GameObject(とその子供)が生成された際に呼び出されます。
	/// </summary>
	/// <remarks>
	/// PhotonMessageInfoパラメータは「誰が」「いつ」作成したかを提供します。
	/// (「いつ」は PhotonNetworking.time に基づきます。)
	/// </remarks>
	public override void OnPhotonInstantiate(PhotonMessageInfo info)
	{
		var cache = GameObject.Find("SystemObjects/BattleManager").GetComponent<MobGenerator>().Caches;
		_mobManager = cache.mobManager;
		_playerManager = cache.playerManager;
		// モブを登録
		_mobManager.SetMob(this);

		// 無所属に全てのファンポイントを設定
		_fanPointArray[0] = Define.FanPointArray[(int)_fanLevel];

		// モブ再生イベント実行
		onMoveMob?.Invoke();

		// モデルの設定
		GameObject model = Instantiate(ModelManager.GetCache(_mobManager.GetRandomModelType()));
		model.transform.SetParent(_modelPlaceObject);
		model.transform.localPosition = Vector3.zero;
		_modelSkinnidMeshs = model.GetComponent<ModelSkinnidMeshs>();
		_animator.avatar = model.GetComponent<Animator>().avatar;
		_animator.runtimeAnimatorController = model.GetComponent<Animator>().runtimeAnimatorController;
		// アニメーションの設定
		_mobController.OnStart(_animator);

		// アウトラインの更新
		_modelSkinnidMeshs.SkinnedMeshs.Foreach(e => e.materials[1].color = new Color(0.2f, 0.2f, 0.2f));
	}

	private void OnTriggerStay(Collider other)
	{
		if (other.tag != "DanceRange")
			return;

		// 既に他のプレイヤーのダンスを視聴している場合無視する
		if (_isViewingInDance)
			return;

		Dance playerDance = other.gameObject.GetComponent<Dance>();

		if (IsFollowTargetChange(playerDance))
		{
			_mobManager.FollowChangeStack(playerDance.PlayerType, photonView.viewID);
			_isPlayChangeFollowTraget = false;
		}
		// ダンスが再生フェーズなら視聴する
		if (playerDance.DancePhase == Dance.Phase.Play ||
			playerDance.DancePhase == Dance.Phase.Start)
		{
			// ダンス開始イベント実行
			OnBeginDance();
		}
	}

	public void SetFunType(Define.PlayerType type)
	{
		if (_funType != type)
		{
			// ファンタイプの更新
			_funType = type;

			// 推しているプレイヤーの更新
			_funPlayer = PlayerManager.GetPlayer(type);

			// ファン番号を初期化(最後尾にする)
			_followInex = 999;

			// 追従対象の更新
			_fllowTarget = type;

			// アウトラインの更新
			_modelSkinnidMeshs.SkinnedMeshs.Foreach(e => e.materials[1].color = Define.playerColor[(int)type]);

			// モブ移動動作の変更
			onChangeFllowPlayer?.Invoke();
		}
	}

	public void SetFollowType(Define.PlayerType type)
	{
		if (_fllowTarget != type)
		{
			// ファン番号を初期化(最後尾にする)
			_followInex = 999;

			// 追従対象の更新
			_fllowTarget = type;

			// モブ移動動作の変更
			onChangeFllowPlayer?.Invoke();

			_isPlayChangeFollowTraget = true;
		}
	}

	/// <summary>
	/// ダンス開始時実行イベント
	/// </summary>
	public void OnBeginDance()
	{
		// ダンス視聴中エフェクト再生
		_danceNowEffect = ParticleManager.Play("DanceNow", new Vector3(), transform);

		_isViewingInDance = true;
	}

	/// <summary>
	/// ダンス終了時実行イベント
	/// </summary>
	public void OnEndDance(Dance.DanceResultState state)
	{
		// 追従対象のプレイヤーのダンスコンポーネントを取得する
		Dance playerDance = _playerManager.GetPlayer(FllowTarget).Dance;
		_isViewingInDance = false;

		switch (state)
		{
			case Dance.DanceResultState.Clear:
				{
					// モブキャラ管理クラスにスコア変更を通知
					_mobManager.OnScoreChange();

					if (playerDance.Player.IsMine)
					{
						// ファンタイプが変更したかチェックする
						Define.PlayerType newFunType = playerDance.Player.Type;
						if (FunType != newFunType)
						{
							_mobManager.FanChangeStack(newFunType, photonView.viewID);
						}
					}
				}
				break;
			case Dance.DanceResultState.Miss:
				{
					if (playerDance.Player.IsMine)
					{
						// 散歩モードに移行する
						if (FunType == Define.PlayerType.None)
						{
							_mobManager.FanChangeStack(Define.PlayerType.None, photonView.viewID);
						}
					}
				}
				break;
			case Dance.DanceResultState.Win:
				{
					// モブキャラ管理クラスにスコア変更を通知
					_mobManager.OnScoreChange();

					// ファン変更処理は上位スクリプトで呼び出す
				}
				break;
			case Dance.DanceResultState.Lose:
				{
					// モブキャラ管理クラスにスコア変更を通知
					_mobManager.OnScoreChange();

					// ファン変更処理は上位スクリプトで呼び出す
				}
				break;
			case Dance.DanceResultState.Cansel:
				{
					if (playerDance.Player.IsMine)
					{
						// ファンが多いプレイヤーに追従する
						if (IsFollowTargetChange(playerDance))
						{
							_mobManager.FollowChangeStack(playerDance.PlayerType, photonView.viewID);
							_isPlayChangeFollowTraget = false;
						}
					}
				}
				break;
		}

		// モブ再生イベント実行
		onMoveMob?.Invoke();

		Destroy(_danceNowEffect);
	}

	/// <summary>
	/// 追従対処が変化したかどうか
	/// </summary>
	private bool IsFollowTargetChange(Dance playerDance)
	{
		if (!_isPlayChangeFollowTraget)
			return false;

		if (_mobManager.GetFunCount(_fllowTarget) < _mobManager.GetFunCount(playerDance.PlayerType)
				|| _fllowTarget == Define.PlayerType.None)
		{
			if (FunType == Define.PlayerType.None)
				return true;
		}
		return false;
	}

	/// <summary>
	/// 定義のみ
	/// </summary>
	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) { }
}
