using System.Collections.Generic;
using Tianyu;
using UnityEngine;

public class AISkillHandler : MonoBehaviour
{
    public List<SkillNode> skillsCDOver = new List<SkillNode>();
    // List<float> FakeCDs = new List<float>();
    public bool allSkillCD;
    public bool canRestoreCD;
    int restoreCD;
    public float skillMaxRange { get; private set; }
    CharacterState thisCs;
    bool controlState;
    Vector3 avoidPos;
    GameObject avoidPoint;

    void Awake()
    {
        thisCs = GetComponent<CharacterState>();
    }

    void OnDestroy ()
    {
        if(avoidPoint != null)
            Destroy(avoidPoint);
    }

    void Update()
    {
        if (thisCs != null && !thisCs.isDie && thisCs.pm.nav.hasPath && !thisCs.pm.CanMoveState())
        {
            thisCs.pm.nav.ResetPath();
            thisCs.pm.Stop();
            if (thisCs.pm.nav.enabled)
            {
                thisCs.pm.nav.enabled = false;
                thisCs.pm.nav.enabled = true;
            }
        }
    }

    public bool NormalAISkill (IComparer<SkillNode> usePriority = null, System.Predicate<SkillNode> filter = null, float restoreHpRate = 0.6f)
    {
        if(controlState != GameLibrary.Instance().CanControlSwitch(thisCs.pm)) {
            controlState = GameLibrary.Instance().CanControlSwitch(thisCs.pm);
        }
        if (!GameLibrary.Instance().CanControlSwitch(thisCs.pm) || !CanSkill())
            return false;

        //if(thisCs == CharacterManager.playerCS)
        //    Summon();

        List<SkillNode> filteredSkills = GetSkillsCanUse(skillsCDOver, filter);
        if(usePriority == null) usePriority = new SkillDistComparer();
        List <SkillNode> skills = GetSkillsCanUse(filteredSkills, (node)=> {
            bool restoreCheck = thisCs.currentHp < (thisCs.maxHp * restoreHpRate) || !node.IsRestore();
            bool canCastToSelf = GameLibrary.Instance().CheckHitCondition(node, thisCs, thisCs);
            bool canCastToEnemy = false;
            if(thisCs.attackTarget != null)
            {
                canCastToEnemy = GameLibrary.Instance().CheckHitCondition(node, thisCs, thisCs.attackTarget);
            }
            return (canCastToSelf || canCastToEnemy) && restoreCheck;
        }, usePriority);
        if(skills.Count > 0)
        {
            if(skills[0].target == TargetState.None)
            {
                if(thisCs.attackTarget != null)
                {
                    float dis = Vector3.Distance(transform.position, thisCs.attackTarget.transform.position);
                    if(skills[0].dist == 0 || dis < GameLibrary.Instance().GetSkillDistBySkillAndTarget(thisCs, skills[0]))
                    {
                        UseSkill(skills[0], usePriority.GetType().ToString());
                    }
                    else
                    {
                        thisCs.pm.Approaching(thisCs.attackTarget.transform.position, 0f);
                    }
                }
                else
                {
                    UseSkill(skills[0], usePriority.GetType().ToString());
                }
                return true;
            }
            else
            {
                if(thisCs.attackTarget == null)
                {
                    return false;
                }
                else
                {
                    float dis = Vector3.Distance(transform.position, thisCs.attackTarget.transform.position);
                    if(skills[0].dist == 0 || dis < GameLibrary.Instance().GetSkillDistBySkillAndTarget(thisCs, skills[0]))
                    {
                        UseSkill(skills[0], usePriority.GetType().ToString());
                    }
                    else
                    {
                        thisCs.pm.Approaching(thisCs.attackTarget.transform.position, 0f);
                    }
                    return true;
                }
            }
        }
        else
        {
            return false;
        }
    }

    public void NormalAttack(CharacterState target=null)
    {
        if (target == null)
            target = thisCs.attackTarget;
        if(target == null)
            return;

        float disRadius = thisCs.pm.nav.radius;
        if(BattleUtil.ReachPos(thisCs, target, thisCs.AttackRange + GameLibrary.Instance().GetExtendDis(target)))
        {
            if(CanAttack())
            {
                thisCs.pm.Stop();
                thisCs.pm.RotateTo(target.transform.position);
                thisCs.pm.ContinuousAttack();
                //Debug.LogError(Time.realtimeSinceStartup + " Do attack");
            }
            else if(!BattleUtil.ReachPos(thisCs, target, 0.2f))
            {
                thisCs.pm.Approaching(target.transform.position);
                //Debug.LogError(Time.realtimeSinceStartup + " move to " + target.transform.position);
            } else
            {
                thisCs.pm.Stop();
                //Debug.LogError(Time.realtimeSinceStartup + " Stop");
            }
        }
        else
        {
            thisCs.pm.Approaching(target.transform.position);
            //Debug.LogError(Time.realtimeSinceStartup + " move to " + target.transform.position);
        }
    }

