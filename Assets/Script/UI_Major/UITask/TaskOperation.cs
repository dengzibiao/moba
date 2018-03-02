using UnityEngine;
using System.Collections;
using Tianyu;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

//任务行为操作
public class TaskOperation
{
    private static TaskOperation single;

    public static TaskOperation Single()
    {
        if (single == null)
        {
            single = new TaskOperation();
        }
        return single;
    }
    //当前操作的任务
    private TaskItem taskitem;

    //对话任务使用
    public TaskNPC taskNpc;
    public long currentTaskNpcID;
    

    //移动到NPC执行事件
    public enum MoveToNpcType
    {
        JuQingTaskZhuiZhong,
        OpenTaskList,
        OpenChatDialog,
        Smalltalk,
        OpenUseItemPanel,
        RewardSendLetter,
    }
    public void SetCurrentTaskItem(TaskItem _taskitem)
    {
        taskitem = _taskitem;
        //如果是主线对话任务
        //if ((TaskClass)taskitem.tasknode.Type == TaskClass.Main && taskitem.tasknode.Requiretype == 1 && taskitem.taskProgress == TaskProgress.Complete)
        //{
        //    //MoveToNpc(_taskitem.tasknode.Acceptnpc, MoveToNpcType.GetRewards);
        //    MoveToNpc(_taskitem.npcid, MoveToNpcType.GetRewards);
        //}
        //else if((TaskClass)taskitem.tasknode.Type == TaskClass.Main && taskitem.tasknode.Requiretype == 1)
        //{
        //    //MoveToNpc(_taskitem.tasknode.Acceptnpc, MoveToNpcType.OpenChatDialog);
        //    MoveToNpc(_taskitem.npcid, MoveToNpcType.OpenChatDialog);
        //}
        //else if ((TaskClass)_taskitem.tasknode.Type == TaskClass.Branch)
        //{

        //}
        //else
        //{

        //}
        //主线任务 并且是剧情任务
        if (taskitem.taskindex == 0 && taskitem.tasknode.Requiretype == 1)
        {
            //
            if (taskitem.taskProgress == TaskProgress.NoAccept || taskitem.taskProgress == TaskProgress.Accept || taskitem.taskProgress == TaskProgress.Complete)
            {
                MoveToNpc(_taskitem.npcid, MoveToNpcType.JuQingTaskZhuiZhong);
                //MveToNpc(_taskitem.npcid, MoveToNpcType.OpenChatDialog);
                //MoveToNpc(_taskitem.npcid, MoveToNpcType.JuQingTaskZhuiZhong);
            }
        }
        else if (taskitem.taskindex == 0 && taskitem.tasknode.Requiretype == 2)//主线任务 并且是副本任务
        {
            if (taskitem.taskProgress == TaskProgress.NoAccept || taskitem.taskProgress == TaskProgress.Complete)//没接 ---> 跑到npc去接任务  完成跑过去找npc交任务
            {
                MoveToNpc(_taskitem.npcid, MoveToNpcType.JuQingTaskZhuiZhong);
            }
            else if (taskitem.taskProgress == TaskProgress.Accept && TaskManager.Single().TaskToFubenDic.ContainsKey(_taskitem.missionid))//已接并且有副本地址信息 --->打开副本面板去通关副本
            {
                if (Singleton<SceneManage>.Instance.Current == EnumSceneID.UI_MajorCity01)
                {
                    //在自动寻路过程中，点击追踪打开副本界面 就关闭掉任务追踪
                    TaskAutoTraceManager._instance.StopTaskAutoFindWay();
                    OpenFubenPanel(_taskitem);
                }
                else
                {
                    Control.ShowGUI(UIPanleID.UITooltips, EnumOpenUIType.DefaultUIOrSecond, false, "野外无法进入副本");
                }
                
            }


        }
        else if (taskitem.taskindex == 0 && taskitem.tasknode.Requiretype == 3)//主线任务 并且是采集任务
        {
            if (taskitem.taskProgress == TaskProgress.NoAccept || taskitem.taskProgress == TaskProgress.Complete)//没接 ---> 跑到npc去接任务  完成跑过去找npc交任务
            {
                MoveToNpc(_taskitem.npcid, MoveToNpcType.JuQingTaskZhuiZhong);
            }
            else if (taskitem.taskProgress == TaskProgress.Accept && TaskManager.Single().TaskToCaijiDic.ContainsKey(_taskitem.missionid))//已接并且有副本地址信息 --->打开副本面板去杀怪
            {
                OpenFubenPanelToCaiji(_taskitem);
            }
        }
        else if (taskitem.taskindex == 0 && taskitem.tasknode.Requiretype == 6)//主线任务 并且是杀怪任务
        {
            if (taskitem.taskProgress == TaskProgress.NoAccept || taskitem.taskProgress == TaskProgress.Complete)//没接 ---> 跑到npc去接任务  完成跑过去找npc交任务
            {
                MoveToNpc(_taskitem.npcid, MoveToNpcType.JuQingTaskZhuiZhong);
            }
            else if (taskitem.taskProgress == TaskProgress.Accept && TaskManager.Single().TaskToSkillMonsterDic.ContainsKey(_taskitem.missionid))//已接并且有副本地址信息 --->打开副本面板去杀怪
            {
                OpenFubenPanelToShaguai(_taskitem);
            }
        }
        else if(taskitem.taskindex == 0 && taskitem.tasknode.Requiretype == 7)//主线任务 并且是杀怪掉落物任务
        {
            if (taskitem.taskProgress == TaskProgress.NoAccept || taskitem.taskProgress == TaskProgress.Complete)//没接 ---> 跑到npc去接任务  完成跑过去找npc交任务
            {
                MoveToNpc(_taskitem.npcid, MoveToNpcType.JuQingTaskZhuiZhong);
            }
            else if (taskitem.taskProgress == TaskProgress.Accept && TaskManager.Single().TaskToSMGoodsDic.ContainsKey(_taskitem.missionid))//已接并且有副本地址信息 --->打开副本面板去杀怪
            {
                ToGetMonsterDropGoods(_taskitem);
            }
        }
        else if (taskitem.taskindex == 0 && taskitem.tasknode.Requiretype == 9)//主线任务 并且是指定地点使用它道具任务
        {
            if (taskitem.taskProgress == TaskProgress.NoAccept || taskitem.taskProgress == TaskProgress.Complete)//没接 ---> 跑到npc去接任务  完成跑过去找npc交任务
            {
                MoveToNpc(_taskitem.npcid, MoveToNpcType.JuQingTaskZhuiZhong);
            }
            else if (taskitem.taskProgress == TaskProgress.Accept && TaskManager.Single().TaskToTargetUseItemDic.ContainsKey(_taskitem.missionid))//已接并且有副本地址信息 --->打开副本面板去杀怪
            {
                ToTargetPosUseItem(_taskitem);
            }
        }
        else if (taskitem.taskindex == 0 && taskitem.tasknode.Requiretype == 8)//主线任务 并且是背包物品任务
        {
            if (taskitem.taskProgress == TaskProgress.NoAccept || taskitem.taskProgress == TaskProgress.Complete)//没接 ---> 跑到npc去接任务  完成跑过去找npc交任务
            {
                MoveToNpc(_taskitem.npcid, MoveToNpcType.JuQingTaskZhuiZhong);
            }
            else if (taskitem.taskProgress == TaskProgress.Accept && TaskManager.Single().TaskToTargetUseItemDic.ContainsKey(_taskitem.missionid))//已接并且有副本地址信息 --->打开副本面板去杀怪
            {
                //ToTargetPosUseItem(_taskitem);
            }
        }

    }
    public void ToTargetPosUseItem(TaskItem _taskitem)
    {
        Debug.Log("到指定地点使用相关道具");

        //通过TaskToTargetUseItemDic找到目标位置 然后自动追踪跑道npc位置  给服务器发送我的位置  然后根据TaskToTargetUseItemDic弹出道具框内容，点击道具框按钮给服务器发送点击npc协议（相关参数通过npc上的任务），服务器告诉我任务状态 我更新任务
        MoveToNpc(_taskitem.npcid,MoveToNpcType.OpenUseItemPanel);//暂时先这样处理 之后修改  读表读出响应的位置然后跑过去
        //ClientSendDataMgr.GetSingle().GetTaskSend().SendIsShowItem(C2SMessageType.Active, _taskitem.missionid, 1.108f, 0.079f);
    }

