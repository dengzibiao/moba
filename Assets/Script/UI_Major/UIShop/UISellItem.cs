/*
文件名（File Name）:   UISellItem.cs

作者（Author）:    #高#

创建时间（CreateTime）:  2016-7-2 10:36:3
*/
using UnityEngine;
using System.Collections;

public class UISellItem : GUISingleItemList
{

    public UISprite icon;
    public GUISingleLabel nameTxt;
    public GUISingleLabel label;
    private GameObject uiPopBuy;
    private object item;
    private UISprite frameSprite;
    public  GUISingleLabel countTxt;
    /// <summary>
    /// item初始化函数
    /// </summary>
    protected override void InitItem()
    {
        //初始化
        icon = transform.Find("Icon").GetComponent<UISprite>();
        nameTxt = transform.Find("NameTxt").GetComponent<GUISingleLabel>();
        label = transform.Find("Label").GetComponent<GUISingleLabel>();
        countTxt= transform.Find("CountTxt").GetComponent<GUISingleLabel>();
    }

    protected override void OnComponentHover(bool state)
    {

    }
    public override void Info(object obj)
    {
        //ID 道具表
        item = obj;
        if (obj == null)
        {
            nameTxt.text = "";
        }
        else
        {
            nameTxt.text = ((ItemData)obj).Name;
            label.text = ((ItemData)obj).Count.ToString();
            icon.spriteName = ((ItemData)obj).IconName;
            countTxt .text= "拥有[36BD47FF]"+((ItemData)obj).Count + "[-]件";
            A_Sprite.spriteName = ItemData.GetFrameByGradeType((GradeType)((ItemData)obj).GradeTYPE);
        }
    }
}
