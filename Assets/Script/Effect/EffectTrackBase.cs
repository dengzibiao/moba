using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

public delegate void VoidResult(SkillNode effect, CharacterState hitCs);
public class EffectTrackBase : MonoBehaviour
{
    public VoidResult result;//伤害回调
    public Transform hit;//追踪目标点
    public CharacterState mHitTargetCs;//目标者
    public Transform attackerTrans;//攻击者位置
    public CharacterState attackerCs;//攻击者
    public SkillNode mCurSkillNode;//技能数据
    [HideInInspector]
    public List<GameObject> mCurMonsters = new List<GameObject>();//当前碰撞到的怪物
    [HideInInspector]
    public List<GameObject> mAllMonsters = new List<GameObject>();//所有碰撞到的怪物
    public Transform pos;//范围作用点
    public int groupIndex = -1;//当前英雄的队列
    [HideInInspector]
    public string mResourceRoot;//资源路径
    //所有目标层级
    private int layer
    {
        get
        {
            return GameLibrary.GetAllLayer();
        }
    }
    //策划配置数据
    public float distance = 0.1f;//技能作用范围
    public float destoryTime = 6f;  //销毁时间
    public float efficiency_time;//技能作用时间
    public CharacterData characterData;
    public CharacterData onceCharacterData;//神牛的回音击的伤害
    private bool isDetectionNavMesh = false;

    /// <summary>
    /// 技能id，攻击对象，回调
    /// </summary>
    public virtual void Init(SkillNode skillNode, GameObject targetTrans, Transform thisTrans, VoidResult action = null)
    {
        result = action;
        mCurSkillNode = skillNode;
        if (targetTrans != null)
        {
            mHitTargetCs = targetTrans.GetComponent<CharacterState>();
            hit = mHitTargetCs.mHitPoint;
        }
        if (thisTrans != null)
        {
            attackerTrans = thisTrans;
            attackerCs = attackerTrans.GetComponent<CharacterState>();
            mResourceRoot = attackerCs.emission.GetEffectResourceRoot();
            if (attackerCs != null)
                groupIndex = (int)attackerCs.groupIndex;
            GameLibrary.Instance().SetSkillDamageCharaData(ref characterData, skillNode, attackerCs);
            if (attackerCs.mCurMobalId == MobaObjectID.HeroShenniu && mCurSkillNode.site == 4)
            {
                GameLibrary.Instance().SetSkillDamageCharaData(ref onceCharacterData, skillNode, attackerCs);
                onceCharacterData.skill_Damage [0] = skillNode.GetSkillBattleValueByRatio(0, attackerCs.CharData, 0.2f);
            }
        }
        //清除所有collider
        SetColliderEnable(transform);
        //属性设置完毕，开始显示播放
        if (mCurSkillNode != null)
        {
            gameObject.SetActive(true);
        }
        else
        {
            Debug.Log(attackerTrans);
        }
    }

    private void SetColliderEnable(Transform parent)
    {
        Collider[] cols = parent.GetComponentsInChildren<Collider>(true);
        for (int i = 0; i < cols.Length; i++)
        {
            cols[i].isTrigger = true;
        }
    }

    //设置特效位置
    public virtual void SetPosition()
    {
        switch (mCurSkillNode.target)
        {
            case TargetState.None:
                if (null != mHitTargetCs)
                {
                    transform.position = mHitTargetCs.transform.position;
                }
                else
                {
                    transform.position = attackerTrans.position + transform.forward * mCurSkillNode.dist;
                }
                break;
            case TargetState.Need:
                if (null != mHitTargetCs)
                {
                    transform.position = mHitTargetCs.transform.position;
                }
                break;
            default:
                break;
        }
        if (GameLibrary.Instance().CheckModelIsBoss006(attackerCs) && mCurSkillNode.site == 2 && mHitTargetCs != null)
        {
            Vector3 target = mHitTargetCs.transform.position;
            if (attackerCs.pm.mCurSkillTime == 0)
            {
                target += Vector3.forward * 10f;
            }
            else
            {
                target -= Vector3.right * 10f;
            }
            UnityEngine.AI.NavMeshHit navHit;
            if (mHitTargetCs.pm.nav.Raycast(target, out navHit))
            {
                transform.position = navHit.position;
                transform.eulerAngles = (attackerCs.pm.mCurSkillTime == 1) ? new Vector3(0, 90, 0) : new Vector3(0, 180, 0);
            }
        }
    }

