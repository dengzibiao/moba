using UnityEngine;
using System.Collections;
using Tianyu;

public class UIPopRefresh : GUIBase
{
    public static UIPopRefresh instance;

    public delegate void ChangeRefreshCount(int count);

    public event ChangeRefreshCount ChangeRefreshCountRvent;
    public GUISingleButton refreshBtn;
    public GUISingleButton cancelBtn;
    private UILabel lable;
    public GUISingleLabel refreshCost; 

    public UIPopRefresh()
    {
        instance = this;
    }


    protected override void Init()
    {
        if (FSDataNodeTable<ResetLaterNode>.GetSingleton().DataNodeList.Count > 0)
        {
            for (int i = 0; i < FSDataNodeTable<ResetLaterNode>.GetSingleton().DataNodeList[1].generalShop.Length; i++)
            {
                int ct = FSDataNodeTable<ResetLaterNode>.GetSingleton().DataNodeList[1].generalShop[i];
                playerData.GetInstance().lotteryInfo.refreshCost.Add(ct);
            }
        }
        refreshBtn.onClick = OnRefreshClick;
        cancelBtn.onClick = OnCancelClick;
        lable = transform.Find("Label").GetComponent<UILabel>();
    }
    public override UIPanleID GetUIKey()
    {
        return UIPanleID.none;
    }
    private void OnRefreshClick()
    {
        if (playerData.GetInstance().baginfo.diamond > refreshPrice)
        {
            ClientSendDataMgr.GetSingle().GetCShopSend().RefreshGoodsList( _index, count, refreshPrice);
        }
    }

    protected override void ShowHandler()
    {
        base.ShowHandler();
        count = playerData.GetInstance().lotteryInfo.shopRefreshCount;
        lable.text = "是否继续（今日已刷新" + count + "次）";
        if (count > playerData.GetInstance().lotteryInfo.refreshCost.Count)
        {
            refreshCost.text = playerData.GetInstance().lotteryInfo.refreshCost[playerData.GetInstance().lotteryInfo.refreshCost.Count-1].ToString();
            refreshPrice = int.Parse(refreshCost.text);
        }
        else
        {
            refreshCost.text = playerData.GetInstance().lotteryInfo.refreshCost[count].ToString();
            refreshPrice = int.Parse(refreshCost.text);
        }
    }

    public void ReceiveDate()
    {
        count = playerData.GetInstance().lotteryInfo.shopRefreshCount;
        if (ChangeRefreshCountRvent != null)
        {
            ChangeRefreshCountRvent(count);
        }
        OnCancelClick();
       
    }
    public void InitData(int index)
    {
        this._index = index;
    }
    private void OnCancelClick()
    {
        Control.HideGUI(this.GetUIKey());
       // UIMask.Instance.ClosePanle(GameLibrary.UIPopRefresh);
    }
    public static int count = 0;
    private int refreshPrice;
    private int _index;
    private ShopVO vo;
}
