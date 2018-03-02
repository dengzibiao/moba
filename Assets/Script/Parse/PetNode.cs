using UnityEngine;
using System.Collections;
using Tianyu;
using System;
using System.Collections.Generic;

public class PetNode : FSDataNodeBase
{

    public int id;//坐骑id
    public int pet_types;//坐骑类型 1：爬行；2：飞行
    public string name;
    public string describe;
    public int grade;
    public int[] cprice;
    public int model_id;
    public string icon_name;
    public int need_lv;
    public int power;
    public int intelligence;
    public int agility;
    public int hp;
    public int attack;
    public int armor;
    public int magic_resist;
    public int critical;
    public int dodge;
    public int hit_ratio;
    public int armor_penetration;
    public int magic_penetration;
    public int suck_blood;
    public int tenacity;
    public int movement_speed;
    public int attack_speed;
    public int released;


    public override void parseJson(object jd)
    {
        Dictionary<string, object> item = (Dictionary<string, object>)jd;
        id = Convert.ToInt32(item["pet_id"]);
        pet_types = Convert.ToInt32(item["pet_types"]);
        name = item["name"].ToString();
        describe = item["describe"].ToString();
        grade = Convert.ToInt32(item["grade"]);
        cprice = item["cprice"] as int[];
        model_id = Convert.ToInt32(item["model_id"]);
        icon_name = item["icon_name"].ToString();
        need_lv = Convert.ToInt32(item["need_lv"]);
        power = Convert.ToInt32(item["power"]);
        intelligence = Convert.ToInt32(item["intelligence"]);
        agility = Convert.ToInt32(item["agility"]);
        hp = Convert.ToInt32(item["hp"]);
        attack = Convert.ToInt32(item["attack"]);
        armor = Convert.ToInt32(item["armor"]);
        magic_resist = Convert.ToInt32(item["magic_resist"]);
        critical = Convert.ToInt32(item["critical"]);
        dodge = Convert.ToInt32(item["dodge"]);
        hit_ratio = Convert.ToInt32(item["hit_ratio"]);
        armor_penetration = Convert.ToInt32(item["armor_penetration"]);
        magic_penetration = Convert.ToInt32(item["magic_penetration"]);
        suck_blood = Convert.ToInt32(item["suck_blood"]);
        tenacity = Convert.ToInt32(item["tenacity"]);
        movement_speed = Convert.ToInt32(item["movement_speed"]);
        attack_speed = Convert.ToInt32(item["attack_speed"]);
        released = Convert.ToInt32(item["released"]);
    }

}
