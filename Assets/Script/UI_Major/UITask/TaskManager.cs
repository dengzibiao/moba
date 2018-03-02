using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Tianyu;
public class TaskArgs : EventArgs
{
    public TaskModifyType modifietype;
}
public delegate void ReachplaceNpcEvent(long npcid);
public delegate void ReachplaceTransferEvent();
public delegate void ReachplaceCollectPointEvent();
public delegate void ReachplaceMonsterEvent();
public delegate void ReachplaceMonsterDropEvent();
/// <summary>
/// 任务单例类 用于管理各种任务
/// </summary>
public class TaskManager
{
    private static TaskManager single;
    public static TaskManager Single()
    {
        if (single == null)
        {
            single = new TaskManager();

        }
        return single;
    }
    public ReachplaceNpcEvent reachplaceNpcEvent;
    public ReachplaceTransferEvent reachplaceTransferEvent;
    public ReachplaceCollectPointEvent reachplaceCollectPointEvent;
    public ReachplaceMonsterEvent reachplaceMonsterEvent;
    public ReachplaceMonsterDropEvent reachplaceMonsterDropEvent;
    public int caijiCount = 0;
    public int skillMonsterCount = 0;
    public bool isAcceptTask = false;//用于弹出接受任务特效
    //用于任务跳转场景后是否继续自动寻路
    public bool isTaskAutoTraceToTransfer = false;//任务自动寻到到传送门
    public List<Vector3> posList = new List<Vector3>();
    //记录上次自动寻路的信息 当跳转场景后，再恢复上次自动寻路信息
    public long taskAutoTraceID = 0;
    public TaskMoveTarget taskMoveType = TaskMoveTarget.Null;
    public int maiID;
    public bool isCollecting = false;//是否正在采集
    public bool isDialogue = false;//是否正在对话

    //------------------------任务对话-------------------------//
    public  List<string> contonts = new List<string>();
    public bool isSmalltalk = true;
    public bool isDialogueTask = false;
    public long npcid;
    //-----------------------任务奖励数据---------------------//
    public List<ItemData> itemlist = new List<ItemData>();

    /* 用于更好的对应任务模块表 taskid 为键值 */
    Dictionary<int, TaskInstructionsNode> taskInstructionsNodeMap;
    public Dictionary<int, TaskInstructionsNode> TaskInstructionsNodeMap
    {
        get
        {
            if (taskInstructionsNodeMap == null)
            {
                taskInstructionsNodeMap = new Dictionary<int, TaskInstructionsNode>();
                foreach (KeyValuePair<long, TaskInstructionsNode> item in FSDataNodeTable<TaskInstructionsNode>.GetSingleton().DataNodeList)
                {
                    if (!taskInstructionsNodeMap.ContainsKey(item.Value.taskid))
                    {
                        taskInstructionsNodeMap.Add(item.Value.taskid, item.Value);
                    }
                }
            }
            return taskInstructionsNodeMap;
        }
    }

    public delegate void TaskHandler(TaskItem sender, TaskArgs e);
    public event TaskHandler OnTaskChange;

    //所有任务状态
    public Dictionary<int, int> taskstate = new Dictionary<int, int>();
    //主线任务
    static public TaskItem MainTask;
    //支线任务列表
    static public Dictionary<int, TaskItem> BranchList = new Dictionary<int, TaskItem>();

    //任务列表 按任务的状态存贮 追踪列表实用
    public Dictionary<TaskProgress, List<TaskItem>> TaskList = new Dictionary<TaskProgress, List<TaskItem>>()
    {
        { TaskProgress.CantAccept,new List<TaskItem>()},
        { TaskProgress.NoAccept,new List<TaskItem>()},
        { TaskProgress.Accept,new List<TaskItem>()},
        { TaskProgress.Complete,new List<TaskItem>()},
        { TaskProgress.Reward,new List<TaskItem>()}
    };

    //当前显示对话信息
    public DialogItem CurrentShowDialogItem;
    public TaskItem CurrentTaskItem;

    //任务列表 仅用于获取每个NPC上任务的最高级状态 NPCID,TASKID,TASKITEM
    //不能接任务列表
    public Dictionary<long, List<TaskItem>> TaskListCantAccept = new Dictionary<long, List<TaskItem>>();
    //未领取任务列表
    public Dictionary<long, List<TaskItem>> TaskListNoAccept = new Dictionary<long, List<TaskItem>>();
    //领取任务列表
    public Dictionary<long, List<TaskItem>> TaskListAccept = new Dictionary<long, List<TaskItem>>();
    //完成未领取任务列表  区别 NPCID为 交任务NPCID
    public Dictionary<long, List<TaskItem>> TaskListComplete = new Dictionary<long, List<TaskItem>>();


