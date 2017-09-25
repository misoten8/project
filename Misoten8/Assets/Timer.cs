using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour {

    [SerializeField]
    private Text TimerText;       //Timer用変数
    private float GameTime = 60;  //秒数用
                                 // Use this for initialization
    void Start () {
		
	}

    // Update is called once per frame
    void Update() {
        if ( GameTime > 0.01 )
        { 
            GameTime -= Time.deltaTime; //スタートしてからの秒数を格納
            GetComponent<Text>().text = GameTime.ToString("F2"); //小数2桁にして表示
        }
    }

   /// <summary>
   /// 現在の残り時間を取得する。
   /// </summary>
   public float GetTimer()
   {
       return GameTime;
   }

   public void AddTimer( float value )
   {
       GameTime += value;
   }
}
