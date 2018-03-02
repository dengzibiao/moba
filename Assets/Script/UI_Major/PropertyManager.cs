using UnityEngine;
using System.Collections;
using System;


/// <summary>
/// 体力活力倒计时管理
/// </summary>
public class PropertyManager : MonoBehaviour
{
    public DateTime dGameTime;//服务器时间
    private DateTime tommorrowTime;
    public TimeSpan oneEnergyRemainTime;//一次体力倒计时剩余时间差
    public TimeSpan allEnergyRemainTime;//所有体力倒计时剩余时间差
    public TimeSpan oneVitalityRemainTime;
    public TimeSpan allVitalityRemainTIme;
    public float energyTimer;
    public float vitalityTimer;
    public float onLinetimer = 0;

    private static PropertyManager instance;
    public TaskItem _taskitem;

    public static PropertyManager Instance
    {
        get { return instance; }
    }
    public PropertyManager()
    {
        instance = this;
    }
    void Awake()
    {
        instance = this;
       
        //ClientSendDataMgr.GetSingle().GetTaskSend().GetTaskListComplete(C2SMessageType.Active);
        //ClientSendDataMgr.GetSingle().GetTaskSend().GetTaskList(C2SMessageType.Active);
      //ClientSendDataMgr.GetSingle().GetTitleSend().SendGetTitleList(C2SMessageType.Active);//获取玩家称号列表
    }
    void Update()
    {
        //OnlineTimeAdd();
        //VitalityTimeCountDown();
        if (playerData.GetInstance().baginfo.strength < playerData.GetInstance().actionData.maxEnergyCount)
        {
            EnergyTimeCountDown();
        }
        //if(Input.GetKeyDown(KeyCode.N))
        //{
        //    Debug.Log("去杀怪");
        //    Debug.LogError(_taskitem.missionid);
        //    Debug.LogError(TaskManager.Single().TaskToSkillMonsterDic[_taskitem.missionid].taskId);
        //    ClientSendDataMgr.GetSingle().GetTaskSend().SendTaskSkillMonster(C2SMessageType.Active, TaskManager.Single().TaskToSkillMonsterDic[_taskitem.missionid].taskId, TaskManager.Single().TaskToSkillMonsterDic[_taskitem.missionid].opt6, TaskManager.Single().TaskToSkillMonsterDic[_taskitem.missionid].opt7);
        //}

        DareWaitingTime();
        SkillPointsCountdown();
    }

    //public void OnlineTimeAdd()
    //{
    //    onLinetimer += Time.deltaTime;
    //    if (onLinetimer > 60f)
    //    {
    //        playerData.GetInstance().singnData.onLineTime += 1;
    //        onLinetimer = 0;
    //    }
    //}
    /// <summary>
    /// 活力倒计时
    /// </summary>
    public void VitalityTimeCountDown()
    {
        vitalityTimer += Time.deltaTime;
        if (playerData.GetInstance().actionData.maxVitalityCount - playerData.GetInstance().baginfo.vitality > 0)
        {
            if (vitalityTimer > 1)
            {
                playerData.GetInstance().actionData.vitalityTime -= 1000;
                playerData.GetInstance().actionData.allVitalityTime -= 1000;
                vitalityTimer = 0;
            }

            if (playerData.GetInstance().actionData.vitalityTime <= 0)
            {
                //倒计时增加活力的时候 防止倒计时的过程中通过购买手段 当前活力值数量大于倒计时活力上线
                if (playerData.GetInstance().actionData.maxVitalityCount - playerData.GetInstance().baginfo.vitality > 0)
                {
                    playerData.GetInstance().ChangeActionPointHandler(ActionPointType.Vitality, 1);
                    //playerData.GetInstance().actionData.vitalityTime = 5000;
                    playerData.GetInstance().actionData.vitalityTime = (long)(playerData.GetInstance().actionData.vitalityTimeBucket * 60 * 1000);
                }
                //UIMoney.instance.ChangeVitality(1);

            }
        }
        else
        {
            playerData.GetInstance().actionData.vitalityTime = 0;
            playerData.GetInstance().actionData.allVitalityTime = 0;
        }
    }
    /// <summary>
    /// 体力倒计时
    /// </summary>
    public void EnergyTimeCountDown()
    {
        energyTimer += Time.deltaTime;
        //Debug.Log(maxEnergyCount + "_____体力_______"+ playerData.GetInstance().baginfo.strength);
        if (playerData.GetInstance().actionData.maxEnergyCount - playerData.GetInstance().baginfo.strength > 0)
        {
            if (energyTimer > 1)
            {
                playerData.GetInstance().actionData.energyTime -= 1000;
                playerData.GetInstance().actionData.allEnergyTime -= 1000;
                energyTimer = 0;
            }

            if (playerData.GetInstance().actionData.energyTime <= 0)
            {
                //UIMoney.instance.ChangeStrength(1);
                //playerData.GetInstance().ChangeActionPointHandler(ActionPointType.Energy, 1);
                //Debug.LogError("体力自动增长了一");
                //playerData.GetInstance().actionData.energyTime = 6000;
                //playerData.GetInstance().actionData.energyTime = (long)(playerData.GetInstance().actionData.energyTimeBucket * 60 * 1000);
            }
        }
        else
        {
            playerData.GetInstance().actionData.energyTime = 0;
            playerData.GetInstance().actionData.allEnergyTime = 0;
        }
    }
    /// <summary>
    /// 游戏时间显示
    /// </summary>
    /// <param name="dTime"></param>
    /// <returns></returns>
    public string GetGameTime(DateTime dTime)
    {
        string hour = Convert.ToDateTime(dTime).ToString("HH");
        string minute = Convert.ToDateTime(dTime).ToString("mm");
        return Convert.ToDateTime(dTime).ToString("HH:mm:ss");
    }
    /// <summary>
    /// 将Unix时间戳转换为DateTime类型时间
    /// </summary>
    /// <param name="d">double型数字</param>
    /// <returns></returns>
    public static DateTime ConvertIntDateTime(long d)
    {
        DateTime time = System.DateTime.MinValue;
        DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
        time = startTime.AddSeconds(d);

        return time;
    }
    ////倒计时显示
    //public string GetTimeSpanTime(TimeSpan span)
    //{
    //    return string.Format("{0:d2}:{1:d2}:{2:d2}", (int)Math.Floor(span.TotalHours), span.Minutes, span.Seconds);
    //}
    ////活力倒计时
    //void VitalityTimeCountDown()
    //{
    //    if (tommorrowTime > dGameTime)
    //    {
    //        oneVitalityRemainTime = tommorrowTime - dGameTime;

