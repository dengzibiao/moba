using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 商店Npctips
/// </summary>
public class UINpcTips : GUIBase {

    public GUISingleLabel tipsLab; 
    private static UINpcTips instance;
    private string str;
    public static UINpcTips Instance
    {
        get { return instance; }
        set { instance = value; }
    }
    public UINpcTips()
    {
        instance = this;
    }
    protected override void ShowHandler()
    {
        tipsLab.text = str;
    }
    public override UIPanleID GetUIKey()
    {
        return UIPanleID.none;
    }

    public void SetNPCDialogue(string str)
    {
        this.str = str;
        if (str!=null)
        {
            Show();
        }    
    }
}
