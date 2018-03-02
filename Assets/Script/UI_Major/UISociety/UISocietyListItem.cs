using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class UISocietyListItem : GUISingleItemList
{
    public GUISingleSprite icon;//公会图标
    public GUISingleLabel societyID;//公会ID
    public GUISingleLabel societyName;//公会名称
    public GUISingleLabel societyLevel;//公会等级
    public GUISingleLabel presidentName;//公会会长
    public GUISingleLabel societyPlayerCount;//公会人数
    public GUISingleLabel societyJoinLevel;//入会等级
    public GUISingleLabel intheapplication;//申请中
    public GUISingleButton joinBtn;//申请入会

    private SocietyData societyData;

    protected override void InitItem()
    {
        UIEventListener.Get(A_Sprite.gameObject).onClick = OnBuyBtnClick;
    }
    public override void Info(object obj)
    {
        societyData = (SocietyData)obj;


        joinBtn.gameObject.SetActive(true);
        intheapplication.gameObject.SetActive(false);
        joinBtn.onClick = OnJoinClick;
    }
    protected override void ShowHandler()
    {
        if (societyData!=null)
        {
            icon.spriteName = societyData.societyIcon;
            societyID.text = societyData.societyID + "";
            societyName.text = societyData.societyName;
            societyLevel.text = societyData.societyLevel + "";
            presidentName.text = societyData.presidentName;
            societyPlayerCount.text = "--";
            societyJoinLevel.text = "--";
            //判断是显示入会申请 还是申请中
           
            if (SocietyManager.Single().playerApplicationSocietyList!=null&& SocietyManager.Single().playerApplicationSocietyList.Length>0)
            {
                for (int i = 0;i< SocietyManager.Single().playerApplicationSocietyList.Length;i++)
                {
                    if (societyData.societyID == SocietyManager.Single().playerApplicationSocietyList[i])
                    {
                        joinBtn.gameObject.SetActive(false);
                        intheapplication.gameObject.SetActive(true);
                        break;
                    }
                }
            }


        }
    }
    private void OnJoinClick()
    {
        //申请加入公会
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", societyData.societyID);//玩家账户
        Singleton<Notification>.Instance.Send(MessageID.union_application_join_req, newpacket, C2SMessageType.ActiveWait);
        //ClientSendDataMgr.GetSingle().GetSocietySend().SendApplicationJoinSociety(C2SMessageType.ActiveWait, societyData.societyID);
    }

    private void OnBuyBtnClick(GameObject go)
    {
        Debug.Log("查看公会信息");
        //UISocietyInfoPanel.Instance.SetData(societyData);
        //Control.ShowGUI(GameLibrary.UISocietyInfoPanel);
        Control.ShowGUI(UIPanleID.UISocietyInfoPanel, EnumOpenUIType.DefaultUIOrSecond, false, societyData);
    }
}
