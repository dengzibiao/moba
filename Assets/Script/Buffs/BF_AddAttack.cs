using UnityEngine;
using System.Collections.Generic;

public class BF_AddAttack : SkillBuff
{
    public BF_AddAttack(float baseVal, object p) : base(baseVal, p)
    {
    }

    public override void Init()
    {
        base.Init();
        if (attacker.mCurMobalId == MobaObjectID.HeroJiansheng && target.mCurMobalId == MobaObjectID.HeroJiansheng)
        {
            mCurRolePart = new List<RolePart>() { RolePart.RightHand };
        }
        if (attacker.mCurMobalId == MobaObjectID.HeroJumo && target.mCurMobalId == MobaObjectID.HeroJumo)
        {
            mCurRolePart = new List<RolePart>() { RolePart.LeftHand, RolePart.RightHand };
        }
        else if (attacker.mCurMobalId == MobaObjectID.HeroXiaohei && target.mCurMobalId == MobaObjectID.HeroXiaohei)
        {
            mCurRolePart = new List<RolePart>() { RolePart.RightHand };
        }
    }

    public override void Excute(CharacterState cs, float mCurValue)
    {
        base.Excute(cs, mCurValue);
        Formula.AddAttrWith(ref cs.CharData.buffAttrs, AttrType.attack, mCurValue);
        if (mCurRolePart.Count > 0)
        {
            cs.SetPartEffectActive(cs, mCurRolePart, true);
        }
    }

    public override void Reverse(CharacterState cs, float mCurValue)
    {
        base.Reverse(cs, mCurValue);
        Formula.AddAttrWith(ref cs.CharData.buffAttrs, AttrType.attack, -1f * mCurValue);
        if (mCurRolePart.Count > 0)
        {
            List<SkillBuff> mCurSkillBuff = SkillBuffManager.GetInst().GetSkillBuffListByCs(cs).FindAll(a => a != this && a.node.type == node.type && a.node.buffActionType == node.buffActionType
                    && attacker == target);
            if (mCurSkillBuff.Count == 0)
            {
                cs.SetPartEffectActive(cs, mCurRolePart, false);
                if (cs.mCurMobalId == MobaObjectID.HeroJiansheng)
                {
                    if (cs.pm.mCurAttackTime == cs.pm.mAmountAttackTime)
                    {
                        cs.pm.mCurAttackTime = 0;
                    }
                }
            }
        }
    }
}
