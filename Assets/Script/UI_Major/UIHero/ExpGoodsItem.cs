using UnityEngine;
using System.Collections;

public class ExpGoodsItem : GUISingleItemList
{

    public UISprite sprite;
    //public GUISingleButton icon;
    public GUISingleSprite icon;
    public GUISingleLabel count;
    public GUISingleLabel value;
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
        Debug.Log("点击药水");
        UIExpPropPanel.instance.selectIndex = index;
        //Control.GetGUI(GameLibrary.UIUseExpVialPanel).GetComponent<UIUseExpVialPanel>().SetData(item);
        //Control.ShowGUI(GameLibrary.UIUseExpVialPanel);
        Control.ShowGUI(UIPanleID.UIUseExpVialPanel, EnumOpenUIType.DefaultUIOrSecond, false, item);
    }

    public override void Info(object obj)
    {
        item = (ItemData)obj;
        if (null != obj)
        {
            icon.spriteName = item.IconName;

            count.text = ((ItemData)obj).Count.ToString();
            sprite.spriteName = ItemData.GetFrameByGradeType(item.GradeTYPE);
            if (GameLibrary.Instance().ItemStateList.ContainsKey(item.Id))
            {
                value.text = "+"+GameLibrary.Instance().ItemStateList[item.Id].exp_gain + "经验";
            }
           
            if (item.Types == 6 || item.Types == 3)
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
        if (index == UIExpPropPanel.instance.selectIndex)
        {
            xuanZhongIcon.gameObject.SetActive(true);
        }
        else
        {
            xuanZhongIcon.gameObject.SetActive(false);
        }
    }
}