    //npc状态只有两种情况（可完成和可领取状态） 所以只记录npc下任务的可领取和可完成即可  其他情况就是闲聊
    static public Dictionary<long, List<TaskProgress>> NpcTaskStateDic = new Dictionary<long, List<TaskProgress>>() { };//{ 101, new List<TaskProgress> { TaskProgress.NoAccept } } 
    //当前npc上的任务        npcid         任务id 任务详情
    static public Dictionary<long, Dictionary<int, TaskItem>> NpcTaskListDic = new Dictionary<long, Dictionary<int, TaskItem>>();

    //记录副本任务 需要去哪个副本的相关信息
    public Dictionary<int, FubenTaskData> TaskToFubenDic = new Dictionary<int, FubenTaskData>();
    //记录采集任务 需要去哪里采集的相关信息
    public Dictionary<int, FubenTaskData> TaskToCaijiDic = new Dictionary<int, FubenTaskData>();
    //记录杀怪任务 需要去哪里杀怪的相关信息
    public Dictionary<int, FubenTaskData> TaskToSkillMonsterDic = new Dictionary<int, FubenTaskData>();
    //记录杀掉掉落物任务 需要去那里杀怪的相关信息
    public Dictionary<int, FubenTaskData> TaskToSMGoodsDic = new Dictionary<int, FubenTaskData>();
    //记录到指定地点使用道具任务，
    public Dictionary<int, FubenTaskData> TaskToTargetUseItemDic = new Dictionary<int, FubenTaskData>();

    //------------------------------------------任务数据(暂时用于测试)--------------------------------------------------------------
    //用户自身任务数据共享：主线 支线   击杀怪物 和 怪物掉落物数量可以共享
    //组队任务共享：击杀怪物（野外普通怪物 野外精英怪物）
    public Dictionary<long, int> TaskSkillMonsterCountsDic = new Dictionary<long, int>();//击杀的怪物数量 怪物id --- count
    public Dictionary<long, int> TaskItemCountsDic = new Dictionary<long, int>();//采集任务物品数量 采集物id -- count 
    public Dictionary<long, int> TaskSMGoodsCountDic = new Dictionary<long, int>();// 杀怪掉落物物品数量 掉落物id --- cout (后端告诉我的是怪物id，我读表转换成掉落物id)

