using UnityEngine;


public class CommonSkillBase : BaseStatus
{
    private STATUS mNextStatus;
    private ANIM_INDEX[] mAnimQueue;
    private float[] mAnimSpeed;
    private int animIndex;
    private float timer = 0.0f;
    private CharacterState cs;
    private int mCurTime;

    public CommonSkillBase (ANIM_INDEX[] animQueue, float[] animSpeed = null)
    {
        this.mAnimQueue = animQueue;
        this.mAnimSpeed = animSpeed;
    }

    public override void UpdateLogic ()
    {
        if (mController.ShouldPlayNextAnimation (mAnimQueue[animIndex])) {
            if (animIndex + 1 <= mAnimQueue.Length - 1) {
                animIndex ++;
                mController.PlayAnimation (mAnimQueue[animIndex]);
                if (mAnimSpeed != null)
                    mController.getAnimator().speed = mAnimSpeed[animIndex];
                mNextStatus = STATUS.NONE;
            } else {
                mNextStatus = OnAnimationEnd ();
            }
        }
        if (cs != null)
        {
            switch (mAnimQueue[animIndex])
            {
                case ANIM_INDEX.SKILL04:
                    if (cs.mCurMobalId == MobaObjectID.HeroJiansheng || cs.mCurMobalId == MobaObjectID.HeroHuanci)
                    {
                        mController.Move(TouchHandler.GetInstance().mOffset);
                    }
                    break;
                default:
                    break;
            }
        }
    }

    public override STATUS GetNextStatus ()
    {
        STATUS buffEffect = mController.GetEffectOfBuff();
        if (buffEffect == STATUS.CANNOTSKILL)
        {
            if (cs != null && !cs.Invincible && !cs.RecieveMagicDamage)
            {
                return STATUS.IDLE;
            }
        }
        STATUS next = base.GetNextStatus ();
        if (next != STATUS.NONE)
            return next;
        return mNextStatus;
    }

    public virtual STATUS OnAnimationEnd () {
        switch (mAnimQueue[animIndex])
        {
            case ANIM_INDEX.SKILL01:
                if (cs.mCurMobalId == MobaObjectID.HeroJiansheng && mController.FoundTarget() != null && mCurTime < 5)
                {
                    mCurTime++;
                    return mStateHelper.CurStatus;
                }
                mCurTime = 0;
                break;
        }
       return STATUS.IDLE;
    }
    
    public override bool OnEnter (STATUS last)
    {
        cs = (mController as CharacterState);
        if (cs != null && cs.mCurMobalId == MobaObjectID.HeroJiansheng && mAnimQueue[animIndex] == ANIM_INDEX.SKILL01)
        {
            cs.Invincible = true;
        }
        shouldMove = false;
        isUseSkill = false;
        animIndex = 0;
        timer = 0.0f;
        mController.PlayAnimation (mAnimQueue[animIndex]);
        mNextStatus = STATUS.NONE;
        if (mAnimSpeed != null)
            mController.getAnimator().speed = mAnimSpeed[animIndex];
        fightTouch.startCd(GetAnimIndex(mAnimQueue[animIndex]));
        return true;
    }


    public override void OnLeave (STATUS next)
    {
        if (mAnimSpeed != null)
            mController.getAnimator ().speed = 1;
        if (cs != null && cs.mCurMobalId == MobaObjectID.HeroJiansheng && next != STATUS.SKILL01)
        {
            cs.Invincible = false;
        }
        base.OnLeave(next);
    }
}