using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.ImageEffects;

public class UITaskRewardPanel : GUIBase
{
    public GUISingleButton closebtn;
    public GUISingleMultList multList;
    private PlayEffect effect;
    private int goldCount;
    private int diamondCount;
    private int zhanduiExp;
    private int heroExp;
    private int xuanshangGold;
    protected override void Init()
    {
        closebtn = transform.Find("Close").GetComponent<GUISingleButton>();
        effect = transform.Find("Effect").GetComponent<PlayEffect>();
        closebtn.onClick = OnClose;
    }
    public override UIPanleID GetUIKey()
    {
        return UIPanleID.UITaskRewardPanel;
    }
    //protected override void SetUI(params object[] uiParams)
    //{
       
    //    base.SetUI();
    //}
    protected override void OnLoadData()
    {
        base.OnLoadData();
        this.State = EnumObjectState.Ready;
        Show();
    }
    protected override void ShowHandler()
    {
        multList.InSize(TaskManager.Single().itemlist.Count, 5);
        multList.Info(TaskManager.Single().itemlist.ToArray());
        effect.Name = "6_000";
        if (Camera.main.GetComponent<Blur>()!=null)
        {
            Camera.main.GetComponent<Blur>().enabled = true;
        }
    }

    void OnClose()
    {

        GuideManager.Single().SetObject(this.gameObject);
        //将物品加入到背包
        //先将 金币 钻石 和 战队经验拿出来
        //剩余的物品加入到背包
        //goldCount = 0;
        //diamondCount = 0;
        //zhanduiExp = 0;
        //heroExp = 0;
        //xuanshangGold = 0;
        //for (int i = itemlist.Count - 1; i >= 0; i--)
        //{
        //    if (itemlist[i].Id == 101)
        //    {
        //        goldCount = itemlist[i].Count;
        //        itemlist.RemoveAt(i);
        //    }
        //    else if (itemlist[i].Id == 10101)
        //    {
        //        diamondCount = itemlist[i].Count;
        //        itemlist.RemoveAt(i);
        //    }
        //    else if (itemlist[i].Id == 1010101)
        //    {
        //        zhanduiExp = itemlist[i].Count;
        //        itemlist.RemoveAt(i);
        //    } 
        //    else if(itemlist[i].Id == 102)
        //    {
        //        heroExp = itemlist[i].Count;
        //        itemlist.RemoveAt(i);
        //    }
        //    else if (itemlist[i].Id == 103)
        //    {
        //        xuanshangGold = itemlist[i].Count;
        //        itemlist.RemoveAt(i);
        //    }
        //}
        //if (goldCount > 0) playerData.GetInstance().MoneyHadler(MoneyType.Gold,goldCount);
        //if(diamondCount > 0) playerData.GetInstance().MoneyHadler(MoneyType.Diamond, diamondCount);
        ////if (zhanduiExp > 0) ;战队经验


        //itemlist.Clear();

        //ClientSendDataMgr.GetSingle().GetTaskSend().GetTaskListComplete(C2SMessageType.Active);
        //ClientSendDataMgr.GetSingle().GetTaskSend().GetTaskList(C2SMessageType.Active);
        //Hide();
        Control.HideGUI(this.GetUIKey());
        if (Camera.main.GetComponent<Blur>() != null)
        {
            Camera.main.GetComponent<Blur>().enabled = false;
        }
        Debug.Log("任务奖励升级  " + playerData.GetInstance().beforePlayerLevel + "=====>" + playerData.GetInstance().selfData.level);
        if (Globe.isMainCityUpgrade)
        {
            Control.ShowGUI(UIPanleID.Upgrade, EnumOpenUIType.DefaultUIOrSecond);
            Globe.isMainCityUpgrade = false;
        }
    }
}
