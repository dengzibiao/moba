using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tianyu;
using UnityEngine.SceneManagement;

public class CChatHandle : CHandleBase
{
    public CChatHandle(CHandleMgr mgr) : base(mgr)
    {
    }
    public override void RegistAllHandle()
    {
        RegistHandle(MessageID.common_server_chat_msg_notify, HaveNewChatResult);
        RegistHandle(MessageID.common_player_chat_msg_ret, SendChatInfoResult);
    }

    private bool HaveNewChatResult(CReadPacket packet)
    {
        Debug.Log("HaveNewChatResult");
        //{msgid=4703,pid=发言者ID,pn=发言者昵称,pp=发言者头像,pv=发言者VIP等级,tp=聊天类型,ct=发言时间}
        //tp 聊天类型 1世界聊天 2公会聊天 3私聊 4附近聊天 5队伍聊天 6系统
        Dictionary<string, object> data = packet.data;

        ChatData chatData = new ChatData();
        chatData.Id = long.Parse(data["pid"].ToString());
        chatData.AccountId = long.Parse(data["aid"].ToString());
        chatData.HeadId = long.Parse(data["pp"].ToString());
        chatData.Vip = int.Parse(data["pv"].ToString());
        if (chatData.Vip >99)
        {
            chatData.Vip = 1;//vip等级超过三位设置为1
        }
        chatData.NickName = data["pn"].ToString();
        chatData.ChatContent = data["c"].ToString(); //聊天内容
        chatData.SpeakingTime = long.Parse(data["ct"].ToString());
        chatData.Time = Convert.ToDateTime(PropertyManager.ConvertIntDateTime(chatData.SpeakingTime)).ToString("HH:mm");
        if (chatData.Id == playerData.GetInstance().selfData.playerId)
        {
            chatData.IsLocalPlayer = true;
        }
        else
        {
            chatData.IsLocalPlayer = false;
        }
        chatData.ContentType = ChatContentType.TextContent;
        switch (int.Parse(data["tp"].ToString()))
        {
            case 1:
                chatData.Type = ChatType.WorldChat;
                //playerData.GetInstance().iChat.worldChatList.Add(chatData);
                break;
            case 2:
                chatData.Type = ChatType.SocietyChat;
                //playerData.GetInstance().iChat.societyChatList.Add(chatData);
                break;
            case 3:
                chatData.Type = ChatType.PrivateChat;
                //私聊在其他频道都可以看到
                //playerData.GetInstance().iChat.privateChatList.Add(chatData);

                //playerData.GetInstance().iChat.worldChatList.Add(chatData);
                //playerData.GetInstance().iChat.societyChatList.Add(chatData);
                //playerData.GetInstance().iChat.nearbyChatList.Add(chatData);
                //playerData.GetInstance().iChat.troopsChatList.Add(chatData);
                //playerData.GetInstance().iChat.systemChatList.Add(chatData);
                break;
            case 4:
                chatData.Type = ChatType.NearbyChat;
                //playerData.GetInstance().iChat.nearbyChatList.Add(chatData);
                break;
            case 5:
                chatData.Type = ChatType.TroopsChat;
                //playerData.GetInstance().iChat.troopsChatList.Add(chatData);
                break;
            case 6:
                chatData.Type = ChatType.SystemChat;
                //playerData.GetInstance().iChat.systemChatList.Add(chatData);
                break;
            default:
                break;
        }
        //暂时聊天只在主城可见 避免在战斗的时候null
        if (SceneManager.GetActiveScene().name == GameLibrary.UI_Major)
        {
            playerData.GetInstance().AddChatInfoToList(chatData);
            //UIChatPanel.Instance.AddChatInfoToList(chatData);
        }
        //界面单条聊天显示
        //playerData.GetInstance().NewChatHandler(chatData);
        return true;
    }

    private bool SendChatInfoResult(CReadPacket packet)
    {
        Debug.Log("SendChatInfoResult");
        Dictionary<string, object> data = packet.data;
        int resolt = int.Parse(data["ret"].ToString());
        if (resolt == 0)
        {
            Debug.Log("发送聊天成功");
        }
        else
        {
            Debug.Log(string.Format("发送聊天失败：{0}", data["desc"].ToString()));
            if (SocietyManager.Single().selfChatData != null&&Globe.selectChatChannel == ChatType.PrivateChat)
            {
                playerData.GetInstance().AddChatInfoToList(SocietyManager.Single().selfChatData);
                //SocietyManager.Single().selfChatData = null;
            }
            //playerData.GetInstance().AddChatInfo(ChatContentType.PrivateTargetNotOnLine,"");
            //UIChatPanel.Instance.AddChatInfo(ChatContentType.PrivateTargetNotOnLine);
            //Control.ShowGUI(GameLibrary.UIPromptBox);
            //UIPromptBox.Instance.ShowLabel(data["desc"].ToString());
        }
        return true;
    }
}
