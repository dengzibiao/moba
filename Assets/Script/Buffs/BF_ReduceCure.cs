public class BF_ReduceCure : SkillBuff
{
    public BF_ReduceCure(float baseVal, object p) : base(baseVal, p)
    {
    }

    public override void Excute(CharacterState cs, float mCurValue)
    {
        base.Excute(cs, mCurValue);
        cs.CurePercent -= mCurValue / 100;
    }

    public override void Reverse(CharacterState cs, float mCurValue)
    {
        base.Reverse(cs, mCurValue);
        cs.CurePercent += mCurValue / 100;
    }
}