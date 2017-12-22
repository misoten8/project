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

        // 目標座標設定
        Vector3 goal = _target.position;
        float angle = Mathf.PI * 0.75f;
        float angle2 = angle * 2;
        bool debugMode = true;
        float rot = SetDirection(_target.position, _player.TargetObj.position);

        if (_mob.IsViewingInDance || debugMode)
        {
            // プレイヤーダンス中追従
            goal.x += Mathf.Sin(rot + angle - ((fanNum % 2) * angle2)) * (-mobInterval * (fanNum % cutNum + 1)) +
                        Mathf.Sin(rot) * ((mobInterval) * (fanNum / cutNum));

            goal.z += Mathf.Cos(rot + angle - ((fanNum % 2) * angle2)) * (-mobInterval * (fanNum % cutNum + 1)) +
                        Mathf.Cos(rot) * ((mobInterval) * (fanNum / cutNum));

            _agent.SetDestination(goal);
        }
        else
        {
            // プレイヤー移動中追従
            goal.x += -Mathf.Sin((rot) * (Mathf.PI * 0.5f)) * 4.0f + (-Mathf.Sin(rot) * ((mobInterval) * (fanNum / cutNum)));
            goal.z += -Mathf.Cos((rot) * (Mathf.PI * 0.5f)) * 4.0f + (-Mathf.Cos(rot) * ((mobInterval) * (fanNum / cutNum)));
            _agent.SetDestination(goal);
        }

            // 遷移チェック
            _onTransCheck?.Invoke();
	}

    // 
    float SetDirection(Vector3 p1, Vector3 p2)
    {
        float dx, dy;
        dx = p1.x - p2.x;
        dy = p1.y - p2.y;
        float rad = Mathf.Atan2(dy, dx);
        return rad * Mathf.Rad2Deg;
    }
}