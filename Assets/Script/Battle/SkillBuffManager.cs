using Tianyu;
using System;
using UnityEngine;
using System.Collections.Generic;

public enum SkillBuffType
{
    Normal = 0,                     //普通
    AddonAttack = 401000100,        //普伤加成
    Slow = 401000200,               //减速
    Frozen = 401000300,             //冻结
    Resist = 401000400,             //抵抗
    Bleeding = 401000500,           //流血
    KnockBack = 401000600,          //击退
    AddSuckBlood = 401000700,       //吸血
    AddAttack = 401000800,          //强击
    AddMaxHp = 401000900,           //耐力
    Cure = 401001000,               //治疗
    Restore = 401001100,            //恢复
    AddArmor = 401001200,           //金刚
    WoundDeeper = 401001300,        //易伤
    Dizzy = 401001400,              //晕眩
    AddAtkSpeed = 401001500,        //急速
    ReduceAtkSpeed = 401001600,     //慢打
    SuckBloodHalo = 401001700,      //吸血光环
    Fast = 401001800,               //加速
    Spurting = 401001900,           //溅射
    BlowUp = 401002000,             //击飞
    Silence = 401002100,            //沉默
    Virtualize = 401002200,         //虚无
    Invincible = 401002400,         //无敌
    AbsorbDamage = 401002500,       //护盾
    Imprison = 401002600,           //禁锢
    KnockDown = 401002700,          //击倒
    Invisible = 401002800,          //隐身
    MagicImmunity = 401002900,      //魔免
    PhysicalImmunity = 401003000,   //物免
    ControlImmunity = 401003100,    //免控
    Blur = 401003200,               //模糊
    ReduceCure = 401003300,         //重伤(降低治疗效果)
    Talent = 401003400,             //天赋buff
    Poison = 401003500,             //中毒
    Curse = 401003600,              //诅咒
    DelayEffective = 401003700,     //延迟生效(减益类)
    Purification = 401003800,       //净化
    Disarm = 401003900,             //缴械
    RollUp = 401004000,             //悬空
    Hover = 401004100,              //滞空
    Petrified = 401004200,          //石化
    Grow = 401004300,               //长大
    LoseBlood = 401004400,          //失血
    ReduceArmor = 401004500,        //减少护甲
    ReduceMagicResist = 401004600,  //减少魔法抗性
    AddMagicResist = 401004700,     //加魔法抗性
    Summon = 401004800,             //召唤
    Netted = 401004900,             //网住
    Burning = 401005000,            //灼烧
    Fury = 401005100,               //暴怒(攻击力)
    Rage = 401005200,               //狂暴(攻击速度)
    SkillTalent = 401005300,        //技能天赋buff
    AddSkillDamage = 401005400      //技能伤害加成
}

public class SkillBuff
{
    public CharacterState target;
    public CharacterState attacker;
    public bool showHud = true;
    public long id;
    public float baseValue;
    public float last;

    public List<long> refuseList = new List<long>();

    public CDTimer.CD cd;
    public SkillBuffNode node;
    public GameObject buffGo;
    public List<SkillBuff> talentBuff = new List<SkillBuff>();
    public SkillNode skillNode;
    public long skillId;
    public int lvl;
    public List<RolePart> mCurRolePart = new List<RolePart>();

    public SkillBuff (float baseVal, object p )
    {
        baseValue = baseVal;
        if(p is Array)
        {
            Array arr = (Array)p;
            if(arr.Length > 0)
            {
                id = Convert.ToInt64(arr.GetValue(0));
                node = FSDataNodeTable<SkillBuffNode>.GetSingleton().DataNodeList[id];
            }
            if(arr.Length > 1)
            {
                last = float.Parse(arr.GetValue(1).ToString());
            }
        }
    }

    public virtual void Init ()
    {
        cd = CDTimer.GetInstance().AddCD(last, ( int c, long cid ) => SkillBuffManager.GetInst().RemoveCalculateCurTargetProp(target, this));
        AddBuffEffect(GameLibrary.Effect_Buff + node.effect, GetBuffParent());
    }

    private Transform GetBuffParent()
    {
        Transform mCurTrans = null;
        switch (node.effectPos)
        {
            case 0:
                mCurTrans = target.mHeadPoint;
                break;
            case 1:
                mCurTrans = target.transform;
                break;
            case 2:
                mCurTrans = target.mHitPoint;
                break;
            default:
                break;
        }
        return mCurTrans;
    }

