using UnityEngine;
using System.Collections;
using Tianyu;

public class UIMoney : GUIBase
{
    public static UIMoney instance;
    public GUISingleButton goldBtn;//点金手功能按钮
    public GUISingleButton jewelBtn;//金币功能按钮
    public GUISingleButton energyBtn;//体力功能按钮
    //public GUISingleButton vitalityBtn;
    public GUISingleLabel goldTxt;
    public GUISingleLabel jewelTxt;
    public GUISingleLabel energyTxt;
    public GUISingleLabel vitalityTxt;
    public int maxCount = 1000;


    private GameObject energyObj;
    //private GameObject vitalityObj;
    protected override void Init()
    {
        energyTxt = transform.Find("EnergyTxt").GetComponent<GUISingleLabel>();
        goldBtn.onClick = OnGlogClick;
        jewelBtn.onClick = OnJewelClick;
        energyBtn.onClick = OnEnergyClick;
        //vitalityBtn.onClick = OnVitalityClick;
        energyObj = transform.Find("EnergyDesBtn").gameObject;
        //vitalityObj = transform.Find("VitalityDesBtn").gameObject;
        UIEventListener.Get(energyObj).onPress = OnEnergyPress;
        //UIEventListener.Get(vitalityObj).onPress = OnVitalityPress;
        instance = this;
       
        playerData.GetInstance().ChangeMoneyEvent += UpdateMoney;
        playerData.GetInstance().ChangeActionPoint += UpdateActionPoint;
        playerData.GetInstance().ChangeActionPointCeiling += UpdateActionPointCeiling;
    }
    protected override void ShowHandler()
    {
        InitShow();
        RefreshFunOpen();
    }
    void RefreshFunOpen()
    {
        //点金手功能开启id 11
     //   goldBtn.gameObject.SetActive(FunctionOpenMng.GetInstance().GetValu(11));
    }
    public override UIPanleID GetUIKey()
    {
        return UIPanleID.UIMoney;
    }
    public void InitShow()
    {
        goldTxt.text = playerData.GetInstance().baginfo.gold.ToString( );
        jewelTxt.text = playerData.GetInstance().baginfo.diamond.ToString();

		if (FSDataNodeTable<PlayerLevelUpNode>.GetSingleton().DataNodeList!=null&&playerData.GetInstance().selfData!=null&&FSDataNodeTable<PlayerLevelUpNode>.GetSingleton().DataNodeList.ContainsKey(playerData.GetInstance().selfData.level))
        {
            PlayerLevelUpNode levelNode = FSDataNodeTable<PlayerLevelUpNode>.GetSingleton().DataNodeList[playerData.GetInstance().selfData.level];
            if (levelNode != null)
            {
                if (playerData.GetInstance().baginfo.strength > levelNode.maxPower)
                {
                    energyTxt.text = "[5eaeff]" + playerData.GetInstance().baginfo.strength + "[-]" + "/" + levelNode.maxPower;
                }
                else
                {
                    energyTxt.text = playerData.GetInstance().baginfo.strength + "/" + levelNode.maxPower;
                }
                if (playerData.GetInstance().baginfo.vitality > levelNode.maxVitality)
                {
                    vitalityTxt.text = "[00ffff]" + playerData.GetInstance().baginfo.vitality + "[-]" + "/" + levelNode.maxVitality;
                }
                else
                {
                    vitalityTxt.text = playerData.GetInstance().baginfo.vitality + "/" + levelNode.maxVitality;
                }
            }
        }
        playerData.GetInstance().ChangeActionPointCeilingHandler();
    }
    /// <summary>
    /// 活力触摸函数
    /// </summary>
    /// <param name="go"></param>
    /// <param name="state"></param>
    private void OnVitalityPress(GameObject go, bool state)
    {
        if (state)
        {
            Control.ShowGUI(UIPanleID.UICountdownPanel, EnumOpenUIType.DefaultUIOrSecond, false, ActionPointType.Vitality);
        }
        else
        {
            Control.HideGUI(UIPanleID.UICountdownPanel);
        }
       
    }
    /// <summary>
    /// 体力触摸函数
    /// </summary>
    /// <param name="go"></param>
    /// <param name="state"></param>
    private void OnEnergyPress(GameObject go, bool state)
    {
        if (state)
        {
            Control.ShowGUI(UIPanleID.UICountdownPanel, EnumOpenUIType.DefaultUIOrSecond, false, ActionPointType.Energy);
        }
        else
        {
            Control.HideGUI(UIPanleID.UICountdownPanel);
        }
    }
    private void OnEnergyClick()
    {
        Control.ShowGUI(UIPanleID.UIBuyEnergyVitality, EnumOpenUIType.DefaultUIOrSecond, false, ActionPointType.Energy);
        //playerData.GetInstance().MoneyHadler(MoneyType.PVEcoin, 10000);
    }

