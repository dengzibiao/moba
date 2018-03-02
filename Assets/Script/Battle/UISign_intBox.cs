using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tianyu;
using System;

public class UISign_intBox : GUIBase
{
    public GUISingleButton cancelBtn;
    public GUISingleButton ensureBtn;
    public GUISingleLabel label;
    private static UISign_intBox instance;
    private string str;
    public static UISign_intBox Instance
    {
        get { return instance; }
        set { instance = value; }
    }
    public UISign_intBox()
    {
        instance = this;
    }

    public override UIPanleID GetUIKey()
    {
        return UIPanleID.none;
    }

    protected override void Init()
    {
        base.Init();
        cancelBtn.onClick = OnCancelClick;
        ensureBtn.onClick = OnEnsureClick;
    }
    /// <summary>
    /// 显示文本内容
    /// </summary>
    /// <param name="str"></param>
    public void ShowLabel(string str)
    {
        this.str = str;
        Show();
    }
    protected override void ShowHandler()
    {
        label.text = str;
    }
    private void OnEnsureClick()
    {
        int jewel = 0;
        foreach (var item in FSDataNodeTable<ResetLaterNode>.GetSingleton().DataNodeList.Values)
        {
            jewel = item.retroactiveBuy;
        }
        if (playerData.GetInstance().baginfo.diamond > jewel)
        {
            ClientSendDataMgr.GetSingle().GetUISign_inSend().SendGetPatchUISign_in(C2SMessageType.PASVWait);//补签
                
        }
        else
        {
            //UIPromptBox.Instance.ShowLabel("钻石不足");
            //Control.ShowGUI(UIPanleID.UIPromptBox);
            Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, "钻石不足");
        }
        Hide();
    }

    /// <summary>
    /// 取消
    /// </summary>
    private void OnCancelClick()
    {
        Hide();
    }
}