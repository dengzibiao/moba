using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Tianyu;
using UnityEngine.SceneManagement;

public enum SceneType
{
    NONE,
    KM, // 推图玩法
    TP, // 占点玩法
    KV, // 护送玩法
    ES, // 逃亡玩法
    TD, // 塔防玩法
    BO, // 挑战BOSS
    ACT_GOLD,  // 活动金币副本
    ACT_EXP,   // 活动经验副本
    ACT_POWER, // 活动力量装备副本
    ACT_INTEL, // 活动智力装备副本
    ACT_AGILE, // 活动敏捷装备副本
    MB1, // MOBA 1V1玩法
    MB3, // MOBA 3V3玩法
    MB5, // MOBA 5V5玩法
    PVP3, //角斗场
    FD_LG, //野外
    Dungeons_MB1 // 副本MOBA玩法
}
/// <summary>
/// 场景元素管理
/// </summary>
public class SceneBaseManager : MonoBehaviour
{
    public static SceneBaseManager instance;
    public SceneType sceneType = SceneType.NONE;

    protected CharacterState player { get { return CharacterManager.playerCS; } }//玩家
    public BetterList<CharacterState> agents = new BetterList<CharacterState>();
    public BetterList<CharacterState> enemy = new BetterList<CharacterState>();
    public BetterList<CharacterState> friendly = new BetterList<CharacterState>();
    public BetterList<CharacterState> wildMonster = new BetterList<CharacterState>();
    public BetterList<CharacterState> wildMonsterRed = new BetterList<CharacterState>();
    public Dictionary<string, GameObject> enemyModel = new Dictionary<string, GameObject>();

    [HideInInspector]
    public List<EffectBlock> effectList = new List<EffectBlock>();
    public List<SpawnMonster> spwanList = new List<SpawnMonster>();
    public Dictionary<long, int> killDic = new Dictionary<long, int>();

    protected long currntTime = 0;
    protected int playerDeadNum = 0;
    protected bool isFinish = false;
    protected BossChuChang bossChuChang = null;

    [HideInInspector]
    public int dungeonTypes = 1;
    public int CollectionCount = 0;

    public string mCurrentSceneName;

    bool isWin = false;
    bool isDungeonsEnd = false;//是否副本结束 

    protected Transform airWallPos = null;

    public List<LevelConfigNode> curlevelConfig = new List<LevelConfigNode>();

    void Awake()
    {
        instance = this;
        mCurrentSceneName = SceneManager.GetActiveScene().name;

        //if (Globe.isUserPool)
        //{
        //    GameObject pool = new GameObject("ObjectPool");
        //    pool.AddComponent<ObjectPool>();
        //}
    }

    void OnDestroy()
    {
        OnSceneDestroy();
    }
    protected virtual void OnSceneDestroy()
    {
        sceneType = SceneType.NONE;
        CHandleMgr.GetSingle().msgDishandled.Clear();
    }

    public virtual void CreateScenePrefab(GameObject sceneCtrl)
    {
        if (null == sceneCtrl) return;
        Globe.isFB = true;

        foreach (int id in FSDataNodeTable<LevelConfigNode>.GetSingleton().DataNodeList.Keys)
        {
            if (FSDataNodeTable<LevelConfigNode>.GetSingleton().DataNodeList[id].levelID == GameLibrary.dungeonId)
            {
                curlevelConfig.Add(FSDataNodeTable<LevelConfigNode>.GetSingleton().DataNodeList[id]);
            }
        }

        if (curlevelConfig.Count <= 0)
        {
            Debug.LogError("levelConfig table did not find id " + GameLibrary.dungeonId);
            return;
        }
        CharacterManager character = CreatePlayer(curlevelConfig[0].playerPos, sceneCtrl.transform);
        if (null != character)
            character.autos = CreateAutoPoint(curlevelConfig[0].playerWayPointPos, character.transform);
        if (sceneType == SceneType.KV || GameLibrary.dungeonId == 10000)
            CreateNPC(curlevelConfig[0], sceneCtrl.transform);
        for (int i = 0; i < curlevelConfig.Count; i++)
            CreateAirWall(curlevelConfig[i], sceneCtrl.transform);
    }

    #region CreateSceneCtrl

    public CharacterManager CreatePlayer(Vector3 pos, Transform parent)
    {
        GameObject character = CreateGO("Character", pos, 0, parent);
        CharacterManager characterManager = character.AddComponent<CharacterManager>();
        return characterManager;
    }

    protected GameObject[] CreateAutoPoint(Vector3[] pos, Transform parent)
    {
        GameObject[] auto = new GameObject[pos.Length];
        for (int i = 0; i < pos.Length; i++)
        {
            auto[i] = CreateGO(Tag.auto, pos[i], 0, parent);
            auto[i].tag = Tag.auto;
        }
        return auto;
    }

    void CreateNPC(LevelConfigNode config, Transform parent)
    {
        GameObject go = CreateGO("BullockCarts", config.npcStarPointPos, 0, parent);
        CreateAutoPoint(config.npcWayPointPos, go.transform);
    }

    void CreateAirWall(LevelConfigNode airWall, Transform parent)
    {
        GameObject go = null;
        if (airWall.airWallType == 1)
        {
            go = Resource.CreatPrefabs("AirWall", parent.gameObject, airWall.airWallPos, "Prefab/SceneCtrls/");

            if (sceneType == SceneType.KV)
            {
                go.GetComponentInChildren<ParticleSystem>().gameObject.SetActive(false);
                UnityUtil.AddComponetIfNull<BoxCollider>(go);
                if (go.GetComponent<UnityEngine.AI.NavMeshObstacle>())
                    go.GetComponent<UnityEngine.AI.NavMeshObstacle>().enabled = false;
            }
            go.transform.localRotation = Quaternion.Euler(0, airWall.airWallRot, 0);
            if (go.GetComponent<BoxCollider>())
            {
                go.GetComponent<BoxCollider>().size = airWall.airWallSize;
                go.GetComponent<BoxCollider>().isTrigger = true;
            }
            if (go.GetComponent<UnityEngine.AI.NavMeshObstacle>())
                go.GetComponent<UnityEngine.AI.NavMeshObstacle>().size = airWall.airWallSize;
            if (!Globe.isFightGuide && airWallPos == null)
                airWallPos = go.transform;
            CreatSpawnMonster(airWall.airWallID, go.transform);
            if (airWall.triggerBoss == 1)
                go.GetComponent<EffectBlock>().isTriggerBoss = true;
        }
        else
        {
            go = Resource.CreatPrefabs("BossChuChangCollider", parent.gameObject, airWall.airWallPos, "Effect/Prefabs/Monster/Boss/");
            go.transform.localScale = airWall.airWallSize;
            go.transform.localRotation = Quaternion.Euler(0, airWall.airWallRot, 0);
            BossChuChang bossCC = go.GetComponent<BossChuChang>();
            bossChuChang = bossCC;
            bossCC.bossobj = CreateBoss(airWall.airWallID, parent);
            bossCC.isWarning = true;
            if (Globe.isFightGuide && airWallPos == null)
                airWallPos = go.transform;
        }
    }
    void CreatSpawnMonster(int airWallID, Transform parent)
    {
        if (airWallID == 0) return;
        List<AirWallConfig> config = new List<AirWallConfig>();
        foreach (int id in FSDataNodeTable<AirWallConfig>.GetSingleton().DataNodeList.Keys)
        {
            if (FSDataNodeTable<AirWallConfig>.GetSingleton().DataNodeList[id].airWallID == airWallID)
            {
                config.Add(FSDataNodeTable<AirWallConfig>.GetSingleton().DataNodeList[id]);
            }
        }
        for (int i = 0; i < config.Count; i++)
        {
            CreateSpawn(config[i].monsterPos, config[i].monsterID, config[i].monsterlvl, config[i].monsterDis, parent);
        }
        if (sceneType != SceneType.KV)
            parent.GetComponent<EffectBlock>().LoadSpawnMonster();
    }

