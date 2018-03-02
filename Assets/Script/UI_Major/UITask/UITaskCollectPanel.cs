using UnityEngine;
using System.Collections;

public enum TaskProgressBarType
{
    Null,
    Collect,//采集
    SendLetter,//送信
}

/// <summary>
/// 可用于采集 送信进度条
/// </summary>
public class UITaskCollectPanel : GUIBase {

    public GUISingleProgressBar progressBar;
    public UILabel operateName;
    private string operateNameS;
    private float dutiaoTime;
    private long collectgoodsID;
    private bool isStart = false;
    private TaskProgressBarType taskPBType;
    private FubenTaskData fubenTaskData;
    private TaskClass taskClass;

    private static UITaskCollectPanel instance;

    public static UITaskCollectPanel Instance { get { return instance; } set { instance = value; } }

    public UITaskCollectPanel()
    {
        instance = this;
    }
    public override UIPanleID GetUIKey()
    {
        return UIPanleID.UITaskCollectPanel;
    }
    protected override void Init()
    {

        progressBar = transform.Find("ProgressBar").GetComponent<GUISingleProgressBar>();
        operateName = transform.Find("OperateName").GetComponent<UILabel>();
    }
    private void ClearData()
    {
        collectgoodsID = 0;
        isStart = false;
        taskPBType = TaskProgressBarType.Null;
        fubenTaskData = null;
    }
    public void SetData(string name,float timer,long collectID,TaskProgressBarType taskPBType,FubenTaskData data,TaskClass taskclass)
    {
        operateNameS = name;
        dutiaoTime = timer;
        this.collectgoodsID = collectID;
        this.taskPBType = taskPBType;
        fubenTaskData = data;
        taskClass = taskclass;
    }
    protected override void SetUI(params object[] uiParams)
    {
        operateNameS = (string)uiParams[0];
        dutiaoTime = (float)uiParams[1];
        this.collectgoodsID = (long)uiParams[2];
        this.taskPBType = (TaskProgressBarType)uiParams[3];
        fubenTaskData = (FubenTaskData)uiParams[4];
        taskClass = (TaskClass)uiParams[5];
        base.SetUI();
    }
    protected override void OnLoadData()
    {
        base.OnLoadData();
        this.State = EnumObjectState.Ready;
        Show();
    }
    protected override void ShowHandler()
    {
        base.ShowHandler();
        progressBar.maxValue = 100;
        progressBar.currentValue = 0;
        operateName.text = operateNameS;
        isStart = true;
    }

