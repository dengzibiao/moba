/*
文件名（File Name）:   UITaskList.cs

作者（Author）:    #高#

创建时间（CreateTime）:  #CreateTime#
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tianyu;
using System;

public class UITaskList : GUIBase
{
    public static long npcid = 0;
    public GUISingleLabel npcname;
    public GUISingleLabel contont;
    public GUISingleLabel taskName;
    public GUISingleLabel taskContent;
    public GUISingleMultList multList;
    public UIScrollView goodsScrollView;
    public GUISingleMultList goodsMultList;
    //创建NPC父层级
    public Transform NPCModelParent;
    public int currentindex;
    public GameObject npcmodel;
    public GameObject backObj;
    public TaskDataNode tasknode;
    private UISprite nextSprite;
    private bool isBreath = false;
    GameObject HeroPosEmb = null;
    private GameObject heroObj;
    protected override void Init()
    {
        HeroPosEmb = GameObject.Find("HeroPosEmbattle");
        backObj = transform.Find("Mask").gameObject;
        goodsScrollView = transform.Find("GoodsListScrollView").GetComponent<UIScrollView>();
        goodsMultList = transform.Find("GoodsListScrollView/GoodsMultList").GetComponent<GUISingleMultList>();
        UIEventListener.Get(backObj).onClick += BtnOnClick;
        nextSprite = transform.Find("NextSprite").GetComponent<UISprite>();
    }
    void FixedUpdate()
    {
        StarBreathingLight();
    }
    public override UIPanleID GetUIKey()
    {
        return UIPanleID.UITaskList;
    }
    /// <summary>
    /// 呼吸灯效果
    /// </summary>
    void StarBreathingLight()
    {
        if (isBreath)
        {
            nextSprite.alpha += 0.02f;

            if (nextSprite.alpha >= 1)
            {
                isBreath = false;
            }
        }
        else
        {
            nextSprite.alpha -= 0.02f;

            if (nextSprite.alpha <= 0.3f)
            {
                isBreath = true;
            }
        }
    }
    private void BtnOnClick(GameObject go)
    {

        HeroPosEmb.transform.Find("NpcPos").gameObject.SetActive(false);
        if (heroObj != null)
        {
            Destroy(heroObj);
        }
        //Control.HideGUI(this.GetUIKey());
        ClientSendDataMgr.GetSingle().GetTaskSend().OpenDialogUI(
            TaskManager.Single().CurrentShowDialogItem.msId,
            currentindex,
            TaskManager.Single().CurrentShowDialogItem.user[0],
            0,
            TaskManager.Single().CurrentShowDialogItem.user[2],
            0
            );
        TaskManager.Single().isAcceptTask = true;
        OnClose();
        //发消息
        //TaskOperation.Single().SetCurrentTaskItem(taskItem);
    }
    protected override void SetUI(params object[] uiParams)
    {
        npcid = (long)uiParams[0];
        base.SetUI();
    }
    protected override void OnLoadData()
    {
        base.OnLoadData();
        //Singleton<Notification>.Instance.RegistMessageID(MessageID.common_open_mission_dialog_ret, UIPanleID.UITaskList);
        this.State = EnumObjectState.Ready;
        Show();
    }
    public override void ReceiveData(UInt32 messageID)
    {
        base.ReceiveData(messageID);
        switch (messageID)
        {
            case MessageID.common_open_mission_dialog_ret:
                //Show();
                //RefreshDialogueData();

                break;
        }
    }
    protected override void ShowHandler()
    {
        Control.HideGUI(UIPanleID.UIMoney);
        Control.HideGUI(UIPanleID.UIRole);
        Control.HideGUI(UIPanleID.UISetting);
        Control.HideGUI(UIPanleID.UITaskTracker);
        Control.HideGUI(UIPanleID.UIChat);
        //Control.HideGUI(GameLibrary.UIMail);
        GameObject.Destroy(npcmodel);
        //当前NPC身上所带任务列表
        //List<TaskItem> tasklist = TaskManager.Single().GetNPCTaskList(npcid);
        NPCNode npcNode = FSDataNodeTable<NPCNode>.GetSingleton().FindDataByType(npcid);
        ArrayList showTaskList = new ArrayList();

        //功能按钮
        for (int i = 0; i < TaskManager.Single().CurrentShowDialogItem.opt.Length; i++)
        {
            //是空就跳转 最后一个是关闭按钮
            if (string.IsNullOrEmpty(TaskManager.Single().CurrentShowDialogItem.opt[i]))
            {
                continue;//这个应该是return, 不然的话 下面 i+1就乱了
            }
            string[] functionbtn = new string[4];
            //任务索引要告诉后端选择的是哪个任务
            functionbtn[0] = (i + 1) + "";
            //任务名称
            functionbtn[1] = TaskManager.Single().CurrentShowDialogItem.opt[i];
            //区分完成状态
            functionbtn[2] = TaskManager.Single().CurrentShowDialogItem.user[1].ToString();//user2
            //区分主线支线
            functionbtn[3] = "1";
            showTaskList.Add(functionbtn);
        }

        ////关闭按钮
        //string[] strclose = new string[4];
        //strclose[0] = "-1";
        //strclose[1] = "路过而已";
        //strclose[2] = "1";
        //strclose[3] = "1";
        //showTaskList.Add(strclose);

        //multList.InSize(showTaskList.Count, 2);
        //multList.Info(showTaskList.ToArray());
        currentindex = int.Parse(((string[])showTaskList[0])[0]);
        if (TaskManager.Single().CurrentShowDialogItem!=null)
        {
            List<ItemData> itemlist = TaskManager.Single().GetItemList(TaskManager.Single().CurrentShowDialogItem.msId);
            goodsScrollView.ResetPosition();
            goodsMultList.InSize(itemlist.Count, itemlist.Count);
            goodsMultList.Info(itemlist.ToArray());
        }
        if (FSDataNodeTable<TaskDataNode>.GetSingleton().DataNodeList.ContainsKey(TaskManager.Single().CurrentShowDialogItem.msId))
        {
            tasknode = FSDataNodeTable<TaskDataNode>.GetSingleton().DataNodeList[TaskManager.Single().CurrentShowDialogItem.msId];
            taskName.text = tasknode.Taskname;
            taskContent.text = tasknode.Require.Replace("c1", GameLibrary.C1)
                                                         .Replace("c2", GameLibrary.C2)
                                                         .Replace("c3", GameLibrary.C3)
                                                         .Replace("c4", GameLibrary.C4)
                                                         .Replace("c5", GameLibrary.C5)
                                                         .Replace("c6", GameLibrary.C6);
            if (tasknode.Requiretype == 3)//采集数量显示
            {
                long collectID1 = 0;
                long collectID2 = 0;
                if (TaskManager.Single().TaskToCaijiDic.ContainsKey(TaskManager.Single().CurrentShowDialogItem.msId))
                {
                    collectID1 = TaskManager.Single().TaskToCaijiDic[TaskManager.Single().CurrentShowDialogItem.msId].opt4;
                    collectID2 = TaskManager.Single().TaskToCaijiDic[TaskManager.Single().CurrentShowDialogItem.msId].opt6;
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
                    if (tasknode.Opt5.Length > 0)
                    {
                        long trackingIndex = tasknode.Opt5[0];
                        if (FSDataNodeTable<CollectNode>.GetSingleton().DataNodeList.ContainsKey(trackingIndex))
                        {
                            collectID1 = FSDataNodeTable<CollectNode>.GetSingleton().DataNodeList[trackingIndex].collectid[0, 0];
                        }
                    }
                    if (tasknode.Opt5.Length > 1)
                    {
                        long trackingIndex = tasknode.Opt5[1];
                        if (FSDataNodeTable<CollectNode>.GetSingleton().DataNodeList.ContainsKey(trackingIndex))
                        {
                            collectID2 = FSDataNodeTable<CollectNode>.GetSingleton().DataNodeList[trackingIndex].collectid[0, 0];
                        }
                    }
                }
                taskContent.text = tasknode.Require.Replace("c1", GameLibrary.C1)
                                                             .Replace("c2", GameLibrary.C2)
                                                             .Replace("c3", GameLibrary.C3)
                                                             .Replace("c4", GameLibrary.C4)
                                                             .Replace("c5", GameLibrary.C5)
                                                             .Replace("c6", GameLibrary.C6)
                                                             .Replace("%d" + collectID1, "0")
                                                             .Replace("%d" + collectID2, "0");
            }
            else if (tasknode.Requiretype == 6)//杀怪数量显示
            {
                long monsterId1 = 0;
                long monsterId2 = 0;
                if (TaskManager.Single().TaskToSkillMonsterDic.ContainsKey(TaskManager.Single().CurrentShowDialogItem.msId))
                {
                    //暂时杀怪是两种 最多三种
                    monsterId1 = TaskManager.Single().TaskToSkillMonsterDic[TaskManager.Single().CurrentShowDialogItem.msId].opt4;
                    monsterId2 = TaskManager.Single().TaskToSkillMonsterDic[TaskManager.Single().CurrentShowDialogItem.msId].opt6;
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
                    Dictionary<long, int> idAndcountDic = tasknode.IdAndcountDic;
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
                taskContent.text = tasknode.Require.Replace("c1", GameLibrary.C1)
                                                             .Replace("c2", GameLibrary.C2)
                                                             .Replace("c3", GameLibrary.C3)
                                                             .Replace("c4", GameLibrary.C4)
                                                             .Replace("c5", GameLibrary.C5)
                                                             .Replace("c6", GameLibrary.C6)
                                                             .Replace("%d" + monsterId1,"0")
                                                             .Replace("%d" + monsterId2, "0");
            }
            else if (tasknode.Requiretype == 7)//杀怪掉落物显示
            {
                long itemId1 = 0;
                long itemId2 = 0;
                if (TaskManager.Single().TaskToSMGoodsDic.ContainsKey(TaskManager.Single().CurrentShowDialogItem.msId))
                {

                    itemId1 = TaskManager.Single().TaskToSMGoodsDic[TaskManager.Single().CurrentShowDialogItem.msId].opt4;
                    itemId2 = TaskManager.Single().TaskToSMGoodsDic[TaskManager.Single().CurrentShowDialogItem.msId].opt6;
                }

                long trackingIndex = tasknode.Opt2;
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

                taskContent.text = tasknode.Require.Replace("c1", GameLibrary.C1)
                                                             .Replace("c2", GameLibrary.C2)
                                                             .Replace("c3", GameLibrary.C3)
                                                             .Replace("c4", GameLibrary.C4)
                                                             .Replace("c5", GameLibrary.C5)
                                                             .Replace("c6", GameLibrary.C6)
                                                             .Replace("%d" + itemId1, "0")
                                                             .Replace("%d" + itemId2, "0");
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
            else if (tasknode.Requiretype == 8)//背包物品数量显示
            {
                long itemId1 = 0;
                long itemId2 = 0;

                Dictionary<long, int> idAndcountDic =tasknode.IdAndcountDic;
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
                taskContent.text = tasknode.Require.Replace("c1", GameLibrary.C1)
                                                             .Replace("c2", GameLibrary.C2)
                                                             .Replace("c3", GameLibrary.C3)
                                                             .Replace("c4", GameLibrary.C4)
                                                             .Replace("c5", GameLibrary.C5)
                                                             .Replace("c6", GameLibrary.C6)
                                                             .Replace("c6", GameLibrary.C6)
                                                             .Replace("%d" + itemId1, "0")
                                                             .Replace("%d" + itemId2, "0");
            }
        }
        
        //根据NPC 查找内容与名称
        npcname.text = npcNode.npcname;
        //contont.text = TaskManager.Single().CurrentShowDialogItem.disc;
        if (FSDataNodeTable<ModelNode>.GetSingleton().DataNodeList.ContainsKey(npcNode.modelid))
        {
            //CrearteNpcModel(FSDataNodeTable<ModelNode>.GetSingleton().DataNodeList[npcNode.modelid].respath);
            heroObj = HeroPosEmbattle.instance.CreatModelByModelID((int)npcNode.modelid, PosType.NpcPos, transform.Find("HeroTexture").GetComponent<SpinWithMouse>(), MountAndPet.Null, 160);
        }
    }

    //创建npc模型
    void CrearteNpcModel(string modelid)
    {
       // string tem = GameLibrary.NPC_URL + modelid;
       // if(FSDataNodeTable<ModelNode>.GetSingleton().DataNodeList.ContainsKey(modelid))
      //  ModelNode modelNode = FSDataNodeTable<ModelNode>.GetSingleton().FindDataByType(modelid);
        npcmodel = Instantiate(Resources.Load(modelid)) as GameObject;
        if (npcmodel != null)
        {
            npcmodel.transform.parent = NPCModelParent;
            npcmodel.transform.localPosition = Vector3.zero;
            npcmodel.transform.localScale = Vector3.one;
            npcmodel.transform.localRotation = Quaternion.Euler(Vector3.zero);
            SkinnedMeshRenderer[] skinnedMeshRenderer = npcmodel.GetComponentsInChildren<SkinnedMeshRenderer>();
            for (int i = 0; i < skinnedMeshRenderer.Length; i++)
            {
                skinnedMeshRenderer[i].gameObject.layer = 5;
            }
        }
    }
    public void OnClose()
    {
        Control.ShowGUI(UIPanleID.UIMoney, EnumOpenUIType.DefaultUIOrSecond);
        Control.ShowGUI(UIPanleID.UIRole, EnumOpenUIType.DefaultUIOrSecond);
        Control.ShowGUI(UIPanleID.UISetting, EnumOpenUIType.DefaultUIOrSecond);
        Control.ShowGUI(UIPanleID.UITaskTracker, EnumOpenUIType.DefaultUIOrSecond);
        Control.ShowGUI(UIPanleID.UIChat, EnumOpenUIType.DefaultUIOrSecond);
        //Control.ShowGUI(GameLibrary.UIMail);
        Control.HideGUI(this.GetUIKey());
        //UIPanleID[] arr = new UIPanleID[] { UIPanleID.UIMoney, UIPanleID.UIRole, UIPanleID.UISetting, UIPanleID.UI_TaskTracker, UIPanleID.UIChat, UIPanleID.UIMail, UIPanleID.UITaskEffectPanel };
        //Control.ShowGUI(arr, EnumOpenUIType.DefaultUIOrSecond);
    }

}
