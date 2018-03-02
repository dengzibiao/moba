using UnityEngine;
using System.Collections;
using Tianyu;
using System.Collections.Generic;
using System;

public class ResetLaterNode : FSDataNodeBase
{
    /// <summary>
    /// id
    /// </summary>
    public int id;
    /// <summary>
    /// 购买体力消耗的钻石数
    /// </summary>
    public int[] powerBuy;
    /// <summary>
    /// 重置副本消耗的钻石数
    /// </summary>
    public int[] resetStage;
    /// <summary>
    /// 购买角斗场挑战次数
    /// </summary>
    public int[] buy_abattoir;
    /// <summary>
    /// 重置竞技场挑战时间钻石数
    /// </summary>
    public int resetarenaCd;
    /// <summary>
    /// 购买活力消耗的钻石数
    /// </summary>
    public int[] vitalityBuy;
    /// <summary>
    /// 购买技能点消耗的钻石数
    /// </summary>
    public int[] skillBuy;
    /// <summary>
    /// 杂货店刷新消耗的钻石数
    /// </summary>
    public int[] generalShop;
    /// <summary>
    /// 竞技场商店刷新消耗-竞技场币
    /// </summary>
    public int[] generalJjc;
    /// <summary>
    /// 角斗场商店刷新消耗-角斗场币
    /// </summary>
    public int[] generalJdc;
    /// <summary>
    /// 悬赏商店刷新消耗-悬赏币
    /// </summary>
    public int[] generalXs;
    /// <summary>
    /// 刷新悬赏任务
    /// </summary>
    public int[] buy_reward;
    /// <summary>
    /// 补签消耗的钻石数
    /// </summary>
    public int retroactiveBuy;
    /// <summary>
    /// 悬赏任务立即完成花费
    /// </summary>
    public int finish_reward;
    /// <summary>
    /// 原地复活死亡次数花费
    /// </summary>
    public int[] standing_resurrection;
    /// <summary>
    ///营地复活死亡次数消耗金币
    /// </summary>
    public int[] camp_resurrection;
    public override void parseJson(object jd)
    {
        Dictionary<string, object> item = (Dictionary<string, object>)jd;
        id = int.Parse(item["id"].ToString());

        int[] powerIntarr = item["power_buy"] as int[];
        if (powerIntarr != null)
        {
            powerBuy = new int[powerIntarr.Length];
            for (int m = 0; m < powerIntarr.Length; m++)
            {
                powerBuy[m] = powerIntarr[m];
            }
        }
         int[] buyreward = item["buy_reward"] as int[];
        if (buyreward != null)
        {
            buy_reward = new int[buyreward.Length];
            for (int m = 0; m < buyreward.Length; m++)
            {
                buy_reward[m] = buyreward[m];
            }
        }

        int[] stageIntarr = item["reset_stage"] as int[];
        if (stageIntarr != null)
        {
            resetStage = new int[stageIntarr.Length];
            for (int m = 0; m < stageIntarr.Length; m++)
            {
                resetStage[m] = stageIntarr[m];
            }
        }

        buy_abattoir = item["buy_abattoir"] as int[];

        resetarenaCd = int.Parse(item["reset_arena_cd"].ToString());


        int[] skillIntarr = item["buy_skill"] as int[];
        if (skillIntarr != null)
        {
            skillBuy = new int[skillIntarr.Length];
            for (int m = 0; m < skillIntarr.Length; m++)
            {
                skillBuy[m] = skillIntarr[m];
            }
        }

        int[] generalshopIntarr = item["general_shop"] as int[];
        if (generalshopIntarr != null)
        {
            generalShop = new int[generalshopIntarr.Length];
            for (int m = 0; m < generalshopIntarr.Length; m++)
            {
                generalShop[m] = generalshopIntarr[m];
            }
        }
        int[] generaljdcArr = item["general_jdc"] as int[];
        if(generaljdcArr!=null)
        {
            generalJdc=new int[generaljdcArr.Length];
            for(int i=0;i<generaljdcArr.Length;i++)
            {
                generalJdc[i] = generaljdcArr[i];
            }
        }
        int[] generaljjcArr = item["general_jjc"] as int[];
        if (generaljjcArr != null)
        {
            generalJjc = new int[generaljjcArr.Length];
            for (int j = 0; j < generaljjcArr.Length; j++)
            {
                generalJjc[j] = generaljjcArr[j];
            }
        }
        int[] generalxsArr = item["general_xs"] as int[];
        if (generalxsArr != null)
        {
            generalXs = new int[generalxsArr.Length];
            for (int l = 0; l < generalxsArr.Length; l++)
            {
                generalXs[l] = generalxsArr[l];
            }
        }
        retroactiveBuy = int.Parse(item["buy_retroactive"].ToString());
        finish_reward = int.Parse(item["finish_reward"].ToString());
        //if (retroactiveIntarr != null)
        //{
        //    retroactiveBuy = new int[retroactiveIntarr.Length];
        //    for (int m = 0; m < retroactiveIntarr.Length; m++)
        //    {
        //        retroactiveBuy[m] = retroactiveIntarr[m];
        //    }
        //}
        //死亡复活
        int[] vitalityIntarr = item["standing_resurrection"] as int[];
        if (vitalityIntarr != null)
        {
            standing_resurrection = new int[vitalityIntarr.Length];
            for (int m = 0; m < vitalityIntarr.Length; m++)
            {
                standing_resurrection[m] = vitalityIntarr[m];
            }
        }

         vitalityIntarr = item["camp_resurrection"] as int[];
        if (vitalityIntarr != null)
        {
            camp_resurrection = new int[vitalityIntarr.Length];
            for (int m = 0; m < vitalityIntarr.Length; m++)
            {
                camp_resurrection[m] = vitalityIntarr[m];
            }
        }

    }
}
