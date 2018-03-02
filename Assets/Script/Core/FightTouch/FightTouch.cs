using UnityEngine;
using Tianyu;
using UnityEngine.SceneManagement;

public class FightTouch : GUIBase
{
    public static FightTouch _instance;
    public static bool Enabled = true;

    public delegate void OnInitFightTouch();
    public OnInitFightTouch OnInit;

    public delegate void OnTouchFightTouch(TOUCH_KEY key);
    public OnTouchFightTouch OnTouchBtn;
    public OnTouchFightTouch OnBtnTargetNil;

    public AttackBtn attack;
    public SkillBtnCD skillRestore;
    public SkillBtnCD skill1;
    public SkillBtnCD skill2;
    public SkillBtnCD skill3;
    public SkillBtnCD skill4;
    public SkillBtnCD summon1;
    public SkillBtnCD summon2;
    public SkillBtnCD summon3;
    public GUISingleButton pauseBtn;
    public GUISingleButton overBtn;
    public GUISingleButton getStarDisBtn;
    public GUISingleButton autoBattle;
    public GUISingleButton tpBtn;
    //public GUISingleButton staticsBtn;
    public MobaStatic mobaStatic;

    public GameObject autoBattleEffect;
    public UIProgressBar progress;
    public UIPromptTip starprompt;
    private GameObject mSkillRestoreLabel;

    [HideInInspector]
    public bool allSkillCD = false;
    [HideInInspector]
    public bool isLock;
    [HideInInspector]
    public GameObject tpEffect1;
    [HideInInspector]
    public GameObject tpEffect2;
    [HideInInspector]
    public Vector3 TpPosition;

    SkillBtnCD[] skillBtns;
    float tpTimer = 0;
    bool isTp = false;

    void Awake()
    {
        _instance = this;
    }

    public override UIPanleID GetUIKey()
    {
        return UIPanleID.none;
    }

    void Start()
    {
        autoBattle.onClick = OnAutoClick;
        attack.OnLongPress += OnAttack;
        skillBtns = new SkillBtnCD[] { skill1, skill2, skill3, skill4, summon1, summon2, summon3, skillRestore };
        progress.gameObject.SetActive(false);

        InitSkillBtnState(skill1, 1);
        InitSkillBtnState(skill2, 2);
        InitSkillBtnState(skill3, 3);
        InitSkillBtnState(skill4, 4);
        InitSummonBtnState(summon1, 5);
        InitSummonBtnState(summon2, 6);
        InitSummonBtnState(summon3, 7);
        if (SceneManager.GetActiveScene().name == GameLibrary.LGhuangyuan)
        {
            autoBattle.gameObject.SetActive(false);
            summon1.gameObject.SetActive(false);
            summon2.gameObject.SetActive(false);
            summon3.gameObject.SetActive(false);
        }
            skillRestore.onPressed += OnRestore;
        skillRestore.SetCd(30);
        mSkillRestoreLabel = skillRestore.CantUseSprite.GetComponentInChildren<UILabel>(true).gameObject;
        tpBtn.onItemClick += OnTp;
        pauseBtn.onClick = OnPauseBtnClick;

        if (Globe.isOverBtn)
            overBtn.onClick = OnOverBtnClick;

        if(CharacterManager.playerCS != null)
            CharacterManager.playerCS.OnDead += ( c ) => CancelRiding();

        UIEventListener.Get(getStarDisBtn.gameObject).onPress = OnStarDis;
        starprompt.RefreshDesUI(FSDataNodeTable<SceneNode>.GetSingleton().FindDataByType(GameLibrary.dungeonId));

        if (null != OnInit)
            OnInit();
    }

    void OnTp(int i)
    {
        isTp = !isTp;
        if (!isTp)
            CancelTp();
    }

