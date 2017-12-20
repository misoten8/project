//=============================================================================
//	タイトル  : マーカーオブジェクト制御スクリプト
//	ファイル名: Marker.cs
//	作成者    : AT14A341 戸部俊太
//	作成日    : 2017/12/20
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//=============================================================================
//	スクリプト
//=============================================================================
public class Marker : MonoBehaviour {

    private MarkerManager _markerManager = null;

    // 初期化処理
    void Start()
    {
        _markerManager = GameObject.Find("MobControlleMarker").GetComponent<MarkerManager>();
        _markerManager.SetMarker(this);
    }

    // Gizmo描画
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawSphere(this.transform.position, 2.5f);
    }
}
//=============================================================================
//	end of file
//=============================================================================