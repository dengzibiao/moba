using System.Collections.Generic;

public class CActionPointSend : CSendBase
{
    public CActionPointSend(ClientSendDataMgr mgr) : base(mgr)
    {
    }
 
    public void SendBuyActionPoint(int type,int count,C2SMessageType c2sType)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", type);
        newpacket.Add("arg2", count);
        PackNormalKvpAndSend(MessageID.common_buy_action_point_req, newpacket, c2sType);
    }
}
