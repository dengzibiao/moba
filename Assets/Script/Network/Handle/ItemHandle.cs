using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tianyu;

public class CItemHandle : CHandleBase
{
	public CItemHandle (CHandleMgr mgr) : base(mgr)
	{
        
    }	
	
	public override void  RegistAllHandle()
	{
        //	RegistHandle( MessageID.Item_D2C_UpdateItemInfo, UpdateItemInfo );
        RegistHandle(MessageID.common_backpack_list_ret, UpdateItemInfo);
        RegistHandle(MessageID.common_sell_item_ret, SellItemResult);
        RegistHandle(MessageID.common_update_item_list_ret, UpdateItemData);
        RegistHandle(MessageID.common_use_item_ret, UseItemResult);
    }

    public bool UseItemResult(CReadPacket packet)
    {
        Dictionary<string, object> data = packet.data;
        int resolt = int.Parse(data["ret"].ToString());
        if (resolt == 0)
        {
            if (data.ContainsKey("expsPool"))
            {
                UIExpPropPanel.instance.beforeExpCount = playerData.GetInstance().selfData.expPool;
                playerData.GetInstance().selfData.expPool = long.Parse(data["expsPool"].ToString()); ;//经验池经验值
                if (playerData.GetInstance().selfData.expPool < 0) playerData.GetInstance().selfData.expPool = 0;
                //英雄培养使用经验药水
                //if (Control.GetGUI(GameLibrary.UIExpPropPanel).gameObject.activeSelf)
                //{
                //    UIExpPropPanel.instance.RefreshData();
                //}
            }
        }
        else
        {
            Debug.Log(string.Format("使用物品失败：{0}", data["desc"].ToString()));
            //UIPromptBox.Instance.ShowLabel(data["desc"].ToString());
            Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, data["desc"].ToString());
        }
        return true;
    }

    public bool UpdateItemInfo( CReadPacket packet )
	{
        Debug.Log("获得背包列表结果");
        Dictionary<string,object> data = packet.data;
        object[] itemList = data["item"] as object[];
        int resolt = int.Parse(data["ret"].ToString());
        if (resolt == 0)
        {
            //先清空一下背包列表
            if (itemList!=null )
            {
                playerData.GetInstance().baginfo.itemlist.Clear();
                for (int i = 0; i < itemList.Length; i++)
                {

                    Dictionary<string, object> itemDataDic = itemList[i] as Dictionary<string, object>;
                    if (itemDataDic != null)
                    {
                        ItemData itemdata = new ItemData();

                        itemdata.Id = int.Parse(itemDataDic["id"].ToString());

                        itemdata.Uuid = itemDataDic["uid"].ToString();
                        itemdata.Count = short.Parse(itemDataDic["at"].ToString());
                        if (itemdata.Count <= 0)
                        {
                            continue;
                        }
                        ItemNodeState itemnodestate = null;
                        if(GameLibrary.Instance().ItemStateList.ContainsKey(itemdata.Id))
                        {
                            itemnodestate = GameLibrary.Instance().ItemStateList[itemdata.Id];

                            itemdata.Name = itemnodestate.name;
                            itemdata.Types = itemnodestate.types;
                            itemdata.Describe = itemnodestate.describe;

                            switch (itemnodestate.grade)
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
                            itemdata.Sprice = itemnodestate.sprice;
                            itemdata.Piles = itemnodestate.piles;
                            itemdata.IconName = itemnodestate.icon_name;
                            itemdata.GoodsType = MailGoodsType.ItemType;
                            if (itemdata.Types == 6)
                            {
                                itemdata.UiAtlas = ResourceManager.Instance().GetUIAtlas("UIHeroHead");
                            }
                            else
                            {
                                itemdata.UiAtlas = ResourceManager.Instance().GetUIAtlas("Prop");
                            }
                            playerData.GetInstance().baginfo.itemlist.Add(itemdata);

                        }                                             
                    }
                }
                GoodsDataOperation.GetInstance().ItemListTypeChangeToDic();
            }
        }
        else
        {
            Debug.Log(string.Format("获取背包信息失败：{0}", data["desc"].ToString()));
        }
        return true;
    }

    public bool SellItemResult(CReadPacket packet)
    {
        Debug.Log("SellItemResult");
        Dictionary<string, object> data = packet.data;
        int resolt = int.Parse(data["ret"].ToString());
        if (resolt == 0)
        {
            playerData.GetInstance().baginfo.gold = UInt32.Parse(data["gold"].ToString());
            playerData.GetInstance().RoleMoneyHadler(MoneyType.Gold, playerData.GetInstance().baginfo.gold);
            //if (Globe.isSaleSingleGood)
            //{
            //    GoodsDataOperation.GetInstance().RefreshGood();
            //    UISalePanel.instance.RefreshGood();
            //}
            //else
            //{
            //    GoodsDataOperation.GetInstance().RefreshGoldProp();
            //}
        }
        else
        {
            Debug.Log(string.Format("出售物品失败：{0}", data["desc"].ToString()));
            //Control.ShowGUI(GameLibrary.UIPromptBox);
            //UIPromptBox.Instance.ShowLabel(data["desc"].ToString());
            Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, data["desc"].ToString());
        }
        return true;
    }
    /// <summary>
    /// 更新物品信息
    /// </summary>
    /// <param name="packet"></param>
    /// <returns></returns>
    public bool UpdateItemData(CReadPacket packet)
    {
        Debug.Log("UpdateItemDta");

        Dictionary<string, object> data = packet.data;
        object[] itemList = data["item"] as object[];
        int resolt = int.Parse(data["ret"].ToString());
        if (resolt == 0)
        {
            if (itemList != null)
            {
                for (int i = 0; i < itemList.Length; i++)
                {

                    Dictionary<string, object> itemDataDic = itemList[i] as Dictionary<string, object>;
                    if (itemDataDic != null)
                    {
                        long itemID = int.Parse(itemDataDic["id"].ToString());
                        int count = short.Parse(itemDataDic["at"].ToString());
                        string uuId = "";
                        if (data.ContainsKey("uid"))
                        {
                            uuId = itemDataDic["uid"].ToString();
                        }
                        GoodsDataOperation.GetInstance().UpdateGoods(itemID,count,uuId);
                        GoodsDataOperation.GetInstance().ItemListTypeChangeToDic();
                    }
                }
              //  Singleton<SceneManage>.Instance.MessageHandle(EnumSceneID.UI_MajorCity01, GameLibrary.UITaskTracker);

                if (Singleton<SceneManage>.Instance.Current == EnumSceneID.UI_MajorCity01)
                {
                    //物品数量改变 更新一下任务追踪信息
                    //if (Control.GetGUI(GameLibrary.UITaskTracker).gameObject.activeSelf)
                    //{
                    //    Control.ShowGUI(GameLibrary.UITaskTracker);
                    //}
                    //刷新背包界面
                    // if (Control.GetGUI(GameLibrary.UIKnapsack).gameObject.activeSelf)
                    //{
                    //    Debug.Log("增加物品成功并刷新");
                    //    UIKnapsack.Instance.RefreshView();
                    //}
                    //刷新聊天面板喇叭数
                    //if (Control.GetGUI(GameLibrary.UIChatPanel).gameObject.activeSelf)
                    //{
                    //    UIChatPanel.Instance.RefreshHorbCount();
                    //}
                    //英雄培养使用经验药水
                    //if (Control.GetGUI(GameLibrary.UIExpPropPanel).gameObject.activeSelf)
                    //{
                    //    UIExpPropPanel.instance.RefreshData();
                    //}
                    //EquipEvolvePanel.instance.RefreshItemData();
                    //if (EquipCompoundPanel.instance.IsShow())
                    //{
                    //    EquipCompoundPanel.instance.RefreshItemData();
                    //}
                }
                //if (Singleton<SceneManage>.Instance.Current == EnumSceneID.LGhuangyuan)
                //{
                //    //物品数量改变 更新一下任务追踪信息
                //    if (Control.GetGUI(GameLibrary.UITaskTracker).gameObject.activeSelf)
                //    {
                //        Control.ShowGUI(GameLibrary.UITaskTracker);
                //    }
                //}

            }
        }
        else
        {
            Debug.Log(string.Format("更新物品信息失败：{0}", data["desc"].ToString()));
        }
        return true;
    }


}

