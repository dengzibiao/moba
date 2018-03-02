using UnityEngine;
using System.Collections;
using System;
using Tianyu;

public class TDLevelConfigNode : LevelConfigBase
{
    public int tdID;
    public Vector3[] auto;

    float[] pos;
    
    public override void parseJson(object jd)
    {
        base.parseJson(jd);
        tdID = Convert.ToInt32(item["tdID"]);

        object[] nodePos = (object[])item["auto"];
        auto = new Vector3[nodePos.Length];

        for (int i = 0; i < nodePos.Length; i++)
        {
            pos = FSDataNodeTable<TDLevelConfigNode>.GetSingleton().ParseToFloatArray(nodePos[i]);
            auto[i] = new Vector3(pos[0], pos[1], pos[2]);
        }

    }

}
