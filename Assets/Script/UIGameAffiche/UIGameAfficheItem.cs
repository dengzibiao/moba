using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tianyu;
using System;

public class UIGameAfficheItem : GUISingleItemList
{
    public GUISingleButton icon;
    public UILabel GameAffichetext;
    public UISprite GameAfficheImg;
    private object item;
    private int Some = 1;
    private UIGameAfficheItem Item;
    protected override void InitItem()
    {
        icon = transform.Find("Icon").GetComponent<GUISingleButton>();
        GameAfficheImg = transform.Find("GameAfficheImg").GetComponent<UISprite>();
        GameAffichetext = transform.Find("GameAffichetext").GetComponent<UILabel>();
        icon.onClick = OnIconClick;
    }
    //点击切换操作
    private void OnIconClick()
    {
        GameAfficheImg.transform.gameObject.SetActive(true);
        UIGameAffiche.instance.OnTabClick(index, this.gameObject);
    }
    public override void Info(object obj)
    {
        if (obj == null)
        {

        }
        else
        {
            if (index == 0)
            {
                OnIconClick();
            }
            item = obj;
            if (((UIGameAfficheNode)obj).id != null)
            {
                GameAffichetext.text = ((UIGameAfficheNode)obj).notice_types;
            }
        }
    }
}
