using System.Collections.Generic;

public class CChatSend : CSendBase {
    public CChatSend(ClientSendDataMgr mgr) : base(mgr)
    {
    }

    public void SendChatInfo(string content,ChatType chatType,C2SMessageType c2sType)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", (int)chatType);
        newpacket.Add("arg2", content);
        switch(chatType)
        {
            case ChatType.WorldChat:
                newpacket.Add("arg3", 0);
                break;
            case ChatType.SystemChat:
                newpacket.Add("arg3", 0);
                break;
            case ChatType.SocietyChat:
                newpacket.Add("arg3", 0);
                break;
            case ChatType.PrivateChat:
                newpacket.Add("arg3", Globe.privateChatPlayerAId);
                break;
            case ChatType.NearbyChat:
                newpacket.Add("arg3", 0);
                break;
            case ChatType.TroopsChat:
                newpacket.Add("arg3", 0);
                break;
        }

        PackNormalKvpAndSend(MessageID.common_player_chat_msg_req, newpacket, C2SMessageType.Active);
    }
}
