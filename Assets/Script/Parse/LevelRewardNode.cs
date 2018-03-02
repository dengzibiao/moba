using UnityEngine;
using System.Collections;
using Tianyu;
using System;
using System.Collections.Generic;

/// <summary>
/// 等级礼包奖励
/// </summary>
public class LevelRewardNode : FSDataNodeBase
{

    public int id;//id
    public int need_lv;//触发等级
    public int diamond;//钻石数
    public int gold;//金币数
    public int power;//体力数
    public long[,] goodsItem;//奖励物品[id,数量]
    public override void parseJson(object jd)
    {
        Dictionary<string, object> item = jd as Dictionary<string, object>;
        id = int.Parse(item["id"].ToString());
        need_lv = int.Parse(item["need_lv"].ToString());
        diamond = int.Parse(item["diamond"].ToString());
        gold = int.Parse(item["gold"].ToString());
        power = int.Parse(item["power"].ToString());

        object[] nodeCond = (object[])item["item"];
        goodsItem = new long[nodeCond.Length, 2];
        if (nodeCond.Length > 0)
        {
            for (int i = 0; i < nodeCond.Length; i++)
            {
                int[] node = (int[])nodeCond[i];

                for (int j = 0; j < node.Length; j++)
                {
                    goodsItem[i, j] = node[j];
                }
            }
        }
    }
}
