/*
文件名（File Name）:   Every.cs

作者（Author）:    #高#

创建时间（CreateTime）:  #CreateTime#
*/
using UnityEngine;
using System.Collections;
using Tianyu;

public class UIEveryRewardsItem : GUISingleItemList
{
    public GUISingleSprite itemIcon;
    public GUISingleSprite point;
    public GUISingleLabel itemCount;
    private ItemData item;
    public override void Info(object obj)
    {
        if (obj == null)
        {

        }
        else
        {
            item = (ItemData)obj;

            // icon.spriteName = data.IconName;
        }
    }

    protected override void ShowHandler()
    {
        base.ShowHandler();


        if (itemIcon != null)
        {
            itemIcon.uiAtlas = item.UiAtlas;
        }
        if (item.Types == 6)
        {
            point.gameObject.SetActive(true);
        }
        else
        {
            point.gameObject.SetActive(false);
        }
        if (item.IconName != null)
        {
            itemIcon.spriteName = item.IconName;
        }
        if (item.Count != null)
        {
            itemCount.text = item.Count.ToString();
        }
        A_Sprite.spriteName = GoodsDataOperation.GetInstance().GetFrameByGradeType(item.GradeTYPE);
    }
}
