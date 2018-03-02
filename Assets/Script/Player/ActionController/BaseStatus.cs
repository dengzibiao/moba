using System;
using System.Collections.Generic;
using UnityEngine;
using Tianyu;

public abstract class BaseStatus : IStatus
{
    protected IActorController mController;
    protected ITouchHandler mTouchHandler;
    protected FightTouch fightTouch;

    public delegate bool Condition();

    private Dictionary<Condition, STATUS> mCommonConditions = new Dictionary<Condition, STATUS>();
    protected StateHelper mStateHelper;
    protected static bool shouldMove = false;
    protected static int mCurIndex;
    protected static bool isUseSkill = false;

    #region IAILogic implementation
    void IStatus.Init(IActorController controller, StateHelper helper)
    {
        mController = controller;
        mStateHelper = helper;
        mTouchHandler = TouchHandler.GetInstance();
        fightTouch = FightTouch._instance;
        AddCondition(IsUnderAttack, STATUS.HIT);
        AddCondition(IsControlPassive, STATUS.IDLE);//stop the state matchine update by using idle state anyway
        AddCondition(IsCanNotControl, STATUS.CANNOTCONTROL);
    }

    public virtual void OnLeave(STATUS next)// for subclass override
    {

    }

    public virtual bool OnEnter(STATUS last)
    {
        return true;
    }

    public virtual void UpdateLogic()
    {
    }

    public virtual STATUS GetNextStatus()
    {
        List<Condition> keyList = new List<Condition>(mCommonConditions.Keys);
        for (int i = 0; i < keyList.Count; i++) {
            Condition condition = keyList[i];
            if (condition()) {
                STATUS mStatus = mCommonConditions[condition];
                if (mStatus == STATUS.ATTACK01 || mStatus == STATUS.ATTACK02 || mStatus == STATUS.ATTACK03 || 
                    mStatus == STATUS.SKILL01 || mStatus == STATUS.SKILL02 || mStatus == STATUS.SKILL03 || mStatus == STATUS.SKILL04)
                {
                    STATUS mCheckStatus = STATUS.NONE;
                    switch (mStatus)
                    {
                        case STATUS.ATTACK01:
                        case STATUS.ATTACK02:
                        case STATUS.ATTACK03:
                            mCheckStatus = GetPlayerUseSkill(0);
                            break;
                        case STATUS.SKILL01:
                            mCheckStatus = GetPlayerUseSkill(1);
                            break;
                        case STATUS.SKILL02:
                            mCheckStatus = GetPlayerUseSkill(2);
                            break;
                        case STATUS.SKILL03:
                            mCheckStatus = GetPlayerUseSkill(3);
                            break;
                        case STATUS.SKILL04:
                            mCheckStatus = GetPlayerUseSkill(4);
                            break;
                        default:
                            break;
                    }
                    return mCheckStatus != STATUS.NONE ? (mCheckStatus == STATUS.IDLE ? STATUS.NONE : mCheckStatus) : mStatus;
                }
                else
                {
                    return mStatus;
                }
            }
        }
        return STATUS.NONE;
    }
    #endregion

    public void AddCondition(Condition c, STATUS s)
    {
        if (!mCommonConditions.ContainsKey(c))
            mCommonConditions.Add(c, s);
    }

    public void RemoveCondition(Condition c)
    {
        if (mCommonConditions.ContainsKey(c))
            mCommonConditions.Remove(c);
    }

    public bool IsUnderAttack()
    {
        return mController.UnderAttack;
    }

    public bool IsCanNotControl()
    {
        return mController.GetEffectOfBuff() == STATUS.CANNOTCONTROL;
    }

    public bool IsCanNotMove()
    {
        return mController.GetEffectOfBuff() == STATUS.CANNOTMOVE;
    }

    public bool IsCanNotSkill()
    {
        return mController.GetEffectOfBuff() == STATUS.CANNOTSKILL;
    }

    public bool IsCanNotAttack()
    {
        return mController.GetEffectOfBuff() == STATUS.CANNOTATTACK;
    }

    public bool IsPlayerAttack()
    {
        return mTouchHandler.IsTouched(TOUCH_KEY.Attack) && IsPlayerCanUseSkill(0) && !IsCanNotAttack();
    }

    public bool IsDead()
    {
        return mController.IsDead();
    }

    public bool IsControlPassive()
    {
        CharacterState st = (mController as CharacterState);
        if (st != null)
        {
            return !st.pm.canAttack && !st.pm.canSkill && !st.pm.canMove;
        }
        return false;
    }

    public bool IsPlayerCastSkill1()
    {
        return mTouchHandler.IsTouched(TOUCH_KEY.Skill1) && IsPlayerCanUseSkill(1) && !IsCanNotSkill();
    }

    public bool IsPlayerCastSkill2()
    {
        return mTouchHandler.IsTouched(TOUCH_KEY.Skill2) && IsPlayerCanUseSkill(2) && !IsCanNotSkill();
    }

    public bool IsPlayerCastSkill3()
    {
        return mTouchHandler.IsTouched(TOUCH_KEY.Skill3) && IsPlayerCanUseSkill(3) && !IsCanNotSkill();
    }

    public bool IsPlayerCastSkill4()
    {
        return mTouchHandler.IsTouched(TOUCH_KEY.Skill4) && IsPlayerCanUseSkill(4) && !IsCanNotSkill();
    }

