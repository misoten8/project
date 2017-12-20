//=============================================================================
//	タイトル  : マーカーマネージャー
//	ファイル名: MarkerManager.cs
//	作成者    : AT14A341 戸部俊太
//	作成日    : 2017/12/20
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//=============================================================================
//	スクリプト
//=============================================================================
public class MarkerManager : MonoBehaviour {

    // マーカーリスト
    public List<Marker> Markers
    {
        get
        {
            return _markers;
        }
    }
    private List<Marker> _markers = new List<Marker>();

	// 初期化処理
	void Start ()
    {
	}
	
	// 更新処理
	void Update () {
		
	}

    //=============================================================================
    //	関数名:void SetMarker(Marker marker)
    //	引数  :Marker marker : リストに追加するインスタンス
    //	戻り値:なし
    //	説明  :マーカーリスト追加処理
    //=============================================================================
    public void SetMarker(Marker marker)
    {
        _markers.Add(marker);
    }

    //=============================================================================
    //	関数名:public int GotoNextPoint(NavMeshAgent agent)
    //	引数  :NavMeshAgent agent : 目標値を設定するagent
    //	戻り値:rand : 設定した目標値のインデックス番号を返す
    //	説明  :NavMeshAgent目標値設定処理関数
    //=============================================================================
    public int GotoNextPoint(NavMeshAgent agent)
    {
        int rand = 0;
        rand = UnityEngine.Random.Range(0, _markers.Count);
        agent.destination = _markers[rand].transform.position;
        return rand;
    }
}
//=============================================================================
//	end of file
//=============================================================================
