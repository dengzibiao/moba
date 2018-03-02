using UnityEngine;

public class TotalPoints_AI : Monster_AI
{

    SceneTotalPoints sceneIns;

    protected override void OnStart ()
    {
        base.OnStart();

        sceneIns = GetComponentInParent<SceneTotalPoints>();

        thisCs.OnBeAttack += (CharacterState cs) => ChangeTarget(cs);
        sceneIns.Tower.OnBeAttack += (CharacterState tCs) => TowerBeAttack(tCs);

    }

    protected override void OnFixedUpdate ()
    {
        base.OnFixedUpdate();
        if (targetCs == SceneTotalPoints.instance.Tower)
        {
            for (int i = 0; i < SceneBaseManager.instance.agents.size; i++)
            {
                CharacterState chs = SceneBaseManager.instance.agents[i];
                if (null == chs) continue;
                float dis = Vector3.Distance(thisCs.transform.position, chs.transform.position);
                if (dis < 2 && chs.state != thisCs.state)
                {
                    targetCs = GetAttackTarget();
                }
            }
        }
        if (null != SceneTotalPoints.instance.Tower &&!BattleUtil.ReachPos(transform.position, SceneTotalPoints.instance.Tower.transform.position, 4))
        {
            targetCs = SceneTotalPoints.instance.Tower;
    }
    }

    public override CharacterState GetAttackTarget(float radius = 2)
    {
        CharacterState result = null;
        CharacterState ifPlayer = null;

        float minDis = float.MaxValue;
        for (int i = 0; i < SceneBaseManager.instance.agents.size; i++)
        {
            CharacterState chs = SceneBaseManager.instance.agents[i];
            if (null == chs) continue;
            float dis = Vector3.Distance(thisCs.transform.position, chs.transform.position);
            if (chs != null && chs.groupIndex != thisCs.groupIndex && dis < radius && !chs.Invisible && !chs.Invincible)
            {
                if (!BattleUtil.IsHeroTarget(chs) && chs.state != Modestatus.Tower && dis < minDis)
                {

                    minDis = dis;
                    result = chs;
                }
                else
                    ifPlayer = chs;
            }
            if (result == null && ifPlayer != null)
                result = ifPlayer;
        }
        return result;
    }

    void ChangeTarget(CharacterState cs)
    {
        for (int i = 0; i < SceneTotalPoints.instance.agents.size; i++)
        {
            Monster_AI mAI = SceneTotalPoints.instance.agents[i].GetComponent<Monster_AI>();
            if (!mAI || null == mAI.targetCs || mAI.targetCs.state != Modestatus.Tower)
            {
                continue;
            }
            if (SceneTotalPoints.instance.agents[i].groupIndex == thisCs.groupIndex)
            {
                mAI.targetCs = cs;
            }
        }
    }

    void TowerBeAttack(CharacterState tCs)
    {
        if (null == thisCs) return;
        if (tCs.groupIndex == thisCs.groupIndex) return;

        targetCs = tCs;
    }

}
