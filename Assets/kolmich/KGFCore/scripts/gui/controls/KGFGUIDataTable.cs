// <author>Christoph Hausjell</author>
// <email>christoph.hausjell@kolmich.at</email>
// <date>2012-03-13</date>
// <summary>short summary</summary>

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public delegate bool PreCellContentHandler(KGFDataRow theRow, KGFDataColumn theColumn);

public class KGFGUIDataTable : KGFIControl
{
	private KGFDataTable itsDataTable;
	private Vector2 itsDataTableScrollViewPosition;
	private uint itsStartRow = 0;
	private uint itsDisplayRowCount = 100;
	private Dictionary<KGFDataColumn, uint> itsColumnWidth = new Dictionary<KGFDataColumn, uint>();
	private Dictionary<KGFDataColumn, bool> itsColumnVisible = new Dictionary<KGFDataColumn, bool>();
	private KGFDataRow itsClickedRow = null;
	private KGFDataRow itsCurrentSelected = null;
	private bool itsVisible = true;
	
	public event EventHandler PreRenderRow;
	public event EventHandler PostRenderRow;
	public event EventHandler PreRenderColumn;
	public event EventHandler PostRenderColumn;
	public event PreCellContentHandler PreCellContentHandler;
	public event EventHandler OnClickRow;
	
	public KGFGUIDataTable(KGFDataTable theDataTable, params GUILayoutOption[] theLayout)
	{
		itsDataTable = theDataTable;
		
		// add the column width auto to all columns
		foreach(KGFDataColumn aColumn in itsDataTable.Columns)
		{
			itsColumnWidth.Add(aColumn, 0);
			itsColumnVisible.Add(aColumn, true);
		}
	}
	
	public uint GetStartRow()
	{
		return itsStartRow;
	}
	
	public void SetStartRow(uint theStartRow)
	{
		itsStartRow = (uint)Math.Min(theStartRow, itsDataTable.Rows.Count);
	}
	
	public uint GetDisplayRowCount()
	{
		return itsDisplayRowCount;
	}
	
	public void SetDisplayRowCount(uint theDisplayRowCount)
	{
		itsDisplayRowCount = (uint)Math.Min(theDisplayRowCount, itsDataTable.Rows.Count - itsStartRow);
	}
	
	public void SetColumnVisible(int theColumIndex, bool theValue)
	{
		if(theColumIndex >= 0 && theColumIndex < itsDataTable.Columns.Count)
		{
			itsColumnVisible[itsDataTable.Columns[theColumIndex]] = theValue;
		}
	}
	
	public bool GetColumnVisible(int theColumIndex)
	{
		if(theColumIndex >= 0 && theColumIndex < itsDataTable.Columns.Count)
		{
			return itsColumnVisible[itsDataTable.Columns[theColumIndex]];
		}
		
		return false;
	}
	
	public void SetColumnWidth(int theColumIndex, uint theValue)
	{
		if(theColumIndex >= 0 && theColumIndex < itsDataTable.Columns.Count)
		{
			itsColumnWidth[itsDataTable.Columns[theColumIndex]] = theValue;
		}
	}
	
	public uint GetColumnWidth(int theColumIndex)
	{
		if(theColumIndex >= 0 && theColumIndex < itsDataTable.Columns.Count)
		{
			return itsColumnWidth[itsDataTable.Columns[theColumIndex]];
		}
		
		return 0;
	}
	
	public KGFDataRow GetCurrentSelected()
	{
		Debug.Log("GetCurrentSelected()");
		return itsCurrentSelected;
	}
	
	public void SetCurrentSelected(KGFDataRow theDataRow)
	{
		if(itsDataTable.Rows.Contains(theDataRow))
		{
			Debug.Log("SetCurrentSelected");
			itsCurrentSelected = theDataRow;
		}
	}
	
	#region Render functions
	