    public STATUS GetEffectOfBuff()
    {
        switch ((SkillBuffType)id)
        {
            case SkillBuffType.Dizzy:
            case SkillBuffType.Frozen:
            case SkillBuffType.KnockBack:
            case SkillBuffType.KnockDown:
            case SkillBuffType.BlowUp:
                return STATUS.CANNOTCONTROL;
            case SkillBuffType.Imprison:
                return STATUS.CANNOTMOVE;
            case SkillBuffType.Silence:
                return STATUS.CANNOTSKILL;
            case SkillBuffType.Virtualize:
                return STATUS.CANNOTATTACK;
            default:
                return STATUS.NONE;
        }
    }

    // 执行buff状态
    public virtual void Excute (CharacterState cs, float mCurValue)
    {

    }

    // 恢复到中buff之前的状态
    public virtual void Reverse (CharacterState cs, float mCurValue)
    {
        SkillBuffManager.GetInst().RemoveBuffFrom(this, target);
        RemoveBuffEffect();
        //Debug.Log("Reverse " + id);
    }

    public void SetBuffSkillNode(SkillNode tempSkillNode, int lvl = 1)
    {
        skillNode = tempSkillNode;
        skillId = tempSkillNode == null ? 0 : tempSkillNode.skill_id;
        this.lvl = lvl;
    }

    // 强制移除buff
    public virtual void Remove(CharacterState cs)
    {
        if (cd != null)
        {
            CDTimer.GetInstance().RemoveCD(cd);
        }
        Reverse(cs, baseValue);
    }

    void AddBuffEffect ( string path, Transform parent = null )
    {
        if(buffGo != null)
            GameObject.Destroy(buffGo);
        GameObject prefab = Resources.Load(path) as GameObject;
        if (prefab != null)
        {
            buffGo = GameObject.Instantiate(prefab);
            buffGo.transform.parent = parent;
            buffGo.transform.localPosition = Vector3.zero;
            buffGo.transform.localScale = Vector3.one;
            buffGo.SetActive(false);
            SkillBuffManager.GetInst().SetBuffActive(target, node.effectPos);
        }
        else
        {
            buffGo = null;
        }
    }

    public void RemoveBuffEffect ()
    {
        if(buffGo != null)
        {
            GameObject.Destroy(buffGo);
        }
        SkillBuffManager.GetInst().SetBuffActive(target, node.effectPos);
    }
}

public class SkillBuffManager {

    Dictionary<CharacterState, List<SkillBuff>> CharBuffs = new Dictionary<CharacterState, List<SkillBuff>>();

    private static SkillBuffManager inst;
    public static SkillBuffManager GetInst ()
    {
        if(inst == null)
            inst = new SkillBuffManager();
        return inst;
    }

    public static SkillBuffType GetBuffType ( object buffConfig ) {
        if(buffConfig is Array && ( (Array)buffConfig ).Length > 0)
        {
            long id = Convert.ToInt64(( (Array)buffConfig ).GetValue(0));
            return (SkillBuffType)id;
        }
        return SkillBuffType.Normal;
    }

