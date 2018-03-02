using UnityEngine;
using System.Collections.Generic;

public class EffectGenerateObstacle : EffectTrackBase
{
    private float mExtendSize;
    private UnityEngine.AI.NavMeshObstacle navOb;
    private float mDeltaTime = 0f;
    private UnityEngine.AI.NavMeshHit navMeshHit;
    void Awake()
    {
        FindPosTrans(transform);
        mExtendSize = Vector3.Distance(transform.position, pos.position);
    }

    public override void Init(SkillNode skillNode, GameObject targetTrans, Transform thisTrans, VoidResult action = null)
    {
        base.Init(skillNode, targetTrans, thisTrans, action);
        Destroy(gameObject, destoryTime);
        if (attackerCs != null && !attackerCs.isDie)
        {
            attackerCs.HitActionDelegate += HitAction;
        }
    }

    private void HitAction(long skillId)
    {
        if (attackerCs != null && skillId == mCurSkillNode.skill_id)
        {
            attackerCs.HitActionDelegate -= HitAction;
            if (this != null)
            {
                GenerateObstacle();
                Hit(GetDamageTarget());
            }
        }
    }

    private void GenerateObstacle()
    {
        navOb = pos.gameObject.AddMissingComponent<UnityEngine.AI.NavMeshObstacle>();
        navOb.shape = UnityEngine.AI.NavMeshObstacleShape.Box;
        navOb.carving = true;
        float x, y, z;
        float mSizeRate = mCurSkillNode.angle / 360f;
        x = y = mCurSkillNode.aoe_wide * mSizeRate;
        z = mCurSkillNode.aoe_long * mSizeRate;
        Vector3 mTargetPos = transform.position + transform.forward * (z + mExtendSize);
        float mTempDis = z;
        if (!attackerCs.pm.nav.enabled)
        {
            attackerCs.pm.nav.enabled = true;
        }
        if (attackerCs.pm.nav.Raycast(mTargetPos, out navMeshHit))
        {
            mTempDis = Vector3.Distance(navMeshHit.position, pos.position);
        }
        if (Physics.Linecast(transform.position, mTargetPos, 1 << (int)GameLayer.Obstacle))
        {
            mTempDis = z;
        }
        z = mTempDis;
        navOb.size = new Vector3(x, y, z);
        navOb.center = new Vector3(0, 0, z / 2);
        Rigidbody rd = pos.gameObject.AddMissingComponent<Rigidbody>();
        rd.useGravity = false;
        rd.isKinematic = true;
        BoxCollider bc = pos.gameObject.AddMissingComponent<BoxCollider>();
        bc.isTrigger = false;
        bc.size = new Vector3(x, y, z);
        bc.center = new Vector3(0, 0, z / 2);
        pos.gameObject.layer = (int)GameLayer.Obstacle;
    }

    private List<GameObject> GetDamageTarget()
    {
        List<GameObject> resultGo = new List<GameObject>();
        float length = (mCurSkillNode.aoe_long + mExtendSize * 2) * 0.5f;
        float width = mCurSkillNode.aoe_wide;
        resultGo = GetTargetByCondition(CheckHitCondition, length, width, transform);
        return resultGo;
    }

    void Update()
    {
        mDeltaTime += Time.deltaTime;
        if (navOb == null && (attackerCs.isDie || !GameLibrary.Instance().CanControlSwitch(attackerCs.pm)))
        {
            Destroy(gameObject);
        }
        if (navOb != null && navOb.enabled && mDeltaTime >= destoryTime - 0.3f)
        {
            navOb.enabled = false;
        }
    }
}
