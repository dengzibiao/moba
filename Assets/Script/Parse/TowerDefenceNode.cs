using UnityEngine;
using System.Collections;
using Tianyu;
using System;
using System.Collections.Generic;

public class TowerDefenceNode : FSDataNodeBase
{


    public int Wellen;
    public float Interval;

    public object[] Monster1;
    public object[] Monster2;
    public object[] Monster3;

    public override void parseJson(object jd)
    {
        Dictionary<string, object> item = jd as Dictionary<string, object>;
        Wellen = Convert.ToInt32(item["wellen"]);

        Interval = float.Parse(item["interval"].ToString());

        Monster1 = item["monster1"] as object[];
        Monster2 = item["monster2"] as object[];
        Monster3 = item["monster3"] as object[];

    }
}
