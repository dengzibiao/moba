
using System.Collections.Generic;
using UnityEngine;

public enum ActionType
{
    idle=-1,
    RunHome=0,
    RunToMyTower=1,
    RunToEnemyTower=2,
    Tp=3,
    Skill=4,
    AttackInEnemyTower=5,
    AttackAroundEnemyTowers=6,
    ChaseAttack=7,
    AttackNormal=8,
    StayInHome=9,
    Summon=10,
    Patrol=11,
    Defend=12,
    DefendCamp=13,
    AttackWildMonster=14,
}

public enum AiLevel
{
    Low=1,
    Mid=2,
    High=3,
}

public class AIPve3V3 : BasePlayerAI
{
    public float outBaseHpRate = 0.9f;
    public float fleeHpRate = 0.3f;
    public float defendHpRate = 0.5f;

    float moralPoint = 0;   
    SceneMoba3 mo;   
    CharacterState myCamp;
    CharacterState enemyCamp;
    GameObject myHome;
    float myHomeRadius;    
    List<CharacterState> enemyTowersLeft = new List<CharacterState>();
    List<CharacterState> enemyTowersRight = new List<CharacterState>();
    List<CharacterState> myTowersLeft = new List<CharacterState>();
    List<CharacterState> myTowersRight = new List<CharacterState>();
    List<CharacterState> wildMonster = new List<CharacterState>();
    public bool isBlueLeft = false;
    public bool isRedLeft = false;    
    public delegate void OnAction(ActionType type);
    public event OnAction OnPlayerAction;    
    public ActionType state = ActionType.idle;
    float towerAtkRange = 3f;
    List<CharacterState> enemyTowers = new List<CharacterState>();
    public bool isInTowerAtkRange { 
       get {
            enemyTowers.Clear();
            enemyTowers.AddRange(enemyTowersRight);
            enemyTowers.AddRange(enemyTowersLeft);
            for(int i = 0; i< enemyTowers.Count; i++)
            {
                if(BattleUtil.ReachPos(thisCs, enemyTowers[i], enemyTowers[i].AttackRange))
                    return true;
            }
            return false;
        }
    }
    private AiLevel aiLv = AiLevel.Low;
    bool AtkMonsterFlag = false;

