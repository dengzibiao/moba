using UnityEngine;
using System.Collections.Generic;
using Tianyu;
using System;

public class CMobaSend : CSendBase
{
    public CMobaSend ( ClientSendDataMgr mgr) : base(mgr)
    {

    }

    // {msgid=4651,mid=服务ID(common服务是1040),playerId=角色ID,account=账户,types=类型,hero=出战英雄列表}
    // types 1 1v1 3 3v3 5 5v5
    public void SendMobaMatch(int types, object heros)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", types);
        newpacket.Add("arg2", heros);
        PackNormalKvpAndSend(MessageID.pve_search_moba_list_req, newpacket);
    }

    /// <summary>
    /// {msgid=4657,mid=服务ID(common服务是1040),playerId=角色ID,account=账户,types=类型}
    /// <param name="types">1成功 2失败</param>
    /// </summary>
    public void SendMobaResult(int types)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", types);
        PackNormalKvpAndSend(MessageID.pve_moba_settlement_req, newpacket);
    }

    // {msgid=4658,mid=服务ID(common服务是1040),playerId=角色ID,account=账户,dn=[1,2]}
    // dn 该次抽取列表
    public void SendFlopResult(int[] dn)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", dn);
        PackNormalKvpAndSend(MessageID.pve_draw_moba_reward_req, newpacket);
    }
}
