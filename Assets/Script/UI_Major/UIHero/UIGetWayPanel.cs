using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// 魂石获取途径
/// </summary>

public class UIGetWayPanel : GUIBase
{

    public GUISingleButton backBtn;     //关闭按钮
    public GUISingleSprite border;      //边框
    public GUISingleSprite icon;        //头像
    public GUISingleButton level1;      //副本1
    public GUISingleButton level2;      //副本2
    public GUISingleButton level3;      //副本3
    BoxCollider backPBtn;    //关闭按钮蒙板


    public override UIPanleID GetUIKey()
    {
        return UIPanleID.none;
    }
    protected override void Init()
    {
        base.Init();

        backPBtn = transform.FindChild("BackPBtn").GetComponent<BoxCollider>();

        //返回按钮
        backBtn.onClick = OnBackBtnClick;
        //backPBtn.onClick = OnBackBtnClick;
        UIEventListener.Get(backPBtn.gameObject).onClick += OnBackPBtnClick;

        level1.onClick = OnLevelBtnClick;

    }


    /// <summary>
    /// 初始化信息
    /// </summary>
    public void InitInfo()
    {
        icon.spriteName = "";
    }


    /// <summary>
    /// 进入副本
    /// </summary>
    private void OnLevelBtnClick()
    {
        print("进入副本");
    }

    void OnBackBtnClick()
    {
        Hide();
    }

    /// <summary>
    /// 关闭按钮
    /// </summary>
    private void OnBackPBtnClick(GameObject go)
    {
        Control.HideGUI(UIPanleID.UIGetWayPanel);
    }

}