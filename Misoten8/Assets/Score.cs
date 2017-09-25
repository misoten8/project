using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour {

	[SerializeField]
    public Text ScoreText;       //Text用変数
    public int  GameScore = 0;	 //スコア用
	
    // Use this for initialization
	void Start () {
        ScoreText.text = "Score: 0"; //初期スコアを代入して画面に表示
    }
	
	// Update is called once per frame
	void Update () {

        ScoreText.text = "Score: " + GameScore.ToString(); 
		GameScore += 1 ;
	}
}
