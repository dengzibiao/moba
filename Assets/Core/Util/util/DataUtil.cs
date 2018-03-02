using UnityEngine;
using System.Collections;

public enum TimeType
{
    Day,
    Hour,
    Minute,
    second,
    All
}

public class DataUtil : MonoBehaviour
{

    /// <summary>
    /// 根据秒数获取时间字符串
    /// </summary>
    /// <param name="second"></param>
    /// <returns></returns>
    public static string GetTimeString(long second, TimeType timeType)
    {
        long minute = (second / 60) % 60;
        long hour = (second / 3600) % 24;
        long day = second / (3600 * 24);
        long sec = second % 60;
        string timeStr = "";

        bool hasD = false, hasH = false, hasM = false;//, hasS = false;

        if(day > 0)
        {
            hasD = true;
            timeStr += day + "天";
            if(timeType == TimeType.Day)
            {
                return day + "天";
            }
        }
        if(!hasD && hour == 0)
        {

        }
        else
        {
            hasH = true;
            if(hour <= 9 && hasD == true && hour > 0)
            {
                timeStr += "0" + hour + "时";
            }
            else if(hour > 0)
            {
                timeStr += hour + "时";
            }
            if(timeType == TimeType.Hour)
            {
                return hour + "时";
            }
        }
        if(!hasD)
        {
            if(!hasD && !hasH && minute == 0)
            {
            }
            else
            {
                hasM = true;
                if(minute <= 9 && hasH == true && minute > 0)
                {
                    timeStr += "0" + minute + "分";
                }
                else if(minute > 0)
                {
                    timeStr += minute + "分";
                }
                if(timeType == TimeType.Minute)
                {
                    return minute + "分";
                }
            }
        }
        if(!hasH)
        {
            if(!hasD && !hasH && !hasM && sec == 0)
            {
            }
            else
            {
               // hasS = true;
                if(sec <= 9 && hasM == true && sec > 0)
                {
                    timeStr += "0" + sec + "秒";
                }
                else if(sec > 0)
                {
                    timeStr += sec + "秒";
                }
                if(timeType == TimeType.second)
                {
                    return sec + "秒";
                }
            }
        }
        return timeStr;
    }
}
