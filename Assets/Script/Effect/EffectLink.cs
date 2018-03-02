using UnityEngine;
using System.Collections.Generic;
using System;

public class EffectLink : EffectTrackBase
{
    private int mBounceAmount;
    private List<EffectLinkBranch> mLinks = new List<EffectLinkBranch>();
    public override void Init(SkillNode skillNode, GameObject targetTrans, Transform thisTrans, VoidResult action = null)
    {
        base.Init(skillNode, targetTrans, thisTrans, action);
        if (mCurSkillNode != null)
        {
            mBounceAmount = mCurSkillNode.target_ceiling;
        }
        AddEffectLinkBranch(null, null);
    }

    private void GetNextLink(EffectLinkBranch go)
    {
        mCurMonsters.Clear();
        mCurMonsters.Add(go.mHitTargetCs.gameObject);
        Hit(mCurMonsters);
        FindNearTarget(go);
    }

    private void FindNearbyTarget(EffectLinkBranch go)
    {
        List<GameObject> mAllTarget = GetTargetByCondition(CheckNearbyHitCondition, distance, go.mHitTargetCs.transform);
        GameObject target = null;
        if (mAllTarget != null && mAllTarget.Count > 0)
        {
            mAllTarget.Sort((a, b) =>
            {
                float aDir = Vector3.Distance(go.mHitTargetCs.transform.position, a.transform.position);
                float bDir = Vector3.Distance(go.mHitTargetCs.transform.position, b.transform.position);
                return Mathf.FloorToInt(aDir - bDir);
            });
            target = mAllTarget[0];
        }
        if (target != null)
        {
            AddEffectLinkBranch(go, target);
        }
    }

    private void AddEffectLinkBranch(EffectLinkBranch go, GameObject target)
    {
        EffectLinkBranch linkBranch;
        if (mLinks.Count == 0)
        {
            linkBranch = gameObject.AddComponent<EffectLinkBranch>();
            linkBranch.destoryTime = destoryTime;
            linkBranch.Init(mCurSkillNode, mHitTargetCs.gameObject, attackerTrans, null);
            mAllMonsters.Add(mHitTargetCs.gameObject);
        }
        else
        {
            linkBranch = NGUITools.AddChild(transform.parent.gameObject, go.gameObject).GetComponent<EffectLinkBranch>();
            EffectLink link = linkBranch.GetComponent<EffectLink>();
            Destroy(link);
            linkBranch.destoryTime = destoryTime;
            linkBranch.Init(mCurSkillNode, target, go.mHitTargetCs.transform, null);
            mAllMonsters.Add(target);
        }
        linkBranch.mNextLink = GetNextLink;
        linkBranch.mLastDestory = DestoryMe;
        mLinks.Add(linkBranch);
    }

    private void FindNearTarget(EffectLinkBranch go)
    {
        if (mLinks.Count < mBounceAmount)
        {
            FindNearbyTarget(go);
        }
    }

    private void DestoryMe(EffectLinkBranch go)
    {
        if (mLinks.LastIndexOf(go) == mLinks.Count - 1)
        {
            for (int i = 0; i < mLinks.Count; i++)
            {
                Destroy(mLinks[i].gameObject);
            }
        }
    }

    public bool CheckNearbyHitCondition(CharacterState cs)
    {
        return cs != attackerCs && GameLibrary.Instance().IsInvisiblityCanSetTarget(attackerCs, cs) && !mAllMonsters.Contains(cs.gameObject) && base.CheckHitCondition(cs);
    }
}

public class EffectLinkBranch : EffectTrackBase
{
    private Transform startPoint;
    private Transform targetPoint;
    private MeshFilter meshFilter;
    private float meshSize;
    public bool isLinked, isEnabled;
    public float mDeltaTime;
    private float mSpeed;
    private float mDeltaDis;
    [HideInInspector]
    public Action<EffectLinkBranch> mNextLink;
    [HideInInspector]
    public Action<EffectLinkBranch> mLastDestory;

    public override void Init(SkillNode skillNode, GameObject targetTrans, Transform thisTrans, VoidResult action = null)
    {
        base.Init(skillNode, targetTrans, thisTrans, action);
        mDeltaTime = 0f;
        isLinked = isEnabled = false;
        mSpeed = mCurSkillNode.flight_speed;
        mDeltaDis = 0;
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
        FindMeshFilter(transform);
    }

    private void FindBip(Transform parent, ref Transform targetPoint)
    {
        if (targetPoint != null) return;
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

    private void EnActiveChild(Transform parent)
    {
        if (parent.childCount > 0)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                parent.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    void Update()
    {
        mDeltaTime += Time.deltaTime;
        if (mDeltaTime >= destoryTime && !isEnabled)
        {
            EnActiveChild(transform);
            mLastDestory(this);
            isEnabled = true;
        }
        if (!isEnabled)
        {
            float mDistance = Vector3.Distance(startPoint.position, targetPoint.position);
            if (mDeltaDis != mDistance)
            {
                transform.position = startPoint.position;
                transform.LookAt(targetPoint);
                mDeltaDis += mDistance * mSpeed * Time.deltaTime;
                mDeltaDis = mDeltaDis >= mDistance ? mDistance : mDeltaDis;
                transform.localScale = new Vector3(1, 1, mDeltaDis / meshSize);
                if (!isLinked && mDeltaDis == mDistance)
                {
                    HitAction();
                }
            }
        }
    }

    private void HitAction()
    {
        mNextLink(this);
        isLinked = true;
    }
}
