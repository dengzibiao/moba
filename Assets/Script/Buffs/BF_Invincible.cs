using System;

public class BF_Invincible : SkillBuff
{
	public BF_Invincible ( float baseVal, object p) : base(baseVal, p)
    {

    }

    public override void Excute(CharacterState cs, float mCurValue)
    {
        base.Excute(cs, mCurValue);
        cs.Invincible = true;
        //添加血条控制
        if (cs.hpBar != null)
        {
            cs.hpBar.ShowAndHideInvincible(true);
        }
    }

    public override void Reverse(CharacterState cs, float mCurValue)
    {
        base.Reverse(cs, mCurValue);
        if (mCurValue == 0)
        {
            cs.Invincible = false;
            //添加血条控制
            if (cs.hpBar != null)
            {
                cs.hpBar.ShowAndHideInvincible(false);
            }
        }
    }
}