    //任何任务发生变化调用
    public void ModifeTask(TaskItem _taskItem, TaskModifyType modifetype)
    {
        //将任务 对应npc存放 (用于点击npc 给服务器发送 npcid 和 任务id)
        if (NpcTaskListDic.ContainsKey(_taskItem.npcid))
        {
            if (NpcTaskListDic[_taskItem.npcid].ContainsKey(_taskItem.missionid))
            {
                NpcTaskListDic[_taskItem.npcid][_taskItem.missionid] = _taskItem;
            }
            else
            {
                NpcTaskListDic[_taskItem.npcid].Add(_taskItem.missionid, _taskItem);
            }

        }
        else
        {
            if (_taskItem.missionid != 0)
            {
                Dictionary<int, TaskItem> temp = new Dictionary<int, TaskItem>();
                temp.Add(_taskItem.missionid, _taskItem);
                //NpcTaskListDic.Add(_taskItem.npcid,new Dictionary<int, TaskItem>() { { _taskItem.missionid, _taskItem } });
                NpcTaskListDic.Add(_taskItem.npcid, temp);
            }

        }
        //将任务状态对应npc存放 (用于显示npc头顶的状态)
        //taskindex == 0 是主线其他都支线
        if (_taskItem.taskindex == 0)
        {
            MainTask = _taskItem;
            if (_taskItem.npcid != 0)
            {
                if (!NpcTaskStateDic.ContainsKey(_taskItem.npcid))
                {
                    NpcTaskStateDic.Add(_taskItem.npcid, new List<TaskProgress> { _taskItem.taskProgress });
                }
                else
                {
                    if (!NpcTaskStateDic[_taskItem.npcid].Contains(_taskItem.taskProgress))
                    {
                        NpcTaskStateDic[_taskItem.npcid].Add(_taskItem.taskProgress);
                    }
                }

            }
        }
        else//支线
        {
            BranchList.Add(_taskItem.missionid, _taskItem);
            if (_taskItem.npcid != 0)
            {
                if (!NpcTaskStateDic.ContainsKey(_taskItem.npcid))
                {
                    NpcTaskStateDic.Add(_taskItem.npcid, new List<TaskProgress> { _taskItem.taskProgress });
                }
                else
                {
                    if (!NpcTaskStateDic[_taskItem.npcid].Contains(_taskItem.taskProgress))
                    {
                        NpcTaskStateDic[_taskItem.npcid].Add(_taskItem.taskProgress);
                    }
                }

            }
        }
        ChangeTaskList(_taskItem);
        /*
       1：对话；
       2：通关副本；
       3：采集；
       4：提升技能等级；
       5：提升英雄装备等级；
       6：杀怪；
       7：怪物掉落物；
       8：背包物品；
       9：指定地点；
    */
        //清除已经完成的任务信息
        if (FSDataNodeTable<TaskDataNode>.GetSingleton().DataNodeList.ContainsKey(_taskItem.missionid))
        {
            TaskDataNode taskDataNode = FSDataNodeTable<TaskDataNode>.GetSingleton().DataNodeList[_taskItem.missionid];

            if (taskDataNode.Requiretype ==2)
            {
                if (_taskItem.taskProgress == TaskProgress.Reward)
                {
                    if(TaskToFubenDic.ContainsKey(_taskItem.missionid))
                    {
                        TaskToFubenDic.Remove(_taskItem.missionid);

                    }
                }
            }
            if (taskDataNode.Requiretype == 3)
            {
                if (_taskItem.taskProgress == TaskProgress.Reward)
                {
                    if (TaskToCaijiDic.ContainsKey(_taskItem.missionid))
                    {
                        TaskToCaijiDic.Remove(_taskItem.missionid);

                    }
                }
            }
            if (taskDataNode.Requiretype == 6)
            {
                if (_taskItem.taskProgress == TaskProgress.Reward)
                {
                    if (TaskToSkillMonsterDic.ContainsKey(_taskItem.missionid))
                    {
                        TaskToSkillMonsterDic.Remove(_taskItem.missionid);

                    }
                }
            }
            if (taskDataNode.Requiretype == 7)
            {
                if (_taskItem.taskProgress == TaskProgress.Reward)
                {
                    if (TaskToSMGoodsDic.ContainsKey(_taskItem.missionid))
                    {
                        TaskToSMGoodsDic.Remove(_taskItem.missionid);

                    }
                }
            }
            if (taskDataNode.Requiretype == 9)
            {
                if (_taskItem.taskProgress == TaskProgress.Reward)
                {
                    if (TaskToTargetUseItemDic.ContainsKey(_taskItem.missionid))
                    {
                        TaskToTargetUseItemDic.Remove(_taskItem.missionid);

                    }
                }
            }

        }
        
       
        //任务变化执行
        //if (OnTaskChange != null)
        //{
        //    OnTaskChange(_taskItem, new TaskArgs() { modifietype = modifetype });
        //}

        //switch ((TaskClass)_taskItem.tasknode.Type)
        //{
        //    case TaskClass.Main:
        //        switch (modifetype)
        //        {
        //            case TaskModifyType.Add:
        //            case TaskModifyType.Change:
        //                MainTask = _taskItem;

        //                break;
        //            case TaskModifyType.Remove:
        //                MainTask = null;//实际是指向下一个主线任务 再次判定
        //                break;
        //            default:
        //                break;
        //        }

        //        break;
        //    case TaskClass.Branch:
        //        switch (modifetype)
        //        {
        //            case TaskModifyType.Add:
        //                BranchList.Add(_taskItem.missionid, _taskItem);
        //                break;
        //            case TaskModifyType.Change:
        //                BranchList[_taskItem.missionid] = _taskItem;
        //                break;
        //            case TaskModifyType.Remove:
        //                BranchList.Remove(_taskItem.missionid);
        //                break;
        //            default:
        //                break;
        //        }
        //        break;
        //    default:
        //        break;
        //}
        //ChangeTaskList(_taskItem);
        ////任务变化执行
        //if (OnTaskChange != null)
        //{
        //    OnTaskChange(_taskItem, new TaskArgs() { modifietype = modifetype });
        //}
    }

