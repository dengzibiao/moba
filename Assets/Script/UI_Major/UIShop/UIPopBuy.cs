using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class UIPopBuy : GUIBase
{
    public static UIPopBuy instance;
    public GUISingleButton closeBtn;
    public GUISingleButton buyBtn;
    public GUISingleLabel priceTxt;
    public GUISingleLabel nameTxt;
    public GUISingleLabel desTxt;
    //public GUISingleLabel refreshCountTxt;
    public GUISingleLabel buyCountTxt;
    public GUISingleSprite money;
    public GUISingleSprite icon;

    public GUISingleSprite frame;
    public GUISingleSprite point;
    public GUISingleLabel countTxt;
    private ItemData vo;
    private GameObject go;
    private int _index = 1;//商店类型
    public UIPopBuy()
    {
        instance = this;
    }

    protected override void Init()
    {
        closeBtn.onClick = OnCloseClick;
        buyBtn.onClick = OnBuyClick;
    }

    //public void InitData(object obj, GameObject go)
    //{
    //    vo = (ItemData)obj;
    //    this.go = go;
    //}
    protected override void SetUI(params object[] uiParams)
    {
        if (uiParams.Length > 0)
        {
            this.vo = (ItemData)uiParams[0];
            this.go = (GameObject)uiParams[1];
        }

        base.SetUI(uiParams);
    }

    protected override void OnLoadData()
    {
        base.OnLoadData();
        Show();
    }
    public void ShowMask()
    {
        if (go== null)
        {
            return;
        }

      //  go.transform.Find("Mask").gameObject.SetActive(true);
        vo.IsBuy = true;
        vo.Cprice = 0;
    }

    private void OnBuyClick()
    {
        Send2Message(vo.MoneyTYPE, vo.Id);
    }

    private void Send2Message(MoneyType moneytype, long id)
    {
        
        if (go == null)
        {
            return;
        }
        switch (moneytype)
        {
            case MoneyType.Gold:
                if (playerData.GetInstance().lotteryInfo.sale == 1)
                {
                    if (playerData.GetInstance().baginfo.gold >= vo.Cprice)
                    {
                        ClientSendDataMgr.GetSingle().GetCShopSend().ShopBuy(id, _index);
                    }
                    else
                    {
                        //UIPromptBox.Instance.ShowLabel("您的金币不足！");
                        Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, "您的金币不足！");
                    }
                }
                else
                {
                    if (playerData.GetInstance().baginfo.gold >= vo.Cprice* playerData.GetInstance().lotteryInfo.sale)
                    {
                        ClientSendDataMgr.GetSingle().GetCShopSend().ShopBuy(id, _index);
                    }
                    else
                    {
                        //UIPromptBox.Instance.ShowLabel("您的金币不足！");
                        Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, "您的金币不足！");
                    }
                }
                
                break;
            case MoneyType.Diamond:
                if (playerData.GetInstance().lotteryInfo.sale == 1)
                {
                    if (playerData.GetInstance().baginfo.diamond >= vo.Cprice)
                    {
                        ClientSendDataMgr.GetSingle().GetCShopSend().ShopBuy(id, _index);
                    }
                    else
                    {
                        //UIPromptBox.Instance.ShowLabel("您的钻石不足！");
                        Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, "您的钻石不足！");
                    }
                }
                else
                {
                    if (playerData.GetInstance().baginfo.diamond >= vo.Cprice*playerData.GetInstance().lotteryInfo.sale)
                    {
                        ClientSendDataMgr.GetSingle().GetCShopSend().ShopBuy(id, _index);
                    }
                    else
                    {
                        //UIPromptBox.Instance.ShowLabel("您的钻石不足！");
                        Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, "您的钻石不足！");
                    }
                }
               
                break;
            case MoneyType.PVPcoin:

                if (playerData.GetInstance().lotteryInfo.sale == 1)
                {
                    if (playerData.GetInstance().baginfo.pvpCoin >= vo.Cprice)
                    {
                        ClientSendDataMgr.GetSingle().GetCShopSend().ShopBuy(id, (int)ShopType.abattoir);
                    }
                    else
                    {
                        //UIPromptBox.Instance.ShowLabel("您的角斗币不足！");
                        Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, "您的角斗币不足！");
                    }
                }
                else
                {
                    if (playerData.GetInstance().baginfo.pvpCoin >= vo.Cprice* playerData.GetInstance().lotteryInfo.sale)
                    {
                        ClientSendDataMgr.GetSingle().GetCShopSend().ShopBuy(id, (int)ShopType.abattoir);
                    }
                    else
                    {
                        //UIPromptBox.Instance.ShowLabel("您的角斗币不足！");
                        Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, "您的角斗币不足！");
                    }
                }
               
                break;
            case MoneyType.AreanCoin:
                if (playerData.GetInstance().lotteryInfo.sale == 1)
                {
                    if (playerData.GetInstance().baginfo.areanCoin >= vo.Cprice)
                    {
                        ClientSendDataMgr.GetSingle().GetCShopSend().ShopBuy(id, (int)ShopType.Arena);
                    }
                    else
                    {
                        //UIPromptBox.Instance.ShowLabel("您的竞技币不足！");
                        Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, "您的竞技币不足！");
                    }
                }
                else
                {
                    if (playerData.GetInstance().baginfo.areanCoin >= vo.Cprice * playerData.GetInstance().lotteryInfo.sale)
                    {
                        ClientSendDataMgr.GetSingle().GetCShopSend().ShopBuy(id, (int)ShopType.Arena);
                    }
                    else
                    {
                        //UIPromptBox.Instance.ShowLabel("您的竞技币不足！");
                        Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, "您的竞技币不足！");
                    }
                }
                
                break;
            case MoneyType.RewardCoin:
                if (playerData.GetInstance().lotteryInfo.sale == 1)
                {
                    if (playerData.GetInstance().baginfo.rewardCoin >= vo.Cprice)
                    {
                        ClientSendDataMgr.GetSingle().GetCShopSend().ShopBuy(id, (int)ShopType.Reward);
                    }
                    else
                    {
                        //UIPromptBox.Instance.ShowLabel("您的悬赏币不足！");
                        Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, "您的悬赏币不足！");
                    }
                }
                else
                {
                    if (playerData.GetInstance().baginfo.rewardCoin >= vo.Cprice* playerData.GetInstance().lotteryInfo.sale)
                    {
                        ClientSendDataMgr.GetSingle().GetCShopSend().ShopBuy(id, (int)ShopType.Reward);
                    }
                    else
                    {
                        //UIPromptBox.Instance.ShowLabel("您的悬赏币不足！");
                        Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, "您的悬赏币不足！");
                    }
                }
               
                break;
            //case MoneyType.PVEcoin:
              //  if()
            //case MoneyType.PVPcoin:
            //    if(playerData.GetInstance().baginfo.)
        }
       // Control.HideGUI(this.GetUIKey());
    }
    private void OnCloseClick()
    {
        // Hide();
        Control.HideGUI(this.GetUIKey());
    }

    protected override void ShowHandler()
    {
        if(playerData.GetInstance().lotteryInfo.sale==1)
        {
            priceTxt.text = (vo.Cprice).ToString();
        }
        else
        {
            priceTxt.text = (vo.Cprice * playerData.GetInstance().lotteryInfo.sale).ToString();
        }
       
        point.gameObject.SetActive(false);
        if (int.Parse(StringUtil.SubString(vo.Id.ToString(), 3)) == 107 || int.Parse(StringUtil.SubString(vo.Id.ToString(), 3)) == 106)
        {
            icon.uiAtlas = ResourceManager.Instance().GetUIAtlas("UIHeroHead");
        }
        else
        {
            icon.uiAtlas = ResourceManager.Instance().GetUIAtlas("Prop");
        }
        //if (int.Parse(StringUtil.SubString(vo.Id.ToString(), 3)) == 106)
        //{
        //    point.gameObject.SetActive(true);
        //}
        if (vo.Types == 6 || vo.Types == 3)
        {
            point.gameObject.SetActive(true);
        }
        if (vo.Types == 3)
        {
            point.spriteName = "materialdebris";
            point.GetComponent<UISprite>().MakePixelPerfect();
        }
        else if (vo.Types == 6)
        {
            point.spriteName = "linghunshi";
            point.GetComponent<UISprite>().MakePixelPerfect();
        }
        nameTxt.text = GoodsDataOperation.GetInstance().JointNameColour(vo.Name, vo.GradeTYPE);
        icon.spriteName = vo.IconName;
        frame.spriteName = ItemData.GetFrameByGradeType(vo.GradeTYPE);
        if (vo.Describe != null)
        {
            desTxt.text = GoodsDataOperation.GetInstance().ConvertGoodsDes(vo);
        }
        else
        {
            desTxt.text = "道具的属性，功能，刷新一批新货物需要消耗";
        }
        switch (vo.MoneyTYPE)
        {
            case MoneyType.Gold:
                money.spriteName = "jinbi"; break;
            case MoneyType.Diamond:
                money.spriteName = "zuanshi"; break;
            case MoneyType.PVPcoin:
                money.spriteName = "juedoubi";break;
            case MoneyType.AreanCoin:
                money.spriteName = "jingjibi";break;
            case MoneyType.PVEcion:
                money.spriteName="";break;
            case MoneyType.RewardCoin:
                money.spriteName="xuanshangbi";break;
        }
        buyCountTxt.text = "购买"+ vo.Count + "件";
       // refreshCountTxt.text = "是否继续（今日已刷新" + UIPopRefresh.count + "次)";
        countTxt.text = "拥有[54ed00]"+playerData.GetInstance().GetItemCountById(vo.Id) + "[-]" + "件";
    }

    public override UIPanleID GetUIKey()
    {
        return UIPanleID.UIPopBuy;
    }

}
