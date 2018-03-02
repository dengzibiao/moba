/*
文件名（File Name）:   UIActivities.cs

作者（Author）:    #高#

创建时间（CreateTime）:  #CreateTime#
*/

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Tianyu;

public class UIActivities : GUIBase
{

    private static UIActivities instance;
    public GUISingleCheckBoxGroup checkBoxs;
    public GUISingleMultList everyMultList;//日常
    public GUISingleMultList rewardMultList;//悬赏
    public Transform archaeologyPanel;//考古
    public Transform disasterPanel;//天灾
    public GUISingleButton backBtn;
    public GUISingleSprite redPoint1;//日常
    public GUISingleSprite redPoint2;//悬赏
    private int _index = 0;
    private Transform _view;
    private GameObject livenessView;
    private GameObject rewardView;
    private GameObject[] obj = new GameObject[4];
    private GameObject[] gobj = new GameObject[2];
    public static UIActivities Instance
    {
        get { return instance; }
        set { instance = value; }
    }
    public UIActivities()
    {
        instance = this;
    }

    public override UIPanleID GetUIKey()
    {
        return UIPanleID.UIActivities;
    }

    protected override void SetUI(params object[] uiParams)
    {
        if (uiParams!= null&& uiParams.Length>0)
            _index = int.Parse(uiParams[0].ToString());
        base.SetUI(uiParams);
    }

    protected override void Init()
    {
        checkBoxs.onClick = OnCheckClick;
        backBtn.onClick = OnBackClick;
        _view = UnityUtil.FindCtrl<UIScrollView>(this.gameObject, "ScrollView").transform;
        livenessView = transform.Find("LivenessView").gameObject;
        rewardView = transform.Find("RewardView").gameObject;
        archaeologyPanel = transform.Find("ArchaeologyPanel");
        disasterPanel = transform.Find("DisasterPanel");

        obj[0] = everyMultList.gameObject;
        obj[1] = rewardMultList.gameObject;
        gobj[0] = livenessView;
        gobj[1] = rewardView;

        obj[1].SetActive(false);
        obj[2] = archaeologyPanel.gameObject;
        obj[3] = disasterPanel.gameObject;
        playerData.GetInstance().taskDataList.Id = int.Parse(TimeManager.Instance.ConvertIntDateTime(Auxiliary.GetNowTime()).ToString("dd")) % 3;
        if (FSDataNodeTable<DayActiveNode>.GetSingleton().DataNodeList.ContainsKey(playerData.GetInstance().taskDataList.Id + 1))
        {
            DayActiveNode dailyTasksNode = FSDataNodeTable<DayActiveNode>.GetSingleton().FindDataByType(playerData.GetInstance().taskDataList.Id + 1);
            playerData.GetInstance().taskDataList.DailyTasks = dailyTasksNode;

        }
    }
    /// <summary>
    /// 显示红点
    /// </summary>
    private void ShowRedPoint()
    {
        if (playerData.GetInstance().taskDataList.itemList.Find(x => x.state == (int)TaskProgress.Complete) != null || playerData.GetInstance().taskDataList.box1State == (int)TaskProgress.Complete || playerData.GetInstance().taskDataList.box2State == (int)TaskProgress.Complete || playerData.GetInstance().taskDataList.box3State == (int)TaskProgress.Complete || playerData.GetInstance().taskDataList.box4State == (int)TaskProgress.Complete)
        {
            redPoint1.Show();
            Singleton<RedPointManager>.Instance.AddChildFlag(EnumRedPoint.RD_ACTIVITY, 1);
        }
        else
        {
            redPoint1.Hide();
            Singleton<RedPointManager>.Instance.DeletChildFlag(EnumRedPoint.RD_ACTIVITY, 1);
        }
        //----------------------------------------------
        //悬赏任务显示红点
        if (DataDefine.isSkipFunction && !FunctionOpenMng.GetInstance().GetFunctionOpen(34))
        {
            if (playerData.GetInstance().taskDataList.getCount > 0)
            {
                redPoint2.Show();
                Singleton<RedPointManager>.Instance.AddChildFlag(EnumRedPoint.RD_ACTIVITY, 2);
            }
        }
        else
        {
            redPoint2.Hide();
            Singleton<RedPointManager>.Instance.DeletChildFlag(EnumRedPoint.RD_ACTIVITY, 2);
        }
    }
    protected override void ShowHandler()
    {
        RefreshUI();
    }