    //根据任务状态 将 其放在不同的列表中
    private void ChangeTaskList(TaskItem _taskItem)
    {
        if (_taskItem.missionid != 0)
        {
            ChangeTaskList(_taskItem.taskProgress, _taskItem);
        }

        //ChangeNpcState(_taskItem.npcid);
        ChangeNpcState();

        //RemoveItemOnOtherList(_taskItem);

        //switch (_taskItem.taskProgress)
        //{
        //    case TaskProgress.CantAccept:
        //        if (TaskListCantAccept.ContainsKey(_taskItem.tasknode.Acceptnpc))
        //        {
        //            if (!TaskListCantAccept[_taskItem.tasknode.Acceptnpc].Contains(_taskItem))
        //            {
        //                if ((TaskClass)_taskItem.tasknode.Type == TaskClass.Main)
        //                {
        //                    TaskListCantAccept[_taskItem.tasknode.Acceptnpc].Insert(0, _taskItem);
        //                }
        //                else
        //                {
        //                    TaskListCantAccept[_taskItem.tasknode.Acceptnpc].Add(_taskItem);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            TaskListCantAccept.Add(_taskItem.tasknode.Acceptnpc, new List<TaskItem>() { _taskItem });
        //        }
        //        ChangeTaskList(TaskProgress.CantAccept, _taskItem);
        //        ChangeNpcState(_taskItem.tasknode.Acceptnpc);
        //        break;
        //    case TaskProgress.NoAccept:
        //        if (TaskListNoAccept.ContainsKey(_taskItem.tasknode.Acceptnpc))
        //        {
        //            if (!TaskListNoAccept[_taskItem.tasknode.Acceptnpc].Contains(_taskItem))
        //            {
        //                if ((TaskClass)_taskItem.tasknode.Type == TaskClass.Main)
        //                {
        //                    TaskListNoAccept[_taskItem.tasknode.Acceptnpc].Insert(0, _taskItem);
        //                }
        //                else
        //                {
        //                    TaskListNoAccept[_taskItem.tasknode.Acceptnpc].Add(_taskItem);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            TaskListNoAccept.Add(_taskItem.tasknode.Acceptnpc, new List<TaskItem>() { _taskItem });
        //        }
        //        ChangeTaskList(TaskProgress.NoAccept, _taskItem);
        //        ChangeNpcState(_taskItem.tasknode.Acceptnpc);
        //        break;
        //    case TaskProgress.Accept:
        //        if (TaskListAccept.ContainsKey(_taskItem.tasknode.Acceptnpc))
        //        {
        //            if (!TaskListAccept[_taskItem.tasknode.Acceptnpc].Contains(_taskItem))
        //            {
        //                if ((TaskClass)_taskItem.tasknode.Type == TaskClass.Main)
        //                {
        //                    TaskListAccept[_taskItem.tasknode.Acceptnpc].Insert(0, _taskItem);
        //                }
        //                else
        //                {
        //                    TaskListAccept[_taskItem.tasknode.Acceptnpc].Add(_taskItem);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            TaskListAccept.Add(_taskItem.tasknode.Acceptnpc, new List<TaskItem>() { _taskItem });
        //        }
        //        ChangeTaskList(TaskProgress.Accept, _taskItem);
        //        ChangeNpcState(_taskItem.tasknode.Acceptnpc);
        //        break;
        //    case TaskProgress.Complete:
        //        if (TaskListComplete.ContainsKey(_taskItem.tasknode.Finishnpc))
        //        {
        //            if (!TaskListComplete[_taskItem.tasknode.Finishnpc].Contains(_taskItem))
        //            {
        //                if ((TaskClass)_taskItem.tasknode.Type == TaskClass.Main)
        //                {
        //                    TaskListComplete[_taskItem.tasknode.Finishnpc].Insert(0, _taskItem);
        //                }
        //                else
        //                {
        //                    TaskListComplete[_taskItem.tasknode.Finishnpc].Add(_taskItem);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            TaskListComplete.Add(_taskItem.tasknode.Finishnpc, new List<TaskItem>() { _taskItem });
        //        }
        //        ChangeTaskList(TaskProgress.Complete, _taskItem);
        //        ChangeNpcState(_taskItem.tasknode.Finishnpc);
        //        break;
        //    case TaskProgress.Reward:
        //        RemoveOtherProgressType(_taskItem);
        //        ChangeNpcState(_taskItem.tasknode.Finishnpc);
        //        break;
        //    default:
        //        break;
        //}
    }

