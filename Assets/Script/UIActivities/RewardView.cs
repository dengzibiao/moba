/*
文件名（File Name）:   RewardView.cs

作者（Author）:    #高#

创建时间（CreateTime）:  2016-11-3 11:48:18
*/
using UnityEngine;
using System.Collections;
using Tianyu;

public class RewardView : GUIBase
{
    public GUISingleLabel getAwardNum;
    public GUISingleLabel needjewelNum;
    public GUISingleButton refreshBtn;
    public override UIPanleID GetUIKey()
    {
        return UIPanleID.none;
    }

    protected override void Init()
    {
        base.Init();
        if (FSDataNodeTable<ResetLaterNode>.GetSingleton().DataNodeList.Count > 0)
        {
            for (int i = 0; i < FSDataNodeTable<ResetLaterNode>.GetSingleton().DataNodeList[1].buy_reward.Length; i++)
            {
                int ct = FSDataNodeTable<ResetLaterNode>.GetSingleton().DataNodeList[1].buy_reward[i];
                playerData.GetInstance().taskDataList.refreshCost.Add(ct);
            }
        }
        refreshBtn.onClick = RefreshBtn;

    }

    /// <summary>
    /// 刷新悬赏任务列表
    /// </summary>
    private void RefreshBtn()
    {
        if (playerData.GetInstance().taskDataList.getCount > 0)
        {
            if (playerData.GetInstance().taskDataList.itList.Find(x => x.state == (int)TaskProgress.Accept) !=
       null) Control.ShowGUI(UIPanleID.UITooltips, EnumOpenUIType.DefaultUIOrSecond, false, "存在已接取的任务时不可刷新");
            else if (playerData.GetInstance().taskDataList.itList.Find(x => x.state == (int)TaskProgress.Complete) != null)
                Control.ShowGUI(UIPanleID.UITooltips, EnumOpenUIType.DefaultUIOrSecond, false, "请先领取任务奖励后再试");
            else if (playerData.GetInstance().taskDataList.itList.Find(x => x.state == (int)TaskProgress.CantAccept) !=
                     null)
                Control.ShowGUI(UIPanleID.UITooltips, EnumOpenUIType.DefaultUIOrSecond, false, "今日悬赏任务已全部完成");
            else
            {
                if (needjewelNum.text == "免费")
                {
                    object[] obj = new object[5]
                    {"消耗" + 0 + "钻石可刷新悬赏任务", "仅刷新处于可接状态的任务", UIPopupType.Refresh, this.gameObject, "Refreshhandle"};
                    Control.ShowGUI(UIPanleID.UIPopUp, EnumOpenUIType.DefaultUIOrSecond, false, obj);
                }
                else
                {
                    object[] obj = new object[5] { "消耗" + needjewelNum.text + "钻石可刷新悬赏任务", "仅刷新处于可接状态的任务", UIPopupType.Refresh, this.gameObject, "Refreshhandle" };
                    Control.ShowGUI(UIPanleID.UIPopUp, EnumOpenUIType.DefaultUIOrSecond, false, obj);
                }
            }

        }
        else
        {
            Control.ShowGUI(UIPanleID.UITooltips, EnumOpenUIType.DefaultUIOrSecond, false, "今日悬赏任务已全部完成");
        }

    }

    private void Refreshhandle()
    {
        if (playerData.GetInstance().taskDataList.refreshCost[playerData.GetInstance().taskDataList.refreshCount]
                     > playerData.GetInstance().baginfo.diamond)
        {
            Control.ShowGUI(UIPanleID.UITooltips, EnumOpenUIType.DefaultUIOrSecond, false, "钻石不足");
        }
        else
        {
            Singleton<Notification>.Instance.Send(MessageID.common_offer_reward_mission_new_list_req);
        }

    }
    protected override void ShowHandler()
    {
        getAwardNum.text = playerData.GetInstance().taskDataList.getCount.ToString();
        if (playerData.GetInstance().taskDataList.refreshCount >
            playerData.GetInstance().taskDataList.refreshCost.Count - 1)
        {
            playerData.GetInstance().taskDataList.refreshCount =
                playerData.GetInstance().taskDataList.refreshCost.Count - 1;
        }
        if (playerData.GetInstance().taskDataList.refreshCost[playerData.GetInstance().taskDataList.refreshCount] == 0)
        {
            needjewelNum.text = "免费";
        }
        else
        {
            needjewelNum.text =
                playerData.GetInstance().taskDataList.refreshCost[playerData.GetInstance().taskDataList.refreshCount]
                    .ToString();
        }
    }
}
