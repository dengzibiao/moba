using UnityEngine;
using System.Collections;

public enum CheckBoxGroupState
{
    Single, Mult
}
public class GUISingleCheckBoxGroup : GUIComponentBase
{

    #region Init
    public delegate void VoidCheckBoxsGroup(int index, bool boo);
    /// <summary>
    /// (int index, bool boo)
    /// </summary>
    public VoidCheckBoxsGroup onClick;
    public VoidCheckBoxsGroup onHover;
    private GUISingleCheckBox[] boxs;

    protected override void Init()
    {
        base.Init();

        this.boxs = this.GetComponentsInChildren<GUISingleCheckBox>();
        this.state = CheckBoxGroupState.Single;
        for (int i = 0; i < boxs.Length; i++)
        {
            boxs[i].index = i;
            boxs[i].onGroupClick = OnComponentClick;
        }
        for (int i = 0; i < boxs.Length; i++)
        {
            boxs[i].index = i;
            boxs[i].onGroupHover = OnComponentHover;
        }
    }

    public void OnComponentHover(int index, bool boo)
    {
        if (onHover != null) onHover(index, boo);
    }

    #endregion


    /// <summary>
    /// 是否不可用
    /// </summary>
    public bool isEnabled
    {
        set
        {
            foreach (GUISingleCheckBox item in boxs)
            {
                item.isEnabled = value;
            }
        }
    }
    /// <summary>
    /// 单选，多选
    /// </summary>
    public CheckBoxGroupState state { set; get; }

    /// <summary>
    /// 当前选择状态
    /// </summary>
    public int index { set; get; }

    /// <summary>
    /// 当前点击选择框状态
    /// </summary>
    private void OnComponentClick(int index, bool boo)
    {
        onClick(index, boo);

        if (this.state == CheckBoxGroupState.Single)
        {
            foreach (GUISingleCheckBox item in boxs)
            {
                item.isSelected = item.index == index ? true : false;
            }
        }
    }
    /// <summary>
    /// 返回 checkbox 序列
    /// </summary>
    /// <returns></returns>
    public GUISingleCheckBox[] GetBoxList()
    {
        return this.boxs;
    }

    /// <summary>
    /// 返回单选情况下选中的index
    /// </summary>
    /// <returns></returns>
    public int GetSeletIndex()
    {
        return index;
    }

    /// <summary>
    /// 返回选择情况，1:选中，0:未选中
    /// </summary>
    /// <returns></returns>
    public int[] GetIndexList()
    {
        int[] list = new int[] { boxs.Length };

        for (int i = 0; i < boxs.Length; i++)
        {
            list[i] = boxs[i].isSelected ? 1 : 0;
        }

        return list;
    }

    /// <summary>
    /// 默认选中状态index
    /// </summary>
    public int DefauleIndex
    {
        set
        {
            boxs[value].isSelected = true;

            OnComponentClick(value, true);
        }
    }
    /// <summary>
    /// 主动改变页签选择状态
    /// </summary>
    /// <param name="index"></param>
    public void setMaskState(int index)
    {
        if (this.state == CheckBoxGroupState.Single)
        {
            foreach (GUISingleCheckBox item in boxs)
            {
                item.isSelected = item.index == index ? true : false;
            }
        }
    }

    public void HideCheckBoxSprite(int index)
    {
        if (this.state == CheckBoxGroupState.Single)
        {
            foreach (GUISingleCheckBox item in boxs)
            {
                item.isHideCheckBoxSprite = item.index == index ? false : true;
            }
        }
    }
}
