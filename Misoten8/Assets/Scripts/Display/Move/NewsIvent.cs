using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NewsIvent : MonoBehaviour {

    [SerializeField]
    private Transform ParentCanvas;
    [SerializeField]
    public GameObject NewsBackGround;
    [SerializeField]
    public GameObject Textmeshpro;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void NewsCreate()
    {
        //News背景生成
       //GameObject News;
       //TextMeshProUGUI component;
       //instance = Instantiate(Textmeshpro, ParentCanvas);
       //component = instance.GetComponent<TextMeshProUGUI>();
       ////News文字生成
       //GameObject image;
       //image = Instantiate(NewsBackGround, ParentCanvas);
       //instance = Instantiate(Textmeshpro, ParentCanvas);
       //component = instance.GetComponent<TextMeshProUGUI>();
    }

}
