using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tianyu;
using System;

public class RouteNode : FSDataNodeBase
{

    public long key;
    public int sn;
    public long from;
    public long to;
    public int from_mapid;
    public int to_mapid;
    public override void parseJson(object jd)
    {
        Dictionary<string, object> item = (Dictionary<string, object>)jd;
        key = long.Parse(item["key"].ToString());
        sn = int.Parse(item["sn"].ToString());
        from = long.Parse(item["from"].ToString());
        to = long.Parse(item["to"].ToString());
        from_mapid = int.Parse(item["from_map_id"].ToString());
        to_mapid = int.Parse(item["to_map_id"].ToString());
    }
}
