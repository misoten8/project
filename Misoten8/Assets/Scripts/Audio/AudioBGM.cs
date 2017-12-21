using UnityEngine;

/// <summary>
/// AudioBGM クラス
/// </summary>
public class AudioBGM : MonoBehaviour 
{
	public AudioSource AudioSource
	{
		get { return _audioSource; }
	}

	[SerializeField]
	private AudioSource _audioSource;

	public BeatControllerSections BeatControllerSections
	{
		get { return _beatControllerSections; }
	}

	[SerializeField]
	private BeatControllerSections _beatControllerSections;
}