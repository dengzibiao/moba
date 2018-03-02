using UnityEngine;
using System.Collections;

public class UISocietyIconPanel : GUIBase
{
    public GUISingleMultList societyIconMultList;
    public GameObject backObj;
    public static UISocietyIconPanel Instance;
    private string[] iconNameArr = new string[] { "biaoyi", "biaoer", "biaosan", "biaosi", "biaowu", "biaoliu", "biaoqi" };
    public UISocietyIconPanel()
    {
        Instance = this;
    }
    protected override void Init()
    {
        backObj = transform.Find("Mask").gameObject;
        societyIconMultList = transform.Find("SocietyIconScrollView/SocietyIconMultList").GetComponent<GUISingleMultList>();
        UIEventListener.Get(backObj).onClick += OnCloseClick;
    }
    public override UIPanleID GetUIKey()
    {
        return UIPanleID.UISocietyIconPanel;
    }
    protected override void SetUI(params object[] uiParams)
    {
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
        Control.HideGUI(this.GetUIKey());
    }
    protected override void ShowHandler()
    {
        societyIconMultList.InSize(iconNameArr.Length, 4);
        societyIconMultList.Info(iconNameArr);
    }
}
