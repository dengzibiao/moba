using UnityEngine;
using System.Collections;
using System;

public class UIUseExpVialPanel : GUIBase
{

    public UISprite goods;
    public UISprite iconS;
    public GUISingleSprite icon;
    public UILabel goodsName;
    public UILabel goodsCount;
    public UILabel unitExpValue;//单个药瓶经验药水值
    public UILabel saleCount;
    public UILabel totalExpValue;
    public UILabel expPoolValue;//经验池经验
    public GUISingleButton reduceBtn;
    public GUISingleButton addBtn;
    public GUISingleButton maxCountBtn;
    public GUISingleButton cancelBtn;
    public GUISingleButton sureBtn;
    private Transform debris;
    private object item;
    ItemData equipItem;
    public static UIUseExpVialPanel instance;
    int currentCount = 1;
    int maxCount = 0;
    public GameObject backObj;
    public override UIPanleID GetUIKey()
    {
        return UIPanleID.UIUseExpVialPanel;
    }


    protected override void Init()
    {
        instance = this;

        backObj = transform.Find("Mask").gameObject;
        goods = transform.Find("Goods").GetComponent<UISprite>();
        iconS = transform.Find("Goods/Icon").GetComponent<UISprite>();
        icon = transform.Find("Goods/Icon").GetComponent<GUISingleSprite>();
        goodsName = transform.Find("GoodsName").GetComponent<UILabel>();
        goodsCount = transform.Find("GoodsCount").GetComponent<UILabel>();
        unitExpValue = transform.Find("UnitExpValue").GetComponent<UILabel>();
        saleCount = transform.Find("SaleCount").GetComponent<UILabel>();
        totalExpValue = transform.Find("TotalExpValue").GetComponent<UILabel>();
        expPoolValue = transform.Find("ExpPoolValue").GetComponent<UILabel>();
        debris = transform.Find("Goods/Debris");
        reduceBtn = transform.Find("ReduceBtn").GetComponent<GUISingleButton>();
        addBtn = transform.Find("AddBtn").GetComponent<GUISingleButton>();
        maxCountBtn = transform.Find("MaxCountBtn").GetComponent<GUISingleButton>();
        cancelBtn = transform.Find("CancelBtn").GetComponent<GUISingleButton>();
        sureBtn = transform.Find("SureBtn").GetComponent<GUISingleButton>();

        reduceBtn.onClick = OnReduceBtnClick;
        addBtn.onClick = OnAddBtnClick;
        maxCountBtn.onClick = OnMaxCountBtnClick;
        cancelBtn.onClick = OnCancelBtnClick;
        sureBtn.onClick = OnSureBtnClick;
        UIEventListener.Get(backObj).onClick += OnCloseClick;
       
    }
    protected override void ShowHandler()
    {
        currentCount = 1;
        ItemNodeState itemnodestate = null;
        if (equipItem == null) return;
        for (int i = 0; i < playerData.GetInstance().baginfo.itemlist.Count; i++)
        {
            if (equipItem.Id == playerData.GetInstance().baginfo.itemlist[i].Id)
            {
                maxCount = playerData.GetInstance().baginfo.itemlist[i].Count;
            }
        }
        //if (playerData.GetInstance().baginfo.ItemDic.ContainsKey(equipItem.Id))
        //{
        //    maxCount = playerData.GetInstance().baginfo.ItemDic[equipItem.Id].Count;
        //}

        iconS.spriteName = equipItem.IconName;
        goods.spriteName = ItemData.GetFrameByGradeType(equipItem.GradeTYPE);
        goodsName.text = GoodsDataOperation.GetInstance().JointNameColour(equipItem.Name, equipItem.GradeTYPE);
        if (equipItem.Types == 6|| equipItem.Types == 3)
        {
            debris.gameObject.SetActive(true);
        }
        else
        {
            debris.gameObject.SetActive(false);
        }
        if (equipItem.Types == 3)
        {
            debris.GetComponent<UISprite>().spriteName = "materialdebris";
            debris.GetComponent<UISprite>().MakePixelPerfect();
        }
        else if (equipItem.Types == 6)
        {
            debris.GetComponent<UISprite>().spriteName = "linghunshi";
            debris.GetComponent<UISprite>().MakePixelPerfect();
        }
        icon.uiAtlas = equipItem.UiAtlas;


        //goodsCount.text = equipItem.conut.ToString();
        goodsCount.text = maxCount + "";
        if (GameLibrary.Instance().ItemStateList.ContainsKey(equipItem.Id))
        {
            itemnodestate = GameLibrary.Instance().ItemStateList[equipItem.Id];
            unitExpValue.text = itemnodestate.exp_gain.ToString();
            totalExpValue.text = currentCount * itemnodestate.exp_gain + "";
        }
        expPoolValue.text = playerData.GetInstance().selfData.expPool + "";//经验池的经验值
        //saleCount.text = currentCount + "/" + equipItem.conut;
        saleCount.text = currentCount + "/" + maxCount;
    }
    protected override void SetUI(params object[] uiParams)
    {
        item = uiParams[0];
        equipItem = (ItemData)item;
        base.SetUI();
    }
    public void SetData(object obj)
    {
        item = obj;
        equipItem = (ItemData)obj;
    }
    protected override void OnLoadData()
    {
        base.OnLoadData();
        this.State = EnumObjectState.Ready;
        Show();
    }
    private void OnSureBtnClick()
    {
        //发送使用经验药水协议

        //ClientSendDataMgr.GetSingle().GetItemSend().SendSellItem(equipItem.itmeid, equipItem.itemuuid, currentCount);
        //ClientSendDataMgr.GetSingle().GetItemSend().SendSellItem(GameLibrary.saleItemList);
        ClientSendDataMgr.GetSingle().GetItemSend().SendUseItem(equipItem.Id, equipItem.Uuid, currentCount);
        Control.HideGUI(this.GetUIKey());
        //Hide();
    }
    private void OnCancelBtnClick()
    {
        Control.HideGUI(this.GetUIKey());
        //Hide();
    }
    private void OnCloseClick(GameObject go)
    {
        Control.HideGUI(this.GetUIKey());
        //Hide();
    }
    private void OnMaxCountBtnClick()
    {
        //saleCount.text = equipItem.conut + "/" + equipItem.conut;
        //currentCount = equipItem.conut;
        //totalPrice.text = currentCount * equipItem.saleprice + "";
        saleCount.text = maxCount + "/" + maxCount;
        currentCount = maxCount;
        if (GameLibrary.Instance().ItemStateList.ContainsKey(equipItem.Id))
        {
            totalExpValue.text = currentCount * GameLibrary.Instance().ItemStateList[equipItem.Id].exp_gain + "";
        }
        
    }

