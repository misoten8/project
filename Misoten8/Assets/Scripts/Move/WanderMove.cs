using System;
using UniRx;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 徘徊移動 クラス
/// 製作者：実川
/// </summary>
public class WanderMove : MonoBehaviour, IMove
{
    private enum State
    {
        Move,
        Stop
    }
    
    private State _state = State.Move;

    /// <summary>
    /// 遷移条件判定イベント
    /// Updateのタイミングで呼ばれます
    /// </summary>
    public Action OnCheck
    {
        set { _onCheck += value; }
    }

    private Action _onCheck;

    /// <summary>
    /// 遷移時実行イベント
    /// </summary>
    public Action OnTrans
    {
        set { _onTrans += value; }
    }

    private Action _onTrans;

    /// <summary>
    /// 移動する速度
    /// </summary>
    [SerializeField]
    private float _velocity;

    /// <summary>
    /// 最大速度
    /// </summary>
    [SerializeField]
    private float _maxVelocity;

    /// <summary>
    /// 移動方向
    /// </summary>
    private Vector3 _moveDirection;

    /// <summary>
    /// 最初に呼ばれるフレームかどうか
    /// </summary>
    private bool _isFirstFrame = true;

	private MarkerManager _marker;
	private NavMeshAgent _agent = null;
	private Mob _mob;

	/// <summary>
	/// 初期化処理
	/// </summary>
	public void OnStart(Mob mob)
    {
        enabled = true;
		_mob = mob;
        _isFirstFrame = true;
        _state = State.Move;
        _agent = _mob.NavMeshAgent;
        _agent.enabled = true;
        _marker = GameObject.Find("MobControlleMarker").GetComponent<MarkerManager>();
		// 目標地点の設定
		byte index = (byte)UnityEngine.Random.Range(0, _marker.MarkerNum);
		_mob.MobManager.MarkerChangeStack(index, (byte)_mob.photonView.viewID);
		_mob.IsSetMarkerStack = true;
	}

    void Update()
    {
        // 遷移チェック
        _onCheck?.Invoke();

		if (!PhotonNetwork.isMasterClient)
			return;

		if (_mob.IsSetMarkerStack)
			return;

        // 目標座標変更処理
        if (_agent.remainingDistance < 5.0f )
        {
			byte index = (byte)UnityEngine.Random.Range(0, _marker.MarkerNum);
			_mob.MobManager.MarkerChangeStack(index, (byte)_mob.photonView.viewID);
			_mob.IsSetMarkerStack = true;
		}
    }
}