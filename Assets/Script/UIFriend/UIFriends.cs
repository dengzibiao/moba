using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tianyu;

public class UIFriends : GUIBase
{
    public static UIFriends Instance;
    public GUISingleCheckBoxGroup checkBoxs;
    public GUISingleMultList[] multList;
    public GUISingleInput serchinput;
    public GUISingleButton backBtn;
    public GUISingleSprite redPoint;
    private GUISingleButton _searchBtn;
    private GUISingleButton _refreshBtn;
    private GUISingleInput _searchInput;
    private Transform _searchHeroPanel;
    private int _index = 0;//当前选择的multList
    private Transform view;
    private bool isFriendAdd = false;//好友界面加好友
    private bool isSearchAdd = false;//搜索界面加好友
    private int _replaceIndex = 0;//刷新标记
    private int temporaryIndex = -1;
    public override UIPanleID GetUIKey()
    {
        return UIPanleID.UIFriends;
    }

    protected override void Init()
    {
        _searchHeroPanel = UnityUtil.FindObjectRecursively(gameObject, "SearchHeroPanel").transform;
        _searchBtn = UnityUtil.FindComponent<GUISingleButton>(_searchHeroPanel, "SearchBtn");
        _refreshBtn = UnityUtil.FindComponent<GUISingleButton>(_searchHeroPanel, "RefreshBtn");
        _searchInput = UnityUtil.FindComponent<GUISingleInput>(_searchHeroPanel, "SearchInput");
        multList = transform.GetComponentsInChildren<GUISingleMultList>(true);
        checkBoxs.onClick = OnCheckClick;
        backBtn.onClick = OnBackClick;
        view = transform.GetComponentInChildren<UIScrollView>().transform;
        _searchBtn.onClick = OnSearch;
        _refreshBtn.onClick = OnRefreshBtn;
        checkBoxs.DefauleIndex = 0;
    }

    private void ShowRedPoint(Dictionary<int, List<int>> redlist)
    {
        bool isShow = Singleton<RedPointManager>.Instance.GetChildList(EnumRedPoint.RD_FRIEND, 1);
        if (isShow)
        {
            redPoint.ShowOrHide(true);
        }
        else
        {
            redPoint.ShowOrHide(false);
        }
    }


    public UIFriends()
    {
        Instance = this;
    }
    /// <summary>
    /// 移除好友列表中的引用
    /// </summary>
    public void TemporaryDelet()
    {
        switch (this._index)
        {
            case 1: playerData.GetInstance().friendListData.friendList.RemoveAt(temporaryIndex); break;
            case 2: playerData.GetInstance().friendListData.applyforList.RemoveAt(temporaryIndex); break;
        }
        temporaryIndex = -1;
    }
    //临时存储Item的index数据，方便操作成功之后从列表中移除
    public void TemporaryData(int index)
    {
        temporaryIndex = index;
    }
    /// <summary>
    /// 临时存储数据用于判断是搜索界面发出的还好友申请
    /// </summary>
    /// <param name="isSearch"></param>
    public void TemporaryAddFriend(bool isSearch)
    {
        this.isSearchAdd = isSearch;
    }
    ///// <summary>
    ///// 打开搜索好友界面
    ///// </summary>
    //public void OpenSerachPanle()
    //{
    //    Control.ShowGUI(GameLibrary.UIFriendTip);
    //}
    //刷新好友列表
    private void OnRefreshBtn()
    {
        _replaceIndex++;
        if (_replaceIndex > 2)
        {
            _replaceIndex = 0;
            Singleton<Notification>.Instance.Send(MessageID.common_recommended_friend_list_req);
            return;
        }
        Initdatalist();
    }
    /// <summary>
    /// 搜索好友
    /// </summary>
    private void OnSearch()
    {
        if (!string.IsNullOrEmpty(_searchInput.text))
        {
            long id = 0;
            bool boo = long.TryParse(_searchInput.text, out id);
            if (boo)
            {
                Control.ShowGUI(UIPanleID.UIFriendTip, EnumOpenUIType.DefaultUIOrSecond, false, id);
            }
            else
            {
                Control.ShowGUI(UIPanleID.UITooltips, EnumOpenUIType.DefaultUIOrSecond, false, "请输入正确的玩家ID号");
            }
        }
        else
        {
            Control.ShowGUI(UIPanleID.UITooltips, EnumOpenUIType.DefaultUIOrSecond, false, "输入栏不可为空");
        }
    }

