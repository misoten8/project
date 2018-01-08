using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.Linq;

public class resultcamera : MonoBehaviour
{
    //=======================================
    //構造体
    //=======================================
    public enum CAMERAMODE
    {
        RESULTS_ANNOUNCE,
        RESULTS_ANNOUNCE2,
        END
    };
    //=======================================
    //プロパティ
    //=======================================
    public CinemachineBrain CameraBrain
    {
        get { return brain; }
    }

    //=======================================
    //グローバル変数
    //=======================================
    public const int PRIORITY_HIGH = 15; // 優先度 高
    public const int PRIORITY_LOW = PRIORITY_HIGH - 1; // HIGHより低ければなんでもOK
    public const int CAMERA_MAX = (int)CAMERAMODE.END; //カメラの最大数
    private const int CHANGE_TIME = 4;//モードを変える時間
    private float changetime;   //動きを更新する時刻 
    private CinemachineBrain brain;
    public static CAMERAMODE m_mode;
    [SerializeField] private DanceCamera[] dancecamera = new DanceCamera[CAMERA_MAX];
    [SerializeField] private CinemachineVirtualCamera[] cinemachineVirtualCamera = new CinemachineVirtualCamera[CAMERA_MAX];
    [SerializeField] private ResultRanking _resultRanking;
    [SerializeField] private Transform[] _playerPosition = new Transform[3];
    private void Awake()
    {
        if ((int)_resultRanking.GetWinner() == 1)
            SetAnnounceTarget(_playerPosition[0]);
        if ((int)_resultRanking.GetWinner() == 2)
            SetAnnounceTarget(_playerPosition[1]);
        if ((int)_resultRanking.GetWinner() == 3)
            SetAnnounceTarget(_playerPosition[2]);
    }
    //=======================================
    //関数名 Start
    //引き数
    //戻り値
    //=======================================
    void Start()
    {
        m_mode = CAMERAMODE.RESULTS_ANNOUNCE;

    
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
            //1位がアピールするカメラ
            //===========================
            case CAMERAMODE.RESULTS_ANNOUNCE:
                Setblend(1);
                SetCameraPriority((int)CAMERAMODE.RESULTS_ANNOUNCE);
                break;
            //===========================
            //全員が見えるカメラ
            //===========================
            case CAMERAMODE.RESULTS_ANNOUNCE2:
                Setblend(1);
                SetCameraPriority((int)CAMERAMODE.RESULTS_ANNOUNCE2);
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
        for (int i = 0; i < CAMERA_MAX-1; i++)
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
    public void SetFollowTarget(Transform transform)
    {
        cinemachineVirtualCamera.ToList().ForEach(e => e.Follow = transform);
    }
    public void SetLookAtTarget(Transform transform)
    {
        cinemachineVirtualCamera.ToList().ForEach(e => e.LookAt = transform);
    }
    public void SetAnnounceTarget(Transform transform)
    {
        cinemachineVirtualCamera[0].Follow = transform;
        cinemachineVirtualCamera[0].LookAt = transform;
    }

}
