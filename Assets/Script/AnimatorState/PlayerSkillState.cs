using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class PlayerSkillState : PlayerBaseState
{
    private List<SkillBuff> mCurAddBuff = new List<SkillBuff>();
    SkillNode mCurSkillNode;
    private int skillIndex;
    private bool mIsTouchRun = false;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (GameLibrary.IsMajorOrLogin())
            return;
        base.OnStateEnter(animator, stateInfo, layerIndex);
        GetSkillNodeByStateInfo(stateInfo);
        pm.canAttack = false;
        pm.canSkill = false;
        pm.canMoveSwitch++;
        pm.canAttackSwitch++;
        pm.canSkillSwitch++;
        mSkillTouchIndex = 0;
        mIsStateStart = true;
        mIsTouchRun = false;
        if (!(cs.mCurMobalId == MobaObjectID.HeroJiansheng && skillIndex == 4 && pm.mCurSkillTime != 0))
        {
            mCurAddBuff.Clear();
        }
        //判断是否有跟随动作的buff
        if (GameLibrary.Instance().CheckExistSpecialBySkillNode(mCurSkillNode))
        {
            if (!(skillIndex == 4 && cs.mCurMobalId == MobaObjectID.HeroJiansheng && pm.mCurSkillTime != 0))
            {
                mCurAddBuff = mCurSkillNode.AddSpecialBuffs(cs);
            }
        }
        if (cs.mCurMobalId == MobaObjectID.HeroJiansheng)
        {
            if (skillIndex == 3)
            {
                SkillBuff mTempBuff = SkillBuffManager.GetInst().GetSkillBuffListByCs(cs).FindLast(a => a.id == (long)SkillBuffType.AddAttack && a.attacker == cs);
                mCurAddBuff.Add(mTempBuff);
            }
            else if (skillIndex == 1)
            {
                pm.canMoveSwitch--;
            }
        }
        else if (cs.mCurMobalId == MobaObjectID.HeroLuosa)
        {
            List<SkillBuff> mExistTalentBuff = SkillBuffManager.GetInst().GetSkillBuffListByCs(cs).FindAll(a => a.attacker == cs &&
                ((a.node.type == BuffType.talent && a.node.buffActionType == buffActionType.skill) || (SkillBuffType)a.id == SkillBuffType.AddSkillDamage));
            for (int i = 0; i < mExistTalentBuff.Count; i++)
            {
                mCurAddBuff.Add(mExistTalentBuff[i]);
            }
        }
        else if (cs.mCurMobalId == MobaObjectID.HeroHuanci)
        {
            if (skillIndex == 4)
            {
                pm.canMoveSwitch--;
            }
        }
        else if (cs.mCurMobalId == MobaObjectID.HeroXiongmao)
        {
            if (skillIndex == 1)
            {
                pm.canMoveSwitch--;
            }
        }
        else if (GameLibrary.Instance().CheckModelIsBoss006(cs))
        {
            if (pm.mIsHangIn && !(skillIndex == 2 || skillIndex == 3))
            {
                cs.transform.position = cs.mOriginPos;
            }
        }
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
        if (cs == CharacterManager.playerCS && mIsStateStart)
        {
            CheckTouchKey();
            if (mCurSkillNode != null)
            {
                if (stateInfo.normalizedTime > mCurSkillNode.castBefore)
                {
                    HandleSkill(animator, stateInfo, layerIndex);
                    HandleAttack(animator, stateInfo, layerIndex);
                    HandleRun(animator, stateInfo, layerIndex);
                }
            }
        }
    }

    private void HandleSkill(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!mIsStateStart || animator.IsInTransition(layerIndex)) return;
        if (mSkillTouchIndex != 0)
        {
            animator.speed = cs.animSpeed;
            pm.Prepare();
            OnStateExitHandle();
            pm.canSkill = true;
            mFightTouch.OnSkill(mSkillTouchIndex);
        }
    }

    private void HandleAttack(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!mIsStateStart || animator.IsInTransition(layerIndex)) return;
        if (mTouchHandler.IsTouched(TOUCH_KEY.Attack))
        {
            pm.Prepare();
            OnStateExitHandle();
            pm.canAttack = true;
        }
    }

    private void HandleRun(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!mIsStateStart || animator.IsInTransition(layerIndex)) return;
        if (mTouchHandler.IsTouched(TOUCH_KEY.Run))
        {
            pm.Prepare();
            OnStateExitHandle();
            pm.canMoveSwitch--;
            mIsTouchRun = true;
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (GameLibrary.IsMajorOrLogin())
            return;
        base.OnStateExit(animator, stateInfo, layerIndex);

        GetSkillNodeByStateInfo(stateInfo);
        if (!mIsTouchRun)
        {
            pm.canMoveSwitch--;
        }
        OnStateExitHandle();
        if (cs.mCurMobalId == MobaObjectID.HeroJiansheng)
        {
            if (skillIndex == 4)
            {
                if (mCurSkillNode != null)
                {
                    GameLibrary.Instance().SetCsAttackTargetByChoseTarget(mCurSkillNode, cs);
                }
                if (cs.attackTarget != null && cs.attackTarget.tag != Tag.tower && pm.mCurSkillTime < 5)
                {
                    pm.mCurSkillTime++;
                    pm.SetSkillAnimator(4);
                }
                else
                {
                    pm.mCurSkillTime = 0;
                    pm.canSkill = true;
                    pm.canAttack = true;
                }
            }
            else
            {
                pm.canSkill = true;
                pm.canAttack = true;
                if (skillIndex == 1)
                {
                    pm.canMoveSwitch++;
                }
                else if (skillIndex == 3)
                {
                    if (pm.isTriggerPassive)
                    {
                        pm.isTriggerPassive = false;
                    }
                }
            }
        }
        else
        {
            pm.canSkill = true;
            pm.canAttack = true;
            if (cs.mCurMobalId == MobaObjectID.HeroHuanci)
            {
                if (skillIndex == 4)
                {
                    pm.canMoveSwitch++;
                }
            }
            else if (cs.mCurMobalId == MobaObjectID.HeroXiongmao)
            {
                if (skillIndex == 1)
                {
                    pm.canMoveSwitch++;
                }
            }
            else if (GameLibrary.Instance().CheckModelIsBoss006(cs))
            {
                if (skillIndex == 1)
                {
                    if (!pm.mIsHangIn && !cs.isDie)
                    {
                        pm.canMoveSwitch++;
                        pm.canAttackSwitch++;
                        pm.nav.enabled = false;
                        cs.Invincible = true;
                        pm.mIsHangIn = true;
                        cs.mOriginPos = cs.transform.position;
                        cs.transform.position += Vector3.up * 5f;
                    }
                }
                else if (skillIndex == 3)
                {
                    if (pm.mIsHangIn)
                    {
                        pm.canMoveSwitch--;
                        pm.canAttackSwitch--;
                        cs.Invincible = false;
                        pm.nav.enabled = true;
                        pm.mIsHangIn = false;
                    }
                }
            }
        }
        //动作结束移除跟随动作的buff
        if (!(skillIndex == 4 && cs.mCurMobalId == MobaObjectID.HeroJiansheng && pm.mCurSkillTime != 0))
        {
            for (int i = 0; i < mCurAddBuff.Count; i++)
            {
                SkillBuffManager.GetInst().RemoveCalculateCurTargetProp(cs, mCurAddBuff[i]);
            }
        }
    }

    private void OnStateExitHandle()
    {
        if (mIsStateStart)
        {
            pm.canSkillSwitch--;
            pm.canAttackSwitch--;
            mIsStateStart = false;
        }
    }

    private void GetSkillNodeByStateInfo(AnimatorStateInfo stateInfo)
    {
        if (mCurSkillNode == null)
        {
            skillIndex = 0;
            int mCurHash = stateInfo.fullPathHash;
            if (mCurHash == pm.Skill1_Hash)
            {
                skillIndex = 1;
            }
            else if (mCurHash == pm.Skill2_Hash)
            {
                skillIndex = 2;
            }
            else if (mCurHash == pm.Skill3_Hash)
            {
                skillIndex = 3;
            }
            else if (mCurHash == pm.Skill4_Hash)
            {
                skillIndex = 4;
            }
            mCurSkillNode = GameLibrary.Instance().GetCurrentSkillNodeByCs(cs, skillIndex);
        }
    }

    protected override void CheckTouchKey()
    {
        if (mSkillTouchIndex == skillIndex)
        {
            mSkillTouchIndex = 0;
        }
        base.CheckTouchKey();
    }
}