    public SkillBuff CreateBuff (float baseVal, object buffConfig)
    {
        if (buffConfig is Array && ((Array)buffConfig).Length > 0)
        {
            long id = Convert.ToInt64(((Array)buffConfig).GetValue(0));
            switch ((SkillBuffType)id)
            {
                case SkillBuffType.Normal:
                    break;
                case SkillBuffType.AddonAttack:
                    return new BF_AddonAttack(baseVal, buffConfig);
                case SkillBuffType.Slow:
                    return new BF_Slow(baseVal, buffConfig);
                case SkillBuffType.Frozen:
                    return new BF_Frozen(baseVal, buffConfig);
                case SkillBuffType.Resist:
                    return new BF_Resist(baseVal, buffConfig);
                case SkillBuffType.Bleeding:
                case SkillBuffType.Burning:
                    return new BF_Bleeding(baseVal, buffConfig);
                case SkillBuffType.KnockBack:
                    return new BF_KnockBack(baseVal, buffConfig);
                case SkillBuffType.AddSuckBlood:
                    return new BF_AddSuckBlood(baseVal, buffConfig);
                case SkillBuffType.AddAttack:
                case SkillBuffType.Fury:
                    return new BF_AddAttack(baseVal, buffConfig);
                case SkillBuffType.AddMaxHp:
                    return new BF_AddMaxHp(baseVal, buffConfig);
                case SkillBuffType.Cure:
                    return new BF_Cure(baseVal, buffConfig);
                case SkillBuffType.Restore:
                    return new BF_Restore(baseVal, buffConfig);
                case SkillBuffType.AddArmor:
                    return new BF_AddArmor(baseVal, buffConfig);
                case SkillBuffType.WoundDeeper:
                    return new BF_WoundDeeper(baseVal, buffConfig);
                case SkillBuffType.Dizzy:
                    return new BF_Dizzy(baseVal, buffConfig);
                case SkillBuffType.AddAtkSpeed:
                case SkillBuffType.Rage:
                    return new BF_AddAtkSpeed(baseVal, buffConfig);
                case SkillBuffType.ReduceAtkSpeed:
                    return new BF_ReduceAtkSpeed(baseVal, buffConfig);
                case SkillBuffType.SuckBloodHalo:
                    return new BF_SuckBloodHalo(baseVal, buffConfig);
                case SkillBuffType.Fast:
                    return new BF_Fast(baseVal, buffConfig);
                case SkillBuffType.Spurting:
                    break;
                case SkillBuffType.BlowUp:
                    return new BF_BlowUp(baseVal, buffConfig);
                case SkillBuffType.Silence:
                    return new BF_Silence(baseVal, buffConfig);
                case SkillBuffType.Virtualize:
                    return new BF_Virtualize(baseVal, buffConfig);
                case SkillBuffType.Invincible:
                    return new BF_Invincible(baseVal, buffConfig);
                case SkillBuffType.AbsorbDamage:
                    return new BF_AbsorbDamage(baseVal, buffConfig);
                case SkillBuffType.Imprison:
                case SkillBuffType.Netted:
                    return new BF_Imprision(baseVal, buffConfig);
                case SkillBuffType.KnockDown:
                    break;
                case SkillBuffType.Invisible:
                    return new BF_Invisible(baseVal, buffConfig);
                case SkillBuffType.MagicImmunity:
                    return new Bf_MagicImmunity(baseVal, buffConfig);
                case SkillBuffType.PhysicalImmunity:
                    return new BF_PhysicalImmunity(baseVal, buffConfig);
                case SkillBuffType.ControlImmunity:
                    return new BF_ControlImmunity(baseVal, buffConfig);
                case SkillBuffType.Blur:
                    return new BF_Blur(baseVal, buffConfig);
                case SkillBuffType.ReduceCure:
                    return new BF_ReduceCure(baseVal, buffConfig);
                case SkillBuffType.Poison:
                    return new BF_Poison(baseVal, buffConfig);
                case SkillBuffType.Curse:
                    return new BF_Curse(baseVal, buffConfig);
                case SkillBuffType.DelayEffective:
                    return new BF_DelayEffective(baseVal, buffConfig);
                case SkillBuffType.Purification:
                    return new BF_Purification(baseVal, buffConfig);
                case SkillBuffType.Disarm:
                    return new BF_Disarm(baseVal, buffConfig);
                case SkillBuffType.RollUp:
                    return new BF_RollUp(baseVal, buffConfig);
                case SkillBuffType.Hover:
                    return new BF_Hover(baseVal, buffConfig);
                case SkillBuffType.Petrified:
                    return new BF_Petrified(baseVal, buffConfig);
                case SkillBuffType.Grow:
                    return new BF_Grown(baseVal, buffConfig);
                case SkillBuffType.LoseBlood:
                    return new BF_LoseBlood(baseVal, buffConfig);
                case SkillBuffType.ReduceArmor:
                    return new BF_ReduceArmor(baseVal, buffConfig);
                case SkillBuffType.ReduceMagicResist:
                    return new BF_ReduceMagicResist(baseVal, buffConfig);
                case SkillBuffType.AddMagicResist:
                    return new BF_AddMagicResist(baseVal, buffConfig);
                case SkillBuffType.Summon:
                    return new BF_Summon(baseVal, buffConfig);
                case SkillBuffType.SkillTalent:
                    return new BF_SkillTalent(baseVal, buffConfig);
                case SkillBuffType.AddSkillDamage:
                    return new BF_AddSkillDamage(baseVal, buffConfig);
                default:
                    break;
            }
        }
        return new SkillBuff(baseVal, buffConfig);
    }

    public SkillBuff AddBuffs(float baseVal, object buffConfig, CharacterState targetCS, CharacterState attackerCS, SkillNode skillNode = null, int lvl = 1, bool needCheck = true)
    {
        return AddBuffTo(CreateBuff(baseVal, buffConfig), targetCS, attackerCS, skillNode, lvl, needCheck);
    }

    public void RemoveBuffFrom (SkillBuff sBuff, CharacterState cs)
    {
        if(CharBuffs.ContainsKey(cs))
        {
            if(CharBuffs[cs].Contains(sBuff))
            {
                CharBuffs[cs].Remove(sBuff);
            }
        }
    }

