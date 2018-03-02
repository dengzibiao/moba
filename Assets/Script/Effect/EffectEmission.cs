using UnityEngine;
using Tianyu;
using System.Collections.Generic;

public enum AttackResultType
{
    //普通，暴击，闪避，恢复
    Normal, Crit, Dodge, Recover
}
public enum EffectType
{
    //单体，buff，近身，远程
    Single, Buff, Melee, LongRange
}

public enum SkillSection
{
    //单体，buff，近身，远程
    Spell,
    Effect
}

public class EffectEmission : MonoBehaviour
{
    [HideInInspector]
    public string fold = "";
    private Effect_Quartz eft;
    public EffectTrackBase et { get; set; }
    static string[] keys = { "attack1", "attack2", "attack3", "skill1", "skill2", "skill3", "skill4" };
    List<string> mNormalSkillkeys = new List<string> { "attack", "attack0", "attack1", "attack2", "attack3" };

    CharacterState _cs;
    CharacterState cs {
        get {
            if(_cs == null)
                _cs = GetComponentInParent<CharacterState>();
            return _cs;
        }
    }
    [HideInInspector]
    public GameObject nip;
    private string mEffectRoot;

    void Awake()
    {
        EffectNip();
        //模型顶级的名字为路径名
        fold = this.name.Split('&')[1];
        eft = GetComponent<Effect_Quartz>();
    }
    /// <summary>
    /// 生成父容器
    /// </summary>
    private void EffectNip()
    {
        nip = GameObject.FindGameObjectWithTag(Tag.effectNip);
        if (nip == null)
        {
            nip = new GameObject("EffectNip");
            nip.tag = Tag.effectNip;
        }
    }

    private Dictionary<GameObject, GameObject> AddMultiAttackEffect(SkillNode skillNode, string id, GameObject target, Transform emissionPoint)
    {
        Dictionary<GameObject, GameObject> result = new Dictionary<GameObject, GameObject>();
        List<GameObject> mViewTarget = GetMultiTargetByCondition(skillNode);
        for (int i = 0; i < mViewTarget.Count; i++)
        {
            result.Add(AddAttackEffect(id, target, skillNode, emissionPoint), mViewTarget [i]);
        }
        return result;
    }

    public List<GameObject> GetMultiTargetByCondition(SkillNode skillNode)
    {
        List<GameObject> mTempMonsters = new List<GameObject>();
        Collider[] colliders = Physics.OverlapSphere(cs.transform.position, GameLibrary.Instance().GetSkillDistBySkillAndTarget(cs, skillNode), GameLibrary.GetAllLayer());
        for (int i = 0; i < colliders.Length; i++)
        {
            CharacterState mCurTargetCs = colliders[i].GetComponent<CharacterState>();
            if (mCurTargetCs != null)
            {
                Vector3 direction = mCurTargetCs.transform.position - cs.transform.position;
                if (GameLibrary.Instance().CheckHitCondition(skillNode, cs, mCurTargetCs) && GameLibrary.Instance().IsInvisiblityCanSetTarget(cs, mCurTargetCs)
                && (skillNode.angle == 0 ? true : direction == Vector3.zero ? true : Vector3.Angle(cs.transform.forward, direction) <= skillNode.angle / 2))
                {
                    mTempMonsters.Add(mCurTargetCs.gameObject);
                }
            }
        }
        if (skillNode.target_ceiling != 0)
        {
            mTempMonsters.Sort((a, b) =>
            {
                if (a == cs.attackTarget)
                {
                    return -1;
                }
                if (b == cs.attackTarget)
                {
                    return 1;
                }
                float aDis = Vector3.Distance(cs.transform.position, a.transform.position);
                float bDis = Vector3.Distance(cs.transform.position, b.transform.position);
                return Mathf.FloorToInt(aDis - bDis);
            });
            mTempMonsters = mTempMonsters.GetRange(0, mTempMonsters.Count < skillNode.target_ceiling ? mTempMonsters.Count : skillNode.target_ceiling);
        }
        return mTempMonsters;
    }

