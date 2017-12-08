using UnityEngine;
using System.Linq;

/// <summary>
/// 複数ディスプレイ対応 ビルボード クラス
/// このスクリプトのアタッチ先は対象オブジェクトの親にしてください
/// 製作者：実川
/// </summary>
public class Billboard : MonoBehaviour
{
	[SerializeField]
	private int _targetDisplayIndex;

	private Transform _camera;

	private void Start()
	{
		_camera = Camera.allCameras.First(x => x.targetDisplay == _targetDisplayIndex).transform;
	}

	void Update()
	{
		Vector3 p = _camera.position;
		p.y = transform.position.y;
		transform.LookAt(p);
	}
}