    protected override void OnLoadData()
    {
        base.OnLoadData();
        Singleton<Notification>.Instance.RegistMessageID(MessageID.common_ask_daily_mission_ret, UIPanleID.UIActivities);
        Singleton<Notification>.Instance.RegistMessageID(MessageID.common_mission_box_info_ret, UIPanleID.UIActivities);
        Singleton<Notification>.Instance.RegistMessageID(MessageID.common_offer_reward_mission_list_ret, UIPanleID.UIActivities);
        if (_index == 0)
        {
            if (playerData.GetInstance().taskDataList.itemList.Count == 0)
            {
                Singleton<Notification>.Instance.Send(MessageID.common_ask_daily_mission_req, C2SMessageType.ActiveWait);
                Singleton<Notification>.Instance.Send(MessageID.common_mission_box_info_req, C2SMessageType.ActiveWait);
            }
            else
            {
                Show();
            }
            
        }
        else if(_index == 1)
        {
            if (playerData.GetInstance().taskDataList.itList.Count >0)
                Show();
        }
   
    }

    public override void ReceiveData(UInt32 messageID)
    {
        base.ReceiveData(messageID);
        switch (messageID)
        {
            case MessageID.common_ask_daily_mission_ret:
                if (playerData.GetInstance().taskDataList.itemList.Count > 0)
                {
                    Show();
                }
                break;
            case MessageID.common_mission_box_info_ret: Show(); ; break;
            case MessageID.common_offer_reward_mission_list_ret:
                Show(); break;
        }
    }

    /// <summary>
    /// 日常任务表
    /// </summary>
    /// <returns></returns>
    public object[] InitItemData()
    {
        return playerData.GetInstance().taskDataList.itemList.ToArray();
    }
    /// <summary>
    /// 悬赏任务表
    /// </summary>
    /// <returns></returns>
    public object[] InitRewardData()
    {
        return playerData.GetInstance().taskDataList.itList.ToArray();
    }
    private void OnCheckClick(int index, bool boo)
    {
        if (boo)
        {
            this._index = index;
            switch (index)
            {
                case 0:
                    // RefreshUI();
                    _view.GetComponent<UIScrollView>().ResetPosition();
                    //  ClientSendDataMgr.GetSingle().GetTaskSend().common_mission_box_info_req();
                    Show();
                    break;
                case 1:
                    if (!DataDefine.isSkipFunction && !FunctionOpenMng.GetInstance().GetFunctionOpen(34))
                    {
                        string text = FSDataNodeTable<UnLockFunctionNode>.GetSingleton().DataNodeList[34].limit_tip;
                        object[] obj = new object[5] { text, null, UIPopupType.OnlyShow, this.gameObject, "DefaultReward" };
                        Control.ShowGUI(UIPanleID.UIPopUp, EnumOpenUIType.DefaultUIOrSecond, false, obj);
                        return;
                    }
                    else
                    {
                        Show();
                        _view.GetComponent<UIScrollView>().ResetPosition();
                    }
                    break;
                case 2:

                    RefreshUI();
                    break;
                case 3:

                    RefreshUI();
                    break;
            }
        }

    }
    private void OnBackClick()
    {
        _view.GetComponent<UIScrollView>().ResetPosition();
      //  Hide();
      Control.HideGUI();
    }

    protected override void OnRelease()
    {
        base.OnRelease();
        this._index = 0;
        checkBoxs.setMaskState(0);
        everyMultList.Release();//日常
        rewardMultList.Release();//悬赏
    }
    /// <summary>
    /// 切换野签
    /// </summary>
    /// <param name="index"></param>
    public void ChangeIndex(int index)
    {
        this._index = index;
        RefreshUI();
        checkBoxs.setMaskState(_index);
    }
    public void RefreshUI()
    {
        ShowRedPoint();
        if (_index == 0)
        {
            if (InitItemData().Length > 0)
            {
                everyMultList.InSize(InitItemData().Length, 1);
                everyMultList.Info(InitItemData());
            }
            everyMultList.ScrollView = _view;
            obj[_index].SetActive(true);

        }
        else if (_index == 1)
        {
            if (InitRewardData().Length > 0)
            {
                rewardMultList.InSize(InitRewardData().Length, 1);
                rewardMultList.Info(InitRewardData());
            }
            else
            {
                rewardMultList.InSize(0, 1);
                rewardMultList.Info(null);
            }

            rewardMultList.ScrollView = _view;
            obj[_index].SetActive(true);
        }
        for (int i = 0; i < obj.Length; i++)
        {
            if (i != _index)
            {
                obj[i].SetActive(false);
            }
            else
            {
                obj[i].SetActive(true);
            }
        }
        for (int i = 0; i < gobj.Length; i++)
        {
            if (i != _index)
            {
                gobj[i].SetActive(false);
            }
            else
            {
                gobj[i].SetActive(true);
            }
        }
    }


}
