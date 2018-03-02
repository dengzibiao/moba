using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Tianyu;

public class UIExpPropPanel : GUIBase
{
    public GUISingleMultList multList;
    public GUISingleButton closeBtn;
    public UIScrollView scrollView;
    public GUISingleLabel expPoolValue;
    public GUISingleLabel useDes;
    public GUISingleLabel des;
    public GameObject backObj;
    public static UIExpPropPanel instance;
    public long beforeExpCount = 0;
    public List<ItemData> expItemList = new List<ItemData>();
    public int selectIndex = -1;
    public UIExpPropPanel()
    {
        instance = this;
    }

    public override UIPanleID GetUIKey()
    {
        return UIPanleID.UIExpPropPanel;
    }

    protected override void Init()
    {
        backObj = transform.Find("Back").gameObject;
        scrollView = transform.Find("ScrollView").GetComponent<UIScrollView>();
        multList = transform.Find("ScrollView/MultList").GetComponent<GUISingleMultList>();
        UIEventListener.Get(backObj).onClick += OnCloseClick;
        closeBtn.onClick = OnBackClick;
    }
    protected override void OnLoadData()
    {
        base.OnLoadData();
        Singleton<Notification>.Instance.RegistMessageID(MessageID.common_use_item_ret, UIPanleID.UIExpPropPanel);
        Singleton<Notification>.Instance.RegistMessageID(MessageID.common_update_item_list_ret, UIPanleID.UIExpPropPanel);
        Show();
    }
    public override void ReceiveData(UInt32 messageID)
    {
       
        switch (messageID)
        {
            case MessageID.common_use_item_ret:
            //case MessageID.common_update_item_list_ret:
                RefreshData();
                ShowPowerChange(beforeExpCount, playerData.GetInstance().selfData.expPool);
                break;
        }
        base.ReceiveData(messageID);
    }
    private void OnCloseClick(GameObject go)
    {
        Control.HideGUI(this.GetUIKey());
        //Hide();
        selectIndex = -1;
    }
    private void OnBackClick()
    {
        Control.HideGUI(this.GetUIKey());
        //Hide();
        selectIndex = -1;
    }
    public void RefreshData()
    {
        expPoolValue.text = playerData.GetInstance().selfData.expPool + "";
        expItemList = GoodsDataOperation.GetInstance().GetItemListByItmeType(ItemType.ExpProp);
        if (expItemList != null && expItemList.Count > 0)
        {
            scrollView.gameObject.SetActive(true);
            des.gameObject.SetActive(false);
            useDes.gameObject.SetActive(true);
            multList.InSize(expItemList.Count, expItemList.Count);
            multList.Info(expItemList.ToArray());
        }
        else if(expItemList != null)
        {
            scrollView.gameObject.SetActive(false);
            des.gameObject.SetActive(true);
            useDes.gameObject.SetActive(false);
        }
        UIUpGradeStar.instance.RefreshSoulStone(UI_HeroDetail.hd);
    }
    protected override void ShowHandler()
    {
        RefreshData();
    }

    private double startValue = 0;
    private long endValue = 0;
    private bool isStart = false;
    private bool isUp = true;
    public int speed = 1000;
    void Update()
    {
        if (isStart)
        {
            if (isUp)
            {
                startValue += speed * Time.deltaTime;
                if (startValue > endValue)
                {
                    isStart = false;
                    startValue = endValue;
                }
            }
            else
            {
                startValue -= speed * Time.deltaTime;
                if (startValue < endValue)
                {
                    isStart = false;
                    startValue = endValue;
                }
            }
            expPoolValue.text = (int)startValue + "";

        }
    }
    /// <summary>
    /// 数字变化
    /// </summary>
    /// <param name="startValue"></param>
    /// <param name="endValue"></param>
    public void ShowPowerChange(long startValue, long endValue)
    {
        this.startValue = startValue;
        this.endValue = endValue;
        if (endValue > startValue)
            isUp = true;
        else
            isUp = false;
        isStart = true;
    }

}