    //控制技能
    public void ControllSkill()
    {
        if (controlState != GameLibrary.Instance().CanControlSwitch(thisCs.pm))
        {
            controlState = GameLibrary.Instance().CanControlSwitch(thisCs.pm);
        }
        if (thisCs.attackTarget == null || !GameLibrary.Instance().CanControlSwitch(thisCs.pm))
            return;
        float dis = Vector3.Distance(transform.position, thisCs.attackTarget.transform.position);
        List<SkillNode> skills = GetSkillsCanUse(skillsCDOver, (SkillNode node) =>
         {
            return !node.IsControl();
         });
        if (CanSkill() && skills.Count > 0)
        {
            List<SkillNode> skillsInRange = GetSkillsCanUse(skills, (SkillNode node) => { return node.dist > dis || node.dist == 0; }, new SkillSiteComparer());
            if (skillsInRange.Count > 0)
                UseSkill(skillsInRange[0]);
            else
                thisCs.pm.Approaching(thisCs.attackTarget.transform.position);
        }
    }

    public void AddHpSkill()
    {       
        if ( !GameLibrary.Instance().CanControlSwitch(thisCs.pm))
            return;
        if (thisCs.Restore())
        {
            //thisCs.pm.Skill(0);
        }
    }
    public bool CanSkill()
    {
        return thisCs != null && !thisCs.isDie && !allSkillCD && thisCs.pm.CanSkillState();
    }

    public bool CanAttack()
    {
        return thisCs != null && !thisCs.isDie && thisCs.pm.CanAttackState();
    }

    public List<SkillNode> GetSkillsCanUse(List<SkillNode> skills, System.Predicate<SkillNode> filter = null, IComparer<SkillNode> usePriority = null)
    {
        List<SkillNode> ret = filter == null ? skills : skills.FindAll(filter);
        if (usePriority != null) ret.Sort(usePriority);
        return ret;
    }

    public void SetSkills(List<long> skills, List<float> startCD = null)
    {
        skillsCDOver.Clear();
        for (int i = 0; i < skills.Count; i++)
        {
            if (startCD != null && startCD.Count > i)
            {
                long sid = skills[i];
                CDTimer.GetInstance().AddCD(startCD[i], ( int count, long cid ) => { AddSkill(GetNode(sid)); });
                //StartRestoreCD();
            }
            else
            {
                AddSkill(GetNode(skills[i]));
            }
        }
    }

    void AddSkill(SkillNode node)
    {
        if(!skillsCDOver.Contains(node))
        {
            skillsCDOver.Add(node);
            //if(thisCs.state == Modestatus.Player)
            //    Debug.LogError(" add " + node.skill_id + " | count is " + skillsCDOver.Count);
        }
    }

    public bool UseSkill(SkillNode node, string skillType = "")
    {
        // Debug.LogError(Time.realtimeSinceStartup + " " + name + " use " + skillType + " skill id " + node.skill_id);
        int indx = (int)node.site;
        if(thisCs == CharacterManager.playerCS && FightTouch._instance != null && FightTouch._instance.gameObject.activeSelf)
        {
            if(FightTouch._instance.GetSkillBtn(indx).isCD)
                return false;
            FightTouch._instance.GetSkillBtn(indx).StartCd();
        }

        if(node.IsSerialSkill())
        {
            float totalCD = UseSerialSkill(node);
            StartAllSkillCD(totalCD + 0.5f);
        } else
        {
            UseNormalSkill(node);
            StartAllSkillCD();
        }
        
        if(skillsCDOver.Contains(node))
        {
            skillsCDOver.Remove(node);
            //if(thisCs.state == Modestatus.Player)
            //    Debug.LogError("use and remove " + node.skill_id + "| count is " + skillsCDOver.Count);
        }
        CDTimer.GetInstance().AddCD(node.cooling, (int count, long cid) => { AddSkill(node); });
        return true;
    }
    
    float UseSerialSkill ( SkillNode node )
    {
        float totalCd = 0f;
        for(int i = 0; i < node.skill_parts.Length; i++)
        {
            SkillNode partNode = GetNode(node.skill_parts[i]);
            totalCd += i > 0 ? partNode.cooling : 0f;
            CDTimer.GetInstance().AddCD(totalCd, ( int c, long id ) => UseNormalSkill(partNode));
        }
        return totalCd;
    }

