using UnityEngine;
using System.Collections;
using System;

public class GoodsItem : GUISingleItemList
{
    public UISprite sprite;
    //public GUISingleButton icon;
    public GUISingleSprite icon;
    public GUISingleLabel count;
    public Transform xuanZhongIcon;
    public Transform debris;
    private ItemData item;
    protected override void InitItem()
    {
        sprite = transform.GetComponent<UISprite>();
        //icon = transform.Find("Icon").GetComponent<GUISingleButton>();
        count = transform.Find("Count").GetComponent<GUISingleLabel>();
        xuanZhongIcon = transform.Find("XuanZhongIcon");
        debris = transform.Find("Debris");
        if (index == 0)
        {
            transform.gameObject.SetActive(true);
        }
        icon.onClick = OnIconClick;
        //icon.onClick = OnIconClick;
    }

    private void OnIconClick()
    {
        //xuanZhongIcon.gameObject.SetActive(true);
        Globe.seletIndex = index;
        GoodsDetials.instance.SetData(item);
        //Singleton<GoodsDetials>.Instance.SetData(item);
    }
    
    public override void Info(object obj)
    {
        item = (ItemData)obj;
        if (null != obj)
        {
            icon.spriteName = item.IconName;

            count.text = ((ItemData)obj).Count.ToString();
            sprite.spriteName = ItemData.GetFrameByGradeType(item.GradeTYPE);
            if (item.Types == 6||item.Types == 3)
            {
                debris.gameObject.SetActive(true);
            }
            else
            {
                debris.gameObject.SetActive(false);
            }
            if (item.Types == 3)
            {
                debris.GetComponent<UISprite>().spriteName = "materialdebris";
                debris.GetComponent<UISprite>().MakePixelPerfect();
            }
            else if (item.Types == 6)
            {
                debris.GetComponent<UISprite>().spriteName = "linghunshi";
                debris.GetComponent<UISprite>().MakePixelPerfect();
            }
            icon.uiAtlas = item.UiAtlas;
        }   
    }

    public void Update()
    {
        if (index == Globe.seletIndex)
        {
            xuanZhongIcon.gameObject.SetActive(true);
        }
        else
        {
            xuanZhongIcon.gameObject.SetActive(false);
        }
    }
}
