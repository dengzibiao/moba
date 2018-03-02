using UnityEngine;
using System.Collections;

//近战攻击
public class EffectMelee : EffectTrackBase
{
    public override void Init(SkillNode skillNode, GameObject targetTrans, Transform thisTrans, VoidResult action)
    {
        base.Init(skillNode, targetTrans, thisTrans, action);
        Destroy(gameObject, destoryTime);
    }

    void Update()
    {
        if (attackerCs != null && (attackerCs.isDie || !GameLibrary.Instance().CanControlSwitch(attackerCs.pm)))
        {
            Destroy(gameObject);
        }
    }
}
