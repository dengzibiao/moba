using System;
using UnityEngine;


public class CommonAttack : BaseStatus
{
//    protected STATUS mBackStatus;
    protected STATUS mNextStatus;
    protected ANIM_INDEX mAnimIndex;
    protected bool mIsChangeNextStatus;
    protected bool mCanUnderAttack = true;
    public CommonAttack (STATUS nextStatus, ANIM_INDEX animIndex,bool CanUnderAttack) : this(nextStatus, animIndex){
        mCanUnderAttack = CanUnderAttack;
    }

    public CommonAttack (STATUS nextStatus, ANIM_INDEX animIndex)
    {
//        this.mBackStatus = backStatus;
        this.mNextStatus = nextStatus;
        this.mAnimIndex = animIndex;
    }

    public override void UpdateLogic ()
    {

    }
    public override STATUS GetNextStatus ()
    {
        STATUS status = base.GetNextStatus ();
        if (status != STATUS.NONE)
            return status;
        if (mTouchHandler.IsTouched(TOUCH_KEY.Attack)) {
            float h = mController.GetCurrentAnimationNormalizedTime();
            if (h > 0.65f && mController.getAnimator().GetCurrentAnimatorStateInfo(0).IsName (AnimatorHelper.GetAnimationName (mAnimIndex)))
                mIsChangeNextStatus = true;
        }
        if (mController.ShouldPlayNextAnimation (mAnimIndex) || mIsChangeNextStatus) {
            if (mNextStatus == STATUS.NONE)
                mNextStatus = STATUS.IDLE;
            //攻击时候是否可以转向
			//if (mTouchHandler.IsTouched (TOUCH_KEY.Left)) {
			//	mController.SetDirection(Dir.LEFT);
			//}else if (mTouchHandler.IsTouched (TOUCH_KEY.Right)) {
			//	mController.SetDirection(Dir.RIGHT);
			//}
            return mIsChangeNextStatus ? mNextStatus : STATUS.IDLE;
//            return mTouchHandler.IsTouched(TOUCH_KEY.Attack) ? mNextStatus : STATUS.IDLE;
        }
        return STATUS.NONE;
    }
    public override bool OnEnter (STATUS last)
    {
        shouldMove = false;
        mController.PlayAnimation (mAnimIndex);
        if (!mCanUnderAttack) {
            RemoveCondition(IsUnderAttack);
        }
        mIsChangeNextStatus = false;
        AddPlayerSkillConditions ();
        return true;
    }
}

