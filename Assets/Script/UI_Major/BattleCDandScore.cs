using UnityEngine;

public class BattleCDandScore : MonoBehaviour
{
    public static BattleCDandScore instance;
    public UILabel LaCD;
    public CDTimer.CD cd;

    void Awake()
    {
        instance = this;
    }

    public void StartCD(int total)
    {
        cd = CDTimer.GetInstance().AddCD(1f, RefreshCDLabel, total);
    }

    void RefreshCDLabel(int countLeft, long id )
    {
        LaCD.text = CDTimer.FormatToMMSS(countLeft);
    }

    public void CountTime()
    {
        TimerCounter.GetInstance().Show(LaCD, CDTimer.FormatToMMSS(TimerCounter.GetInstance().result));
    }

    public void SetTime ( bool isRun )
    {
        if(null != cd)
            cd.IsRun = isRun;
    }
}