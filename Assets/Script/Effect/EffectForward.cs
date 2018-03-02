using UnityEngine;
using System.Collections.Generic;

public class EffectForward : EffectTrackBase
{
    public bool isContinue = false;
    private float mCurDeltaTime = 0f;
    private bool isReachTarget = false;
    private bool mIgnoreTerrain = true;

    void Awake()
    {
        FindPosTrans(transform);
    }

    public override void Init(SkillNode skillNode, GameObject targetTrans, Transform thisTrans, VoidResult action)
    {
        base.Init(skillNode, targetTrans, thisTrans, action);
        isContinue = mCurSkillNode.interval_time != null && mCurSkillNode.interval_time.Length > 0;
        if (isContinue)
        {
            int count = mCurSkillNode.interval_time.Length;
            for (int i = 0; i < count; i++)
            {
                CDTimer.GetInstance().AddCD(mCurSkillNode.interval_time[i], HitTarget);
            }
        }
        isReachTarget = false;
        mIgnoreTerrain = mCurSkillNode.ignoreTerrain;
        if (mCurSkillNode.target != TargetState.Need)
        {
            Destroy(gameObject, efficiency_time);
        }
        else
        {
            if (attackerCs != null)
            {
                if (attackerCs.mCurMobalId == MobaObjectID.HeroLuosa && mCurSkillNode.site == 1)
                {
                    attackerCs.pm.ReachTarget(false);
                }
                attackerCs.pm.canMoveSwitch++;
                attackerCs.pm.canAttackSwitch++;
                attackerCs.pm.canSkillSwitch++;
            }
        }
    }

    private void HitTarget(int count, long id)
    {
        if (this != null)
        {
            mAllMonsters.Clear();
            GetCommonDamageRange(pos);
            Hit(mCurMonsters);
        }
    }

    void Update()
    {
        if (attackerCs != null && (attackerCs.isDie || !GameLibrary.Instance().CanControlSwitch(attackerCs.pm)))
        {
            //打断声音
            AudioController.Instance.StopEffectSound(attackerCs);
            attackerCs.pm.ReachTarget(true);
            Destroy(gameObject);
            return;
        }
        if (mCurSkillNode.target == TargetState.Need)
        {
            if (transform != null && attackerTrans != null)
                transform.position = attackerTrans.position;
            if (mHitTargetCs == null)
            {
                attackerCs.pm.ReachTarget(true);
                Destroy(gameObject);
                return;
            }
            if (!isReachTarget)
            {
                attackerCs.transform.LookAt(mHitTargetCs.transform);
                if (Vector3.Distance(attackerCs.transform.position, mHitTargetCs.transform.position) <= GetTargetRadius(mHitTargetCs))
                {
                    if (attackerCs.mCurMobalId == MobaObjectID.HeroLuosa && mCurSkillNode.site == 1)
                    {
                        SkillBuff buff = SkillBuffManager.GetInst().GetSkillBuffListByCs(attackerCs).Find
                            (a => (SkillBuffType)a.id == SkillBuffType.SkillTalent && a.attacker == attackerCs);
                        string idIndex = buff != null ? "_2_2" : "_1_2";
                        GameObject effect_go = BattleUtil.AddEffectTo(attackerCs.emission.GetEffectResourceRoot() + "skill" + mCurSkillNode.site + idIndex, transform);
                        attackerCs.pm.ReachTarget(true);
                        if (effect_go != null)
                        {
                            effect_go.transform.parent = attackerCs.emission.nip.transform;
                            effect_go.gameObject.SetActive(true);
                            Destroy(effect_go, 2f);
                        }
                    }
                    mCurMonsters.Clear();
                    mCurMonsters.Add(mHitTargetCs.gameObject);
                    Hit(mCurMonsters);
                    isReachTarget = true;
                    Destroy(gameObject);
                    return;
                }
                Vector3 dir = mHitTargetCs.transform.position - attackerCs.transform.position;
                attackerCs.pm.FastMove(Time.deltaTime * mCurSkillNode.flight_speed * dir.normalized, mIgnoreTerrain);
            }
        }
        else
        {
            mCurDeltaTime += Time.deltaTime;
            if (attackerTrans != null && mCurDeltaTime <= destoryTime)
            {
                if (transform != null && attackerTrans != null)
                    transform.position = attackerTrans.position;
                if (attackerCs != null && attackerCs.pm != null)
                {
                    attackerCs.pm.FastMove(transform.forward * Time.deltaTime * mCurSkillNode.flight_speed, mIgnoreTerrain);
                }
                if (!isContinue)
                {
                    GetCommonDamageRange(pos);
                    Hit(mCurMonsters);
                }
                if (attackerCs.mCurMobalId == MobaObjectID.HeroXiongmao && mCurSkillNode.site == 2)
                {
                    for (int i = 0; i < mCurMonsters.Count; i++)
                    {
                        CharacterState mCurTargetCs = mCurMonsters[i].GetComponent<CharacterState>();
                        if (mCurTargetCs != null && !mCurTargetCs.isDie && !mCurTargetCs.Invincible && mCurTargetCs.RecieveControl && !GameLibrary.Instance().CheckNotHeroBoss(mCurTargetCs))
                        {
                            mCurMonsters[i].AddMissingComponent<HitReAction>().Init(mCurTargetCs, (mCurSkillNode.max_fly - mCurSkillNode.flight_speed * mCurDeltaTime) / mCurSkillNode.flight_speed,
                                attackerCs.transform.forward, mCurSkillNode.flight_speed);
                        }
                    }
                }
            }
            else
            {
                DetectionNavMesh();
            }
        }
    }

    void OnDestroy()
    {
        if (attackerCs != null && mCurSkillNode.target == TargetState.Need)
        {
            attackerCs.pm.canMoveSwitch--;
            attackerCs.pm.canAttackSwitch--;
            attackerCs.pm.canSkillSwitch--;
            if (!(attackerCs.mCurMobalId == MobaObjectID.HeroLuosa && mCurSkillNode.site == 1))
            {
                attackerCs.pm.ForceChangePrepare();
            }
            DetectionNavMesh();
        }
    }
}
