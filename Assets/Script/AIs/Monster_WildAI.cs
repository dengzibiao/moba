using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Tianyu;
using UnityEngine.SceneManagement;

public class Monster_WildAI : BasePlayerAI
{
    public float chaseRange = 4f;
    Vector3 defaultPos;
    Quaternion defaultRot;
    bool restoreFlag;
    bool backFlag;

    protected override void OnStart () {
        base.OnStart();
        defaultPos = transform.position;
        defaultRot = transform.rotation;
        BetterList<CharacterState> mate = SceneMoba3.instance.wildMonster;
        for (int i = 0; i < mate.size; i++)
        {
            if (mate[i].groupIndex==thisCs.groupIndex && mate[i].CharData.monsterAreaId==thisCs.CharData.monsterAreaId)
            {
                mate[i].OnBeAttack += CheckChangeTarget;
            }
        }
        thisCs.OnBeAttack += CheckChangeTarget;
    }

    protected override void OnFixedUpdate ()
    {
        if (Time.frameCount % GameLibrary.mMonsterDelay != 0) return;
        base.OnFixedUpdate();
        if(backFlag)
        {
            targetCs = null;
            thisCs.SetAttackTargetTo(null);
            BackToPos();
        }
        else if(!BattleUtil.ReachPos(transform.position, defaultPos, thisCs.CharData.attrNode.chase_range))
        {
            backFlag = true;
        }
        else if(targetCs != null)
        {
            thisCs.SetAttackTargetTo(targetCs);
            if(!aiSkillHandler.NormalAISkill())
                aiSkillHandler.NormalAttack();
        }
        else
        {
            BackToPos();
        }
    }

    void BackToPos()
    {
        if (BattleUtil.ReachPos(transform.position, defaultPos, thisCs.pm.nav.stoppingDistance))
        {
            transform.position = defaultPos;
            transform.rotation = defaultRot;
            thisCs.moveSpeed = thisCs.moveInitSpeed;
            thisCs.Invincible = false;
            restoreFlag = false;
            backFlag = false;
            thisCs.pm.Stop();
        }
        else
        {
            thisCs.pm.Move(defaultPos);
            Restore();
        }
    }

    void Restore()
    {
        thisCs.Invincible = true;
        thisCs.moveSpeed = thisCs.moveInitSpeed * 2;
        if (!restoreFlag)
        {
            restoreFlag = true;
            CDTimer.CD cd = CDTimer.GetInstance().AddCD(0.1f, (int count, long cid) =>
            {
                if (thisCs.currentHp < thisCs.maxHp)
                    thisCs.Hp(-(int)(thisCs.maxHp * 0.1f), HUDType.Cure);
            }, 10);
        }
    }
}
