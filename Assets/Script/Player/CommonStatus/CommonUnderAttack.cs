using System;
using UnityEngine;

public class CommonUnderAttack : BaseStatus
{
    protected ANIM_INDEX mAnimIndex;
    protected float mDuration;
    protected float mKeepDuration = 0f;
    protected STATUS mNext = STATUS.NONE;
    //private XHTimer mStiffTimer = XHTimer.Create();

    public CommonUnderAttack (ANIM_INDEX animIndex, float duration = 0f)
    {
        this.mAnimIndex = animIndex;
        this.mDuration = duration;
    }

    public CommonUnderAttack (ANIM_INDEX animIndex, float duration, float KeepDuration) : this(animIndex,duration)
    {
        this.mKeepDuration = KeepDuration;
    }

    public override void UpdateLogic ()
    {
        mNext = GetUpdatedStatus ();
    }

    public override STATUS GetNextStatus ()
    {
        return mNext;
    }

    public override bool OnEnter (STATUS last)
    {
        if ((!RoleEnum.IsPlayer(mController.GetRoleType()) && last != STATUS.IDLE && last != STATUS.PREPARE) || RoleEnum.IsPlayer(mController.GetRoleType()))
            return false;
        mController.PlayAnimation (mAnimIndex, 0.0f, 0.0f);
        mNext = STATUS.NONE;
        return true;
    }

    public override void OnLeave (STATUS next)
    {
        if (next != STATUS.HIT)
            mController.UnderAttack = false;
        base.OnLeave(next);
    }

    protected virtual STATUS GetUpdatedStatus ()
    {
        if (mController.ShouldPlayNextAnimation (mAnimIndex) && !mController.UnderAttack)
            return STATUS.IDLE;
        return STATUS.NONE;
    }
}