    public GameObject AddAttackEffect(string id, GameObject target, SkillNode skillNode, Transform emissionPoint = null)
    {
        string idIndex = id;
        if (skillNode != null && skillNode.site == 0 &&
            (cs.mCurMobalId == MobaObjectID.HeroXiaohei || cs.mCurMobalId == MobaObjectID.HeroChenmo))
        {
            SkillBuff buff = SkillBuffManager.GetInst().GetSkillBuffListByCs(cs).Find
                (a => (SkillBuffType)a.id == SkillBuffType.Talent && a.attacker == cs);
            if (buff != null)
            {
                idIndex += "_1";
            }
        }
        else if (skillNode != null && skillNode.site == 1 && cs.mCurMobalId == MobaObjectID.HeroLuosa)
        {
            SkillBuff buff = SkillBuffManager.GetInst().GetSkillBuffListByCs(cs).Find
                (a => (SkillBuffType)a.id == SkillBuffType.SkillTalent && a.attacker == cs);
            idIndex += buff != null ? "_2_1" : "_1_1";
        }
        GameObject effect_go = BattleUtil.AddEffectTo(GetEffectResourceRoot() + idIndex, transform, id, skillNode, cs);
        if (effect_go != null)
        {
            effect_go.transform.parent = nip.transform;
            if (emissionPoint != null)
                effect_go.transform.position = emissionPoint.position;
        }
        return effect_go;
    }

