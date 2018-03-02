using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;
using Tianyu;

public enum Modestatus
{
    Player, NpcPlayer, Monster, Boss, Tower, SummonHero
}

public enum ModelType
{
    Hero, NPC
}

public enum SkillStatus
{
    idle,
    attack1,
    attack2,
    attack3,
    attack4,
    skill1,
    skill2,
    skill3,
    skill4,
    run,
    hit,
    prepare,
}

public enum TargetState
{
    None = 0,
    Need = 1
}
public enum HeroAttackLine
{
    none = 0,
    leftLine = 1,
    midLine = 2,
    RightLine = 3,
}

public class CharacterState : BaseActorController, IStatusHandler, BattleAgent
{
    [HideInInspector]
    public CharacterState LastAttackBy; // 最近一次受到的攻击来自此目标
    [HideInInspector]
    public CharacterState LastAttack; // 最近攻击目标
    [HideInInspector]
    public CharacterState Summon; // 当前召唤出的英雄
    [HideInInspector]
    public CharacterState Master; // 如果为召唤英雄，此值保存召唤者

    CharacterState _attackTarget;
    public CharacterState attackTarget {
        get
        {
            return _attackTarget;
        }
    }
    public void SetAttackTargetTo ( CharacterState target ) {
        _attackTarget = target;
        //if(target != null)
        //    Debug.LogError(name + " ATKtarget set to " + target.name);
        //else
        //    Debug.LogError(name + " ATKtarget set to null ");
    }

    public delegate void LiveEvent(CharacterState mCs);
    public LiveEvent OnDead;
    public LiveEvent OnBorn;
    public LiveEvent OnHit;
    public LiveEvent OnBeAttack;
    public LiveEvent OnTowerAttack;
    public LiveEvent OnShieldOff;
    public LiveEvent OnEnterOver;

    public delegate void OnAttactDamage(CharacterState cs, CharacterState attackerCs, float damage);
    public OnAttactDamage OnAttackDmg;
    public delegate void OnHpDamage(CharacterState cs, CharacterState attackerCs);
    public OnHpDamage OnDamageBy;

    public delegate void HelpEvent(CharacterState mCs, CharacterState targetCs, MobaAIPlayerPriority priotity);
    public HelpEvent OnHelp;
    public System.Action BlinkDelegate, JumpStartDelegate, JumpOverDelegate;
    public System.Action<long> HitActionDelegate;

    [HideInInspector]
    public UISprite MapIcon;
    //public Transform redCircle;
    [HideInInspector]
    public EffectEmission emission;
    Transform emissionPoint, emissionPoint1;
    Renderer[] skins;
    Material[] materials;


    PlayerMotion _pm;
    public PlayerMotion pm
    {
        get
        {
            if (_pm == null)
                _pm = GetComponent<PlayerMotion>();
            return _pm;
        }
    }

    CharacterController _cc;
    public CharacterController cc
    {
        get
        {
            if (_cc == null)
                _cc = GetComponent<CharacterController>();
            return _cc;
        }
    }
    [HideInInspector]
    public float outBaseHpRate = 0.95f;
    [HideInInspector]
    public float fleeHpRate = 0.2f;
    [HideInInspector]
    public float defendHpRate = 0.5f;
    // [HideInInspector]
    public uint keyId; // 玩家ID
    [HideInInspector]
    public int rc;

    public HeroAttackLine atkLine = HeroAttackLine.none;
    public bool isDie { set; get; }
    public bool IsMainHero
    {
        get
        {
            return CharacterManager.playerCS == this;
        }
    }
    public float inBattleTimer;
    public GameObject AddHp;
    public MobaAddLife addLife;
    // 所属阵营: -1中立, 0红队, 1蓝队 
    public UInt32 groupIndex
    {
        get
        {
            if (CharData != null)
                return CharData.groupIndex;//这块野外打怪会报错
            else
                return 0;
        }

    }

    public void SetDead()
    {
        isDie = true;

        PlayerMotion pm = GetComponent<PlayerMotion>();
        if (pm != null)
        {
            pm.Die();
        }
    }

    public void SetBorn(int hp)
    {
        isDie = false;
        InitHp(hp);
        RefreshHpBar();
        PlayerMotion pm = GetComponent<PlayerMotion>();
        if (pm != null)
        {
            pm.DieToPrepare();
        }
    }


    public Modestatus state
    {
        get
        {
            if (CharData != null)
                return CharData.state;//野外打怪会是空值默认是Monster
            else
                return Modestatus.Monster;
        }
    }

    public ModelType type = ModelType.Hero;


    [HideInInspector]
    public MobaObjectID mCurMobalId;
    private string mMobaName = string.Empty;
    public SkillStatus mCurSkillStatus = SkillStatus.idle;
    [HideInInspector]
    public SkillNode mCurSkillNode;
    [HideInInspector]
    public List<CharacterPart> playerPart = new List<CharacterPart>();
    [HideInInspector]
    public List<RolePart> mShowPart = new List<RolePart>();
    private Dictionary<RolePart, GameObject> mPartEffect = new Dictionary<RolePart, GameObject>();

    public MobaAIPlayerPriority priority;
    public bool isNetworking;//战斗是否联网


    public Vector2 Pos
    {
        get
        {
            return new Vector2(transform.position.x, transform.position.z);
        }
    }

    private IActionHandler mActionHandler;

    public IActionHandler ActionHandler
    {
        get
        {
            return mActionHandler;
        }
        set
        {
            mActionHandler = value;
        }
    }
    [HideInInspector]
    public Transform mHeadPoint, mHitPoint, mShadowTrans;
    [HideInInspector]
    public Vector3 mOriginHitPos;
    [HideInInspector]
    public Vector3 mOriginPos;
    [HideInInspector]
    public Vector3 mOriginLocalScale;
    [HideInInspector]
    public Transform mBossShowCamera;
    [HideInInspector]
    public GameObject pet = null;

    GameObject NPCeffect = null;
    [HideInInspector]
    public int mAmountDlg = 1;

    void Awake()
    {
        emission = GetComponentInChildren<EffectEmission>();
        mCurMobalId = GetMobalIdByStr();
        playerPart = new List<CharacterPart>(GetComponentsInChildren<CharacterPart>());
        Transform redCircle = transform.FindChild("Effect_targetselected01");
        if (redCircle != null)
            redCircle.gameObject.SetActive(false);
        AddPartEffect();
        emissionPoint = transform.Find("EmissionPoint");
        emissionPoint1 = transform.Find("EmissionPoint1");
        mHeadPoint = transform.Find("Headbuff");
        mHitPoint = transform.Find("Hit");
        if (mHitPoint != null)
            mOriginHitPos = mHitPoint.localPosition;
        mBossShowCamera = transform.Find("BossShowCamera");
        if (type == ModelType.NPC && transform.Find("IdleEffect"))
            NPCeffect = transform.Find("IdleEffect").gameObject;
    }

    void OnDestroy()
    {
        HidePet();
    }

