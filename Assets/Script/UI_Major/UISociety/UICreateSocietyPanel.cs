using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class UICreateSocietyPanel : GUIBase
{
    private bool isBreath = false;
    public static UICreateSocietyPanel Instance;
    public UISprite icon;
    public GUISingleButton lastIconBtn;//上一个
    public GUISingleButton nextIconBtn;//下一个
    public GUISingleButton createBtn;//刷新按钮
    public GUISingleButton changeIconBtn;
    private GUISingleInput nameInput;//输入公会id
    private GUISingleInput joinLevelInput;//输入公会id
    private GUISingleInput societyManifestoInput;//输入公会id
    private string[] iconNameArr = new string[] { "gh0001", "gh0002" };
    string societyName = "";//公会图标名称
    private int currentIndex = 0;
    public UICreateSocietyPanel()
    {
        Instance = this;
    }
    protected override void Init()
    {
        societyName = "biaoyi";
        icon = transform.Find("SocietyIcon").GetComponent<UISprite>();
        createBtn = transform.Find("CreateBtn").GetComponent<GUISingleButton>();
        nameInput = transform.Find("NameInput").GetComponent<GUISingleInput>();
        joinLevelInput = transform.Find("JoinLevelInput").GetComponent<GUISingleInput>();
        societyManifestoInput = transform.Find("SocietyManifestoInput").GetComponent<GUISingleInput>();
        lastIconBtn.onClick = OnLastClick;
        nextIconBtn.onClick = OnNextClick;
        createBtn.onClick = OnCreateSocietyClick;
        changeIconBtn.onClick = OnChangeIconClick;
    }
    public override UIPanleID GetUIKey()
    {
        return UIPanleID.none;
    }
    void FixedUpdate()
    {
        //StarBreathingLight();
    }
    /// <summary>
    /// 呼吸灯效果
    /// </summary>
    void StarBreathingLight()
    {
        if (isBreath)
        {
            lastIconBtn.GetComponent<UISprite>().alpha += 0.02f;
            nextIconBtn.GetComponent<UISprite>().alpha += 0.02f;
            if (lastIconBtn.GetComponent<UISprite>().alpha >= 1 && nextIconBtn.GetComponent<UISprite>().alpha >= 1)
            {
                isBreath = false;
            }
        }
        else
        {
            lastIconBtn.GetComponent<UISprite>().alpha -= 0.02f;
            nextIconBtn.GetComponent<UISprite>().alpha -= 0.02f;
            if (lastIconBtn.GetComponent<UISprite>().alpha <= 0.3f && nextIconBtn.GetComponent<UISprite>().alpha <= 0.3f)
            {
                isBreath = true;
            }
        }
    }
    /// <summary>
    /// 设置刷新选中的公会图标
    /// </summary>
    /// <param name="name"></param>
    public void RefreshSocietyIcon(string name)
    {
        societyName = name;
        icon.spriteName = societyName;
    }
    private void OnNextClick()
    {
        currentIndex++;
        if (currentIndex>= iconNameArr.Length)
        {
            currentIndex = 0;
        }
        if(currentIndex< iconNameArr.Length)
            icon.spriteName = iconNameArr[currentIndex];
    }

    private void OnLastClick()
    {
        currentIndex--;
        if (currentIndex<=0)
        {
            currentIndex = iconNameArr.Length - 1;
        }
        if (currentIndex < iconNameArr.Length)
            icon.spriteName = iconNameArr[currentIndex];
    }
    private void CreateSocietyEvent()
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", nameInput.text.Trim());
        newpacket.Add("arg2", societyManifestoInput.text);
        newpacket.Add("arg3", societyName);
        Singleton<Notification>.Instance.Send(MessageID.union_create_someone_req, newpacket, C2SMessageType.ActiveWait);
        //ClientSendDataMgr.GetSingle().GetSocietySend().SendCreateSociety(C2SMessageType.ActiveWait,nameInput.text.Trim(), societyManifestoInput.text, societyName);
    }
    private void OnCreateSocietyClick()
    {
        Debug.Log("发送创建公会协议");
        //if(currentIndex<iconNameArr.Length)
        //{
        //    societyName = iconNameArr[currentIndex];
        //}
        if (!string.IsNullOrEmpty(nameInput.text.Trim()))
        {
            object[] obj = new object[5] { "花费400钻石创建公会，是否确认？", "", UIPopupType.EnSure, this.gameObject, "CreateSocietyEvent" };
            Control.ShowGUI(UIPanleID.UIPopUp, EnumOpenUIType.DefaultUIOrSecond, false, obj);
        }
        else
        {
            Control.ShowGUI(UIPanleID.UITooltips, EnumOpenUIType.DefaultUIOrSecond, false, "公会名称不可为空");
        }
        
    }

    private void OnChangeIconClick()
    {
        Debug.Log("修改公会图标");
        Control.ShowGUI(UIPanleID.UISocietyIconPanel, EnumOpenUIType.DefaultUIOrSecond);
        //Control.ShowGUI(GameLibrary.UISocietyIconPanel);
    }

    protected override void ShowHandler()
    {
        currentIndex = 0;
        icon.spriteName = "biaoyi";

    }
}
