using UnityEngine;
using System.Collections;

public class UIMask : GUIBase
{
    private static UIMask instance;
    private UIPanleID uiKey;
    private EnumOpenUIType type;
    private object[] uiParam=null;
    public static UIMask Instance
    {
        get { return instance; }
        set { instance = value ; }
    }

    public UIMask()
    {
        instance = this;
    }
    protected override void OnLoadData()
    {
        Show();
        base.OnLoadData();
    }

    public override UIPanleID GetUIKey()
    {
        return UIPanleID.UIMask;
    }
}