    public void PlayAttackEffect(string id, GameObject target, Transform emissionPoint = null)
    {
        SkillNode skillNode = GetSkillNodeByKey(id, cs);
        if (skillNode.skill_type == SkillCastType.MultiTrackSkill || skillNode.skill_type == SkillCastType.MultiTractionSkill)
        {
            Dictionary<GameObject, GameObject> effects = AddMultiAttackEffect(skillNode, id, target, emissionPoint);
            List<GameObject> effectKeys = new List<GameObject>(effects.Keys);
            for (int i = 0; i < effectKeys.Count; i++)
            {
                et = SetSkillCategory(skillNode, effectKeys[i]);
                if (et != null)
                    et.Init(skillNode, effects[effectKeys[i]], transform.parent);
            }
        }
        else
        {
            GameObject effect_go = AddAttackEffect(id, target, skillNode, emissionPoint);
            if (effect_go == null && skillNode.skill_type == SkillCastType.ToSelfCenterSkill)
            {
                effect_go = new GameObject("buff");
                effect_go.transform.position = transform.position;
                effect_go.transform.parent = nip.transform;
            }
            if (effect_go != null)
            {
                et = SetSkillCategory(skillNode, effect_go);
                if (et != null)
                {
                    if (skillNode.skill_type == SkillCastType.MeleeEffect && GetSkillIsAttack(id))
                    {
                        ChangeSpeed(effect_go.transform, cs.attackSpeed);
                        et.destoryTime = skillNode.efficiency_time / cs.attackSpeed;
                    }
                    et.Init(skillNode, target, transform.parent);
                }
            }
            else
            {
                et = null;
            }
        }
    }
    /// <summary>
    /// 通过技能类型分类
    /// </summary>
    /// <param name="mCurSkillNode"></param>
    /// <param name="go"></param>
    /// <returns></returns>
    public EffectTrackBase SetSkillCategory(SkillNode mCurSkillNode, GameObject go)
    {
        if (mCurSkillNode == null) return null;
        SkillCastType type = mCurSkillNode.skill_type;
        EffectTrackBase[] effectBase = go.GetComponents<EffectTrackBase>();
        for (int i = 0; i < effectBase.Length; i++)
        {
            Destroy(effectBase[i]);
        }
        EffectTrackBase et = null;
        switch (type)
        {
            case SkillCastType.MeleeEffect:
                et = go.AddComponent<EffectMelee>();
                et.distance = mCurSkillNode.dist;
                et.destoryTime = mCurSkillNode.efficiency_time;
                break;
            case SkillCastType.CastSkill:
                et = go.AddComponent<EffectMonomer>();
                et.destoryTime = mCurSkillNode.max_fly / mCurSkillNode.flight_speed;
                break;
            case SkillCastType.FlyEffect:
                et = go.AddComponent<FlyPropUnit>();
                et.destoryTime = mCurSkillNode.efficiency_time;
                if (mCurSkillNode.aoe_long != 0)
                {
                    et.distance = mCurSkillNode.aoe_long;
                }
                break;
            case SkillCastType.BlinkSkill:
                et = go.AddComponent<EffectBlink>();
                if (mCurSkillNode.aoe_long != 0)
                {
                    et.distance = mCurSkillNode.aoe_long;
                }
                et.destoryTime = mCurSkillNode.efficiency_time;
                break;
            case SkillCastType.FrontSprintSkill:
                et = go.AddComponent<EffectForward>();
                et.destoryTime = mCurSkillNode.max_fly / mCurSkillNode.flight_speed;
                et.distance = mCurSkillNode.aoe_long;
                if (cs.mCurMobalId == MobaObjectID.HeroXiaoxiao)
                {
                    SkillBuff mGrowBuff = SkillBuffManager.GetInst().GetSkillBuffListByCs(cs).Find(a => a.id == (long)SkillBuffType.Grow);
                    et.distance += mGrowBuff == null ? 0 : et.distance * 0.4f;
                }
                if (mCurSkillNode.efficiency_time != 0)
                {
                    et.efficiency_time = mCurSkillNode.efficiency_time;
                }
                break;
            case SkillCastType.ShockWaveSkill:
                et = go.AddComponent<EffectStraightFly>();
                et.destoryTime = mCurSkillNode.flight_speed == 0 ? mCurSkillNode.efficiency_time : mCurSkillNode.max_fly / mCurSkillNode.flight_speed;
                et.efficiency_time = mCurSkillNode.efficiency_time;
                et.distance = mCurSkillNode.aoe_long;
                break;
            case SkillCastType.TrackSkill:
                et = go.AddComponent<EffectTrack>();
                if (mCurSkillNode.aoe_long != 0)
                {
                    et.distance = mCurSkillNode.aoe_long;
                }
                break;
            case SkillCastType.CenterSkill:
                et = go.AddComponent<EffectAOECenter>();
                if (mCurSkillNode.aoe_long != 0)
                {
                    (et as EffectAOECenter).aoeType = mCurSkillNode.aoe_wide == 0 ? AoeType.CircleAoe : AoeType.RectAoe;
                    et.distance = mCurSkillNode.aoe_long;
                }
                break;
            case SkillCastType.TrapSkill:
                break;
            case SkillCastType.SummonSkill:
                et = go.AddComponent<EffectSummon>();
                break;
            case SkillCastType.AoeSKill:
                et = go.AddComponent<EffectAOE>();
                if (mCurSkillNode.aoe_long != 0)
                {
                    (et as EffectAOE).aoeType = mCurSkillNode.aoe_wide == 0 ? AoeType.CircleAoe : AoeType.RectAoe;
                    et.distance = mCurSkillNode.aoe_long;
                }
                break;
            case SkillCastType.LinkSkill:
                et = go.AddComponent<EffectLink>();
                et.destoryTime = mCurSkillNode.efficiency_time;
                et.distance = mCurSkillNode.aoe_long;
                break;
            case SkillCastType.ToSelfCenterSkill:
                et = go.AddComponent<EffectCircle>();
                if (mCurSkillNode.aoe_long != 0)
                {
                    (et as EffectCircle).aoeType = mCurSkillNode.aoe_wide == 0 ? AoeType.CircleAoe : AoeType.RectAoe;
                    et.distance = mCurSkillNode.aoe_long;
                }
                break;
            case SkillCastType.ChainSkill:
                et = go.AddComponent<EffectChain>();
                et.destoryTime = mCurSkillNode.efficiency_time;
                break;
            case SkillCastType.Boom:
                et = go.AddComponent<ThrowEffect>();
                if (mCurSkillNode.aoe_long != 0)
                {
                    et.distance = mCurSkillNode.aoe_long;
                }
                et.destoryTime = mCurSkillNode.efficiency_time;
                break;
            case SkillCastType.Bounce:
                et = go.AddComponent<EffectBounce>();
                if (mCurSkillNode.aoe_long != 0)
                {
                    et.distance = mCurSkillNode.aoe_long;
                }
                break;
            case SkillCastType.MultiTrackSkill:
                et = go.AddComponent<EffectTrack>();
                if (mCurSkillNode.aoe_long != 0)
                {
                    et.distance = mCurSkillNode.aoe_long;
                }
                break;
            case SkillCastType.MoveShrinkSkill:
                et = go.AddComponent<EffectMoveDiffusionShrink>();
                break;
            case SkillCastType.BurrowSkill:
                et = go.AddComponent<EffectBurrow>();
                if (mCurSkillNode.aoe_long != 0)
                {
                    (et as EffectBurrow).aoeType = mCurSkillNode.aoe_wide == 0 ? AoeType.CircleAoe : AoeType.RectAoe;
                    et.distance = mCurSkillNode.aoe_long;
                }
                et.destoryTime = mCurSkillNode.efficiency_time;
                break;
            case SkillCastType.JumpChopSkill:
                et = go.AddComponent<EffectJumpChop>();
                et.destoryTime = mCurSkillNode.efficiency_time;
                et.distance = mCurSkillNode.aoe_long;
                break;
            case SkillCastType.MultiBounce:
                et = go.AddComponent<EffectMultiBounce>();
                if (mCurSkillNode.aoe_long != 0)
                {
                    et.distance = mCurSkillNode.aoe_long;
                }
                et.destoryTime = mCurSkillNode.efficiency_time;
                break;
            case SkillCastType.GenerateObstacle:
                et = go.AddComponent<EffectGenerateObstacle>();
                et.destoryTime = mCurSkillNode.efficiency_time;
                break;
            case SkillCastType.DiffuseSkill:
                et = go.AddComponent<EffectDiffuse>();
                et.destoryTime = mCurSkillNode.efficiency_time;
                break;
            case SkillCastType.MultiTractionSkill:
                et = go.AddComponent<EffectTraction>();
                if (mCurSkillNode.aoe_long != 0)
                {
                    et.distance = mCurSkillNode.aoe_long;
                }
                break;
            case SkillCastType.LaunchDiffuseMeterSkill:
                et = go.AddComponent<EffectLaunchDiffuseMeter>();
                et.destoryTime = mCurSkillNode.efficiency_time;
                break;
            case SkillCastType.SnakeSteerSkill:
                et = go.AddComponent<EffectSnakeSteer>();
                if (mCurSkillNode.aoe_long != 0)
                {
                    et.distance = mCurSkillNode.aoe_long;
                }
                break;
            default:
                break;
        }
        return et;
    }

