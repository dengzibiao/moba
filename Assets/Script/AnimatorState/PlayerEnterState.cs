using UnityEngine;

public class PlayerEnterState : PlayerBaseState
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
        if (cs != null && cs.OnEnterOver != null)
        {
            cs.OnEnterOver(cs);
        }
    }
}
