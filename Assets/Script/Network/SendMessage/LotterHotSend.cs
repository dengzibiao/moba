/*
文件名（File Name）:   LotterHotSend.cs

作者（Author）:    #高#

创建时间（CreateTime）:  2016-7-5 14:26:1
*/

public class CLotterHotSend : CSendBase
{

    public CLotterHotSend(ClientSendDataMgr mgr) : base(mgr)
    {
    }

    public void LotteryHotRequest()
    {
        NormalSend(MessageID.common_lucky_gamble_list_req);
    }
}
