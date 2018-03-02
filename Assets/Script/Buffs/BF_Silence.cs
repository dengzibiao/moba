using System;

public class BF_Silence : SkillBuff
{
    private FightTouch mFightTouch;
    private SkillBuff mExistBuff;
	public BF_Silence ( float baseVal, object p) : base(baseVal, p)
    {
        mFightTouch = FightTouch._instance;
    }

    public override void Excute(CharacterState cs, float mCurValue)
    {
        base.Excute(cs, mCurValue);
        cs.pm.canSkill = false;
        cs.pm.canSkillSwitch++;
        SetSkillBtnCD(true);
    }

    public override void Reverse(CharacterState cs, float mCurValue)
    {
        base.Reverse(cs, mCurValue);
        cs.pm.canSkill = mCurValue == 0;
        cs.pm.canSkillSwitch--;
        SetSkillBtnCD(false);
    }

    private void SetSkillBtnCD(bool b)
    {
        if (!b)
        {
            mExistBuff = SkillBuffManager.GetInst().GetSkillBuffListByCs(target).Find(a => a != this && (SkillBuffType)a.id == SkillBuffType.Silence);
        }
        if (target == CharacterManager.playerCS)
        {
            mFightTouch.ChangeAllCDTo(b ? b : !(mExistBuff == null && GameLibrary.Instance().CanControlSwitch(target.pm)));
        }
    }
}
