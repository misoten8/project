using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// パーティクルコンポーネント
/// 再生終了時に消去するクラス
/// 製作者：実川
/// </summary>
public class ParticleDestroyer : MonoBehaviour
{
	[SerializeField]
	private ParticleSystem m_ps;

	void Update ()
	{
		if (m_ps.IsAlive()) return;
		Destroy(gameObject);
	}
}