    private void OnJewelClick()
    {
        //playerData.GetInstance().MoneyHadler(MoneyType.Diamond, 10000);
    }

    private void OnGlogClick()
    {
        //playerData.GetInstance().MoneyHadler(MoneyType.Gold, 10000);
        if (DataDefine.isSkipFunction|| FunctionOpenMng.GetInstance().GetFunctionOpen(11))
        {
            Control.ShowGUI(UIPanleID.UIGoldHand, EnumOpenUIType.DefaultUIOrSecond);
        }
        else
        {
            string text = FSDataNodeTable<UnLockFunctionNode>.GetSingleton().DataNodeList[11].limit_tip;
            //UIPromptBox.Instance.ShowLabel(text);
            Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, text);
        }
    }
    /// <summary>
    /// 增加活力点击事件
    /// </summary>
    //private void OnVitalityClick()
    //{
    //    UIBuyEnergyVitality.Instance.SetInfo(ActionPointType.Vitality);
    //    Control.ShowGUI(GameLibrary.UIBuyEnergyVitality);

    //}
    /// <summary>
    /// 刷新主城货显示
    /// </summary>
    /// <param name="moneyType"></param>
    /// <param name="money"></param>
    private void UpdateMoney(MoneyType moneyType, long money)
    {
        switch (moneyType)
        {
            case MoneyType.Gold:goldTxt.text = money.ToString(); break;
            case MoneyType.Diamond: jewelTxt.text = money.ToString(); break;
        }
    }
    /// <summary>
    /// 刷新主城行动点（体力 活力）显示
    /// </summary>
    /// <param name="type"></param>
    /// <param name="count"></param>
    private void UpdateActionPoint(ActionPointType type,int count)
    {
        switch (type)
        {
            //"[ff0000]" + "20" +"[-]" 
            case ActionPointType.Vitality: ;
                if (count > playerData.GetInstance().actionData.maxVitalityCount)
                {
                    vitalityTxt.text = "[00ffff]" + count.ToString() +"[-]" + "/" + playerData.GetInstance().actionData.maxVitalityCount;
                }
                else
                {
                    vitalityTxt.text = count.ToString() + "/" + playerData.GetInstance().actionData.maxVitalityCount;

                    playerData.GetInstance().UpdateCountdownTime(type);
                }
                break;
            case ActionPointType.Energy:
                Debug.Log(energyTxt);
                if (count > playerData.GetInstance().actionData.maxEnergyCount)
                {
                    energyTxt.text = "[5eaeff]" + count.ToString() + "[-]" + "/" + playerData.GetInstance().actionData.maxEnergyCount;
                }
                else
                {
                    energyTxt.text = count.ToString() + "/" + playerData.GetInstance().actionData.maxEnergyCount;
                    //playerData.GetInstance().UpdateCountdownTime(type);
                    //playerData.GetInstance().RefreshTime();
                }
                 break;
        }
    }
    /// <summary>
    /// 更新主城行动点上限（体力 活力）显示
    /// </summary>
    private void UpdateActionPointCeiling()
    {
        //if (playerData.GetInstance().baginfo.vitality > playerData.GetInstance().actionData.maxVitalityCount)
        //{
        //    vitalityTxt.text = "[00ffff]" + playerData.GetInstance().baginfo.vitality.ToString() + "[-]" + "/" + playerData.GetInstance().actionData.maxVitalityCount;
        //}
        //else
        //{
        //    vitalityTxt.text = playerData.GetInstance().baginfo.vitality.ToString() + "/" + playerData.GetInstance().actionData.maxVitalityCount;

        //    playerData.GetInstance().UpdateCountdownTime(ActionPointType.Vitality);
        //}
        if (playerData.GetInstance().baginfo.strength > playerData.GetInstance().actionData.maxEnergyCount)
        {
            if (energyTxt!=null)
            {
                energyTxt.text = "[5eaeff]" + playerData.GetInstance().baginfo.strength.ToString() + "[-]" + "/" + playerData.GetInstance().actionData.maxEnergyCount;
            }
            
        }
        else
        {
            if(energyTxt!=null)
            {
                energyTxt.text = playerData.GetInstance().baginfo.strength + "/" + playerData.GetInstance().actionData.maxEnergyCount;
            }
           
           // playerData.GetInstance().UpdateCountdownTime(ActionPointType.Energy);
        }
    }

    protected override void RegisterComponent()
    {
        base.RegisterComponent();
        RegisterComponentID(21, 25, goldBtn.gameObject);

    }
    protected override void RegisterIsOver()
    {
        base.RegisterIsOver();
    }
}

