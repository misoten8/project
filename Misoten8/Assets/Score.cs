using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour {

	[SerializeField]
    private Text ScoreText;       //Text用変数
    private int  GameScore = 0;	 //スコア用
	
    // Use this for initialization
	void Start () {
        ScoreText.text = "Score: 0"; //初期スコアを代入して画面に表示
    }
	
	// Update is called once per frame
	void Update () {

        ScoreText.text = "Score: " + GameScore.ToString();
	}
    /// <summary>
    /// セットされた引き数の値を加算する
    /// </summary>
    public void AddScore( int value )
    {
        GameScore += value;
    }

    /// <summary>
    /// スコア取得
    /// </summary>
    public int GetScore()
    {
        return GameScore;
    }
}
