using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tianyu;
public class  Archaeology_rewardNode :FSDataNodeBase
{
    public int id;
    public int sn;
    public int map_id;
    public int grade;
    public string map_name;
   // public int[][] reward=new int[8][];
    public long[,] rewardItem;
    public override void parseJson(object jd)
    {
        Dictionary<string, object> item = (Dictionary<string, object>)jd;
        if (item["id"] != null)
        {
            id = int.Parse(item["id"].ToString());
        }

        if (item["sn"] != null)
        {
            sn = int.Parse(item["sn"].ToString());
        }

        if (item["map_id"] != null)
        {
            map_id = int.Parse(item["map_id"].ToString());
        }

        if (item["Grade"] != null)
        {
            grade = int.Parse(item["Grade"].ToString());
        }
        if (item["map_name"] != null)
        {
            map_name = item["map_name"].ToString();
        }
        //reward = new int[4];
       
        //if (item["reward"] != null)
        //{
        //    reward = (int[][])item["reward"];
        //}
        if (item["reward"] != null)
        {
            object[] nodeCount = (object[])item["reward"];
            rewardItem = new long[nodeCount.Length, 3];
            if (nodeCount.Length > 0)
            {
                for (int i = 0; i < nodeCount.Length; i++)
                {
                    int[] node = (int[])nodeCount[i];
                    for (int j = 0; j < node.Length; j++)
                    {
                        rewardItem[i,j] = node[j];
                    }
                }
            }
        }
        
    }
	
}
