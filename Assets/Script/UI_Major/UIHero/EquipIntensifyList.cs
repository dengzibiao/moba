using UnityEngine;
using System.Collections;
using Tianyu;
using System.Collections.Generic;

public class EquipIntensifyList : GUISingleItemList
{
    public UILabel propType;
    public UILabel FrontNum;
    public UILabel QueenNum;
    static int[] FrontEquipPropertylist = new int[18];
    static int[] QueenEquipPropertyList = new int[18];
    /// <summary>
    /// 单例
    /// </summary>
    private static EquipIntensifyList mSingleton;
    public static EquipIntensifyList Instance()
    {
        if (mSingleton == null)
            mSingleton = new EquipIntensifyList();
        return mSingleton;
    }
    static ItemEquip IEs;
    protected override void InitItem()
    {

    }
    public override void Info(object obj)
    {
        for (int i = 0; i < UI_HeroDetail.instance.propTypeName.Length; i++)
        {
            if (index == i)
            {
                propType.text = UI_HeroDetail.instance.propTypeName[i];
            }
        }
        for (int i = 0; i < FrontEquipPropertylist.Length; i++)
        {
            if (index == i)
            {
                switch (index)
                {
                    case 0:
                        FrontNum.text = FrontEquipPropertylist[i].ToString();
                        break;
                    case 1:
                        FrontNum.text = FrontEquipPropertylist[i].ToString();
                        break;
                    case 2:
                        FrontNum.text = FrontEquipPropertylist[i].ToString();
                        break;
                    case 3:
                        FrontNum.text = FrontEquipPropertylist[i].ToString();
                        break;
                    case 4:
                        FrontNum.text = FrontEquipPropertylist[i].ToString();
                        break;
                    case 5:
                        FrontNum.text = FrontEquipPropertylist[i].ToString();
                        break;
                    case 6:
                        FrontNum.text = FrontEquipPropertylist[i].ToString();
                        break;
                    case 7:
                        FrontNum.text = FrontEquipPropertylist[i].ToString();
                        break;
                    case 8:
                        FrontNum.text = FrontEquipPropertylist[i].ToString();
                        break;
                    case 9:
                        FrontNum.text = FrontEquipPropertylist[i].ToString();
                        break;
                    case 10:
                        FrontNum.text = FrontEquipPropertylist[i].ToString();
                        break;
                    case 11:
                        FrontNum.text = FrontEquipPropertylist[i].ToString();
                        break;
                    case 12:
                        FrontNum.text = FrontEquipPropertylist[i].ToString();
                        break;
                    case 13:
                        FrontNum.text = FrontEquipPropertylist[i].ToString();
                        break;
                    case 14:
                        FrontNum.text = FrontEquipPropertylist[i].ToString();
                        break;
                    case 15:
                        FrontNum.text = FrontEquipPropertylist[i].ToString();
                        break;
                    case 16:
                        FrontNum.text = FrontEquipPropertylist[i].ToString();
                        break;
                    case 17:
                        FrontNum.text = FrontEquipPropertylist[i].ToString();
                        break;
                }
            }
        }
        for (int i = 0; i < QueenEquipPropertyList.Length; i++)
        {
            if (index == i)
            {
                switch (index)
                {
                    case 0:
                        QueenNum.text = QueenEquipPropertyList[i].ToString();
                        break;
                    case 1:
                        QueenNum.text = QueenEquipPropertyList[i].ToString();
                        break;
                    case 2:
                        QueenNum.text = QueenEquipPropertyList[i].ToString();
                        break;
                    case 3:
                        QueenNum.text = QueenEquipPropertyList[i].ToString();
                        break;
                    case 4:
                        QueenNum.text = QueenEquipPropertyList[i].ToString();
                        break;
                    case 5:
                        QueenNum.text = QueenEquipPropertyList[i].ToString();
                        break;
                    case 6:
                        QueenNum.text = QueenEquipPropertyList[i].ToString();
                        break;
                    case 7:
                        QueenNum.text = QueenEquipPropertyList[i].ToString();
                        break;
                    case 8:
                        QueenNum.text = QueenEquipPropertyList[i].ToString();
                        break;
                    case 9:
                        QueenNum.text = QueenEquipPropertyList[i].ToString();
                        break;
                    case 10:
                        QueenNum.text = QueenEquipPropertyList[i].ToString();
                        break;
                    case 11:
                        QueenNum.text = QueenEquipPropertyList[i].ToString();
                        break;
                    case 12:
                        QueenNum.text = QueenEquipPropertyList[i].ToString();
                        break;
                    case 13:
                        QueenNum.text = QueenEquipPropertyList[i].ToString();
                        break;
                    case 14:
                        QueenNum.text = QueenEquipPropertyList[i].ToString();
                        break;
                    case 15:
                        QueenNum.text = QueenEquipPropertyList[i].ToString();
                        break;
                    case 16:
                        QueenNum.text = QueenEquipPropertyList[i].ToString();
                        break;
                    case 17:
                        QueenNum.text = QueenEquipPropertyList[i].ToString();
                        break;
                }
            }
        }
    }
    public void refreshUI(ItemEquip IE)
    {
        IEs = IE;
        FrontEquipPropertylist = HeroAndEquipNodeData.Instance().GetFrontEquipPropertyNum(IE);
        QueenEquipPropertyList = HeroAndEquipNodeData.Instance().GetQueenEquipPropertyNum(IE);
    }
}
