//=============================================================================
//	タイトル  : 弾
//	ファイル名: Bullet.cs
//	作成者    : AT14A341-34 戸部俊太
//	作成日    : 2017/9/11
//=============================================================================
//	更新履歴    -2017/9/11 戸部俊太
//			    V0.01 InitialVersion
//
//				-2017/9/25 戸部俊太
//				・弾が命中した際にBOXタグオブジェクトが削除されるように変更
//				-2017/9/25 吉田幸太郎
//              ・スコア加算を追加
//              -2017/9/25 江澤秋人
//              ・ステージとの当たり判定
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//=============================================================================
//  スクリプト
//=============================================================================
public class Bullet : MonoBehaviour {

    public GameObject Effect;

    [SerializeField]
    Score GameScore;

    // 生成時処理
    void Awake()
	{
	}

	// 初期化処理
	void Start ()
	{
	}
	
	// 更新処理
	void Update ()
	{
	}

	//=========================================================================
	//	関数名: void OnCollisionEnter(Collision collision)
	//	引数  : Collision collision : オブジェクトのコリジョン
	//	戻り値: なし
	//	説明  : 弾のあたり判定処理
	//			箱に当たると削除
	//=========================================================================
	void OnCollisionEnter(Collision collision)
	{
		// 箱とのあたり判定
		if (collision.gameObject.tag == "BOX")
		{

            GameScore = GameObject.Find("Score").GetComponent<Score>();

            GameScore.AddScore(100);
            Debug.Log("弾が命中しました");
			Destroy(collision.gameObject);
		}

        if (collision.gameObject.tag == "Stage")
        {
            Instantiate(Effect, transform.position, Quaternion.identity);
            
        }
    }

	//=========================================================================
	//	関数名: void OnCollisionStay(Collision collision)
	//	引数  : Collision collision : オブジェクトのコリジョン
	//	戻り値: なし
	//	説明  : 弾の削除処理
	//=========================================================================
	void OnCollisionStay(Collision collision)
	{
		Destroy(gameObject);
	}
}
//=============================================================================
//  end of file
//=============================================================================
