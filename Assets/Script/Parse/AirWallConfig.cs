using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tianyu;
using System;

public class AirWallConfig : FSDataNodeBase
{
    public int id;
    public int airWallID;
    public int monsterID;
    public float monsterlvl;
    public Vector3 monsterPos;
    public float monsterDis;
    float[] pos;

    public override void parseJson(object jd)
    {
        Dictionary<string, object> item = (Dictionary<string, object>)jd;
        id = Convert.ToInt32(item["id"]);
        airWallID = Convert.ToInt32(item["airWallID"]);
        monsterID = Convert.ToInt32(item["monsterID"]);
        monsterlvl = float.Parse(item["monsterlvl"].ToString());
        pos = FSDataNodeTable<LevelConfigNode>.GetSingleton().ParseToFloatArray(item["monsterPos"]);
        if (null != pos)
        {
            monsterPos = new Vector3(pos[0], pos[1], pos[2]);
            pos = null;
        }
        monsterDis = float.Parse(item["monsterDis"].ToString());
    }

}