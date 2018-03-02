/*
文件名（File Name）:   RewardGoodsItem.cs

作者（Author）:    #高#

创建时间（CreateTime）:  2016-11-7 14:54:36
*/
using UnityEngine;
using System.Collections;

public class RewardGoodsItem : GUISingleItemList
{
    public GUISingleSprite itemIcon;
    public GUISingleSprite point;
    public GUISingleLabel itemCount;
    private ItemData vo;
    public override void Info(object obj)
    {
        base.Info(obj);
        if (obj == null)
        {

        }
        else
        {
            vo = (ItemData)obj;
        }
    }
    protected override void ShowHandler()
    {
        base.ShowHandler();


        if (itemIcon != null)
        {
            itemIcon.uiAtlas = vo.UiAtlas;
        }
        if (vo.Types == 6)
        {
            point.gameObject.SetActive(true);
        }
        else
        {
            point.gameObject.SetActive(false);
        }
        itemCount.text = vo.Count.ToString();
        itemIcon.spriteName = vo.IconName;
        A_Sprite.spriteName = GoodsDataOperation.GetInstance().GetFrameByGradeType(vo.GradeTYPE);
    }
}
