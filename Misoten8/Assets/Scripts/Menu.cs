using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Menu クラス
/// 製作者：実川
/// </summary>
public class Menu : MonoBehaviour
{
	void Start ()
	{
		
	}
	
	void Update ()
	{
		
	}

	private void OnGUI()
	{
		GUI.Label(new Rect(new Vector2(0, 0), new Vector2(300, 200)), "Menu Scene");
	}

	public void TransScene()
	{
		SceneManager.LoadScene("Battle");
	}
}
