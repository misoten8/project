using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AndoGenerator : MonoBehaviour {

    //プレハブ（白と赤のCube（ボックス））を変数に代入
    public GameObject AndoPrehub;//白のCube（ボックス）

    private float time;//ボックス生成時間

    void Start()
    {
        time = 0.5f;
    }

    void Update()
    {
        //0.5秒間隔でボックス生成。60秒で120個のボックス生成。
        time -= Time.deltaTime;
        if (time <= 0.0)
        {
            time = 0.5f;

            //ボックスの出現座標
            float x = Random.Range(-2.27f, -11.78f);
            float y = 0.0f;
            float z = Random.Range(-2.27f, -11.78f);

            //ボックス生成
            Instantiate(AndoPrehub, new Vector3(x, y, z), Quaternion.identity);

        }
    }
}
