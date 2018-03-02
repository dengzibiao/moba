using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tianyu;
using System.Text;
/// <summary>
/// 物品数据操作类 ：增加物品  出售物品 查找物品 （减少）使用物品
/// </summary>
public class GoodsDataOperation
{

    static GoodsDataOperation instance;
    private bool isHave;
    private bool isCanGetGoods;

    //背包中当前被出售的物品和个数（用于出售成功后刷新背包界面）
    public ItemData equipItem;
    public int currentCount = 1;

    public static GoodsDataOperation GetInstance()
    {
        if (instance == null)
            instance = new GoodsDataOperation();
        return instance;
    }
    #region 查找物品
    /// <summary>
    /// 通过ID查找背包物品
    /// </summary>
    /// <param name="itemid">物品id</param>
    /// <returns></returns>
    public ItemData GetItemDatatByID(long itemid)
    {
        if (playerData.GetInstance().baginfo.ItemDic.ContainsKey(itemid))
        {
            return playerData.GetInstance().baginfo.ItemDic[itemid];
        }
        return null;
    }

    /// <summary>
    /// 通过物品类型查找背包物品
    /// </summary>
    /// <param name="type">物品类型</param>
    /// <returns></returns>
    public List<ItemData> GetItemListByItmeType(ItemType type)
    {
        List<ItemData> list = new List<ItemData>();
        foreach (ItemData item in playerData.GetInstance().baginfo.ItemDic.Values)
        {
            if ((int)type == GameLibrary.Instance().ItemStateList[item.Id].types)
            {
                list.Add(item);
            }
        }
        return list;
    }

    /// <summary>
    /// 通过物品类型查找背包物品
    /// </summary>
    /// <param name="type">可变参数（传递多个类型）</param>
    /// <returns></returns>
    public List<ItemData> GetItemListByItmeType(params ItemType[] type)
    {
        List<ItemData> list = new List<ItemData>();
        foreach (ItemData item in playerData.GetInstance().baginfo.ItemDic.Values)
        {
            for (int j = 0; j < type.Length; j++)
            {
                if ((int)type[j] == GameLibrary.Instance().ItemStateList[item.Id].types)//((int)type[j] == (VOManager.Instance().GetCSV<ItemCSV>("Item").GetVO((baginfo.itemlist[i].Id)).types))
                {
                    list.Add(item);
                }
            }
        }
        return list;
    }

    /// <summary>
    /// 通过ID查找物品数量
    /// </summary>
    /// <param name="itemid">物品id</param>
    /// <returns></returns>
    public int GetItemCountById(long itemid)
    {
        if (playerData.GetInstance().baginfo.ItemDic.ContainsKey(itemid))
        {
            return playerData.GetInstance().baginfo.ItemDic[itemid].Count;
        }
        return 0;
    }
    #endregion

    #region 出售物品
    /// <summary>
    /// 出售物品
    /// </summary>
    /// <param name="itemid">物品id</param>
    /// <param name="uuid">物品uuid</param>
    /// <param name="count">数量</param>
    public void SaleGood(long itemid, string uuid, int count)
    {
        for (int i = playerData.GetInstance().baginfo.itemlist.Count - 1; i >= 0 ; i--)
        {
            if (playerData.GetInstance().baginfo.itemlist[i].Id == itemid)
            {
                if (count < playerData.GetInstance().baginfo.itemlist[i].Count)
                {
                    //playerData.GetInstance().MoneyHadler(MoneyType.Gold, count * playerData.GetInstance().baginfo.itemlist[i].Sprice);
                    playerData.GetInstance().baginfo.itemlist[i].Count = (playerData.GetInstance().baginfo.itemlist[i].Count - count);
                    //Globe.seletIndex = 0;
                    break;

                }
                else if (count == playerData.GetInstance().baginfo.itemlist[i].Count)
                {
                    //playerData.GetInstance().MoneyHadler(MoneyType.Gold, count * playerData.GetInstance().baginfo.itemlist[i].Sprice);
                    playerData.GetInstance().baginfo.itemlist.RemoveAt(i);
                    Globe.seletIndex = 0;
                    break;

                }
            }
        }
        ItemListTypeChangeToDic();
        //不打开背包界面改变道具 只处理数据不刷新界面（防null）
        if (Globe.isExternalChangeGoods)
        {
            return;
        }
        //出售物品后刷新界面
        UIKnapsack.Instance.RefreshView();
    }

    /// <summary>
    /// 出售金币道具（用于商城）
    /// </summary>
    public void SaleGoldProp()
    {
        GameLibrary.saleItemList.Clear();
        List<ItemData> data = playerData.GetInstance().GetItemListByItmeType(ItemType.GoldProp);
        for (int i = 0; i < data.Count; i++)
        {
            object[] obj = new object[] { data[i].Id, data[i].Uuid, data[i].Count };
            GameLibrary.saleItemList.Add(obj);
        }
        Globe.isSaleSingleGood = false;
        Globe.isExternalChangeGoods = true;
        ClientSendDataMgr.GetSingle().GetItemSend().SendSellItem(GameLibrary.saleItemList);
    }

