using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Tianyu;

public class UIKnapsack : GUIBase
{
    #region Define
    public GUISingleButton backBtn;
    public GUISingleCheckBoxGroup checkBoxs;
    public GUISingleMultList goodsMultList;
    public UIScrollBar scrollBar;
    public GameObject goodsDetials;
    private Transform view;
    private Color c;
    private bool isHave = false;
    private List<ItemData> itemList = new List<ItemData>();
    //object[] allObj;
    /// <summary>
    /// 全部
    /// </summary>
    object[] objs;//全部
    /// <summary>
    /// 材料
    /// </summary>
    object[] materials;
    /// <summary>
    /// 灵魂石
    /// </summary>
    object[] soulStones;
    /// <summary>
    /// 消耗品（金币道具 经验道具 恢复道具 宝箱）
    /// </summary>
    object[] consumables;
    /// <summary>
    /// 符文
    /// </summary>
    object[] runes;
    List<object> allList = new List<object>();
    List<object> materialsList = new List<object>();
    List<object> runesList = new List<object>();
    List<object> soulStonesList = new List<object>();
    List<object> consumablesList = new List<object>();
    #endregion

    #region Init
    private static UIKnapsack instance;

    public static UIKnapsack Instance{get{ return instance;} set{ instance = value;}}

    public object[] Objs{get{ return objs;} set{ objs = value;}}

    public UIKnapsack()
    {
        instance = this;
    }

    public override UIPanleID GetUIKey()
    {
        return UIPanleID.UIKnapsack;
    }
    protected override void Init()
    {
        Instance = this;
        backBtn = transform.FindComponent<GUISingleButton>("BackBtn");
        view = transform.Find("GoodsListScrollView");
        scrollBar = transform.Find("ScrollBar").GetComponent<UIScrollBar>();
        goodsDetials = transform.Find("GoodsDetials").gameObject;
        goodsMultList = transform.FindComponent<GUISingleMultList>("GoodsMultList");

        backBtn.onClick = OnBackBtnClick;
        checkBoxs.onClick = OnGoodTypeTabClick;
        InitDate();
        goodsMultList.ScrollView = view;
        c = new Color(172 / 255f, 213 / 255f, 255 / 255f);
        checkBoxs.DefauleIndex = 0;
        OnGoodTypeTabClick(0, true);
        Globe.isRefresh = true;
    }
    protected override void ShowHandler()
    {
        InitDate();
        OnGoodTypeTabClick(Globe.seletTagIndex, true);
    }
    protected override void OnLoadData()
    {
        base.OnLoadData();
        Singleton<Notification>.Instance.RegistMessageID(MessageID.common_backpack_list_ret, UIPanleID.UIKnapsack);
        Singleton<Notification>.Instance.RegistMessageID(MessageID.common_update_item_list_ret, UIPanleID.UIKnapsack);
        if (playerData.GetInstance().baginfo.itemlist.Count == 0)
        {
            Singleton<Notification>.Instance.Send(MessageID.common_backpack_list_req, C2SMessageType.ActiveWait);
        }
        else
        {
            this.State = EnumObjectState.Ready;
            Show();
        }
    }
    public override void ReceiveData(UInt32 messageID)
    {
        base.ReceiveData(messageID);
        switch (messageID)
        {
            case MessageID.common_backpack_list_ret:
                Show();
                break;
            case MessageID.common_update_item_list_ret:
                RefreshView();
                break;
        }
    }
    #endregion