    public void InitData(CharacterData characterData)
    {
        mOriginLocalScale = transform.localScale;
        isDie = false;
        mShadowTrans = transform.Find("Shadow");
        CharData = characterData;
        moveInitSpeed = moveSpeed = characterData.attrNode.movement_speed;
        attackInitSpeed = attackSpeed = characterData.attrNode.attack_speed;
        InitHp(maxHp);
        RefreshHpBar();

        for (int i = 0; i < Formula.ATTR_COUNT - 3; i++)
        {
            float val = Formula.GetSingleAttribute(characterData, (AttrType)i);
            Formula.SetAttrTo(ref CharData.battleInitAttrs, (AttrType)i, val);
        }

        //if (SceneManager.GetActiveScene().name == GameLibrary.PVP_Zuidui)
        //{
        //    InitHp((int)(_currentHp * 1.5f));
        //}

        if (state == Modestatus.Tower)
            skins = GetComponentsInChildren<MeshRenderer>();
        else
            skins = GetComponentsInChildren<SkinnedMeshRenderer>();
        materials = new Material[skins.Length];
        for (int i = 0; i < skins.Length; i++)
        {
            materials[i] = skins[i].material;
        }
        if (CharData != null && CharData.attrNode != null && CharData.attrNode is HeroAttrNode)
        {
            mAmountDlg = (CharData.attrNode as HeroAttrNode).heroNode.dlgAmount;
        }
        if (OnBorn != null) OnBorn(this);
        OnDead += (cs) => SetPartEffectActive(cs, cs.mShowPart, false);
    }

    public void InitHp(int hp)
    {
        _currentHp = _maxHp = hp;
    }

    #region IActorController
    public void ResetActionHandler(MobaObjectID type, bool changeModelOnly = false)
    {
        IActorController handler = GetPlayerActionHanlder();
        IStatusHandler statushandler = (IStatusHandler)handler;
        mActionHandler = ActionHandlerCreator.CreateCommonActionHandler(handler, statushandler, type);
    }

    private IActorController GetPlayerActionHanlder()
    {
        return GetActionHanlderByType(mCurMobalId);
    }

    private IActorController GetActionHanlderByType(MobaObjectID type)
    {
        return this;
    }
    #endregion IActorController

    #region IStatusHandler implementation
    private STATUS nextStatus;
    public bool ChangeStatusTo(STATUS next)
    {
        if (mActionHandler != null)
        {
            nextStatus = next;
            mActionHandler.GetStateHelper().ChangeStatusTo((STATUS)nextStatus);
            return true;
        }
        return false;
    }
    #endregion

    #region IActorController implementation
    public override short GetRoleType()
    {
        return (short)mCurMobalId;
    }

    public override void Move(Vector3 dir)
    {
        if (pm != null && dir != Vector3.zero)
        {
            pm.Move(dir);
        }
    }

    public override void SetDirection(Vector3 dir)
    {
        if (dir != Vector3.zero)
        {
            CharacterManager.instance.RotatePlayer(dir);
        }
    }

    public override void PlayAnimation(ANIM_INDEX index, float translationDuration, float normalizedTime)
    {
        if (pm != null)
        {
            pm.PlayAnimation(index, translationDuration, normalizedTime);
        }
    }

    public override void PlayAnimation(ANIM_INDEX index)
    {
        if (pm != null)
        {
            pm.PlayAnimation(index);
        }
    }

    public override bool ShouldPlayNextAnimation(ANIM_INDEX index)
    {
        if (pm != null)
        {
            return pm.ShouldPlayNextAnimation(index);
        }
        return false;
    }
    public override float GetCurrentAnimationNormalizedTime()
    {
        if (pm != null)
        {
            return pm.ani.GetCurrentAnimatorStateInfo(0).normalizedTime;
        }
        return pm.ani.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }

    public override Animator getAnimator()
    {
        if (pm != null)
        {
            return pm.ani;
        }
        return null;
    }

    public override STATUS GetEffectOfBuff()
    {
        STATUS status = STATUS.NONE;
        List<SkillBuff> buffs = SkillBuffManager.GetInst().GetSkillBuffListByCs(this);
        if (buffs.Count > 0)
        {
            buffs.Sort((a, b) => b.GetEffectOfBuff() - a.GetEffectOfBuff());
            status = buffs[0].GetEffectOfBuff();
        }
        return status;
    }

    public override bool IsUnderControl()
    {
        return true;
    }

    public override IActorController FoundTarget()
    {
        if (!isDie)
        {
            CharacterManager.instance.GetAttackTarget(CharacterManager.playerCS.TargetRange, CharacterManager.playerCS);
            return attackTarget == null ? null : attackTarget.GetComponent<CharacterState>();
        }
        return null;
    }
    #endregion

    void Update()
    {
        //if (mActionHandler != null && !pm.isAutoMode)
        //    mActionHandler.HandleAction();
        if (inBattleTimer > 0) inBattleTimer -= Time.deltaTime;
    }

    public bool Restore()
    {
        if (canRestore())
        {
            CDTimer.GetInstance().AddCD(1f, (int count, long id) =>
            {
                if (currentHp < maxHp && inBattleTimer <= 0)
                    Hp(-Mathf.CeilToInt(0.1f * maxHp), HUDType.Cure, this == CharacterManager.playerCS);
            }, 3, true);
            return true;
        }
        return false;
    }

    bool canRestore()
    {
        bool playerUICD = false;
        if(state == Modestatus.Player && FightTouch._instance != null)
        {
            playerUICD = FightTouch._instance.skillRestore.isCD;
        }
        return currentHp < maxHp && inBattleTimer <= 0 && !playerUICD;
    }

    public void SetPartEffectActive(CharacterState cs, List<RolePart> rolePart, bool b)
    {
        if (this != null && gameObject != null)
        {
            for (int i = 0; i < rolePart.Count; i++)
            {
                if (mPartEffect.ContainsKey(rolePart[i]))
                {
                    mPartEffect[rolePart[i]].gameObject.SetActive(b);
                }
            }
        }
    }

