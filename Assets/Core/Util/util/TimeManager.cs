using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TimeManager : MonoBehaviour
{
    private static TimeManager mInstance;

    public static TimeManager Instance
    {
        get { return mInstance; }
    }

    public delegate void Interval();

    private Dictionary<Interval, float> mDicinterval = new Dictionary<Interval, float>();
    private bool isStop = false;
    //Awake is called when the script instance is being loaded. 

    void Awake()
    {
        mInstance = this;
    }

    void Start()
    {
        StartTimer(DataPoolUtil.TimerInterval);
    }

    public void StartTimer(float value)
    {
        InvokeRepeating("Run", 0, value);
    }

    public void StopTimer()
    {
        CancelInvoke("Run");
    }

    public void AddTimerEvent(Interval interval, float time)
    {
        if (null != interval)
            mDicinterval[interval] = Time.time + time;
    }

    private void RemoveTimerEvent(Interval interval)
    {
        if (null != interval)
        {
            if (mDicinterval.ContainsKey(interval))
            {
                mDicinterval.Remove(interval);
            }
        }
    }

    private void StopTimerEvent(Interval interval)
    {
        if (mDicinterval.ContainsKey(interval) && interval != null)
            isStop = true;
    }

    private void ResumeTimerEvent(Interval interval)
    {
        if (mDicinterval.ContainsKey(interval) && interval != null)
            isStop = false;
    }

    void Run()
    {
        if (mDicinterval.Count > 0)
        {
            List<Interval> remove = new List<Interval>();
            foreach (KeyValuePair<Interval, float> KeyValue in mDicinterval)
            {
                if (KeyValue.Value <= Time.time)
                {
                    remove.Add(KeyValue.Key);
                }
            }
            for (int i = 0; i < remove.Count; i++)
            {
                if (isStop) continue;
                remove[i]();
                RemoveTimerEvent(remove[i]);
            }
        }
    }

    #region 获取时间格式

    /// <summary>
    /// 获取时间格式 00:00:00
    /// </summary>
    /// <returns>The time clock text.</returns>
    /// <param name="timeSecond">传入秒</param>
    public string GetTimeClockText(long timeSecond)
    {
        if (timeSecond <= 0)
        {
            return "00:00:00";
        }
        //string str;
        DateTime dateTime = new DateTime(timeSecond * 10000000);

        // Debug.Log("dateTime.Hour" + dateTime.Hour);

        int day = dateTime.Day - 1;
        int hour = dateTime.Hour;
        int min = dateTime.Minute;
        int sec = dateTime.Second;
        hour += day * 24;
        string strHour = "00";
        if (hour > 0)
        {
            if (hour >= 10)
            {
                strHour = "" + hour;
            }
            else
            {
                strHour = "0" + hour;
            }
        }
        string strMin = "00";
        if (min > 0)
        {
            if (min >= 10)
            {
                strMin = "" + min;
            }
            else
            {
                strMin = "0" + min;
            }
        }
        string strSec = "00";
        if (sec > 0)
        {
            if (sec >= 10)
            {
                strSec = "" + sec;
            }
            else
            {
                strSec = "0" + sec;
            }
        }
        //switch (timeType)
        //{
        //    case TimeType.All:return strHour + ":" + strMin + ":" + strSec;
        //    case TimeType.Minute:return strMin + ":" + strSec;
        //    case TimeType.second:return strSec;
        //}
        //return null;
        return strHour + ":" + strMin + ":" + strSec;
    }

    /// <summary>
    /// 获取时间格式 00:00:00
    /// </summary>
    /// <returns>The time clock text.</returns>
    /// <param name="timeSecond">传入毫秒</param>
    public string GetMilliTimeClockText(long timeSecond)
    {
        if (timeSecond <= 0)
        {
            return "00:00:00";
        }
        //string str;
        DateTime dateTime = new DateTime(timeSecond * 10000);
        int day = dateTime.Day - 1;
        int hour = dateTime.Hour;
        int min = dateTime.Minute;
        int sec = dateTime.Second;
        hour += day * 24;
        string strHour = "00";
        if (hour > 0)
        {
            if (hour >= 10)
            {
                strHour = "" + hour;
            }
            else
            {
                strHour = "0" + hour;
            }
        }
        string strMin = "00";
        if (min > 0)
        {
            if (min >= 10)
            {
                strMin = "" + min;
            }
            else
            {
                strMin = "0" + min;
            }
        }
        string strSec = "00";
        if (sec > 0)
        {
            if (sec >= 10)
            {
                strSec = "" + sec;
            }
            else
            {
                strSec = "0" + sec;
            }
        }
        return strHour + ":" + strMin + ":" + strSec;
    }
    /// <summary>
    /// 获取时间的完整形式 年月日 - 时分秒
    /// </summary>
    /// <param name="timeSecond"></param>
    /// <returns></returns>
    public string GetAllTimeClockText(long timeSecond)
    {
        DateTime time = System.DateTime.MinValue;
        DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
        time = startTime.AddSeconds(timeSecond);
        return time.ToString("yyyy.MM.dd HH:mm:ss");
    }


    /// <summary>
    /// 将Unix时间戳转换为DateTime类型时间
    /// </summary>
    /// <param name="d">double型数字</param>
    /// <returns></returns>
    public DateTime ConvertIntDateTime(long d)
    {
        DateTime time = System.DateTime.MinValue;
        DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
        time = startTime.AddSeconds(d);
        return time;
    }
    #endregion

    /// <summary>
    /// 时间判断 当前时间大于给定的时间返回1，等于返回0，小于返回-1
    /// </summary>
    /// <param name="givenTime">给定时间，</param>
    /// <returns></returns>
    public int CheckTimeIsNowadays(string givenTime, bool isyMd, bool isRetDay = false)
    {
        DateTime currentDate = ConvertIntDateTime(Auxiliary.GetNowTime());
        DateTime nextDate = DateTime.ParseExact(givenTime, isyMd ? "yyMMdd" : "yyMMddHHmmss", new System.Globalization.CultureInfo("zh-CN", true));
        if (isRetDay)
        {
            TimeSpan sp = currentDate - nextDate;
            return (int)sp.TotalDays;
        }
        return nextDate.CompareTo(currentDate);
    }

    public TimeSpan CheckTimeNowadays(string givenTime, bool isyMd)
    {
        DateTime currentDate = TimeManager.Instance.ConvertIntDateTime(Auxiliary.GetNowTime());
        DateTime nextDate = DateTime.ParseExact(givenTime, isyMd ? "yyMMdd" : "yyMMddHHmmss", new System.Globalization.CultureInfo("zh-CN", true));
        TimeSpan sp = currentDate - nextDate;
        return sp;
    }

}
