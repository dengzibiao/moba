using UnityEngine;
using System.Collections;
using System;

public class SocietyIconItem : GUISingleItemList
{
    public GUISingleSprite icon;//公会图标
    private string iconName;
    protected override void InitItem()
    {
        icon.onClick = OnIconClick;
    }

    private void OnIconClick()
    {
        Debug.Log("点击图片");
        UICreateSocietyPanel.Instance.RefreshSocietyIcon(iconName);
        Control.HideGUI(UIPanleID.UISocietyIconPanel);
        //Control.HideGUI(GameLibrary.UISocietyIconPanel);
    }
    public override void Info(object obj)
    {
        iconName = (string)obj;
    }
    protected override void ShowHandler()
    {
        icon.spriteName = iconName;
    }
}
