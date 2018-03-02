using UnityEngine;
using System.Collections.Generic;

public class BF_Purification : SkillBuff
{
    public BF_Purification(float baseVal, object p) : base(baseVal, p)
    {
    }

    public override void Excute(CharacterState cs, float mCurValue)
    {
        base.Excute(cs, mCurValue);
        //查询所有的减益效果
        List<SkillBuff> mAllDebuff = SkillBuffManager.GetInst().GetAllDebuffByCs(cs);
        for (int i = mAllDebuff.Count - 1; i >= 0 ; i--)
        {
            SkillBuff mTempBuff = mAllDebuff[i];
            if (mTempBuff.node.type == BuffType.DelayEffective)
            {
                SkillBuffManager.GetInst().RemoveBuffFrom(mTempBuff, mTempBuff.target);
                mTempBuff.RemoveBuffEffect();
            }
            else
            {
                SkillBuffManager.GetInst().RemoveCalculateCurTargetProp(mTempBuff.target, mTempBuff);
            }
            SkillBuffManager.GetInst().RemoveContinousSkillBuff(mTempBuff);
        }
    }
}