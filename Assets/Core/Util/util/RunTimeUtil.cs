using System;
using System.Collections.Generic;
using UnityEngine;

public enum RunState
{
    LOOP, IMMEDIATELY
}
public class RunTimeUtil : MonoBehaviour
{
    private bool _repeat = false;
    private List<TimeVo> list_Vo = new List<TimeVo>();
    private static RunTimeUtil instance;
    public static RunTimeUtil Instance
    {
        get
        {
            if(instance == null)
            {
                GameObject timer = new GameObject("Timers");
                timer.AddComponent<RunTimeUtil>();
                DontDestroyOnLoad(timer);
            }
            return instance;
        }
    }
    void Awake()
    {
        instance = this;
    }
    void Update()
    {
        if(_repeat)
        {
            Repeat();
        }
    }
    /// <summary>
    /// callBack:回调函数,millisecond:间隔时间,count:执行次数,state:循环类型
    /// </summary>
    public void InAction(Action callBack, int millisecond, int count = -1, RunState state = RunState.LOOP)
    {
        if (list_Vo.Count == 0) { _repeat = true; }
        list_Vo.Add(new TimeVo(callBack, millisecond, count, state));
    }

    public void OutAction(Action callBack)
    {
        foreach(TimeVo vo in list_Vo)
        {
            if(vo.callBack == callBack)
            {
                list_Vo.Remove(vo);
                break;
            }
        }
    }

    private void Repeat()
    {
        for(int i = 0; i < list_Vo.Count; i++)
        {
            if(Time.time * 1000 - list_Vo[i].recordTimer >= list_Vo[i].millisecond)
            {

                list_Vo[i].callBack();
                list_Vo[i].count--;
                list_Vo[i].recordTimer = Time.time * 1000;
            }
        }

        for(int i = list_Vo.Count - 1; i >= 0; i--)
        {
            if(list_Vo[i].count == 0)
            {
                list_Vo.RemoveAt(i);
            }
        }

        if(list_Vo.Count == 0)
        {
            _repeat = false;
        }
    }
    public class TimeVo
    {
        public TimeVo(Action _callBack, int _millisecond, int _count, RunState _state)
        {
            callBack = _callBack;
            millisecond = _millisecond;
            state = _state;
            count = _count;
            recordTimer = Time.time * 1000;

            if (_state == RunState.IMMEDIATELY) { count--; callBack(); }
        }

        public Action callBack { set; get; }
        public int millisecond { set; get; }
        public RunState state { set; get; }
        public int count { set; get; }
        public float recordTimer { set; get; }
    }
}
