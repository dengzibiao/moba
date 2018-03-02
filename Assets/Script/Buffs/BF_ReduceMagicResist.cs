public class BF_ReduceMagicResist : SkillBuff
{
    public BF_ReduceMagicResist(float baseVal, object p) : base(baseVal, p)
    {
    }


    public override void Excute(CharacterState cs, float mCurValue)
    {
        base.Excute(cs, mCurValue);
        Formula.AddAttrWith(ref cs.CharData.buffAttrs, AttrType.magic_resist, -1f * mCurValue);
    }

    public override void Reverse(CharacterState cs, float mCurValue)
    {
        base.Reverse(cs, mCurValue);
        Formula.AddAttrWith(ref cs.CharData.buffAttrs, AttrType.magic_resist, mCurValue);
    }
}