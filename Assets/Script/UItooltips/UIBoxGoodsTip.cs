using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tianyu;

/// <summary>
/// 宝箱物品展示面板（可以用于展示多个物品）
/// </summary>
public class UIBoxGoodsTip : GUIBase {

    public GUISingleButton closeButton;
    public UIScrollView goodsListScrollView;
    public GUISingleMultList goodsMultList;

    public UIItemTips uiitemtips;
    private List<ItemData> itemDataList = new List<ItemData>();

    public static UIBoxGoodsTip instance;

    public UIBoxGoodsTip()
    {
        instance = this;
    }

    public void SetData(List<ItemData> itemDatalist)
    {
        this.itemDataList = itemDatalist;
    }
    public override UIPanleID GetUIKey()
    {
        return UIPanleID.UIBoxGoodsTip;
    }
    protected override void SetUI(params object[] uiParams)
    {
        if (uiParams.Length > 0)
        {
            this.itemDataList = (List<ItemData>)uiParams[0];
        }
        base.SetUI();
    }
    protected override void OnLoadData()
    {
        base.OnLoadData();
        Show();
    }
    protected override void ShowHandler()
    {
        //List<ItemData> itemlist = TaskManager.Single().GetItemList(clickTaskItem.tasknode.Reward_prop);
        goodsMultList.ScrollView = goodsListScrollView.transform;
        goodsMultList.InSize(itemDataList.Count, itemDataList.Count);
        goodsMultList.Info(itemDataList.ToArray());
        if(itemDataList.Count==1) goodsMultList.transform.localPosition=new Vector3(68.1f, goodsMultList.transform.localPosition.y,0);
        if (itemDataList.Count == 2) goodsMultList.transform.localPosition = new Vector3(19.93f, goodsMultList.transform.localPosition.y, 0);
        if (itemDataList.Count == 3) goodsMultList.transform.localPosition = new Vector3(-38f, goodsMultList.transform.localPosition.y, 0);
        goodsListScrollView.enabled = false;
        //goodsListScrollView.transform.localPosition = Vector3.zero;

    }
    protected override void Init()
    {
        //mulitlist = transform.Find("GoodsListScrollView/GoodsMultList").GetComponent<GUISingleMultList>();
        goodsListScrollView = transform.Find("GoodsListScrollView").GetComponent<UIScrollView>();
        closeButton.onClick = OnClose;
    }
    private void OnClose()
    {
        //Hide();
        Control.HideGUI(this.GetUIKey());
    }
}