    GameObject CreateSpawn(Vector3 pos, int id, float lvl, float dis, Transform parent, string name = "Spawn", float startTime = 0f, float intervals = 0f)
    {
        GameObject go = CreateGO(name, pos, 0, parent);
        SpawnMonster spawn = go.AddComponent<SpawnMonster>();

        ResModelAndEffect(id);
        //if (!enemyModel.ContainsKey(id))
        //{
        //    GameObject model = ResModel(id);
        //    if (null != model)
        //        enemyModel.Add(id, model);
        //}

        spawn.spawnQueue = id;
        spawn.monsterLevel = (int)lvl;
        spawn.lvlRate = lvl;
        spawn.distance = dis;
        spawn.spawnTimer = startTime;
        spawn.interval = intervals;
        return go;
    }

    GameObject CreateBoss(int airWallID, Transform parent)
    {
        GameObject boss = null;
        AirWallConfig config = null;
        foreach (int id in FSDataNodeTable<AirWallConfig>.GetSingleton().DataNodeList.Keys)
        {
            if (FSDataNodeTable<AirWallConfig>.GetSingleton().DataNodeList[id].airWallID == airWallID)
            {
                config = FSDataNodeTable<AirWallConfig>.GetSingleton().DataNodeList[id];
            }
        }
        if (null != config)
        {
            boss = CreateSpawn(config.monsterPos, config.monsterID, config.monsterlvl, config.monsterDis, parent, "Boss");
            boss.tag = Tag.boss;
        }
        return boss;
    }
    GameObject CreateGO(string gameObj, Vector3 pos, float rot = 0, Transform parent = null)
    {
        GameObject go = new GameObject(gameObj);
        if (null != parent)
            go.transform.parent = parent;
        go.transform.localPosition = pos;
        go.transform.localRotation = Quaternion.Euler(0, rot, 0);
        return go;
    }

    protected List<LevelConfigBase> levelConfig = new List<LevelConfigBase>();
    protected List<LevelConfigBase> playerConfig = new List<LevelConfigBase>();
    protected List<LevelConfigBase> monsterConfig = new List<LevelConfigBase>();
    protected List<LevelConfigBase> buildingConfig = new List<LevelConfigBase>();
    protected List<LevelConfigBase> homeConfig = new List<LevelConfigBase>();

    protected GameObject CreateBlock(Transform parent)
    {
        GameObject zudang = Resource.CreatPrefabs("chushengzudang", parent.gameObject, new Vector3(-17.5f, 0.462f, -7.26f), "Prefab/Character/");
        if (null != zudang)
        {
            zudang.transform.localRotation = Quaternion.Euler(0, 180, 0);
            return zudang;
        }
        return null;
    }

    protected GameObject CreateBlock(Transform parent, Vector3 pos)
    {
        GameObject zudang = Resource.CreatPrefabs("chushengzudang", parent.gameObject, new Vector3(-17.5f, 0.462f, -7.26f), "Prefab/Character/");
        if (null != zudang)
        {
            zudang.transform.localRotation = Quaternion.Euler(0, 180, 0);
            return zudang;
        }
        return null;
    }
    protected SpawnMonster[] CreateSpawn(Transform parent)
    {
        GameObject spawn = null;
        SpawnMonster[] spawnMonster = new SpawnMonster[monsterConfig.Count];
        for (int i = 0; i < monsterConfig.Count; i++)
        {
            spawn = CreateSpawn(monsterConfig[i].modelPos, monsterConfig[i].modelID, monsterConfig[i].modellvl, 0, parent, monsterConfig[i].groupIndex == 1 ? "Spawn_Blue" : "Spawn_Red", monsterConfig[i].startTime, monsterConfig[i].intervals);
            spawnMonster[i] = spawn.GetComponent<SpawnMonster>();
            spawnMonster[i].groupIndex = (uint)monsterConfig[i].groupIndex;
            spawnMonster[i].isTP = true;
        }
        return spawnMonster;
    }

    protected SpawnMonster[] CreateSpawn(Transform parent, List<Moba3SceneConfigNode> nodeList)
    {
        GameObject spawn = null;
        SpawnMonster[] spawnMonster = new SpawnMonster[nodeList.Count];
        for (int i = 0; i < nodeList.Count; i++)
        {
            spawn = CreateSpawn(nodeList[i].modelPos, nodeList[i].modelID, nodeList[i].modellvl, 0, parent, nodeList[i].groupIndex == 1 ? "Spawn_Blue" : "Spawn_Red", nodeList[i].startTime, nodeList[i].intervals);
            spawnMonster[i] = spawn.GetComponent<SpawnMonster>();
            spawnMonster[i].groupIndex = (uint)nodeList[i].groupIndex;
            spawnMonster[i].isTP = true;
        }
        return spawnMonster;
    }

    protected SpawnMonster CreateSpawn(Transform parent, Moba3SceneConfigNode nodeList)
    {
        GameObject spawn = null;
        SpawnMonster spawnMonster = new SpawnMonster();
        spawn = CreateSpawn(nodeList.modelPos, nodeList.modelID, nodeList.modellvl, 0, parent, nodeList.groupIndex == 1 ? "Spawn_Blue" : "Spawn_Red", nodeList.startTime, nodeList.intervals);
        spawnMonster = spawn.GetComponent<SpawnMonster>();
        spawnMonster.groupIndex = (uint)nodeList.groupIndex;
        spawnMonster.isTP = true;

        return spawnMonster;
    }

