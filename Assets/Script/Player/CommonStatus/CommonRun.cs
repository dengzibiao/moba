using UnityEngine;
using System.Collections;

public class CommonRun : BaseStatus
{
    public Vector3 mDir = Vector3.zero;
    protected STATUS mNext = STATUS.NONE;

    public CommonRun (Vector3 dir)
    {
        mDir = dir;
    }

    public override bool OnEnter (STATUS last)
    {
        mController.PlayAnimation (ANIM_INDEX.RUN);
        AddPlayerSkillConditions ();
        AddCondition (IsPlayerAttack, STATUS.ATTACK01);
        return true;
    }

    public override void UpdateLogic ()
    {
        mNext = STATUS.NONE;
        if (shouldMove)
        {
            PlayerMotion playerMotion = CharacterManager.playerCS.pm;
            if (playerMotion.cs == null || playerMotion.cs.attackTarget == null || playerMotion.cs.isDie || playerMotion.cs.attackTarget.isDie)
                return;
            Vector3 relativePos = playerMotion.cs.attackTarget.transform.position - playerMotion.cs.transform.position;
            float tempDistance = Vector3.Distance(playerMotion.cs.transform.position, playerMotion.cs.attackTarget.transform.position);
            if (playerMotion.cs.mCurSkillNode != null)
            {
                if (tempDistance <= playerMotion.cs.mCurSkillNode.dist)
                {
                    ReleaseSkill();
                }
                else
                {
                    mController.SetDirection(relativePos);
                    mController.Move(relativePos);
                }
            }
        }
        else
        {
            mController.SetDirection(TouchHandler.GetInstance().mOffset);
            mController.Move(TouchHandler.GetInstance().mOffset);
        }
    }

    private void ReleaseSkill()
    {
        if (isUseSkill && IsPlayerCanUseSkill(mCurIndex) && !IsCanNotAttack())
        {
            mNext = GetSkillStatusByIndex(mCurIndex);
        }
        else
        {
            mNext = STATUS.ATTACK01;
        }
    }


    public override void OnLeave (STATUS next)
    {
        base.OnLeave(next);
    }

    public override STATUS GetNextStatus ()
    {
        STATUS next = base.GetNextStatus();
        if (next != STATUS.NONE && !shouldMove)
        {
            if ((int)next >= (int)STATUS.SKILL01 && (int)next <= (int)STATUS.SKILL04)
            {
                if (IsPlayerCanUseSkill(mCurIndex) && !IsCanNotAttack())
                {
                    return next;
                }
            }
            else
            {
                return next;
            }
        }

        if (shouldMove)
        {
            if (mTouchHandler.IsTouched(TOUCH_KEY.Run))
            {
                shouldMove = false;
            }
            return mNext;
        }
        if (!mTouchHandler.IsTouched(TOUCH_KEY.Run))
        {
            return STATUS.IDLE;
        }
        else
        {
            return STATUS.NONE;
        }
    }
}