    //去相应的怪物地点杀怪 获得怪物掉落物
    public void ToGetMonsterDropGoods(TaskItem _taskitem)
    {
        Debug.Log("去获得怪物掉落物");
        //去的怪物地址(只会在野外) 通过采集物品的id 去的怪物地点 然后自动寻路的怪物点去杀怪获得掉落物

        TaskAutoTraceManager._instance.MoveToTargetPosition(_taskitem.tasknode.Opt2,TaskMoveTarget.MoveToMonsterDropPis);
        //TaskAutoTraceManager._instance.SetReachplaceMonsterDropEvent(MoveToMonsterDropEvent);

        //if (TaskManager.Single().TaskToSMGoodsDic.ContainsKey(_taskitem.missionid))
        //{
        //    Debug.Log("怪物掉落物收集完毕");
        //    //发送响应的协议
        //    //现默认杀怪成功
        //    ClientSendDataMgr.GetSingle().GetTaskSend().SendTaskSkillMonster(C2SMessageType.Active, TaskManager.Single().TaskToSMGoodsDic[_taskitem.missionid].taskId, TaskManager.Single().TaskToSMGoodsDic[_taskitem.missionid].opt6, 1);
        //}
    }

    //杀怪任务目前只杀野外怪物
    public void OpenFubenPanelToShaguai(TaskItem _taskitem)
    {
        int mapId = 0;
        Debug.Log("去野外杀怪喽");
        if (TaskManager.Single().TaskToSkillMonsterDic.ContainsKey(_taskitem.missionid))//杀怪任务 可能去野外 可能去副本 如果是副本直接跳转副本界面 如果是野外(怪物id，移动类型 到时候在传递一个mapid) 
        {
            if (FSDataNodeTable<TaskDataNode>.GetSingleton().DataNodeList.ContainsKey(_taskitem.missionid))
            {
                mapId = FSDataNodeTable<TaskDataNode>.GetSingleton().DataNodeList[_taskitem.missionid].MapID;
            }
            TaskAutoTraceManager._instance.MoveToTargetPosition(TaskManager.Single().TaskToSkillMonsterDic[_taskitem.missionid].opt4, TaskMoveTarget.MoveToMonsterPos,mapId);
        }

        ////杀怪完成 要检测是否有杀怪任务（TaskToFubenDic），然后杀怪数量满足 然后给服务器发送消息 让服务器验证
        //if (TaskManager.Single().TaskToSkillMonsterDic.ContainsKey(_taskitem.missionid))
        //{
        //    Debug.Log("杀怪完成");
        //    //先默认杀怪成功 给服务器发送副本成功的协议
        //    ClientSendDataMgr.GetSingle().GetTaskSend().SendTaskSkillMonster(C2SMessageType.Active, TaskManager.Single().TaskToSkillMonsterDic[_taskitem.missionid].taskId, TaskManager.Single().TaskToSkillMonsterDic[_taskitem.missionid].opt4,1);
        //    ClientSendDataMgr.GetSingle().GetTaskSend().SendTaskSkillMonster(C2SMessageType.Active, TaskManager.Single().TaskToSkillMonsterDic[_taskitem.missionid].taskId, TaskManager.Single().TaskToSkillMonsterDic[_taskitem.missionid].opt6,1);
        //    // ClientSendDataMgr.GetSingle().GetTaskSend().SendTaskSkillMonster(C2SMessageType.Active, TaskManager.Single().TaskToSkillMonsterDic[_taskitem.missionid].taskId, TaskManager.Single().TaskToSkillMonsterDic[_taskitem.missionid].opt4, TaskManager.Single().TaskToSkillMonsterDic[_taskitem.missionid].opt5);
        //    //ClientSendDataMgr.GetSingle().GetTaskSend().SendTaskSkillMonster(C2SMessageType.Active, TaskManager.Single().TaskToSkillMonsterDic[_taskitem.missionid].taskId, TaskManager.Single().TaskToSkillMonsterDic[_taskitem.missionid].opt6, TaskManager.Single().TaskToSkillMonsterDic[_taskitem.missionid].opt7);
        //    PropertyManager.Instance._taskitem = _taskitem;
        //}
    }
    //杀怪完成 检测玩家是否有杀怪任务 和 杀怪掉落物任务（杀死怪物后要调的接口）
    public void SkillMonsterToTestTask(int monsterId)
    {
        foreach (FubenTaskData tem in TaskManager.Single().TaskToSkillMonsterDic.Values)
        {
            if (tem.opt4 == monsterId)
            {
                ClientSendDataMgr.GetSingle().GetTaskSend().SendTaskSkillMonster(C2SMessageType.Active, tem.taskId, monsterId, 1);//杀死一只发数量1 然后马灯消息回复成功 我将响应的杀怪数量+1 任务完成后 后端会推送任务状态更新
            }
            if(tem.opt6 == monsterId)
            {
                ClientSendDataMgr.GetSingle().GetTaskSend().SendTaskSkillMonster(C2SMessageType.Active, tem.taskId, monsterId, 1);
            }
        }

        foreach (FubenTaskData tem in TaskManager.Single().TaskToSMGoodsDic.Values)
        {
            if (tem.opt6 == monsterId)
            {
                ClientSendDataMgr.GetSingle().GetTaskSend().SendTaskSkillMonster(C2SMessageType.Active, tem.taskId, monsterId, 1);
            }

        }
    }
    //去指定的地点去采集
    public void OpenFubenPanelToCaiji(TaskItem _taskitem)
    {
        if (_taskitem.tasknode.Opt5.Length > 0)
        {
            Debug.Log("去指定地点采集喽" + _taskitem.tasknode.Opt5[0]);
            //去的采集信息 通过任务id找到任务详情表中的任务索引 通过采集物品的id 去的采集地点 然后自动寻路的采集点去采集TODO

            TaskAutoTraceManager._instance.MoveToTargetPosition(_taskitem.tasknode.Opt5[0], TaskMoveTarget.MoveToCollectPos);//现在写死跑到指定的采集点
            //TaskAutoTraceManager._instance.SetReachplaceCollectPointEvent(MoveToCollectPointEvent);
        }
        ////采集完成 要检测是否有采集任务（TaskToFubenDic），然后采集数量满足 然后给服务器发送消息 让服务器验证
        //if (TaskManager.Single().TaskToCaijiDic.ContainsKey(_taskitem.missionid))
        //{
        //    Debug.Log("采集完成");
        //    int taskid = TaskManager.Single().TaskToCaijiDic[_taskitem.missionid].taskId;
        //    long itemid = TaskManager.Single().TaskToCaijiDic[_taskitem.missionid].opt4;
        //    long count = TaskManager.Single().TaskToCaijiDic[_taskitem.missionid].opt5;
        //    //先默认采集成功 给服务器发送副本成功的协议
        //    ClientSendDataMgr.GetSingle().GetTaskSend().SendCompleteGatherTask(C2SMessageType.Active, TaskManager.Single().TaskToCaijiDic[_taskitem.missionid].taskId, TaskManager.Single().TaskToCaijiDic[_taskitem.missionid].opt4, TaskManager.Single().TaskToCaijiDic[_taskitem.missionid].opt5);
        //}

    }
    //采集成功 检测玩家是否有采集的任务 （采集完成要掉的接口）
    public void GatherSucceedToTestCaijiTask(int itemId)
    {
        foreach (FubenTaskData tem in TaskManager.Single().TaskToCaijiDic.Values)
        {
            if (tem.opt4 == itemId)
            {
                //然后查询背包中 该采集物品的数量 如果数量满足 发给服务器 
                int count = 0;//背包中该采集物品的数量
                if(count == tem.opt5)
                {
                    ClientSendDataMgr.GetSingle().GetTaskSend().SendCompleteGatherTask(C2SMessageType.Active, tem.taskId, itemId, count,0);
                }
            }
            if (tem.opt6 == itemId)
            {
                //然后查询背包中 该采集物品的数量 如果数量满足 发给服务器 
                int count = 0;//背包中该采集物品的数量
                if (count == tem.opt7)
                {
                    ClientSendDataMgr.GetSingle().GetTaskSend().SendCompleteGatherTask(C2SMessageType.Active, tem.taskId, itemId, count,0);
                }
            }
        }
    }

