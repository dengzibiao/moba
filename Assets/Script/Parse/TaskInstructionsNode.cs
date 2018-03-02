using System.Collections.Generic;
using UnityEngine;
using Tianyu;
public class TaskInstructionsNode : FSDataNodeBase
{
    public int taskid;//任务id
    public int indexarr;//索引位置
    public short zeroposition;//起始位置
    public short length;//长度
    public override void parseJson(object jd)
    {
        Dictionary<string, object> item = (Dictionary<string, object>)jd;
        taskid = int.Parse(item["taskid"].ToString());
        indexarr = int.Parse(item["index_arr"].ToString());
        zeroposition = short.Parse(item["zero_position"].ToString());
        length = short.Parse(item["length"].ToString());
    }
}