    public void OnAutoClick()
    {
        CancelTp();
        if (SceneManager.GetActiveScene().name != GameLibrary.LGhuangyuan)
        {
            if (!Enabled || CharacterManager.playerCS == null)
                return;
            bool isAuto = CharacterManager.playerCS.pm.isAutoMode;
            CharacterManager.instance.SwitchAutoMode(!isAuto);
        }
        else
        {
            //龙骨荒原里自动战斗先用于玩家复活了，策划得加复活机制了
            int hp = 0;
            CharacterState cs = null;

            if (CharacterManager.player != null)
            {
                cs = CharacterManager.player.GetComponent<CharacterState>();
            }
            if (cs != null)
            {
                hp = cs.maxHp;
            }
            ClientSendDataMgr.GetSingle().GetWalkSend().SendPlayerRevive(0, 0, 1, hp);
        }
    }

    void OnRestore(int i, bool b)
    {
        if (CharacterManager.player != null && !skillRestore.isCD)
        {
            CharacterState playerCs = CharacterManager.player.GetComponent<CharacterState>();
            if (playerCs.Restore())
            {
                skillRestore.StartCd();
            }
        }
    }

    void InitSkillBtnState(SkillBtnCD skillBtn, int index)
    {
        SkillNode skillNode = GetSkillNodeBySeat(index);
        if (skillNode != null)
        {
            skillBtn.index = skillNode.site;
            skillBtn.skillNode = skillNode;
            skillBtn.SetCd(skillNode.cooling);
#if UNITY_EDITOR
            skillBtn.SetCd(1);
#endif
            skillBtn.IconSprite.spriteName = skillNode.skill_icon;
            skillBtn.onPressed = OnClickSkillBtn;
        }
    }

    void InitSummonBtnState(SkillBtnCD summonBtn, int index)
    {
        summonBtn.index = index;
        HeroData hd = Globe.Heros()[index - 4];
        if (null == hd || hd.id == 0)
        {
            summonBtn.GetComponent<UISprite>().spriteName = "";
        }
        else
        {
            summonBtn.SetCd(Globe.isFightGuide ? 1 : 30);
#if UNITY_EDITOR
            summonBtn.SetCd(1);
#endif
            summonBtn.onPressed = OnSummon;
            summonBtn.GetComponent<UISprite>().spriteName = hd.node.icon_name;
        }
    }

    void OnClickSkillBtn(int index, bool b)
    {
        if (b)
        {
            TouchHandler.GetInstance().Touch(GetSkillStatusByIndex(index));
        }
        else
        {
            TouchHandler.GetInstance().Release(GetSkillStatusByIndex(index));
        }
        OnSkill(index);
    }

    void OnPauseBtnClick()
    {
        CDTimer.GetInstance().CDRunOrStop(false);
        Control.ShowGUI(UIPanleID.UIPause, EnumOpenUIType.DefaultUIOrSecond, false);
        Time.timeScale = 0;
    }

    void OnOverBtnClick()
    {
        if (GameLibrary.SceneType(SceneType.MB1))
        {
            ClientSendDataMgr.GetSingle().GetMobaSend().SendMobaResult(1);
        }
        else
        {
            SceneBaseManager.instance.WinCondition(true);
           // TaskOperation.Single().PassFubenToTestFubenTask(GameLibrary.dungeonId);
        }
    }

    void OnStarDis(GameObject go, bool state)
    {
        starprompt.gameObject.SetActive(state);
    }

