using UnityEngine;
using System.Collections;

/// <summary>
/// 单体远程特效
/// </summary>
public class EffectMonomer : EffectTrackBase
{
    bool hasTarget = false;     //是否有目标
    Collider col;
    void Awake()
    {
        FindPosTrans(transform);
    }

    void Start()
    {
        col = GetComponent<Collider>();
    }

    void Update()
    {
        if(hasTarget)
        {
            if (mHitTargetCs == null || mHitTargetCs.isDie)
            {
                Destroy(gameObject);
                return;
            }
            transform.LookAt(hit);
            transform.Translate(Vector3.forward * Time.deltaTime * mCurSkillNode.flight_speed);
            if (col == null)
            {
                mCurMonsters = GetTargetByCondition(CheckHitCondition, distance, pos);
                HitAction();
            }
        }
        else
        {
            if (mCurSkillNode == null)
            {
                Debug.LogError("技能数据为空", gameObject);
            }
            else
            {
                //目标为空，向前飞行
                transform.Translate(Vector3.forward * Time.deltaTime * mCurSkillNode.flight_speed);
            }
        }
    }

    public override bool CheckHitCondition(CharacterState cs)
    {
        return mHitTargetCs != null && cs.transform == mHitTargetCs.transform && base.CheckHitCondition(cs);
    }

    public override void Init(SkillNode skillNode, GameObject targetTrans, Transform thisTrans, VoidResult action)
    {
        base.Init(skillNode, targetTrans, thisTrans, action);
        hasTarget = targetTrans != null;
        if (!hasTarget)
        {
            Destroy(gameObject, mCurSkillNode.max_fly / mCurSkillNode.flight_speed);
        }
    }

    private void HitAction()
    {
        if (mCurMonsters.Count != 0)
        {
            Hit(mCurMonsters);
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider col)
    {
        HitTarget(col);
    }

    private void HitTarget(Collider col)
    {
        GameObject gameObject = CheckColiderByCondition(col, CheckHitCondition);
        if (gameObject != null)
        {
            mCurMonsters.Add(gameObject);
            HitAction();
        }
    }
}