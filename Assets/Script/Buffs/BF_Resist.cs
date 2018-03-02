using System;

public class BF_Resist : SkillBuff
{
	public BF_Resist ( float baseVal, object p) : base(baseVal, p)
    {

    }

    public override void Excute(CharacterState cs, float mCurValue)
    {
        base.Excute(cs, mCurValue);
        cs.RecieveMagicDamage = false;
        cs.RecievePhysicDamage = false;
        cs.RecieveFixDamage = false;
    }

    public override void Reverse(CharacterState cs, float mCurValue)
    {
        base.Reverse(cs, mCurValue);
        if (mCurValue == 0)
        {
            cs.RecieveMagicDamage = true;
            cs.RecievePhysicDamage = true;
            cs.RecieveFixDamage = true;
        }
    }
}