    void Update()
    {
        if (CharacterManager.player != null)
        {
            CharacterState playerCs = CharacterManager.playerCS;
            bool canUse = playerCs.currentHp < playerCs.maxHp && playerCs.inBattleTimer <= 0;
            skillRestore.CantUseSprite.gameObject.SetActive(!canUse && !skillRestore.isCD);
            mSkillRestoreLabel.SetActive(playerCs.inBattleTimer > 0);
        }

        if (isTp)
        {
            if (tpEffect1 != null && !tpEffect1.activeInHierarchy)
            {
                progress.gameObject.SetActive(true);
                CharacterManager.playerCS.OnBeAttack += (c) =>
                {
                    CancelTp();
                };
                tpEffect1.SetActive(true);
                tpEffect1.transform.position = CharacterManager.playerCS.transform.position;
            }
            tpTimer += Time.deltaTime;
            progress.value += Time.deltaTime / 6f;
            if (progress.value >= 0.99f)
                progress.gameObject.SetActive(false);
            if (tpTimer > 5.5f)
            {
                if (tpEffect2 != null && !tpEffect2.activeInHierarchy)
                {
                    CharacterManager.playerCS.pm.nav.enabled = false;
                    tpEffect2.transform.position = CharacterManager.playerCS.transform.position = TpPosition;
                    tpEffect2.SetActive(true);
                    CharacterManager.playerCS.pm.nav.enabled = true;
                }
            }
            if (tpTimer > 7f)
            {
                CancelTp();
            }
        }

        if (isLock || GameLibrary.isBossChuChang) return;

        if (Input.GetKey(KeyCode.J)) OnAttack(true);

        if (Input.GetKeyDown(KeyCode.U)) OnSkill(1);
        if (Input.GetKeyDown(KeyCode.I)) OnSkill(2);
        if (Input.GetKeyDown(KeyCode.K)) OnSkill(3);
        if (Input.GetKeyDown(KeyCode.L)) OnSkill(4);

        if (Input.GetKeyDown(KeyCode.O)) OnRestore(0, true);
        if (Input.GetKeyDown(KeyCode.H)) OnSummon(summon1.index, true);
        if (Input.GetKeyDown(KeyCode.N)) OnSummon(summon2.index, true);
        if (Input.GetKeyDown(KeyCode.M)) OnSummon(summon3.index, true);

        CheckKey(KeyCode.J, TOUCH_KEY.Attack);
        CheckKey(KeyCode.U, TOUCH_KEY.Skill1);
        CheckKey(KeyCode.I, TOUCH_KEY.Skill2);
        CheckKey(KeyCode.K, TOUCH_KEY.Skill3);
        CheckKey(KeyCode.L, TOUCH_KEY.Skill4);
        CheckKey(KeyCode.H, TOUCH_KEY.Summon1);
        CheckKey(KeyCode.N, TOUCH_KEY.Summon2);
        CheckKey(KeyCode.M, TOUCH_KEY.Summon3);

        if (CharacterManager.playerCS != null)
        {
            tpBtn.isEnabled = CharacterManager.playerCS.pm.NormalState() && !CharacterManager.playerCS.pm.isAutoMode;
        }
    }

    public void CancelTp()
    {
        if (!GameLibrary.isMoba || tpEffect1 == null || tpEffect2 == null)
            return;

        tpTimer = 0;
        progress.value = 0;
        progress.gameObject.SetActive(false);
        tpEffect1.SetActive(false);
        tpEffect2.SetActive(false);
        isTp = false;
    }

    void CheckKey(KeyCode k, TOUCH_KEY tk)
    {
        if (Input.GetKeyDown(k))
        {
            TouchHandler.GetInstance().Touch(tk);
        }
        if (Input.GetKeyUp(k))
        {
            TouchHandler.GetInstance().Release(tk);
        }
    }

    public void ChangeAllCDTo(bool b)
    {
        if (b != allSkillCD)
        {
            allSkillCD = b;
            skill1.IsEnabled = !b;
            skill2.IsEnabled = !b;
            skill3.IsEnabled = !b;
            skill4.IsEnabled = !b;
        }
    }

    public SkillBtnCD GetSkillBtn(int site)
    {
        for (int i = 0; i < skillBtns.Length; i++)
        {
            if (skillBtns[i].skillNode.site == site)
                return skillBtns[i];
        }
        return null;
    }

    public SkillBtnCD GetSkillBtnBySkillId ( long sid )
    {
        for(int i = 0; i < skillBtns.Length; i++)
        {
            if(skillBtns[i].skillNode.skill_id == sid)
                return skillBtns[i];
        }
        return null;
    }

    public SkillBtnCD GetSummonBtn(int index)
    {
        for (int i = 0; i < skillBtns.Length; i++)
        {
            if (skillBtns[i].index == index)
                return skillBtns[i];
        }
        return null;
    }

    SkillNode GetSkillNodeBySeat(int skillIndex)
    {
        SkillNode mTempSkillNode = null;
        long[] skillIds = Globe.Heros() != null && Globe.Heros()[0] != null ? Globe.Heros()[0].node.skill_id : Globe.playHeroList[0].node.skill_id;
        for (int i = 0; i < skillIds.Length; i++)
        {
            if (GameLibrary.skillNodeList.ContainsKey(skillIds[i]) && GameLibrary.skillNodeList[skillIds[i]].seat == 5 - skillIndex)
            {
                mTempSkillNode = GameLibrary.skillNodeList[skillIds[i]];
                break;
            }
        }
        return mTempSkillNode;
    }

