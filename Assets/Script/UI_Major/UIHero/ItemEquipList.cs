using UnityEngine;
using System.Collections;

/// <summary>
/// 装备按钮
/// </summary>

public class ItemEquipList : GUISingleItemList
{

    public GUISingleButton mailBtn; //装备按钮
    public UISprite icon;           //装备底板
    public UILabel label;           //装备等级

    /// <summary>
    /// item初始化函数
    /// </summary>
    protected override void InitItem()
    {
        //初始化
        mailBtn = transform.Find("MailBtn").GetComponent<GUISingleButton>();
        mailBtn.onClick = OnBtnClick;
    }

    public override void Info(object obj)
    {
        if (obj == null)
        {
            //icon.spriteName = item.icon;
            label.text = "";
        }
        else
        {
            //icon.spriteName = item.icon;
            label.text = Random.value * 10 + "";
        }

    }

    /// <summary>
    /// 装备按钮
    /// </summary>
    private void OnBtnClick()
    {
        print("装备" + index);
    }
}