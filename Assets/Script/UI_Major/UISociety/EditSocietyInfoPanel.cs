using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class EditSocietyInfoPanel : GUIBase
{

    public static EditSocietyInfoPanel Instance;
    public UISprite icon;
    public GUISingleButton sendBtn;//修改公会宣言
    public GUISingleInput societyManifestoInput;//输入修改的宣言内容
    public GameObject backObj;
    public EditSocietyInfoPanel()
    {
        Instance = this;
    }
    protected override void Init()
    {
        backObj = transform.Find("Mask").gameObject;
        sendBtn.onClick = OnSendClick;
        UIEventListener.Get(backObj).onClick += OnCloseClick;
    }
    public override UIPanleID GetUIKey()
    {
        return UIPanleID.none;
    }
    private void OnCloseClick(GameObject go)
    {
        Hide();
    }
    private void OnSendClick()
    {
        if (societyManifestoInput.text.Trim() == "")
        {
            Control.ShowGUI(UIPanleID.UITooltips, EnumOpenUIType.DefaultUIOrSecond, false, "宣言不能为空");
            return;
        }
        //发送修改宣言协议
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", SocietyManager.Single().mySocityID);
        newpacket.Add("arg2", societyManifestoInput.text);
        Singleton<Notification>.Instance.Send(MessageID.union_change_some_info_req, newpacket, C2SMessageType.ActiveWait);
        //ClientSendDataMgr.GetSingle().GetSocietySend().SendChangeSocietyInfo(C2SMessageType.ActiveWait,SocietyManager.Single().mySocityID, societyManifestoInput.text);
        Hide(); 
    }
}
