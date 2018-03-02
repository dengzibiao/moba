/*
文件名（File Name）:   DailyTasksNode.cs

作者（Author）:    #高#

创建时间（CreateTime）:  #CreateTime#
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tianyu;

public class DailyTasksNode : FSDataNodeBase
{
    public int id;//任务id
    public string taskName;//任务名称
    public string des;//任务描述
    public string deblockingDes;//任务解锁描述
    public string iconName;//任务图标
    public int active;//活跃度
    public int type;//任务类型
    public int count;//任务完成次数
    public int scriptId;//脚本ID
    public int leave_for;//前往界面ID
    public int unlockSystem;//解锁系统ID
    public int released;//当前版本是否开放
    public int[,] goodsItem;//奖励物品[id,数量]
    public int open;//解锁等级，排序用
    public override void parseJson(object jd)
    {
        Dictionary<string, object> item = (Dictionary<string, object>)jd;
        id= int.Parse(item["daily_id"].ToString());
        taskName = item["name"].ToString();
        des = item["info"].ToString();
        open= int.Parse(item["open"].ToString());
        iconName = item["icon_name"].ToString();
        type= int.Parse(item["type"].ToString());
        leave_for = int.Parse(item["leave_for"].ToString());
        count = int.Parse(item["require"].ToString());
        scriptId= int.Parse(item["script_id"].ToString());
        unlockSystem = int.Parse(item["unlock_system"].ToString());
        released = int.Parse(item["released"].ToString());
        active= int.Parse(item["active"].ToString());
        deblockingDes= item["condition"].ToString();
        //object[] nodeCond = (object[])item["reward_prop"];
        ////goodsItem = new int[nodeCond.Length, 2];
        ////if (nodeCond.Length > 0)
        ////{
        ////    for (int i = 0; i < nodeCond.Length; i++)
        ////    {
        ////        int[] node = (int[])nodeCond[i];

        ////        for (int j = 0; j < node.Length; j++)
        ////        {
        ////            goodsItem[i, j] = node[j];
        ////        }
        ////    }
        ////}
    }
}
