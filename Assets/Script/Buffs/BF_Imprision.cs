using System;

public class BF_Imprision : SkillBuff
{
	public BF_Imprision ( float baseVal, object p) : base(baseVal, p)
    {

    }

    public override void Excute(CharacterState cs, float mCurValue)
    {
        base.Excute(cs, mCurValue);
        cs.pm.canMove = false;
        cs.pm.canMoveSwitch++;
    }

    public override void Reverse(CharacterState cs, float mCurValue)
    {
        base.Reverse(cs, mCurValue);
        cs.pm.canMove = mCurValue == 0;
        cs.pm.canMoveSwitch--;
    }
}
