// <author>Christoph Hausjell</author>
// <email>christoph.hausjell@kolmich.at</email>
// <date>2012-03-13</date>
// <summary>short summary</summary>

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class KGFGUIDropDown : KGFIControl
{
	public enum eDropDirection
	{
		eAuto = 0,
		eDown = 1,
		eUp = 2
	}
	
	private List<string> itsEntrys = new List<string>();
	private GUILayoutOption [] itsLayoutOptions;
	private string itsCurrentSelected = string.Empty;
	private bool itsVisible = true;
	public Vector2 itsScrollPosition = Vector2.zero;
	public Rect itsLastRect;
	public static KGFGUIDropDown itsOpenInstance = null;
	public uint itsWidth = 0;
	public uint itsHeight = 0;
	private uint itsMaxVisibleItems = 1;
	public eDropDirection itsDirection;
	public string itsTitle = string.Empty;
	public Texture2D itsIcon = null;
	
	public bool itsHover = false;
	
	public event EventHandler SelectedValueChanged;
	
	public static bool itsCorrectedOffset = false;
	
	public KGFGUIDropDown(IEnumerable<string> theEntrys, uint theWidth, uint theMaxVisibleItems, eDropDirection theDirection, params GUILayoutOption[] theLayout)
	{
		if(theEntrys != null)
		{
			foreach(string aEntry in theEntrys)
			{
				itsEntrys.Add(aEntry);
			}
			
			itsWidth = theWidth;
			itsMaxVisibleItems = theMaxVisibleItems;
			itsDirection = theDirection;
			
			if(itsEntrys.Count > 0)
			{
				itsCurrentSelected = itsEntrys[0];
			}
		}
		else
		{
			UnityEngine.Debug.LogError("the list of entrys was null");
		}
	}
	
	public void SetEntrys(IEnumerable<string> theEntrys)
	{
		itsEntrys.Clear();
		
		foreach(string aEntry in theEntrys)
		{
			itsEntrys.Add(aEntry);
		}
		
		if(itsEntrys.Count > 0)
		{
			itsCurrentSelected = itsEntrys[0];
		}
	}
	
	public IEnumerable<string> GetEntrys()
	{
		return itsEntrys;
	}
	
	public string SelectedItem()
	{
		return itsCurrentSelected;
	}
	
	public void SetSelectedItem(string theValue)
	{
		itsCurrentSelected = theValue;
		
		if(SelectedValueChanged != null)
		{
			SelectedValueChanged(theValue, EventArgs.Empty);
		}
	}
	
	public void Render()
	{
		if(itsEntrys.Count <= itsMaxVisibleItems)
		{
			itsHeight = (uint)itsEntrys.Count * (uint)KGFGUIUtility.GetSkinHeight();
		}
		else
		{
			itsHeight = itsMaxVisibleItems * (uint)KGFGUIUtility.GetSkinHeight();
		}
		
		if(itsVisible)
		{
			GUILayout.BeginHorizontal(GUILayout.Width(itsWidth));
			{
				KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxLeft);
				{
					if(itsTitle != string.Empty)
					{
						KGFGUIUtility.Label(itsTitle, KGFGUIUtility.eStyleLabel.eLabelFitIntoBox, GUILayout.ExpandWidth(true));
					}
					else
					{
						KGFGUIUtility.Label(itsCurrentSelected, KGFGUIUtility.eStyleLabel.eLabelFitIntoBox, GUILayout.ExpandWidth(true));
					}
				}
				KGFGUIUtility.EndHorizontalBox();
				
				if(itsIcon == null)
				{
					if(KGFGUIUtility.Button("v", KGFGUIUtility.eStyleButton.eButtonRight, GUILayout.ExpandWidth(false)))
					{
						if(itsOpenInstance != this)
						{
							itsOpenInstance = this;
							itsCorrectedOffset = false;
						}
						else
						{
							itsOpenInstance = null;
							itsCorrectedOffset = false;
						}
					}
				}
				else
				{
					if(KGFGUIUtility.Button(itsIcon, KGFGUIUtility.eStyleButton.eButtonRight, GUILayout.ExpandWidth(false)))
					{
						if(itsOpenInstance != this)
						{
							itsOpenInstance = this;
							itsCorrectedOffset = false;
						}
						else
						{
							itsOpenInstance = null;
							itsCorrectedOffset = false;
						}
					}
				}
				
			}
			GUILayout.EndHorizontal();
			
			if(Event.current.type == EventType.Repaint)
			{
				itsLastRect = GUILayoutUtility.GetLastRect();
			}
			else
			{
				Vector3 aMousePosition = Input.mousePosition;
				aMousePosition.y = Screen.height - aMousePosition.y;
				
				if(itsLastRect.Contains(aMousePosition))
				{
					itsHover = true;
				}
				else
				{
					if(KGFGUIDropDown.itsOpenInstance != this)
					{
						itsHover = false;
					}
				}
			}
		}
	}

	public string GetName()
	{
		return "KGFGUIDropDown";
	}

	public bool IsVisible()
	{
		return itsVisible;
	}
	
	public bool GetHover()
	{
		return itsHover;
	}
}
