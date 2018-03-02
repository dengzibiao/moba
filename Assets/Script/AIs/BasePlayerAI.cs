using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePlayerAI : MonoBehaviour
{
    public CharacterState targetCs;
    public GameObject[] autoPoints;
    
    public bool isAttack = true;       // 是否主动攻击
    public bool isPatrol;       // 是否巡逻


    protected int pIndex = 0;
    protected GameObject currentPathPoint;

    protected CharacterState thisCs;
    public AISkillHandler aiSkillHandler;

    void Awake ()
    {
        thisCs = GetComponent<CharacterState>();
        aiSkillHandler = UnityUtil.AddComponetIfNull<AISkillHandler>(gameObject);
        aiSkillHandler.SetSkills(thisCs.GetSkills());
        OnAwake();
    }

    void Start ()
    {
        currentPathPoint = GetPoint(pIndex);
        if(currentPathPoint != null) transform.LookAt(currentPathPoint.transform);
        OnStart();
    }

    void FixedUpdate ()
    {
        OnFixedUpdate();
    }

    protected virtual void OnAwake () { }
    protected virtual void OnStart () { }
    protected virtual void OnFixedUpdate () { }

    public virtual CharacterState GetAttackTarget (float radius = 2f) {
        return null;
    }

    protected GameObject GetPoint ( int index )
    {
        return ( autoPoints != null && index < autoPoints.Length ) ? autoPoints[index] : null;
    }

    float waitTime = 0.02f;
    protected virtual void PathFinding ()
    {
        if(currentPathPoint != null)
        {
            if(BattleUtil.ReachPos(transform.position, currentPathPoint.transform.position, thisCs.pm.nav.stoppingDistance))
            {
                thisCs.pm.Stop();
                if(waitTime > 0)
                {
                    waitTime -= Time.fixedDeltaTime;
                    if(waitTime <= 0)
                    {
                        pIndex++;
                        currentPathPoint = GetPoint(pIndex);
                        PathFinding();
                        waitTime = isPatrol ? Random.Range(0.1f, 1f) : 0.02f;
                    }
                }
            }
            else
            {
                thisCs.pm.Approaching(currentPathPoint.transform.position);
            }
        }
        else
        {
            if(isPatrol && pIndex >= autoPoints.Length)
            {
                pIndex = 0;
                currentPathPoint = GetPoint(pIndex);
            }
            thisCs.pm.Stop();
        }
    }

    protected void CheckChangeTarget ( CharacterState tcs )
    {
        if(targetCs == null)
        {
            if(tcs.state == Modestatus.SummonHero && tcs.Master != null)
                targetCs = tcs.Master;
            else
                targetCs = tcs;
        }
        thisCs.SetAttackTargetTo(targetCs);
    }
}