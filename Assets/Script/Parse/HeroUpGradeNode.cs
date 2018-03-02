using UnityEngine;
using System.Collections;
using Tianyu;
using System;
using System.Collections.Generic;

public class HeroUpGradeNode : FSDataNodeBase
{

    public int id;
    public int exp;
    public int skill_limit;
    public int skill_ceiling;
    public int grade;

    public override void parseJson(object jd)
    {
        Dictionary<string, object> item = jd as Dictionary<string, object>;
        id = int.Parse(item["id"].ToString());
        exp = int.Parse(item["exp"].ToString());
        skill_limit = int.Parse(item["skill_limit"].ToString());
        skill_ceiling = int.Parse(item["skill_ceiling"].ToString());
        grade = int.Parse(item["grade"].ToString());
    }
}
