using UnityEngine;
using System.Collections;
using Tianyu;
using System;
using System.Collections.Generic;

public class CommentsStarNode : FSDataNodeBase
{

    public int sceneID;
    public int starID;
    public string des;
    public double[] heroID;
    public double[] killID;
    public int time;

    public override void parseJson(object jd)
    {
        Dictionary<string, object> item = jd as Dictionary<string, object>;
        sceneID = int.Parse(item["sceneID"].ToString());
        starID = int.Parse(item["starID"].ToString());
        des = item["des"].ToString();

        if (null != item["heroID"])
        {
            if (item["heroID"] is int[])
            {
                int[] nodeID = (int[])item["heroID"];
                heroID = new double[nodeID.Length];
                for (int m = 0; m < nodeID.Length; m++)
                {
                    heroID[m] = nodeID[m];
                }
            }
            else
            {
                object[] nodeID = (object[])item["heroID"];
                heroID = new double[nodeID.Length];
                for (int m = 0; m < nodeID.Length; m++)
                {
                    heroID[m] = (double)nodeID[m];
                }
            }
        }

        if (null != item["killID"])
        {
            if (item["killID"] is int[])
            {
                int[] nodeID = (int[])item["killID"];
                killID = new double[nodeID.Length];
                for (int m = 0; m < nodeID.Length; m++)
                {
                    killID[m] = nodeID[m];
                }
            }
            else
            {
                object[] nodeID = (object[])item["killID"];
                killID = new double[nodeID.Length];
                for (int m = 0; m < nodeID.Length; m++)
                {
                    killID[m] = (double)nodeID[m];
                }
            }
        }

        time = int.Parse(item["time"].ToString());

    }
}
