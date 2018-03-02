using UnityEngine;
using System.Collections;

public class KGFCustomGUITutorial : KGFObject, KGFICustomGUI
{
	public string GetName()
	{
		return "KGFCustomGUITutorial";
	}
	
	public string GetHeaderName()
	{
		return "Custom GUI Tutorial";
	}
	
	public Texture2D GetIcon()
	{
		return null;
	}
	
	public void Render()
	{
		return;
	}
}
