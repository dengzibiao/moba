using System.Collections.Generic;

public class CGetEnergySend : CSendBase {
    public CGetEnergySend(ClientSendDataMgr mgr)
        : base(mgr)
    {
    }

    public void SendGetEnergy()
    {
        NormalSend(MessageID.common_timeing_dining_req);
    }

    public void SendGetOnlineReward(C2SMessageType type)
    {
        NormalSend(MessageID.common_draw_online_reward_req);
    }

    public void SendGetLevelReward(C2SMessageType type,int id)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", id);
        PackNormalKvpAndSend(MessageID.common_player_level_reward_req, newpacket, type);
    }

}
