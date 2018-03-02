using UnityEngine;
using System.Collections;
using System;

public class TaskItemOnTaskInfo : GUISingleItemList
{
    private bool isSetItemData;
    public GUISingleButton itembtn;

    //void OnPress(GameObject go, bool state)
    //{
    //    Control.GetGUI(GameLibrary.UITaskInfoPanel).GetComponentInChildren<UIItemTips>(true).gameObject.SetActive(state);

    //    if (state && !isSetItemData)
    //    {
    //        GetComponentInChildren<UIDragScrollView>().enabled = false;
    //        Control.GetGUI(GameLibrary.UITaskInfoPanel).GetComponentInChildren<UIItemTips>(true).SetItemInfo(itemdata);
    //        isSetItemData = true;
    //    }
    //    else if (!state)
    //    {
    //        GetComponentInChildren<UIDragScrollView>().enabled = true;
    //        isSetItemData = false;
    //    }
    //}


    //物品图标
    public GUISingleSprite itemicon;
    //品质边框
    public GUISingleSprite qualityframe;
    //物品数量
    public GUISingleLabel itemcount;
    public Transform debris;
    //当前物品
    public ItemData itemdata;
    protected override void InitItem()
    {
        itemicon = transform.Find("Itemicon").GetComponent<GUISingleSprite>();
        qualityframe = transform.Find("Qualityframe").GetComponent<GUISingleSprite>();
        itemcount = transform.Find("Itemcount").GetComponent<GUISingleLabel>();
        debris = transform.Find("Debris");
        //UIEventListener.Get(itemicon.gameObject).onPress = OnPress;
        //UIEventListener.Get(itemicon.gameObject).onClick = OnClick;
    }

    private void OnClick(GameObject go)
    {
        UIgoodstips.Instances.SetItemData(itemdata);
        Control.Show(UIPanleID.UIgoodstips);
    }

    public override void Info(object obj)
    {
        itemdata = (ItemData)obj;
        if (null != obj)
        {
            if (itemdata.Types == 6)
            {
                debris.gameObject.SetActive(true);
            }
            else
            {
                debris.gameObject.SetActive(false);
            }
            itemicon.uiAtlas = itemdata.UiAtlas;
            itemicon.spriteName = itemdata.IconName;
            itemcount.text = itemdata.Count.ToString();
            qualityframe.spriteName = ItemData.GetFrameByGradeType(itemdata.GradeTYPE);
            
            //string qualitycolor = "123";
            //GameLibrary.QualityColor.TryGetValue(itemdata.GradeTYPE, out qualitycolor);
            //if (!string.IsNullOrEmpty(qualitycolor))
            //{
            //    qualityframe.spriteName = qualitycolor;
            //}
            //else
            //{
            //    //金币和钻石 品质框默认为白
            //    qualityframe.spriteName = GameLibrary.QualityColor[GradeType.Gray];
            //}
        }
    }



}