using UnityEngine;
using System.Collections;
using Tianyu;
using System.Collections.Generic;
using System;

public class Moba3v3NaviNode : FSDataNodeBase
{
    protected Dictionary<string, object> item;
    public int id;
    public Vector3 naviPoint0, naviPoint1, naviPoint2,naviPoint3;
    float[] pos;

    public override void parseJson(object jd)
    {
        item = jd as Dictionary<string, object>;
        id = Convert.ToInt32(item["id"]);
        LoadPos(item["NaviPoints0"], ref naviPoint0);
        LoadPos(item["NaviPoints1"], ref naviPoint1);
        LoadPos(item["NaviPoints2"], ref naviPoint2);
        LoadPos(item["NaviPoints3"], ref naviPoint3);
    }

    void LoadPos(object items, ref Vector3 vector)
    {
        pos = FSDataNodeTable<Moba3v3NaviNode>.GetSingleton().ParseToFloatArray(items);
        if (null != pos && pos.Length > 0)
        {
            vector = new Vector3(pos[0], pos[1], pos[2]);
            pos = null;
        }
    }
}
