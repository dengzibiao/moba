/*
文件名（File Name）:   UI_ShopSell.cs

作者（Author）:    #高#

创建时间（CreateTime）:  2016-7-1 16:46:51
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tianyu;
public class UIShopItem : GUISingleItemList
{
    public GUISingleButton buyBtn;
    public GUISingleSprite icon;
    public GUISingleSprite point;
    public GUISingleLabel nameTxt;
    public GUISingleLabel lable;
    private GameObject uiMask;
    private GameObject uiPopBuy;
    public GUISingleLabel priceLabel;
    public UISprite money;
    private object item;
    public GUISingleSprite frame;
    public GUISingleSprite sprite;
    public UISprite salepriceMark;//特价显示
    public UISprite salePriceLien;//打折价
    private ItemData itemData;
    /// <summary>
    /// item初始化函数
    /// </summary>
    protected override void InitItem()
    {
        //初始化
        money =transform.Find("Money").GetComponent<UISprite>();
        salepriceMark=transform.Find("SalepriceMark").GetComponent<UISprite>();
        salePriceLien = transform.Find("SalePriceLien").GetComponent<UISprite>();
        buyBtn.onClick = OnBtnClick;
        UIEventListener.Get(A_Sprite.gameObject).onClick = OnBuyBtnClick;
        GameObject mask = transform.Find("Mask").gameObject;
    }

    private void OnBuyBtnClick(GameObject go)
    {
        OnBtnClick();
    }

    protected override void OnComponentHover(bool state)
    {

    }

    public void  BuyResult()
    { 
        if (this.gameObject  == null)
        {
            return;
        }
        GameObject mask = transform.Find("Mask").gameObject;
        mask.SetActive(true);
    }
    public override void Info(object obj)
    {
        //ID 道具表
        item = obj;
        if (obj == null)
        {
            nameTxt.text = "";
        }
        else
        {
            itemData = (ItemData) obj;
            if (int.Parse(itemData.Id.ToString().Substring(0, 3)) == 107 || int.Parse(itemData.Id.ToString().Substring(0, 3)) == 106)
            {
                icon.uiAtlas = ResourceManager.Instance().GetUIAtlas("UIHeroHead");
            }
            else
            {
                icon.uiAtlas = ResourceManager.Instance().GetUIAtlas("Prop");
            }
            if (itemData.Types == 6 || itemData.Types == 3)
            {
                point.gameObject.SetActive(true);
            }
            if (itemData.Types == 3)
            {
                point.spriteName = "materialdebris";
                point.GetComponent<UISprite>().MakePixelPerfect();
            }
            else if (itemData.Types == 6)
            {
                point.spriteName = "linghunshi";
                point.GetComponent<UISprite>().MakePixelPerfect();
            }
            //if ((int.Parse(StringUtil.SubString(itemData.Id.ToString(), 3)) == 106))
            //{
            //    point.gameObject.SetActive(true);
            //}

            if (playerData.GetInstance().lotteryInfo.sale== 1)
            {
                salepriceMark.gameObject.SetActive(false);
                salePriceLien.gameObject.SetActive(false);
            }
            else
            {
                salepriceMark.gameObject.SetActive(true);
                salePriceLien.gameObject.SetActive(true);
                salePriceLien.GetComponentInChildren<UILabel>().text = (itemData.Cprice * playerData.GetInstance().lotteryInfo.sale).ToString();//折扣价格
            }
            salepriceMark.GetComponentInChildren<UILabel>().text = FSDataNodeTable<ShopNode>.GetSingleton().DataNodeList[UIShop.Instance._index].saleShow;
            nameTxt.text = GoodsDataOperation.GetInstance().JointNameColour(itemData.Name, itemData.GradeTYPE);
            priceLabel.text = itemData.Cprice.ToString();
            lable.text = itemData.Count.ToString();
            icon.spriteName = itemData.IconName;
            if (itemData.Count== 0||itemData.IsBuy==true)
            {
                BuyResult();
            }
            switch (((ItemData)obj).MoneyTYPE)
            {
                case MoneyType.Gold:
                    money.spriteName = "jinbi";
                    break;
                case MoneyType.Diamond:
                    money.spriteName = "zuanshi";
                    break;
                case MoneyType.PVPcoin:
                    money.spriteName = "juedoubi";
                    break;
                case MoneyType.AreanCoin:
                    money.spriteName = "jingjibi"; 
                    break;
                case MoneyType.PVEcion:
                    money.spriteName = "";break;
                case MoneyType.RewardCoin:
                    money.spriteName = "xuanshangbi";
                    break;

            }
            switch (itemData.GradeTYPE)
            {
                case GradeType.Gray:
                     frame.spriteName = "hui";
                sprite.spriteName = "";
                    break;
                case GradeType.Green:
                    frame.spriteName = "lv";
                sprite.spriteName = "";
                    break;
                case GradeType.Blue:
                   frame.spriteName = "lan";
                sprite.spriteName = "";
                    break;
                case GradeType.Purple:
                  frame.spriteName = "zi";
                    PlayEffect.mActive = true;
                sprite.GetComponent<PlayEffect>().Name = "2_000";
                    break;
                case GradeType.Orange:
                    frame.spriteName = "cheng";
                    PlayEffect.mActive = true;
                sprite.GetComponent<PlayEffect>().Name = "0_000";
                    break;
                case GradeType.Red:
                     frame.spriteName = "hong";
                    PlayEffect.mActive = true;
                    sprite.GetComponent<PlayEffect>().Name = "1_000";
                    break;
            }
            //if(UIShop.Instance._index==(int)ShopType.Prop)
            //{
            //    salepriceMark.GetComponentInChildren<UILabel>().text = FSDataNodeTable<ShopNode>.GetSingleton().DataNodeList[1].saleShow;
            //}
            //else if(UIShop.Instance._index==(int)ShopType.abattoir)
            //{
            //    salepriceMark.GetComponentInChildren<UILabel>().text = FSDataNodeTable<ShopNode>.GetSingleton().DataNodeList[7].saleShow;
            //}
            //else if(UIShop.Instance._index==(int)ShopType.Arena)
            //{
            //    salepriceMark.GetComponentInChildren<UILabel>().text = FSDataNodeTable<ShopNode>.GetSingleton().DataNodeList[5].saleShow;
            //}
            //else if (UIShop.Instance._index == (int)ShopType.Reward)
            //{
            //    salepriceMark.GetComponentInChildren<UILabel>().text = FSDataNodeTable<ShopNode>.GetSingleton().DataNodeList[8].saleShow;
            //}
          }
    }

    private void OnBtnClick()
    {
         //Control.GetGUI(GameLibrary.UI_PopBuy).GetComponent<UIPopBuy>().InitData(item,this.gameObject);
        // UIMask.Instance.ShowPanle(GameLibrary.UI_PopBuy);

         object[] obj = new object[2] { item, this.gameObject };
         Control.ShowGUI(UIPanleID.UIPopBuy, EnumOpenUIType.DefaultUIOrSecond, false, obj);
         //Control.ShowGUI(UIPanleID.UIPopBuy, EnumOpenUIType.DefaultUIOrSecond, false, item);
        //UIMask.Instance.ShowPanle(UIPanleID.UIPopBuy.ToString());
    }
}
