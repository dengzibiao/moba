/*
文件名（File Name）:   SendMessage.cs

作者（Author）:    #高#

创建时间（CreateTime）:  #CreateTime#
*/

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CSendMessage : CSendBase
{
    public CSendMessage(ClientSendDataMgr mgr) : base(mgr)
    {

    }
    public void SendAllMessage(UInt32 msgId, Dictionary<string, object> dict, C2SMessageType c2sType)
    {
        PackNormalKvpAndSend(msgId, dict, c2sType);
    }
    public void SendAllMessage(UInt32 msgId, C2SMessageType c2sType)
    {
        NormalSend(msgId, c2sType);
    }
}
