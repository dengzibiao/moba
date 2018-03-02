using UnityEngine;
using System.Collections;

public class GUISingleLabel : GUIComponentBase
{
    #region Init
    public delegate void VoidSingleLabel();
    public VoidSingleLabel onClick;

    private UILabel label;
    protected override void Init()
    {
        label = GetComponentInChildren<UILabel>(true);
        this.onComponentClick = OnComponentClick;
    }
    /// <summary>
    /// 开启点击检测
    /// </summary>
    public void AddCollider()
    {
        AddBoxCollider();
    }

    private void OnComponentClick()
    {
        if(onClick != null) this.onClick();
    }
    #endregion

    public string text
    {
        set { if(label!=null) label.text = value; }
        get
        {
            if (label != null) return label.text;
            else return null;
        }
    }
    public Color color
    {
        get
        {
            return label.color;
        }
        set
        {
            if (label.color != value)
            {
                label.color = value;
            }
        }
    }
}
