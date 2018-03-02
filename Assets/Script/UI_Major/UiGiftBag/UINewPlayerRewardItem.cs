/*
文件名（File Name）:   UINewPlayerRewardItem.cs

作者（Author）:    #高#

创建时间（CreateTime）:  #CreateTime#
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UINewPlayerRewardItem : GUISingleItemList
{

    private ItemData item;
    public GUISingleButton icon;
    public GUISingleLabel count;
    public Transform debris;
    public UIGrid grid;
    public List<GameObject> star = new List<GameObject>();
    protected override void InitItem()
    {
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
        if (obj == null)
        {

        }
        else
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
            if (item.Star != 0)
            {
                count.text = "";
                for (int j = 0; j < item.Star; j++)
                {
                    star[j].SetActive(true);
                }
                for (int j = item.Star; j < item.Star; j++)
                {
                    star[j].SetActive(false);
                }
                grid.Reposition();
            }
            if (item.Types == 6)
            {
                debris.gameObject.SetActive(true);
            }
            else
            {
                debris.gameObject.SetActive(false);
            }
            icon.GetComponent<UISprite>().atlas = item.UiAtlas;
            switch (item.GradeTYPE)
            {
                case GradeType.Gray:
                    A_Sprite.spriteName = "hui";
                    break;
                case GradeType.Green:
                    A_Sprite.spriteName = "lv";
                    break;
                case GradeType.Blue:
                    A_Sprite.spriteName = "lan";
                    break;
                case GradeType.Purple:
                    A_Sprite.spriteName = "zi";
                    break;
                case GradeType.Orange:
                    A_Sprite.spriteName = "cheng";
                    break;
                case GradeType.Red:
                    A_Sprite.spriteName = "hong";
                    break;
                default:
                    break;
            }

        }
    }
}
