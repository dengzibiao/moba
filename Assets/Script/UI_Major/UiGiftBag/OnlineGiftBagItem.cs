using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Tianyu;

public class OnlineGiftBagItem : GUISingleItemList
{
    public GUISingleButton canGetBtn;
    public GUISingleMultList multList;
    public UILabel getCondition;
    public Transform alreadyGet;
    public Transform dontGet;
    public UILabel countDownTime;
    private OnlineRewardNode node;
    private List<ItemData> dataList = new List<ItemData>();
    private List<ItemData> dtList = new List<ItemData>();//剔除金币和钻石
    protected override void InitItem()
    {
        canGetBtn = transform.Find("CanGetBtn").GetComponent<GUISingleButton>();
        getCondition = transform.Find("GetCondition").GetComponent<UILabel>();
         multList = transform.Find("MultList").GetComponent<GUISingleMultList>();
        countDownTime = transform.Find("CountDownTime").GetComponent<UILabel>();
        //multList = transform.Find("MultList").GetComponent<GUISingleMultList>();
        alreadyGet = transform.Find("AlreadyGet");
        dontGet = transform.Find("DontGet");
        canGetBtn.onClick = OnGetOnlineGiftBagClick;
    }

    private void OnGetOnlineGiftBagClick()
    {
        Debug.Log("领取在线礼包奖励");
        UIWelfare._instance.TemporaryData(dtList);
        ClientSendDataMgr.GetSingle().GetEnergySend().SendGetOnlineReward(C2SMessageType.ActiveWait);
        TaskManager.Single().itemlist = dataList;
        //playerData.GetInstance().singnData.getRewardTime = Auxiliary.GetNowTime();
        //playerData.GetInstance().singnData.onlineReward = "1608120001";
        //playerData.GetInstance().singnData.onLineTime = int.Parse(playerData.GetInstance().singnData.onlineReward.Substring(6, 3));
        //Instance.AlreadyGetCount = int.Parse(playerData.GetInstance().singnData.onlineReward.Substring(9, 1));
        //UIOnlineGiftBag.Instance.IsRefresh = true;
        //Debug.Log(UIOnlineGiftBag.Instance.AlreadyGetCount + "______累计时长"+ playerData.GetInstance().singnData.onLineTime);
        //Debug.Log(playerData.GetInstance().singnData.getRewardTime + "_______登陆时间"+ playerData.GetInstance().singnData.logintime);
    }

