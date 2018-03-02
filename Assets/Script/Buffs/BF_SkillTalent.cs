using System.Collections.Generic;

public class BF_SkillTalent : SkillBuff
{
    public BF_SkillTalent(float baseVal, object p) : base(baseVal, p)
    {
    }

    public override void Init()
    {
        base.Init();
        if (attacker.mCurMobalId == MobaObjectID.HeroLuosa && target.mCurMobalId == MobaObjectID.HeroLuosa)
        {
            mCurRolePart = new List<RolePart>() { RolePart.RightHand };
        }
    }

    public override void Excute(CharacterState cs, float mCurValue)
    {
        base.Excute(cs, mCurValue);
        if (mCurRolePart.Count > 0)
        {
            cs.SetPartEffectActive(cs, mCurRolePart, true);
        }
    }

    public override void Reverse(CharacterState cs, float mCurValue)
    {
        base.Reverse(cs, mCurValue);
        if (mCurRolePart.Count > 0)
        {
            List<SkillBuff> mCurSkillBuff = SkillBuffManager.GetInst().GetSkillBuffListByCs(cs).FindAll(a => a != this && a.node.type == node.type && a.node.buffActionType == node.buffActionType
                    && attacker.mCurMobalId == MobaObjectID.HeroLuosa && target.mCurMobalId == MobaObjectID.HeroLuosa);
            if (mCurSkillBuff.Count == 0)
            {
                cs.SetPartEffectActive(cs, mCurRolePart, false);
            }
        }
    }
}
