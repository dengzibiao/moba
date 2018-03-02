using System;
using System.Collections.Generic;
using Tianyu;
using System.Linq;
using UnityEngine;

public class MapNode : FSDataNodeBase
{
    public int MapId;
    public string MapName;
    public string map_name;
    public long[] ordinary;
    public long[] elite;
    public int[] is_opened;
    public double[][] ordinary_reward;
    public double[][] elite_reward;



    //public List<int> Scenes = new List<int>();
    
    //public List<SceneNode> Scenes = new List<SceneNode>();

    public override void parseJson ( object jd )
    {
        Dictionary<string, object> items = (Dictionary<string, object>)jd;
        MapId = Convert.ToInt32(items["bigmap_id"]);
        MapName = Convert.ToString(items["name"]);
        map_name = Convert.ToString(items["map_name"]);

        if (items["ordinary"] is int[])
        {
            int[] node = (int[])items["ordinary"];
            ordinary = new long[node.Length];
            for (int i = 0; i < node.Length; i++)
            {
                ordinary[i] = long.Parse(node[i].ToString());
            }
        }
        else
        {
            object[] node = (object[])items["ordinary"];
            ordinary = new long[node.Length];
            for (int i = 0; i < node.Length; i++)
            {
                ordinary[i] = long.Parse(node[i].ToString());
            }
        }

        //ordinary = items["ordinary"] as double[];
        //elite = items["elite"] as double[];

        if (items["elite"] is int[])
        {
            int[] node = (int[])items["elite"];
            elite = new long[node.Length];
            for (int i = 0; i < node.Length; i++)
            {
                elite[i] = long.Parse(node[i].ToString());
            }
        }
        else
        {
            object[] node = (object[])items["elite"];
            elite = new long[node.Length];
            for (int i = 0; i < node.Length; i++)
            {
                elite[i] = long.Parse(node[i].ToString());
            }
        }

        int[] nodeOpen = (int[])items["is_opened"];
        is_opened = new int[nodeOpen.Length];
        for (int i = 0; i < is_opened.Length; i++)
        {
            is_opened[i] = nodeOpen[i];
        }

        //Debug.Log(Scenes.Length);

        //StarsGot = Convert.ToInt32(items["stars"]);
        //Locked = Convert.ToBoolean(items["locked"]);
    }
}