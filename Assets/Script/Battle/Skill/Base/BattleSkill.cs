using UnityEngine;
using System.Collections.Generic;
using Tianyu;

public class BattleSkill
{
    public SkillNode Node;
    public BattleAgent Caster;

    public delegate void CastDele ();
    public CastDele OnSpellAnimOver;

    BattleEffect[] BattleEffects;

    public List<BattleAgent> TargetList = new List<BattleAgent>();

    public BattleSkill ( long skillId ) {

        Node = FSDataNodeTable<SkillNode>.GetSingleton().DataNodeList[skillId];

        //SkillRange = new 
    }

    public BattleSkill ( SkillNode skillNode )
    {
        Node = skillNode;
    }

    public virtual List<BattleAgent> GetEffectTargets ()
    {
        List<BattleAgent> ret = new List<BattleAgent>();
        for(int i = 0; i< TargetList.Count; i++)
        {
            if(Node.battleRange.InRange(TargetList[i].Pos, Caster.Pos))
            {
                ret.Add(TargetList[i]);
            }
        }
        return ret;
    }

    BattleEffect[] GetSkillEffects ()
    {
        BattleEffect[] effs = new BattleEffect[Node.battleEffects.Length];
        SkillEffectNode[] effNodes = SkillNode.GetSkillEffectNodes(Node.battleEffects);
        for(int i = 0; i< effs.Length; i++)
        {
            effs[i] = GetEffectByType(effNodes[i]);
        }
        return effs;
    }

    BattleEffect GetEffectByType ( SkillEffectNode effectNode )
    {
        switch(effectNode.effectType)
        {
            case SkillEffectType.CreateArea:
                return new ChangeValueEffect(effectNode);
            case SkillEffectType.CreateObject:
                return new ChangeValueEffect(effectNode);
            case SkillEffectType.ChangeValue:
            default:
                return new ChangeValueEffect(effectNode);
        }
    }

    public virtual void PreSpell ()
    {
        //OnSpellAnimOver += DoEffects;
        //BattleUtil.DoWait(300, this, "DoEffects", null);
        BattleEffects = GetSkillEffects();
        
        PlaySpellAnimation();
    }

    //System.Timers.Timer skillTimer;
    Dictionary<long, BattleEffect> effectTimeCDs = new Dictionary<long, BattleEffect>();

    public virtual void Spell ( BattleAgent caster )
    {
        Caster = caster;
        if(!CanSpell())
        {
            return;
        }
        PreSpell();
        if(BattleUtil.useUnityUpdate)
        {
            for(int i = 0; i< BattleEffects.Length; i++)
            {
                if(BattleEffects[i].EffectTiming() == 0)
                {
                    DoEffect(BattleEffects[i]);
                }
                else if(BattleEffects[i].EffectTiming() == -1)
                {

                }
                else
                {
                    CDTimer.CD cd = CDTimer.GetInstance().AddCD(0.001f * BattleEffects[i].EffectTiming(), CDTimeDoEffect);
                    effectTimeCDs.Add(cd.Id, BattleEffects[i]);
                }
            }
        }
        else
        {
            //skillTimer = new System.Timers.Timer(BattleUtil.BattleTimeStep);
            //skillTimer.Elapsed += RepDoEffect;
            //skillTimer.AutoReset = true;
            //skillTimer.Enabled = true;
        }
    }

    void CDTimeDoEffect (int count, long id )
    {
        DoEffect(effectTimeCDs[id]);
    }

    void DoEffect (BattleEffect battleEffect )
    {
        List<BattleAgent> battleAgents = GetEffectTargets();
        for(int j = 0; j < battleAgents.Count; j++)
        {
            battleEffect.Cast(battleAgents[j], Caster);
        }
    }

    public virtual void PlaySpellAnimation ()
    {

        //skillNode.preSpellTime;

        //( (BaseBAPlayer)Caster ).GetComponent<PlayerAnimator>().Skill(1);
    }

    public void CancelSpell ()
    {
        if(Node.Cancelable)
        {
            //OnSpellAnimOver -= DoEffects;
        }
    }

    public bool CanSpell ()
    {

        return true;
    }
    
}