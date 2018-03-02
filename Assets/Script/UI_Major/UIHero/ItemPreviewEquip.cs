using UnityEngine;
using System.Collections;

/// <summary>
/// 装备一览
/// </summary>

public class ItemPreviewEquip : GUISingleItemList
{

    GUISingleButton icon;        //装备边框
    UISprite border;             //装备Icon

    BackpackEquipVO equipVO;

    /// <summary>
    /// item初始化函数
    /// </summary>
    protected override void InitItem()
    {
        //初始化
        icon = transform.Find("Icon").GetComponent<GUISingleButton>();
        border = GetComponent<UISprite>();
        icon.onClick = OnIconClick;
    }

    public override void Info(object obj)
    {

        equipVO = (BackpackEquipVO)obj;

        switch (equipVO.qualitytype)
        {
            case 1:
                border.spriteName = "hui";
                break;
            case 2:
                border.spriteName = "lv";
                break;
            case 3:
                border.spriteName = "lan";
                break;
            case 4:
                border.spriteName = "zi";
                break;
            case 5:
                border.spriteName = "cheng";
                break;
            case 6:
                border.spriteName = "hong";
                break;
            default:
                break;
        }

        icon.spriteName = equipVO.id + equipVO.name;

    }

    private void OnIconClick()
    {

        EquipInfoPanel equip = Control.Show(UIPanleID.UIEquipInfoPanel) as EquipInfoPanel;

        //equip.vo = equipVO;


        Control.Show(UIPanleID.UIEquipInfoPanel);

    }



}