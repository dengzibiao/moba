using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum AoeType
{
    CircleAoe,
    RectAoe
}

public class EffectAOE : EffectAOECenter
{
    public override void Play()
    {
        SetPosition();
        if (mCurSkillNode.isSingle)
        {
            if (mHitTargetCs != null)
            {
                mCurMonsters.Add(mHitTargetCs.gameObject);
                Hit(mCurMonsters);
            }
        }
        else
        {
            base.Play();
        }
    }
}
