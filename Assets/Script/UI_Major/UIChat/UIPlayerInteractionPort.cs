using UnityEngine;
using System.Collections;
using System;

public class UIPlayerInteractionPort : GUIBase {

    private UIGrid grid;
    //private UIScrollView scrollView;
    private GUISingleButton lookPlayerInfoBtn;
    private GUISingleButton privateChatBtn;
    //private GUISingleButton addTeamBtn;
    private GUISingleButton addFriendBtn;
    private GUISingleButton inviteAddSocietyBtn;
    private GUISingleButton shieldBtn;
    private GameObject maskObj;

    public override UIPanleID GetUIKey()
    {
        return UIPanleID.UIPlayerInteractionPort;
    }
    protected override void Init()
    {
        lookPlayerInfoBtn = transform.Find("Container/ScrollView/Grid/lookPlayerInfoBtn").GetComponent<GUISingleButton>();
        privateChatBtn = transform.Find("Container/ScrollView/Grid/privateChatBtn").GetComponent<GUISingleButton>();
        //addTeamBtn = transform.Find("Container/ScrollView/Grid/addTeamBtn").GetComponent<GUISingleButton>();
        addFriendBtn = transform.Find("Container/ScrollView/Grid/addFriendBtn").GetComponent<GUISingleButton>();
        inviteAddSocietyBtn = transform.Find("Container/ScrollView/Grid/inviteAddSocietyBtn").GetComponent<GUISingleButton>();
        shieldBtn = transform.Find("Container/ScrollView/Grid/shieldBtn").GetComponent<GUISingleButton>();

        //scrollView = transform.Find("Container/ScrollView").GetComponent<UIScrollView>();
        grid =transform.Find("Container/ScrollView/Grid").GetComponent<UIGrid>();

        maskObj = transform.Find("Mask").gameObject;

        UIEventListener.Get(maskObj).onClick += OnCloseClick;
        lookPlayerInfoBtn.onClick = OnLookPlayerInfoClick;
        privateChatBtn.onClick = OnPrivateChatClick;
        //addTeamBtn.onClick = OnAddTeamClick;
        addFriendBtn.onClick = OnAddFriendClick;
        inviteAddSocietyBtn.onClick = OnInviteAddSocietyClick;
        shieldBtn.onClick = OnShieldClick;
    }

    private void OnCloseClick(GameObject go)
    {
        //Hide();
        Control.HideGUI(this.GetUIKey());
    }

    /// <summary>
    /// 屏蔽按钮事件
    /// </summary>
    private void OnShieldClick()
    {
        grid.Reposition();
        string str1 = "屏蔽玩家后，玩家将进入黑名单，是否确认";
        string str2 = "";
        object[] obj = new object[5] { str1, str2, UIPopupType.EnSure, this.gameObject, "ShieldEvent" };
        Control.ShowGUI(UIPanleID.UIPopUp, EnumOpenUIType.DefaultUIOrSecond, false, obj);
        //Hide();
    }
    private void ShieldEvent()
    {
        Debug.Log("将该玩家加入黑名单");
        //Hide();
        Control.HideGUI(this.GetUIKey());
        ClientSendDataMgr.GetSingle().GetFriendSend().FriendsDelete(Globe.privateChatPlayerId, (int)Friends.Delete);
    }
    /// <summary>
    /// 邀请入帮按钮事件
    /// </summary>
    private void OnInviteAddSocietyClick()
    {
        grid.Reposition();
        //UIPromptBox.Instance.ShowLabel("公会功能还未开放");
        Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, "公会功能还未开放");
        //Hide();
        Control.HideGUI(this.GetUIKey());
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
        Debug.Log("发送添加好友");
        //Hide();
        Control.HideGUI(this.GetUIKey());
        ClientSendDataMgr.GetSingle().GetFriendSend().FriendsAdd(Globe.privateChatPlayerId, Globe.privateChatPlayerAId);
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
        grid.Reposition();
        Debug.Log("私聊");
        //Hide();
        //if (UIChatPanel.Instance.gameObject.activeSelf)
        //{
        //    //如果聊天面板已经打开 直接切换到聊天频道
        //    Globe.isHavePrivateTarget = true;
        //    UIChatPanel.Instance.checkBoxs.setMaskState((int)(ChatType.PrivateChat) - 1);
        //    UIChatPanel.Instance.OnChatChannelClick((int)(ChatType.PrivateChat)-1,true);
        //}
        //else
        //{
        //    //如果聊天面板未打开 打开面板并且换到聊天频道
        //}
        Control.HideGUI(this.GetUIKey());
        if (alreadyOpen)
        {
            Globe.isHavePrivateTarget = true;
            UIChatPanel.Instance.checkBoxs.setMaskState((int)(ChatType.PrivateChat) - 1);
            UIChatPanel.Instance.OnChatChannelClick((int)(ChatType.PrivateChat) - 1, true);
        }
        else
        {
            object[] temlist = new object[] { ChatType.PrivateChat, Globe.privateChatPlayerId, Globe.privateChatPlayerAId, Globe.chatingPlayerNickName };
            Control.ShowGUI(UIPanleID.UIChatPanel, EnumOpenUIType.DefaultUIOrSecond, false, temlist);
        }
    }
    /// <summary>
    /// 查看信息按钮事件
    /// </summary>
    private void OnLookPlayerInfoClick()
    {
        grid.Reposition();
        Debug.Log("未开放查看信息");
    }
    bool alreadyOpen = false;
    protected override void SetUI(params object[] uiParams)
    {
        if (uiParams.Length > 0)
        {
            alreadyOpen = (bool)uiParams[0];
            Globe.privateChatPlayerId = (long)uiParams[1];
            Globe.privateChatPlayerAId = (long)uiParams[2];
            Globe.chatingPlayerNickName = (string)uiParams[3];
        }
        base.SetUI();
    }
    protected override void OnLoadData()
    {
        base.OnLoadData();
        Show();
    }
    protected override void ShowHandler()
    {
        base.ShowHandler();
        //grid.repositionNow = true;
        grid.Reposition();
    }
}