    protected SpawnMonster CreateWildSpawn(Transform parent, Moba3SceneConfigNode nodeList)
    {
        GameObject spawn = null;
        SpawnMonster spawnMonster = new SpawnMonster();
        spawn = CreateSpawn(nodeList.modelPos, nodeList.modelID, nodeList.modellvl, 0, parent, "Spawn_Wild", nodeList.startTime, nodeList.intervals);
        spawnMonster = spawn.GetComponent<SpawnMonster>();
        spawnMonster.monsterCount = 5;
        spawnMonster.groupIndex = (uint)nodeList.groupIndex;
        spawnMonster.spawnAfterDie = true;
        spawnMonster.monsterAreaId = nodeList.route;
        spawnMonster.rotationY = nodeList.modelRot;
        return spawnMonster;
    }
    protected GameObject CreateEnemyPos(LevelConfigBase config, Transform parent)
    {
        if (null == config || config.modelID == 0) return null;

        return CreateGO("EnemyBornPos", config.modelPos, 0, parent);
    }
    protected GameObject CreateBuilding(LevelConfigBase config, Transform parent)
    {
        MonsterAttrNode node = FSDataNodeTable<MonsterAttrNode>.GetSingleton().FindDataByType(config.modelID);
        GameObject go = null;
        go = Resource.CreateCharacter(node, parent.gameObject, config.modelPos);
        go.transform.localRotation = Quaternion.Euler(0, config.modelRot, 0);
        return go;
    }

    protected CharacterState CreateBuilding(Moba3SceneConfigNode config, Transform parent, Vector3 pos)
    {

        GameObject go = null;
        CharacterState cs;
        if (config.type == 3)
        {
            go = Resource.CreatPrefabs(config.groupIndex == 1 ? "Tower_Blue" : "Tower_Red", parent.gameObject, config.modelPos, GameLibrary.Monster_URL);
            go.transform.localEulerAngles = new Vector3(0, config.modelRot, 0);
            cs = go.GetComponent<CharacterState>();
            cs.AddHp = go.transform.FindChild("AddHp").gameObject;
            cs.addLife = cs.AddHp.GetComponent<MobaAddLife>();
        }
        else
        {
            go = Resource.CreatPrefabs(config.groupIndex == 1 ? "Camp_Blue" : "Camp_Red", parent.gameObject, config.modelPos, GameLibrary.Monster_URL);
            cs = go.GetComponent<CharacterState>();
        }
        go.transform.localRotation = Quaternion.Euler(0, config.modelRot, 0);
        go.transform.position = pos;
        go.name = config.des;
        return cs;
    }
    protected GameObject CreateHome(LevelConfigBase config, Transform parent)
    {
        GameObject go = CreateGO(config.groupIndex == 1 ? "BlueHome" : "RedHome", config.modelPos, 0, parent);
        go.AddComponent<SphereCollider>().radius = config.modelRot;
        go.AddComponent<BaseHome>().groupIndex = config.groupIndex;
        return go;
    }

    void ResModelAndEffect(int id)
    {
        MonsterAttrNode monster = FSDataNodeTable<MonsterAttrNode>.GetSingleton().FindDataByType(id);
        if (null == monster) return;
        ModelNode model = FSDataNodeTable<ModelNode>.GetSingleton().FindDataByType(monster.model);
        if (null == model) return;

        //if (Globe.isUserPool)
        //{
        //    ObjectPool.instance.AddObjToPool(model.modelPath, model.respath);
        //    ObjectPool.instance.AddObjToPool(monster.effect_sign, GameLibrary.Effect_Shuaxin + monster.effect_sign);
        //}
        //else
        //{
        GameObject go = Resources.Load(model.respath) as GameObject;
        if (!enemyModel.ContainsKey(monster.icon_name) && null != go)
            enemyModel.Add(monster.icon_name, go);
        if (string.IsNullOrEmpty(monster.effect_sign)) return;
        GameObject effect = Resources.Load(GameLibrary.Effect_Monster + monster.effect_sign) as GameObject;
        if (!enemyModel.ContainsKey(monster.effect_sign) && null != effect)
            enemyModel.Add(monster.icon_name, effect);
        //}
    }

    #endregion

    public virtual void StartCD()
    {
        if (GameLibrary.dungeonId != 0)
        {
            // 屏蔽副本时间
            if (GameLibrary.dungeonId < 30000) return;
            SceneNode sceneNode = FSDataNodeTable<SceneNode>.GetSingleton().FindDataByType(GameLibrary.dungeonId);
            BattleCDandScore.instance.StartCD(null != sceneNode && sceneNode.time_limit != 0 ? (int)sceneNode.time_limit : 300);
            BattleCDandScore.instance.cd.OnRemove += (int count, long id) =>
            {
                WinCondition(false);
            };
        }
    }

    public virtual void InitScene()
    {

        Globe.currentDepth = 0;
        if (sceneType != SceneType.ACT_GOLD && sceneType != SceneType.ACT_EXP)// && sceneType != SceneType.ES
        {
            SpawnMonster[] spawns = GetComponentsInChildren<SpawnMonster>();
            for (int i = 0; i < spawns.Length; i++)
            {
                if (spawns[i].name != "Boss")
                {
                    if (sceneType != SceneType.KV && spawns[i].isKM)
                        spawns[i].StartSpawn();
                    spawns[i].OnCreatMonster += (GameObject go, CharacterData cd) =>
                    {
                        if (BattleUtil.IsBoss(cd))
                            bossCs = go.GetComponent<CharacterState>();
                    };
                }
            }
        }

        Debug.Log("SceneUIManager.instance.gamePrompt");
        SceneUIManager.instance.gamePrompt.StartGamePrompt(sceneType);

        InitStarConditions();

        EnterDungensTask();
    }

    public void StartSpawn()
    {
        SpawnMonster[] spawns = GetComponentsInChildren<SpawnMonster>();
        for (int i = 0; i < spawns.Length; i++)
        {
            spwanList.Add(spawns[i]);
        }
    }

    GameObject preloadParent;
    public void PreLoadCharEffects(List<CharacterState> css)
    {
        preloadParent = new GameObject();
        preloadParent.name = "PreloadEffects";
        preloadParent.transform.position = new Vector3(9999f, 9999f, 9999f);
        Dictionary<string, GameObject[]> effs = new Dictionary<string, GameObject[]>();
        for (int i = 0; i < css.Count; i++)
        {
            string sPath = css[i].emission.GetEffectResourceRoot();
            if (!effs.ContainsKey(sPath))
            {
                GameObject[] allGo = Resources.LoadAll<GameObject>(sPath);
                effs.Add(sPath, allGo);
            }
        }

        foreach(string k in effs.Keys)
        {
            for(int j = 0; j < effs[k].Length; j++)
            {
                if(j%2 == 0)
                {
                    GameObject effGo = Instantiate<GameObject>(effs[k][j]);
                    effGo.transform.parent = preloadParent.transform;
                    effGo.transform.localPosition = Vector3.zero;
                }
            }
        }

        Invoke("DetroyPreloadEffects", 3f);
    }

    void DetroyPreloadEffects ()
    {
        if(preloadParent != null)
            Destroy(preloadParent);
    }

    void InitStarConditions()
    {
        if (GameLibrary.dungeonId <= 0)
            ConditionStar(101);
        else
            ConditionStar(GameLibrary.dungeonId);

        if (GameLibrary.dungeonId % 2 == 0)
            dungeonTypes = 2;
        else
            dungeonTypes = 1;
    }

