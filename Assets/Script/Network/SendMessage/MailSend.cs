using System.Collections.Generic;

public class CMailSend : CSendBase
{
    public CMailSend(ClientSendDataMgr mgr)
        : base(mgr)
    {

    }

    public void SendGetAllMailList(C2SMessageType c2sType)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", 0);//开始位置
        newpacket.Add("arg2", 20);//获取个数
        PackNormalKvpAndSend(MessageID.common_player_mail_list_req, newpacket, c2sType);
    }

    public void SendGetSingleMailInfo(long mailID,C2SMessageType c2sType)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", mailID);
        PackNormalKvpAndSend(MessageID.common_read_mail_req, newpacket, c2sType);
    }

    public void SendGetSingleMailGoods(long mailID,C2SMessageType c2sType)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", mailID);
        PackNormalKvpAndSend(MessageID.common_distill_mail_item_req, newpacket, c2sType);
    }

    public void SendDeleteSingleMail(long mailID,C2SMessageType c2sType)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", mailID);
        PackNormalKvpAndSend(MessageID.common_delete_mail_req, newpacket, c2sType);
    }

    public void SendGetNewMailCount(C2SMessageType c2sType)
    {
        NormalSend(MessageID.common_new_mail_count_req, c2sType);
    }

    public void SendBatchChangeMailState(List<long> idList,C2SMessageType c2sType)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", idList);//邮件ID列表 {23,567,980}
        PackNormalKvpAndSend(MessageID.common_change_mail_newf_req, newpacket, c2sType);
    }
}

