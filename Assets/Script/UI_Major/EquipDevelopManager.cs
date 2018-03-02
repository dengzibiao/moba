using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EquipDevelopManager{

    private static EquipDevelopManager single;
    public static EquipDevelopManager Single()
    {
        if (single == null)
        {
            single = new EquipDevelopManager();

        }
        return single;
    }


    //装备升级成功后的数据
    public Dictionary<string, object> item;
    public HeroData hd;
}
