using UnityEngine;

public class BF_Fast : SkillBuff
{
	public BF_Fast ( float baseVal, object p) : base(baseVal, p)
    {

    }

    public override void Excute(CharacterState cs, float mCurValue)
    {
        base.Excute(cs, mCurValue);
        cs.moveSpeed += cs.moveInitSpeed * mCurValue / 100;
    }

    public override void Reverse(CharacterState cs, float mCurValue)
    {
        base.Reverse(cs, mCurValue);
        cs.moveSpeed -= cs.moveInitSpeed * mCurValue / 100;
    }
}