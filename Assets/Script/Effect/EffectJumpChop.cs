using UnityEngine;

public class EffectJumpChop : EffectTrackBase
{
    private Vector3 targetPos;
    private bool mJumpDelegate = false;
    private float mSpeed;

    public override void Init(SkillNode skillNode, GameObject targetTrans, Transform thisTrans, VoidResult action)
    {
        base.Init(skillNode, targetTrans, thisTrans, action);
        Destroy(gameObject, destoryTime);
        if (attackerCs != null)
        {
            attackerCs.JumpStartDelegate += JumpStart;
            attackerCs.JumpOverDelegate += JumpOver;
            attackerCs.HitActionDelegate += HitAction;
        }
    }

    private void JumpStart()
    {
        if (attackerCs != null && !attackerCs.isDie)
        {
            attackerCs.JumpStartDelegate -= JumpStart;
            if (mHitTargetCs != null)
            {
                targetPos = mHitTargetCs.transform.position;
                transform.forward = attackerCs.transform.forward = targetPos - attackerCs.transform.position;
                float mJumpTime = 0.3f;
                if (mCurSkillNode.interval_time.Length > 0)
                {
                    mJumpTime = mCurSkillNode.interval_time[0];
                }
                mSpeed = Vector3.Distance(attackerCs.transform.position, targetPos) / mJumpTime;
            }
            mJumpDelegate = true;
        }
    }

    private void JumpOver()
    {
        if (attackerCs != null && !attackerCs.isDie)
        {
            attackerCs.JumpOverDelegate -= JumpOver;
            mJumpDelegate = false;
        }
    }

    private void HitAction(long skillId)
    {
        if (attackerCs != null && skillId == mCurSkillNode.skill_id)
        {
            attackerCs.HitActionDelegate -= HitAction;
            if (this != null)
            {
                mAllMonsters.Clear();
                GetCommonDamageRange(transform);
                Hit(mCurMonsters);
            }
        }
    }

    void Update()
    {
        if (attackerCs == null || attackerCs.isDie)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            transform.position = attackerCs.transform.position;
        }
        if (mJumpDelegate && mHitTargetCs != null)
        {
            Vector3 dir = mHitTargetCs.transform.position - attackerCs.transform.position;
            attackerCs.pm.FastMove((dir - dir.normalized * GetTargetRadius(mHitTargetCs))
                * Time.deltaTime * mSpeed);
        }
    }

    public override float GetTargetRadius(CharacterState target)
    {
        float result = 0;
        if (target != null)
        {
            UnityEngine.AI.NavMeshAgent nav = target.GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (nav != null)
            {
                result = nav.height;
            }
            else
            {
                CapsuleCollider cp = target.GetComponent<CapsuleCollider>();
                result = cp == null ? result : cp.radius;
            }
        }
        return result;
    }
}