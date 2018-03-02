using UnityEngine;

public class SummonHero : MonoBehaviour
{
    public void Summon ( HeroData hero)
    {
        GameObject go = Resource.CreateCharacter(hero.node.icon_name,SceneBaseManager.instance.gameObject, transform.position);
        CharacterState playerCS = GetComponent<CharacterState>();
        CharacterState cs = BattleUtil.AddMoveComponents(go, hero.attrNode.modelNode);
        playerCS.Summon = cs;
        cs.Master = playerCS;
        hero.groupIndex = playerCS.groupIndex;
        hero.state = Modestatus.SummonHero;
        cs.InitData(hero);
        cs.Invincible = true;
        cs.pm.nav.obstacleAvoidanceType = UnityEngine.AI.ObstacleAvoidanceType.NoObstacleAvoidance;
        cs.pm.nav.avoidancePriority = 59;

        SummonHeroAI sAI = UnityUtil.AddComponetIfNull<SummonHeroAI>(go);

        if(playerCS.attackTarget != null)
        {
            SetSummonHeroTrans(go.transform, playerCS.attackTarget.transform);
            sAI.target = playerCS.attackTarget;
            cs.SetAttackTargetTo(playerCS.attackTarget);
        }
        else
        {
            go.transform.position = 1.5f * transform.forward.normalized + transform.position;
            go.transform.position = BattleUtil.GetNearestNavPos(go);
            go.transform.LookAt(go.transform.position + 2f * transform.forward.normalized);
        }
        if(cs.mCurMobalId == MobaObjectID.HeroShengqi)
        {
            go.transform.position = 0.5f * transform.forward.normalized + transform.position;
            go.transform.position = BattleUtil.GetNearestNavPos(go);
            go.transform.LookAt(transform.position);
        }
    }

    void SetSummonHeroTrans ( Transform summnonHeroTrans, Transform targetTrans )
    {
        //summnonHeroTrans.position = targetTrans.position - 0.3f * ( targetTrans.position - transform.position ).normalized;
        summnonHeroTrans.position = targetTrans.position + GetRandomDirPos();
        summnonHeroTrans.position = BattleUtil.GetNearestNavPos(summnonHeroTrans.gameObject);
        summnonHeroTrans.LookAt(targetTrans.transform.position);
    }

    Vector3 GetRandomDirPos ()
    {
        float randomRot = Random.Range(0, 360);
        return 0.3f * ( Quaternion.Euler(0f, randomRot, 0f) * Vector3.one );
    }
}