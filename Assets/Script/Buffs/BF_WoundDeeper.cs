public class BF_WoundDeeper : SkillBuff
{
    public BF_WoundDeeper(float baseVal, object p) : base(baseVal, p)
    {
    }

    public override void Excute(CharacterState cs, float mCurValue)
    {
        base.Excute(cs, mCurValue);
        cs.DamagePercent += mCurValue / 100;
    }

    public override void Reverse(CharacterState cs, float mCurValue)
    {
        base.Reverse(cs, mCurValue);
        cs.DamagePercent -= mCurValue / 100;
    }
}