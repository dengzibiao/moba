/*
文件名（File Name）:   Notification.cs

作者（Author）:    #高#

创建时间（CreateTime）:  #CreateTime#
*/
using System;
using System.Collections.Generic;
using System.Collections;


public class Notification
{
    private Dictionary<UInt32, List<UIPanleID>> dicData = null;
    private List<UInt32> messageList = null;
    public Notification()
    {
        dicData = new Dictionary<UInt32, List<UIPanleID>>();
        messageList = new List<uint>();
    }

    public List<UInt32> MessageListCount()
    {
        return messageList;
    }
    public void Send(UInt32 messageID, Dictionary<string, object> newpacket, C2SMessageType type = C2SMessageType.PASVWait)
    {
        ClientSendDataMgr.GetSingle().GetMessageSend().SendAllMessage(messageID, newpacket, type);
    }

    public void Send(UInt32 messageID, C2SMessageType type = C2SMessageType.PASVWait)
    {
        ClientSendDataMgr.GetSingle().GetMessageSend().SendAllMessage(messageID, type);
    }
    /// <summary>
    /// 注册消息
    /// </summary>
    /// <param name="messageID"></param>
    /// <param name="uiKey"></param>
    public void RegistMessageID(UInt32 messageID, UIPanleID uiKey)
    {
        if (!dicData.ContainsKey(messageID))
        {
            List<UIPanleID> ls = new List<UIPanleID>();
            ls.Add(uiKey);
            dicData.Add(messageID, ls);
        }
        else
        {
            List<UIPanleID> ls = null;
            if (dicData.TryGetValue(messageID, out ls))
            {
                if (!ls.Contains(uiKey))
                {
                    ls.Add(uiKey);
                }
                   
            }


        }

    }

    /// <summary>
    /// /服务器返回消息加入消息队列
    /// </summary>
    /// <param name="messageID"></param>
    public void ReceiveMessageList(UInt32 messageID)
    {
        messageList.Add(messageID);
    }

    public void ReceiveHandle(UInt32 messageID)
    {
        List<UIPanleID> ls = null;
        if (dicData.TryGetValue(messageID, out ls))
        {
            for (int i = 0; i < ls.Count; i++)
            {
                if (Singleton<GUIManager>.Instance.GetUI<GUIBase>(ls[i]) != null)
                    Singleton<GUIManager>.Instance.GetUI<GUIBase>(ls[i]).ReceiveData(messageID);
            }
       
        }

    }

    public void Clear()
    {
        dicData.Clear();
    }

}