    //打开相应的副本界面去通关
    public void OpenFubenPanel(TaskItem _taskitem)
    {
        Debug.Log("去通关副本" + TaskManager.Single().TaskToFubenDic[_taskitem.missionid].opt4 + "章" + TaskManager.Single().TaskToFubenDic[_taskitem.missionid].opt5 + "关卡");
        //(Control.ShowGUI(GameLibrary.UILevel) as UILevel).ShowParam((int)TaskManager.Single().TaskToFubenDic[_taskitem.missionid].opt4, (int)TaskManager.Single().TaskToFubenDic[_taskitem.missionid].opt5);
        object[] openParams = new object[] { OpenLevelType.ByIDOpen, (int)TaskManager.Single().TaskToFubenDic[_taskitem.missionid].opt5 };
        Control.ShowGUI(UIPanleID.UILevel, EnumOpenUIType.OpenNewCloseOld, false, openParams);
        //if (TaskManager.Single().TaskToFubenDic.ContainsKey(_taskitem.missionid))
        //{
        //    Debug.Log("副本通关完成");
        //    //先默认副本成功 给服务器发送副本成功的协议
        //    ClientSendDataMgr.GetSingle().GetTaskSend().SendCompleteCopyTask(C2SMessageType.Active, TaskManager.Single().TaskToFubenDic[_taskitem.missionid].taskId, TaskManager.Single().TaskToFubenDic[_taskitem.missionid].opt5, 1);
        //}

    }
    /// <summary>
    /// 进入副本 检测玩家是否有需要创建临时角色的任务 返回任务id
    /// </summary>
    /// <param name="fubenId">副本id</param>
    /// <returns></returns>
    public int IntoFubenGetTaskID(int fubenId)
    {
        if (fubenId == 0)
        {
            return 0;
        }
        int taskId = 0;
        foreach (FubenTaskData tem in TaskManager.Single().TaskToFubenDic.Values)
        {
            if (tem.opt5 == fubenId && TaskManager.MainTask.missionid == tem.taskId && TaskManager.MainTask.taskProgress == TaskProgress.Accept )
            {
                TaskDataNode taskDataNode;
                if (FSDataNodeTable<TaskDataNode>.GetSingleton().DataNodeList.ContainsKey(tem.taskId))
                {
                    taskDataNode = FSDataNodeTable<TaskDataNode>.GetSingleton().DataNodeList[tem.taskId];
                    if (taskDataNode.Place1 != 0|| taskDataNode.Place2 != 0)
                    {
                        taskId = tem.taskId;
                        break;
                    }
                }
            }
        }
        return taskId;
    }
    /// <summary>
    /// 通关副本 检测玩家是否有通关该副本的任务  (副本通关后要调的接口)
    /// </summary>
    /// <param name="fubenId">副本id</param>
    public void PassFubenToTestFubenTask(int fubenId)
    {
        if (fubenId == 0)
        {
            return;
        }
        //杀怪完成检测是否有该副本的任务(TaskToFubenDic） 然后副本通关完成  然后给服务器发送副本通关的协议 让服务器验证
        foreach(FubenTaskData tem in TaskManager.Single().TaskToFubenDic.Values)
         {
            if(tem.opt5 == fubenId)
            {
                Debug.Log("副本通关完成");
                //先默认副本成功 给服务器发送副本成功的协议
                ClientSendDataMgr.GetSingle().GetTaskSend().SendCompleteCopyTask(C2SMessageType.Active, tem.taskId, tem.opt5, 1);//1是成功通关副本 也只有成功才给服务器发，不成功不发
            }
         }

    }

