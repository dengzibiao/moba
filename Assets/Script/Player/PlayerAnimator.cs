using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerAnimator : MonsterAnimator
{
    [HideInInspector]
    public int attack1_Hash, attack2_Hash, attack3_Hash;
    [HideInInspector]
    public int Skill1_Hash, Skill2_Hash, Skill3_Hash, Skill4_Hash;

    public override void Awake()
    {
        base.Awake();
        attack1_Hash = Animator.StringToHash("Base.Attack1");
        attack2_Hash = Animator.StringToHash("Base.Attack2");
        attack3_Hash = Animator.StringToHash("Base.Attack3");
        Skill1_Hash = Animator.StringToHash("Base.Skill1");
        Skill2_Hash = Animator.StringToHash("Base.Skill2");
        Skill3_Hash = Animator.StringToHash("Base.Skill3");
        Skill4_Hash = Animator.StringToHash("Base.Skill4");
    }

    public override void Run()
    {
        PlayRunMusic(true);
        RandomPlayDialogMusic(mRunDlgRatio);
        base.Run();
        SetSkillIndex(9);
    }
    public override void DieToPrepare()
    {
        base.DieToPrepare();
        SetSkillIndex(11);
    }
    public override void Prepare()
    {
        if (cs.isDie/* || GetCurrentHash() == attack1_Hash || GetCurrentHash() == attack2_Hash || GetCurrentHash() == attack3_Hash*/) return;
        base.Prepare();
        SetSkillIndex(11);
    }

    public override void Hit()
    {
        ani.SetTrigger("Hit");
        AudioController.Instance.PlayEffectSound("hit", cs);
        SetSkillIndex(10);
    }

    public void Skillbyid(long skillid)
    {
        //if (!(canSkill && canSkillSwitch <= 0) || !canControl) return;
        ////技能延时buff释放技能buff生效
        //List<SkillBuff> mCurSkillBuff = SkillBuffManager.GetInst().GetSkillBuffListByCs(cs).FindAll(a => a.node.type == BuffType.DelayEffective && a.node.buffActionType == buffActionType.skill);
        //for (int i = 0; i < mCurSkillBuff.Count; i++)
        //{
        //    SkillBuffManager.GetInst().RemoveCalculateCurTargetProp(cs, mCurSkillBuff[i]);
        //}
        //if (index == 4)
        //{
        //    AngerPoint._instance.ChangePoint(-(cs.mCurSkillNode == null ? 2 : cs.mCurSkillNode.energy));
        //}
        //GameLibrary.Instance().BrokenInvisibility(cs);
        //if (gameObject == CharacterManager.player)
        //{
        //    long keyId = 0;
        //    if (cs.attackTarget != null)
        //    {
        //        if (cs != null)
        //        {
        //            keyId = cs.keyId;
        //        }
        //    }
        //    ClientSendDataMgr.GetSingle().GetWalkSend().SendAttack(keyId, index + 3);
        //    //Debug.LogError( "~~~~~~~~~~~~~~~~~~~~~发送技能攻击" );
        //}
        //if ((cs.mCurMobalId == MobaObjectID.HeroShangjinlieren && index == 2) ||
        //    (cs.mCurMobalId == MobaObjectID.HeroXiaohei && index == 3))
        //{
        //    SetNoAnimatorSkill(index);
        //    AudioController.Instance.PlayEffectSound(cs.GetMobaName() + "/skill" + index, cs);
        //}
        //else
        //{
        //    SetSkillAnimator(index);
        //}
    }
    public void Skill(int index)
    {
        if (cs.isDie || !CanSkillState()) return;
        //技能延时buff释放技能buff生效
        List<SkillBuff> mCurSkillBuff = SkillBuffManager.GetInst().GetSkillBuffListByCs(cs).FindAll(a => a.node.type == BuffType.DelayEffective && a.node.buffActionType == buffActionType.skill);
        for (int i = 0; i < mCurSkillBuff.Count; i++)
        {
            SkillBuffManager.GetInst().RemoveCalculateCurTargetProp(cs, mCurSkillBuff[i]);
        }
        //if (index == 4)
        //{
        //    AngerPoint._instance.ChangePoint(-(cs.mCurSkillNode == null ? 2 : cs.mCurSkillNode.energy));
        //}
        GameLibrary.Instance().BrokenInvisibility(cs);
        AddPassiveBuff(index);
        if (gameObject == CharacterManager.player)
        {
            uint keyId = 0;
            if (cs.attackTarget != null)
            {
                if (cs != null)
                {
                    keyId = cs.keyId;
                }
            }
            ClientSendDataMgr.GetSingle().GetWalkSend().SendAttack(keyId, index + 3);
            //Debug.LogError( "~~~~~~~~~~~~~~~~~~~~~发送技能攻击" );
        }
        if (GameLibrary.Instance().CheckModelIsBoss006(cs) && index == 3)
        {
            transform.position = cs.attackTarget.transform.position;
        }
        if ((GameLibrary.Instance().CheckModelIsBoss006(cs) && index == 2))
        {
            canSkill = false;
            canAttack = false;
            ++mCurSkillTime;
            SetNoAnimatorSkill(index);
            Invoke("ForceSkill", 1.0f);
        }
        else if ((cs.mCurMobalId == MobaObjectID.HeroShangjinlieren && index == 2) ||
                 (cs.mCurMobalId == MobaObjectID.HeroXiaohei && index == 3) ||
                 (cs.mCurMobalId == MobaObjectID.HeroHuonv && index == 1) ||
                 (cs.mCurMobalId == MobaObjectID.HeroChenmo && index == 2) ||
                 (cs.mCurMobalId == MobaObjectID.HeroMeidusha && index == 3) ||
                 (cs.mCurMobalId == MobaObjectID.HeroShenling && index == 3) ||
                 (cs.mCurMobalId == MobaObjectID.HeroLuosa && index == 3))
        {
            SetNoAnimatorSkill(index);
        }
        else
        {
            SetSkillAnimator(index);
        }
        SkillNode mCurSkillNode = GameLibrary.Instance().GetCurrentSkillNodeByCs(cs, index);
        if (GameLibrary.Instance().CheckNotHeroBoss(cs) && cs.attackTarget != null)
        {
            cs.SetForward();
        }
        else
        {
            cs.AttackSetForward(mCurSkillNode);
        }
        if (GameLibrary.Instance().CheckNotHeroBoss(cs))
        {
            if (mCurSkillNode != null && mCurSkillNode.alertedType != 0)
            {
                EffectBossWarn bossWarn = NGUITools.AddMissingComponent<EffectBossWarn>(gameObject);
                bossWarn.Init(mCurSkillNode, null, transform, null);
            }
        }
        RandomPlayDialogMusic(mSkillDlgRatio);
    }

    private void AddPassiveBuff(int index)
    {
        if (cs.mCurMobalId == MobaObjectID.HeroJiansheng && index == 3 && !isTriggerPassive)
        {
            SkillNode mCurSkillNode = GameLibrary.Instance().GetCurrentSkillNodeByCs(cs, index);
            if (mCurSkillNode != null)
            {
                CharacterData characterData = null;
                GameLibrary.Instance().SetSkillDamageCharaData(ref characterData, mCurSkillNode, cs);
                cs.AddBuffManager(mCurSkillNode, cs, characterData);
            }
        }
    }

    private void ForceSkill()
    {
        mCurSkillTime = 0;
        canSkill = true;
        canAttack = true;
        SetNoAnimatorSkill(2);
    }

    private void SetNoAnimatorSkill(int index)
    {
        ani.SetInteger("Attack", 0);
        cs.Attack("skill" + index);
        AudioController.Instance.PlayEffectSound(cs.GetMobaName() + "/skill" + index, cs);
    }

    public void SetSkillAnimator(int index)
    {
        ani.SetInteger("Speed", 0);
        ani.SetInteger("Prepare", 0);
        ani.SetTrigger("Skill" + index);
        SetSkillIndex(index);
    }

    private void SetSkillIndex(int index)
    {
        SkillStatus skillStatus = SkillStatus.idle;
        switch (index)
        {
            case 1:
                skillStatus = SkillStatus.skill1;
                break;
            case 2:
                skillStatus = SkillStatus.skill2;
                break;
            case 3:
                skillStatus = SkillStatus.skill3;
                break;
            case 4:
                skillStatus = SkillStatus.skill4;
                break;
            case 5:
                skillStatus = SkillStatus.attack1;
                break;
            case 6:
                skillStatus = SkillStatus.attack2;
                break;
            case 7:
                skillStatus = SkillStatus.attack3;
                break;
            case 8:
                skillStatus = SkillStatus.attack4;
                break;
            case 9:
                skillStatus = SkillStatus.run;
                break;
            case 10:
                skillStatus = SkillStatus.hit;
                break;
            case 11:
                skillStatus = SkillStatus.prepare;
                break;
            default:
                skillStatus = SkillStatus.idle;
                break;
        }
        SetCurCharacterStateStatus(skillStatus);
    }

    private void SetCurCharacterStateStatus(SkillStatus skillStatus)
    {
        cs.mCurSkillStatus = skillStatus;
    }

    public virtual void PlayAnimation(ANIM_INDEX index)
    {
        float length = ani == null ? 1 : ani.GetCurrentAnimatorStateInfo(0).length;
        if (length == 0)
        {
            length = 1;
        }
        PlayAnimation(index, 0.2f / length, 0f);
    }

    public virtual bool ShouldPlayNextAnimation(ANIM_INDEX current)
    {
        AnimatorStateInfo info = ani.GetCurrentAnimatorStateInfo(0);
        switch (current)
        {
            case ANIM_INDEX.IDLE:
            case ANIM_INDEX.PREPARE:
            case ANIM_INDEX.RUN:
                return true;
            case ANIM_INDEX.DEAD:
                return false;
            default:
                return !ani.IsInTransition(0) && info.IsName(AnimatorHelper.GetAnimationName(current)) && info.normalizedTime >= 0.95f;
        }
    }

    public virtual void PlayAnimation(ANIM_INDEX index, float translationDuration, float normalizedTime)
    {
        if (ani != null)
        {
            ani.CrossFade(AnimatorHelper.GetAnimationName(index), translationDuration, -1, normalizedTime);
            ani.SetInteger(CURRENT_ANIMATION, (int)index);
        }
    }

    public override void Update()
    {
        base.Update();
        AttackTimer += Time.deltaTime;
        Timer();
        CheckAttack();
    }

    float AttackTimer = 0f;
    public int AttackCount = 1;
    public int curIndex = -1;
    float NetAttackTimer = 0f;
    public bool isCheck = true;
    int attackIndex = -1;
    public List<int> attackL = new List<int>();

    public void NetAttack(int index)
    {
        attackL.Add(index);
    }
    void Timer()
    {
        if (!isCheck)
        {
            NetAttackTimer += Time.deltaTime;
            AnimatorStateInfo tempASI = ani.GetCurrentAnimatorStateInfo(0);
            int num = tempASI.nameHash;
            float aniLen = tempASI.length / ani.speed;

            if (NetAttackTimer > 0.5f * aniLen && NetAttackTimer < aniLen)
            {
                NetAttackTimer = 0f;
                if (num == prepare_Hash)
                {
                }
                else
                {
                    attackIndex = -1;
                }
                isCheck = true;
            }
        }
    }
    void CheckAttack()
    {
        if (isCheck)
        {
            if (attackIndex == -1)
            {
                if (attackL.Count > 0)
                {
                    switch (attackL[0])
                    {
                        case 1:
                            {
                                ani.SetInteger("Prepare", 1);
                                ani.SetInteger("Attack", attackL[0]);
                                attackIndex = attackL[0];
                                attackL.RemoveAt(0);
                                isCheck = false;
                            }
                            break;
                        case 2:
                            {
                                ani.SetInteger("Prepare", 1);
                                ani.SetInteger("Attack", attackL[0]);
                                attackIndex = attackL[0];
                                attackL.RemoveAt(0);
                                isCheck = false;
                            }
                            break;
                        case 3:
                            {
                                ani.SetInteger("Prepare", 1);
                                ani.SetInteger("Attack", attackL[0]);
                                attackIndex = attackL[0];
                                attackL.RemoveAt(0);
                                isCheck = false;
                            }
                            break;
                    }
                }
            }
            else
            {
                ani.SetInteger("Prepare", 1);
                ani.SetInteger("Attack", attackIndex);
                isCheck = false;
            }
        }
    }

    public void StartAttack()
    {
        int hash = GetCurrentHash();
        if (hash == prepare_Hash || hash == run_Hash)
        {
            ani.SetInteger("Prepare", 0);
            AttackCount = 1;
            if (AttackTimer > 0)
                AttackTimer = 0;
        }
        else if (hash == attack1_Hash)
        {
            AttackCount = NextAttack(AttackCount);
            AttackCount = AttackCount > 2 ? 2 : AttackCount;
        }
        else if (hash == attack2_Hash)
        {
            AttackCount = NextAttack(AttackCount);
        }
        CheckPassiveAttack(hash);
    }

    int[] atkEliteOrder = new int[] { 1, 2, 2, 1, 1, 2, 1, 1 };
    int atkEliteIndex = 0;
    public void StarEliteAttack()
    {
        int hash = GetCurrentHash();
        if ((hash == prepare_Hash || hash == run_Hash) && !ani.IsInTransition(0))
        {
            ani.SetInteger("Prepare", 0);
            AttackCount = atkEliteOrder[atkEliteIndex];
            atkEliteIndex = atkEliteIndex >= atkEliteOrder.Length - 1 ? atkEliteIndex = 0 : ++atkEliteIndex;
            if (AttackTimer > 0)
                AttackTimer = 0;
        }
        CheckPassiveAttack(hash);
    }

    private void CheckPassiveAttack(int hash)
    {
        if (ShouldTriggerPassive())
        {
            bool mHandlePassive = false;
            float aniLen = ani.GetCurrentAnimatorStateInfo(0).normalizedTime;
            if ((hash == prepare_Hash || hash == run_Hash) ||
                ((hash == attack1_Hash || hash == attack2_Hash || hash == attack3_Hash) && (aniLen > 0.5f && aniLen < 1)))
            {
                mHandlePassive = true;
            }
            if (mHandlePassive)
            {
                SetPassiveSkill();
                RandomPlayDialogMusic(mSkillDlgRatio);
            }
        }
        else
        {
            SetAttackAnimator();
            RandomPlayDialogMusic(mAttackDlgRatio);
        }
    }

    void SendPlayerAttack()
    {
        if (curIndex != AttackCount && gameObject == CharacterManager.player)
        {
            uint keyId = 0;
            if (cs.attackTarget != null)
            {
                if (cs != null)
                {
                    keyId = cs.keyId;
                }
            }
            ClientSendDataMgr.GetSingle().GetWalkSend().SendAttack(keyId, AttackCount);
            //Debug.LogError( "~~~~~~~~~~~~~~~~~~~~~发送普通攻击" + AttackCount.ToString() );
            curIndex = AttackCount;
        }
    }

    int NextAttack(int current)
    {
        float aniLen = ani.GetCurrentAnimatorStateInfo(0).normalizedTime;
        if (aniLen > 0.5f && aniLen < 1)
        {
            AttackTimer = 0f;
            current++;
            if (current > 3)
                current = 3;
        }
        return current;
    }

    public void SetAttackAnimator()
    {
        if ((cs.mCurMobalId == MobaObjectID.HeroMeidusha && AttackCount == 2) ||
            (cs.mCurMobalId == MobaObjectID.HeroShenling && AttackCount == 3)) return;
        SendPlayerAttack();
        ani.SetInteger("Attack", AttackCount);
        ani.SetInteger("Speed", 0);
        SetSkillIndex(AttackCount + 4);
        if (cs.state == Modestatus.Player || cs.state == Modestatus.NpcPlayer)
        {
            cs.SetForward();
        }
    }

    public void SetPassiveSkill()
    {
        Prepare();
        isTriggerPassive = true;
        SetSkillAnimator(3);
    }

    public bool ShouldTriggerPassive()
    {
        return cs.mCurMobalId == MobaObjectID.HeroJiansheng && mCurAttackTime == mAmountAttackTime;
    }

    public void Summon()
    {
        ani.SetTrigger("Summon");
    }
}