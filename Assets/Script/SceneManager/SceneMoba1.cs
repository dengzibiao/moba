using UnityEngine;
using Tianyu;
using System.Collections.Generic;
using System.Collections;
using System;

public enum MobaAIPlayerPriority
{
    AutoNav = 0,
    Patrol = 1,
    Defense = 1,
    Monster = 2,
    Attack = 3,
    Protect = 4,
    FriendHeroLow = 5,
    CampLow = 6,
    TowerLow = 7,
    DefenseCamp = 8,
    DefenseTower = 9,
    FriendHeroHigh = 10,
    CampHigh = 11,
    TowerHigh = 12,
    BackHome = 14,
}

public enum MobaObjectID
{
    None = 0,
    HeroBingnv = 101,
    HeroJumo = 102,
    HeroShengqi = 103,
    HeroShawang = 104,
    HeroXiaoxiao = 105,
    HeroKulouwang = 106,
    HeroShangjinlieren = 107,
    HeroTongkunvwang = 108,
    HeroXiaohei = 109,
    HeroXiongmao = 110,
    HeroJiansheng = 111,
    HeroShuiren = 112,
    HeroDifa = 113,
    HeroHuonv = 114,
    HeroYingci = 115,
    HeroChenmo = 116,
    HeroXiaolu = 117,
    HeroShenniu = 118,
    HeroMeidusha = 119,
    HeroBaihu = 120,
    HeroShenling = 121,
    HeroMori = 122,
    HeroHuanci = 130,
    HeroLuosa = 133
}

public class SceneMoba1 : SceneMobaBase
{
    public static new SceneMoba1 instance;

    public const int BATTLE_DURATION = 600;

    public CharacterState TowerRed;
    public CharacterState TowerBlue;
    public CharacterState CampRed;
    public CharacterState CampBlue;
    public GameObject HomeRed;
    public GameObject HomeBlue;
    public GameObject Block;
    public GameObject enemyBornPos;

    [HideInInspector]
    public CharacterState HeroRed;
    [HideInInspector]
    public CharacterState HeroBlue;

    SpawnMonster[] SpawnMonsters;
    int rebornTime = 12;
    int ScoreRed;
    int ScoreBlue;
    public HeroData HeroDataBlue;
    public CharacterData HeroDataRed;
    CDTimer.CD rebornCD;
    CDTimer.CD timeOverCD;

    long[] FubenHero = new long[4];
    float Fubenlvl = 1;
    List<LevelConfigBase> dungeonTower;
    List<LevelConfigBase> dungeonCamp;

    /// <summary>
    /// 创建场景预制体
    /// </summary>
    /// <param name="sceneCtrl"></param>
    public override void CreateScenePrefab(GameObject sceneCtrl)
    {
        if (null == sceneCtrl) return;
        Globe.isFB = true;
        MobaLevelConfigNode node = null;
        foreach (int id in FSDataNodeTable<MobaLevelConfigNode>.GetSingleton().DataNodeList.Keys)
        {
            node = FSDataNodeTable<MobaLevelConfigNode>.GetSingleton().DataNodeList[id];
            if (node.sceneid == GameLibrary.dungeonId)
            {
                levelConfig.Add(node);
                if (node.type == 1)
                    playerConfig.Add(node);
                else if (node.type == 2)
                    monsterConfig.Add(node);
                else if (node.type == 3 || node.type == 4)
                    buildingConfig.Add(node);
                else if (node.type == 5)
                    homeConfig.Add(node);
            }
        }

        if (levelConfig.Count <= 0)
        {
            Debug.LogError("levelConfig table did not find id " + GameLibrary.dungeonId);
            return;
        }

        CreatePlayer(levelConfig[0].modelPos, sceneCtrl.transform);

        for (int i = 0; i < playerConfig.Count; i++)
        {
            if (null != playerConfig[i] && playerConfig[i].modelID != 0)
            {
                enemyBornPos = CreateEnemyPos(playerConfig[i], sceneCtrl.transform);
                FubenHero[0] = playerConfig[i].modelID;
                Fubenlvl = playerConfig[i].modellvl;
            }
        }

        GameObject home = null;
        for (int i = 0; i < homeConfig.Count; i++)
        {
            home = CreateHome(homeConfig[i], sceneCtrl.transform);
            if (homeConfig[i].groupIndex == 1)
                HomeBlue = home;
            else
                HomeRed = home;
        }

        GameObject build = null;
        dungeonTower = new List<LevelConfigBase>();
        dungeonCamp = new List<LevelConfigBase>();
        for (int i = 0; i < buildingConfig.Count; i++)
        {
            build = CreateBuilding(buildingConfig[i], sceneCtrl.transform);
            if (buildingConfig[i].groupIndex == 1)
            {
                if (buildingConfig[i].type == 3)
                    TowerBlue = build.GetComponent<CharacterState>();
                else
                    CampBlue = build.GetComponent<CharacterState>();
            }
            else
            {
                if (buildingConfig[i].type == 3)
                    TowerRed = build.GetComponent<CharacterState>();
                else
                    CampRed = build.GetComponent<CharacterState>();
            }
            if (buildingConfig[i].type == 3)
                dungeonTower.Add(buildingConfig[i]);
            else
                dungeonCamp.Add(buildingConfig[i]);
        }

        SpawnMonster[] monster = CreateSpawn(sceneCtrl.transform);
        for (int i = 0; i < monster.Length; i++)
        {
            monster[i].autos = new GameObject[] { monster[i].groupIndex == 1 ? CampRed.gameObject : CampBlue.gameObject };
        }

        Block = CreateBlock(sceneCtrl.transform);
    }

