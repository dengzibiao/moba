/*
文件名（File Name）:   UI_ShopSell.cs

作者（Author）:    #高#

创建时间（CreateTime）:  2016-7-1 16:46:51
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIShopSell : GUIBase
{

    public static UIShopSell instance;
    public GUISingleButton closeBtn;
    public GUISingleButton sellBtn;
    public GUISingleLabel priceTxt;
    public GUISingleLabel desTxt;
    public GUISingleSprite money;
    public GUISingleSprite icon;
    private List<ItemData> bagGoldItem = new List<ItemData>();
    public GUISingleMultList multList;
    private Transform view;
    public UIShopSell()
    {
        instance = this;
    }

    protected override void Init()
    {
        closeBtn.onClick = OnCloseClick;
        sellBtn.onClick = OnSellClick;
        view = transform.Find("Scroll View");
    }
    protected override void OnLoadData()
    {
        base.OnLoadData();
        Show();
    }
    /// <summary>
    /// 出售按钮
    /// </summary>
    private void OnSellClick()
    {
        Globe.isOnce = false;
        GoodsDataOperation.GetInstance().SaleGoldProp();
        //UIMask.Instance.ClosePanle(GameLibrary.UI_ShopSell);
        Control.HideGUI(this.GetUIKey());
    }
    private void OnCloseClick()
    {
        //Globe.isOnce = false;
        price =0;
      //  UIMask.Instance.ClosePanle(GameLibrary.UI_ShopSell);
        Control.HideGUI(this.GetUIKey());
    }
    /// <summary>
    /// 检测背包金币道具
    /// </summary>
    /// <returns></returns>
    public object[] CheckGoldItem()
    {
        bagGoldItem = playerData.GetInstance().GetItemListByItmeType(ItemType.GoldProp);
        return bagGoldItem.ToArray();
    }

    private int price = 0;
    protected override void ShowHandler()
    {
        CheckGoldItem();
        for (int i = 0; i < bagGoldItem.Count; i++)
        {
            price += (bagGoldItem[i].Count)*(bagGoldItem[i].Sprice);
        }
        priceTxt.text = price.ToString();
        desTxt.text = "[acd5ff]是否将以下[ffc937] 无用[-]商品出售?[-]";
        multList.InSize(bagGoldItem.Count, 2);
        multList.Info(CheckGoldItem());
        multList.ScrollView = view;
    }

    public override UIPanleID GetUIKey()
    {
        return UIPanleID.UIShopSell;
    }

}
