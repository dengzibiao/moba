using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;

public class UIChatPanel : GUIBase
{
    private long nowtime;
    private GUISingleButton backBtn;//关闭界面按钮 
    private GUISingleButton sendBtn;//发送消息按钮
    public GUISingleCheckBoxGroup checkBoxs;//页签
    public GUISingleMultList chatMultList;
    private UIScrollView chatScrollView;
    private UIScrollBar scrollBar;
    private UIInput inputContent;//聊天输入框
    private GameObject cannotSpeakObj;
    private UILabel cannotSpeakLabel;
    private GameObject worldHitObj;//世界新消息提示点
    private GameObject societyHitObj;//公会新消息提示点
    private GameObject privateHitObj;//私聊新消息提示点
    private GameObject nearbyHitObj;//附近新消息提示点
    private GameObject troopsHitObj;//队伍新消息提示点
    private GameObject systemHitObj;//系统
    public GameObject maskObj;
    private UILabel horbCoutLabel;//喇叭数量
    private UISprite horbSprite;//喇叭图标
    public object[] worldChatObjs;//系统
    public object[] societyChatObjs;//公会
    public object[] privateChatObjs;//私聊
    public object[] nearbyChatObjs;//附近
    public object[] troopsChatObjs;//队伍
    public object[] systemChatObjs;//系统

    private int worldSpeakTimesLimit = 10;//世界频道发言次数上限
    private int worldSpeakTimes = 0;//当天世界频道发言次数
    private int clearTime = 600;//清理时间600秒

    public static int horbCount = 10;//当前喇叭数量
    int needDiamondCount = 0;//一个喇叭需要的钻石数
    int buyhorbCount = 10;//一次购买10个喇叭
    int chatContCount = 20;//聊天保存条数
    private static UIChatPanel instance;
    public static UIChatPanel Instance { get { return instance; } set { instance = value; } }
    public UIChatPanel()
    {
        instance = this;
    }

    public void Update()
    {
       
    }
    public override UIPanleID GetUIKey()
    {
        return UIPanleID.UIChatPanel;
    }

    protected override void Init()
    {
        base.Init();
        backBtn = transform.Find("BackBtn").GetComponent<GUISingleButton>();
        sendBtn = transform.Find("Button").GetComponent<GUISingleButton>();
        checkBoxs = transform.Find("CheckBoxs").GetComponent<GUISingleCheckBoxGroup>();
        chatMultList = transform.Find("ChatScrollView/ChatMultList").GetComponent<GUISingleMultList>();
        chatScrollView = transform.Find("ChatScrollView").GetComponent<UIScrollView>();
        scrollBar = transform.Find("ScrollBar").GetComponent<UIScrollBar>();
        inputContent = transform.Find("Input").GetComponent<UIInput>();
        cannotSpeakObj = transform.Find("CannotSpeakPrompt").gameObject;
        cannotSpeakLabel = transform.Find("CannotSpeakPrompt/Label").GetComponent<UILabel>();
        worldHitObj = transform.Find("Hit/worldHit").gameObject;
        societyHitObj = transform.Find("Hit/societyHit").gameObject;
        privateHitObj = transform.Find("Hit/privateHit").gameObject;
        nearbyHitObj = transform.Find("Hit/nearbyHit").gameObject;
        troopsHitObj = transform.Find("Hit/troopsHit").gameObject;
        systemHitObj = transform.Find("Hit/systemHit").gameObject;
        maskObj = transform.Find("Mask").gameObject;
        horbCoutLabel = transform.Find("HorbCount").GetComponent<UILabel>();
        horbSprite = transform.Find("HorbSprite").GetComponent<UISprite>();
        backBtn.onClick = OnBackBtnClick;
        checkBoxs.onClick = OnChatChannelClick;
        sendBtn.onClick = OnSendChatClick;
        UIEventListener.Get(maskObj).onClick += OnCloseClick;
        InitChatInfoList();
        JudgePlayerIsCanSpeak();
        //checkBoxs.DefauleIndex = 0;
        //OnChatChannelClick(0, true);
        UIEventListener.Get(inputContent.gameObject).onClick = OnInputClick;
#if UNITY_EDITOR


#elif UNITY_ANDROID
        EventDelegate.Add(inputContent.onSubmit,OnSunmit);
#endif
        //EventDelegate.Add(inputContent.onChange, OnChange);
    }
    /// <summary>
    /// input文字提交事件
    /// </summary>
    private void OnSunmit()
    {
        if (Globe.selectChatChannel == ChatType.PrivateChat)
        {
            
            if (string.IsNullOrEmpty(inputContent.value.Trim()))
            {
                inputContent.value = "正在和" + "[" + Globe.chatingPlayerNickName + "]聊天";
            }
        }
    }
    /// <summary>
    /// input的点击事件
    /// </summary>
    /// <param name="go"></param>
    private void OnInputClick(GameObject go)
    {
        inputContent.value = "";
    }