	private void RenderTableHeadings()
	{
		KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxDarkTop, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(false));
		{
			foreach(KGFDataColumn aColumn in itsDataTable.Columns)
			{
				// check if visible is set to true
				if(itsColumnVisible[aColumn])
				{
					// check if the width is fixed size
					if(itsColumnWidth[aColumn] != 0)
					{
						KGFGUIUtility.Label(aColumn.ColumnName,KGFGUIUtility.eStyleLabel.eLabelFitIntoBox, GUILayout.Width(itsColumnWidth[aColumn]));
					}
					else
					{
						KGFGUIUtility.Label(aColumn.ColumnName,KGFGUIUtility.eStyleLabel.eLabelFitIntoBox, GUILayout.ExpandWidth(true));
					}
					
					KGFGUIUtility.Separator(KGFGUIUtility.eStyleSeparator.eSeparatorVerticalFitInBox);
				}
			}
		}
		KGFGUIUtility.EndHorizontalBox();
	}
	
	bool itsForceDisplayVerticalSlider = false;
	public void ForceDisplayVerticalSlider(bool theForceDisplay)
	{
		itsForceDisplayVerticalSlider = theForceDisplay;
	}
	
	private void RenderTableRows()
	{
		itsDataTableScrollViewPosition = KGFGUIUtility.BeginScrollView(itsDataTableScrollViewPosition, false, itsForceDisplayVerticalSlider, GUILayout.ExpandHeight(true));
		{
			//Log List Heading
			RenderTableHeadings();
			
			if(itsDataTable.Rows.Count > 0)
			{
				GUILayout.BeginVertical();
				{
					Color aDefaultColor = GUI.color;
					
					for(int aRowIndex = (int)itsStartRow; aRowIndex < itsStartRow + itsDisplayRowCount && aRowIndex < itsDataTable.Rows.Count; aRowIndex++)
					{
						KGFDataRow aRow = itsDataTable.Rows[aRowIndex];
						
						//Pre Row Hook
						if(PreRenderRow != null)
						{
							PreRenderRow(aRow, EventArgs.Empty);
						}
						
						if(aRow == itsCurrentSelected)
						{
							KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxDarkTopInteractive, GUILayout.ExpandWidth(true));
						}
						else
						{
							KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxMiddleVerticalInteractive, GUILayout.ExpandWidth(true));
						}
						
						//Row
						{
							foreach(KGFDataColumn aColumn in itsDataTable.Columns)
							{
								//check if the column is visible
								if(itsColumnVisible[aColumn])
								{
									//Pre Column Hook
									if(PreRenderColumn != null)
									{
										PreRenderColumn(aColumn, EventArgs.Empty);
									}
									
									bool aCustomDrawer = false;
									if(PreCellContentHandler != null)
									{
										aCustomDrawer = PreCellContentHandler(aRow, aColumn);
									}
									
									if(!aCustomDrawer)
									{
										// crate the string
										int itsStringMaxLenght = 85;
										string aString = aRow[aColumn].ToString().Substring(0, Math.Min(itsStringMaxLenght,aRow[aColumn].ToString().Length));
										
										if(aString.Length == itsStringMaxLenght)
										{
											aString += "...";
										}
										
										if(itsColumnWidth[aColumn] > 0)
										{
											KGFGUIUtility.Label(aString, KGFGUIUtility.eStyleLabel.eLabelFitIntoBox, GUILayout.Width(itsColumnWidth[aColumn]));
										}
										else
										{
											KGFGUIUtility.Label(aString, KGFGUIUtility.eStyleLabel.eLabelFitIntoBox, GUILayout.ExpandWidth(true));
										}
									}
									
									KGFGUIUtility.Separator(KGFGUIUtility.eStyleSeparator.eSeparatorVerticalFitInBox);

									//Post Column Hook
									if(PostRenderColumn != null)
									{
										PostRenderColumn(aColumn, EventArgs.Empty);
									}
								}
							}
						}
						KGFGUIUtility.EndHorizontalBox();
						
						//check if the rect contains the mouse and the pressed mouse button is the left mouse button
						if(GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown && Event.current.button == 0)
						{
							itsClickedRow = aRow;
						}
						
						//only send this event on layouting
						if(OnClickRow != null && itsClickedRow != null && Event.current.type == EventType.Layout)
						{
							if(itsCurrentSelected != itsClickedRow)
							{
								itsCurrentSelected = itsClickedRow;
								//Debug.Log("itsCurrentSelected is set");
							}
							else
							{
								itsCurrentSelected = null;
							}
							
							OnClickRow(itsClickedRow, EventArgs.Empty);
							itsClickedRow = null;
							//Debug.Log("itsClickedRow is set to null");
						}
						
						//Post Row Hook
						if(PostRenderRow != null)
						{
							PostRenderRow(aRow, EventArgs.Empty);
						}
					}
					GUI.color = aDefaultColor;
					GUILayout.FlexibleSpace();
				}
				GUILayout.EndVertical();
			}
			else
			{
				GUILayout.Label("no items found");
				GUILayout.FlexibleSpace();
			}
		}
		GUILayout.EndScrollView();
	}
	
	#endregion
	
	/*
	private void OnDataColumnChanged(object theSender, KGFCollectionChangeEventArgs theArgument)
	{
		KGFDataColumn aColumn = theArgument.Element as KGFDataColumn;
		
		if(theArgument.Action == CollectionChangeAction.Add)
		{
			itsColumnVisible.Add(aColumn, true);
			itsColumnWidth.Add(aColumn, 0);
		}
		else if(theArgument.Action == CollectionChangeAction.Remove)
		{
			if(aColumn != null)
			{
				itsColumnVisible.Remove(aColumn);
				itsColumnWidth.Remove(aColumn);
			}
		}
	}
	*/
	
	#region KGFIGUIControl
	
	public void Render()
	{
		if(itsVisible)
		{
			//Log List
			GUILayout.BeginVertical();
			{
				//Log List Heading
				//RenderTableHeadings();
				
				//Log List
				RenderTableRows();
			}
			GUILayout.EndVertical();
		}
	}
	
	public string GetName()
	{
		return "KGFGUIDataTable";
	}
	
	public bool IsVisible()
	{
		return itsVisible;
	}
	
	#endregion
}