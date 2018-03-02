/*
文件名（File Name）:   UIPopup.cs

作者（Author）:    #高#

创建时间（CreateTime）:  2016-11-8 19:21:1
*/
using UnityEngine;
using System.Collections;

public enum UIPopupType : int
{
    Refresh = 0,//刷新
    EnSure,
    OnlyShow
}
public class UIPopUp : GUIBase
{
    public GUISingleButton refreshBtn;
    public GUISingleButton cancelBtn;
    public GUISingleLabel label1;
    public GUISingleLabel label2;
    private GameObject _go;
    private string _functionName;
    public static UIPopUp instance;

    private UIPopupType _type;
    private string _str1;
    private string _str2;
    protected override void Init()
    {
        base.Init();
        refreshBtn.onClick = RefreshBtn;
        cancelBtn.onClick = RefreshBtn;
    }

    public UIPopUp()
    {
        instance = this;
    }
    protected override void ShowHandler()
    {
        base.ShowHandler();
        InitUIShow();
    }

    protected override void SetUI(params object[] uiParams)
    {
        if (uiParams[2] != null)
            this._type = (UIPopupType)uiParams[2];
        if (uiParams[0] != null)
            this._str1 = uiParams[0].ToString();
        if (uiParams[1] != null)
            this._str2 = uiParams[1].ToString();
        if (uiParams[4] != null)
            _functionName = uiParams[4].ToString();
        if (uiParams[3] != null)
            this._go = uiParams[3] as GameObject;
        base.SetUI(uiParams);

    }

    protected override void OnLoadData()
    {
        base.OnLoadData();
        Show();
    }

    private void InitUIShow()
    {
        refreshBtn.functionName = _functionName;
        refreshBtn.Target = _go;

        switch (_type)
        {
            case UIPopupType.EnSure:
                refreshBtn.text = "确认";
                break;
            case UIPopupType.Refresh:
                refreshBtn.text = "刷新";
                break;
            case UIPopupType.OnlyShow:
                cancelBtn.Hide();
                refreshBtn.transform.localPosition = new Vector3(0, refreshBtn.transform.localPosition.y, 0);
                refreshBtn.text = "确认";
                break;
        }
        label1.text = this._str1;
        label2.text = this._str2;
    }
    private void RefreshBtn()
    {
        if (_type == UIPopupType.OnlyShow)
        {
            cancelBtn.Show();
            refreshBtn.transform.localPosition = new Vector3(114, refreshBtn.transform.localPosition.y, 0);
        }
        Hide();
        Control.HideGUI(this.GetUIKey());
    }


    public override UIPanleID GetUIKey()
    {
        return UIPanleID.UIPopUp;
    }
}
