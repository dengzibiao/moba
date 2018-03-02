/*
文件名（File Name）:   SoulDrawNode.cs

作者（Author）:    #高#

创建时间（CreateTime）:  #CreateTime#
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tianyu;

public class SoulDrawNode : FSDataNodeBase
{
    private  int[,] goodsItem;//奖励物品[id,数量]
    public  int id;
    public override void parseJson(object jd)
    {
        Dictionary<string, object> item = jd as Dictionary<string, object>;
        int[] nodeCond = (int[])item["give_exp"];
        if (nodeCond.Length > 0)
        {
            id = nodeCond[0];
        }
    }
}
