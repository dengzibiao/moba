using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Tianyu;

public class UIUpgradeGiftBag : GUIBase
{
    public GUISingleMultList multList;
    public List<int> canGetLevelList;
    Dictionary<long, LevelRewardNode> node;
    List<LevelRewardNode> levelRewardList = new List<LevelRewardNode>();
    private static UIUpgradeGiftBag instance;

    public static UIUpgradeGiftBag Instance { get { return instance; } set { instance = value; } }
    protected override void Init()
    {
        multList = transform.Find("GiftbagScrollView/MultList").GetComponent<GUISingleMultList>();
        if (FSDataNodeTable<LevelRewardNode>.GetSingleton().DataNodeList.Count > 0)
        {
            foreach (LevelRewardNode node in FSDataNodeTable<LevelRewardNode>.GetSingleton().DataNodeList.Values)
            {
                levelRewardList.Add(node);
            }
        }
        

    }
    protected override void ShowHandler()
    {
        base.ShowHandler();
        multList.InSize(levelRewardList.Count, 1);
        multList.Info(levelRewardList.ToArray());
    }
    public override UIPanleID GetUIKey()
    {
        return UIPanleID.none;
    }
    public void RefreshPanel()
    {
        multList.InSize(levelRewardList.Count, 1);
        multList.Info(levelRewardList.ToArray());
    }
}
