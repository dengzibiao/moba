using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CD = CDTimer.CD;

public class EffectAOECenter : EffectTrackBase
{
    public AoeType aoeType = AoeType.CircleAoe;
    public bool isContinue = false;
    public bool isControl = false;
    public bool isHitAction = false;
    private BetterList<CD> cds = new BetterList<CD>();
    private int mCdIndex;
    private TouchHandler mTouchHandler;
    [HideInInspector]
    public Vector3 joystickRotate, mForceForward;

    public override void Init(SkillNode skillNode, GameObject targetTrans, Transform thisTrans, VoidResult action)
    {
        base.Init(skillNode, targetTrans, thisTrans, action);
        isContinue = mCurSkillNode.interval_time != null && mCurSkillNode.interval_time.Length > 0;
        if (attackerCs.mCurMobalId == MobaObjectID.HeroHuonv && skillNode.site == 4)
        {
            isControl = true;
        }
        Play();
        destoryTime = mCurSkillNode.efficiency_time;
        if (!((attackerCs.mCurMobalId == MobaObjectID.HeroHuonv && mCurSkillNode.site == 1) ||
            (attackerCs.mCurMobalId == MobaObjectID.HeroMeidusha && mCurSkillNode.site == 3) ||
            (attackerCs.GetMobaName() == "boss_003" && mCurSkillNode.site == 2)))
        {
            Destroy(gameObject, destoryTime);
        }
    }

    void Awake()
    {
        FindPosTrans(transform);
        mTouchHandler = TouchHandler.GetInstance();
    }

    //播放特效
    public virtual void Play()
    {
        if (!isContinue)
        {
            if ((attackerCs.mCurMobalId == MobaObjectID.HeroXiongmao && mCurSkillNode.site == 1)
                || (attackerCs.mCurMobalId == MobaObjectID.HeroHuonv && mCurSkillNode.site == 3)
                || (attackerCs.mCurMobalId == MobaObjectID.HeroTongkunvwang && mCurSkillNode.site == 4)
                || (attackerCs.mCurMobalId == MobaObjectID.HeroShawang && mCurSkillNode.site == 1)
                || (attackerCs.mCurMobalId == MobaObjectID.HeroShengqi && mCurSkillNode.site == 1)
                || (attackerCs.mCurMobalId == MobaObjectID.HeroChenmo && mCurSkillNode.site == 1)
                || (attackerCs.mCurMobalId == MobaObjectID.HeroShenniu && mCurSkillNode.site == 3)
                || (attackerCs.mCurMobalId == MobaObjectID.HeroMori && mCurSkillNode.site == 2)
                || (attackerCs.mCurMobalId == MobaObjectID.HeroXiaoxiao && mCurSkillNode.site == 3)
                || (attackerCs.mCurMobalId == MobaObjectID.HeroXiaolu && mCurSkillNode.site == 3)
                || (attackerCs.mCurMobalId == MobaObjectID.HeroLuosa && (mCurSkillNode.site == 2 || mCurSkillNode.site == 4))
                || GameLibrary.Instance().CheckNotHeroBoss(attackerCs)
                || GameLibrary.Instance().CheckIsEliteMonster(attackerCs))
            {
                attackerCs.HitActionDelegate += SingleHitDamage;
            }
            else
            {
                if (attackerCs.mCurMobalId == MobaObjectID.HeroXiongmao && mCurSkillNode.site == 4)
                {
                    CDTimer.GetInstance().AddCD(0.8f, HitTarget, 1);
                }
                else
                {
                    Hit(GetDamageTarget());
                }
            }
        }
        else
        {
            cds.Clear();
            int count = mCurSkillNode.interval_time.Length;
            for (int i = 0; i < count; i++)
            {
                if (i == count - 1 && attackerCs.mCurMobalId == MobaObjectID.HeroHuonv && mCurSkillNode.site == 4)
                {
                    cds.Add(CDTimer.GetInstance().AddCD(mCurSkillNode.interval_time[i], DestoryMe, 1));
                }
                cds.Add(CDTimer.GetInstance().AddCD(mCurSkillNode.interval_time [i], HitTarget, 1));
            }
        }
    }

    private void SingleHitDamage(long skillId)
    {
        if (attackerCs != null && skillId == mCurSkillNode.skill_id)
        {
            attackerCs.HitActionDelegate -= SingleHitDamage;
            if (this != null)
            {
                isHitAction = true;
                Hit(GetDamageTarget());
            }
        }
    }

    private void HitTarget(int count, long id)
    {
        if (this != null)
        {
            for (int i = 0; i < cds.size; i++)
            {
                if (cds [i].Id == id)
                {
                    mCdIndex = i;
                    break;
                }
            }
            mAllMonsters.Clear();
            Hit(GetDamageTarget());
        }
    }

