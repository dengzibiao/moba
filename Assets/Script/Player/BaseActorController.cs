using UnityEngine;
using System.Collections;
using System;

public abstract class BaseActorController : MonoBehaviour, IActorController
{
    #region IActorController implementation

    public virtual float GetSkillStiff ()
    {
        throw new System.NotImplementedException ();
    }

    public virtual void MoveTo (Vector3 pos, float speed)
    {
        throw new System.NotImplementedException ();
    }

    public virtual bool IsDead ()
    {
        throw new System.NotImplementedException ();
    }

    public virtual bool IsFalling ()
    {
        throw new System.NotImplementedException ();
    }

    public virtual bool IsSkillOrAttackCoolDown (STATUS status)
    {
        throw new System.NotImplementedException ();
    }

    public virtual Animator getAnimator ()
    {
        throw new System.NotImplementedException ();
    }

    public virtual float GetActorHpPercent ()
    {
        throw new System.NotImplementedException ();
    }

    public virtual void Transmit ()
    {
        throw new System.NotImplementedException ();
    }

    public virtual void Transmit (float distance)
    {
        throw new System.NotImplementedException ();
    }

    public virtual bool InFollowRange ()
    {
        throw new System.NotImplementedException ();
    }

    public virtual bool InTransmitRange ()
    {
        throw new System.NotImplementedException ();
    }

    public virtual bool SetAnimatorSpeed (float speed, short level)
    {
        throw new System.NotImplementedException ();
    }

    public virtual ANIM_INDEX GetCurrentAnimation ()
    {
        throw new System.NotImplementedException ();
    }

    public virtual float GetCurrentAnimationNormalizedTime ()
    {
        throw new System.NotImplementedException ();
    }

    public virtual IActorController FoundTarget ()
    {
        throw new System.NotImplementedException ();
    }

    public virtual void SetDirection (Vector3 dir)
    {
        throw new System.NotImplementedException ();
    }

    public virtual bool InSkillRange (STATUS s)
    {
        throw new System.NotImplementedException ();
    }

    public virtual bool InAttackRange ()
    {
        throw new System.NotImplementedException ();
    }

    public virtual bool IsOutOfPursueRange ()
    {
        throw new System.NotImplementedException ();
    }

    public virtual Dir PursueTarget ()
    {
        throw new System.NotImplementedException ();
    }

    public virtual void Move (Vector3 dir)
    {
        throw new System.NotImplementedException ();
    }

    public virtual void DestroyGameObject ()
    {
        throw new System.NotImplementedException ();
    }

    public virtual void OnNewStatus (STATUS lastStatus, STATUS newStatus)
    {
        
    }

    public virtual bool ShouldPlayNextAnimation (ANIM_INDEX current)
    {
        throw new System.NotImplementedException ();
    }

    public virtual void StartJump (bool forceJump)
    {
        throw new System.NotImplementedException ();
    }

    public virtual void PlayAnimation (ANIM_INDEX index)
    {
        throw new System.NotImplementedException ();
    }

    public virtual void PlayAnimation (ANIM_INDEX index, float translationDuration, float normalizedTime)
    {
        throw new System.NotImplementedException ();
    }

    public virtual Dir GetDirection ()
    {
        throw new System.NotImplementedException ();
    }

    public virtual bool IsGrounded ()
    {
        throw new System.NotImplementedException ();
    }

    public virtual bool IsUnderControl ()
    {
        return true;
    }

    public virtual Vector3 GetPosition ()
    {
        throw new System.NotImplementedException ();
    }

    public virtual short GetSpawnEnemyType ()
    {
        throw new System.NotImplementedException ();
    }

    public virtual STATUS GetEffectOfBuff ()
    {
        throw new System.NotImplementedException ();
    }

    public virtual short GetRoleType ()
    {
        throw new System.NotImplementedException ();
    }

    public virtual bool ShouldCastSkill (STATUS st)
    {
        throw new System.NotImplementedException ();
    }

    public virtual bool IsHpLowerThan (int hp)
    {
        throw new System.NotImplementedException ();
    }

    public virtual bool IsHpReduceThan (int hp)
    {
        throw new System.NotImplementedException ();
    }

    public virtual int GetSkillId (STATUS skillStatus)
    {
        throw new System.NotImplementedException ();
    }

    #endregion

    #region IActorController implementation
    public virtual long id {
        get;
        set;
    }

    public virtual short Level {
        get;
        set;
    }

    public virtual sbyte IsElite {
        get;
        set;
    }

    public virtual bool IsLockedPosition {
        get;
        set;
    }

    public virtual int JumpSection {
        get;
        set;
    }

    public virtual int JumpSectionCursor {
        get;
        set;
    }

    public virtual float MoveFactor {
        get;
        set;
    }

    public virtual float GravityFactor {
        get;
        set;
    }

    public virtual int JumpType {
        get;
        set;
    }

    public virtual int MoveType {
        get;
        set;
    }

    public virtual bool CanFloat {
        get;
        set;
    }

    public virtual bool IsPAState {
        get;
        set;
    }

    public virtual bool UnderAttack {
        get;
        set;
    }

    public virtual IActorController Target {
        get;
        set;
    }

    public virtual STATUS LastSkillOcc {
        get;
        set;
    }

    public virtual STATUS LastSkillPro {
        get;
        set;
    }

    public virtual bool IsCanNotControl
    {
        get;
        set;
    }
    #endregion
}
