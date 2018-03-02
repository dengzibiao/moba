/*
文件名（File Name）:   Every.cs

作者（Author）:    #高#

创建时间（CreateTime）:  #CreateTime#
*/
using System.Collections.Generic;

public class CEveryDailyTaskSend : CSendBase
{
    public CEveryDailyTaskSend(ClientSendDataMgr mgr) : base(mgr)
    {
    }

    public void EveryDailyRequest(int id,int type,int boxInndex=0)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", id);//任务ID
        newpacket.Add("arg2", type);//任务类型1日常2领箱子
        newpacket.Add("arg3", boxInndex);//任务ID
        PackNormalKvpAndSend(MessageID.common_get_reward_props_req, newpacket, C2SMessageType.Active);
    }
}