    private void OnCloseClick(GameObject go)
    {
        Control.HideGUI(this.GetUIKey());
        //Hide();
    }

    /// <summary>
    /// input改变事件
    /// </summary>
    private void OnChange()
    {
        if (Globe.selectChatChannel == ChatType.PrivateChat)
        {
            if (string.IsNullOrEmpty(inputContent.value.Trim()))
            {
                inputContent.value = "正在和" + "[" + Globe.chatingPlayerNickName + "]聊天";
            }
        }
    }


    /// <summary>
    /// 判断当前玩家是否满足发言条件 等级orVIP
    /// </summary>
    private void JudgePlayerIsCanSpeak()
    {
        if (playerData.GetInstance().selfData.level >= 1)
        {
            Globe.playerCanSpeak = true;
            //cannotSpeakObj.SetActive(false);
        }
        else
        {
            Globe.playerCanSpeak = false;

            //cannotSpeakObj.SetActive(true);
            //cannotSpeakLabel.text = "玩家不满足发言条件";
        }
        //如果玩家不满足发言条件 隐藏发言框
        sendBtn.gameObject.SetActive(Globe.playerCanSpeak);
        inputContent.gameObject.SetActive(Globe.playerCanSpeak);
    }
    private void BuyHorbCountEvent()
    {
        //购买喇叭
        ClientSendDataMgr.GetSingle().GetBattleSend().SendBuySomeone(113000100, buyhorbCount);
    }
    private void OnSendChatClick()
    {
        //世界频道发言需要消耗喇叭 所以要判断喇叭个数
        if (Globe.selectChatChannel == ChatType.WorldChat)
        {
            //判断一下是否超过发言次数
            if (horbCount <= 0)
            {
                Debug.Log("请购买喇叭");
                //UIChat.Instance.isSpeakTime = false;
                //UIPromptBox.Instance.ShowLabel("小喇叭不足，请购买小喇叭");
                ItemNodeState item = GameLibrary.Instance().ItemStateList[110000100];
                if (item.cprice.Length>2)
                {
                    needDiamondCount = item.cprice[1];
                }
                string str1 = "花费"+buyhorbCount* needDiamondCount +"钻石购买"+ buyhorbCount + "个喇叭";
                string str2 = "";
                object[] obj = new object[5] { str1, str2, UIPopupType.EnSure, this.gameObject, "BuyHorbCountEvent" };
                Control.ShowGUI(UIPanleID.UIPopUp, EnumOpenUIType.DefaultUIOrSecond, false, obj);
                return;
            }
        }
        //发言是否过快判断
        if (!UIChat.Instance.isSpeakTime)// && Globe.selectChatChannel == ChatType.WorldChat
        {
            playerData.GetInstance().AddChatInfo(ChatContentType.NotSpeakFast,"");
            //AddChatInfo(ChatContentType.NotSpeakFast);
            UIChat.Instance.isSpeakTime = false;
            return;
        }
        ////世界频道发言次数判断
        //if (Globe.selectChatChannel == ChatType.WorldChat)
        //{
        //    //判断一下是否超过发言次数
        //    if (worldSpeakTimes >= worldSpeakTimesLimit)
        //    {
        //        playerData.GetInstance().AddChatInfo(ChatContentType.SpeakTimesLimit,"");
        //        //AddChatInfo(ChatContentType.SpeakTimesLimit);
        //        UIChat.Instance.isSpeakTime = false;
        //        return;
        //    }
        //}
        
        if (Globe.selectChatChannel == ChatType.PrivateChat && Regex.IsMatch(inputContent.value.Trim(), @"^正在和\[[\u4e00-\u9fa5a-zA-Z0-9\.~]+\]聊天$"))
        {
            //私聊频道发言不能为空
            playerData.GetInstance().AddChatInfo(ChatContentType.NoCharacter,"");
            //AddChatInfo(ChatContentType.NoCharacter);
            UIChat.Instance.isSpeakTime = false;
            return;
        }
        if (inputContent.value.Trim() == "")
        {
            //发言不能为空
            playerData.GetInstance().AddChatInfo(ChatContentType.NoCharacter,"");
            //AddChatInfo(ChatContentType.NoCharacter);
            UIChat.Instance.isSpeakTime = false;
            return;
        }
        if (UIChat.Instance.isSpeakTime && inputContent.value.Trim() != "" )//&& Globe.selectChatChannel == ChatType.WorldChat
        {
            //发言成功一次 设置为false
            UIChat.Instance.isSpeakTime = false;
            //世界频道发言一次 世界频道发言次数+1
            if (Globe.selectChatChannel == ChatType.WorldChat)
            {
                worldSpeakTimes++;
            }
            //世界频道发言一次 喇叭数量-1
            if (Globe.selectChatChannel == ChatType.WorldChat)
            {
                RefreshHorbCount();
            }
           
        }

        //公会队伍附近测试
        if (Globe.selectChatChannel == ChatType.NearbyChat)
        {
            Globe.nearbyPlayerIdList.Clear();
            Globe.nearbyPlayerIdList.Add(playerData.GetInstance().selfData.playerId);//附近需要把自己id也添加进去  附近列表不能为空
        }
        //if (Globe.selectChatChannel == ChatType.TroopsChat)
        //{
        //    Globe.troopsPlayerIdList.Clear();
        //    //队伍需要把自己id添加进去  队伍列表不能为空
        //    Globe.troopsPlayerIdList.Add(playerData.GetInstance().selfData.playerId);
        //}
        //if (Globe.selectChatChannel == ChatType.SocietyChat)
        //{
        //    Globe.societyPlayerIdList.Clear();
        //    Globe.societyPlayerIdList.Add(1049559);
        //}
        ClientSendDataMgr.GetSingle().GetChatSend().SendChatInfo(inputContent.value,Globe.selectChatChannel,C2SMessageType.ActiveWait);
        if (Globe.selectChatChannel == ChatType.PrivateChat)
        {
            SaveSelfChatData(inputContent.value, Globe.selectChatChannel);
        }
        if (Globe.selectChatChannel == ChatType.PrivateChat)
        {
            inputContent.value = "正在和" + "[" + Globe.chatingPlayerNickName + "]聊天";
        }
        else
        {
            inputContent.value = "";
        }
        //AddChatInfo(ChatContentType.TextContent);
    }
    //保存自己发送的消息
    void SaveSelfChatData(string content, ChatType chattype)
    {
        ChatData chatData = new ChatData();
        chatData.Id = playerData.GetInstance().selfData.playerId;
        chatData.AccountId = playerData.GetInstance().selfData.accountId;
        chatData.HeadId = GameLibrary.player;
        chatData.Vip = playerData.GetInstance().selfData.vip;
        if (chatData.Vip > 99)
        {
            chatData.Vip = 1;//vip等级超过三位设置为1
        }
        chatData.NickName = playerData.GetInstance().selfData.playeName;
        chatData.ChatContent = content; //聊天内容
        chatData.SpeakingTime = Auxiliary.GetNowTime();
        chatData.Time = Convert.ToDateTime(PropertyManager.ConvertIntDateTime(chatData.SpeakingTime)).ToString("HH:mm");
        if (chatData.Id == playerData.GetInstance().selfData.playerId)
        {
            chatData.IsLocalPlayer = true;
        }
        else
        {
            chatData.IsLocalPlayer = false;
        }
        chatData.ContentType = ChatContentType.TextContent;
        chatData.Type = chattype;
        SocietyManager.Single().selfChatData = chatData;
    }
    /// <summary>
    /// 新消息红点提示状态设置
    /// </summary>
    void SetHitState()
    {
        if (ChatType.WorldChat == Globe.selectChatChannel)
        {
            Globe.worldChatUnReadCount = 0;
            worldHitObj.SetActive(false);
        }
        else if (Globe.worldChatUnReadCount > 0)
        {
            worldHitObj.SetActive(true);
        }
        else
        {
            worldHitObj.SetActive(false);
        }

        if (ChatType.SocietyChat == Globe.selectChatChannel)
        {
            Globe.societyChatUnReadCount = 0;
            societyHitObj.SetActive(false);
        }
        else if (Globe.societyChatUnReadCount > 0)
        {
            societyHitObj.SetActive(true);
        }
        else
        {
            societyHitObj.SetActive(false);
        }
        if (ChatType.PrivateChat == Globe.selectChatChannel)
        {
            Globe.privateChatUnReadCount = 0;
            privateHitObj.SetActive(false);
        }
        else if (Globe.privateChatUnReadCount > 0)
        {
            privateHitObj.SetActive(true);
        }
        else
        {
            privateHitObj.SetActive(false);
        }

        if (ChatType.NearbyChat == Globe.selectChatChannel)
        {
            Globe.nearbyChatUnReadCount = 0;
            nearbyHitObj.SetActive(false);
        }
        else if (Globe.nearbyChatUnReadCount > 0)
        {
            nearbyHitObj.SetActive(true);
        }
        else
        {
            nearbyHitObj.SetActive(false);
        }

        if (ChatType.TroopsChat == Globe.selectChatChannel)
        {
            Globe.troopsChatUnReadCount = 0;
            troopsHitObj.SetActive(false);
        }
        else if (Globe.troopsChatUnReadCount > 0)
        {
            troopsHitObj.SetActive(true);
        }
        else
        {
            troopsHitObj.SetActive(false);
        }

        if (ChatType.SystemChat == Globe.selectChatChannel)
        {
            Globe.systemChatUnReadCount = 0;
            systemHitObj.SetActive(false);
        }
        else if (Globe.systemChatUnReadCount > 0)
        {
            systemHitObj.SetActive(true);
        }
        else
        {
            systemHitObj.SetActive(false);
        }
    }