    protected CharacterState CreateMainHero(HeroData hd = null, GameObject parent = null)
    {
        if (hd == null)
            hd = CharacterManager.instance.DefaultMainHeroData();
        if (parent == null)
            parent = CharacterManager.instance.gameObject;

        CharacterState cs = CreateBattleHero(hd, parent);
        hd.useServerAttr = GameLibrary.isNetworkVersion;
        CharacterManager.instance.SetMainHero(cs);
        UnityUtil.AddComponetIfNull<SummonHero>(cs.gameObject);
        AddDirectionArrow(cs.gameObject);
        AddHeroAIBySceneType(cs).autoPoints = CharacterManager.instance.autos;
        if (sceneType != SceneType.PVP3 && sceneType != SceneType.MB1 && sceneType != SceneType.MB3 && sceneType != SceneType.MB5)
            CharacterManager.playerCS.CreatePet(MountAndPetNodeData.Instance().godefPetID);
        cs.OnDead += (c) =>
        {
            if (cs.pet != null) cs.HidePet();
            if (arrowGo != null) Destroy(arrowGo);
        };
        return cs;
    }

    GameObject arrowGo;
    void AddDirectionArrow(GameObject go)
    {
        arrowGo = Resource.CreatPrefabs("jiaobiao", go, new Vector3(0f, 0f, 0.3f), "Prefab/Character/");
        Quaternion qua = new Quaternion();
        qua.eulerAngles = new Vector3(90f, 0f, 0f);
        arrowGo.transform.localRotation = qua;
        arrowGo.transform.localScale = 0.3f * Vector3.one;
    }

    public BasePlayerAI AddHeroAIBySceneType(CharacterState cs)
    {
        BasePlayerAI ai = null;
        if (sceneType == SceneType.MB3)
            ai = UnityUtil.AddComponetIfNull<AIPve3V3>(cs.gameObject);
        else if (sceneType == SceneType.TP)
            ai = UnityUtil.AddComponetIfNull<TotalPoints_PlayerAI>(cs.gameObject);
        else if (sceneType == SceneType.TD)
            ai = UnityUtil.AddComponetIfNull<Player_TowerDefenceAI>(cs.gameObject);
        else if (sceneType == SceneType.MB1 || sceneType == SceneType.Dungeons_MB1)
            ai = UnityUtil.AddComponetIfNull<Player_MobaAI>(cs.gameObject);
        else if (sceneType == SceneType.PVP3)
            ai = UnityUtil.AddComponetIfNull<Player_ArenaAI>(cs.gameObject);
        else
            ai = UnityUtil.AddComponetIfNull<Player_AutoAI>(cs.gameObject);
        ai.enabled = cs.groupIndex != CharacterManager.playerGroupIndex;
        ai.isPatrol = (sceneType == SceneType.TD);
        ai.isAttack = (sceneType != SceneType.ES);
        return ai;
    }

    public CharacterState CreateBattleHero(CharacterData hd, GameObject parent)
    {
        //string name = hd is HeroData ? ((HeroData)hd).node.icon_name : hd.attrNode.icon_name;
        GameObject go = Resource.CreateCharacter(hd.attrNode, parent);
        if (null == go)
        {
            Debug.LogError("go is null , model : " + hd.attrNode.model);
            return null;
        }
        if (sceneType != SceneType.MB1 && sceneType != SceneType.MB3 && sceneType != SceneType.MB5)
            hd.useServerAttr = GameLibrary.isNetworkVersion;
        else
            hd.useServerAttr = false;
        CharacterState cs = BattleUtil.AddMoveComponents(go, hd.attrNode.modelNode);
        cs.pm.nav.obstacleAvoidanceType = UnityEngine.AI.ObstacleAvoidanceType.NoObstacleAvoidance;
        cs.pm.nav.avoidancePriority = 59;
        cs.pm.nav.stoppingDistance = 0.2f;
        cs.InitData(hd);
        cs.OnDead += (CharacterState mCs) => cs.CharData.buffAttrs = new float[Formula.ATTR_COUNT];
        AddCs(cs);
        return cs;
    }

    public virtual CharacterState CreateBattleMonster(MonsterData md, GameObject parent)
    {
        //md.attrNode.model
        //string modelName = Resource.GetNameByPath(md.attrNode.model);
        string modelName = md.attrNode.modelNode.modelPath;
        if (null == modelName)
        {
            Debug.LogError("Model table not found id : " + md.attrNode.model);
            return null;
        }
        //if ((GameLibrary.isMoba || sceneType == SceneType.TP) && (md.groupIndex != 99 && md.groupIndex != 100 && md.groupIndex != 101))
        //    modelName = modelName + md.groupIndex;
        //(GameLibrary.isMoba && (md.groupIndex != 99) && (md.groupIndex != 100) && (md.groupIndex != 101)) || sceneType == SceneType.TP ? md.attrNode.icon_name + md.groupIndex : md.attrNode.icon_name;
        GameObject go = null;
        //if (Globe.isUserPool)
        //{
        //    go = ObjectPool.instance.GetObj(modelName, md.attrNode.modelNode.respath);
        //}
        //else
        //{
        if (enemyModel.ContainsKey(modelName))
        {
            go = Instantiate(enemyModel[modelName]);
            go.transform.parent = parent.transform;
            go.transform.localPosition = Vector3.zero;
        }
        else
        {
            go = Resource.CreateCharacter(modelName, parent);
        }
        //}

        CharacterState mCs = null;
        if (go != null)
        {
            //go.transform.parent = parent.transform;
            //go.transform.localPosition = Vector3.zero;
            mCs = BattleUtil.AddMoveComponents(go, md.attrNode.modelNode);
            if (BattleUtil.IsBoss(md))
            {
                mCs.pm.nav.obstacleAvoidanceType = UnityEngine.AI.ObstacleAvoidanceType.NoObstacleAvoidance;
                mCs.pm.nav.avoidancePriority = 59;
            }
            else
            {
                mCs.pm.nav.obstacleAvoidanceType = UnityEngine.AI.ObstacleAvoidanceType.LowQualityObstacleAvoidance;
            }
            mCs.pm.nav.stoppingDistance = 0.2f;
            mCs.InitData(md);
            mCs.OnDead += (CharacterState cs) => RemoveCs(cs);
            if (md.state != Modestatus.Boss)
                mCs.AddHpBar();
            go.transform.position = BattleUtil.GetNearestNavPos(go);
            AddCs(mCs);
        }
        return mCs;
    }

    public virtual void OnCloseWall(int totalNum)
    {

    }
    /// <summary>
    /// 处理输赢
    /// </summary>
    /// <param name="isWin">输赢</param>
    public virtual void WinCondition(bool isWin, bool isBackMajor = false)
    {
        //副本结束直接返回
        if (isDungeonsEnd)
            return;

        if (null != BattleCDandScore.instance && BattleCDandScore.instance.cd != null && null != BattleCDandScore.instance.cd.OnCd)
            BattleCDandScore.instance.cd.OnCd = null;
        isDungeonsEnd = true;
        isFinish = true;

        this.isWin = isWin;

        StopTime();

        DisableComponents();

        if (isWin)
        {
            BossSummonMonsterDead();
        }

        GameLibrary.star = new int[3];
        if (BattleCDandScore.instance != null && BattleCDandScore.instance.cd != null)
        {
            BattleCDandScore.instance.cd.OnCd = null;
        }
        SendEndMessage(isWin, isBackMajor);
    }