    //移动到NPC位置 设置执行事件
    public void MoveToNpc(long Npcid, MoveToNpcType moveToNpcType)
    {
        //GameObject npcObj = NPCManager.GetSingle().GetNpcObj(Npcid);//现在不能这样使用了，因为npc是动态创建的，可能取得时候不存在
        //if (npcObj != null)
        //{
        //    taskNpc = npcObj.GetComponent<TaskNPC>();
        //}
        if (currentTaskNpcID != Npcid)
        {
            currentTaskNpcID = Npcid;
        }
        clearEvent();
        TaskAutoTraceManager._instance.MoveToTargetPosition(Npcid, TaskMoveTarget.MoveToNpc);
        switch (moveToNpcType)
        {
            case MoveToNpcType.OpenTaskList:
                TaskAutoTraceManager._instance.SetReachplaceNpcEvent(OpenTaskList);
                break;
            case MoveToNpcType.OpenChatDialog:
                TaskAutoTraceManager._instance.SetReachplaceNpcEvent(OpenChatDialog);
                break;
            case MoveToNpcType.Smalltalk:
                TaskAutoTraceManager._instance.SetReachplaceNpcEvent(Smalltalk);
                break;
            case MoveToNpcType.JuQingTaskZhuiZhong:
                TaskAutoTraceManager._instance.SetReachplaceNpcEvent(JuQingTaskZhuiZhong);
                
                break;
            case MoveToNpcType.OpenUseItemPanel:
                TaskAutoTraceManager._instance.SetReachplaceNpcEvent(OpenUseItemPanel);
                break;
            case MoveToNpcType.RewardSendLetter:
                TaskAutoTraceManager._instance.SetReachplaceNpcEvent(RewardOpenUseItemPanel);
                break;
            default:
                break;
        }
    }
    public void clearEvent()
    {
        TaskManager.Single().reachplaceNpcEvent = null;
        TaskManager.Single().reachplaceTransferEvent = null;
    }
    public void MoveToMonsterDropEvent()
    {
        Debug.Log("移动到怪物掉落物点 开始自动杀怪");
    }
    public void MoveToCollectPointEvent()
    {
        Debug.Log("移动到采集点 开始自动采集");
    }
    //移动到指定位置 采集 杀怪 （因为这两个任务移动到指定位置后 杀怪和采集交给玩家自己操作 不同于npc）
    public void MoveToAssignPosition(int id)
    {

    }
    //用于任务追踪触发点击npc协议  各种任务都有可能:接任务或者交任务或者领取奖励
    private void JuQingTaskZhuiZhong(long npcid)
    {
        if (TaskManager.NpcTaskListDic.ContainsKey(npcid))
        {
            int taskID = 0;
            foreach (int key in TaskManager.NpcTaskListDic[npcid].Keys)
            {
                taskID = key;
            }
            ClientSendDataMgr.GetSingle().GetTaskSend().ClickNpc(npcid, taskID, TaskManager.NpcTaskListDic[npcid][taskID].parm0,0);
        }
        //ClientSendDataMgr.GetSingle().GetTaskSend().ClickNpc(npcid, npcid, npcid);
    }
    private void RewardOpenUseItemPanel(long npcid)
    {
        EveryTaskData data = playerData.GetInstance().taskDataList.itList.Find(x => (TaskType)x.type == TaskType.NamedPComplete);
        if (data != null)
        {
            for (int i = 0; i < playerData.GetInstance().taskDataList.itList.Count; i++)
            {
                if (playerData.GetInstance().taskDataList.itList[i].state == (int)TaskProgress.Accept)
                {
                    data = playerData.GetInstance().taskDataList.itList[i];
                    Debug.Log("展示使用道具面板");
                    //UITaskUseItemPanel.Instance.SetTaskID(data.useprops_id, TaskClass.Reward);
                    //Control.ShowGUI(GameLibrary.UITaskUseItemPanel);
                    object[] tempObj = new object[] { data.useprops_id, TaskClass.Reward };
                    Control.ShowGUI(UIPanleID.UITaskUseItemPanel, EnumOpenUIType.DefaultUIOrSecond, false, tempObj);
                    return;
                }
            }
        }

    }
    //用于任务追踪触发打开使用任务道具面板  
    private void OpenUseItemPanel(long npcid)
    {
        if (TaskManager.NpcTaskListDic.ContainsKey(npcid))
        {
            int taskID = 0;
            foreach (int key in TaskManager.NpcTaskListDic[npcid].Keys)
            {
                taskID = key;
            }
            //ClientSendDataMgr.GetSingle().GetTaskSend().ClickNpc(npcid, taskID, TaskManager.NpcTaskListDic[npcid][taskID].parm0);
            //ClientSendDataMgr.GetSingle().GetTaskSend().SendIsShowItem(C2SMessageType.Active, taskID, 1.108f, 0.079f);
            ClientSendDataMgr.GetSingle().GetTaskSend().SendIsShowItem(C2SMessageType.Active, taskID, TaskAutoTraceManager._instance.targetPosition.x, TaskAutoTraceManager._instance.targetPosition.z,0);
        } 
    }


