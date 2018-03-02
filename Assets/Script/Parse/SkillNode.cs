using System.Collections.Generic;
using System;
using Tianyu;
using UnityEngine;
//伤害类型
public enum DamageType : byte
{
    physics = 1,//物理
    magic = 2,//魔法
    fix = 3//真实伤害
}
//技能类型 0：近战攻击；1：投掷；2：飞行；3：闪现；4：冲锋；5：冲击波；6：追踪目标；7：中心式；8：陷阱触发；9：召唤类；10：区域型；11：链式；12：移动中心式；13：链接式；14：爆炸；15:弹射；16：多目标追踪;
//17:移动扩散收缩；18：掘地；19：跳砍；20：多目标弹射；21:生成障碍物；22：扩散型；23：牵引；24：发射扩散米字型；25：游走
public enum SkillCastType
{
    MeleeEffect = 0,
    CastSkill = 1,
    FlyEffect = 2,
    BlinkSkill = 3,
    FrontSprintSkill = 4,
    ShockWaveSkill = 5,
    TrackSkill = 6,
    CenterSkill = 7,
    TrapSkill = 8,
    SummonSkill = 9,
    AoeSKill = 10,
    LinkSkill = 11,
    ToSelfCenterSkill = 12,
    ChainSkill = 13,
    Boom = 14,
    Bounce = 15,
    MultiTrackSkill = 16,
    MoveShrinkSkill = 17,
    BurrowSkill = 18,
    JumpChopSkill = 19,
    MultiBounce = 20,
    GenerateObstacle = 21,
    DiffuseSkill = 22,
    MultiTractionSkill = 23,
    LaunchDiffuseMeterSkill = 24,
    SnakeSteerSkill = 25
}
//作用类型 0：自己：1：我方英雄；2：我方小兵；3：敌方英雄；4：敌方小兵；5：敌方防御塔；6：敌方基地；
public enum influence_type
{
    self = 0,
    selfHero = 1,
    selfMonster = 2,
    enemyHero = 3,
    enemyMonster = 4,
    enemyTower = 5,
    enemyBase = 6
}
//技能作用范围类型 0：无；1：溅射；2：被格挡；3：推进
public enum rangeType
{
    none = 0,
    spurting = 1,
    canBlock = 2,
    boost = 3,
}
//选目标规则 0：当前 1：随机 2：最远
public enum ChoseTarget
{
    none = 0,
    random = 1,
    farthest = 2,
}

public class SkillNode : FSDataNodeBase
{
    public long skill_id;//主技能ID
    public long hero_id;//所属英雄
    public string name;//所属英雄名称
    public string skill_name;//主技能名称
    public string des;//技能说明
    public string skill_icon;//技能图标icon

    public float cooling;//技能冷却时间/s
    public float dist;//施放距离
    public float castBefore;//施法前摇

    public string spell_motion;//施法动作
    public string spell_effect;//施法特效

    public string sound;//技能音效
    public string hit_sound;//技能命中音效
    public byte target_ceiling;//目标上限
    public byte types;//伤害类型  1：物理；2：法术；3：真实伤害；
    public bool isSingle;//0:单体；1：aoe;
    public bool isPierce;//是否为穿透技能
    public bool ignoreTerrain;//是否无视地形0:无视；1：不无视
    public float flight_speed;//飞行速度
    public float max_fly;//飞行最远距离
    public SkillCastType skill_type;//技能类型 0：近战攻击；1：投掷；2：飞行；3：闪现；4：冲锋；5：冲击破；6：追踪目标；7：中心式；8：陷阱触发；9：召唤类；10：区域型；11：链式；12：移动中心式
    public rangeType range_type;//技能作用范围类型 0：单体；1：分裂；2：溅射；3：中心；4：直线；5：圆形；6：矩形	
    public float aoe_long;//范围长度/半径
    public float aoe_wide;//范围宽度
    public float angle;//扇形角度
    public float length_base;//扇形底部长度
    public int site;//技能下标
    public int seat;//技能图标位置
    public int alertedType;//警告类型
    public int energy;//施放消耗值(豆)
    public int[] influence_type;//目标类型
    public int[] nullity_type;//目标无效类型 0：无；1：魔免；2：物免；3：建筑；
    public byte missable;//是否判断命中，1判断，0不判断
    public float efficiency_time;//技能作用时间//
    public float effect_time;//持续施法时间/s 0：瞬发
    public bool isFiringPoint;//是否为发射点 0:脚底下；1：发射点
    public float[] interval_time;//间隔时间
    public float[] damage_ratio;//多段伤害伤害系数

