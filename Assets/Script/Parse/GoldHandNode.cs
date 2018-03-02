using UnityEngine;
using System.Collections;
using Tianyu;
using System.Collections.Generic;
using System;

public class GoldHandNode : FSDataNodeBase
{
    /// <summary>
    /// 点金次数
    /// </summary>
    public int id;
    /// <summary>
    /// 点金手段位
    /// </summary>
    public int time;
    /// <summary>
    /// 钻石消耗
    /// </summary>
    public int diamondCost;
    /// <summary>
    /// 基础倍率
    /// </summary>
    public float basicRate;
    /// <summary>
    /// 递增公差
    /// </summary>
    public float commonDifference;
    /// <summary>
    /// 点金双倍概率
    /// </summary>
    public int midas2;
    /// <summary>
    /// 点金五倍概率
    /// </summary>
    public int midas5;
    /// <summary>
    /// 点金十倍概率 
    /// </summary>
    public int midas10;
    public override void parseJson(object jd)
    {
        Dictionary<string, object> item = (Dictionary<string, object>)jd;
        id = int.Parse(item["id"].ToString());
        time = int.Parse(item["time"].ToString());
        diamondCost = int.Parse(item["diamond_cost"].ToString());
        basicRate = float.Parse(item["basic_rate"].ToString());
        commonDifference = float.Parse(item["common_difference"].ToString());
        midas2 = int.Parse(item["midas2"].ToString());
        midas5 = int.Parse(item["midas5"].ToString());
        midas10 = int.Parse(item["midas10"].ToString());
    }

}
