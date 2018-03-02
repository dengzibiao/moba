using UnityEngine;
using System.Collections;
using Tianyu;
using System;
using System.Collections.Generic;

public class LevelConfigBase : FSDataNodeBase
{
    protected Dictionary<string, object> item;
    public int id;
    public int sceneid;
    public Vector3 modelPos;
    public float modelRot;
    public int modelID;
    public float modellvl;
    public float startTime;
    public float intervals;
    public int groupIndex;
    public int type;

    float[] pos;

    public override void parseJson(object jd)
    {
        item = jd as Dictionary<string, object>;
        id = Convert.ToInt32(item["id"]);
        sceneid = Convert.ToInt32(item["sceneid"]);
        LoadPos(item["modelPos"], ref modelPos);
        modelRot = float.Parse(item["modelRot"].ToString());
        modelID = Convert.ToInt32(item["modelID"]);
        modellvl = float.Parse(item["modellvl"].ToString());
        startTime = float.Parse(item["startTime"].ToString());
        intervals = float.Parse(item["intervals"].ToString());
        groupIndex = Convert.ToInt32(item["groupIndex"]);
        type = Convert.ToInt32(item["type"]);
    }

    void LoadPos(object items, ref Vector3 vector)
    {
        pos = FSDataNodeTable<MobaLevelConfigNode>.GetSingleton().ParseToFloatArray(items);
        if (null != pos && pos.Length > 0)
        {
            vector = new Vector3(pos[0], pos[1], pos[2]);
            pos = null;
        }
    }

}
