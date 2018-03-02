using System.Collections.Generic;

public class CSocietySend : CSendBase
{
    public CSocietySend(ClientSendDataMgr mgr) : base(mgr)
    {
    }

    public void SendGetSocietyList(C2SMessageType c2sType,int count)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", count);//玩家账户
        PackNormalKvpAndSend(MessageID.union_query_uncion_list_req, newpacket, c2sType);
    }

    public void SendSearchSocietyList(C2SMessageType c2sType,long societyId)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", societyId);//玩家账户
        PackNormalKvpAndSend(MessageID.union_search_someone_req, newpacket, c2sType);
    }

    public void SendCreateSociety(C2SMessageType c2sType, string name,string content,string icon)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", name);
        newpacket.Add("arg2", content);
        newpacket.Add("arg3", icon);
        PackNormalKvpAndSend(MessageID.union_create_someone_req, newpacket, c2sType);
    }

    public void SendDissolveSociety(C2SMessageType c2sType)
    {
        NormalSend(MessageID.union_disband_someone_req, c2sType);
    }

    public void SendApplicationJoinSociety(C2SMessageType c2sType,long societyId)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", societyId);//玩家账户
        PackNormalKvpAndSend(MessageID.union_application_join_req, newpacket, c2sType);
    }

    public void SendGetApplicationJoinSocietyList(C2SMessageType c2sType)
    {
        NormalSend(MessageID.union_query_application_list_req, c2sType);
    }

    public void SendApproveJoinSociety(C2SMessageType c2sType,long Id,int type)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", Id);//玩家账户
        newpacket.Add("arg2", type);//玩家账户
        PackNormalKvpAndSend(MessageID.union_approve_application_req, newpacket, c2sType);
    }

    public void SendExitSociety(C2SMessageType c2sType)
    {
        NormalSend(MessageID.union_exits_someone_req, c2sType);
    }

    public void SendKickoutSocietyMember(C2SMessageType c2sType,long id)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", id);
        PackNormalKvpAndSend(MessageID.union_kickout_someone_req, newpacket, c2sType);
    }

    public void SendPresidentChange(C2SMessageType c2sType,long id)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", id);
        PackNormalKvpAndSend(MessageID.union_change_someone_position_req, newpacket, c2sType);
    }

    public void SendGetSocietyMemberList(C2SMessageType c2sType,long societyId)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", societyId);
        PackNormalKvpAndSend(MessageID.union_query_all_member_req, newpacket, c2sType);
    }

    public void SendChangeSocietyInfo(C2SMessageType c2sType,long societyId,string str1)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", societyId);
        newpacket.Add("arg2", str1);
        PackNormalKvpAndSend(MessageID.union_change_some_info_req, newpacket, c2sType);
    }

    public void SendGetSocietyInfo(C2SMessageType c2sType,long id)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", id);//工会ID，此数值为空则显示查询人所在工会信息
        PackNormalKvpAndSend(MessageID.union_query_detailed_info_req, newpacket, c2sType);
    }

}
