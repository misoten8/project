using System.Collections;
using System.Collections.Generic;
using TextFx;
using UnityEngine;
using Misoten8Utility;

/// <summary>
/// NavAnnounceInfo クラス
/// </summary>
public class NavAnnounceInfo : MonoBehaviour 
{
	[SerializeField]
	List<string> _messages = new List<string>();

	[SerializeField]
	private TextFxTextMeshProUGUI _message;

	public uint readIndex = 0;

	public bool isRun = true;

	public void Start()
	{
		StartCoroutine(StepDo());
	}

	private IEnumerator StepDo()
	{
		do
		{
			// メッセージの設定
			_message.SetText(_messages[(int)readIndex]);

			// メッセージのスクロールアニメーション
			_message.AnimationManager.PlayAnimation();

			readIndex++;
			if(_messages.Count >= readIndex)
			{
				readIndex = 0;
			}
			yield return new WaitForSeconds(10.0f);
		} while (isRun);
	}
}