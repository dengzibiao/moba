using System.Collections.Generic;

using UnityEngine;
using Tianyu;
public class UpGradeSkillConsume : FSDataNodeBase
{


    public long id;//当前技能等级
    public int[] skillsike = new int[4];//技能1升级消耗金币
   




    public override void parseJson(object jd)
    {
        Dictionary<string, object> item = (Dictionary<string, object>)jd;
        id = long.Parse(item["id"].ToString());;
        skillsike[0] = int.Parse(item["skill1"].ToString());
        skillsike[1] = int.Parse(item["skill2"].ToString());
        skillsike[2] = int.Parse(item["skill3"].ToString());
        skillsike[3] = int.Parse(item["skill4"].ToString());
    }

}

