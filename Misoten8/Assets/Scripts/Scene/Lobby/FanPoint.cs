using UnityEngine;
using Misoten8Utility;
using System.Linq;

/// <summary>
/// FanPoint クラス
/// 製作者：実川
/// </summary>
public class FanPoint : MonoBehaviour
{
	public Color[] MeterColor
	{
		get { return _meterColor; }
	}

	[SerializeField]
	private Mob _people;

	/// <summary>
	/// マテリアルを設定する対象メッシュ
	/// </summary>
	[SerializeField]
	private MeshRenderer _meshRenderer;

	/// <summary>
	/// インポートするマテリアル(複製するので、このマテリアルは直接使用しない)
	/// </summary>
	[SerializeField]
	private Material _importMaterial;

	[SerializeField]
	private Color[] _meterColor;

	/// <summary>
	/// 複製したマテリアル
	/// </summary>
	private Material _localMaterial;

	private void Start()
	{
		_localMaterial = new Material(_importMaterial);
		_meshRenderer.material = _localMaterial;

		_localMaterial.SetColorArray("_MeterColor", _meterColor);

		UpdateMaterial();
	}

	public void UpdateMaterial()
	{
		float[] i = _people.InterpolationFanPointArray;
		float[] value = new float[5]
		{
			i[0],
			i.ElementsRange(0, 2).Sum(),
			i.ElementsRange(0, 3).Sum(),
			i.ElementsRange(0, 4).Sum(),
			i.ElementsRange(0, 5).Sum()
		};

		_localMaterial.SetFloatArray("_BorderValue", value);
	}
}
