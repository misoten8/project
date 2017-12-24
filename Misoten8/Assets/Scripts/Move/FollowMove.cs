using System;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 追従移動 クラス
/// Rigitbodyを使用せず、直接座標を設定します
/// 製作者：実川
/// </summary>
public class FollowMove : MonoBehaviour, IMove
{
    /// <summary>
    /// 遷移条件判定イベント
    /// FixedUpdateのタイミングで呼ばれます
    /// </summary>
    public Action OnTransCheck
    {
        set { _onTransCheck += value; }
    }

    private Action _onTransCheck;

    /// <summary>
    /// 移動する速度
    /// </summary>
    [SerializeField]
    private float _velocity;

    /// <summary>
    /// 移動を中止する距離
    /// </summary>
    [SerializeField]
    private float _stopDistance;

    /// <summary>
    /// 速度の減少を開始する距離
    /// </summary>
    [SerializeField]
    private float _slowDistance;

    private Transform _target = null;

    private NavMeshAgent _agent = null;

    private Mob _mob = null;
    private Player _player = null;
    public int cutNum = 10;      // 列を区切る番号

    private float mobInterval = 1.5f;
    public int fanNum = 0;
    public float testRot = 0.0f;

    /// <summary>
	/// 初期化処理
	/// </summary>
	public void OnStart(Transform target)
    {
        _target = target;
        _player = _target.GetComponent<Player>();
        enabled = true;
        _agent = GetComponent<NavMeshAgent>();
        _agent.enabled = true;
        _mob = GetComponent<Mob>();
        mobInterval = 2.0f;
    }

    void OnDisable()
    {
        _target = null;
        _agent = null;
    }

    void Update()
    {
        if (_target == null)
        {
            Debug.Log("追従対象が見つからない為、非アクティブになります");
            enabled = false;
            return;
        }
        Vector3 goal = _player.TargetObj.position;
        goal.z = -goal.z;
        _agent.SetDestination(_player.TargetObj.position);

        if (_agent.remainingDistance < 1.0f + (_mob._followInex * mobInterval))
        {
            _agent.isStopped = true;
        }
        else
        {
            _agent.isStopped = false;
        }
        // 遷移チェック
        _onTransCheck?.Invoke();
    }

    // 
    float SetDirection(Vector3 p1, Vector3 p2)
    {
        Debug.Log("Target: " + p2);
        float dx, dy;
        dx = p1.x - p2.x;
        dy = p1.z - p2.z;
        float rad = Mathf.Atan2(dy, dx);
        Debug.Log("rot : " + rad);
        return rad * Mathf.Rad2Deg;
    }
}