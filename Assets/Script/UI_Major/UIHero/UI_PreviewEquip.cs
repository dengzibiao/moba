using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

/// <summary>
/// 装备一览面板
/// </summary>

public class UI_PreviewEquip : GUIBase
{
    public GUISingleButton closeBtn;
    public GUISingleMultList equipMultList;

    List<int> equipList = new List<int>();

    object[] equipObj = new object[6];

    public override UIPanleID GetUIKey()
    {
        return UIPanleID.UI_PreviewEquip;
    }

    protected override void Init()
    {

        for (int i = 0; i < equipList.Count; i++)
        {
            equipObj[i] = VOManager.Instance().GetCSV<BackpackEquipCSV>("BackpackEquip").GetVO(equipList[i]);
        }

        closeBtn.onClick = OnCloseClick;
        equipMultList.InSize(6, 3);
        equipMultList.Info(equipObj);

    }

    /// <summary>
    /// 更新装备
    /// </summary>
    public void UpdateEquip(List<int> equipList)
    {
        this.equipList = equipList;
    }

    /// <summary>
    /// 关闭按钮
    /// </summary>
    private void OnCloseClick()
    {
        Hide();
    }


}
