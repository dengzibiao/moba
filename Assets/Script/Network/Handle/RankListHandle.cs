using System;
using UnityEngine;
using System.Collections.Generic;
using Tianyu;
public class CRankListHandle : CHandleBase
{

    public CRankListHandle(CHandleMgr mgr)
        : base(mgr)
    {
    }

    public override void RegistAllHandle()
    {
        RegistHandle(MessageID.common_ranklist_ret, RankListHandle);
    }
    /// <summary>
    /// 获取排行榜信息
    /// </summary>
    /// <param name="packet"></param>
    /// <returns></returns>
    public bool RankListHandle(CReadPacket packet)
    {
        Debug.Log("ranklist返回");
        Dictionary<string, object> data = packet.data;

        int resolt = int.Parse(data["ret"].ToString());
        if (resolt == 0)
        {
            
            int currentValue = 0;
            int currentRank = 0;
            int yesterdayRank = 0;
            currentRank = int.Parse(data["myRank"].ToString());//自己的当前排名
            playerData.GetInstance().playerRankData.rankListType = (RankListType)data["typeid"];
            if (data["rInfo"] != null)
            {      
                object[][] rankInfoList = data["rInfo"] as object[][];
                switch ((RankListType)data["typeid"])
                {
                    //case RankListType.Fight:
                    //    playerData.GetInstance().playerRankData.fightList.Clear();
                    //    break;
                    //case RankListType.StarSum:
                    //    playerData.GetInstance().playerRankData.starSumList.Clear();
                    //    break;
                    //case RankListType.DiamondUser:
                    //    playerData.GetInstance().playerRankData.diamondUserList.Clear();
                    //    break;
                    //case RankListType.BestFourPersons:
                    //    playerData.GetInstance().playerRankData.bestFourPersonsList.Clear();
                    //    break;
                    //case RankListType.Fortune:
                    //    playerData.GetInstance().playerRankData.fortuneList.Clear();
                    //    break;
                    //case RankListType.YesterdayRank:
                    //    playerData.GetInstance().playerRankData.yesterdayList.Clear();
                    //    break;
                    case RankListType.Level:
                        if (playerData.GetInstance().playerRankData.levelList.Count >= 20)
                        playerData.GetInstance().playerRankData.levelList.Clear();
                        break;
                    case RankListType.RealTimeRank:
                        if (playerData.GetInstance().playerRankData.realTimeRankList.Count>=20)
                        playerData.GetInstance().playerRankData.realTimeRankList.Clear();
                        break;
                    default:
                        break;
                }
                for (int i = 0; i < rankInfoList.Length; i++)
                {

                    RankListData rankListData = new RankListData();
                    if (int.Parse(rankInfoList[i][1].ToString()) != 0)
                    {
                        rankListData.currentRank = int.Parse(rankInfoList[i][0].ToString());//当前排名
                        rankListData.playerId = int.Parse(rankInfoList[i][1].ToString());
                        rankListData.name = rankInfoList[i][2].ToString();
                        rankListData.iconId = long.Parse(rankInfoList[i][3].ToString());
                        rankListData.iconBoxId = long.Parse(rankInfoList[i][4].ToString());
                        rankListData.rankValue = int.Parse(rankInfoList[i][5].ToString());//战斗力
                        rankListData.playerLv = int.Parse(rankInfoList[i][6].ToString());//角色等级
                    }
                  
                    if (!string.IsNullOrEmpty(rankInfoList[i][7].ToString()))
                    {
                        string arenas = rankInfoList[i][7].ToString();
                      object  []aa=  Jsontext.ReadeData(arenas)as object[];
                       for(int j=0;j<aa.Length;j++)
                       {
                           string[] str = aa[j] as string[];
                           RankHeroList heroList = new RankHeroList();
                          heroList.iconId1= long.Parse(str[0]);
                          heroList.lv = int.Parse(str[1]);
                          heroList.star = int.Parse(str[2]);
                          heroList.iconIdFrame = int.Parse(str[3]);
                          rankListData.arenaHeroList.Add(heroList);
                       }
                    }
                    switch ((RankListType)data["typeid"])
                    {
                        //case RankListType.Fight:
                        //    playerData.GetInstance().playerRankData.fightList.Add(rankListData);
                        //    playerData.GetInstance().playerRankData.fight = currentValue;
                        //    playerData.GetInstance().playerRankData.fightCurrentRank = currentRank;
                        //    playerData.GetInstance().playerRankData.fightYesterdayRank = yesterdayRank;
                        //    break;
                        //case RankListType.StarSum:
                        //    playerData.GetInstance().playerRankData.starSumList.Add(rankListData);
                        //    playerData.GetInstance().playerRankData.startSum = currentValue;
                        //    playerData.GetInstance().playerRankData.starttSumCurrentRank = currentRank;
                        //    playerData.GetInstance().playerRankData.starttSumYesterday = yesterdayRank;
                        //    break;
                        //case RankListType.DiamondUser:
                        //    playerData.GetInstance().playerRankData.diamondUserList.Add(rankListData);
                        //    break;
                        //case RankListType.BestFourPersons:
                        //    playerData.GetInstance().playerRankData.bestFourPersonsList.Add(rankListData);
                        //    playerData.GetInstance().playerRankData.bestfourCurrentRank = currentRank;
                        //    playerData.GetInstance().playerRankData.bestfourYesterday = yesterdayRank;
                        //    break;
                        //case RankListType.Fortune:
                        //    playerData.GetInstance().playerRankData.fortuneList.Add(rankListData);
                        //    break;
                        //case RankListType.YesterdayRank:
                        //    playerData.GetInstance().playerRankData.yesterdayList.Add(rankListData);
                        //    playerData.GetInstance().playerRankData.yestterday = currentValue;
                        //    playerData.GetInstance().playerRankData.yestterdayCurrentRank = currentRank;
                        //    playerData.GetInstance().playerRankData.yestterday_yes = yesterdayRank;
                        //    break;
                        case RankListType.Level:
                            playerData.GetInstance().playerRankData.levelList.Add(rankListData);
                            playerData.GetInstance().playerRankData.lv = currentValue;
                            playerData.GetInstance().playerRankData.lvCurrentRank = currentRank;
                            playerData.GetInstance().playerRankData.lvYesterday = yesterdayRank;
                            break;
                        case RankListType.RealTimeRank:
                            playerData.GetInstance().playerRankData.realTimeRankList.Add(rankListData);
                            playerData.GetInstance().playerRankData.realTime = currentValue;
                            playerData.GetInstance().playerRankData.realTimeCurrentRank = currentRank;
                            playerData.GetInstance().playerRankData.realTimeYesterday = yesterdayRank;
                            break;
                        default:
                            break;
                    }
                }
                int intCount = 20 / 4;
                int page = 20 % 4;
                if (page != 0)
                {
                    intCount++;
                }
                playerData.GetInstance().playerRankData.page = intCount;
              //  RankListManage.GetInstace().GetRankListData((int)playerData.GetInstance().playerRankData.rankListType);
            }
            return true;
        }
        else
        {

            //   Debug.Log(string.Format("返回信息失败：{0}", data["desc"].ToString()));
            Debug.Log("获取排行榜列表失败");
            return false;
        }
       
    }
}
