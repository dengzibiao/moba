using System.Collections.Generic;

public class CGoldHandSend : CSendBase
{
    public CGoldHandSend(ClientSendDataMgr mgr) : base(mgr)
    {
    }

    public void GetGoldHandTimes(C2SMessageType c2sType)
    {
        NormalSend(MessageID.common_lucky_draw_count_req, c2sType);
    }

    public void UseGoldHand(int type,C2SMessageType c2sType)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", type); // (0:一次 1:连续4次)
        PackNormalKvpAndSend(MessageID.common_use_lucky_draw_req, newpacket, c2sType);
    }
}
