/*
文件名（File Name）:   UIActivitiesRewardItem.cs

作者（Author）:    #高#

创建时间（CreateTime）:  #CreateTime#
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tianyu;

public class UIActivitiesRewardItem : GUISingleItemList
{
    public GUISingleButton enterBtn;
    public GUISingleButton giveBtn;
    public GUISingleButton immediatelyBtn;
    public GUISingleButton quitBtn;
    public GUISingleLabel titleName;
    public GUISingleMultList multList;
    public GUISingleLabel des;
    public GUISingleSprite icon;
    private EveryTaskData vo;

    public GUISingleSpriteGroup star;
    private TaskData _taskData;
    private List<ItemData> dataList = new List<ItemData>();//存储数据列表
    private List<ItemData> dtList = new List<ItemData>();//剔除金币和钻石
    protected override void InitItem()
    {
        enterBtn.onClick = EnterBtn;
        immediatelyBtn.onClick = ImmediatelyBtn;
        quitBtn.onClick = QuitBtn;
        giveBtn.onClick = GiveBtn;
    }
    /// <summary>
    /// 领取奖励
    /// </summary>
    private void GiveBtn()
    {
        //  UITaskTracker.instance.ChangeTaskList(_taskData, TaskListHandleType.Delet);
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", this.index);
        newpacket.Add("arg2", (int)TaskProgress.Get); //4领取奖励
        Singleton<Notification>.Instance.Send(MessageID.common_offer_reward_mission_operation_req, newpacket, C2SMessageType.ActiveWait);
    }
    /// <summary>
    /// 放弃任务
    /// </summary>
    private void QuitBtn()
    {
        object[] obj = new object[5] { "确定放弃[" + vo.taskName + "]悬赏任务", "任务清空后将清空任务进度", UIPopupType.EnSure, this.gameObject, "QuitHanler" };
        Control.ShowGUI(UIPanleID.UIPopUp, EnumOpenUIType.DefaultUIOrSecond, false, obj);
    }
    /// <summary>
    /// 放弃任务
    /// </summary>
    private void QuitHanler()
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", this.index);
        newpacket.Add("arg2", (int)TaskProgress.Accept);//2领取奖励
        Singleton<Notification>.Instance.Send(MessageID.common_offer_reward_mission_operation_req, newpacket);

    }

    /// <summary>
    /// 立即完成
    /// </summary>
    private void ImmediatelyBtn()
    {
        int price = FSDataNodeTable<ResetLaterNode>.GetSingleton().DataNodeList[1].finish_reward;
        object[] obj = new object[5] { "消耗" + price + "钻石立即完成", "[" + vo.taskName + "]悬赏任务", UIPopupType.EnSure, this.gameObject, "ImmediatelyHandle" };
        Control.ShowGUI(UIPanleID.UIPopUp, EnumOpenUIType.DefaultUIOrSecond, false, obj);

    }

    private void ImmediatelyHandle()
    {
        int price = FSDataNodeTable<ResetLaterNode>.GetSingleton().DataNodeList[1].finish_reward;
        if (price > playerData.GetInstance().baginfo.diamond)
        {
            Control.ShowGUI(UIPanleID.UITooltips, EnumOpenUIType.DefaultUIOrSecond, false, "钻石不足");
        }
        else
        {
            Dictionary<string, object> newpacket = new Dictionary<string, object>();
            newpacket.Add("arg1", this.index);
            newpacket.Add("arg2", (int)TaskProgress.Complete);//3领取奖励
            Singleton<Notification>.Instance.Send(MessageID.common_offer_reward_mission_operation_req, newpacket);
        }
    }
    private void EnterBtn()
    {
        if (enterBtn.text == "接取")
        {
            if (playerData.GetInstance().taskDataList.getCount > 0)
            {
                Dictionary<string, object> newpacket = new Dictionary<string, object>();
                newpacket.Add("arg1", this.index);
                newpacket.Add("arg2", (int)TaskProgress.NoAccept);//1领取奖励
                Singleton<Notification>.Instance.Send(MessageID.common_offer_reward_mission_operation_req, newpacket);

            }
            else
            {
                Control.ShowGUI(UIPanleID.UITooltips, EnumOpenUIType.DefaultUIOrSecond, false, "今日悬赏任务已全部完成");
            }
        }
        else if (enterBtn.text == "前往")
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

            Control.HideGUI();
        }
    }

    public override void Info(object obj)
    {
        base.Info(obj);
        if (obj == null)
        {

        }
        else
        {
            vo = (EveryTaskData)obj;
        }
    }

    protected override void ShowHandler()
    {
        base.ShowHandler();
        titleName.text = vo.taskName;
        if (vo.countIndex > vo.count)
        {
            vo.countIndex = vo.count;
            Debug.Log("服务器发来的数据错误，当前数量超出总数");
        }
        des.text = vo.des + "(" + vo.countIndex + "/" + vo.count + ")";
        icon.spriteName = vo.iconName;
        star.IsShow(vo.star);
        dataList = TaskManager.Single().GetItemList(vo.star);
        switch ((TaskProgress)vo.state)
        {
            case TaskProgress.CantAccept:
                enterBtn.Hide();
                immediatelyBtn.Hide();
                quitBtn.Hide();
                giveBtn.Hide();
                break;
            case TaskProgress.NoAccept:
                enterBtn.text = "接取";
                enterBtn.Show();
                immediatelyBtn.Hide();
                quitBtn.Hide();
                giveBtn.Hide();

                break;
            case TaskProgress.Accept:
                enterBtn.text = "前往";
                immediatelyBtn.Show();
                quitBtn.Show();
                giveBtn.Hide();

                break;
            case TaskProgress.Complete:
                immediatelyBtn.Hide();
                quitBtn.Hide();
                enterBtn.Hide();
                giveBtn.Show();

                break;
        }
        if (dataList.Count > 0)
        {
            multList.InSize(dataList.Count, dataList.Count);
            multList.Info(dataList.ToArray());

        }
    }
}
