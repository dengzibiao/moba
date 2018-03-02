using System;
using UnityEngine;

public interface IActorController
{
    short Level {
        get;
        set;
    }

    sbyte IsElite {
        get;
        set;
    }

    bool IsLockedPosition {
        get;
        set;
    }

    int JumpSection {
        get;
        set;
    }

    int JumpSectionCursor {
        get;
        set;
    }

    float MoveFactor {
        get;
        set;
    }

    float GravityFactor {
        get ;
        set ;
    }

    int JumpType {
        set;
        get;
    }

    int MoveType {
        set;
        get;
    }

    bool CanFloat {
        get;
        set;
    }

    bool IsPAState {
        get;
        set;
    }

    STATUS LastSkillOcc {
        get;
        set;
    }

    STATUS LastSkillPro {
        get;
        set;
    }

    bool IsCanNotControl{
        get;
        set;
    }

    void MoveTo (Vector3 pos, float speed) ;

    bool IsDead ();

    bool IsFalling ();

    bool IsSkillOrAttackCoolDown (STATUS status);

    UnityEngine.Animator getAnimator () ;

    float GetSkillStiff ();

    float GetActorHpPercent ();

    void Transmit () ;

    void Transmit (float distance);

    bool InFollowRange ();

    bool InTransmitRange ();

    bool SetAnimatorSpeed (float speed,short level);

    ANIM_INDEX GetCurrentAnimation ();

    float GetCurrentAnimationNormalizedTime ();

    bool UnderAttack { set; get; }

    IActorController FoundTarget ();

    IActorController Target {
        get;
        set;
    }

    void SetDirection (Vector3 dir);

    bool InSkillRange (STATUS s);

    bool InAttackRange ();

    bool IsOutOfPursueRange ();

    Dir PursueTarget ();

    void Move (Vector3 dir);

    /// <summary>
    /// Destroies the game object.
    /// </summary>
    void DestroyGameObject ();

    /// <summary>
    /// Raises the new status event.
    /// </summary>
    /// <param name="newStatus">New status.</param>
    void OnNewStatus (STATUS lastStatus, STATUS newStatus);
    
    bool ShouldPlayNextAnimation (ANIM_INDEX current);

    void StartJump (bool forceJump);

    void PlayAnimation (ANIM_INDEX index);

    void PlayAnimation (ANIM_INDEX index, float translationDuration, float normalizedTime);

    Dir GetDirection ();

    bool IsGrounded ();

    bool IsUnderControl ();

    Vector3 GetPosition ();

    short GetSpawnEnemyType ();

    STATUS GetEffectOfBuff ();

    short GetRoleType ();

    bool ShouldCastSkill (STATUS st);

    bool IsHpLowerThan (int hp);

    bool IsHpReduceThan (int hp);

    int GetSkillId (STATUS skillStatus);
}