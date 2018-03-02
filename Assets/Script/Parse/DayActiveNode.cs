/*
文件名（File Name）:   DayActiveNode.cs

作者（Author）:    #高#

创建时间（CreateTime）:  2016-10-24 20:13:42
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tianyu;

public class DayActiveNode : FSDataNodeBase
{
    public int Active_value1;//活跃度阶段1
    public int Active_value2;//活跃度阶段2
    public int Active_value3;//活跃度阶段3
    public int Active_value4;//活跃度阶段4
    public long[,] active_prop1;//奖励物品[id,数量]活跃度阶段1奖励
    public long[,] active_prop2;//奖励物品[id,数量]活跃度阶段2奖励
    public long[,] active_prop3;//奖励物品[id,数量]活跃度阶段3奖励
    public long[,] active_prop4;//奖励物品[id,数量]活跃度阶段4奖励
    public string prop_explain1;//奖励1说明
    public string prop_explain2;//奖励2说明
    public string prop_explain3;//奖励3说明
    public string prop_explain4;//奖励4说明
    public override void parseJson(object jd)
    {
        Dictionary<string, object> item = jd as Dictionary<string, object>;

        Active_value1 = int.Parse(item["Active_value1"].ToString());
        Active_value2 = int.Parse(item["Active_value2"].ToString());
        Active_value3 = int.Parse(item["Active_value3"].ToString());
        Active_value4 = int.Parse(item["Active_value4"].ToString());
        prop_explain1 = item["prop_explain1"].ToString();
        prop_explain2 = item["prop_explain2"].ToString();
        prop_explain3 = item["prop_explain3"].ToString();
        prop_explain4 = item["prop_explain4"].ToString();

        object[] nodeCond1 = (object[])item["active_prop1"];
        active_prop1 = new long[nodeCond1.Length, 2];
        if (nodeCond1.Length > 0)
        {
            for (int i = 0; i < nodeCond1.Length; i++)
            {
                int[] node = (int[])nodeCond1[i];

                for (int j = 0; j < node.Length; j++)
                {
                    active_prop1[i, j] = node[j];
                }
            }
        }
        object[] nodeCond2 = (object[])item["active_prop2"];
        active_prop2 = new long[nodeCond2.Length, 2];
        if (nodeCond2.Length > 0)
        {
            for (int i = 0; i < nodeCond2.Length; i++)
            {
                int[] node = (int[])nodeCond2[i];

                for (int j = 0; j < node.Length; j++)
                {
                    active_prop2[i, j] = node[j];
                }
            }
        }
        object[] nodeCond3 = (object[])item["active_prop3"];
        active_prop3 = new long[nodeCond3.Length, 2];
        if (nodeCond3.Length > 0)
        {
            for (int i = 0; i < nodeCond3.Length; i++)
            {
                int[] node =(int[]) nodeCond3[i];

                for (int j = 0; j < node.Length; j++)
                {
                    active_prop3[i, j] = node[j];
                }
            }
        }
        object[] nodeCond4 = item["active_prop4"] as object[];
        active_prop4 = new long[nodeCond4.Length, 2];
        if (nodeCond4.Length > 0)
        {
            for (int i = 0; i < nodeCond4.Length; i++)
            {
                int[] node = (int[])nodeCond4[i];

                for (int j = 0; j < node.Length; j++)
                {
                    active_prop4[i, j] = node[j];
                }
            }
        }
    }
}
