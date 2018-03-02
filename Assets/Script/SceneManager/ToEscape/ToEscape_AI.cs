using UnityEngine;

public class ToEscape_AI : Monster_AI
{
    bool isAttackTar = true;
    float stopTime = 0;

    protected override void OnFixedUpdate ()
    {
        if (null == targetCs)
        {
            if (null == CharacterManager.playerCS) return;
            targetCs = CharacterManager.playerCS;
            thisCs.SetAttackTargetTo(CharacterManager.playerCS);
        }
        else
        {
            if(!aiSkillHandler.NormalAISkill())
                aiSkillHandler.NormalAttack();
            //if (Vector3.Distance(thisCs.transform.position, targetCs.transform.position) < 0.7f)
            //{
            //    isAttackTar = false;
            //    thisCs.pm.Stop();
            //    if (thisCs.pm.canAttack)
            //        DoAttack();

            //}
            //else if (isAttackTar)
            //{
            //    //thisCs.pm.Move(targetCs.transform.position);
            //    thisCs.pm.Approaching(targetCs.transform.position);
            //}
        }
        if (!isAttackTar)
        {
            stopTime += Time.deltaTime;
            if (stopTime > 1)
            {
                isAttackTar = true;
                stopTime = 0;
            }
        }
    }
}