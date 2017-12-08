using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DanceArea : MonoBehaviour {
    [SerializeField]
    private Material Circle;

    [SerializeField]
    private MeshRenderer _mesh;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 t = transform.position;
        _mesh.material.SetVector("_Center", t);
        //Circle.SetVector("_Center", new Vector4(t.x, t.y, t.z, 0.0f));
	}
}
