using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Tianyu;
public class ItemRankList : GUISingleItemList
{

    public GUISingleSprite leveRankSp;//等级显示
    public GUISingleSprite leveRankBg;//等级显示底

    public GUISingleSprite icon;//头像
    public GUISingleSprite iconBorder;//头像框
    public GUISingleLabel playerNameLab;//玩家名字
    public GUISingleLabel leveRankLab;//排名等级显示LAB
    public GUISingleSprite fightSumSp_a;//总战力-美术字显示
    public GUISingleLabel playerLv;//玩家列表等级数值

    public GUISingleLabel lvLab;//玩家等价

    public GUISingleSprite fightSp_a;//战力-美术字显示
    public UIWidget bestFourWdt;//四强显示
    public GUISingleMultList multList;//自己的三个英雄信息
    /// <summary>四强/// </summary>
    public UISprite[] bestFour_Icon;
    public UILabel[] bestFour_level;
    public UISprite[] bestFour_star1;
    public UISprite[] bestFour_star2;
    public UISprite[] bestFour_star3;
    public UISprite[] bestFour_star4;
    public UISprite[] bestFour_IconBorder;//四强头像框

    List<UISprite[]> stars = new List<UISprite[]>();
    private RoleIconAttrNode item = null;
    private RoleIconAttrNode itemBox = null;
    int iconLen = 5;
    private RankListData rankListData;
    int currentPae =1;//记录页数
    public string[] besrFours;//储存四强数据
    public static ItemRankList _instance;
    protected override void InitItem()
    {
        _instance = this;
      ///  multList = transform.FindComponent<GUISingleMultList>("MultList");
        stars.Add(bestFour_star1);
        stars.Add(bestFour_star2);
        stars.Add(bestFour_star3);
        stars.Add(bestFour_star4);
    }
    protected override void ShowHandler()
    {
        base.ShowHandler();
    }
    public override void Info(object obj)
    {
        if (obj == null) return;
        if (((RankListData)obj).currentRank == 1)
        {
            leveRankLab.gameObject.SetActive(false);
            leveRankSp.spriteName = "1";
        }
        else if (((RankListData)obj).currentRank == 2)
        {
            leveRankLab.gameObject.SetActive(false);
            leveRankSp.spriteName = "2";
        }
        else if (((RankListData)obj).currentRank == 3)
        {
            leveRankLab.gameObject.SetActive(false);
            leveRankSp.spriteName = "3";
        }
        else if (((RankListData)obj).currentRank == 4)
        {
            leveRankSp.gameObject.SetActive(false);
            leveRankBg.gameObject.SetActive(false);
            leveRankLab.text = "4";

        }
        else
        {
            leveRankSp.gameObject.SetActive(false);
            leveRankBg.gameObject.SetActive(false);
            currentPae = UIRankList._instance.currentPage;
            leveRankLab.text = (index + currentPae * 4+ 1).ToString();
        }
        lvLab.text = ((RankListData)obj).playerLv.ToString();
        playerNameLab.text = ((RankListData)obj).name;
        Dictionary<long, RoleIconAttrNode> mAllIconNode = FSDataNodeTable<RoleIconAttrNode>.GetSingleton().DataNodeList;
      
        mAllIconNode.TryGetValue(((RankListData)obj).iconId, out item);
       //mAllIconNode.TryGetValue(((RankListData)obj).iconBoxId, out itemBox);

        if (null != item)
        {
           icon.spriteName = item.icon_name + "_head";
           //iconBorder.spriteName = itemBox.icon_name;
        }
        if (playerData.GetInstance().playerRankData.rankListType == RankListType.RealTimeRank)
        {
            fightSumSp_a.GetComponentInChildren<UILabel>().text = ((RankListData)obj).rankValue.ToString();//三英雄的总战力
            playerLv.gameObject.SetActive(false);
            fightSp_a.gameObject.SetActive(false);
            fightSumSp_a.gameObject.SetActive(true);
            bestFourWdt.gameObject.SetActive(true);

            multList.InSize(((RankListData)obj).arenaHeroList.Count, 5);
            multList.Info(((RankListData)obj).arenaHeroList.ToArray());
        }
        else
        {
            playerLv.text = "战队等级：" + ((RankListData)obj).playerLv.ToString();
            fightSp_a.GetComponentInChildren<GUISingleLabel>().text = ((RankListData)obj).rankValue.ToString();
            fightSp_a.gameObject.SetActive(true);
            fightSumSp_a.gameObject.SetActive(false);

            bestFourWdt.gameObject.SetActive(false);
        }

        //if (playerData.GetInstance().playerRankData.rankListType == RankListType.BestFourPersons)
        //{
        //    int fc = 0;
        //    bestFourWdt.gameObject.SetActive(true);
        //    for (int i = 0; i < ((RankListData)obj).bestFour.Length; i++)
        //    {
        //        if (((RankListData)obj).bestFour[i] != null)
        //        {

        //            string bestFour = ((RankListData)obj).bestFour[i].ToString();

        //            string[] bestFourss = bestFour.Split(',');

        //            bestFour_level[i].text = bestFourss[2];

        //            for (int j = 0; j < int.Parse(bestFourss[4]); j++)//星星
        //            {
        //                stars[i][j].gameObject.SetActive(true);
        //            }

        //            for (int j = int.Parse(bestFourss[4]); j < 5; j++)
        //            {
        //                stars[i][j].gameObject.SetActive(false);
        //            }

        //            int id = int.Parse(bestFourss[1]);///获取四强头像
        //            mAllIconNode.TryGetValue(id, out item);
        //            if (item != null)
        //            {
        //                bestFour_Icon[i].atlas = ResourceManager.Instance().GetUIAtlas("UIHeroHead");
        //                bestFour_Icon[i].spriteName = item.icon_name;
        //            }
        //            bestFour_IconBorder[i].atlas = ResourceManager.Instance().GetUIAtlas("Prop");
        //            switch(int.Parse(bestFourss[5]))
        //            {
        //                case (int)GradeType.Gray:
        //                    bestFour_IconBorder[i].spriteName = "bai";
        //                    break;
        //                case (int)GradeType.Green:
        //                    bestFour_IconBorder[i].spriteName = "lv";
        //                    break;
        //                case (int)GradeType.Blue:
        //                    bestFour_IconBorder[i].spriteName = "lan";
        //                    break;
        //                case (int)GradeType.Purple:
        //                    bestFour_IconBorder[i].spriteName = "zi";
        //                    break;
        //                case (int)GradeType.Orange:
        //                    bestFour_IconBorder[i].spriteName = "cheng";
        //                    break;
        //                case (int)GradeType.Red:
        //                    bestFour_IconBorder[i].spriteName = "hong";
        //                    break;
        //                default:
        //                    break;
        //            }
        //            fc += int.Parse(bestFourss[3]);
        //        }
        //    }
            //fightSumSp_a.gameObject.SetActive(true);
           // bestFour_Sumfight.text = fc.ToString();
        //}

    }
}
