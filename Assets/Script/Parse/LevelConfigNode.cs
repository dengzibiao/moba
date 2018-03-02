using UnityEngine;
using System.Collections.Generic;
using Tianyu;
using System;

public class LevelConfigNode : FSDataNodeBase
{

    public int id;
    public int levelID;
    public Vector3 playerPos;
    public float playerRot;
    public int airWallType;
    public int airWallID;
    public Vector3 airWallPos;
    public float airWallRot;
    public Vector3 airWallSize;
    public Vector3[] playerWayPointPos;
    public Vector3 npcStarPointPos;
    public Vector3[] npcWayPointPos;
    public int npcID;
    public float npclvl;
    public int triggerBoss;

    float[] pos;
    public override void parseJson(object jd)
    {
        Dictionary<string, object> item = (Dictionary<string, object>)jd;
        id = Convert.ToInt32(item["id"]);
        levelID = Convert.ToInt32(item["levelID"]);
        LoadPos(item["playerPos"], ref playerPos);
        playerRot = float.Parse(item["playerRot"].ToString());
        airWallType = Convert.ToInt32(item["airWallType"]);
        airWallID = Convert.ToInt32(item["airWallID"]);
        LoadPos(item["airWallPos"], ref airWallPos);
        airWallRot = float.Parse(item["airWallRot"].ToString());
        LoadPos(item["airWallSize"], ref airWallSize);
        ArrayObj(item["playerWayPointPos"],ref playerWayPointPos);
        LoadPos(item["npcStarPointPos"], ref npcStarPointPos);
        ArrayObj(item["npcWayPointPos"], ref npcWayPointPos);
        npcID = Convert.ToInt32(item["npcId"]);
        npclvl = float.Parse((item["npcLvl"]).ToString());
        if (item.ContainsKey("triggerBoss"))
            triggerBoss = Convert.ToInt32(item["triggerBoss"]);
    }

    void ArrayObj(object items, ref Vector3[] obj)
    {
        object[] nodePos = (object[])items;
        obj = new Vector3[nodePos.Length];
        for (int m = 0; m < nodePos.Length; m++)
        {
            LoadPos((object)nodePos[m], ref obj[m]);
        }
    }

    void LoadPos(object items, ref Vector3 vector)
    {
        pos = FSDataNodeTable<LevelConfigNode>.GetSingleton().ParseToFloatArray(items);
        if (null != pos && pos.Length > 0)
        {
            vector = new Vector3(pos[0], pos[1], pos[2]);
            pos = null;
        }
    }

}
