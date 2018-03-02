using System;
using UnityEngine;


public class CommonDead : BaseStatus
{
    private bool EnterShaderEff = false;
    public override bool OnEnter (STATUS last)
    {
        //mController.Move (Dir.NONE);
        EnterShaderEff = false;
        //mController.SetAnimatorSpeed(1.0f,ActionController.ChangeSpeedLevelDead);
        mController.PlayAnimation (ANIM_INDEX.DEAD);
        //mTimer.Reset ();
        return true;
    }
	#region implemented abstract members of BaseStatus
    public override void UpdateLogic ()
    {
        //if (mController.ShouldPlayNextAnimation (ANIM_INDEX.DEAD) && !EnterShaderEff) {
        //    GetActionController().mShaderEffect.ShowEffect(ShaderEffState.GRADIENT_DEAD);
        //    EnterShaderEff = true;
        //    mTimer.Reset ();
        //}
        //if (EnterShaderEff) {
        //    if (GameGlobal.IsGuideMap () && mController.GetRoleType () == RoleEnum.Role_Type.ACTOR_ENEMY_GUIDE_SHIZIWANG) {
        //        if (mTimer.IsEndOfDuration(SceneConst.GuideMapBossDeadDuration)) {
        //            mController.DestroyGameObject ();
        //        }
        //    }else {
        //        if (mTimer.IsEndOfDuration(SceneController.inst.DeadLastTime)) {
        //            mController.DestroyGameObject ();
        //        }
        //    }
        //}
    }
    public override STATUS GetNextStatus ()
    {
        return STATUS.NONE;
    }
	#endregion
}