    private void OnAddBtnClick()
    {
        string[] textColumn = saleCount.text.Split("/".ToCharArray());
        currentCount = Int32.Parse(textColumn[0]);
        //if (currentCount < equipItem.conut)
        //{
        //    saleCount.text = (++currentCount) + "/" + equipItem.conut;
        //    totalPrice.text = currentCount * equipItem.saleprice + "";
        //}
        //else
        //{
        //    totalPrice.text = currentCount * equipItem.saleprice + "";
        //    return;
        //}
        if (GameLibrary.Instance().ItemStateList.ContainsKey(equipItem.Id))
        {
            if (currentCount < maxCount)
            {
                saleCount.text = (++currentCount) + "/" + maxCount;
                totalExpValue.text = currentCount * GameLibrary.Instance().ItemStateList[equipItem.Id].exp_gain + "";
            }
            else
            {
                totalExpValue.text = currentCount * GameLibrary.Instance().ItemStateList[equipItem.Id].exp_gain + "";
                return;
            }
        }
        
    }

    private void OnReduceBtnClick()
    {
        string[] textColumn = saleCount.text.Split("/".ToCharArray());
        currentCount = Int32.Parse(textColumn[0]);
        //if (currentCount > 1)
        //{
        //    saleCount.text = (--currentCount) + "/" + equipItem.conut;
        //    totalPrice.text = currentCount * equipItem.saleprice + "";
        //}
        //else
        //{
        //    totalPrice.text = currentCount * equipItem.saleprice + "";
        //    return;
        //}
        if (GameLibrary.Instance().ItemStateList.ContainsKey(equipItem.Id))
        {
            if (currentCount > 1)
            {
                saleCount.text = (--currentCount) + "/" + maxCount;
                totalExpValue.text = currentCount * GameLibrary.Instance().ItemStateList[equipItem.Id].exp_gain + "";
            }
            else
            {
                totalExpValue.text = currentCount * GameLibrary.Instance().ItemStateList[equipItem.Id].exp_gain + "";
                return;
            }
        }
        

    }
}
