using System;
using UnityEngine;
using System.Collections.Generic;
using Tianyu;
public class CMarqueeHandle : CHandleBase
{

    public CMarqueeHandle(CHandleMgr mgr)
        : base(mgr)
    {
    }

    public override void RegistAllHandle()
    {
        RegistHandle(MessageID.common_notice_common_ret, MarqueeHandle);
    }
    public bool KeepAliveHandle(CReadPacket packet)
    {

        return true;
    }
    public bool MarqueeHandle(CReadPacket packet)
    {
        Debug.Log("MarqueeHandle 跑马灯");
        Dictionary<string, object> data = packet.data;

        int resolt = int.Parse(data["ret"].ToString());
        if (resolt == 0)
        {
            if (null != data)
            {
                if (int.Parse(data["opt2"].ToString()) == 2)//1：跑马灯 2 :任务 3:播放任务接收成功特效4，悬赏
                {
                    Debug.Log("类型" + int.Parse(data["opt2"].ToString()) + "___" + "任务id" + int.Parse(data["opt3"].ToString()));

                   
                    TaskDataNode taskDataNode = FSDataNodeTable<TaskDataNode>.GetSingleton().FindDataByType(int.Parse(data["opt3"].ToString()));
                    if (taskDataNode != null)
                    {
                        if (taskDataNode.Requiretype == 9)
                        {
                            Debug.Log("展示使用道具面板");
                            //UITaskUseItemPanel.Instance.SetTaskID(int.Parse(data["opt3"].ToString()), TaskClass.Main);
                            //Control.ShowGUI(GameLibrary.UITaskUseItemPanel);
                            object[] tempObj = new object[] { long.Parse(data["opt3"].ToString()) , TaskClass.Main };
                            Control.ShowGUI(UIPanleID.UITaskUseItemPanel, EnumOpenUIType.DefaultUIOrSecond, false, tempObj);
                        }
                        else if (taskDataNode.Requiretype == 3)
                        {
                            if (TaskManager.Single().TaskToCaijiDic.ContainsKey(int.Parse(data["opt3"].ToString())))
                            {
                                //long collectID = TaskManager.Single().TaskToCaijiDic[int.Parse(data["opt3"].ToString())].opt4;
                                long collectID = long.Parse(data["user1"].ToString());
                                if (TaskManager.Single().TaskItemCountsDic.ContainsKey(collectID))
                                {
                                    TaskManager.Single().TaskItemCountsDic[collectID] = TaskManager.Single().TaskItemCountsDic[collectID] + 1;
                                }
                                else
                                {
                                    TaskManager.Single().TaskItemCountsDic.Add(collectID, 1);
                                }
                            }
                        }
                        else if (taskDataNode.Requiretype == 6)
                        {
                            if (TaskManager.Single().TaskToSkillMonsterDic.ContainsKey(int.Parse(data["opt3"].ToString())))
                            {
                                long monsterId1 = TaskManager.Single().TaskToSkillMonsterDic[int.Parse(data["opt3"].ToString())].opt4;
                                long monsterId2 = TaskManager.Single().TaskToSkillMonsterDic[int.Parse(data["opt3"].ToString())].opt6;
                                if (int.Parse(data["user1"].ToString()) == monsterId1)
                                {
                                    if (TaskManager.Single().TaskSkillMonsterCountsDic.ContainsKey(monsterId1))
                                    {
                                        //如果杀怪数量满足 不再增加
                                        if (TaskManager.Single().TaskSkillMonsterCountsDic[monsterId1] < TaskManager.Single().TaskToSkillMonsterDic[int.Parse(data["opt3"].ToString())].opt5)
                                        {
                                            TaskManager.Single().TaskSkillMonsterCountsDic[monsterId1] = TaskManager.Single().TaskSkillMonsterCountsDic[monsterId1] + 1;
                                        }
                                        
                                    }
                                    else
                                    {
                                        TaskManager.Single().TaskSkillMonsterCountsDic.Add(monsterId1, 1);
                                    }
                                }
                                if (int.Parse(data["user1"].ToString()) == monsterId2)
                                {
                                    if (TaskManager.Single().TaskSkillMonsterCountsDic.ContainsKey(monsterId2))
                                    {
                                        //如果杀怪数量满足 不再增加
                                        if (TaskManager.Single().TaskSkillMonsterCountsDic[monsterId2] < TaskManager.Single().TaskToSkillMonsterDic[int.Parse(data["opt3"].ToString())].opt7)
                                        {
                                            TaskManager.Single().TaskSkillMonsterCountsDic[monsterId2] = TaskManager.Single().TaskSkillMonsterCountsDic[monsterId2] + 1;
                                        } 
                                    }
                                    else
                                    {
                                        TaskManager.Single().TaskSkillMonsterCountsDic.Add(monsterId2, 1);
                                    }
                                }
                               
                            }
                        }
                        else if (taskDataNode.Requiretype == 7)
                        {
                            if (TaskManager.Single().TaskToSMGoodsDic.ContainsKey(int.Parse(data["opt3"].ToString())))
                            {
                                long itemId1 = TaskManager.Single().TaskToSMGoodsDic[int.Parse(data["opt3"].ToString())].opt4;
                                long itemId2 = TaskManager.Single().TaskToSMGoodsDic[int.Parse(data["opt3"].ToString())].opt6;
                                if (int.Parse(data["user1"].ToString()) == itemId1)
                                {
                                    if (TaskManager.Single().TaskSMGoodsCountDic.ContainsKey(itemId1))
                                    {
                                        //掉落物数量满足 不再增加
                                        if (TaskManager.Single().TaskSMGoodsCountDic[itemId1]< TaskManager.Single().TaskToSMGoodsDic[int.Parse(data["opt3"].ToString())].opt5)
                                        {
                                            TaskManager.Single().TaskSMGoodsCountDic[itemId1] = TaskManager.Single().TaskSMGoodsCountDic[itemId1] + 1;
                                        }
                                        
                                    }
                                    else
                                    {
                                        TaskManager.Single().TaskSMGoodsCountDic.Add(itemId1, 1);
                                    }
                                }
                                if (int.Parse(data["user1"].ToString()) == itemId2)
                                {
                                    if (TaskManager.Single().TaskSMGoodsCountDic.ContainsKey(itemId2))
                                    {
                                        //掉落物数量满足 不再增加
                                        if (TaskManager.Single().TaskSMGoodsCountDic[itemId2]< TaskManager.Single().TaskToSMGoodsDic[int.Parse(data["opt3"].ToString())].opt7)
                                        {
                                            TaskManager.Single().TaskSMGoodsCountDic[itemId2] = TaskManager.Single().TaskSMGoodsCountDic[itemId2] + 1;
                                        }
                                    }
                                    else
                                    {
                                        TaskManager.Single().TaskSMGoodsCountDic.Add(itemId2, 1);
                                    }
                                }
                                
                            }
                        }
                        //Control.ShowGUI(GameLibrary.UITaskTracker);
                    }

                    //if (int.Parse(data["opt3"].ToString()) ==507 )
                    //{

                    //    //if (TaskManager.NpcTaskListDic.ContainsKey(102))
                    //    //{
                    //    //    int taskID = 0;
                    //    //    foreach (int key in TaskManager.NpcTaskListDic[102].Keys)
                    //    //    {
                    //    //        taskID = key;
                    //    //    }
                    //    //    ClientSendDataMgr.GetSingle().GetTaskSend().ClickNpc(102, taskID, TaskManager.NpcTaskListDic[102][taskID].parm0);
                    //    //}


                    //    //ClientSendDataMgr.GetSingle().GetTaskSend().ClickNpc(102, 507, TaskManager.NpcTaskListDic[102][507].parm0);
                    //}
                }
                else if (int.Parse(data["opt2"].ToString()) == 3)
                {
                    //TaskEffectManager.instance.ShowTaskEffect(TaskEffectType.AcceptTask);
                    UITaskEffectPanel.instance.ShowTaskEffect(TaskEffectType.AcceptTask);
                    UIDialogue.instance.ShowTaskHidePanel();
                }
                if (int.Parse(data["opt2"].ToString()) == 4)
                {
                  // Singleton<SceneManage>.Instance.MessageHandle(EnumSceneID.UI_MajorCity01,GameLibrary.UITaskTracker, EnumSceneID.LGhuangyuan);
                   // UITaskTracker.instance.GetRewardData();
                }
                if (int.Parse(data["opt2"].ToString()) == 1)
                {
                    MarqueeData marqueeData = new MarqueeData();
                    marqueeData.opt1 = int.Parse(data["opt1"].ToString());
                    marqueeData.opt2 = int.Parse(data["opt2"].ToString());
                    marqueeData.opt3 = int.Parse(data["opt3"].ToString());

                    marqueeData.user1 = data["user1"].ToString();
                    marqueeData.user2 = data["user2"].ToString();
                    marqueeData.user3 = data["user3"].ToString();

                    if (!playerData.GetInstance().marqueeListDic.ContainsKey(marqueeData.opt3))
                    {
                        List<MarqueeData> list = new List<MarqueeData>();
                        list.Add(marqueeData);
                        playerData.GetInstance().marqueeListDic.Add(marqueeData.opt3, list);

                    }
                    else
                    {
                        //  List<MarqueeData> list = null;
                        // list.Add(marqueeData);
                        // playerData.GetInstance().marqueeListDic.TryGetValue(marqueeData.opt3, out list);//out从字典里取出来
                        // list.Add(marqueeData);
                        playerData.GetInstance().marqueeListDic[marqueeData.opt3].Add(marqueeData);
                    }
                    if (Singleton<SceneManage>.Instance.Current == EnumSceneID.UI_MajorCity01 || Singleton<SceneManage>.Instance.Current == EnumSceneID.LGhuangyuan)
                    {
                        Control.ShowGUI(UIPanleID.UIMarquee,EnumOpenUIType.DefaultUIOrSecond);
                    }

                }

            }
            return true;
        }
        else
        {
            //   Debug.Log(string.Format("返回信息失败：{0}", data["desc"].ToString()));
            Debug.Log("跑马灯返回消息失败");
            return false;
        }
        
    }
}
