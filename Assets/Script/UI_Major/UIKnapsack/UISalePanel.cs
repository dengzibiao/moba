using UnityEngine;
using System.Collections;
using System;

public class UISalePanel : GUIBase {

    public UISprite goods;
    public UISprite iconS;
    public GUISingleSprite icon;
    public UILabel goodsName;
    public UILabel goodsCount;
    public UILabel unitPrice;
    public UILabel saleCount;
    public UILabel totalPrice;

    public GUISingleButton reduceBtn;
    public GUISingleButton addBtn;
    public GUISingleButton maxCountBtn;
    public GUISingleButton cancelBtn;
    public GUISingleButton sureBtn;
    private Transform debris;
    public GameObject backObj;
    private object item;
    ItemData equipItem;
    public static UISalePanel instance;
    int currentCount = 1;
    int maxCount = 0;
    public override UIPanleID GetUIKey()
    {
        return UIPanleID.UISalePanel;
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
        unitPrice = transform.Find("UnitPrice").GetComponent<UILabel>();
        saleCount = transform.Find("SaleCount").GetComponent<UILabel>();
        totalPrice = transform.Find("TotalPrice").GetComponent<UILabel>();
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

        currentCount = 1;
        iconS.spriteName = equipItem.IconName;
        goods.spriteName = ItemData.GetFrameByGradeType(equipItem.GradeTYPE);
        goodsName.text = GoodsDataOperation.GetInstance().JointNameColour(equipItem.Name, equipItem.GradeTYPE); ;
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
        unitPrice.text = equipItem.Sprice.ToString();
        //saleCount.text = currentCount + "/" + equipItem.conut;
        saleCount.text = currentCount + "/" + maxCount;
        totalPrice.text = currentCount * equipItem.Sprice + "";
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
        GameLibrary.saleItemList.Clear();
        object[] obj = new object[] { equipItem.Id, equipItem.Uuid, currentCount };
        GameLibrary.saleItemList.Add(obj);
        Globe.isSaleSingleGood = true;
        GoodsDataOperation.GetInstance().equipItem = equipItem;
        GoodsDataOperation.GetInstance().currentCount = currentCount;
        //ClientSendDataMgr.GetSingle().GetItemSend().SendSellItem(equipItem.itmeid, equipItem.itemuuid, currentCount);
        ClientSendDataMgr.GetSingle().GetItemSend().SendSellItem(GameLibrary.saleItemList);
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
        totalPrice.text = currentCount * equipItem.Sprice + "";
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
        if (currentCount < maxCount)
        {
            saleCount.text = (++currentCount) + "/" + maxCount;
            totalPrice.text = currentCount * equipItem.Sprice + "";
        }
        else
        {
            totalPrice.text = currentCount * equipItem.Sprice + "";
            return;
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
        if (currentCount > 1)
        {
            saleCount.text = (--currentCount) + "/" + maxCount;
            totalPrice.text = currentCount * equipItem.Sprice + "";
        }
        else
        {
            totalPrice.text = currentCount * equipItem.Sprice + "";
            return;
        }

    }
}
