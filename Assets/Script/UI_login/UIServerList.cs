/*
文件名（File Name）:   InitLogin.cs

作者（Author）:    #高#

创建时间（CreateTime）:  #CreateTime#
*/using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Tianyu;

public class UIServerList : GUIBase
{
    private static UIServerList instanse;

    public UIServerList()
    {
        instanse = this;
    }
    public GUISingleButton recentlyLoginBtn;
    public GUISingleCheckBox checkBox;
    private ServeData dater;//推荐服务器或上次登录
    public GUISingleMultList serverList;
    public GUISingleMultList serverAllList;
    public GUISingleLabel lastEnter;
    public GUISingleMultList roleList;
    private GUISingleSprite recentlyState;
    private Transform serverView;
    private Transform roleView;
    private GameObject go;//指定区服按钮高亮的GameObject
    private List<ServeData> serverData = new List<ServeData>();
    private object[] ServerDataList;
    public Transform loginEffect;

    public static UIServerList Instanse
    {
        get { return instanse; }
        set { instanse = value; }
    }

    protected override void Init()
    {
        serverView = transform.Find("ServerView").transform;
        roleView = transform.Find("RoleView").transform;
        recentlyState = recentlyLoginBtn.GetComponentInChildren<GUISingleSprite>();
        loginEffect = transform.parent.Find("Effect");
        recentlyLoginBtn.onClick = OnRecentlyLoginBtn; 
    }
    public override UIPanleID GetUIKey()
    {
        return UIPanleID.UI_ServerList;
    }

    protected override void OnLoadData()
    {
        base.OnLoadData();
        Show();
    }

    protected override void ShowHandler()
    {
        checkBox.isSelected = true;
        checkBox.onClick = OnRecommended;
        loginEffect.gameObject.SetActive(false);
        if (Globe.SelectedServer!=null)
        {
            recentlyLoginBtn.text = Globe.SelectedServer.name + "【" + Globe.SelectedServer.areaId + "服】";

            switch (Globe.SelectedServer.state)
            {
                case 0:
                    recentlyState.spriteName = "huidian";
                    break;
                case 2:
                    recentlyState.spriteName = "lvdian";
                    break;
                case 1:
                    recentlyState.spriteName = "hongdian";
                    break;
            }

        }
        serverList.InSize(ShowListCount(), 1);
        serverList.Info(ShowText());
        serverList.ScrollView = serverView;
        InitServerOrRole();
    }
    /// <summary>
    /// 点击推荐按钮显示服务器
    /// </summary>
    /// <param name="state"></param>
    private void OnRecommended(bool state)
    {
        if (state)
        {
            if(go!=null)
            go.SetActive(false);
            InitServerOrRole();
        }
    }

