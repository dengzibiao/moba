using UnityEngine;
using System.Collections;
using Tianyu;
using System.Collections.Generic;
using System;
public class Moba3SceneConfigNode : LevelConfigBase
{
    //protected Dictionary<string, object> item;
    //public int id;
    //public Vector3 modelPos;
    //public float modelRot;
    //public int modelID;
    //public int modellvl;
    //public float startTime;
    //public float intervals;
    //public int groupIndex;
    //public int type;
    public int route;
    public SceneType sceneType;
    float[] pos;
    public string des;
    public Vector3 navi0, navi1, navi2, navi3;
    public override void parseJson(object jd)
    {
        item = jd as Dictionary<string, object>;
        id = Convert.ToInt32(item["id"]);
        //sceneid = Convert.ToInt32(item["sceneid"]);
        LoadPos(item["modelPos"], ref modelPos);
        //LoadPos(item["NaviPoints0"], ref navi0);
        //LoadPos(item["NaviPoints1"], ref navi1);
        //LoadPos(item["NaviPoints2"], ref navi2);
        //LoadPos(item["NaviPoints3"], ref navi3);
        modelRot = float.Parse(item["modelRot"].ToString());
        modelID = Convert.ToInt32(item["modelID"]);
        modellvl = Convert.ToInt32(item["modellvl"]);
        startTime = float.Parse(item["startTime"].ToString());
        intervals = float.Parse(item["intervals"].ToString());
        groupIndex = Convert.ToInt32(item["groupIndex"]);
        type = Convert.ToInt32(item["type"]);
        route = Convert.ToInt32(item["route"]);
        des = item["des"].ToString();
        sceneType =(SceneType) Enum.Parse(typeof(SceneType), item["sceneid"].ToString());
    }

    void LoadPos(object items, ref Vector3 vector)
    {
        pos = FSDataNodeTable<Moba3SceneConfigNode>.GetSingleton().ParseToFloatArray(items);
        if (null != pos && pos.Length > 0)
        {
            vector = new Vector3(pos[0], pos[1], pos[2]);
            pos = null;
        }
    }
}
