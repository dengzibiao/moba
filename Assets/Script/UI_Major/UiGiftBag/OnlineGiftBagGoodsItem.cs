using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Tianyu;

public class OnlineGiftBagGoodsItem : GUISingleItemList
{

    public UISprite gradeIcon;
    public GUISingleButton icon;
    public UILabel count;
    public Transform debris;
    private ItemData item;
    protected override void InitItem()
    {
        gradeIcon = transform.GetComponent<UISprite>();
        icon = transform.Find("Icon").GetComponent<GUISingleButton>();
        count = transform.Find("Count").GetComponent<UILabel>();
        debris = transform.Find("Debris");
        icon.onClick = OnIconClick;
    }
    private void OnIconClick()
    {
        UIgoodstips.Instances.SetItemData(item);
        Control.Show(UIPanleID.UIgoodstips);
    }
    public override void Info(object obj)
    {
        item = (ItemData)obj;
        if (item.GoodsType == MailGoodsType.DiamomdType)
        {
            count.text = item.Diamond + "";
            icon.spriteName = "zuanshi";
        }
        if (item.GoodsType == MailGoodsType.GoldType)
        {
            count.text = item.Gold + "";
            icon.spriteName = "jinbi";
        }
        if (item.GoodsType == MailGoodsType.ItemType)
        {
            count.text = item.Count + "";

            if (GameLibrary.Instance().ItemStateList.ContainsKey(item.Id))
            {
                icon.spriteName = GameLibrary.Instance().ItemStateList[item.Id].icon_name;
            }
        }
        icon.GetComponent<UISprite>().atlas = item.UiAtlas;
        if (item.Types == 6)
        {
            debris.gameObject.SetActive(true);
        }
        else
        {
            debris.gameObject.SetActive(false);
        }
        switch (item.GradeTYPE)
        {
            case GradeType.Gray:
                gradeIcon.spriteName = "hui";
                break;
            case GradeType.Green:
                gradeIcon.spriteName = "lv";
                break;
            case GradeType.Blue:
                gradeIcon.spriteName = "lan";
                break;
            case GradeType.Purple:
                gradeIcon.spriteName = "zi";
                break;
            case GradeType.Orange:
                gradeIcon.spriteName = "cheng";
                break;
            case GradeType.Red:
                gradeIcon.spriteName = "hong";
                break;
            default:
                break;
        }
    }
}
