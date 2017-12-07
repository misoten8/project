using UnityEngine;

/// <summary>
/// DancingThread クラス
/// ダンスモード時にのみアクティブになる
/// モックアップでは使用しない
/// 製作者：実川
/// </summary>
public class DancingThread : MonoBehaviour
{
	[SerializeField]
	private Camera _camera;

	[SerializeField]
	private Dance _danceObserver;

	/// <summary>
	/// ダンス開始
	/// </summary>
	private void OnEnable ()
	{

	}

	/// <summary>
	/// ダンス終了
	/// </summary>
	private void OnDisable()
	{

	}

	void Update ()
	{
		// 連打によって
	}
}
