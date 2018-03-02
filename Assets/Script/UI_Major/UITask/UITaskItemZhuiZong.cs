using UnityEngine;
using System.Collections;
using Tianyu;
using System.Collections.Generic;

public class UITaskItemZhuiZong : GUISingleItemList
{
    public GUISingleButton taskInfoButton;
    public UISprite task_state;
    public UILabel task_biaoti;
    public UILabel task_contont;
    public GameObject bg;
    private Transform taskEffect;
    private Transform taskStateEffect;
    private TaskItem taskItem;
    private TaskData taskData;
    public int caijiCount = 0;
    public int monster1Count = 0;
    public int monster2Count = 0;
    public int smGoodsCount = 0;//杀怪掉落物的数量
    private Vector3 vec1 = new Vector3(-92f,25f,0f);
    private Vector3 vec2 = new Vector3(-92f,16f,0f);
    

    protected override void InitItem()
    {
        taskInfoButton.onClick = ShowTaskInfoPanel;
        bg.GetComponent<GUISingleButton>().onClick = QianWang;
        taskEffect = transform.Find("rwwcktx");
        taskStateEffect = transform.Find("state/UI_RWTX_TanHao_01");
    }
    public override void Info(object obj)
    {
        if (obj != null)
        {
            taskData = (TaskData)obj;
            taskItem = taskData.taskItem;
        }
        
    }
    protected override void ShowHandler()
    {


        task_biaoti.text = taskData.title;
        task_contont.text = taskData.content;
        switch (taskData.taskState)
        {
            case TaskProgress.CantAccept:
                task_state.spriteName = "tanhao-hui";
                task_contont.text = "";
                task_biaoti.transform.localPosition = vec2;
                taskStateEffect.gameObject.SetActive(false);
                break;
            case TaskProgress.NoAccept:
                task_state.spriteName = "tanhao-jin";
                task_contont.text = "";
                task_biaoti.transform.localPosition = vec2;
                taskStateEffect.gameObject.SetActive(true);
                break;
            case TaskProgress.Accept:
                task_state.spriteName = "wenhao-hui";
                task_biaoti.transform.localPosition = vec1;
                taskStateEffect.gameObject.SetActive(false);
                break;
            case TaskProgress.Complete:
                task_state.spriteName = "wenhao-jin";
                task_biaoti.transform.localPosition = vec1;
                taskStateEffect.gameObject.SetActive(true);
                break;
            case TaskProgress.Reward:
                task_state.spriteName = "";
                task_contont.text = "";
                task_biaoti.transform.localPosition = vec2;
                taskStateEffect.gameObject.SetActive(false);
                break;
            default:
                break;
        }
        task_state.MakePixelPerfect();
        if (taskData.taskType != TaskClass.Reward && taskData.taskItem != null && taskData.taskItem.tasknode != null)
        {
            if (taskData.taskItem.tasknode.Task_effects == 1)
            {
                taskEffect.gameObject.SetActive(true);
            }
            else
            {
                taskEffect.gameObject.SetActive(false);
            }
        }
        if (index == 0)
        {
            if (NextGuidePanel.Single() != null && !NextGuidePanel.Single().isInit)
            {
                NextGuidePanel.Single().Init();
                NextGuidePanel.Single().isInit = true;
                Debug.Log("init nextgui");
            }
        }

    }
    void ShowTaskInfoPanel()
    {
        if (taskData.taskType == TaskClass.Reward)
        {
            if (Singleton<SceneManage>.Instance.Current == EnumSceneID.UI_MajorCity01)
            {
                Control.ShowGUI(UIPanleID.UIActivities, EnumOpenUIType.OpenNewCloseOld,false, 1);
            }
        }
        else
        {
            UITaskInfoPanel.clickTaskItem = taskItem;
            //这里设置 显示任务
            //Control.ShowGUI(GameLibrary.UITaskInfoPanel);
            Control.ShowGUI(UIPanleID.UITaskInfoPanel, EnumOpenUIType.DefaultUIOrSecond);
        }
    }

    void QianWang()
    {
        //如果当前玩家正在采集或者送信 读进度条就返回
        TaskOperation.Single().clearEvent();
        if (taskData.taskType == TaskClass.Reward)
        {
            if (Singleton<SceneManage>.Instance.Current == EnumSceneID.UI_MajorCity01|| Singleton<SceneManage>.Instance.Current == EnumSceneID.LGhuangyuan)
            {
                //UIActivities.Instance.ChangeIndex(1);
                //Control.ShowGUI(GameLibrary.UIActivities);
                EveryTaskData data =
                playerData.GetInstance()
                .taskDataList.itList.Find(
                    x => x.state == (int)TaskProgress.Accept);//|| x.state == (int)TaskProgress.Complete
                if (data != null && data.state == (int)TaskProgress.Accept)
                {
                    UIActivitiesManager.Instance.RewardTaskOperate(data);
                }
            }
            if (Singleton<SceneManage>.Instance.Current == EnumSceneID.UI_MajorCity01)
            {
                EveryTaskData data =
                playerData.GetInstance()
                .taskDataList.itList.Find(
                    x => x.state == (int)TaskProgress.Complete);//|| x.state == (int)TaskProgress.Complete
                if (data != null && data.state == (int)TaskProgress.Complete)
                {
                    Control.ShowGUI(UIPanleID.UIActivities, EnumOpenUIType.OpenNewCloseOld, false, 1);
                }
            }
            if (Singleton<SceneManage>.Instance.Current == EnumSceneID.LGhuangyuan)
            {
                EveryTaskData data =
               playerData.GetInstance()
               .taskDataList.itList.Find(
                   x => x.state == (int)TaskProgress.Complete);//|| x.state == (int)TaskProgress.Complete
                if (data != null && data.state == (int)TaskProgress.Complete)
                {
                    Control.ShowGUI(UIPanleID.UITooltips, EnumOpenUIType.DefaultUIOrSecond, false, "请返回主城，提交悬赏任务");
                }
            }
        }
        else
        {
            if (taskData.taskItem.tasknode.Level <= playerData.GetInstance().selfData.level)
            {
                //追踪的时候需要掉接口
                TaskOperation.Single().SetCurrentTaskItem(taskItem);
            }
            else
            {
                Control.ShowGUI(UIPanleID.UITooltips, EnumOpenUIType.DefaultUIOrSecond, false, "需战队等级达到" + taskData.taskItem.tasknode.Level + "级才可领取");
            }
            
        }
    }

    //protected override void RegisterComponent()
    //{
    //    base.RegisterComponent();
    //    if (index == 0)
    //    {
           
    //        RegisterComponentID((int)UIPanleID.UITaskTracker, 1, task_biaoti.gameObject);

    //        //if (!Singleton<GUIManager>.Instance.dic.ContainsKey(1061))
    //        //{

    //        //    if (NextGuidePanel.Single() != null)
    //        //        NextGuidePanel.Single().Init();
    //        //}
    //    }
        
    //}

}
