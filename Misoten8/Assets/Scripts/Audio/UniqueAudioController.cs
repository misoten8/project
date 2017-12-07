using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// オーディオ操作クラス
/// このクラスは一つのオーディオにしか対応していません。
/// 製作者：実川
/// </summary>
public class UniqueAudioController : MonoBehaviour
{
	/// <summary>
	/// インスタンス
	/// </summary>
	static UniqueAudioController m_currentInstance = null;

	[SerializeField]
	private AudioSource m_audioSource;

	void Awake()
	{
		if (m_currentInstance != null) Destroy(m_currentInstance.gameObject);
		m_currentInstance = this;
	}

	/// <summary>
	/// オーディオの再生中断
	/// </summary>
	public static void Pause()
	{
		if (m_currentInstance == null) return;
		m_currentInstance.m_audioSource.Pause();
	}

	/// <summary>
	/// オーディオの再生続行
	/// </summary>
	public static void Continue()
	{
		if (m_currentInstance == null) return;
		if (m_currentInstance.m_audioSource.isPlaying) return;
		m_currentInstance.m_audioSource.Play();
	}

	/// <summary>
	/// オーディオを最初から再生
	/// </summary>
	public static void RePlay()
	{
		if (m_currentInstance == null) return;
		m_currentInstance.m_audioSource.Stop();
		m_currentInstance.m_audioSource.Play();
	}

	/// <summary>
	/// オーディオの音量を調整する
	/// </summary>
	/// <param name="volume">音量</param>
	public static void SetVolume(float volume)
	{
		if (m_currentInstance == null) return;
		m_currentInstance.m_audioSource.volume = volume;
	}
}
