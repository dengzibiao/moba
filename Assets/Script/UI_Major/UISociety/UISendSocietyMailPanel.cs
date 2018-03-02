using UnityEngine;
using System.Collections;
using System;

public class UISendSocietyMailPanel : GUIBase
{

    public static UISendSocietyMailPanel Instance;
    public GUISingleButton sendBtn;//刷新按钮
    private GUISingleInput nameInput;//输入公会id
    private GUISingleInput societyManifestoInput;//输入公会id
    public UISendSocietyMailPanel()
    {
        Instance = this;
    }
    protected override void Init()
    {
        sendBtn = transform.Find("SendBtn").GetComponent<GUISingleButton>();
        nameInput = transform.Find("NameInput").GetComponent<GUISingleInput>();
        societyManifestoInput = transform.Find("SocietyManifestoInput").GetComponent<GUISingleInput>();
        sendBtn.onClick = OnSendClick;
    }
    public override UIPanleID GetUIKey()
    {
        return UIPanleID.none;
    }
    private void OnSendClick()
    {
        Debug.Log("发送公会邮件");
    }

    protected override void ShowHandler()
    {

    }
}
