/*
文件名（File Name）:   LotteryResultManager.cs

作者（Author）:    #高#

创建时间（CreateTime）:  2016-11-10 17:26:32
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LotteryResultManager
{
    public void LotteryHandler(Dictionary<string, object> data)
    {
        LotteryType type = (LotteryType)int.Parse(data["dt"].ToString());
        int count = int.Parse(data["at"].ToString());
        long money = long.Parse(data["vv"].ToString());
        playerData.GetInstance().lotteryInfo.diamondDrawCounnt = int.Parse(data["dd"].ToString());
        playerData.GetInstance().lotteryInfo.goldDrawCount = int.Parse(data["gd"].ToString());
        playerData.GetInstance().lotteryInfo.diamondTime = long.Parse(data["ddt"].ToString());
        playerData.GetInstance().lotteryInfo.goldTime = long.Parse(data["gdt"].ToString());
        if (type == LotteryType.LotterySoul && count == 10)
        {
            playerData.GetInstance().lotteryInfo.itemList.Clear();
            object[] goodList = data["item"] as object[];
            if (goodList.Length < count) count = goodList.Length;
            for (int i = 0; i < count; i++)
            {
                Dictionary<string, object> goodInfo = goodList[i] as Dictionary<string, object>;
                ItemData itemInfo = new ItemData();
                itemInfo.Id = int.Parse(goodInfo["id"].ToString());
                if (int.Parse(StringUtil.SubString(itemInfo.Id.ToString(), 3)) == 107 || int.Parse(StringUtil.SubString(itemInfo.Id.ToString(), 3)) == 106)
                {
                    itemInfo.UiAtlas = ResourceManager.Instance().GetUIAtlas("UIHeroHead");
                }
                else
                {
                    itemInfo.UiAtlas = ResourceManager.Instance().GetUIAtlas("Prop");
                }
                itemInfo.Count = int.Parse(goodInfo["at"].ToString());
                if (GameLibrary.Instance().ItemStateList.ContainsKey(itemInfo.Id))
                {
                    ItemNodeState vo = GameLibrary.Instance().ItemStateList[itemInfo.Id];
                    itemInfo.Name = vo.name;
                    itemInfo.Describe = vo.describe;
                    itemInfo.IconName = vo.icon_name;
                    switch (vo.grade)
                    {
                        case 1:
                            itemInfo.GradeTYPE = GradeType.Gray;
                            break;
                        case 2:
                            itemInfo.GradeTYPE = GradeType.Green;
                            break;
                        case 4:
                            itemInfo.GradeTYPE = GradeType.Blue;
                            break;
                        case 7:
                            itemInfo.GradeTYPE = GradeType.Purple;
                            break;
                        case 11:
                            itemInfo.GradeTYPE = GradeType.Orange;
                            break;
                        case 16:
                            itemInfo.GradeTYPE = GradeType.Red;
                            break;
                    }
                    if ((int.Parse(StringUtil.SubString(itemInfo.Id.ToString(), 3)) == 106) || (int.Parse(StringUtil.SubString(itemInfo.Id.ToString(), 3)) == 107))
                    {

                        playerData.GetInstance().lotteryInfo.itemList.Add(itemInfo);
                        Singleton<LotteryResultManager>.Instance.LotteryResultSort(playerData.GetInstance().lotteryInfo.itemList);
                    }
                }
            }
            playerData.GetInstance().RoleMoneyHadler(MoneyType.Diamond, money);
        }
        else
        {
            playerData.GetInstance().lotteryInfo.itemList.Clear();
            object[] goodList = data["item"] as object[];
            int cout = goodList.Length;
            if (cout >= 1 && cout < 10) cout = 1;else if (cout >= 10) cout = 10;
            for (int i = 0; i < cout; i++)
            {
                Dictionary<string, object> goodInfo = (Dictionary<string, object>)goodList[i];
                ItemData itemInfo = new ItemData();
                itemInfo.Id = int.Parse(goodInfo["id"].ToString());
                if (int.Parse(StringUtil.SubString(itemInfo.Id.ToString(), 3)) == 107 || int.Parse(StringUtil.SubString(itemInfo.Id.ToString(), 3)) == 106)
                {
                    itemInfo.UiAtlas = ResourceManager.Instance().GetUIAtlas("UIHeroHead");
                }
                else
                {
                    itemInfo.UiAtlas = ResourceManager.Instance().GetUIAtlas("Prop");
                }
                //Debug.Log(itemInfo.Id+ "        itemInfo.Id ");
                itemInfo.Count = int.Parse(goodInfo["at"].ToString());
                ItemNodeState vo = GameLibrary.Instance().ItemStateList[itemInfo.Id];
                itemInfo.Name = vo.name;
                itemInfo.Describe = vo.describe;
                itemInfo.IconName = vo.icon_name;
                switch (vo.grade)
                {
                    case 1:
                        itemInfo.GradeTYPE = GradeType.Gray;
                        break;
                    case 2:
                        itemInfo.GradeTYPE = GradeType.Green;
                        break;
                    case 4:
                        itemInfo.GradeTYPE = GradeType.Blue;
                        break;
                    case 7:
                        itemInfo.GradeTYPE = GradeType.Purple;
                        break;
                    case 11:
                        itemInfo.GradeTYPE = GradeType.Orange;
                        break;
                    case 16:
                        itemInfo.GradeTYPE = GradeType.Red;
                        break;
                }
                playerData.GetInstance().lotteryInfo.itemList.Add(itemInfo);
                Singleton<LotteryResultManager>.Instance.LotteryResultSort(playerData.GetInstance().lotteryInfo.itemList);
            }
            switch (type)
            {
                case LotteryType.GoldLottery:

 
                    playerData.GetInstance().RoleMoneyHadler(MoneyType.Gold, money);
                    break;
                case LotteryType.DiamondLottery:


                    playerData.GetInstance().RoleMoneyHadler(MoneyType.Diamond, money);
                    break;
                case LotteryType.LotterySoul:

                    playerData.GetInstance().RoleMoneyHadler(MoneyType.Diamond, money);
                    break;
            }
        }
    }
    /// <summary>
    /// 对抽奖结果进行随机排序
    /// </summary>
    /// <param name="list"></param>
    public void LotteryResultSort(List<ItemData> list)
    {
        if (list.Count > 1)
        {
            int count = 0;
            if (list.Count % 2 == 0)
            {
                count = list.Count;
            }
            else
            {
                count = list.Count - 1;
            }
            for (int j = 0; j < count / 2; j++)
            {
                int a = Random.Range(0, list.Count);
                int b = Random.Range(0, list.Count);
                if (a != b)
                {
                    ItemData tmp = list[a];
                    list[a] = list[b];
                    list[b] = tmp;
                }
            }
        }
    }
}

