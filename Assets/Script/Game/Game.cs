/*
文件名（File Name）:   Game.cs

作者（Author）:    #高#

创建时间（CreateTime）:  2017-1-3 15:43:26
*/
using UnityEngine;
using System.Collections;
using System;

public class Game : MonoBehaviour, IGame
{
    private static Game gameInstance = null;
    private GameObject uiRoot = null;
    private GameObject parent = null;
    private bool isblock = false;//设置消息阻塞
    public static Game Instance
    {
        get
        {
            return gameInstance;
        }
    }
    public EnumObjectState State
    {
        get
        {
            throw new NotImplementedException();
        }
    }

    public GameObject UiRoot
    {
        get
        {
            return GetUIRoot();
        }
    }

    public bool Isblock
    {
        get { return isblock; }
        set { isblock = value; }
    }


    /// <summary>
    /// 如果父级不是UIRooT需要传进来
    /// </summary>
    /// <param name="isUIRoot"></param>
    /// <returns></returns>
    public void SetParent(GameObject parent = null)
    {
        this.parent = parent;
    }

    private GameObject GetUIRoot()
    {
        GameObject go = null;
        if (parent != null)
            go = parent;
        parent = null;
        if (go != null)
        {
            return go;
        }
        try
        {
            if (FindObjectOfType<UIRoot>() != null)
            {
                uiRoot = FindObjectOfType<UIRoot>().gameObject;
                return uiRoot;
            }
            else
            {
                uiRoot = GameObject.Find("UI Root").gameObject;
                if (uiRoot == null)
                {
                    uiRoot = GameObject.Find("UI Root(Clone)");
                    if (uiRoot != null) return uiRoot;
                }
                else
                {
                    return uiRoot;
                }
            }
        }
        catch (Exception)
        {


        }
        return null;
    }
    public void Exit()
    {
        throw new NotImplementedException();
    }

    public void Run()
    {
        throw new NotImplementedException();
    }
    void Awake()
    {
        if (gameInstance == null)
        {
            gameInstance = this;
        }
    }

    void Update()
    {
        if (!Isblock)
        {
            if (Singleton<Notification>.Instance.MessageListCount().Count > 0)
            {
                Singleton<Notification>.Instance.ReceiveHandle(Singleton<Notification>.Instance.MessageListCount()[0]);
                Singleton<Notification>.Instance.MessageListCount().RemoveAt(0);
            }
        }
        //Singleton<GUIManager>.Instance.buttonClick = Time.realtimeSinceStartup;//临时用

    }
}
