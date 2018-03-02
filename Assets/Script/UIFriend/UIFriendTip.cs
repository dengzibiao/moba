using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIFriendTip : GUIBase
{
    public static UIFriendTip instance;
    public GUISingleButton addBtn;//添加好友按钮
    public GUISingleButton cancleBtn;//取消按钮
    public GUISingleLabel playerName;//玩家名字
    public GUISingleLabel fightValueLab;//战斗力
    public GUISingleLabel treamLevelLab;//等级
    public GUISingleLabel heroLevel;//等级
    public GUISingleSprite heroBg;//头像框
    public GUISingleSprite icon;//头像
    private long id;
    public UIFriendTip()
    {
        instance = this;
    }

    public override UIPanleID GetUIKey()
    {
        return UIPanleID.UIFriendTip;
    }

    protected override void SetUI(params object[] uiParams)
    {
        if (uiParams.Length > 0)
        {
            this.id = long.Parse(uiParams[0].ToString());
        }

        base.SetUI(uiParams);
    }

    protected override void OnLoadData()
    {
        base.OnLoadData();
        Singleton<Notification>.Instance.RegistMessageID(MessageID.common_search_friend_ret, this.GetUIKey());
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", this.id);//好友id
        Singleton<Notification>.Instance.Send(MessageID.common_search_friend_req, newpacket);
    }

    public override void ReceiveData(uint messageID)
    {
        if (playerData.GetInstance().friendListData.searchList.Count > 0)
        {
            Show();
        }
        else
        {
            Control.ShowGUI(UIPanleID.UITooltips, EnumOpenUIType.DefaultUIOrSecond, false, "请输入正确的玩家ID号");
            Control.HideGUI(this.GetUIKey());
        }

        base.ReceiveData(messageID);
    }

    protected override void Init()
    {
        addBtn.onClick = AddFriend;
        cancleBtn.onClick = ClosePanle;
    }

    private void ClosePanle()
    {
        Control.HideGUI(this.GetUIKey());
    }

    private void AddFriend()
    {
        if (playerData.GetInstance().friendListData.searchList[0].PlayerId == playerData.GetInstance().selfData.playerId)
        {
            Control.ShowGUI(UIPanleID.UITooltips, EnumOpenUIType.DefaultUIOrSecond, false, "你要添加自己为好友?");
            Control.HideGUI(this.GetUIKey()); return;
        }
        else
        {
            Dictionary<string, object> newpacket = new Dictionary<string, object>();
            newpacket.Add("arg1", playerData.GetInstance().friendListData.searchList[0].PlayerId);//好友id
            newpacket.Add("arg2", playerData.GetInstance().friendListData.searchList[0].AcountPlayerId);//账号id
            Singleton<Notification>.Instance.Send(MessageID.common_add_friend_req, newpacket);
            UIFriends.Instance.TemporaryAddFriend(true);
            Control.ShowGUI(UIPanleID.UITooltips, EnumOpenUIType.DefaultUIOrSecond, false, "已经发送好友申请");
            Control.HideGUI(this.GetUIKey());
        }

    }

    protected override void ShowHandler()
    {
        if (playerData.GetInstance().friendListData.searchList.Count > 0)
        {
            playerName.text = playerData.GetInstance().friendListData.searchList[0].Name;
            fightValueLab.text = playerData.GetInstance().friendListData.searchList[0].Fighting.ToString();
            heroLevel.text = playerData.GetInstance().friendListData.searchList[0].HeroLevel.ToString();
            treamLevelLab.text = playerData.GetInstance().friendListData.searchList[0].Level.ToString();
            icon.spriteName = playerData.GetInstance().friendListData.searchList[0].PlayerIcon;
            heroBg.spriteName = playerData.GetInstance().friendListData.searchList[0].PlayerFrame;
        }
    }
}