    public override void Info(object obj)
    {
        node = (OnlineRewardNode)obj;

        getCondition.text = node.online_time + "分钟";

        if (node.gold > 0)
        {
            ItemData tempData = new ItemData();
            tempData.GoodsType = MailGoodsType.GoldType;
            tempData.GradeTYPE = GradeType.Purple;
            tempData.Gold = node.gold;
            tempData.Name = "金币";
            tempData.Describe = "";
            tempData.Count = node.gold;
            tempData.IconName = "jinbi";
            tempData.UiAtlas = ResourceManager.Instance().GetUIAtlas("Prop");
            dataList.Add(tempData);
        }
        if (node.diamond > 0)
        {
            ItemData  tempData = new ItemData();
            tempData.GoodsType = MailGoodsType.DiamomdType;
            tempData.GradeTYPE = GradeType.Orange;
            tempData.Diamond = node.diamond;
            tempData.Name = "钻石";
            tempData.Describe = "";
            tempData.IconName = "zuanshi";
            tempData.Count = node.diamond;
            tempData.UiAtlas = ResourceManager.Instance().GetUIAtlas("Prop");
            dataList.Add(tempData);
        }
        for (int i = 0; i < node.goodsItem.Length / 2; i++)
        {
            ItemData tempData = new ItemData();
            tempData.GoodsType = MailGoodsType.ItemType;

            tempData.Id = (int)( node.goodsItem[i, 0]);
            tempData.Count = (int)node.goodsItem[i, 1];
            if (GameLibrary.Instance().ItemStateList.ContainsKey(tempData.Id))
            {
                switch (GameLibrary.Instance().ItemStateList[tempData.Id].grade) //(VOManager.Instance().GetCSV<ItemCSV>("Item").GetVO(accessoryData.Id).grade)
                {
                    case 1:
                        tempData.GradeTYPE = GradeType.Gray;
                        break;
                    case 2:
                        tempData.GradeTYPE = GradeType.Green;
                        break;
                    case 4:
                        tempData.GradeTYPE = GradeType.Blue;
                        break;
                    case 7:
                        tempData.GradeTYPE = GradeType.Purple;
                        break;
                    case 11:
                        tempData.GradeTYPE = GradeType.Orange;
                        break;
                    case 16:
                        tempData.GradeTYPE = GradeType.Red;
                        break;
                }
                tempData.Name = GameLibrary.Instance().ItemStateList[tempData.Id].name;
                tempData.Describe = GameLibrary.Instance().ItemStateList[tempData.Id].describe;
                tempData.Types = GameLibrary.Instance().ItemStateList[tempData.Id].types;
                tempData.IconName = GameLibrary.Instance().ItemStateList[tempData.Id].icon_name;
                if (tempData.Types == 6)
                {
                    tempData.UiAtlas = ResourceManager.Instance().GetUIAtlas("UIHeroHead");
                }
                else
                {
                    tempData.UiAtlas = ResourceManager.Instance().GetUIAtlas("Prop");
                }
            }
            dtList.Add(tempData);
            dataList.Add(tempData);
        }

        multList.InSize(dataList.Count, dataList.Count);
        multList.Info(dataList.ToArray());


        //先判断是被领取过，如果领取过直接显示已领取过 TODO

        SetItemState();


    }
    //延迟1秒 保障所有item状态都刷新 之后关闭刷新状态
    IEnumerator SetRefreshState()
    {
        yield return new WaitForSeconds(1f);
        //UIOnlineGiftBag.Instance.IsRefresh = false;
        playerData.GetInstance().singnData.onlineIsRefresh = false;
        yield return 0;
    }
    void Update()
    {
        if (playerData.GetInstance().singnData.onlineIsRefresh)
        {
            //刷新的时候 也要重新判断一下 是否领取过
            SetItemState();//刷新全部item的状态
            StartCoroutine(SetRefreshState());
        }
        if (countDownTime.gameObject.activeSelf)
        {
            countDownTime.text = TimeManager.Instance.GetTimeClockText((playerData.GetInstance().singnData.onLineRewardTime));
            if ((playerData.GetInstance().singnData.onLineRewardTime) <=0|| countDownTime.text == "00:00:00")
            {
                playerData.GetInstance().singnData.isCanGetOnlineReward = true;
                UIWelfare._instance.ShowRedTag();
                SetItemState();//只刷新本身item的状态
                //UIOnlineGiftBag.Instance.IsRefresh = true;
                playerData.GetInstance().singnData.onlineIsRefresh = true;
            }
        }
    }

