using UnityEngine;
using System.Collections;

public class CommonUnDown : BaseStatus
{
    private STATUS mNext = STATUS.NONE;
    public const float DownDuration = 2.0f;
    #region implemented abstract members of BaseStatus

    public override void UpdateLogic ()
    {
        //if (mController.ShouldPlayNextAnimation (ANIM_INDEX.UNDOWN) && mTimer.IsEndOfDuration(DownDuration)) {
        //    mNext = STATUS.UNSTAND;
        //}
    }

    #endregion
    public override bool OnEnter (STATUS last)
    {
        mNext = STATUS.NONE;
        //mTimer.Reset ();
        mController.PlayAnimation (ANIM_INDEX.DOWN);
        return base.OnEnter (last);
    }

    public override STATUS GetNextStatus ()
    {
        return mNext;
    }
}

