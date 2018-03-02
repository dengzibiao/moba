using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class UIShopPanel : GUIBase
{
    public GUISingleCheckBoxGroup checkBoxs;
    public GUISingleButton backBtn;
    private GameObject[] go;
    private int _index;
    private int _indexs;
    private static UIShopPanel instance;
    public static UIShopPanel Instance
    {
        get { return instance; }
        set { instance = value; }
    }
    public UIShopPanel()
    {
        instance = this;
    }
    protected override void Init()
    {
        base.Init();
        checkBoxs.onClick = OnCheckBoxs;
        backBtn.onClick = OnBackClick;

      //  checkBoxs.DefauleIndex = 0;
    }
    public void OnCheckBoxs(int index, bool boo)
    {
        if (boo)
        {
            _index = index;
            switch (index)
            {
                case 0:
                   // ClientSendDataMgr.GetSingle().GetCShopSend().RequestGoodsList((int)ShopType.Prop);
                    break;
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    break;
            }
        }
    }

    protected override void ShowHandler()
    {
       checkBoxs.DefauleIndex = this._index;
    }
    protected override void SetUI(params object[] uiParams)
    {
        this._index = (int)uiParams[0];//设置显示第一个标签
        this._indexs = (int)uiParams[1];//设置小面板
        base.SetUI();
    }
    protected override void OnLoadData()
    {
        base.OnLoadData();
        Singleton<Notification>.Instance.RegistMessageID(MessageID.common_shop_goods_list_ret, UIPanleID.UIShopPanel);//购买是否成功
        Singleton<Notification>.Instance.RegistMessageID(MessageID.common_buy_shop_goods_ret, UIPanleID.UIShopPanel);//创建商店物品，商品数据
        Singleton<Notification>.Instance.RegistMessageID(MessageID.common_refresh_shop_goods_ret, UIPanleID.UIShopPanel);//刷新物品列表

        //请求第一个 
        SendInfo(this._index);

 
       
    }
    public void SendInfo(int _indexx)
    {

        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        switch (_indexx)
        {
            case 0:
                if (this._indexs==0)
                {
                    newpacket.Add("arg1", (int)ShopType.Prop);//商店类型
                    newpacket.Add("arg2", playerData.GetInstance().selfData.level);//人物等级
                    Singleton<Notification>.Instance.Send(MessageID.common_shop_goods_list_req, newpacket, C2SMessageType.ActiveWait);
                }
                else if(this._indexs==1)
                {
                    newpacket.Add("arg1", (int)ShopType.abattoir);//商店类型
                    newpacket.Add("arg2", playerData.GetInstance().selfData.level);//人物等级
                    Singleton<Notification>.Instance.Send(MessageID.common_shop_goods_list_req, newpacket, C2SMessageType.ActiveWait);
                }
                else if(this._indexs==2)
                {
                    newpacket.Add("arg1", (int)ShopType.Arena);//商店类型
                    newpacket.Add("arg2", playerData.GetInstance().selfData.level);//人物等级
                    Singleton<Notification>.Instance.Send(MessageID.common_shop_goods_list_req, newpacket, C2SMessageType.ActiveWait);
                }

                break;

        }
        
    }
    public override void ReceiveData(uint messageID)
    {
        base.ReceiveData(messageID);
        switch(messageID)
        {
            case MessageID.common_shop_goods_list_ret:
                Show();

                UIShop.Instance.IsShop(this._indexs);

               // UIShop.Instance.RefrshData();
                break;
            case MessageID.common_buy_shop_goods_ret:
                //Show();
                UIPopBuy.instance.ShowMask();
                UIShop.Instance.UpdateShow();
                Control.HideGUI(UIPanleID.UIPopBuy);
                break;
            case MessageID.common_refresh_shop_goods_ret:
                Show();
                break;
        }
    }
    private void OnBackClick()
    {
        
        Control.ShowGUI(UIPanleID.UIMoney, EnumOpenUIType.DefaultUIOrSecond);
        GetComponentInChildren<UIShop>().coinTypeSp.gameObject.SetActive(false);
        GetComponentInChildren<UIShop>().coinTypeLab.gameObject.SetActive(false);
        GetComponentInChildren<UIShop>().checkBoxs.setMaskState(0);
        GetComponentInChildren<UIShop>().isOnce = false;
        GetComponentInChildren<UIShop>()._index = 1;
        GetComponentInChildren<UIShop>().checkBoxs.index = 0;
        Control.HideGUI();
        Control.PlayBGmByClose(this.GetUIKey());
    }

    public override UIPanleID GetUIKey()
    {
        return UIPanleID.UIShopPanel;
    }

}
