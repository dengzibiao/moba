using UnityEngine;

public class MountAndPetState : StateMachineBehaviour
{
    protected MountAndPetMotion pm;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        SetCsAndPm(animator);
        //if (pm != null)
        //{
        //    pm.PlayMusicByAnim(stateInfo);
        //}
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
        SetCsAndPm(animator);
        if (pm != null)
        {
            pm.StopMusic();
        }
    }

    private void SetCsAndPm(Animator animator)
    {
        if (pm == null)
        {
            pm = animator.GetComponent<MountAndPetMotion>();
        }
    }
}
