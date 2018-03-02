using UnityEngine;
using System.Collections.Generic;

public class EffectBurrow : EffectTrackBase
{
    public AoeType aoeType = AoeType.CircleAoe;
    private float mDeltaTime;
    private float mEndTime = 0;

    public override void Init(SkillNode skillNode, GameObject targetTrans, Transform thisTrans, VoidResult action = null)
    {
        base.Init(skillNode, targetTrans, thisTrans, action);
        if (attackerCs != null)
        {
            pos = attackerTrans;
        }
        mEndTime = mCurSkillNode.max_fly / mCurSkillNode.flight_speed;
    }

    void Update()
    {
        mDeltaTime += Time.deltaTime;
        if (mCurSkillNode.effect_time != 0 && attackerCs != null && (attackerCs.isDie || !GameLibrary.Instance().CanControlSwitch(attackerCs.pm)))
        {
            //打断声音
            AudioController.Instance.StopEffectSound(attackerCs);
            DetectionNavMesh();
            Destroy(gameObject);
            return;
        }
        if (mDeltaTime >= destoryTime)
        {
            Destroy(gameObject);
            return;
        }
        if (mDeltaTime <= mEndTime)
        {
            attackerCs.pm.FastMove(transform.forward.normalized * (mCurSkillNode.max_fly / mEndTime) * Time.deltaTime);
            HandleDamage();
        }
        else
        {
            DetectionNavMesh();
        }
    }

    private void HandleDamage()
    {
        mCurMonsters.Clear();
        List<GameObject> mTempTargets = GetDamageTarget();
        for (int i = 0; i < mTempTargets.Count; i++)
        {
            if (!mAllMonsters.Contains(mTempTargets [i]))
            {
                mCurMonsters.Add(mTempTargets [i]);
                mAllMonsters.Add(mTempTargets[i]);
            }
        }
        Hit(mCurMonsters);
    }

    private List<GameObject> GetDamageTarget()
    {
        List<GameObject> resultGo = new List<GameObject>();
        switch (aoeType)
        {
            case AoeType.CircleAoe:
                resultGo = GetTargetByCondition(CheckHitCondition, distance, pos);
                break;
            case AoeType.RectAoe:
                resultGo = GetTargetByCondition(CheckHitCondition, distance, pos, AoeType.RectAoe);
                break;
            default:
                break;
        }
        return resultGo;
    }
}
