using UnityEngine;
using System.Collections;
using Tianyu;
using System;
using System.Collections.Generic;

public class AfcNode : FSDataNodeBase
{

    int id;
    float hp;
    float attack;
    float armor;
    float magic_resist;
    float critical;
    float dodge;
    float hit_ratio;
    float armor_penetration;
    float magic_penetration;
    float suck_blood;
    float tenacity;
    public float skill { get; private set; }

    public float[] attrRate; //11项系数

    public override void parseJson(object jd)
    {
        Dictionary<string, object> item = jd as Dictionary<string, object>;
        id = Convert.ToInt32(item["id"]);
        hp = float.Parse(item["hp"].ToString());
        attack = float.Parse(item["attack"].ToString());
        armor = float.Parse(item["armor"].ToString());
        magic_resist = float.Parse(item["magic_resist"].ToString());
        critical = float.Parse(item["critical"].ToString());
        dodge = float.Parse(item["dodge"].ToString());
        hit_ratio = float.Parse(item["hit_ratio"].ToString());
        armor_penetration = float.Parse(item["armor_penetration"].ToString());
        magic_penetration = float.Parse(item["magic_penetration"].ToString());
        suck_blood = float.Parse(item["suck_blood"].ToString());
        tenacity = float.Parse(item["tenacity"].ToString());
        skill = float.Parse(item["skill"].ToString());
        attrRate = new float[] { hp, attack, armor, magic_resist, critical, dodge, hit_ratio, armor_penetration, magic_penetration, suck_blood, tenacity };
    }
    
}
