using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Tianyu;
using UnityEngine.SceneManagement;
using System;

public enum GameLayer
{
    Player = 9,
    Monster = 10,
    Tower = 11,
    Obstacle = 12,
    BossShow = 14
}

public enum NavAreas
{
    Walkable = 0,
    NotWalkable = 1,
    Jump = 2,
    Obstacle = 3,
    BlueHeroPass = 9,
    RedHeroPass = 18
}

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager instance;
    public static GameObject player;
    public static CharacterState playerCS;
    public static UInt32 playerGroupIndex = 1;

    public GameObject[] autos;
    public int lifeCount = 1;

    [HideInInspector]
    public bool shouldMove = false;
    public CharacterState lockedTarget; // 当前手动锁定的目标

    public Dictionary<CharacterState, GameObject> mTowerCircle = new Dictionary<CharacterState, GameObject>();

    private GameObject mShowTips;
    private TweenAlpha mShowTipsTween;
    PlayerMotion playerMotion;
    private Vector3 mTouchPos;
    private bool mIsTouched;
    private string mCurActiveSceneName;
    private Touch[] mTouches;
    public bool isInit = false;
    void Awake()
    {
        GameLibrary.playerstate = 0;//重置为游戏状态
        instance = this;
        //获取当前场景名字
        mCurActiveSceneName = SceneManager.GetActiveScene().name;
        //创建人物后 添加任务自动寻路脚本
        if (transform.gameObject.GetComponent<TaskAutoTraceManager>() == null)
        {
            transform.gameObject.AddComponent<TaskAutoTraceManager>();
        }
        if (stateobj == null)
        {
            stateobj = Resources.Load<GameObject>("Prefab/UIPanel/" + "NpcTaskState") as GameObject;
        }
        if (headtipobj == null)
        {
            headtipobj = Resources.Load<GameObject>("Prefab/UIPanel/" + "NpcHeadTip") as GameObject;
        }
        if (GameLibrary.Instance().CheckSceneNeedSync(mCurActiveSceneName) && CreatePeople.GetInstance() != null
            && transform.parent != null && CreatePeople.GetInstance().transform.localPosition != Vector3.zero)
        {
            CreatePeople.GetInstance().transform.localPosition = transform.parent.localPosition;
        }
    }

    private void AddShowTips()
    {
        GameObject prefab = Resources.Load("Prefab/ShowTips") as GameObject;
        if (prefab != null)
        {
            // mShowTips = NGUITools.AddChild(SceneUIManager.instance.gameObject, prefab);
            mShowTips = NGUITools.AddChild(FindObjectOfType<UIRoot>().gameObject, prefab);
            mShowTipsTween = mShowTips.GetComponentInChildren<TweenAlpha>();
            mShowTips.SetActive(false);
        }
    }

    public void ShowTip(string str = "")
    {
        if (mShowTips == null)
        {
            AddShowTips();
        }
        if (mShowTips != null && !mShowTips.activeSelf)
        {
            mShowTips.gameObject.SetActive(true);
            if (!string.IsNullOrEmpty(str))
                mShowTips.GetComponentInChildren<UILabel>().text = str;
            mShowTipsTween.ResetToBeginning();
        }
    }

    Vector3 mPos;
    Quaternion mRot;
    Vector3 mScale;
    public void CreateTownPlayer()
    {
        if (player != null)
        {
            if (SceneManager.GetActiveScene().name == GameLibrary.UI_Major)
            {
				transform.localPosition = Vector3.zero;

				mPos = playerData.GetInstance().selfData.GetPos();
            }
            else if (SceneManager.GetActiveScene().name == GameLibrary.LGhuangyuan)
            {
                mPos = FSDataNodeTable<TransferTableNode>.GetSingleton().DataNodeList[4294713000].toPos;
                mRot = player.transform.localRotation;
            }
            else if (SceneManager.GetActiveScene().name == GameLibrary.PVP_1V1)
            {
                mPos = playerData.GetInstance().selfData.GetPos();
            }
            else
            {
                mPos = player.transform.localPosition;
            }

            mScale = player.transform.localScale;
            if (player != null)
            {
                mPos = player.transform.localPosition;
                Destroy(player);
            }

        }
        else
        {
            transform.localPosition = Vector3.zero;

            mPos = playerData.GetInstance().selfData.GetPos();

        }
        HeroData defaultHd = DefaultMainHeroData();

        GameObject go = Resource.CreateCharacter(defaultHd.node.icon_name, gameObject);
        CharacterState cs = BattleUtil.AddMoveComponents(go, defaultHd.attrNode.modelNode);
        cs.pm.nav.obstacleAvoidanceType = UnityEngine.AI.ObstacleAvoidanceType.NoObstacleAvoidance;
        cs.InitData(defaultHd);
        SetMainHero(cs);

        player.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;  // 防止生成的模型位置出错
        player.transform.localPosition = mPos;
        player.transform.localRotation = mRot;
        player.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = true;
        if (mCurActiveSceneName == GameLibrary.UI_Major)
        {
            Debug.Log("Major init player");
            if (!isInit)
            {
                player.transform.parent = CreatePeople.GetInstance().OhterPlayer.transform;
                player.transform.localPosition = playerData.GetInstance().selfData.GetPos();
                isInit = true;
            }
            //CreateNpc();
            //CreateCollect();
        }
        else if (mCurActiveSceneName == GameLibrary.LGhuangyuan || mCurActiveSceneName == GameLibrary.PVP_1V1)
        {
            Debug.Log(mCurActiveSceneName + " init player");
            if (!isInit)
            {
                player.transform.parent = CreatePeople.GetInstance().OhterPlayer.transform;
                player.transform.localPosition = playerData.GetInstance().selfData.GetPos();
                isInit = true;
            }
            //是野外向服务器同步血量
            CharacterState csp = player.GetComponent<CharacterState>();
            if (csp != null)
            {
                csp.InitData(Globe.Heros()[0]);
                csp.keyId = playerData.GetInstance().selfData.keyId;
                csp.isNetworking = true;
                ClientSendDataMgr.GetSingle().GetWalkSend().SendSetPlayerHp(csp.maxHp);
                playerData.GetInstance().selfData.hp = csp.maxHp;
            }
            UnityUtil.AddComponetIfNull<SummonHero>(csp.gameObject);


        }
        if (!GameLibrary.Instance().isLoadOtherPepole)
        {
            if (SceneManager.GetActiveScene().name == GameLibrary.UI_Major
            || SceneManager.GetActiveScene().name == GameLibrary.LGhuangyuan
            || SceneManager.GetActiveScene().name == GameLibrary.PVP_1V1
           )
            {
                Debug.Log(" =====>CreateOthers");
                CreateNearElement();
                GameLibrary.Instance().isLoadOtherPepole = true;
            }
        }
        if (player != null)
        {
            if (player.transform.GetComponent<SetMainHeroName>() == null)
                player.transform.gameObject.AddComponent<SetMainHeroName>();
        }
        GameLibrary.isSkipingScene = false;
        //if (MiniMap.instance != null)
        //    MiniMap.instance.CreateTargetPos(player, ShowType.player);
    }
    public void CreateNearElement()
    {
        RoleInfo ri;
        foreach (RoleInfo ritem in playerData.GetInstance().NearRIarr.Values)
        {
            ri = ritem;
            if (ri.RoleObj == null)
            {
                CreatePeople.GetInstance().CreatOtherObject(ref ri);
            }
        }
    }

    public HeroData DefaultMainHeroData()
    {
        HeroData hd = null;
        if (Globe.playHeroList[0] == null || Globe.playHeroList[0].id == 0)
        {
            if (!GameLibrary.isNetworkVersion)
                Globe.fightHero[0] = (int)GameLibrary.player;
            else
                GameLibrary.player = Globe.fightHero[0];
            playerData.GetInstance().AddHeroToList(Globe.fightHero[0]);
        }
        if (mCurActiveSceneName == GameLibrary.UI_Major || mCurActiveSceneName == GameLibrary.LGhuangyuan)
        {
            hd = Globe.playHeroList[0];
        }
        else
        {
            hd = Globe.Heros()[0];
            if (null == hd)
                hd = new HeroData(GameLibrary.player);
        }
        hd.state = Modestatus.Player;
        hd.groupIndex = 1;
        return hd;
    }

    public void SetMainHero(CharacterState cs)
    {
        player = cs.gameObject;
        playerCS = cs;
        playerCS.OnDead += (c) =>
        {
            SwitchAutoMode(false);
            redCircle.SetActive(false);
            //if (playerCS.attackTarget != null && playerCS.attackTarget.redCircle != null)
            //{
            //    playerCS.attackTarget.redCircle.gameObject.SetActive(false);
            //}
        };
        playerGroupIndex = playerCS.groupIndex;
        playerMotion = playerCS.pm;
        playerMotion.isAutoMode = false;
        if (ThirdCamera.instance != null && player != null && !GameLibrary.SceneType(SceneType.PVP3))
            ThirdCamera.instance._MainPlayer = player.transform;
        shouldMove = false;
        playerMotion.Stop();
    }

    bool controlState;
    void Update()
    {
        if (mCurActiveSceneName == GameLibrary.UI_Major || mCurActiveSceneName == GameLibrary.PVP_Zuidui) return;

        if (SceneBaseManager.instance != null)
        {
            CheckLockTargetOutScreen();
            if (playerCS != null && !playerCS.isDie)
            {
                SetLockTarget();

                if (controlState != GameLibrary.Instance().CanControlSwitch(playerCS.pm))
                {
                    controlState = GameLibrary.Instance().CanControlSwitch(playerCS.pm);
                }
                if (FightTouch._instance != null)
                {
                    //FightTouch._instance.ChangeAllCDTo(!playerCS.pm.CanSkillState());
                    //FightTouch._instance.ChangeAllCDTo(!GameLibrary.Instance().CanControlSwitch(playerCS.pm));
                }

                if (!playerCS.pm.isAutoMode)
                {
                    CharacterState targetToSelect = (lockedTarget != null) ? lockedTarget : GetAttackTarget(playerCS.TargetRange, playerCS);
                    ChangePlayerAttackTargetTo(targetToSelect);
                }
                else
                {
                    if (redCircle != null) redCircle.SetActive(false);
                }
            }
            else
            {
                lockedTarget = null;
            }
        }

        if (shouldMove)
        {
            GameLibrary.Instance().CheckLockTargetCastSkillValid(playerCS, playerCS.mCurSkillNode);
            if (playerCS == null || playerCS.attackTarget == null || playerCS.isDie || playerCS.attackTarget.isDie)
            {
                PlayerStop();
                return;
            }
            Rotating();
            float tempDistance = Vector3.Distance(playerCS.transform.position, playerCS.attackTarget.transform.position);
            if (playerCS.mCurSkillNode != null)
            {
                if (tempDistance < GameLibrary.Instance().GetSkillDistBySkillAndTarget(playerCS, playerCS.mCurSkillNode))
                {
                    ReleaseSkill();
                }
                else
                {
                    PlayerMove(playerCS.attackTarget.transform.position - playerCS.transform.position);
                }
            }
        }
    }

    void SetLockTarget()
    {
        mTouches = Input.touches;
        if (Input.GetMouseButtonDown(0) || mTouches.Length > 0)
        {
            if (mTouches.Length > 0)
            {
                for (int i = 0; i < mTouches.Length; i++)
                {
                    Touch touch = mTouches[i];
                    if (touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled)
                    {
                        mIsTouched = true;
                        mTouchPos = touch.position;
                    }
                }
            }
            else
            {
                mIsTouched = true;
                mTouchPos = Input.mousePosition;
            }
        }
        if (mIsTouched)
        {
            mIsTouched = false;
            Ray ray = Camera.main.ScreenPointToRay(mTouchPos);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, GameLibrary.GetMaxRayDistance(), GameLibrary.GetAllLayer()))
            {
                CharacterState hitCS = hit.transform.GetComponent<CharacterState>();
                if (hitCS != null && hitCS.groupIndex != playerGroupIndex)
                    lockedTarget = hitCS;
            }
        }
    }

    void CheckLockTargetOutScreen()
    {
        if (lockedTarget != null)
        {
            Vector3 targetPos = Camera.main.WorldToViewportPoint(lockedTarget.transform.position);
            if (targetPos.x < 0 || targetPos.x > 1 || targetPos.y < 0 || targetPos.y > 1 || lockedTarget.isDie)
                lockedTarget = null;
        }
    }

    public CharacterState GetAttackTarget(float radius = 2f, CharacterState attacker = null)
    {
        if (SceneBaseManager.instance == null) return null;
        CharacterState target = null;
        CharacterState minHpHero = null;
        CharacterState nearestHero = null;
        CharacterState nearestTarget = null;
        float disHeroFinal = float.MaxValue;
        float disFinal = float.MaxValue;
        float targetDis = 0;
        for (int i = 0; i < SceneBaseManager.instance.agents.size; i++)
        {
            CharacterState chs = SceneBaseManager.instance.agents[i];
            if (null == chs || chs.isDie) continue;
            targetDis = GameLibrary.Instance().CheckIsWildCs(chs) ? playerCS.AttackRange : radius;
            if (BattleUtil.IsTargeted(attacker, chs, targetDis))
            {
                if (chs.state == Modestatus.Boss)
                    return chs;
                float dis = Vector3.Distance(attacker.transform.position, chs.transform.position);
                if (chs.state == Modestatus.NpcPlayer)
                {
                    if (minHpHero == null) minHpHero = chs;
                    if (chs.currentHp < minHpHero.currentHp)
                    {
                        minHpHero = chs;
                    }
                    if (nearestHero == null) nearestHero = chs;
                    if (dis < disHeroFinal)
                    {
                        disHeroFinal = dis;
                        nearestHero = chs;
                    }
                }
                else
                {
                    if (nearestTarget == null)
                        nearestTarget = chs;
                    if (dis < disFinal)
                    {
                        disFinal = dis;
                        nearestTarget = chs;
                    }
                }
            }
        }
        target = (minHpHero != null) ? minHpHero : ((nearestHero != null) ? nearestHero : nearestTarget);
        return target;
    }

    public GameObject attackCircle;
    public GameObject redCircle;
    public void ChangePlayerAttackTargetTo(CharacterState newTarget)
    {
        //if(attackCircle == null)
        //{
        //    attackCircle = Resource.CreatPrefabs("quan_hero", player, Vector3.zero, "Effect/Prefabs/UI/");
        //    attackCircle.SetActive(true);
        //    attackCircle.transform.localScale = Vector3.one * playerCS.AttackRange / attackCircle.transform.lossyScale.x;
        //}
        if (redCircle == null)
        {
            redCircle = Resource.CreatPrefabs("Effect_targetselected01", gameObject, Vector3.zero, "Effect/Prefabs/Targetselected/");
        }
        redCircle.SetActive(null != newTarget && newTarget != playerCS);
        playerCS.SetAttackTargetTo(newTarget);
        if (null != newTarget)
        {
            float radiusRate = newTarget.CharData.attrNode.modelNode.colliderRadius * 4f;
            redCircle.transform.localScale = radiusRate * newTarget.transform.localScale;
            redCircle.transform.position = newTarget.transform.position;
        }
    }

    void Rotating()
    {
        if (!playerMotion.CanMoveState()) return;
        Vector3 relativePos = playerCS.attackTarget.transform.position - playerCS.transform.position;
        if (relativePos != Vector3.zero)
        {
            playerCS.transform.forward = relativePos;
            playerCS.transform.localEulerAngles = new Vector3(0, playerCS.transform.localEulerAngles.y, 0);
        }
    }

    public void SwitchAutoMode(bool isAuto)
    {
        if (SceneUIManager.instance != null)
        {
            SceneUIManager.instance.autoFight.SetActive(isAuto);
            ResetAutoFightEffect(SceneUIManager.instance.autoFight.transform);
        }
        if (FightTouch._instance != null)
        {
            FightTouch._instance.autoBattleEffect.SetActive(isAuto);
        }

        if (!PlayerAlive())
            return;
        BasePlayerAI ai = player.GetComponent<BasePlayerAI>();
        if (ai != null)
        {
            ai.enabled = isAuto;
            if(isAuto)
            {
                AISkillHandler aisHandler = player.GetComponent<AISkillHandler>();
                if(aisHandler != null && FightTouch._instance != null)
                    aisHandler.SetSkills(playerCS.GetSkills(), GetCds());
            }
        }
        if (playerMotion.isAutoMode != isAuto)
        {
            playerMotion.isAutoMode = isAuto;
            playerMotion.nav.enabled = false;
            playerMotion.nav.enabled = true;
            if (!isAuto) playerMotion.Stop();
        }
    }

    List<float> GetCds ()
    {
        List<float> ret = new List<float>();
        List<long> sids = playerCS.GetSkills();
        for(int i = 0; i < sids.Count; i++)
        {
            SkillBtnCD sBtn = FightTouch._instance.GetSkillBtnBySkillId(sids[i]);
            //if(sBtn != null)
                ret.Add(sBtn.CD);
        }
        return ret;
    }

    void ResetAutoFightEffect(Transform trans)
    {
        TweenPosition[] tps = trans.GetComponentsInChildren<TweenPosition>();
        for (int i = 0; i < tps.Length; i++)
        {
            tps[i].ResetToBeginning();
        }
    }

    public GameObject GetMonster(string name)
    {
        string monName = name;
        int indexOfSpliter = name.IndexOf("_");
        if (indexOfSpliter != -1 && monName.Substring(0, indexOfSpliter) == "yx")
        {
            return Resources.Load(GameLibrary.Hero_URL + name) as GameObject;
        }
        else
        {
            return Resources.Load(GameLibrary.Monster_URL + name) as GameObject;
        }
    }

    public static bool PlayerAlive()
    {
        return player != null && playerCS != null && !playerCS.isDie;
    }

    public void PlayerMove(Vector3 v)
    {
        if (PlayerAlive()) playerMotion.Move(v);
    }

    public void PlayerStop()
    {
        if (PlayerAlive()) playerMotion.Stop();
    }

    public void PlayerAttack()
    {
        if (PlayerAlive()) playerMotion.ContinuousAttack();
    }

    public void PlayerSkill(int index)
    {
        if (PlayerAlive()) playerMotion.Skill(index);
    }

    public void RotatePlayer(Vector3 v)
    {
        Vector3 q = Quaternion.Euler(0, 0, 0) * v;
        Quaternion qq = Quaternion.LookRotation(q);
        if (player != null)
        {
            player.transform.rotation = Quaternion.Lerp(player.transform.rotation, qq, Time.deltaTime * 100);
        }
    }

    public int mCurIndex;
    public bool isUseSkill;

    private void ReleaseSkill()
    {
        shouldMove = false;
        if (isUseSkill)
        {
            if (playerMotion.canSkill && playerMotion.canSkillSwitch <= 0 && FightTouch._instance != null)
            {
                PlayerSkill(mCurIndex);
                FightTouch._instance.startCd(mCurIndex);
            }
        }
        else
        {
            if (playerMotion.canAttack && playerMotion.canAttackSwitch <= 0)
            {
                PlayerAttack();
            }
        }
    }



    public bool PlayerSkill(SkillNode skillNode, int indexOfSkillArray)
    {
        bool isUseSkill = indexOfSkillArray > 3;
        if (player == null || playerCS.isDie || !GameLibrary.Instance().CanControlSwitch(playerMotion) || (isUseSkill && !(playerMotion.canSkill && playerMotion.canSkillSwitch <= 0)) || (!isUseSkill && !(playerMotion.canAttack && playerMotion.canAttackSwitch <= 0))) return false;
        shouldMove = false;
        playerCS.mCurSkillNode = skillNode;
        mCurIndex = indexOfSkillArray - 3;
        this.isUseSkill = isUseSkill;
        GameLibrary.Instance().CheckLockTargetCastSkillValid(playerCS, skillNode);
        bool canUseSkill = false;
        switch (skillNode.target)
        {
            //不需要目标，随意释放
            case TargetState.None:
                if (playerCS.attackTarget != null && skillNode.dist != 0)
                {
                    float tempDistance = Vector3.Distance(playerCS.transform.position, playerCS.attackTarget.transform.position);
                    if (tempDistance < GameLibrary.Instance().GetSkillDistBySkillAndTarget(playerCS, skillNode))
                    {
                        canUseSkill = true;
                    }
                    else
                    {
                        shouldMove = true;
                    }
                }
                else
                {
                    canUseSkill = true;
                }
                break;
            //需要目标，没有目标无法施法，有目标先跑向目标满足施法距离再施法
            case TargetState.Need:
                if (playerCS.attackTarget != null)
                {
                    shouldMove = true;
                }
                else
                {
                    //if (mShowTips == null)
                    //{
                    //    AddShowTips();
                    //}
                    //if (mShowTips != null && !mShowTips.activeSelf)
                    //{
                    //    mShowTips.gameObject.SetActive(true);
                    //    mShowTipsTween.ResetToBeginning();
                    //}
                    ShowTip();
                }
                break;
            default:
                break;
        }
        if (canUseSkill)
        {
            if (isUseSkill)
            {
                PlayerSkill(indexOfSkillArray - 3);
            }
            else
            {
                PlayerAttack();
            }
        }
        return canUseSkill;
    }

    public Transform npcControl;
    public Transform UIControl;
    public GameObject stateobj;
    public GameObject headtipobj;

    //npc创建 暂时先自己创建用于任务，之后全部动态创建
    private int[] npclist = { 101, 102, 103 };
    private Vector3[] vec = { new Vector3(0.1f, 11.55f, 4.31f), new Vector3(1.108f, 11.297f, 0.079f), new Vector3(0.10162f, 11.598f, -13.565f) };
    public void CreateNpc()
    {
        if (mCurActiveSceneName == GameLibrary.UI_Major)
        {
            for (int i = 0; i < npclist.Length; i++)
            {
                int npcid = npclist[i];
                NPCNode npcNode = FSDataNodeTable<NPCNode>.GetSingleton().FindDataByType(npcid);
                ModelNode modelNode = FSDataNodeTable<ModelNode>.GetSingleton().FindDataByType(npcNode.modelid);
                string respath = modelNode.respath;
                GameObject go = Instantiate(Resources.Load(respath)) as GameObject;
                go.transform.parent = npcControl;
                go.transform.position = vec[i];
                go.transform.rotation = npcNode.rota;
                go.AddComponent<TaskNPC>();
                go.GetComponent<TaskNPC>().NPCID = npcid;
                NPCManager.GetSingle().CreateNpcModel(npcid, go);


                string footEffctpath = "Effect/Prefabs/UI/jiaodgq";
                GameObject footgo = Instantiate(Resources.Load(footEffctpath)) as GameObject;
                footgo.transform.parent = go.transform;
                footgo.transform.position = vec[i];
                footgo.transform.localScale = Vector3.one;
                footgo.name = "jiaodgq";
                footgo.SetActive(false);

                string toudjstbpath = "Effect/Prefabs/UI/toudjstb";
                GameObject toudjstb = Instantiate(Resources.Load(toudjstbpath)) as GameObject;
                toudjstb.transform.parent = go.transform;
                toudjstb.transform.position = vec[i];// 位置设在头顶
                toudjstb.transform.localScale = Vector3.one;
                toudjstb.name = "toudjstb";
                toudjstb.SetActive(false);

                string toudwctbpath = "Effect/Prefabs/UI/toudwctb";
                GameObject toudwctb = Instantiate(Resources.Load(toudwctbpath)) as GameObject;
                toudwctb.transform.parent = go.transform;
                toudwctb.transform.position = vec[i];// 位置设在头顶
                toudwctb.transform.localScale = Vector3.one;
                toudwctb.name = "toudwctb";
                toudwctb.SetActive(false);

            }
        }
    }

    public void CreateCollect()
    {
        int npcid = npclist[1];
        NPCNode npcNode = FSDataNodeTable<NPCNode>.GetSingleton().FindDataByType(npcid);
        if (FSDataNodeTable<ModelNode>.GetSingleton().DataNodeList.ContainsKey(npcNode.modelid))
        {
            ModelNode modelNode = FSDataNodeTable<ModelNode>.GetSingleton().DataNodeList[npcNode.modelid];
            if (modelNode != null)
            {
                string respath = modelNode.respath;
                respath = "Prefab/Renwu/xuelhuazy";//雪莲花
                GameObject go = Instantiate(Resources.Load(respath)) as GameObject;
                go.transform.parent = npcControl;
                go.transform.position = new Vector3(-7.0f, 11.56f, -0.8f);
                //go.transform.rotation = npcNode.rota;
                go.AddComponent<TaskCollectPoint>();
                go.GetComponent<TaskCollectPoint>().collectID = 4294718000;
                //NPCManager.GetSingle().CreateNpcModel(npcid, go);
            }
        }
    }
}
