using UnityEngine;

public class Player_TowerDefenceAI : BasePlayerAI
{
    void FixedUpdate()
    {
        if (GameLibrary.isBossChuChang)
            return;
        if (!GameLibrary.Instance().CanControlSwitch(thisCs.pm))
        {
            thisCs.pm.Stop();
            return;
        }
        targetCs = GetAttackTarget(thisCs.TargetRange);
        thisCs.SetAttackTargetTo(targetCs);
        if(isAttack && targetCs != null)
        {
            if(!aiSkillHandler.NormalAISkill())
                aiSkillHandler.NormalAttack();
        }
        else
        {
            PathFinding();
        }
    }

    public override CharacterState GetAttackTarget ( float radius = 2f )
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
