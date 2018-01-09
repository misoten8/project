﻿//=============================================================================
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

	/// <summary>
	/// 登録されているマーカーの数
	/// </summary>
	public int MarkerNum
	{
		get { return _markerNum; }
	}

	private int _markerNum = 0;

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
		_markerNum++;
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

	/// <summary>
	/// 目標地点を設定する
	/// </summary>
	public void SetTargetMarker(NavMeshAgent agent, byte index)
	{
		if(_markerNum >= index)
		{
			Debug.LogWarning("不正な番号が指定されました\n");
			return;
		}
		agent.destination = _markers[index].transform.position;
	}

	/// <summary>
	/// 目標地点の取得
	/// </summary>
	public Vector3 GetMarker(byte index)
	{
		if (_markerNum >= index)
		{
			Debug.LogWarning("不正な番号が指定されました\n");
			return new Vector3();
		}
		return _markers[index].transform.position;
	}

	/// <summary>
	/// ランダムな目標地点の取得
	/// </summary>
	public Vector3 GetMarkerRandom()
	{
		int index = UnityEngine.Random.Range(0, MarkerNum);

		return _markers[index].transform.position;
	}
}
//=============================================================================
//	end of file
//=============================================================================
