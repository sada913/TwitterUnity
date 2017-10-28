using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tes : MonoBehaviour {
    string cmd = "-f(12)";
	// Use this for initialization
	void Start () {
        string n = "";
        int font_num;
        for (int i = 3;i<cmd.Length -1 ;i++)
        {
            n += cmd[i];
        }
        font_num = int.Parse(n);
        Debug.Log(font_num);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
