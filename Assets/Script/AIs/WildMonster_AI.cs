using UnityEngine;
using System.Collections;

public class WildMonster_AI : MonsterAI3v3
{
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
            float dis = Vector3.Distance(thisCs.transform.position, chs.transform.position);
            if (BattleUtil.IsTargeted(thisCs, chs, radius))
            {
                if ((chs.state != Modestatus.Monster ) && chs.state != Modestatus.Tower &&  chs.groupIndex != 99 && chs.groupIndex != 100)
                {
                    if (dis < minDis)
                    {
                        minDis = dis;
                        result = chs;
                    }
                }
                if (BattleUtil.IsHeroTarget(chs))
                {
                    ifPlayer = chs;
                }
            }
            if (result == null && ifPlayer != null)
                result = ifPlayer;
        }
        return result;
    }


}
