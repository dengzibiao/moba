using UnityEngine;
using System.Collections.Generic;

public class EffectMultiBounce : EffectTrackBase
{
    private bool isHitAction;
    private GameObject prefab;
    void Awake()
    {
        FindPosTrans(transform);
    }

    public override void Init(SkillNode skillNode, GameObject targetTrans, Transform thisTrans, VoidResult action)
    {
        base.Init(skillNode, targetTrans, thisTrans, action);
        Destroy(gameObject, destoryTime);
        if (attackerCs != null && !attackerCs.isDie)
        {
            prefab = Resources.Load(attackerCs.emission.GetEffectResourceRoot() + "skill" + mCurSkillNode.site + "_Bounce") as GameObject;
            isHitAction = false;
            attackerCs.HitActionDelegate += HitDelegate;
        }
    }

    private void HitDelegate(long skillId)
    {
        if (attackerCs != null && skillId == mCurSkillNode.skill_id)
        {
            attackerCs.HitActionDelegate -= HitDelegate;
            if (this != null)
            {
                isHitAction = true;
                GetCommonDamageRange(pos);
                GenerateBounce(0);
            }
        }
    }

    private void GenerateBounce(int count)
    {
        for (int i = 0; i < mCurMonsters.Count; i++)
        {
            if (prefab != null)
            {
                GameObject bouncePrefab =  NGUITools.AddChild(transform.parent.gameObject, prefab);
                bouncePrefab.transform.position = pos.transform.position;
                EffectBounce eb = bouncePrefab.AddMissingComponent<EffectBounce>();
                if (count == 0)
                {
                    eb.DestoryDelegate += moreHitDelegate;
                }
                eb.count = count;
                eb.Init(mCurSkillNode, mCurMonsters[i], attackerTrans, null);
                eb.distance = mCurSkillNode.aoe_wide;
            }
        }
    }

    private void moreHitDelegate(EffectBounce eb, GameObject target)
    {
        if (this != null)
        {
            eb.DestoryDelegate -= moreHitDelegate;
            mCurMonsters = GetTargetByCondition(CheckHitCondition, mCurSkillNode.aoe_wide, target.transform);
            mCurMonsters.Remove(mCurMonsters.Find(a => a.gameObject == target));
            GenerateBounce(1);
        }
    }

    void Update()
    {
        if (attackerCs != null && (attackerCs.isDie || (!GameLibrary.Instance().CanControlSwitch(attackerCs.pm) && isHitAction)))
        {
            Destroy(gameObject);
        }
    }
}
