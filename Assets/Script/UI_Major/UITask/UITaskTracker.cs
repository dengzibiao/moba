/*
文件名（File Name）:   UITaskTracker.cs

作者（Author）:    #高#

创建时间（CreateTime）:  #CreateTime#
*/
using UnityEngine;
using System.Collections.Generic;
using Tianyu;
public enum TaskListHandleType : int
{
    AddOrUpdata = 0,
    Delet,

}
public class UITaskTracker : GUIBase
{
    public static UITaskTracker instance;
    public GUISingleCheckBoxGroup checkBoxs;
    public Animation TaskListAnimation;
    private bool isclose = false;
    public GameObject jianTou;
    public GUISingleMultList multList;
    public GameObject taskitemobj;
    public Transform recycle;
    private UITweener tweenP;
    private TaskData _data;

    public GameObject GuideBut;

    public UITaskTracker()
    {
        instance = this;
    }

    public override UIPanleID GetUIKey()
    {
        return UIPanleID.UITaskTracker;
    }
    protected override void Init()
    {
        checkBoxs.onClick = OnCheckClick;
        UIEventListener.Get(jianTou).onClick = OpenTaskList;
        tweenP = transform.GetComponent<TweenPosition>();
        checkBoxs.DefauleIndex = 0;
        OnCheckClick(0, true);
        OpenTaskList(this.gameObject);
        //if (TaskManager.Single().GetStateTaskList().Count > 0)
        //{
        //    TaskItem item = TaskManager.Single().GetStateTaskList()[0];
        //    ClientSendDataMgr.GetSingle().GetTaskSend().SendReqTaskInfo(C2SMessageType.Active, item.missionid, item.scripid);
        //}
        //if (playerData.GetInstance().taskDataList.itList.Count <= 0)
        //{
        //    ClientSendDataMgr.GetSingle().GetTaskSend().RequestRewardList();
        //}
        // 
        Debug.Log("111111111111111111111111111111111");
    }

    public override void ReceiveData(uint messageID)
    {
        base.ReceiveData(messageID);
        switch (messageID)
        {
            case MessageID.common_offer_reward_mission_list_ret:
                SetTaskTrackerShow();
                break;
            case MessageID.common_notice_common_ex_req://任务通用协议
                Show();
                if (playerData.GetInstance().taskDataList.itList.Count <= 0)
                {
                    Singleton<Notification>.Instance.Send(MessageID.common_ask_offer_reward_mission_req, C2SMessageType.ActiveWait);
                }
                break;
            case MessageID.common_notice_common_ret:
                SetTaskTrackerShow();
                break;
            case MessageID.common_mission_complete_list_ret:
                Singleton<Notification>.Instance.Send(MessageID.common_mission_list_req, C2SMessageType.ActiveWait);
                break;
            case MessageID.common_mission_list_ret:
                if (TaskManager.Single().GetStateTaskList().Count > 0)
                {
                    TaskItem item = TaskManager.Single().GetStateTaskList()[0];
                    ClientSendDataMgr.GetSingle().GetTaskSend().SendReqTaskInfo(C2SMessageType.Active, item.missionid, item.scripid);
                }
                break;
        }
    }

    protected override void OnLoadData()
    {
        base.OnLoadData();
        Singleton<Notification>.Instance.RegistMessageID(MessageID.common_offer_reward_mission_list_ret, this.GetUIKey());
        Singleton<Notification>.Instance.RegistMessageID(MessageID.common_notice_common_ex_req, this.GetUIKey()); 
        Singleton<Notification>.Instance.RegistMessageID(MessageID.common_notice_common_ret, this.GetUIKey()); 
        Singleton<Notification>.Instance.RegistMessageID(MessageID.common_mission_list_ret, this.GetUIKey());
        Singleton<Notification>.Instance.RegistMessageID(MessageID.common_mission_complete_list_ret, this.GetUIKey());
        Singleton<Notification>.Instance.Send(MessageID.common_mission_complete_list_req, C2SMessageType.ActiveWait);
        if (playerData.GetInstance().taskDataList.itList.Count > 0)
        {
            Show();
        }
    }

