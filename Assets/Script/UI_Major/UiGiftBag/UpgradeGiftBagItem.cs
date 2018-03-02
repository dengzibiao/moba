using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Tianyu;

public class UpgradeGiftBagItem : GUISingleItemList
{
    public GUISingleButton canGetBtn;
    public GUISingleMultList multList;
    public UILabel getCondition;
    public Transform alreadyGet;
    public Transform dontGet;
    private LevelRewardNode node;
    private List<ItemData> dataList = new List<ItemData>();
    private List<ItemData> dtList = new List<ItemData>();//剔除金币和钻石
    protected override void InitItem()
    {
        canGetBtn = transform.Find("CanGetBtn").GetComponent<GUISingleButton>();
        getCondition = transform.Find("GetCondition").GetComponent<UILabel>();
        multList = transform.Find("MultList").GetComponent<GUISingleMultList>();
        alreadyGet = transform.Find("AlreadyGet");
        dontGet = transform.Find("DontGet");
        canGetBtn.onClick = OnGetLevelGiftBagClick;
    }

    private void OnGetLevelGiftBagClick()
    {
        Debug.Log("领取等级礼包奖励");
        UIWelfare._instance.TemporaryData(dtList);
        ClientSendDataMgr.GetSingle().GetEnergySend().SendGetLevelReward(C2SMessageType.ActiveWait,node.id);
        TaskManager.Single().itemlist = dataList;
    }

    public override void Info(object obj)
    {
        node = (LevelRewardNode)obj;

        //需求等级 <= 玩家当前等级  显示领取按钮
        if (node.need_lv <= playerData.GetInstance().selfData.level)
        {
            alreadyGet.gameObject.SetActive(false);
            dontGet.gameObject.SetActive(false);
            canGetBtn.gameObject.SetActive(true);
        }
        //如果玩家已经领取 显示已经领取标识
        if (playerData.GetInstance().singnData.alreadylevelRewardDic.ContainsKey(node.id))
        {
            dontGet.gameObject.SetActive(false);
            alreadyGet.gameObject.SetActive(true);
            canGetBtn.gameObject.SetActive(false);
        }
        //需求等级 > 玩家当前等级 显示不能领取按钮
        if (node.need_lv > playerData.GetInstance().selfData.level)
        {
            canGetBtn.gameObject.SetActive(false);
            alreadyGet.gameObject.SetActive(false);
            dontGet.gameObject.SetActive(true);
        }

        getCondition.text = node.need_lv + "级";

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
        if (node.diamond>0)
        {
            ItemData tempData = new ItemData();
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
        if (node.power > 0)
        {
            ItemData tempData = new ItemData();
            tempData.GoodsType = MailGoodsType.PowerType;
            tempData.GradeTYPE = GradeType.Blue;
            tempData.Power = node.power;
            tempData.Name = "体力";
            tempData.IconName = "tili";
            tempData.Count = node.power;
            tempData.UiAtlas = ResourceManager.Instance().GetUIAtlas("Prop");
            dataList.Add(tempData);
        }
        for (int i =0;i< node.goodsItem.Length/2;i++)
        {
            ItemData tempData = new ItemData();
            tempData.GoodsType = MailGoodsType.ItemType;

            tempData.Id = (int)node.goodsItem[i, 0];
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
                if (tempData.Types == 6|| tempData.Types==7)
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
    }
}

/// <summary>
/// 等级礼包中的物品数据 包含金币 钻石 体力 道具
/// </summary>
public class LevelGiftBagData
{
    public MailGoodsType _type;//附件类型
    public GradeType _gradeType;
    public long id;//道具id
    public int count;//道具数量
    public int gold;//金币数量
    public int diamond;//钻石数量
    public int power;//体力数量
}
