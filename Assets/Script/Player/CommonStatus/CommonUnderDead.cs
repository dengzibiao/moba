using System;
using UnityEngine;

public class CommonUnderDead : BaseStatus
{
    private STATUS mBackStatus;
    private ANIM_INDEX mAnimIndex;
    private bool mIsPlayedAnim;

    public CommonUnderDead (STATUS backStatus, ANIM_INDEX animIndex)
    {
        this.mBackStatus = backStatus;
        this.mAnimIndex = animIndex;
        mIsPlayedAnim = false;
    }

    public override void UpdateLogic ()
    {
    }

    public override STATUS GetNextStatus ()
    {
        return STATUS.NONE;
    }

    public override bool OnEnter (STATUS last)
    {
        //mController.SetAnimatorSpeed(1.0f,ActionController.ChangeSpeedLevelDead);
        mController.PlayAnimation (mAnimIndex);
        return true;
    }
}

