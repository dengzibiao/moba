using UnityEngine;
using System.Collections;
using System;
using System.Text;
using Tianyu;

public class ChatItem : GUISingleItemList
{
    public int rowFontCount = 26;//每行限定汉字数

    #region 其他玩家变量
    private GUISingleButton iconBtn;//图标
    private UILabel channelLabel;//频道文字
    private UILabel playerName;//名字
    private UILabel vipLabel;//vip
    private UILabel content;//内容
    private UISprite qiPao;//气泡
    private UISprite iconBorder;//图像框
    private GameObject vipObj;//vip对象
    private GameObject contentObj;//内容对象
    private GameObject channelObj;//频道对象
    private UILabel sendTime;//聊天发送时间
    #endregion


    #region 自己变量
    private GUISingleButton mIconBtn;//图标
    private UILabel mChannelLabel;//频道文字
    private UILabel mPlayerName;//名字
    private UILabel mVipLabel;//vip
    private UILabel mContent;//内容
    private UISprite mIconBorder;//图像框
    private UISprite mQiPao;//气泡
    private GameObject mVipObj;//vip对象
    private GameObject mContentObj;//内容对象
    private GameObject mChannelObj;//频道对象
    private UILabel mSendTime;//聊天发送时间
    #endregion


    private ChatData chatData;
    protected override void InitItem()
    {
        base.InitItem();
        iconBorder = transform.Find("IconBorder").GetComponent<UISprite>();
        iconBtn = transform.Find("Icon").GetComponent<GUISingleButton>();
        channelLabel = transform.Find("Sprite/ChannelLabel").GetComponent<UILabel>();
        channelObj = transform.Find("Sprite").gameObject;
        vipObj = transform.Find("vipbg").gameObject;
        vipLabel = transform.Find("vipbg/Vip").GetComponent<UILabel>();
        qiPao = transform.Find("Container/QiPao").GetComponent<UISprite>();
        content = transform.Find("Container/Label").GetComponent<UILabel>();
        contentObj = transform.Find("Container").gameObject;
        playerName = transform.Find("PlayerName").GetComponent<UILabel>();
        sendTime = transform.Find("SendTime").GetComponent<UILabel>();

        mIconBorder = transform.Find("MIconBorder").GetComponent<UISprite>();
        mIconBtn = transform.Find("MIcon").GetComponent<GUISingleButton>();
        mChannelLabel = transform.Find("MSprite/MChannelLabel").GetComponent<UILabel>();
        mChannelObj = transform.Find("MSprite").gameObject;
        mVipObj = transform.Find("Mvipbg").gameObject;
        mVipLabel = transform.Find("Mvipbg/Vip").GetComponent<UILabel>();
        mQiPao = transform.Find("MContainer/QiPao").GetComponent<UISprite>();
        mContent = transform.Find("MContainer/Label").GetComponent<UILabel>();
        mContentObj = transform.Find("MContainer").gameObject;
        mPlayerName = transform.Find("MPlayerName").GetComponent<UILabel>();
        mSendTime = transform.Find("MSendTime").GetComponent<UILabel>();
        iconBtn.onClick = OnIconClick;
        mIconBtn.onClick = OnIconClick;
    }
    /// <summary>
    /// 点击头像按钮事件
    /// </summary>
    private void OnIconClick()
    {
        Debug.Log("点击头像" + chatData.Id + "____" + chatData.NickName);
        if (chatData.IsLocalPlayer)
        {
            return;
        }
        Globe.privateChatPlayerId = chatData.Id;
        Globe.privateChatPlayerAId = chatData.AccountId;
        Globe.chatingPlayerNickName = chatData.NickName;
        object[] temlist = new object[] { true, chatData.Id, chatData.AccountId, chatData.NickName };
        Control.ShowGUI(UIPanleID.UIPlayerInteractionPort, EnumOpenUIType.DefaultUIOrSecond, false, temlist);
        //Control.ShowGUI(GameLibrary.UIPlayerInteractionPort);
    }

