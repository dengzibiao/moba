using UnityEngine;

public class EffectLaunchDiffuseMeter : EffectTrackBase
{
    private Vector3 mTargetPos;
    private GameObject mDiffusePrefab;
    private bool mStartLaunch, mStartDiffuse;
    private Vector3 mTargetDir;
    private BoxCollider mLongCol, mWideCol;
    private float mLong, mWide;
    private float mDeltaTime;

    public override void Init(SkillNode skillNode, GameObject targetTrans, Transform thisTrans, VoidResult action = null)
    {
        base.Init(skillNode, targetTrans, thisTrans, action);
        mStartLaunch = mStartDiffuse = false;
        mLong = mCurSkillNode.aoe_long;
        mWide = mCurSkillNode.aoe_wide;
        if (attackerCs != null)
        {
            attackerCs.HitActionDelegate += LaunchPoint;
        }
        if (targetTrans != null)
        {
            mTargetPos = targetTrans.transform.position;
            mTargetDir = mTargetPos - transform.position;
            mLongCol = gameObject.AddComponent<BoxCollider>();
            mWideCol = gameObject.AddComponent<BoxCollider>();
            mLongCol.isTrigger = mWideCol.isTrigger = true;
            mLongCol.size = mWideCol.size = Vector3.one * mWide;
        }
    }

    private void AddMeterDiffuse()
    {
        if (mDiffusePrefab == null)
        {
            mDiffusePrefab = BattleUtil.AddEffectTo(attackerCs.emission.GetEffectResourceRoot() + "skill" + mCurSkillNode.site + "_1", transform);
            mDiffusePrefab.transform.parent = transform.parent;
            mDiffusePrefab.transform.position = mTargetPos;
            mDiffusePrefab.gameObject.SetActive(true);
            mDiffusePrefab.transform.eulerAngles = transform.eulerAngles = new Vector3(0, attackerCs.transform.eulerAngles.y, 0);
            mLongCol.size = new Vector3(mWide, mWide, mLong);
            mWideCol.size = new Vector3(mLong, mWide, mWide);
        }
    }

    private void LaunchPoint(long skillId)
    {
        RemoveDelegate(skillId);
        if (this != null)
        {
            mStartLaunch = true;
        }
    }

    void Update()
    {
        if (mStartLaunch)
        {
            DestoryMe();
            transform.LookAt(mTargetPos);
            transform.Translate(Vector3.forward * Time.deltaTime * mCurSkillNode.flight_speed);
            if (Vector3.Distance(transform.position, mTargetPos) <= 0.1f)
            {
                mStartLaunch = false;
                mStartDiffuse = true;
                mDeltaTime = 0;
                AddMeterDiffuse();
            }
        }
        else
        {
            if (attackerCs == null || attackerCs.isDie)
            {
                Destroy(gameObject);
            }
        }
        if (mStartDiffuse)
        {
            DestoryMe();
        }
    }

    private void DestoryMe()
    {
        mDeltaTime += Time.deltaTime;
        if (mDeltaTime >= destoryTime)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        HandleHit(other.gameObject);
    }

    private void HandleHit(GameObject monster)
    {
        CharacterState mCurTargetCs = monster.GetComponent<CharacterState>();
        if (mCurTargetCs != null && CheckHitCondition(mCurTargetCs) && CheckInView(mCurTargetCs, attackerCs) && !mAllMonsters.Contains(monster))
        {
            mAllMonsters.Add(monster);
            Hit(mAllMonsters);
        }
    }

    void OnDestroy()
    {
        if (mDiffusePrefab != null)
        {
            Destroy(mDiffusePrefab);
        }
    }

    private void RemoveDelegate(long skillId)
    {
        if (attackerCs != null && skillId == mCurSkillNode.skill_id)
        {
            attackerCs.HitActionDelegate -= LaunchPoint;
        }
    }
}
