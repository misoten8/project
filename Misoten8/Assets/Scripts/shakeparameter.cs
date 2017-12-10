using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WiimoteApi;
/// <summary>
/// wiiリモコンの動作クラス
/// 製作者：安藤
/// </summary>
public class shakeparameter : SingletonMonoBehaviour<shakeparameter>
{
    //=====================================
    //マクロ定義
    //=====================================
    private const int REDUCTION_TIME = 1;//モードを変える時間
    //=====================================
    //変数
    //=====================================
    static int _shakeparameter;
    private float _changetime;
    //=====================================
    // Use this for initialization
    //=====================================
    void Start () {
        _shakeparameter = 0;
        _changetime = 0;
	}
    //=====================================
    // Update is called once per frame
    //=====================================
    void Update () {

        if (_changetime < Time.time && _shakeparameter > 0) //時間経過で数値が０になるようにする
        {
            _shakeparameter--;                         //ゲージ減少
            _changetime = Time.time + REDUCTION_TIME;  //次の更新時刻を決める
        }
        if (Input.GetKeyDown("return") || WiimoteManager.GetSwing(0))  //リモコンを振るとゲージが足される
        {
            _shakeparameter++;
        }
        //Debug.Log(_shakeparameter);
	}
    //=====================================
    //関数名：GetShakeParameter()
    //説明　：_shakeparameterを取得できる
    //=====================================
    public static int GetShakeParameter()
    {
        return _shakeparameter;
    }
    //=====================================
    //関数名：IsShakeMax
    //説明　：渡された引数と現在の_shakeparameterを比較
    //　　　　渡された値より大きければtrueを返却
    //=====================================
    public static bool IsCompareWithValue(int value)
    {
        if (_shakeparameter >= value)
            return true;
        else
            return false;
    }

	// 仮
	public static void ResetShakeParameter()
	{
		_shakeparameter = 0;
	}
}
