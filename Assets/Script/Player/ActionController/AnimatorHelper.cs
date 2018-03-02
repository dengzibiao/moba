using System;
using UnityEngine;

public class AnimatorHelper
{
    public AnimatorHelper ()
    {
    }
    private static string att = "Base.Attack";
    private static string att01 = "Base.Attack1";
    private static string att02 = "Base.Attack2";
    private static string att03 = "Base.Attack3";
    private static string att04 = "Base.Attack4";
    private static string att05 = "Base.Attack5";

    private static string skill01 = "Base.Skill1";
    private static string skill02 = "Base.Skill2";
    private static string skill03 = "Base.Skill3";
    private static string skill04 = "Base.Skill4";
    private static string skill05 = "Base.Skill5";
    private static string skill06 = "Base.Skill6";
    private static string idleString = "Base.Idle";
    private static string runString = "Base.Run";
    private static string prepareString = "Base.Prepare";
    private static string hitString = "Base.Hit";
    private static string deadString = "Base.Die";
    private static string downString = "Base.Down";
    private static string summonString = "Base.Summon";
    private static string initString = "Base.Init";
    public static string GetAnimationName (ANIM_INDEX index)
    {
        string result = null;
        switch (index)
        {
            case ANIM_INDEX.IDLE:
                result = idleString;
                break;
            case ANIM_INDEX.RUN:
                result = runString;
                break;
            case ANIM_INDEX.PREPARE:
                result = prepareString;
                break;
            case ANIM_INDEX.HIT:
                result = hitString;
                break;
            case ANIM_INDEX.DOWN:
                result = downString;
                break;
            case ANIM_INDEX.DEAD:
                result = deadString;
                break;
            case ANIM_INDEX.SUMMON:
                result = summonString;
                break;
            case ANIM_INDEX.ATT:
                result = att;
                break;
            case ANIM_INDEX.ATT01:
                result = att01;
                break;
            case ANIM_INDEX.ATT02:
                result = att02;
                break;
            case ANIM_INDEX.ATT03:
                result = att03;
                break;
            case ANIM_INDEX.ATT04:
                result = att04;
                break;
            case ANIM_INDEX.ATT05:
                result = att05;
                break;
            case ANIM_INDEX.SKILL01:
                result = skill01;
                break;
            case ANIM_INDEX.SKILL02:
                result = skill02;
                break;
            case ANIM_INDEX.SKILL03:
                result = skill03;
                break;
            case ANIM_INDEX.SKILL04:
                result = skill04;
                break;
            case ANIM_INDEX.INIT:
                result = initString;
                break;
            case ANIM_INDEX.NONE:
                break;
            default:
                break;
        }
        return result;
    }

    public static bool ShouldPlayNextAnimation (Animator animator, ANIM_INDEX current, float normalized)
    {
        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo (0);
        return  !animator.IsInTransition (0) && info.IsName (GetAnimationName (current)) && info.normalizedTime >= normalized;
    }
}

