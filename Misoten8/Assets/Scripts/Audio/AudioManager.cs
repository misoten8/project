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
	private GameObject SEPrefab;
	[SerializeField]
	private BeatController m_beatController;

	private static readonly Dictionary<BGMType, string> _BGM_PATH_MAP = new Dictionary<BGMType, string>
	{
		{ BGMType.Title, "TitleBGM" },
		{ BGMType.Lobby, "LobbyBGM" },
		{ BGMType.Battle, "BattleBGM" },
		{ BGMType.Result, "ResultBGM" }
	};

	private static readonly Dictionary<SEType, string> _SE_PATH_MAP = new Dictionary<SEType, string>
	{
		{ SEType.Click, "button" },
	};

	private const string _BGM_PATH = "Prefab/Audios/BGM/";
	private const string _SE_PATH = "Audios/SE/";

	/// <summary>
	/// BGMのキャッシュリスト
	/// </summary>
	private Dictionary<BGMType, AudioBGM> _bgms = new Dictionary<BGMType, AudioBGM>();

	private Dictionary<BGMType, GameObject> _BGMPrefabs = new Dictionary<BGMType, GameObject>();

	/// <summary>
	/// サウンドのAudioClipキャッシュリスト
	/// </summary>
	private List<AudioClip> m_SECacheList = new List<AudioClip>();

	void Start()
	{
		Load();
	}

	/// <summary>
	/// 使用するオーディオを事前に読み込む
	/// </summary>
	public static void Load()
	{
		// BGM
		if (Instance._bgms.Count == 0)
		{
			//foreach (var audioMap in Resources.LoadAll<GameObject>(_BGM_PATH))
			//{
			//	string path = _BGM_PATH + audioMap.Value;
			//	var audioPrefab = Resources.Load(path) as GameObject;
			//	if(audioPrefab == null)
			//	{
			//		Debug.LogWarning("audioPrefabが取得できませんでした	指定パス：" + path);
			//	}
			//	Instance._BGMPrefabs.Add(audioMap.Key, audioPrefab);
			//}
		}
		// SE
		if (Instance.m_SECacheList.Count == 0)
		{
			foreach (AudioClip audioClip in Resources.LoadAll<AudioClip>(_SE_PATH))
			{
				Instance.m_SECacheList.Add(audioClip);
			}
		}
	}

	/// <summary>
	/// BGMを再生
	/// </summary>
	/// <param name="fileName">ファイル名</param>
	public static void Play(BGMType type)
	{
		GameObject obj = Instantiate(Instance._BGMPrefabs[type].gameObject, Instance.transform);
		obj.name = "BGM_" + _BGM_PATH_MAP[type];
		AudioBGM audio = obj.GetComponent<AudioBGM>();
		if (audio == null)
		{
			Debug.LogWarning("指定された名前のBGMファイルが見つかりませんでした　ファイル名：" + _BGM_PATH_MAP[type]);
			Destroy(obj);
			return;
		}
		audio.AudioSource.Play();
	}

	/// <summary>
	/// SEを再生
	/// </summary>
	/// <param name="fileName">ファイル名</param>
	public static void Play(SEType type)
	{
		GameObject obj = Instantiate(Instance.SEPrefab, Instance.transform);
		obj.name = "SE_" + _SE_PATH_MAP[type];
		AudioSource audioSource = obj.GetComponent<AudioSource>();
		audioSource.clip = Instance.m_SECacheList.Where(e => e.name == _SE_PATH_MAP[type]).FirstOrDefault();
		audioSource.volume = 1.0f;
		if (audioSource.clip == null)
		{
			Debug.LogWarning("指定された名前のオーディオファイルが見つかりませんでした　ファイル名：" + _SE_PATH_MAP[type]);
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
