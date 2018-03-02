using System.Collections.Generic;
using Tianyu;
using UnityEngine;

public class CharacterAttrNode : FSDataNodeBase
{
    protected Dictionary<string, object> item;

    public long id;
    public int types; // 5,boss
    public string describe;
    public string info;
    public string icon_name;
    public int model;
    public ModelNode modelNode;
    public int released;
    public int is_icon;
    public long[] skill_id;
    public SkillNode[] skills;
    public Dictionary<int, SkillNode> skillNodeDict = new Dictionary<int, SkillNode>();
    public int hpBar_count = 6;

    public string name;// 名字

    public float power;//初始力量
    public float intelligence;//初始智力
    public float agility;//初始敏捷
    public int hp;//初始生命值
    public float attack;//初始攻击
    public float armor;//初始护甲
    public float magic_resist;//初始魔抗
    public float critical;//初始暴击
    public float dodge;//初始闪避
    public float hit_ratio;//初始命中
    public float armor_penetration;//初始护甲穿透
    public float magic_penetration;//初始魔法穿透
    public float suck_blood;//初始物理吸血
    public float tenacity;//韧性

    public float movement_speed;//初始移动速度
    public float attack_speed;//初始攻击速度
    public float field_distance;//目标距
    public float chase_range = 4f;//追击距离
    public float hp_regain;//生命恢复

    public float model_size = 1;

    public float[] base_Propers;//14个基础属性

    public override void parseJson ( object jd )
    {
        item = jd as Dictionary<string, object>;

        movement_speed = float.Parse(item["movement_speed"].ToString());
        attack_speed = float.Parse(item["attack_speed"].ToString());
        field_distance = float.Parse(item["field_distance"].ToString());
        name = item["name"].ToString();

        hp = Mathf.FloorToInt(System.Convert.ToInt32(item["hp"]));
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

        if(item.ContainsKey("power"))
            power = float.Parse(item["power"].ToString());
        if(item.ContainsKey("intelligence"))
            intelligence = float.Parse(item["intelligence"].ToString());
        if(item.ContainsKey("agility"))
            agility = float.Parse(item["agility"].ToString());
        if(item.ContainsKey("chase_distanc"))
            chase_range = float.Parse(item["chase_distanc"].ToString());
        if(item.ContainsKey("hpBar_count"))
            hpBar_count = int.Parse(item["hpBar_count"].ToString());

        base_Propers = new float[Formula.ATTR_COUNT] { power, intelligence, agility, hp, attack, armor, magic_resist, critical, dodge, hit_ratio, armor_penetration, magic_penetration, suck_blood, tenacity};
    }
}