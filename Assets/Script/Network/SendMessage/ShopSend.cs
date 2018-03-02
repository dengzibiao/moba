/*
文件名（File Name）:   ShopSend.cs

作者（Author）:    #高#

创建时间（CreateTime）:  2016-6-16 15:44:30
*/
using System.Collections.Generic;

public class CShopSend : CSendBase
{
    public CShopSend(ClientSendDataMgr mgr) : base(mgr)
    {
    }

    public void RequestGoodsList(int types)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", types);//商店类型
        newpacket.Add("arg2", playerData.GetInstance().selfData.level);//人物等级
        PackNormalKvpAndSend(MessageID.common_shop_goods_list_req, newpacket, C2SMessageType.PASVWait);
    }

    public void ShopBuy(long itemId, int index)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", index);//商店类型
        newpacket.Add("arg2", itemId);//物品ID
        PackNormalKvpAndSend(MessageID.common_buy_shop_goods_req, newpacket, C2SMessageType.PASVWait);
    }

    public void RefreshGoodsList( int types, int count, int money)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("v", money);//系统刷新（0）否则正常消耗的钻石
        newpacket.Add("arg1", types);//商店类型 1杂货商店；2精英商店；3神秘商店；4远征商店；5竞技场商店；6工会商店
        newpacket.Add("arg2", playerData.GetInstance().selfData.level);//人物等级 当前战队等级
        newpacket.Add("arg3", count);//刷新次数
        PackNormalKvpAndSend(MessageID.common_refresh_shop_goods_req, newpacket);
    }
}
