using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ゲーム中に生成される同期オブジェクト
/// </summary>
public class PhotonObjectBase : Photon.MonoBehaviour 
{
	/// <summary>
	/// 全ユーザーに同期されるオブジェクトID
	/// </summary>
	protected uint objectID = 0;

	/// <summary>
	/// 初期化処理
	/// </summary>
	protected void OnInitialize()
	{
		
	}
}