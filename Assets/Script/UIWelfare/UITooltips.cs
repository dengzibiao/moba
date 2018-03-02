using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tianyu;
using System;


public class UITooltips : GUIBase
{
    public enum BlackerBottomType
    {
        TenWords = 1,
        twentyWords = 2,
        thirtyWords = 3,

    }
    public GUISingleLabel titleTxt; 
    private static UITooltips instance;
    private string str;
    public static UITooltips Instance
    {
        get { return instance; }
        set { instance = value; }
    }
    public UITooltips()
    {
        instance = this;
    }

    protected override void SetUI(params object[] uiParams)
    {
        if (uiParams[0] != null)
        {
            this.str = uiParams[0].ToString();
            base.SetUI(uiParams);
        }      

    }

    protected override void OnLoadData()
    {
        base.OnLoadData();
        Show();
    }

    public override UIPanleID GetUIKey()
    {
      return UIPanleID.UITooltips;
    }

    protected override void ShowHandler()
    {
        titleTxt.text = str;
        StartCoroutine(Hidel());
    }
    public IEnumerator Hidel()
    {
        yield return new WaitForSeconds(1f);
        Control.HideGUI(this.GetUIKey());
    }
}
