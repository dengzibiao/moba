using UnityEngine;
using System.Collections;

public class GUISingleCheckBox : GUIComponentBase
{
    #region Init
    public delegate void VoidSingleCheckBox(bool state);
    /// <summary>
    /// (bool state)
    /// </summary>
    public VoidSingleCheckBox onClick;

    public delegate void VoidSingleCheckBoxs(int index, bool boo);
    /// <summary>
    /// (int index, bool boo)
    /// </summary>
    public VoidSingleCheckBoxs onGroupClick;
    public VoidSingleCheckBoxs onGroupHover;
    private UISprite _mark;
    private UISprite _mSprite;
    private UILabel _label;
    private bool _state;
    private Color _c = new Color(172 / 255f, 213 / 255f, 255 / 255f);
    #endregion

    protected override void Init()
    {
        if (_mark != null) return;
        _mSprite = GetComponent<UISprite>();
        _mark = transform.Find("mark").GetComponent<UISprite>();
        if(transform.Find("Label")!=null) _label = transform.Find("Label").GetComponent<UILabel>();
        _state = _mark.gameObject.activeInHierarchy;
        this.onComponentClick = OnComponentClick;
        this.onComponentClick += OnGroupClick;
        this.onComponentHover += OnComponentHover;
        this.AddBoxCollider();
    }

    private void OnComponentHover(bool state)
    {
        if (onGroupHover != null) onGroupHover(index, state);
    }

    private void OnComponentClick()
    {
        GuideManager.Single().SetObject(this.gameObject);
        _state = !_state;
        _mark.gameObject.SetActive(_state);
      
        if (onClick != null) onClick(_state);
    }

    /// <summary>
    /// 按钮是否不可用
    /// </summary>
    public bool isEnabled
    {
        set { this.enabled = value; }
        get { return this.enabled; }
    }
    public string spriteName
    {
        set { if (value != null) _mSprite.spriteName = value; }
        get { return _mSprite.spriteName; }
    }
    public bool isSelected
    {
        set
        {
            this._state = value; _mark.gameObject.SetActive(value);
            if (_label!=null)
            {
                if (_state) _label.color = Color.white;
                if (!_state) _label.color = _c;
            }
           
        }
        get { return _state; }
    }
    public bool isHideCheckBoxSprite
    {
        set
        {
            if (transform.GetComponent<UISprite>()!=null)
            {
                transform.GetComponent<UISprite>().enabled = value;
            }
        }
        get { return _state; }
    }
    /// <summary>
    /// 多选点击
    /// </summary>
    private void OnGroupClick()
    {
        if (onGroupClick != null) onGroupClick(index, isSelected);
    }

    public int index { set; get; }

    /// <summary>
    /// 清除点击事件
    /// </summary>
    public void ClearListener()
    {
        this.onComponentClick -= OnComponentClick;
    }
}