    /// <summary>
    /// 出售金币道具成功后 处理本地数据
    /// </summary>
    public void RefreshGoldProp()
    {
        List<ItemData> data = playerData.GetInstance().GetItemListByItmeType(ItemType.GoldProp);
        for (int j = 0; j < data.Count; j++)
        {
            SaleGood(data[j].Id, data[j].Uuid, data[j].Count);
            if ((j + 1) == data.Count)
            {
                Globe.isExternalChangeGoods = false;
            }
        }
    }

    /// <summary>
    /// 出售单个物品后刷新数据
    /// </summary>
    public void RefreshGood()
    {
        if (equipItem!=null)
        {
            SaleGood(equipItem.Id, equipItem.Uuid, currentCount);
        }
        equipItem = null;
        currentCount = 0;
        //UIKnapsack.Instance.SaleGood(equipItem.Id, equipItem.Uuid, currentCount);
    }
    #endregion

    #region 增加物品重载 背包中只展示 材料 灵魂石 消耗品 符文 其他物品不展示但要存入背包数据
    //道具类型 1：装备，2：材料，3：材料碎片，4：金币道具，5：经验道具，6：英雄灵魂石，7：英雄整卡，8：符文，9：恢复道具，10：其他，11：宝箱
    /// <summary>
    /// 增加物品
    /// </summary>
    /// <param name="obj">LotteryGoldVO</param>
    /// <param name="count"></param>
    public void AddGoods(ItemData obj)
    {
        //ItemData itemdata = obj;
        //if (itemdata == null) return;
        //if (itemdata.Count == 0) return;
        //if (AddGoodsToBag(itemdata.Id, itemdata.Count))
        //{
        //    ItemListTypeChangeToDic();
        //    //物品数量改变 更新一下任务追踪信息
        //    if (Control.GetGUI(GameLibrary.UITaskTracker).gameObject.activeSelf)
        //    {
        //        Control.ShowGUI(GameLibrary.UITaskTracker);
        //    }

        //    //刷新背包界面
        //    if (Control.GetGUI(GameLibrary.UIKnapsack).gameObject.activeSelf)
        //    {
        //        Debug.Log("增加物品成功并刷新");
        //        UIKnapsack.Instance.RefreshView();
        //    }
        //}
        //else
        //{
        //    Debug.Log("该物品已经达到上限");
        //    //将不能领取的物品托送到邮箱 TODO
        //}
    }
    /// <summary>
    /// 增加物品
    /// </summary>
    /// <param name="type"></param>
    public void AddGoods(params List<ItemData>[] type)
    {
        //if (type.Length == 0) return;
        //for (int i =0; i<type.Length;i++)
        //{
        //    for (int j = 0;j<type[i].Count;j++)
        //    {
        //        if (AddGoodsToBag(type[i][j].Id, type[i][j].Count))
        //        {
        //            ItemListTypeChangeToDic();

        //            //物品数量改变 更新一下任务追踪信息
        //            if (Control.GetGUI(GameLibrary.UITaskTracker).gameObject.activeSelf)
        //            {
        //                Control.ShowGUI(GameLibrary.UITaskTracker);
        //            }
        //            //刷新背包界面
        //            if (Control.GetGUI(GameLibrary.UIKnapsack).gameObject.activeSelf)
        //            {
        //                Debug.Log("增加物品成功并刷新");
        //                UIKnapsack.Instance.RefreshView();
        //            }
        //        }
        //        else
        //        {
        //            Debug.Log("该物品已经达到上限");
        //            //将不能领取的物品托送到邮箱 TODO
        //        }
        //    }
        //}
    }

