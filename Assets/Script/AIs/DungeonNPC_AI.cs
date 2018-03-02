using System.Collections.Generic;
using UnityEngine;

public class DungeonNPC_AI : Player_AutoAI
{
    float distance;
    CharacterState player;

    public bool isNPC = false;

    protected override void OnStart()
    {
        player = CharacterManager.player.GetComponent<CharacterState>();
    }

    protected override void OnFixedUpdate()
    {
        if (GameLibrary.isBossChuChang) return;
        if (isNPC)
        {
            PathFinding();
        }
        else
        {
            if (null != player && null != player.attackTarget)
            {
                targetCs = player.attackTarget;
                thisCs.SetAttackTargetTo(player.attackTarget);
            }
            else
            {
                targetCs = CharacterManager.instance.GetAttackTarget(3, thisCs);
            }

            if (null != targetCs)
                AttackTarget();
            else
                PathFinding();
        }
    }

    void AttackTarget()
    {

        float dis = Vector3.Distance(transform.position, targetCs.transform.position);
        List<SkillNode> skillsCanUse = aiSkillHandler.GetSkillsCanUse(aiSkillHandler.skillsCDOver, (SkillNode node) => { return (node.dist > dis || node.dist == 0); }, new SkillOrderComparer());
        if (skillsCanUse.Count > 0 && aiSkillHandler.CanSkill())
            aiSkillHandler.UseSkill(skillsCanUse[0]);
        else
            aiSkillHandler.NormalAttack();

    }

    protected override void PathFinding()
    {
        distance = Vector3.Distance(CharacterManager.player.transform.position, transform.position);

        if ((isNPC || null == thisCs.attackTarget) && distance > 1)
        {
            //thisCs.pm.Move(CharacterManager.player.transform.position);
            thisCs.pm.Approaching(CharacterManager.player.transform.position);
        }

        if (distance <= 1)
        {
            thisCs.pm.Stop();
        }
    }
}
