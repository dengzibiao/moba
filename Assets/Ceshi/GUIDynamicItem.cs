/*
文件名（File Name）:   GUIDynamicItem.cs

作者（Author）:    #高#

创建时间（CreateTime）:  #CreateTime#
*/
using UnityEngine;
using System.Collections;

public class GUIDynamicItem : GUIDynamicItemList
{
    private UILabel lable;
    protected override void InitItem()
    {
       
    }

    public override void Info(object obj)
    {
        if (null==obj)
        {
            
        }
        else
        {
            lable = transform.FindChild("Label").GetComponent<UILabel>();
             lable.text =obj.ToString();
        }
    }
}
