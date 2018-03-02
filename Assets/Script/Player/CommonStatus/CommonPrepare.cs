using System;
using System.Collections.Generic;
using UnityEngine;

public class CommonPrepare : BaseStatus
{
    public override void UpdateLogic ()
    {
                
    }

    public override bool OnEnter (STATUS last)
    {
        //mController.Move (Dir.NONE);
        mController.PlayAnimation (ANIM_INDEX.PREPARE);
        AddPlayerSkillConditions ();
        AddCondition (IsPlayerAttack, STATUS.ATTACK01);
        return true;
    }

    public override STATUS GetNextStatus()
    {
        STATUS next = base.GetNextStatus();
        if (next != STATUS.NONE)
            return next;
        if (mTouchHandler.IsTouched(TOUCH_KEY.Run)
            && mStateHelper.ContainsStatus(STATUS.RUN))
        {
            return STATUS.RUN;
        }
        return STATUS.NONE;
    }
}