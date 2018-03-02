using UnityEngine;
using System.Collections;
using Tianyu;
using System;
using System.Collections.Generic;

public class UIGameAfficheNode : FSDataNodeBase
{
    public int id;
    public string notice_types;
    public string notice;
    public float version;

    public override void parseJson(object jd)
    {
        Dictionary<string, object> item = jd as Dictionary<string, object>;
        id = int.Parse(item["id"].ToString());
        notice_types = item["notice_types"].ToString();
        notice = item["notice"].ToString();
        version = float.Parse(item["version"].ToString());

    }
}