    /// <summary>
    /// 攻击结束触发，（攻击物体，伤害值）
    /// </summary>

    static BattleEffect[] GetSkillEffects(SkillNode Node)
    {
        BattleEffect[] effs = new BattleEffect[Node.battleEffects.Length];
        SkillEffectNode[] effNodes = SkillNode.GetSkillEffectNodes(Node.battleEffects);
        for (int i = 0; i < effs.Length; i++)
        {
            effs[i] = new ChangeValueEffect(effNodes[i]);
        }
        return effs;
    }

    public static BattleEffect GetBattleEffect(string s, CharacterState attackerCS)
    {
        BattleEffect[] effects = GetSkillEffects(GetSkillNodeByKey(s, attackerCS));
        return effects[0];
    }

    public static SkillNode GetSkillNodeByKey(string s, CharacterState attackerCS)
    {
        if (s == "attack" || s == "attack0")
        {
            return FSDataNodeTable<SkillNode>.GetSingleton().DataNodeList[attackerCS.CharData.attrNode.skill_id[0]];
        }
        for (int i = 0; i < keys.Length; i++)
        {
            if (keys[i] == s)
            {
                long skillKey = 0;
                if (i > 2 && (GameLibrary.Instance().CheckNotHeroBoss(attackerCS) || GameLibrary.Instance().CheckIsEliteMonster(attackerCS)
                    || attackerCS.mCurMobalId == MobaObjectID.HeroMeidusha || attackerCS.mCurMobalId == MobaObjectID.HeroShenling))
                {
					for (int j = 0; j < attackerCS.CharData.attrNode.skill_id.Length; j++)
                    {
                        long mCurkeys = attackerCS.CharData.attrNode.skill_id[j];
                        SkillNode mCurSkillNode;
                        GameLibrary.skillNodeList.TryGetValue(mCurkeys, out mCurSkillNode);
                        if (mCurSkillNode != null && mCurSkillNode.site == i - 2)
                        {
                            skillKey = mCurkeys;
                            break;
                        }
                    }
                }
                else
                {
                    skillKey = attackerCS.CharData.attrNode.skill_id[i];
                }
                if (GameLibrary.skillNodeList.ContainsKey(skillKey))
                {
                    return GameLibrary.skillNodeList[skillKey];
                }
                else
                {
                    Debug.Log(skillKey);
                }
            }
        }
        return null;
    }

