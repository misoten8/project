using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dancecameradolly : MonoBehaviour {

    private void OnGUI()
    {
        GUI.Label(new Rect(new Vector2(0, 150), new Vector2(300, 200)), "dancecameradollyPos" + transform.position.ToString());
    }
    public void SetPosition(Transform transf)
    {
        Vector3 pos = transf.position;
        pos.y += 3;
        pos.z += 10;
        transform.position = pos;
    }
}