    private bool CheckNeedAddDis()
    {
        return mCurSkillNode.skill_type == SkillCastType.CastSkill || mCurSkillNode.skill_type == SkillCastType.TrackSkill || mCurSkillNode.skill_type == SkillCastType.Bounce;
    }

    //获取当前受击集合并加入总受击集合，常用语持续性伤害的伤害获取
    public virtual void GetCommonDamageRange(Transform target)
    {
        mCurMonsters.Clear();
        Collider[] colliders = Physics.OverlapSphere(target.position, distance + (CheckNeedAddDis() ? 0 : GameLibrary.Instance().GetExtendDis(mHitTargetCs)), layer);
        for (int i = 0; i < colliders.Length; i++)
        {
            CharacterState mCurTargetCs = colliders[i].GetComponent<CharacterState>();
            if (mCurTargetCs != null && CheckHitCondition(mCurTargetCs) && CheckInView(mCurTargetCs, attackerCs))
            {
                if (!mAllMonsters.Contains(colliders[i].gameObject))
                {
                    mCurMonsters.Add(colliders[i].gameObject);
                    mAllMonsters.Add(colliders[i].gameObject);
                }
            }
        }
    }

    //通过传入的委托来判断当前的应该将哪些当做作用范围
    public virtual List<GameObject> GetTargetByCondition(Func<CharacterState, bool> func, float mCurDist, Transform target, AoeType aoeType = AoeType.CircleAoe)
    {
        List<GameObject> mTempMonsters = new List<GameObject>();
        Collider[] colliders = new Collider[] { };
        switch (aoeType)
        {
            case AoeType.CircleAoe:
                colliders = Physics.OverlapSphere(target.position, mCurDist + (CheckNeedAddDis() ? 0 : GameLibrary.Instance().GetExtendDis(mHitTargetCs)), layer);
                break;
            case AoeType.RectAoe:
                colliders = Physics.OverlapBox(target.position, new Vector3(mCurSkillNode.aoe_wide / 2, mCurSkillNode.aoe_long / 2, mCurSkillNode.aoe_long / 2 + GameLibrary.Instance().GetExtendDis(mHitTargetCs)), target.rotation, layer);
                break;
            default:
                break;
        }
        for (int i = 0; i < colliders.Length; i++)
        {
            CharacterState mCurTargetCs = colliders[i].GetComponent<CharacterState>();
            if (mCurTargetCs != null && func(mCurTargetCs) && CheckInView(mCurTargetCs, attackerCs))
            {
                mTempMonsters.Add(colliders[i].gameObject);
            }
        }
        return mTempMonsters;
    }

    public virtual List<GameObject> GetTargetByCondition(Func<CharacterState, bool> func, float length, float width, Transform target)
    {
        List<GameObject> mTempMonsters = new List<GameObject>();
        Collider[] colliders = new Collider[] { };
        colliders = Physics.OverlapBox(target.position, new Vector3(width, length, length + GameLibrary.Instance().GetExtendDis(mHitTargetCs)), target.rotation, layer);
        for (int i = 0; i < colliders.Length; i++)
        {
            CharacterState mCurTargetCs = colliders[i].GetComponent<CharacterState>();
            if (mCurTargetCs != null && func(mCurTargetCs) && CheckInView(mCurTargetCs, attackerCs))
            {
                mTempMonsters.Add(colliders[i].gameObject);
            }
        }
        return mTempMonsters;
    }

    public virtual GameObject CheckColiderByCondition(Collider col, Func<CharacterState, bool> func)
    {
        GameObject result = null;
        if (((1 << col.gameObject.layer) & GameLibrary.GetAllLayer()) != 0)
        {
            CharacterState mCurTargetCs = col.GetComponent<CharacterState>();
            if (mCurTargetCs != null && func(mCurTargetCs) && CheckInView(mCurTargetCs, attackerCs))
            {
                result = col.gameObject;
            }
        }
        return result;
    }

    public virtual void Hit(List<GameObject> monsters)
    {
        for (int i = 0; i < monsters.Count; i++)
        {
            monsters[i].GetComponentInParent<CharacterState>().HitBy(mCurSkillNode, attackerCs, characterData);
        }
    }

