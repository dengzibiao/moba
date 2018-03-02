using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Tianyu;
enum GoldHandType
{
    One = 0,
    Multiple = 1
}
/// <summary>
/// 点金手
/// </summary>
public class UIGoldHand : GUIBase
{
    public GUISingleLabel useCount;//今日使用次数
    public GUISingleLabel des;//描述
    public GUISingleLabel jewelCountTxt;
    public GUISingleLabel goldCountTxt;
    public GUISingleMultList multList;
    public GUISingleButton backBtn;
    public GUISingleButton oneUseBtn;
    public GUISingleButton moreUseBtn;
    public GameObject maskObj;
    public UIScrollBar scrollBar;
    private GameObject resultShowObj;
    private Transform view;
    public static int goldHandTimes = 0;//今日点金手已使用次数
    public int muiltTimes = 4;//多次点金手数量
    float baseGold = 14950;
    public int lastDataCount = 0;//用于是否显示特效
    public Transform useEffect;
    public Transform towBaojiEffect;
    public Transform fiveBaojiEffect;
    public Transform tenBaojiEffect;
    private static UIGoldHand instance;

    public static UIGoldHand Instance
    {
        get { return instance; }
    }
    public UIGoldHand()
    {
        instance = this;
    }
    public override UIPanleID GetUIKey()
    {
        return UIPanleID.UIGoldHand;
    }
    protected override void Init()
    {
        base.Init();
        resultShowObj = transform.Find("ResultShow").gameObject;
        multList = transform.Find("ResultShow/ScrollView/MultList").GetComponent<GUISingleMultList>();
        view = transform.Find("ResultShow/ScrollView");
        scrollBar = transform.Find("ResultShow/ScrollBar").GetComponent<UIScrollBar>();
        useEffect = transform.Find("EffectPanel/UI_DianJinShou_01");
        towBaojiEffect = transform.Find("EffectPanel/UI_DianJinShou_BJx2");
        fiveBaojiEffect = transform.Find("EffectPanel/UI_DianJinShou_BJx5");
        tenBaojiEffect = transform.Find("EffectPanel/UI_DianJinShou_BJx10");
        useEffect.gameObject.SetActive(false);
        towBaojiEffect.gameObject.SetActive(false);
        fiveBaojiEffect.gameObject.SetActive(false);
        tenBaojiEffect.gameObject.SetActive(false);
        maskObj = transform.Find("Mask").gameObject;
        backBtn = transform.Find("BackBtn").GetComponent<GUISingleButton>();
        oneUseBtn = transform.Find("OneUseBtn").GetComponent<GUISingleButton>();
        moreUseBtn = transform.Find("MoreUseBtn").GetComponent<GUISingleButton>();
        des = transform.Find("Des").GetComponent<GUISingleLabel>();
        useCount = transform.Find("UseCount").GetComponent<GUISingleLabel>();
        jewelCountTxt = transform.Find("JewelCountTxt").GetComponent<GUISingleLabel>();
        goldCountTxt = transform.Find("GoldCountTxt").GetComponent<GUISingleLabel>();
        backBtn.onClick = OnCancleClick;
        oneUseBtn.onClick = OnOneUseBtnClick;
        moreUseBtn.onClick = OnMoreUseBtnClick;
        UIEventListener.Get(maskObj).onClick += OnCloseClick;
        //multList.ScrollView = view;
        //goldHandTimes = playerData.GetInstance().goldHand.maxcount - playerData.GetInstance().goldHand.curcount;//连接服务器后 这个需要注销
    }

    protected override void OnLoadData()
    {
        Singleton<Notification>.Instance.RegistMessageID(MessageID.common_use_lucky_draw_ret, this.GetUIKey());
        base.OnLoadData();
        Show();
    }

    public override void ReceiveData(uint messageID)
    {
        base.ReceiveData(messageID);
        switch (messageID)
        {
            case MessageID.common_use_lucky_draw_ret:
                RefreshInfo();//点金手成功后刷新界面
                useEffect.gameObject.SetActive(false);
                useEffect.gameObject.SetActive(true);
                break;
        }
    }

