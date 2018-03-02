using UnityEngine;

public class EscoerMonster_AI : Monster_AI
{
    bool isPlayer = false;

    protected override void OnStart ()
    {
        base.OnStart();
        thisCs.OnBeAttack += (CharacterState cs) => ChangeTarget(cs);
    }

    public override CharacterState GetAttackTarget(float radius = 5)
    {
        CharacterState result = null;
        BetterList<CharacterState> targetTrans = new BetterList<CharacterState>();

        if (isPlayer)
            if (null != CharacterManager.player)
                return CharacterManager.player.GetComponent<CharacterState>();

        if (SceneBaseManager.instance.friendly.size <= 0)
            return null;

        if (SceneBaseManager.instance.friendly.size == 1)
        {
            result = SceneBaseManager.instance.friendly[0];
        }
        else
        {
            for (int i = 0; i < SceneBaseManager.instance.friendly.size; i++)
            {
                if (SceneBaseManager.instance.friendly[i].gameObject.tag == "Player") continue;
                targetTrans.Add(SceneBaseManager.instance.friendly[i]);
            }

            if (targetTrans.size == 1)
            {
                result = targetTrans[0].GetComponent<CharacterState>();
            }
            else
            {
                result = SortArray(targetTrans);
            }
            
        }

        return result;

    }

    void ChangeTarget(CharacterState cs)
    {

        if (null == CharacterManager.player || targetCs == CharacterManager.player.GetComponent<CharacterState>()) return;

        if (cs == CharacterManager.player.GetComponent<CharacterState>())
        {
            isPlayer = true;
            targetCs = CharacterManager.player.GetComponent<CharacterState>();
        }
    }

    CharacterState SortArray(BetterList<CharacterState> targetTrans)
    {
        CharacterState tran = null;

        float distance = float.MaxValue;
        float dis = 0;

        for (int i = 0; i < targetTrans.size; i++)
        {
            dis = Vector3.Distance(transform.position, targetTrans[i].transform.position);
            if (dis < distance)
            {
                distance = dis;
                tran = targetTrans[i];
            }
        }

        return tran;

    }

}
