/*
文件名（File Name）:   UIActivityDailyItem.cs

作者（Author）:    #高#

创建时间（CreateTime）:  #CreateTime#
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tianyu;

public class UIActivityDailyItem : GUISingleItemList
{

    public GUISingleButton enterBtn;
    public GUISingleButton getBtn;
    public GUISingleMultList multList;
    public GUISingleSprite icon;
    public GUISingleSprite lockSprite;
    public GUISingleSprite overSprite;
    public GUISingleLabel titleName;
    public GUISingleLabel lockDes;
    public GUISingleLabel des;
    public GUISingleLabel activeNum;
    public GUISingleLabel count;
    private EveryTaskData vo;
    private List<ItemData> dataList = new List<ItemData>();//存储数据列表
    private List<ItemData> dtList = new List<ItemData>();//剔除金币和钻石
    protected override void InitItem()
    {
        enterBtn.onClick = EnterBtnOnclick;
        getBtn.onClick = GetBtnBtnOnclick;
    }

    private void GetBtnBtnOnclick()
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", vo.id);//任务ID
        newpacket.Add("arg2", (int)(GetTaskItemType.Daily));//任务类型1日常2领箱子
        newpacket.Add("arg3", 0);//任务ID
        Singleton<Notification>.Instance.Send(MessageID.common_get_reward_props_req, newpacket, C2SMessageType.Active);
    }

    private void EnterBtnOnclick()
    {
        if (vo.leave_for != 0)
        {
            Debug.Log("界面ID为0");
            if (vo.id == 10)
            {
                if (Control.GetUI<GUIBase>(UIPanleID.UIActivities) != null)
                {
                    UIActivities.Instance.ChangeIndex(1);
                }               
            }else if (vo.id == 2)
            {
                Control.ShowGUI(UIPanleID.UIGoldHand, EnumOpenUIType.DefaultUIOrSecond);
            }
            else if (vo.id == 7)
            {
                object[] openParams = new object[] { OpenLevelType.ByIDOpen, 0 };
                Control.ShowGUI(UIPanleID.UILevel, EnumOpenUIType.OpenNewCloseOld, false, openParams);
            }
            else if (vo.id == 13)
            {
                UI_Setting.GetInstance().OnEquipBtn();
            }
            else if (vo.id == 8 || vo.id == 9)
            {
                UI_Setting.GetInstance().OnEnchantBtnClick();
            }
            else if(vo.id == 4)
            {
                //Control.ShowGUI(vo.leave_for);
                object[] openParams = new object[] { OpenLevelType.ByIDOpen, playerData.GetInstance().CanEnterMap.Count > 0 ? playerData.GetInstance().CanEnterMap[playerData.GetInstance().CanEnterMap.Count - 1] : 100 };
                Control.ShowGUI(UIPanleID.UILevel, EnumOpenUIType.OpenNewCloseOld, false, openParams);
            }
            else if(vo.id == 11)
            {
                UIPVP.instance.OnAbattoirBtnClick();
            }
            else
            {
                Control.ShowGUI((UIPanleID)vo.leave_for, EnumOpenUIType.OpenNewCloseOld);
            }
        }
    }

    protected override void ShowHandler()
    {
        base.ShowHandler();
        icon.spriteName = vo.iconName;
        titleName.text = vo.taskName;
        des.text = vo.des;
        dataList = TaskManager.Single().GetItemList(vo.scriptId);
        switch ((TaskProgress)vo.state)
        {
            case TaskProgress.CantAccept:
                enterBtn.gameObject.SetActive(false);
                getBtn.gameObject.SetActive(false);
                lockSprite.gameObject.SetActive(true);
                lockDes.text = vo.deblockingDes;
                overSprite.gameObject.SetActive(false);
                break;
            case TaskProgress.NoAccept:
                enterBtn.gameObject.SetActive(true);
                getBtn.gameObject.SetActive(false);
                lockSprite.gameObject.SetActive(false);
                overSprite.gameObject.SetActive(false);
                lockDes.text = "";

                break;
            case TaskProgress.Accept:
                break;

            case TaskProgress.Complete:
                enterBtn.gameObject.SetActive(false);
                getBtn.gameObject.SetActive(true);
                lockSprite.gameObject.SetActive(false);
                overSprite.gameObject.SetActive(false);
                lockDes.text = "";
                break;

            case TaskProgress.Reward:
                overSprite.gameObject.SetActive(true);
                enterBtn.gameObject.SetActive(false);
                getBtn.gameObject.SetActive(false);
                lockDes.text = "";
                break;
        }
        if (vo.count != 0 && (TaskProgress)vo.state != TaskProgress.CantAccept &&
            (TaskProgress)vo.state != TaskProgress.Reward)
        {
            count.text = vo.countIndex + "/" + vo.count;
        }
        else
        {
            count.text = "";
        }

        if (vo.active != 0)
        {
            //if (vo.activeIndex > vo.active) vo.activeIndex = vo.active;
            //activeNum.text = "活跃度" + vo.activeIndex + "/" + vo.active;
            if (vo.activeIndex > vo.active) vo.activeIndex = vo.active;
            activeNum.text = "活跃度" + vo.active;
        }
        else
        {
            activeNum.text = "";
        }
        if (vo.type == (int)EveryTaskProgress.Mopping)
        {
            for (int i = 0; i < dataList.Count; i++)
            {
                if (GameLibrary.Instance().ItemStateList.ContainsKey(dataList[i].Id))
                {
                    if (GameLibrary.Instance().ItemStateList[dataList[i].Id].types == 10)
                    {
                        dataList[i].IconName = GameLibrary.Instance().ItemStateList[dataList[i].Id].icon_name;
                        if (FSDataNodeTable<VipNode>.GetSingleton().DataNodeList.ContainsKey(playerData.GetInstance().selfData.vip))
                        {
                            dataList[i].Count = FSDataNodeTable<VipNode>.GetSingleton().DataNodeList[playerData.GetInstance().selfData.vip].sweep;
                        }
                    }
                }
            }
        }
        if (dataList.Count > 0)
        {
            multList.InSize(dataList.Count, dataList.Count);
            multList.Info(dataList.ToArray());

        }

    }

    public override void Info(object obj)
    {
        if (obj == null)
        {

        }
        else
        {
            vo = (EveryTaskData)obj;


        }
    }
}