    #region Event
    public void OnGoodTypeTabClick(int index, bool boo)
    {
        if (Objs == null)
        {
            goodsDetials.SetActive(false);
            //Control.HideGUI(GameLibrary.UIGoodsDetials);
            return;
        }
        if (boo)
        {
            checkBoxs.transform.GetChild(index).Find("Label").GetComponent<UILabel>().color = Color.white;
            Globe.seletIndex = 0;
            if (index != 0)
            {
                Globe.isUpdateBag = true;
            }
            switch (index)
            {
                case 0:
                    goodsMultList.InSize(Objs.Length, 5);
                    goodsMultList.Info(Objs);
                    scrollBar.value = 0;
                    Globe.seletTagIndex = 0;

                    if (Globe.isUpdateBag && Objs.Length > 0)
                    {
                        //Control.ShowGUI(GameLibrary.UIGoodsDetials);
                        //GoodsDetials.instance.SetData(Objs[0]);
                        goodsDetials.SetActive(true);
                    }
                    else
                    {
                        if (Objs.Length > 0)
                        {
                            //Control.ShowGUI(GameLibrary.UIGoodsDetials);
                            goodsDetials.SetActive(true);
                        }
                        else
                        {
                            //Control.HideGUI(GameLibrary.UIGoodsDetials);
                            goodsDetials.SetActive(false);
                        }
                    }
                    break;
                case 1:
                    goodsMultList.InSize(materials.Length, 5);
                    goodsMultList.Info(materials);
                    scrollBar.value = 0;
                    Globe.seletTagIndex = 1;
                    if (materials.Length > 0)
                    {
                        //Control.ShowGUI(GameLibrary.UIGoodsDetials);
                        goodsDetials.SetActive(true);
                        GoodsDetials.instance.SetData(materials[0]);
                    }
                    else
                    {
                        //Control.HideGUI(GameLibrary.UIGoodsDetials);
                        goodsDetials.SetActive(false);
                    }
                    break;
                case 2:
                    goodsMultList.InSize(soulStones.Length, 5);
                    goodsMultList.Info(soulStones);
                    scrollBar.value = 0;
                    Globe.seletTagIndex = 2;
                    if (soulStones.Length > 0)
                    {
                        //Control.ShowGUI(GameLibrary.UIGoodsDetials);
                        goodsDetials.SetActive(true);
                        GoodsDetials.instance.SetData(soulStones[0]);
                    }
                    else
                    {
                        goodsDetials.SetActive(false);
                        //Control.HideGUI(GameLibrary.UIGoodsDetials);
                    }
                    break;
                case 3:
                    goodsMultList.InSize(consumables.Length, 5);
                    goodsMultList.Info(consumables);
                    scrollBar.value = 0;
                    Globe.seletTagIndex = 3;
                    if (consumables.Length > 0)
                    {
                        goodsDetials.SetActive(true);
                        //Control.ShowGUI(GameLibrary.UIGoodsDetials);
                        GoodsDetials.instance.SetData(consumables[0]);
                    }
                    else
                    {
                        goodsDetials.SetActive(false);
                        //Control.HideGUI(GameLibrary.UIGoodsDetials);
                    }
                    break;
                case 4:
                    goodsMultList.InSize(runes.Length,5);
                    goodsMultList.Info(runes);
                    scrollBar.value = 0;
                    Globe.seletTagIndex = 4;
                    if (runes.Length > 0)
                    {
                        goodsDetials.SetActive(true);
                        //Control.ShowGUI(GameLibrary.UIGoodsDetials);
                        GoodsDetials.instance.SetData(runes[0]);
                    }
                    else
                    {
                        goodsDetials.SetActive(false);
                        //Control.HideGUI(GameLibrary.UIGoodsDetials);
                    }
                    break;
                default:
                    break;
            }
        }
        for (int i = 0; i < checkBoxs.transform.childCount; i++)
        {
            if (i != index)
            {
                checkBoxs.transform.GetChild(i).Find("Label").GetComponent<UILabel>().color = c;
            }
        }
    }
    private void OnBackBtnClick()
    {
        Control.HideGUI();
        //Hide();
    }
    #endregion

