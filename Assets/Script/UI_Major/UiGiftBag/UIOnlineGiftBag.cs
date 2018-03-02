using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Tianyu;

public class UIOnlineGiftBag : GUIBase
{
    private long onLineTime = 280;//在线时长 单位秒
    float timer = 0;
    public GUISingleMultList multList;
    Dictionary<long, OnlineRewardNode> node;
    List<OnlineRewardNode> onlineRewardList = new List<OnlineRewardNode>();

    private static UIOnlineGiftBag instance;

    public static UIOnlineGiftBag Instance { get { return instance; } set { instance = value; } }

    public long OnLineTime
    {
        get
        {
            return onLineTime;
        }

        set
        {
            onLineTime = value;
        }
    }
    public override UIPanleID GetUIKey()
    {
        return UIPanleID.none;
    }

    public UIOnlineGiftBag()
    {
        instance = this;
    }

    protected override void Init()
    {
        multList = transform.Find("GiftbagScrollView/MultList").GetComponent<GUISingleMultList>();
        if (FSDataNodeTable<OnlineRewardNode>.GetSingleton().DataNodeList.Count > 0)
        {
            foreach (OnlineRewardNode node in FSDataNodeTable<OnlineRewardNode>.GetSingleton().DataNodeList.Values)
            {
                onlineRewardList.Add(node);
            }
        }

    }
    protected override void ShowHandler()
    {
        base.ShowHandler();
        //AlreadyGetCount = int.Parse(playerData.GetInstance().singnData.onlineReward.Substring(playerData.GetInstance().singnData.onlineReward.Length -1, 1));
        playerData.GetInstance().singnData.onlineAlreadyGetCount = int.Parse(playerData.GetInstance().singnData.onlineReward.Substring(playerData.GetInstance().singnData.onlineReward.Length - 1, 1));
        multList.InSize(onlineRewardList.Count, 1);
        multList.Info(onlineRewardList.ToArray());
    }
    //void Update()
    //{
    //    OnLineTime = Auxiliary.GetNowTime() - playerData.GetInstance().singnData.logintime + playerData.GetInstance().singnData.onLineTime * 60;
    //}
}