    protected override void OnStart()
    {
        mo = SceneMoba3.instance;
        if (mo==null)
        {
            return;
        }
        aiLv = mo.aiLv;
        myCamp = thisCs.groupIndex == 1 ? mo.CampBlue : mo.CampRed;
        enemyCamp = thisCs.groupIndex == 1 ? mo.CampRed : mo.CampBlue;
        myHome = thisCs.groupIndex == 1 ? mo.HomeBlue : mo.HomeRed;
        InitTowers();       
        myHomeRadius = myHome.GetComponent<SphereCollider>().radius;
        aiSkillHandler.SetSkills(thisCs.GetSkills());
        OnPlayerAction += this.PlayerAction;
    }
    void InitTowers()
    {
        if (thisCs.atkLine == HeroAttackLine.leftLine)
        {
            myTowersLeft = thisCs.groupIndex == 1 ? mo.towersBlueLeft : mo.towersRedLeft;
            enemyTowersLeft = thisCs.groupIndex == 1 ? mo.towersRedLeft : mo.towersBlueLeft;
            
        }
        else if (thisCs.atkLine == HeroAttackLine.RightLine)
        {
            myTowersRight = thisCs.groupIndex == 1 ? mo.towersBlueRight : mo.towersRedRight;
            enemyTowersRight = thisCs.groupIndex == 1 ? mo.towersRedRight : mo.towersBlueRight;
        }
        for (int i = 0; i < 2; i++)
        {
            if (mo.towersRedLeft[i]!=null)
            {
                mo.towersRedLeft[i].GetComponent<TowerState>().SetPatrolPoint(new Vector3(-2, 0, 0));
            }
            if (mo.towersRedRight[i]!=null)
            {
                mo.towersRedRight[i].GetComponent<TowerState>().SetPatrolPoint(new Vector3(-2, 0, -2));
            }
            if (mo.towersBlueLeft[i]!=null)
            {
                mo.towersBlueLeft[i].GetComponent<TowerState>().SetPatrolPoint(new Vector3(-2.3f, 0, -0.5f));
            }
            if (mo.towersBlueRight[i]!=null)
            {
                mo.towersBlueRight[i].GetComponent<TowerState>().SetPatrolPoint(new Vector3(-2, 0, -1));
            }
            
        }
        if (myCamp!=null)
        {
            myCamp.GetComponent<TowerState>().SetPatrolPoint(new Vector3(0.5f,0,1.5f));
        }
        if (enemyCamp!=null)
        {
            enemyCamp.GetComponent<TowerState>().SetPatrolPoint(new Vector3(-0.5f, 0, -2f));
        }
    }
    void PlayerAction(ActionType type)
    {

        switch (type)
        {
            case ActionType.idle:
                break;
            case ActionType.RunHome:
                this.ActionRunHome();
                break;
            case ActionType.RunToMyTower:
                this.ActionRuntoMyTower();
                break;
            case ActionType.RunToEnemyTower:
                this.RuntoEnemyTowers();
                break;
            case ActionType.Tp:
                break;
            case ActionType.Skill:
                break;
            case ActionType.AttackInEnemyTower:
                this.ActionAtkInEnemyTower();
                break;
            case ActionType.AttackAroundEnemyTowers:
                break;
            case ActionType.AttackNormal:
                this.NormalAttack();
                break;
            case ActionType.StayInHome:
                this.ActionStayInHome();
                break;
            case ActionType.Summon:
                this.SummonHero();
                break;
            case ActionType.Patrol:
                this.ActionPatrol();
                break;
            case ActionType.Defend:
                this.ActionDefend();
                break;
            //case ActionType.AttackWildMonster:
            //    this.RunToAttackWildMonster();
            //    break;
            default:
                break;
        }
    }
    float timer = 0;
    protected override void OnFixedUpdate ()
    {
        base.OnFixedUpdate();
        timer += Time.deltaTime;
        if (Time.frameCount % GameLibrary.mPlayDelay != 0) return;
        //if (Time.frameCount % 2== 0 &&thisCs.groupIndex==0)
        //    return;
        //if (Time.frameCount % 2 == 1 && thisCs.groupIndex == 1)
        //{
        //    return;
        //}
        
        if (thisCs==null)
        {
            return;
        }
        if (SceneMoba3.instance.Block.activeInHierarchy)
        {
            thisCs.pm.Stop();
            return;
        }
        else
        {
            if (timer < 10 && thisCs.groupIndex == 0)
            {
                thisCs.pm.Stop();
                return;
            }
        }
        targetCs = GetAttackTarget(thisCs.TargetRange);
        thisCs.SetAttackTargetTo(targetCs);
        //if (IsAtkMonsterState())
        //{
        //    state = ActionType.AttackWildMonster;
        //}
        //else

        {
            if (IsDefendState())
            {
                state = ActionType.Defend;
            }
            else if (IsRunHomeState())
                {
                    state = ActionType.RunHome;
                }

                else if (IsRuntoEnemyTower())
                {
                    state = ActionType.RunToEnemyTower;
                }
                else if (HaveTarget())
                {
                    state = ActionType.AttackNormal;
                }

                else if (IsStayInHome())
                {
                    state = ActionType.StayInHome;
                }
                //else if (IsSummonState())
                //{
                //    state = ActionType.Summon;
                //}
                
                
                else if (isAtkInEnemyTower())
                {
                    state = ActionType.AttackInEnemyTower;

                }
                else
                {
                    state = ActionType.RunToEnemyTower;
                }
            
           
        }
       
       
        
        OnPlayerAction(state);

    }
    #region Actions