    public override void StartCD()
    {
        if (sceneType == SceneType.Dungeons_MB1)
            base.StartCD();
    }

    public override void InitScene()
    {
        instance = this;

        isDungeons = Globe.isFB;//不是副本
        GameLibrary.isMoba = true;//moba
        if (!isDungeons)
            sceneType = SceneType.MB1;//moba类型场景

        InitHeroDatas();

        HeroReborn(HeroDataBlue);
        HeroReborn(HeroDataRed);
        if (HeroRed != null)
            HeroRed.GetComponent<BasePlayerAI>().enabled = false;

        InitHpAndAttackRatio();

        if (!isDungeons)
        {
            InitTower(TowerBlue, (uint)212000101, 1, Modestatus.Tower);
            InitTower(TowerRed, (uint)212000102, 0, Modestatus.Tower);
            InitTower(CampBlue, (uint)211000101, 1, Modestatus.Tower);
            InitTower(CampRed, (uint)211000102, 0, Modestatus.Tower);
        }
        else
        {
            InitTower(TowerBlue, Modestatus.Tower, dungeonTower[0].groupIndex == 1 ? dungeonTower[0] : dungeonTower[1]);
            InitTower(TowerRed, Modestatus.Tower, dungeonTower[0].groupIndex == 0 ? dungeonTower[0] : dungeonTower[1]);
            InitTower(CampBlue, Modestatus.Tower, dungeonCamp[0].groupIndex == 1 ? dungeonCamp[0] : dungeonCamp[1]);
            InitTower(CampRed, Modestatus.Tower, dungeonCamp[0].groupIndex == 0 ? dungeonCamp[0] : dungeonCamp[1]);
        }

        CampRed.OnDead += (CharacterState cs) => ShowResult(true);
        CampBlue.OnDead += (CharacterState cs) => ShowResult(false);
        CampRed.Invincible = true;
        CampBlue.Invincible = true;
        TowerBlue.OnDead += (CharacterState cs) => CampBlue.Invincible = false;
        TowerRed.OnDead += (CharacterState cs) => CampRed.Invincible = false;

        SpawnMonsters = GetComponentsInChildren<SpawnMonster>();
        for (int i = 0; i < SpawnMonsters.Length; i++)
        {
            SpawnMonsters[i].OnCreatMonster += OnMonsterCreated;
        }
        RefreshInfo();


        CDTimer.GetInstance().AddCD(5, (int count, long id) =>
        {
            if (HeroRed != null)
                HeroRed.GetComponent<BasePlayerAI>().enabled = true;
            Block.SetActive(false);

            if (FightTouch._instance.overBtn != null)
                FightTouch._instance.overBtn.onClick = () => ShowResult(true);
        });

        CDTimer.GetInstance().AddCD(20, (int count, long id) => rebornTime++, 30);

        //if (!isDungeons)
        //{
        //    //CDTimer.CD timeOverCD = CDTimer.GetInstance().AddCD(BATTLE_DURATION, (int count, long id) =>
        //    //{
        //    //    ShowResult(ScoreRed < ScoreBlue);
        //    //    Debug.Log("Result " + count);
        //    //});
        //    //BattleCDandScore.instance.StartCD(BATTLE_DURATION);
        //}
        //else
        //{
        //    BattleCDandScore.instance.gameObject.SetActive(false);
        //}

        TowerRed.priority = MobaAIPlayerPriority.TowerLow;
        CampRed.priority = MobaAIPlayerPriority.CampLow;

        spwanList.AddRange(GetComponentsInChildren<SpawnMonster>());

        if (FightTouch._instance != null)
        {
            FightTouch._instance.TpPosition = HomeBlue.transform.position;
            FightTouch._instance.tpEffect1 = Resource.CreatPrefabs("HuiCheng_01", gameObject, Vector3.one, "Effect/Prefabs/Item/");
            FightTouch._instance.tpEffect2 = Resource.CreatPrefabs("HuiCheng_02", gameObject, Vector3.one, "Effect/Prefabs/Item/");
        }
        base.InitScene();
    }
    /// <summary>
    /// 初始化 英雄数据
    /// </summary>
    void InitHeroDatas()
    {
        int level = playerData.GetInstance().selfData.level;
        MobaRobotNode robotCfg = FSDataNodeTable<MobaRobotNode>.GetSingleton().FindDataByType(level);
        if (!isDungeons && robotCfg != null)
        {
            List<long> randIds = null;
            HeroData enemyHeroData = null;
            if (!GameLibrary.isNetworkVersion)
            {
# if UNITY_EDITOR
                randIds = BattleUtil.GetRandomTeam(3, new List<long>() { GameLibrary.player });
                Globe.mobaMyTeam = new HeroData[] { new HeroData(GameLibrary.player), new HeroData(randIds[0]), new HeroData(randIds[1]), new HeroData(randIds[2]) };
                Globe.mobaMyTeam[0].AddFakeEquips(robotCfg.equipment_lv, robotCfg.equipment_grade);
                enemyHeroData = new HeroData(GameLibrary.emeny);
                enemyHeroData.AddFakeEquips(robotCfg.equipment_lv, robotCfg.equipment_grade);
#endif
            }
            else
            {
                randIds = BattleUtil.GetRandomTeam(1, new List<long>() { Globe.mobaMyTeam[0].id });
                enemyHeroData = new HeroData(randIds[0], robotCfg.hero_lv, robotCfg.hero_grade, robotCfg.hero_star);
                enemyHeroData.AddFakeEquips(robotCfg.equipment_lv, robotCfg.equipment_grade);
            }
            Globe.mobaEnemyTeam = new HeroData[] { enemyHeroData, null, null, null };
        }

        if (isDungeons)
        {
            ReadTask();
            for (int i = 0; i < Globe.mobaMyTeam.Length; i++)
            {
                Globe.mobaMyTeam[i] = Globe.playHeroList[i];
            }
            Globe.mobaEnemyTeam = new MonsterData[] { new MonsterData(FubenHero[0]), new MonsterData(FubenHero[1]), new MonsterData(FubenHero[2]), new MonsterData(FubenHero[3]) };
            for (int i = 0; i < Globe.mobaEnemyTeam.Length; i++)
            {
                if (null != Globe.mobaEnemyTeam[i] && Globe.mobaEnemyTeam[i].id != 0)
                {
                    Globe.mobaEnemyTeam[i].lvl = (int)Fubenlvl;
                    ((MonsterData)Globe.mobaEnemyTeam[i]).lvlRate = Fubenlvl;
                }
            }
            playerData.GetInstance().AddHeroToList(GameLibrary.player);
            Globe.fightHero[0] = (int)GameLibrary.player;
            HeroDataBlue = Globe.playHeroList[0];
        }
        else
        {
            HeroDataBlue = Globe.mobaMyTeam[0];
        }
        if (HeroDataBlue != null)
        {
            HeroDataBlue.groupIndex = 1;
            HeroDataBlue.state = Modestatus.Player;
            HeroDataBlue.fakeMobaPlayerName = playerData.GetInstance().selfData.playeName;
        }
        //if (HeroRed != null) {
        HeroDataRed = Globe.mobaEnemyTeam[0];
        HeroDataRed.groupIndex = 0;
        HeroDataRed.state = Modestatus.NpcPlayer;
        HeroDataRed.fakeMobaPlayerName = UICreateRole.GetRandomName();
        //}
    }

