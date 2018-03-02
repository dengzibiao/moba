using UnityEngine;
using System.Collections.Generic;

public class EffectTraction : EffectTrackBase
{
    public Transform startPoint, targetPoint, tractionPoint;
    private MeshFilter meshFilter;
    private float meshSize;
    private float mTractionDistance;
    private Vector3 mTractionDir;
    void Awake()
    {
        FindMeshFilter(transform);
    }

    private void FindMeshFilter(Transform parent)
    {
        MeshFilter mesh = parent.GetComponent<MeshFilter>();
        if (mesh != null)
        {
            meshFilter = mesh;
            Vector3 meshBoundSize = mesh.mesh.bounds.size;
            Vector3 meshScale = mesh.transform.localScale;
            meshSize = meshBoundSize.z * meshScale.z;
            return;
        }
        for (int i = 0; i < parent.childCount; i++)
        {
            if (meshFilter != null) break;
            FindMeshFilter(parent.GetChild(i));
        }
    }

    public override void Init(SkillNode skillNode, GameObject targetTrans, Transform thisTrans, VoidResult action = null)
    {
        base.Init(skillNode, targetTrans, thisTrans, action);
        if (mCurSkillNode != null && attackerCs != null)
        {
            if (thisTrans)
            {
                CharacterPart rightPart = attackerCs.playerPart.Find(a => a.mRolePart == RolePart.RightHand);
                tractionPoint = rightPart == null ? null : rightPart.transform;
                FindBip(thisTrans, ref tractionPoint, "Bip001 R Hand");
                FindBip(thisTrans, ref startPoint, "Bip001");
            }
            if (targetTrans != null)
            {
                if (mHitTargetCs.state == Modestatus.Tower)
                {
                    targetPoint = hit;
                }
                else
                {
                    FindBip(targetTrans.transform, ref targetPoint, "Bip001");
                }
                HitTarget();
                mTractionDistance = attackerCs.pm.nav.radius * attackerCs.transform.localScale.z + mHitTargetCs.pm.nav.radius * attackerCs.transform.localScale.z;
            }
        }
    }

    private void HitTarget()
    {
        if (this != null && gameObject != null && mHitTargetCs != null && attackerCs != null)
        {
            if (mHitTargetCs.state != Modestatus.Tower)
            {
                mCurMonsters.Clear();
                mCurMonsters.Add(attackerCs.gameObject);
                mCurMonsters.Add(mHitTargetCs.gameObject);
                Hit(mCurMonsters);
            }
        }
    }

    private void FindBip(Transform parent, ref Transform targetPoint, string mTempName)
    {
        if (parent.name.Equals(mTempName))
        {
            targetPoint = parent;
        }
        for (int i = 0; i < parent.childCount; i++)
        {
            if (targetPoint != null) break;
            FindBip(parent.GetChild(i), ref targetPoint, mTempName);
        }
    }

    void Update()
    {
        if (mHitTargetCs != null && !mHitTargetCs.isDie && attackerCs != null && !attackerCs.isDie && tractionPoint != null)
        {
            float mDistance = Vector3.Distance(startPoint.position, targetPoint.position);
            if (mDistance <= mTractionDistance)
            {
                DestoryMe();
                return;
            }
            mTractionDir = attackerTrans.position - mHitTargetCs.transform.position;
            transform.position = tractionPoint.position;
            transform.LookAt(targetPoint);
            float mTractionDis = Vector3.Distance(transform.position, targetPoint.position);
            mHitTargetCs.transform.Translate(mTractionDir.normalized * Time.deltaTime * mCurSkillNode.flight_speed, Space.World);
            transform.localScale = new Vector3(1, 1, mTractionDis / meshSize);
        }
        else
        {
            DestoryMe();
        }
    }

    private void DestoryMe()
    {
        if (this != null && gameObject != null)
        {
            enabled = false;
            SetChainHitDisplay();
            Destroy(gameObject);
        }
    }
}
