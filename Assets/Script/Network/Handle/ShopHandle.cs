/*
文件名（File Name）:   ShopHandle.cs

作者（Author）:    #高#

创建时间（CreateTime）:  2016-6-16 15:43:15
*/
using System;
using UnityEngine;
using System.Collections.Generic;
using Tianyu;

public class CShopHandle : CHandleBase
{
    public CShopHandle(CHandleMgr mgr)
        : base(mgr)
    {
    }
    public override void RegistAllHandle()
    {
        RegistHandle(MessageID.common_shop_goods_list_ret, CreateGoodsResultHandle);
        RegistHandle(MessageID.common_buy_shop_goods_ret, ResultBuyHandle);
        RegistHandle(MessageID.common_refresh_shop_goods_ret, ResultRefreshGoodListHandle);
    }
    /// <summary>
    /// 购买是否成功
    /// </summary>
    /// <param name="packet"></param>
    /// <returns></returns>
    private bool ResultBuyHandle(CReadPacket packet)
    {
        Debug.Log("<color=#FFc937>ResultBuyHandle购买商店物品</color>");
        Dictionary<string, object> data = packet.data;

        int result = int.Parse(data["ret"].ToString());
        //if (null!= result)
        //{
        //     //UIMask.Instance.ClosePanle(UIPanleID.UIPopBuy.ToString());
        //}       
        if (result == 0)
        {
            UInt32 playerMoney = UInt32.Parse(data["vv"].ToString());
            int moneyType = int.Parse(data["vt"].ToString());
            switch (moneyType)
            {
                case 1:
                    playerData.GetInstance().RoleMoneyHadler(MoneyType.Gold, playerMoney);
                    break;
                case 2:
                    playerData.GetInstance().RoleMoneyHadler(MoneyType.Diamond, playerMoney);
                    break;
                case 3:
                    playerData.GetInstance().RoleMoneyHadler(MoneyType.PVPcoin, playerMoney);
                    break;
                case 4:
                    playerData.GetInstance().RoleMoneyHadler(MoneyType.AreanCoin, playerMoney);
                    break;
                case 5:
                    playerData.GetInstance().RoleMoneyHadler(MoneyType.PVEcion, playerMoney);
                    break;
                case 6:
                    playerData.GetInstance().RoleMoneyHadler(MoneyType.RewardCoin, playerMoney);
                    break;
            }
            //UIPopBuy.instance.ShowMask();
            // UIShop.Instance.UpdateShow();
            return true;
            // break;
        }
        else
        {
            Debug.Log(string.Format("获取商店物品列表失败：{0}", data["desc"].ToString()));
            UIPromptBox.Instance.ShowLabel(data["desc"].ToString());
            return false;
        }

    }

