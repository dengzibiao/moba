/*
文件名（File Name）:   ShopItem.cs

作者（Author）:    #高#

创建时间（CreateTime）:  2016-6-17 15:8:56
*/
using UnityEngine;
using System.Collections;

/// <summary>
/// 商城物品数据
/// </summary>
public class ShopItem
{
    private int id;
    private string _name;
    private MoneyType _moneyType;
    private int _count;
    private int _price;
    private string _des;
    private string _icon;
    private GradeType _grade;
    private bool isSell=false;

    public string Name
    {
        get { return _name; }
        set { _name = value; }
    }

    public MoneyType MoneyTYPE
    {
        get { return _moneyType; }
        set { _moneyType = value; }
    }

    public int Count
    {
        get { return _count; }
        set { _count = value; }
    }

    public int Price
    {
        get { return _price; }
        set { _price = value; }
    }

    public string Icon
    {
        get { return _icon; }
        set { _icon = value; }
    }

    public GradeType GradeTYPE
    {
        get { return _grade; }
        set { _grade = value; }
    }

    public int Id
    {
        get { return id; }
        set { id = value; }
    }

    public string Des
    {
        get { return _des; }
        set { _des = value; }
    }

    public bool IsSell
    {
        get { return isSell; }
        set { isSell = value; }
    }
}