    /// <summary>
    /// 增加物品
    /// </summary>
    /// <param name="id"></param>
    /// <param name="count"></param>
    public void AddGoods(long id, int count)
    {
        //if (count == 0) return;
        ////增加邮件中的物品肯定是成功的，因为超过上限就不让领取了 在领取邮件的时候已经判断过了
        ////主要是处理商城 魂匣购买的物品，当增加后物品数量达到上限 就将该物品的数量设置为该物品上限
        //if (AddGoodsToBag(id, count))
        //{
        //    ItemListTypeChangeToDic();
        //    //物品数量改变 更新一下任务追踪信息
        //    if (Control.GetGUI(GameLibrary.UITaskTracker).gameObject.activeSelf)
        //    {
        //        Control.ShowGUI(GameLibrary.UITaskTracker);
        //    }
        //    //刷新背包界面
        //    if (Control.GetGUI(GameLibrary.UIKnapsack).gameObject.activeSelf)
        //    {
        //        Debug.Log("增加物品成功并刷新");
        //        UIKnapsack.Instance.RefreshView();
        //    }
           
        //}
        //else
        //{
        //    //将不能领取的物品托送到邮箱 TODO
        //    Debug.Log("物品达到上限 多余物品被发送到邮箱");
        //}
    }
    bool AddGoodsToBag(long id, int count)
    {
        int surplusCount = 0;
        //先判断 背包中有无物品 如果有物品再判断当前数量加上count是否超过上限 如果无物品判断count是否超过上限   不满足领取条件要通知服务器发送到邮箱

        ItemData item;
        if ((item = GetItemDatatByID(id)) != null)
        {
            isHave = true;
            if (GameLibrary.Instance().ItemStateList.ContainsKey(id)&&item.Count + count > GameLibrary.Instance().ItemStateList[id].piles)
            {
                isCanGetGoods = false;
            }
            else
            {
                isCanGetGoods = true;
            }
        }
        else
        {
            isHave = false;
            if (GameLibrary.Instance().ItemStateList.ContainsKey(id))
            {
                if (count > GameLibrary.Instance().ItemStateList[id].piles)
                    isCanGetGoods = false;
                else
                    isCanGetGoods = true;
            }else
            {
                Debug.Log(id);
            }
        }
        if (isHave && isCanGetGoods)// 背包中有该物品 增加后没超过上限
        {
            //将改变的物品放到第一位
            for (int i = 0; i < playerData.GetInstance().baginfo.itemlist.Count; i++)
            {
                if (id == playerData.GetInstance().baginfo.itemlist[i].Id)
                {
                    isHave = true;
                    ItemData tempData = playerData.GetInstance().baginfo.itemlist[i];
                    playerData.GetInstance().baginfo.itemlist.RemoveAt(i);
                    tempData.Count += count;
                    playerData.GetInstance().baginfo.itemlist.Insert(0, tempData);
                    break;
                }
            }

        }
        else if (!isHave && isCanGetGoods)//背包中没有该物品 物品数量没超过上限
        {
            ItemData itemdata = new ItemData();

            itemdata.Id = id;

            itemdata.Count = count;
            ItemNodeState itemstate = null;
            if(GameLibrary.Instance().ItemStateList.ContainsKey(itemdata.Id))
            {
                itemstate = GameLibrary.Instance().ItemStateList[itemdata.Id];

                itemdata.Name = itemstate.name;
                itemdata.Types = itemstate.types;
                itemdata.Describe = itemstate.describe;

                itemdata.Sprice = itemstate.sprice;
                itemdata.Piles = itemstate.piles;
                itemdata.IconName = itemstate.icon_name;
                if (itemdata.Types == 6)
                {
                    itemdata.UiAtlas = ResourceManager.Instance().GetUIAtlas("UIHeroHead");
                }
                else
                {
                    itemdata.UiAtlas = ResourceManager.Instance().GetUIAtlas("Prop");
                }

                switch (GameLibrary.Instance().ItemStateList[itemdata.Id].grade)//(VOManager.Instance().GetCSV<ItemCSV>("Item").GetVO(itemdata.Id).grade)
                {
                    case 1:
                        itemdata.GradeTYPE = GradeType.Gray;
                        break;
                    case 2:
                        itemdata.GradeTYPE = GradeType.Green;
                        break;
                    case 4:
                        itemdata.GradeTYPE = GradeType.Blue;
                        break;
                    case 7:
                        itemdata.GradeTYPE = GradeType.Purple;
                        break;
                    case 11:
                        itemdata.GradeTYPE = GradeType.Orange;
                        break;
                    case 16:
                        itemdata.GradeTYPE = GradeType.Red;
                        break;
                }


            }
            playerData.GetInstance().baginfo.itemlist.Insert(0, itemdata);
            //playerData.GetInstance().baginfo.ItemDic.Add(id,itemdata);
        }
        else if (isHave && !isCanGetGoods)//背包中有该物品 增加后超过物品上限
        {
            //将改变的物品放到第一位
            for (int i = 0; i < playerData.GetInstance().baginfo.itemlist.Count; i++)
            {
                if (id == playerData.GetInstance().baginfo.itemlist[i].Id)
                {
                    isHave = true;
                    ItemData tempData = playerData.GetInstance().baginfo.itemlist[i];
                    playerData.GetInstance().baginfo.itemlist.RemoveAt(i);
                    tempData.Count = GameLibrary.Instance().ItemStateList[id].piles;
                    playerData.GetInstance().baginfo.itemlist.Insert(0, tempData);
                    break;
                }
            }
            //多余的物品处理
            surplusCount = item.Count + count - GameLibrary.Instance().ItemStateList[id].piles;
            return false;
        }
        else if (!isHave && !isCanGetGoods)//背包中没有该物品 物品数量超过物品上限
        {
            ItemData itemdata = new ItemData();

            itemdata.Id = id;
            ItemNodeState itemstate = null;
            if(GameLibrary.Instance().ItemStateList.ContainsKey(itemdata.Id))
            {
                itemstate = GameLibrary.Instance().ItemStateList[itemdata.Id];

                itemdata.Count = itemstate.piles;
                itemdata.Name = itemstate.name;
                itemdata.Types = itemstate.types;
                itemdata.Describe = itemstate.describe;
                if (itemdata.Types == 6)
                {
                    itemdata.UiAtlas = ResourceManager.Instance().GetUIAtlas("UIHeroHead");
                }
                else
                {
                    itemdata.UiAtlas = ResourceManager.Instance().GetUIAtlas("Prop");
                }
                switch (itemstate.grade)//(VOManager.Instance().GetCSV<ItemCSV>("Item").GetVO(itemdata.Id).grade)
                {
                    case 1:
                        itemdata.GradeTYPE = GradeType.Gray;
                        break;
                    case 2:
                        itemdata.GradeTYPE = GradeType.Green;
                        break;
                    case 4:
                        itemdata.GradeTYPE = GradeType.Blue;
                        break;
                    case 7:
                        itemdata.GradeTYPE = GradeType.Purple;
                        break;
                    case 11:
                        itemdata.GradeTYPE = GradeType.Orange;
                        break;
                    case 16:
                        itemdata.GradeTYPE = GradeType.Red;
                        break;
                }
                itemdata.Sprice = itemstate.sprice;
                itemdata.Piles = itemstate.piles;
                itemdata.IconName = itemstate.icon_name;

                playerData.GetInstance().baginfo.itemlist.Insert(0, itemdata);
                //playerData.GetInstance().baginfo.ItemDic.Add(id,itemdata);
                //多余的物品处理
                surplusCount = count - itemstate.piles;
            }
          

            
            return false;
        }
        return true;
    }
    #endregion
    #region 使用物品
    /// <summary>
    /// 使用物品
    /// </summary>
    /// <param name="id"></param>
    /// <param name="count"></param>
    public void UseGoods(int id, int count)
    {
        //if (count <= 0) return;
        //if (GetItemCountById(id) < count) return;
        //for (int i = 0; i < playerData.GetInstance().baginfo.itemlist.Count; i++)
        //{
        //    if (playerData.GetInstance().baginfo.itemlist[i].Id == id)
        //    {
        //        if (count < playerData.GetInstance().baginfo.itemlist[i].Count)
        //        {
        //            playerData.GetInstance().baginfo.itemlist[i].Count = (short)(playerData.GetInstance().baginfo.itemlist[i].Count - count);
        //            break;

        //        }
        //        else if (count == playerData.GetInstance().baginfo.itemlist[i].Count)
        //        {
        //            playerData.GetInstance().baginfo.itemlist.RemoveAt(i);
        //            break;
        //        }
        //    }
        //}
        //ItemListTypeChangeToDic();
        //if (Control.GetGUI(GameLibrary.UIKnapsack).gameObject.activeSelf)
        //{
        //    //刷新背包界面
        //    UIKnapsack.Instance.RefreshView();
        //}
    }
    /// <summary>
    /// 使用物品
    /// </summary>
    /// <param name="dic">物品id和数量的字典</param>
    public void UseGoods(Dictionary<long,int> dic)
    {
        //if (dic.Count <= 0) return;
        //foreach (long key in dic.Keys)
        //{

        //    for (int i = 0; i < playerData.GetInstance().baginfo.itemlist.Count; i++)
        //    {
        //        if (playerData.GetInstance().baginfo.itemlist[i].Id == key)
        //        {
        //            if (dic[key] < playerData.GetInstance().baginfo.itemlist[i].Count)
        //            {
        //                playerData.GetInstance().baginfo.itemlist[i].Count = (short)(playerData.GetInstance().baginfo.itemlist[i].Count - dic[key]);
        //                break;

        //            }
        //            else if (dic[key] == playerData.GetInstance().baginfo.itemlist[i].Count)
        //            {
        //                playerData.GetInstance().baginfo.itemlist.RemoveAt(i);
        //                break;
        //            }
        //        }
        //    }

        //}
        //ItemListTypeChangeToDic();
        //if (Control.GetGUI(GameLibrary.UIKnapsack).gameObject.activeSelf)
        //{
        //    //刷新背包界面
        //    UIKnapsack.Instance.RefreshView();
        //}
    }
    #endregion


