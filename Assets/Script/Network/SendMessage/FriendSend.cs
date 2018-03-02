/*
文件名（File Name）:   FriendSend.cs

作者（Author）:    #高#

创建时间（CreateTime）:  #CreateTime#
*/
using System.Collections.Generic;

public class CFriendSend : CSendBase
{
    public CFriendSend(ClientSendDataMgr mgr) : base(mgr)
    {
    }

    public void FriendsRecommendRequest()
    {
        NormalSend(MessageID.common_recommended_friend_list_req);
    }

    public void FriendsList()
    {
        NormalSend(MessageID.common_player_friend_list_req);
    }

    public void FriendsSearch(long id)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", id);//好友id
        PackNormalKvpAndSend(MessageID.common_search_friend_req, newpacket);
    }

    public void FriendsAdd(long id,long aid)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", id);//好友id
        newpacket.Add("arg2", aid);//账号id
        PackNormalKvpAndSend(MessageID.common_add_friend_req, newpacket);
    }

    public void FriendsDelete(long id,int type)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", id);//好友id
        newpacket.Add("arg2", type);//types 1好友 2黑名单
        PackNormalKvpAndSend(MessageID.common_delete_friend_req, newpacket);
    }

    public void FriendsFunction(int type)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", type);//types 1申请列表 2黑名单 3仇人列表
        PackNormalKvpAndSend(MessageID.common_friend_function_listreq, newpacket);
    }

    public void FriendsAgreeOrRefuse(int type, long id,long aid)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", id);//好友id
        newpacket.Add("arg2", type);//types 1同意 2拒绝
        newpacket.Add("arg3", aid);//目标账号ID
        PackNormalKvpAndSend(MessageID.common_allow_add_friend_req, newpacket);
    }
}

 
