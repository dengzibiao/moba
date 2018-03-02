/*
文件名（File Name）:   RewardTask.cs

作者（Author）:    #高#

创建时间（CreateTime）:  2016-10-28 13:36:53
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tianyu;

public class RewardTaskNode : FSDataNodeBase
{
    public int id;//任务id
    public int mapId;//任务id
    public string taskName;//任务名称
    public string des;//任务描述
    public string iconName;//任务图标
    public int type;//任务类型
    public int count;//杀怪个数
    public int scriptId;//脚本ID
    public long task_target;//任务目标
    public string taskListdes;//主城任务面板描述
    public long useprops_id;//送信·关联送信表
    public override void parseJson(object jd)
    {
        Dictionary<string, object> item = (Dictionary<string, object>)jd;
        id = int.Parse(item["daily_id"].ToString());
        taskName = item["name"].ToString();
        iconName = item["icon_name"].ToString();
        type = int.Parse(item["type"].ToString());
        des = item["info"].ToString();
        useprops_id = long.Parse(item["Useprops_id"].ToString());
        taskListdes = item["info2"].ToString();
        mapId = int.Parse(item["map_id"].ToString());
        count = int.Parse(item["require"].ToString());
        task_target= long.Parse(item["task_target"].ToString());
        mapId= int.Parse(item["map_id"].ToString());
    }
}
