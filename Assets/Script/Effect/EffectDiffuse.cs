using UnityEngine;

public class EffectDiffuse : EffectTrackBase
{
    private float[] intervals;
    private float mDeltaTime;
    private SphereCollider sphere;
    private float speed;
    private byte mIntervalTime;
    private float mRadiusMax;
    void Awake()
    {
        FindPosTrans(transform);
    }

    public override void Init(SkillNode skillNode, GameObject targetTrans, Transform thisTrans, VoidResult action)
    {
        base.Init(skillNode, targetTrans, thisTrans, action);
        intervals = mCurSkillNode.interval_time;
        mIntervalTime = mCurSkillNode.target_ceiling == 0 ? (byte)1 : mCurSkillNode.target_ceiling;
        distance = 0;
        sphere = gameObject.AddMissingComponent<SphereCollider>();
        if (pos != transform)
        {
            sphere.center = pos.localPosition;
        }
        sphere.isTrigger = true;
        sphere.radius = 0f;
        mRadiusMax = mCurSkillNode.aoe_long + GameLibrary.Instance().GetExtendDis(mHitTargetCs);
        Destroy(gameObject, destoryTime);
    }

    void Update()
    {
        if (intervals.Length > 1)
        {
            mDeltaTime += Time.deltaTime;
            if (mDeltaTime >= intervals[0] && mDeltaTime <= intervals[1])
            {
                speed = mRadiusMax * mIntervalTime / (intervals[1] - intervals[0]);
                distance += speed * Time.deltaTime;
                if (distance >= mRadiusMax)
                {
                    distance = mIntervalTime > 1 ? 0 : mRadiusMax;
                    mAllMonsters.Clear();
                }
            }
            if (sphere != null)
            {
                sphere.radius = distance;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        HandleHit(other.gameObject);
    }

    private void HandleHit(GameObject monster)
    {
        CharacterState mCurTargetCs = monster.GetComponent<CharacterState>();
        if (mCurTargetCs != null && CheckHitCondition(mCurTargetCs) && CheckInView(mCurTargetCs, attackerCs))
        {
            mCurMonsters.Clear();
            if (!mAllMonsters.Contains(monster))
            {
                mAllMonsters.Add(monster);
                mCurMonsters.Add(monster);
                Hit(mCurMonsters);
            }
        }
    }
}