    public void ClearBuffsFrom (CharacterState cs)
    {
        if(CharBuffs.ContainsKey(cs))
        {
            for(int i = 0; i< CharBuffs[cs].Count; i++)
            {
                RemoveCalculateCurTargetProp(cs, CharBuffs[cs][i]);
                CDTimer.GetInstance().RemoveCD(CharBuffs[cs][i].cd);
                RemoveContinousSkillBuff(CharBuffs[cs][i]);
            }
            CharBuffs.Remove(cs);
        }
    }

    public List<SkillBuff> GetSkillBuffListByCs(CharacterState cs)
    {
        List<SkillBuff> resultList = new List<SkillBuff>();
        if (CharBuffs.ContainsKey(cs))
        {
            resultList = CharBuffs[cs];
        }
        return resultList;
    }

    public List<SkillBuff> GetAllDebuffByCs(CharacterState cs)
    {
        List<SkillBuff> resultList = new List<SkillBuff>();
        List<SkillBuff> tempList = GetSkillBuffListByCs(cs);
        for (int i = 0; i < tempList.Count; i++)
        {
            SkillBuff tempBuff = tempList[i];
            if (tempBuff.node.type == BuffType.control || tempBuff.node.type == BuffType.decrease || tempBuff.node.type == BuffType.continuousDecrease || tempBuff.node.type == BuffType.DelayEffective)
            {
                resultList.Add(tempBuff);
            }
        }
        return resultList;
    }

    public void RemoveContinousSkillBuff(SkillBuff tempBuff)
    {
        if (tempBuff.node.type == BuffType.continuousDecrease || tempBuff.node.type == BuffType.continuousIncrease)
        {
            if (tempBuff.node.type == BuffType.continuousDecrease)
            {
                CDTimer.GetInstance().RemoveCD((tempBuff as BF_Bleeding).bleedCd);
            }
            else
            {
                CDTimer.GetInstance().RemoveCD((tempBuff as BF_Restore).restoreCd);
            }
        }
    }

    public SkillBuff AddBuffTo (SkillBuff sBuff , CharacterState cs, CharacterState attackerCS, SkillNode skillNode, int lvl, bool needCheck)
    {
        List<CharacterState> keys = new List<CharacterState>(CharBuffs.Keys);
        for (int i = keys.Count - 1; i >= 0; i--)
        {
            if (keys [i] == null)
            {
                CharBuffs.Remove(keys[i]);
            }
        }
        if (CheckCanAdd(sBuff, cs, attackerCS) || !needCheck)
        {
            if (CharBuffs.ContainsKey(cs))
            {
                CharBuffs[cs].Add(sBuff);
            }
            else
            {
                List<SkillBuff> buffs = new List<SkillBuff>();
                buffs.Add(sBuff);
                CharBuffs.Add(cs, buffs);
            }
            sBuff.target = cs;
            sBuff.attacker = attackerCS;
            sBuff.SetBuffSkillNode(skillNode, lvl);
            sBuff.showHud = GameLibrary.Instance().ShouldShowHud(cs, attackerCS);
            sBuff.Init();
            AddCalculateCurTargetProp(cs, sBuff);
            //sBuff.Excute(cs);
            cs.OnDead += (CharacterState mcs) => RemoveCalculateCurTargetProp(mcs, sBuff);
            return sBuff;
        }
        return null;
    }

    /// <summary>
    /// 通过目标和buff类型判断添加buff之后造成的效果
    /// </summary>
    /// <param name="cs"></param>
    /// <param name="sBuff"></param>
    public void AddCalculateCurTargetProp(CharacterState cs, SkillBuff sBuff)
    {
        if (CharBuffs.ContainsKey(cs))
        {
            float resultValue = 0;
            List<SkillBuff> mCurSkillBuff = new List<SkillBuff>();
            //持续减益/增益类buff相互抵消
            if (sBuff.node.type == BuffType.continuousDecrease || sBuff.node.type == BuffType.continuousIncrease)
            {
                SkillBuff tempBuff = CharBuffs[cs].Find(a => a != sBuff && a.node.type == sBuff.node.type && a.skillNode.skill_id == sBuff.skillNode.skill_id);
                if (tempBuff != null)
                {
                    tempBuff.Reverse(cs, tempBuff.baseValue);
                    RemoveContinousSkillBuff(tempBuff);
                }
            }
            else
            {
                //查找和当前buff同类型的
                mCurSkillBuff = CharBuffs[cs].FindAll(a => a != sBuff && a.node.type == sBuff.node.type && a.node.buffActionType == sBuff.node.buffActionType);
            }
            //不存在取当前值，存在取差值
            if (mCurSkillBuff.Count > 0)
            {
                mCurSkillBuff.Sort((a, b) => Mathf.FloorToInt(b.baseValue - a.baseValue));
                if (sBuff.baseValue > mCurSkillBuff[0].baseValue)
                {
                    resultValue = sBuff.baseValue - mCurSkillBuff[0].baseValue;
                }
            }
            else
            {
                resultValue = sBuff.baseValue;
            }
            sBuff.Excute(cs, resultValue);
        }
    }