    bool isVirgin = true;
    void RuntoEnemyTowers()
    {
        CharacterState tower = this.GetEnemyTower();
        CharacterState mytower = this.GetMyTowers()[1];
        if (isVirgin && mytower!=null)
        {
            if (!BattleUtil.ReachPos(thisCs, mytower, 0.6f))
            {
                thisCs.pm.Approaching(mytower.transform.position, 0.4f);
            }
            else
            {
                isVirgin = false;
            }
           
        }
        else
        {
            if (tower != null)
            {
                thisCs.pm.Approaching(GetPatrolPos(AiLevel.Low));
            }
        }     
       

    }

    Vector3 GetPatrolPos(AiLevel sb)
    {
        Vector3 pos = Vector3.zero;
        var tower = GetEnemyTower();
        if (tower!=null)
        {
            switch (sb)
            {
                case AiLevel.Low:
                    pos = tower.transform.TransformPoint((tower.GetComponent<TowerState>().patrolPoint.transform.localPosition).normalized *0.5f);
                    break;
                case AiLevel.Mid:
                    break;
                case AiLevel.High:
                    pos = tower.transform.TransformPoint((tower.GetComponent<TowerState>().patrolPoint.transform.localPosition).normalized * (tower.AttackRange + 0.5f));
                    break;
                default:
                    break;
            }
           
        }
        return pos;
    }
   
    void NormalAttack()
    {
        if (targetCs != null &&targetCs.CharData.monsterAreaId==0)
        {
            if (targetCs.state == Modestatus.Tower)
            {
                AttackTower();
            }
            else 
            {
                thisCs.SetAttackTargetTo(targetCs);
                if (!aiSkillHandler.NormalAISkill())
                    aiSkillHandler.NormalAttack();
            }
           
           
        }
       
    }

    void AttackWildMonster()
    {
        if (targetCs != null)
        {
            thisCs.SetAttackTargetTo(targetCs);
            if (!aiSkillHandler.NormalAISkill())
                aiSkillHandler.NormalAttack();
        }
    }

    void ActionStayInHome ()
    {
        //float t = 0;
        //t += Time.deltaTime;
        thisCs.pm.Stop();
        //if(t > 1)
        //{

        //    thisCs.currentHp += 10;
        //    t = 0;
        //}

    }

    //Todo patrol bug 
    void ActionPatrol()
    {
        CharacterState mytower = GetMyTower();
        CharacterState enemytower = GetEnemyTower();
        if (mytower!=null)
        {

            if (BattleUtil.ReachPos(thisCs.transform.position, mytower.transform.position, 1.1f))
            {
                if (enemytower != null)
                {
                    thisCs.ApproachTo(enemytower.transform.position, 0.3f);
                }
            }
            else
            {
                thisCs.ApproachTo(mytower.transform.position, 1f);
            }
        }       
        
    }
    
    void ActionDefend()
    {
       // Debug.Log(thisCs.groupIndex+thisCs.CharData.memberId);
        CharacterState myTower = GetMyTower();
        aiSkillHandler.Restore();
        if (myTower!=null)
        {
            if (myTower.AddHp != null && myTower.addLife.isShowing)
            {
                thisCs.pm.Approaching(myTower.AddHp.transform.position);

            }
            else
            {
                thisCs.pm.Approaching(myHome.transform.position);
            }
            
        } 
                
    }

    HeroData GetMyHeroData()
    {
        HeroData data = null;
        if (mo!=null)
        {
            data = mo.herosDataBlue[0];
        }
        return data;
    }
    void ActionRunHome()
    {
        if (myHome==null)
        {
            return;
        }
        aiSkillHandler.Restore();
        thisCs.pm.Approaching(myHome.transform.position);
        if (targetCs!=null && targetCs.groupIndex!=99 && targetCs.groupIndex!=100)
        {
            thisCs.SetAttackTargetTo(targetCs);
            aiSkillHandler.ControllSkill();
        }
    }

