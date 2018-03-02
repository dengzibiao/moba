using UnityEngine;
using System.Collections.Generic;

public enum PopAlign
{
    UP, Down
}
public class GUISinglePopupList : GUIComponentBase
{

    #region Init
    public delegate void VoidSinglePopupList(int index);
    /// <summary>
    /// (bool state)
    /// </summary>
    public VoidSinglePopupList onChange;

    private List<GUISingleButton> _list = new List<GUISingleButton>();
    private UISprite _floor;
    private UISprite _arrow;
    private UILabel _label;
    private GUISingleButton _item;

    private string[] _lines;
    private int _width;
    private int _height;
    private int _index;
    #endregion
    protected override void Init()
    {
        if(_floor != null) return;
        _floor = transform.FindComponent<UISprite>("Floor");
        _floor.pivot = UIWidget.Pivot.TopLeft;
        _arrow = transform.FindComponent<UISprite>("Arrow");
        _label = transform.FindComponent<UILabel>("Label");
        _item = transform.FindComponent<GUISingleButton>("Item");
        _list.Add(_item);
        _height = _item.GetComponent<UIWidget>().height;
        align = PopAlign.Down;

        this.onComponentClick = OnComponentClick;
        this.AddBoxCollider();
    }
    private void OnComponentClick()
    {
        ShowOrHide();
    }
    private void ItemClick(int index)
    {
        currentIndex = index;
        _label.text = _list[index].text;
        ShowOrHide();

        if(onChange != null) onChange(index);
    }
    public void ShowOrHide()
    {
        _floor.gameObject.ShowOrHide();
        _arrow.transform.ReverScale(1, -1, 1);
        foreach(GUISingleButton item in _list)
        {
            item.gameObject.SetActive(_floor.isVisible);
        }
    }
    public void AddItem(params string[] lines)
    {
        _lines = lines;

        for(int i = 0; i < lines.Length; i++)
        {
            if(i != 0)
            {
                GameObject go = NGUITools.AddChild(this.gameObject, _item.gameObject);
                go.transform.localPosition = _item.transform.localPosition;
                int ut = align == PopAlign.Down ? -1 : 1;
                go.transform.SetLocalVector3(0f, ut * (3 + _height) * i, 0f);
                _list.Add(go.GetComponent<GUISingleButton>());
            }

            _list[i].InitUI();
            _list[i].index = i;
            _list[i].text = lines[i];
            _list[i].onItemClick = ItemClick;
        }

        _floor.height = _list.Count * (_height + 8);
        _label.text = _item.text;
        currentIndex = 0;
    }
    public int currentIndex
    {
        set { this._index = value; this._label.text = _lines[value]; }
        get { return _index; }
    }
    public PopAlign align { set; get; }
    /// <summary>
    /// 按钮是否不可用
    /// </summary>
    public bool isEnabled
    {
        set { this.enabled = value; }
        get { return this.enabled; }
    }


}