    SkillNode GetSkillNodeByIndex(int skillIndex)
    {
        if (!GameLibrary.isNetworkVersion && Globe.Heros()[0] == null)
            return FSDataNodeTable<SkillNode>.GetSingleton().DataNodeList[new HeroData(GameLibrary.player).node.skill_id[skillIndex]];
        if (Globe.Heros()[0] != null)
            return FSDataNodeTable<SkillNode>.GetSingleton().DataNodeList[Globe.Heros()[0].node.skill_id[skillIndex]];
        else
            return null;
    }

    public void OnSkill(int indexOfUI)
    {
        if (CharacterManager.playerCS == null || CharacterManager.playerCS.isDie) return;
        FightTouchDelegate(TOUCH_KEY.Skill4);
        CharacterManager.instance.SwitchAutoMode(false);
        CancelTp();
        if (TaskAutoTraceManager._instance != null)
            TaskAutoTraceManager._instance.StopTaskAutoFindWay();
        if (CharacterManager.playerCS.pm.isRiding)
        {
            indexOfSkillToUse = indexOfUI;
            InvokeRepeating("ContinueCheckSkill", 0f, 0.02f);
            CancelRiding();
        }
        else
        {
                UseSkill(indexOfUI);
            }
        }

    int indexOfSkillToUse;
    void ContinueCheckSkill()
    {
        if (CheckRiding())
        {
            UseSkill(indexOfSkillToUse);
            CancelInvoke("ContinueCheckSkill");
        }
    }

    void UseSkill(int indexOfUI)
    {
        HeroData hd = null != Globe.Heros() && null != Globe.Heros()[0] ? Globe.Heros()[0] : Globe.playHeroList[0];
        SkillNode skillNode = GetSkillNodeByIndex(hd.node.skill_id.Length - 1 - (4 - indexOfUI) % 4);
        if (!Enabled || allSkillCD)
            return;
        if (GetSkillBtn(indexOfUI).isCD)
            return;
        CharacterManager.instance.shouldMove = false;
        if (CharacterManager.instance.PlayerSkill(skillNode, indexOfUI + 3))
        {
            startCd(indexOfUI);
        }
    }

    public void startCd(int indexOfUI)
    {
        GetSkillBtn(indexOfUI).StartCd();
    }

    void OnAttack(bool b)
    {
        //Debug.Log("ON ATTACK " + b);
        if (!Enabled)
            return;
        if (b)
            TouchHandler.GetInstance().Touch(GetSkillStatusByIndex(0));
        else
            TouchHandler.GetInstance().Release(GetSkillStatusByIndex(0));

        if (!b)
            return;

        FightTouchDelegate(TOUCH_KEY.Attack);

        CharacterManager.instance.shouldMove = false;
        CharacterManager.instance.SwitchAutoMode(false);
        CancelTp();
        CancelRiding();
        if (TaskAutoTraceManager._instance != null)
            TaskAutoTraceManager._instance.StopTaskAutoFindWay();
        attack.Effect.SetActive(true);
        BattleUtil.PlayParticleSystems(attack.Effect);
        if (CharacterManager.playerCS != null && CharacterManager.playerCS.pm != null)
        {
            SkillNode skillNode = GetSkillNodeByIndex(CharacterManager.playerCS.pm.AttackCount - 1);
            if (skillNode.site != 0) return;
            CharacterManager.instance.PlayerSkill(skillNode, CharacterManager.playerCS.pm.AttackCount);
        }
    }

    void OnSummon(int index, bool b)
    {
        CancelTp();
        if (TaskAutoTraceManager._instance != null)
            TaskAutoTraceManager._instance.StopTaskAutoFindWay();
        CancelRiding();
        if (b)
            TouchHandler.GetInstance().Touch(GetSkillStatusByIndex(index + 4));
        else
            TouchHandler.GetInstance().Release(GetSkillStatusByIndex(index + 4));

        DoSummon(index, Globe.Heros()[index - 4]);
    }

