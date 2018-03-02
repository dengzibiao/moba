using UnityEngine;
using System.Collections;

/// 任务
/// </summary>
public class TaskItem
{
    public int taskindex;//任务类型
    //任务id
    public int missionid;
    public TaskClass type;//主线支线悬赏
    public int scripid;//任务脚本id

    public bool able;

    public long parm0, parm1, parm2, parm3,parm4 = 0;

    public long npcid;//当前任务的npc

    public TaskDataNode tasknode;
    //任务状态
    public TaskProgress taskProgress;
    //进度
    public int currentValue;
}

//任务对话框详细信息
public class DialogItem
{
    public string title;
    public string disc;
    public string[] opt = new string[4];
    public int msId;
    public int scrId;
    public int[] user = new int[3];
}
