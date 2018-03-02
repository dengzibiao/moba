using UnityEngine;
using System.Collections;

public enum ProgressState
{
    FLOAT, STRING
}
public class GUISingleProgressBar : GUIComponentBase
{

    #region Init
    public delegate void VoidSingleProgressBar(float percent);
    /// <summary>
    /// float percent
    /// </summary>
    public VoidSingleProgressBar onChange;

    private UISprite _up;
    private UILabel _label;
    private float _count = 0;
    #endregion
    protected override void Init()
    {
        _up = transform.Find("up").GetComponent<UISprite>();
        _label = transform.GetComponentInChildren<UILabel>();
        maxWidth = this.GetComponent<UIWidget>().width;

        _up.pivot = UIWidget.Pivot.Left;
        _up.width = 1;
        _count = currentValue = 0.001f;
        state = ProgressState.FLOAT;
        this.AddBoxCollider();
    }

    void Update()
    {
        if (_count != currentValue)
        {
            ValueToWidth();
            if (onChange != null) onChange(currentValue / maxValue);
        }
    }

    private void ValueToWidth()
    {
        float _value = currentValue / maxValue;
        //0-1
        if (state == ProgressState.FLOAT)
        {
            _label.text = (_value * 100).ToString("0") + " %";
        }
        else
        {
            _label.text = (int)currentValue + "/" + maxValue;
        }
        _count = currentValue;

        if (_up.type == UIBasicSprite.Type.Filled)
        {
            _up.width = maxWidth;
            _up.fillAmount = _value;
        }
        else
        {
            _up.width = (int)(_value * maxWidth);
            if (_up.width >= maxWidth)
            {
                _up.width = maxWidth;
            }
        }

    }
    public void InValue(float current, float max)
    {
        maxValue = max;
        currentValue = current;
    }
    /// <summary>
    /// 按钮是否不可用
    /// </summary>
    public bool isEnabled
    {
        set { this.enabled = value; }
        get { return this.enabled; }
    }
    /// <summary>
    /// 0-1
    /// </summary>
    public float currentValue { set; get; }
    public float maxValue { set; get; }///最大数值
    public ProgressState state { set; get; }///最大数值
    public int maxWidth { set; get; }///最大宽度
}
