// <author>Michal Kolasinski</author>
// <email>michal.kolasinski@kolmich.at</email>
// <summary>Demonstrates some tutorial code explaining the use of the KGFGUIUtility class
// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This tutorial demonstrates the easiest way to render all gui controls included in a KGFSkin
/// We use the KGFGUIUtility class.
/// If you don't want to use this class to render your gui controls please check out the KGFSkinTutorial2!!
/// </summary>
public class KGFSkinTutorial1 : MonoBehaviour
{	
	public string itsSkinName = "default";
	
	/// <summary>
	/// Unity3d Awake is used here to set the correct skin
	/// </summary>
	private void Awake()
	{		
		KGFGUIUtility.SetSkinPath("KGFSkins/"+itsSkinName+"/skins/skin_"+itsSkinName+"_32");
	}
	
	/// <summary>
	/// OnGUI renders the demo window
	/// </summary>
	private void OnGUI()
	{
		KGFGUIUtility.Window(0,CalculateWindowSize(),RenderTutorialWindow,"Please examine the KGFSkinTutorial1 script");	//Renders the window using the KGFGUIUtility
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
		KGFGUIUtility.TextField("textfield left",KGFGUIUtility.eStyleTextField.eTextFieldLeft);
		KGFGUIUtility.Button("button right",KGFGUIUtility.eStyleButton.eButtonRight);
	}
	
	/// <summary>
	/// renders a butt left a button middle and a button right next to each other
	/// </summary>
	private void RenderControlCombination2()
	{
		KGFGUIUtility.Button("button left",KGFGUIUtility.eStyleButton.eButtonLeft);
		KGFGUIUtility.Button("button middle",KGFGUIUtility.eStyleButton.eButtonMiddle);
		KGFGUIUtility.Button("button right",KGFGUIUtility.eStyleButton.eButtonRight);
	}
	
	/// <summary>
	/// renders a dark box top and a normal box bottom including two labels
	/// </summary>
	private void RenderControlCombination3()
	{
		KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxDarkTop,GUILayout.ExpandWidth(true));		//render first row
		{
			GUILayout.FlexibleSpace();
			KGFGUIUtility.Label("box dark top",KGFGUIUtility.eStyleLabel.eLabelFitIntoBox);
			GUILayout.FlexibleSpace();
		}
		KGFGUIUtility.EndHorizontalBox();
		KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxBottom,GUILayout.ExpandWidth(true));		//render first row
		{
			KGFGUIUtility.Label("box bottom",KGFGUIUtility.eStyleLabel.eLabelFitIntoBox);
		}
		KGFGUIUtility.EndHorizontalBox();
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