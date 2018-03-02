using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class KGFDataTable
{
	private List<KGFDataColumn> itsColumns = new List<KGFDataColumn>();
	private List<KGFDataRow> itsRows = new List<KGFDataRow>();
	
	public List<KGFDataColumn> Columns
	{
		get
		{
			return itsColumns;
		}
	}
	
	public List<KGFDataRow> Rows
	{
		get
		{
			return itsRows;
		}
	}
	
	public KGFDataRow NewRow()
	{
		return new KGFDataRow(this);
	}
}

public class KGFDataColumn
{
	private string itsName;
	private Type itsType;
	
	public KGFDataColumn(string theName, Type theType)
	{
		itsName = theName;
		itsType = theType;
	}
	
	public void Add(string theName, Type theType)
	{
		itsName = theName;
		itsType = theType;
	}
	
	public string ColumnName
	{
		get
		{
			return itsName;
		}
		set
		{
			itsName = value;
		}
	}
	
	public Type ColumnType
	{
		get
		{
			return itsType;
		}
		set
		{
			itsType = value;
		}
	}
}

public class KGFDataRow
{
	KGFDataTable itsTable;
	List<KGFDataCell> itsCells = new List<KGFDataCell>();
	
	public KGFDataRow()
	{
		itsTable = null;
	}
	
	public KGFDataRow(KGFDataTable theTable)
	{
		itsTable = theTable;
		
		foreach(KGFDataColumn aColumn in theTable.Columns)
		{
			itsCells.Add(new KGFDataCell(aColumn, this));
		}
	}
	
	public KGFDataCell this[int theIndex]
	{
		get
		{
			if(theIndex >= 0 && theIndex < itsTable.Columns.Count)
			{
				return itsCells[theIndex];
			}
			else
			{
				throw new ArgumentOutOfRangeException();
			}
		}
		set
		{
			if(theIndex >= 0 && theIndex < itsTable.Columns.Count)
			{
				itsCells[theIndex] = value;
			}
			else
			{
				throw new ArgumentOutOfRangeException();
			}
		}
	}
	
	public KGFDataCell this[string theName]
	{
		get
		{
			foreach(KGFDataCell aCell in itsCells)
			{
				if(aCell.Column.ColumnName.Equals(theName))
				{
					return aCell;
				}
			}
			
			throw new ArgumentOutOfRangeException();
		}
		set
		{
			bool found = false;
			
			for(int aCounter = 0; aCounter < itsCells.Count; aCounter++)
			{
				if(itsCells[aCounter].Column.ColumnName.Equals(theName))
				{
					itsCells[aCounter] = value;
					found = true;
					break;
				}
			}
			
			if(!found)
			{
				throw new ArgumentOutOfRangeException();
			}
		}
	}
	
	public KGFDataCell this[KGFDataColumn theColumn]
	{
		get
		{
			for(int aCounter = 0; aCounter < itsTable.Columns.Count; aCounter++)
			{
				if(itsCells[aCounter].Column.Equals(theColumn))
				{
					return itsCells[aCounter];
				}
			}
			
			throw new ArgumentOutOfRangeException();
		}
		set
		{
			for(int aCounter = 0; aCounter < itsTable.Columns.Count; aCounter++)
			{
				if(itsCells[aCounter].Column.Equals(theColumn))
				{
					itsCells[aCounter] = value;
				}
			}
			
			throw new ArgumentOutOfRangeException();
		}
	}
	
	public bool IsNull(KGFDataColumn theColumn)
	{
		return IsNull(theColumn.ColumnName);
	}
	
	public bool IsNull(string theColumn)
	{
		foreach(KGFDataCell aCell in itsCells)
		{
			if(aCell.Column.ColumnName.Equals(theColumn) && aCell.Value != null)
			{
				return false;
			}
		}
		
		return true;
	}
}

public class KGFDataCell
{
	private KGFDataColumn itsColumn;
	private KGFDataRow itsRow;
	private object itsValue;
	
	public KGFDataCell(KGFDataColumn theColumn, KGFDataRow theRow)
	{
		itsColumn = theColumn;
		itsRow = theRow;
		itsValue = null;
	}
	
	public KGFDataColumn Column
	{
		get
		{
			return itsColumn;
		}
	}
	
	public KGFDataRow Row
	{
		get
		{
			return itsRow;
		}
	}
	
	public object Value
	{
		get
		{
			return itsValue;
		}
		set
		{
			itsValue = value;
		}
	}
	
	public override string ToString()
	{
		return itsValue.ToString();
	}
}