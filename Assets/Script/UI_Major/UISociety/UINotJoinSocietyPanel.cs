using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tianyu;
using System;

public class UINotJoinSocietyPanel : GUIBase
{
    public static UINotJoinSocietyPanel Instance;
    public GUISingleCheckBoxGroup checkBoxs;
    private Transform societyScrollview;//公会列表scrollview
    private Transform searchSociettScrollView;//搜索公会列表scrollview
    private Transform otherSocietyInviteScrollView;//未加入公会时，其他公会邀请信息scrollview
    private Transform titleTransfrom;//公会列表scrollview
    private Transform searchSocietyPanel;//搜索公会面板
    private Transform notSearchSociety;//未搜索到公会提示
    private Transform createSocietyPanel;//创建公会面板
    public GUISingleMultList societyMultList;
    public GUISingleMultList searchSocietyMultList;
    public GUISingleMultList otherSocietyInviteMultList;
    public GUISingleButton backBtn;//返回按钮
    public GUISingleButton refreshBtn;//刷新按钮
    private GUISingleButton searchBtn;//搜索公会按钮
    private GUISingleInput searchInput;//输入公会id
    Vector3 titlePos1 = new Vector3(0f,273f,0f);
    Vector3 titlePos2 = new Vector3(0f,172f, 0f);

   
    public object[] societyDataObjs;//公会列表
    public object[] searchSocietyDataObjs;//搜索到的公会列表
    public UINotJoinSocietyPanel()
    {
        Instance = this;
    }
    protected override void Init()
    {
        societyScrollview = transform.Find("SocietyScrollView");
        societyMultList = transform.Find("SocietyScrollView/SocietyMultList").GetComponent<GUISingleMultList>();

        searchSociettScrollView = transform.Find("SearchSocietyScrollView");
        searchSocietyMultList = transform.Find("SearchSocietyScrollView/SearchSocietyMultList").GetComponent<GUISingleMultList>();

        otherSocietyInviteScrollView = transform.Find("OtherSocietyInviteScrollView");
        otherSocietyInviteMultList = transform.Find("OtherSocietyInviteScrollView/OtherSocietyInviteMultList").GetComponent<GUISingleMultList>();
        titleTransfrom = transform.Find("Title");
        searchSocietyPanel = transform.Find("SearchSocietyPanel");
        searchBtn = transform.Find("SearchSocietyPanel/SearchBtn").GetComponent<GUISingleButton>();
        searchInput = transform.Find("SearchSocietyPanel/SearchInput").GetComponent<GUISingleInput>();
        notSearchSociety = transform.Find("NotSearchSociety");
        createSocietyPanel = transform.Find("UICreateSocietyPanel");
        checkBoxs.onClick = OnCheckClick;
        backBtn.onClick = OnBackClick;
        searchBtn.onClick = OnSearchClick;
        refreshBtn.onClick = OnRefreshClick;
    }
    public override UIPanleID GetUIKey()
    {
        return UIPanleID.UINotJoinSocietyPanel;
    }
    private void OnRefreshClick()
    {
        //发送刷新协议
        Debug.Log("发送刷新协议");
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", 10);//玩家账户
        Singleton<Notification>.Instance.Send(MessageID.union_query_uncion_list_req, newpacket, C2SMessageType.ActiveWait);
    }
    private void OnSearchClick()
    {
        if (!string.IsNullOrEmpty(searchInput.text))
        {
            long id = 0;
            bool boo = long.TryParse(searchInput.text, out id);
            if (boo)
            {
                //搜索
                Debug.Log("发送搜索协议" + id);
                //ClientSendDataMgr.GetSingle().GetSocietySend().SendSearchSocietyList(C2SMessageType.ActiveWait, id);
                Dictionary<string, object> newpacket = new Dictionary<string, object>();
                newpacket.Add("arg1", id);//玩家账户
                Singleton<Notification>.Instance.Send(MessageID.union_search_someone_req, newpacket, C2SMessageType.ActiveWait);
            }
            else
            {
                Control.ShowGUI(UIPanleID.UITooltips, EnumOpenUIType.DefaultUIOrSecond, false, "请输入正确的公会ID");
            }
        }
        else
        {
            Control.ShowGUI(UIPanleID.UITooltips, EnumOpenUIType.DefaultUIOrSecond, false, "公会不可为空");
        }
    }
    /// <summary>
    /// 设置公会列表数据
    /// </summary>
    public void SetSocietyListData()
    {
        societyMultList.InSize(societyDataObjs.Length,1);
        societyMultList.Info(societyDataObjs);
        societyScrollview.GetComponent<UIScrollView>().ResetPosition();
    }
    /// <summary>
    /// 设置搜索到的公会列表
    /// </summary>
    /// <param name="have"></param>
    public void SetSearchSocietyData(bool have)
    {
        if (have)
        {
            titleTransfrom.gameObject.SetActive(true);
            searchSociettScrollView.gameObject.SetActive(true);
            notSearchSociety.gameObject.SetActive(false);
            SetTitlePos(2);
            searchSocietyMultList.InSize(searchSocietyDataObjs.Length, 1);
            searchSocietyMultList.Info(searchSocietyDataObjs);
            searchSociettScrollView.GetComponent<UIScrollView>().ResetPosition();
        }
        else
        {
            titleTransfrom.gameObject.SetActive(false);
            searchSociettScrollView.gameObject.SetActive(false);
            notSearchSociety.gameObject.SetActive(true);
        }
       
    }
    protected override void OnLoadData()
    {
        base.OnLoadData();
        Singleton<Notification>.Instance.RegistMessageID(MessageID.union_query_uncion_list_ret, UIPanleID.UINotJoinSocietyPanel);
        Singleton<Notification>.Instance.RegistMessageID(MessageID.union_search_someone_ret, UIPanleID.UINotJoinSocietyPanel);
        Singleton<Notification>.Instance.RegistMessageID(MessageID.union_application_join_ret, UIPanleID.UINotJoinSocietyPanel);
        Singleton<Notification>.Instance.RegistMessageID(MessageID.union_create_someone_ret, UIPanleID.UINotJoinSocietyPanel);
        if (SocietyManager.Single().societyList.Count == 0)
        {
            Dictionary<string, object> newpacket = new Dictionary<string, object>();
            newpacket.Add("arg1", 10);//玩家账户
            Singleton<Notification>.Instance.Send(MessageID.union_query_uncion_list_req, newpacket, C2SMessageType.ActiveWait);
        }
        else
        {
            this.State = EnumObjectState.Ready;
            Show();
        }
        
    }
    public override void ReceiveData(UInt32 messageID)
    {
        base.ReceiveData(messageID);
        switch (messageID)
        {
            case MessageID.union_query_uncion_list_ret:
                Show();
                break;
            case MessageID.union_search_someone_ret:
                RefreshData(1);
                break;
            case MessageID.union_application_join_ret:
                if (SocietyManager.Single().currentCheckBoxIndex == 0)
                {
                    RefreshData(0);
                }
                else if (SocietyManager.Single().currentCheckBoxIndex == 1)
                {
                    RefreshData(1);
                }
                break;
            case MessageID.union_create_someone_ret:
                Control.HideGUI();
                break;
        }
    }
    protected override void ShowHandler()
    {
        InitData();
        checkBoxs.DefauleIndex = 0;
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
                    refreshBtn.gameObject.SetActive(true);
                    searchSocietyPanel.gameObject.SetActive(false);
                    societyScrollview.gameObject.SetActive(true);
                    searchSociettScrollView.gameObject.SetActive(false);
                    createSocietyPanel.gameObject.SetActive(false);
                    otherSocietyInviteScrollView.gameObject.SetActive(false);
                    notSearchSociety.gameObject.SetActive(false);
                    SetTitlePos(1);
                    RefreshData(0);
                    //ClientSendDataMgr.GetSingle().GetSocietySend().SendGetSocietyList(C2SMessageType.ActiveWait, 10);
                    //societyMultList.InSize(societyDataObjs.Length,1);
                    //societyMultList.Info(societyDataObjs);
                    SocietyManager.Single().currentCheckBoxIndex = 0;
                    break;
                case 1:
                    titleTransfrom.gameObject.SetActive(false);
                    refreshBtn.gameObject.SetActive(false);
                    searchSocietyPanel.gameObject.SetActive(true);
                    societyScrollview.gameObject.SetActive(false);
                    searchSociettScrollView.gameObject.SetActive(false);
                    createSocietyPanel.gameObject.SetActive(false);
                    otherSocietyInviteScrollView.gameObject.SetActive(false);
                    notSearchSociety.gameObject.SetActive(false);
                    SocietyManager.Single().currentCheckBoxIndex = 1;
                    break;
                case 2:
                    titleTransfrom.gameObject.SetActive(false);
                    refreshBtn.gameObject.SetActive(false);
                    searchSocietyPanel.gameObject.SetActive(false);
                    societyScrollview.gameObject.SetActive(false);
                    searchSociettScrollView.gameObject.SetActive(false);
                    createSocietyPanel.gameObject.SetActive(true);
                    otherSocietyInviteScrollView.gameObject.SetActive(false);
                    notSearchSociety.gameObject.SetActive(false);
                    SocietyManager.Single().currentCheckBoxIndex = 2;
                    break;
                case 3:
                    titleTransfrom.gameObject.SetActive(false);
                    refreshBtn.gameObject.SetActive(false);
                    searchSocietyPanel.gameObject.SetActive(false);
                    societyScrollview.gameObject.SetActive(false);
                    searchSociettScrollView.gameObject.SetActive(false);
                    createSocietyPanel.gameObject.SetActive(false);
                    otherSocietyInviteScrollView.gameObject.SetActive(true);
                    notSearchSociety.gameObject.SetActive(false);
                    SocietyManager.Single().currentCheckBoxIndex = 3;
                    break;
            }

        }
    }

    private void InitData()
    {
        societyDataObjs = SocietyManager.Single().societyList.ToArray();
    }
    /// <summary>
    /// 刷新公会列表数据
    /// </summary>
    /// <param name="type">0:刷新公会列表数据 1：刷新搜索到的公会列表数据</param>
    public void RefreshData(int type)
    {
        if (type == 0)
        {
            societyDataObjs = SocietyManager.Single().societyList.ToArray();
            SetSocietyListData();
        }
        if (type == 1)
        {
            searchSocietyDataObjs = SocietyManager.Single().searchSocietyList.ToArray();
            if (searchSocietyDataObjs.Length >0)
            {
                SetSearchSocietyData(true);
            }
            else
            {
                SetSearchSocietyData(false);
            }
        }
    }
    /// <summary>
    /// 设置公会标题的位置
    /// </summary>
    /// <param name="type">1：公会列表的位置 2：搜索公会列表的位置</param>
    private void SetTitlePos(int type)
    {
        if (type ==1) titleTransfrom.localPosition = titlePos1;
        else if(type == 2) titleTransfrom.localPosition = titlePos2;
    }
}
