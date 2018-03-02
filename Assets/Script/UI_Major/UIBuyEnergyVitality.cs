using UnityEngine;
using System.Collections;
using System;

public class UIBuyEnergyVitality : GUIBase
{
    public GUISingleButton buyBtn;
    public GUISingleButton cancelBtn;
    public GUISingleLabel des;
    public GUISingleLabel buyCount;
    public ActionPointType type;
    public GameObject maskObj;
    public int needJewelCount = 50; //购买一次花费钻石数
    public int buyVitalityCount = 120;//购买活力数量
    public int buyEnergyCount = 120;//购买体力数量
    public int buyVitalityTimes = 0; //今日购买活力次数
    public int buyEnergyTimes = 0; //近日购买体力次数
    public int MaxBuyVitalityTimes = 5;
    public int MaxBuyEnergyTimes = 5;
    public static UIBuyEnergyVitality instance;
    public static UIBuyEnergyVitality Instance { get { return instance; } set { instance = value; } }
    public UIBuyEnergyVitality()
    {
        instance = this;
    }

    public override UIPanleID GetUIKey()
    {
        return UIPanleID.UIBuyEnergyVitality;
    }

    protected override void Init()
    {
        instance = this;
        base.Init();
        maskObj = transform.Find("Mask").gameObject;
        buyBtn.onClick = OnBuyClick;
        cancelBtn.onClick = OnCancleClick;
        UIEventListener.Get(maskObj).onClick += OnCloseClick;
    }

    private void OnCloseClick(GameObject go)
    {
        Control.HideGUI(this.GetUIKey());
        //Hide();
    }
    protected override void SetUI(params object[] uiParams)
    {
        this.type = (ActionPointType)uiParams[0];
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
        int times;
        switch (this.type)
        {
            //[ff0000]" + "20" +"[-]" 
            case ActionPointType.Vitality:
                 //读取一下信息 TODO
                des.text = "花费" + playerData.GetInstance().actionData.vitalityJewelArray[playerData.GetInstance().actionData.vitalityBuyTimes] + "点钻石购买" + playerData.GetInstance().actionData.buyVitalityCount + "点活力";
                times = playerData.GetInstance().actionData.maxVitalityBuyTimes - playerData.GetInstance().actionData.vitalityBuyTimes;
                if (times <= 0)
                {
                    times = 0;
                }
                buyCount.text = "[ff0000]" + "(" + times + "/"+ playerData.GetInstance().actionData.maxVitalityBuyTimes + ")" + "[-]";
                break;
            case ActionPointType.Energy:
                if (playerData.GetInstance().actionData.energyBuyTimes< playerData.GetInstance().actionData.EnergyJewelArray.Length)
                {
                    des.text = "花费" + playerData.GetInstance().actionData.EnergyJewelArray[playerData.GetInstance().actionData.energyBuyTimes] + "点钻石购买" + playerData.GetInstance().actionData.buyEnergyCount + "点体力";
                }
                times = playerData.GetInstance().actionData.maxEnergyBuyTimes - playerData.GetInstance().actionData.energyBuyTimes;
                if (times <= 0)
                {
                    times = 0;
                }
                buyCount.text = "[ff0000]" + "(" + times + "/" + playerData.GetInstance().actionData.maxEnergyBuyTimes + ")" + "[-]";
                break;
            default:
                break;
        }
    }
    private void OnCancleClick()
    {
        Control.HideGUI(this.GetUIKey());
        //Hide();
    }

    private void OnBuyClick()
    {
        //更新界面 TODO
        switch (this.type)
        {
            case ActionPointType.Vitality:
                //购买次数超过近日最大购买次数不能购买
                //活力值满了，不需要购买
                if (playerData.GetInstance().actionData.vitalityBuyTimes >= playerData.GetInstance().actionData.maxVitalityBuyTimes)
                {
                    //UIPromptBox.Instance.ShowLabel("今日购买次数已用完");
                    //Control.ShowGUI(GameLibrary.UIPromptBox);
                    Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, "今日购买次数已用完");
                    Control.HideGUI(this.GetUIKey());
                    //Hide();
                    return;
                }
                if (playerData.GetInstance().baginfo.vitality >= playerData.GetInstance().actionData.maxCurrentVitality)
                {
                    //UIPromptBox.Instance.ShowLabel("活力已达到上限");
                    //Control.ShowGUI(GameLibrary.UIPromptBox);
                    Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, "活力已达到上限");
                    Control.HideGUI(this.GetUIKey());
                    //Hide();
                    return;
                }
                //playerData.GetInstance().ChangeActionPointHandler(ActionPointType.Vitality, PropertyManager.Instance.buyVitalityCount);
                //UIMoney.instance.ChangeVitality(PropertyManager.Instance.buyVitalityCount);
                //playerData.GetInstance().actionData.vitalityBuyTimes++;
                ClientSendDataMgr.GetSingle().GetActionPointSend().SendBuyActionPoint(2,1,C2SMessageType.PASVWait);
                break;
            case ActionPointType.Energy:
                if (playerData.GetInstance().actionData.energyBuyTimes >= playerData.GetInstance().actionData.maxEnergyBuyTimes)
                {
                    //UIPromptBox.Instance.ShowLabel("今日购买次数已用完");
                    //Control.ShowGUI(GameLibrary.UIPromptBox);
                    Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, "今日购买次数已用完");
                    Control.HideGUI(this.GetUIKey());
                    //Hide();
                    return;
                }
                if (playerData.GetInstance().baginfo.strength >= playerData.GetInstance().actionData.maxCurrentEnerty)
                {
                    //UIPromptBox.Instance.ShowLabel("已达到上限");
                    //Control.ShowGUI(GameLibrary.UIPromptBox);
                    Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, "已达到上限");
                    Control.HideGUI(this.GetUIKey());
                    //Hide();
                    return;
                }
                //UIMoney.instance.ChangeStrength(buyEnergyCount);
                //playerData.GetInstance().ChangeActionPointHandler(ActionPointType.Energy, 1);
                //playerData.GetInstance().actionData.energyBuyTimes++;
                ClientSendDataMgr.GetSingle().GetActionPointSend().SendBuyActionPoint(1, 1,C2SMessageType.PASVWait);
                break;
            default:
                break;
        }
        Control.HideGUI(this.GetUIKey());
        //Hide();
    }
    public void SetInfo(ActionPointType type)
    {
        this.type = type;
    }
}
