using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tianyu;
using System;
public class RoleInfo
{
    // 参数是mapID地图 key唯一标示 accid账户ID playid玩家ID roleid角色ID posX坐标 posZ坐标　name名字
    public UInt32 mapID;
    public UInt32 keyID;//键值
    public UInt32 accID;//账号
    public UInt32 playID;//玩家id
    public UInt32 roleID;//英雄id
    public int rc;//死亡次数
    public float posX;//位置
    public float posY;
    public float posZ;
    public float orientX;
    public float orientY;
    public float orientZ;
    public string name;//名字
    public short hp;//血量
    public UInt32 typeid;//类型id
    public UInt32 type;//元素类型
    public int title; //称号
    public UInt32 unionId;//公会id
    public string unionName;//公会名字
    public long petid;//跟随宠物id
    public long mount;//骑乘的坐骑id

    public GameObject RoleObj;
}

public class WalkSend : CSendBase//WalkSendBase
{
    public WalkSend( ClientSendDataMgr mgr) //WalkSendMgr mgr)
        : base(mgr)
    {
    }
    public bool ping = false;
    bool isSend (out MapInfoNode MIN )
    {
        if ( FSDataNodeTable<MapInfoNode>.GetSingleton().DataNodeList.ContainsKey( playerData.GetInstance().selfData.mapID ) )
        {
            MapInfoNode tempMN = FSDataNodeTable<MapInfoNode>.GetSingleton().DataNodeList [ playerData.GetInstance().selfData.mapID ];
            if ( tempMN != null )
            {
                if ( Application.loadedLevelName == tempMN.MapName )
                {
                    MIN = tempMN;
                    return true;
                }
            }
        }
        MIN = null;
        return false;
    }

    void Send ( Dictionary<string , object> newpacket, CWritePacket packet )
    {
        System.Text.StringBuilder stringbuilder = Jsontext.WriteData( newpacket );
        stringbuilder.Append( "\0" );

        string json_s = stringbuilder.ToString();
        if ( DataDefine.isEFS == 1 )
        {
            //加密处理
            json_s = packet.Compress( json_s , DataDefine.datakey );
        }
        packet.WriteString( json_s );
        // Debug.Log( json_s );

       SendPacket( packet ,C2SMessageType.Active);
    }

    //发送自己的初始信息
    public void SendInitializePosInfo(RoleInfo ri)
    {
        if(GameLibrary.serverInit)
        {
         //   CallLoad();
            MapInfoNode tempMN;
            if (isSend(out tempMN))
            {
                CWritePacket packet = new CWritePacket(MessageID.player_load_scene_finished);
                Dictionary<string, object> newpacket = new Dictionary<string, object>();
                newpacket.Add("msgid", (ushort)MessageID.player_load_scene_finished);
                newpacket.Add("ai", ri.accID);
                Send(newpacket, packet);
            }
        }
       
    }

    float lastSendTime;
    //发送自己的位置改变信息
    public void SendSelfPos()
    {
       if(GameLibrary.isSkipingScene || Time.realtimeSinceStartup - lastSendTime < 0.2f)
            return;
        MapInfoNode tempMN;
        if ( isSend( out tempMN ) )
        {
            if ( tempMN != null && CharacterManager.player != null)
            {
                Vector3 pos = CharacterManager.player.transform.localPosition;
                Vector3  rotation = CharacterManager.player.transform.localEulerAngles;
                CWritePacket packet = new CWritePacket( MessageID.player_walk );
                Dictionary<string , object> newpacket = new Dictionary<string , object>();

                newpacket.Add( "msgid" , ( ushort ) MessageID.player_walk);
                newpacket.Add( "ky" , playerData.GetInstance().selfData.keyId );
                newpacket.Add( "ai" , playerData.GetInstance().selfData.accountId );
                newpacket.Add( "px" , pos.x - tempMN.Xmin );
                newpacket.Add( "py" , pos.y );
                newpacket.Add( "pz" , pos.z - tempMN.Zmin );
                newpacket.Add("ox", rotation.x);
                newpacket.Add("oy", rotation.y);
                newpacket.Add("oz", rotation.z);

                Send( newpacket , packet );
                // Debug.LogError("Send pos " + pos + " at " + Time.realtimeSinceStartup);
                lastSendTime = Time.realtimeSinceStartup;
            }
        }
    }