    void Update()
    {
        if (isStart)
        {
            progressBar.currentValue += 1 / dutiaoTime * Time.deltaTime * progressBar.maxValue;
            //progressBar.currentValue += 1 / 2.0f * Time.deltaTime * progressBar.maxValue;
            if (progressBar.currentValue >= progressBar.maxValue)
            {
                progressBar.currentValue = progressBar.maxValue;
                isStart = false;
                if (this.taskPBType == TaskProgressBarType.Collect)
                {
                    for (int i = 0; i < playerData.GetInstance().taskDataList.itList.Count; i++)
                    {
                        if (playerData.GetInstance().taskDataList.itList[i].type == (int) TaskType.Collect)
                        {
                          long id=  UIActivitiesManager.Instance.GetCollectItemID(
                                playerData.GetInstance().taskDataList.itList[i].taskTarget);
                            if (id == collectgoodsID)
                            {
                                ClientSendDataMgr.GetSingle().GetTaskSend().SendCompleteGatherTask(C2SMessageType.Active, i, collectgoodsID, 1, 1);//采集一次 就获得一个采集物品
                                TaskManager.Single().isCollecting = false;
                            }
                           
                        }
                    }

                    foreach (int id in TaskManager.Single().TaskToCaijiDic.Keys)
                    {
                        if (TaskManager.Single().TaskToCaijiDic[id].opt4 == collectgoodsID)
                        {
                            //if (TaskManager.Single().TaskItemCountsDic.ContainsKey(collectgoodsID))
                            //{
                            //    Debug.LogError(TaskManager.Single().TaskItemCountsDic[collectgoodsID] + "...." + TaskManager.Single().TaskToCaijiDic[id].opt5);
                            //    //有采集任务，但是采集数量已经够了 也不进行采集
                            //    if (TaskManager.Single().TaskItemCountsDic[collectgoodsID] < TaskManager.Single().TaskToCaijiDic[id].opt5)
                            //    {
                            //        Debug.Log("再次采集：发送采集协议");
                            //        ClientSendDataMgr.GetSingle().GetTaskSend().SendCompleteGatherTask(C2SMessageType.Active, TaskManager.Single().TaskToCaijiDic[id].taskId, TaskManager.Single().TaskToCaijiDic[id].opt4, 1);//采集一次 就获得一个采集物品
                            //    }
                            //    else
                            //    {
                            //        Debug.Log("采集数量已满足");
                            //    }
                            //}
                            //else//还没有采集数量的时候肯定发
                            //{
                            //    Debug.Log("第一次采集：发送采集协议");
                            //    ClientSendDataMgr.GetSingle().GetTaskSend().SendCompleteGatherTask(C2SMessageType.Active, TaskManager.Single().TaskToCaijiDic[id].taskId, TaskManager.Single().TaskToCaijiDic[id].opt4, 1);//采集一次 就获得一个采集物品
                            //}
                            Debug.Log("第一次采集：发送采集协议");
                            ClientSendDataMgr.GetSingle().GetTaskSend().SendCompleteGatherTask(C2SMessageType.Active, TaskManager.Single().TaskToCaijiDic[id].taskId, TaskManager.Single().TaskToCaijiDic[id].opt4, 1,0);//采集一次 就获得一个采集物品
                            TaskManager.Single().isCollecting = false;

                        }
                        if (TaskManager.Single().TaskToCaijiDic[id].opt6 == collectgoodsID)
                        {
                            Debug.Log("第一次采集：发送采集协议");
                            ClientSendDataMgr.GetSingle().GetTaskSend().SendCompleteGatherTask(C2SMessageType.Active, TaskManager.Single().TaskToCaijiDic[id].taskId, TaskManager.Single().TaskToCaijiDic[id].opt6, 1,0);//采集一次 就获得一个采集物品
                            TaskManager.Single().isCollecting = false;
                        }
                    }
                }
                else if (this.taskPBType == TaskProgressBarType.SendLetter)
                {
                    if (taskClass == TaskClass.Main)
                    {
                        if (fubenTaskData != null)
                        {
                            if (TaskManager.NpcTaskListDic.ContainsKey((int)fubenTaskData.opt5))
                            {
                                int taskID = 0;
                                foreach (int key in TaskManager.NpcTaskListDic[(int)fubenTaskData.opt5].Keys)
                                {
                                    taskID = key;
                                }
                                Debug.Log("发送给予信件协议");
                                ClientSendDataMgr.GetSingle().GetTaskSend().ClickNpc((int)fubenTaskData.opt5, taskID, TaskManager.NpcTaskListDic[(int)fubenTaskData.opt5][taskID].parm0, 0);
                            }
                        }
                    }
                    else if (taskClass == TaskClass.Reward)
                    {
                        EveryTaskData data = playerData.GetInstance().taskDataList.itList.Find(x => (TaskType)x.type == TaskType.NamedPComplete);
                        if (data != null)
                        {
                            for (int i = 0; i < playerData.GetInstance().taskDataList.itList.Count; i++)
                            {
                                if (playerData.GetInstance().taskDataList.itList[i].state == (int)TaskProgress.Accept)
                                {
                                    ClientSendDataMgr.GetSingle().GetTaskSend().ClickNpc(0, 0, 0, 2);
                                }
                            }
                        }
                    }  
                }
                Control.HideGUI(this.GetUIKey());
                //Hide();
                ClearData();
            }
        }
    }
}
