using UnityEngine;
using System.Collections;
using Tianyu;
using System;
using System.Collections.Generic;

public class UISign_inNode : FSDataNodeBase
{

    public int day { get; set; }
    public long[] reward_prop;
    public int reward_money { get; set; }
    public int vip_limit { get; set; }


    public override void parseJson(object jd)
    {
        Dictionary<string, object> item = jd as Dictionary<string, object>;
        day = int.Parse(item["day"].ToString());
        int[] nodeEquip = item["reward_prop"] as int[];
        if (nodeEquip!=null)
        {
            reward_prop = new long[nodeEquip.Length];
            for (int i = 0; i < nodeEquip.Length; i++)
            {
                reward_prop[i] = nodeEquip[i];
            }
        }

        reward_money = int.Parse(item["reward_money"].ToString());
        vip_limit = int.Parse(item["vip_limit"].ToString());
    }
}