using UnityEngine;
public class BF_Cure : SkillBuff
{
    public BF_Cure(float baseVal, object p) : base(baseVal, p)
    {
    }

    public override void Excute(CharacterState cs, float mCurValue)
    {
        base.Excute(cs, mCurValue);
        cs.Hp(-(mCurValue < 1 ? 1 : Mathf.FloorToInt(mCurValue)), HUDType.Cure, target.state == Modestatus.Player);
    }

    public override void Reverse(CharacterState cs, float mCurValue)
    {
        base.Reverse(cs, mCurValue);
    }
}