    #region 设置物品信息
    /// <summary>
    /// 设置物品信息
    /// </summary>
    public void InitDate()
    {
        itemList = playerData.GetInstance().baginfo.itemlist;
        isHave = false;
        int goodsCount = 0;
        allList.Clear();
        materialsList.Clear();
        runesList.Clear();
        soulStonesList.Clear();
        consumablesList.Clear();
        //现在物品没有堆叠问题，因为上限是999  如果超过上限都放到邮箱寄存
        for (int i = 0; i < itemList.Count; i++)
        {
            //背包里面部分物品不展示 1:装备  7:英雄整卡 12：任务收集品 14：货币 15：坐骑卡；16：宠物卡
            if (itemList[i].Types == 1 || itemList[i].Types == 7|| itemList[i].Types == 12|| itemList[i].Types == 14 || itemList[i].Types == 15 || itemList[i].Types == 16)
            {
                continue;
            }
            goodsCount++;
        }
        Objs = new object[goodsCount];
        int index = 0;
        for (int j = 0; j < itemList.Count; j++)
        {
            //背包里面部分物品不展示
            if (itemList[j].Types == 1 || itemList[j].Types == 7 || itemList[j].Types == 12 || itemList[j].Types == 14 || itemList[j].Types == 15 || itemList[j].Types == 16)
            {
                continue;
            }
            if (index <= goodsCount)
            {
                Objs[index] = itemList[j];
                index++;
            }
        }
        for (int m = 0; m < Objs.Length; m++)
        {

            if (((ItemData)Objs[m]).Types == 2|| ((ItemData)Objs[m]).Types == 3)
            {
                materialsList.Add(Objs[m]);
            }
            if (((ItemData)Objs[m]).Types == 8)
            {
                runesList.Add(Objs[m]);
            }
            if (((ItemData)Objs[m]).Types == 6)
            {
                soulStonesList.Add(Objs[m]);
            }
            if (((ItemData)Objs[m]).Types == 4 || ((ItemData)Objs[m]).Types == 5 || ((ItemData)Objs[m]).Types == 9 || ((ItemData)Objs[m]).Types == 10 || ((ItemData)Objs[m]).Types == 11 || ((ItemData)Objs[m]).Types == 13)
            {
                consumablesList.Add(Objs[m]);
            }
        }
        materials = materialsList.ToArray();
        runes = runesList.ToArray();
        soulStones = soulStonesList.ToArray();
        consumables = consumablesList.ToArray();
    }
    /*public void InitDate()
    {
        itemList = playerData.GetInstance().baginfo.itemlist;
        isHave = false;
        int goodsCount = 0;
        allList.Clear();
        materialsList.Clear();
        runesList.Clear();
        soulStonesList.Clear();
        consumablesList.Clear();
        //UpdateGoodsData();
        //if (itemList.Count <= 0)
        //{
        //    return;
        //}
        //物品堆叠处理 之后要修改 之前数量多的物品会占多个格子，现在物品数量不会超过物品堆叠上限的
        for (int i = 0; i < itemList.Count; i++)
        {
            //背包里面部分物品不展示
            if (itemList[i].Types == 1 || itemList[i].Types == 3 || itemList[i].Types == 7  || itemList[i].Types == 10)
            {
                continue;
            }
            if (itemList[i].Count > FSDataNodeTable<ItemNode>.GetSingleton().FindDataByType(itemList[i].Id).piles)//(VOManager.Instance().GetCSV<ItemCSV>("Item").GetVO(itemList[i].Id).piles))
            {
                goodsCount += itemList[i].Count / itemList[i].Piles;
                if ((itemList[i].Count % itemList[i].Piles) > 0)
                {
                    goodsCount++;
                }
            }
            else
            {
                goodsCount++;
            }
        }
        Objs = new object[goodsCount];
        int index = 0;
        for (int j = 0; j < itemList.Count; j++)
        {
            //背包里面部分物品不展示
            if (itemList[j].Types == 1 || itemList[j].Types == 3 || itemList[j].Types == 7 || itemList[j].Types == 10 )
            {
                continue;
            }
            ItemData data1 = new ItemData();
            ItemData data2 = new ItemData();

            data1.Id = itemList[j].Id;
            data1.Uuid = itemList[j].Uuid;
            data1.Count = itemList[j].Count;
            data1.GradeTYPE = itemList[j].GradeTYPE;
            data1.Name = itemList[j].Name;
            data1.Types = itemList[j].Types;
            data1.Describe = itemList[j].Describe;
            data1.Cprice = itemList[j].Cprice;
            data1.Sprice = itemList[j].Sprice;
            data1.Piles = itemList[j].Piles;
            data1.IconName = itemList[j].IconName;

            data2.Id = itemList[j].Id;
            data2.Uuid = itemList[j].Uuid;
            data2.Count = itemList[j].Count;
            data2.GradeTYPE = itemList[j].GradeTYPE;
            data2.Name = itemList[j].Name;
            data2.Types = itemList[j].Types;
            data2.Describe = itemList[j].Describe;
            data2.Cprice = itemList[j].Cprice;
            data2.Sprice = itemList[j].Sprice;
            data2.Piles = itemList[j].Piles;
            data2.IconName = itemList[j].IconName;

            while (data2.Count > itemList[j].Piles)
            {
                data1.Count = (short)itemList[j].Piles;
                Objs[index] = data1;
                index++;
                data2.Count = (short)(data2.Count - itemList[j].Piles);
            }
            ItemData data3 = new ItemData();
            data3.Uuid = data2.Uuid;
            data3.Id = data2.Id;
            data3.Count = data2.Count;
            data3.GradeTYPE = data2.GradeTYPE;
            data3.Name = data2.Name;
            data3.Types = data2.Types;
            data3.Describe = data2.Describe;
            data3.Cprice = data2.Cprice;
            data3.Sprice = data2.Sprice;
            data3.Piles = data2.Piles;
            data3.IconName = data2.IconName;
            Objs[index] = data3;
            index++;
        }
        for (int m = 0; m < Objs.Length; m++)
        {

            if (((ItemData)Objs[m]).Types == 2)
            {
                materialsList.Add(Objs[m]);
            }
            if (((ItemData)Objs[m]).Types == 8)
            {
                runesList.Add(Objs[m]);
            }
            if (((ItemData)Objs[m]).Types == 6)
            {
                soulStonesList.Add(Objs[m]);
            }
            if (((ItemData)Objs[m]).Types == 4|| ((ItemData)Objs[m]).Types == 5|| ((ItemData)Objs[m]).Types == 9|| ((ItemData)Objs[m]).Types == 11)
            {
                consumablesList.Add(Objs[m]);
            }
        }
        materials = materialsList.ToArray();
        runes = runesList.ToArray();
        soulStones = soulStonesList.ToArray();
        consumables = consumablesList.ToArray();

    }*/
    #endregion

