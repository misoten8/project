using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// オーディオ管理クラス
/// 製作者：実川
/// </summary>
public class AudioManager : SingletonMonoBehaviour<AudioManager>
{
	[SerializeField]
	GameObject m_BGMPrefab;
	[SerializeField]
	GameObject m_SEPrefab;

	/// <summary>
	/// サウンドのAudioClipキャッシュリスト
	/// </summary>
	List<AudioClip> m_SECacheList = new List<AudioClip>();

	/// <summary>
	/// BGMのAudioClipキャッシュリスト
	/// </summary>
	List<AudioClip> m_BGMChaheList = new List<AudioClip>();

	void Start()
	{
		Load();
	}

	/// <summary>
	/// 使用するオーディオを事前に読み込む
	/// </summary>
	public static void Load()
	{
		if (Instance.m_SECacheList.Count == 0)
		{
			foreach (AudioClip audioClip in Resources.LoadAll<AudioClip>("Audios/SE"))
			{
				Instance.m_SECacheList.Add(audioClip);
			}
		}
		if(Instance.m_BGMChaheList.Count == 0)
		{
			foreach (AudioClip audioClip in Resources.LoadAll<AudioClip>("Audios/BGM"))
			{
				Instance.m_BGMChaheList.Add(audioClip);
			}
		}
	}

	/// <summary>
	/// BGMを再生
	/// </summary>
	/// <param name="fileName">ファイル名</param>
	public static void PlayBGM(string fileName)
	{
		GameObject obj = Instantiate(Instance.m_BGMPrefab, Instance.transform);
		obj.name = "BGM_" + fileName;
		AudioSource audioSource = obj.GetComponent<AudioSource>();
		audioSource.clip = Instance.m_BGMChaheList.Where(e => e.name == fileName).FirstOrDefault();
		audioSource.volume = 0.5f;
		if (audioSource.clip == null)
		{
			Debug.LogWarning("指定された名前のオーディオファイルが見つかりませんでした　ファイル名：" + fileName);
			Destroy(obj);
			return;
		}
		audioSource.Play();
	}

	/// <summary>
	/// SEを再生
	/// </summary>
	/// <param name="fileName">ファイル名</param>
	public static void PlaySE(string fileName)
	{
		GameObject obj = Instantiate(Instance.m_SEPrefab, Instance.transform);
		obj.name = "SE_" + fileName;
		AudioSource audioSource = obj.GetComponent<AudioSource>();
		audioSource.clip = Instance.m_SECacheList.Where(e => e.name == fileName).FirstOrDefault();
		audioSource.volume = 1.0f;
		if (audioSource.clip == null)
		{
			Debug.LogWarning("指定された名前のオーディオファイルが見つかりませんでした　ファイル名：" + fileName);
			Destroy(obj);
			return;
		}
		audioSource.Play();
	}

	/// <summary>
	/// BGMの再生中断
	/// </summary>
	public static void PauseBGM()
	{
		UniqueAudioController.Pause();
	}

	/// <summary>
	/// BGMの再生続行
	/// </summary>
	public static void ContinueBGM()
	{
		UniqueAudioController.Continue();
	}

	/// <summary>
	/// BGMを最初から再生する
	/// </summary>
	public static void RePlayBGM()
	{
		UniqueAudioController.RePlay();
	}

	/// <summary>
	/// BGMの音量を調整する
	/// </summary>
	/// <param name="volume">音量</param>
	public static void SetVolumeBGM(float volume)
	{
		UniqueAudioController.SetVolume(volume);
	}


}