    void ActionRuntoMyTower()
    {
        CharacterState tower = this.GetMyTower();
        if (tower != null)
        {
            thisCs.pm.Approaching(tower.transform.position);
        }
    }
    void AttackTower()
    {
        var tower = this.GetEnemyTower();

        if (tower != null)
        {
            this.aiSkillHandler.NormalAttack(tower);
        }
    }

    void SummonHero()
    {
        if (this.aiSkillHandler!=null)
        {
            //thisCs.pm.Skill(5);
           // Debug.LogError("do summon");        
        }
    }

    
   
    void ActionAtkInEnemyTower()
    {
        if (thisCs==null)
        {
            return;
        }
        //Debug.Log(string.Format("id:{0}/member:{1}", id, memberId));
        BetterList<CharacterState> enemyAroundTower = null;
        var tower = GetEnemyTower();
      
        int heroCnt = 0;
        int monsterCnt = 0;
       enemyAroundTower = SceneBaseManager.instance.enemy;
        if (tower==null)
        {
            return;
        }
        for (int i = 0; i < enemyAroundTower.size; i++)
        {
            if (enemyAroundTower[i].state ==  Modestatus.NpcPlayer)
            {
                if (BattleUtil.ReachPos(enemyAroundTower[i], tower, tower.AttackRange) && enemyAroundTower[i].state == Modestatus.NpcPlayer)
                {
                    heroCnt++;
                }
            }
            
        }
        if (heroCnt>1)
        {
            if (tower.currentHp < 100)
            {
                this.AttackTower();
            }
            else
            {
                if (myCamp!=null)
                {
                    thisCs.pm.Approaching(myCamp.transform.position);
                }
               
            }
        }
        else
        {
            for (int i = 0; i < enemyAroundTower.size; i++)
            {
                if (enemyAroundTower[i].state == Modestatus.Monster)
                {
                    if (BattleUtil.ReachPos(enemyAroundTower[i], tower, tower.AttackRange) && enemyAroundTower[i].state == Modestatus.Monster)
                    {
                        monsterCnt++;
                    }
                }
               
            }
            if (monsterCnt>0)
            {
                AttackMonster(tower);
            }

            else
            {
                this.AttackTower();
            }
            
        }

    }
    void RunToAttackWildMonster()
    {
        
        if (thisCs.groupIndex==1)
        {           
            int area1 = mo.wildMonster1.size;
            int area2= mo.wildMonster2.size;
            int area3= mo.wildMonster3.size;
            if (area1 > 0)
            {
                if (!BattleUtil.ReachPos(thisCs.transform.position, mo.wildMonster1[0].transform.position, 1f))
                {
                    thisCs.pm.Approaching(mo.wildMonster1[0].transform.position);
                }
                else
                {
                    this.AttackWildMonster();
                }
            }
            else if (area2 > 0)
            {
                if (!BattleUtil.ReachPos(thisCs.transform.position, mo.wildMonster2[0].transform.position, 1f))
                {
                    thisCs.pm.Approaching(mo.wildMonster2[0].transform.position);
                }
                else
                {
                    this.AttackWildMonster();
                }
            }
            else
            {
                if (!BattleUtil.ReachPos(thisCs.transform.position, mo.wildMonster3[0].transform.position, 1f))
                {
                    thisCs.pm.Approaching(mo.wildMonster3[0].transform.position);
                }
                else
                {
                    this.AttackWildMonster();
                }
            }
           
          
        }
        else if (thisCs.groupIndex==0)
        {
            int area1 = mo.wildRed1.size;
            int area2 = mo.wildRed2.size;
            int area3 = mo.wildRed3.size;
            if (area1 > 0)
            {
                if (!BattleUtil.ReachPos(thisCs.transform.position, mo.wildRed1[0].transform.position, 1f))
                {
                    thisCs.pm.Approaching(mo.wildRed1[0].transform.position);
                }
                else
                {
                    this.AttackWildMonster();
                }
            }
            else if (area2 > 0)
            {
                if (!BattleUtil.ReachPos(thisCs.transform.position, mo.wildRed2[0].transform.position, 1f))
                {
                    thisCs.pm.Approaching(mo.wildRed2[0].transform.position);
                }
                else
                {
                    this.AttackWildMonster();
                }
            }
            else
            {
                if (!BattleUtil.ReachPos(thisCs.transform.position, mo.wildRed3[0].transform.position, 1f))
                {
                    thisCs.pm.Approaching(mo.wildRed3[0].transform.position);
                }
                else
                {
                    this.AttackWildMonster();
                }
            }
        }
      
    }
    void AttackMonster(CharacterState targetTower)
    {
        CharacterState tower = GetEnemyTower();
        List<CharacterState> monster = new List<CharacterState>();
        if (tower==null)
        {
            return;
        }
        if (targetTower!=null)
        {
            var monsterList = SceneBaseManager.instance.enemy;
            if (monster!=null)
            {
                for (int i = 0; i < monsterList.size; i++)
                {
                    if (monsterList[i].state== Modestatus.Monster)
                    {
                        if (BattleUtil.ReachPos(monsterList[i],tower,tower.AttackRange))
                        {
                            monster.Add(monsterList[i]);
                        }
                    }
                }
            }
            if (monster!=null)
            {
                aiSkillHandler.NormalAttack(monster[0]);
            }
            
        }
    }
    #endregion

    
    CharacterState GetEnemyTower()
    {
        CharacterState cs = null;

        if (thisCs.atkLine == HeroAttackLine.leftLine)
        {
            if (enemyTowersLeft != null)
            {
                if (enemyTowersLeft[0] != null && !enemyTowersLeft[0].isDie)
                {
                    cs = enemyTowersLeft[0];
                }
                else
                {
                    if (enemyTowersLeft[1] != null && !enemyTowersLeft[1].isDie)
                    {
                        cs = enemyTowersLeft[1];
                    }
                    else

                    {
                        cs = enemyCamp;
                    }
                   
                }

            }
            else
            {
                cs = enemyCamp;
            }

        }
        else if (thisCs.atkLine == HeroAttackLine.RightLine)
        {
            if (enemyTowersRight != null)
            {
                if (enemyTowersRight[0] != null && !enemyTowersRight[0].isDie)
                {
                    cs = enemyTowersRight[0];
                }
                else
                {
                    if (enemyTowersRight[1] != null && !enemyTowersRight[1].isDie)
                    {
                        cs = enemyTowersRight[1];
                    }
                    else

                    {
                        cs = enemyCamp;
                    }

                }

            }
            else
            {
                cs = enemyCamp;
            }

        }
        return cs;
    }

