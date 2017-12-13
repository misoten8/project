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
    private float _shakeparameter;

    //=====================================
    // Use this for initialization
    //=====================================
    void Start ()
	{
        _shakeparameter = 0;
	}
    //=====================================
    // Update is called once per frame
    //=====================================
    void Update ()
	{

		//時間経過で数値が０になるようにする
		_shakeparameter -= Time.deltaTime;

		//0以下にならないようにする
		_shakeparameter = Mathf.Max(_shakeparameter, 0.0f);

		//リモコンを振るとゲージが足される
		if (Input.GetKeyDown("return") || WiimoteManager.GetSwing(0))  
        {
            _shakeparameter += 1.0f;
        }
	}
    //=====================================
    //関数名：GetShakeParameter()
    //説明　：_shakeparameterを取得できる
    //=====================================
    public static float GetShakeParameter()
    {
        return Instance._shakeparameter;
    }
	//=====================================
	//関数名：IsShakeMax
	//説明　：渡された引数と現在の_shakeparameterを比較
	//　　　　渡された値より大きければtrueを返却
	//=====================================
	public static bool IsOverWithValue(int value)
    {
		return (int)Instance._shakeparameter >= value;
	}
	//=====================================
	//関数名：IsShakeMax
	//説明　：渡された引数と現在の_shakeparameterを比較
	//　　　　渡された値より大きければtrueを返却
	//=====================================
	public static bool IsOverWithValue(float value)
	{
		return Instance._shakeparameter >= value;
	}
	//=====================================
	//関数名：ResetShakeParameter
	//説明　：_shakeparameterの初期化
	//=====================================
	public static void ResetShakeParameter()
	{
		Instance._shakeparameter = 0.0f;
	}
}