    #region 刷新背包界面
    /// <summary>
    /// 刷新背包界面
    /// </summary>
    public void RefreshView()
    {
        InitDate();
        //出售物品后，界面刷新
        switch (Globe.seletTagIndex)
        {
            case 0:
                goodsMultList.InSize(Objs.Length, 5);
                goodsMultList.Info(Objs);
                scrollBar.value = 0;
                if (Objs.Length > 0)
                {
                    GoodsDetials.instance.SetData(Objs[Globe.seletIndex]);

                }
                else { GoodsDetials.instance.gameObject.SetActive(false); }
                break;
            case 1:
                goodsMultList.InSize(materials.Length, 5);
                goodsMultList.Info(materials);
                scrollBar.value = 0;
                if (materials.Length > 0)
                {
                    GoodsDetials.instance.SetData(materials[Globe.seletIndex]);
                }
                else { GoodsDetials.instance.gameObject.SetActive(false); }
                break;
            case 2:
                goodsMultList.InSize(soulStones.Length, 5);
                goodsMultList.Info(soulStones);
                scrollBar.value = 0;
                if (soulStones.Length > 0)
                {
                    GoodsDetials.instance.SetData(soulStones[Globe.seletIndex]);
                }
                else { GoodsDetials.instance.gameObject.SetActive(false); }
                break;
            case 3:
                goodsMultList.InSize(consumables.Length, 5);
                goodsMultList.Info(consumables);
                scrollBar.value = 0;
                if (consumables.Length > 0)
                {
                    //GoodsDetials.instance.SetData(consumables[consumables.Length - 1]);
                    GoodsDetials.instance.SetData(consumables[Globe.seletIndex]);
                }
                else { GoodsDetials.instance.gameObject.SetActive(false); }
                break;
            case 4:
                goodsMultList.InSize(runes.Length, 5);
                goodsMultList.Info(runes);
                scrollBar.value = 0;
                if (runes.Length > 0)
                {
                    //GoodsDetials.instance.SetData(consumables[runes.Length - 1]);
                    GoodsDetials.instance.SetData(runes[Globe.seletIndex]);
                }
                else { GoodsDetials.instance.gameObject.SetActive(false); }
                break;
            default:
                break;
        }
    }
    #endregion
}
