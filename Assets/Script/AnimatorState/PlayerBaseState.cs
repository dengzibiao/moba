using UnityEngine;
using System.Collections;

public class PlayerBaseState : StateMachineBehaviour
{
    protected CharacterState cs;
    protected PlayerMotion pm;
    protected TouchHandler mTouchHandler;
    protected FightTouch mFightTouch;
    protected int mSkillTouchIndex;
    protected bool mIsStateStart = false;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        SetCsAndPm(animator);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
        SetCsAndPm(animator);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
        SetCsAndPm(animator);
    }

    private void SetCsAndPm(Animator animator)
    {
        if (cs == null)
        {
            cs = animator.GetComponent<CharacterState>();
        }
        if (pm == null)
        {
            pm = animator.GetComponent<PlayerMotion>();
        }
        mTouchHandler = TouchHandler.GetInstance();
        mFightTouch = FightTouch._instance;
    }

    protected bool CheckAnimatorIsAttack(AnimatorStateInfo stateInfo)
    {
        return stateInfo.IsName(AnimatorHelper.GetAnimationName(ANIM_INDEX.ATT)) || stateInfo.IsName(AnimatorHelper.GetAnimationName(ANIM_INDEX.ATT01)) 
            || stateInfo.IsName(AnimatorHelper.GetAnimationName(ANIM_INDEX.ATT02)) || stateInfo.IsName(AnimatorHelper.GetAnimationName(ANIM_INDEX.ATT03));
    }

    protected virtual void CheckTouchKey()
    {
        if (mSkillTouchIndex == 0)
        {
            if (mTouchHandler.IsTouched(TOUCH_KEY.Skill1))
            {
                mSkillTouchIndex = 1;
            }
            if (mTouchHandler.IsTouched(TOUCH_KEY.Skill2))
            {
                mSkillTouchIndex = 2;
            }
            if (mTouchHandler.IsTouched(TOUCH_KEY.Skill3))
            {
                mSkillTouchIndex = 3;
            }
            if (mTouchHandler.IsTouched(TOUCH_KEY.Skill4))
            {
                mSkillTouchIndex = 4;
            }
        }
    }

}
