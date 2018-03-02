using UnityEngine;
using System.Collections;
using System;

public class GUISingleSprite : GUIComponentBase
{

    #region Init
    public delegate void VoidSingleSprite();
    public VoidSingleSprite onClick;

    private UISprite sprite;
    protected override void Init()
    {
        sprite = this.GetComponentInChildren<UISprite>(true);      
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
        if (onClick != null) this.onClick();
    }
    #endregion
    public void ChangeColor(bool value)
    {
        ChangeColorGray.Instance.ChangeSpriteColor(sprite, value);
    }
    public float fillAmount
    {
        set { sprite.fillAmount = value; }
        get { return sprite.fillAmount; }
    }
    public string spriteName
    {
        set { if(value!=null) sprite.spriteName = value; }
        get { return sprite.spriteName; }
    }

    public UIWidget.Pivot pivot
    {
        set { sprite.pivot = value; }
        get { return sprite.pivot; }
    }
    public int width
    {
        set { sprite.width = value; }
        get { return sprite.width; }
    }
    public UIAtlas uiAtlas
    {
        set { sprite.atlas = value; }
        get { return sprite.atlas; }
    }
    public void MakePixelPerfect()
    {
        sprite.MakePixelPerfect();
    }
}
