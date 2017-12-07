using UnityEngine;

/// <summary>
/// オーディオコンポーネント
/// 再生終了時に消去するクラス
/// 製作者：実川
/// </summary>
public class AudioDestroyer : MonoBehaviour
{
	[SerializeField]
    AudioSource audioSource;

    void Update()
    {
		if (audioSource.isPlaying) return;

		//再生終了したら消します
		Destroy(gameObject);
    }
}
