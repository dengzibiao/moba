using System;
using System.Collections.Generic;
using UnityEngine;
using Tianyu;

public class CLoginSend : CSendBase
{
	public CLoginSend ( ClientSendDataMgr mgr) : base(mgr)
	{
	}

    public void SendCreateRole(String strName,string heroId,string areaId )
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("msgid",  ((int)MessageID.login_create_player_req));
        newpacket.Add("tt", 3);//转发类型
        newpacket.Add("mid", 1010);//服务id
        newpacket.Add("name", strName);
        newpacket.Add("account",serverMgr.GetInstance().GetMobile());
        newpacket.Add("heroId", int.Parse(heroId));//角色id
        newpacket.Add( "areaId" , int.Parse(areaId) );//角色id
        newpacket.Add("cv", DataDefine.ClientVersion);//客户端版本号
		newpacket.Add("udid",SystemInfo.deviceUniqueIdentifier);
        PacketDictAndSend(MessageID.login_create_player_req, newpacket, C2SMessageType.ActiveWait);
    }

	public void SendPlayerLogin (uint playerId ,long heroId, int areaId ,int bOnline=1)
    {
		
		
        Dictionary<string , object> newpacket = new Dictionary<string , object>();
        newpacket.Add( "msgid" , ( ( int ) MessageID.login_player_login_req) );
        newpacket.Add( "mid" , 1010 );//服务id
        newpacket.Add( "account" , serverMgr.GetInstance().GetMobile() );
        newpacket.Add( "playerId" , playerId );//角色id
        newpacket.Add( "heroId" , heroId );//英雄id
        newpacket.Add( "areaId" , areaId );//区号
        newpacket.Add( "bOnline" , bOnline );//是否断线
        newpacket.Add( "cv" , DataDefine.ClientVersion );//客户端版本号
		//newpacket.Add("udid",SystemInfo.deviceUniqueIdentifier);
        PacketDictAndSend(MessageID.login_player_login_req, newpacket);

    }

    public void SendCheckAccount ()
    {
        Dictionary<string , object> newpacket = new Dictionary<string , object>();
        newpacket.Add( "msgid" , ( ( int ) MessageID.login_check_account_req) );
        newpacket.Add("mid", 1010);//服务id
        newpacket.Add( "account" , serverMgr.GetInstance().GetMobile() );
        PacketDictAndSend(MessageID.login_check_account_req, newpacket);
    }
    /// <summary>
    /// 向服务器申请跳转场景
    /// </summary>
    /// <param name="fromScenceid">当前场景</param>
    /// <param name="toScenceid">将要跳转的场景</param>
    public void SendChengeScene(int fromScenceid,int toScenceid, short flag =1)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("msgid", ((int)MessageID.s_player_change_scene));
        newpacket.Add("mid", 1040);//服务id
        newpacket.Add("account", serverMgr.GetInstance().GetMobile());
        newpacket.Add("fr", fromScenceid);
        newpacket.Add("to", toScenceid);
        newpacket.Add("fl", flag);
        PacketDictAndSend(MessageID.s_player_change_scene, newpacket, C2SMessageType.Active);
        Debug.Log("SendChengeScene " + toScenceid);
    }
 
    ///// <summary>
    ///// 向服务器申请离开场景
    ///// </summary>
    //public void SendLeaveScene()
    //{
    //    Dictionary<string, object> newpacket = new Dictionary<string, object>();
    //    newpacket.Add("msgid", ((int)MessageID.s_player_leave_scene));
    //    newpacket.Add("mid", 1040);//服务id
    //    newpacket.Add("account", serverMgr.GetInstance().GetMobile());
    //    PacketDictAndSend(MessageID.s_player_leave_scene, newpacket);
    //}
    ///// <summary>
    ///// 向服务器申请进入场景
    ///// </summary>
    //public void SendEnterScene()
    //{
    //    Dictionary<string, object> newpacket = new Dictionary<string, object>();
    //    newpacket.Add("msgid", ((int)MessageID.s_player_enter_scene));
    //    newpacket.Add("mid", 1040);//服务id
    //    newpacket.Add("account", serverMgr.GetInstance().GetMobile());
    //    PacketDictAndSend(MessageID.s_player_enter_scene, newpacket);
    //}
}

