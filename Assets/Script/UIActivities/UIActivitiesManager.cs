/*
文件名（File Name）:   UIActivitiesManager.cs

作者（Author）:    #高#

创建时间（CreateTime）:  2016-11-15 13:6:53
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tianyu;

public class UIActivitiesManager  {
    private static UIActivitiesManager instance = null;
    public static UIActivitiesManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new UIActivitiesManager();
            }
            return instance;
        }
    }

    private EveryTaskData vo;
    /// <summary>
    /// 创建人物追的数据
    /// </summary>
    /// <param name="voo"></param>
    /// <returns></returns>
    public TaskData CreatTaskData(EveryTaskData voo)
    {
        this.vo = voo;
        TaskData taskData = new TaskData();
        taskData.taskState = TaskProgress.Accept;
        taskData.taskType = TaskClass.Reward;
        taskData.title = "[悬]:" + voo.taskName;
        taskData.content = Singleton<UIActivitiesManager>.Instance.GetTaskItemType(voo, voo.type);
        return taskData;
    }
    /// <summary>
    /// 获取采集物ID
    /// </summary>
    /// <returns></returns>
    public long GetCollectItemID(long flag)
    {
        if (FSDataNodeTable<CollectNode>.GetSingleton().DataNodeList.ContainsKey(flag))
        {
            //通过采集源id 得到采集物的id 用于发给服务器
            long[,] collectid = FSDataNodeTable<CollectNode>.GetSingleton().DataNodeList[flag].collectid;
            return collectid[0, 0];
        }
        else
        {
            return 0;
        }
    }
    /// <summary>
    /// 用于任务追踪执行操作
    /// </summary>
    /// <param name="vo"></param>
    public void RewardTaskOperate(EveryTaskData vo)
    {
        switch ((TaskType)vo.type)
        {
            case TaskType.Dialogue:
                break;
            case TaskType.PassCopy:
                break;
            case TaskType.Collect:
                TaskAutoTraceManager._instance.MoveToTargetPosition(vo.taskTarget, TaskMoveTarget.MoveToCollectPos, vo.mapId);
                break;
            case TaskType.UpgradeSkillLv:
                break;
            case TaskType.UpgradeHeroEquipLv:
                break;
            case TaskType.KillMonster:
                TaskAutoTraceManager._instance.MoveToTargetPosition(vo.taskTarget, TaskMoveTarget.MoveToMonsterPos, vo.mapId);
                break;
            case TaskType.KillDropSth:
                break;
            case TaskType.knapsackItem:
                break;
            case TaskType.NamedPComplete:
                //TaskAutoTraceManager._instance.MoveToTargetPosition(vo.taskTarget, TaskMoveTarget.MoveToNpc, vo.mapId);
                TaskOperation.Single().MoveToNpc((int)vo.taskTarget, TaskOperation.MoveToNpcType.RewardSendLetter);
                break;
            case TaskType.KillTempMonster:
                break;
            case TaskType.KillPerson:
                break;
        }
    }
    ///根据类型取任务描述
    public string GetTaskItemType(EveryTaskData vo,int type)
    {
        if (vo.countIndex > vo.count)
        {
            vo.countIndex = vo.count;
            Debug.Log("服务器发来的数据错误，当前数量超出总数");
        }
        switch ((TaskType)vo.type)
        {
            case TaskType.Dialogue:

                return vo.taskListDes + "(" + vo.countIndex + "/" + vo.count + ")";
                break;
            case TaskType.PassCopy:

                return vo.taskListDes + "(" + vo.countIndex + "/" + vo.count + ")";
                break;
            case TaskType.Collect:
                return vo.taskListDes + "(" + vo.countIndex + "/" + vo.count + ")";
                break;
            case TaskType.UpgradeSkillLv:
                return vo.taskListDes + "(" + vo.countIndex + "/" + vo.count + ")";
                break;
            case TaskType.UpgradeHeroEquipLv:
                return vo.taskListDes + "(" + vo.countIndex + "/" + vo.count + ")";
                break;
            case TaskType.KillMonster:
                return vo.taskListDes + "(" + vo.countIndex + "/" + vo.count + ")";
                break;
            case TaskType.KillDropSth:
                return vo.taskListDes + "(" + vo.countIndex + "/" + vo.count + ")";
                break;
            case TaskType.knapsackItem:
                return vo.taskListDes + "(" + vo.countIndex + "/" + vo.count + ")";
                break;
            case TaskType.NamedPComplete:
                return vo.taskListDes + "(" + vo.countIndex + "/" + vo.count + ")";
                break;
            case TaskType.KillTempMonster:
                return vo.taskListDes + "(" + vo.countIndex + "/" + vo.count + ")";
                break;
            case TaskType.KillPerson:
                return vo.taskListDes + "(" + vo.countIndex + "/" + vo.count + ")";
                break;
            default:
                return null;
        }
    }
    /// <summary>
    /// 处理悬赏人任务列表数据
    /// </summary>
    /// <param name="data"></param>
    public void RewardListHandler(Dictionary<string, object> data)
    {
        if (data.ContainsKey("misinfo"))
        {
            object[] mState = data["misinfo"] as object[];
            if (mState != null)
            {
                playerData.GetInstance().taskDataList.itList.Clear();
                if (data.ContainsKey("rtms"))
                {
                    playerData.GetInstance().taskDataList.getCount = int.Parse(data["rtms"].ToString());

                }
                if (data.ContainsKey("utms"))
                {
                    playerData.GetInstance().taskDataList.refreshCount = int.Parse(data["utms"].ToString());

                }
                for (int i = 0; i < mState.Length; i++)
                {
                    EveryTaskData taskData = new EveryTaskData();

                    object[] lt = mState[i] as object[];
                    taskData.id = int.Parse(lt[1].ToString());
                    taskData.countIndex = int.Parse(lt[5].ToString());
                    taskData.state = int.Parse(lt[4].ToString());
                    taskData.star = int.Parse(lt[10].ToString());
                    if (taskData.id != 0)
                    {
                        if (FSDataNodeTable<RewardTaskNode>.GetSingleton().DataNodeList.ContainsKey(taskData.id))
                        {
                            RewardTaskNode dailyTasksNode = FSDataNodeTable<RewardTaskNode>.GetSingleton().FindDataByType(taskData.id);
                            taskData.count = dailyTasksNode.count;
                            taskData.des = dailyTasksNode.des;
                            taskData.taskListDes = dailyTasksNode.taskListdes;
                            taskData.iconName = dailyTasksNode.iconName;
                            taskData.taskName = dailyTasksNode.taskName;
                            taskData.useprops_id = dailyTasksNode.useprops_id;
                            taskData.type = dailyTasksNode.type;
                            taskData.taskTarget = dailyTasksNode.task_target;
                            taskData.mapId = dailyTasksNode.mapId;
                            playerData.GetInstance().taskDataList.itList.Add(taskData);
                        }

                    }
                }
                //if (Singleton<SceneManage>.Instance.Current == EnumSceneID.UI_MajorCity01 && UITaskTracker.instance.IsShow() || Singleton<SceneManage>.Instance.Current == EnumSceneID.LGhuangyuan && UITaskTracker.instance.IsShow())
                //{
                //    UITaskTracker.instance.GetRewardData();
                //}
            }
        }
    }
    /// <summary>
    /// 处理日常任务的数据
    /// </summary>
    /// <param name="data"></param>
    public void DailyHandler(Dictionary<string, object> data)
    {
        playerData.GetInstance().taskDataList.itemList.Clear();
        object[] missionlist = data["misinfo"] as object[];

        //读取日常列表的所有数据
        foreach (var value in FSDataNodeTable<DailyTasksNode>.GetSingleton().DataNodeList.Values)
        {
            EveryTaskData taskData = new EveryTaskData();
            taskData.released = value.released;
            if (taskData.released != 0)
            {
                taskData.unlockSystem = value.unlockSystem;
                if (taskData.unlockSystem == 0)
                {
                    taskData.id = value.id;
                    taskData.active = value.active;
                    taskData.count = value.count;
                    taskData.state = 0;
                    taskData.countIndex = 0;
                    taskData.activeIndex = 0;
                    taskData.des = value.des;
                    taskData.deblockingDes = "";
                    taskData.iconName = value.iconName;
                    taskData.leave_for = value.leave_for;
                    taskData.scriptId = value.scriptId;
                    taskData.type = value.type;
                    taskData.open = 0;
                    taskData.taskName = value.taskName;
                    playerData.GetInstance().taskDataList.itemList.Add(taskData);
                }
                else
                {
                    if (FSDataNodeTable<UnLockFunctionNode>.GetSingleton().DataNodeList.ContainsKey(taskData.unlockSystem))
                    {
                        UnLockFunctionNode Node = FSDataNodeTable<UnLockFunctionNode>.GetSingleton().FindDataByType(taskData.unlockSystem);
                        taskData.id = value.id;
                        taskData.active = value.active;
                        taskData.count = value.count;
                        taskData.state = 0;
                        taskData.countIndex = 0;
                        taskData.activeIndex = 0;
                        taskData.des = value.des;
                        taskData.deblockingDes = Node.limit_tip;
                        taskData.iconName = value.iconName;
                        taskData.leave_for = value.leave_for;
                        taskData.scriptId = value.scriptId;
                        taskData.type = value.type;
                        int a = Node.unlock_system_type;
                        if (a == 1)
                        {
                            taskData.open = Node.condition_parameter;
                        }
                        else
                        {
                            taskData.open = 0;
                        }
                        taskData.taskName = value.taskName;
                        playerData.GetInstance().taskDataList.itemList.Add(taskData);
                    }
                }
            }
        }
        //根据服务器发送的日常列表进行状态重置
        for (int i = 0; i < missionlist.Length; i++)
        {
            int[] taskDataDic = missionlist[i] as int[];
            int ID = taskDataDic[0];

            if (ID != 0)
            {
                EveryTaskData taskServerData = playerData.GetInstance().taskDataList.itemList.Find(x => x.id == ID);
                if (taskServerData != null)
                {
                    taskServerData.state = taskDataDic[1];
                    taskServerData.countIndex = taskDataDic[2];
                }
            }
        }
        if (playerData.GetInstance().taskDataList.itemList.Count > 0)
        {
            List<EveryTaskData> itemLt = new List<EveryTaskData>();
            var a = playerData.GetInstance().taskDataList.itemList.FindAll(x => x.state == (int)TaskProgress.Complete);
            itemLt.AddRange(a);
            var b = playerData.GetInstance().taskDataList.itemList.FindAll(x => x.state == (int)TaskProgress.NoAccept);
            itemLt.AddRange(b);
            var c = playerData.GetInstance().taskDataList.itemList.FindAll(x => x.state == (int)TaskProgress.CantAccept);
            c.Sort((x, y) => x.open - y.open);
            itemLt.AddRange(c);
            var d = playerData.GetInstance().taskDataList.itemList.FindAll(x => x.state == (int)TaskProgress.Reward);
            itemLt.AddRange(d);
            playerData.GetInstance().taskDataList.itemList.Clear();
            playerData.GetInstance().taskDataList.itemList.AddRange(itemLt);
            itemLt.Clear();
        }
        //if (playerData.GetInstance().taskDataList.itemList.Count > 0)
        //{
        //    if (Singleton<SceneManage>.Instance.Current == EnumSceneID.UI_MajorCity01 && UIActivities.Instance.IsShow())
        //    {
        //        UIActivities.Instance.RefreshUI();
        //    }
        //}
        Singleton<Notification>.Instance.ReceiveMessageList(MessageID.common_ask_daily_mission_req);
    }
}