    List<GameObject> effectGo = new List<GameObject>();
    //给模型添加部位特效
    void AddPartEffect()
    {
        effectGo.Clear();
        string loadPath = (type == ModelType.Hero ? GameLibrary.Effect_Hero : GameLibrary.Effect_NPC) + emission.name.Split('&')[1] + "/";
        bool isAppeared = ShowAppearedEffect(loadPath);
        for (int i = 0; i < playerPart.Count; i++)
        {
            string loadName = "";
            switch (playerPart[i].mRolePart)
            {
                case RolePart.Head:
                    loadName = "head";
                    break;
                case RolePart.Body:
                    loadName = "body";
                    break;
                case RolePart.Foot:
                    loadName = "foot";
                    break;
                case RolePart.LeftHand:
                    loadName = "leftHand";
                    break;
                case RolePart.RightHand:
                    loadName = "rightHand";
                    break;
                default:
                    break;
            }

            if (mCurMobalId == MobaObjectID.HeroBingnv)
            {
                GameObject selfGO = Resource.CreatPrefabs(loadName, playerPart[i].gameObject, Vector3.zero, loadPath);
                if (null != selfGO)
                {
                    selfGO.SetActive(false);
                    effectGo.Add(selfGO);
                }
            }

            if (isAppeared && GameLibrary.AddAppearedEffect(mCurMobalId))
                loadName = "Enter_" + loadName;
            GameObject go = Resource.CreatPrefabs(loadName, playerPart[i].gameObject, Vector3.zero, loadPath);
            if (go == null)
                continue;
            else
                go.transform.localRotation = Quaternion.identity;
            if (!mPartEffect.ContainsKey(playerPart[i].mRolePart))
                mPartEffect.Add(playerPart[i].mRolePart, go);
            if (mCurMobalId == MobaObjectID.HeroJumo && (playerPart[i].mRolePart == RolePart.LeftHand || playerPart[i].mRolePart == RolePart.RightHand))
            {
                go.SetActive(false);
            }
            else if (mCurMobalId == MobaObjectID.HeroChenmo && (playerPart[i].mRolePart == RolePart.LeftHand || playerPart[i].mRolePart == RolePart.RightHand))
            {
                go.SetActive(false);
            }
            else if (mCurMobalId == MobaObjectID.HeroJiansheng && playerPart[i].mRolePart == RolePart.RightHand)
            {
                go.SetActive(false);
            }
            else if (mCurMobalId == MobaObjectID.HeroShenling && (playerPart[i].mRolePart == RolePart.LeftHand || playerPart[i].mRolePart == RolePart.RightHand))
            {
                go.SetActive(false);
            }
            else if (mCurMobalId == MobaObjectID.HeroXiaohei && playerPart[i].mRolePart == RolePart.RightHand)
            {
                go.SetActive(false);
            }
            else if (mCurMobalId == MobaObjectID.HeroLuosa && playerPart[i].mRolePart == RolePart.RightHand)
            {
                go.SetActive(false);
            }
            else
            {
                mShowPart.Add(playerPart[i].mRolePart);
            }
            if (type == ModelType.NPC)
            {
                go.SetActive(false);
            }
            if (GameLibrary.IsMajorOrLogin())
            {
                effectGo.Add(go);
                if (GameLibrary.IsAppeared() && mCurMobalId == MobaObjectID.HeroHuonv && (playerPart[i].mRolePart == RolePart.LeftHand || playerPart[i].mRolePart == RolePart.RightHand))
                {
                    go.SetActive(false);
                }
                else if (GameLibrary.IsAppeared() && mCurMobalId == MobaObjectID.HeroXiaohei && playerPart[i].mRolePart == RolePart.RightHand)
                {
                    go.SetActive(false);
                }
                else if (GameLibrary.IsAppeared() && mCurMobalId == MobaObjectID.HeroShenling && playerPart[i].mRolePart == RolePart.RightHand)
                {
                    go.SetActive(true);
                }

                NGUITools.SetChildLayer(go.transform, gameObject.layer);
            }
        }
    }
    GameObject enter;
    bool ShowAppearedEffect(string loadPath)
    {

        if (null == HeroPosEmbattle.instance || HeroPosEmbattle.instance.type != AnimType.Appeared) return false;

        enter = Globe.GetAppearedEffect(transform.name);
        Transform parent = null;
        if (SceneManager.GetActiveScene().name == GameLibrary.UI_Major)
            parent = transform.parent.parent;
        else
            parent = transform.parent;
        if (null == enter)
        {
            enter = Resource.CreatPrefabs("Enter", parent.gameObject, Vector3.zero, loadPath);
            if (null != enter)
            {
                Globe.AddAppearedEffect(transform.name, enter);
                enter.transform.localRotation = Quaternion.Euler(0, 180, 0);
                enter.transform.parent = transform.parent;
                NGUITools.SetChildLayer(enter.transform, 5);
            }
        }
        else
        {
            enter.gameObject.SetActive(false);
            enter.gameObject.SetActive(true);
        }
        if (mCurMobalId == MobaObjectID.HeroChenmo)
        {
            ShowEffectGo(true);
        }
        return true;
    }

    public void ShowNPCEffect(bool isShow)
    {
        if (null != NPCeffect)
        {
            NPCeffect.SetActive(isShow);
        }
        ShowEffectGo(isShow);
    }

    public void EndAppeared(int iEff)
    {
        if (iEff == 0)
        {
            if (mCurMobalId == MobaObjectID.HeroBingnv)
            {
                for (int i = 0; i < effectGo.Count; i++)
                {
                    if (effectGo[i].name.Contains("Enter"))
                        effectGo[i].SetActive(false);
                    else
                        effectGo[i].SetActive(true);
                }
            }
            else
                ShowEffectGo(true);
        }
        else
        {
            if (null != enter)
                enter.SetActive(false);

        }
    }

    void ShowEffectGo(bool isShow)
    {
        for (int i = 0; i < effectGo.Count; i++)
        {
            effectGo[i].SetActive(isShow);
        }
    }

    // 怪物攻击动作回调
    public void AttackMonster(int index)
    {
        if (index != 0 && this.enabled)
        {
            emission.PlayAttackEffect("attack", attackTarget == null ? null : attackTarget.gameObject);
        }
    }
    // 怪物技能动作回调
    public void AttackCast(string key)
    {
        if (this.enabled)
            emission.PlaySpellEffect(key);
    }

    //动作锁定结束
    public void EndAction()
    {
        if (GameLibrary.IsMajorOrLogin())
            return;
        if (this.enabled)
        {
            if (gameObject != null)
            {
                transform.localRotation = new Quaternion(0, transform.localRotation.y, 0, transform.localRotation.w);
            }
        }
    }

    //跳跃回调
    public void JumpDelegate(int index)
    {
        if (index == 0)
        {
            if (JumpStartDelegate != null)
            {
                JumpStartDelegate();
            }
        }
        else if (index == 1)
        {
            if (JumpOverDelegate != null)
            {
                JumpOverDelegate();
            }
        }
    }
    /// <summary>
    /// 受击,多次攻击卡的话可以改成次数加间隔
    /// </summary>
    public void HitAction(string key)
    {
        if (GameLibrary.IsMajorOrLogin())
            return;
        if (this.enabled)
        {
            SkillNode mCurSkillNode = EffectEmission.GetSkillNodeByKey(key, this);
            if (HitActionDelegate != null)
            {
                HitActionDelegate(mCurSkillNode.skill_id);
            }
            emission.HitAction(mCurSkillNode, attackTarget == null ? null : attackTarget.gameObject);
        }
    }
    /// <summary>
    /// 震动
    /// </summary>
    public void ShakeAction(int count)
    {
        if (this.enabled)
        {
            //目标在屏幕内震屏
            Vector3 targetPos = Camera.main.WorldToViewportPoint(transform.position);
            if (!(targetPos.x < 0 || targetPos.x > 1 || targetPos.y < 0 || targetPos.y > 1))
            {
                ThirdCamera.instance.DoShake(count, 0f);
            }
        }
    }

