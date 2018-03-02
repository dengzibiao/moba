using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class UISocietyInfoPanel : GUIBase
{
    private UISprite societyIcon;//公会图标
    private UILabel societyName;//公会名称
    private UILabel societyID;//公会id
    private UILabel presidentName;//会长名称
    private UILabel societyLevel;//公会等级
    private UILabel allContributionValue;//公会贡献度
    private UILabel societyManifesto;//公会宣言
    private UILabel applyforing;//申请中
    public GUISingleButton joinBtn;//申请入会Btn
    public GameObject backObj;
    private SocietyData societyData;

    public static UISocietyInfoPanel Instance;
    public UISocietyInfoPanel()
    {
        Instance = this;
    }
    protected override void Init()
    {
        backObj = transform.Find("Mask").gameObject;
        societyIcon = transform.Find("SocietyIcon").GetComponent<UISprite>();
        societyName = transform.Find("SocietyName").GetComponent<UILabel>();
        societyID = transform.Find("SocietyID").GetComponent<UILabel>();
        presidentName = transform.Find("PresidentName").GetComponent<UILabel>();
        societyLevel = transform.Find("SocietyLevel").GetComponent<UILabel>();
        allContributionValue = transform.Find("AllContributionValue").GetComponent<UILabel>();
        societyManifesto = transform.Find("SocietyManifesto").GetComponent<UILabel>();
        applyforing = transform.Find("Applyforing").GetComponent<UILabel>();
        joinBtn = transform.Find("JoinBtn").GetComponent<GUISingleButton>();
        joinBtn.onClick = OnJoinClick;
        UIEventListener.Get(backObj).onClick += OnCloseClick;
    }
    public override UIPanleID GetUIKey()
    {
        return UIPanleID.UISocietyInfoPanel;
    }
    public void SetData(SocietyData data)
    {
        societyData = data;
    }
    protected override void SetUI(params object[] uiParams)
    {
        societyData = (SocietyData)uiParams[0];
        base.SetUI();
    }
    protected override void OnLoadData()
    {
        base.OnLoadData();
        this.State = EnumObjectState.Ready;
        Show();
    }
    private void OnCloseClick(GameObject go)
    {
        //Hide();
        Control.HideGUI(this.GetUIKey());
    }
    private void OnJoinClick()
    {
        Debug.Log("申请加入公会");
        //Hide();
        //ClientSendDataMgr.GetSingle().GetSocietySend().SendApplicationJoinSociety(C2SMessageType.ActiveWait, societyData.societyID);
        Control.HideGUI(this.GetUIKey());
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", societyData.societyID);//玩家账户
        Singleton<Notification>.Instance.Send(MessageID.union_application_join_req, newpacket, C2SMessageType.ActiveWait);
    }

    protected override void ShowHandler()
    {
        applyforing.gameObject.SetActive(false);
        if (societyData!=null)
        {
            societyIcon.spriteName = societyData.societyIcon;
            societyID.text = "ID:"+societyData.societyID;
            societyName.text = societyData.societyName;
            societyLevel.text = societyData.societyLevel + "";
            presidentName.text = societyData.presidentName;
            societyManifesto.text = societyData.societyManifesto;

            joinBtn.gameObject.SetActive(true);
            applyforing.gameObject.SetActive(false);
            //判断是显示入会申请 还是申请中

            if (SocietyManager.Single().playerApplicationSocietyList != null && SocietyManager.Single().playerApplicationSocietyList.Length > 0)
            {
                for (int i = 0; i < SocietyManager.Single().playerApplicationSocietyList.Length; i++)
                {
                    if (societyData.societyID == SocietyManager.Single().playerApplicationSocietyList[i])
                    {
                        joinBtn.gameObject.SetActive(false);
                        applyforing.gameObject.SetActive(true);
                        break;
                    }
                }
            }

        }
    }
}