    public int[] buffs_target;//buff作用类型
    public object[] specialBuffs;//特殊buff,buff持续时间和动作时间一致
    public object[] add_state;//附加状态[[状态1id,持续时间],[状态2id,持续时间]]
    public float[] base_num1;//基础数值[伤害,状态1,状态2]
    public float[] growth_ratio1;//成长系数[伤害,状态1,状态2]
    public float[] skill_ratio;//技能加成系数[伤害,状态1,状态2] 
    public int[] stats;//加成属性 [伤害,状态1,状态2] 0：无；1：power；2：intelligence；3：agility；4：hp；5：attack；6：armor；7：magic_resist；8：critical；9：dodge；10：hit_ratio；11：armor_penetration；12：magic_penetration；13：suck_blood；14：tenacity；15：movement_speed；16：attack_speed；17：striking_distance；

    public long[] skill_parts;

    public float range;
    public BattleRange battleRange;
    public object[] battleEffects;
    public bool Cancelable = true;
    public TargetState target;
    public ChoseTarget choseTarget;

    public int castOrder = 0;
    public int casePriority = 0;

    public override void parseJson(object jd)
    {
        Dictionary<string, object> item = (Dictionary<string, object>)jd;
        skill_id = long.Parse(item["skill_id"].ToString());
        name = item["name"].ToString();
        hero_id = long.Parse(item["hero_id"].ToString());
        skill_name = item["skill_name"].ToString();
        if (item["des"] != null)
        {
            des = item["des"].ToString();
        }
        skill_icon = item["skill_icon"].ToString();
        spell_motion = item["skill_motion"].ToString();
        sound = item["sound"].ToString();
        hit_sound = item["hit_sound"].ToString();
        target_ceiling = byte.Parse(item["target_ceiling"].ToString());
        types = byte.Parse(item["types"].ToString());
        dist = float.Parse(item["dist"].ToString());
        castBefore = float.Parse(item["end_motion"].ToString());
        isSingle = item["singular_aoe"] == null ? true : byte.Parse(item["singular_aoe"].ToString()) == 0;
        ignoreTerrain = item["pierce through"] == null ? false : byte.Parse(item["ignore_terrain"].ToString()) == 0;
        isPierce = item["pierce through"] == null ? false : byte.Parse(item["pierce through"].ToString()) == 0;
        flight_speed = float.Parse(item["flight_speed"].ToString());
        max_fly = item["max_fly"] == null ? 0 : float.Parse(item["max_fly"].ToString());
        skill_type = item.ContainsKey("skill_type") && item["skill_type"] != null ? (SkillCastType)byte.Parse(item["skill_type"].ToString()) : (SkillCastType)0;
        range_type = (rangeType)byte.Parse(item["range_type"].ToString());
        aoe_long = float.Parse(item["aoe_long"].ToString());
        aoe_wide = float.Parse(item["aoe_wide"].ToString());
        angle = float.Parse(item["angle"].ToString());
        site = int.Parse(item["site"].ToString());
        seat = int.Parse(item["seat"].ToString());
        alertedType = int.Parse(item["alerted_type"].ToString());
        length_base = float.Parse(item["length_base"].ToString());
        energy = int.Parse(item["energy"].ToString());
        target = (TargetState)(int.Parse(item["target"].ToString()));
        choseTarget = (ChoseTarget)(int.Parse(item["choose_target"].ToString()));

        interval_time = FSDataNodeTable<SkillNode>.GetSingleton().ParseToFloatArray(item["interval_time"]);
        damage_ratio = FSDataNodeTable<SkillNode>.GetSingleton().ParseToFloatArray(item["damage_ratio"]);

        int[] nodelist = item["nullity_type"] as int[];
        if (nodelist != null)
        {
            nullity_type = new int[nodelist.Length];

            for (int m = 0; m < nodelist.Length; m++)
            {
                nullity_type[m] = nodelist[m];
            }
        }
        int[] influenceList = item["influence_type"] as int[];
        if (influenceList != null)
        {
            influence_type = influenceList;
        }
        // nullity_type = byte.Parse(item["nullity_type"].ToString());
        missable = byte.Parse(item["missable"].ToString());
        efficiency_time = float.Parse(item["efficiency_time"].ToString());
        effect_time = float.Parse(item["effect_time"].ToString());
        isFiringPoint = item["firing"] == null ? false : float.Parse(item["firing"].ToString()) == 1;
        cooling = float.Parse(item["cooling"].ToString());
        //cooling = 1;

        base_num1 = FSDataNodeTable<SkillNode>.GetSingleton().ParseToFloatArray(item["base_num1"]);
        growth_ratio1 = FSDataNodeTable<SkillNode>.GetSingleton().ParseToFloatArray(item["growth_ratio1"]);
        skill_ratio = FSDataNodeTable<SkillNode>.GetSingleton().ParseToFloatArray(item["skill_ratio"]);
        if(skill_ratio == null)
            Debug.LogError("skill_ratio null");
        stats = (int[])item["stats"];
        buffs_target = item["buffs_target"] as int[];
        specialBuffs = (object[])item["special_buffs"];

        skill_parts = FSDataNodeTable<SkillNode>.GetSingleton().ParseToLongArray(item["skill_parts"]); 

        add_state = (object[])item["add_state"];
        range = float.Parse(( item["dist"] ).ToString());
        battleRange = new SectorRange(range);
    }

