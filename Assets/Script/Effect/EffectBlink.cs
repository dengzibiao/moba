using UnityEngine;
using System.Collections.Generic;

public class EffectBlink : EffectTrackBase
{
    private Renderer[] renderList;
    private bool isChange;

    void Awake()
    {
        FindPosTrans(transform);
    }

    public override void Init(SkillNode skillNode, GameObject targetTrans, Transform thisTrans, VoidResult action = null)
    {
        base.Init(skillNode, targetTrans, thisTrans, action);
        if (attackerCs != null)
        {
            if (attackerCs.mCurMobalId == MobaObjectID.HeroHuanci && mCurSkillNode != null && mCurSkillNode.site == 1)
            {
                attackerCs.HitActionDelegate += HitAction;
            }
            attackerCs.BlinkDelegate += HandleBlink;
            renderList = attackerCs.GetSkins();
            SetRenderEnable(false);
            CDTimer.GetInstance().AddCD(0.4f, (a, b) =>
            {
                SetRenderEnable(true);
            });
        }
        Destroy(gameObject, destoryTime);
    }


    void HandleBlink()
    {
        if (attackerCs != null)
        {
            attackerCs.BlinkDelegate -= HandleBlink;
            if ((mCurSkillNode != null && (mCurSkillNode.target == 0 && mCurSkillNode.dist == 0)) || mHitTargetCs == null)
            {
                attackerCs.pm.FastMove(transform.forward.normalized * (mCurSkillNode == null ? 0 : mCurSkillNode.max_fly));
            }
            else
            {
                Vector3 dir = mHitTargetCs.transform.position - transform.position;
                attackerCs.pm.FastMove(dir - dir.normalized * 0.3f);
            }
            DetectionNavMesh();
        }
    }

    private void SetRenderEnable(bool b)
    {
        for (int i = 0; i < renderList.Length; i++)
        {
            if (renderList[i] != null)
            {
                renderList[i].gameObject.SetActive(b);
            }
        }
        isChange = b;
    }

    void OnDestroy()
    {
        if (!isChange)
        {
            SetRenderEnable(true);
        }
        if (attackerCs != null)
        {
            attackerCs.HitActionDelegate -= HitAction;
            attackerCs.BlinkDelegate -= HandleBlink;
        }
    }

    private void HitAction(long skillId)
    {
        if (attackerCs != null && skillId == mCurSkillNode.skill_id)
        {
            attackerCs.HitActionDelegate -= HitAction;
            if (this != null)
            {
                GetCommonDamageRange(pos);
                Hit(mCurMonsters);
            }
        }
    }

    void Update()
    {
        if (attackerCs != null || !attackerCs.isDie)
        {
            transform.position = attackerTrans.position;
        }
    }
}