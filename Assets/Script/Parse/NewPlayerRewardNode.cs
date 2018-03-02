/*
文件名（File Name）:   NewPlayerRewardNode.cs

作者（Author）:    #高#

创建时间（CreateTime）:  #CreateTime#
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tianyu;

public class NewPlayerRewardNode : FSDataNodeBase
{

    public int id;//id
    public int diamond;//钻石数
    public int gold;//金币数
    public int[,] goodsItem;//奖励物品[id,数量]

    public override void parseJson(object jd)
    {
        Dictionary<string, object> item = jd as Dictionary<string, object>;
        id = int.Parse(item["id"].ToString());
        diamond = int.Parse(item["diamond"].ToString());
        gold = int.Parse(item["gold"].ToString());
        object[] nodeCond = (object[])item["item"];
        goodsItem=new int[nodeCond.Length,2];
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

