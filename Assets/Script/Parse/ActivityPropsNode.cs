using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tianyu;
using System;

public class ActivityPropsNode : FSDataNodeBase
{

    public int wellen;
    public float interval;

    public object[] monster_simple;
    public object[] monster_ordinary;
    public object[] monster_difficult;
    public object[] monster_great;
    public object[] monster_nightmare;
    public object[] monster_abyss;


    public override void parseJson(object jd)
    {
        Dictionary<string, object> items = jd as Dictionary<string, object>;

        wellen = Convert.ToInt32(items["wellen"]);

        if (wellen < 2000)
            SceneActivityProps.POWERCOUNT++;
        else if (wellen >= 2000 && wellen < 3000)
            SceneActivityProps.AGILECOUNT++;
        else if (wellen >= 3000)
            SceneActivityProps.INTELCOUNT++;

        interval = float.Parse(items["interval"].ToString());

        monster_simple = items["monster_simple"] as object[];
        monster_ordinary = items["monster_ordinary"] as object[];
        monster_difficult = items["monster_difficult"] as object[];
        monster_great = items["monster_great"] as object[];
        monster_nightmare = items["monster_nightmare"] as object[];
        monster_abyss = items["monster_abyss"] as object[];
    }

}
