using UnityEngine;

public class Player_MobaAI : BasePlayerAI
{
    public float fleeHpRate = 0.2f;
    public float defendHpRate = 0.5f;
    public float restoreHpRate = 0.6f;
    public float towerHpRate = 0.2f;
    float outBaseHpRate = 0.9f;

    SceneMoba1 mo;
    Vector3 currentDefendPos;
    Vector3 lastDefendPos;
    CharacterState myTower;
    CharacterState myCamp;
    CharacterState enemyTower;
    CharacterState enemyCamp;
    GameObject myHome;
    MobaAddLife mal;
    float myHomeRadius;
    bool stayinHome;
    float initDefendHp;

    protected override void OnStart ()
    {
        mo = SceneMoba1.instance;
        myTower = thisCs.groupIndex == 1 ? mo.TowerBlue : mo.TowerRed;
        myCamp = thisCs.groupIndex == 1 ? mo.CampBlue : mo.CampRed;
        enemyTower = thisCs.groupIndex == 1 ? mo.TowerRed : mo.TowerBlue;
        enemyCamp = thisCs.groupIndex == 1 ? mo.CampRed : mo.CampBlue;
        myHome = thisCs.groupIndex == 1 ? mo.HomeBlue : mo.HomeRed;
        myHomeRadius = myHome.GetComponent<SphereCollider>().radius;
        aiSkillHandler.SetSkills(thisCs.GetSkills());

        initDefendHp = defendHpRate;
        if(myTower != null)
            mal = myTower.GetComponentInChildren<MobaAddLife>();
    }

    protected override void OnFixedUpdate ()
    {
        if(!GameLibrary.Instance().CanControlSwitch(thisCs.pm))
        {
            thisCs.pm.Stop();
            return;
        }
        if(BattleUtil.ReachPos(transform.position, myHome.transform.position, myHomeRadius))
        {
            stayinHome = true;
            if(thisCs.currentHp > outBaseHpRate * thisCs.maxHp)
            {
                stayinHome = false;
                defendHpRate = initDefendHp;
            }
        }
        thisCs.SetAttackTargetTo(GetAttackTarget(thisCs.TargetRange));
        if(thisCs.currentHp < restoreHpRate * thisCs.maxHp)
        {
            aiSkillHandler.Restore();
        }

        if(thisCs.attackTarget != null) {
            if(BattleUtil.IsHeroTarget(thisCs.attackTarget) && ChaseHpRate(thisCs.attackTarget))
            {
                if(!aiSkillHandler.NormalAISkill(new SkillChaseComparer()))
                {
                    aiSkillHandler.NormalAttack();
                }
            }
            else if(ReachDefendHp())
            {
                if(!aiSkillHandler.NormalAISkill(new SkillDefendComparer(), (SkillNode node)=> {
                    return (node.IsTransfer() && node.target == TargetState.None) || node.IsProtect() || node.IsRestore() || node.IsControl() || node.IsMoveSpeed();
                }))
                {
                    PathFinding();
                }
            }
            else
            {
                if(!aiSkillHandler.NormalAISkill())
                {
                    aiSkillHandler.NormalAttack();
                }
            }
        }
        else
        {
            if(thisCs.currentHp < restoreHpRate * thisCs.maxHp)
            {
                if(!aiSkillHandler.NormalAISkill(new SkillDistComparer(), ( SkillNode node ) => { return node.IsRestore(); }))
                    PathFinding();
            }
            else
            {
                PathFinding();
            }
        }
    }

    protected override void PathFinding ()
    {
        currentDefendPos = GetDefendPos();
        if(BattleUtil.ReachPos(thisCs.transform.position, currentDefendPos, thisCs.pm.nav.stoppingDistance))
        {
            if(currentDefendPos != lastDefendPos)
            {
                // Debug.LogError(name + " Reach " + currentDefendPos);
                // thisCs.pm.Stop();
                lastDefendPos = currentDefendPos;
                if(defendHpRate > fleeHpRate && ReachDefendHp())
                {
                    defendHpRate -= 0.1f;
                    // Debug.LogError(name + " Change DefRate to " + defendHpRate);
                }
            }
            if(thisCs.attackTarget != null)
            {
                if(!aiSkillHandler.NormalAISkill())
                    aiSkillHandler.NormalAttack();
            }
            else
            {
                thisCs.pm.Stop();
            }
        }
        else
        {
            thisCs.pm.Approaching(currentDefendPos);
        }
    }

