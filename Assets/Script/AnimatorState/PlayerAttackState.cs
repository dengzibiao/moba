using UnityEngine;
using System.Collections;

public class PlayerAttackState : PlayerBaseState
{
    private bool mIsTriggerPassive = false;
    private bool mIsTouchRun = false;
    SkillNode mCurSkillNode;
    private int skillIndex;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        if (CheckAnimatorIsAttack(stateInfo))
        {
            pm.canMoveSwitch++;
            pm.canSkillSwitch++;
            animator.speed = cs.attackSpeed;
            mTouchHandler.Clear();
            mSkillTouchIndex = 0;
            mIsStateStart = true;
            mIsTriggerPassive = mIsTouchRun = false;
        }
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
        if (cs == CharacterManager.playerCS & mIsStateStart && CheckAnimatorIsAttack(stateInfo))
        {
            CheckTouchKey();
            GetSkillNodeByStateInfo(stateInfo);
            if (mCurSkillNode != null)
            {
                if (stateInfo.normalizedTime > mCurSkillNode.castBefore)
                {
                    HandleSkill(animator, stateInfo, layerIndex);
                }
                if (skillIndex == 1 || skillIndex == 2)
                {
                    if (stateInfo.normalizedTime > 0.55)
                    {
                        HandleRun(animator, stateInfo, layerIndex);
                    }
                }
            }
            if (!mIsTriggerPassive && stateInfo.normalizedTime > 0.5f && cs.mCurMobalId == MobaObjectID.HeroJiansheng)
            {
                //添加剑圣普攻被动攻击计数判断逻辑
                pm.mCurAttackTime++;
                if (pm.mCurAttackTime == pm.mAmountAttackTime)
                {
                    //添加剑圣普攻被动攻击达到条件后添加武器特效
                    SkillNode mTempSkillNode = GameLibrary.Instance().GetCurrentSkillNodeByCs(cs, 3);
                    if (mTempSkillNode != null)
                    {
                        CharacterData characterData = null;
                        GameLibrary.Instance().SetSkillDamageCharaData(ref characterData, mTempSkillNode, cs);
                        cs.AddBuffManager(mTempSkillNode, cs, characterData);
                    }
                }
                mIsTriggerPassive = true;
            }
        }
    }

    private void HandleRun(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!mIsStateStart || animator.IsInTransition(layerIndex)) return;
        if (mTouchHandler.IsTouched(TOUCH_KEY.Run))
        {
            OnStateExitHandle(stateInfo);
            if (CheckAnimatorIsAttack(stateInfo))
            {
                pm.canMoveSwitch--;
            }
            mIsStateStart = false;
            mIsTouchRun = true;
            pm.Prepare();
        }
    }

    private void HandleSkill(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!mIsStateStart || animator.IsInTransition(layerIndex)) return;
        if (mSkillTouchIndex != 0)
        {
            animator.speed = cs.animSpeed;
            OnStateExitHandle(stateInfo);
            mIsStateStart = false;
            pm.Prepare();
            mFightTouch.OnSkill(mSkillTouchIndex);
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
        if (mIsStateStart)
        {
            OnStateExitHandle(stateInfo);
            mIsStateStart = false;
        }
        if (!mIsTouchRun && CheckAnimatorIsAttack(stateInfo))
        {
            pm.canMoveSwitch--;
        }
        if (!CheckAnimatorIsAttack(animator.GetCurrentAnimatorStateInfo(layerIndex)))
        {
            animator.speed = cs.animSpeed;
            pm.curIndex = -1;
            pm.AttackCount = 1;
            pm.Prepare();
        }
    }

    private void OnStateExitHandle(AnimatorStateInfo stateInfo)
    {
        if (CheckAnimatorIsAttack(stateInfo))
        {
            pm.canSkillSwitch--;
        }
    }

    private void GetSkillNodeByStateInfo(AnimatorStateInfo stateInfo)
    {
        if (mCurSkillNode == null)
        {
            skillIndex = 0;
            int mCurHash = stateInfo.fullPathHash;
            if (mCurHash == pm.attack1_Hash)
            {
                skillIndex = 1;
            }
            else if (mCurHash == pm.attack2_Hash)
            {
                skillIndex = 2;
            }
            else if (mCurHash == pm.attack3_Hash)
            {
                skillIndex = 3;
            }
            if (skillIndex > 0)
            {
                mCurSkillNode = GameLibrary.skillNodeList[cs.CharData.attrNode.skill_id[skillIndex - 1]];
            }
        }
    }
}
