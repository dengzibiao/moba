using System;
using System.Collections;
using UnityEngine;

public class CBehaviour : MonoBehaviour
{
	void Start()
	{
		GameObject.DontDestroyOnLoad(this.gameObject);
	}
	
	public CBehaviour ()
	{
		//ClientNetMgr.GetSingle().SetBehaviour( this );
	}
	
	void GetMainUIInfo()
	{
		//Debug.Log("main");
	}
	
}