    //判断目标是否在视野内
    public virtual bool CheckInView(CharacterState mCurTargetCs, CharacterState attackerCs)
    {
        if (mCurSkillNode.length_base != 0)
        {
            Transform mTargetTrans = pos == null ? transform : pos;
            float mLengthBase = mCurSkillNode.length_base / 2 + 0.2f;
            float mRadian = Mathf.PI / 180 * mCurSkillNode.angle / 2;
            float mExtendDis = mLengthBase / Mathf.Tan(mRadian);
            //float mExtendRadius = mLengthBase * mCurSkillNode.max_fly / Mathf.Sin(mRadian);
            Vector3 mCenterPoint = mTargetTrans.position - mTargetTrans.forward * mExtendDis;
            Vector3 direction = mCurTargetCs.transform.position - mCenterPoint;
            Vector3 mOriginDir = mCurTargetCs.transform.position - mTargetTrans.position;
            return (mCurSkillNode.angle == 0 || mCurSkillNode.angle == 360 || new Vector2(direction.x, direction.z) == Vector2.zero) ? true : 
                (Vector3.Angle(mTargetTrans.forward, direction) <= mCurSkillNode.angle / 2 && (Vector3.Angle(mTargetTrans.forward, mOriginDir) <= 90));
        }
        else
        {
            Vector3 direction = mCurTargetCs.transform.position - (characterData.state == Modestatus.Boss ? (pos == null ? transform.position : pos.position) : transform.position);
            if (new Vector2(direction.x, direction.z) == Vector2.zero || mCurSkillNode.angle == 0)
            {
                return true;
            }
            else
            {
                return mCurSkillNode.angle > 0 ? Vector3.Angle(transform.forward, direction) <= mCurSkillNode.angle / 2 :
                    Vector3.Angle(transform.forward, direction) >= (360 + mCurSkillNode.angle) / 2;
            }
        }
    }

    //默认的伤害选择
    public virtual bool CheckHitCondition(CharacterState cs)
    {
        return GameLibrary.Instance().CheckHitCondition(mCurSkillNode, attackerCs, cs) && (
            (mCurSkillNode.isSingle ? (mHitTargetCs == null ? false : mHitTargetCs.transform == cs.transform) : true));
    }

    public virtual GameObject Bomb(Transform parent, string boomName = null)
    {
        GameObject effGo = BattleUtil.AddEffectTo(mResourceRoot + "skill" + mCurSkillNode.site + "_" + (boomName == null ? "hit" : boomName), parent);
        if (effGo != null)
        {
            if (attackerCs != null)
                AudioController.Instance.PlayEffectSound(attackerCs.GetMobaName() + "/Boom", attackerCs);
            effGo.transform.parent = transform.parent;
            effGo.SetActive(true);
            Destroy(effGo, 1.5f);
        }
        return effGo;
    }

    public virtual float GetTargetRadius(CharacterState target)
    {
        float result = 0;
        if (target != null)
        {
            UnityEngine.AI.NavMeshAgent nav = target.GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (nav != null)
            {
                result = nav.radius;
            }
            else
            {
                CharacterController cc = target.GetComponent<CharacterController>();
                result = cc == null ? result : cc.radius;
            }
        }
        return result;
    }

    public virtual void FindPosTrans(Transform parent)
    {
        ExcuteFindPos(parent);
        pos = pos == null ? transform : pos;
    }

    private void ExcuteFindPos(Transform parent)
    {
        if (pos == null)
        {
            if (parent.name.Equals("pos"))
            {
                pos = parent;
            }
            else if (pos == null)
            {
                for (int i = 0; i < parent.childCount; i++)
                {
                    ExcuteFindPos(parent.GetChild(i));
                }
            }
        }
    }

    /// <summary>
    /// 隐藏当前技能产生的hit特效
    /// </summary>
    public void SetChainHitDisplay()
    {
        if (hit != null)
        {
            for (int i = 0; i < hit.childCount; i++)
            {
                Transform childName = hit.GetChild(i);
                if (childName.name.Equals("skill" + mCurSkillNode.site + "_hit(Clone)"))
                {
                    childName.gameObject.SetActive(false);
                }
            }
        }
    }

    public void DetectionNavMesh()
    {
        if (!isDetectionNavMesh && !attackerCs.isDie && GameLibrary.Instance().CanControlSwitch(attackerCs.pm))
        {
            attackerCs.pm.nav.areaMask = attackerCs.pm.moveAreaMask;
            attackerCs.pm.nav.enabled = false;
            attackerCs.pm.nav.enabled = true;
            isDetectionNavMesh = true;
        }
    }
}