    //        Debug.Log(GetTimeSpanTime(oneVitalityRemainTime));
    //        //OnPlayerInfoChanged(InfoType.All);
    //    }
    //    else
    //    {
    //        UIMoney.instance.ChangeVitality(1);
    //        tommorrowTime = dGameTime.Add(oneVitalityRemainTime);
    //        VitalitymillisecondConvert();
    //        //oneVitalityRemainTime = new TimeSpan(0, 0, 0, 0);
    //    }
    //}
    ////体力倒计时
    //void EnergyTimeCountDown()
    //{
    //    if (tommorrowTime > dGameTime)
    //    {
    //        oneEnergyRemainTime = tommorrowTime - dGameTime;
    //        Debug.Log(GetTimeSpanTime(oneEnergyRemainTime));
    //        //OnPlayerInfoChanged(InfoType.All);
    //    }
    //    else
    //    {
    //        oneEnergyRemainTime = new TimeSpan(0, 0, 0, 0);
    //    }
    //}

    ////寿命倒计时毫秒转换成时间段timespan
    //private void VitalitymillisecondConvert()
    //{
    //    long day = 0;
    //    long hour = vitalityTime / (60 * 60 * 1000);
    //    long minute = (vitalityTime - hour * 60 * 60 * 1000) / (60 * 1000);
    //    long second = (vitalityTime - hour * 60 * 60 * 1000 - minute * 60 * 1000) / 1000;
    //    if (second >= 60)
    //    {
    //        minute += second / 60;
    //        second = second % 60;
    //    }
    //    if (minute >= 60)
    //    {
    //        hour += minute / 60;
    //        minute = minute % 60;
    //    }
    //    if (hour >= 24)
    //    {
    //        day += hour / 60;
    //        hour = hour % 24;
    //    }
    //    oneVitalityRemainTime = new TimeSpan((int)day, (int)hour, (int)minute, (int)second);// 

    //}

    //public delegate void OnVitalityChangedEvent();
    //public event OnVitalityChangedEvent OnVitalityChanged;
    //void VitalityChanged()
    //{

    //}


    #region 角斗场等待倒计时

    public delegate void OnCDTiming(float time);
    public OnCDTiming OnCD;
    public OnCDTiming OnSkillCD;

    float DareTime = 0;

    bool isTiming = false;
    public bool _isTiming
    {
        get { return isTiming; }
    }

    public void StarTiming()
    {
        DareTime = 300f;
        isTiming = true;
        OnCD = null;
    }

    public void EndTiming()
    {
        DareTime = 0f;
        isTiming = false;
        OnCD = null;
    }

    /// <summary>
    /// 挑战等待时间
    /// </summary>
    void DareWaitingTime()
    {
        return;
        if (!isTiming) return;
        DareTime -= Time.deltaTime;
        if (null != OnCD)
            OnCD(DareTime);

        if (DareTime <= 0)
        {
            isTiming = false;
            OnCD = null;
        }
    }

    #endregion

    #region 技能升级技能点

    float skillTime = 5f;

    void SkillPointsCountdown()
    {

        if (playerData.GetInstance().skillPoints >= 20)
        {
            skillTime = 5;
            if (null != OnSkillCD)
                OnSkillCD(-999);
            return;
        }

        skillTime -= Time.deltaTime;
        if (null != OnSkillCD)
            OnSkillCD(skillTime);

        if (skillTime < 0)
        {
            playerData.GetInstance().skillPoints++;
            skillTime = 5;
        }

    }

    #endregion

}
