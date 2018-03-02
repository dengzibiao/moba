using System.Collections.Generic;

public class CTitleSend : CSendBase
{
    public CTitleSend(ClientSendDataMgr mgr) : base(mgr)
    {
    }

    public void SendGetTitleList(C2SMessageType type)
    {
        NormalSend(MessageID.common_title_list_req);
    }

    public void SendChangeTitleState(C2SMessageType type,int titleid, int oprtype)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("titleId", titleid);//称号id
        newpacket.Add("oprType", oprtype);//oprType==1穿 / oprType==2卸 /默认为0
        PackNormalKvpAndSend(MessageID.common_title_wear_or_takeoff_req, newpacket, type);
    }
}
