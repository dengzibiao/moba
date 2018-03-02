using UnityEngine;
using System.Collections;
using Tianyu;
using System;
using System.Collections.Generic;

public class MealAttrNode : FSDataNodeBase
{
    /// <summary> id/// </summary>
    public int id;
    /// <summary> 名字/// </summary>
    public string name;
    /// <summary> 描述信息/// </summary>
    public string info;
    /// <summary> 头像/// </summary>
    public string icon;
    /// <summary> 开始时间/// </summary>
    public int startTime;
    /// <summary> 结束时间/// </summary>
    public int endTime;
    /// <summary> 增加力量/// </summary>
    public int power;
    /// <summary> /// </summary>
    public int releassed;
    public override void parseJson(object jd)
    {
        Dictionary<string, object> item = (Dictionary<string, object>)jd;
        id = int.Parse(item["id"].ToString());
        name = item["name"].ToString();
        info = item["info"].ToString();
        icon = item["icon"].ToString();
        startTime = int.Parse(item["start_time"].ToString());
        endTime = int.Parse(item["end_time"].ToString());
        power = int.Parse(item["physical_power"].ToString());
        releassed = int.Parse(item["released"].ToString());
    }

}