    public void OnChatChannelClick(int index, bool boo)
    {
        if (boo)
        {
            //系统频道 隐藏发送框
            if (index == 5)
            {
                sendBtn.gameObject.SetActive(false);
                inputContent.gameObject.SetActive(false);
                //cannotSpeakObj.SetActive(true);
                //cannotSpeakLabel.text = "当前频道不能发言";
            }
            else
            {
                sendBtn.gameObject.SetActive(Globe.playerCanSpeak);
                inputContent.gameObject.SetActive(Globe.playerCanSpeak);
            }
            cannotSpeakObj.SetActive(false);
            chatScrollView.gameObject.SetActive(true);
            checkBoxs.transform.GetChild(index).Find("Label").GetComponent<UILabel>().color = Color.white;
            switch (index)
            {
                case 0:
                    Globe.selectChatChannel = ChatType.WorldChat;
                    chatMultList.InSize(worldChatObjs.Length,1);
                    chatMultList.Info(worldChatObjs);
                    StartCoroutine(AutoScrollview());
                    Globe.worldChatUnReadCount = 0;
                    //scrollBar.value = 1;
                    //chatScrollView.ResetPosition();
                    break;
                case 1:
                    Globe.selectChatChannel = ChatType.SocietyChat;
                    chatMultList.InSize(societyChatObjs.Length,1);
                    chatMultList.Info(societyChatObjs);
                    StartCoroutine(AutoScrollview());
                    Globe.societyChatUnReadCount = 0;
                    //scrollBar.value = 1;
                    //chatScrollView.ResetPosition();
                    break;
                case 2:
                    Globe.selectChatChannel = ChatType.PrivateChat;
                    chatMultList.InSize(privateChatObjs.Length, 1);
                    chatMultList.Info(privateChatObjs);
                    StartCoroutine(AutoScrollview());
                    Globe.privateChatUnReadCount = 0;
                    //inputContent.value
                    //scrollBar.value = 1;
                    //chatScrollView.ResetPosition();
                    break;
                case 3:
                    Globe.selectChatChannel = ChatType.NearbyChat;
                    chatMultList.InSize(nearbyChatObjs.Length, 1);
                    chatMultList.Info(nearbyChatObjs);
                    StartCoroutine(AutoScrollview());
                    Globe.nearbyChatUnReadCount = 0;
                    //scrollBar.value = 1;
                    //chatScrollView.ResetPosition();
                    break;
                case 4:
                    Globe.selectChatChannel = ChatType.TroopsChat;
                    chatMultList.InSize(troopsChatObjs.Length, 1);
                    chatMultList.Info(troopsChatObjs);
                    StartCoroutine(AutoScrollview());
                    Globe.troopsChatUnReadCount = 0;
                    //scrollBar.value = 1;
                    //chatScrollView.ResetPosition();
                    break;
                case 5:
                    Globe.selectChatChannel = ChatType.SystemChat;
                    chatMultList.InSize(systemChatObjs.Length, 1);
                    chatMultList.Info(systemChatObjs);
                    StartCoroutine(AutoScrollview());
                    Globe.systemChatUnReadCount = 0;
                    //scrollBar.value = 1;
                    //chatScrollView.ResetPosition();
                    break;
                default:
                    break;
            }
            SetHitState();
            RefreshHorbCount();
            if (!Globe.playerCanSpeak)
            {
                playerData.GetInstance().AddChatInfo(ChatContentType.NotSpeak,"");
                //AddChatInfo(ChatContentType.NotSpeak);
            }
            if(Globe.playerCanSpeak)
            {
                inputContent.value = "";
                if (Globe.selectChatChannel == ChatType.TroopsChat)
                {
                    if (!Globe.isHaveTeam)
                    {
                        sendBtn.gameObject.SetActive(Globe.isHaveTeam);
                        inputContent.gameObject.SetActive(Globe.isHaveTeam);
                        cannotSpeakObj.SetActive(true);
                        cannotSpeakLabel.text = "组队功能未开放";
                        //playerData.GetInstance().AddChatInfo(ChatContentType.NotTeam,"");
                        //AddChatInfo(ChatContentType.NotTeam);
                    }
                }
                else if (Globe.selectChatChannel == ChatType.SocietyChat)
                {
                    if (!Globe.isHaveSociety)
                    {
                        sendBtn.gameObject.SetActive(Globe.isHaveSociety);
                        inputContent.gameObject.SetActive(Globe.isHaveSociety);
                        chatScrollView.gameObject.SetActive(false);
                        cannotSpeakObj.SetActive(true);
                        cannotSpeakLabel.text = "未加入公会";
                        //playerData.GetInstance().AddChatInfo(ChatContentType.NotSociety,"");
                        //AddChatInfo(ChatContentType.NotSociety);
                    }
                }
                else if(Globe.selectChatChannel == ChatType.PrivateChat)
                {
                    if (!Globe.isHavePrivateTarget)
                    {
                        sendBtn.gameObject.SetActive(Globe.isHavePrivateTarget);
                        inputContent.gameObject.SetActive(Globe.isHavePrivateTarget);
                        cannotSpeakObj.SetActive(true);
                        cannotSpeakLabel.text = "没选择私聊对象";
                        //playerData.GetInstance().AddChatInfo(ChatContentType.NotPrivateTarget,"");
                        //AddChatInfo(ChatContentType.NotPrivateTarget);
                    }
                    else
                    {
                        inputContent.value = "正在和" + "[" + Globe.chatingPlayerNickName + "]聊天";
                    }
                }

            }
        }
    }
    /// <summary>
    /// 刷新喇叭数量
    /// </summary>
    public void RefreshHorbCount()
    {
        horbCount = GoodsDataOperation.GetInstance().GetItemCountById(113000100);
        if (Globe.selectChatChannel == ChatType.WorldChat)
        {
            horbCoutLabel.gameObject.SetActive(true);
            horbSprite.gameObject.SetActive(true);
            horbCoutLabel.text =  "x"+horbCount;
        }
        else
        {
            horbSprite.gameObject.SetActive(false);
            horbCoutLabel.gameObject.SetActive(false);
        }
    }
    /// <summary>
    /// 刷新界面显示
    /// </summary>
    public void RefreshChatShow()
    {
        
        switch (Globe.selectChatChannel)
        {
            case ChatType.WorldChat:
                chatMultList.InSize(worldChatObjs.Length, 1);
                chatMultList.Info(worldChatObjs);
                StartCoroutine(AutoScrollview());
                //scrollBar.value = 1;
                //chatScrollView.ResetPosition();
                break;
            case ChatType.SocietyChat:
                chatMultList.InSize(societyChatObjs.Length, 1);
                chatMultList.Info(societyChatObjs);
                StartCoroutine(AutoScrollview());
                //scrollBar.value = 1;
                //chatScrollView.ResetPosition();
                break;
            case ChatType.PrivateChat:
                chatMultList.InSize(privateChatObjs.Length, 1);
                chatMultList.Info(privateChatObjs);
                StartCoroutine(AutoScrollview());

                //inputContent.value
                //scrollBar.value = 1;
                //chatScrollView.ResetPosition();
                break;
            case ChatType.NearbyChat:
                chatMultList.InSize(nearbyChatObjs.Length, 1);
                chatMultList.Info(nearbyChatObjs);
                StartCoroutine(AutoScrollview());
                //scrollBar.value = 1;
                //chatScrollView.ResetPosition();
                break;
            case ChatType.TroopsChat:
                chatMultList.InSize(troopsChatObjs.Length, 1);
                chatMultList.Info(troopsChatObjs);
                StartCoroutine(AutoScrollview());
                //scrollBar.value = 1;
                //chatScrollView.ResetPosition();
                break;
            case ChatType.SystemChat:
                chatMultList.InSize(systemChatObjs.Length, 1);
                chatMultList.Info(systemChatObjs);
                StartCoroutine(AutoScrollview());
                //scrollBar.value = 1;
                //chatScrollView.ResetPosition();
                break;
            default:
                break;
        }
        SetHitState();

    }
    IEnumerator AutoScrollview()
    {
        yield return new WaitForSeconds(0.1f);
        //scrollBar.value = 1;
        chatScrollView.ResetPosition();
        yield return 0;
    }
    private void OnBackBtnClick()
    {
        Control.HideGUI(this.GetUIKey());
        //Hide();
        //关闭聊天界面计算未读的数量 如果为0就隐藏聊天闪烁特效
        if (GetNotReadCount() == 0)
        {
            UIChat.Instance.HideNewChatEffect();
        }
    }
    private int GetNotReadCount()
    {
        return Globe.worldChatUnReadCount + Globe.societyChatUnReadCount + Globe.privateChatUnReadCount;
    }
    protected override void SetUI(params object[] uiParams)
    {
        if (uiParams.Length>0)
        {
            switch ((ChatType)uiParams[0])
            {
                case ChatType.WorldChat:
                    break;
                case ChatType.PrivateChat:
                    Globe.selectChatChannel = (ChatType)uiParams[0];
                    Globe.privateChatPlayerId = (long)uiParams[1];
                    Globe.privateChatPlayerAId = (long)uiParams[2];
                    Globe.chatingPlayerNickName = (string)uiParams[3];
                    Globe.isHavePrivateTarget = true;
                    break;
                case ChatType.SocietyChat:
                    Globe.selectChatChannel = ChatType.SocietyChat;
                    break;
            }
        }
        base.SetUI();
    }
    protected override void OnLoadData()
    {
        base.OnLoadData();
        Singleton<Notification>.Instance.RegistMessageID(MessageID.common_update_item_list_ret, UIPanleID.UIChatPanel);
        Singleton<Notification>.Instance.RegistMessageID(MessageID.common_server_chat_msg_notify, UIPanleID.UIChatPanel);
        Show();
    }
    public override void ReceiveData(UInt32 messageID)
    {
        switch (messageID)
        {
            case MessageID.common_update_item_list_ret:
                RefreshHorbCount();
                break;
            case MessageID.common_server_chat_msg_notify:
                InitChatInfoList();
                RefreshChatShow();
                break;
        }
        base.ReceiveData(messageID);
    }
    protected override void ShowHandler()
    {
        base.ShowHandler();
        nowtime = Auxiliary.GetNowTime();
        scrollBar.value = 1;
        ClearListData();
        InitChatInfoList();
        
        //OnChatChannelClick((int)(Globe.selectChatChannel) - 1, true);
        //打开聊天面板只需判断一次，因为在聊天面板内，玩家等级和vip等级是不会变的
        List<ItemData> list = GoodsDataOperation.GetInstance().GetItemListByItmeType(ItemType.Horn);
        if (list.Count > 0)
        {
            horbCount = list[0].Count;
        }
        else
        {
            horbCount = 10;
        }
        RefreshChatShow();
        RefreshHorbCount();
        JudgePlayerIsCanSpeak();
        checkBoxs.DefauleIndex = (int)(Globe.selectChatChannel) - 1;
    }

