using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ando : MonoBehaviour {

    public ParticleSystem Effect;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

    }

    void OnDestroy()
    {
        Instantiate(Effect, transform.position, Quaternion.identity);
        //Effect.Play();
    }
}
