using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class UISocietyInteractionPort : GUIBase
{
    public static UISocietyInteractionPort Instance;
    private UIGrid grid;
    private UIScrollView scrollView;
    private GUISingleButton lookPlayerInfoBtn;
    private GUISingleButton privateChatBtn;
    //private GUISingleButton addTeamBtn;
    private GUISingleButton addFriendBtn;
    private GUISingleButton shieldBtn;
    private GUISingleButton kickoutBtn;//踢出
    private GUISingleButton demiseBtn;//传位
    private GameObject maskObj;
    private UISprite bgSprite;
    private int offset = 40;
    private SocietyMemberData memberData;


    int[] jurisdictionArr;
    //控制显示哪几个选项
    private bool isLook = false;
    private bool isFriend = false;
    private bool isPrivate = false;
    private bool isShield = false;//屏蔽
    private bool isKickout = false;//踢出
    private bool isDemise = false;//传位
    public UISocietyInteractionPort()
    {
        Instance = this;
    }
    protected override void Init()
    {
        lookPlayerInfoBtn = transform.Find("Container/Grid/lookPlayerInfoBtn").GetComponent<GUISingleButton>();
        privateChatBtn = transform.Find("Container/Grid/privateChatBtn").GetComponent<GUISingleButton>();
        //addTeamBtn = transform.Find("Container/ScrollView/Grid/addTeamBtn").GetComponent<GUISingleButton>();
        addFriendBtn = transform.Find("Container/Grid/addFriendBtn").GetComponent<GUISingleButton>();
        shieldBtn = transform.Find("Container/Grid/shieldBtn").GetComponent<GUISingleButton>();
        kickoutBtn = transform.Find("Container/Grid/KickoutBtn").GetComponent<GUISingleButton>();
        demiseBtn = transform.Find("Container/Grid/DemiseBtn").GetComponent<GUISingleButton>();
        scrollView = transform.Find("Container/ScrollView").GetComponent<UIScrollView>();
        grid = transform.Find("Container/Grid").GetComponent<UIGrid>();
        bgSprite = transform.Find("BgSprite").GetComponent<UISprite>();
        maskObj = transform.Find("Mask").gameObject;

        UIEventListener.Get(maskObj).onClick += OnCloseClick;
        lookPlayerInfoBtn.onClick = OnLookPlayerInfoClick;
        privateChatBtn.onClick = OnPrivateChatClick;
        //addTeamBtn.onClick = OnAddTeamClick;
        addFriendBtn.onClick = OnAddFriendClick;
        shieldBtn.onClick = OnShieldClick;
        kickoutBtn.onClick = OnKickoutClick;
        demiseBtn.onClick = OnDemiseClick;
    }
    public override UIPanleID GetUIKey()
    {
        return UIPanleID.UISocietyInteractionPort;
    }
    public void SetData(int[] arr, SocietyMemberData data)
    {
        jurisdictionArr = arr;
        memberData = data;
    }
    protected override void SetUI(params object[] uiParams)
    {
        if (uiParams.Length > 0)
        {
            jurisdictionArr = (int[])uiParams[0];
            memberData = (SocietyMemberData)uiParams[1];
        }
        base.SetUI();
    }
    protected override void OnLoadData()
    {
        base.OnLoadData();
        Show();
    }
    private void OnDemiseClick()
    {
        //grid.Reposition();
        Debug.Log("传位");
        if (memberData != null)
        {
            Control.HideGUI(this.GetUIKey());
            //Hide();
            Dictionary<string, object> newpacket = new Dictionary<string, object>();
            newpacket.Add("arg1", memberData.playerId);
            Singleton<Notification>.Instance.Send(MessageID.union_change_someone_position_req, newpacket, C2SMessageType.ActiveWait);
            //ClientSendDataMgr.GetSingle().GetSocietySend().SendPresidentChange(C2SMessageType.ActiveWait, memberData.playerId);
        }
    }

    private void OnKickoutClick()
    {
        //grid.Reposition();
        Debug.Log("踢出公会");
        if (memberData!=null)
        {
            Control.HideGUI(this.GetUIKey());
            //Hide();
            Dictionary<string, object> newpacket = new Dictionary<string, object>();
            newpacket.Add("arg1", memberData.playerId);
            Singleton<Notification>.Instance.Send(MessageID.union_kickout_someone_req, newpacket, C2SMessageType.ActiveWait);
            //ClientSendDataMgr.GetSingle().GetSocietySend().SendKickoutSocietyMember(C2SMessageType.ActiveWait, memberData.playerId);
        }
        
    }
    private void OnCloseClick(GameObject go)
    {
        Control.HideGUI(this.GetUIKey());
        //Hide();
    }

    /// <summary>
    /// 屏蔽按钮事件
    /// </summary>
    private void OnShieldClick()
    {
        //grid.Reposition();
        string str1 = "屏蔽玩家后，玩家将进入黑名单，是否确认";
        string str2 = "";
        object[] obj = new object[5] { str1, str2, UIPopupType.EnSure, this.gameObject, "ShieldEvent" };
        Control.ShowGUI(UIPanleID.UIPopUp, EnumOpenUIType.DefaultUIOrSecond, false, obj);
        //Hide();
    }
    private void ShieldEvent()
    {
        grid.Reposition();
        Debug.Log("将该玩家加入黑名单");
        Control.HideGUI(this.GetUIKey());
        //Hide();
        ClientSendDataMgr.GetSingle().GetFriendSend().FriendsDelete(Globe.privateChatPlayerId, (int)Friends.Delete);
    }
    /// <summary>
    /// 加好友按钮事件
    /// </summary>
    private void OnAddFriendClick()
    {
        grid.Reposition();
        string str1 = "添加该玩家为好友，是否确认";
        string str2 = "";
        object[] obj = new object[5] { str1, str2, UIPopupType.EnSure, this.gameObject, "AddFriendEvent" };
        Control.ShowGUI(UIPanleID.UIPopUp, EnumOpenUIType.DefaultUIOrSecond, false, obj);
        //Hide();
    }

    private void AddFriendEvent()
    {
        grid.Reposition();
        Debug.Log("发送添加好友");
        //Hide();
        Control.HideGUI(this.GetUIKey());
        ClientSendDataMgr.GetSingle().GetFriendSend().FriendsAdd(memberData.playerId, memberData.accountId);
    }

    /// <summary>
    /// 组队按钮事件
    /// </summary>
    private void OnAddTeamClick()
    {
        grid.Reposition();
        Debug.Log("未开放组队");
    }
    /// <summary>
    /// 私聊按钮事件
    /// </summary>
    private void OnPrivateChatClick()
    {
        //grid.Reposition();
        Debug.Log("私聊");
        //Hide();
        Control.HideGUI(this.GetUIKey());
        object[] temlist= new object[] { ChatType.PrivateChat, memberData.playerId, memberData.accountId, memberData.memberName };
        Control.ShowGUI(UIPanleID.UIChatPanel, EnumOpenUIType.DefaultUIOrSecond, false, temlist);
        //if (UIChatPanel.Instance.gameObject.activeSelf)
        //{
        //    //如果聊天面板已经打开 直接切换到聊天频道
        //    Globe.privateChatPlayerId = memberData.playerId;
        //    Globe.privateChatPlayerAId = memberData.accountId;
        //    Globe.chatingPlayerNickName = memberData.memberName;
        //    Globe.isHavePrivateTarget = true;
        //    UIChatPanel.Instance.checkBoxs.setMaskState((int)(ChatType.PrivateChat) - 1);
        //    UIChatPanel.Instance.OnChatChannelClick((int)(ChatType.PrivateChat) - 1, true);
        //}
        //else
        //{
        //    //如果聊天面板未打开 打开面板并且换到聊天频道
        //    List<object> temlist = new List<object>();
        //    temlist.Add(ChatType.PrivateChat);
        //    temlist.Add(memberData.playerId);
        //    temlist.Add(memberData.accountId);
        //    temlist.Add(memberData.memberName);
        //    Control.ShowGUI(UIPanleID.UIChatPanel, EnumOpenUIType.DefaultUIOrSecond, false,temlist);
        //    //UIChatPanel.Instance.ExternalOpenPrivateChat(memberData.playerId,memberData.accountId,memberData.memberName);
        //}
    }
    /// <summary>
    /// 查看信息按钮事件
    /// </summary>
    private void OnLookPlayerInfoClick()
    {
        //grid.Reposition();
        Debug.Log("未开放查看信息");
    }

    public void SetShowBtnCount(int[] arr)
    {

    }
    protected override void ShowHandler()
    {
        base.ShowHandler();
        //grid.repositionNow = true;
        if (jurisdictionArr!=null&& jurisdictionArr.Length == 6)
        {

            isLook = jurisdictionArr[0] == 1 ? true : false;
            isFriend = jurisdictionArr[1] == 1 ? true : false;
            isPrivate = jurisdictionArr[2] == 1 ? true : false;
            isShield = jurisdictionArr[3] == 1 ? true : false;
            isKickout = jurisdictionArr[4] == 1 ? true : false;
            isDemise = jurisdictionArr[5] == 1 ? true : false;
        }
        lookPlayerInfoBtn.gameObject.SetActive(isLook);
        addFriendBtn.gameObject.SetActive(isFriend);
        privateChatBtn.gameObject.SetActive(isPrivate);
        shieldBtn.gameObject.SetActive(isShield);
        kickoutBtn.gameObject.SetActive(isKickout);
        demiseBtn.gameObject.SetActive(isDemise);
        grid.Reposition();
        bgSprite.height = (int)grid.cellHeight * grid.GetChildList().Count + offset;

    }
}