    //打开聊天界面
    private void OpenChatDialog(long npcid)
    {
        //if (TaskManager.Single().CurrentShowDialogItem != null)
        //{
        //    if (FSDataNodeTable<TaskDataNode>.GetSingleton().DataNodeList.ContainsKey(TaskManager.Single().CurrentShowDialogItem.msId))
        //    {
        //        if (FSDataNodeTable<TaskDataNode>.GetSingleton().DataNodeList[TaskManager.Single().CurrentShowDialogItem.msId].Requiretype == 1)
        //        {
        //            UIDialogue.instance.SetData(false, true);//不是闲聊  是对话任务
        //        }
        //        else
        //        {
        //            UIDialogue.instance.SetData(false, false); // 不是闲聊 不是对话任务
        //        }
        //    }
        //}
        //UIDialogue.contonts.Clear();
        //UIDialogue.npcid = npcid;
        //UIDialogue.contonts.Add(TaskManager.Single().CurrentShowDialogItem.disc);
        //if (Control.GetGUI(GameLibrary.UI_Dialogue).gameObject.activeSelf)
        //{
        //    UIDialogue.instance.RefreshDialogueData();
        //}
        //else
        //{
        //    Control.ShowGUI(GameLibrary.UI_Dialogue);
        //}
        //UIDialogue.OnChatFinishEvent = EditTaskOnChatFinish;

        UIDialogue.contonts.Clear();
        TaskManager.Single().contonts.Clear();
        if (TaskManager.Single().CurrentShowDialogItem != null)
        {
            if (FSDataNodeTable<TaskDataNode>.GetSingleton().DataNodeList.ContainsKey(TaskManager.Single().CurrentShowDialogItem.msId))
            {
                if (FSDataNodeTable<TaskDataNode>.GetSingleton().DataNodeList[TaskManager.Single().CurrentShowDialogItem.msId].Requiretype == 1)
                {
                    //UIDialogue.instance.SetData(false, true);//不是闲聊  是对话任务
                    TaskManager.Single().isSmalltalk = false;
                    TaskManager.Single().isDialogueTask = true;
                    TaskManager.Single().npcid = npcid;
                    TaskManager.Single().contonts.Add(TaskManager.Single().CurrentShowDialogItem.disc);
                    //object[] temObj = new object[] { false, true, npcid, TaskManager.Single().CurrentShowDialogItem.disc };
                    if (UIDialogue.instance != null&& UIDialogue.instance.IsShow())
                    {

                        UIDialogue.instance.RefreshDialogueData();
                    }
                    else
                    {
                        //Control.HideGUI(true);//需要在打开之前关闭所有的界面
                        Control.ShowGUI(UIPanleID.UIDialogue, EnumOpenUIType.DefaultUIOrSecond);
                    }
                    

                }
                else
                {
                    //UIDialogue.instance.SetData(false, false); // 不是闲聊 不是对话任务
                    TaskManager.Single().isSmalltalk = false;
                    TaskManager.Single().isDialogueTask = false;
                    TaskManager.Single().npcid = npcid;
                    TaskManager.Single().contonts.Add(TaskManager.Single().CurrentShowDialogItem.disc);
                    //object[] temObj = new object[] { false, false, npcid, TaskManager.Single().CurrentShowDialogItem.disc };
                    //Control.ShowGUI(UIPanleID.UIDialogue, EnumOpenUIType.DefaultUIOrSecond, false, temObj);
                    if (UIDialogue.instance != null&&UIDialogue.instance.IsShow())
                    {

                        UIDialogue.instance.RefreshDialogueData();
                    }
                    else
                    {
                        //Control.HideGUI(true);//需要在打开之前关闭所有的界面
                        Control.ShowGUI(UIPanleID.UIDialogue, EnumOpenUIType.DefaultUIOrSecond);
                    }
                    
                }
            }
        }
        UIDialogue.OnChatFinishEvent = EditTaskOnChatFinish;
    }