    /// <summary>
    /// 设置item的状态  倒计时 可领取 不可领取状态并且不展示倒计时
    /// </summary>
    void SetItemState()
    {
        canGetBtn.gameObject.SetActive(false);
        alreadyGet.gameObject.SetActive(false);
        dontGet.gameObject.SetActive(false);
        countDownTime.transform.gameObject.SetActive(false);


        //如果序列小于已经领取的次数 标识已经领取过
        if (index < playerData.GetInstance().singnData.onlineAlreadyGetCount)
        {
            alreadyGet.gameObject.SetActive(true);
        }
        //如果序列大于已经领取的次数 标识还不能领取
        if (index > playerData.GetInstance().singnData.onlineAlreadyGetCount)
        {
            dontGet.gameObject.SetActive(true);
        }
        if (index == playerData.GetInstance().singnData.onlineAlreadyGetCount)
        {
            if ((playerData.GetInstance().singnData.onLineRewardTime) > 0)
            {
                //UIOnlineGiftBag.Instance.residueTime = 5 * 60 - UIOnlineGiftBag.Instance.onLineTime;
                countDownTime.gameObject.SetActive(true);
            }
            else if ((playerData.GetInstance().singnData.onLineRewardTime) <= 0)
            {
                canGetBtn.gameObject.SetActive(true);
            }
        }
        //switch (node.online_time)
        //{
        //    case 5:
        //        //如果序列等于已经领取的次数 标识目前正在倒计时 或者 已经可以领取
        //        if (index == UIOnlineGiftBag.Instance.AlreadyGetCount)
        //        {
        //            if ((playerData.GetInstance().singnData.onLineRewardTime) > 0)
        //            {
        //                //UIOnlineGiftBag.Instance.residueTime = 5 * 60 - UIOnlineGiftBag.Instance.onLineTime;
        //                countDownTime.gameObject.SetActive(true);
        //            }
        //            else if ((playerData.GetInstance().singnData.onLineRewardTime) <= 0)
        //            {
        //                canGetBtn.gameObject.SetActive(true);
        //            }
        //        }
        //        //  在线时长 < 5分钟 倒计时 [0,5)
        //        //  5分钟=<在线时长 可领取 [5,++)
        //        // 不可领取状态并且不展示倒计时

        //        break;
        //    case 15:
        //        if (index == UIOnlineGiftBag.Instance.AlreadyGetCount)
        //        {
        //            if ((playerData.GetInstance().singnData.onLineRewardTime) > 0)
        //            {
        //                //UIOnlineGiftBag.Instance.residueTime = 10 * 60 - UIOnlineGiftBag.Instance.onLineTime;
        //                countDownTime.gameObject.SetActive(true);
        //            }
        //            else if ((playerData.GetInstance().singnData.onLineRewardTime) <= 0)
        //            {
        //                canGetBtn.gameObject.SetActive(true);
        //            }
        //        }

        //        //   在线时长 < 15分钟 显示倒计时  [0,15)
        //        //   15分钟 =< 在线时长          可领取     [15,++)
        //        //   不可领取状态并且不展示倒计时


        //        break;
        //    case 30:
        //        if (index == UIOnlineGiftBag.Instance.AlreadyGetCount)
        //        {
        //            if ((playerData.GetInstance().singnData.onLineRewardTime) > 0)
        //            {
        //                //UIOnlineGiftBag.Instance.residueTime = 30 * 60 - UIOnlineGiftBag.Instance.onLineTime;
        //                countDownTime.gameObject.SetActive(true);
        //            }
        //            else if ((playerData.GetInstance().singnData.onLineRewardTime) <= 0)
        //            {
        //                canGetBtn.gameObject.SetActive(true);
        //            }
        //        }
        //        //    在线时长 < 30分钟 显示倒计时  [0,30)
        //        //   30分钟 =< 在线时长          可领取      [30,++)
        //        //   不可领取状态并且不展示倒计时

        //        break;
        //    case 60:
        //        if (index == UIOnlineGiftBag.Instance.AlreadyGetCount)
        //        {
        //            if ((playerData.GetInstance().singnData.onLineRewardTime) >0)
        //            {
        //                //UIOnlineGiftBag.Instance.residueTime = 60 * 60 - UIOnlineGiftBag.Instance.onLineTime;
        //                countDownTime.gameObject.SetActive(true);
        //            }
        //            else if ((playerData.GetInstance().singnData.onLineRewardTime) <= 0)
        //            {
        //                canGetBtn.gameObject.SetActive(true);
        //            }
        //        }
        //        //   在线时长 < 60分钟 显示倒计时  [0,60)
        //        //   60分钟 =< 在线时长          可领取      [60,++)
        //        //   不可领取状态并且不展示倒计时

        //        break;
        //    case 90:
        //        if (index == UIOnlineGiftBag.Instance.AlreadyGetCount)
        //        {
        //            if ((playerData.GetInstance().singnData.onLineRewardTime) >0)
        //            {
        //                //UIOnlineGiftBag.Instance.residueTime = 60 * 60 - UIOnlineGiftBag.Instance.onLineTime;
        //                countDownTime.gameObject.SetActive(true);
        //            }
        //            else if ((playerData.GetInstance().singnData.onLineRewardTime) <= 0)
        //            {
        //                canGetBtn.gameObject.SetActive(true);
        //            }
        //        }
        //        //   在线时长 < 90分钟 显示倒计时  [0,90)
        //        //   90分钟 =< 在线时长          可领取      [90,++)
        //        //   不可领取状态并且不展示倒计时
        //        break;
        //}
    }
}

/// <summary>
/// 在线礼包中的物品数据 包含金币 钻石 体力 道具
/// </summary>
public class OnlineGiftBagData
{
    public MailGoodsType _type;//附件类型
    public GradeType _gradeType;
    public long id;//道具id
    public int count;//道具数量
    public int gold;//金币数量
    public int diamond;//钻石数量
}
