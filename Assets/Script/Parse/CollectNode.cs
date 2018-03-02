using UnityEngine;
using System.Collections;
using Tianyu;
using System.Collections.Generic;
using System;
public class CollectNode : FSDataNodeBase
{
    /// <summary>
    ///  采集资源id
    /// </summary>
    public long id;
    /// <summary>
    ///  序号
    /// </summary>
    public int sn;
    /// <summary>
    /// 类型
    /// </summary>
    public int type;
    /// <summary>
    /// 地图序号
    /// </summary>
    public int mapid;
    /// <summary>
    /// 怪物id
    /// </summary>
    public long monsterid;
    /// <summary>
    /// 资源名称
    /// </summary>
    public string resourcename;
    /// <summary>
    /// 模型
    /// </summary>
    public int model;
    /// <summary>
    /// 采集物id
    /// </summary>
    public long[,] collectid;
    public string note;
    public override void parseJson(object jd)
    {
        Dictionary<string, object> item = (Dictionary<string, object>)jd;
        id = long.Parse(item["id"].ToString());
        sn = int.Parse(item["sn"].ToString());
        type = int.Parse(item["types"].ToString());
        mapid = int.Parse(item["mapid"].ToString());
        monsterid = long.Parse(item["monster_id"].ToString());
        resourcename = item["resource_name"].ToString();
        model = int.Parse(item["model"].ToString());
        note = item["note"].ToString();

        object[] nodeCond = (object[])item["collect_id"];
        collectid = new long[nodeCond.Length, 2];
        if (nodeCond.Length > 0)
        {
            for (int i = 0; i < nodeCond.Length; i++)
            {
                int[] node = (int[])nodeCond[i];

                for (int j = 0; j < node.Length; j++)
                {
                    collectid[i, j] = node[j];
                }
            }
        }
    }
}