    #region 更新物品
    public void UpdateGoods(long id, int count,string uuid)
    {

        //只要物品有改变 后端就会给我推送改变的物品和数量，当数量为0的时候就把该物品从背包中删除
        ItemData item;
        if ((item = GetItemDatatByID(id)) != null)//背包中有这个物品
        {
            if (count <= 0)//数量为0删除这个物品
            {
                for (int i = playerData.GetInstance().baginfo.itemlist.Count - 1; i >= 0; i--)
                {
                    if (playerData.GetInstance().baginfo.itemlist[i].Id == id)
                    {
                        playerData.GetInstance().baginfo.itemlist.RemoveAt(i);
                        Globe.seletIndex = 0;
                        break;
                    }
                }
            }
            else
            {
                //更新数量
                for (int i = 0; i < playerData.GetInstance().baginfo.itemlist.Count; i++)
                {
                    if (id == playerData.GetInstance().baginfo.itemlist[i].Id)
                    {
                        //ItemData tempData = playerData.GetInstance().baginfo.itemlist[i];
                        //playerData.GetInstance().baginfo.itemlist.RemoveAt(i);
                        //if (GameLibrary.Instance().ItemStateList.ContainsKey(id) && count > GameLibrary.Instance().ItemStateList[id].piles)//如果后端发过来的数据大于物品上限就将物品数量设置为上限
                        //{
                        //    tempData.Count = GameLibrary.Instance().ItemStateList[id].piles;
                        //}
                        //else
                        //{
                        //    tempData.Count = count;
                        //}
                        //playerData.GetInstance().baginfo.itemlist.Insert(0, tempData);
                        if (GameLibrary.Instance().ItemStateList.ContainsKey(id) && count > GameLibrary.Instance().ItemStateList[id].piles)//如果后端发过来的数据大于物品上限就将物品数量设置为上限
                        {
                            playerData.GetInstance().baginfo.itemlist[i].Count = GameLibrary.Instance().ItemStateList[id].piles;
                        }
                        else
                        {
                            playerData.GetInstance().baginfo.itemlist[i].Count = count;
                        }
                        break;
                    }
                }
            }

        }
        else//背包中没有这个物品
        {
            if (count <= 0) return;
            if (count > 0)
            {
                ItemData itemdata = new ItemData();

                itemdata.Id = id;
                itemdata.Uuid = uuid;
                if (GameLibrary.Instance().ItemStateList.ContainsKey(id) && count > GameLibrary.Instance().ItemStateList[id].piles)//如果后端发过来的数据大于物品上限就将物品数量设置为上限
                {
                    itemdata.Count = GameLibrary.Instance().ItemStateList[id].piles;
                }
                else
                {
                    itemdata.Count = count;
                }
                ItemNodeState itemstate = null;
                if (GameLibrary.Instance().ItemStateList.ContainsKey(itemdata.Id))
                {
                    itemstate = GameLibrary.Instance().ItemStateList[itemdata.Id];

                    itemdata.Name = itemstate.name;
                    itemdata.Types = itemstate.types;
                    itemdata.Describe = itemstate.describe;

                    itemdata.Sprice = itemstate.sprice;
                    itemdata.Piles = itemstate.piles;
                    itemdata.IconName = itemstate.icon_name;
                    if (itemdata.Types == 6)
                    {
                        itemdata.UiAtlas = ResourceManager.Instance().GetUIAtlas("UIHeroHead");
                    }
                    else
                    {
                        itemdata.UiAtlas = ResourceManager.Instance().GetUIAtlas("Prop");
                    }

                    switch (GameLibrary.Instance().ItemStateList[itemdata.Id].grade)//(VOManager.Instance().GetCSV<ItemCSV>("Item").GetVO(itemdata.Id).grade)
                    {
                        case 1:
                            itemdata.GradeTYPE = GradeType.Gray;
                            break;
                        case 2:
                            itemdata.GradeTYPE = GradeType.Green;
                            break;
                        case 4:
                            itemdata.GradeTYPE = GradeType.Blue;
                            break;
                        case 7:
                            itemdata.GradeTYPE = GradeType.Purple;
                            break;
                        case 11:
                            itemdata.GradeTYPE = GradeType.Orange;
                            break;
                        case 16:
                            itemdata.GradeTYPE = GradeType.Red;
                            break;
                    }


                }
                //新增加的物品插入到第一位
                playerData.GetInstance().baginfo.itemlist.Insert(0, itemdata);
            }
        }
    }
    #endregion

