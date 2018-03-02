/*
文件名（File Name）:   InitMainUI.cs

作者（Author）:    #高#

创建时间（CreateTime）:  #CreateTime#
*/
using UnityEngine;
using System.Collections;

/// <summary>
/// 主城面板资源加载
/// </summary>
public class InitMainScene : BaseScene
{
    protected override void InitState()
    {
        this.SceneID = EnumSceneID.UI_MajorCity01;
        this.Name = GameLibrary.UI_Major;
    }
    protected override void Init()
    {
        CreatPrefab("HeroPosEmbattle", 10, 1000, 0);


        /////// ---------------------------------------------------------------------------
        Control.ShowGUI(UIPanleID.UIRole, EnumOpenUIType.DefaultUIOrSecond);
        Control.ShowGUI(UIPanleID.UIMoney, EnumOpenUIType.DefaultUIOrSecond);
        Control.ShowGUI(UIPanleID.UIChat, EnumOpenUIType.DefaultUIOrSecond);
        Control.ShowGUI(UIPanleID.UISetting, EnumOpenUIType.DefaultUIOrSecond);
        Control.ShowGUI(UIPanleID.UISkillAndGoldHintPanel, EnumOpenUIType.DefaultUIOrSecond);
        Control.ShowGUI(UIPanleID.UISign_intBox, EnumOpenUIType.DefaultUIOrSecond);
        Control.ShowGUI(UIPanleID.UIgoodstips, EnumOpenUIType.DefaultUIOrSecond);
        Control.ShowGUI(UIPanleID.UITaskTracker, EnumOpenUIType.DefaultUIOrSecond);
        Control.ShowGUI(UIPanleID.UIExpBar, EnumOpenUIType.DefaultUIOrSecond);
        Control.ShowGUI(UIPanleID.UITaskEffectPanel, EnumOpenUIType.DefaultUIOrSecond);

        /////// ------------------------------------------------------------------------------------------
        GameObject obj = GameObject.Find("StarData");
        if (obj != null)
        {
            if (obj.GetComponent<PropertyManager>() == null)
            {
                obj.AddComponent<PropertyManager>();//用于倒计时
            }
            //if (obj.GetComponent<TaskAutoTraceManager>() ==null)
            //{
            //    obj.AddComponent<TaskAutoTraceManager>();//用于任务的自动寻路
            //}
        }
        //RegistUI(GameLibrary.UIActivity, UIPanleID.UIActivityPanel);
        //if (GameObject.Find("TaskAutoTrace") == null)
        //{
        //    GameObject taskAutoTrace = new GameObject("TaskAutoTrace");
        //    if (taskAutoTrace.GetComponent<TaskAutoTraceManager>() == null)
        //    {
        //        taskAutoTrace.AddComponent<TaskAutoTraceManager>();
        //    }

        //}

        //if (GameObject.Find("TaskEffect") == null)
        //{
        //    GameObject taskeffect = new GameObject("TaskEffect");
        //    DontDestroyOnLoad(taskeffect);
        //    if (taskeffect.GetComponent<TaskEffectManager>() == null)
        //    {
        //        taskeffect.AddComponent<TaskEffectManager>();
        //    }

        //}




    }
}
