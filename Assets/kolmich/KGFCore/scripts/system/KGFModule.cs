// <author>Christoph Hausjell</author>
// <email>christoph.hausjell@kolmich.at</email>

using System;
using System.CodeDom;
using UnityEngine;
using System.Collections;

/// <summary>
///	Contains the current version of a module and the required KGFCore version.
/// </summary>
/// <remarks>
/// This class is the base class for each module in the KOLMICH Game Framework. By calling the constructor of a module this class checks the installed KGFCore version and compares the version number to the minimum required one of the module.
/// </remarks>
public abstract class KGFModule : KGFObject, KGFIValidator
{
	// contains the current version of the module
	private Version itsCurrentVersion;
	// contains the minimum version of the KGFCore to use the module
	private Version itsMinimumCoreVersion;
	
	/// <remarks>
	/// the constructor of the KGFModule.
	/// </remarks>
	/// <param name="theCurrentVersion">the versionnumber of the module</param>
	/// <param name="theMinimumCoreVersion">the minimual required core version for this module</param>
	public KGFModule(Version theCurrentVersion, Version theMinimumCoreVersion)
	{
		itsCurrentVersion = theCurrentVersion;
		itsMinimumCoreVersion = theMinimumCoreVersion;
		
		// check if the current core version is older than the required one
		if(KGFCoreVersion.GetCurrentVersion() < itsMinimumCoreVersion)
		{
			Debug.LogError("the KGFCore verison installed in this scene is older than the required version. please update the KGFCore to the latest version");
		}
	}
	
	/// <summary>
	///	use this method to get the current version of a module.
	/// </summary>
	/// <remarks>
	/// returns the current version of the module.
	/// </remarks>
	public Version GetCurrentVersion()
	{
		return itsCurrentVersion.Clone() as Version;
	}
	
	/// <summary>
	///	use this method to get the mimimum required version of the KGFCore to use this module.
	/// </summary>
	/// <remarks>
	/// returns the minimum required core Version of this module.
	/// </remarks>
	public Version GetRequiredCoreVersion()
	{
		return itsMinimumCoreVersion.Clone() as Version;
	}
	
	/// <remarks>
	/// returns the name of the derivating module.
	/// </remarks>
	public abstract string GetName();
	
	/// <remarks>
	/// returns the icon of the derivating module.
	/// </remarks>
	public abstract Texture2D GetIcon();
	
	/// <remarks>
	/// returns the documentation subpath of the module.
	/// </remarks>
	public abstract string GetDocumentationPath();
	
	/// <remarks>
	/// returns the forum subpath of the module.
	/// </remarks>
	public abstract string GetForumPath();
	
	/// <remarks>
	/// for further information look into KGFIValidator documentation
	/// </remarks>
	public abstract KGFMessageList Validate();
	
	
	private static KGFModule itsOpenModule = null;
	
	private const string itsCopyrightText =
		"KOLMICH Creations e.U. is a small company based out of Vienna, Austria.\n"+
		"While developing cool unity3d projects we put an immense amount of time \n"+
		"to create professional tools and professional content. \n\n\n"+
		"If you have any ideas on improvements or you just want to give us some feedback use one of the links below.";
	
	public static void OpenHelpWindow(KGFModule theModule)
	{
		itsOpenModule = theModule;
	}
	
	public static void RenderHelpWindow()
	{
		if(itsOpenModule != null)
		{
			int aWidth = 512+(int)KGFGUIUtility.GetSkinHeight()*2;
			int aHeight = 256+(int)KGFGUIUtility.GetSkinHeight()*7;
			
			Rect aRect = new Rect((Screen.width - aWidth) / 2, (Screen.height - aHeight) / 2, aWidth, aHeight);
			
			KGFGUIUtility.Window(12345689, aRect, RenderHelpWindowMethod, itsOpenModule.GetName() + " (part of KOLMICH Game Framework)");
			
			if(aRect.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown && Event.current.button == 0)
			{
				itsOpenModule = null;
			}
		}
		else
		{
			itsOpenModule = null;
		}
	}
	
	private static void RenderHelpWindowMethod(int theWindowID)
	{
		GUILayout.BeginHorizontal();
		{
			GUILayout.FlexibleSpace();
			KGFGUIUtility.BeginVerticalBox(KGFGUIUtility.eStyleBox.eBoxInvisible, GUILayout.ExpandHeight(true));
			{
				KGFGUIUtility.BeginHorizontalPadding();
				{
					KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxDarkTop, GUILayout.ExpandWidth(true));
					{
						GUILayout.FlexibleSpace();
						GUILayout.Label(KGFGUIUtility.GetLogo(), GUILayout.Height(50));
						GUILayout.FlexibleSpace();
					}
					KGFGUIUtility.EndHorizontalBox();
					
					KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxBottom, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
					{
						GUILayout.Label(itsCopyrightText, GUILayout.ExpandWidth(true));
					}
					KGFGUIUtility.EndHorizontalBox();
					
					GUILayout.Space(KGFGUIUtility.GetSkinHeight());
					
					KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxDarkTop, GUILayout.ExpandWidth(true));
					{
						KGFGUIUtility.Label(itsOpenModule.GetName() +" version:", KGFGUIUtility.eStyleLabel.eLabelFitIntoBox);
						KGFGUIUtility.Label(itsOpenModule.GetCurrentVersion().ToString(), KGFGUIUtility.eStyleLabel.eLabelFitIntoBox);
						GUILayout.FlexibleSpace();
						KGFGUIUtility.Label("req. KGFCore version:", KGFGUIUtility.eStyleLabel.eLabelFitIntoBox);
						KGFGUIUtility.Label(itsOpenModule.GetRequiredCoreVersion().ToString(), KGFGUIUtility.eStyleLabel.eLabelFitIntoBox);
					}
					KGFGUIUtility.EndHorizontalBox();
					
					KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxDarkBottom, GUILayout.ExpandWidth(true));
					{
						KGFGUIUtility.BeginVerticalPadding();
						{
							if(KGFGUIUtility.Button(KGFGUIUtility.GetHelpIcon(),"documentation", KGFGUIUtility.eStyleButton.eButtonLeft, GUILayout.ExpandWidth(true)))
							{
								Application.OpenURL("http://www.kolmich.at/documentation/" + itsOpenModule.GetDocumentationPath());
								itsOpenModule = null;
							}
							/*
							if(KGFGUIUtility.Button("user documentation", KGFGUIUtility.eStyleButton.eButtonMiddle, GUILayout.ExpandWidth(true)))
							{
								Application.OpenURL("http://www.kolmich.at/documentation/frames.html");
								itsOpenModule = null;
							}
							 */
							if(KGFGUIUtility.Button(KGFGUIUtility.GetHelpIcon(), "forum", KGFGUIUtility.eStyleButton.eButtonMiddle, GUILayout.ExpandWidth(true)))
							{
								Application.OpenURL("http://www.kolmich.at/forum/" + itsOpenModule.GetForumPath());
								itsOpenModule = null;
							}
							if(KGFGUIUtility.Button(KGFGUIUtility.GetHelpIcon(), "homepage", KGFGUIUtility.eStyleButton.eButtonRight, GUILayout.ExpandWidth(true)))
							{
								Application.OpenURL("http://www.kolmich.at");
								itsOpenModule = null;
							}
						}
						KGFGUIUtility.EndVerticalPadding();
					}
					KGFGUIUtility.EndHorizontalBox();
				}
				KGFGUIUtility.EndHorizontalPadding();
			}
			KGFGUIUtility.EndVerticalBox();
			GUILayout.FlexibleSpace();
		}
		GUILayout.EndHorizontal();
	}
}