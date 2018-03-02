using UnityEngine;

public class Monster_AI : BasePlayerAI
{


    public bool IsElite = false;

    Transform escapePos = null;

    protected override void OnStart ()
    {
        thisCs.OnBeAttack += CheckChangeTarget;
    }

    protected override void OnFixedUpdate ()
    {
        if (Time.frameCount % GameLibrary.mMonsterDelay != 0) return;

        if (null != escapePos)
        {
            if (Vector3.Distance(transform.position, escapePos.position) < 0.5f)
            {
                thisCs.pm.Stop();
            }
            else
            {
                thisCs.pm.Approaching(escapePos.position);
            }
            return;
        }

        if (targetCs == null || GameLibrary.isMoba)
        {
            targetCs = GetAttackTarget(thisCs.TargetRange);
        }
        if (targetCs != null && (targetCs.Invisible || targetCs.isDie))
        {
            targetCs = GetAttackTarget(thisCs.TargetRange);
        }

        if (targetCs != null)
        {
            if (thisCs.currentHp <= thisCs.maxHp * 0.5f)
            {
                if (!aiSkillHandler.NormalAISkill())
                    aiSkillHandler.NormalAttack();
            }
            else
            {
                aiSkillHandler.NormalAttack();
            }
        }
        else
        {
            PathFinding();
        }
        thisCs.SetAttackTargetTo(targetCs);
    }

    public override CharacterState GetAttackTarget(float radius = 2f)
    {
        CharacterState result = null;
        CharacterState ifPlayer = null;

        float minDis = float.MaxValue;
        if (SceneBaseManager.instance == null)
            return null;
        for (int i = 0; i < SceneBaseManager.instance.agents.size; i++)
        {
            CharacterState chs = SceneBaseManager.instance.agents[i];
            if (BattleUtil.IsTargeted(thisCs, chs, radius))
            {
                float dis = Vector3.Distance(thisCs.transform.position, chs.transform.position);
                if (chs.state == Modestatus.Monster || chs.state == Modestatus.Tower)
                {
                    if (dis < minDis)
                    {
                        minDis = dis;
                        result = chs;
                    }
                }
                if(BattleUtil.IsHeroTarget(chs))
                {
                    ifPlayer = chs;
                }
            }
            if (result == null && ifPlayer != null)
                result = ifPlayer;
        }
        return result;
    }

    public void StopMonsterAI()
    {
        if (null != thisCs)
        {
            thisCs.Hp(100 * 100);
        }
        enabled = false;
    }

    public void StartEscape(Transform pos)
    {
        escapePos = pos;
    }
}
