using UnityEngine;
using System.Collections.Generic;

public class CGuideSend : CSendBase {

    public CGuideSend(ClientSendDataMgr mgr) : base(mgr)
    {

    }
    ///// <summary>
    ///// 请求新手引导
    ///// </summary>
    ///// <param name="c2sType"></param>
    //public void GuidesStepFinish(C2SMessageType c2sType)
    //{
    //    Debug.Log("GuidesStepFinish");
    //    //

    //    CWritePacket packet = new CWritePacket(MessageID.s_player_guide_info_req);
    //    Dictionary<string, object> newpacket = new Dictionary<string, object>();
    //    newpacket.Add("msgid", ((int)MessageID.s_player_guide_info_req));//某新手引导步骤完成通知 9745

    //    newpacket.Add("playerId", playerData.GetInstance().selfData.playerId);
    //    newpacket.Add("account", playerData.GetInstance().selfData.accountId);//玩家账户
    //    newpacket.Add("arg1", playerData.GetInstance().guideData.GuideId);// guideId  完成新手引导ID
    //    newpacket.Add("arg2", playerData.GetInstance().guideData.Skip);//是否跳过新手引导 0否 1是

    //    System.Text.StringBuilder stringbuilder = Jsontext.WriteData(newpacket);
    //    stringbuilder.Append("\0");

    //    string json_s = stringbuilder.ToString();
    //    if (DataDefine.isEFS == 1)
    //    {
    //        //加密处理
    //        json_s = packet.Compress(json_s, DataDefine.datakey);
    //    }

    //    packet.WriteString(json_s);
    //    Debug.Log(json_s);

    //    SendPacket(packet, c2sType);
    //}
    /// <summary>
    /// 请求功能开放通知
    /// </summary>
    /// <param name="c2sType"></param>
    //public void GuideFunctionOpeningNotify(C2SMessageType c2sType)
    //{
    //    Debug.Log("GuideFunctionOpeningNotify");
    //    //

    //    CWritePacket packet = new CWritePacket(MessageID.guide_function_opening_notify);
    //    Dictionary<string, object> newpacket = new Dictionary<string, object>();
    //    newpacket.Add("msgid", ((int)MessageID.guide_function_opening_notify));//功能开放通知 1552

    //    newpacket.Add("guide", 0);
    //    newpacket.Add("func", 0);

    //    System.Text.StringBuilder stringbuilder = Jsontext.WriteData(newpacket);
    //    stringbuilder.Append("\0");

    //    string json_s = stringbuilder.ToString();
    //    if (DataDefine.isEFS == 1)
    //    {
    //        //加密处理
    //        json_s = packet.Compress(json_s, DataDefine.datakey);
    //    }

    //    packet.WriteString(json_s);
    //    Debug.Log(json_s);

    //    SendPacket(packet, c2sType);
    //}

    //   
    public void SendGuidStep(int step)
    {
        //Debug.Log("<color=#10DF11>SendGuidStep</color>" + step.ToString());
        Dictionary<string, object> newpacket = new Dictionary<string, object>();

        if(playerData.GetInstance().guideData.scripId == 0)
        {
            playerData.GetInstance().guideData.scripId = GuideManager.Single().GetBitValue(playerData.GetInstance().selfData.infodata[830], 0, 16);
            playerData.GetInstance().guideData.typeId = GuideManager.Single().GetBitValue(playerData.GetInstance().selfData.infodata[831], 0, 16);
            playerData.GetInstance().guideData.stepId = GuideManager.Single().GetBitValue(playerData.GetInstance().selfData.infodata[831], 17, 16);
            //Debug.Log("<color=#10DF11>scripId</color>" + playerData.GetInstance().guideData.scripId + "<color=#10DF11>typeId</color>" + playerData.GetInstance().guideData.typeId
            //   + "<color=#10DF11>stepId</color>" + playerData.GetInstance().guideData.stepId);
        }

        if(step == 99)
        {
            //Debug.Log("<color=#10DF11>SendGuidStep</color>" + step.ToString());
            newpacket.Add("sp", playerData.GetInstance().guideData.stepId = 99);
        }
        else
        {
            newpacket.Add("sp", playerData.GetInstance().guideData.stepId);
        }

        newpacket.Add("sd", playerData.GetInstance().guideData.scripId);
        newpacket.Add("td", playerData.GetInstance().guideData.typeId);

        PackNormalKvpAndSend(MessageID.s_player_guide_info_req, newpacket, C2SMessageType.Active);
        if (step == 99)
        {
            playerData.GetInstance().guideData.scripId = 0;
            playerData.GetInstance().guideData.typeId = 0;
            playerData.GetInstance().guideData.stepId = 0;
        }
     
    }

    /// <summary>
    /// 引导激活信息
    /// </summary>
    public void SendGuidSet(GameObject go)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        switch(go.name)
        {
            case "bc":
                //newpacket.Add("arg1", 833);
                //newpacket.Add("arg2", 0);
                //newpacket.Add("arg3", 1);
                break;
            default:
                break;
        }
        PackNormalKvpAndSend(MessageID.guide_set_req, newpacket, C2SMessageType.Active);
    }

    public void SendGuidAskEvent()
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        if(playerData.GetInstance().guideData.uId == 0)
        {
            //newpacket.Add("arg1", playerData.GetInstance().guideData.uId = 906);
        }
        else
        {
            //Debug.Log("<color=#10DF11>SendGuidAskEvent uId</color>" + playerData.GetInstance().guideData.uId);
            newpacket.Add("arg1", playerData.GetInstance().guideData.uId);
        }
        PackNormalKvpAndSend(MessageID.guide_ask_for_event_req, newpacket, C2SMessageType.Active);
        //Debug.Log("<color=#10DF11>SendGuidAskEvent uId</color>" + playerData.GetInstance().guideData.uId);
    }
}
