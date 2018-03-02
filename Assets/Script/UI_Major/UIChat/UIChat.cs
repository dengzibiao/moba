using UnityEngine;
using System.Collections;
using System;

public class UIChat : GUIBase {

    public GUISingleButton chatBtn;
    public GUISingleButton bgBtn;
    public UILabel channelLabel;
    public UILabel contentLabel;
    //public UILabel nickNameLabel;
    private UISprite sprite;    

    public float speakRateLimit = 5;//世界频道发言间隔5秒
    public float speakTimer = 5;//世界频道发言计时器
    public bool isSpeakTime = true;//每次登陆默认可以发言

    public Transform chatEffect;
    public Transform bg;


    private static UIChat instance;
    public static UIChat Instance { get { return instance; } set { instance = value; } }
    public UIChat()
    {
        Instance = this;
    }
    public void SetChatPosition(bool isShowConetent)
    {
        if (isShowConetent)
        {
            bg.gameObject.SetActive(true);
            channelLabel.gameObject.SetActive(true);
            contentLabel.gameObject.SetActive(true);
        }
        else
        {
            bg.gameObject.SetActive(false);
            channelLabel.gameObject.SetActive(false);
            contentLabel.gameObject.SetActive(false);
        }
    }

    public override UIPanleID GetUIKey()
    {
        return UIPanleID.UIChat;
    }

    public override void ReceiveData(uint messageID)
    {
        base.ReceiveData(messageID);
        switch (messageID)
        {
            //case MessageID.common_offer_reward_mission_list_ret:
              //  Show(); break;
        }
    }

    protected override void OnLoadData()
    {
        base.OnLoadData();
        //Singleton<Notification>.Instance.RegistMessageID(MessageID.common_offer_reward_mission_list_ret, this.GetUIKey());
        Show();
    }
    protected override void Init()
    {
        base.Init();
        instance = this;
        bg = transform.Find("BgBtn");
        chatEffect = transform.Find("ChatBtn/UI_XiaoXi_TiXing_01");
        chatBtn = transform.Find("ChatBtn").GetComponent<GUISingleButton>();
        channelLabel = transform.Find("Channel").GetComponent<UILabel>();
        contentLabel = transform.Find("Content").GetComponent<UILabel>();
        //nickNameLabel = transform.Find("NickName").GetComponent<UILabel>();
        sprite = transform.Find("Sprite").GetComponent<UISprite>();
        speakRateLimit = 3;
        speakTimer = 3;
        chatBtn.onClick = OnChatBtnClick;
        bgBtn.onClick = OnChatBtnClick;
        playerData.GetInstance().NewChatHint += SetNewChatHint;
        SetNewChatHint(null);
        chatEffect.gameObject.SetActive(GetNotReadCount() > 0 ? true : false);
    }
    void Update()
    {
        //不可发言的时候才倒计时
        if (!isSpeakTime)
        {
            speakTimer -= Time.deltaTime;
            if (speakTimer <= 0)
            {
                speakTimer = speakRateLimit;
                isSpeakTime = true;
            }
        }
    }
    
    public void HideNewChatEffect()
    {
        chatEffect.gameObject.SetActive(false);
    }
    private int GetNotReadCount()
    {
        return Globe.worldChatUnReadCount + Globe.societyChatUnReadCount + Globe.privateChatUnReadCount;
    }
    public void SetNewChatHint(ChatData chatData)
    {
        chatBtn = transform.Find("ChatBtn").GetComponent<GUISingleButton>();
        channelLabel = transform.Find("Channel").GetComponent<UILabel>();
        contentLabel = transform.Find("Content").GetComponent<UILabel>();
        //nickNameLabel = transform.Find("NickName").GetComponent<UILabel>();
        sprite = transform.Find("Sprite").GetComponent<UISprite>();
        if (chatData == null)
        {
            //channelLabel.gameObject.SetActive(false);
            //contentLabel.gameObject.SetActive(false);
            //sprite.gameObject.SetActive(false);
            SetChatPosition(false);
            //chatEffect.gameObject.SetActive(false);
            return;
        }
        else if(chatData!=null)
        {
            //channelLabel.gameObject.SetActive(true);
            //contentLabel.gameObject.SetActive(true);
            SetChatPosition(UI_Setting.GetInstance().isShrink);
            //chatEffect.gameObject.SetActive(true);

        }
        chatEffect.gameObject.SetActive(GetNotReadCount()>0?true:false);
        switch (chatData.Type)
        {
            case ChatType.WorldChat:
                channelLabel.text = "[fdee03]" + "【世界】" + "[-]";
                break;
            case ChatType.SocietyChat:
                channelLabel.text = "[0dd0f7]" + "【公会】" + "[-]";
                break;
            case ChatType.PrivateChat:
                channelLabel.text = "[db4bff]" + "【私聊】" + "[-]";
                break;
            case ChatType.NearbyChat:
                channelLabel.text = "附近";
                break;
            case ChatType.TroopsChat:
                channelLabel.text = "队伍";
                break;
            case ChatType.SystemChat:
                channelLabel.text = "系统";
                break;
        }
        //nickNameLabel.text = chatData.NickName;
        contentLabel.text = chatData.NickName + ":" + "[26c926]" + chatData.ChatContent + "[-]";
    }
    //聊天按钮
    private void OnChatBtnClick()
    {
        //Control.ShowGUI(GameLibrary.UIChatPanel);
        Control.ShowGUI(UIPanleID.UIChatPanel, EnumOpenUIType.DefaultUIOrSecond);
    }
}
