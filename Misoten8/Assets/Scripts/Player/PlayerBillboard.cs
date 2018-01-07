using UnityEngine;
using System.Linq;
/// <summary>
/// PlayerBillboard クラス
/// </summary>
public class PlayerBillboard : MonoBehaviour 
{
	private Transform _camera = null;

	[SerializeField]
	private TextMesh _textMesh;

	public void OnAwake(Transform targetCamera, Player player)
	{
		_camera = targetCamera;
		_textMesh.text = ((int)player.Type).ToString() + "P";
	}

	void Update()
	{
		if (_camera == null)
			return;

		Vector3 p = _camera.position;
		p.y = transform.position.y;
		transform.LookAt(p);
	}
}