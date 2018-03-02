using System.Collections.Generic;
public class BF_AddSuckBlood : SkillBuff
{
    public BF_AddSuckBlood(float baseVal, object p) : base(baseVal, p)
    {
    }

    public override void Init()
    {
        base.Init();
        if (attacker.mCurMobalId == MobaObjectID.HeroShenling && target.mCurMobalId == MobaObjectID.HeroShenling)
        {
            mCurRolePart = new List<RolePart>() { RolePart.LeftHand, RolePart.RightHand };
        }
    }
    public override void Excute(CharacterState cs, float mCurValue)
    {
        base.Excute(cs, mCurValue);
        Formula.AddAttrWith(ref cs.CharData.buffAttrs, AttrType.suck_blood, mCurValue);
        if (mCurRolePart.Count > 0)
        {
            cs.SetPartEffectActive(cs, mCurRolePart, true);
        }
    }

    public override void Reverse(CharacterState cs, float mCurValue)
    {
        base.Reverse(cs, mCurValue);
        Formula.AddAttrWith(ref cs.CharData.buffAttrs, AttrType.suck_blood, -1f * mCurValue);
        if (mCurRolePart.Count > 0)
        {
            List<SkillBuff> mCurSkillBuff = SkillBuffManager.GetInst().GetSkillBuffListByCs(cs).FindAll(a => a != this && a.node.type == node.type && a.node.buffActionType == node.buffActionType
                    && attacker.mCurMobalId == MobaObjectID.HeroShenling && target.mCurMobalId == MobaObjectID.HeroShenling);
            if (mCurSkillBuff.Count == 0)
            {
                cs.SetPartEffectActive(cs, mCurRolePart, false);
            }
        }
    }
}
