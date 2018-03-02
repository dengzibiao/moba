using UnityEngine;
using System.Collections;

public class MailGoodsItem : GUISingleItemList {

    public UISprite sprite;
    public UISprite getMask;
    public GUISingleButton icon;
    public GUISingleLabel count;
    public Transform xuanZhongIcon;
    private MailAccessoryData item;
    public Transform debris;
    protected override void InitItem()
    {
        sprite = transform.GetComponent<UISprite>();
        icon = transform.Find("Icon").GetComponent<GUISingleButton>();
        count = transform.Find("Count").GetComponent<GUISingleLabel>();
        getMask = transform.Find("GetMask").GetComponent<UISprite>();
        xuanZhongIcon = transform.Find("XuanZhongIcon");
        debris = transform.Find("Debris");
        if (index == 0)
        {
            transform.gameObject.SetActive(true);
        }
        icon.onClick = OnIconClick;
    }

    private void OnIconClick()
    {
        //xuanZhongIcon.gameObject.SetActive(true);
        //Globe.seletIndex = index;
        //GoodsDetials.instance.SetData(item);
        //Singleton<GoodsDetials>.Instance.SetData(item);
    }

    public override void Info(object obj)
    {
        item = (MailAccessoryData)obj;
        if (null != obj)
        {

            if (item.ItemType == 6||item.ItemType == 3)
            {
                debris.gameObject.SetActive(true);
            }
            else
            {
                debris.gameObject.SetActive(false);
            }
            if (item.ItemType == 3)
            {
                debris.GetComponent<UISprite>().spriteName = "materialdebris";
                debris.GetComponent<UISprite>().MakePixelPerfect();
            }
            else if (item.ItemType == 6)
            {
                debris.GetComponent<UISprite>().spriteName = "linghunshi";
                debris.GetComponent<UISprite>().MakePixelPerfect();
            }
            icon.GetComponent<UISprite>().atlas = item.UiAtlas;
            if (GameLibrary.Instance().ItemStateList.ContainsKey(item.Id))
            {
                icon.spriteName = GameLibrary.Instance().ItemStateList[item.Id].icon_name;
            }
                
            if (item.Type == MailGoodsType.ItemType)
            {
                count.text = item.Count.ToString();
            }
            else if (item.Type == MailGoodsType.GoldType)
            {
                count.text = item.Gold.ToString();
                icon.spriteName = "jinbi";
            }
            else if (item.Type == MailGoodsType.DiamomdType)
            {
                count.text = item.Diamond.ToString();
                icon.spriteName = "zuanshi";
            }

            if (item.IsHaveGet)
            {
                getMask.gameObject.SetActive(true);
            }
            else
            {
                getMask.gameObject.SetActive(false);
            }
            sprite.spriteName = ItemData.GetFrameByGradeType(item.GradeType);
        }
    }
    //public void Update()
    //{
    //    if (index == Globe.seletIndex)
    //    {
    //        xuanZhongIcon.gameObject.SetActive(true);
    //    }
    //    else
    //    {
    //        xuanZhongIcon.gameObject.SetActive(false);
    //    }
    //}
}
