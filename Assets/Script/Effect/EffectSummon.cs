using UnityEngine;

public class EffectSummon : EffectTrackBase
{

    void Awake()
    {
        FindPosTrans(transform);
    }

    public override void Init(SkillNode skillNode, GameObject targetTrans, Transform thisTrans, VoidResult action = null)
    {
        base.Init(skillNode, targetTrans, thisTrans, action);
        mCurSkillNode.AddSpecialBuffs(attackerCs);
        Destroy(gameObject, mCurSkillNode.efficiency_time);
    }
}
