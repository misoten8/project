using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.Linq;

public class cameramanager : MonoBehaviour {
    //=======================================
    //構造体
    //=======================================
    public enum CAMERATYPE
    {
        DANCE1,
        DANCE2,
        DANCE3,
        DANCE4,
        PLAYER,
        END
    };
    //=======================================
    //構造体
    //=======================================
    public enum CAMERAMODE
    {
        NORMAL,
        DANCE_INTRO,
        DANCE,
        END
    };
    //=======================================
    //グローバル変数
    //=======================================
    public const int PRIORITY_HIGH      = 15; // 優先度 高
    public const int PRIORITY_LOW       = PRIORITY_HIGH - 1; // HIGHより低ければなんでもOK
    public const int CAMERA_MAX  = (int)CAMERATYPE.END; //カメラの最大数
    private const int CHANGE_TIME = 4;//モードを変える時間
    private float changetime;   //動きを更新する時刻
    private CinemachineBrain brain;
    private CAMERAMODE g_mode;
    [SerializeField] private DanceCamera[] dancecamera = new DanceCamera[CAMERA_MAX];
	[SerializeField] private CinemachineVirtualCamera[] cinemachineVirtualCamera = new CinemachineVirtualCamera[CAMERA_MAX];
	//=======================================
	//関数名 Start
	//引き数
	//戻り値
	//=======================================
	void Start ()
    {
        g_mode = CAMERAMODE.NORMAL;
    }
    //=======================================
    //関数名 Update
    //引き数
    //戻り値
    //=======================================
    void Update()
    {
        switch (g_mode)
        {     
            //===========================
            //後ろから追従するカメラ
            //===========================
            case CAMERAMODE.NORMAL:
                Setblend(1);
                SetCameraPriority((int)CAMERATYPE.PLAYER);
                break;
            //===========================
            //プレイヤーを前から撮影
            //===========================
            case CAMERAMODE.DANCE_INTRO:
                SetCameraPriority((int)CAMERATYPE.DANCE1);//０番目のカメラを優先表示
                //一定時間経過でランダム
                if (changetime < Time.time)
                {
                    g_mode = CAMERAMODE.DANCE;
                    changetime = Time.time + CHANGE_TIME;  //次の更新時刻を決める
                }
                break;
            //====================================
            //こっからカメラランダム＆切り替わり減衰無し
            //====================================
            case CAMERAMODE.DANCE:
                if (changetime < Time.time)
                {
                    Setblend(0);
                    int random = Random.Range(0, CAMERA_MAX-1);
                    Debug.Log(random);
                    SetCameraPriority(random);
                    changetime = Time.time + CHANGE_TIME;  //次の更新時刻を決める
                }
                break;
        }
    }
    //=======================================
    //関数名 Update
    //引き数
    //戻り値
    //=======================================
    void Setblend(int num)
    {
        brain = FindObjectOfType<CinemachineBrain>();
        brain.m_DefaultBlend.m_Time = num; // 0 Time equals a cut
    }
    //=======================================
    //関数名 SetCameraPriority
    //引き数 num番のカメラを優先表示する
    //戻り値
    //=======================================
    void SetCameraPriority(int type)
    {
        for (int i = 0; i < CAMERA_MAX; i++)
        {
            dancecamera[i].SetPriority(PRIORITY_LOW);
        }
        dancecamera[type].SetPriority(PRIORITY_HIGH);
    }
    //=======================================
    //関数名 SetCameraMode
    //引き数 カメラのモードを設定
    //戻り値
    //=======================================
    public void SetCameraMode(CAMERAMODE mode)
    {
        g_mode = mode;
    }
    //=======================================
    //関数名 ChangeCameraMode
    //引き数 ダンス終了かダンス開始か
    //戻り値
    //=======================================
    public void ChangeCameraMode()
    {
        if (g_mode == CAMERAMODE.NORMAL)
        {
            g_mode = CAMERAMODE.DANCE_INTRO;
            return;
        }
        else if(g_mode == CAMERAMODE.DANCE || g_mode == CAMERAMODE.DANCE_INTRO)
        {
            g_mode = CAMERAMODE.NORMAL;
            return;
        }

    }
	public void SetFollowTarget(Transform transform)
	{
		cinemachineVirtualCamera.ToList().ForEach(e => e.Follow = transform);
	}
	public void SetLookAtTarget(Transform transform)
	{
		cinemachineVirtualCamera.ToList().ForEach(e => e.LookAt = transform);
	}
}
