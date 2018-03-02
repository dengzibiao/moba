public class BF_AddSkillDamage : SkillBuff
{
    public BF_AddSkillDamage(float baseVal, object p) : base(baseVal, p)
    {
    }

    public override void Excute(CharacterState cs, float mCurValue)
    {
        base.Excute(cs, mCurValue);
        cs.addSkillDamage += mCurValue;
    }

    public override void Reverse(CharacterState cs, float mCurValue)
    {
        base.Reverse(cs, mCurValue);
        cs.addSkillDamage -= mCurValue;
    }
}
