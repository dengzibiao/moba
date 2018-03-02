using System.Collections.Generic;

public class CPetSend : CSendBase
{
    public CPetSend(ClientSendDataMgr mgr) : base(mgr)
    {
    }

    public void SendGetHaveMountOrPetList(C2SMessageType c2sType, int type)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", type);//1宠物 2坐骑
        PackNormalKvpAndSend(MessageID.pet_query_list_req, newpacket, c2sType);
    }

    public void SendChangeMountOrPetState(C2SMessageType c2sType, int type,long id,int state)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", type);//1宠物 2坐骑
        newpacket.Add("arg2", id);
        newpacket.Add("arg3", state);//1使用 0卸载
        PackNormalKvpAndSend(MessageID.pet_change_status_req, newpacket, c2sType);
    }

    public void SendChangeMountInfo(C2SMessageType c2sType)
    {
        NormalSend(MessageID.pet_update_mounts_list_req, c2sType);
    }

    public void SendChangePetInfo(C2SMessageType c2sType)
    {
        NormalSend(MessageID.pet_update_pet_list_req, c2sType);
    }

    public void SendUseMountOrPet(C2SMessageType c2sType,int type,long id)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", type);//1宠物 2坐骑
        newpacket.Add("arg2", id);
        PackNormalKvpAndSend(MessageID.pet_set_defend_status_req, newpacket, c2sType);
    }
}
