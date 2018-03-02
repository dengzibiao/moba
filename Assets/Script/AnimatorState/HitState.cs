using UnityEngine;

public class HitState : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerMotion pm = animator.GetComponent<PlayerMotion>();
        pm.canAttackSwitch ++;
        pm.canSkillSwitch++;
        pm.canMoveSwitch++;
    }

    override public void OnStateExit ( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
    {
        PlayerMotion pm = animator.GetComponent<PlayerMotion>();
        pm.canAttackSwitch--;
        pm.canSkillSwitch--;
        pm.canMoveSwitch--;
    }
}
