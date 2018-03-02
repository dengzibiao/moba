using UnityEngine;
using System.Collections;

public enum HitActionType
{
    None = 0,
    HitHange = 1,
    HitRepel = 2
}

public class BaseAction : MonoBehaviour
{
    public CharacterState target;
    public UnityEngine.AI.NavMeshAgent nav;
    public float distance;
    public float speed;
    public Vector3 dir;
    public float duration;
    public float mDeltaTime;

    public virtual void Init(CharacterState cs, float duration, Vector3 dir, float mSpeed = 0)
    {
        target = cs;
        if (target != null && target.pm != null)
        {
            nav = target.pm.nav;
            if (!nav.enabled)
                nav.enabled = true;
            if (nav.isOnNavMesh)
            {
                cs.mOriginPos = cs.transform.position;
            }
        }
        this.duration = duration;
        this.dir = dir;
        mDeltaTime = 0;
        SetTargetEnable(false);
        cs.pm.HitFly();
    }

    public virtual void Update()
    {
        mDeltaTime += Time.deltaTime;
    }

    public virtual void SetTargetEnable(bool b)
    {
        if (target != null && target.pm != null)
        {
            enabled = !b;
            target.pm.SetControlSwitch(!b);
        }
    }

    public virtual void ResetPosition()
    {
        if (target.isDie || (nav != null && !nav.isOnNavMesh))
        {
            transform.position = target.mOriginPos;
            transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
        }
    }
}
