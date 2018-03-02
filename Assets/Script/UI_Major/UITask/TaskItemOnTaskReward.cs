using UnityEngine;
using System.Collections;

public class TaskItemOnTaskReward : GUISingleItemList
{
    public GUISingleLabel itemnamelab;
    //物品图标
    public GUISingleSprite itemicon;
    //品质边框
    public GUISingleSprite qualityframe;
    //物品数量
    public GUISingleLabel itemcount;
    //当前物品
    public ItemData itemdata;
    public Transform debris;
    private PlayEffect effect;
    Transform ziEffect;
    Transform chengEffect;
    protected override void InitItem()
    {
        itemicon = transform.Find("Itemicon").GetComponent<GUISingleSprite>();
        qualityframe = transform.Find("Qualityframe").GetComponent<GUISingleSprite>();
        itemcount = transform.Find("Itemcount").GetComponent<GUISingleLabel>();
        effect = transform.Find("Effect").GetComponent<PlayEffect>();
        debris = transform.Find("Debris");
        ziEffect = transform.Find("UI_HDJL_01");
        chengEffect = transform.Find("UI_HDJL_02");
    }

    public override void Info(object obj)
    {
        itemdata = (ItemData)obj;

        if (null != obj)
        {
            itemnamelab.text = itemdata.Name;
            if (itemdata.Types == 6 || itemdata.Types == 3)
            {
                debris.gameObject.SetActive(true);
            }
            else
            {
                debris.gameObject.SetActive(false);
            }
            if (itemdata.Types == 3)
            {
                debris.GetComponent<UISprite>().spriteName = "materialdebris";
                debris.GetComponent<UISprite>().MakePixelPerfect();
            }
            else if (itemdata.Types == 6)
            {
                debris.GetComponent<UISprite>().spriteName = "linghunshi";
                debris.GetComponent<UISprite>().MakePixelPerfect();
            }
            itemicon.uiAtlas = itemdata.UiAtlas;
            //itemicon.spriteName = itemdata.IconName;
            itemcount.text = itemdata.Count.ToString();
            qualityframe.spriteName = ItemData.GetFrameByGradeType(itemdata.GradeTYPE);
            if (itemdata.GradeTYPE == GradeType.Purple)
            {
                ziEffect.gameObject.SetActive(true);
            }
            else if (itemdata.GradeTYPE == GradeType.Orange)
            {
                chengEffect.gameObject.SetActive(true);
            }
            itemicon.spriteName = itemdata.IconName;
        }
    }

}