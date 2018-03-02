using UnityEngine;
using System.Collections;
using Tianyu;
using System;
using System.Collections.Generic;

public class EquipUpgradeNode : FSDataNodeBase
{

    public int id;
    public long consume;

    public override void parseJson(object jd)
    {
        Dictionary<string, object> item = jd as Dictionary<string, object>;
        id = int.Parse(item["id"].ToString());
        consume = long.Parse(item["consume"].ToString());
    }
}
