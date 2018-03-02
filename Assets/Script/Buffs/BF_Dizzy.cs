public class BF_Dizzy : SkillBuff
{
	public BF_Dizzy ( float baseVal, object p) : base(baseVal, p)
    {

    }

    public override void Excute( CharacterState cs, float mCurValue)
    {
        base.Excute(cs, mCurValue);
        cs.pm.CanNotControl();
    }

    public override void Reverse(CharacterState cs, float mCurValue)
    {
        base.Reverse(cs, mCurValue);
        cs.pm.CanControl();
    }
}