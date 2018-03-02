using System.Collections.Generic;
using UnityEngine;

public class Player_ArenaAI : BasePlayerAI
{
    bool controlState;
    protected override void OnStart ()
    {
        base.OnStart();
        aiSkillHandler.SetSkills(thisCs.GetSkills());
    }

    protected override void OnFixedUpdate ()
    {
        if(!GameLibrary.Instance().CanControlSwitch(thisCs.pm))
        {
            thisCs.pm.Stop();
            return;
        }
        CharacterState minHpCS = GetMinHpCS();
        thisCs.SetAttackTargetTo(minHpCS);//GetAttackTarget(thisCs.TargetRange)
        if(thisCs.attackTarget != null)
        {
            //float dis = Vector3.Distance(transform.position, minHpCS.transform.position);
            //List<SkillNode> skillsCanUse = aiSkillHandler.GetSkillsCanUse(aiSkillHandler.skillsCDOver, (SkillNode node) => { return (node.dist > dis || node.dist == 0); }, new SkillOrderComparer());
            //if (skillsCanUse.Count > 0 && aiSkillHandler.CanSkill())
            //    aiSkillHandler.UseSkill(skillsCanUse[0]);
            //else
            //    aiSkillHandler.NormalAttack();
            // aiSkillHandler.NormalAISkill(new SkillOrderComparer());
            if(!aiSkillHandler.NormalAISkill(new SkillOrderComparer()))
                aiSkillHandler.NormalAttack();
        }
        else if(minHpCS != null && !BattleUtil.ReachPos(thisCs.transform.position, minHpCS.transform.position, thisCs.TargetRange))
        {
            thisCs.pm.Approaching(minHpCS.transform.position);
        }
        else
        {
            thisCs.pm.Stop();
        }
    }

    public override CharacterState GetAttackTarget ( float radius = 2f )
    {
        if(SceneBaseManager.instance == null)
            return null;
        for(int i = 0; i < SceneBaseManager.instance.agents.size; i++)
        {
            CharacterState chs = SceneBaseManager.instance.agents[i];
            if(BattleUtil.IsTargeted(thisCs, chs, radius))
            {
                return chs;
            }
        }
        return null;
    }

    CharacterState GetMinHpCS ()
    {
        CharacterState ret = null;
        //int minHp = int.MaxValue;
        BetterList<CharacterState> agents = SceneBaseManager.instance.agents;
        //for(int i = 0; i < agents.size; i++)
        //{
        //    if(agents[i].groupIndex != thisCs.groupIndex && agents[i].currentHp < minHp)
        //        ret = agents[i];
        //}

        float recentDis = 1000;
        float dis = 0;
        for (int i = 0; i < agents.size; i++)
        {
            dis = Vector3.Distance(thisCs.transform.position, agents[i].transform.position);
            if (agents[i].groupIndex != thisCs.groupIndex && dis < recentDis)
            {
                ret = agents[i];
                recentDis = dis;
            }
        }
        return ret;
    }
}