    CharacterState GetMyTower()
    {
        CharacterState cs = null;

        if (thisCs.atkLine == HeroAttackLine.leftLine)
        {
            if (myTowersLeft != null)
            {
                if (myTowersLeft[0] != null && !myTowersLeft[0].isDie)
                {
                    cs = myTowersLeft[0];
                }
                else
                {
                    if (myTowersLeft[1] != null && !myTowersLeft[1].isDie)
                    {
                        cs = myTowersLeft[1];
                    }
                    else

                    {
                        cs = myCamp;
                    }

                }

            }
            else
            {
                cs = myCamp;
            }

        }
        else if (thisCs.atkLine == HeroAttackLine.RightLine)
        {
            if (enemyTowersRight != null)
            {
                if (myTowersRight[0] != null && !myTowersRight[0].isDie)
                {
                    cs = myTowersRight[0];
                }
                else
                {
                    if (myTowersRight[1] != null && !myTowersRight[1].isDie)
                    {
                        cs = myTowersRight[1];
                    }
                    else

                    {
                        cs = myCamp;
                    }

                }

            }
            else
            {
                cs = myCamp;
            }

        }
        return cs;
    }

    public override  CharacterState GetAttackTarget(float radius = 2f)
    {
        CharacterState target = null;
        float disMin = float.MaxValue;
        for (int i = 0; i < SceneBaseManager.instance.agents.size; i++)
        {
            CharacterState chs = SceneBaseManager.instance.agents[i];
            if (BattleUtil.IsTargeted(thisCs, chs, radius))
            {
                float dis = Vector3.Distance(thisCs.transform.position, chs.transform.position);
                if (BattleUtil.IsHeroTarget(chs))
                {
                    target = chs;
                    break;
                }
                if (chs.state == Modestatus.Tower && chs.currentHp < 0.2f * chs.maxHp)
                {
                    target = chs;
                    break;
                }
                if (dis < disMin)
                {
                    disMin = dis;
                    target = chs;
                }
            }
        }
        return target;
    }

