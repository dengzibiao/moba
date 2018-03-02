/*
文件名（File Name）:   InitLogin.cs

作者（Author）:    #高#

创建时间（CreateTime）:  #CreateTime#
*/
using UnityEngine;
using System.Collections;

public class InitLoginScene : BaseScene
{

    protected override void InitState()
    {
        this.SceneID = EnumSceneID.UI_Login;
        this.Name = "UI_Login";
    }
    protected override void Init()
    {
        if (StartLandingShuJu.GetInstance() == null)
        {
            GameObject go = new GameObject();
            go.name = "StartLoadData";
            go.AddComponent<StartLandingShuJu>();
            go.AddComponent<Game>();
        }
        Control.ShowGUI(UIPanleID.UILogin, EnumOpenUIType.DefaultUIOrSecond);
        CreatPrefab("HeroPosEmbattle", 10, 1000, 0);
    }
}

