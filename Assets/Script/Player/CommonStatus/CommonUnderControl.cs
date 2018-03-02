using System;
using UnityEngine;

public class CommonUnderControl : BaseStatus
{
    protected ANIM_INDEX mAnimIndex;
    protected float mDuration;
    protected float mKeepDuration = 0f;
    protected STATUS mNext = STATUS.NONE;
    //private XHTimer mStiffTimer = XHTimer.Create();

    public CommonUnderControl (ANIM_INDEX animIndex, float duration = 0f)
    {
        this.mAnimIndex = animIndex;
        this.mDuration = duration;
    }

    public CommonUnderControl (ANIM_INDEX animIndex, float duration, float KeepDuration) : this(animIndex,duration)
    {
        this.mKeepDuration = KeepDuration;
    }

    public override void UpdateLogic ()
    {
        //mNext = GetUpdatedStatus ();
    }

    public override STATUS GetNextStatus ()
    {
        mNext = base.GetNextStatus();
        if (mNext != STATUS.CANNOTCONTROL)
            return STATUS.IDLE;
        return STATUS.NONE;
    }

    public override bool OnEnter (STATUS last)
    {
        mController.PlayAnimation (mAnimIndex, 0.0f, 0.0f);
        mNext = STATUS.NONE;
        return true;
    }

    public override void OnLeave (STATUS next)
    {
        if (next != STATUS.CANNOTCONTROL)
            mController.IsCanNotControl = false;
        base.OnLeave(next);
    }

    protected virtual STATUS GetUpdatedStatus ()
    {
        if (!mController.IsCanNotControl)
            return STATUS.IDLE;
        return STATUS.NONE;
    }
}
