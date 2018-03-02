using UnityEngine;
using System.Collections.Generic;

public class EffectBounce : EffectTrackBase
{
    private int mBounceAmount = 1;
    private int mBounceCount = 0;
    Collider col;
    [HideInInspector]
    public System.Action<EffectBounce, GameObject> DestoryDelegate;
    [HideInInspector]
    public int count;

    void Start()
    {
        col = GetComponent<Collider>();
    }

    public override void Init(SkillNode skillNode, GameObject targetTrans, Transform thisTrans, VoidResult action)
    {
        base.Init(skillNode, targetTrans, thisTrans, action);
        if (mCurSkillNode != null)
        {
            mBounceAmount = mCurSkillNode.target_ceiling;
        }
    }

    void Update()
    {
        if (mHitTargetCs == null || mHitTargetCs.isDie)
        {
            Destroy(gameObject);
        }
        else
        {
            transform.LookAt(hit);
            transform.Translate(Vector3.forward * Time.deltaTime * mCurSkillNode.flight_speed);
            mCurMonsters = GetTargetByCondition(CheckHitCondition, 0.1f, transform);
            HitAction();
        }
    }

    public override bool CheckHitCondition(CharacterState cs)
    {
        return mHitTargetCs != null && cs.transform == mHitTargetCs.transform && base.CheckHitCondition(cs);
    }

    private void HitAction()
    {
        if (mCurMonsters.Count != 0)
        {
            Hit(mCurMonsters);
            mBounceCount++;
            if (mBounceCount >= mBounceAmount)
            {
                if (DestoryDelegate != null)
                {
                    DestoryDelegate(this, mCurMonsters[0]);
                }
                Destroy(gameObject);
            }
            else
            {
                FindNearbyTarget();
            }
        }
    }

    public override void Hit(List<GameObject> monsters)
    {
        if (count == 0)
        {
            base.Hit(monsters);
        }
        else
        {
            for (int i = 0; i < monsters.Count; i++)
            {
                monsters[i].GetComponentInParent<CharacterState>().HitBy(mCurSkillNode, attackerCs, onceCharacterData);
            }
        }
    }

    public bool CheckNearbyHitCondition(CharacterState cs)
    {
        return mHitTargetCs != null && cs.transform != mHitTargetCs.transform && base.CheckHitCondition(cs);
    }

    private void FindNearbyTarget()
    {
        List<GameObject> mAllTarget = GetTargetByCondition(CheckNearbyHitCondition, distance, transform);
        GameObject target = null;
        if (mAllTarget != null && mAllTarget.Count > 0)
        {
            target = mAllTarget[Random.Range(0, mAllTarget.Count)];
        }
        if (target == null)
        {
            mHitTargetCs = null;
            hit = null;
        }
        else
        {
            mHitTargetCs = target.GetComponent<CharacterState>();
            hit = mHitTargetCs.mHitPoint;
        }
    }
}