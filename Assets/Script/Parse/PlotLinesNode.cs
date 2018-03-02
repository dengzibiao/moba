using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tianyu;
using System;
/// <summary>
/// 剧情台词表现表
/// </summary>
public class PlotLinesNode : FSDataNodeBase
{
    private int plotID;
    private int taskID;
    private int place;
    private int plotType;
    private int speakerType;
    private string speakerID;
    private float intervaltime;
    private int nextplot;
    private string content;
    private string[] voice;

    #region GetAndSet
    /// <summary>
    /// 剧情序号
    /// </summary>
    public int PlotID
    {
        get
        {
            return plotID;
        }

        set
        {
            plotID = value;
        }
    }
    /// <summary>
    /// 所属任务编号
    /// </summary>
    public int TaskID
    {
        get
        {
            return taskID;
        }

        set
        {
            taskID = value;
        }
    }
    /// <summary>
    /// 触发位置
    /// </summary>
    public int Place
    {
        get
        {
            return place;
        }

        set
        {
            place = value;
        }
    }
    /// <summary>
    /// 剧情类型
    /// </summary>
    public int PlotType
    {
        get
        {
            return plotType;
        }

        set
        {
            plotType = value;
        }
    }
    /// <summary>
    /// 说话者类型
    /// </summary>
    public int SpeakerType
    {
        get
        {
            return speakerType;
        }

        set
        {
            speakerType = value;
        }
    }
    /// <summary>
    /// 说话者id
    /// </summary>
    public string SpeakerID
    {
        get
        {
            return speakerID;
        }

        set
        {
            speakerID = value;
        }
    }
    /// <summary>
    /// 冒泡间隔时间
    /// </summary>
    public float Intervaltime
    {
        get
        {
            return intervaltime;
        }

        set
        {
            intervaltime = value;
        }
    }
    /// <summary>
    /// 下句台词
    /// </summary>
    public int Nextplot
    {
        get
        {
            return nextplot;
        }

        set
        {
            nextplot = value;
        }
    }
    /// <summary>
    /// 台词内容
    /// </summary>
    public string Content
    {
        get
        {
            return content;
        }

        set
        {
            content = value;
        }
    }

    public string[] Voice
    {
        get
        {
            return voice;
        }

        set
        {
            voice = value;
        }
    }
    #endregion


    public override void parseJson(object jd)
    {
        Dictionary<string, object> item = (Dictionary<string, object>)jd;

        plotID = int.Parse(item["plot_id"].ToString());
        taskID = int.Parse(item["taskid"].ToString());
        place = int.Parse(item["place"].ToString());
        plotType = int.Parse(item["plot_type"].ToString());
        speakerType = int.Parse(item["speaker_type"].ToString());
        speakerID = item["speaker_id"].ToString();
        intervaltime = float.Parse(item["interval_time"].ToString());
        nextplot = int.Parse(item["next_plot"].ToString());
        content = item["lines_content"].ToString();
        if (item.ContainsKey("voice"))
        {
            voice = item["voice"].ToString().Split('|');
        }  
    }
}
