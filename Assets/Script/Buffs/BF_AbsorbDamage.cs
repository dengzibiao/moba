using UnityEngine;

public class BF_AbsorbDamage : SkillBuff
{
	public BF_AbsorbDamage ( float baseVal, object p) : base(baseVal, p)
    {

    }

    public override void Excute(CharacterState cs, float mCurValue)
    {
        base.Excute(cs, mCurValue);
        cs.MagicShields = baseValue;
    }

    public override void Reverse(CharacterState cs, float mCurValue)
    {
        base.Reverse(cs, mCurValue);
        if (mCurValue == 0)
        {
            if (cs.OnShieldOff != null)
            {
                cs.OnShieldOff(cs);
            }
            cs.MagicShields = 0;
        }
    }
}