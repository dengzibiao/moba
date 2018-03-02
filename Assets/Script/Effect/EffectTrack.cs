using UnityEngine;
using System.Collections.Generic;
public class EffectTrack : EffectTrackBase
{
    private Vector3 mOriginPos;
    private CharacterState mAttackerCs;

    public override void Init(SkillNode skillNode, GameObject targetTrans, Transform thisTrans, VoidResult action)
    {
        base.Init(skillNode, targetTrans, thisTrans, action);
        if (targetTrans == null)
        {
            Destroy(gameObject, destoryTime);
        }
        mOriginPos = transform.position;
        if (attackerCs != null)
        {
            mAttackerCs = attackerCs.state == Modestatus.SummonHero ? attackerCs.Master : attackerCs;
        }
    }

    void Awake()
    {
        FindPosTrans(transform);
    }

    void Update()
    {
        if (mHitTargetCs == null || mHitTargetCs.isDie)
        {
            Destroy(gameObject);
            return;
        }
        if (mHitTargetCs != null)
        {
            transform.LookAt(hit);
            transform.Translate(Vector3.forward * Time.deltaTime * mCurSkillNode.flight_speed);
            if (mCurSkillNode.range_type == rangeType.spurting)
            {
                if (Vector3.Distance(transform.position, hit.position) <= 0.1f)
                {
                    Bomb(mHitTargetCs.transform);
                    if (mHitTargetCs.state == Modestatus.Tower)
                    {
                        Destroy(gameObject);
                    }
                    else
                    {
                        HitDamage();
                    }
                }
            }
            else
            {
                HitDamage();
            }
        }
    }

    private void HitDamage()
    {
        mCurMonsters = GetTargetByCondition(CheckHitCondition, distance, pos);
        if (mCurMonsters.Count != 0)
        {
            Hit(mCurMonsters);
            if (attackerCs.mCurMobalId == MobaObjectID.HeroShenling && mAttackerCs != null && !mAttackerCs.isDie)
            {
                ComeBack();
            }
            Destroy(gameObject);
        }
    }

    public void ComeBack()
    {
        string id = "skill" + mCurSkillNode.site + "_1";
        GameObject comeObj = BattleUtil.AddEffectTo(mResourceRoot + id, mAttackerCs.mHitPoint);
        if (comeObj != null)
        {
            comeObj.gameObject.SetActive(true);
            comeObj.transform.parent = transform.parent;
            comeObj.transform.position = mAttackerCs.mHitPoint.position;
            comeObj.transform.rotation = transform.rotation;
            comeObj.AddMissingComponent<EffectFollowTarget>().init(mAttackerCs, 1.0f);
        }
    }

    public override void Hit(List<GameObject> monsters)
    {
        if (mCurSkillNode.range_type == rangeType.boost)
        {
            CharacterData mTempData = characterData.Clone();
            float mDistance = Vector3.Distance(transform.position, mOriginPos);
            mTempData.skill_Damage[0] += (mDistance >= mCurSkillNode.max_fly ? mCurSkillNode.max_fly : mDistance) * mTempData.skill_Damage[0] * 0.2f;
            for (int i = 0; i < monsters.Count; i++)
            {
                CharacterState cs = monsters[i].GetComponent<CharacterState>();
                cs.HitBy(mCurSkillNode, attackerCs, mTempData);
            }
        }
        else
        {
            base.Hit(monsters);
        }
    }
}
