using UnityEngine;
using System.Collections;
using Tianyu;
using System;
using System.Collections.Generic;

public class UIMountNode : FSDataNodeBase
{
    public long mount_id { get; set; }
    public int mount_types { get; set; }
    public string name { get; set; }
    public string describe { get; set; }
    public int grade { get; set; }
    public long[] cprice { get; set; }
    public string model_id { get; set; }
    public string icon_name { get; set; }
    public int need_lv { get; set; }
    public float power { get; set; }
    public float intelligence { get; set; }
    public float agility { get; set; }
    public float hp { get; set; }
    public float attack { get; set; }
    public float armor { get; set; }
    public float magic_resist { get; set; }
    public float critical { get; set; }
    public float dodge { get; set; }
    public float hit_ratio { get; set; }
    public float armor_penetration { get; set; }
    public float magic_penetration { get; set; }
    public float suck_blood { get; set; }
    public float tenacity { get; set; }
    public float movement_speed { get; set; }
    public float attack_speed { get; set; }
    public float released { get; set; }
    public float[] propertyList = new float[17];
    public float ride_x { get; set; }
    public float ride_y { get; set; }
    public float ride_z { get; set; }

    public override void parseJson(object jd)
    {
        Dictionary<string, object> item = jd as Dictionary<string, object>;
        mount_id = long.Parse(item["mount_id"].ToString());
        mount_types = int.Parse(item["mount_types"].ToString());
        name = item["name"].ToString();
        describe = item["describe"].ToString();
        grade = int.Parse(item["grade"].ToString());
        int[] nodeEquip = item["cprice"] as int[];
        if (nodeEquip != null)
        {
            cprice = new long[nodeEquip.Length];
            for (int i = 0; i < nodeEquip.Length; i++)
            {
                cprice[i] = nodeEquip[i];
            }
        }
        model_id = item["model_id"].ToString();
        icon_name = item["icon_name"].ToString();
        need_lv = int.Parse(item["need_lv"].ToString());

        ride_x = float.Parse(item["ride_x"].ToString());
        ride_y = float.Parse(item["ride_y"].ToString());
        ride_z = float.Parse(item["ride_z"].ToString());

        power = float.Parse(item["power"].ToString());
        intelligence = float.Parse(item["intelligence"].ToString());
        agility = float.Parse(item["agility"].ToString());
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
        movement_speed = float.Parse(item["movement_speed"].ToString());
        attack_speed = float.Parse(item["attack_speed"].ToString());
        released = float.Parse(item["released"].ToString());
        propertyList[0] = power;
        propertyList[1] = intelligence;
        propertyList[2] = agility;
        propertyList[3] = hp;
        propertyList[4] = attack;
        propertyList[5] = armor;
        propertyList[6] = magic_resist;
        propertyList[7] = critical;
        propertyList[8] = dodge;
        propertyList[9] = hit_ratio;
        propertyList[10] = armor_penetration;
        propertyList[11] = magic_penetration;
        propertyList[12] = suck_blood;
        propertyList[13] = tenacity;
        propertyList[14] = movement_speed;
        propertyList[15] = attack_speed;
        propertyList[16] = released;
    }
}
