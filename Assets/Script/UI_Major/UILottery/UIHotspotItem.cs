/*
文件名（File Name）:   UIHotspotItem.cs

作者（Author）:    #高#

创建时间（CreateTime）:  2016-6-24 15:48:3
*/
using UnityEngine;
using System.Collections;

public class UIHotspotItem : GUISingleItemList
{


    public UISprite icon;
    public UISprite point;
    private object item;
    private ItemData itemData;

    /// <summary>
    /// item初始化函数
    /// </summary>
    protected override void InitItem()
    {
        //初始化
        icon = transform.Find("Icon").GetComponent<UISprite>();
        point = transform.Find("Point").GetComponent<UISprite>();
    }

    protected override void OnComponentHover(bool state)
    {

    }
    public override void Info(object obj)
    {
        if (index == 3)
        {
            this.transform.localPosition += new Vector3(40f, 0, 0);
        }
        //ID 道具表
  
        if (obj == null)
        {
            
        }
        else
        {
            item = obj;
            itemData= (ItemData)obj;
            point.gameObject.SetActive(false);
            if (int.Parse(itemData.Id.ToString().Substring(0,3))==107|| int.Parse(itemData.Id.ToString().Substring(0, 3)) == 106)
            {
                icon.atlas = ResourceManager.Instance().GetUIAtlas("UIHeroHead");
            }
            else
            {
                icon.atlas = ResourceManager.Instance().GetUIAtlas("Prop");
            }
            if ((int.Parse(StringUtil.SubString(itemData.Id.ToString(), 3)) == 106))
            {
                point.gameObject.SetActive(true);
            }
            icon.spriteName = ((ItemData)obj).IconName;
            A_Sprite.spriteName = ItemData.GetFrameByGradeType(( (ItemData)obj ).GradeTYPE);
        }
    }

}