    /// <summary>
    /// 施法特效
    /// </summary>
    public void QuartzAction(string key)
    {
        if (GameLibrary.IsMajorOrLogin())
            return;
        if (this.enabled)
        {
            emission.PlaySpellEffect(key);
            LastAttack = attackTarget;
        }
    }

    /// <summary>
    /// 瞬间移动
    /// </summary>
    public void FlashMove()
    {
        if (GameLibrary.IsMajorOrLogin())
            return;
        if (this.enabled)
        {
            if (BlinkDelegate != null)
            {
                BlinkDelegate();
            }
        }
    }

    /// <summary>
    /// 播放音乐
    /// </summary>
    /// <param name="s"></param>
    public virtual void PlayMusic(string s)
    {
        if (s != null && s.Length != 0)
            AudioController.Instance.PlayEffectSound(GetMobaName() + "/" + s, this);
    }

    public void AttackSetForward(SkillNode skillNode)
    {
        if (attackTarget != null && attackTarget != this)
        {
            if ((GameLibrary.Instance().CheckNotHeroBoss(this) && skillNode.target == TargetState.Need) ||
                (!GameLibrary.Instance().CheckNotHeroBoss(this) && !(skillNode.target == 0 && skillNode.dist == 0)))
            {
                SetForward();
            }
        }
    }

    public void SetForward()
    {
        if (attackTarget != null && attackTarget != this)
        {
            Vector3 mCurForward = attackTarget.transform.position - transform.position;
            transform.forward = mCurForward == Vector3.zero ? attackTarget.transform.forward : mCurForward;
            transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
        }
    }

    /// <summary>
    /// 指示技能播放时机,不涉及伤害
    /// </summary>
    public void Attack(string key)
    {
        if (GameLibrary.IsMajorOrLogin())
            return;
        mCurSkillNode = EffectEmission.GetSkillNodeByKey(key, this);
        GameLibrary.Instance().AfreshTargetCastSkill(this, mCurSkillNode);
        if (this.enabled)
        {
            if (attackTarget != null)
            {
                AttackSetForward(mCurSkillNode);
            }
            #region start
            bool isUseEmissionPoint = mCurSkillNode.isFiringPoint;
            if (key == "skill4" && mCurMobalId == MobaObjectID.HeroJiansheng)
            {
                if (attackTarget != null)
                {
                    transform.LookAt(attackTarget.transform.position);
                    float mTempDistance = 0.3f;
                    pm.FastMove(attackTarget.transform.position - transform.position + new Vector3(UnityEngine.Random.Range(-mTempDistance, mTempDistance), 0f, UnityEngine.Random.Range(-mTempDistance, mTempDistance)));
                    StartCoroutine(SetJSS1Offset(attackTarget, 0.2f));
                }
            }
            #endregion end
            Transform mTempEmissionPoint = emissionPoint;
            if (mCurMobalId == MobaObjectID.HeroShenling)
            {
                mTempEmissionPoint = (key == "attack2" || key == "skill1") ? emissionPoint1 : mTempEmissionPoint;
            }
            emission.PlayAttackEffect(key, attackTarget == null ? null : attackTarget.gameObject, isUseEmissionPoint ? mTempEmissionPoint : null);
            LastAttack = attackTarget;
        }
    }

    IEnumerator SetJSS1Offset(CharacterState target, float time)
    {
        transform.LookAt(target.transform);
        transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
        pm.FastMove(transform.forward.normalized * time * 3);
        //transform.position += transform.forward * time * 3;
        yield return new WaitForSeconds(time);
        transform.LookAt(target.transform);
        yield break;
    }

    public void ApproachTo(Vector3 targetPos, float atkRange)
    {
        pm.Approaching(targetPos);
        float dis = Vector3.Distance(this.transform.position, targetPos);
        if (dis < atkRange)
        {
            pm.Stop();
        }
    }

    bool IsTp()
    {
        return SceneBaseManager.instance != null && SceneBaseManager.instance.sceneType == SceneType.TP;
    }

    public Renderer[] GetSkins()
    {
        return skins;
    }

    public Material[] GetMaterials()
    {
        return materials;
    }

