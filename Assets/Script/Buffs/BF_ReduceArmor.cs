
public class BF_ReduceArmor : SkillBuff
{
	public BF_ReduceArmor ( float baseVal, object p) : base(baseVal, p)
    {

    }

    public override void Excute(CharacterState cs, float mCurValue)
    {
        base.Excute(cs, mCurValue);
        Formula.AddAttrWith(ref cs.CharData.buffAttrs, AttrType.armor, -1f * mCurValue);
    }

    public override void Reverse(CharacterState cs, float mCurValue)
    {
        base.Reverse(cs, mCurValue);
        Formula.AddAttrWith(ref cs.CharData.buffAttrs, AttrType.armor, mCurValue);
    }
}
