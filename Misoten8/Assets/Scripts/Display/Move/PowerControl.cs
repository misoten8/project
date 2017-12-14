using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerControl : MonoBehaviour {

    private float _Player1Power = 1.0f;
    private float _Player2Power = 2.0f;
    private float _Player3Power = 3.0f;




    public float Player1Power
    {
        get { return _Player1Power; }
    }
    public float Player2Power
    {
        get { return _Player2Power; }
    }
    public float Player3Power
    {
        get { return _Player3Power; }
    }

    public float GetPlayerPower(int No)
    {
        float AllPower =   Player1Power + Player2Power + Player3Power;
        float[] Power  = { Player1Power , Player2Power , Player3Power };
        if (AllPower > 0.0f) Power[ No - 1 ] = Power[ No - 1 ] / AllPower;
        return Power[ No - 1 ];
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        
        //UIobj = GetComponent<Image>();
        //UIobj.value = GetPlayerPower(No);
    }
}
