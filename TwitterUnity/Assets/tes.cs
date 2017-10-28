using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tes : MonoBehaviour {
    string cmd = "-f(12)";
    string url = "http://sada-913.xyz/Unity/TwitterToRTF/rtfdl.php";

    string a = "{\\rtf1\\ansi\\ansicpg932\\deff0{\\fonttbl{\\f0\\fnil\\fcharset128 \\'82\\'6c\\'82\\'72 \\'96\\'be\\'92\\'a9;}}{\\colortbl;\\red255\\green0\\blue0;}{\\*\\generator Msftedit 5.41.21.2510;}\\viewkind4\\uc1\\pard\\sa200\\sl276\\slmult1\\lang17\\f0\\fs22 ああああ\nあああ}";
    // Use this for initialization
    void Start () {
        StartCoroutine(post());
        StartCoroutine(get());
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator post()
    {


        WWWForm form = new WWWForm();
        form.AddField("id", "ad");
        form.AddField("data", a);


        WWW www = new WWW(url, form);
        yield return www;
        if (www.error == null)
        {
            Debug.Log(www.text);
        }
    }

    IEnumerator get()
    {
        WWW www = new WWW(url);
        yield return www;
        if (www.error == null)
        {
            Debug.Log(www.text);
        }
    }

    string UtftoSjis(string utf)
    {
        //stringUTF8に何らかUTF8の文字列が入ってくる
        string stringUTF8 = utf;

        //まずはバイト配列に変換する
        byte[] bytesUTF8 = System.Text.Encoding.Default.GetBytes(stringUTF8);

        //バイト配列をUTF8の文字コードとしてStringに変換する
        return  System.Text.Encoding.UTF8.GetString(bytesUTF8);
    }
}
