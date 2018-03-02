using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UIHaveJoinSocietyPanel : GUIBase
{
    public static UIHaveJoinSocietyPanel Instance;
    public GUISingleCheckBoxGroup checkBoxs;
    private Transform memberScrollView;//公会成员列表scrollview
    private Transform societyLogScrollView;//公会日志scrollview
    private Transform applyforScrollView;//如果是会长和副会长 其他玩家的申请列表
    private Transform titleTransfrom;//标题
    private Transform sendSocietyMailPanel;//发送公会邮件面板
    private Transform editSocietyInfoPanel;
    private Transform notApplyfor;//没有申请列表提示
    public GUISingleMultList memberMultList;
    public GUISingleMultList societyLogMultList;
    public GUISingleMultList applyforMultList;
    public GUISingleButton backBtn;//返回按钮
    public GUISingleButton chatBtn;//返回按钮
    public GUISingleSprite redPoint;//申请列表的红点

    //公会信息
    public Transform societyInfoTrans;
    private UISprite societyIcon;
    private UILabel societyName;//公会名称
    private UILabel societyID;//公会id
    private UILabel presidentName;//会长名称
    private UILabel societyLevel;//公会等级
    private UILabel allContributionValue;//公会贡献度
    private UILabel societyManifesto;//公会宣言
    public GUISingleButton editManifestoBtn;//编辑公会宣言Btn
    public GUISingleButton recruitBtn;//招募队员Btn
    public GUISingleButton exitSocietyBtn;//退出公会Btn
    public GUISingleButton dissolveSocietyBtn;//退出公会Btn

    public object[] memberDataObjs;//公会成员列表
    public object[] applicationSocietyDataObjs;//申请加入公会列表
    public UIHaveJoinSocietyPanel()
    {
        Instance = this;
    }
    protected override void Init()
    {
        memberScrollView = transform.Find("MemberScrollView");
        memberMultList = transform.Find("MemberScrollView/MemberMultList").GetComponent<GUISingleMultList>();

        societyLogScrollView = transform.Find("SocietyLogScrollView");
        societyLogMultList = transform.Find("SocietyLogScrollView/SocietyLogMultList").GetComponent<GUISingleMultList>();

        applyforScrollView = transform.Find("ApplyforScrollView");
        applyforMultList = transform.Find("ApplyforScrollView/ApplyforMultList").GetComponent<GUISingleMultList>();
        titleTransfrom = transform.Find("Title");
        sendSocietyMailPanel = transform.Find("UISendSocietyMailPanel");
        editSocietyInfoPanel = transform.Find("EditSocietyInfoPanel");
        notApplyfor = transform.Find("NotApplyfor");

        societyInfoTrans = transform.Find("SocietyInfo");
        societyIcon = transform.Find("SocietyInfo/SocietyIcon").GetComponent<UISprite>();
        societyName = transform.Find("SocietyInfo/SocietyName").GetComponent<UILabel>();
        societyID = transform.Find("SocietyInfo/SocietyID").GetComponent<UILabel>();
        presidentName = transform.Find("SocietyInfo/PresidentName").GetComponent<UILabel>();
        societyLevel = transform.Find("SocietyInfo/SocietyLevel").GetComponent<UILabel>();
        allContributionValue = transform.Find("SocietyInfo/AllContributionValue").GetComponent<UILabel>();
        societyManifesto = transform.Find("SocietyInfo/SocietyManifesto").GetComponent<UILabel>();
        editManifestoBtn = transform.Find("SocietyInfo/EditManifestoBtn").GetComponent<GUISingleButton>();
        recruitBtn = transform.Find("SocietyInfo/RecruitBtn").GetComponent<GUISingleButton>();
        exitSocietyBtn = transform.Find("SocietyInfo/ExitSocietyBtn").GetComponent<GUISingleButton>();
        dissolveSocietyBtn = transform.Find("SocietyInfo/DissolveSocietyBtn").GetComponent<GUISingleButton>();
        checkBoxs.onClick = OnCheckClick;
        backBtn.onClick = OnBackClick;
        chatBtn.onClick = OnChatClick;
        editManifestoBtn.onClick = OnEditManifestoClick;
        recruitBtn.onClick = OnRecruitClick;
        exitSocietyBtn.onClick = OnExitSocietyClick;
        dissolveSocietyBtn.onClick = OnDissolveSocietyClick;
    }
    public override UIPanleID GetUIKey()
    {
        return UIPanleID.UIHaveJoinSocietyPanel;
    }
    private void ShowRedPoint(Dictionary<int, List<int>> redlist)
    {
        bool isShow = Singleton<RedPointManager>.Instance.GetChildList(EnumRedPoint.RD_Guild, 1);
        if (isShow)
        {
            redPoint.ShowOrHide(isShow);
        }
        else
        {
            redPoint.ShowOrHide(isShow);
        }
    }
    private void OnChatClick()
    {
        Debug.Log("打开公会聊天界面");
        Control.ShowGUI(UIPanleID.UIChatPanel, EnumOpenUIType.DefaultUIOrSecond, false,ChatType.SocietyChat);
        //UIChatPanel.Instance.ExternalOpenSocietyChat();
    }

    private void OnDissolveSocietyClick()
    {
        Debug.Log("解散公会");
        object[] obj = new object[5] { "确定要解散公会吗？", "解散后公会将不复存在，是否确认？", UIPopupType.EnSure, this.gameObject, "DissolveSocietyEvent" };
        Control.ShowGUI(UIPanleID.UIPopUp, EnumOpenUIType.DefaultUIOrSecond, false, obj);
        //ClientSendDataMgr.GetSingle().GetSocietySend().SendDissolveSociety(C2SMessageType.ActiveWait);
    }
    private void DissolveSocietyEvent()
    {
        Singleton<Notification>.Instance.Send(MessageID.union_disband_someone_req, C2SMessageType.ActiveWait);
        //ClientSendDataMgr.GetSingle().GetSocietySend().SendDissolveSociety(C2SMessageType.ActiveWait);
    }

    private void OnExitSocietyClick()
    {
        Debug.Log("退出公会");
        if (SocietyManager.Single().societyStatus == SocietyStatus.Member)
        {
            object[] obj = new object[5] { "确定要退出公会吗？", "退出后贡献值将清空不可恢复，是否确认？", UIPopupType.EnSure, this.gameObject, "MemberExitSocietyEvent" };
            Control.ShowGUI(UIPanleID.UIPopUp, EnumOpenUIType.DefaultUIOrSecond, false, obj);
        }
        else if (SocietyManager.Single().societyStatus == SocietyStatus.President)
        {
            object[] obj = new object[5] { "确定要退出公会吗？", "退出后会长将有系统指认当前贡献度最高的人，是否确认？", UIPopupType.EnSure, this.gameObject, "PresidentExitSocietyEvent" };
            Control.ShowGUI(UIPanleID.UIPopUp, EnumOpenUIType.DefaultUIOrSecond, false, obj);
        }
        //ClientSendDataMgr.GetSingle().GetSocietySend().SendExitSociety(C2SMessageType.ActiveWait);
    }
    private void MemberExitSocietyEvent()
    {
        Singleton<Notification>.Instance.Send(MessageID.union_exits_someone_req, C2SMessageType.ActiveWait);
        //ClientSendDataMgr.GetSingle().GetSocietySend().SendExitSociety(C2SMessageType.ActiveWait);
    }
    private void PresidentExitSocietyEvent()
    {
        ClientSendDataMgr.GetSingle().GetSocietySend().SendExitSociety(C2SMessageType.ActiveWait);
    }
    private void OnRecruitClick()
    {
        Debug.Log("招募队员");
    }

    private void OnEditManifestoClick()
    {
        Debug.Log("编辑公会宣言");
        editSocietyInfoPanel.gameObject.SetActive(true);
    }
    public void SetCheckBoxGroup()
    {
        GUISingleCheckBox[]  aa =checkBoxs.GetBoxList();
        if (SocietyManager.Single().societyStatus == SocietyStatus.Member)
        {
            if (aa.Length >= 3)
            {
                aa[aa.Length - 1].gameObject.SetActive(false);
                aa[aa.Length - 2].gameObject.SetActive(false);
                Singleton<RedPointManager>.Instance.DeletChildFlag(EnumRedPoint.RD_Guild, 1);
                redPoint.Hide();
            }
        }
        else if (SocietyManager.Single().societyStatus == SocietyStatus.President)
        {
            aa[aa.Length - 1].gameObject.SetActive(false);
            aa[aa.Length - 2].gameObject.SetActive(true);
        }

    }

    /// <summary>
    /// 设置或者刷新公会详情
    /// </summary>
    public void SetSocietyInfo()
    {
        societyIcon.spriteName = SocietyManager.Single().playerSocietyData.societyIcon;
        societyName.text = SocietyManager.Single().playerSocietyData.societyName;
        societyID.text = "ID:"+SocietyManager.Single().playerSocietyData.societyID;
        presidentName.text = SocietyManager.Single().playerSocietyData.presidentName;
        societyLevel.text = SocietyManager.Single().playerSocietyData.societyLevel + "";
        societyManifesto.text = SocietyManager.Single().playerSocietyData.societyManifesto;
        if (SocietyManager.Single().societyStatus == SocietyStatus.Member)
        {
            editManifestoBtn.gameObject.SetActive(false);
            dissolveSocietyBtn.gameObject.SetActive(false);
            exitSocietyBtn.gameObject.SetActive(true);
        }
        else if (SocietyManager.Single().societyStatus == SocietyStatus.President)
        {
            editManifestoBtn.gameObject.SetActive(true);
            dissolveSocietyBtn.gameObject.SetActive(true);
            exitSocietyBtn.gameObject.SetActive(false);
        }
    }
    /// <summary>
    /// 设置或者刷新申请列表
    /// </summary>
    public void SetApplicationSocietyList()
    {
        applicationSocietyDataObjs = SocietyManager.Single().SocietyApplicationList.ToArray();
        if (applicationSocietyDataObjs != null && applicationSocietyDataObjs.Length > 0)
        {
            applyforMultList.InSize(applicationSocietyDataObjs.Length, 1);
            applyforMultList.Info(applicationSocietyDataObjs);
            notApplyfor.gameObject.SetActive(false);
            Singleton<RedPointManager>.Instance.AddChildFlag(EnumRedPoint.RD_Guild,1);
            redPoint.ShowOrHide(true);

        }
        else
        {
            applyforMultList.InSize(applicationSocietyDataObjs.Length, 1);
            applyforMultList.Info(applicationSocietyDataObjs);
            notApplyfor.gameObject.SetActive(true);
            Singleton<RedPointManager>.Instance.DeletChildFlag(EnumRedPoint.RD_Guild, 1);
            redPoint.ShowOrHide(false);
        }
        
    }
    /// <summary>
    /// 设置或者刷新公会成员列表
    /// </summary>
    public void SetSocietyMemberList()
    {
        memberDataObjs = SocietyManager.Single().societyMemberlist.ToArray();
        memberMultList.InSize(memberDataObjs.Length, 1);
        memberMultList.Info(memberDataObjs);
    }
    private void InitData()
    {
        memberDataObjs = SocietyManager.Single().societyMemberlist.ToArray();
    }


    protected override void OnLoadData()
    {
        base.OnLoadData();
        Singleton<Notification>.Instance.RegistMessageID(MessageID.union_query_all_member_ret, UIPanleID.UIHaveJoinSocietyPanel);
        Singleton<Notification>.Instance.RegistMessageID(MessageID.union_query_detailed_info_ret, UIPanleID.UIHaveJoinSocietyPanel);
        Singleton<Notification>.Instance.RegistMessageID(MessageID.union_change_some_info_ret, UIPanleID.UIHaveJoinSocietyPanel);
        Singleton<Notification>.Instance.RegistMessageID(MessageID.union_query_application_list_ret, UIPanleID.UIHaveJoinSocietyPanel);
        Singleton<Notification>.Instance.RegistMessageID(MessageID.union_approve_application_ret, UIPanleID.UIHaveJoinSocietyPanel);
        Singleton<Notification>.Instance.RegistMessageID(MessageID.union_change_someone_position_ret, UIPanleID.UIHaveJoinSocietyPanel);
        Singleton<Notification>.Instance.RegistMessageID(MessageID.union_kickout_someone_ret, UIPanleID.UIHaveJoinSocietyPanel);
        Singleton<Notification>.Instance.RegistMessageID(MessageID.union_exits_someone_ret, UIPanleID.UIHaveJoinSocietyPanel);
        Singleton<Notification>.Instance.RegistMessageID(MessageID.union_disband_someone_ret, UIPanleID.UIHaveJoinSocietyPanel);
        Singleton<Notification>.Instance.RegistMessageID(MessageID.union_information_changed_notify, UIPanleID.UIHaveJoinSocietyPanel);
        this.State = EnumObjectState.Ready;
        Show();
    }
    public override void ReceiveData(UInt32 messageID)
    {
        base.ReceiveData(messageID);
        switch (messageID)
        {
            case MessageID.union_query_all_member_ret://公会成员列表
                SetSocietyMemberList();
                break;
            case MessageID.union_query_detailed_info_ret://公会详细信息
                SetSocietyInfo();
                break;
            case MessageID.union_change_some_info_ret://修改公会宣言
                Dictionary<string, object> newpacket2 = new Dictionary<string, object>();
                newpacket2.Add("arg1", SocietyManager.Single().mySocityID);//工会ID，此数值为空则显示查询人所在工会信息
                Singleton<Notification>.Instance.Send(MessageID.union_query_detailed_info_req, newpacket2, C2SMessageType.ActiveWait);
                break;
            case MessageID.union_query_application_list_ret://公会申请列表
                SetApplicationSocietyList();
                break;
            case MessageID.union_approve_application_ret://同意或者拒绝申请
                Singleton<Notification>.Instance.Send(MessageID.union_query_application_list_req, C2SMessageType.ActiveWait);
                break;
            case MessageID.union_change_someone_position_ret://传位
                break;
            case MessageID.union_kickout_someone_ret://踢出
                break;
            case MessageID.union_exits_someone_ret://退出公会
            case MessageID.union_disband_someone_ret://解散公会
                Control.HideGUI();
                break;
            case MessageID.union_information_changed_notify://权限改变
                Show();
                break;
        }
    }
    protected override void ShowHandler()
    {
        //InitData();
        SetCheckBoxGroup();
        checkBoxs.DefauleIndex = 0;
        ShowRedPoint(Singleton<RedPointManager>.Instance.GetRedList());
    }
    private void OnBackClick()
    {
        Control.HideGUI();
        //Hide();
    }
    private void OnCheckClick(int index, bool boo)
    {
        if (boo)
        {
            switch (index)
            {
                case 0:
                    titleTransfrom.gameObject.SetActive(true);
                    memberScrollView.gameObject.SetActive(true);
                    societyLogScrollView.gameObject.SetActive(false);
                    sendSocietyMailPanel.gameObject.SetActive(false);
                    applyforScrollView.gameObject.SetActive(false);
                    societyInfoTrans.gameObject.SetActive(true);
                    notApplyfor.gameObject.SetActive(false);
                    //获取公会成员信息
                    Dictionary<string, object> newpacket1 = new Dictionary<string, object>();
                    newpacket1.Add("arg1", SocietyManager.Single().mySocityID);
                    Singleton<Notification>.Instance.Send(MessageID.union_query_all_member_req, newpacket1, C2SMessageType.ActiveWait);
                    //ClientSendDataMgr.GetSingle().GetSocietySend().SendGetSocietyMemberList(C2SMessageType.ActiveWait, SocietyManager.Single().mySocityID);
                    //获取公会详情
                    Dictionary<string, object> newpacket2 = new Dictionary<string, object>();
                    newpacket2.Add("arg1", SocietyManager.Single().mySocityID);//工会ID，此数值为空则显示查询人所在工会信息
                    Singleton<Notification>.Instance.Send(MessageID.union_query_detailed_info_req, newpacket2, C2SMessageType.ActiveWait);
                    //ClientSendDataMgr.GetSingle().GetSocietySend().SendGetSocietyInfo(C2SMessageType.ActiveWait, SocietyManager.Single().mySocityID);
                    break;
                //case 1:
                //    titleTransfrom.gameObject.SetActive(false);
                //    memberScrollView.gameObject.SetActive(false);
                //    societyLogScrollView.gameObject.SetActive(true);
                //    sendSocietyMailPanel.gameObject.SetActive(false);
                //    applyforScrollView.gameObject.SetActive(false);
                //    societyInfoTrans.gameObject.SetActive(false);
                //    break;
                case 1:
                    titleTransfrom.gameObject.SetActive(false);
                    memberScrollView.gameObject.SetActive(false);
                    societyLogScrollView.gameObject.SetActive(false);
                    sendSocietyMailPanel.gameObject.SetActive(false);
                    applyforScrollView.gameObject.SetActive(true);
                    societyInfoTrans.gameObject.SetActive(false);
                    notApplyfor.gameObject.SetActive(false);
                    //获取公会申请列表
                    Singleton<Notification>.Instance.Send(MessageID.union_query_application_list_req,C2SMessageType.ActiveWait);
                    //ClientSendDataMgr.GetSingle().GetSocietySend().SendGetApplicationJoinSocietyList(C2SMessageType.ActiveWait);
                    break;
                case 2:
                    titleTransfrom.gameObject.SetActive(false);
                    memberScrollView.gameObject.SetActive(false);
                    societyLogScrollView.gameObject.SetActive(false);
                    applyforScrollView.gameObject.SetActive(false);
                    sendSocietyMailPanel.gameObject.SetActive(true);
                    societyInfoTrans.gameObject.SetActive(false);
                    notApplyfor.gameObject.SetActive(false);
                    break;
            }

        }
    }
}