    public void BossSummonMonsterDead()
    {
        for (int i = enemy.size - 1; i >= 0; i--)
        {
            if (enemy[i].GetComponent<Monster_AI>())
                enemy[i].GetComponent<Monster_AI>().StopMonsterAI();
        }

        for (int i = 0; i < spwanList.Count; i++)
            spwanList[i].enabled = false;
    }

    public virtual void SendEndMessage(bool isWin, bool isBackMajor)
    {
        if (isWin)
        {
            SceneUIManager.instance.SetMaskPanel(true);
            Time.timeScale = 0.3f;
            CommentsStar();
            if (GameLibrary.dungeonId >= 30100)
            {
                foreach (int count in killDic.Values)
                {
                    CollectionCount += count;
                }
                ClientSendDataMgr.GetSingle().GetBattleSend().SendEventFightSettlement(GameLibrary.dungeonId, 0, GameLibrary.star, 2);
            }
            else
            {

                ClientSendDataMgr.GetSingle().GetBattleSend().SendFightSettlement(GameLibrary.mapId, GameLibrary.dungeonId, dungeonTypes, GameLibrary.star, 2);

                Dictionary<int, int[]> dungeon;
                if (GameLibrary.mapOrdinary.TryGetValue(GameLibrary.mapId, out dungeon))
                {
                    dungeon = GameLibrary.mapOrdinary[GameLibrary.mapId];
                    if (dungeon.ContainsKey(GameLibrary.dungeonId))
                    {
                        if (Globe.GetStar(dungeon[GameLibrary.dungeonId]) < 3)
                            dungeon[GameLibrary.dungeonId] = GameLibrary.star;
                    }

                    MapNode node = FSDataNodeTable<MapNode>.GetSingleton().FindDataByType(GameLibrary.mapId);
                    if (dungeonTypes == 1)
                    {
                        if (dungeon.ContainsKey(GameLibrary.dungeonId + 2) && Globe.GetStar(dungeon[GameLibrary.dungeonId + 2]) < 0)
                        {
                            dungeon[GameLibrary.dungeonId + 2] = new int[3] { 0, 0, 0 };
                        }
                        if (null != node && GameLibrary.dungeonId == node.ordinary[node.ordinary.Length - 1])
                        {
                            if (dungeon.ContainsKey((int)node.elite[0]) && Globe.GetStar(dungeon[(int)node.elite[0]]) < 0)
                            {
                                dungeon[(int)node.elite[0]] = new int[] { 0, 0, 0 };
                            }
                        }
                    }
                    else
                    {
                        if (null != node)
                        {
                            int index = 0;
                            for (int i = 0; i < node.elite.Length; i++)
                            {
                                if (node.elite[i] == GameLibrary.dungeonId)
                                {
                                    index = i;
                                    break;
                                }
                            }
                            if (index + 1 < node.elite.Length && dungeon.ContainsKey((int)node.elite[index + 1]) && Globe.GetStar(dungeon[(int)node.elite[index + 1]]) < 0)
                                dungeon[(int)node.elite[index + 1]] = new int[3] { 0, 0, 0 };
                        }
                        GameLibrary.mapElite[GameLibrary.dungeonId][0]--;
                    }
                }
            }
        }
        else
        {
            if (GameLibrary.dungeonId >= 30100)
            {
                ClientSendDataMgr.GetSingle().GetBattleSend().SendEventFightSettlement(GameLibrary.dungeonId, 0, GameLibrary.star, 9);
            }
            else
            {
                ClientSendDataMgr.GetSingle().GetBattleSend().SendFightSettlement(GameLibrary.mapId, GameLibrary.dungeonId, dungeonTypes, GameLibrary.star, 9);
            }
        }
        if (GameLibrary.mapId == 100 && GameLibrary.dungeonId == 101 && playerData.GetInstance().guideData.uId == 919)//新手引导用户体验优化添加功能
            ClientSendDataMgr.GetSingle().GetGuideSend().SendGuidStep(99);
        if (!isBackMajor)
            Invoke("UIShowResult", 1f);

    }

    public virtual void UIShowResult()
    {
        Control.ShowGUI(UIPanleID.UITheBattlePanel, EnumOpenUIType.DefaultUIOrSecond, false, new object[] { false, isWin });
        SceneUIManager.instance.HideUI();
        Time.timeScale = 1f;
        SceneUIManager.instance.SetMaskPanel(false);
        AudioController.Instance.PlayBackgroundMusic(isWin ? "win" : "lose", false);
    }

    public virtual void AddCs(CharacterState cs)
    {
        if (!agents.Contains(cs))
        {
            cs.OnDead += RemoveCs;
            agents.Add(cs);
        }
        if (cs.groupIndex != CharacterManager.playerGroupIndex)
        {
            if (!enemy.Contains(cs))
            {
                enemy.Add(cs);
                cs.OnDead += (CharacterState mCs) =>
                {
                    if (cs.state == Modestatus.Tower)
                    {
                        //ClientSendDataMgr.GetSingle().GetBattleSend().SendFightSettlement();
                    }

                    if (null != mCs.CharData.attrNode)
                    {
                        if (killDic.ContainsKey(mCs.CharData.attrNode.id))
                            killDic[mCs.CharData.attrNode.id]++;
                        else
                            killDic.Add(mCs.CharData.attrNode.id, 1);
                    }
                };
            }

            if (BattleUtil.IsBoss(cs.CharData))
                cs.OnDead += (c) => isFinish = true;
        }
        else
        {
            if (!friendly.Contains(cs))
                friendly.Add(cs);
        }
        //if (cs.groupIndex==99)
        //{
        //    if (!wildMonster.Contains(cs))
        //    {
        //        cs.OnDead += RemoveCs;
        //        wildMonster.Add(cs);
        //    }
        //}
        //else if (cs.groupIndex == 100)
        //{
        //    if (!wildMonsterRed.Contains(cs))
        //    {
        //        cs.OnDead += RemoveCs;
        //        wildMonsterRed.Add(cs);
        //    }
        //}

    }

    void AddCsToList(CharacterState cs, BetterList<CharacterState> list)
    {
        if (!list.Contains(cs))
        {
            cs.OnDead += (CharacterState mcs) =>
            {
                if (list.Contains(mcs))
                    list.Remove(mcs);
            };
            list.Add(cs);
        }
    }