    #region Conditions
    public override CharacterState GetAttackTarget ( float radius = 2f )
    {
        CharacterState target = null;
        int hpMin = int.MaxValue;
        for(int i = 0; i < SceneBaseManager.instance.agents.size; i++)
        {
            CharacterState chs = SceneBaseManager.instance.agents[i];
            float dis = Vector3.Distance(thisCs.transform.position, chs.transform.position);
            if(BattleUtil.IsTargeted(thisCs, chs, radius))
            {
                if(BattleUtil.IsHeroTarget(chs))
                {
                    target = chs;
                    break;
                }
                if(chs.state == Modestatus.Tower && chs.currentHp < towerHpRate * chs.maxHp)
                {
                    target = chs;
                    break;
                }
                if(chs.currentHp < hpMin)
                {
                    hpMin = chs.currentHp;
                    target = chs;
                }
            }
        }
        return target;
    }

    Vector3 GetDefendPos ()
    {
        GameObject defendTarget = null;
        float offset = 0f;
        if(stayinHome)
        {
            defendTarget = myHome;
        }
        else
        {
            if(ReachDefendHp())
            {
                if(mal != null && mal.isShowing)
                {
                    defendTarget = mal.gameObject;
                }
                else if(ReachFleeHp())
                {
                    defendTarget = myHome;
                }
                else
                {
                    defendTarget = ( myTower != null && !myTower.isDie ) ? myTower.gameObject : ( myCamp != null && !myCamp.isDie ? myCamp.gameObject : null );
                    offset = 1.2f;
                }
            }
            else
            {
                if(enemyCamp != null && !enemyCamp.isDie)
                {
                    defendTarget = enemyCamp.gameObject;
                    offset = FriendMonsterBefore() && !AttackByTower() ? thisCs.AttackRange - 0.2f : enemyCamp.AttackRange + 0.5f;
                }
                if(enemyTower != null && !enemyTower.isDie)
                {
                    defendTarget = enemyTower.gameObject;
                    offset = FriendMonsterBefore() && !AttackByTower() ? thisCs.AttackRange - 0.2f : enemyTower.AttackRange + 0.5f;
                }
                if(mal != null && mal.isShowing && BattleUtil.ReachPos(mal.transform.position, transform.position, 1f) && thisCs.currentHp < 0.7f * thisCs.maxHp)
                {
                    defendTarget = mal.gameObject;
                }
            }
        }
        offset *= thisCs.groupIndex == 0 ? 1f : -1f;
        if(defendTarget == null)
            return Vector3.zero;
        return new Vector3(defendTarget.transform.position.x + offset, transform.position.y, defendTarget.transform.position.z);
    }

    bool AttackByTower ()
    {
        return IsAttackBy(enemyTower, thisCs) || IsAttackBy(enemyCamp, thisCs);
    }

    bool ReachDefendHp ()
    {
        return thisCs.currentHp < defendHpRate * thisCs.maxHp;
    }

    bool ReachFleeHp ()
    {
        return thisCs.currentHp < fleeHpRate * thisCs.maxHp;
    }

    bool IsAttackBy ( CharacterState attacker, CharacterState self )
    {
        return attacker != null && !attacker.isDie && attacker.attackTarget != null && attacker.attackTarget == self;
    }

    bool ChaseHpRate ( CharacterState targetHero)
    {
        return fleeHpRate + 1f * targetHero.currentHp / targetHero.maxHp < 1f * thisCs.currentHp / thisCs.maxHp;
    }

    bool FriendMonsterBefore ()
    {
        float offset = thisCs.groupIndex == 0 ? -1f : 1f;
        for(int i = 0; i < mo.agents.size; i++)
        {
            if(mo.agents[i].groupIndex == thisCs.groupIndex && ( mo.agents[i].state == Modestatus.Monster))
            {
                if(( thisCs.groupIndex == 0 && mo.agents[i].transform.position.x < thisCs.transform.position.x + offset ) || ( thisCs.groupIndex == 1 && mo.agents[i].transform.position.x > thisCs.transform.position.x + offset ))
                    return true;
            }
        }
        return false;
    }
    #endregion Conditions
}