    public override void Hit(List<GameObject> monsters)
    {
        if (mCurSkillNode.damage_ratio != null && mCurSkillNode.damage_ratio.Length > mCdIndex && mCurSkillNode.damage_ratio[mCdIndex] != 1)
        {
            CharacterData mTempData = characterData.Clone();
            mTempData.skill_Damage[0] *= mCurSkillNode.damage_ratio[mCdIndex];
            for (int i = 0; i < monsters.Count; i++)
            {
                monsters[i].GetComponentInParent<CharacterState>().HitBy(mCurSkillNode, attackerCs, mTempData);
            }
        }
        else if (attackerCs.mCurMobalId == MobaObjectID.HeroLuosa && mCurSkillNode.site == 4)
        {
            for (int i = 0; i < monsters.Count; i++)
            {
                CharacterState mCs = monsters[i].GetComponent<CharacterState>();
                if (mCs == null) continue;
                int mLoseBloodRatio = Mathf.CeilToInt((mCs.maxHp - mCs.currentHp) * 100f / mCs.maxHp);
                CharacterData mTempData = characterData.Clone();
                mTempData.skill_Damage[0] = mCurSkillNode.GetSkillBattleValueByLuosa(0, mTempData, mLoseBloodRatio);
                monsters[i].GetComponentInParent<CharacterState>().HitBy(mCurSkillNode, attackerCs, mTempData);
            }
        }
        else
        {
            base.Hit(monsters);
        }
    }

    public void DestoryMe(int count, long id)
    {
        if (this != null)
        {
            Destroy(gameObject);
        }
    }

    void OnDestory()
    {
        if (attackerCs != null && attackerCs.HitActionDelegate != null)
        {
            attackerCs.HitActionDelegate -= SingleHitDamage;
        }
    }

    public virtual void Update()
    {
        if (mCurSkillNode.effect_time != 0 && attackerCs != null && (attackerCs.isDie || !GameLibrary.Instance().CanControlSwitch(attackerCs.pm)))
        {
            //打断声音
            AudioController.Instance.StopEffectSound(attackerCs);
            if ((attackerCs.mCurMobalId == MobaObjectID.HeroHuonv && mCurSkillNode.site == 4) ||
                (attackerCs.GetMobaName().Equals("boss_003") && mCurSkillNode.site == 3) ||
                (attackerCs.GetMobaName().Equals("boss_006") && mCurSkillNode.site == 4))
            {
                Effect_Quartz ef = attackerCs.emission.GetComponent<Effect_Quartz>();
                string key = "skill" + mCurSkillNode.site;
                if (ef != null && ef.dic.ContainsKey(key))
                {
                    ef.dic[key].SetActive(false);
                }
            }
            Destroy(gameObject);
            return;
        }
    }

    public virtual void FixedUpdate()
    {
        if (isControl && attackerCs != null && !attackerCs.isDie)
        {
            joystickRotate = attackerCs.pm.isAutoMode ? mForceForward : mTouchHandler.mOffset;
            //Vector3 mCurOffset = mTouchHandler.mOffset;
            //if (Camera.main != null)
            //{
            //    Quaternion q = Quaternion.Inverse(Camera.main.transform.rotation);
            //    mCurOffset = q * mCurOffset;
            //}
            //joystickRotate = 0f;
            //if (mCurOffset.x != 0)
            //{
            //    joystickRotate = mCurOffset.x > 0 ? 1 : - 1;
            //}
            //attackerCs.transform.Rotate(0, joystickRotate * 60f * Time.deltaTime, 0, Space.Self);
            SetAttackerRotate(joystickRotate);
        }
    }

    private void SetAttackerRotate(Vector3 v)
    {
        if (v == Vector3.zero) return;
        Vector3 q = Quaternion.Euler(0, 0, 0) * v;
        Quaternion qq = Quaternion.LookRotation(q);
        Transform playerTrans = attackerCs.transform;
        playerTrans.rotation = Quaternion.Lerp(playerTrans.rotation, qq, Time.deltaTime * 1f);
        transform.rotation = attackerCs.transform.rotation;
    }

    private List<GameObject> GetDamageTarget()
    {
        List<GameObject> resultGo = new List<GameObject>();
        switch (aoeType)
        {
            case AoeType.CircleAoe:
                resultGo = GetTargetByCondition(CheckHitCondition, distance, pos);
                break;
            case AoeType.RectAoe:
                resultGo = GetTargetByCondition(CheckHitCondition, distance, pos, AoeType.RectAoe);
                break;
            default:
                break;
        }
        return resultGo;
    }
}
