using UnityEngine;
using Tianyu;
using System.Collections.Generic;

public class TotalPoints_PlayerAI : BasePlayerAI
{
    CharacterState tower;

    protected override void OnStart()
    {
        targetCs = tower = SceneTotalPoints.instance.Tower;
        thisCs.OnBeAttack += (CharacterState tCs) => targetCs = tCs;
        tower.OnBeAttack += (CharacterState tCs) => TowerBeAttack(tCs);
    }

    protected override void OnFixedUpdate()
    {
        if (null != targetCs && targetCs.isDie){
            targetCs = null;
        }
        if (null != tower && Vector3.Distance(transform.position, tower.transform.position) > 4)
        {
            targetCs = tower;
        }
        else
        {
            if (null == targetCs || targetCs.isDie)
            {
                targetCs = GetTarget(thisCs.TargetRange);
                thisCs.SetAttackTargetTo(targetCs);
            }
            if (null == targetCs || targetCs.isDie || Vector3.Distance(targetCs.transform.position, tower.transform.position) > 4)
            {
                targetCs = tower;
            }
        }

        if (null != targetCs && !targetCs.isDie)
        {
            if (!BattleUtil.ReachPos(thisCs, targetCs, thisCs.TargetRange))
            {
                thisCs.pm.Approaching(targetCs.transform.position);
            }
            else
            {
                thisCs.SetAttackTargetTo(targetCs);
                if (!aiSkillHandler.NormalAISkill())
                    aiSkillHandler.NormalAttack();
            }
        }
    }

    CharacterState GetTarget(float radius = 2f)
    {
        CharacterState state = null;

        float dis = 0;

        List<CharacterState> targetList = new List<CharacterState>();

        for (int i = 0; i < SceneBaseManager.instance.agents.size; i++)
        {
            if (SceneBaseManager.instance.agents[i].groupIndex == thisCs.groupIndex) continue;
            if (SceneBaseManager.instance.agents[i].state == Modestatus.Tower) continue;
            dis = Vector3.Distance(SceneBaseManager.instance.agents[i].transform.position, transform.position);
            if (dis <= radius)
                targetList.Add(SceneBaseManager.instance.agents[i]);
        }
        if (targetList.Count > 0)
        {
            targetList.Sort(new TPTargetComparer());
            return targetList[0];
        }
        return state;
    }

    void TowerBeAttack(CharacterState tCs)
    {
        if (tCs.groupIndex == thisCs.groupIndex) return;
        if (null == targetCs || targetCs.isDie) return;
        targetCs = tCs;
        //if (null != targetCs && targetCs.state != Modestatus.Tower) return;

    }

}

public class TPTargetComparer : IComparer<CharacterState>
{
    public int Compare(CharacterState nodeA, CharacterState nodeB)
    {
        return nodeA.state > nodeB.state ? 1 : -1;
    }
}