    public void InitChatInfoList()
    {
        worldChatObjs = playerData.GetInstance().iChat.worldChatList.ToArray();
        societyChatObjs = playerData.GetInstance().iChat.societyChatList.ToArray();
        privateChatObjs = playerData.GetInstance().iChat.privateChatList.ToArray();
        nearbyChatObjs = playerData.GetInstance().iChat.nearbyChatList.ToArray();
        troopsChatObjs = playerData.GetInstance().iChat.troopsChatList.ToArray();
        systemChatObjs = playerData.GetInstance().iChat.systemChatList.ToArray();
    }
    /// <summary>
    /// 聊天信息清理机制 10分钟以前的消息清除 当链表数量大于100条清理
    /// </summary>
    public void ClearListData()
    {
        //删除发送时间超过10分钟的消息
        //for (int i = playerData.GetInstance().iChat.worldChatList.Count - 1; i >= 0; i--)
        //{
        //    if (nowtime - playerData.GetInstance().iChat.worldChatList[i].SpeakingTime > clearTime)
        //    {
        //        playerData.GetInstance().iChat.worldChatList.RemoveAt(i);
        //    }
        //}
        //for (int i = playerData.GetInstance().iChat.societyChatList.Count - 1; i >= 0; i--)
        //{
        //    if (nowtime - playerData.GetInstance().iChat.societyChatList[i].SpeakingTime > clearTime)
        //    {
        //        playerData.GetInstance().iChat.societyChatList.RemoveAt(i);
        //    }
        //}
        //for (int i = playerData.GetInstance().iChat.privateChatList.Count - 1; i >= 0; i--)
        //{
        //    if (nowtime - playerData.GetInstance().iChat.privateChatList[i].SpeakingTime > clearTime)
        //    {
        //        playerData.GetInstance().iChat.privateChatList.RemoveAt(i);
        //    }
        //}
        //for (int i = playerData.GetInstance().iChat.nearbyChatList.Count - 1; i >= 0; i--)
        //{
        //    if (nowtime - playerData.GetInstance().iChat.nearbyChatList[i].SpeakingTime > clearTime)
        //    {
        //        playerData.GetInstance().iChat.nearbyChatList.RemoveAt(i);
        //    }
        //}
        //for (int i = playerData.GetInstance().iChat.troopsChatList.Count - 1; i >= 0; i--)
        //{
        //    if (nowtime - playerData.GetInstance().iChat.troopsChatList[i].SpeakingTime > clearTime)
        //    {
        //        playerData.GetInstance().iChat.troopsChatList.RemoveAt(i);
        //    }
        //}
        //for (int i = playerData.GetInstance().iChat.systemChatList.Count - 1; i >= 0; i--)
        //{
        //    if (nowtime - playerData.GetInstance().iChat.systemChatList[i].SpeakingTime > clearTime)
        //    {
        //        playerData.GetInstance().iChat.systemChatList.RemoveAt(i);
        //    }
        //}
        //  如果聊天数量大于100条 只保留后100条
        if (playerData.GetInstance().iChat.worldChatList.Count > chatContCount)
        {
            playerData.GetInstance().iChat.worldChatList.RemoveRange(0, playerData.GetInstance().iChat.worldChatList.Count- chatContCount);
        }
        if (playerData.GetInstance().iChat.societyChatList.Count > chatContCount)
        {
            playerData.GetInstance().iChat.societyChatList.RemoveRange(0, playerData.GetInstance().iChat.societyChatList.Count - chatContCount);
        }
        if (playerData.GetInstance().iChat.privateChatList.Count > chatContCount)
        {
            playerData.GetInstance().iChat.privateChatList.RemoveRange(0, playerData.GetInstance().iChat.privateChatList.Count - chatContCount);
        }
        if (playerData.GetInstance().iChat.nearbyChatList.Count > chatContCount)
        {
            playerData.GetInstance().iChat.nearbyChatList.RemoveRange(0, playerData.GetInstance().iChat.nearbyChatList.Count - chatContCount);
        }
        if (playerData.GetInstance().iChat.troopsChatList.Count > chatContCount)
        {
            playerData.GetInstance().iChat.troopsChatList.RemoveRange(0, playerData.GetInstance().iChat.troopsChatList.Count - chatContCount);
        }
        if (playerData.GetInstance().iChat.systemChatList.Count > chatContCount)
        {
            playerData.GetInstance().iChat.systemChatList.RemoveRange(0, playerData.GetInstance().iChat.systemChatList.Count - chatContCount);
        }
    }
    /// <summary>
    /// 外部打开私聊频道
    /// </summary>
    /// <param name="id">私聊对象id</param>
    /// <param name="nickname">私聊对象昵称</param>
    public void ExternalOpenPrivateChat(long id,long Aid,string nickname)
    {
        Globe.privateChatPlayerId = id;
        Globe.privateChatPlayerAId = Aid;
        Globe.chatingPlayerNickName = nickname;
        Globe.isHavePrivateTarget = true;
        Globe.selectChatChannel = ChatType.PrivateChat;

        //Control.ShowGUI(UIPanleID.UIChatPanel);
        
        //checkBoxs.setMaskState((int)(ChatType.PrivateChat) - 1);
        
    }
    /// <summary>
    /// 外部打开公会频道
    /// </summary>
    public void ExternalOpenSocietyChat()
    {
        Globe.selectChatChannel = ChatType.SocietyChat;

        //Control.ShowGUI(UIPanleID.UIChatPanel);
    }
}