    public override void Info(object obj)
    {
        base.Info(obj);
        {

        }
        chatData = (ChatData)obj;
        if (chatData.ContentType == ChatContentType.TextContent)
        {
            iconBorder.gameObject.SetActive((!chatData.IsLocalPlayer) ? true : false);
            iconBtn.gameObject.SetActive((!chatData.IsLocalPlayer) ? true : false);
            channelObj.gameObject.SetActive((!chatData.IsLocalPlayer) ? true : false);
            playerName.gameObject.SetActive((!chatData.IsLocalPlayer) ? true : false);
            sendTime.gameObject.SetActive((!chatData.IsLocalPlayer) ? true : false);
            if (chatData.Vip > 0)
            {
                //其他玩家是vip
                vipObj.SetActive((!chatData.IsLocalPlayer) ? true : false);
                //设置其他玩家名字显示位置
                //Vector3 vec = playerName.transform.localPosition;
                //vec.x = -151;
                //playerName.transform.localPosition = vec;

                //设置其他玩家时间位置
                //vec.x =vec.x + playerName.width + 20;
                //sendTime.transform.localPosition = vec;
            }
            else
            {
                //其他玩家不是vip
                vipObj.SetActive(false);
                //设置其他玩家名字显示位置
                //Vector3 vec = playerName.transform.localPosition;
                //vec.x = -196;
                //playerName.transform.localPosition = vec;
                //设置其他玩家时间位置
                //vec.x = vec.x + playerName.width + 20;
                //sendTime.transform.localPosition = vec;

            }
            contentObj.SetActive((!chatData.IsLocalPlayer) ? true : false);

            mIconBorder.gameObject.SetActive(chatData.IsLocalPlayer ? true : false);
            mIconBtn.gameObject.SetActive(chatData.IsLocalPlayer ? true : false);
            mChannelObj.gameObject.SetActive(chatData.IsLocalPlayer ? true : false);
            mPlayerName.gameObject.SetActive(chatData.IsLocalPlayer ? true : false);
            mSendTime.gameObject.SetActive(chatData.IsLocalPlayer ? true : false);
            if (chatData.Vip > 0)
            {
                //本地玩家是vip
                mVipObj.SetActive(chatData.IsLocalPlayer ? true : false);
                //设置本地玩家名字显示
                //Vector3 vec = mPlayerName.transform.localPosition;
                //vec.x = 147;
                //mPlayerName.transform.localPosition = vec;
                //设置本地玩家时间位置 

                //vec.x = vec.x - playerName.width - 20;
                //mSendTime.transform.localPosition = vec;
            }
            else
            {
                //本地玩家不是vip
                mVipObj.SetActive(false);
                //设置本地玩家名字显示
                //Vector3 vec = mPlayerName.transform.localPosition;
                //vec.x = 200;
                //mPlayerName.transform.localPosition = vec;
                //设置本地玩家时间位置 
                //vec.x = vec.x - playerName.width - 20;
                //mSendTime.transform.localPosition = vec;
            }
            mContentObj.SetActive(chatData.IsLocalPlayer ? true : false);
        }
        else
        {
            iconBorder.gameObject.SetActive(false);
            iconBtn.gameObject.SetActive(false);
            channelLabel.gameObject.SetActive(true);
            playerName.gameObject.SetActive(false);
            vipObj.SetActive(false);
            contentObj.SetActive(true);
            sendTime.gameObject.SetActive(true);

            mIconBorder.gameObject.SetActive(false);
            mIconBorder.gameObject.SetActive(false);
            mIconBtn.gameObject.SetActive(false);
            mChannelObj.gameObject.SetActive(false);
            mPlayerName.gameObject.SetActive(false);
            mVipObj.SetActive(false);
            mContentObj.SetActive(false);
            mSendTime.gameObject.SetActive(false);
        }
        //如果是系统提示内容
        if (chatData.ContentType != ChatContentType.TextContent)
        {
            SetChannelLabel();
            content.text = ToSBC(chatData.ChatContent);
            qiPao.enabled = false;
            sendTime.text = chatData.Time;

            //系统提示内容 发送时间位置设置
            Vector3 vec = channelObj.transform.localPosition;
            vec.x = -200;
            sendTime.transform.localPosition = vec;

            return;
        }
       

        if (!chatData.IsLocalPlayer)
        {
            SetChannelLabel();
            if (FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList.ContainsKey(chatData.HeadId))
            {
                iconBtn.GetComponent<UISprite>().spriteName = FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList[chatData.HeadId].icon_name + "_head";
            }

            playerName.text = chatData.NickName;
            vipLabel.text = chatData.Vip + "";

            content.text = ToSBC(chatData.ChatContent);
            sendTime.text = chatData.Time;
            //设置气泡大小
            //SetQipao(chatData.IsLocalPlayer);

        }
        else
        {
            switch (chatData.Type)
            {
                case ChatType.WorldChat:
                    mChannelLabel.text = "[fdee03]" + "【世界】" + "[-]";
                    break;
                case ChatType.SocietyChat:
                    mChannelLabel.text = "[0dd0f7]" + "【公会】" + "[-]";
                    break;
                case ChatType.PrivateChat:
                    mChannelLabel.text = "[db4bff]" + "【私聊】" + "[-]";
                    break;
                case ChatType.NearbyChat:
                    mChannelLabel.text = "附近";
                    break;
                case ChatType.TroopsChat:
                    mChannelLabel.text = "队伍";
                    break;
                case ChatType.SystemChat:
                    mChannelLabel.text = "系统";
                    break;
            }
            if (FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList.ContainsKey(chatData.HeadId))
            {
                mIconBtn.GetComponent<UISprite>().spriteName = FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList[chatData.HeadId].icon_name + "_head";
            }
           
            mPlayerName.text = chatData.NickName;
            mVipLabel.text = chatData.Vip + "";

            mContent.text = ToSBC(chatData.ChatContent);
            mSendTime.text = chatData.Time;
            //气泡设置
            //SetQipao(chatData.IsLocalPlayer);

        }
        SetNameTimePosition(chatData);
    }
    /// <summary>
    /// 设置频道文字
    /// </summary>
    private void SetChannelLabel()
    {
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
    }
    /// <summary>
    /// 动态设置玩家名时间显示位置
    /// </summary>
    /// <param name="chatdata"></param>
    private void SetNameTimePosition(ChatData chatdata)
    {
        if (chatdata.IsLocalPlayer)
        {
            if (chatdata.Vip > 0)
            {
                //设置本地玩家名字显示
                Vector3 vec = mPlayerName.transform.localPosition;
                vec.x = 147;
                mPlayerName.transform.localPosition = vec;
                //设置本地玩家时间位置 

                vec.x = vec.x - mPlayerName.width - 10;
                mSendTime.transform.localPosition = vec;
            }
            else
            {
                //设置本地玩家名字显示
                Vector3 vec = mPlayerName.transform.localPosition;
                vec.x = 200;
                mPlayerName.transform.localPosition = vec;
                //设置本地玩家时间位置 
                vec.x = vec.x - mPlayerName.width - 10;
                mSendTime.transform.localPosition = vec;
            }
        }
        else
        {
            if (chatData.Vip > 0)
            {
                //设置其他玩家名字显示位置
                Vector3 vec = playerName.transform.localPosition;
                vec.x = -151;
                playerName.transform.localPosition = vec;

                //设置其他玩家时间位置
                vec.x = vec.x + playerName.width + 10;
                sendTime.transform.localPosition = vec;
            }
            else
            {
                //设置其他玩家名字显示位置
                Vector3 vec = playerName.transform.localPosition;
                vec.x = -196;
                playerName.transform.localPosition = vec;
                //设置其他玩家时间位置
                vec.x = vec.x + playerName.width + 10;
                sendTime.transform.localPosition = vec;

            }
        }
    }
    /// <summary>
    /// 设置气泡
    /// </summary>
    /// <param name="isLocalPlayer"></param>
    private void SetQipao(bool isLocalPlayer)
    {
        if (isLocalPlayer)
        {
            mQiPao.height = mContent.height + 10;
            //输入内容字节数 与限定文字的字节数 比较
            if(mContent.width <510)//if (Encoding.UTF8.GetBytes(mContent.text.ToString()).Length < rowFontCount * 3)
            {
                mQiPao.width = mContent.width + 20;
                //mQiPao.width = Encoding.UTF8.GetBytes(mContent.text.ToString()).Length * 529 / (rowFontCount * 3) + 20;
                mContent.alignment = NGUIText.Alignment.Right;
            }
            else
            {
                mQiPao.width = 529 + 20;
                mContent.alignment = NGUIText.Alignment.Left;
            }
        }
        else
        {
            qiPao.height = content.height + 10;
            if(mContent.width<510) //(Encoding.UTF8.GetBytes(content.text.ToString()).Length < rowFontCount * 3)
            {
                qiPao.width = content.width + 20;
                //qiPao.width = Encoding.UTF8.GetBytes(content.text.ToString()).Length * 529 / (rowFontCount * 3) + 20;
                content.alignment = NGUIText.Alignment.Right;
            }
            else
            {
                qiPao.width = 529 + 20;
                content.alignment = NGUIText.Alignment.Left;
            }
        }
    }
    /// <summary>
    /// 将文字内容中的半角空格转换成全交空格
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public string ToSBC(string input)
    {
        //半角转全角： 
        char[] c = input.ToCharArray();
        for (int i = 0; i < c.Length; i++)
        {
            if (c[i] == 32)
            {
                c[i] = (char)12288;
                continue;
            }
            //if (c[i] < 127&&c[i]>32)
            //   c[i] = (char)(c[i] + 65248);
        }
        return new string(c);
    }

}
