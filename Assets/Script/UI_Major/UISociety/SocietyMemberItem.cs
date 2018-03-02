using UnityEngine;
using System.Collections;
using System;

public class SocietyMemberItem : GUISingleItemList
{
    private int[] arr = new int[6] { 0,0,0,0,0,0};
    public GUISingleSprite rank;
    public GUISingleLabel ranking;//公会成员排名
    public GUISingleLabel memberName;//成员名称
    public GUISingleLabel contributionValue;//贡献度
    public GUISingleLabel weekContributionValue;//周贡献度
    public GUISingleLabel societyStatus;//身份
    public GUISingleLabel state;//在线状态
    public GUISingleSprite icon;//成员图标
    private SocietyMemberData memberData;
    protected override void InitItem()
    {
        UIEventListener.Get(A_Sprite.gameObject).onClick = OnBuyBtnClick;
    }
    public override void Info(object obj)
    {
        memberData = (SocietyMemberData)obj;
    }
    protected override void ShowHandler()
    {
        if (memberData!=null)
        {
            
            if (index + 1>0&&index + 1 <=3)
            {
                rank.gameObject.SetActive(true);
                ranking.gameObject.SetActive(false);
                rank.spriteName = index + 1+"";

            }
            else
            {
                rank.gameObject.SetActive(false);
                ranking.gameObject.SetActive(true);
                ranking.text = (index + 1) + "";
            }
            memberName.text = memberData.memberName;
            icon.spriteName = memberData.memberIcon;
            if (memberData.societyStatus == SocietyStatus.Member)
            {
                societyStatus.text = "成员";
            }
            else if (memberData.societyStatus == SocietyStatus.President)
            {
                societyStatus.text = "会长";
            }
            contributionValue.text = "--";
            weekContributionValue.text = "--";
            state.text = "在线";

        }
    }
    private void OnBuyBtnClick(GameObject go)
    {
        //先判断是否是自己 是自己弹出提示

        //不是自己 （现在只有会长和普通会员）
        //所以如果自己是会长 ：查看信息 好友 私聊 屏蔽 踢出 传位
        //不是会长：查看信息 好友 私聊 屏蔽
        int[] jurisdictionArr;
        if (memberData!=null)
        {
            if (playerData.GetInstance().selfData.playerId == memberData.playerId)
            {
                Control.ShowGUI(UIPanleID.UITooltips, EnumOpenUIType.DefaultUIOrSecond, false, "点击的是自己");
            }
            else
            {
                if (SocietyManager.Single().societyStatus == SocietyStatus.Member)
                {
                    //查看 好友 私聊 屏蔽 踢出 传位 1显示 0 不显示
                    jurisdictionArr = new int[6] { 0, 1, 1, 0, 0, 0 };
                    //UISocietyInteractionPort.Instance.SetData(jurisdictionArr,memberData);
                    //Control.ShowGUI(GameLibrary.UISocietyInteractionPort);

                    object[] temlist = new object[] { jurisdictionArr, memberData };
                    Control.ShowGUI(UIPanleID.UISocietyInteractionPort, EnumOpenUIType.DefaultUIOrSecond, false, temlist);

                }
                else if (SocietyManager.Single().societyStatus == SocietyStatus.President)
                {
                    jurisdictionArr = new int[6] { 0, 1, 1, 0, 1, 1 };
                    //UISocietyInteractionPort.Instance.SetData(jurisdictionArr,memberData);
                    //Control.ShowGUI(GameLibrary.UISocietyInteractionPort);
                    object[] temlist = new object[] { jurisdictionArr, memberData };
                    Control.ShowGUI(UIPanleID.UISocietyInteractionPort, EnumOpenUIType.DefaultUIOrSecond, false, temlist);
                }
            }
            
        }
    }
}