    #region Conditions

   

    List<CharacterState> GetMyTowers()
    {
        List<CharacterState> csList = null;
        if (thisCs.atkLine == HeroAttackLine.leftLine)
        {
            csList = myTowersLeft;
        }
        else if (thisCs.atkLine == HeroAttackLine.RightLine)
        {
            csList = myTowersRight;
        }
        return csList;
    }
    
    List<CharacterState> GetEnemyTowers()
    {
        List<CharacterState> csList = null;
        if (thisCs.atkLine == HeroAttackLine.leftLine)
        {
            csList = enemyTowersLeft;
        }
        else if (thisCs.atkLine == HeroAttackLine.RightLine)
        {
            csList = enemyTowersRight;
        }
        return csList;
    }
    bool IsAttackedByTowers(List<CharacterState> enemyList, GameObject selfGo)
    {
        bool flag = false;
        if (enemyList != null)
        {
            for (int i = 0; i < enemyList.Count; i++)
            {
                flag = (flag || IsAttackBy(enemyList[i], selfGo));
            }
        }
        return flag;

    }

    bool IsRunHomeState()
    {
        bool result = false;
       
       
        {
          
            {
                if ((float)thisCs.currentHp / (float)thisCs.maxHp < fleeHpRate )
                {
                    result = true;
                }
            }
        }
       
        return result;
    }

    bool IsAttackBy(CharacterState cs, GameObject go)
    {
        return cs != null && cs.attackTarget != null && cs.attackTarget.gameObject == go;
    }

    bool HaveTarget()
    {
        bool isAttack = false;
        if (targetCs != null  )
        {
            if (targetCs.groupIndex != 99 && targetCs.groupIndex != 100 && targetCs.groupIndex != 101 )
            {
                isAttack = true;
            }
            
        }
        return isAttack ;
    }

    bool IsRuntoEnemyTower()
    {
        bool result = false;
        result = ((float)thisCs.currentHp / (float)thisCs.maxHp > outBaseHpRate) && !HaveTarget() && !isInTowerAtkRange;
        //Debug.Log(string.Format("id:{0}/member:{1}", id, memberId));
        return result ;
    }

    bool IsStayInHome()
    {
        bool result = false;
        float dis = Vector3.Distance(thisCs.transform.position, myHome.transform.position);
        if (dis < myHomeRadius)
        {
            result = true;
        }
        return result;
    }

    bool IsMyMonsterOutEnemyTower()
    {
        bool result = false;
        var allCs = SceneBaseManager.instance.friendly;
        var enemyTower = this.GetEnemyTower();
        for (int i = 0; i < allCs.size; i++)
        {
            if (enemyTower != null)
            {
                float dis = Vector3.Distance(enemyTower.transform.position, allCs[i].transform.position);
                if (dis < towerAtkRange)
                {
                    result = true;
                }
                else result = false;
            }

        }
        return result;

    }
    bool IsDefendState()
    {
        bool result = false;
        if ((float)thisCs.currentHp / (float)thisCs.maxHp < defendHpRate && (float)thisCs.currentHp / (float)thisCs.maxHp>fleeHpRate)
        {
            result = true;
        }
        return result;
    }

