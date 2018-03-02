using System;
using UnityEngine;

public class BF_Bleeding : SkillBuff
{
    public CDTimer.CD bleedCd;

    public BF_Bleeding ( float baseVal, object p) : base(baseVal, p) { }

    public override void Excute(CharacterState cs, float mCurValue)
    {
        base.Excute(cs, mCurValue);
        bleedCd = CDTimer.GetInstance().AddCD(1f, DoEffect, Mathf.FloorToInt(last));
    }

    public virtual void DoEffect (int c, long cid)
    {
        if(target != null && !target.isDie)
        {
            int hpResult = GameLibrary.Instance().CalcSkillDamage(baseValue, node.damageType, lvl, target, attacker);
            target.Hp(hpResult, target.state == Modestatus.Player ? HUDType.DamagePlayer : HUDType.DamageEnemy, showHud);
        }
    }
}
