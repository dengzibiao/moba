using UnityEngine;
using Tianyu;
using System.Collections;
using System.Collections.Generic;
using System;

public class MapInfoNode : FSDataNodeBase
{
    public UInt32 key;
    public string MapName;
    public float Zmin;
    public float Zmax;
    public float Zlength;
    public float Xmin;
    public float Xmax;
    public float Xlength;
    public int map_info;

    public override void parseJson ( object jd )
    {
        Dictionary<string , object> item = ( Dictionary<string , object> ) jd;
        key = UInt32.Parse( item [ "key" ].ToString() );
        MapName = item [ "MapName" ].ToString();
        Xmin = float.Parse(item["Xmin"].ToString());
        Xmax = float.Parse(item["Xmax"].ToString());
        Zmin = float.Parse(item["Zmin"].ToString());
        Zmax = float.Parse(item["Zmax"].ToString());
        Xlength = float.Parse(item["Xlength"].ToString());
        Zlength = float.Parse(item["Zlength"].ToString());
        map_info = int.Parse( item [ "map_info" ].ToString() );
    }
}
