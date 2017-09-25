//=============================================================================
//	タイトル  : 発射装置
//				MainCameraにアタッチして使用
//	ファイル名: Gun.cs
//	作成者    : AT14A341-34 戸部俊太
//	作成日    : 2017/9/11
//=============================================================================
//	更新履歴    -2017/9/11 戸部俊太
//			    V0.01 InitialVersion
//
//				-2017/9/25 戸部俊太
//				・弾の発射処理を外部から(非)有効化できるように変更
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//=============================================================================
//  スクリプト
//=============================================================================
public class Gun : MonoBehaviour {

	// 弾プレハブ(Bulletをアタッチ)
	public GameObject BulletPrefab;

	// 移動量
	public float BulletMovement = 5.0f;

	// 弾を発射処理が有効であるかないか
	private bool Active;

	//　生成時処理
	void Awake()
	{
	}

	// 初期化処理
	void Start ()
	{
		Active = true;
	}
	
	// 更新処理
	void Update ()
	{
		// 弾発射処理
		if (Active == true && Input.GetMouseButtonDown(0))
		{
			// 弾生成
			GameObject Bullet = Instantiate(BulletPrefab);

			// マウス座標取得
			Vector3 screenPoint = Input.mousePosition;
			screenPoint.z = 10.0f;

			// カメラのワールド座標を取得
			Vector3 worldPoint = GetComponent<Camera>().ScreenToWorldPoint(screenPoint);
			Vector3 direction = (worldPoint - transform.position);

			// 弾の移動量を設定
			Bullet.GetComponent<Rigidbody>().velocity = direction * BulletMovement;
		}
	}

	//=========================================================================
	//	関数名: public void GunActive(bool flag)
	//	引数  : bool flag : 発射処理を有効にするかしないか
	//	戻り値: なし
	//	説明  : 発射処理の(非)有効化関数
	//=========================================================================
	public void GunActive(bool flag)
	{
		Active = flag;
	}
}
//=============================================================================
//  end of file
//=============================================================================