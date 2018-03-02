using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tianyu;
using System;


public class UIGameAffiche : GUIBase
{
    public GUISingleButton BackBtn;
    public static UILabel GameAfficheText;
    public UILabel[] boxlab;
    public GUISingleMultList goodsMultList;
    public int Some = 0;
    public static List<UIGameAfficheNode> itemRankList = new List<UIGameAfficheNode>();
    private UIGameAfficheItem item;
    private int _index = -1;
    private GameObject gameobj;
    private static UIScrollView GameAffiche;
    /// <summary>
    /// 单例
    /// </summary>
    public static UIGameAffiche instance;
    //public static UIGameAffiche Instance()
    //{
    //    if (mSingleton == null)
    //        mSingleton = new UIGameAffiche();
    //    return mSingleton;
    //}
    public UIGameAffiche()
    {
        instance = this;
    }

    public override UIPanleID GetUIKey()
    {
        return UIPanleID.UIGameAffiche;
    }

    protected override void OnLoadData()
    {
        base.OnLoadData();
        Show();
    }

    /// <summary>
    /// 初始化
    /// </summary>
    protected override void Init()
    {

        GameAffiche = transform.Find("Panel/GameAffiche/GameAfficheScrollView").GetComponent<UIScrollView>();
        GameAfficheText = transform.Find("Panel/GameAffiche/GameAfficheScrollView/Grid/GameAfficheText").GetComponent<UILabel>();
        foreach (var item in FSDataNodeTable<UIGameAfficheNode>.GetSingleton().DataNodeList.Values)
        {
            if (item.id == 1)
            {
                GameAfficheText.text = item.notice.ToString();
            }
            for (int i = 0; i < boxlab.Length; i++)
            {
                if (item.id == i + 1)
                {
                    boxlab[i].text = item.notice_types.ToString();
                }
            }
            Some++;
            itemRankList.Add(item);
        }
        BackBtn.onClick = OnBackBtn;
    }

    protected override void ShowHandler()
    {
        GameAfficheData();
    }
    /// <summary>
    /// 动态生成Item
    /// </summary>
    public void GameAfficheData()
    {
        if (goodsMultList != null)
        {
            goodsMultList.InSize(Some, 1);
            goodsMultList.Info(itemRankList.ToArray());
        }

    }
    public void OnTabClick(int index, GameObject obj)
    {
        Debug.Log(_index);

        if (_index != -1)
        {

            if (_index != index)
            {
                gameobj.GetComponent<UIGameAfficheItem>().GameAfficheImg.transform.gameObject.SetActive(false);
                _index = index;
            }
        }
        else
        {
            _index = index;

        }
        gameobj = obj;
        Debug.Log(gameobj);
        foreach (var item in FSDataNodeTable<UIGameAfficheNode>.GetSingleton().DataNodeList.Values)
        {
            if (index + 1 == item.id)
            {
                GameAfficheText.text = item.notice.ToString(); ;
            }
        }
        ScrollViewResetPosition();
    }
    /// <summary>
    /// 返回按钮事件
    /// </summary>
    private void OnBackBtn()
    {
        ScrollViewResetPosition();
        Control.HideGUI(this.GetUIKey());
    }
    /// <summary>
    /// ScrillView位置还原
    /// </summary>
    private void ScrollViewResetPosition()
    {
        GameAffiche.GetComponent<UIScrollView>().ResetPosition();
        //GameAffiche.Find("GameAfficheScrollView").GetComponent<UIScrollView>().ResetPosition();
    }

}