    public bool IsSerialSkill ()
    {
        return skill_parts.Length > 1;
    }

    public bool IsPartSkill ()
    {
        return skill_parts.Length == 1;
    }

    public float GetSkillBattleValue (int index, CharacterData cd)
    {
        float attrVal = 0f;
        
        if (cd != null)
            attrVal = stats[index] > 0 ? Formula.GetSingleAttribute(cd, (AttrType)( stats[index] - 1 )) : 0;
        return base_num1[index] + cd.skill[skill_id] * growth_ratio1[index] + skill_ratio[index] * attrVal;
    }

    public float GetSkillBattleValueByRatio(int index, CharacterData cd, float ratio)
    {
        float attrVal = 0f;

        if (cd != null)
            attrVal = stats[index] > 0 ? Formula.GetSingleAttribute(cd, (AttrType)(stats[index] - 1)) : 0;
        return skill_ratio[index] * ratio * attrVal;
    }

    public float GetSkillBattleValueByLuosa(int index, CharacterData cd, int LoseBloodRatio)
    {
        float attrVal = 0f;

        if (cd != null)
            attrVal = stats[index] > 0 ? Formula.GetSingleAttribute(cd, (AttrType)(stats[index] - 1)) : 0;
        return base_num1[index] + cd.skill[skill_id] * growth_ratio1[index] + (skill_ratio[index] + LoseBloodRatio * 0.0025f) * attrVal;
    }

    public static SkillEffectNode[] GetSkillEffectNodes ( object[] effectConfigs )
    {
        SkillEffectNode[] ret = new SkillEffectNode[effectConfigs.Length];
        for(int i = 0; i < effectConfigs.Length; i++)
        {
            double[] bEffsCfg = GetEffectsConfig(effectConfigs[i]);
            SkillEffectNode sen = FSDataNodeTable<SkillEffectNode>.GetSingleton().FindDataByType((int)bEffsCfg[1]);
            sen.effectTiming = (int)bEffsCfg[0];
            object[] bEffsParam = new object[bEffsCfg.Length - 2];
            for(int m = 2; m < bEffsCfg.Length; m++)
            {
                bEffsParam[m - 2] = bEffsCfg[m];
            }
            if(bEffsParam.Length > 0)
                sen.config = string.Format(sen.config, bEffsParam);
            ret[i] = sen;
        }
        return ret;
    }

    static double[] GetEffectsConfig ( object config )
    {
        if(config is int[])
        {
            int[] retObjArr = (int[])config;
            return Array.ConvertAll<int, double>(retObjArr, o => (int)o);
        }
        else if(config is object[])
        {
            object[] retObjArr = (object[])config;
            return Array.ConvertAll<object, double>(retObjArr, o => double.Parse(o.ToString()));
        }
        return null;
    }

    public bool isNormalAttack ()
    {
        return spell_motion.StartsWith("attack");
    }

    public List<SkillBuff> AddSpecialBuffs(CharacterState cs)
    {
        List<SkillBuff> mSpecialBuffs = new List<SkillBuff>();
        for (int i = 0; i < specialBuffs.Length; i++)
        {
            if (specialBuffs[i] is Array)
            {
                Array o = (Array)specialBuffs[i];
                if (o != null && o.Length == 3)
                {
                    object a = o.GetValue(0);
                    long mBuffId = long.Parse(a.ToString());
                    if ((SkillBuffType)mBuffId == SkillBuffType.Summon) continue;
                    float mBuffValue = float.Parse(o.GetValue(2).ToString());
                    SkillBuff mTempBuff = SkillBuffManager.GetInst().AddBuffs(mBuffValue, new object[2] { a, o.GetValue(1) }, cs, cs, this, cs.CharData.lvl, false);
                    mSpecialBuffs.Add(mTempBuff);
                }
            }
        }
        return mSpecialBuffs;
    }