    private void RemoveItemOnOtherList(TaskItem _taskItem)
    {
        if (TaskListCantAccept.ContainsKey(_taskItem.tasknode.Acceptnpc) && TaskListCantAccept[_taskItem.tasknode.Acceptnpc].Contains(_taskItem))
        {
            TaskListCantAccept[_taskItem.tasknode.Acceptnpc].Remove(_taskItem);
        }
        if (TaskListNoAccept.ContainsKey(_taskItem.tasknode.Acceptnpc) && TaskListNoAccept[_taskItem.tasknode.Acceptnpc].Contains(_taskItem))
        {
            TaskListNoAccept[_taskItem.tasknode.Acceptnpc].Remove(_taskItem);
        }
        if (TaskListAccept.ContainsKey(_taskItem.tasknode.Acceptnpc) && TaskListAccept[_taskItem.tasknode.Acceptnpc].Contains(_taskItem))
        {
            TaskListAccept[_taskItem.tasknode.Acceptnpc].Remove(_taskItem);
        }
        if (TaskListComplete.ContainsKey(_taskItem.tasknode.Finishnpc) && TaskListComplete[_taskItem.tasknode.Finishnpc].Contains(_taskItem))
        {
            TaskListComplete[_taskItem.tasknode.Finishnpc].Remove(_taskItem);
        }
    }

    //检测NPC所带任务最大状态
    public TaskProgress GetNPCShowTaskMaxState(int Npcid)
    {
        if (TaskListComplete.ContainsKey(Npcid) && TaskListComplete[Npcid].Count > 0)
        {
            return TaskProgress.Complete;
        }
        //else if (TaskListAccept.ContainsKey(Npcid) && TaskListAccept[Npcid].Count > 0)
        //{
        //    return TaskProgress.Accept;
        //}
        else if (TaskListNoAccept.ContainsKey(Npcid) && TaskListNoAccept[Npcid].Count > 0)
        {
            return TaskProgress.NoAccept;
        }
        //else if (TaskListCantAccept.ContainsKey(Npcid) && TaskListCantAccept[Npcid].Count > 0)
        //{
        //    return TaskProgress.CantAccept;
        //}
        return TaskProgress.CantAccept;
    }

    //更改主城NPC状态 调用： 任务发生变化 NPC 创建之后
    public void ChangeNpcState(long Npcid)
    {
        //TaskProgress state = GetNPCShowTaskMaxState(Npcid);
        //GameObject NPCObject = NPCManager.GetSingle().GetNpcObj(Npcid);
        //if (NPCObject)
        //{
        //    NPCManager.GetSingle().GetNpcObj(Npcid).GetComponent<TaskNPC>().SetTaskState(state);
        //}


        //不在npcr任务状态列表中的就是没任务 什么也不显示
        TaskProgress state;
        GameObject NPCObject = NPCManager.GetSingle().GetNpcObj(Npcid);
        if (NpcTaskStateDic.ContainsKey(Npcid))
        {
            if (NpcTaskStateDic[Npcid].Contains(TaskProgress.Complete))
            {
                state = TaskProgress.Complete;
            }
            else if (NpcTaskStateDic[Npcid].Contains(TaskProgress.NoAccept))
            {
                state = TaskProgress.NoAccept;
            }
            else
            {
                state = TaskProgress.CantAccept;
            }

        }
        else
        {
            state = TaskProgress.CantAccept;
        }
        if (NPCObject)
        {
            NPCManager.GetSingle().GetNpcObj(Npcid).GetComponent<TaskNPC>().SetTaskState(state);
        }
    }

    public void ChangeNpcState()
    {
        List<int> list = NPCManager.GetSingle().GetNpcIdList();
        if (list != null && list.Count > 0)
        {
            for (int i = 0; i < list.Count; i++)
            {
                TaskProgress state;
                GameObject NPCObject = NPCManager.GetSingle().GetNpcObj(list[i]);
                if (NpcTaskStateDic.ContainsKey(list[i]))
                {
                    if (NpcTaskStateDic[list[i]].Contains(TaskProgress.Complete))
                    {
                        state = TaskProgress.Complete;
                    }
                    else if (NpcTaskStateDic[list[i]].Contains(TaskProgress.NoAccept))
                    {
                        state = TaskProgress.NoAccept;
                    }
                    else
                    {
                        state = TaskProgress.CantAccept;
                    }

                }
                else
                {
                    state = TaskProgress.CantAccept;
                }
                if (NPCObject)
                {
                    NPCManager.GetSingle().GetNpcObj(list[i]).GetComponent<TaskNPC>().SetTaskState(state);
                }
            }
        }
    }
    //获取该NPC身上所有任务
    public List<TaskItem> GetNPCTaskList(int Npcid)
    {
        List<TaskItem> list = new List<TaskItem>();
        if (TaskListComplete.ContainsKey(Npcid))
        {
            list.AddRange(TaskListComplete[Npcid]);
        }
        if (TaskListAccept.ContainsKey(Npcid))
        {
            list.AddRange(TaskListAccept[Npcid]);
        }
        if (TaskListNoAccept.ContainsKey(Npcid))
        {
            list.AddRange(TaskListNoAccept[Npcid]);
        }
        if (TaskListCantAccept.ContainsKey(Npcid))
        {
            list.AddRange(TaskListCantAccept[Npcid]);
        }
        return list;
    }

