using UnityEngine;
using Cinemachine;
//using UnityEditor;

public class cameramove : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    public CinemachinePath path;
    public AnimationCurve curve;
   // public float velocity;
    public static float currentDistance = 0;
    public static int cameraNum = 0;
    private float pathLength;
    private static CinemachineTrackedDolly dolly;
    public static float dollytime = 0.0f;

    void SamplePath(int stepsPerSegment)
    {
        curve = new AnimationCurve();
        float minPos = path.MinPos;
        float maxPos = path.MaxPos;
        float stepSize = 1f / Mathf.Max(1, stepsPerSegment);

        pathLength = 0;
        Vector3 p0 = path.EvaluatePosition(0);
        curve.AddKey(new Keyframe(0, 0));
        for (float pos = minPos + stepSize; pos < (maxPos + stepSize / 2); pos += stepSize)
        {
            Vector3 p = path.EvaluatePosition(pos);
            pathLength += Vector3.Distance(p0, p);
            curve.AddKey(new Keyframe(pathLength, pos));
            p0 = p;
        }
        #if UNITY_EDITOR
        for (int i = 0; i < curve.keys.Length; i++)
        {
            //TODO: replace this with something that does not depend on editor
            //AnimationUtility.SetKeyLeftTangentMode(curve, i, AnimationUtility.TangentMode.Linear);
            //AnimationUtility.SetKeyRightTangentMode(curve, i, AnimationUtility.TangentMode.Linear);
        }
        #endif
    }
    void Start()
    {
        cameraNum = 0;
        dolly = virtualCamera.GetCinemachineComponent<CinemachineTrackedDolly>();
        if (path != null)
            SamplePath(path.m_Appearance.steps); // TODO: decouple numSteps from appearance setting
        currentDistance = 0;
    }
    void Update()
    {
        int numKeys = (curve != null && curve.keys != null) ? curve.keys.Length : 0;
        if (dolly != null && numKeys > 0 && pathLength > Vector3.kEpsilon)
        {
            currentDistance = dollytime;
            currentDistance = currentDistance % pathLength;
            dolly.m_PathPosition = curve.Evaluate(currentDistance);
        }
        if (cameraNum == 2 && playercamera.GetCameraMode() == playercamera.CAMERAMODE.DANCE)
        {
            dollytime += 0.06f;
        }
        else
        {
            InitDolly();
        }
    }
    void InitDolly()
    {
        dolly.m_PathPosition = 0.00f;
        dollytime = 0.00f;
        currentDistance = 0.00f;
    }
    public static void SetCameraNum(int num)
    {
        cameraNum = num;
    }

}