    /// <summary>
    /// 将背包物品 list格式转换成字典方便查找
    /// </summary>
    public void ItemListTypeChangeToDic()
    {
        playerData.GetInstance().baginfo.ItemDic.Clear();
        foreach (ItemData val in playerData.GetInstance().baginfo.itemlist)
        {
            playerData.GetInstance().baginfo.ItemDic.Add(val.Id,val);
        }
    }
    #region 获得物品品质框和英雄品质框 以及颜色
    /// <summary>
    /// 根据品质获得名字颜色值
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public string GetNameColour(GradeType type)
    {
        string colour = "";
        switch (type)
        {
            case GradeType.Gray:
                colour = fc_white;
                break;
            case GradeType.Green:
            case GradeType.Green1:
                colour = fc_green;
                break;
            case GradeType.Blue:
            case GradeType.Blue1:
            case GradeType.Blue2:
                colour = fc_blue;
                break;
            case GradeType.Purple:
            case GradeType.Purple1:
            case GradeType.Purple2:
            case GradeType.Purple3:
                colour = fc_purple;
                break;
            case GradeType.Orange:
            case GradeType.Orange1:
            case GradeType.Orange2:
            case GradeType.Orange3:
            case GradeType.Orange4:
                colour = fc_orange;
                break;
            case GradeType.Red:
            case GradeType.Red1:
            case GradeType.Red2:
            case GradeType.Red3:
            case GradeType.Red4:
            case GradeType.Red5:
                colour = fc_red;
                break;
            default:
                break;
        }
        return colour;
    }
    public string GetEquipStrengthCount(GradeType type)
    {
        string colour = "";
        switch (type)
        {
            case GradeType.Gray:
                colour = "";
                break;
            case GradeType.Green:
                colour = "";
                break;
            case GradeType.Green1:
                colour = "+1";
                break;
            case GradeType.Blue:
                colour = "";
                break;
            case GradeType.Blue1:
                colour = "+1";
                break;
            case GradeType.Blue2:
                colour = "+2";
                break;
            case GradeType.Purple:
                colour = "";
                break;
            case GradeType.Purple1:
                colour = "+1";
                break;
            case GradeType.Purple2:
                colour = "+2";
                break;
            case GradeType.Purple3:
                colour = "+3";
                break;
            case GradeType.Orange:
                colour = "";
                break;
            case GradeType.Orange1:
                colour = "+1";
                break;
            case GradeType.Orange2:
                colour = "+2";
                break;
            case GradeType.Orange3:
                colour = "+3";
                break;
            case GradeType.Orange4:
                colour = "+4";
                break;
            case GradeType.Red:
                colour = "";
                break;
            case GradeType.Red1:
                colour = "+1";
                break;
            case GradeType.Red2:
                colour = "+2";
                break;
            case GradeType.Red3:
                colour = "+3";
                break;
            case GradeType.Red4:
                colour = "+4";
                break;
            case GradeType.Red5:
                colour = "+5";
                break;
            default:
                break;
        }
        return colour;
    }
    /// <summary>
    /// 根据英雄品质获得名字颜色值
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public string GetNameColourByHeroGrade(int type)
    {
        string colour = "";
        switch (type)
        {
            case 1:
                colour = fc_white;
                break;
            case 2:
                colour = fc_green;
                break;
            case 3:
                colour = fc_blue;
                break;
            case 4:
                colour = fc_purple;
                break;
            case 5:
                colour = fc_orange;
                break;
            case 6:
                colour = fc_red;
                break;
            default:
                break;
        }
        return colour;
    }
    /// <summary>
    /// 将名字和响应的品质颜色值拼接
    /// </summary>
    /// <param name="name"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public string JointNameColour(string name, GradeType type)
    {
        return "[" + GetNameColour(type) + "]" + name + "[-]";
    }
    public string JointEquipNameColour(string name, GradeType type)
    {
        return "[" + GetNameColour(type) + "]" + name + GetEquipStrengthCount(type)+"[-]";
    }
    public string fc_white = "ffffff";
    public string fc_green = "2dd740";
    public string fc_blue = "5eaeff";
    public string fc_purple = "9c35fe";
    public string fc_orange = "f78204";
    public string fc_red = "ff0000";
    /// <summary>
    /// 通过英雄品质 获得英雄列表的大品质框
    /// </summary>
    /// <param name="grade"></param>
    /// <returns></returns>
    public string GetHeroGrameByHeroGrade(int grade)
    {
        switch (grade)
        {
            case 1:
                return "baikuang";
            case 2:
                return "lvkuang";
            case 3:
                return "lankuang";
            case 4:
                return "zikuang";
            case 5:
                return "chengkuang";
            case 6:
                return "hongkuang";
            default:
                return "baikuang";
        }
    }
    /// <summary>
    /// 通过英雄品质 获得英雄头像的小品质框
    /// </summary>
    /// <param name="grade"></param>
    /// <returns></returns>
    public string GetSmallHeroGrameByHeroGrade(int grade)
    {
        switch (grade)
        {
            case 1:
                return "bai";
            case 2:
                return "lv";
            case 3:
                return "lan";
            case 4:
                return "zi";
            case 5:
                return "cheng";
            case 6:
                return "hong";
            default:
                return "bai";
        }
    }
    /// <summary>
    /// 根据品质获得品质框的名称
    /// </summary>
    /// <param name="gradeType"></param>
    /// <returns></returns>
    public string GetFrameByGradeType(GradeType gradeType)
    {
        switch (gradeType)
        {
            case GradeType.Green:
                return "lv";
            case GradeType.Green1:
                return "lv1";
            case GradeType.Blue:
                return "lan";
            case GradeType.Blue1:
                return "lan1";
            case GradeType.Blue2:
                return "lan2";
            case GradeType.Purple:
                return "zi";
            case GradeType.Purple1:
                return "zi1";
            case GradeType.Purple2:
                return "zi2";
            case GradeType.Purple3:
                return "zi3";
            case GradeType.Orange:
            case GradeType.Orange1:
            case GradeType.Orange2:
            case GradeType.Orange3:
            case GradeType.Orange4:
                return "cheng";
            case GradeType.Red:
            case GradeType.Red1:
            case GradeType.Red2:
            case GradeType.Red3:
            case GradeType.Red4:
            case GradeType.Red5:
                return "hong";
            case GradeType.Gray:
            default:
                return "hui";
        }
    }
    public string GetFrameByGrade(int grade)
    {
        switch (grade)
        {
            case 2:
                return "lv";
            case 3:
                return "lv1";
            case 4:
                return "lan";
            case 5:
                return "lan1";
            case 6:
                return "lan2";
            case 7:
                return "zi";
            case 8:
                return "zi1";
            case 9:
                return "zi2";
            case 10:
                return "zi3";
            case 11:
            case 12:
            case 13:
            case 14:
            case 15:
                return "cheng";
            case 16:
            case 17:
            case 18:
            case 19:
            case 20:
            case 21:
                return "hong";
            case 1:
            default:
                return "hui";
        }
    }
    #endregion
    /// <summary>
    /// 转换item描述文字
    /// </summary>
    /// <param name="itemdata"></param>
    /// <returns></returns>
    public string ConvertGoodsDes(ItemData itemdata)
    {
        ItemNodeState itemnodestate = null;
        //道具类型 1：装备，2：材料，3：材料碎片，4：金币道具，5：经验道具，6：英雄灵魂石，7：英雄整卡，8：符文，9：恢复道具，10：其他，11：宝箱。(4,5,9,11为消耗品)
        if (itemdata.Types == (int)ItemType.SoulStone)
        {
            int heroid = int.Parse(StringUtil.StrReplace((itemdata.Id).ToString(), "201", 0, 3));
            if (FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList.ContainsKey(heroid))
            {
                int initStar = FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList[heroid].init_star;
                int count = FSDataNodeTable<StarUpGradeNode>.GetSingleton().DataNodeList[initStar].call_stone_num;
                return itemdata.Describe.Replace("[N]", count.ToString());
            }
        }
        else if (itemdata.Types == (int)ItemType.Rune)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(itemdata.Describe);

            if (GameLibrary.Instance().ItemStateList.ContainsKey(itemdata.Id))
            {
                itemnodestate = GameLibrary.Instance().ItemStateList[itemdata.Id];
                if (itemnodestate.power > 0) sb.Append("力量" + itemnodestate.power.ToString());
                if (itemnodestate.intelligence > 0) sb.Append("智力" + itemnodestate.intelligence.ToString());
                if (itemnodestate.agility > 0) sb.Append("敏捷" + itemnodestate.agility.ToString());
                if (itemnodestate.hp > 0) sb.Append("生命值" + itemnodestate.hp.ToString());
                if (itemnodestate.attack > 0) sb.Append("攻击强度" + itemnodestate.attack.ToString());
                if (itemnodestate.armor > 0) sb.Append("护甲" + itemnodestate.armor.ToString());
                if (itemnodestate.magic_resist > 0) sb.Append("魔抗" + itemnodestate.magic_resist.ToString());
                if (itemnodestate.critical > 0) sb.Append("暴击" + itemnodestate.critical.ToString());
                if (itemnodestate.dodge > 0) sb.Append("闪避" + itemnodestate.dodge.ToString());
                if (itemnodestate.hit_ratio > 0) sb.Append("命中" + itemnodestate.hit_ratio.ToString());
                if (itemnodestate.armor_penetration > 0) sb.Append("护甲穿透" + itemnodestate.armor_penetration.ToString());
                if (itemnodestate.magic_penetration > 0) sb.Append("魔法穿透" + itemnodestate.magic_penetration.ToString());
                //缺少吸血 和 韧性
                if (itemnodestate.movement_speed > 0) sb.Append("移动速度" + itemnodestate.movement_speed.ToString());
                if (itemnodestate.attack_speed > 0) sb.Append("攻击速度" + itemnodestate.attack_speed.ToString());
                if (itemnodestate.striking_distance > 0) sb.Append("攻击距离" + itemnodestate.striking_distance.ToString());
                //缺少生命恢复
            }
            return sb.ToString();
        }
        else if (itemdata.Types == (int)ItemType.ExpProp)
        {
            if (GameLibrary.Instance().ItemStateList.ContainsKey(itemdata.Id))
            {
                itemnodestate = GameLibrary.Instance().ItemStateList[itemdata.Id];
                return itemdata.Describe.Replace("[N]", itemnodestate.exp_gain.ToString());
            }
        }
        else if (itemdata.Types == (int)ItemType.GoldProp)
        {
            if (GameLibrary.Instance().ItemStateList.ContainsKey(itemdata.Id))
            {
                itemnodestate = GameLibrary.Instance().ItemStateList[itemdata.Id];
                return itemdata.Describe.Replace("[N]", itemnodestate.sprice.ToString());
            }
        }
        else if (itemdata.Types == (int)ItemType.RecoverProp)
        {
            if (GameLibrary.Instance().ItemStateList.ContainsKey(itemdata.Id))
            {
                itemnodestate = GameLibrary.Instance().ItemStateList[itemdata.Id];
                return itemdata.Describe.Replace("[N]", itemnodestate.power_add.ToString());
            }
        }
        return itemdata.Describe;
    }
    /// <summary>
    /// 转换item描述文字
    /// </summary>
    /// <param name="itemdata"></param>
    /// <returns></returns>
    public string GetConvertGoodsDes(ItemNodeState ItemNode, long Id)
    {
        ItemNodeState itemnodestate = null;
        //道具类型 1：装备，2：材料，3：材料碎片，4：金币道具，5：经验道具，6：英雄灵魂石，7：英雄整卡，8：符文，9：恢复道具，10：其他，11：宝箱。(4,5,9,11为消耗品)
        if (ItemNode.types == (int)decType.SoulStone)
        {
            int heroid = int.Parse(StringUtil.StrReplace((Id).ToString(), "201", 0, 3));
            if (FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList.ContainsKey(heroid))
            {
                int initStar = FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList[heroid].init_star;
                int count = FSDataNodeTable<StarUpGradeNode>.GetSingleton().DataNodeList[initStar].call_stone_num;
                return ItemNode.describe.Replace("[N]", count.ToString());
            }
        }
        else if (ItemNode.types == (int)decType.Rune)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(ItemNode.describe);

            if (GameLibrary.Instance().ItemStateList.ContainsKey(Id))
            {
                itemnodestate = GameLibrary.Instance().ItemStateList[Id];
                if (itemnodestate.power > 0) sb.Append("力量" + itemnodestate.power.ToString());
                if (itemnodestate.intelligence > 0) sb.Append("智力" + itemnodestate.intelligence.ToString());
                if (itemnodestate.agility > 0) sb.Append("敏捷" + itemnodestate.agility.ToString());
                if (itemnodestate.hp > 0) sb.Append("生命值" + itemnodestate.hp.ToString());
                if (itemnodestate.attack > 0) sb.Append("攻击强度" + itemnodestate.attack.ToString());
                if (itemnodestate.armor > 0) sb.Append("护甲" + itemnodestate.armor.ToString());
                if (itemnodestate.magic_resist > 0) sb.Append("魔抗" + itemnodestate.magic_resist.ToString());
                if (itemnodestate.critical > 0) sb.Append("暴击" + itemnodestate.critical.ToString());
                if (itemnodestate.dodge > 0) sb.Append("闪避" + itemnodestate.dodge.ToString());
                if (itemnodestate.hit_ratio > 0) sb.Append("命中" + itemnodestate.hit_ratio.ToString());
                if (itemnodestate.armor_penetration > 0) sb.Append("护甲穿透" + itemnodestate.armor_penetration.ToString());
                if (itemnodestate.magic_penetration > 0) sb.Append("魔法穿透" + itemnodestate.magic_penetration.ToString());
                //缺少吸血 和 韧性
                if (itemnodestate.movement_speed > 0) sb.Append("移动速度" + itemnodestate.movement_speed.ToString());
                if (itemnodestate.attack_speed > 0) sb.Append("攻击速度" + itemnodestate.attack_speed.ToString());
                if (itemnodestate.striking_distance > 0) sb.Append("攻击距离" + itemnodestate.striking_distance.ToString());
                //缺少生命恢复
            }
            return sb.ToString();
        }
        else if (ItemNode.types == (int)decType.ExpProp)
        {
            if (GameLibrary.Instance().ItemStateList.ContainsKey(Id))
            {
                itemnodestate = GameLibrary.Instance().ItemStateList[Id];
                return ItemNode.describe.Replace("[N]", itemnodestate.exp_gain.ToString());
            }
        }
        else if (ItemNode.types == (int)decType.GoldProp)
        {
            if (GameLibrary.Instance().ItemStateList.ContainsKey(Id))
            {
                itemnodestate = GameLibrary.Instance().ItemStateList[Id];
                return ItemNode.describe.Replace("[N]", itemnodestate.sprice.ToString());
            }
        }
        else if (ItemNode.types == (int)ItemType.RecoverProp)
        {
            if (GameLibrary.Instance().ItemStateList.ContainsKey(Id))
            {
                itemnodestate = GameLibrary.Instance().ItemStateList[Id];
                return ItemNode.describe.Replace("[N]", itemnodestate.power_add.ToString());
            }
        }
        return ItemNode.describe;
    }
}
