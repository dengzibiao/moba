using UnityEngine;
using System.Collections.Generic;

public class EffectMoveDiffusionShrink : EffectTrackBase
{
    private float[] intervals;
    private float mDeltaTime;
    private SphereCollider sphere;
    private float speed;
    private float mRadiusMax;

    void Awake()
    {
        FindPosTrans(transform);
    }

    public override void Init(SkillNode skillNode, GameObject targetTrans, Transform thisTrans, VoidResult action)
    {
        base.Init(skillNode, targetTrans, thisTrans, action);
        intervals = mCurSkillNode.interval_time;
        distance = 0;
        sphere = gameObject.AddMissingComponent<SphereCollider>();
        sphere.isTrigger = true;
        sphere.radius = 0f;
        mRadiusMax = mCurSkillNode.max_fly + GameLibrary.Instance().GetExtendDis(mHitTargetCs);
    }

    void Update()
    {
        mDeltaTime += Time.deltaTime;
        if (attackerTrans != null && !attackerCs.isDie)
        {
            transform.position = attackerTrans.position;
        }
        if (mDeltaTime >= intervals[0] && mDeltaTime <= intervals[1])
        {
            speed = mRadiusMax / (intervals[1] - intervals[0]);
            distance += speed * Time.deltaTime;
        }
        else if (mDeltaTime >= intervals[1] && mDeltaTime <= intervals[2])
        {
            speed = mRadiusMax / (intervals[2] - intervals[1]);
            distance -= speed * Time.deltaTime;
        }
        else if (mDeltaTime >= intervals [2])
        {
            Destroy(gameObject);
        }
        sphere.radius = distance;
    }

    void OnTriggerEnter(Collider other)
    {
        HandleHit(other.gameObject);
    }

    void OnTriggerExit(Collider other)
    {
        HandleHit(other.gameObject);
    }

    private void HandleHit(GameObject monster)
    {
        CharacterState mCurTargetCs = monster.GetComponent<CharacterState>();
        if (mCurTargetCs != null && CheckHitCondition(mCurTargetCs) && CheckInView(mCurTargetCs, attackerCs))
        {
            mAllMonsters.Clear();
            mAllMonsters.Add(monster);
            Hit(mAllMonsters);
        }
    }
}