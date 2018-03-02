using UnityEngine;
using System.Collections;

public class GUIComponentBase : MonoBehaviour
{
    #region Init
    public delegate void VoidComponentDelegate();
    public delegate void BoolComponentDelegate(bool state);
    public delegate void FloatComponentDelegate(float delta);
    public delegate void KeyCodeComponentDelegate(KeyCode key);

    protected VoidComponentDelegate onComponentClick;
    protected VoidComponentDelegate onComponentDoubleClick;
    protected BoolComponentDelegate onComponentHover;
    protected BoolComponentDelegate onComponentPress;
    protected BoolComponentDelegate onComponentSelect;
    protected FloatComponentDelegate onComponentScroll;
    protected KeyCodeComponentDelegate onComponentKey;
    #endregion


    bool isColliderEnabled
    {
        get
        {
            Collider c = GetComponent<Collider>();
            if(c != null) return c.enabled;
            Collider2D b = GetComponent<Collider2D>();
            return (b != null && b.enabled);
        }
    }
    void OnClick()
    {
        if(isColliderEnabled && onComponentClick != null) onComponentClick();
    }
    void OnDoubleClick()
    {
        if(isColliderEnabled && onComponentDoubleClick != null) onComponentDoubleClick();
    }
    void OnHover(bool isOver)
    {
        if(isColliderEnabled && onComponentHover != null) onComponentHover(isOver);
    }
    void OnPress(bool isPressed)
    {
        if(isColliderEnabled && onComponentPress != null) onComponentPress(isPressed);
    }

    void OnSelect(bool selected)
    {
        if(isColliderEnabled && onComponentSelect != null) onComponentSelect(selected);
    }
    void OnScroll(float delta)
    {
        if(isColliderEnabled && onComponentScroll != null) onComponentScroll(delta);
    }

    void OnKey(KeyCode key)
    {
        if(isColliderEnabled && onComponentKey != null) onComponentKey(key);
    }
    public bool isDrag
    {
        set
        {
            this.GetComponent<GUISingleDragObject>().isDrag = value;
        }
    }

    //void OnHover(bool isOver):当鼠标移出或者悬停在某个碰撞器上的时候返回布尔值.在触摸设备上不会有作用.
    //void OnPress(bool isDown):当鼠标或者触摸到碰撞器发生布尔值返回.
    //void OnSelect(bool selected):当鼠标或者触摸从OnPress发生后的释放将会返回这个布尔值.
    //void OnClick():和OnSelect的产生条件相同,当点击或触摸碰撞器并且没有发生拖拽时候触发.
    //void OnDrag(Vector2 delta):当移动鼠标或者触摸按下时候位移超过特定阀值时触发.
    //void OnDrop(GameObject drag):当鼠标或触摸释放于从发生OnDrag的不同碰撞器伤触发.传递的参数是产生OnDrag的游戏对象.
    //void OnInput(string text):当一个OnSelect发生后在同一个碰撞器上触发输入.一般只有UIInput用它.
    //void OnTooltip(bool show):当鼠标悬停超过tooltipDelay时间后触发该命令.触摸设备上不会有作用.
    //可以用UICamera.lastCamera找到谁发出的事件,可用UICamera.lastHit得到谁接受这个事件,以及用UICamera.lastTouchPosition得到触摸或屏幕的位置

    void Awake()
    {
        this.Init();
    }

    void Start()
    {
        OnStart();
    }
    /// <summary>
    /// 初始化
    /// </summary>
    protected virtual void Init() { }
    protected virtual void OnStart() { }

    /// <summary>
    ///
    /// </summary>
    /// <param name="isReset">判断是否需要重置widget</param>
    protected void AddBoxCollider(bool isReset = false)
    {
        UIWidget widget = this.GetComponent<UIWidget>();
        if(isReset)
        {
            widget.height = 1;
            widget.width = 1;

            Bounds bound = NGUIMath.CalculateRelativeWidgetBounds(this.transform, false);
            widget.height = (int)bound.size.y;
            widget.width = (int)bound.size.x;
        }
        if (widget != null)
        {
            bool vis = widget.isVisible;

            if (!vis) this.gameObject.SetActive(true);
                     
            this.gameObject.SetActive(vis);
        }
        NGUITools.AddWidgetCollider(this.gameObject, true);
    }
    public void Show()
    {
        if (gameObject != null)
        {
            if (gameObject.activeSelf)
            {
                gameObject.SetActive(false);
            }
            gameObject.SetActive(true);
        }
    
    }

    public void ShowOrHide(bool isShow)
    {
        if (isShow)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }
    public void Hide()
    {
        this.gameObject.SetActive(false);
    }
    private Hashtable table = new Hashtable();
    /// <summary>
    /// 获取key 对应的 T
    /// </summary>
    public void In<T>(Transform parent, string key)
    {
        table.Add(key, parent.FindComponent<T>(key));
    }
    /// <summary>
    /// 获取key 对应的 Transform
    /// </summary>
    public void In(Transform parent, string key)
    {
        table.Add(key, parent.Find(key));
    }
    /// <summary>
    /// 返回key对应的obj
    /// </summary>
    public Transform GetTransform(string key)
    {
        return table[key] as Transform;
    }
    public UILabel GetLabel(string key)
    {
        return table[key] as UILabel;
    }
    public UISprite GetSprite(string key)
    {
        return table[key] as UISprite;
    }
    public GUISingleButton GetButton(string key)
    {
        return table[key] as GUISingleButton;
    }
    public GUISingleCheckBox GetCheckBox(string key)
    {
        return table[key] as GUISingleCheckBox;
    }
    public GUISingleCheckBoxGroup GetCheckBoxGroup(string key)
    {
        return table[key] as GUISingleCheckBoxGroup;
    }
    public GUISingleInput GetInput(string key)
    {
        return table[key] as GUISingleInput;
    }
    public GUISinglePopupList GetPopupList(string key)
    {
        return table[key] as GUISinglePopupList;
    }
    public GUISingleProgressBar GetProgressBar(string key)
    {
        return table[key] as GUISingleProgressBar;
    }
    public GUISingleMultList GetMultList(string key)
    {
        return table[key] as GUISingleMultList;
    }
    public GUISingleItemList GetItemList(string key)
    {
        return table[key] as GUISingleItemList;
    }
}