    void HeroReborn(CharacterData hd)
    {
        if (hd == null)
            return;
        if (hd.groupIndex == 1 && hd is HeroData)
        {
            HeroData heroDt = (HeroData)hd;
            heroDt.useServerAttr = GameLibrary.isNetworkVersion;
            heroDt.RefreshAttr();
            CreateMainHero(heroDt);
            HeroBlue = CharacterManager.playerCS;
        }
        else
        {
            if (sceneType == SceneType.MB1 && hd is HeroData)
            {
                HeroData heroDt = (HeroData)hd;
                heroDt.useServerAttr = false;
                heroDt.RefreshAttr();
            }
            HeroRed = CreateBattleHero(hd, enemyBornPos);
            HeroRed.pm.isAutoMode = true;
        }
        CharacterState cs = (hd.groupIndex == 0) ? HeroRed : HeroBlue;
        AddHeroAIBySceneType(cs);
        cs.AddHpBar(true);
        AddSaveAttrsAddRefreshHpBar(cs, cs.CharData.mobaMorale);
        cs.OnDamageBy += (CharacterState mcs, CharacterState attackerCs) =>
        {
            if (BattleUtil.IsHeroTarget(attackerCs))
            {
                RefreshAttackedHeroRecords(mcs.CharData, attackerCs.CharData);
            }
            else if (attackerCs.state == Modestatus.SummonHero && attackerCs.Master != null)
            {
                RefreshAttackedHeroRecords(mcs.CharData, attackerCs.Master.CharData);
            }
        };
        cs.OnHelp += TowerHelp;
        cs.OnDead += OnHeroDead;
        cs.OnDead += ChangeMorale;
        cs.moveSpeed = cs.moveInitSpeed = 0.75f * cs.moveInitSpeed;
        MobaMiniMap.instance.AddMapIconByType(cs);
    }

