using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

//all animation translate conditions use this enum
public enum ANIM_INDEX
{
    NONE = -1,
    TRANSLATED = -1,
    IDLE = 0,
    RUN = 1,//gameobject'translate depends on this param 
    PREPARE = 2,
    HIT = 3,
    DOWN = 4,
    DEAD = 5,
    SUMMON = 6,
    ATT = 10,
    ATT01 = 11,
    ATT02 = 12,
    ATT03 = 13,
    ATT04 = 14,
    ATT05 = 15,
    SKILL01 = 21,
    SKILL02 = 22,
    SKILL03 = 23,
    SKILL04 = 24,
    SKILL05 = 25,
    INIT = 30
}

public enum STATUS
{
    IDLE = 0,
    RUN = 1,
    PREPARE = 2,
    HIT = 3,
    DOWN = 4,
    DEAD = 5,
    SUMMON1 = 6,
    SUMMON2 = 7,
    SUMMON3 = 8,
    PATROL = 10,
    ATTACK01 = 11,
    ATTACK02 = 12,
    ATTACK03 = 13,
    ATTACK04 = 14,
    ATTACK05 = 15,
    CANNOTMOVE = 27,
    CANNOTATTACK = 28,
    CANNOTSKILL = 29,
    CANNOTCONTROL = 30,
    NONE = -1,

    GUIDESKILL02 = 58,
    SKILL01 = 101,
    SKILL02 = 102,
    SKILL03 = 103,
    SKILL04 = 104
}

public enum TYPE
{
    ENEMY,
    ACTOR,
    NPC
}

public enum Dir
{
    LEFT=-1,
    RIGHT=1,
    DOWN = 2,
    UP = 3,
    NONE = 4,
}

public class StateHelper : IStatusHandler
{

    private static bool globalPause;

    public static bool GlobalPause {
        get {
            return globalPause;
        }
        set {
            globalPause = value;
        }
    }

    private bool ignoreGlobalPause = false;

    public bool IgnoreGlobalPause {
        get {
            return ignoreGlobalPause;
        }
        set {
            ignoreGlobalPause = value;
        }
    }

    public TYPE mType = TYPE.ENEMY;
    private ANIM_INDEX curAnimIndex;

    public ANIM_INDEX CurAnimIndex {
        get {
            return curAnimIndex;
        }
        set {
            curAnimIndex = value;
        }
    }

    private STATUS mCurStatus = STATUS.IDLE;

    public STATUS CurStatus {
        get {
            return mCurStatus;
        }
    }

    private Dictionary<STATUS, IStatus> statusMap = new Dictionary<STATUS, IStatus> ();
    private IActorController mActionController;

    public IActorController ActionController {
        get {
            return mActionController;
        }
    }

    private bool mUseNet;
    private bool mLockAutoChange;
    private int mTouchKey;

    private bool IsPlayer;

    public bool UseNet {
        get {
            return mUseNet;
        }
        set {
            mUseNet = value;
        }
    }

    public bool ContainsStatus (STATUS status)
    {
        return statusMap.ContainsKey (status);
    }

    public void SetActionController (IActorController controller)
    {
        this.mActionController = controller;
        IsPlayer = RoleEnum.IsPlayer (controller.GetRoleType ());
    }

    public bool ChangeStatusTo (STATUS st)
    {
        if (!statusMap.ContainsKey (st)) {
            Debug.LogError ("Invalid New Status:" + st);
            UnlockAutoChange ();
            return false;
        }
        IStatus lastStatus = null;
        statusMap.TryGetValue (CurStatus, out lastStatus);

        IStatus nextStatus = null;
        statusMap.TryGetValue (st, out nextStatus);

        if (nextStatus != null && nextStatus.OnEnter (CurStatus) && lastStatus != null) {
            lastStatus.OnLeave(st);
            STATUS last = CurStatus;
            mCurStatus = st;
            UnlockAutoChange ();
//            Debug.Log ("Change Status To:" + st);
            if (mActionController != null) {
                mActionController.OnNewStatus (last, CurStatus);
            } else {
                Debug.Log ("There is None Listener to Recieve change Event:" + st);
                Debug.Log("There is None Listener to Recieve change Event:" + st);
            }
            return true;
        } else {
            UnlockAutoChange ();
//            Debug.Log ("Enter Status " + curStatus + " failed!");
            return false;
        }
    }

    public void AddStatus (STATUS[] sts, IStatusFactory factory)
    {
        for (int i=0; i<sts.Length; i++) {
            STATUS st = sts [i];
            AddStatus (st, factory);        
        }
    }

    public void AddStatus (STATUS st, IStatusFactory factory)
    {
        if (statusMap.ContainsKey (st)) {
            Debug.Log ("Duplicate Status:" + st);
            return;
        }
        IStatus status = factory.CreateLogicByStatus (st);
        if (status != null) {
            status.Init (mActionController, this);
            statusMap.Add (st, status);
        }
    }

    public IStatus GetStatus (STATUS s) {
        IStatus ret = null;
        if (statusMap.ContainsKey(s)) {
            statusMap.TryGetValue(s, out ret);
        }
        return ret;
    }

    public void Update ()
    {
        Update (null);
    }

    public void Update (IStatusHandler netStatusHandlerDelegate)
    {
        if (!IgnoreGlobalPause && GlobalPause && CurStatus != STATUS.DEAD) {
            return;
        }
        if (!IsPlayer && Time.frameCount % 4 > 0)
            return;
        IStatus status = null;
        if (statusMap.TryGetValue (CurStatus, out status)) {
            status.UpdateLogic ();
            if (!IsLockAutoChange () && mActionController.IsUnderControl ()) {
                STATUS next = status.GetNextStatus ();
                if (next != STATUS.NONE) {
                    next = GameLibrary.Instance().SetIdleStatusByScene(next);
                    if (UseNet && netStatusHandlerDelegate != null) {
                        LockAutoChange ();
                        netStatusHandlerDelegate.ChangeStatusTo (next);
                    } else {
                        ChangeStatusTo (next);
                    }
                }
            }
        } else {
            Debug.Log ("Invalid Status:" + CurStatus);
        }
    }


    public bool IsLockAutoChange ()
    {
        return mLockAutoChange;
    }

    public void LockAutoChange ()
    {
        mLockAutoChange = true;
        mActionController.IsLockedPosition = true;
    }
        
    public void UnlockAutoChange ()
    {
        mLockAutoChange = false;
        mActionController.IsLockedPosition = false;
    }
}
