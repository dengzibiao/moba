/*
文件名（File Name）:   UI_ShopSell.cs

作者（Author）:    #高#

创建时间（CreateTime）:  2016-7-1 16:46:51
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tianyu;
using System;
//using UnityEngine.EventSystems;
public class UIShop : GUIBase
{
    private static UIShop instance;
    public GUISingleCheckBoxGroup checkBoxs;
    public GUISingleMultList multList;
    public GUISingleButton refreshBtn;//刷新按钮
    // public GUISingleButton backBtn;
    public GUISingleLabel timeTxt;
    public GUISingleLabel refreshCount;
    public bool isOnce = false;//
  //  public List<ItemData> itemList = new List<ItemData>();

    private int refreshPrice;//刷新价格
    private int count = 0;//刷新次数
    public List<int> jdcRefreshCostList = new List<int>();//
    public List<int> jjcRefreshCostList = new List<int>();
    public List<int> xsRefreshCostList = new List<int>();

    public GUISingleSprite coinTypeSp;//货币类型
    public GUISingleLabel coinTypeLab;//货币总数里
    public UIButton npcBtn;//npc按钮
    public UIButton npcBtn_2;
    public UIButton npcBtn_3;
    public UIButton npcBtn_4;
    public UIPanel npcTips;//Npc对话
    public UISprite UserDiamondSp;////刷新花费钻石
    private int userDiamond;
    public GUISingleSpriteGroup points;//点
    public GUISingleButton arrowRight;
    public GUISingleButton arrowLeft;
    private int currentPage = 0;//点击页数记录

    private int indexss;//标签
    public static UIShop Instance
    {
        get { return instance; }
        set { instance = value; }
    }
    public UIShop()
    {
        instance = this;
    }
    protected override void Init()
    {
        if (FSDataNodeTable<ResetLaterNode>.GetSingleton().DataNodeList.Count > 0)
        {
                for (int i = 0; i < FSDataNodeTable<ResetLaterNode>.GetSingleton().DataNodeList[1].generalShop.Length; i++)
                {
                    int ct = FSDataNodeTable<ResetLaterNode>.GetSingleton().DataNodeList[1].generalShop[i];
                    playerData.GetInstance().lotteryInfo.refreshCost.Add(ct);
                }

                for (int j = 0; j < FSDataNodeTable<ResetLaterNode>.GetSingleton().DataNodeList[1].generalJdc.Length; j++)
                {
                    int jdc = FSDataNodeTable<ResetLaterNode>.GetSingleton().DataNodeList[1].generalJdc[j];
                    jdcRefreshCostList.Add(jdc);
                }
                for (int k = 0; k < FSDataNodeTable<ResetLaterNode>.GetSingleton().DataNodeList[1].generalJjc.Length; k++)
                {
                    int jjc = FSDataNodeTable<ResetLaterNode>.GetSingleton().DataNodeList[1].generalJjc[k];
                    jjcRefreshCostList.Add(jjc);
                }
                for (int l = 0; l < FSDataNodeTable<ResetLaterNode>.GetSingleton().DataNodeList[1].generalXs.Length; l++)
                {
                    int xs = FSDataNodeTable<ResetLaterNode>.GetSingleton().DataNodeList[1].generalXs[l];
                    xsRefreshCostList.Add(xs);
                }
        }
        multList = transform.FindComponent<GUISingleMultList>("Scroll View/MultList");
        npcTips = transform.Find("NpcTips").GetComponent<UIPanel>();
        UserDiamondSp = transform.Find("UserDiamondSp").GetComponent<UISprite>();
        npcBtn = transform.Find("NpcBtn").GetComponent<UIButton>();
        npcBtn_2 = transform.Find("NpcBtn2").GetComponent<UIButton>();
        npcBtn_3 = transform.Find("NpcBtn3").GetComponent<UIButton>();
        npcBtn_4 = transform.Find("NpcBtn4").GetComponent<UIButton>();
        checkBoxs.onClick = OnCheckClick;
        refreshBtn.onClick = OnRefreshClick;
        arrowRight.onClick = OnArrowRTClick;
        arrowLeft.onClick = OnArrowLTClick;
        playerData.GetInstance().ChangeMoneyEvent += UpdataMonery;
        // backBtn.onClick = OnBackClick;
        EventDelegate ed = new EventDelegate(this, "OnNPCClick");
        npcBtn.onClick.Add(ed);
        EventDelegate ed2 = new EventDelegate(this, "OnNPC_2Click");
        npcBtn_2.onClick.Add(ed2);
        EventDelegate ed3 = new EventDelegate(this, "OnNPC_3Click");
        npcBtn_3.onClick.Add(ed3);
        EventDelegate ed4 = new EventDelegate(this, "OnNPC_4Click");
        npcBtn_4.onClick.Add(ed4);
        _view = transform.Find("Scroll View").GetComponentInChildren<UIScrollView>();//transform.Find("Scroll View");
        
        _view.GetComponent<UIScrollView>().onDragStarted += StartDrag;
        _view.GetComponent<UIScrollView>().onDragFinished += EndDrag;
      //  lostTime = maxTimer;
       // checkBoxs.DefauleIndex = isShopss;
    
    }
    public int isShopss =0;
    /// <summary>
    /// 打开相应的商店页签 0杂货店，1角斗场，2竞技场
    /// </summary>
    /// <param name="isShopss"></param>
    public void IsShop(int isShopss)
    {
        this.isShopss = isShopss;

    }
    void Update()
    {
        ShowTime();
        timer += Time.deltaTime;
        if (timer >= 1)
        {
            lostTime -= 1;
            timer = 0;
        }
    }
    /// <summary>
    /// 商店物品排序，英雄永远在前，而且英雄id按顺序排
    /// </summary>
    public void ShopGoodsSort()
    {
        List<ItemData> itemLt = new List<ItemData>();
        var a = playerData.GetInstance().lotteryInfo.shopItemList.FindAll(x => x.Id.ToString().Substring(0, 3) == "106");
        a.Sort((x, y) => (int)(x.Id - y.Id));
        itemLt.AddRange(a);
        //a.Sort((x, y) => x.open - y.open);
        for (int i = playerData.GetInstance().lotteryInfo.shopItemList.Count - 1; i >= 0; i--)
        {
            if (playerData.GetInstance().lotteryInfo.shopItemList[i].Id.ToString().Substring(0, 3) == "106")
            {
                playerData.GetInstance().lotteryInfo.shopItemList.RemoveAt(i);
            }
        }
        playerData.GetInstance().lotteryInfo.shopItemList.InsertRange(0, a);
    }
    /// <summary>
    /// 生成商城物品
    /// </summary>
    public object[] InitItemData()
    {
        return playerData.GetInstance().lotteryInfo.shopItemList.ToArray();
    }
    public void ShowTime()
    {
        if (PropertyManager.ConvertIntDateTime(Auxiliary.GetNowTime()).Hour >= 21 && PropertyManager.ConvertIntDateTime(Auxiliary.GetNowTime()).Hour < 24)//服务器发的当前时间大于晚上9点时改变现实状态为晚上
        {
            timeTxt.text = "明日" + Convert.ToDateTime(PropertyManager.ConvertIntDateTime(playerData.GetInstance().lotteryInfo.shopTime)).ToString("HH") + "点自动刷新" + TimeManager.Instance.GetTimeClockText(lostTime);
        }
        else
        {
            timeTxt.text = "今日" + Convert.ToDateTime(PropertyManager.ConvertIntDateTime(playerData.GetInstance().lotteryInfo.shopTime)).ToString("HH") + "点自动刷新" + TimeManager.Instance.GetTimeClockText(lostTime);
        }
        if (lostTime <= 0)
        {
            ClientSendDataMgr.GetSingle().GetCShopSend().RefreshGoodsList(_index, 0, 0);//到时间后手动刷新商店列表
            lostTime = 1;
            
        }
    }
    public void SetLostTime()
    {
        if (Auxiliary.GetNowTime() - playerData.GetInstance().lotteryInfo.shopTime >= 0)//到晚上00点
        {//TOdo:放置不一致刷新暂时这样，等服务器刷新ShopTime后改为lostTime=0;
            lostTime = 24*3600;
            Debug.Log(Auxiliary.GetNowTime() - playerData.GetInstance().lotteryInfo.shopTime);
        }
        else
        {
            lostTime = (long)Mathf.Abs(Auxiliary.GetNowTime() - playerData.GetInstance().lotteryInfo.shopTime);//服务器的当前时间-服务器的刷新时间
        }
    }
    //private void OnBackClick()
    //{
    //    isOnce = false;
    //    checkBoxs.setMaskState(0);
    //    checkBoxs.index = 0;
    //    Hide();
    //}
    private void OnCheckClick(int index, bool boo)
    {
        if (boo)
        {
            this.indexss = index;
            IsShop(this.indexss);
            currentPage = 0;
            switch (index)
            {
                case 0:
                    this._index = (int)ShopType.Prop;
                    multList.gameObject.SetActive(true);
                    points.gameObject.SetActive(true);
                    npcBtn.gameObject.SetActive(true);
                    npcBtn_2.gameObject.SetActive(false);
                    npcBtn_3.gameObject.SetActive(false);
                    npcBtn_4.gameObject.SetActive(false);

                    UserDiamondSp.spriteName = "zuanshi";
                  
                   // coinTypeLab.gameObject.SetActive(false);
                    ClientSendDataMgr.GetSingle().GetCShopSend().RequestGoodsList((int)ShopType.Prop);
                    break;
                case 1:
                    this._index = (int)ShopType.abattoir;
                    multList.gameObject.SetActive(true);
                    points.gameObject.SetActive(true);
                     npcBtn.gameObject.SetActive(false);
                    npcBtn_2.gameObject.SetActive(true);
                    npcBtn_3.gameObject.SetActive(false);
                    npcBtn_4.gameObject.SetActive(false);
                    
                    coinTypeSp.spriteName = "juedoubi";
                    UserDiamondSp.spriteName = "juedoubi";
                    coinTypeLab.text = playerData.GetInstance().baginfo.pvpCoin.ToString();
                    ClientSendDataMgr.GetSingle().GetCShopSend().RequestGoodsList((int)ShopType.abattoir);
                    break;
                case 2:
                    this._index = (int)ShopType.Arena;
                    multList.gameObject.SetActive(true);
                    points.gameObject.SetActive(true);
                     npcBtn.gameObject.SetActive(false );
                    npcBtn_2.gameObject.SetActive(false);
                    npcBtn_3.gameObject.SetActive(true);
                    npcBtn_4.gameObject.SetActive(false);
                   
                    coinTypeSp.spriteName = "jingjibi";
                    UserDiamondSp.spriteName = "jingjibi";
                    coinTypeLab.text = playerData.GetInstance().baginfo.areanCoin.ToString();
                    ClientSendDataMgr.GetSingle().GetCShopSend().RequestGoodsList((int)ShopType.Arena);
                    break;
                case 3:
                    this._index = (int)ShopType.Reward;
                    multList.gameObject.SetActive(true);
                    points.gameObject.SetActive(true);
                     npcBtn.gameObject.SetActive(false);
                    npcBtn_2.gameObject.SetActive(false);
                    npcBtn_3.gameObject.SetActive(false);
                    npcBtn_4.gameObject.SetActive(true);
                    coinTypeSp.gameObject.SetActive(true);
                    coinTypeLab.gameObject.SetActive(true);
                    ClientSendDataMgr.GetSingle().GetCShopSend().RequestGoodsList((int)ShopType.Reward);
                    coinTypeSp.spriteName = "xuanshangbi";
                    UserDiamondSp.spriteName = "xuanshangbi";
                    coinTypeLab.text = playerData.GetInstance().baginfo.rewardCoin.ToString();
                    break;
                case 4:
                    Control.ShowGUI(UIPanleID.UITooltips, EnumOpenUIType.DefaultUIOrSecond, false, "此功能暂未开放！");
                    multList.gameObject.SetActive(false);
                    arrowRight.gameObject.SetActive(false);
                    arrowLeft.gameObject.SetActive(false);
                    points.gameObject.SetActive(false);
                    break;
                default:
                    break;
            }

        }

    }

    public void UpdateShow()
    {
        CreatItemData();
    }
    /// <summary>
    /// 检测背包金币道具
    /// </summary>
    /// <returns></returns>
    public bool CheckGoldItem()
    {
        List<ItemData> bagGoldItem = playerData.GetInstance().GetItemListByItmeType(ItemType.GoldProp);
        if (bagGoldItem.Count > 0)//&& this._index == 1 && Globe.isOnce == true)
        {
            return true;
        }
        return false;
    }
    protected override void ShowHandler()
    {
       checkBoxs.setMaskState(isShopss);
        RefrshData();

       if(this.indexss==0)
       {
           coinTypeLab.text = "";
           coinTypeSp.gameObject.SetActive(false);
           Control.ShowGUI(UIPanleID.UIMoney, EnumOpenUIType.DefaultUIOrSecond);
       }
       else if(this.indexss==1)
       {
           coinTypeSp.gameObject.SetActive(true);
           coinTypeLab.gameObject.SetActive(true);

           Control.HideGUI(UIPanleID.UIMoney);
       }
       else if (this.indexss == 2)
       {
           coinTypeSp.gameObject.SetActive(true);
           coinTypeLab.gameObject.SetActive(true);

           Control.HideGUI(UIPanleID.UIMoney);
       }
       else if (this.indexss == 3)
       {

       }
       }
    /// <summary>
    /// 刷新数据方法
    /// </summary>
    public void RefrshData()
    {
        refreshCount.text = "(今日已刷新" + count + "次)";
        //UserDiamondSp.GetComponentInChildren<UILabel>().text = playerData.GetInstance().lotteryInfo.refreshCost[count].ToString();
        // userDiamondLab.text = playerData.GetInstance().lotteryInfo.refreshCost[count].ToString();
        SetLostTime();
        if (!isOnce)
        {
            isOnce = true;
            if (CheckGoldItem())
            {
                //UIMask.Instance.ShowPanle(GameLibrary.UI_ShopSell);
                Control.ShowGUI(UIPanleID.UIShopSell,EnumOpenUIType.DefaultUIOrSecond);
            }
        }
        if (playerData.GetInstance().lotteryInfo.page > 1)
        {
            _view.disableDragIfFits = false;
        }
        else
        {
            _view.disableDragIfFits = true;
        }
        ClickType();
        CreatItemData();
        ReceiveDate();
    }
    /// <summary>
    /// 生成抽奖物品
    /// </summary>
    public void CreatItemData()
    {
        if (playerData.GetInstance().lotteryInfo.shopItemList != null && playerData.GetInstance().lotteryInfo.shopItemList.Count > 0)
        {
            ShopGoodsSort();
        }
        if (playerData.GetInstance().lotteryInfo.shopItemList.Count > 0)
        {
            if (playerData.GetInstance().lotteryInfo.page > 1 && currentPage == 0)
            {
                arrowRight.gameObject.SetActive(true);
                arrowLeft.gameObject.SetActive(false);
               
            }
            else if (currentPage != 0 && currentPage == playerData.GetInstance().lotteryInfo.page - 1)
            {
                arrowRight.gameObject.SetActive(false);
                arrowLeft.gameObject.SetActive(true);
            }
            else if (currentPage == 0 && currentPage == playerData.GetInstance().lotteryInfo.page - 1)
            {
                arrowLeft.gameObject.SetActive(false);
                arrowRight.gameObject.SetActive(false);
            }
            else
            {
                arrowLeft.gameObject.SetActive(true);
                arrowRight.gameObject.SetActive(true);
            } 
            points.IsShow("point2", "point1", currentPage, playerData.GetInstance().lotteryInfo.page);//currentpage当前显示个数、一共页数的个数
            multList.InSize(PageData().Length, 3);
            multList.Info(PageData());
              
            //multList.InSize(itemList.Count, 3);
            //multList.Info(InitItemData());
            //multList.ScrollView = _view;
        }
       
    }
    float startDrag=0;

    private void StartDrag()
    {
        startDrag = _view.transform.position.x;

    }
    private void EndDrag()
    {
        float endDrag = _view.transform.position.x;
        if (startDrag != 0 && startDrag != 0)
        {
            int s = GetS(endDrag);
            startDrag = 0;
            if (s == 1 && arrowLeft.gameObject.activeInHierarchy)
            {
                OnArrowLTClick();
            }
            else if (s == -1 && arrowRight.gameObject.activeInHierarchy)
            {
                OnArrowRTClick();
            }
        }
    }
    private int GetS(float endDrag)
    {
        if(startDrag-endDrag>0)
        {
            return -1;

        }
        else
        {
            return 1;

        }
    }
    private object[] PageData()
    {
        int index = 0;
        //列表数量个数大于所取区间最大值正常显示，例如数据12g个 区间最大值也是12
        if (playerData.GetInstance().lotteryInfo.shopItemList.Count >= 6 * currentPage + 6)
        {
            object[] data = new object[6];//每次显示4个数据
            for (int i = 6 * currentPage; i <= 6 * currentPage + 5; i++)
            {
                data[index] = playerData.GetInstance().lotteryInfo.shopItemList[i];
                index++;
            }
            return data;
        }
        //列表数量个数大于所取区间最小值，小于区间最大值，取最小值和列表的个数-1，例如数据6g个 区间4-7
        else if (playerData.GetInstance().lotteryInfo.shopItemList.Count >= 6 * currentPage + 1 && playerData.GetInstance().lotteryInfo.shopItemList.Count < 6 * currentPage + 6)
        {
            object[] data = new object[playerData.GetInstance().lotteryInfo.shopItemList.Count - 6 * currentPage];//显示最小区间到friendList.Count个数据
            for (int i = 6 * currentPage; i <= playerData.GetInstance().lotteryInfo.shopItemList.Count - 1; i++)
            {
                data[index] = playerData.GetInstance().lotteryInfo.shopItemList[i];
                index++;
            } 
            return data;
        }
        //列表数量个数小于所取区间最小值，取上次数据_replaceIndex-1，例如数据4g个 区间4-7
        else if (playerData.GetInstance().lotteryInfo.shopItemList.Count < 6 * currentPage + 1)
        {
            object[] data = new object[playerData.GetInstance().lotteryInfo.shopItemList.Count - 6 * (currentPage - 1)];//显示上一个最小区间到friendList.Count个数据
            for (int i = 6 * (currentPage - 1); i <= playerData.GetInstance().lotteryInfo.shopItemList.Count - 1; i++)
            {
                data[index] = playerData.GetInstance().lotteryInfo.shopItemList[i];
                index++;
            }
            currentPage -= 1;
            return data;
        }
        return null;
    }

    private void OnRefreshClick()
    {
        IsShop(this.indexss);///点刷新时标签归位
        if (this._index == (int)ShopType.Prop)
        {
            if (playerData.GetInstance().baginfo.diamond >= refreshPrice)
            {
                if (count >= playerData.GetInstance().lotteryInfo.refreshCost.Count)
                {
                    refreshPrice = playerData.GetInstance().lotteryInfo.refreshCost[playerData.GetInstance().lotteryInfo.refreshCost.Count - 1];
                }
                else
                {
                    refreshPrice = playerData.GetInstance().lotteryInfo.refreshCost[count];
                }
                object[] obj = new object[5] { "刷新一批新货物需要消耗" + refreshPrice + "钻石", "是否继续（今日已刷新" + count + "次）", UIPopupType.Refresh, this.gameObject, "RefreshSend" };
                Control.ShowGUI(UIPanleID.UIPopUp, EnumOpenUIType.DefaultUIOrSecond, false, obj);
            }
            else
            {
                //UIPromptBox.Instance.ShowLabel("您的钻石不足！");
                Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, "您的钻石不足！");
            }
        }
        else if (this._index == (int)ShopType.abattoir)
        {
            if (playerData.GetInstance().baginfo.pvpCoin >= refreshPrice)
            {
                if (count >= jdcRefreshCostList.Count)
                {
                    refreshPrice = jdcRefreshCostList[jdcRefreshCostList.Count - 1];
                }
                else
                {
                    refreshPrice = jdcRefreshCostList[count];
                }
                object[] obj = new object[5] { "刷新一批新货物需要消耗" + refreshPrice + "角斗币", "是否继续（今日已刷新" + count + "次）", UIPopupType.Refresh, this.gameObject, "RefreshSend" };
                Control.ShowGUI(UIPanleID.UIPopUp, EnumOpenUIType.DefaultUIOrSecond, false, obj);
            }
            else
            {
                //UIPromptBox.Instance.ShowLabel("您的角斗币不足！");
                Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, "您的决斗币不足！");
            }
        }
        else if (this._index == (int)ShopType.Arena)
        {
            if (playerData.GetInstance().baginfo.areanCoin >= refreshPrice)
            {
                if (count >= jjcRefreshCostList.Count)
                {
                    refreshPrice = jjcRefreshCostList[jjcRefreshCostList.Count - 1];
                }
                else
                {
                    refreshPrice = jjcRefreshCostList[count];
                }
                object[] obj = new object[5] { "刷新一批新货物需要消耗" + refreshPrice + "竞技币", "是否继续（今日已刷新" + count + "次）", UIPopupType.Refresh, this.gameObject, "RefreshSend" };
                Control.ShowGUI(UIPanleID.UIPopUp, EnumOpenUIType.DefaultUIOrSecond, false, obj);
            }
            else
            {
                //UIPromptBox.Instance.ShowLabel("您的竞技币不足！");
                Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, "您的竞技币不足！");
            }
        }
        else if (this._index == (int)ShopType.Reward)
        {
            if (playerData.GetInstance().baginfo.rewardCoin >= refreshPrice)
            {
                if (count >= xsRefreshCostList.Count)
                {
                    refreshPrice = xsRefreshCostList[xsRefreshCostList.Count - 1];
                }
                else
                {
                    refreshPrice = xsRefreshCostList[count];
                }
                object[] obj = new object[5] { "刷新一批新货物需要消耗" + refreshPrice + "悬赏币", "是否继续（今日已刷新" + count + "次）", UIPopupType.Refresh, this.gameObject, "RefreshSend" };
                Control.ShowGUI(UIPanleID.UIPopUp, EnumOpenUIType.DefaultUIOrSecond, false, obj);
            }
            else
            {
                //UIPromptBox.Instance.ShowLabel("您的悬赏币不足！");
                Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, "您的悬赏币不足！");
            }
        }
    }
    private void RefreshSend()
    {
        //if (refreshPrice > playerData.GetInstance().baginfo.diamond)
        //{
        //    UITooltips.Instance.SetBlackerBottom_Text("您的钻石不足");
        //}
        //else
        //{
        ClientSendDataMgr.GetSingle().GetCShopSend().RefreshGoodsList(_index, count, refreshPrice);
        //   }
    }
    public void ReceiveDate()
    {
        count = playerData.GetInstance().lotteryInfo.shopRefreshCount;
        if (this._index == (int)ShopType.Prop)
        {
            if (count >= playerData.GetInstance().lotteryInfo.refreshCost.Count)
            {
                userDiamond = playerData.GetInstance().lotteryInfo.refreshCost[playerData.GetInstance().lotteryInfo.refreshCost.Count - 1];
            }
            else
            {
                userDiamond = playerData.GetInstance().lotteryInfo.refreshCost[count];
            }
        }
        else if (this._index == (int)ShopType.abattoir)
        {
            if (count >= jdcRefreshCostList.Count)
            {
                userDiamond = jdcRefreshCostList[jdcRefreshCostList.Count - 1];
            }
            else
            {
                userDiamond = jdcRefreshCostList[count];
            }
        }
        else if (this._index == (int)ShopType.Arena)
        {
            if (count >= jjcRefreshCostList.Count)
            {
                userDiamond = jjcRefreshCostList[jjcRefreshCostList.Count - 1];
            }
            else
            {
                userDiamond = jjcRefreshCostList[count];
            }
        }
        else if (this._index == (int)ShopType.Reward)
        {
            if (count >= xsRefreshCostList.Count)
            {
                userDiamond = xsRefreshCostList[xsRefreshCostList.Count - 1];
            }
            else
            {
                userDiamond = xsRefreshCostList[count];
            }
        }
        UserDiamondSp.GetComponentInChildren<UILabel>().text = userDiamond.ToString();
        refreshCount.text = "(今日已刷新" + count + "次)";
    }
    private void OnArrowRTClick()
    { 
        if (currentPage >= playerData.GetInstance().lotteryInfo.page)
        {
            return;
        }
        currentPage++;
        CreatItemData();
    }
    private void OnArrowLTClick()
    {
        if (currentPage <= 0)
        {
            return;

        } currentPage--;
        CreatItemData();
        
    }
    private void OnNPCClick()
    {
        ClickType();
    }
    private void OnNPC_2Click()
    {
        ClickType();
    }
    private void OnNPC_3Click()
    {
        ClickType();
    }
    private void OnNPC_4Click()
    {
        ClickType();
    }
    private void ClickType()
    {
        if (this._index == 1)
        {
            NpcDialogueInfo(1);
        }
        else if (this._index == 5)
        {
            NpcDialogueInfo(5);
        }
        else if (this._index == 7)
        {
            NpcDialogueInfo(7);
        }
        else if (this._index == 8)
        {
            NpcDialogueInfo(8);
        }
    }
    private void NpcDialogueInfo(int types)
    {
        if (FSDataNodeTable<ShopNode>.GetSingleton().DataNodeList.Count > 0)
        {
            if (types == 1)
            {
                string str1 = FSDataNodeTable<ShopNode>.GetSingleton().DataNodeList[1].npcDialogue;
                string[] strs1 = str1.Split('|');
                int strIndex = 0;
                strIndex = UnityEngine.Random.Range(0, strs1.Length);
                UINpcTips.Instance.SetNPCDialogue(strs1[strIndex].ToString());
            }
            else if (types == 5)
            {
                string str1 = FSDataNodeTable<ShopNode>.GetSingleton().DataNodeList[5].npcDialogue;
                string[] strs1 = str1.Split('|');
                int strIndex = 0;
                strIndex = UnityEngine.Random.Range(0, strs1.Length);
                UINpcTips.Instance.SetNPCDialogue(strs1[strIndex].ToString());
            }
            else if (types == 7)
            {
                string str1 = FSDataNodeTable<ShopNode>.GetSingleton().DataNodeList[7].npcDialogue;
                string[] strs1 = str1.Split('|');
                int strIndex = 0;
                strIndex = UnityEngine.Random.Range(0, strs1.Length);
                UINpcTips.Instance.SetNPCDialogue(strs1[strIndex].ToString());
            }
            else if (types == 8)
            {
                string str1 = FSDataNodeTable<ShopNode>.GetSingleton().DataNodeList[8].npcDialogue;
                string[] strs1 = str1.Split('|');
                int strIndex = 0;
                strIndex = UnityEngine.Random.Range(0, strs1.Length);
                UINpcTips.Instance.SetNPCDialogue(strs1[strIndex].ToString());
            }
        }
    }
    private void UpdataMonery(MoneyType moneryType,long monery)
    {
        switch (moneryType)
        {
            case MoneyType.PVPcoin:
                coinTypeLab.text = monery.ToString();
                break;
            case MoneyType.AreanCoin:
                coinTypeLab.text = monery.ToString();
                break;
            //case MoneyType.PVEcion:
            case MoneyType.RewardCoin:
                coinTypeLab.text = monery.ToString();
                break;

        }
    }

    public override UIPanleID GetUIKey()
    {
        return UIPanleID.none;
    }


    private UIScrollView _view;
    private long maxTimer = 600;
    private long lostTime;
    private float timer = 0;
    public int _index = 1;

}