    public IEnumerator GrownAndSmaller(float mChangeFactor, float mDeltaTime, Vector3 localScale)
    {
        float mChangeValue = 0;
        while (Mathf.Abs(mChangeValue) < Mathf.Abs(mChangeFactor))
        {
            float mDelta = mChangeFactor / mDeltaTime * Time.deltaTime;
            mChangeValue += mDelta;
            mChangeValue = Mathf.Abs(mChangeValue) < Mathf.Abs(mChangeFactor) ? mChangeValue : mChangeFactor;
            transform.localScale = localScale + Vector3.one * mChangeValue;
            yield return null;
        }
        yield break;
    }
    //处理攻击
    public void HitBy(SkillNode skillNode, CharacterState attackerCS, CharacterData attackerData)
    {
        if (isDie || Invincible)
            return;

        bool showHud = GameLibrary.Instance().ShouldShowHud(this, attackerCS);

        float dodgeRate = 0;

        if (CharData != null)
        {
            //闪避 = 自己的闪避-攻击方的命中率

            dodgeRate = Formula.GetSingleAttributeRate(CharData, AttrType.dodge, attackerData.lvl) -
            Formula.GetSingleAttributeRate(attackerData, AttrType.hit_ratio, CharData.lvl);
        }
        if (skillNode.types == (byte)DamageType.physics && groupIndex != attackerData.groupIndex && UnityEngine.Random.Range(0f, 1f) < dodgeRate)
        {
            Hp(0, HUDType.Miss, showHud);
            //向服务器发送伤害信息
        }
        else
        {
            float skillFinalVal = attackerData.skill_Damage[0] + (skillNode.site == 0 ? 0 : (attackerCS == null ? 0 : attackerCS.addSkillDamage));
            int result = GameLibrary.Instance().CalcSkillDamage(skillFinalVal, skillNode.types, attackerData.lvl, this, attackerCS);
            if (attackerData != null && attackerData.groupIndex == groupIndex && result > 0)
            {
                result = 0;
            }
#if UNITY_EDITOR
            // result = result == 0 ? 0 : 1;
#endif
            float suckBloodRate = 0;
            if (result > 0)
            {
                if (GameLibrary.Instance().CheckInvisiblityCanReveal(this))
                {
                    GameLibrary.Instance().BrokenInvisibility(this);
                }
                inBattleTimer = 5f;
                LastAttackBy = attackerCS;
                float criticalRate = 0;
                if (CharData != null)
                {
                    criticalRate = Formula.GetSingleAttributeRate(attackerData, AttrType.critical, CharData.lvl) - Formula.GetSingleAttributeRate(CharData, AttrType.tenacity, attackerData.lvl);
                }
                bool isCritical = false;
                if (UnityEngine.Random.Range(0f, 1f) < criticalRate && state != Modestatus.Tower)
                {
                    result = Mathf.CeilToInt(result * 1.5f);
                    isCritical = true;
                }
                if (isNetworking && attackerCS != null)
                {
                    long myKey = playerData.GetInstance().selfData.keyId;
                    if (attackerCS.keyId == myKey || (attackerCS.state == Modestatus.SummonHero && attackerCS.Master != null && attackerCS.Master.keyId == myKey))//|| keyId == myKey)//攻击者或被攻击者是自己
                    {
                        // Debug.Log(_maxHp + "===>" + result);
                        HUDAndHPBarManager.instance.HUD(gameObject, result, isCritical ? HUDType.Crit : HUDType.DamagePlayer);
                        ClientSendDataMgr.GetSingle().GetWalkSend().SendHp(attackerCS.keyId, keyId, result);//
                    }
                }
                else
                {
                    if (attackerData.state == Modestatus.Player)
                    {
                        if (isCritical)
                            Hp(result, HUDType.Crit, showHud, attackerCS);
                        else
                            Hp(result, HUDType.DamagePlayer, showHud, attackerCS);
                    }
                    else
                    {
                        Hp(result, HUDType.DamageEnemy, showHud, attackerCS);
                    }
                }

                if (CharData != null)
                {
                    suckBloodRate = Formula.GetSingleAttributeRate(attackerData, AttrType.suck_blood, CharData.lvl);
                }
                int suckCount = Mathf.CeilToInt(result * suckBloodRate);
                if (suckCount > 0 && attackerCS != null && !attackerCS.isDie)
                    attackerCS.Hp(-suckCount, HUDType.SuckBlood, attackerCS.state == Modestatus.Player, attackerCS);

                if (null != OnBeAttack && null != attackerCS) OnBeAttack(attackerCS);

                // if (state != Modestatus.Tower || IsTp())
                bool checkNoEffectAndRecolor = !(state == Modestatus.Monster && attackerData.state == Modestatus.Monster);
                if (showHud && checkNoEffectAndRecolor)
                {
                    SetSkinColor();
                    Invoke("ReColor", 0.15f);
                }
                if (emission != null && checkNoEffectAndRecolor)
                    emission.PlayHitEffect(skillNode, this, attackerCS);
                if (attackerData.state == Modestatus.Player)
                {
                    //ComboCalc._instance.ChangeCombo();
                }
                if (null != OnAttackDmg)
                    OnAttackDmg(this, attackerCS, result);
                if (null != OnHelp)
                    OnHelp(this, attackerCS, priority);

            }
            else if (result < 0)
            {
                Hp(result, HUDType.Cure, showHud, attackerCS);
            }
            else
            {
                if (attackerCS != null &&
                    ((attackerCS.mCurMobalId == MobaObjectID.HeroBaihu && skillNode.site == 2) ||
                    (attackerCS.mCurMobalId == MobaObjectID.HeroMori && skillNode.site == 4) ||
                    (attackerCS.mCurMobalId == MobaObjectID.HeroShenling && skillNode.site == 2) ||
                    (attackerCS.mCurMobalId == MobaObjectID.HeroShengqi && skillNode.site == 4)))
                {
                    emission.PlayHitEffect(skillNode, this, attackerCS);
                }
            }
            //添加buff
            AddBuffManager(skillNode, attackerCS, attackerData);
            //if (isNetworking && attackerCS != null)
            //{
            //    long myKey = playerData.GetInstance().selfData.keyId;
            //    if (attackerCS.keyId == myKey || keyId == myKey)//攻击者是自己同步血量
            //    {
            //        ClientSendDataMgr.GetSingle().GetWalkSend().SendHp(attackerCS.keyId, keyId, result);//自己打怪物同步血量
            //    }
            //}
        }
    }

