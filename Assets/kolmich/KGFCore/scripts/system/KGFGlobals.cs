using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Use this file for all globally useable types
/// </summary>

public enum KGFAlignmentVertical{Top,Middle,Bottom};
public enum KGFAlignmentHorizontal{Left,Middle,Right};
public enum KGFOrientation{Horizontal,Vertical};

namespace System.Runtime.CompilerServices

{

	public class ExtensionAttribute : Attribute { }

}
public static class KGFGlobals
{
	
	/// <summary>
	/// Get the path to the object as string
	/// </summary>
	/// <param name="theObject"></param>
	/// <returns></returns>
	public static string GetObjectPath(this GameObject theObject)
	{
		List<string> aStringList = new List<string>();
		Transform aTransform = theObject.transform;
		
		do{
			aStringList.Add(aTransform.name);
			aTransform = aTransform.parent;
		}while (aTransform != null);
		
		aStringList.Reverse();
		return string.Join("/",aStringList.ToArray());
	}
}