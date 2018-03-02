using Tianyu;
using System.Collections.Generic;

public enum BuffType
{
    control = 1,                //控制类buff
    translate = 2,              //行为类buff
    decrease = 3,               //减益类buff
    increase = 4,               //增益类buff
    immune = 5,                 //免疫类buff
    other = 6,                  //其他
    continuousDecrease = 7,     //持续减益类buff
    continuousIncrease = 8,     //持续增益类buff
    talent = 9,                 //天赋buff
    DelayEffective = 10,        //延迟生效buff
    Summon = 11,                //召唤
}

//作用类型 1:当前生命；2：攻击力；3：护甲；4：魔抗；5：暴击率；6:闪避；7：命中率：8：吸血率；9：移动速度；10：攻击速度；11：最大生命；
//12:控制；13：移动；14：攻击；15：技能；16：物免；17：魔免；18：悬空
public enum buffActionType
{
    none = 0,
    hp = 1,
    attackDamage = 2,
    armor = 3,
    magic_resist = 4,
    critical = 5,
    dodge = 6,
    hit = 7,
    suckBlood = 8,
    moveSpeed = 9,
    attackSpeed = 10,
    maxHp = 11,
    control = 12,
    move = 13,
    attack = 14,
    skill = 15,
    immunePhysics = 16,
    immuneMagic = 17,
    rollUp = 18
}

public class SkillBuffNode : FSDataNodeBase
{
    public long id;
    public BuffType type;
    public int effectPos;
    public string effect;
    public string name;
    public string desc;
    public buffActionType buffActionType;
    public int priorityBuff;
    public int priorityShow;
    public int damageType;

    public override void parseJson ( object jd )
    {
        Dictionary<string, object> items = (Dictionary<string, object>)jd;
        effect = items["state_effects"] == null ? "" : items["state_effects"].ToString();
        effectPos = (int)items["effect_pos"];
        type = (BuffType)(int)items["types"];
        object actionType = items["action_type"];
        buffActionType = actionType == null ? buffActionType.none : (buffActionType)actionType;
        priorityBuff = (int)items["priority_buff"];
        priorityShow = (int)items["priority_show"];
        damageType = items["damage_types"] == null ? 0 : (int)items["damage_types"];
    }
}
