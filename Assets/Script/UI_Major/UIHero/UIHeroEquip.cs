using UnityEngine;
using System.Collections;
using System;

public class UIHeroEquip : GUIBase {
    public GUISingleButton closeBtn;
    public GUISingleMultList multList;
    public override UIPanleID GetUIKey()
    {
        return UIPanleID.none;
    }

    protected override void Init()
    {
        closeBtn.onClick = OnCloseClick;
        multList.InSize(6, 3);
    }

    private void OnCloseClick()
    {
        Hide();
    }
}
