using System;

public class BF_Virtualize : SkillBuff
{
	public BF_Virtualize ( float baseVal, object p) : base(baseVal, p)
    {

    }

    public override void Excute(CharacterState cs, float mCurValue)
    {
        base.Excute(cs, mCurValue);
        cs.RecievePhysicDamage = false;
        cs.moveSpeed -= cs.moveInitSpeed * mCurValue / 100;
    }

    public override void Reverse(CharacterState cs, float mCurValue)
    {
        base.Reverse(cs, mCurValue);
        if (mCurValue == 0)
        {
            cs.moveSpeed += cs.moveInitSpeed * mCurValue / 100;
            cs.RecievePhysicDamage = false;
        }
    }
}