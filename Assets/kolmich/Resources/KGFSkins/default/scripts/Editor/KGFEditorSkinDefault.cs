// <author>Michal Kolasinski</author>
// <email>michal.kolasinski@kolmich.at</email>
// <summary>
// This class adds 2 custom menu entries to the unity3d menu. 
// The entries can be used to switch the current skin to one of the two default skins.
// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class KGFEditorSkinDefault
{
	private static string itsGuiSkinPathDefault32 = "KGFSkins/default/skins/skin_default_32";
	private static string itsGuiSkinPathDefault16 = "KGFSkins/default/skins/skin_default_16";
	
	[MenuItem ("Edit/KGF/skins/default/enable 32")]
	public static void Enable32 ()
	{
		KGFGUIUtility.SetSkinPath(itsGuiSkinPathDefault32);
	}
	
	[MenuItem ("Edit/KGF/skins/default/enable 16")]
	public static void Enable16 ()
	{
		KGFGUIUtility.SetSkinPath(itsGuiSkinPathDefault16);
	}
}