    protected override void OnLoadData()
    {
        base.OnLoadData();
        Singleton<Notification>.Instance.RegistMessageID(MessageID.common_recommended_friend_list_ret, this.GetUIKey());
        Singleton<Notification>.Instance.RegistMessageID(MessageID.common_player_friend_list_ret, this.GetUIKey());
        Singleton<Notification>.Instance.RegistMessageID(MessageID.common_friend_function_listret, this.GetUIKey());
        Singleton<Notification>.Instance.RegistMessageID(MessageID.common_allow_add_friend_ret, this.GetUIKey());
        Singleton<Notification>.Instance.RegistMessageID(MessageID.common_add_friend_ret, this.GetUIKey()); 
        if (this._index == 0) Singleton<Notification>.Instance.Send(MessageID.common_recommended_friend_list_req);
    }
    public override void ReceiveData(UInt32 messageID)
    {

        switch (messageID)
        {
            case MessageID.common_recommended_friend_list_ret:
                break;
            case MessageID.common_player_friend_list_ret:
                
                break;
            case MessageID.common_friend_function_listret:
                
                break;
            case MessageID.common_allow_add_friend_ret:
                TemporaryDelet();

                break;
            case MessageID.common_add_friend_ret:

                break;
            case MessageID.common_delete_friend_ret:
                TemporaryDelet();
                break;
        }
        Show();
        base.ReceiveData(messageID);
    }
    private void OnBackClick()
    {
        Control.HideGUI();
    }
    private void OnCheckClick(int index, bool boo)
    {
        if (boo)
        {
            this._index = index;
            switch (_index)
            {
                case 0:
                    RefreshUI();
                    //推荐好友
                    // ClientSendDataMgr.GetSingle().GetFriendSend().FriendsRecommendRequest();

                    _searchInput.text = "";

                    break;
                case 1:
                    //好友列表
                    //  ClearData();
                    //  RefreshUI();
                    Singleton<Notification>.Instance.Send(MessageID.common_player_friend_list_req);


                    break;
                //case 2://最近联系列表
                //    // ClearData();
                //    // RefreshUI();
                //    ClientSendDataMgr.GetSingle().GetFriendSend().FriendsFunction((int)Friends.DeleteLast);

                //    break;
                case 2:
                    //申请列表
                    // ClearData();
                    //  RefreshUI();
                    Dictionary<string, object> newpacket = new Dictionary<string, object>();
                    newpacket.Add("arg1", (int)Friends.Apply);//types 1申请列表 2黑名单 3仇人列表
                    Singleton<Notification>.Instance.Send(MessageID.common_friend_function_listreq, newpacket);
                    break;
                case 3:
                    //黑名单列表
                    //  RefreshUI();
                    // ClientSendDataMgr.GetSingle().GetFriendSend().FriendsFunction((int)Friends.Blacklist);
                    break;
                    //case 5: //仇人列表
                    //    //  ClearData();
                    //    // RefreshUI();
                    //    ClientSendDataMgr.GetSingle().GetFriendSend().FriendsFunction((int)Friends.DeleteEnemy);
                    //    break;
            }

        }
    }
    //推荐好友列表数据
    private object[] FriendData()
    {
        int index = 0;

