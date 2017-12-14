using UnityEngine;
using Cinemachine;
using UnityEditor;

public class cameramove : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    public CinemachinePath path;
    public AnimationCurve curve;
    public float velocity;
    public float currentDistance;

    private float pathLength;
    private CinemachineTrackedDolly dolly;

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

        for (int i = 0; i < curve.keys.Length; i++)
        {
            // TODO: replace this with something that does not depend on editor
            AnimationUtility.SetKeyLeftTangentMode(curve, i, AnimationUtility.TangentMode.Linear);
            AnimationUtility.SetKeyRightTangentMode(curve, i, AnimationUtility.TangentMode.Linear);
        }
    }

    void Start()
    {
        dolly = virtualCamera.GetCinemachineComponent<CinemachineTrackedDolly>();
        if (path != null)
            SamplePath(path.m_Appearance.steps); // TODO: decouple numSteps from appearance setting
    }

    void Update()
    {
        int numKeys = (curve != null && curve.keys != null) ? curve.keys.Length : 0;
        if (dolly != null && numKeys > 0 && pathLength > Vector3.kEpsilon)
        {
            currentDistance += velocity * Time.deltaTime;
            currentDistance = currentDistance % pathLength;
            dolly.m_PathPosition = curve.Evaluate(currentDistance);
        }
    }
}