    public bool IsPlayerCanUseSkill(int index)
    {
        if (RoleEnum.IsPlayer(mController.GetRoleType()))
        {
            bool canUse = false;
            if (index == 0)
            {
                canUse = true;
            }
            else
            {
                int skillLength = Globe.Heros()[0].node.skill_id.Length;
                SkillNode skillNode = GetSkillNodeByIndex(skillLength - 1 - (4 - index) % 4);
                canUse = !fightTouch.GetSkillBtn(index).isCD;
            }
            return !fightTouch.isLock && !fightTouch.allSkillCD && canUse;
        }
        return true;
    }

    private STATUS GetPlayerUseSkill(int index)
    {
        if (RoleEnum.IsPlayer(mController.GetRoleType()))
        {
            if (index == 0)
            {
                int attackCount = 0;
                switch (mStateHelper.CurStatus)
                {
                    case STATUS.ATTACK01:
                        attackCount = 0;
                        break;
                    case STATUS.ATTACK02:
                        attackCount = 1;
                        break;
                    case STATUS.ATTACK03:
                        attackCount = 2;
                        break;
                    case STATUS.ATTACK04:
                        attackCount = 3;
                        break;
                    default:
                        break;
                }
                SkillNode skillNode = GetSkillNodeByIndex(attackCount);
                return PlayerSkill(skillNode, attackCount, false);
            }
            else
            {
                int skillLength = Globe.Heros()[0].node.skill_id.Length;
                SkillNode skillNode = GetSkillNodeByIndex(skillLength - 1 - (4 - index) % 4);
                return PlayerSkill(skillNode, index, true);
            }
        }
        return STATUS.NONE;
    }

    SkillNode GetSkillNodeByIndex(int skillIndex)
    {
        return FSDataNodeTable<SkillNode>.GetSingleton().DataNodeList[Globe.Heros()[0].node.skill_id[skillIndex]];
    }

    public STATUS PlayerSkill(SkillNode skillNode, int index, bool useSkill)
    {
        GameObject player = CharacterManager.player;
        if (player != null && !player.GetComponent<PlayerMotion>().canAttack)
            return STATUS.NONE;
        shouldMove = false;
        CharacterManager.playerCS.mCurSkillNode = skillNode;
        mCurIndex = index;
        isUseSkill = useSkill;
        switch (skillNode.target)
        {
            //不需要目标，随意释放
            case TargetState.None:
                if (CharacterManager.playerCS.attackTarget != null && skillNode.dist != 0)
                {
                    shouldMove = true;
                    if (mStateHelper.CurStatus != STATUS.RUN)
                    {
                        return STATUS.RUN;
                    }
                }
                break;
            //需要目标，没有目标无法施法，有目标先跑向目标满足施法距离再施法
            case TargetState.Need:
                if (CharacterManager.playerCS.attackTarget != null)
                {
                    shouldMove = true;
                    if (mStateHelper.CurStatus != STATUS.RUN)
                    {
                        return STATUS.RUN;
                    }
                }
                else
                {
                    return STATUS.IDLE;
                }
                break;
            default:
                break;
        }
        return STATUS.NONE;
    }

    protected int GetAnimIndex(ANIM_INDEX index)
    {
        switch (index)
        {
            case ANIM_INDEX.SKILL01:
                return 1;
            case ANIM_INDEX.SKILL02:
                return 2;
            case ANIM_INDEX.SKILL03:
                return 3;
            case ANIM_INDEX.SKILL04:
                return 4;
            default:
                break;
        }
        return 0;
    }

    protected STATUS GetSkillStatusByIndex(int index)
    {
        switch (index)
        {
            case 1:
                return STATUS.SKILL01;
            case 2:
                return STATUS.SKILL02;
            case 3:
                return STATUS.SKILL03;
            case 4:
                return STATUS.SKILL04;
            default:
                break;
        }
        return STATUS.NONE;
    }

    public bool IsPlayerCastSummon1()
    {
        return mTouchHandler.IsTouched(TOUCH_KEY.Summon1) && IsPlayerCanUseSummon(1);
    }

    public bool IsPlayerCastSummon2()
    {
        return mTouchHandler.IsTouched(TOUCH_KEY.Summon2) && IsPlayerCanUseSummon(2);
    }

    public bool IsPlayerCastSummon3()
    {
        return mTouchHandler.IsTouched(TOUCH_KEY.Summon3) && IsPlayerCanUseSummon(3);
    }

    private bool IsPlayerCanUseSummon(int index)
    {
        return fightTouch.IsSummonCanUse(index, null);
    }

    protected void AddPlayerSkillConditions ()
    {
        AddCondition(IsPlayerCastSkill1, STATUS.SKILL01);
        AddCondition(IsPlayerCastSkill2, STATUS.SKILL02);
        AddCondition(IsPlayerCastSkill3, STATUS.SKILL03);
        AddCondition(IsPlayerCastSkill4, STATUS.SKILL04);
        AddCondition(IsPlayerCastSummon1, STATUS.SUMMON1);
        AddCondition(IsPlayerCastSummon2, STATUS.SUMMON2);
        AddCondition(IsPlayerCastSummon3, STATUS.SUMMON3);
    }
}
