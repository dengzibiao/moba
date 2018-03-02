using System.Collections.Generic;

public class CRankListSend : CSendBase
{
    public CRankListSend(ClientSendDataMgr mgr)
        : base(mgr)
    {
    }

    public void SendRankList(int typeId,int min,int max)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("typeId", typeId);//typeId);//昵称
        newpacket.Add("minV", min);
        newpacket.Add("maxV", max);
        PackNormalKvpAndSend(MessageID.common_ranklist_req, newpacket);
    }
}
