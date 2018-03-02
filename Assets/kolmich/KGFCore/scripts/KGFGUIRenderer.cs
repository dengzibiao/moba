// <author>Michal Kolasinski</author>
// <email>michal.kolasinski@kolmich.at</email>
// <summary>Collects all objects that implement the KGFIGui2D interface and calls their RenderGUI methods in the OnGUI</summary>

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Represents the main class of the Kolmich Game Framework Custom GUI.
/// </summary>
/// <remarks>
/// This class can list and manage all added instances which must implement the KGFIDebug interface.
/// </remarks>
public class KGFGUIRenderer : KGFObject, KGFIValidator
{
	/// <summary>
	/// Holds the list of all KGFIGui2D interfaces in the project
	/// </summary>
	private List<KGFIGui2D> itsGUIs = new List<KGFIGui2D>();
	
	protected override void KGFAwake()
	{
		itsGUIs = KGFAccessor.GetObjects<KGFIGui2D>();
		KGFAccessor.RegisterAddEvent<KGFIGui2D>(OnRegisterKGFIGui2D);
		KGFAccessor.RegisterRemoveEvent<KGFIGui2D>(OnUnregisterKGFIGui2D);
	}
	
	/// <summary>
	/// Add registering KGFIGui2D objets to the itsGUIs list
	/// </summary>
	/// <param name="theSender"></param>
	/// <param name="theArgs"></param>
	private void OnRegisterKGFIGui2D(object theSender, EventArgs theArgs)
	{
		KGFAccessor.KGFAccessorEventargs anEventArgs = theArgs as KGFAccessor.KGFAccessorEventargs;
		if (anEventArgs != null)
		{
			KGFIGui2D aGui2D = anEventArgs.GetObject() as KGFIGui2D;
			if(aGui2D != null)
			{
				itsGUIs.Add(aGui2D);
				itsGUIs.Sort(CompareKGFIGui2D);
			}
		}
	}
	
	/// <summary>
	/// Removes destroyed KGFIGui2D objects from the itsGUIs list
	/// </summary>
	/// <param name="theSender"></param>
	/// <param name="theArgs"></param>
	private void OnUnregisterKGFIGui2D(object theSender, EventArgs theArgs)
	{
		KGFAccessor.KGFAccessorEventargs anEventArgs = theArgs as KGFAccessor.KGFAccessorEventargs;
		if (anEventArgs != null)
		{
			KGFIGui2D aGui2D = anEventArgs.GetObject() as KGFIGui2D;
			if(aGui2D != null && itsGUIs.Contains(aGui2D))
			{
				itsGUIs.Remove(aGui2D);
			}
		}
	}
	
	int CompareKGFIGui2D(KGFIGui2D theGui1,KGFIGui2D theGui2)
	{
		return theGui1.GetLayer().CompareTo(theGui2.GetLayer());
	}
	
	/// <summary>
	/// The one and only OnGUI method that invokes all the RenderGUI methods of all registered KGFIGui2D objects
	/// </summary>
	protected void OnGUI()
	{
		float aScaleFactor = KGFScreen.GetScaleFactor2D();
		GUIUtility.ScaleAroundPivot(new Vector2(aScaleFactor,aScaleFactor),Vector2.zero);
		foreach(KGFIGui2D aGui2D in itsGUIs)
		{
			aGui2D.RenderGUI();
		}
		GUI.matrix = Matrix4x4.identity;
	}

	public KGFMessageList Validate()
	{
		return new KGFMessageList();
	}
}