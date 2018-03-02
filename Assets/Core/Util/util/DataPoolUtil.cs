using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ClearState
{
    Random,All
}
public class DataPoolUtil
{
    public const float TimerInterval = 0.1f;

    private static DataPoolUtil instance;

    private Dictionary<string, DataPool> dic = new Dictionary<string, DataPool>();
    
    public static DataPoolUtil Instance()
    {
        if (instance == null) instance = new DataPoolUtil();
        return instance;
    }

    public void SetData(string key,DataPool value)
    {
        if (dic.ContainsKey(key)){ dic.Remove(key); }
        dic.Add(key, value);
    }

    public DataPool? GetData(string key)
    {
        DataPool V;
        if (dic.TryGetValue(key, out V))
        {
            return V;
        }
        return null;
    }

    public void Clear(ClearState state=ClearState.Random,params string[] keys)
    {
        if (state== ClearState.Random)
        {
            foreach (string key in keys)
            {
                DataPool V;
                if (dic.TryGetValue(key, out V))
                {
                    V.Clear();
                    dic.Remove(key);
                }
                PlayerPrefs.DeleteKey(key);
            }
        }
        else
        {
            dic.Clear();
            PlayerPrefs.DeleteAll();
        }
        
    }

    public void Save()
    {
        foreach (KeyValuePair<string ,DataPool> item in dic)
        {
            if (item.Value.intData>0)
            {
                PlayerPrefs.SetInt(item.Key, item.Value.intData);
            }
            else if (item.Value.stringData != null)
            {
                PlayerPrefs.SetString(item.Key, item.Value.stringData);
            }
            else if (item.Value.floatData > 0)
            {
                PlayerPrefs.SetFloat(item.Key, item.Value.floatData);
            }
        }
    }
}
public struct DataPool
{
    
    public int intData;
    public string stringData;
    public float floatData;
    
    public DataPool(int key)
    {
        intData = key;
        stringData = "";
        floatData = -99999;
    }

    public DataPool(string key)
    {
        intData = -99999;
        stringData = key;
        floatData = -99999;
    }

    public DataPool(float key)
    {
        intData = -99999;
        stringData = "";
        floatData = key;
    }

    public void Clear()
    {
        intData = 0;
        stringData = "";
        floatData = 0;
    }
}