    //玩家退出游戏协议
    public void SendQuit()
    {
       //if( ClientNetMgr.GetSingle().IsConnect())
       // {
       //     ClientNetMgr.GetSingle().Close();
       // }

        //CWritePacket packet = new CWritePacket(MessageID.unregiste_player_walk_info);
        //Dictionary<string, object> newpacket = new Dictionary<string, object>();
        //newpacket.Add("msgid", (ushort)MessageID.unregiste_player_walk_info);
        //newpacket.Add("mapid", selfRI.mapID);
        //newpacket.Add("key", selfRI.accID);

        //System.Text.StringBuilder stringbuilder = Jsontext.WriteData(newpacket);
        //stringbuilder.Append("\0");

        //string json_s = stringbuilder.ToString();
        //if (DataDefine.isEFS == 1)
        //{
        //    //加密处理
        //    json_s = packet.Compress(json_s, DataDefine.datakey);
        //}
        //packet.WriteString(json_s);
        //Debug.Log(json_s);

        //SendPacket(packet);
        //playerData.GetInstance().selfData.oldMapID = -1;
        //CreatePeople.GetInstance().Clear();
        ////for (int i=0 ;i< WalkHandleMgr.GetSingle().m_WalkHandle.OtherPlayerInfoList.Count ;i++ )
        ////{
        ////    if ( WalkHandleMgr.GetSingle().m_WalkHandle.OtherPlayerInfoList[i].RoleObj != null )
        ////    {
        ////        WalkHandleMgr.GetSingle().m_WalkHandle.OtherPlayerInfoList [ i ].RoleObj.GetComponent<OtherPlayer>().DeleteOtherPlayer();
        ////    }
        ////}
        ////WalkHandleMgr.GetSingle().m_WalkHandle.OtherPlayerInfoList.Clear();
    }

    public void Ping ()
    {
        CWritePacket packet = new CWritePacket( MessageID.player_ping_req );
        Dictionary<string , object> newpacket = new Dictionary<string , object>();

        newpacket.Add( "msgid" , ( ushort ) MessageID.player_ping_req );
        newpacket.Add( "key" , playerData.GetInstance().selfData.keyId );
        System.DateTime tempDT = new System.DateTime( 1970 , 1 , 1 , 0 , 0 , 0 );
        System.Int64 locaTime = System.Convert.ToInt64( ( System.DateTime.Now.ToUniversalTime() - tempDT ).TotalMilliseconds );
        newpacket.Add( "ch" , ( System.UInt32 ) ( locaTime >> 32 ) );
        newpacket.Add( "cl" , ( System.UInt32 ) ( locaTime & 0xFFFFFFFF ) );

        Send( newpacket , packet );
    }

    public void SendAttack ( uint targetKeyId , int skillIndex )
    {
        MapInfoNode tempMN;
        if ( isSend( out tempMN ) )
        {
            CWritePacket packet = new CWritePacket( MessageID.s_player_attack_req );
            Dictionary<string , object> newpacket = new Dictionary<string , object>();
            newpacket.Add( "msgid" , ( ushort ) MessageID.s_player_attack_req );
            newpacket.Add( "tk" , System.UInt32.Parse( targetKeyId.ToString() ) );
            newpacket.Add( "sd" , System.Int32.Parse( skillIndex.ToString() ) );
            GameObject playerobj = CharacterManager.player;
            if (playerobj != null)
            {
                newpacket.Add("sx", playerobj.transform.position.x);//自己的位置
                newpacket.Add("sy", playerobj.transform.position.y);
                newpacket.Add("sz", playerobj.transform.position.z);
                newpacket.Add("dx", playerobj.transform.rotation.x);//自己的朝向 
                newpacket.Add("dy", playerobj.transform.rotation.y);
                newpacket.Add("dz", playerobj.transform.rotation.z);
            }
            else
            {
                newpacket.Add("sx", 0);//自己的位置
                newpacket.Add("sy", 0);
                newpacket.Add("sz", 0);
                newpacket.Add("dx", 0);//自己的朝向 
                newpacket.Add("dy", 0);
                newpacket.Add("dz", 0);
            }
            GameObject obj = CreatePeople.GetInstance().GetTargert(targetKeyId);
            if (targetKeyId!=0)
            {
                if (obj != null)
                {
                    newpacket.Add("tx", obj.transform.position.x);//目标位置
                    newpacket.Add("ty", obj.transform.position.y);
                    newpacket.Add("tz", obj.transform.position.z);
                }
            }
            else
            {
                if (playerobj != null)
                {
                    newpacket.Add("tx", playerobj.transform.position.x);//目标位置
                    newpacket.Add("ty", playerobj.transform.position.y);
                    newpacket.Add("tz", playerobj.transform.position.z);
                }
            }

            Send( newpacket , packet );
        }
    }
    public void SendSetPlayerHp ( int hp )
    {
        MapInfoNode tempMN;
        if ( isSend( out tempMN ) )
        {
            CWritePacket packet = new CWritePacket( MessageID.s_set_player_hp );
            Dictionary<string , object> newpacket = new Dictionary<string , object>();
            newpacket.Add( "msgid" , ( ushort ) MessageID.s_set_player_hp );
            newpacket.Add( "hp" , hp );           

            Send( newpacket , packet );
        }
    }


