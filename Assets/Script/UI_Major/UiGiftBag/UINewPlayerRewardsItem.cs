/*
文件名（File Name）:   UINewPlayerRewardsItem.cs

作者（Author）:    #高#

创建时间（CreateTime）:  #CreateTime#
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tianyu;

public class UINewPlayerRewardsItem : GUISingleItemList
{

    public GUISingleLabel day;
    public GUISingleButton canGetBtn;
    public GUISingleMultList itemMultList;
    public GUISingleSprite alreadyGet;
    public GUISingleLabel label;
    private ItemData vo;
    private long heroId = 0;//奖励物品是英雄但此处是物品ID需要转换
    private object[] goodsList;//奖励物品[id,数量]
    private List<ItemData> dataList = new List<ItemData>();//存储数据列表
    private List<ItemData> dtList = new List<ItemData>();//剔除金币和钻石

    private int mStar;

    protected override void InitItem()
    {
        canGetBtn.onClick = CanGetBtnOnClick;
    }

    private void CanGetBtnOnClick()
    {
        ClientSendDataMgr.GetSingle().GetNewplayerReward().NewplayerRewardSend();
        UIWelfare._instance.TemporaryData(dtList);
        TaskManager.Single().itemlist = dataList;
        if (heroId != 0)
        {
            if ((int.Parse(StringUtil.SubString(heroId.ToString(), 3)) == 201))
            {
                if (playerData.GetInstance().GetHeroDataByID(heroId) == null)
                {
                    UIWelfare._instance.HeroID(heroId);
                }             
            }
        }
    }

    public override void Info(object obj)
    {
        if (obj == null)
        {

        }
        else
        {
            vo = (ItemData)obj;
            day.text = "第" + vo.Id + "天";
            long a = long.Parse(vo.GameTime.ToString().Substring(0, 6));//今天日期
            long b = long.Parse(vo.TimeSign.ToString().Substring(0, 6));//标记日期
            int c = int.Parse(TimeManager.Instance.ConvertIntDateTime(Auxiliary.GetNowTime()).ToString("HH"));
            int count = int.Parse(vo.TimeSign.ToString().Substring(6, 2));//次数

            if (a == b)//判断是今天
            {
                if (count == 0 && index == 0&& c>5)//如果是今天注册的默认打开
                {
                    canGetBtn.gameObject.SetActive(true);
                    alreadyGet.gameObject.SetActive(false);
                    label.text = "";
                }
                else if (count > index) //今天之前包括今天已领
                {
                    canGetBtn.gameObject.SetActive(false);
                    alreadyGet.gameObject.SetActive(true);
                    label.text = "";
                }
                else
                {
                    canGetBtn.gameObject.SetActive(false);
                    alreadyGet.gameObject.SetActive(false);
                    label.text = "时间未达到";
                }
            }
            else if (a > b)
            {
                if (count > index) //标记日期小次数的index全部已领
                {
                    canGetBtn.gameObject.SetActive(false);
                    alreadyGet.gameObject.SetActive(true);
                    label.text = "";
                }
                else if (count == index&& c > 5)//可领
                {
                    canGetBtn.gameObject.SetActive(true);
                    alreadyGet.gameObject.SetActive(false);
                    label.text = "";
                }
                else
                {
                    canGetBtn.gameObject.SetActive(false);
                    alreadyGet.gameObject.SetActive(false);
                    label.text = "时间未达到";
                }
            }
            for (int i = 0; i < vo.goodsItem.Length / 2; i++)
            {
                ItemData tempData = new ItemData();
                tempData.GoodsType = MailGoodsType.ItemType;

                tempData.Id = vo.goodsItem[i, 0];
                tempData.Count = vo.goodsItem[i, 1];
                if (GameLibrary.Instance().ItemStateList.ContainsKey(tempData.Id))
                {
                    switch (GameLibrary.Instance().ItemStateList[tempData.Id].grade)
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
                    if (tempData.Types == 7)
                    {
                        heroId = int.Parse(201 + StringUtil.SubString(tempData.Id.ToString(), 6, 3));
                        if (FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList.ContainsKey(heroId))
                        {
                            tempData.Star = FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList[heroId].init_star;

                        }
                    }
                    if (tempData.Types == 6 || tempData.Types == 7)
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
            if (vo.Gold > 0)
            {
                ItemData tempData = new ItemData();
                tempData.GoodsType = MailGoodsType.GoldType;
                tempData.GradeTYPE = GradeType.Purple;
                tempData.Gold = vo.Gold;
                tempData.Name = "金币";
                tempData.Describe = "";
                tempData.Count = vo.Gold;
                tempData.IconName = "jinbi";
                tempData.UiAtlas = ResourceManager.Instance().GetUIAtlas("Prop");
                dataList.Add(tempData);
            }
            if (vo.Diamond > 0)
            {
                ItemData tempData = new ItemData();
                tempData.GoodsType = MailGoodsType.DiamomdType;
                tempData.GradeTYPE = GradeType.Orange;
                tempData.Diamond = vo.Diamond;
                tempData.Name = "钻石";
                tempData.Describe = "";
                tempData.IconName = "zuanshi";
                tempData.Count = vo.Diamond;
                tempData.UiAtlas = ResourceManager.Instance().GetUIAtlas("Prop");
                dataList.Add(tempData);
            }
 
            itemMultList.InSize(dataList.Count, dataList.Count);
            itemMultList.Info(dataList.ToArray());
        }
    }
}
