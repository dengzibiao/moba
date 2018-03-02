public class BF_PhysicalImmunity : SkillBuff
{
    public BF_PhysicalImmunity(float baseVal, object p) : base(baseVal, p)
    {
    }

    public override void Excute(CharacterState cs, float mCurValue)
    {
        base.Excute(cs, mCurValue);
        cs.RecievePhysicDamage = false;
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