using System;
using System.Collections.Generic;

public class CRoleSend :CSendBase {

    public CRoleSend(ClientSendDataMgr mgr)
        : base(mgr)
	{
	}

    public void SendRoleInfo(String strName)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", strName);//昵称
        PackNormalKvpAndSend(MessageID.login_change_name_req, newpacket, C2SMessageType.Active);
    }

    public void SendIcon()
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("photo", playerData.GetInstance().iconData.icon_id);//头像
        newpacket.Add("frame", playerData.GetInstance().iconFrameData.iconFrame_id);//头像框
        PackNormalKvpAndSend(MessageID.login_change_photo_frame_req, newpacket, C2SMessageType.Active);
    }
}
