using System;
using System.Collections.Generic;
using UnityEngine;

public class CommonIdle : BaseStatus
{
    public override void UpdateLogic ()
    {
                
    }

    public override bool OnEnter (STATUS last)
    {
        //mController.Move (Dir.NONE);
        mController.PlayAnimation (ANIM_INDEX.IDLE);
        return true;
    }

    public override STATUS GetNextStatus ()
    {
        STATUS next = base.GetNextStatus();
        if (next != STATUS.NONE)
            return next;
        if (mTouchHandler.IsTouched(TOUCH_KEY.Run)
            && mStateHelper.ContainsStatus(STATUS.RUN))
        {
            return STATUS.RUN;
        }
        else if (IsPlayerAttack() && mStateHelper.ContainsStatus(STATUS.ATTACK01))
            return STATUS.ATTACK01;
        return STATUS.NONE;
    }
}