    public void DoSummon(int index, HeroData hd)
    {
        if (!IsSummonCanUse(index, hd) || CharacterManager.playerCS == null || CharacterManager.playerCS.isDie)
            return;

        if (null != CharacterManager.playerCS && null == CharacterManager.playerCS.attackTarget)
        {
            if (null != OnBtnTargetNil)
            {
                OnBtnTargetNil(GetSkillStatusByIndex(index));
                return;
            }
        }

        FightTouchDelegate(GetSkillStatusByIndex(index));
        CharacterManager.player.GetComponent<SummonHero>().Summon(hd);
        for (int i = 5; i < 8; i++)
        {
            if (GetSummonBtn(i) != null)
            {
                GetSummonBtn(i).StartCd();
            }
        }
    }

    public bool IsSummonCanUse(int index, HeroData hd)
    {
        return hd != null && hd.id != 0 && GetSummonBtn(index) != null && !GetSummonBtn(index).isCD;
    }

    public TOUCH_KEY GetSkillStatusByIndex(int index)
    {
        switch (index)
        {
            case 0:
                return TOUCH_KEY.Attack;
            case 1:
                return TOUCH_KEY.Skill1;
            case 2:
                return TOUCH_KEY.Skill2;
            case 3:
                return TOUCH_KEY.Skill3;
            case 4:
                return TOUCH_KEY.Skill4;
            case 5:
                return TOUCH_KEY.Summon1;
            case 6:
                return TOUCH_KEY.Summon2;
            case 7:
                return TOUCH_KEY.Summon3;
            default:
                break;
        }
        return TOUCH_KEY.Idle;
    }

    public void SetAllBtnLockStatus(bool isLock)
    {
        this.isLock = isLock;
        SetBtnLockStatus(autoBattle.gameObject, isLock);
        SetBtnLockStatus(attack.gameObject, isLock);
        SetBtnLockStatus(skill1.gameObject, isLock);
        SetBtnLockStatus(skill2.gameObject, isLock);
        SetBtnLockStatus(skill3.gameObject, isLock);
        SetBtnLockStatus(skill4.gameObject, isLock);
        SetBtnLockStatus(summon1.gameObject, isLock);
        SetBtnLockStatus(summon2.gameObject, isLock);
        SetBtnLockStatus(summon3.gameObject, isLock);
        SetBtnLockStatus(skillRestore.gameObject, isLock);
    }

    public void SetSkillBtnLockStatus(bool isLock)
    {
        SetBtnLockStatus(skill1.gameObject, isLock);
        SetBtnLockStatus(skill2.gameObject, isLock);
        SetBtnLockStatus(skill3.gameObject, isLock);
        SetBtnLockStatus(skill4.gameObject, isLock);
    }

    void SetBtnLockStatus(GameObject btn, bool isLock)
    {
        if (btn.transform.FindChild("Lock"))
            btn.transform.FindChild("Lock").gameObject.SetActive(isLock);
        else
            Debug.Log("Can not find lock." + btn.name);
    }

    public void SetSkillEffect(bool isShow)
    {
        for (int i = 0; i < skillBtns.Length; i++)
        {
            skillBtns[i].isShowEffect = isShow;
        }
    }

    void CancelRiding()
    {
        if (CharacterManager.playerCS != null)
        {
            if (CharacterManager.playerCS.pm.isRiding)
            {
                ClientSendDataMgr.GetSingle().GetPetSend().SendChangeMountOrPetState(C2SMessageType.ActiveWait, 2, MountAndPetNodeData.Instance().currentMountID, 0);
            }
            else if (UIRole.instance != null && UIRole.instance.riding)
            {
                UIRole.instance.CancelRide();
            }
            else
            {
            }
        }
    }

    bool CheckRiding()
    {
        if (CharacterManager.playerCS != null && !CharacterManager.playerCS.pm.isRiding && (CharacterManager.playerCS.pm.ani.GetCurrentAnimatorStateInfo(0).IsName("Prepare") || CharacterManager.playerCS.pm.ani.GetCurrentAnimatorStateInfo(0).IsName("Run")))
        {
            return true;
        }
        return false;
    }

    #region 战斗新手引导

