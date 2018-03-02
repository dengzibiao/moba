using System.Collections.Generic;

public class CHeroSkillSend : CSendBase
{
    public CHeroSkillSend ( ClientSendDataMgr mgr ) : base( mgr )
    {
    }

    public void SendHeroMsg(long heroId, C2SMessageType type)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", heroId);//英雄id
        PackNormalKvpAndSend(MessageID.common_hero_skill_list_req, newpacket, C2SMessageType.Active);
    }

    public void SendUpgradeMsg(long heroId, Dictionary<long, int> item)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", heroId);//英雄id
        newpacket.Add("arg2", item);//英雄id
        PackNormalKvpAndSend(MessageID.common_upgrade_hero_skill_req, newpacket, C2SMessageType.Active);
    }
}
