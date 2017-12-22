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

    private MarkerManager _marker;
    private NavMeshAgent _agent = null;

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
	/// NPCキャッシュ
	/// </summary>
	private Mob _mob = null;

    /// <summary>
    /// 初期化処理
    /// </summary>
    public void OnStart(Mob mob)
    {
		_mob = mob;
        enabled = true;
        _state = State.Move;
        _agent = GetComponent<NavMeshAgent>();
        _agent.enabled = true;
        _marker = GameObject.Find("MobControlleMarker").GetComponent<MarkerManager>();
        _marker.GotoNextPoint(_agent);
    }

    void Update()
    {
        // 遷移チェック
        _onCheck?.Invoke();

        // 目標座標変更処理
        if (_agent.remainingDistance < 5.0f )
        {
            int targetNum;
            targetNum = _marker.GotoNextPoint(_agent);
            // TODO:ここにMobの目標マーカー番号送信処理？
        }
    }
}