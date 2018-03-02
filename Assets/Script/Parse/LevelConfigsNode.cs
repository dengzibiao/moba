using UnityEngine;
using System.Collections;
using Tianyu;
using System.Collections.Generic;
using System;

public class LevelConfigsNode : FSDataNodeBase
{

    public int id;
    public int monsterID;
    public float monsterlvl;
    public float scale;

    public override void parseJson(object jd)
    {
        Dictionary<string, object> item = (Dictionary<string, object>)jd;
        id = Convert.ToInt32(item["id"]);
        monsterID = Convert.ToInt32(item["monsterID"]);
        monsterlvl = float.Parse(item["monsterlvl"].ToString());
        scale = float.Parse(item["scale"].ToString());
    }

}
