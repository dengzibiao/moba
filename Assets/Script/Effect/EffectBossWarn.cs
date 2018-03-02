using UnityEngine;
using System.Collections;

enum BossWarnAoeType
{
    None = 0,                   //不需要
    CircleAoe = 1,              //圆形
    RectAoe = 2,                //矩形
    SectorAOe1 = 3,             //30度扇形
    SectorAoe2 = 4,             //60度扇形
    LadderAoe = 5,              //不在中心点的梯形（死亡之翼）
    SprayIceAoe = 6,            //冰霜巨龙喷冰
    RoundOffAoe = 7,            //冰霜巨龙甩尾
}

public class EffectBossWarn : MonoBehaviour
{
    private GameObject mCirclePrefab, mRectPrefab, mTSectorPrefab, mSSectorPrefab, mLadderPrefab, mSprayIcePrefab, mRoundOffAoefab;
    private BossWarnAoeType aoeType = BossWarnAoeType.CircleAoe;
    private string mEffectResource = "Effect/Prefabs/Monsters/";
    public Transform attackerTrans;//攻击者位置
    public CharacterState attackerCs;//攻击者
    private SkillNode mCurSkillNode;
    private GameObject mCurPrefab;
    private float mDeltaLong;
    private float mMaxDelta, mMaxWide;
    private float mSpeed;
    private CharacterController cc;
    private float mExtendRadius;

    void Awake()
    {
        mCirclePrefab = Resources.Load(mEffectResource + "quan_1") as GameObject;
        mRectPrefab = Resources.Load(mEffectResource + "chang_1") as GameObject;
        mTSectorPrefab = Resources.Load(mEffectResource + "shan30") as GameObject;
        mSSectorPrefab = Resources.Load(mEffectResource + "shan60") as GameObject;
        mLadderPrefab = Resources.Load(mEffectResource + "tixing_01") as GameObject;
        mSprayIcePrefab = Resources.Load(mEffectResource + "shan30_penbing") as GameObject;
        mRoundOffAoefab = Resources.Load(mEffectResource + "shan30_saowei") as GameObject;
        if (CharacterManager.playerCS != null)
        {
            cc = CharacterManager.playerCS.GetComponent<CharacterController>();
        }
    }

    public void Init(SkillNode skillNode, GameObject targetTrans, Transform thisTrans, VoidResult action = null)
    {
        if (CharacterManager.playerCS != null && cc != null)
        {
            mExtendRadius = CharacterManager.playerCS.transform.localScale.z * cc.radius;
        }
        if (skillNode != null)
        {
            mCurSkillNode = skillNode;
        }
        aoeType = (BossWarnAoeType)mCurSkillNode.alertedType;
        if (thisTrans != null)
        {
            attackerTrans = thisTrans;
            attackerCs = attackerTrans.GetComponent<CharacterState>();
        }
        mDeltaLong = 0;
        mCurPrefab = null;
        switch (aoeType)
        {
            case BossWarnAoeType.CircleAoe:
                if (mCirclePrefab != null)
                {
                    mCurPrefab = NGUITools.AddChild(attackerCs.emission.nip, mCirclePrefab);
                    mCurPrefab.transform.localScale = Vector3.zero;
                    mMaxDelta = mCurSkillNode.aoe_long + mExtendRadius;
                }
                break;
            case BossWarnAoeType.RectAoe:
                if (mRectPrefab != null)
                {
                    mCurPrefab = NGUITools.AddChild(attackerCs.emission.nip, mRectPrefab);
                    mMaxDelta = (mCurSkillNode.max_fly + mExtendRadius) * 0.5f;
                    mMaxWide = mCurSkillNode.aoe_long * 2 + mExtendRadius;
                    mCurPrefab.transform.localScale = new Vector3(mMaxWide, 0, 0);
                }
                break;
            case BossWarnAoeType.SectorAOe1:
                if (mTSectorPrefab != null)
                {
                    mCurPrefab = NGUITools.AddChild(attackerCs.emission.nip, mTSectorPrefab);
                }
                break;
            case BossWarnAoeType.SectorAoe2:
                if (mSSectorPrefab != null)
                {
                    mCurPrefab = NGUITools.AddChild(attackerCs.emission.nip, mSSectorPrefab);
                }
                break;
            case BossWarnAoeType.LadderAoe:
                if (mLadderPrefab != null)
                {
                    mCurPrefab = NGUITools.AddChild(attackerCs.emission.nip, mLadderPrefab);
                }
                break;
            case BossWarnAoeType.SprayIceAoe:
                if (mSprayIcePrefab != null)
                {
                    mCurPrefab = NGUITools.AddChild(attackerCs.emission.nip, mSprayIcePrefab);
                }
                break;
            case BossWarnAoeType.RoundOffAoe:
                if (mRoundOffAoefab != null)
                {
                    mCurPrefab = NGUITools.AddChild(attackerCs.emission.nip, mRoundOffAoefab);
                }
                break;
            default:
                break;
        }
        if (mCurPrefab != null)
        {
            mSpeed = mMaxDelta * 2;
            mCurPrefab.transform.position = transform.position;
            mCurPrefab.transform.rotation = transform.rotation;
            Destroy(mCurPrefab, 1f);
        }
    }

    void Update()
    {
        if (mCurPrefab != null)
        {
            if (attackerCs == null || attackerCs.isDie)
            {
                Destroy(mCurPrefab);
            }
            mDeltaLong += mSpeed * Time.deltaTime;
            mDeltaLong = mDeltaLong >= mMaxDelta ? mMaxDelta : mDeltaLong;
            switch (aoeType)
            {
                case BossWarnAoeType.CircleAoe:
                    mCurPrefab.transform.localScale = Vector3.one * mDeltaLong;
                    break;
                case BossWarnAoeType.RectAoe:
                    mCurPrefab.transform.localScale = new Vector3(mMaxWide, 1, mDeltaLong);
                    break;
                case BossWarnAoeType.LadderAoe:
                    mCurPrefab.transform.localScale = Vector3.one;
                    break;
                default:
                    break;
            }
            if (attackerCs != null && attackerCs.emission != null && attackerCs.emission.et != null && attackerCs.emission.et.pos != null && 
                (aoeType == BossWarnAoeType.CircleAoe || aoeType == BossWarnAoeType.SprayIceAoe || aoeType == BossWarnAoeType.RoundOffAoe))
            {
                mCurPrefab.transform.position = attackerCs.emission.et.pos.position;
            }
        }
    }
}
