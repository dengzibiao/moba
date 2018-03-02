public class BF_Disarm : SkillBuff
{
    public BF_Disarm(float baseVal, object p) : base(baseVal, p)
    {
    }

    public override void Excute(CharacterState cs, float mCurValue)
    {
        base.Excute(cs, mCurValue);
        cs.pm.canAttack = false;
        cs.pm.canAttackSwitch++;
    }

    public override void Reverse(CharacterState cs, float mCurValue)
    {
        base.Reverse(cs, mCurValue);
        cs.pm.canAttack = mCurValue == 0;
        cs.pm.canAttackSwitch--;
    }
}
