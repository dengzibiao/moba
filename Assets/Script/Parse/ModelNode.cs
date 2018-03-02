using UnityEngine;
using System.Collections;
using Tianyu;
using System.Collections.Generic;

public class ModelNode : FSDataNodeBase
{
    public int id;
    public string modelName;
    public string respath;
    public string modelRoot;
    public string modelPath;
    public string modelLowPath;
    public float colliderRadius;
    public float navRadius;
    public float modelSize = 1.2f;

    public override void parseJson(object jd)
    {
        Dictionary<string, object> item = (Dictionary<string, object>)jd;
        id = int.Parse(item["id"].ToString());
        modelName = item["remarks"].ToString();
        respath = item["respath"].ToString();
        modelRoot = respath.Substring(0, respath.LastIndexOf("/") + 1);
        modelPath = respath.Substring(respath.LastIndexOf("/") + 1);
        modelLowPath = modelPath + "_low";
        navRadius = float.Parse(item["navSize"].ToString());
        colliderRadius = float.Parse(item["colliderSize"].ToString());
        if (item.ContainsKey("size"))
        {
            modelSize = float.Parse(item["size"].ToString());
        }
    }
}