    /// <summary>
    ///有角色显示账号信息，没有角色显示服务器列表
    /// </summary>
    private void InitServerOrRole()
    {
        RoleItemData();//如果没有账号信息则执行Globe.isSelectServer只是当前服数据不能用于这里判断
       
        if (serverData.Count == 0)
        {
            lastEnter.text = "推荐服务器";
            if (go != null)
                go.SetActive(true);
            checkBox.isSelected = false;
            ShowAllList();
        }
        else//有信息
        {
            lastEnter.text = "最近登录";
            if (go != null)
                go.SetActive(false);
            ShowRoleList();
        }
    }
    /// <summary>
    /// 显示服务器列表
    /// </summary>
    private void ShowAllList()
    {
        serverAllList.gameObject.SetActive(true);
        roleList.gameObject.SetActive(false);
        serverAllList.InSize(ServerItemData().Length, 2);
        serverAllList.Info(ServerItemData());
        serverAllList.ScrollView = roleView;
    }
    /// <summary>
    /// 初始化时候调用
    /// </summary>
    /// <param name="go"></param>
    public void ChangeGoEffrct(GameObject go)
    {
        this.go = go;
        go.SetActive(true);
    }
    //显示指定的区服例如1-10区
    public void ShowSpecificServer(GameObject go,int a,int b)
    {
        this.go = go;
        checkBox.isSelected = false;
        serverAllList.gameObject.SetActive(true);
        roleList.gameObject.SetActive(false);
        if (serverMgr.GetInstance().serverlist.Count>=b+1)
        {
            serverAllList.InSize(b+1-a, 2);
            serverAllList.Info(ServerData(a,b));
        }else if (serverMgr.GetInstance().serverlist.Count>=a+1&& serverMgr.GetInstance().serverlist.Count<b+1)
        {
            serverAllList.InSize(serverMgr.GetInstance().serverlist.Count - a, 2);
            serverAllList.Info(ServerData(a, serverMgr.GetInstance().serverlist.Count-1));
        }
        serverAllList.ScrollView = roleView;
    }
    //所有服务器数据
    public object[] ServerData(int a,int b)
    {
        ServerDataList=new object[b+1];
        int index = 0;
        for (int i = a; i <= b; i++)
        {
            ServerDataList[index]= serverMgr.GetInstance().serverlist[i];
            index++;
        }
        return ServerDataList;
    }
    /// <summary>
    /// 显示账号内的角色信息
    /// </summary>
    private void ShowRoleList()
    {
        if (serverData.Count > 0)
        {
            roleList.gameObject.SetActive(true);
            serverAllList.gameObject.SetActive(false);
            roleList.InSize(RoleItemData().Length, 2);
            roleList.Info(RoleItemData());
            roleList.ScrollView = roleView;
        }
     
    }
    /// <summary>
    /// 账号内的角色信息数据
    /// </summary>
    /// <returns></returns>
    private object[] RoleItemData()
    {
        serverData.Clear();
        for (int i = 0; i < serverMgr.GetInstance().serverlist.Count; i++)
        {
            if (serverMgr.GetInstance().serverlist[i].playerId != 0)
            {      
                serverData.Add(serverMgr.GetInstance().serverlist[i]);
            }
        }
        return serverData.ToArray();
    }
    /// <summary>
    /// 服务器数据新服
    /// </summary>
    /// <returns></returns>
    private object[] ServerItemData()
    {
        if (serverData.Count == 0)
        {     
            for (int i = 0; i < serverMgr.GetInstance().serverlist.Count; i++)
            {
                if (serverMgr.GetInstance().serverlist[i].state == 1)
                {
                    serverData.Add(serverMgr.GetInstance().serverlist[i]);
                }
            }           
        }
        return serverData.ToArray();
    }
    /// <summary>
    /// 显示大区的个数
    /// </summary>
    /// <returns></returns>
    private int ShowListCount()
    {
        int count = serverMgr.GetInstance().serverlist.Count / 11 + 1;
        return count;
    }
    /// <summary>
    /// 计算显示的大区区间
    /// </summary>
    /// <returns></returns>
    private object[] ShowText()
    {
        object[] objList = new object[ShowListCount()];
        for (int i = 0; i < ShowListCount(); i++)
        {
            objList[i] = i + 1;
        }
        return objList;
    }
    /// <summary>
    /// 最近登录或新服按钮执行方法
    /// </summary>
    void OnRecentlyLoginBtn()
    {
        Control.HideGUI(this.GetUIKey());
        loginEffect.gameObject.SetActive(true);
        if (Globe.SelectedServer != null)
        {
            if (Globe.SelectedServer.playerId != 0)
            {
                serverMgr.GetInstance().SetName(Globe.SelectedServer.playerName);
                GameLibrary.player = Globe.SelectedServer.heroId;
                GameLibrary.nickName = Globe.SelectedServer.playerName;//角色的显示信息
            }
        }
        Control.ShowGUI(UIPanleID.UI_SelectServer, EnumOpenUIType.DefaultUIOrSecond, false, Globe.SelectedServer);
    }

}