
using UnityEngine;

public class CommonPlayerActionHandler : IActionHandler, IStatusFactory
{
    protected IActorController actController;
    protected TouchHandler mTouchHandler;
    protected ANIM_INDEX mLastAnimIndex = ANIM_INDEX.TRANSLATED;
    protected StateHelper mStateHelper;

    public StateHelper StateHelper {
        get {
            return mStateHelper;
        }
    }

    protected STATUS[] defaultStatus = {
        STATUS.IDLE,
        STATUS.RUN,
        STATUS.PREPARE,
        STATUS.HIT,
        STATUS.DEAD,
        STATUS.CANNOTCONTROL,
        STATUS.ATTACK01,
        STATUS.ATTACK02,
        STATUS.ATTACK03,
        STATUS.SKILL01,
        STATUS.SKILL02,
        STATUS.SKILL03,
        STATUS.SKILL04,
        STATUS.SUMMON1,
        STATUS.SUMMON2,
        STATUS.SUMMON3,
    };
    protected IStatusHandler mNetStatusHandler;

    public CommonPlayerActionHandler (IActorController actionController)
    {
        this.ActController = actionController;
        mTouchHandler = TouchHandler.GetInstance ();
        mStateHelper = new StateHelper ();
        mStateHelper.SetActionController (actionController);
        mStateHelper.UseNet = true;
        InitStatus ();
    }

    public CommonPlayerActionHandler (IActorController actionController, IStatusHandler netStatusHandler)
    {
        this.ActController = actionController;
        this.mNetStatusHandler = netStatusHandler;
        mTouchHandler = TouchHandler.GetInstance ();
        mStateHelper = new StateHelper ();
        mStateHelper.SetActionController (actionController);
        mStateHelper.UseNet = true;
        InitStatus ();
    }

    public virtual void InitStatus ()
    {
        mStateHelper.AddStatus (defaultStatus, this);
    }

    public virtual void HandleAction ()
    {
        mStateHelper.Update (mNetStatusHandler);
        ActController.FoundTarget();
    }

    public StateHelper GetStateHelper ()
    {
        return mStateHelper;
    }

    public IActorController ActController {
        get {
            return actController;
        }
        set {
            actController = value;
        }
    }

    #region IStatusFactory implementation

    public virtual IStatus CreateLogicByStatus (STATUS st)
    {
        switch (st) {
        case STATUS.IDLE:
            return new CommonIdle ();
        case STATUS.PREPARE:
            return new CommonPrepare();
        case STATUS.RUN:
            return new CommonRun(mTouchHandler.mOffset);
        case STATUS.HIT:
            return new CommonUnderAttack(ANIM_INDEX.HIT);
        case STATUS.CANNOTCONTROL:
            return new CommonUnderControl(ANIM_INDEX.PREPARE);
        case STATUS.DEAD:
            return new CommonUnderDead(STATUS.NONE, ANIM_INDEX.DEAD);
        case STATUS.ATTACK01:
            return new CommonAttack (STATUS.ATTACK02, ANIM_INDEX.ATT01);
        case STATUS.ATTACK02:
            return new CommonAttack (STATUS.ATTACK03, ANIM_INDEX.ATT02);
        case STATUS.ATTACK03:
            return new CommonAttack (STATUS.IDLE, ANIM_INDEX.ATT03);
        case STATUS.SKILL01:
            return new CommonSkillBase(new ANIM_INDEX[] {ANIM_INDEX.SKILL01});
        case STATUS.SKILL02:
            return new CommonSkillBase(new ANIM_INDEX[] { ANIM_INDEX.SKILL02 });
        case STATUS.SKILL03:
            return new CommonSkillBase(new ANIM_INDEX[] { ANIM_INDEX.SKILL03 });
        case STATUS.SKILL04:
            return new CommonSkillBase(new ANIM_INDEX[] { ANIM_INDEX.SKILL04 });
        case STATUS.SUMMON1:
            return new CommonSummon(1);
        case STATUS.SUMMON2:
            return new CommonSummon(2);
        case STATUS.SUMMON3:
            return new CommonSummon(3);
        }
        return new CommonIdle ();
    }

    #endregion
}