    public void RemoveCalculateCurTargetProp(CharacterState cs, SkillBuff sBuff)
    {
        if (cs != null && CharBuffs.ContainsKey(cs) && CharBuffs[cs].Contains(sBuff))
        {
            float resultValue = 0;
            //查找和当前buff同类型的
            List<SkillBuff> mCurSkillBuff = CharBuffs[cs].FindAll(a => a != sBuff && a.node.type == sBuff.node.type && a.node.buffActionType == sBuff.node.buffActionType);
            //增益减益的不存在取当前值，存在取差值，其它的只有最后一个buff存在的值才为0
            if ((sBuff.node.type == BuffType.increase || sBuff.node.type == BuffType.decrease) && (SkillBuffType)sBuff.id != SkillBuffType.AbsorbDamage)
            {
                if (mCurSkillBuff.Count > 0)
                {
                    mCurSkillBuff.Sort((a, b) => Mathf.FloorToInt(b.baseValue - a.baseValue));
                    if (sBuff.baseValue > mCurSkillBuff[0].baseValue)
                    {
                        resultValue = sBuff.baseValue - mCurSkillBuff[0].baseValue;
                    }
                }
                else
                {
                    resultValue = sBuff.baseValue;
                }
            }
            else
            {
                resultValue = mCurSkillBuff.Count;
            }
            sBuff.Reverse(cs, resultValue);

        }
    }

    bool CheckCanAdd ( SkillBuff sBuff, CharacterState cs, CharacterState attackerCS)
    {
        if (attackerCS == null) return cs.RecieveControl;
        if (attackerCS.groupIndex != cs.groupIndex)
        {
            if (cs.RecieveControl)
            {
                if (GameLibrary.Instance().CheckNotHeroBoss(cs))
                {
                    return (sBuff.node.type == BuffType.decrease || sBuff.node.type == BuffType.continuousDecrease || sBuff.node.type == BuffType.DelayEffective) && sBuff.id != (long)SkillBuffType.Slow;
                }
                else
                {
                    return sBuff.node.type == BuffType.control || sBuff.node.type == BuffType.decrease || sBuff.node.type == BuffType.translate || sBuff.node.type == BuffType.continuousDecrease || sBuff.node.type == BuffType.DelayEffective;
                }
            }
            else
            {
                return false;
            }
        }
        else
        {
            return sBuff.node.type == BuffType.increase || sBuff.node.type == BuffType.immune || sBuff.node.type == BuffType.other || sBuff.node.type == BuffType.continuousIncrease || sBuff.node.type == BuffType.talent || sBuff.node.type == BuffType.Summon;
        }
    }

    public void SetBuffActive(CharacterState target, int effectPos)
    {
        if (target.Invisible && target.groupIndex == 0) return;
        List<SkillBuff> buffs = SkillBuffManager.GetInst().GetSkillBuffListByCs(target);
        buffs.Sort(SortSkillBuff);
        SkillBuff mExitBuff = buffs.Find(a => a.node.effectPos == effectPos && a.buffGo != null && a.buffGo.gameObject.activeSelf);
        SkillBuff mCurBuff = buffs.Find(a => a.node.effectPos == effectPos && a.buffGo != null && !a.buffGo.gameObject.activeSelf);
        if (mExitBuff != null)
        {
            if (mCurBuff != null)
            {
                if (buffs.IndexOf(mCurBuff) > buffs.IndexOf(mExitBuff) && mCurBuff.id != mExitBuff.id)
                {
                    mExitBuff.buffGo.gameObject.SetActive(false);
                    mCurBuff.buffGo.gameObject.SetActive(true);
                }
            }
        }
        else
        {
            if (mCurBuff != null)
            {
                mCurBuff.buffGo.gameObject.SetActive(true);
            }
        }
    }

    private int SortSkillBuff(SkillBuff a, SkillBuff b)
    {
        if (a.node.priorityBuff == b.node.priorityBuff)
        {
            return b.node.priorityShow - a.node.priorityShow;
        }
        else
        {
            return b.node.priorityBuff - a.node.priorityShow;
        }
    }
}
