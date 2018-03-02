using UnityEngine;

public class CDTimer : MonoBehaviour
{
    public delegate void CdDelegate ( int count, long id);
    static CDTimer instance;
    BetterList<CD> cds = new BetterList<CD>();

    static long timerId;

    bool isRunCD = true;

    public class CD
    {
        public long Id;
        public float Start;
        public float Interval;
        public float Elapsed;
        public int CountElapsed;        // 已完成次数
        public int CountLeft;           // 剩余次数
        public bool IsRealTime;         // 是否实时(实时不可暂停，非实时可暂停)
        public bool IsRun = true;       // 是否在运行
        public bool IsCountDown = true; // 是否是倒计时
        public CdDelegate OnCd;         // 每次到达计时点回调
        public CdDelegate OnRemove;     // 被移除时回调（手动或自动移除）
    }

    public static CDTimer GetInstance ()
    {
        if(instance == null)
        {
            instance = new GameObject().AddComponent<CDTimer>();
            instance.name = "CDTimer";
        }
        return instance;
    }

    void FixedUpdate ()
    {
        for(int i = cds.size - 1; i >= 0; i--)
        {
            if(!cds[i].IsRealTime && isRunCD && cds[i].IsRun)
            {
                cds[i].Elapsed += Time.fixedDeltaTime;
                if(cds[i].Elapsed >= cds[i].Interval && cds[i].CountLeft > 0)
                {
                    cds[i].Elapsed -= cds[i].Interval;
                    RunCD(cds[i]);
                }
            }
        }
    }

    void Update ()
    {
        for(int i = cds.size - 1; i >= 0; i--)
        {
            if(cds[i].IsRealTime)
            {
                if(Time.realtimeSinceStartup > cds[i].Start + cds[i].Interval && cds[i].CountLeft > 0)
                {
                    cds[i].Start = Time.realtimeSinceStartup;
                    RunCD(cds[i]);
                }
            }
        }
    }

    /// <summary>
    /// 增加一个计时器
    /// </summary>
    /// <param name="duration">计时长度</param>
    /// <param name="cdCallback">到达计时点回调</param>
    /// <param name="count">计时次数</param>
    /// <param name="doInstant">是否马上执行一次回调</param>
    /// <param name="isRealTime">是否实时（不可暂停）</param>
    /// <param name="isCountDown">是否倒计时（倒计时回调返回剩余次数，正计时回调返回完成次数）</param>
    /// <returns>返回值：计时器</returns>
    public CD AddCD ( float duration, CdDelegate cdCallback, int count = 1 , bool doInstant = false, bool isRealTime = false, bool isCountDown  = true)
    {
        if(duration != 0f && duration < Time.fixedDeltaTime)
        {
            Debug.LogError("CD duration Too Short!");
        }
        CD cd = new CD();
        timerId++;
        cd.Id = timerId;
        cd.Start = Time.realtimeSinceStartup;
        cd.Elapsed = 0f;

        cd.Interval = duration;
        cd.CountLeft = count;
        cd.IsRealTime = isRealTime;
        cd.IsCountDown = isCountDown;
        cd.OnCd += cdCallback;
        cds.Add(cd);
        if(doInstant) RunCD(cd);
        return cd;
    }

    void RunCD (CD cd)
    {
        cd.CountLeft--;
        cd.CountElapsed++;
        if(cd.OnCd != null)
        {
            if(cd.IsCountDown)
                cd.OnCd(cd.CountLeft, cd.Id);
            else
                cd.OnCd(cd.CountElapsed, cd.Id);
        }

        if(cd.CountLeft <= 0) RemoveCD(cd);
    }

    public CD GetCD ( long id )
    {
        for(int i = 0; i<cds.size; i++)
        {
            if(cds[i].Id == id)
                return cds[i];
        }
        return null;
    }

    public void RemoveCD ( CD cd)
    {
        if(cds.Contains(cd))
        {
            cds.Remove(cd);
            if(cd.OnRemove != null)
                cd.OnRemove(cd.CountLeft, cd.Id);
            if(cd.OnCd != null)
                cd.OnCd = null;
            if(cd != null)
                cd = null;
        }
    }

    public static string FormatToMMSS (int seconds)
    {
        int minutes = seconds / 60;
        int secondsLeft = seconds % 60;
        return GetTwoDigits(minutes) + ":" + GetTwoDigits(secondsLeft);
    }

    static string GetTwoDigits ( int digit)
    {
        return digit < 10 ? "0" + digit : digit.ToString();
    }

    /// <summary>
    /// 暂停或运行所有非实时的计时器
    /// </summary>
    /// <param name="isRun"></param>
    public void CDRunOrStop(bool isRun)
    {
        isRunCD = isRun;
    }
}
