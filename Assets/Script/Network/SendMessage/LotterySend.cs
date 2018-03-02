/*
文件名（File Name）:   LotterySend.cs

作者（Author）:    #高#

创建时间（CreateTime）:  2016-6-20 18:13:32
*/
using System.Collections.Generic;

public class CLotterySend : CSendBase
{
    public CLotterySend(ClientSendDataMgr mgr) : base(mgr)
    {
    }

    public void LotteryRequest(LotteryType type, int count, CostType costType)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", (int)costType);//0：免费1正常消费
        newpacket.Add("arg2", count);//购买次数
        newpacket.Add("arg3", (int)type);//抽奖类型 1金币抽；2钻石抽；3魂匣抽
        PackNormalKvpAndSend(MessageID.common_lucky_gamble_req, newpacket);
    }
}
