using UnityEngine;
using System.Collections;

public class ItemArenaRank : GUISingleItemList
{

    public GUISingleSpriteGroup stars;//星星
    public GUISingleLabel levelLab;//等级
    public GUISingleSprite icon;//头像
    public GUISingleSprite iconFrame;//头像框
    protected override void InitItem()
    {
        
    }
    public override void Info(object obj)
    {
        base.Info(obj);
        if (obj == null) return;
        if (playerData.GetInstance().playerRankData.rankListType == RankListType.YesterdayRank || playerData.GetInstance().playerRankData.rankListType == RankListType.RealTimeRank)//之前判断战力的
        {
            stars.gameObject.SetActive(true);
            levelLab.gameObject.SetActive(true);
            icon.gameObject.SetActive(true);
            icon.gameObject.SetActive(true);
            //stars.IsShow("xing", "xing-hui", currentPage, playerData.GetInstance().lotteryInfo.page);
        }
        else
        {
            stars.gameObject.SetActive(false);
            levelLab.gameObject.SetActive(false);
            icon.gameObject.SetActive(false);
            icon.gameObject.SetActive(false);
        }

    }
}