    void OnHeroDead(CharacterState cs)
    {
        HandleKillInfo(cs);
        if (cs.groupIndex == 0)
            ScoreBlue++;
        else
            ScoreRed++;
        RefreshInfo();
        if (cs.state == Modestatus.Player)
        {
            // SceneUIManager.instance.playerCenterHpBar.ShowPlayerCenterHpBar(false, cs);
            SceneUIManager.instance.pnReborn.ShowRebornCD(rebornTime);
            ThirdCamera.instance._MainPlayer = null;
        }

        rebornCD = CDTimer.GetInstance().AddCD(1f * rebornTime, (int count, long id) => HeroReborn((cs.groupIndex == 1) ? HeroDataBlue : HeroDataRed));
    }

    List<HeroData> HeroDatas = new List<HeroData>();
    HeroData GetHeroData(int groupIndex, int memberIndex)
    {
        for (int i = 0; i < HeroDatas.Count; i++)
        {
            if (HeroDatas[i].groupIndex == groupIndex && HeroDatas[i].memberId == memberIndex)
            {
                return HeroDatas[i];
            }
        }
        return null;
    }

    void OnMonsterCreated(GameObject go, CharacterData cd)
    {
        CharacterState cs = go.GetComponent<CharacterState>();
        InitMonsterHpAndAttack(cs);
        MobaMiniMap.instance.AddMapIconByType(cs);
        AddCs(cs);
        cs.OnDead += ChangeMorale;
    }

    protected override void OnSceneDestroy()
    {
        base.OnSceneDestroy();
        if (isDungeons && null != HeroDataBlue)
        {
            HeroDataBlue.mobaKillCount = 0;
            HeroDataBlue.mobaAidCount = 0;
            HeroDataBlue.mobaDeathCount = 0;
        }
        ClearCsData(Globe.mobaMyTeam);
        GameLibrary.isMoba = false;
        // Globe.UseServerBattleAttrs = false;
    }

    bool win = false;
    void ShowResult(bool isWin)
    {
        win = isWin;
        isFinish = true;
        ThirdCamera.instance._MainPlayer = isWin ? CampRed.transform : CampBlue.transform;
        // SceneUIManager.instance.playerCenterHpBar.gameObject.SetActive(false);
        // AngerPoint._instance.gameObject.SetActive(false);

        //for(int i = 0; i< SpawnMonsters.Length; i++)
        //{
        //    SpawnMonsters[i].gameObject.SetActive(false);
        //}

        if (null != rebornCD)
        {
            rebornCD.OnCd = null;
            rebornCD = null;
        }
        if (null != timeOverCD)
        {
            timeOverCD.OnCd = null;
            timeOverCD = null;
        }

        OverResult();
        
        if (isDungeons)
        {


            if (win)
                DungeonOverHandle(null);
            else
                WinCondition(win);
        }
        else
        {
            DisableComponents();
            Invoke("ShowResultPanel", 3f);
        }
    }

    void ShowResultPanel()
    {
        Control.ShowGUI(UIPanleID.UITheBattlePanel, EnumOpenUIType.DefaultUIOrSecond, false, new object[] { true, win });
        AudioController.Instance.PlayBackgroundMusic(win ? "win" : "lose", false);
    }

    void GetDungeonInfo()
    {

    }

    protected override void RefreshInfo()
    {
        if (HeroDataBlue != null && HeroDataRed != null)
        {
            //BattleCDandScore.instance.LaVS.text = ScoreBlue + "/" + HeroDataBlue.mobaMorale + "% VS " + ScoreRed + "/" + HeroDataRed.mobaMorale + "%";
            SceneUIManager.instance.fightTouch.mobaStatic.Refresh(HeroDataBlue, ScoreBlue, ScoreRed);
        }
    }
}
