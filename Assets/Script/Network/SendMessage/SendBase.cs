using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Tianyu;

public class CSendBase
{
    public CSendBase(ClientSendDataMgr mgr)
    {
        m_Parent = mgr;
    }

    protected void NormalSend(UInt32 msgId, C2SMessageType c2sType = C2SMessageType.PASVWait)
    {
        PackNormalKvpAndSend(msgId, new Dictionary<string, object>(), c2sType);
    }

    protected void PackNormalKvpAndSend(UInt32 msgId, Dictionary<string, object> dict, C2SMessageType c2sType = C2SMessageType.PASVWait)
    {
        dict.Add("msgid", msgId);
        dict.Add("playerId", playerData.GetInstance().selfData.playerId);
        dict.Add("account", serverMgr.GetInstance().GetMobile());
        PacketDictAndSend(msgId, dict, c2sType);
    }

    protected void PacketDictAndSend(UInt32 msgID, Dictionary<string, object> newpacket, C2SMessageType c2sType = C2SMessageType.PASVWait)
    {
        CWritePacket packet = new CWritePacket(msgID);
        System.Text.StringBuilder stringbuilder = Jsontext.WriteData(newpacket);
        stringbuilder.Append("\0");

        string json_s = stringbuilder.ToString();
        if (DataDefine.isEFS == 1)
        {
            //加密处理
            json_s = packet.Compress(json_s, DataDefine.datakey);
        }
        packet.WriteString(json_s);

        if (DataDefine.isLogMsgDetail)
            Debug.Log("Send msgDetail: " + json_s);

        SendPacket(packet, c2sType);
    }
    //public uint lastsendTime = 0;

    bool isFilteredMsg(uint msgid)
    {
        //return true;
        return msgid != MessageID.player_walk && msgid != MessageID.s_player_attack_req
                    && msgid != MessageID.pve_dungeon_list_req && msgid != MessageID.s_player_revive_req
            && msgid != MessageID.s_player_change_scene && msgid != MessageID.s_player_change_scene;
    }

    private uint lastmessageid = 0;
    public void SendPacket(CWritePacket packet, C2SMessageType type = C2SMessageType.PASVWait)
    {


        //if (packet.GetPacketID() != MessageID.player_walk && packet.GetPacketID() != MessageID.s_player_attack_req
        //          && packet.GetPacketID() != MessageID.pve_dungeon_list_req && packet.GetPacketID() != MessageID.s_player_revive_req
        //          && packet.GetPacketID() != MessageID.s_player_change_scene)
        //{
        //    Globe.lastNetTime = 0;
        //    Debug.Log(Globe.lastNetTime);
        //    lastmessageid = packet.GetPacketID();
        //}


        if (Singleton<SceneManage>.Instance.Current == EnumSceneID.UI_MajorCity01 || Globe.isFightGuide)
        {
            if (SceneManager.GetActiveScene().name == GameLibrary.UI_Major || Globe.isFightGuide)
            {
                switch (type)
                {
                    case C2SMessageType.Active:
                        break;
                    case C2SMessageType.ActiveWait:
                        GameLibrary.isSendPackage = true;
                        if (Control.GetUIObject(UIPanleID.UIWaitForSever) != null)
                            Control.ShowGUI(UIPanleID.UIWaitForSever, EnumOpenUIType.DefaultUIOrSecond);
                        break;
                    case C2SMessageType.PASVWait:
                        // Debug.Log("True服务器正在接收其他消息     " + GameLibrary.isSendPackage);
                        if (GameLibrary.isSendPackage)
                        {
                            return;
                        }
                        GameLibrary.isSendPackage = true;
                        if (Control.GetUIObject(UIPanleID.UIWaitForSever) != null)
                            Control.ShowGUI(UIPanleID.UIWaitForSever, EnumOpenUIType.DefaultUIOrSecond);
                        break;
                }
            }
        }

        // Debug.Log("True代表服务器正在接收本次消息     " + GameLibrary.isSendPackage);
        if (ClientNetMgr.GetSingle().IsConnect())
        {
            if (m_Parent != null)
            {
                m_Parent.SendPacket(packet);
                if (DataDefine.isLogMsg && DataDefine.filterWalkMsg(packet.GetPacketID()))
                {
                    Debug.Log("Send msgid:" + packet.GetPacketID() + packet.GetLogPacketID() + " Sender: " + GetType() + " Time:" + Time.realtimeSinceStartup);
                }
            }
        }
        else
        {
            Debug.Log("和服务器已经断开了连接悄悄申请重连");
            //if (UIPromptBox.Instance != null)
            //{
            //    UIPromptBox.Instance.ShowLabel("与服务器链接丢失，将重新与服务器建立链接");
            //}
            Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, "与服务器链接丢失，将重新与服务器建立链接");

        }
    }

    private ClientSendDataMgr m_Parent;
}