    public void HideAllFightBtn()
    {
        Transform fightTouch = transform.Find("FightTouch");
        Transform pauseBtn = transform.Find("Btn");
        if (null != fightTouch )
            HideAllBtn(fightTouch);
        if (null != pauseBtn)
            HideAllBtn(pauseBtn);
    }

    void HideAllBtn(Transform touch)
    {
        for (int i = 0; i < touch.childCount; i++)
        {
            touch.GetChild(i).gameObject.SetActive(false);
        }
    }

    public Transform ShowFightBtn(TOUCH_KEY key)
    {
        if (!Globe.isFightGuide) return null;
        switch (key)
        {
            case TOUCH_KEY.Attack:
                attack.gameObject.SetActive(true);
                return attack.transform;
            case TOUCH_KEY.Skill1:
            case TOUCH_KEY.Skill2:
            case TOUCH_KEY.Skill3:
            case TOUCH_KEY.Skill4:
                skill1.gameObject.SetActive(true);
                skill2.gameObject.SetActive(true);
                skill3.gameObject.SetActive(true);
                skill4.gameObject.SetActive(true);
                return skill4.transform;
            case TOUCH_KEY.Summon1:
                summon1.gameObject.SetActive(true);
                return summon1.transform;
            case TOUCH_KEY.Summon2:
                summon2.gameObject.SetActive(true);
                return summon2.transform;
            case TOUCH_KEY.Summon3:
                summon3.gameObject.SetActive(true);
                return summon3.transform;
        }
        return null;
    }

    public void HideSummonHero(TOUCH_KEY key)
    {
        switch (key)
        {
            case TOUCH_KEY.Summon1:
                SetBtnState(summon1.GetComponent<UISprite>(), false);
                break;
            case TOUCH_KEY.Summon2:
                SetBtnState(summon2.GetComponent<UISprite>(), false);
                break;
            case TOUCH_KEY.Summon3:
                summon1.SetCd(15);
                summon2.SetCd(15);
                summon3.SetCd(15);
                SetBtnState(summon1.GetComponent<UISprite>());
                SetBtnState(summon2.GetComponent<UISprite>());
                SetBtnState(summon3.GetComponent<UISprite>());
                break;
        }
        
    }

    public void InvokeSummon(int index)
    {
        OnSummon(index, true);
    }

    public void SetFightBtnStatus(bool isLock)
    {
        SetBtnCollider(autoBattle.gameObject, isLock);
        SetBtnCollider(attack.gameObject, isLock);
        SetBtnCollider(skill1.gameObject, isLock);
        SetBtnCollider(skill2.gameObject, isLock);
        SetBtnCollider(skill3.gameObject, isLock);
        SetBtnCollider(skill4.gameObject, isLock);
        SetBtnCollider(summon1.gameObject, isLock);
        SetBtnCollider(summon2.gameObject, isLock);
        SetBtnCollider(summon3.gameObject, isLock);
        SetBtnCollider(skillRestore.gameObject, isLock);
    }

    void SetBtnCollider(GameObject go, bool isEnable)
    {
        if (go.GetComponent<BoxCollider>())
        {
            go.GetComponent<BoxCollider>().enabled = isEnable;
        }
    }

    void SetBtnState(UISprite sprite, bool isEnable = true)
    {
        sprite.color = isEnable ? new Color(1, 1, 1) : new Color(0, 0, 0);
        sprite.GetComponent<BoxCollider>().enabled = isEnable;
    }

    void FightTouchDelegate(TOUCH_KEY key)
    {
        if (null != OnTouchBtn)
            OnTouchBtn(key);
    }

    public void CopySummonBtn(SkillBtnCD summonBtn, TOUCH_KEY key)
    {
        switch (key)
        {
            case TOUCH_KEY.Summon1:
                InitSummonBtnState(summonBtn, 5);
                break;
            case TOUCH_KEY.Summon2:
                InitSummonBtnState(summonBtn, 6);
                break;
            case TOUCH_KEY.Summon3:
                InitSummonBtnState(summonBtn, 7);
                break;
        }
    }

    #endregion

    public void ClearAttackBtn()
    {
        attack.ClearBtn();
    }

}
