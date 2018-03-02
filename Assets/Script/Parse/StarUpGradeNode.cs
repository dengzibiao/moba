using UnityEngine;
using System.Collections;
using Tianyu;
using System;
using System.Collections.Generic;

public class StarUpGradeNode : FSDataNodeBase
{

    public int star;
    public int call_stone_num;
    public int evolve_cost;
    public int evolve_stone_num;
    public int convert_stone_num;

    public override void parseJson(object jd)
    {
        Dictionary<string, object> item = jd as Dictionary<string, object>;
        star = int.Parse(item["star"].ToString());
        call_stone_num = int.Parse(item["call_stone_num"].ToString());
        evolve_cost = int.Parse(item["evolve_cost"].ToString());
        evolve_stone_num = int.Parse(item["evolve_stone_num"].ToString());
        convert_stone_num = int.Parse(item["convert_stone_num"].ToString());
    }
}
