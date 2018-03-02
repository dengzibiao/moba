using System;
using UnityEngine;

public class BF_Slow : SkillBuff
{
	public BF_Slow(float baseVal, object p) : base(baseVal, p)
    {

    }

    public override void Excute(CharacterState cs, float mCurValue)
    {
        base.Excute(cs, mCurValue);
        cs.moveSpeed -= cs.moveInitSpeed * mCurValue / 100;
    }

    public override void Reverse(CharacterState cs, float mCurValue)
    {
        base.Reverse(cs, mCurValue);
        cs.moveSpeed += cs.moveInitSpeed * mCurValue / 100;
    }
}