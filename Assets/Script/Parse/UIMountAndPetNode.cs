using UnityEngine;
using System.Collections;
using Tianyu;
using System;
using System.Collections.Generic;

public class UIMountAndPetNode : FSDataNodeBase
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
    public int power { get; set; }
    public int intelligence { get; set; }
    public int agility { get; set; }
    public int hp { get; set; }
    public int attack { get; set; }
    public int armor { get; set; }
    public int magic_resist { get; set; }
    public int critical { get; set; }
    public int dodge { get; set; }
    public int hit_ratio { get; set; }
    public int armor_penetration { get; set; }
    public int magic_penetration { get; set; }
    public int suck_blood { get; set; }
    public int tenacity { get; set; }
    public int movement_speed { get; set; }
    public int attack_speed { get; set; }
    public int released { get; set; }
    public int[] propertyList = new int[17];
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

        power = int.Parse(item["power"].ToString());
        intelligence = int.Parse(item["intelligence"].ToString());
        agility = int.Parse(item["agility"].ToString());
        hp = int.Parse(item["hp"].ToString());
        attack = int.Parse(item["attack"].ToString());
        armor = int.Parse(item["armor"].ToString());
        magic_resist = int.Parse(item["magic_resist"].ToString());
        critical = int.Parse(item["critical"].ToString());
        dodge = int.Parse(item["dodge"].ToString());
        hit_ratio = int.Parse(item["hit_ratio"].ToString());
        armor_penetration = int.Parse(item["armor_penetration"].ToString());
        magic_penetration = int.Parse(item["magic_penetration"].ToString());
        suck_blood = int.Parse(item["suck_blood"].ToString());
        tenacity = int.Parse(item["tenacity"].ToString());
        movement_speed = int.Parse(item["movement_speed"].ToString());
        attack_speed = int.Parse(item["attack_speed"].ToString());
        released = int.Parse(item["released"].ToString());
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
