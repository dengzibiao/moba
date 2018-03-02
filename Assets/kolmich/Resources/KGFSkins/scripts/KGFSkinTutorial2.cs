// <author>Michal Kolasinski</author>
// <email>michal.kolasinski@kolmich.at</email>
// <summary>Demonstrates some tutorial code explaining the use of the KGFGUIUtility class
// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This tutorial demonstrates the way how to render all gui controls included in a KGFSkin
/// Tipp: We provide a KGFGUIUtility class that makes things easier. Check the KGFSkinTutorial1 script to see how to use it.
/// If you don't want to use the KGFGUIUtility class continue reding this code.
/// </summary>
public class KGFSkinTutorial2 : MonoBehaviour
{
	public string itsSkinName = "default";
	
	GUISkin itsGuiSkin = null;
	GUIStyle itsStyleWindow = null;
	GUIStyle itsStyleButtonLeft = null;
	GUIStyle itsStyleButtonMiddle = null;
	GUIStyle itsStyleButtonRight = null;
	GUIStyle itsStyleTextfieldLeft = null;
	GUIStyle itsStyleBoxDarkTop = null;
	GUIStyle itsStyleBoxBottom = null;
	GUIStyle itsStyleLabelFitInBox = null;
	
	/// <summary>
	/// Unity3d Awake is used here to set the correct skin and cache the gui styles
	/// </summary>
	private void Awake()
	{
		itsGuiSkin = Resources.Load("KGFSkins/"+itsSkinName+"/skins/skin_"+itsSkinName+"_32") as GUISkin;
		
		if(itsGuiSkin != null)
		{
			itsStyleWindow = ExctractStyle("window");			//cache the styles			
			itsStyleButtonLeft = ExctractStyle("button_left");			
			itsStyleButtonMiddle = ExctractStyle("button_middle");			
			itsStyleButtonRight = ExctractStyle("button_right");			
			itsStyleTextfieldLeft = ExctractStyle("textfield_left");			
			itsStyleBoxDarkTop = ExctractStyle("box_dark_top");			
			itsStyleBoxBottom = ExctractStyle("box_bottom");
			itsStyleLabelFitInBox = ExctractStyle("label_fitintobox");
		}
		else
			Debug.LogError("unable to load skin: ");
	}
	
	/// <summary>
	/// This method will extract the style named theStyleName from itsGuiSkin
	/// </summary>
	/// <param name="theStyleName"></param>
	/// <returns></returns>
	private GUIStyle ExctractStyle(string theStyleName)
	{
		GUIStyle aStyle = itsGuiSkin.GetStyle(theStyleName);
		if(aStyle == null)
		{
			Debug.LogError("unable to extract style: "+theStyleName + " from: "+itsGuiSkin.name);
			return null;
		}
		return aStyle;
	}
	
	/// <summary>
	/// OnGUI renders the demo window
	/// </summary>
	private void OnGUI()
	{
		if(itsGuiSkin == null)
			return;
		GUILayout.Window(0,CalculateWindowSize(),RenderTutorialWindow,"Please examine the KGFSkinTutorial2 script",itsStyleWindow);	 //Renders the window
	}
	
	/// <summary>
	/// We use here the KGFGUIUtility to render all the elements.
	/// </summary>
	/// <param name="windowID"></param>
	void RenderTutorialWindow(int windowID)
	{
		GUILayout.BeginHorizontal();
		{
			KGFGUIUtility.Space();
			GUILayout.BeginVertical();
			{
				KGFGUIUtility.Space();
				GUILayout.BeginHorizontal();
				{
					RenderControlCombination1();	//render textfield left and button right
					KGFGUIUtility.Space();
					RenderControlCombination2();	//render button left button middle and button right
				}
				GUILayout.EndHorizontal();
				KGFGUIUtility.Space();
				RenderControlCombination3();		//render box dark top and box bottom + labels
			}
			GUILayout.EndVertical();
			KGFGUIUtility.Space();
		}
		GUILayout.EndHorizontal();
	}
	
	/// <summary>
	/// renders a textfield left and a button right next to each other
	/// </summary>
	private void RenderControlCombination1()
	{
		GUILayout.TextField("textfield left",itsStyleTextfieldLeft);
		GUILayout.Button("button right",itsStyleButtonRight);
	}
	
	/// <summary>
	/// renders a butt left a button middle and a button right next to each other
	/// </summary>
	private void RenderControlCombination2()
	{
		GUILayout.Button("button left",itsStyleButtonLeft);
		GUILayout.Button("button middle",itsStyleButtonMiddle);
		GUILayout.Button("button right",itsStyleButtonRight);
	}
	
	/// <summary>
	/// renders a dark box top and a normal box bottom including two labels
	/// </summary>
	private void RenderControlCombination3()
	{
		GUILayout.BeginHorizontal(itsStyleBoxDarkTop,GUILayout.ExpandWidth(true));		//render first row
		{
			GUILayout.FlexibleSpace();
			GUILayout.Label("box dark top",itsStyleLabelFitInBox);
			GUILayout.FlexibleSpace();
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal(itsStyleBoxBottom,GUILayout.ExpandWidth(true));		//render first row
		{
			GUILayout.Label("box bottom",itsStyleLabelFitInBox);
		}
		GUILayout.EndHorizontal();
	}
	
	/// <summary>
	/// Calculate a centered 300x300 window
	/// </summary>
	/// <returns></returns>
	private Rect CalculateWindowSize()
	{
		float aWindowWidth = 600.0f;
		float aWindowHeight = 300.0f;
		Rect aWindowRect = new Rect();
		aWindowRect.x = (Screen.width - aWindowWidth)/2.0f;	//center window
		aWindowRect.y = (Screen.height - aWindowHeight)/2.0f;
		aWindowRect.width = aWindowWidth;
		aWindowRect.height = aWindowHeight;
		return aWindowRect;
	}
}