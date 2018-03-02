/*
文件名（File Name）:   GUIDynamicItemList.cs

作者（Author）:    #高#

创建时间（CreateTime）:  #CreateTime#
*/
using UnityEngine;
using System.Collections;

public class GUIDynamicItemList : GUIComponentBase {

    #region Init
    public delegate void VoidItemList(int index);
    public VoidItemList onClick;

    public UISprite A_Sprite;

    protected override void Init()
    {
        A_Sprite = this.GetComponent<UISprite>();
        this.onComponentClick = OnComponentClick;
        this.InitItem();
    }
    private void OnComponentClick()
    {
        if (onClick != null) onClick(index);
    }

    public int index { set; get; }

    #endregion
    protected virtual void InitItem() { }
    public virtual void Info(object obj) { }
}
