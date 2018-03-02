using UnityEngine;
using System.Collections;

public class ItemEMatList : GUISingleItemList
{

    GUISingleButton icon;         //装备
    UILabel count;
    ItemNodeState itemNode;
    ItemData item;


    /// <summary>
    /// item初始化函数
    /// </summary>
    protected override void InitItem()
    {
        //初始化
        icon = transform.Find("Icon").GetComponent<GUISingleButton>();
        count = transform.Find("Label").GetComponent<UILabel>();
        //icon.onClick = OnBtnClick;

    }

    int needMat = 0;

    public override void Info(object obj)
    {

        itemNode = (ItemNodeState)obj;

        if (obj == null)
        {
            //icon.spriteName = item.icon;
        }
        else
        {
            icon.spriteName = itemNode.icon_name;

            //获取背包中所需材料的个数
            item = playerData.GetInstance().GetItemDatatByID(itemNode.props_id);
            count.color = Color.red;
            //背包中是否有该材料
            if (null == item)
            {
                count.text = 0 + "/" + EquipInfoPanel.instance.matCountM;
            }
            else
            {
                //计算可合成数量
                count.text = item.Count + "/" + EquipInfoPanel.instance.matCountM;
                
                if (item.Count >= EquipInfoPanel.instance.matCountM)
                {
                    count.color = Color.white;
                    EquipInfoPanel.instance.isNood = true;
                }
            }
        }

    }

    private void OnBtnClick()
    {

        //隐藏装备信息，显示材料信息
        EquipInfoPanel.instance.HideEquipInfo(false, itemNode, needMat);

        EquipInfoPanel.instance.lastItem = itemNode;


    }

}