using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.Linq;

public class playercamera : MonoBehaviour {
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
        WAITING,
        END
    };
    //=======================================
    //構造体
    //=======================================
    public enum CAMERAMODE
    {
        WAITING,
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
    private int cameratypeold;
    public static CAMERAMODE m_mode;
    [SerializeField] private DanceCamera[] dancecamera = new DanceCamera[CAMERA_MAX];
    [SerializeField] private CinemachineVirtualCamera[] cinemachineVirtualCamera = new CinemachineVirtualCamera[CAMERA_MAX];
    [SerializeField] private dancecameradolly dancecameradolly;
    Transform retChild; 
    //=======================================
    //関数名 Start
    //引き数
    //戻り値
    //=======================================
    void Start ()
    {
        m_mode = CAMERAMODE.WAITING;
        cameratypeold = 0;


    }
    //=======================================
    //関数名 Update
    //引き数
    //戻り値
    //=======================================
    void Update()
    {
        switch (m_mode)
        {
            //===========================
            //後ろから追従するカメラ
            //===========================
            case CAMERAMODE.WAITING:
                Setblend(1);
                SetCameraPriority((int)CAMERATYPE.WAITING);
                //SetCameraMode(CAMERAMODE.NORMAL);
                break;
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
                    retChild = GameObject.Find("Player1").transform.FindChild("player1").transform.FindChild("locator1").transform;
                    SetFollowTarget(retChild);
                    SetLookAtTarget(retChild);
                    m_mode = CAMERAMODE.DANCE;
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
                    int random = Random.Range(0, CAMERA_MAX - 2);//プレイヤーカメラ、ウェイトカメラを抽選対象から除外
                    while (random == cameratypeold)
                    {
                        random = Random.Range(0, CAMERA_MAX - 2);
                    }
            
                    Debug.Log(random);
                    cameramove.SetCameraNum(random);
                    SetCameraPriority(random);
                    cameratypeold = random;
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
        m_mode = mode;
    }
    //=======================================
    //関数名 ChangeCameraMode
    //引き数 ダンス終了かダンス開始か
    //戻り値
    //=======================================
    public void ChangeCameraMode()
    {
        if (m_mode == CAMERAMODE.NORMAL)
        {
            m_mode = CAMERAMODE.DANCE_INTRO;
            return;
        }
        else if(m_mode == CAMERAMODE.DANCE || m_mode == CAMERAMODE.DANCE_INTRO)
        {
            m_mode = CAMERAMODE.NORMAL;
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
    //=========================
    //ドリーをプレイヤーと同期
    //=========================
    public void SetDollyPosition(Transform transform)
    {
        dancecameradolly.SetPosition(transform);
    }
    public static CAMERAMODE GetCameraMode()
    {
        return m_mode;
    }
}