    //"[ff0000]" + "20" +"[-]" 
    protected override void ShowHandler()
    {
        base.ShowHandler();
        des.text = "[4484c4]"+ "用少量钻石换取大量金币" + "[-]";
        //des.text = "用少量" + "[4484c4]" + "钻石" + "[-]" + "换取大量" + "[b55f10]" + "金币" + "[-]";
        //jewelCountTxt.text = "[4484c4]"+ 40 +"[-]";
        //goldCountTxt.text = "[b55f10]"+ 10000 + "[-]";
        playerData.GetInstance().goldHand.goldHandList.Clear();//每次打开点金手界面清除一下itemlist信息
        lastDataCount = 0;
        RefreshInfo();
        multList.gameObject.SetActive(true);
        resultShowObj.SetActive(false);
    }
    private void OnOneUseBtnClick()
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", 1); // (0:一次 1:连续4次)
        Singleton<Notification>.Instance.Send(MessageID.common_use_lucky_draw_req, newpacket, C2SMessageType.ActiveWait);
        //ClientSendDataMgr.GetSingle().GetGoldHandSend().UseGoldHand(1,C2SMessageType.PASVWait);  
    }
    private void OnMoreUseBtnClick()
    {
        if ((playerData.GetInstance().goldHand.maxcount - playerData.GetInstance().goldHand.alreadyUseCount) < 4)
        {
            //UIPromptBox.Instance.ShowLabel("可使用次数不足4次");
            Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, "可使用次数不足4次");
        }
        else
        {
            Dictionary<string, object> newpacket = new Dictionary<string, object>();
            newpacket.Add("arg1", 4); // (0:一次 1:连续4次)
            Singleton<Notification>.Instance.Send(MessageID.common_use_lucky_draw_req, newpacket, C2SMessageType.ActiveWait);
            //ClientSendDataMgr.GetSingle().GetGoldHandSend().UseGoldHand(4, C2SMessageType.PASVWait);
        }
        
    }
    /// <summary>
    /// 更新展示下次购买的信息
    /// </summary>
    public void RefreshInfo()
    {
        if (!resultShowObj.activeSelf)
        {
            resultShowObj.SetActive(true);
            multList.gameObject.SetActive(true);
        }
        //goldHandTimes = playerData.GetInstance().goldHand.maxcount - playerData.GetInstance().goldHand.curcount;
        //useCount.text = "(" + playerData.GetInstance().goldHand.curcount + "/" + playerData.GetInstance().goldHand.maxcount + ")";
        useCount.text = "(" + (playerData.GetInstance().goldHand.maxcount- playerData.GetInstance().goldHand.alreadyUseCount) + "/" + playerData.GetInstance().goldHand.maxcount + ")";
        int consumeJewel = 0;
        float getGold = 0;
        int level = playerData.GetInstance().selfData.level;
        int id = playerData.GetInstance().goldHand.id;
        int time = playerData.GetInstance().goldHand.time;
        if (FSDataNodeTable<GoldHandNode>.GetSingleton().DataNodeList.ContainsKey(id))
        {
            if (playerData.GetInstance().goldHand.time + 1 > FSDataNodeTable<GoldHandNode>.GetSingleton().DataNodeList[id].time)
            {
                id = id + 1;
                time = 1;
            }
            else
            {
                time = time + 1;
            }
            if (FSDataNodeTable<GoldHandNode>.GetSingleton().DataNodeList.ContainsKey(id))
            {
                consumeJewel = FSDataNodeTable<GoldHandNode>.GetSingleton().DataNodeList[id].diamondCost;
                getGold = (baseGold + level * 50) *( FSDataNodeTable<GoldHandNode>.GetSingleton().DataNodeList[id].basicRate + time *FSDataNodeTable<GoldHandNode>.GetSingleton().DataNodeList[id].commonDifference);
            } 
        }
        jewelCountTxt.text = consumeJewel+"";
        goldCountTxt.text = Math.Floor(getGold) + "";
        multList.InSize(playerData.GetInstance().goldHand.goldHandList.ToArray().Length,1);
        multList.Info(playerData.GetInstance().goldHand.goldHandList.ToArray());
        //multList.InSize(objs.Length, 1);
        StartCoroutine(AutoScrollview());
    }

    IEnumerator AutoScrollview()
    {
        if (playerData.GetInstance().goldHand.goldHandList.Count < 3)
        {
            scrollBar.value = 0;
        }
        yield return new WaitForSeconds(0.2f);
        if (playerData.GetInstance().goldHand.goldHandList.Count >= 3)
        {
            scrollBar.value = Mathf.Lerp(0, 1, 3);
        }

        yield return 0;
    }


    private void OnCloseClick(GameObject go)
    {
        //view.GetComponent<UIScrollView>().ResetPosition();
        useEffect.gameObject.SetActive(false);
        towBaojiEffect.gameObject.SetActive(false);
        fiveBaojiEffect.gameObject.SetActive(false);
        tenBaojiEffect.gameObject.SetActive(false);
        Control.HideGUI(this.GetUIKey());
    }
    private void OnCancleClick()
    {
        //view.GetComponent<UIScrollView>().ResetPosition();
        useEffect.gameObject.SetActive(false);
        towBaojiEffect.gameObject.SetActive(false);
        fiveBaojiEffect.gameObject.SetActive(false);
        tenBaojiEffect.gameObject.SetActive(false);
        Control.HideGUI(this.GetUIKey());
    }

    protected override void RegisterComponent()
    {
        base.RegisterComponent();
        RegisterComponentID(22, 69, oneUseBtn.gameObject);

    }
    protected override void RegisterIsOver()
    {
        base.RegisterIsOver();
    }
}