    public void SendOrientation ( Vector3 rot )
    {
        MapInfoNode tempMN;
        if ( isSend( out tempMN ) )
        {
            CWritePacket packet = new CWritePacket( MessageID.s_update_player_orientation );
            Dictionary<string , object> newpacket = new Dictionary<string , object>();
            newpacket.Add( "msgid" , ( ushort ) MessageID.s_update_player_orientation );
            newpacket.Add( "ai" , playerData.GetInstance().selfData.accountId );
            newpacket.Add( "ky" , playerData.GetInstance().selfData.keyId );
            newpacket.Add( "ox" , rot.x );
            newpacket.Add( "oy" , rot.y );
            newpacket.Add( "oz" , rot.z );

            Send( newpacket , packet );
        }
    }

    public void SendHp ( long attackKeyId , long targetKeyId , int damageHp , long skillID = 0 , float baseVal = 0.0f )
    {
        MapInfoNode tempMN;
        if ( isSend( out tempMN ) )
        {
            CWritePacket packet = new CWritePacket( MessageID.s_lose_blood_req);
            Dictionary<string , object> newpacket = new Dictionary<string , object>();
            newpacket.Add( "msgid" , ( ushort ) MessageID.s_lose_blood_req);
            newpacket.Add( "sk" , attackKeyId );
            newpacket.Add( "tk" , targetKeyId );
            newpacket.Add( "hp" , damageHp );
            newpacket.Add( "sd" , skillID );
            newpacket.Add( "bv" , baseVal );
        
            Send( newpacket , packet );
        } 
    }
    public void SendPlayerRevive(long attackKeyId ,long targetKeyId,int reviewtype,int hp)
    {
        CWritePacket packet = new CWritePacket( MessageID.s_player_revive_req );
        Dictionary<string , object> newpacket = new Dictionary<string , object>();
        newpacket.Add( "msgid" , ( ushort ) MessageID.s_player_revive_req );
        newpacket.Add( "sk" , attackKeyId );
        newpacket.Add( "tk" , targetKeyId );
        newpacket.Add( "rt" , reviewtype );//复活类型1免费安全区域复活，2花钱的安全区域复活，3花钻石的原地复活
        newpacket.Add( "hp" , hp );

        Send( newpacket , packet );

    }

    public void SendDead ( long attackKeyId, long targetKeyId ,int rc)
    {
        //MapInfoNode tempMN;
        //if ( isSend( out tempMN ) )
        //{
        //    CWritePacket packet = new CWritePacket( MessageID.s_someone_dead_req);
        //    Dictionary<string , object> newpacket = new Dictionary<string , object>();

        //    GameObject tempobj = null;
        //    CreatePeople.GetInstance().GetOtherObjectByKeyId(targetKeyId, ref tempobj);
        //    if (tempobj.GetComponent<CharacterState>() != null)
        //    {
        //        newpacket.Add("rc", tempobj.GetComponent<CharacterState>().rc);  
        //    }
        //    else
        //    {
        //        newpacket.Add("rc", rc);
        //    }

        //    newpacket.Add( "msgid" , ( ushort ) MessageID.s_someone_dead_req);

        //    newpacket.Add( "sk" , attackKeyId );
        //    newpacket.Add( "tk" , targetKeyId );
        //    Debug.Log(rc + " SendDead!!!");

        //    Send( newpacket , packet );
        //}
    }
}

