using UnityEngine;


public class CommonComboSkill : BaseStatus
{
    protected ANIM_INDEX mAnimIndex;
    protected STATUS mCurrent;
    protected STATUS mNext;
    protected float timer = 0.0f;

    public CommonComboSkill (ANIM_INDEX animIndex, STATUS current, STATUS next)
    {
        this.mAnimIndex = animIndex;
        this.mCurrent = current;
        this.mNext = next;
    }

    public bool IsPet;

    public override void UpdateLogic ()
    {
    }

    public override STATUS GetNextStatus ()
    {
        STATUS buffEffect = mController.GetEffectOfBuff ();
        if (buffEffect == STATUS.CANNOTCONTROL)
            return STATUS.IDLE;
        STATUS next = base.GetNextStatus ();
        if (next != STATUS.NONE)
            return next;

        if (mController.ShouldPlayNextAnimation (mAnimIndex)) {

        }
        return STATUS.NONE;
    }

    public override bool OnEnter (STATUS last)
    {
        timer = 0.0f;
        RemoveCondition (IsUnderAttack);
        mController.PlayAnimation (mAnimIndex);
        return true;
    }
}