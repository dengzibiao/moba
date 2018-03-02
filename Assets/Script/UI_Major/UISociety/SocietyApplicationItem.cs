using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
/// <summary>
/// 会长： 其他玩家入会申请item
/// </summary>
public class SocietyApplicationItem : GUISingleItemList
{

    public GUISingleLabel playerID;//玩家id
    public GUISingleLabel playerName;//玩家名字
    public GUISingleLabel applicationTime;//申请时间
    public GUISingleButton sureBtn;//同意入会
    public GUISingleButton refuseBtn;//拒绝入会
    private SocietyApplicationData applicationData;

    protected override void InitItem()
    {
        sureBtn.onClick = OnSureClick;
        refuseBtn.onClick = OnRefuseClick;
    }
    public override void Info(object obj)
    {
        applicationData = (SocietyApplicationData)obj;
    }
    protected override void ShowHandler()
    {
        if (applicationData!=null)
        {
            //playerID.text = applicationData.playerId + "";
            //playerName.text = applicationData.playeName;
            //applicationTime.text = applicationData.applicationTime;

            playerID.text = "【" + applicationData.applicationTime + "】"+ "[2dd740]" + applicationData.playeName+"[-]"+"申请加入公会";
        }
    }
    private void OnRefuseClick()
    {
        Debug.Log("发送拒绝玩家入会协议");
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", applicationData.playerId);//玩家账户
        newpacket.Add("arg2", 2);//玩家账户
        Singleton<Notification>.Instance.Send(MessageID.union_approve_application_req, newpacket, C2SMessageType.ActiveWait);
        //ClientSendDataMgr.GetSingle().GetSocietySend().SendApproveJoinSociety(C2SMessageType.ActiveWait, applicationData.playerId,2);
    }

    private void OnSureClick()
    {
        Debug.Log("发送同意玩家入会协议");
        if (applicationData!=null)
        {
            Dictionary<string, object> newpacket = new Dictionary<string, object>();
            newpacket.Add("arg1", applicationData.playerId);//玩家账户
            newpacket.Add("arg2", 1);//玩家账户
            Singleton<Notification>.Instance.Send(MessageID.union_approve_application_req, newpacket, C2SMessageType.ActiveWait);
            //ClientSendDataMgr.GetSingle().GetSocietySend().SendApproveJoinSociety(C2SMessageType.ActiveWait, applicationData.playerId,1);
        }
       
    }
}
