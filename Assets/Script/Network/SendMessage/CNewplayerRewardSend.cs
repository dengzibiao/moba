/*
文件名（File Name）:   CNewplayerRewardSend.cs

作者（Author）:    #高#

创建时间（CreateTime）:  #CreateTime#
*/

public class CNewplayerRewardSend : CSendBase
{
    public CNewplayerRewardSend(ClientSendDataMgr mgr) : base(mgr)
    {
    }

    public void NewplayerRewardSend()
    {
        NormalSend(MessageID.common_newbie_reward_req);
    }
}
