using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AndoGenerator : MonoBehaviour {

    //プレハブ（白と赤のCube（ボックス））を変数に代入
    public GameObject AndoPrehub;//白のCube（ボックス）
    public float WitdhHeight;   // ジェネレータからこの変数の範囲内に生成する
    public bool stop;

    private float time;//ボックス生成時間

    void Start()
    {
        WitdhHeight = 10.0f;
        time = 0.5f;
        stop = false;
    }

    void Update()
    {
        //0.5秒間隔でボックス生成。60秒で120個のボックス生成。
        if ( !stop )
        {
            time -= Time.deltaTime;
            if (time <= 0.0)
            {
                time = 0.5f;

                //ボックスの出現座標
                float x = Random.Range(transform.position.x - WitdhHeight, transform.position.x + WitdhHeight);
                float y = transform.position.y;
                float z = Random.Range(transform.position.z - WitdhHeight, transform.position.z + WitdhHeight);

                //ボックス生成
                Instantiate(AndoPrehub, new Vector3(x, y, z), Quaternion.identity);

            }
        }
    }

    public void StopGenerate()
    {
        stop = true;
    }
}
