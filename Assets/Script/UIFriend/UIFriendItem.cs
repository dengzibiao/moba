using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

//#define TCB
public class UIFriendItem : GUISingleItemList
{
    public GUISingleButton addBtn;//添加好友
    public GUISingleLabel playerName;//玩家名字
    public GUISingleLabel titleName;//称号
    public GUISingleLabel guildName;//公会
    public GUISingleLabel heroLevelLab;
    public GUISingleLabel fightValueLab;//战斗力
    public GUISingleButton acceptBtn;//接受好友申请
    public GUISingleButton removeBtn;//移除黑名单好友按钮
    public GUISingleButton teamBtn;//组队
    public GUISingleButton deleteBtn;//删除好友
    public GUISingleButton chatBtn;//私聊
    public GUISingleButton deleteEnemyBtn;//仇人列表删除玩家
    public GUISingleButton deleteLastBtn;//删除最近联系列表
    public GUISingleButton refuseBtn;
    public GUISingleSprite icon;
    public GUISingleSprite heroBg;
    private FriendData dataVo;
    #region Init


    protected override void InitItem()
    {
        addBtn.onClick = OnAdd;
        acceptBtn.onClick = OnAcceptBtn;
        refuseBtn.onClick = OnRefuse;
        removeBtn.onClick = OnRemoveBtn;
        teamBtn.onClick = OnTeamBtn;
        deleteBtn.onClick = OnDelete;
        chatBtn.onClick = OnChat;
        icon.onClick = OnIconClick;
        deleteEnemyBtn.onClick = OnDeleteEnemy;
        deleteLastBtn.onClick = OnDeleteLast;
    }
    //最近联系列表删除
    private void OnDeleteLast()
    {
          ClientSendDataMgr.GetSingle().GetFriendSend().FriendsDelete(dataVo.PlayerId, (int)Friends.DeleteLast);
    }
    //仇人列表删除
    private void OnDeleteEnemy()
    {
         ClientSendDataMgr.GetSingle().GetFriendSend().FriendsDelete(dataVo.PlayerId, (int)Friends.DeleteEnemy);
    }

    //点击头像
    private void OnIconClick()
    {
        print("调用详情借口");
    }

    //组队
    private void OnTeamBtn()
    {
        print("调用组队接口");
    }

    //从黑名单移除
    private void OnRemoveBtn()
    {
        ClientSendDataMgr.GetSingle().GetFriendSend().FriendsDelete(dataVo.PlayerId, (int)Friends.Remove);
    }

    #endregion
    public override void Info(object obj)
    {
        if (null == obj)
        {

        }
        else
        {
            this.dataVo = (FriendData)obj;
            playerName.text = dataVo.Name;
            heroLevelLab.text = dataVo.Level.ToString();
            if (null != dataVo.PlayerIcon)
            {
                icon.spriteName = dataVo.PlayerIcon;
            }
            fightValueLab.text = "战斗力:" + dataVo.Fighting.ToString();
            titleName.text = dataVo.Title;
            heroBg.spriteName = dataVo.PlayerFrame;
        }
    }

    //添加
    private void OnAdd()
    {
        Control.ShowGUI(UIPanleID.UITooltips, EnumOpenUIType.DefaultUIOrSecond, false, "已发送好友申请");
        ClientSendDataMgr.GetSingle().GetFriendSend().FriendsAdd(this.dataVo.PlayerId, dataVo.AcountPlayerId);
    }
    //删除好友
    private void OnDelete()
    {
        object[] obj = new object[5] { "", "确定要删除好友吗?", UIPopupType.EnSure, this.gameObject, "DeleteFriendHandler" };
        Control.ShowGUI(UIPanleID.UIPopUp, EnumOpenUIType.DefaultUIOrSecond, false, obj);
    }

    private void DeleteFriendHandler()
    {
        UIFriends.Instance.TemporaryData(index);
        ClientSendDataMgr.GetSingle().GetFriendSend().FriendsDelete(dataVo.PlayerId, (int)Friends.Delete);
    }
    //聊天
    private void OnChat()
    {
        object[] temlist = new object[] { ChatType.PrivateChat, dataVo.PlayerId, dataVo.AcountPlayerId, dataVo.Name };
        Control.ShowGUI(UIPanleID.UIChatPanel, EnumOpenUIType.DefaultUIOrSecond, false, temlist);
        //UIChatPanel.Instance.ExternalOpenPrivateChat(dataVo.PlayerId,dataVo.AcountPlayerId, dataVo.Name);
    }
    //同意
    private void OnAcceptBtn()
    {
        UIFriends.Instance.TemporaryData(index);
        ClientSendDataMgr.GetSingle().GetFriendSend().FriendsAgreeOrRefuse((int)Friends.Agree, dataVo.PlayerId,dataVo.AcountPlayerId);
    }
    ////拒绝
    private void OnRefuse()
    {
        UIFriends.Instance.TemporaryData(index);
        ClientSendDataMgr.GetSingle().GetFriendSend().FriendsAgreeOrRefuse((int)Friends.Refuse, dataVo.PlayerId, dataVo.AcountPlayerId);
    }

}
