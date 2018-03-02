using System.Collections.Generic;

public class UISign_inSend : CSendBase
{

    public UISign_inSend(ClientSendDataMgr mgr) : base(mgr)
    {

    }

    public void SendGetUISign_inList(C2SMessageType c2sType, int types)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", types);
        PackNormalKvpAndSend(MessageID.common_everyday_sign_list_req, newpacket, c2sType);
    }

    public void SendGetUISign_in(C2SMessageType c2sType)
    {
        NormalSend(MessageID.common_everyday_sign_req, c2sType);
    }

    public void SendGetUISign_inCumulative(C2SMessageType c2sType, int index)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", index);
        PackNormalKvpAndSend(MessageID.common_everyday_sign_reward_req, newpacket, c2sType);
    }
    
    public void SendGetPatchUISign_in(C2SMessageType c2sType)
    {
        NormalSend(MessageID.common_everyday_sign_again_req, c2sType);
    }
}
