using UnityEngine;
using System.Collections;

public class CommonItem : GUISingleItemList
{
    //物品图标
    public GUISingleSprite itemicon;
    //品质边框
    public GUISingleSprite qualityframe;
    //物品数量
    public GUISingleLabel itemcount;
    //当前物品
    public ItemData itemdata;
    protected override void InitItem()
    {
        itemicon = transform.Find("itemicon").GetComponent<GUISingleSprite>();
        qualityframe = transform.Find("qualityframe").GetComponent<GUISingleSprite>();
        itemcount = transform.Find("itemcount").GetComponent<GUISingleLabel>();
    }

    public override void Info(object obj)
    {
        itemdata = (ItemData)obj;
        if (null != obj)
        {
            //goldId=101, diamondId=10101
            if (itemdata.Id == 101)
            {
                itemicon.spriteName = "jinbi";
            }
            else if (itemdata.Id == 10101)
            {
                itemicon.spriteName = "jinbi";
            }
            else
            {
                itemicon.spriteName = itemdata.IconName;
            }
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