/*
文件名（File Name）:   NewPlayerRewards.cs

作者（Author）:    #高#

创建时间（CreateTime）:  #CreateTime#
*/
using UnityEngine;
using System.Collections;
using Tianyu;

public class NewPlayerRewards : GUIBase
{
    public GUISingleMultList multList;
    public Transform view;

    protected override void Init()
    {
       
    
           
        view = UnityUtil.FindCtrl<UIScrollView>(this.gameObject, "ScrollView").transform;
    }

    private object[] InitData()
    {
        return playerData.GetInstance().newPlayerRewardList.rewardList.ToArray();
    }
    protected override void ShowHandler()
    {

        multList.InSize(InitData().Length, 1);
        multList.Info(InitData());
        multList.ScrollView = view;
    }
    public override UIPanleID GetUIKey()
    {
        return UIPanleID.none;
    }
}