    public TaskItem GetTaskByTaskId(int taskid)
    {
        if (MainTask != null && MainTask.missionid == taskid)
        {
            return MainTask;
        }
        else
        {
            if (BranchList.ContainsKey(taskid))
            {
                return BranchList[taskid];
            }
        }
        return null;
    }
    void ChangeTaskList(TaskProgress taskProgress, TaskItem taskItem)
    {
        RemoveOtherProgressType(taskItem);
        if ((TaskClass)taskItem.tasknode.Type == TaskClass.Main)
        {
            TaskList[taskProgress].Insert(0, taskItem);
        }
        else
        {
            TaskList[taskProgress].Add(taskItem);
        }
    }

    private void RemoveOtherProgressType(TaskItem _taskItem)
    {
        //姑且这样 随后改成键值对 方便删除
        for (int i = 0; i < TaskList[TaskProgress.CantAccept].Count; i++)
        {
            if (TaskList[TaskProgress.CantAccept][i].tasknode.Taskid == _taskItem.tasknode.Taskid)
            {
                TaskList[TaskProgress.CantAccept].RemoveAt(i);
                break;
            }
        }

        for (int i = 0; i < TaskList[TaskProgress.NoAccept].Count; i++)
        {
            if (TaskList[TaskProgress.NoAccept][i].tasknode.Taskid == _taskItem.tasknode.Taskid)
            {
                TaskList[TaskProgress.NoAccept].RemoveAt(i);
                break;
            }
        }
        for (int i = 0; i < TaskList[TaskProgress.Accept].Count; i++)
        {
            if (TaskList[TaskProgress.Accept][i].tasknode.Taskid == _taskItem.tasknode.Taskid)
            {
                TaskList[TaskProgress.Accept].RemoveAt(i);
                break;
            }
        }
        for (int i = 0; i < TaskList[TaskProgress.Complete].Count; i++)
        {
            if (TaskList[TaskProgress.Complete][i].tasknode.Taskid == _taskItem.tasknode.Taskid)
            {
                TaskList[TaskProgress.Complete].RemoveAt(i);
                break;
            }
        }
    }

