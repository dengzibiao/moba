using UnityEngine;
using System.Collections;
using Tianyu;
using System;
using System.Collections.Generic;

public class MobaRobotNode : FSDataNodeBase
{
    //public int id;
    public int hero_lv;
    public int hero_star;
    public int hero_grade;
    public int equipment_grade;
    public int equipment_lv;
    public override void parseJson(object jd)
    {
        Dictionary<string, object> item = (Dictionary<string, object>)jd;
        //id = int.Parse(item["id"].ToString());
        hero_lv= int.Parse(item["hero_lv"].ToString());
        hero_star= int.Parse(item["hero_star"].ToString());
        hero_grade = int.Parse(item["hero_grade"].ToString());
        equipment_grade = int.Parse(item["equipment_grade"].ToString());
        equipment_lv = int.Parse(item["equipment_lv"].ToString());
    }

}
