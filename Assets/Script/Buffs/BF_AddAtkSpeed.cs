public class BF_AddAtkSpeed : SkillBuff
{
    public BF_AddAtkSpeed(float baseVal, object p) : base(baseVal, p)
    {
    }
    public override void Excute(CharacterState cs, float mCurValue)
    {
        base.Excute(cs, mCurValue);
        cs.attackSpeed += cs.attackInitSpeed * mCurValue / 100;
    }

    public override void Reverse(CharacterState cs, float mCurValue)
    {
        base.Reverse(cs, mCurValue);
        cs.attackSpeed -= cs.attackInitSpeed * mCurValue / 100;
    }
}