using UnityEngine;
using System.Collections;

public class EffectStraightFly : EffectTrackBase
{
    private float mDeltaTime;

    void Awake()
    {
        FindPosTrans(transform);
    }

    void Update()
    {
        mDeltaTime += Time.deltaTime;
        if (mDeltaTime < destoryTime)
        {
            transform.Translate(Vector3.forward * Time.deltaTime * mCurSkillNode.flight_speed);
            GetCommonDamageRange(pos);
            if (mCurMonsters.Count > 0)
            {
                if (mCurSkillNode.range_type == rangeType.spurting)
                {
                    CharacterState mCurPos = mCurMonsters[0].GetComponent<CharacterState>();
                    Bomb(mCurPos.mHitPoint);
                    mAllMonsters.Clear();
                    GetCommonDamageRange(mCurPos.mHitPoint);
                    Hit(mCurMonsters);
                }
                else
                {
                    Hit(mCurMonsters);
                }
                if (!mCurSkillNode.isPierce)
                {
                    Destroy(gameObject);
                }
            }
        }
    }

    public override void Init(SkillNode skillNode, GameObject targetTrans, Transform thisTrans, VoidResult action)
    {
        base.Init(skillNode, targetTrans, thisTrans, action);
        mDeltaTime = 0;
        Destroy(gameObject, efficiency_time);
    }
}