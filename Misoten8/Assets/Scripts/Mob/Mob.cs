using System.Linq;
using UnityEngine;
using Misoten8Utility;
using System;

/// <summary>
/// モブキャラ クラス
/// 製作者：実川
/// </summary>
public class Mob : Photon.PunBehaviour
{
	/// <summary>
	/// ファンポイント
	/// 配列の合計で人のファンポイント最大値になるようにする
	/// ファンポイント最大値はファンタイプに応じて変わる
	/// </summary>
	public float[] FanPointArray
	{
		get { return _fanPointArray; }
	}

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
	/// 接触したプレイヤーのダンスコンポーネント
	/// </summary>
	private Dance _playerDance = null;

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

		// ファンタイプの更新
		_funType = (Define.PlayerType)_fanPointArray.FindIndexMax();

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

		_playerDance = other.gameObject.GetComponent<Dance>();

		// ダンスが再生フェーズなら視聴する
		if (_playerDance.DancePhase == Dance.Phase.Play)
		{
			// ダンス開始イベント実行
			OnBeginDance();
		}
		else
		{
			if (IsFollowTargetChange(_playerDance))
			{
				_mobManager.FollowChangeStack(_playerDance.PlayerType, photonView.viewID);
				_isPlayChangeFollowTraget = false;
			}
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
	public void OnEndDance(bool isCancel, bool isSuccess)
	{
		if (_playerDance == null)
		{
			Debug.LogWarning("ダンスコンポーネントがnullです\n処理を中断しました");
			return;
		}

		_isViewingInDance = false;

		if (!isCancel)
		{
			// モブキャラ管理クラスにスコア変更を通知
			_mobManager.OnScoreChange();

			// ファンタイプが変更したかチェックする
			Define.PlayerType newFunType = isSuccess ? _playerDance.Player.Type : Define.PlayerType.None;
			if (FunType != newFunType)
			{
				_mobManager.FanChangeStack(newFunType, photonView.viewID);
			}
		}
		else
		{
			if (IsFollowTargetChange(_playerDance))
			{
				_mobManager.FollowChangeStack(_playerDance.PlayerType, photonView.viewID);
				_isPlayChangeFollowTraget = false;
			}
		}

		// モブ再生イベント実行
		onMoveMob?.Invoke();

		Destroy(_danceNowEffect);

		_playerDance = null;
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
