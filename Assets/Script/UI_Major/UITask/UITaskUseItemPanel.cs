using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Tianyu;

public class UITaskUseItemPanel : GUIBase
{
    public GUISingleButton btn;
    public UILabel itemName;
    public UILabel des;
    public UILabel btnName;
    public UILabel sign;
    public UISprite goods;
    public UISprite icon;
    private long taskid = 0;
    private GameObject backObj;
    private FubenTaskData fubenTaskData;
    private TaskPropsNode taskPropNode;
    private static UITaskUseItemPanel instance;
    private TaskClass _type;

    public static UITaskUseItemPanel Instance { get { return instance; } set { instance = value; } }

    public UITaskUseItemPanel()
    {
        instance = this;
    }
    public override UIPanleID GetUIKey()
    {
        return UIPanleID.UITaskUseItemPanel;
    }
    public void SetTaskID(long id, TaskClass type)
    {
        taskid = id;
        this._type = type;
    }
    protected override void SetUI(params object[] uiParams)
    {
        taskid = (long)uiParams[0];
        this._type =(TaskClass)uiParams[1];
        base.SetUI();
    }
    protected override void OnLoadData()
    {
        base.OnLoadData();
        this.State = EnumObjectState.Ready;
        Show();
    }
    protected override void Init()
    {
        btn = transform.Find("Btn").GetComponent<GUISingleButton>();
        itemName = transform.Find("ItemName").GetComponent<UILabel>();
        des = transform.Find("Des").GetComponent<UILabel>();
        btnName = transform.Find("Btn/BtnName").GetComponent<UILabel>();
        goods = transform.Find("Goods").GetComponent<UISprite>();
        icon = transform.Find("Goods/Icon").GetComponent<UISprite>();
        backObj = transform.Find("Back").gameObject;
        sign = transform.Find("Sign").GetComponent<UILabel>();
        UIEventListener.Get(backObj).onClick += OnCloseClick;
        btn.onClick = OnBtnClick;
    }

    private void OnCloseClick(GameObject go)
    {
        Control.HideGUI(this.GetUIKey());
        //Hide();
    }

    protected override void ShowHandler()
    {
        if (_type == TaskClass.Main)
        {
            if (TaskManager.Single().TaskToTargetUseItemDic.ContainsKey((int)taskid))
            {
                fubenTaskData = TaskManager.Single().TaskToTargetUseItemDic[(int)taskid];
                if (FSDataNodeTable<TaskPropsNode>.GetSingleton().DataNodeList.ContainsKey(fubenTaskData.opt4))
                {
                    taskPropNode = FSDataNodeTable<TaskPropsNode>.GetSingleton().DataNodeList[fubenTaskData.opt4];

                    itemName.text = taskPropNode.iconName;
                    des.text = taskPropNode.des;
                    btnName.text = taskPropNode.btnName;
                    icon.spriteName = taskPropNode.icon;
                    sign.text = taskPropNode.sign;
                }
            }

        }
        if(_type == TaskClass.Reward)
        {
            if (taskid != 0)
            {
                if (FSDataNodeTable<TaskPropsNode>.GetSingleton().DataNodeList.ContainsKey(taskid))
                {
                    taskPropNode = FSDataNodeTable<TaskPropsNode>.GetSingleton().DataNodeList[taskid];

                    itemName.text = taskPropNode.iconName;
                    des.text = taskPropNode.des;
                    btnName.text = taskPropNode.btnName;
                    icon.spriteName = taskPropNode.icon;
                    sign.text = taskPropNode.sign;
                }
            }
        }


    }

    private void OnBtnClick()
    {
        if (taskPropNode!=null)
        {
            //UITaskCollectPanel.Instance.SetData(taskPropNode.title, 2f, 0, TaskProgressBarType.SendLetter, fubenTaskData, _type);
            Debug.Log("送信");
            //Control.ShowGUI(GameLibrary.UITaskCollectPanel);
            object[] tempObj = new object[] { taskPropNode.title, 2f, (long)0, TaskProgressBarType.SendLetter, fubenTaskData, _type };
            Control.ShowGUI(UIPanleID.UITaskCollectPanel, EnumOpenUIType.DefaultUIOrSecond, false, tempObj);
        }
        Control.HideGUI(this.GetUIKey());
        //Hide();
        //if (TaskManager.NpcTaskListDic.ContainsKey((int)fubenTaskData.opt5))
        //{
        //    int taskID = 0;
        //    foreach (int key in TaskManager.NpcTaskListDic[(int)fubenTaskData.opt5].Keys)
        //    {
        //        taskID = key;
        //    }
        //    Debug.Log("给予信件");
        //    ClientSendDataMgr.GetSingle().GetTaskSend().ClickNpc((int)fubenTaskData.opt5, taskID, TaskManager.NpcTaskListDic[(int)fubenTaskData.opt5][taskID].parm0);
        //    Hide();
        //}
    }

}
