using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 考古奖励物品面板
/// </summary>
public class UIArchaeologyPanelItem : GUISingleItemList
{
    private Archaeology_rewardNode archNode;
    private ItemData itemData;
    public UISprite itemIcon;
    protected override void InitItem()
    {
        itemIcon = transform.Find("ItemIcon").GetComponent<UISprite>();
    }
    public override void Info(object obj)
    {
        itemData = (ItemData)obj;

        if (itemData.GoodsType == MailGoodsType.ItemType)
        {
            if (GameLibrary.Instance().ItemStateList.ContainsKey(itemData.Id))
            {
                itemIcon.spriteName = GameLibrary.Instance().ItemStateList[itemData.Id].icon_name;
            }
        }

    }

}