    public virtual void RemoveCs(CharacterState cs)
    {
        if (enemy.Contains(cs))
            enemy.Remove(cs);
        if (friendly.Contains(cs))
            friendly.Remove(cs);
        if (agents.Contains(cs))
            agents.Remove(cs);
        if (wildMonster.Contains(cs))
        {
            wildMonster.Remove(cs);
        }
        if (wildMonsterRed.Contains(cs))
        {
            wildMonsterRed.Remove(cs);
        }

    }

    protected void ClearAllCs()
    {
        enemy.Clear();
        friendly.Clear();
        agents.Clear();
    }

    bool endDisable = false;
    public virtual void DisableComponents()
    {
        if (!endDisable)
            endDisable = true;
        else
            return;
        for (int i = 0; i < agents.size; i++)
        {
            if (agents[i].pm != null)
            {
                if (agents[i].pm.nav != null && agents[i].pm.nav.isOnNavMesh)
                {
                    agents[i].pm.Stop();
                    agents[i].pm.nav.Resume();
                }
            }
            agents[i].ClearBattleData();
            agents[i].DisableScripts();
        }
        for (int i = 0; i < spwanList.Count; i++)
        {
            spwanList[i].StopCreatMonster();
        }
        SceneUIManager.instance.SwitchBloodScreen(false);
    }

    public void StopBaseAllCoroutinesAndInvok()
    {
        CancelInvoke();
        StopAllCoroutines();
    }

    public void RunTime()
    {
        //Debug.Log("RunTime");
        currntTime = GameLibrary.starTime;
        InvokeRepeating("Timing", 0, 1);
    }

    void Timing()
    {
        currntTime++;
        if (currntTime > GameLibrary.endTime)
        {
            //Debug.Log("失败");
            StopTime();
        }
    }

    void StopTime()
    {
        CancelInvoke("Timing");
    }

    public virtual void CommentsStar()
    {
        List<int> starList = new List<int>(starDic.Keys);
        for (int i = 0; i < starList.Count; i++)
        {
            starDic[i] = GetCommentsStar(starID[i]);
        }
        int index = 0;
        foreach (bool item in starDic.Values)
        {
            if (item)
                star[index] = 1;
            index++;
        }
        GameLibrary.star = star;
        GameLibrary.meetStar = starDic;
    }

    #region 评星配置

    public SceneNode sceneNode;

    int[] starID = new int[3];

    Dictionary<int, int> StarPara = new Dictionary<int, int>();
    Dictionary<int, int> beAttack = new Dictionary<int, int>();

    Dictionary<int, bool> starDic = new Dictionary<int, bool>();
    Dictionary<long, int> killMons = new Dictionary<long, int>();
    Dictionary<long, int> getGoods = new Dictionary<long, int>();
    //Dictionary<long, int> rewardDic = new Dictionary<long, int>();
    Dictionary<long, int> goodsDic = new Dictionary<long, int>();

    [HideInInspector]
    public CharacterState escortNPC; //护送的NPC

    int escortHP = 0;
    protected int playerbeAtt7 = 0;
    protected int playerbeAtt11 = 0;
    int beAttackCount7 = 0;
    //int beAttackCount11 = 0;
    int evenKills = 0;

    float time9 = 0;
    int monsterID9 = 0;
    int monsterCount9 = 0;

    int customsTime = -1;
    int[] star = new int[3];

    /// <summary>
    /// 存储评星条件
    /// </summary>
    /// <param name="sceneID">副本id</param>
    public void ConditionStar(int sceneID)
    {

        sceneNode = FSDataNodeTable<SceneNode>.GetSingleton().FindDataByType(sceneID);

        double[] star1 = sceneNode.star_parameter1;
        double[] star2 = sceneNode.star_parameter2;
        double[] star3 = sceneNode.star_parameter3;

        starDic.Add(0, false);
        starDic.Add(1, false);
        starDic.Add(2, false);

        starID[0] = (int)star1[0];
        starID[1] = (int)star2[0];
        starID[2] = (int)star3[0];

        SetParameter(star1, 1);
        SetParameter(star2, 2);
        SetParameter(star3, 3);

    }

    int starIndex = 0;

    /// <summary>
    /// 开始评星
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public bool GetCommentsStar(int id)
    {
        starIndex++;
        switch (id)
        {
            case 0:
                return Condition0();
            case 1:
                return Condition1();
            case 2:
                return Condition2();
            case 3:
                return Condition3();
            case 4:
                return Condition4();
            case 5:
                return Condition5();
            case 6:
                return Condition6();
            case 7:
                return Condition7();
            case 8:
                return Condition8();
            case 9:
                return Condition9();
            case 10:
                return Condition10();
            case 11:
                return Condition11();
            default:
                return false;
        }
    }

    void SetParameter(double[] starID, int star = 1)
    {
        switch ((int)starID[0])
        {
            case 0:
                break;
            case 1:
                customsTime = (int)starID[1];
                break;
            case 2:
                killMons.Add((long)starID[1], (int)starID[2]);
                break;
            case 3:
                break;
            case 4:
                getGoods.Add((long)starID[1], (int)starID[2]);
                break;
            case 5:
                //surplusTowerCount = (int)starID[2];
                StarPara.Add(star, (int)starID[1]);
                break;
            case 6:
                escortHP = (int)starID[1];
                break;
            case 7:
                if (starID.Length >= 2)
                    beAttackCount7 = (int)starID[1];
                else
                    beAttackCount7 = 0;
                break;
            case 8:
                evenKills = (int)starID[2];
                break;
            case 9:
                time9 = (int)starID[2];
                monsterID9 = (int)starID[3];
                monsterCount9 = (int)starID[4];
                break;
            case 10:

                break;
            case 11:
                beAttack.Add(star, (int)starID[1]);
                //beAttackCount11 = (int)starID[1];
                break;
        }
    }

    /// <summary>
    /// 0 - 通关
    /// </summary>
    /// <returns></returns>
    bool Condition0()
    {
        if (isFinish)
            return true;

        return false;
    }

