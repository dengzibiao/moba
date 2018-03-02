using UnityEngine;
using System.Collections;
using System;
public enum ActionPointType
{
    Energy = 0,
    Vitality = 1
}
public class UICountdownPanel : GUIBase
{
    public GUISingleLabel currentTime;
    public GUISingleLabel buyCount;
    public GUISingleLabel nextTime;
    public GUISingleLabel allTime;
    public GUISingleLabel time;
    public ActionPointType type;
    public UILabel singleLabel;
    public UILabel allLabel;
    public static UICountdownPanel instance;
    public static UICountdownPanel Instance { get { return instance; } set { instance = value; } }
    public UICountdownPanel()
    {
        instance = this;
    }

    public override UIPanleID GetUIKey()
    {
        return UIPanleID.UICountdownPanel;
    }

    protected override void Init()
    {
        instance = this;
        base.Init();
        //currentTime =transform.Find("CurrentTime").GetComponent<GUISingleLabel>();
        //buyCount = transform.Find("BuyCount").GetComponent<GUISingleLabel>();
        //nextTime = transform.Find("NextTime").GetComponent<GUISingleLabel>(); 
        //allTime = transform.Find("AllTime").GetComponent<GUISingleLabel>();
        //time = transform.Find("Time").GetComponent<GUISingleLabel>(); 
        singleLabel = transform.Find("Empty/SingleRecoverLabel").GetComponent<UILabel>();
        allLabel = transform.Find("Empty/AllRecoverLabel").GetComponent<UILabel>();
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
        switch (this.type)
        {
            case ActionPointType.Vitality:
                //读取一下信息 TODO
                //Debug.Log(TimeManager.Instance.GetMilliTimeClockText(PropertyManager.Instance.vitalityTime));
                //nextTime.text = PropertyManager.Instance.GetTimeSpanTime(PropertyManager.Instance.oneVitalityRemainTime);
                nextTime.text = TimeManager.Instance.GetMilliTimeClockText(playerData.GetInstance().actionData.vitalityTime);
                allTime.text = TimeManager.Instance.GetMilliTimeClockText(playerData.GetInstance().actionData.allVitalityTime);
                buyCount.text = playerData.GetInstance().actionData.vitalityBuyTimes.ToString();
                time.text = playerData.GetInstance().actionData.vitalityTimeBucket + "分钟";
                singleLabel.text = "下次活力恢复";
                allLabel.text = "恢复全部活力";
                break;
            case ActionPointType.Energy:
                nextTime.text = TimeManager.Instance.GetMilliTimeClockText(playerData.GetInstance().actionData.energyTime);
                allTime.text = TimeManager.Instance.GetMilliTimeClockText(playerData.GetInstance().actionData.allEnergyTime);
                buyCount.text = playerData.GetInstance().actionData.energyBuyTimes.ToString();
                time.text = playerData.GetInstance().actionData.energyTimeBucket + "分钟";
                singleLabel.text = "下次体力恢复";
                allLabel.text = "恢复全部体力";
                break;
            default:
                break;
        }
    }
    public void SetInfo(ActionPointType type)
    {
        this.type = type;
    }
    void Update()
    {
        //currentTime.text = PropertyManager.Instance.GetGameTime(PropertyManager.Instance.dGameTime);
        currentTime.text = PropertyManager.Instance.GetGameTime(PropertyManager.ConvertIntDateTime(Auxiliary.GetNowTime()));
        switch (this.type)
        {
            case ActionPointType.Vitality:
                //读取一下信息 TODO
                //Debug.Log(TimeManager.Instance.GetMilliTimeClockText(PropertyManager.Instance.vitalityTime));
                //nextTime.text = PropertyManager.Instance.GetTimeSpanTime(PropertyManager.Instance.oneVitalityRemainTime);
                nextTime.text = TimeManager.Instance.GetMilliTimeClockText(playerData.GetInstance().actionData.vitalityTime);
                allTime.text = TimeManager.Instance.GetMilliTimeClockText(playerData.GetInstance().actionData.allVitalityTime);
                buyCount.text = playerData.GetInstance().actionData.vitalityBuyTimes.ToString();
                time.text = playerData.GetInstance().actionData.vitalityTimeBucket + "分钟";
                singleLabel.text = "下次活力恢复";
                allLabel.text = "恢复全部活力";
                break;
            case ActionPointType.Energy:
                nextTime.text = TimeManager.Instance.GetMilliTimeClockText(playerData.GetInstance().actionData.energyTime);
                allTime.text = TimeManager.Instance.GetMilliTimeClockText(playerData.GetInstance().actionData.allEnergyTime);
                buyCount.text = playerData.GetInstance().actionData.energyBuyTimes.ToString();
                time.text = playerData.GetInstance().actionData.energyTimeBucket + "分钟";
                singleLabel.text = "下次体力恢复";
                allLabel.text = "恢复全部体力";
                break;
            default:
                break;
        }
    }
}
