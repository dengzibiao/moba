using UnityEngine;

public class BF_LoseBlood : SkillBuff
{
    public BF_LoseBlood(float baseVal, object p) : base(baseVal, p)
    {
    }

    public override void Excute(CharacterState cs, float mCurValue)
    {
        base.Excute(cs, mCurValue);
        int hpResult = GameLibrary.Instance().CalcSkillDamage(mCurValue, node.damageType, lvl, target, attacker);
        HUDType mCurType = attacker.state == Modestatus.Player ? HUDType.DamagePlayer : HUDType.DamageEnemy;
        cs.Hp(hpResult < 1 ? 1 : hpResult, mCurType, true, attacker);
    }

    public override void Reverse(CharacterState cs, float mCurValue)
    {
        base.Reverse(cs, mCurValue);
    }
}