    public bool IsTransfer ()
    {
        return skill_type == SkillCastType.BlinkSkill || skill_type == SkillCastType.FrontSprintSkill || skill_type == SkillCastType.JumpChopSkill;
    }

    public bool IsMoveSpeed ()
    {
        if(add_state.Length > 0)
        {
            for(int i = 0; i < add_state.Length; i++)
            {
                long buffId = GetBuffId(add_state[i]);
                if(buffId > 0)
                {
                    if(buffId == (long)SkillBuffType.Fast || buffId == (long)SkillBuffType.Slow)
                        return true;
                }
            }
        }
        return false;
    }

    public bool IsRestore ()
    {
        if(add_state.Length > 0)
        {
            for(int i = 0; i <add_state.Length; i++)
            {
                long buffId = GetBuffId(add_state[i]);
                if(buffId > 0)
                {
                    if(buffId == (long)SkillBuffType.Cure || buffId == (long)SkillBuffType.Restore)
                        return true;
                }
            }
        }
        return false;
    }

    public bool IsControl ()
    {
        if(add_state.Length > 0)
        {
            for(int i = 0; i < add_state.Length; i++)
            {
                long buffId = GetBuffId(add_state[i]);
                if(buffId > 0)
                {
                    if(buffId == (long)SkillBuffType.Dizzy || buffId == (long)SkillBuffType.Frozen || buffId == (long)SkillBuffType.RollUp || buffId == (long)SkillBuffType.Hover || buffId == (long)SkillBuffType.Petrified || buffId == (long)SkillBuffType.BlowUp || buffId == (long)SkillBuffType.KnockBack || buffId == (long)SkillBuffType.Imprison || buffId == (long)SkillBuffType.Disarm)
                        return true;
                }
            }
        }
        return false;
    }

    public bool IsBoost ()
    {
        if(add_state.Length > 0)
        {
            for(int i = 0; i < add_state.Length; i++)
            {
                long buffId = GetBuffId(add_state[i]);
                if(buffId > 0)
                {
                    if(buffId == (long)SkillBuffType.Fast || buffId == (long)SkillBuffType.AddonAttack || buffId == (long)SkillBuffType.Bleeding || buffId == (long)SkillBuffType.AddSuckBlood || buffId == (long)SkillBuffType.AddAttack || buffId == (long)SkillBuffType.AddAtkSpeed || buffId == (long)SkillBuffType.Spurting || buffId == (long)SkillBuffType.Grow || buffId == (long)SkillBuffType.SuckBloodHalo)
                        return true;
                }
            }
        }
        return false;
    }

    public bool IsDebuff ()
    {
        if(add_state.Length > 0)
        {
            for(int i = 0; i < add_state.Length; i++)
            {
                long buffId = GetBuffId(add_state[i]);
                if(buffId > 0)
                {
                    if(buffId == (long)SkillBuffType.ReduceArmor || buffId == (long)SkillBuffType.ReduceAtkSpeed || buffId == (long)SkillBuffType.ReduceCure || buffId == (long)SkillBuffType.Poison || buffId == (long)SkillBuffType.Curse || buffId == (long)SkillBuffType.DelayEffective || buffId == (long)SkillBuffType.KnockBack)
                        return true;
                }
            }
        }
        return false;
    }

    public bool IsProtect ()
    {
        if(add_state.Length > 0)
        {
            for(int i = 0; i < add_state.Length; i++)
            {
                long buffId = GetBuffId(add_state[i]);
                if(buffId > 0)
                {
                    if(buffId == (long)SkillBuffType.Resist || buffId == (long)SkillBuffType.AddArmor || buffId == (long)SkillBuffType.Virtualize || buffId == (long)SkillBuffType.Invincible || buffId == (long)SkillBuffType.AbsorbDamage || buffId == (long)SkillBuffType.Invisible || buffId == (long)SkillBuffType.MagicImmunity || buffId == (long)SkillBuffType.PhysicalImmunity || buffId == (long)SkillBuffType.ControlImmunity || buffId == (long)SkillBuffType.Blur || buffId == (long)SkillBuffType.AddMaxHp)
                        return true;
                }
            }
        }
        return false;
    }

    long GetBuffId (object buffConfig )
    {
        if(buffConfig is Array && ( (Array)buffConfig ).Length > 0)
        {
            return Convert.ToInt64(( (Array)buffConfig ).GetValue(0));
        }
        return 0;
    }
}