    private void OnCheckClick(int index, bool boo)
    {
        if (boo)
        {
            switch (index)
            {
                case 0:

                    break;
                case 1:
                    Control.ShowGUI(UIPanleID.UITooltips, EnumOpenUIType.DefaultUIOrSecond, false, "组队功能暂未开放");
                    break;
            }

        }
    }
    private void OpenTaskList(GameObject go)
    {
        isclose = !isclose;
        if (isclose)
        {
            jianTou.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        else
        {
            jianTou.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
        tweenP.Play(isclose);
    }

    protected override void ShowHandler()
    {
        //TaskManager.Single().OnTaskChange += InitTaskList;
        //InitTaskList(null, null);
        SetTaskTrackerShow();
        //  UIGuidePanel.Single().Init();
    }
    /// <summary>
    /// 处理悬赏任务处理列表操作
    /// </summary>
    /// <param name="data"></param>
    /// <param name="type"></param>
    public void ChangeTaskList(TaskData data, TaskListHandleType type)
    {
        switch (type)
        {
            case TaskListHandleType.AddOrUpdata:
                if (taskDataList.Find(x => x.taskType == TaskClass.Reward) != null)
                {
                    for (int i = 0; i < taskDataList.Count; i++)
                    {
                        if (taskDataList[i].taskType == TaskClass.Reward)
                            taskDataList[i] = data;

                    }
                    multList.Info(taskDataList.ToArray());
                }
                else taskDataList.Add(data);
                CreatTaskList();

                break;
            case TaskListHandleType.Delet:
                if (taskDataList.Find(x => x.taskType == TaskClass.Reward) != null)
                {
                    for (int i = 0; i < taskDataList.Count; i++)
                    {
                        if (taskDataList[i].taskType == TaskClass.Reward)
                            taskDataList.RemoveAt(i);
                    }
                }
                CreatTaskList();
                break;
        }
    }

    //追踪列表随任务发生变化
    public void InitTaskList(TaskItem sender, TaskArgs e)
    {
        SetTaskTrackerShow();
    }
    public List<TaskData> taskDataList = new List<TaskData>();
    private TaskItem taskItem;
    void SetTaskTrackerShow()
    {
        taskDataList.Clear();

        //for (int i = taskDataList.Count - 1; i >= 0; i--)
        //{
        //    if (taskDataList[i].taskItem.taskindex<=9)
        //    {
        //        taskDataList .RemoveAt(i);
        //    }
        //}

        List<TaskItem> TaskItemList = TaskManager.Single().GetStateTaskList();
        for (int i = 0; i < TaskItemList.Count; i++)
        {
            TaskData taskData = new TaskData();
            taskItem = TaskItemList[i];
            taskData.taskItem = taskItem;
            TaskDataNode taskDataNode = FSDataNodeTable<TaskDataNode>.GetSingleton().FindDataByType(taskItem.missionid);
            if (taskDataNode != null)
            {
                taskData.title = GetTaskTypeString(taskDataNode.Type) + taskItem.tasknode.Taskname;
                if (taskDataNode.Requiretype == 3)//采集数量显示
                {
                    long collectID1 = 0;
                    long collectID2 = 0;
                    if (TaskManager.Single().TaskToCaijiDic.ContainsKey(taskItem.missionid))
                    {
                        collectID1 = TaskManager.Single().TaskToCaijiDic[taskItem.missionid].opt4;
                        collectID2 = TaskManager.Single().TaskToCaijiDic[taskItem.missionid].opt6;
                        //if (TaskManager.Single().TaskItemCountsDic.ContainsKey(collectID))
                        //{
                        //    if (TaskManager.Single().TaskItemCountsDic[collectID] < TaskManager.Single().TaskToCaijiDic[_taskItem.missionid].opt5)
                        //    {
                        //        caijiCount = TaskManager.Single().TaskItemCountsDic[collectID];
                        //    }
                        //    else
                        //    {
                        //        caijiCount = (int)TaskManager.Single().TaskToCaijiDic[_taskItem.missionid].opt5;
                        //    }
                        //}
                        //else
                        //{
                        //    caijiCount = 0;
                        //}
                    }
                    else//后端给我的信息是在 接收任务之后。这就导致未接的时候找不到ID，没法替换追踪显示中的数量。这时候我自己去读表
                    {
                        if (taskDataNode.Opt5.Length > 0)
                        {
                            long trackingIndex = taskDataNode.Opt5[0];
                            if (FSDataNodeTable<CollectNode>.GetSingleton().DataNodeList.ContainsKey(trackingIndex))
                            {
                                collectID1 = FSDataNodeTable<CollectNode>.GetSingleton().DataNodeList[trackingIndex].collectid[0, 0];
                            }
                        }
                        if (taskDataNode.Opt5.Length > 1)
                        {
                            long trackingIndex = taskDataNode.Opt5[1];
                            if (FSDataNodeTable<CollectNode>.GetSingleton().DataNodeList.ContainsKey(trackingIndex))
                            {
                                collectID2 = FSDataNodeTable<CollectNode>.GetSingleton().DataNodeList[trackingIndex].collectid[0, 0];
                            }
                        }

                    }
                    taskData.content = taskItem.tasknode.Require.Replace("c1", GameLibrary.C1)
                                                                 .Replace("c2", GameLibrary.C2)
                                                                 .Replace("c3", GameLibrary.C3)
                                                                 .Replace("c4", GameLibrary.C4)
                                                                 .Replace("c5", GameLibrary.C5)
                                                                 .Replace("c6", GameLibrary.C6)
                                                                 .Replace("%d" + collectID1, GetTaskGoodsCount(taskDataNode.Requiretype, collectID1, taskItem).ToString())
                                                                 .Replace("%d" + collectID2, GetTaskGoodsCount(taskDataNode.Requiretype, collectID2, taskItem).ToString());
                }
                else if (taskDataNode.Requiretype == 6)//杀怪数量显示
                {
                    long monsterId1 = 0;
                    long monsterId2 = 0;
                    if (TaskManager.Single().TaskToSkillMonsterDic.ContainsKey(taskItem.missionid))
                    {
                        //暂时杀怪是两种 最多三种
                        monsterId1 = TaskManager.Single().TaskToSkillMonsterDic[taskItem.missionid].opt4;
                        monsterId2 = TaskManager.Single().TaskToSkillMonsterDic[taskItem.missionid].opt6;
                        //if (TaskManager.Single().TaskSkillMonsterCountsDic.ContainsKey(monsterId1))
                        //{
                        //    if (TaskManager.Single().TaskSkillMonsterCountsDic[monsterId1] < TaskManager.Single().TaskToSkillMonsterDic[_taskItem.missionid].opt5)
                        //    {
                        //        monster1Count = TaskManager.Single().TaskSkillMonsterCountsDic[monsterId1];
                        //    }
                        //    else
                        //    {
                        //        monster1Count = (int)TaskManager.Single().TaskToSkillMonsterDic[_taskItem.missionid].opt5;
                        //    }
                        //}
                        //if (TaskManager.Single().TaskSkillMonsterCountsDic.ContainsKey(monsterId2))
                        //{
                        //    if (TaskManager.Single().TaskSkillMonsterCountsDic[monsterId2] < TaskManager.Single().TaskToSkillMonsterDic[_taskItem.missionid].opt7)
                        //    {
                        //        monster2Count = TaskManager.Single().TaskSkillMonsterCountsDic[monsterId2];
                        //    }
                        //    else
                        //    {
                        //        monster2Count = (int)TaskManager.Single().TaskToSkillMonsterDic[_taskItem.missionid].opt7;
                        //    }
                        //}
                    }
                    else
                    {
                        Dictionary<long, int> idAndcountDic = taskDataNode.IdAndcountDic;
                        List<long> idArr = new List<long>();
                        foreach (long id in idAndcountDic.Keys)
                        {
                            idArr.Add(id);
                        }
                        if (idArr.Count >= 2)
                        {
                            monsterId1 = idArr[0];
                            monsterId2 = idArr[1];
                        }
                        else if (idArr.Count == 1)
                        {
                            monsterId1 = idArr[0];
                        }


                    }
                    taskData.content = taskItem.tasknode.Require.Replace("c1", GameLibrary.C1)
                                                                 .Replace("c2", GameLibrary.C2)
                                                                 .Replace("c3", GameLibrary.C3)
                                                                 .Replace("c4", GameLibrary.C4)
                                                                 .Replace("c5", GameLibrary.C5)
                                                                 .Replace("c6", GameLibrary.C6)
                                                                 .Replace("%d" + monsterId1, GetTaskGoodsCount(taskDataNode.Requiretype, monsterId1, taskItem).ToString())
                                                                 .Replace("%d" + monsterId2, GetTaskGoodsCount(taskDataNode.Requiretype, monsterId2, taskItem).ToString());
                }
                else if (taskDataNode.Requiretype == 7)//杀怪掉落物显示
                {

                    long itemId1 = 0;
                    long itemId2 = 0;
                    if (TaskManager.Single().TaskToSMGoodsDic.ContainsKey(taskItem.missionid))
                    {
                        itemId1 = TaskManager.Single().TaskToSMGoodsDic[taskItem.missionid].opt4;
                        itemId2 = TaskManager.Single().TaskToSMGoodsDic[taskItem.missionid].opt6;
                    }
                    else
                    {
                        long trackingIndex = taskDataNode.Opt2;
                        if (FSDataNodeTable<CollectNode>.GetSingleton().DataNodeList.ContainsKey(trackingIndex))
                        {
                            if (FSDataNodeTable<CollectNode>.GetSingleton().DataNodeList[trackingIndex].collectid.GetLength(0) >= 2)
                            {
                                itemId1 = FSDataNodeTable<CollectNode>.GetSingleton().DataNodeList[trackingIndex].collectid[0, 0];

                                itemId2 = FSDataNodeTable<CollectNode>.GetSingleton().DataNodeList[trackingIndex].collectid[1, 0];

                            }
                            else if (FSDataNodeTable<CollectNode>.GetSingleton().DataNodeList[trackingIndex].collectid.GetLength(0) == 1)
                            {
                                itemId1 = FSDataNodeTable<CollectNode>.GetSingleton().DataNodeList[trackingIndex].collectid[0, 0];
                            }

                        }

                    }
                    taskData.content = taskItem.tasknode.Require.Replace("c1", GameLibrary.C1)
                                                                 .Replace("c2", GameLibrary.C2)
                                                                 .Replace("c3", GameLibrary.C3)
                                                                 .Replace("c4", GameLibrary.C4)
                                                                 .Replace("c5", GameLibrary.C5)
                                                                 .Replace("c6", GameLibrary.C6)
                                                                 .Replace("%d" + itemId1, GetTaskGoodsCount(taskDataNode.Requiretype, itemId1, taskItem).ToString())
                                                                 .Replace("%d" + itemId2, GetTaskGoodsCount(taskDataNode.Requiretype, itemId2, taskItem).ToString());


                    //long smonsterId1 = 0;
                    //long itemId = 0;//任务详情表中是掉落物的id，但是后端告诉我的是 物品掉落的怪物id
                    //if (TaskManager.Single().TaskToSMGoodsDic.ContainsKey(taskItem.missionid))
                    //{
                    //    smonsterId1 = TaskManager.Single().TaskToSMGoodsDic[taskItem.missionid].opt6;
                    //    //杀怪掉落物 后端没有告诉我 需要去获得几个  只告诉我已经获得的数量
                    //    //if (TaskManager.Single().TaskSMGoodsCountDic.ContainsKey(smonsterId1))
                    //    //{
                    //    //    smGoodsCount = TaskManager.Single().TaskSMGoodsCountDic[smonsterId1];
                    //    //}
                    //}

                    //long trackingIndex = taskDataNode.Opt2;
                    //if (FSDataNodeTable<CollectNode>.GetSingleton().DataNodeList.ContainsKey(trackingIndex))
                    //{
                    //    itemId = FSDataNodeTable<CollectNode>.GetSingleton().DataNodeList[trackingIndex].collectid[0, 0];
                    //}

                    //taskData.content = taskItem.tasknode.Require.Replace("c1", GameLibrary.C1)
                    //                                             .Replace("c2", GameLibrary.C2)
                    //                                             .Replace("c3", GameLibrary.C3)
                    //                                             .Replace("c4", GameLibrary.C4)
                    //                                             .Replace("c5", GameLibrary.C5)
                    //                                             .Replace("c6", GameLibrary.C6)
                    //                                             .Replace("%d" + itemId, GetTaskGoodsCount(taskDataNode.Requiretype, itemId, taskItem).ToString());
                }
                else if (taskDataNode.Requiretype == 8)//背包物品数量显示
                {

                    long itemId1 = 0;
                    long itemId2 = 0;

                    Dictionary<long, int> idAndcountDic = taskDataNode.IdAndcountDic;
                    List<long> idArr = new List<long>();
                    foreach (long id in idAndcountDic.Keys)
                    {
                        idArr.Add(id);
                    }
                    if (idArr.Count >= 1)
                    {
                        itemId1 = idArr[0];
                    }
                    if (idArr.Count >= 2)
                    {
                        itemId2 = idArr[1];
                    }
                    taskData.content = taskItem.tasknode.Require.Replace("c1", GameLibrary.C1)
                                                                 .Replace("c2", GameLibrary.C2)
                                                                 .Replace("c3", GameLibrary.C3)
                                                                 .Replace("c4", GameLibrary.C4)
                                                                 .Replace("c5", GameLibrary.C5)
                                                                 .Replace("c6", GameLibrary.C6)
                                                                 .Replace("%d" + itemId1, GetTaskGoodsCount(taskDataNode.Requiretype, itemId1, taskItem).ToString())
                                                                 .Replace("%d" + itemId2, GetTaskGoodsCount(taskDataNode.Requiretype, itemId2, taskItem).ToString());
                }
                else
                {
                    taskData.content = taskItem.tasknode.Require.Replace("c1", GameLibrary.C1)
                                                                 .Replace("c2", GameLibrary.C2)
                                                                 .Replace("c3", GameLibrary.C3)
                                                                 .Replace("c4", GameLibrary.C4)
                                                                 .Replace("c5", GameLibrary.C5)
                                                                 .Replace("c6", GameLibrary.C6);
                }
            }

            taskData.taskState = taskItem.taskProgress;
            taskDataList.Add(taskData);

        }
        //添加其他任务信息
        GetRewardData();
        CreatTaskList();
    }
    /// <summary>
    /// 获取悬赏任务
    /// </summary>
    public void GetRewardData()
    {
        EveryTaskData data =
            playerData.GetInstance()
                .taskDataList.itList.Find(
                    x => x.state == (int)TaskProgress.Accept || x.state == (int)TaskProgress.Complete);
        if (data != null)
        {
            TaskData td = UIActivitiesManager.Instance.CreatTaskData(data);
            ChangeTaskList(td, TaskListHandleType.AddOrUpdata);
        }
        else
        {
            TaskData td = taskDataList.Find(x => x.taskType == TaskClass.Reward);
            ChangeTaskList(td, TaskListHandleType.Delet);
        }
    }
    public void CreatTaskList()
    {
        if (multList!=null&& taskDataList!=null)
        {
            multList.InSize(taskDataList.Count, 1);
            multList.Info(taskDataList.ToArray());
        }
    }

    string GetTaskTypeString(int type)
    {
        string str = "";
        if (type == 0)
        {
            str = "[主]";
        }
        else if (type > 0 && type < 10)
        {
            str = "[支]";
        }
        return str;
    }

    int GetTaskGoodsCount(int type, long id, TaskItem _taskItem)
    {
        int count = 0;
        switch (type)
        {
            case 1:
                break;
            case 2:
                break;
            case 3:
                if (TaskManager.Single().TaskItemCountsDic.ContainsKey(id))
                {
                    //if (TaskManager.Single().TaskItemCountsDic[id] < TaskManager.Single().TaskToCaijiDic[_taskItem.missionid].opt5)
                    //{
                    //    count = TaskManager.Single().TaskItemCountsDic[id];
                    //}
                    //else
                    //{
                    //    count = (int)TaskManager.Single().TaskToCaijiDic[_taskItem.missionid].opt5;
                    //}
                    count = TaskManager.Single().TaskItemCountsDic[id];
                    if (id == TaskManager.Single().TaskToCaijiDic[_taskItem.missionid].opt4)
                    {
                        if (count > TaskManager.Single().TaskToCaijiDic[_taskItem.missionid].opt5)
                        {
                            count = (int)TaskManager.Single().TaskToCaijiDic[_taskItem.missionid].opt5;
                        }
                    }
                    if (id == TaskManager.Single().TaskToCaijiDic[_taskItem.missionid].opt6)
                    {
                        if (count > TaskManager.Single().TaskToCaijiDic[_taskItem.missionid].opt7)
                        {
                            count = (int)TaskManager.Single().TaskToCaijiDic[_taskItem.missionid].opt7;
                        }
                    }
                }
                else
                {
                    count = 0;
                }
                break;
            case 4:
                break;
            case 5:
                break;
            case 6:
                if (TaskManager.Single().TaskSkillMonsterCountsDic.ContainsKey(id))
                {
                    //if (TaskManager.Single().TaskSkillMonsterCountsDic[id] < TaskManager.Single().TaskToSkillMonsterDic[_taskItem.missionid].opt5)
                    //{
                    //    count = TaskManager.Single().TaskSkillMonsterCountsDic[id];
                    //}
                    //else
                    //{
                    //    count = (int)TaskManager.Single().TaskToSkillMonsterDic[_taskItem.missionid].opt5;
                    //}
                    count = TaskManager.Single().TaskSkillMonsterCountsDic[id];
                    if (id == TaskManager.Single().TaskToSkillMonsterDic[_taskItem.missionid].opt4)
                    {
                        if (count > TaskManager.Single().TaskToSkillMonsterDic[_taskItem.missionid].opt5)
                        {
                            count = (int)TaskManager.Single().TaskToSkillMonsterDic[_taskItem.missionid].opt5;
                        }
                    }
                    if (id == TaskManager.Single().TaskToSkillMonsterDic[_taskItem.missionid].opt6)
                    {
                        if (count > TaskManager.Single().TaskToSkillMonsterDic[_taskItem.missionid].opt7)
                        {
                            count = (int)TaskManager.Single().TaskToSkillMonsterDic[_taskItem.missionid].opt7;
                        }
                    }
                }
                break;
            case 7:
                if (TaskManager.Single().TaskSMGoodsCountDic.ContainsKey(id))
                {
                    count = TaskManager.Single().TaskSMGoodsCountDic[id];
                    if (id == TaskManager.Single().TaskToSMGoodsDic[_taskItem.missionid].opt4)
                    {
                        if (count > TaskManager.Single().TaskToSMGoodsDic[_taskItem.missionid].opt5)
                        {
                            count = (int)TaskManager.Single().TaskToSMGoodsDic[_taskItem.missionid].opt5;
                        }
                    }
                    if (id == TaskManager.Single().TaskToSMGoodsDic[_taskItem.missionid].opt6)
                    {
                        if (count > TaskManager.Single().TaskToSMGoodsDic[_taskItem.missionid].opt7)
                        {
                            count = (int)TaskManager.Single().TaskToSMGoodsDic[_taskItem.missionid].opt7;
                        }
                    }
                }
                break;
            case 8:
                count = GoodsDataOperation.GetInstance().GetItemCountById(id);
                break;
            case 9:
                break;
        }
        return count;
    }

}
public class TaskData
{
    public string title = "";
    public string content = "";
    public TaskProgress taskState;
    public TaskItem taskItem;
    public TaskClass taskType;
}
