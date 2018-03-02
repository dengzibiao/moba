/*
文件名（File Name）:   UIDownItem.cs

作者（Author）:    #高#

创建时间（CreateTime）:  2016-6-30 20:8:52
*/
using UnityEngine;
using System.Collections;

public class UIDownItem : GUISingleItemList
{

    public UISprite aSprite;
    public UISprite icon;
    public UILabel label;
    public UILabel nameTxt;
    private object item;
    public UISprite effect;
    protected override void InitItem()
    {
        ////初始化
        aSprite = GetComponent<UISprite>();
        icon = transform.Find("Icon").GetComponent<UISprite>();
        label = transform.Find("Label").GetComponent<UILabel>();
        nameTxt = transform.Find("NameTxt").GetComponent<UILabel>();
        effect = transform.Find("Effect").GetComponent<UISprite>();

    }
    public override void Info(object obj)
    {
        //ID 道具表
        // object _item = obj;



        if (obj == null)
        {
            icon.spriteName = "";
            label.text = "";
            nameTxt.text = "";
            effect.spriteName = "";
        }
        else
        {
           
          
            nameTxt.text = ((ItemData)obj).Name;
            label.text = ((ItemData)obj).Count.ToString();
            icon.spriteName = ((ItemData)obj).IconName;
            switch (((ItemData)obj).GradeTYPE)
            {
                case GradeType.Gray:
                    aSprite.spriteName = "hui";
                    effect.spriteName = "";
                    break;
                case GradeType.Green:
                    aSprite.spriteName = "lv";
                    effect.spriteName = "";
                    break;
                case GradeType.Blue:
                    aSprite.spriteName = "lan";
                    effect.spriteName = "";
                    break;
                case GradeType.Purple:
                    aSprite.spriteName = "zi";
                    PlayEffect.mActive = true;
                    effect.GetComponent<PlayEffect>().Name = "2_000";
                    break;
                case GradeType.Orange:
                    aSprite.spriteName = "cheng";
                    PlayEffect.mActive = true;
                    effect.GetComponent<PlayEffect>().Name = "0_000";
                    break;
                case GradeType.Red:
                    aSprite.spriteName = "hong";
                    break;
            }
        }
    }
}