    void SetHeroDeathEffect()
    {
        if (SceneBaseManager.instance.sceneType == SceneType.PVP3)
        {
            if (materials != null)
            {
                for (int i = 0; i < materials.Length; i++)
                {
                    materials[i].SetFloat("_Alpha", 0.3f);
                }
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void SetSkinColor()
    {
        if (materials != null)
        {
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i].SetFloat("_Blink", 500f);
            }
        }
    }

    void ReColor()
    {
        if (materials != null)
        {
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i].SetFloat("_Blink", 0f);
            }
        }
    }

    public List<SkillBuff> AddBuffManager(SkillNode skillNode, CharacterState attackerCS, CharacterData attackerData)
    {
        List<SkillBuff> mResultBuff = new List<SkillBuff>();
        if (!isDie && state != Modestatus.Tower)
        {
            //添加buff和产生天赋buff
            if (skillNode.add_state != null)
            {
                for (int i = 0; i < skillNode.add_state.Length; i++)
                {
                    if (isDie) break;
                    object o = skillNode.add_state[i];
                    if (o != null && o is System.Array && ((System.Array)o).Length > 0)
                    {
                        if (skillNode.buffs_target.Length > i && skillNode.buffs_target[i] == 2) continue;
                        if (skillNode.buffs_target.Length > i && skillNode.buffs_target[i] == 1 && attackerCS != null && attackerCS != this) continue;
                        float mResultValue = GetFinalBuffValue(skillNode, attackerCS, attackerData, i);
                        SkillBuff buff = SkillBuffManager.GetInst().AddBuffs(mResultValue, o, this, attackerCS, skillNode, attackerData.lvl,
                            !(skillNode.buffs_target.Length > i && skillNode.buffs_target[i] == 1 && attackerCS != null && attackerCS == this));
                        if (buff != null)
                        {
                            mResultBuff.Add(buff);
                            if (buff.node.type == BuffType.talent || buff.node.type == BuffType.DelayEffective)
                            {
                                System.Array childObj = new System.Array[skillNode.add_state.Length - i - 1];
                                System.Array.Copy(skillNode.add_state, i + 1, childObj, 0, skillNode.add_state.Length - i - 1);
                                for (int j = 0; j < childObj.Length; j++)
                                {
                                    SkillBuff mTempSkillBuff = SkillBuffManager.GetInst().CreateBuff(attackerData.skill_Damage[i + j + 2], childObj.GetValue(j));
                                    mTempSkillBuff.SetBuffSkillNode(skillNode, attackerData.lvl);
                                    buff.talentBuff.Add(mTempSkillBuff);
                                }
                                break;
                            }
                        }
                    }
                }
            }
            //拥有天赋buff普攻触发
            if (attackerCS != null && skillNode.site == 0)
            {
                List<SkillBuff> mExistTalentBuff = SkillBuffManager.GetInst().GetSkillBuffListByCs(attackerCS).FindAll(a => a.node.type == BuffType.talent && a.node.buffActionType == buffActionType.attack);
                for (int i = 0; i < mExistTalentBuff.Count; i++)
                {
                    for (int j = 0; j < mExistTalentBuff[i].talentBuff.Count; j++)
                    {
                        SkillBuff mTempBuff = mExistTalentBuff[i].talentBuff[j];
                        SkillBuffManager.GetInst().AddBuffs(mTempBuff.baseValue,
                            new object[2] { mTempBuff.id, mTempBuff.last }, this, attackerCS, mTempBuff.skillNode, mTempBuff.lvl);
                    }
                }
            }
            //拥有天赋技能buff释放技能触发
            if (attackerCS != null && skillNode.site != 0 && attackerData.groupIndex != groupIndex)
            {
                List<SkillBuff> mExistTalentBuff = SkillBuffManager.GetInst().GetSkillBuffListByCs(attackerCS).FindAll(a => a.node.type == BuffType.talent && a.node.buffActionType == buffActionType.skill);
                for (int i = 0; i < mExistTalentBuff.Count; i++)
                {
                    for (int j = 0; j < mExistTalentBuff[i].talentBuff.Count; j++)
                    {
                        SkillBuff mTempBuff = mExistTalentBuff[i].talentBuff[j];
                        float resultValue = mTempBuff.last;
                        if (attackerCS.mCurMobalId == MobaObjectID.HeroLuosa)
                            resultValue = skillNode.site == 1 ? 1.8f : (skillNode.site == 2 ? 1.2f : 0.6f);
                        SkillBuffManager.GetInst().AddBuffs(mTempBuff.baseValue,
                            new object[2] { mTempBuff.id, resultValue }, this, attackerCS, mTempBuff.skillNode, mTempBuff.lvl);
                    }
                }
            }
        }
        return mResultBuff;
    }

    private float GetFinalBuffValue(SkillNode skillNode, CharacterState attackerCS, CharacterData attackerData, int i)
    {
        float mResultValue = attackerData.skill_Damage[i + 1];
        //神灵武士二技能为百分比互相掉血
        if (GameLibrary.Instance().CheckShenlingSkillIndex(attackerCS, skillNode, 2) && i == 0)
        {
            mResultValue = attackerCS.currentHp * mResultValue / 100;
            SkillBuffManager.GetInst().AddBuffs(mResultValue, skillNode.add_state[i], attackerCS, attackerCS, skillNode, attackerData.lvl, false);
        }
        //神灵武士四技能为吸收和收回
        if (GameLibrary.Instance().CheckShenlingSkillIndex(attackerCS, skillNode, 4))
        {
            CharacterState mComeBackCs = attackerCS.state == Modestatus.SummonHero ? attackerCS.Master : attackerCS;
            if (i == 0)
            {
                mResultValue = Formula.GetSingleAttribute(CharData, AttrType.armor) * mResultValue / 100;
                if (mComeBackCs != null && !mComeBackCs.isDie)
                    SkillBuffManager.GetInst().AddBuffs(mResultValue, skillNode.add_state[i + 2], attackerCS, attackerCS, skillNode, attackerData.lvl);
            }
            else if (i == 1)
            {
                mResultValue = Formula.GetSingleAttribute(CharData, AttrType.magic_resist) * mResultValue / 100;
                if (mComeBackCs != null && !mComeBackCs.isDie)
                    SkillBuffManager.GetInst().AddBuffs(mResultValue, skillNode.add_state[i + 2], attackerCS, attackerCS, skillNode, attackerData.lvl);
            }
        }
        return mResultValue;
    }

    public void AddNetSkillManager(SkillNode skillNode)
    {
        if (!isDie && state != Modestatus.Tower)
        {
            if (skillNode.add_state != null)
            {
                for (int i = 0; i < skillNode.add_state.Length; i++)
                {
                    object o = skillNode.add_state[i];
                    if (o != null && o is System.Array && ((System.Array)o).Length > 0)
                    {

                    }
                }
            }

        }
    }

    int CheckImmune(int result, bool immuneType, bool showHud)
    {
        if (!immuneType)
            Hp(0, HUDType.Immune, showHud);
        else
            return result;
        return 0;
    }
    /// <summary>
    /// 野外同步血量用
    /// </summary>
    /// <param name="count"></param>
    public void ChangeHp(int count, HUDType hType = HUDType.DamageEnemy, bool showHUD = true)
    {
        if (isDie)
            return;
        int tempcount = count - _currentHp;
        if (tempcount == 0)
        {
            return;
        }

        if (hType == HUDType.Absorb && HUDAndHPBarManager.instance != null)
        {
            HUDAndHPBarManager.instance.HUD(gameObject, tempcount, hType);
            return;
        }
        _currentHp = count;
        // _currentHp = currentHp > maxHp ? maxHp : currentHp;//
        //  _currentHp = currentHp < 0 ? 0 : currentHp;
        if (IsMainHero && showHUD && HUDAndHPBarManager.instance != null) HUDAndHPBarManager.instance.HUD(gameObject, tempcount, hType);

        RefreshHpBar();


    }

    /// <summary>
    /// 使用Hp(0)刷新血条显示
    /// </summary>
    public void Hp(int count, HUDType hType = HUDType.DamageEnemy, bool showHUD = false, CharacterState attackerCS = null, bool isOtherPlayer = false)
    {
        if (isDie)
            return;
        if (isNetworking && !isOtherPlayer)
        {
            if (CharacterManager.player.name != gameObject.name)
            {
                if (CreatePeople.GetInstance().OtherplayerDic.ContainsKey(gameObject.GetComponent<CharacterState>().keyId))
                {
                    return;
                }
            }
        }
        if (count > 0)
        {
            count = Mathf.FloorToInt(count * DamagePercent);
        }
        else if (count < 0)
        {
            count = Mathf.FloorToInt(count * CurePercent);
        }
        //魔法护盾
        if (count > 0 && MagicShields > 0)
        {
            HUDAndHPBarManager.instance.HUD(gameObject, -Mathf.FloorToInt(count > MagicShields ? MagicShields : count), HUDType.Absorb);
            float mRemainResult = count - MagicShields;
            MagicShields = -mRemainResult > 0 ? -mRemainResult : 0;
            if (MagicShields == 0 && OnShieldOff != null)
            {
                OnShieldOff(this);
            }
            count = mRemainResult > 0 ? Mathf.FloorToInt(mRemainResult) : 0;
            if (count == 0) return;
        }
        _currentHp -= count;
        _currentHp = currentHp > maxHp ? maxHp : currentHp;
        _currentHp = currentHp < 0 ? 0 : currentHp;

        if (showHUD && !(groupIndex == 0 && Invisible)) HUDAndHPBarManager.instance.HUD(gameObject, count, hType);

        if (count > 0 && OnDamageBy != null && attackerCS != null) OnDamageBy(this, attackerCS);
        if (currentHp == 0)
        {
            HandleDieLogic();
        }
        else
        {
            if (count > 0)
            {
                if (null != OnHit) OnHit(this);
                if (pm != null) pm.Hit();
            }
            ChangePriority();
            HandleHpPassiveSkill();
        }
        RefreshHpBar();

    }

    public void HandleDieLogic()
    {
        if (isDie) return;
        isDie = true;
        if (!isNetworking)
        {
            if (pm != null) pm.Die();
        }
        else
        {
            ClientSendDataMgr.GetSingle().GetWalkSend().SendDead(playerData.GetInstance().selfData.keyId, keyId, rc);
        }
        DeleteHpAndHud();
        if (!BattleUtil.IsHeroTarget(this))
        {
            if (!isNetworking)
            {
                // DeleteMonster();
                if (state != Modestatus.Tower)
                {
                    StartCoroutine(HandleBodySinking(3f, 1.5f));
                }
            }
        }
        else
        {
            Invoke("SetHeroDeathEffect", 2f);
        }
        if (OnDead != null) OnDead(this);
        DisableComponents();//DisableComponets();

    }

    IEnumerator HandleBodySinking(float dieTime, float sinkTime)
    {
        yield return new WaitForSeconds(dieTime);
        float time = 0;
        float downStep = 0.3f;
        Destroy(gameObject, sinkTime);
        while (time < sinkTime)
        {
            time += Time.deltaTime;
            transform.Translate(Vector3.down * downStep * Time.deltaTime);
            yield return null;
        }
        yield break;
    }

    private void HandleHpPassiveSkill()
    {
        if (mCurMobalId == MobaObjectID.HeroShenling)
        {
            long mPassiveBuffId = 0;
            float mCurAddonSpeed = 0, mExistAddonSpeed = 0;
            SkillNode mCurPassive = GameLibrary.Instance().GetCurrentSkillNodeByCs(this, 3);
            if (mCurPassive.add_state.Length > 0)
            {
                object o = mCurPassive.add_state[0];
                if (o != null && o is Array && ((Array)o).Length > 0)
                {
                    mPassiveBuffId = Convert.ToInt64(((Array)o).GetValue(0));
                }
            }
            if (mPassiveBuffId == 0) return;
            float mHpRatio = (maxHp - currentHp) * 100f / maxHp;
            mCurAddonSpeed = Mathf.CeilToInt(mHpRatio / 10) * 10f;
            List<SkillBuff> mAddAtkSpeedBuffs = SkillBuffManager.GetInst().GetSkillBuffListByCs(this).FindAll(a => a.id == mPassiveBuffId && a.attacker != this);
            if (mAddAtkSpeedBuffs.Count > 0)
            {
                mAddAtkSpeedBuffs.Sort((a, b) => Mathf.FloorToInt(b.baseValue - a.baseValue));
                mExistAddonSpeed = mAddAtkSpeedBuffs[0].baseValue;
            }
            if (mExistAddonSpeed <= mCurAddonSpeed)
            {
                attackSpeed = attackInitSpeed + attackInitSpeed * mCurAddonSpeed / 100;
            }
        }
    }

    void ChangePriority()
    {
        if (currentHp <= maxHp * 0.2f)
        {
            if (state == Modestatus.NpcPlayer)
                priority = MobaAIPlayerPriority.FriendHeroHigh;
            else if (state == Modestatus.Tower && groupIndex == 0)
                priority = MobaAIPlayerPriority.TowerHigh;
        }
    }

    public void DeleteMonster()
    {
        DeleteHpAndHud();
        InvokeRepeating("MoveDownRep", 1f, 0.01f);
    }

    public void DeleteHpAndHud(float HpBarDelay = 0.3f, float HudDelay = 1f)
    {
        if (null != hpBar) Destroy(hpBar.gameObject, HpBarDelay);
        if (HUDAndHPBarManager.instance != null)
            HUDAndHPBarManager.instance.DestroyHUD(gameObject, HudDelay);
    }

    float downStep = 0.05f;
    void MoveDownRep()
    {
        GetComponentInChildren<Renderer>().gameObject.layer = 0;
        // transform.Translate(Vector3.down * downStep * Time.deltaTime);
        downStep += 0.01f;
        if (downStep > 1.5f)
        {
            if (pm != null && pm.ani != null)
                pm.ani.enabled = false;
            //if (isNetworking)
            //{
            //    CreatePeople.GetInstance().DeleteOtherObject(keyId, true);
            //}
            CancelInvoke("MoveDownRep");
            Destroy(this.gameObject);
        }
    }
    /// <summary>
    /// 失效一些组件
    /// </summary>
    public void DisableComponents()
    {
        CharacterController cc = GetComponent<CharacterController>();
        //关闭角色控制器 
        if (cc != null)
            cc.enabled = false;
        //关闭寻路组件
        if (transform.GetComponent<UnityEngine.AI.NavMeshAgent>() != null)
            transform.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
        //关闭指定脚本
        DisableScripts();
    }

    /// <summary>
    /// 让指定脚本失效
    /// </summary>
    public void DisableScripts()
    {
        //如果有玩家，关闭玩家ai
        if (transform.GetComponent<BasePlayerAI>())
            transform.GetComponent<BasePlayerAI>().enabled = false;
        //如果有战车，关闭战车脚本
        if (transform.GetComponent<BullockCarts>() != null)
            transform.GetComponent<BullockCarts>().enabled = false;
        //如果有塔ai,关闭塔ai
        if (transform.GetComponent<Tower_AI>() != null)
            transform.GetComponent<Tower_AI>().enabled = false;
    }

    bool hasShowBossBlood;
    #region HPBar
    public HPBar hpBar;
    void RefreshHpBar()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == GameLibrary.UI_Major)
            return;
        if (state == Modestatus.Boss)
        {
            if (sceneName == GameLibrary.PVP_Moba3v3) {
                if (!hasShowBossBlood)
                {
                    OnBeAttack += (c) => GameLibrary.bossBlood.ShowBlood(this);
                    hasShowBossBlood = true;
                }
            }
            else
                GameLibrary.bossBlood.ShowBlood(this);
        }
        if (this == CharacterManager.playerCS)
        {
            SceneUIManager.instance.SwitchBloodScreen(currentHp < 0.3f * maxHp && !isDie);
            if (UIRole.instance != null && UIRole.instance.gameObject.activeInHierarchy)
                UIRole.instance.OnHpBarChange(currentHp, maxHp, MagicShields);
        }
        if (hpBar != null)
        {
            hpBar.gameObject.SetActive(!(groupIndex == 0 && Invisible));
            hpBar.UpdateHPBar(1f * currentHp / maxHp, MagicShields / maxHp);
            if (BattleUtil.IsHeroTarget(this) && !GameLibrary.SceneType(SceneType.PVP3))
                hpBar.SetRuler(maxHp);
        }
    }

    public void SetHpBarEnable(bool b)
    {
        if (hpBar != null)
        {
            hpBar.gameObject.SetActive(b);
        }
    }

    public void AddHpBar(bool isShowName = false)
    {
        GameObject hpPoint = Instantiate(Resources.Load<GameObject>("HUD/HP Point"), Vector3.zero, Quaternion.identity) as GameObject;
        hpPoint.transform.parent = transform;
        hpBar = NGUITools.AddChild(HUDAndHPBarManager.instance.gameObject, Resources.Load<GameObject>("HUD/HP Bar")).GetComponent<HPBar>();
        hpBar.GetComponent<UIFollowTarget>().target = hpPoint.transform;
        HUDAndHPBarManager.instance.AddHUDAndHP(hpBar.gameObject, CharData.state);
        Globe.currentDepth -= 5;
        UISprite[] sps = hpBar.GetComponentsInChildren<UISprite>(true);
        for (int i = 0; i < sps.Length; i++)
        {
            sps[i].depth += Globe.currentDepth;
        }
        Vector3 offset = Vector3.one;
        switch (state)
        {
            case Modestatus.Tower:
                offset = Vector3.up * 1.2f;
                hpBar.transform.localScale = new Vector3(1f, 1f, 1f);
                break;
            case Modestatus.NpcPlayer:
            case Modestatus.Player:
                offset = Vector3.up * 1f;
                hpBar.transform.localScale = new Vector3(1f, 1f, 1f);
                break;
            case Modestatus.Monster:
                hpBar.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
                Transform headBuffTrans = transform.FindChild("Headbuff");
                if(headBuffTrans != null)
                    offset = new Vector3(0f, headBuffTrans.localPosition.y, 0f);
                else
                    offset = Vector3.up * 0.5f;
                break;
        }
        hpPoint.transform.position = transform.position + offset;

        string spriteN = "";
        if (state == Modestatus.Monster)
            spriteN = (groupIndex == 0 || groupIndex == 99 || groupIndex == 100 || groupIndex == 101) ? "xiaoguaixuetiao" : "xiaoguaixuetiao-lv";
        else
            spriteN = (groupIndex == 0) ? "hongxuetiao" : "lvxuetiao";
        hpBar.SpHp.spriteName = spriteN;
        hpBar.SpMask.spriteName = spriteN;
        hpBar.SpMask.alpha = 0.5f;
        hpBar.LaName.alpha = 0f;
        if (BattleUtil.IsHeroTarget(this))
        {
            if (isShowName)
                hpBar.LaName.alpha = 1f;

            hpBar.LaName.text = !string.IsNullOrEmpty(CharData.fakeMobaPlayerName) ? CharData.fakeMobaPlayerName : CharData.attrNode.name;
            if (!GameLibrary.SceneType(SceneType.PVP3))
                hpBar.SetRuler(maxHp);
        }
        else if (state != Modestatus.Tower)
        {
            hpBar.gameObject.SetActive(false);
        }
    }
    #endregion HPBar

    public void CreatePet(long perid)
    {
        if (perid == 0) return;

        if(pet!= null)
        {
            Pet_AI petAI = pet.GetComponent<Pet_AI>();
            if(petAI != null && petAI.petID == perid)
                return;
        }

        PetNode petNode = FSDataNodeTable<PetNode>.GetSingleton().FindDataByType(perid);
        if (null == petNode) return;
        ModelNode model = FSDataNodeTable<ModelNode>.GetSingleton().FindDataByType(petNode.model_id);
        if (null == model) return;

        GameObject resPet = Resources.Load(model.respath) as GameObject;
        if (null == resPet) return;
        GameObject petGo = Instantiate(resPet);
        petGo.transform.parent = transform.parent;
        BattleUtil.GetRadiusRandomPos(petGo.transform, transform, 1f, 2f);
        petGo.transform.LookAt(transform);
        BattleUtil.AddPetComponents(petGo);
        petGo.GetComponent<Pet_AI>().InitPet(this, perid);
        pet = petGo;
    }

    public void HidePet()
    {
        if (null != pet)
        {
            pet.GetComponent<Pet_AI>().DestroyName();
            Destroy(pet.gameObject);
        }
    }

    public MobaObjectID GetMobalIdByStr()
    {
        return (MobaObjectID)GameLibrary.GetHeroMobaId(GetMobaName());
    }

    public string GetMobaName()
    {
        if (string.IsNullOrEmpty(mMobaName))
        {
            string mobalName = name;
            if (mobalName.Contains("(Clone)"))
            {
                mobalName = mobalName.Replace("(Clone)", "").Trim();
            }
            string[] mobalNames = mobalName.Split('_');
            if (mobalNames.Length >= 2)
            {
                mMobaName = mobalNames[0] + "_" + mobalNames[1];
            }
            else
            {
                mMobaName = mobalName;
            }
        }
        return mMobaName;
    }

    public RoleInfo RoInfo;

    public CharacterData CharData;
    // 当前动画速度
    public float animSpeed = 1f;
    // 当前移动速度
    public float moveSpeed = 1f;
    // 当前攻击速度
    public float attackSpeed = 1f;
    // 战斗初始移动速度
    public float moveInitSpeed = 1f;
    // 战斗初始攻击速度
    public float attackInitSpeed = 1f;

    [SerializeField]
    int _currentHp;
    public int currentHp { get { return _currentHp; }}

    int _maxHp;
    public int maxHp
    {
        get
        {
            if (!isNetworking)
            {
                int newMaxHp = Mathf.FloorToInt(Formula.GetSingleAttribute(CharData, AttrType.hp));
                if(_maxHp != newMaxHp)
                {
                    _currentHp += newMaxHp - _maxHp;
                }
                _maxHp = newMaxHp;
            }

            return _maxHp;
        }
        set
        {
            _maxHp = value;
        }
    }

    public List<long> GetSkills()
    {
        List<long> skills = new List<long>();
        int len = CharData.attrNode.skill_id.Length;

        for (int i = 0; i < len; i++)
        {
            SkillNode skillNode = GameLibrary.skillNodeList[CharData.attrNode.skill_id[i]];
            if (skillNode.site != 0 && !skillNode.IsPartSkill())
            {
                if (CharData.attrNode is HeroAttrNode)
                    skillNode.castOrder = GetCastIndexBySite(skillNode.site);
                skills.Add(CharData.attrNode.skill_id[i]);
            }
        }
        return skills;
    }
    int GetCastIndexBySite(int site)
    {
        int[] order = ((HeroAttrNode)CharData.attrNode).heroNode.skill_order;
        for (int i = 0; i < order.Length; i++)
        {
            if (order[i] == site)
            {
                return order.Length - i;
            }
        }
        return 0;
    }

    public void ClearBattleData ()
    {
        if(CharData != null)
        {
            CharData.buffAttrs = new float[Formula.ATTR_COUNT];
        }
    }

    public float AttackRange { get { return CharData == null ? 0f : CharData.attrNode.skills[0].dist; } }
    /// <summary>
    /// 视野范围
    /// </summary>
    public float TargetRange { get { return CharData == null ? 0f : CharData.attrNode.field_distance; } }
    public float AvoidRange = 0.3f;

    // 能否受到物理伤害
    public bool RecievePhysicDamage = true;
    // 能否受到法术伤害
    public bool RecieveMagicDamage = true;
    // 能否受到真实伤害
    public bool RecieveFixDamage = true;
    // 能否被控制
    public bool RecieveControl = true;
    // 是否无敌
    public bool Invincible = false;
    // 是否隐身
    public bool Invisible = false;
    //技能伤害加成
    public float addSkillDamage = 0;
    //治疗效果百分比
    public float CurePercent = 1;
    //伤害加成百分比
    public float DamagePercent = 1;
    //魔法护盾
    float mMagicShields = 0f;
    public float MagicShields {
        get {
            return mMagicShields;
        }
        set
        {
            mMagicShields = value;
            RefreshHpBar();
            if (this == CharacterManager.playerCS)
            {
                if (UIRole.instance != null && UIRole.instance.gameObject.activeInHierarchy)
                    UIRole.instance.OnHpBarChange(currentHp, maxHp, MagicShields);
            }
        }
    }
}