    public void PlaySpellEffect(string id)
    {
        if (eft != null)
        {
            eft.Quartz(GetEffectResourceRoot() + id + "_shifa", id);
            if (cs.GetMobaName().Equals("boss_003") && id.Equals("skill3") ||
                (cs.GetMobaName().Equals("boss_006") && id.Equals("skill4")))
            {
                GameObject spellEffect;
                eft.dic.TryGetValue(id, out spellEffect);
                CharacterPart cp = cs.playerPart.Find(a => a.mRolePart == RolePart.Head);
                if (spellEffect != null && cp != null)
                {
                    spellEffect.transform.parent = cp.transform;
                    spellEffect.transform.localPosition = Vector3.zero;
                    spellEffect.transform.localRotation = Quaternion.identity;
                    spellEffect.transform.localScale = Vector3.one;
                }
            }
        }
    }

    public void PlaySpellEffectByUrl(string url, string id)
    {
        if (eft != null)
        {
            eft.Quartz(url, id);
        }
    }

    public void PlayHitEffect(SkillNode effect, CharacterState targetCs, CharacterState attackerCs)
    {
        if (attackerCs == null || !(attackerCs.state == Modestatus.NpcPlayer || attackerCs.state == Modestatus.Player || attackerCs.state == Modestatus.Boss || attackerCs.state == Modestatus.SummonHero || (attackerCs.state == Modestatus.Monster && effect.skill_type == SkillCastType.CastSkill))
            || effect.skill_type == SkillCastType.Boom || effect.range_type == rangeType.spurting) return;
        Transform hit = (targetCs.mHitPoint == null || (attackerCs.mCurMobalId == MobaObjectID.HeroShengqi && effect.site == 4)) ? targetCs.transform : targetCs.mHitPoint;
        GameObject effGo = BattleUtil.AddEffectTo(GetEffectRoot(attackerCs) + attackerCs.emission.fold + "/" + effect.spell_motion + "_hit", hit);
        if (effGo != null)
        {
            effGo.transform.parent = hit.transform;
            effGo.SetActive(true);
        }
        else if (cs == CharacterManager.playerCS || (attackerCs != null && attackerCs == CharacterManager.playerCS))
        {
            effGo = BattleUtil.AddEffectTo("Effect/Prefabs/Hit/Hit1", hit);
            if (effGo != null)
            {
                effGo.transform.parent = hit.transform;
                effGo.SetActive(true);
            }
        }
        if (effGo != null)
        {
            float destoryTime = 0.5f;
            if (attackerCs != null && effect.add_state != null && effect.add_state.Length > 0)
            {
                if ((attackerCs.mCurMobalId == MobaObjectID.HeroBaihu && effect.site == 2) ||
                    (attackerCs.mCurMobalId == MobaObjectID.HeroMori && effect.site == 4))
                {
                    object o = effect.add_state[0];
                    if (o != null && o is System.Array && ((System.Array)o).Length > 1)
                    {
                        destoryTime = float.Parse(((System.Array)o).GetValue(1).ToString());
                    }
                    cs.OnDead += a => Destroy(effGo);
                }
            }
            Destroy(effGo, destoryTime);
        }
    }

