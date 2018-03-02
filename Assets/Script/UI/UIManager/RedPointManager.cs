/*
文件名（File Name）:   RedPointManager.cs

作者（Author）:    #高#

创建时间（CreateTime）:  2016-12-27 16:23:10
*/

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RedPointManager
{
    /// <summary>
    /// 用于红点的状态改变通知事件只用于大类
    /// </summary>
    /// <param name="redList"></param>
    public delegate void ChangeRed(Dictionary<int, List<int>> redList);
    public event ChangeRed NotifyRedChangeEvent;
    private Dictionary<int, List<int>> redList = new Dictionary<int, List<int>>();
    /// <summary>
    /// 服务器返回数据添加到字典中或用于客户端更新字典状态
    /// </summary>
    /// <param name="key"></param>
    /// <param name="rd"></param>
    public void Add(EnumRedPoint key, List<int> rd)
    {
        if (GetunlockFunction((int)key) != -1)
        {
            if (DataDefine.isSkipFunction || FunctionOpenMng.GetInstance().GetFunctionOpen(GetunlockFunction((int) key)))
            {
                if (!redList.ContainsKey((int)key))
                    redList.Add((int)key, rd);
                else
                {
                    //List<int> ls = null;
                    //redList.TryGetValue((int)key, out ls);
                    //ls = rd;
                    if (rd != null)
                    {
                        redList[(int)key] = rd;
                    }
                }
            }
        }
        else
        {
            if (!redList.ContainsKey((int)key))
                redList.Add((int)key, rd);
            else
            {
                //List<int> ls = null;
                //redList.TryGetValue((int)key, out ls);
                //ls = rd;
                if (rd!=null)
                {
                    redList[(int)key] = rd;
                }
               
            }
        }
     
    }

    private int GetunlockFunction(int key)
    {
        int a=-1;
        switch (key)
        {
            case 1:
                break;
            case 2:
                break;
            case 3:
                a = 13;
                break;
            case 4:
                a = 33;
                break;
            case 5:
                a = 12;
                break;
            case 6:
                a = 1;
                break;
            case 7:
                a = 8;
                break;
            case 8:
                a = 20;
                break;
            case 9:
                a = 39;
                break;
            case 10:
                a = 37;
                break;
        }
        return a;
    }
    /// <summary>
    /// 添加子项的标记
    /// </summary>
    /// <param name="key"></param>
    /// <param name="childFlag"></param>
    public void AddChildFlag(EnumRedPoint key, int childFlag)
    {
        List<int> ls = null;
        if (redList.ContainsKey((int)key))
        {
            redList.TryGetValue((int)key, out ls);
            bool isHave = false;
            if (ls != null)
            {
                for (int i = 0; i < ls.Count; i++)
                {
                    if (ls[i] == childFlag)
                    {
                        isHave = true;
                        break;
                    }
                }
                if (!isHave)
                {
                    ls.Add(childFlag);
                }
            }
            else
            {
                redList.Remove((int)key);
                ls = new List<int>();
                ls.Add(childFlag);
                Add(key, ls);
            }


        }
        else
        {
            ls = new List<int>();
            ls.Add(childFlag);
            Add(key, ls);
            NotifyChange();
        }

    }
    /// <summary>
    /// 从列表中移除整个大类
    /// </summary>
    /// <param name="key"></param>
    /// <param name="rd"></param>
    public void DeletType(EnumRedPoint key)
    {
        if (redList.ContainsKey((int)key))
        {
            redList.Remove((int)key);
            NotifyChange();
        }
    }
    /// <summary>
    /// 删除子项的标记
    /// </summary>
    /// <param name="key"></param>
    /// <param name="childFlag"></param>
    public void DeletChildFlag(EnumRedPoint key, int childFlag)
    {
        if (redList.ContainsKey((int)key))
        {
            List<int> ls = null;
            redList.TryGetValue((int)key, out ls);
            if (ls != null)
            {
                int isHave = -1;
                for (int i = 0; i < ls.Count; i++)
                {
                    if (ls[i] == childFlag)
                    {
                        isHave = i;
                        break;
                    }
                }
                if (isHave!=-1)
                {
                    ls.RemoveAt(isHave);
                }
                if (ls.Count < 1)
                {
                    DeletType(key);
                }
            }
            else
            {
                DeletType(key);
            }

        }

    }
    /// <summary>
    /// 获取字典
    /// </summary>
    /// <returns></returns>
    public Dictionary<int, List<int>> GetRedList()
    {
        return redList;
    }
    /// <summary>
    /// 获取大类里子项的所有标记返回boollist按传进来的顺序反回自己区分.没有匹配反回null
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public List<bool> GetChildList(EnumRedPoint key, params int[] ChildKey)
    {
        if (ChildKey.Length <= 0) return null;
        List<int> ls = null;
        List<bool> isShow = null;
        if (redList.ContainsKey((int)key))
        {
            redList.TryGetValue((int)key, out ls);
        }
        if (ChildKey.Length > 0 && ls != null)
        {
            isShow = new List<bool>();
            for (int i = 0; i < ChildKey.Length; i++)
            {
                bool isHave = false;
                for (int j = 0; j < ls.Count; j++)
                {
                    if (ChildKey[i] == ls[j])
                    {
                        isHave = true;
                        break;
                    }
                }

                if (isHave)
                {
      
                    isShow.Add(true);
                }
                else
                {
                    isShow.Add(false);
                }
            }
            return isShow;
        }
        else
        {
            return null;
        }
    }
    /// <summary>
    /// 获取大类里面单一子项的标记列表.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool GetChildList(EnumRedPoint key, int childKey)
    {
        List<int> ls = null;
        if (redList.ContainsKey((int)key))
        {
            redList.TryGetValue((int)key, out ls);
            if (ls != null)
            {
                bool isHave = false;
                for (int i = 0; i < ls.Count; i++)
                {
                    if (ls[i] == childKey)
                    {
                        isHave = true;
                    }
                }
                if (isHave)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }


    }
    /// <summary>
    /// 红点变更通知
    /// </summary>
    public void NotifyChange()
    {
        //if (NotifyRedChangeEvent != null && Singleton<SceneManage>.Instance.Current == EnumSceneID.UI_MajorCity01&& Singleton<SceneManage>.Instance.State== EnumObjectState.Ready)
        //{
        //    if (Control.GetGUI(GameLibrary.UISetting) != null)
        //    {
        if (UI_Setting.GetInstance() != null)
        {
            UI_Setting.GetInstance().SetMainSettingRed(redList);
        }
        //         NotifyRedChangeEvent(redList);
        //    }
        //}
    }

    public void Clear()
    {
        redList.Clear();
    }
}
