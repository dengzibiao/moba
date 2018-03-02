using UnityEngine;
using System.Collections.Generic;

public class EffectCircle : EffectAOECenter
{
    private float delayTime = 0.5f;
    private float mCurDeltaTime = 0f;
    private float mPartTime = 0f;
    public override void Play()
    {
        base.Play();
        if (attackerCs != null && !attackerCs.isDie && 
            (attackerCs.mCurMobalId == MobaObjectID.HeroHuonv && mCurSkillNode.site == 1) ||
            (attackerCs.mCurMobalId == MobaObjectID.HeroMeidusha && mCurSkillNode.site == 3) ||
            (attackerCs.GetMobaName() == "boss_003" && mCurSkillNode.site == 2))
        {
            attackerCs.OnShieldOff += DestoryShieldOff;
        }
    }

    private void DestoryShieldOff(CharacterState cs)
    {
        if (attackerCs != null && !attackerCs.isDie)
        {
            attackerCs.OnShieldOff -= DestoryShieldOff;
            if (this != null)
            {
                if (attackerCs.MagicShields != 0)
                {
                    //播放爆炸特效
                    Bomb(pos);
                    GetCommonDamageRange(pos);
                    Hit(mCurMonsters);
                }
                else
                {
                    //播放击碎特效
                    Bomb(pos, "hit1");
                }
                Destroy(gameObject);
            }
        }
    }

    void Awake()
    {
        FindPosTrans(transform);
    }

    public override void Update()
    {
        base.Update();
        if (attackerCs != null && !attackerCs.isDie)
        {
            if (attackerCs.mCurMobalId == MobaObjectID.HeroXiongmao && mCurSkillNode.site == 1)
            {
                if (!isHitAction)
                {
                    transform.rotation = attackerTrans.rotation;
                    transform.position = attackerTrans.position;
                }
            }
            else
            {
                transform.position = attackerTrans.position;
            }
        }
        mCurDeltaTime += Time.deltaTime;
        if (attackerCs != null && attackerCs.mCurMobalId == MobaObjectID.HeroJumo)
        {
            //TODO需要添加位移判断
            AnimatorStateInfo asInfo = attackerCs.pm.ani.GetCurrentAnimatorStateInfo(0);
            if (asInfo.fullPathHash == attackerCs.pm.Skill1_Hash)
            {
                if (mCurDeltaTime <= delayTime)
                {
                    if (mHitTargetCs != null)
                    {
                        Vector3 dir = mHitTargetCs.transform.position - attackerCs.transform.position;
                        attackerCs.pm.FastMove((dir - dir.normalized * GetTargetRadius(mHitTargetCs))
                            * Time.deltaTime * (mCurSkillNode.dist / delayTime));
                    }
                    else
                    {
                        attackerCs.pm.FastMove(attackerCs.transform.forward.normalized * Time.deltaTime * (mCurSkillNode.dist / delayTime));
                    }
                }
                else
                {
                    DetectionNavMesh();
                }
            }
        }
    }


    public override float GetTargetRadius(CharacterState target)
    {
        float result = 0;
        if (target != null)
        {
            UnityEngine.AI.NavMeshAgent nav = target.GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (nav != null)
            {
                result = nav.height;
            }
            else
            {
                CapsuleCollider cp = target.GetComponent<CapsuleCollider>();
                result = cp == null ? result : cp.radius;
            }
        }
        return result;
    }
}
