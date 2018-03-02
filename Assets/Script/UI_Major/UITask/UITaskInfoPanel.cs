using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tianyu;

public class UITaskInfoPanel : GUIBase
{
    public static TaskItem clickTaskItem;

    public GUISingleButton closebtn;
    public GUISingleSprite taskstatespr;//标记主线还是支线任务
    public GUISingleLabel tasknamelab;//任务名称
    public GUISingleLabel taskcontontlab;//任务内容

    public GUISingleLabel tasktargetlab1;//任务目标1
    public GUISingleLabel tasktargetlab2;//任务目标2
    public GUISingleLabel tasktargetlab3;//任务目标3

    public GUISingleMultList mulitlist;

    public UIItemTips uiitemtips;
    private UIScrollView scrollView;
    protected override void OnLoadData()
    {
        base.OnLoadData();
        this.State = EnumObjectState.Ready;
        Show();
    }
    public override void ReceiveData(uint messageID)
    {
        base.ReceiveData(messageID);
        switch (messageID)
        {
            //case MessageID.common_offer_reward_mission_list_ret:
            //  Show(); break;
        }
    }
    protected override void ShowHandler()
    {
        ShowTaskInfo();
    }
    public override UIPanleID GetUIKey()
    {
        return UIPanleID.UITaskInfoPanel;
    }
    protected override void Init()
    {
        closebtn.onClick = OnClose;
        scrollView = transform.Find("GoodsListScrollView").GetComponent<UIScrollView>();
    }

    private void OnClose()
    {
        //Control.ShowGUI(GameLibrary.UI_Money);
        //Control.ShowGUI(GameLibrary.UIRole);
        //Control.ShowGUI(GameLibrary.UISetting);
        //Control.ShowGUI(GameLibrary.UI_TaskTracker);
        // Hide();
        Control.HideGUI(this.GetUIKey());
    }