    //获取当前所有任务按状态排序
    public List<TaskItem> GetStateTaskList()
    {
        List<TaskItem> list = new List<TaskItem>();
        list.AddRange(TaskList[TaskProgress.Reward]);
        list.AddRange(TaskList[TaskProgress.Complete]);
        list.AddRange(TaskList[TaskProgress.Accept]);
        list.AddRange(TaskList[TaskProgress.NoAccept]);
        list.AddRange(TaskList[TaskProgress.CantAccept]);
        return list;
    }
    /// <summary>
    /// 通过任务id 获得任务奖励list
    /// </summary>
    /// <param name="id">任务id</param>
    /// <returns></returns>
    public List<ItemData> GetItemList(int id)
    {
        List<ItemData> itemlist = new List<ItemData>();
        TaskRewardNode taskRewardNode;
        if (FSDataNodeTable<TaskRewardNode>.GetSingleton().DataNodeList.ContainsKey(id))
        {
            taskRewardNode = FSDataNodeTable<TaskRewardNode>.GetSingleton().DataNodeList[id];

            if (taskRewardNode.Gold > 0)
            {
                
                ItemData itemdata = SetItemDate(114000100,taskRewardNode.Gold);
                itemdata.Gold = taskRewardNode.Gold;
                itemdata.GoodsType = MailGoodsType.GoldType;
                itemlist.Add(itemdata);
            }
            if (taskRewardNode.Diamond > 0)
            {
                ItemData itemdata = SetItemDate(114000200, taskRewardNode.Diamond);
                itemdata.GoodsType = MailGoodsType.DiamomdType;
                itemdata.Diamond = taskRewardNode.Diamond;
                itemlist.Add(itemdata);
            }
            if (taskRewardNode.ZhanduiExp > 0)
            {
                ItemData itemdata = SetItemDate(114000300, taskRewardNode.ZhanduiExp);
                itemdata.GoodsType = MailGoodsType.ExE;
                itemdata.Exe = taskRewardNode.ZhanduiExp;
                itemlist.Add(itemdata);
            }
            if (taskRewardNode.HeroExp > 0)
            {
                ItemData itemdata = SetItemDate(114000400, taskRewardNode.HeroExp);
                itemdata.GoodsType = MailGoodsType.HeroExp;
                itemdata.HeroExp = taskRewardNode.HeroExp;
                itemlist.Add(itemdata);
            }
            if (taskRewardNode.XuanshangGold > 0)
            {
                ItemData itemdata = SetItemDate(114000800, taskRewardNode.XuanshangGold);
                itemdata.GoodsType = MailGoodsType.XuanshangGold;
                itemdata.XuanshangGold = taskRewardNode.XuanshangGold;
                itemlist.Add(itemdata);
            }
            for (int i = 0; i < taskRewardNode.ItemArr.GetLength(0); i++)
            {
                ItemData itemdata = new ItemData();
                itemdata.Id = taskRewardNode.ItemArr[i, 0];
                itemdata.Count = taskRewardNode.ItemArr[i, 1];
                if (itemdata.Count <= 0)
                {
                    continue;
                }

                itemdata.Name = GameLibrary.Instance().ItemStateList[itemdata.Id].name;
                itemdata.Types = GameLibrary.Instance().ItemStateList[itemdata.Id].types;
                itemdata.Describe = GameLibrary.Instance().ItemStateList[itemdata.Id].describe;
                itemdata.GoodsType = MailGoodsType.ItemType;
                switch (GameLibrary.Instance().ItemStateList[itemdata.Id].grade)//(VOManager.Instance().GetCSV<ItemCSV>("Item").GetVO(itemdata.Id).grade)
                {
                    case 1:
                        itemdata.GradeTYPE = GradeType.Gray;
                        break;
                    case 2:
                        itemdata.GradeTYPE = GradeType.Green;
                        break;
                    case 4:
                        itemdata.GradeTYPE = GradeType.Blue;
                        break;
                    case 7:
                        itemdata.GradeTYPE = GradeType.Purple;
                        break;
                    case 11:
                        itemdata.GradeTYPE = GradeType.Orange;
                        break;
                    case 16:
                        itemdata.GradeTYPE = GradeType.Red;
                        break;
                }
                if (itemdata.Types == 6)
                {
                    itemdata.UiAtlas = ResourceManager.Instance().GetUIAtlas("UIHeroHead");
                }
                else
                {
                    itemdata.UiAtlas = ResourceManager.Instance().GetUIAtlas("Prop");
                }
                itemdata.Sprice = GameLibrary.Instance().ItemStateList[itemdata.Id].sprice;
                itemdata.Piles = GameLibrary.Instance().ItemStateList[itemdata.Id].piles;
                itemdata.IconName = GameLibrary.Instance().ItemStateList[itemdata.Id].icon_name;

                itemlist.Add(itemdata);
            }

        }
        return itemlist;
    }

    private ItemData SetItemDate(long id,int count)
    {
        ItemData itemdata = new ItemData();
        itemdata.Id = id;
        ItemNodeState itemnodestate = null;
        if (GameLibrary.Instance().ItemStateList.ContainsKey(itemdata.Id))
        {
            itemnodestate = GameLibrary.Instance().ItemStateList[itemdata.Id];
            itemdata.Count = count;
            itemdata.Name = itemnodestate.name;
            itemdata.Types = itemnodestate.types;
            itemdata.Describe = itemnodestate.describe;

            itemdata.Sprice = itemnodestate.sprice;
            itemdata.Piles = itemnodestate.piles;
            itemdata.IconName = itemnodestate.icon_name;
            switch (itemnodestate.grade)//(VOManager.Instance().GetCSV<ItemCSV>("Item").GetVO(itemdata.Id).grade)
            {
                case 1:
                    itemdata.GradeTYPE = GradeType.Gray;
                    break;
                case 2:
                    itemdata.GradeTYPE = GradeType.Green;
                    break;
                case 4:
                    itemdata.GradeTYPE = GradeType.Blue;
                    break;
                case 7:
                    itemdata.GradeTYPE = GradeType.Purple;
                    break;
                case 11:
                    itemdata.GradeTYPE = GradeType.Orange;
                    break;
                case 16:
                    itemdata.GradeTYPE = GradeType.Red;
                    break;
            }
            if (itemdata.Types == 6)
            {
                itemdata.UiAtlas = ResourceManager.Instance().GetUIAtlas("UIHeroHead");
            }
            else
            {
                itemdata.UiAtlas = ResourceManager.Instance().GetUIAtlas("Prop");
            }
        }
        return itemdata;
    }

