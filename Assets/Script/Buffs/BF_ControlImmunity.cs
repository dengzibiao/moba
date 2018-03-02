public class BF_ControlImmunity : SkillBuff
{
    public BF_ControlImmunity(float baseVal, object p) : base(baseVal, p)
    {
    }

    public override void Excute(CharacterState cs, float mCurValue)
    {
        base.Excute(cs, mCurValue);
        cs.RecieveControl = false;
        //添加血条控制
        if (cs.hpBar != null)
        {
            cs.hpBar.ShowAndHideImmune(true);
        }
    }

    public override void Reverse(CharacterState cs, float mCurValue)
    {
        base.Reverse(cs, mCurValue);
        if (mCurValue == 0)
        {
            cs.RecieveControl = true;
            //添加血条控制
            if (cs.hpBar != null)
            {
                cs.hpBar.ShowAndHideImmune(false);
            }
        }
    }
}
