public class Bf_MagicImmunity : SkillBuff
{
    public Bf_MagicImmunity(float baseVal, object p) : base(baseVal, p)
    {
    }

    public override void Excute(CharacterState cs, float mCurValue)
    {
        base.Excute(cs, mCurValue);
        cs.RecieveMagicDamage = false;
    }

    public override void Reverse(CharacterState cs, float mCurValue)
    {
        base.Reverse(cs, mCurValue);
        if (mCurValue == 0)
        {
            cs.RecieveMagicDamage = true;
        }
    }
}