    bool IsSummonState()
    {
        bool result = false;
        var target = thisCs.attackTarget;
        if (target != null)
        {
            if (target.gameObject.CompareTag("EnemyPlayer"))
                result = true;
        }
        return result ;
    }

    bool IsPatrolState()
    {
        bool result = false;
        float ratio = (float)thisCs.currentHp / (float)thisCs.maxHp;
        bool enemyIntower = false;
        bool towerHpState = false;
        bool isVisible = false;
        CharacterState tower = this.GetEnemyTower();
        var enemy = SceneBaseManager.instance.enemy;
        if (enemy!=null && tower!=null)
        {
            for (int i = 0; i < enemy.size; i++)
            {
                if (enemy[i].state == Modestatus.NpcPlayer)
                {
                    float dis = Vector3.Distance(enemy[i].transform.position, tower.transform.position);
                    if (dis<tower.AttackRange)
                    {
                        enemyIntower = true;
                    }
                }
            }
            if (tower.currentHp>100)
            {
                towerHpState = true;
            }
            if (Vector3.Distance(thisCs.transform.position,tower.transform.position)>thisCs.AttackRange+tower.AttackRange)
            {
                isVisible = true;
            }
        }
        //Debug.Log(string.Format("id:{0}/member:{1}",id,memberId));
        result =  enemyIntower  && isInTowerAtkRange;
        return result;

    }

  
    bool isAtkInEnemyTower()
    {
        bool haveMate = false;
        bool result = false;
        bool enemy = false;
        int matesCount = 0;
        int enemyHeroCnt = 0;
        CharacterState tower = this.GetEnemyTower();
        if (tower!=null)
        {            
            var mates = SceneBaseManager.instance.friendly;
            var enemyHero = SceneBaseManager.instance.enemy;
            for (int i = 0; i < mates.size; i++)
            {
                if (BattleUtil.ReachPos(mates[i].transform.position,tower.transform.position,tower.AttackRange))
                {
                    matesCount++;
                }
            }
            if (matesCount>0)
            {
                haveMate = true;
                matesCount=0;
            }
            for (int i = 0; i < enemyHero.size; i++)
            {
                if (enemyHero[i].state==Modestatus.NpcPlayer)
                {
                    if (BattleUtil.ReachPos(enemyHero[i].transform.position, tower.transform.position, tower.AttackRange))
                    {
                        enemyHeroCnt++;
                    }
                }
            }
            if (enemyHeroCnt>0)
            {
                enemy = true;
                enemyHeroCnt = 0;
            }
        }
        result =( isInTowerAtkRange && haveMate &&!enemy ) || (isInTowerAtkRange && haveMate && enemy && tower.currentHp<100)&&!IsDefendState();
        //Debug.Log(string.Format("id:{0}/member:{1}", id, memberId));
       // Debug.Log(string.Format("id:{0}/member:{1}", id, memberId));
        
        return result;
    }

    bool IsAtkMonsterState()
    {
        bool result = false;
        float moral = 0;
        if (thisCs.groupIndex==1)
        {
            for (int i = 0; i < mo.herosDataBlue.Count; i++)
            {
                moral += mo.herosDataBlue[i].mobaMorale;
            }
        }
        else if (thisCs.groupIndex == 0)
        {
            for (int i = 0; i < mo.herosDataBlue.Count; i++)
            {
                moral += mo.herosDataRed[i].mobaMorale;
            }
        }
        int cnt = SceneBaseManager.instance.wildMonster.size;
        int cntRed= SceneBaseManager.instance.wildMonsterRed.size;
        result = (moral >= moralPoint ) && thisCs.CharData.memberId==1 && ((thisCs.groupIndex==1?cnt:cntRed) > 0) &&thisCs.groupIndex==1;
         //Debug.Log(string.Format("id:{0}/member:{1}", thisCs.groupIndex, thisCs.CharData.memberId));
        return result;
    }
    #endregion Conditions

    void OnDestroy()
    {

    }
}