    /* 这个太牛了 学习了 彭 */
    static uint[] FixedSize1 = new uint[]
    {
            0x0,
            0x01,           0x03,           0x07,           0x0F,
            0x1F,           0x3F,           0x7F,           0xFF,
            0x01FF,         0x03FF,         0x07FF,         0x0FFF,
            0x1FFF,         0x3FFF,         0x7FFF,         0xFFFF,
            0x01FFFF,       0x03FFFF,       0x07FFFF,       0x0FFFFF,
            0x1FFFFF,       0x3FFFFF,       0x7FFFFF,       0xFFFFFF,
            0x01FFFFFF,     0x03FFFFFF,     0x07FFFFFF,     0x0FFFFFFF,
            0x1FFFFFFF,     0x3FFFFFFF,     0x7FFFFFFF,     0xFFFFFFFF,
    };

    public int GetBitValue(int value, short start, short size)
    {
        uint _mask = FixedSize1[size];
        long _data = (value >> start) & _mask;
        return (int)_data;
    }
}
/// <summary>
/// 副本任务---地址相关数据
/// </summary>
public class FubenTaskData
{
    public int opt1;
    public int taskType;
    public int taskId;
    public long opt4;//副本任务 -- 地图id 采集任务--物品id 杀怪任务 --怪物id   杀怪掉落物--怪物id
    public long opt5;//副本任务-- 副本id 采集任务--数量  杀怪任务 -- 怪物数量  杀怪掉落物--掉落物数量
    public long opt6;//采集任务--物品id 杀怪任务 --怪物id    杀怪掉落物--怪物id
    public long opt7;//采集任务--数量 杀怪任务 -- 怪物数量   杀怪掉落物--掉落物数量
    public string user1;
    public string user2;
    public string user3;
}
/// <summary>
/// 日常任务
/// </summary>
public class EveryTaskData
{
    /// <summary>
    /// 任务id
    /// </summary>
    public int id;
    /// <summary>
    /// 地图id
    /// </summary>
    public int mapId;
    /// <summary>
    /// 任务目标
    /// </summary>
    public long taskTarget;

    /// <summary>
    /// 任务解锁状态
    /// </summary>
    public int state;
    /// <summary>
    /// 任务名称
    /// </summary>
    public string taskName;
    /// <summary>
    /// 描述
    /// </summary>
    public string des;
    public string taskListDes;
    /// <summary>
    /// 任务图标
    /// </summary>
    public string iconName;
    /// <summary>
    /// 活跃度
    /// </summary>
    public int active;
    /// <summary>
    /// 前往界面ID
    /// </summary>
    public int leave_for;
    /// <summary>
    /// 当前活跃度
    /// </summary>
    public int activeIndex;
    /// <summary>
    /// 任务类型
    /// </summary>
    public int type;
    /// <summary>
    /// 任务需要完成次数
    /// </summary>
    public int count;
    /// <summary>
    /// 任务当前完成次数
    /// </summary>
    public int countIndex;
    /// <summary>
    /// 脚本ID
    /// </summary>
    public int scriptId;
    /// <summary>
    /// 解锁系统ID
    /// </summary>
    public int unlockSystem;
    /// <summary>
    /// 当前版本是否开放
    /// </summary>
    public int released;
    /// <summary>
    /// 奖励物品[id,数量]
    /// </summary>
    public int[,] goodsItem;
    /// <summary>
    /// <summary>
    /// 任务解锁描述
    /// </summary>
    public string deblockingDes;
    /// <summary>
    /// 星级
    /// </summary>
    public int star;
    /// <summary>
    /// 解锁等级
    /// </summary>
    public int open;

    public long useprops_id;
}