    //闲时对话
    private void Smalltalk(long npcid)
    {


        //UIDialogue.instance.SetData(true,false);//是闲聊 不是对话任务

        //NPCNode npcNode = FSDataNodeTable<NPCNode>.GetSingleton().FindDataByType(npcid);
        //if (npcNode !=null)
        //{
        //    int desindex = Random.Range(0, npcNode.info.Length);
        //    UIDialogue.contonts.Add(npcNode.info[desindex]);
        //}

        ////for (int i = 0; i < npcNode.info.Length; i++)
        ////{
        ////    if (string.IsNullOrEmpty(npcNode.info[i]))
        ////    {
        ////        continue;
        ////    }
        ////    UIDialogue.contonts.Add(npcNode.info[i]);
        ////}
        ////if (npcid == 103&&SceneManager.GetActiveScene().name == GameLibrary.UI_Major)
        ////{

        ////    UIPromptBox.Instance.ShowLabel("想去野外吗？来吧！");
        ////    Control.ShowGUI(GameLibrary.UIPromptBox);

        ////}
        //UIDialogue.npcid = npcid;
        //Control.ShowGUI(GameLibrary.UI_Dialogue);
        //UIDialogue.OnChatFinishEvent = null;

        TaskManager.Single().contonts.Clear();
        NPCNode npcNode = FSDataNodeTable<NPCNode>.GetSingleton().FindDataByType(npcid);
        if (npcNode != null)
        {
            int desindex = Random.Range(0, npcNode.info.Length);
            TaskManager.Single().isSmalltalk = true;
            TaskManager.Single().isDialogueTask = false;
            TaskManager.Single().npcid = npcid;
            TaskManager.Single().contonts.Add(npcNode.info[desindex]);
            //object[] temObj = new object[] { true, false, npcid, npcNode.info[desindex]};
            //Control.HideGUI(true);//需要在打开之前关闭所有的界面
            Control.ShowGUI(UIPanleID.UIDialogue, EnumOpenUIType.DefaultUIOrSecond);
        }
        UIDialogue.OnChatFinishEvent = null;

    }

