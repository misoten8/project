using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class MoveIventManager : MonoBehaviour {
    
    [SerializeField]
    static public TextMeshProUGUI textmeshpro;

    //public Action onHoge;

    // Use this for initialization
    void Start()
    {

       //if(onHoge != null)
       //{
       //    onHoge.Invoke();
       //}
       //
       //onHoge?.Invoke();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    


    public void TextSet(GameObject parent, Text information)
    {
    }


    public void IventSet()
    {
        //Textmeshpro. = InputText.text;
        // GameObject instance;
        // TextMeshProUGUI component;
        Text info = null;
        info.text = "！イベントアメハッセイ！イベントアメハッセイ！";
        //場所を移動する
        textmeshpro.transform.localPosition = new Vector3(12.0f, 5.0f, 0.0f);
        //入力された文字を入れる
        //textmeshpro.text = information.text;
        textmeshpro.text = info.text;
    }



}
