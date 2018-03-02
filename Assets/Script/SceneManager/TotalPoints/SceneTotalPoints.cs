using UnityEngine;
using Tianyu;

public class SceneTotalPoints : SceneBaseManager
{

    public new static SceneTotalPoints instance;

    public CharacterState Tower;
    public GameObject HomeRed;
    public GameObject HomeBlue;

    SpawnMonster[] spawn;
    GameObject enemyPlayerPos = null;

    public long EnemyPlayer = 201000100;
    public float Enemylvl = 1;
    public float Towerlvl = 1;

    //MonsterData enemyhd;
    CDTimer.CD cd = null;

    [HideInInspector]
    public CharacterState enemyCs;

    float redSumRatio = 0;
    float blueSumRatio = 0;

    public override void CreateScenePrefab(GameObject sceneCtrl)
    {

        TPLevelConfigNode node = null;
        foreach (int id in FSDataNodeTable<TPLevelConfigNode>.GetSingleton().DataNodeList.Keys)
        {
            node = FSDataNodeTable<TPLevelConfigNode>.GetSingleton().DataNodeList[id];
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
        for (int i = 0; i < buildingConfig.Count; i++)
        {
            build = CreateBuilding(buildingConfig[i], sceneCtrl.transform);
            Tower = build.GetComponent<CharacterState>();
            Towerlvl = buildingConfig[i].modellvl;
            if (build.transform.GetComponentInChildren<MobaAddLife>())
                build.transform.GetComponentInChildren<MobaAddLife>().gameObject.SetActive(false);
        }

        for (int i = 0; i < playerConfig.Count; i++)
        {
            if (null != playerConfig[i] && playerConfig[i].modelID != 0)
            {
                enemyPlayerPos = CreateEnemyPos(playerConfig[i], sceneCtrl.transform);
                EnemyPlayer = playerConfig[i].modelID;
                Enemylvl = playerConfig[i].modellvl;
            }
        }

        SpawnMonster[] monster = CreateSpawn(sceneCtrl.transform);
        for (int i = 0; i < monster.Length; i++)
        {
            monster[i].autos = new GameObject[] { Tower.gameObject };
        }
    }

    public override void StartCD()
    {
        
    }

    public override void InitScene ()
    {
        sceneType = SceneType.TP;
        instance = this;
        ReadTask();
        SpawnMonster[] sm = GetComponentsInChildren<SpawnMonster>();
        for (int i = 0; i < sm.Length; i++)
        {
            spwanList.Add(sm[i]);
        }

        Tower.OnBorn += AddCallbacks;
        Tower.OnDead += (CharacterState mCs) =>
        {
            WinCondition(blueSumRatio > redSumRatio ? true : false && blueSumRatio != redSumRatio);
            SceneUIManager.instance.pnReborn.HideRevornCD();
        };
        MobaMiniMap.instance.AddMapIconByType(Tower);
        MonsterData towerData = new MonsterData(206000200, (int)Towerlvl);
        towerData.groupIndex = 2;
        towerData.lvlRate = Towerlvl;
        towerData.state = Modestatus.Tower;
        Tower.InitData(towerData);
        Tower.GetComponent<Tower_AI>().InitTowerAI();
        AddCs(Tower);

        spawn = GetComponentsInChildren<SpawnMonster>();
        for (int i = 0; i < spawn.Length; i++)
        {
            spawn[i].OnCreatMonster += OnMonsterCreated;
        }

        CreateMainHero();
        MobaMiniMap.instance.AddMapIconByType(player);
        escortNPC = player;
        player.AddHpBar();
        CreateEnemyHero();
        player.OnDead += (CharacterState mCs) => OnHeroDead(mCs);

        //SceneNode sceneNode = FSDataNodeTable<SceneNode>.GetSingleton().FindDataByType(GameLibrary.dungeonId);
        //BattleCDandScore.instance.StartCD(null != sceneNode && sceneNode.time_limit != 0 ? (int)sceneNode.time_limit : 300);
        //BattleCDandScore.instance.cd.OnRemove += (int count, long id) =>
        //{
        //    if (blueSumRatio > redSumRatio)
        //        WinCondition(true);
        //    else
        //        WinCondition(false);
        //};

        base.InitScene();
    }

    void OnHeroDead(CharacterState cs)
    {
        if (cs.state == Modestatus.Player)
        {
            // SceneUIManager.instance.playerCenterHpBar.ShowPlayerCenterHpBar(false, cs);
            SceneUIManager.instance.pnReborn.ShowRebornCD(5);
            ThirdCamera.instance._MainPlayer = null;
        }
        cd = CDTimer.GetInstance().AddCD(1f * 5, (int count, long id) => RebornHero(cs));
    }

    void RebornHero(CharacterState cs)
    {
        if (cs.groupIndex == 0)
        {
            CreateEnemyHero();
        }
        else
        {
            CreateMainHero();
            MobaMiniMap.instance.AddMapIconByType(player);
            // SceneUIManager.instance.playerCenterHpBar.ShowPlayerCenterHpBar(true, player);
            player.OnDead += (CharacterState mCs) => OnHeroDead(mCs);
            player.AddHpBar();
        }
    }

    void CreateEnemyHero ()
    {
        MonsterData md = new MonsterData(EnemyPlayer);
        //enemyhd = md;
        md.state = Modestatus.NpcPlayer;
        md.groupIndex = 0;
        md.lvl = (int)Enemylvl;
        md.lvlRate = Enemylvl;
        CharacterState enemyCs = CreateBattleHero(md, enemyPlayerPos);
        if (null == enemyCs)
            return;
        this.enemyCs = enemyCs;
        enemyCs.AddHpBar();
        AddHeroAIBySceneType(enemyCs);
        enemyCs.OnDead += (CharacterState mCs) => OnHeroDead(mCs);
        MobaMiniMap.instance.AddMapIconByType(enemyCs);
    }

    public void OnMonsterCreated(GameObject go, CharacterData cd )
    {
        go.transform.LookAt(go.GetComponentInParent<SpawnMonster>().autos[0].transform.position);
        CharacterState cs = go.GetComponent<CharacterState>();
        go.GetComponentInChildren<SkinnedMeshRenderer>().material.color = cs.groupIndex == 0 ? Color.red : Color.blue;
        MobaMiniMap.instance.AddMapIconByType(cs);
        AddCallbacks(cs);
    }

    void AttackDmg(CharacterState mCs, CharacterState attackCs, float damage)
    {
        if (mCs != Tower) return;

        if (attackCs.groupIndex == 0)
        {
            redSumRatio += damage;
        }
        else if (attackCs.groupIndex == 1)
        {
            blueSumRatio += damage;
        }

        if (blueSumRatio >= (mCs.maxHp * 0.5f))
        {
            ClearCd();
            Tower.Hp(Tower.currentHp);
            DungeonOverHandle(null);
        }
        if (redSumRatio >= (mCs.maxHp * 0.5f))
        {
            ClearCd();
            Tower.Hp(Tower.currentHp);
            WinCondition(false);
            SceneUIManager.instance.pnReborn.HideRevornCD();
        }

        SceneUIManager.instance.TowerPoint.RefreshUI((float)blueSumRatio / mCs.maxHp * 100, (float)redSumRatio / mCs.maxHp * 100);
    }

    void ClearCd()
    {
        if (null != cd)
        {
            cd.OnCd = null;
            cd = null;
        }
    }

    void AddCallbacks(CharacterState cs)
    {
        cs.OnAttackDmg += AttackDmg;
        MobaMiniMap.instance.AddMapIconByType(cs);
    }
}
