using UnityEngine;
using System.Collections.Generic;

public class EffectSnakeSteer : EffectTrackBase
{
    private int mBounceAmount = 1;
    private int mBounceCount = 0;
    Collider col;
    private bool mBackAttacker = false;

    void Start()
    {
        col = GetComponent<Collider>();
    }

    public override void Init(SkillNode skillNode, GameObject targetTrans, Transform thisTrans, VoidResult action)
    {
        base.Init(skillNode, targetTrans, thisTrans, action);
        mBackAttacker = false;
        if (mCurSkillNode != null)
        {
            mBounceAmount = mCurSkillNode.target_ceiling;
        }
    }

    void Update()
    {
        transform.LookAt(hit);
        transform.Translate(Vector3.forward * Time.deltaTime * mCurSkillNode.flight_speed);
        if (mHitTargetCs == attackerCs)
        {
            if (attackerCs == null || attackerCs.isDie)
            {
                Destroy(gameObject);
                return;
            }
            float dis = Vector3.Distance(transform.position, hit.position);
            if (dis <= 0.1f)
            {
                mCurMonsters.Clear();
                mCurMonsters.Add(mHitTargetCs.gameObject);
                Hit(mCurMonsters);
                ComeBack();
                Destroy(gameObject);
            }
        }
        else
        {
            if (mHitTargetCs == null || mHitTargetCs.isDie)
            {
                FindNearTarget();
                return;
            }
            mCurMonsters = GetTargetByCondition(CheckHitCondition, 0.1f, transform);
            HitAction();
        }
    }

    public void ComeBack()
    {
        string id = "skill" + mCurSkillNode.site + "_1";
        attackerCs.emission.PlaySpellEffectByUrl(attackerCs.emission.GetEffectResourceRoot() + id, id);
    }

    public override bool CheckHitCondition(CharacterState cs)
    {
        return cs != null && cs.transform == mHitTargetCs.transform && base.CheckHitCondition(cs);
    }

    private void HitAction()
    {
        if (mCurMonsters.Count != 0)
        {
            mAllMonsters.Add(mHitTargetCs.gameObject);
            Hit(mCurMonsters);
            mBounceCount++;
            FindNearTarget();
        }
    }

    private void FindNearTarget()
    {
        if (mBounceCount >= mBounceAmount)
        {
            SetTargetToAttacker();
        }
        else
        {
            FindNearbyTarget();
        }
    }

    public override void Hit(List<GameObject> monsters)
    {
        if (mBackAttacker)
        {
            characterData.skill_Damage[1] = mBounceCount * characterData.skill_Damage[1];
        }
        base.Hit(monsters);
    }

    private void SetTargetToAttacker()
    {
        if (attackerCs != null && !attackerCs.isDie)
        {
            mHitTargetCs = attackerCs;
            hit = attackerCs.mHitPoint;
            mBackAttacker = true;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool CheckNearbyHitCondition(CharacterState cs)
    {
        return cs != null && cs != attackerCs && GameLibrary.Instance().IsInvisiblityCanSetTarget(attackerCs, cs) && !mAllMonsters.Contains(cs.gameObject) && base.CheckHitCondition(cs);
    }

    private void FindNearbyTarget()
    {
        List<GameObject> mAllTarget = GetTargetByCondition(CheckNearbyHitCondition, distance, transform);
        GameObject target = null;
        if (mAllTarget != null && mAllTarget.Count > 0)
        {
            mAllTarget.Sort((a, b) =>
            {
                float aDir = Vector3.Distance(transform.position, a.transform.position);
                float bDir = Vector3.Distance(transform.position, b.transform.position);
                return Mathf.FloorToInt(aDir - bDir);
            });
            target = mAllTarget[0];
        }
        if (target == null)
        {
            SetTargetToAttacker();
        }
        else
        {
            mHitTargetCs = target.GetComponent<CharacterState>();
            hit = mHitTargetCs.mHitPoint;
        }
    }
}