        //列表数量个数大于所取区间最大值正常显示，例如数据12g个 区间最大值也是12
        if (playerData.GetInstance().friendListData.RecommendfriendList.Count >= 4 * _replaceIndex + 4)
        {
            object[] data = new object[4];//每次显示4个数据
            for (int i = 4 * _replaceIndex; i <= 4 * _replaceIndex + 3; i++)
            {
                data[index] = playerData.GetInstance().friendListData.RecommendfriendList[i];
                index++;
            }
            return data;
        }
        //列表数量个数大于所取区间最小值，小于区间最大值，取最小值和列表的个数-1，例如数据6g个 区间4-7
        else if (playerData.GetInstance().friendListData.RecommendfriendList.Count >= 4 * _replaceIndex + 1 && playerData.GetInstance().friendListData.RecommendfriendList.Count < 4 * _replaceIndex + 4)
        {
            object[] data = new object[playerData.GetInstance().friendListData.RecommendfriendList.Count - 4 * _replaceIndex];//显示最小区间到friendList.Count个数据
            for (int i = 4 * _replaceIndex; i <= playerData.GetInstance().friendListData.RecommendfriendList.Count - 1; i++)
            {
                data[index] = playerData.GetInstance().friendListData.RecommendfriendList[i];
                index++;
            }
            return data;
        }
        //列表数量个数小于所取区间最小值，取上次数据_replaceIndex-1，例如数据4g个 区间4-7
        else if (playerData.GetInstance().friendListData.RecommendfriendList.Count < 4 * _replaceIndex + 1)
        {
            object[] data = new object[playerData.GetInstance().friendListData.RecommendfriendList.Count - 4 * (_replaceIndex - 1)];//显示上一个最小区间到friendList.Count个数据
            for (int i = 4 * (_replaceIndex - 1); i <= playerData.GetInstance().friendListData.RecommendfriendList.Count - 1; i++)
            {
                data[index] = playerData.GetInstance().friendListData.RecommendfriendList[i];

                index++;
            }
            _replaceIndex -= 1;
            return data;
        }
        return null;
    }
    /// <summary>
    /// 其他列表数据
    /// </summary>
    private object[] OtherData(int index)
    {
        switch (index)
        {
            case 0: return playerData.GetInstance().friendListData.RecommendfriendList.ToArray();
            case 1: return playerData.GetInstance().friendListData.friendList.ToArray();
            case 2: return playerData.GetInstance().friendListData.applyforList.ToArray();
        }
        return null;
    }
    protected override void ShowHandler()
    {
        ShowRedPoint(Singleton<RedPointManager>.Instance.GetRedList());
        RefreshUI();
        Initdatalist();

    }

    void Initdatalist()
    {

        if (isFriendAdd)
        {
            isFriendAdd = false;
        }
        else if (isSearchAdd)
        {
            isSearchAdd = false;
        }
        else
        {
            if (playerData.GetInstance().friendListData.applyforList.Count > 0)
            {
                redPoint.Show();
                Singleton<RedPointManager>.Instance.AddChildFlag(EnumRedPoint.RD_FRIEND, 1);
            }
            else
            {
                redPoint.Hide();
                Singleton<RedPointManager>.Instance.DeletChildFlag(EnumRedPoint.RD_FRIEND, 1);
            }
            switch (_index)
            {
                case 0:
                    CreatList(FriendData()); break;
                case 1: CreatList(OtherData(1)); break;
                case 2: CreatList(OtherData(2)); break;
            }
        }
    }

    private void CreatList(object[] array)
    {
        if (array.Length > 0)
        {
            multList[_index].InSize(array.Length, 1);
            multList[_index].Info(array);
        }
        else
        {
            multList[_index].InSize(0, 1);
            multList[_index].Info(null);
        }
        multList[_index].ScrollView = view;
    }
    void ClearData()
    {
        playerData.GetInstance().friendListData.friendList.Clear();
    }

    void RefreshUI()
    {
        for (int i = 0; i < multList.Length; i++)
        {
            if (i != _index)
            {
                multList[i].gameObject.SetActive(false);
            }
        }
        if (!multList[_index].gameObject.activeInHierarchy)
        {
            multList[_index].gameObject.SetActive(true);
        }
        if (_index == 0)
        {
            checkBoxs.setMaskState(_index);
            _searchHeroPanel.gameObject.SetActive(true);
        }
        else
        {
            _searchHeroPanel.gameObject.SetActive(false);
        }
    }
}

