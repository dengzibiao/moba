using UnityEngine;
using System.Collections.Generic;
using CD = CDTimer.CD;

public class EffectChain : EffectTrackBase
{
    private Transform startPoint;
    private Transform targetPoint;
    private MeshFilter meshFilter;
    private float meshSize;
    public bool isContinue = false;
    private float mMaxDis;

    public override void Init(SkillNode skillNode, GameObject targetTrans, Transform thisTrans, VoidResult action = null)
    {
        base.Init(skillNode, targetTrans, thisTrans, action);
        transform.localScale = Vector3.zero;
        if (thisTrans)
        {
            FindBip(thisTrans, ref startPoint);
        }
        if (targetTrans != null)
        {
            if (mHitTargetCs.state == Modestatus.Tower)
            {
                targetPoint = hit;
            }
            else
            {
                FindBip(targetTrans.transform, ref targetPoint);
            }
        }
        isContinue = mCurSkillNode.interval_time != null && mCurSkillNode.interval_time.Length != 0;
        if (isContinue)
        {
            int count = mCurSkillNode.interval_time.Length;
            for (int i = 0; i < count; i++)
            {
                if (i == count - 1)
                {
                    CDTimer.GetInstance().AddCD(mCurSkillNode.interval_time[i], DestoryMe, 1);
                }
                CDTimer.GetInstance().AddCD(mCurSkillNode.interval_time[i], HitTarget, 1);
            }
        }
        else
        {
            DestoryMe(destoryTime);
        }
        mMaxDis = mCurSkillNode.max_fly + GameLibrary.Instance().GetExtendDis(mHitTargetCs);
    }

    private void FindBip(Transform parent, ref Transform targetPoint)
    {
        if (parent.name.Equals("Bip001"))
        {
            targetPoint = parent;
        }
        for (int i = 0; i < parent.childCount; i++)
        {
            if (targetPoint != null) break;
            FindBip(parent.GetChild(i), ref targetPoint);
        }
    }

    private void HitTarget(int count, long id)
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

    private void DestoryMe(int count, long id)
    {
        if (this != null && gameObject != null)
        {
            DestoryMe(0);
        }
    }

    private void DestoryMe(float count)
    {
        if (this != null && gameObject != null)
        {
            SetChainHitDisplay();
            Destroy(gameObject, count);
        }
    }

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
        }
        for (int i = 0; i < parent.childCount; i++)
        {
            if (meshFilter != null) break;
            FindMeshFilter(parent.GetChild(i));
        }
    }

    void Update()
    {
        if (mHitTargetCs != null && !mHitTargetCs.isDie && attackerCs != null && !attackerCs.isDie)
        {
            transform.position = startPoint.position;
            transform.LookAt(targetPoint);
            float mDistance = Vector3.Distance(startPoint.position, targetPoint.position);
            if (mDistance > mMaxDis)
            {
                DestoryMe(0);
                return;
            }
            transform.localScale = new Vector3(1, 1, mDistance / meshSize);
        }
        else
        {
            DestoryMe(0);
        }
    }

}
