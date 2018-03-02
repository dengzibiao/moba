/*
文件名（File Name）:   Singleton.cs

作者（Author）:    #高#

创建时间（CreateTime）:  #CreateTime#
*/
using UnityEngine;
using System;
using System.Collections;
using System.Reflection;

/// <summary>
/// 泛型单例类
/// </summary>
/// <typeparam name="T"></typeparam>
public class Singleton<T> where T : new()
{
    private static T instance;
    private static readonly object oSync = new object();
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                //这里加个锁，如果在多线程情况下同时调用类将引起逻辑错误
                lock (oSync)
                {
                    if (instance == null)
                    {
                        instance = new T();
                    }
                }
            }
            return instance;
        }
    }
}