    void UseNormalSkill ( SkillNode node )
    {
        int indx = (int)node.site;
        thisCs.pm.Stop();
        if(thisCs.state == Modestatus.Boss && thisCs.attackTarget != null)
        {
            thisCs.SetForward();
        }
        thisCs.pm.Skill(indx);
    }

    public void UseSkill2(SkillNode node)
    {
        if (CanSkill())
        {
            this.UseSkill(node);
        }
    }

    public void Restore()
    {
        if (restoreCD <= 0 && thisCs.Restore()) StartRestoreCD();
    }

    public void Summon ()
    {
        int randIndex = Random.Range(5, 8);
        if(Globe.Heros()[randIndex - 4] == null)
            return;
        if(thisCs == CharacterManager.playerCS && FightTouch._instance != null)
            FightTouch._instance.DoSummon(randIndex, Globe.Heros()[randIndex - 4]);
        else
            GetComponent<SummonHero>().Summon(Globe.Heros()[randIndex - 4]);
    }

    bool CanSummon ()
    {
        return true;
    }

    void StartRestoreCD()
    {
        restoreCD = 30;
        CDTimer.CD cd = CDTimer.GetInstance().AddCD(1, (int count, long cid) => { restoreCD--; }, 30);
        if(thisCs == CharacterManager.playerCS && FightTouch._instance != null)
            FightTouch._instance.skillRestore.StartCd();
        StartAllSkillCD();
    }

    void StartAllSkillCD(float allCd = 0.5f)
    {
        allSkillCD = true;
        CDTimer.GetInstance().AddCD(allCd, (int count, long cid) => allSkillCD = false);
    }

    public static SkillNode GetNode (long id)
    {
        return FSDataNodeTable<SkillNode>.GetSingleton().DataNodeList[id];
    }
}


public class SkillDistComparer : IComparer<SkillNode>
{
    public int Compare(SkillNode nodeA, SkillNode nodeB)
    {
        if (nodeA.dist == 0)
            return -1;
        else if (nodeB.dist == 0)
            return 1;
        else
            return nodeA.dist > nodeB.dist ? -1 : 1;
    }
}

public class SkillSiteComparer : IComparer<SkillNode>
{
    public int Compare ( SkillNode nodeA, SkillNode nodeB )
    {
        return nodeA.site > nodeB.site ? -1 : 1;
    }
}

public class SkillDistAndSiteComparer : IComparer<SkillNode>
{
    public int Compare ( SkillNode nodeA, SkillNode nodeB )
    {
        if((nodeA.dist == 0 && nodeB.dist == 0) || (nodeA.dist == nodeB.dist))
        {
            return compareSite(nodeA, nodeB);
        } else if(nodeA.dist == 0)
            return -1;
        else if(nodeB.dist == 0)
            return 1;
        else
            return nodeA.dist > nodeB.dist ? -1 : 1;
    }

    int compareSite ( SkillNode nodeA, SkillNode nodeB )
    {
        if(nodeA.site > nodeB.site)
        {
            return -1;
        }
        else
        {
            return 1;
        }
    }
}

public class SkillOrderComparer : IComparer<SkillNode>
{
    public int Compare(SkillNode nodeA, SkillNode nodeB)
    {
        return nodeA.castOrder > nodeB.castOrder ? -1 : 1;
    }
}

public class SkillChaseComparer : IComparer<SkillNode>
{
    public int Compare ( SkillNode nodeA, SkillNode nodeB )
    {
        if(nodeA.dist == nodeB.dist)
            return GetPriority(nodeB) - GetPriority(nodeA);
        else if(nodeA.dist == 0)
            return -1;
        else if(nodeB.dist == 0)
            return 1;
        else
            return nodeA.dist > nodeB.dist ? -1 : 1;
    }

    int GetPriority ( SkillNode node)
    {
        if(node.IsTransfer())
            return 3;
        if(node.IsControl())
            return 2;
        if(!node.IsRestore())
            return 1;
        return 0;
    }
}

public class SkillDefendComparer : IComparer<SkillNode>
{
    public int Compare ( SkillNode nodeA, SkillNode nodeB )
    {
        return GetPriority(nodeB) - GetPriority(nodeA);
        //if(nodeA.dist == nodeB.dist)
        //else if(nodeA.dist == 0)
        //    return -1;
        //else if(nodeB.dist == 0)
        //    return 1;
        //else
        //    return nodeA.dist > nodeB.dist ? 1 : -1;
    }

    int GetPriority ( SkillNode node )
    {
        if(node.IsTransfer())
            return 3;
        if(node.IsRestore() || node.IsProtect())
            return 2;
        if(node.IsControl() || node.IsMoveSpeed())
            return 1;
        if(node.IsDebuff())
            return 0;
        return -1;
    }
}