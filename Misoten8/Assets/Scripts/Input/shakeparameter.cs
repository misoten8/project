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
    //=======================================
    //構造体
    //=======================================
    private enum PARAMETERMODE
    {
        NORMAL,
        NOT_DECREASE,
        END
    };
    //=====================================
    //変数
    //=====================================
    private float _shakeparameter;
    private PARAMETERMODE _mode;
    private float _changetime;
    public static bool _active;
    //=====================================
    // Use this for initialization
    //=====================================
    void Start ()
	{
        _shakeparameter = 0;
        _mode = PARAMETERMODE.NORMAL;
        _changetime = 0;
        _active = true;
       
	}
    //=====================================
    // Update is called once per frame
    //=====================================
    void Update ()
	{
        if (_active == true)
        {
            //0以下にならないようにする
            _shakeparameter = Mathf.Max(_shakeparameter, 0.0f);

            //リモコンを振るとゲージが足される
            if (Input.GetKeyDown("return") || WiimoteManager.GetSwing(0))
            {
                _shakeparameter += 1.0f;
                _mode = PARAMETERMODE.NOT_DECREASE;

            }

            //一定時間たつとゲージが減るように戻る
            switch (_mode)
            {
                case PARAMETERMODE.NOT_DECREASE:
                    _changetime += Time.deltaTime;
                    if (_changetime >= REDUCTION_TIME)
                    {
                        _mode = PARAMETERMODE.NORMAL;
                        _changetime = 0.0f;
                    }
                    break;
                case PARAMETERMODE.NORMAL:
                    _shakeparameter -= Time.deltaTime;
                    break;

            }
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
    //=====================================
    //関数名：SetActive
    //説明　：_shakeparameterの有効/非有効の切り替え
    //=====================================
    public static void SetActive(bool active)
    {
        _active = active;
    }
}
