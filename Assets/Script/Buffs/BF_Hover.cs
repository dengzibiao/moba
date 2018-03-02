using UnityEngine;
public class BF_Hover : SkillBuff
{
    public BF_Hover(float baseVal, object p) : base(baseVal, p)
    {
    }

    public override void Excute(CharacterState cs, float mCurValue)
    {
        base.Excute(cs, mCurValue);
        cs.pm.HitFly();
        SetTargetEnable(false);
    }

    public override void Reverse(CharacterState cs, float mCurValue)
    {
        base.Reverse(cs, mCurValue);
        SetTargetEnable(true);
    }

    public virtual void SetTargetEnable(bool b)
    {
        if (target != null && target.pm != null)
        {
            target.pm.SetControlSwitch(!b);
        }
    }
}
