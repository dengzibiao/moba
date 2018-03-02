using UnityEngine;
using System.Collections.Generic;
public class BF_AddonAttack : SkillBuff
{
    public BF_AddonAttack(float baseVal, object p) : base(baseVal, p)
    {
    }

    public override void Init()
    {
        base.Init();
        if (attacker.mCurMobalId == MobaObjectID.HeroChenmo && target.mCurMobalId == MobaObjectID.HeroChenmo)
        {
            mCurRolePart = new List<RolePart>() { RolePart.LeftHand, RolePart.RightHand };
        }
    }

    public override void Excute(CharacterState cs, float mCurValue)
    {
        base.Excute(cs, mCurValue);
        Formula.AddAttrWith(ref cs.CharData.buffAttrs, AttrType.attack, mCurValue);
        if (cs.mCurMobalId == MobaObjectID.HeroChenmo)
        {
            cs.SetPartEffectActive(cs, mCurRolePart, true);
        }
    }

    public override void Reverse(CharacterState cs, float mCurValue)
    {
        base.Reverse(cs, mCurValue);
        Formula.AddAttrWith(ref cs.CharData.buffAttrs, AttrType.attack, -1f * mCurValue);
        List<SkillBuff> mCurSkillBuff = SkillBuffManager.GetInst().GetSkillBuffListByCs(cs).FindAll(a => a != this && a.node.type == node.type && a.node.buffActionType == node.buffActionType);
        if (mCurSkillBuff.Count == 0)
        {
            if (cs.mCurMobalId == MobaObjectID.HeroChenmo)
            {
                cs.SetPartEffectActive(cs, mCurRolePart, false);
            }
        }
    }
}