    //打开任务界面
    private void OpenTaskList(long npcid)
    {
        //发消息

        //UITaskList.npcid = npcid;
        //Control.ShowGUI(GameLibrary.UI_TaskList);
        //Control.HideGUI(true);//需要在打开之前关闭所有的界面
        Control.ShowGUI(UIPanleID.UITaskList, EnumOpenUIType.DefaultUIOrSecond, false, npcid);
    }

    private void EditTaskOnChatFinish(long npcid)
    {
        ClientSendDataMgr.GetSingle().GetTaskSend().OpenDialogUI(
              TaskManager.Single().CurrentShowDialogItem.msId,
              0,
              TaskManager.Single().CurrentShowDialogItem.user[0],
              0,//跳过1 不跳过0
              TaskManager.Single().CurrentShowDialogItem.user[2],
              0
              );

        //在这里检测对话完毕设置任务状态
        //如果未接受任务 且是对话任务 接受对话完直接制成任务完成状态
        //if (taskitem.taskProgress == TaskProgress.NoAccept)
        //{
        //    if (taskitem.tasknode.Requiretype == 1)
        //    {
        //        taskitem.taskProgress = TaskProgress.Complete;
        //        TaskManager.Single().ModifeTask(taskitem, TaskModifyType.Change);
        //    }
        //}
        ////如果任务完成弹出任务奖励界面
        //else if (taskitem.taskProgress == TaskProgress.Complete)
        //{
        //    if (taskitem.tasknode.Requiretype == 1)
        //    {
        //        taskitem.taskProgress = TaskProgress.Reward;

        //        //任务就奖励显示
        //        TaskDataNode taskDataNode = FSDataNodeTable<TaskDataNode>.GetSingleton().FindDataByType(taskitem.missionid);
        //        List<ItemData> itemlist = TaskManager.Single().GetItemList(taskDataNode.Reward_prop);
        //        UITaskRewardPanel.itemlist.AddRange(itemlist);

        //        //任务奖励弹窗
        //        Control.ShowGUI(GameLibrary.UITaskRewardPanel);
        //        TaskManager.Single().ModifeTask(taskitem, TaskModifyType.Remove);
        //    }
        //}
    }
   
}
