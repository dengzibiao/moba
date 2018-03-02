using UnityEngine;
using System.Collections;
using Tianyu;
using System;
using System.ComponentModel;
using System.Collections.Generic;

public class UIActModeSelect : GUIBase
{

    public UIActivity activity;
    public GUISingleButton backBtn;
    public ActivityItem[] modeItem;
    public GUISingleButton[] btnClick;
    public int num = 0;
    Transform[] redArr = new Transform[5];
    int mapIndex = 30100;

    bool isFirstOpened = false;
    private static UIActModeSelect mSingleton;
    public static UIActModeSelect Instance()
    {
        if (mSingleton == null)
            mSingleton = new UIActModeSelect();
        return mSingleton;
    }

    public override UIPanleID GetUIKey()
    {
        return UIPanleID.none;
    }
    protected override void Init()
    {
        backBtn.onClick = OnBackBtnClick;

        for (int i = 0; i < modeItem.Length; i++)
        {
            btnClick[i].index = mapIndex;
            btnClick[i].onItemClick = OnItemClick;

            modeItem[i].RefreshUI(FSDataNodeTable<MapNode>.GetSingleton().FindDataByType(mapIndex));
            mapIndex += 100;
        }

        RefreshItem();
        isFirstOpened = true;
        for (int i =0;i<redArr.Length;i++)
        {
            int tem = i + 1;
            redArr[i] = transform.Find("red/RedPoint"+tem);
        }
    }
    private void ShowRedPoint(Dictionary<int, List<int>> redlist)
    {
        redArr[0].gameObject.SetActive(Singleton<RedPointManager>.Instance.GetChildList(EnumRedPoint.RP_EVENT_DUNGEON, 1));
        redArr[1].gameObject.SetActive(Singleton<RedPointManager>.Instance.GetChildList(EnumRedPoint.RP_EVENT_DUNGEON, 2));
        redArr[2].gameObject.SetActive(Singleton<RedPointManager>.Instance.GetChildList(EnumRedPoint.RP_EVENT_DUNGEON, 3));
        redArr[3].gameObject.SetActive(Singleton<RedPointManager>.Instance.GetChildList(EnumRedPoint.RP_EVENT_DUNGEON, 4));
        redArr[4].gameObject.SetActive(Singleton<RedPointManager>.Instance.GetChildList(EnumRedPoint.RP_EVENT_DUNGEON, 5));
    }
    protected override void ShowHandler()
    {
        if (!isFirstOpened) return;
        RefreshItem();
        ShowRedPoint(Singleton<RedPointManager>.Instance.GetRedList());
    }

    void RefreshItem()
    {
        if (null == GameLibrary.eventOpen || GameLibrary.eventOpen.Length <= 0)
        {
            for (int i = 0; i < modeItem.Length; i++)
            {
                modeItem[i].SetBtnState(false, 0);
            }
            return;
        }
        for (int i = 0; i < modeItem.Length; i++)
        {
            for (int j = 0; j < GameLibrary.eventOpen.Length; j++)
            {
                if ((i + 1) == GameLibrary.eventOpen[j])
                {
                    modeItem[i].SetBtnState(true, GameLibrary.eventdList[GameLibrary.eventOpen[j]]);
                    break;
                }
                else
                {
                    modeItem[i].SetBtnState(false, 0);
                }
            }
        }
    }

    public void OnItemClick(int mapID)
    {
        //Debug.Log(mapID);
        switch (mapID)
        {
            case 30100: activity.type = OpenSourceType.actGold; break;
            case 30200: activity.type = OpenSourceType.actExpe; break;
            case 30300: activity.type = OpenSourceType.actPower; break;
            case 30400: activity.type = OpenSourceType.actAgile; break;
            case 30500: activity.type = OpenSourceType.actIntel; break;
        }
        activity.ActDifficSelect.gameObject.SetActive(true);
        activity.ActDifficSelect.RefreshUI(FSDataNodeTable<MapNode>.GetSingleton().FindDataByType(mapID));
        gameObject.SetActive(false);
    }

    void OnBackBtnClick()
    {
        UIActivity.instance.HidePanel();
        //Control.HideGUI(GameLibrary.UIActivity);
        //Control.PlayBGmByClose(GameLibrary.UIActivity);
    }

}
