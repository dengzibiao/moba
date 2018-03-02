/*
文件名（File Name）:   InitSound.cs

作者（Author）:    #高#

创建时间（CreateTime）:  #CreateTime#
*/
using UnityEngine;
using System.Collections;

public class InitSound : MonoBehaviour {

   void Awake(){
       
    }
	// Use this for initialization
	void Start () {
        SoundManager.Init();
        SoundManager.InitBGM();
	    if (Application.loadedLevelName== GameLibrary.UI_Major)
	    {
	        SoundManager.PlayBGM("主城BGM");
	    }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