    /// <summary>
    /// 1 - 指定时间内通关
    /// </summary>
    /// <returns></returns>
    bool Condition1()
    {
        if (customsTime > 0)
        {
            if ((currntTime - GameLibrary.starTime) <= customsTime)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    ///  2 - 杀死指定怪物指定个数
    /// </summary>
    /// <returns></returns>
    bool Condition2()
    {

        List<long> killKey = new List<long>(killMons.Keys);

        bool isEnough = true;

        for (int i = 0; i < killKey.Count; i++)
        {
            bool isEnoughCount = false;
            if (killDic.ContainsKey(killKey[i]))
            {
                if (killDic[killKey[i]] >= killMons[killKey[i]])
                    isEnoughCount = true;
                else
                    isEnoughCount = false;
            }
            else
            {
                isEnoughCount = false;
            }

            if (!isEnoughCount)
                isEnough = false;
        }

        if (isEnough)
            return true;

        return false;
    }

    /// <summary>
    /// 3 - 英雄不死
    /// </summary>
    /// <returns></returns>
    bool Condition3()
    {
        if (playerDeadNum <= 0)
            return true;
        else
            return false;
    }

    /// <summary>
    /// 4 - 获得指定道具指定个数
    /// </summary>
    /// <returns></returns>
    bool Condition4()
    {
        List<long> goodsKey = new List<long>();
        foreach (var item in getGoods.Keys)
        {
            goodsKey.Add(item);
        }

        bool isEnough = true;

        for (int i = 0; i < goodsKey.Count; i++)
        {
            bool isEnoughCount = false;
            if (goodsDic.ContainsKey(goodsKey[i]))
            {
                if (goodsDic[goodsKey[i]] >= getGoods[goodsKey[i]])
                    isEnoughCount = true;
                else
                    isEnoughCount = false;
            }
            else
            {
                isEnoughCount = false;
            }

            if (!isEnoughCount)
                isEnough = false;
        }

        if (isEnough)
            return true;

        return false;
    }

    /// <summary>
    /// 5 - 剩余塔数
    /// </summary>
    /// <returns></returns>
    bool Condition5()
    {
        int towerCount = 0;
        for (int i = 0; i < agents.size; i++)
        {
            if (agents[i].state == Modestatus.Tower && agents[i].groupIndex == player.groupIndex)
            {
                towerCount++;
            }
        }

        if (towerCount >= StarPara[starIndex])
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// 6 - 护送npc血量不少于百分比
    /// </summary>
    /// <returns></returns>
    bool Condition6()
    {
        if (null == escortNPC) escortNPC = player;
        if (escortNPC.currentHp >= (escortNPC.maxHp * (escortHP * 0.01f)))
            return true;
        else
            return false;
    }

    /// <summary>
    /// 7 - 英雄不被攻击（逃亡玩法)
    /// </summary>
    /// <returns></returns>
    bool Condition7()
    {
        if (playerbeAtt7 <= beAttackCount7)
            return true;
        else
            return false;
    }

    /// <summary>
    /// 8 - 完成多杀
    /// </summary>
    /// <returns></returns>
    bool Condition8()
    {

        return false;
    }

    /// <summary>
    /// 9 - 单位时间内杀怪
    /// </summary>
    /// <returns></returns>
    bool Condition9()
    {

        return false;
    }

    /// <summary>
    /// 10 - 团灭
    /// </summary>
    /// <returns></returns>
    bool Condition10()
    {

        return false;
    }

    /// <summary>
    /// 11 - Boss攻击次数
    /// </summary>
    /// <returns></returns>
    bool Condition11()
    {
        if (playerbeAtt11 <= beAttack[starIndex])
            return true;
        else
            return false;
    }

    #endregion

    public void ReadTask(int taskid = 0)
    {
        if (DataDefine.isConectSocket)
        {
            if (taskid == 0)
            {
                int taskID = TaskOperation.Single().IntoFubenGetTaskID(GameLibrary.dungeonId);
                if (taskID != 0)
                    ReadTaskData(taskID);
            }
            else
            {
                ReadTaskData(taskid);
            }
        }
        //ReadTaskData(500);
    }

    #region 任务剧情

    TaskDataNode taskNode = null;
    protected CharacterState bossCs = null;

    bool isEnterDungens = false;
    bool isBossDead = false;
    float bossBloodRata = 0f;
    float playerBloodRata = 0f;
    float temporaryBloodRata = 0f;
    CharacterState TemporaryHero = null;
    bool DungeonsEnd = false;
    Dictionary<long, GameObject> npcList = new Dictionary<long, GameObject>();

    // 关卡结束处理
    public void DungeonOverHandle(CharacterState bCs)
    {
        isFinish = true;
        if (isBossDead)
        {
            GetNpcType(4);
            for (int i = enemy.size - 1; i >= 0; i--)
            {
                if (enemy[i].GetComponent<Monster_AI>())
                    enemy[i].GetComponent<Monster_AI>().StopMonsterAI();
            }
        }
        else if (DungeonsEnd)
        {
            GetNpcType(8);
        }
        else
        {
            DungeonsDiaLogEnd();
        }
    }

    //关卡进入任务
    public void EnterDungensTask()
    {
        // 任务类型1 进入副本创建临时英雄或NPC
        if (isEnterDungens)
        {
            NPCType(taskNode.Setup1, GetTaskIndex(1));
            ReadTaskPlotLines(taskNode, 1);
            isEnterDungens = false;
        }

        TaskBossBlood();

        if (playerBloodRata != 0)
        {
            player.OnHit += PlayerBloodRata;
        }
    }

    public void TaskBossBlood()
    {
        if (null != bossCs && bossBloodRata != 0)
        {
            bossCs.OnHit += BossBloodRata;
        }
    }

    // 血量百分比触发任务
    void BossBloodRata(CharacterState bCs)
    {
        if (bCs.currentHp <= bCs.maxHp * (bossBloodRata * 0.01f))
        {
            bossCs.OnHit -= BossBloodRata;
            GetNpcType(5);
        }
    }
    void PlayerBloodRata(CharacterState bCs)
    {
        if (player.currentHp <= player.maxHp * (playerBloodRata * 0.01f))
        {
            player.OnHit -= PlayerBloodRata;
            GetNpcType(6);
        }
    }
    void TemporaryBloodRata(CharacterState bCs)
    {
        if (TemporaryHero.currentHp <= TemporaryHero.maxHp * (temporaryBloodRata * 0.01f))
        {
            TemporaryHero.OnHit -= TemporaryBloodRata;
            GetNpcType(7);
        }
    }

    // 读取任务数据
    void ReadTaskData(int taskID)
    {
        taskNode = FSDataNodeTable<TaskDataNode>.GetSingleton().FindDataByType(taskID);
        if (null == taskNode) return;
        SetTaskParameter(taskNode.Place1, 1);
        SetTaskParameter(taskNode.Place2, 2);
    }
    void SetTaskParameter(int taskPlace, int index)
    {
        if (0 == taskPlace) return;
        switch (taskPlace)
        {
            case 1: isEnterDungens = true; break;
            case 4: isBossDead = true; break;
            case 5: ParameterSet(ref bossBloodRata, index); break;
            case 6: ParameterSet(ref playerBloodRata, index); break;
            case 7: ParameterSet(ref temporaryBloodRata, index); break;
            case 8: DungeonsEnd = true; break;
        }
    }
    void ParameterSet(ref float para, int index)
    {
        if (index == 1)
            para = taskNode.Condition1;
        else
            para = taskNode.Condition2;
    }

    // 创建NPC或英雄
    void GetNpcType(int index)
    {
        if (GetTaskIndex(index) == 1)
            NPCType(taskNode.Setup1, GetTaskIndex(index));
        else
            NPCType(taskNode.Setup2, GetTaskIndex(index));
        ReadTaskPlotLines(taskNode, index);
    }
    void NPCType(int type, int index)
    {
        if (type == 1)
        {
            if (index == 1 && NPCHeroData(1))
                CreatTaskHero(taskNode.Setup1IDAndLevelDic);
            else if (index == 2 && NPCHeroData(2))
                CreatTaskHero(taskNode.Setup2IDAndLevelDic);
        }
        else if (type == 2)
        {
            if (index == 1 && taskNode.Opt3 != 0)
                CreatTaskNPC(taskNode.Opt3);
            else if (index == 2 && taskNode.Opt4 != 0)
                CreatTaskNPC(taskNode.Opt4);
        }
    }
    void CreatTaskHero(Dictionary<long, int> npcHero)
    {
        if (null == npcHero || npcHero.Count <= 0) return;
        List<long> key = new List<long>(npcHero.Keys);


        MonsterData hd = new MonsterData(key[0]);
        hd.lvl = npcHero[key[0]];
        hd.lvlRate = npcHero[key[0]];
        hd.groupIndex = player.groupIndex;
        hd.state = Modestatus.NpcPlayer;
        GameObject npc = CreateBattleHero(hd, CharacterManager.instance.gameObject).gameObject;
        npc.transform.position = player.transform.position + Vector3.forward;
        npc.transform.position = BattleUtil.GetNearestNavPos(npc);
        npc.tag = Tag.player;

        CharacterState cs = npc.GetComponent<CharacterState>();
        cs.pm.isAutoMode = true;
        cs.AddHpBar(true);
        cs.pm.nav.enabled = false;
        DungeonNPC_AI ai = UnityUtil.AddComponetIfNull<DungeonNPC_AI>(npc);
        ai.autoPoints = CharacterManager.instance.autos;
        cs.pm.nav.enabled = true;

        if (temporaryBloodRata != 0)
        {
            TemporaryHero = cs;
            cs.OnHit += TemporaryBloodRata;
        }

        //AddCs(cs);
        npcList.Add((int)cs.CharData.attrNode.id, npc);
    }
    void CreatTaskNPC(long npc)
    {
        NPCNode npcNode = FSDataNodeTable<NPCNode>.GetSingleton().FindDataByType(npc);
        if (null == npcNode)
        {
            Debug.Log("npc table not found id : " + npc);
            return;
        }
        ModelNode modelNode = FSDataNodeTable<ModelNode>.GetSingleton().FindDataByType(Convert.ToInt32(npcNode.modelid));//Convert.ToInt32(npcNode.modelid)
        if (null == modelNode)
        {
            Debug.Log("model table not found id : " + npcNode.modelid);
            return;
        }
        GameObject npcGO = Instantiate(Resources.Load(modelNode.respath)) as GameObject;//Resources.Load(modelNode.respath)
        NPCMotion npcMotion = UnityUtil.AddComponetIfNull<NPCMotion>(npcGO);
        if (isEnterDungens)
            npcMotion.SetAutoPoint(airWallPos);
        npcGO.transform.position = player.transform.position + Vector3.forward;
        npcList.Add(npc, npcGO);
    }
    bool NPCHeroData(int index)
    {
        if (index == 1)
        {
            return null != taskNode.Setup1IDAndLevelDic && taskNode.Setup1IDAndLevelDic.Count > 0;
        }
        else
        {
            return null != taskNode.Setup2IDAndLevelDic && taskNode.Setup2IDAndLevelDic.Count > 0;
        }
    }
    int GetTaskIndex(int index)
    {
        for (int i = 0; i < taskNode.PlaceArr.Length; i++)
        {
            if (taskNode.PlaceArr[i] == index)
            {
                return i + 1;
            }
        }
        return 1;
    }

    //对话
    PlotLinesNode plot = null;
    void ReadTaskPlotLines(TaskDataNode task, int place)
    {
        if (task.Lines == 0) return;

        List<PlotLinesNode> plot = new List<PlotLinesNode>();

        foreach (PlotLinesNode item in FSDataNodeTable<PlotLinesNode>.GetSingleton().DataNodeList.Values)
        {
            if (item.TaskID == task.Taskid)
            {
                plot.Add(item);
            }
        }

        if (plot.Count > 0)
        {
            for (int i = 0; i < plot.Count; i++)
            {
                if (plot[i].Place == place)
                {
                    StartCoroutine(StarDialog(plot[i].PlotID));
                    break;
                }
            }
        }
        else
        {
            DungeonsDiaLogEnd();
        }
    }
    IEnumerator StarDialog(int plotID)
    {
        plot = FSDataNodeTable<PlotLinesNode>.GetSingleton().FindDataByType(plotID);
        if (null == plot) StopAllCoroutines();

        // 1-冒泡 2-对话 3-动画
        if (plot.PlotType == 1)
        {
            SceneUIManager.instance.InsDialogBubble(SetTarget(plot), plot.Content, plot.Intervaltime);
            yield return new WaitForSeconds(plot.Intervaltime);
            if (plot.Nextplot != 0)
                StartCoroutine(StarDialog(plot.Nextplot));
            else
                StopAllCoroutines();
        }
        else if (plot.PlotType == 2)
        {
            Invoke("PlotTheDialogue", 1f);
        }
    }
    GameObject SetTarget(PlotLinesNode plot)
    {

        switch (plot.SpeakerType)
        {
            case 4:
                return player.gameObject;
            case 3:
            case 1:
                if (npcList.ContainsKey(Convert.ToInt32(plot.SpeakerID)))
                {
                    return npcList[Convert.ToInt32(plot.SpeakerID)];
                }
                return null;
            default:
                break;
        }

        return null;
    }
    void PlotTheDialogue()
    {
        StopAllCoroutines();
        if (null != BattleCDandScore.instance)
            BattleCDandScore.instance.SetTime(false);
        if (isFinish)
        {
            if (null != BattleCDandScore.instance)
            {
                BattleCDandScore.instance.cd.OnCd = null;
                BattleCDandScore.instance.cd.OnRemove = null;
            }
            DisableComponents();
            BossSummonMonsterDead();
        }
        Control.ShowGUI(UIPanleID.UIFubenTaskDialogue, EnumOpenUIType.DefaultUIOrSecond, false, plot.PlotID);
        UIFubenTaskDialogue.instance.DialogEnd = DungeonsDiaLogEnd;
        //UIFubenTaskDialogue.instance.SetData(plot.PlotID);

        if (!DungeonsEnd)
        {
            SetNPCState(false);
            Time.timeScale = 0;
        }
    }
    void DungeonsDiaLogEnd()
    {
        if (null != BattleCDandScore.instance)
            BattleCDandScore.instance.SetTime(true);
        SetNPCState(true);
        if (isFinish)
        {
            WinCondition(true);
            //  TaskOperation.Single().PassFubenToTestFubenTask(GameLibrary.dungeonId);
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    void SetNPCState(bool isEnable = false)
    {
        List<long> id = new List<long>(npcList.Keys);
        for (int i = 0; i < npcList.Count; i++)
        {
            if (npcList[id[i]].GetComponent<DungeonNPC_AI>())
                npcList[id[i]].GetComponent<DungeonNPC_AI>().enabled = isEnable;
            else if (npcList[id[i]].GetComponent<NPCMotion>())
                npcList[id[i]].GetComponent<NPCMotion>().enabled = isEnable;
        }
    }

    #endregion

}