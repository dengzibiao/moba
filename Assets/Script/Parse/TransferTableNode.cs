using UnityEngine;
using Tianyu;
using System.Collections;
using System.Collections.Generic;

public class TransferTableNode : FSDataNodeBase
{
    public long key;
    public string model;
    public Vector3 pos;
    public int scene;
    public int toscence;
    public Vector3 toPos;
    public override void parseJson(object jd)
    {
        Dictionary<string, object> item = (Dictionary<string, object>)jd;
        key = long.Parse(item["key"].ToString());
        model = item["modle"].ToString();
        scene = int.Parse(item["scene"].ToString());
        pos.x = float.Parse(item["posX"].ToString());
        pos.y = float.Parse(item["posY"].ToString());
        pos.z = float.Parse(item["posZ"].ToString());
        toscence = int.Parse(item["toscene"].ToString());
        toPos.x = float.Parse(item["toX"].ToString());
        toPos.y = float.Parse(item["toY"].ToString());
        toPos.z = float.Parse(item["toZ"].ToString());
    }
}
