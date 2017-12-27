using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dancecameradolly : MonoBehaviour {

    public void SetPosition(Transform transf)
    {
        Vector3 pos = transf.position;
        pos.x += 5;
        pos.y += 1;
        pos.z += 4;
        transform.position = pos;
    }
}