    public void PlayDieEffect()
    {
        if (cs.mCurMobalId == MobaObjectID.HeroShuiren)
        {
            GameObject effect_go = BattleUtil.AddEffectTo(GetEffectResourceRoot() + "die", transform);
            if (effect_go != null)
            {
                effect_go.transform.parent = nip.transform;
                effect_go.SetActive(true);
                Destroy(effect_go, 2f);
            }
        }
    }

    public void HitAction(SkillNode effect, GameObject target)
    {
        if (effect != null && effect.skill_type == SkillCastType.MeleeEffect)
        {
            CharacterData characterData = null;
            GameLibrary.Instance().SetSkillDamageCharaData(ref characterData, effect, cs);
            if (et != null && et.mCurSkillNode.skill_type == SkillCastType.MeleeEffect && et.mHitTargetCs != null && !et.mHitTargetCs.isDie)
            {
                target = et.mHitTargetCs.gameObject;
            }
            List<GameObject> monsters = GetTargetByCondition(target, effect);
            for (int i = 0; i < monsters.Count; i++)
            {
                monsters[i].GetComponent<CharacterState>().HitBy(effect, cs, characterData);
            }
        }
    }

    public List<GameObject> GetTargetByCondition(GameObject target, SkillNode skillNode)
    {
        List<GameObject> mTempMonsters = new List<GameObject>();
        Collider[] colliders = Physics.OverlapSphere(cs.transform.position, GameLibrary.Instance().GetSkillDistBySkillAndTarget(cs, skillNode), GameLibrary.GetAllLayer());
        for (int i = 0; i < colliders.Length; i++)
        {
            CharacterState mCurTargetCs = colliders[i].GetComponent<CharacterState>();
            if (mCurTargetCs != null)
            {
                Vector3 direction = mCurTargetCs.transform.position - cs.transform.position;
                if (GameLibrary.Instance().CheckHitCondition(skillNode, cs, mCurTargetCs)
                && (skillNode.isSingle ? (target == null ? false : target.transform == mCurTargetCs.transform) : true)
                && (skillNode.angle == 0 ? true : direction == Vector3.zero ? true : Vector3.Angle(cs.transform.forward, direction) <= skillNode.angle / 2))
                {
                    mTempMonsters.Add(colliders[i].gameObject);
                }
            }
        }
        return mTempMonsters;
    }

    public static string GetEffectRoot(CharacterState mCurCs)
    {
        return GameLibrary.Instance().CheckNotHeroBoss(mCurCs) || mCurCs.state == Modestatus.Monster || mCurCs.state == Modestatus.Tower ? GameLibrary.Effect_Monster : GameLibrary.Effect_Hero;
    }

    public string GetEffectResourceRoot()
    {
        if (mEffectRoot == null)
        {
            mEffectRoot = GetEffectRoot(cs) + fold + "/";
        }
        return mEffectRoot;
    }

    void ChangeSpeed(Transform tr, float mSpeed)
    {
        Animator[] mAnimator = tr.GetComponentsInChildren<Animator>();
        for (int i = 0; i < mAnimator.Length; i++)
        {
            mAnimator[i].speed = mSpeed;
        }
        ParticleSystem[] mParticleSystem = tr.GetComponentsInChildren<ParticleSystem>();
        for (int i = 0; i < mParticleSystem.Length; i++)
        {
            mParticleSystem[i].playbackSpeed = mSpeed;
        }
    }

    bool GetSkillIsAttack(string id)
    {
        return mNormalSkillkeys.Contains(id);
    }
}
