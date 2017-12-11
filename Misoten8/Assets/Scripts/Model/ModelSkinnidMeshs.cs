using UnityEngine;

/// <summary>
/// 使用するスキンメッシュ登録クラス
/// </summary>
public class ModelSkinnidMeshs : MonoBehaviour 
{
	public SkinnedMeshRenderer[] SkinnedMeshs
	{
		get { return _skinnedMeshs; }
	}

	[SerializeField]
	private SkinnedMeshRenderer[] _skinnedMeshs;
}