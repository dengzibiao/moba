using UnityEngine;
using System.Collections.Generic;

public class Player_AutoAI : BasePlayerAI
{
    public CharacterState protectionCs;
    List<GameObject> mAutoPoints = new List<GameObject>();

    protected override void OnStart()
    {
        base.OnStart();
        for (int i = 0; i < autoPoints.Length; i++)
        {
            mAutoPoints.Add(autoPoints[i]);
        }
    }

    Vector3 avoidPos;
    GameObject avoidPoint;
    protected override void OnFixedUpdate()
    {
        if (GameLibrary.isBossChuChang)
            return;
        if (!GameLibrary.Instance().CanControlSwitch(thisCs.pm))
        {
            thisCs.pm.Stop();
            return;
        }

        if (autoPoints.Length > 0)
            CheckAutos();
        targetCs = GetAttackTarget(thisCs.TargetRange);
        thisCs.SetAttackTargetTo(targetCs);
        if (isAttack && targetCs != null)
        {
            float targetRadius = targetCs.cc != null ? targetCs.CharData.attrNode.model_size * targetCs.cc.radius : 0.2f;
            float selfRadius = thisCs.cc != null ? thisCs.cc.radius : 0.2f;
            float minDis = targetRadius + selfRadius;

            if(BattleUtil.ReachPos(transform.position, targetCs.transform.position, minDis))
            {
                if(avoidPos == Vector3.zero)
                {
                    avoidPos = BattleUtil.GetCanNavPos(thisCs.pm.nav, transform.position + 2f * minDis * ( transform.position - targetCs.transform.position ).normalized);
                    //Debug.LogError("set pos to " + avoidPos);
                }
                //if(thisCs == CharacterManager.playerCS)
                //{
                //    if(avoidPoint == null)
                //    {
                //        avoidPoint = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //        avoidPoint.transform.localScale = 0.1f * Vector3.one;
                //    }
                //    avoidPoint.transform.position = avoidPos;
                //}
                //Debug.LogError("pos is " + avoidPos);
            }
            else
            {
                avoidPos = Vector3.zero;
                //Debug.LogError("set pos to 0");
            }

            if(avoidPos != Vector3.zero)
            {
                if(BattleUtil.ReachPos(transform.position, avoidPos, 0.1f))
                {
                    avoidPos = Vector3.zero;
                    //Debug.LogError("reach pos and reset to 0");
                }
                else
                {
                    thisCs.pm.Approaching(avoidPos, 0f);
                    //Debug.LogError("nav to " + avoidPos);
                }
            } else if(!aiSkillHandler.NormalAISkill())
            {
                aiSkillHandler.NormalAttack();
            }
        }
        else
        {
            if (null != protectionCs)
            {
                BullockCarts bc = protectionCs.GetComponent<BullockCarts>();
                if (bc == null)
                    return;
                Vector3 targetPos = Vector3.zero;
                if (bc.autoPoint != null)
                    targetPos = bc.autoPoint.transform.position + Vector3.forward * 0.2f;
                if (bc.autoPoint == null || BattleUtil.ReachPos(transform.position, targetPos, 0.1f))
                    thisCs.pm.Stop();
                else
                    thisCs.pm.Approaching(targetPos);
            }
            else
            {
                AutoNav();
            }
        }
    }

    GameObject nextAuto;
    void CheckAutos(float radius = 0.5f)
    {
        //寻找离你最远的怪物 如果有则去离最远的怪的路点 没有寻找离你最近的路点和下一个路点的距离选择下一个路点
        if (SceneBaseManager.instance.enemy.size > 0)
        {
            //选择最远的怪物点
            SceneBaseManager.instance.enemy.Sort((a, b) =>
            {
                float aDis = BattleUtil.V3ToV2Dis(a.transform.position, transform.position);
                float bDis = BattleUtil.V3ToV2Dis(b.transform.position, transform.position);
                return aDis - bDis > 0 ? -1 : (aDis == bDis ? 0 : 1);
            });
            CharacterState mFartestCs = SceneBaseManager.instance.enemy[0];
            //寻找离最远怪物的最近寻路点
            mAutoPoints.Sort((a, b) =>
            {
                if (a == null || b == null) return 0;
                float aDis = BattleUtil.V3ToV2Dis(a.transform.position, mFartestCs.transform.position);
                float bDis = BattleUtil.V3ToV2Dis(b.transform.position, mFartestCs.transform.position);
                return aDis - bDis > 0 ? 1 : (aDis == bDis ? 0 : -1);
            });
            nextAuto = mAutoPoints[0];
        }
        else
        {
            //寻找离你最近的寻路点，且有下一个寻路点，切到下一个寻路点
            mAutoPoints.Sort((a, b) =>
            {
                if (a == null || b == null) return 0;
                float aDis = BattleUtil.V3ToV2Dis(a.transform.position, transform.position);
                float bDis = BattleUtil.V3ToV2Dis(b.transform.position, transform.position);
                return aDis - bDis > 0 ? 1 : (aDis == bDis ? 0 : -1);
            });
            nextAuto = mAutoPoints[0];
            int mTempIndex = -1;
            for (int i = 0; i < autoPoints.Length; i++)
            {
                if (nextAuto == autoPoints[i] && i < autoPoints.Length - 1)
                {
                    mTempIndex = i;
                    break;
                }
            }
            if (mTempIndex != -1)
            {
                nextAuto = autoPoints[mTempIndex + 1];
            }
        }
    }

    void AutoNav()
    {
        if (nextAuto != null && !BattleUtil.ReachPos(transform.position, nextAuto.transform.position, thisCs.pm.nav.stoppingDistance))
            thisCs.pm.Approaching(nextAuto.transform.position);
        else
            thisCs.pm.Stop();
    }

    public override CharacterState GetAttackTarget(float radius = 2f)
    {
        if (SceneBaseManager.instance == null)
            return null;
        for (int i = 0; i < SceneBaseManager.instance.agents.size; i++)
        {
            CharacterState chs = SceneBaseManager.instance.agents[i];
            if (BattleUtil.IsTargeted(thisCs, chs, radius))
            {
                return chs;
            }
        }
        return null;
    }
}
