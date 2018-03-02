/*
文件名（File Name）:   UILottryEffect.cs

作者（Author）:    #高#

创建时间（CreateTime）:  #CreateTime#
*/
using UnityEngine;
using System.Collections;

public class UILottryEffect : GUIBase {
    public override UIPanleID GetUIKey()
    {
        return UIPanleID.UILottryEffect;
    }

    protected override void OnLoadData()
    {
        base.OnLoadData();
        Show();
    }
}
