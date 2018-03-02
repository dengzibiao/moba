using UnityEngine;
using System.Collections;

public class PlayerAppearedState : PlayerBaseState
{


    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        if (cs.type == ModelType.NPC)
            cs.ShowNPCEffect(true);
        if (cs.mCurMobalId == MobaObjectID.HeroChenmo)
            cs.EndAppeared(0);
        if (GameLibrary.IsMajorOrLogin())
        {
            if (cs.mCurMobalId != MobaObjectID.None)
            {
                //if (GameLibrary.IsLoginScene())
                //{
                //    AudioController.Instance.PlayUISound(GameLibrary.Resource_Sound + cs.GetMobaName() + "/dlgShow", true);
                //}
                //else if (GameLibrary.IsMajorScene())
                //{
                    int mDialogIndex = Random.Range(0, cs.mAmountDlg);
                    AudioController.Instance.PlayUISound(GameLibrary.Resource_Sound + cs.GetMobaName() + "/dlg" + (mDialogIndex + 1), true);
                //}
            }
        }
        return;
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        if (GameLibrary.IsMajorOrLogin())
        {
            if (null != UICreateRole.instance)
            {
                UICreateRole.instance.SetSpinWithMouse();
            }
            if (null != cs)
            {
                //cs.EndAppeared(1);
            }
            if (cs.type == ModelType.NPC)
            {
                cs.ShowNPCEffect(false);
            }
            return;
        }
    }
}