    /// <summary>
    /// 创建商店物品，商品数据
    /// </summary>
    /// <param name="packet"></param>
    /// <returns></returns>
    private bool CreateGoodsResultHandle(CReadPacket packet)
    {
        Debug.Log("<color=#FFc937>Create GoodsResult创建商店物品，商品数据</color>");
        Dictionary<string, object> data = packet.data;

        int result = int.Parse(data["ret"].ToString());
        if (result == 0)
        {
            playerData.GetInstance().lotteryInfo.shopItemList.Clear();
            object[] goodList = data["item"] as object[];
            playerData.GetInstance().lotteryInfo.shopTime = long.Parse(data["gft"].ToString());
            playerData.GetInstance().lotteryInfo.sale = float.Parse(data["sale"].ToString());
            if (data.ContainsKey("bat"))
            {
                playerData.GetInstance().lotteryInfo.shopRefreshCount = int.Parse(data["bat"].ToString());
            }
            if (goodList != null)
            {
                for (int i = 0; i < goodList.Length; i++)
                {
                    Dictionary<string, object> goodInfo = goodList[i] as Dictionary<string, object>;
                    ItemData itemInfo = new ItemData();
                    itemInfo.Id = int.Parse(goodInfo["id"].ToString());
                    ItemNodeState vo = null;
                    if (GameLibrary.Instance().ItemStateList.ContainsKey(itemInfo.Id))
                    {
                        vo = GameLibrary.Instance().ItemStateList[itemInfo.Id];
                        itemInfo.Types = vo.types;
                        itemInfo.Name = vo.name;
                        itemInfo.Describe = vo.describe;
                        itemInfo.IconName = vo.icon_name;
                        itemInfo.Cprice = int.Parse(goodInfo["v"].ToString());
                        // itemInfo.Cprice = vo.sprice;
                        int moneyType = int.Parse(goodInfo["vt"].ToString());
                        itemInfo.Count = int.Parse(goodInfo["at"].ToString());
                        switch (moneyType)
                        {
                            case 1:
                                itemInfo.MoneyTYPE = MoneyType.Gold;
                                break;
                            case 2:
                                itemInfo.MoneyTYPE = MoneyType.Diamond;
                                break;
                            case 3:
                                itemInfo.MoneyTYPE = MoneyType.PVPcoin;
                                break;
                            case 4:
                                itemInfo.MoneyTYPE = MoneyType.AreanCoin;
                                break;
                            case 5:
                                itemInfo.MoneyTYPE = MoneyType.PVEcion;
                                break;
                            case 6:
                                itemInfo.MoneyTYPE = MoneyType.RewardCoin;
                                break;
                        }
                        switch (vo.grade)
                        {
                            case 1:
                                itemInfo.GradeTYPE = GradeType.Gray;
                                break;
                            case 2:
                                itemInfo.GradeTYPE = GradeType.Green;
                                break;
                            case 4:
                                itemInfo.GradeTYPE = GradeType.Blue;
                                break;
                            case 7:
                                itemInfo.GradeTYPE = GradeType.Purple;
                                break;
                            case 11:
                                itemInfo.GradeTYPE = GradeType.Orange;
                                break;
                            case 16:
                                itemInfo.GradeTYPE = GradeType.Red;
                                break;
                        }
                        playerData.GetInstance().lotteryInfo.shopItemList.Add(itemInfo);
                    }



                    //Singleton<LotteryResultManager>.Instance.LotteryResultSort(UIShop.Instance.itemList);
                    //Debug.Log(" 创建商店物品:" + id);
                }
                int intCount = playerData.GetInstance().lotteryInfo.shopItemList.Count / 6;
                int page = playerData.GetInstance().lotteryInfo.shopItemList.Count % 6;
                if (page != 0)
                {
                    intCount++;
                }
                playerData.GetInstance().lotteryInfo.page = intCount;
            }
            // Singleton<SceneManage>.Instance.MessageHandle(EnumSceneID.UI_MajorCity01, GameLibrary.UIShop);//没有打开的状态不刷新界面
            //Control.ShowGUI(GameLibrary.UIShop);
            // UIShop.Instance.RefrshData();//刷新方法
            return true;
        }
        else
        {
            Debug.Log(string.Format("获取商店物品列表失败：{0}", data["desc"].ToString()));
            return false;
        }

    }
    /// <summary>
    /// 刷新物品列表
    /// </summary>
    /// <param name="packet"></param>
    /// <returns></returns>
    private bool ResultRefreshGoodListHandle(CReadPacket packet)
    {
        Debug.Log("<color=#FFc937>ResultRefreshGoodListHandle刷新物品列表</color>");
        Dictionary<string, object> data = packet.data;

        int result = int.Parse(data["ret"].ToString());
        if (result == 0)
        {
            playerData.GetInstance().lotteryInfo.shopItemList.Clear();
            object[] goodList = data["item"] as object[];

            if (data.ContainsKey("vv") && data.ContainsKey("vt"))
            {
                UInt32 playerMoney = UInt32.Parse(data["vv"].ToString());
                int moneyTypes = int.Parse(data["vt"].ToString());//
                switch (moneyTypes)
                {
                    case 1:
                        playerData.GetInstance().RoleMoneyHadler(MoneyType.Gold, playerMoney);
                        break;
                    case 2:
                        playerData.GetInstance().RoleMoneyHadler(MoneyType.Diamond, playerMoney);
                        break;
                    case 3:
                        playerData.GetInstance().RoleMoneyHadler(MoneyType.PVPcoin, playerMoney);
                        break;
                    case 4:
                        playerData.GetInstance().RoleMoneyHadler(MoneyType.AreanCoin, playerMoney);
                        break;
                    case 5:
                        playerData.GetInstance().RoleMoneyHadler(MoneyType.PVEcion, playerMoney);
                        break;
                    case 6:
                        playerData.GetInstance().RoleMoneyHadler(MoneyType.RewardCoin, playerMoney);
                        break;
                }
            }
            if (data.ContainsKey("gft"))
            {
                playerData.GetInstance().lotteryInfo.shopTime = long.Parse(data["gft"].ToString());
            }
            if (data.ContainsKey("bat"))
            {
                playerData.GetInstance().lotteryInfo.shopRefreshCount = int.Parse(data["bat"].ToString());
            }
            //playerData.GetInstance().RoleMoneyHadler(MoneyType.Diamond, playerMoney);//只扣钻石改为分类扣
            for (int i = 0; i < goodList.Length; i++)
            {
                Dictionary<string, object> goodInfo = (Dictionary<string, object>)goodList[i];
                ItemData itemInfo = new ItemData();
                itemInfo.Id = int.Parse(goodInfo["id"].ToString());
                ItemNodeState vo = null;
                if (GameLibrary.Instance().ItemStateList.ContainsKey(itemInfo.Id))
                {
                    vo = GameLibrary.Instance().ItemStateList[itemInfo.Id];
                    itemInfo.Types = vo.types;
                    itemInfo.Name = vo.name;
                    itemInfo.Describe = vo.describe;
                    itemInfo.IconName = vo.icon_name;
                    itemInfo.Cprice = int.Parse(goodInfo["v"].ToString());
                    itemInfo.Count = int.Parse(goodInfo["at"].ToString());
                    int moneyType = int.Parse(goodInfo["vt"].ToString());
                    switch (moneyType)
                    {
                        case 1:
                            itemInfo.MoneyTYPE = MoneyType.Gold;
                            break;
                        case 2:
                            itemInfo.MoneyTYPE = MoneyType.Diamond;
                            break;
                        case 3:
                            itemInfo.MoneyTYPE = MoneyType.PVPcoin;
                            break;
                        case 4:
                            itemInfo.MoneyTYPE = MoneyType.AreanCoin;
                            break;
                        case 5:
                            itemInfo.MoneyTYPE = MoneyType.PVEcion;
                            break;
                        case 6:
                            itemInfo.MoneyTYPE = MoneyType.RewardCoin;
                            break;
                    }
                    switch (vo.grade)
                    {
                        case 1:
                            itemInfo.GradeTYPE = GradeType.Gray;
                            break;
                        case 2:
                            itemInfo.GradeTYPE = GradeType.Green;
                            break;
                        case 4:
                            itemInfo.GradeTYPE = GradeType.Blue;
                            break;
                        case 7:
                            itemInfo.GradeTYPE = GradeType.Purple;
                            break;
                        case 11:
                            itemInfo.GradeTYPE = GradeType.Orange;
                            break;
                        case 16:
                            itemInfo.GradeTYPE = GradeType.Red;
                            break;
                    }
                    playerData.GetInstance().lotteryInfo.shopItemList.Add(itemInfo);
                }


                // Singleton<LotteryResultManager>.Instance.LotteryResultSort(UIShop.Instance.itemList);
                //Debug.Log(" 创建商店物品:" + id);
            }
            int intCount = playerData.GetInstance().lotteryInfo.shopItemList.Count / 6;
            int page = playerData.GetInstance().lotteryInfo.shopItemList.Count % 6;
            if (page != 0)
            {
                intCount++;
            }
            playerData.GetInstance().lotteryInfo.page = intCount;
            //UIPopRefresh.instance.ReceiveDate();
            UIShop.Instance.ReceiveDate();
            UIShop.Instance.UpdateShow();
            UIShop.Instance.SetLostTime();//到点免费刷新并从新设定值
            return true;
        }
        //else if (result == 1)
        //{
        //    Debug.Log(string.Format("获取商店物品列表失败：{0}", data["desc"].ToString()));
        //  //  return false;
        //    //UIPromptBox.Instance.ShowLabel(data["desc"].ToString()));
        //}
        else// if (result == 3)
        {
            Debug.Log(string.Format("获取商店物品列表失败：{0}", data["desc"].ToString()));
            Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, data["desc"].ToString());
            UIPromptBox.Instance.ShowLabel(data["desc"].ToString());
           // Control.HideGUI(UIPanleID.UIMask);
            return false;
            // Control.HideGUI(UIPanleID.UIPopRefresh);
        }
       
    }
}
