using UnityEngine;
using System.Collections;
using Tianyu;
using System.Collections.Generic;
using System;

public class TaskPropsNode : FSDataNodeBase
{
    /// <summary>
    /// 任务道具id
    /// </summary>
    public long id;
    public int sn;
    /// <summary>
    /// 任务道具图标
    /// </summary>
    public string icon;
    /// <summary>
    /// 任务道具名称
    /// </summary>
    public string iconName;
    /// <summary>
    /// 按钮名称
    /// </summary>
    public string btnName;
    /// <summary>
    /// 描述
    /// </summary>
    public string des;
    /// <summary>
    /// 标题
    /// </summary>
    public string title;
    /// <summary>
    /// 信件标签
    /// </summary>
    public string sign;
    public override void parseJson(object jd)
    {
        Dictionary<string, object> item = (Dictionary<string, object>)jd;
        id = long.Parse(item["id"].ToString());
        sn = int.Parse(item["sn"].ToString());
        icon = item["iocn"].ToString();
        iconName = item["item_name"].ToString();
        btnName = item["button_name"].ToString();
        des = item["explain"].ToString();
        title = item["note"].ToString();
        sign = item["sign"].ToString();
    }
}