    private void ShowTaskInfo()
    {
        if (clickTaskItem.tasknode.Type == 0)
        {
            taskstatespr.spriteName = "zhu";
        }
        else
        {
            taskstatespr.spriteName = "zhi";
        }
        taskstatespr.MakePixelPerfect();
        tasknamelab.text = clickTaskItem.tasknode.Taskname;
        taskcontontlab.text = clickTaskItem.tasknode.Taskinfo;
        tasktargetlab1.text = clickTaskItem.tasknode.Require.Replace("c1", GameLibrary.C1).Replace("c2", GameLibrary.C2).Replace("c3", GameLibrary.C3).Replace("c4", GameLibrary.C4).Replace("c5", GameLibrary.C5).Replace("c6", GameLibrary.C6).Replace("%d", TaskManager.Single().caijiCount.ToString());
        if (clickTaskItem.tasknode.Requiretype == 3)//采集数量显示
        {
            long collectID1 = 0;
            long collectID2 = 0;
            if (TaskManager.Single().TaskToCaijiDic.ContainsKey(clickTaskItem.missionid))
            {
                collectID1 = TaskManager.Single().TaskToCaijiDic[clickTaskItem.missionid].opt4;
                collectID2 = TaskManager.Single().TaskToCaijiDic[clickTaskItem.missionid].opt6;
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
                if (clickTaskItem.tasknode.Opt5.Length > 0)
                {
                    long trackingIndex = clickTaskItem.tasknode.Opt5[0];
                    if (FSDataNodeTable<CollectNode>.GetSingleton().DataNodeList.ContainsKey(trackingIndex))
                    {
                        collectID1 = FSDataNodeTable<CollectNode>.GetSingleton().DataNodeList[trackingIndex].collectid[0, 0];
                    }
                }
                if (clickTaskItem.tasknode.Opt5.Length > 1)
                {
                    long trackingIndex = clickTaskItem.tasknode.Opt5[1];
                    if (FSDataNodeTable<CollectNode>.GetSingleton().DataNodeList.ContainsKey(trackingIndex))
                    {
                        collectID2 = FSDataNodeTable<CollectNode>.GetSingleton().DataNodeList[trackingIndex].collectid[0, 0];
                    }
                }
            }
            tasktargetlab1.text = clickTaskItem.tasknode.Require.Replace("c1", GameLibrary.C1)
                                                         .Replace("c2", GameLibrary.C2)
                                                         .Replace("c3", GameLibrary.C3)
                                                         .Replace("c4", GameLibrary.C4)
                                                         .Replace("c5", GameLibrary.C5)
                                                         .Replace("c6", GameLibrary.C6)
                                                         .Replace("%d" + collectID1, GetTaskGoodsCount(clickTaskItem.tasknode.Requiretype, collectID1, clickTaskItem).ToString())
                                                         .Replace("%d" + collectID2, GetTaskGoodsCount(clickTaskItem.tasknode.Requiretype, collectID2, clickTaskItem).ToString());
        }
        else if (clickTaskItem.tasknode.Requiretype == 6)//杀怪数量显示
        {
            long monsterId1 = 0;
            long monsterId2 = 0;
            if (TaskManager.Single().TaskToSkillMonsterDic.ContainsKey(clickTaskItem.missionid))
            {
                //暂时杀怪是两种 最多三种
                monsterId1 = TaskManager.Single().TaskToSkillMonsterDic[clickTaskItem.missionid].opt4;
                monsterId2 = TaskManager.Single().TaskToSkillMonsterDic[clickTaskItem.missionid].opt6;
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
                Dictionary<long, int> idAndcountDic = clickTaskItem.tasknode.IdAndcountDic;
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
                else if (idArr.Count ==1)
                {
                    monsterId1 = idArr[0];
                }

            }
            tasktargetlab1.text = clickTaskItem.tasknode.Require.Replace("c1", GameLibrary.C1)
                                                         .Replace("c2", GameLibrary.C2)
                                                         .Replace("c3", GameLibrary.C3)
                                                         .Replace("c4", GameLibrary.C4)
                                                         .Replace("c5", GameLibrary.C5)
                                                         .Replace("c6", GameLibrary.C6)
                                                         .Replace("%d" + monsterId1, GetTaskGoodsCount(clickTaskItem.tasknode.Requiretype, monsterId1, clickTaskItem).ToString())
                                                         .Replace("%d" + monsterId2, GetTaskGoodsCount(clickTaskItem.tasknode.Requiretype, monsterId2, clickTaskItem).ToString());
        }
        else if (clickTaskItem.tasknode.Requiretype == 7)//杀怪掉落物显示
        {
            long itemId1 = 0;
            long itemId2 = 0;
            if (TaskManager.Single().TaskToSMGoodsDic.ContainsKey(clickTaskItem.missionid))
            {

                itemId1 = TaskManager.Single().TaskToSMGoodsDic[clickTaskItem.missionid].opt4;
                itemId2 = TaskManager.Single().TaskToSMGoodsDic[clickTaskItem.missionid].opt6;
            }

            long trackingIndex = clickTaskItem.tasknode.Opt2;
            if (FSDataNodeTable<CollectNode>.GetSingleton().DataNodeList.ContainsKey(trackingIndex))
            {
                if (FSDataNodeTable<CollectNode>.GetSingleton().DataNodeList[trackingIndex].collectid.GetLength(0) >= 2)
                {
                    itemId1 = FSDataNodeTable<CollectNode>.GetSingleton().DataNodeList[trackingIndex].collectid[0, 0];
                    itemId2 = FSDataNodeTable<CollectNode>.GetSingleton().DataNodeList[trackingIndex].collectid[1, 0];
                }
                else if (FSDataNodeTable<CollectNode>.GetSingleton().DataNodeList[trackingIndex].collectid.GetLength(0) ==1)
                {
                    itemId1 = FSDataNodeTable<CollectNode>.GetSingleton().DataNodeList[trackingIndex].collectid[0, 0];
                }
            }

            tasktargetlab1.text = clickTaskItem.tasknode.Require.Replace("c1", GameLibrary.C1)
                                                         .Replace("c2", GameLibrary.C2)
                                                         .Replace("c3", GameLibrary.C3)
                                                         .Replace("c4", GameLibrary.C4)
                                                         .Replace("c5", GameLibrary.C5)
                                                         .Replace("c6", GameLibrary.C6)
                                                         .Replace("%d" + itemId1, GetTaskGoodsCount(clickTaskItem.tasknode.Requiretype, itemId1, clickTaskItem).ToString())
                                                         .Replace("%d" + itemId2, GetTaskGoodsCount(clickTaskItem.tasknode.Requiretype, itemId2, clickTaskItem).ToString());
            //long smonsterId1 = 0;
            //long itemId = 0;//任务详情表中是掉落物的id，但是后端告诉我的是 物品掉落的怪物id
            //if (TaskManager.Single().TaskToSMGoodsDic.ContainsKey(clickTaskItem.missionid))
            //{
            //    smonsterId1 = TaskManager.Single().TaskToSMGoodsDic[clickTaskItem.missionid].opt6;
            //    //杀怪掉落物 后端没有告诉我 需要去获得几个  只告诉我已经获得的数量
            //    //if (TaskManager.Single().TaskSMGoodsCountDic.ContainsKey(smonsterId1))
            //    //{
            //    //    smGoodsCount = TaskManager.Single().TaskSMGoodsCountDic[smonsterId1];
            //    //}
            //}

            //long trackingIndex = clickTaskItem.tasknode.Opt2;
            //if (FSDataNodeTable<CollectNode>.GetSingleton().DataNodeList.ContainsKey(trackingIndex))
            //{
            //    itemId = FSDataNodeTable<CollectNode>.GetSingleton().DataNodeList[trackingIndex].collectid[0, 0];
            //}

            //tasktargetlab1.text = clickTaskItem.tasknode.Require.Replace("c1", GameLibrary.C1)
            //                                             .Replace("c2", GameLibrary.C2)
            //                                             .Replace("c3", GameLibrary.C3)
            //                                             .Replace("c4", GameLibrary.C4)
            //                                             .Replace("c5", GameLibrary.C5)
            //                                             .Replace("c6", GameLibrary.C6)
            //                                             .Replace("%d" + itemId, GetTaskGoodsCount(clickTaskItem.tasknode.Requiretype, itemId, clickTaskItem).ToString());
        }
        else if (clickTaskItem.tasknode.Requiretype == 8)//背包物品数量显示
        {
            long itemId1 = 0;
            long itemId2 = 0;

            Dictionary<long, int> idAndcountDic = clickTaskItem.tasknode.IdAndcountDic;
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
                itemId1 = idArr[0];
                itemId2 = idArr[1];
            }
            tasktargetlab1.text = clickTaskItem.tasknode.Require.Replace("c1", GameLibrary.C1)
                                                         .Replace("c2", GameLibrary.C2)
                                                         .Replace("c3", GameLibrary.C3)
                                                         .Replace("c4", GameLibrary.C4)
                                                         .Replace("c5", GameLibrary.C5)
                                                         .Replace("c6", GameLibrary.C6)
                                                         .Replace("c6", GameLibrary.C6)
                                                         .Replace("%d" + itemId1, GetTaskGoodsCount(clickTaskItem.tasknode.Requiretype, itemId1, clickTaskItem).ToString())
                                                         .Replace("%d" + itemId2, GetTaskGoodsCount(clickTaskItem.tasknode.Requiretype, itemId2, clickTaskItem).ToString());
        }

        tasktargetlab2.text = "";
        tasktargetlab3.text = "";
        //List<ItemData> itemlist = TaskManager.Single().GetItemList(clickTaskItem.tasknode.Reward_prop);
        List<ItemData> itemlist = TaskManager.Single().GetItemList(clickTaskItem.tasknode.Taskid);
        mulitlist.InSize(itemlist.Count, itemlist.Count);
        mulitlist.Info(itemlist.ToArray());
        scrollView.ResetPosition();
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
