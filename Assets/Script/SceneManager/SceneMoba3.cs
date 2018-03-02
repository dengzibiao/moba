using UnityEngine;
using System.Collections.Generic;
using Tianyu;

public class SceneMoba3 : SceneMobaBase
{
    public static new SceneMoba3 instance;
    public CharacterState CampRed;
    public CharacterState CampBlue;
    public GameObject HomeRed;
    public GameObject HomeBlue;
    public GameObject Block;
    public List<CharacterState> towersRedLeft;
    public List<CharacterState> towersRedRight;
    public List<CharacterState> towersBlueLeft;
    public List<CharacterState> towersBlueRight;
    int RebornCD = 12;
    int ScoreRed;
    int ScoreBlue;
    public Transform[] matesPos;
    SpawnMonster[] SpawnMonsters;
    public List<CharacterState> herosRed = new List<CharacterState>();
    public List<CharacterState> herosBlue = new List<CharacterState>();
    public List<HeroData> herosDataRed = new List<HeroData>();
    public List<HeroData> herosDataBlue = new List<HeroData>();
    public const int BATTLE_DURATION = 600;
    public AiLevel aiLv = AiLevel.Low;
    List<Moba3SceneConfigNode> heroCfg = new List<Moba3SceneConfigNode>();
    List<Moba3SceneConfigNode> monsterCfg = new List<Moba3SceneConfigNode>();
    List<Moba3SceneConfigNode> towersCfg = new List<Moba3SceneConfigNode>();
    List<Moba3SceneConfigNode> campCfg = new List<Moba3SceneConfigNode>();
    List<Moba3SceneConfigNode> homeCfg = new List<Moba3SceneConfigNode>();
    public List<Moba3SceneConfigNode> wildMonsterCfg = new List<Moba3SceneConfigNode>();
    public List<Moba3SceneConfigNode> wildMonsterCfgRed = new List<Moba3SceneConfigNode>();
    public List<Moba3SceneConfigNode> wildBossCfg= new List<Moba3SceneConfigNode>();
    public BetterList<CharacterState> wildMonster1 = new BetterList<CharacterState>();       
    public BetterList<CharacterState> wildMonster2 = new BetterList<CharacterState>();
    public BetterList<CharacterState> wildMonster3 = new BetterList<CharacterState>();
    public BetterList<CharacterState> wildRed1 = new BetterList<CharacterState>();
    public BetterList<CharacterState> wildRed2 = new BetterList<CharacterState>();
    public BetterList<CharacterState> wildRed3 = new BetterList<CharacterState>();
    Moba3SceneConfigNode block = new Moba3SceneConfigNode();
    Dictionary<long, Moba3SceneConfigNode> cfg;
    HeroRebornThumb[] rebornThumbs;

    public override void InitScene()
    {
        instance = this;
        GameLibrary.isMoba = true;
        sceneType = SceneType.MB3;
        Debug.Log("SceneUIManager.instance");
        if (SceneUIManager.instance != null)
        {
            Debug.Log("SceneUIManager.instance.heroRebornThumb");
            rebornThumbs = SceneUIManager.instance.heroRebornThumb.GetComponentsInChildren<HeroRebornThumb>();
        }

        int level = playerData.GetInstance().selfData.level;
        MobaRobotNode robotCfg = FSDataNodeTable<MobaRobotNode>.GetSingleton().DataNodeList[level];

        if (robotCfg!=null)
        {
            List<long> randIds = null;
            if(!GameLibrary.isNetworkVersion)
            {
                long mainId = BattleUtil.GetRandomTeam(1)[0];
                randIds = BattleUtil.GetRandomTeam(8, new List<long>() { mainId });
                HeroData myLocalData = new HeroData(mainId);
                myLocalData.AddFakeEquips(robotCfg.equipment_lv, robotCfg.equipment_grade);
                Globe.moba3v3MyTeam1 = new HeroData[] { myLocalData, new HeroData(randIds[5]), new HeroData(randIds[6]), new HeroData(randIds[7]) };
            }
            else
            {
                randIds = BattleUtil.GetRandomTeam(5, new List<long>() { Globe.moba3v3MyTeam1[0].id});
            }

            HeroData myTeammate2 = new HeroData(randIds[0], robotCfg.hero_lv, robotCfg.hero_grade, robotCfg.hero_star);
            myTeammate2.AddFakeEquips(robotCfg.equipment_lv, robotCfg.equipment_grade);
            Globe.moba3v3MyTeam2 = new HeroData[] { myTeammate2, null, null, null };
            HeroData myTeammate3 = new HeroData(randIds[1], robotCfg.hero_lv, robotCfg.hero_grade, robotCfg.hero_star);
            myTeammate3.AddFakeEquips(robotCfg.equipment_lv, robotCfg.equipment_grade);
            Globe.moba3v3MyTeam3 = new HeroData[] { myTeammate3, null, null, null };
            HeroData enemyTeamm1 = new HeroData(randIds[2],  robotCfg.hero_lv, robotCfg.hero_grade, robotCfg.hero_star);
            enemyTeamm1.AddFakeEquips(robotCfg.equipment_lv, robotCfg.equipment_grade);
            Globe.moba3v3EnemyTeam1 = new HeroData[] { enemyTeamm1, null, null, null };
            HeroData enemyTeamm2 = new HeroData(randIds[3], robotCfg.hero_lv, robotCfg.hero_grade, robotCfg.hero_star);
            enemyTeamm2.AddFakeEquips(robotCfg.equipment_lv, robotCfg.equipment_grade);
            Globe.moba3v3EnemyTeam2 = new HeroData[] { enemyTeamm2, null, null, null };
            HeroData enemyTeamm3 = new HeroData(randIds[4], robotCfg.hero_lv, robotCfg.hero_grade, robotCfg.hero_star);
            enemyTeamm3.AddFakeEquips(robotCfg.equipment_lv, robotCfg.equipment_grade);
            Globe.moba3v3EnemyTeam3 = new HeroData[] { enemyTeamm3, null, null, null };
        }
       
        Moba3SceneConfigNode node = null;
        #region Scene build 
        var naviCfg = FSDataNodeTable<Moba3v3NaviNode>.GetSingleton().DataNodeList;
        if (naviCfg != null)
        {
            for (int i = 0; i < blueLeft.Length; i++)
            {
                blueLeft[i].transform.position = GetNaviById(i + 1, naviCfg, 1);
                blueRight[i].transform.position = GetNaviById(i + 1, naviCfg, 2);
                redLeft[i].transform.position = GetNaviById(i + 1, naviCfg, 3);
                RedRight[i].transform.position = GetNaviById(i + 1, naviCfg, 4);
            }
        }
        cfg = FSDataNodeTable<Moba3SceneConfigNode>.GetSingleton().DataNodeList;
        if (cfg != null)
        {
            foreach (var id in cfg.Keys)
            {
                node = cfg[id];
                switch (node.type)
                {
                    case 0:
                        block = node;
                        break;
                    case 1:
                        heroCfg.Add(node);
                        break;
                    case 2:
                        monsterCfg.Add(node);
                        break;
                    case 3:
                        towersCfg.Add(node);
                        break;
                    case 4:
                        campCfg.Add(node);
                        break;
                    case 5:
                        homeCfg.Add(node);
                        break;
                    case 6:
                        wildMonsterCfg.Add(node);
                        break;
                    case 7:
                        wildMonsterCfgRed.Add(node);
                        break;
                    case 8:
                        wildBossCfg.Add(node);
                        break;
                    default:
                        break;
                }
            }

        }

        BuildScene();
        #endregion
        InitHeroData(sceneType);
        CreatAllHeros(sceneType);
        InitHpAndAttackRatio();
        RefreshInfo();

        InitAllTowers();
        SetPriority();
        CampRed.OnDead += (CharacterState cs) => ShowResult(true);
        CampBlue.OnDead += (CharacterState cs) => ShowResult(false);

        SpawnMonsters = GetComponentsInChildren<SpawnMonster>();
        for (int i = 0; i < SpawnMonsters.Length; i++)
        {
            SpawnMonsters[i].OnCreatMonster += OnMonsterCreated;
        }

        List<CharacterState> allHeros = new List<CharacterState>();
        allHeros.AddRange(herosBlue);
        allHeros.AddRange(herosRed);
        allHeros.ForEach((CharacterState c) => AddHeroAIs(c));
        InitHeroRebornThumbs(allHeros);
        ReloadAllHeroSounds(allHeros);
        CDTimer.GetInstance().AddCD(5, (int count, long id) => Block.SetActive(false));
        CDTimer.GetInstance().AddCD(30, (int count, long id) => RebornCD++, 10);
        CampRed.priority = MobaAIPlayerPriority.CampLow;

        if (FightTouch._instance != null)
        {
            FightTouch._instance.TpPosition = HomeBlue.transform.position;
            FightTouch._instance.tpEffect1 = Resource.CreatPrefabs("HuiCheng_01", gameObject, Vector3.one, "Effect/Prefabs/Item/");
            FightTouch._instance.tpEffect2 = Resource.CreatPrefabs("HuiCheng_02", gameObject, Vector3.one, "Effect/Prefabs/Item/");
        }
        Debug.Log("base.InitScene");
        base.InitScene();

    }

    public override void ChangeMorale(CharacterState cs)
    {
        
        if (cs.state== Modestatus.Monster)
        {
            if (cs.groupIndex == 101)
            {
                AddMorale(cs, 10);

            }
            else
                AddMorale(cs,1);
        }
        else if (cs.state== Modestatus.Tower)
        {
            AddMorale(cs,10);
        }
    }

    private void ReloadAllHeroSounds(List<CharacterState> mAllHeros)
    {
        for (int i = 0; i < mAllHeros.Count; i++)
        {
            CharacterState cs = mAllHeros[i];
            if (cs == null) continue;
            AudioController.Instance.CreateAudioClipPool(cs);
        }
    }

    void InitHeroRebornThumbs ( List<CharacterState> thumbs)
    {
        for(int i = 0; i < thumbs.Count; i++)
        {
            int pos = GetPos(thumbs[i]);
            rebornThumbs[pos].SpHeadIcon.spriteName = thumbs[i].CharData.attrNode.icon_name + "_head";
            rebornThumbs[pos].LaRebornCD.text = "";
            if(thumbs[i].groupIndex != CharacterManager.playerGroupIndex)
                rebornThumbs[pos].gameObject.SetActive(false);
        }
    }

    int GetPos ( CharacterState cs)
    {
        return cs.groupIndex == 0 ? 2 * cs.CharData.memberId + 1 : 2 * cs.CharData.memberId;
    }

    enum NaviType
    {
        blueLeft = 0,
        blueRight = 1,
        redLeft = 2,
        redRight = 3,
        none = 255,
    }

    void SetPriority()
    {
        CampBlue.Invincible = true;
        CampRed.Invincible = true;
        towersBlueLeft[1].Invincible = true;
        towersBlueRight[1].Invincible = true;
        towersRedLeft[1].Invincible = true;
        towersRedRight[1].Invincible = true;
        towersBlueRight[0].OnDead += (CharacterState cs) => towersBlueRight[1].Invincible = false;
        towersBlueLeft[0].OnDead += (CharacterState cs) => towersBlueLeft[1].Invincible = false;
        towersRedLeft[0].OnDead += (CharacterState cs) => towersRedLeft[1].Invincible = false;
        towersRedRight[0].OnDead += (CharacterState cs) => towersRedRight[1].Invincible = false;
        towersBlueLeft[1].OnDead+= (CharacterState cs) => CampBlue.Invincible = false;
        towersBlueRight[1].OnDead += (CharacterState cs) => CampBlue.Invincible = false;
        towersRedRight[1].OnDead += (CharacterState cs) => CampRed.Invincible = false;
        towersRedLeft[1].OnDead += (CharacterState cs) => CampRed.Invincible = false;
    }

    //public override CharacterState CreateBattleMonster(MonsterData md, GameObject parent)
    //{
    //    string modelName = (GameLibrary.isMoba && (md.groupIndex != 99) && (md.groupIndex != 100) && (md.groupIndex != 101)) || sceneType == SceneType.TP ? md.attrNode.icon_name + md.groupIndex : md.attrNode.icon_name;
    //    GameObject go = null;
    //    if (enemyModel.ContainsKey(modelName))
    //    {
    //        go = Instantiate(enemyModel[modelName]);
    //        go.transform.parent = parent.transform;
    //        go.transform.localPosition = Vector3.zero;
    //    }
    //    else
    //    {
    //        go = Resource.CreateCharacter(modelName, parent);
    //    }

    //    CharacterState mCs = null;
    //    if (go != null)
    //    {
    //        mCs = BattleUtil.AddMoveComponents(go);
    //        if (BattleUtil.IsBoss(md))
    //        {
    //            mCs.pm.nav.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
    //            mCs.pm.nav.avoidancePriority = 59;
    //        }
    //        else
    //        {
    //            mCs.pm.nav.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;
    //        }
    //        mCs.pm.nav.stoppingDistance = 0.2f;
    //        mCs.InitData(md);
    //        mCs.OnDead += (CharacterState cs) => RemoveCs(cs);
    //        if (md.state != Modestatus.Boss)
    //            mCs.AddHpBar();
    //        go.transform.position = BattleUtil.GetNearestNavPos(go);
    //        mCs.CharData.monsterAreaId = md.monsterAreaId;
    //        AddCs(mCs);
            
    //    }
    //    return mCs;
    //}

    public override void AddCs(CharacterState cs)
    {
        base.AddCs(cs);
        if (cs.groupIndex == 99)
        {
            if (!wildMonster.Contains(cs))
            {
                cs.OnDead += RemoveCs;
                wildMonster.Add(cs);
               
            }
            if (!wildMonster1.Contains(cs) && cs.CharData.monsterAreaId == 1)
            {
                cs.OnDead += RemoveCs;
                wildMonster1.Add(cs);
            }
            if (!wildMonster2.Contains(cs) && cs.CharData.monsterAreaId == 2)
            {
                cs.OnDead += RemoveCs;
                wildMonster2.Add(cs);
            }
            if (!wildMonster3.Contains(cs) && cs.CharData.monsterAreaId == 3)
            {
                cs.OnDead += RemoveCs;
                wildMonster3.Add(cs);
            }
        }
        else if (cs.groupIndex == 100)
        {
            if (!wildMonsterRed.Contains(cs))
            {
                cs.OnDead += RemoveCs;
                wildMonsterRed.Add(cs);                
            }
            if (!wildRed1.Contains(cs) && cs.CharData.monsterAreaId == 1)
            {
                cs.OnDead += RemoveCs;
                wildRed1.Add(cs);
            }
            if (!wildRed2.Contains(cs) && cs.CharData.monsterAreaId == 2)
            {
                cs.OnDead += RemoveCs;
                wildRed2.Add(cs);
            }
            if (!wildRed3.Contains(cs) && cs.CharData.monsterAreaId == 3)
            {
                cs.OnDead += RemoveCs;
                wildRed3.Add(cs);
            }
        }
    }

    public override void RemoveCs(CharacterState cs)
    {
        //if (enemy.Contains(cs))
        //    enemy.Remove(cs);
        //if (friendly.Contains(cs))
        //    friendly.Remove(cs);
        //if (agents.Contains(cs))
        //    agents.Remove(cs);
        base.RemoveCs(cs);
        if (wildMonster.Contains(cs))
        {
            wildMonster.Remove(cs);
        }
        if (wildMonsterRed.Contains(cs))
        {
            wildMonsterRed.Remove(cs);
        }
        if (wildMonster1.Contains(cs))
        {
            wildMonster1.Remove(cs);
        }
        if (wildMonster2.Contains(cs))
        {
            wildMonster2.Remove(cs);
        }
        if (wildMonster3.Contains(cs))
        {
            wildMonster3.Remove(cs);
        }
        if (wildRed1.Contains(cs))
        {
            wildRed1.Remove(cs);
        }
        if (wildRed2.Contains(cs))
        {
            wildRed2.Remove(cs);
        }
        if (wildRed3.Contains(cs))
        {
            wildRed3.Remove(cs);
        }
    }
    Vector3 GetNaviById(int index, Dictionary<long, Moba3v3NaviNode> node, int type)
    {
        Vector3 pos = Vector3.zero;
        switch (index)
        {
            case 1:
                pos = node[(long)type].naviPoint0;
                break;
            case 2:
                pos = node[(long)type].naviPoint1;
                break;
            case 3:
                pos = node[(long)type].naviPoint2;
                break;
            case 4:
                pos = node[(long)type].naviPoint3;
                break;
            default:
                break;
        }
        return pos;
    }
    void BuildScene()
    {
        for (int i = 0; i < 2; i++)
        {
            towersBlueLeft.Add(CreateBuilding(towersCfg[i], this.transform, towersCfg[i].modelPos));
            towersBlueRight.Add(CreateBuilding(towersCfg[i + 2], this.transform, towersCfg[i + 2].modelPos));
            towersRedLeft.Add(CreateBuilding(towersCfg[i + 4], this.transform, towersCfg[i + 4].modelPos));
            towersRedRight.Add(CreateBuilding(towersCfg[i + 6], this.transform, towersCfg[i + 6].modelPos));

        }
        for (int i = 0; i < homeCfg.Count; i++)
        {
            if (homeCfg[i].groupIndex == 1)
            {
                HomeBlue = CreateHome(homeCfg[i], this.transform);
            }
            else if (homeCfg[i].groupIndex == 0)
            {
                HomeRed = CreateHome(homeCfg[i], this.transform);
            }
        }
        for (int i = 0; i < campCfg.Count; i++)
        {

            if (campCfg[i].groupIndex == 1)
            {
                CampBlue = CreateBuilding(campCfg[i], this.transform, campCfg[i].modelPos);
                CampBlue.transform.localScale = new Vector3(1.6f,1.2f,1.6f);
            }
            else if (campCfg[i].groupIndex == 0)
            {
                CampRed = CreateBuilding(campCfg[i], this.transform, campCfg[i].modelPos);
                CampRed.transform.localScale = new Vector3(1.6f, 1.2f, 1.6f);
            }
        }
        for (int i = 0; i < heroCfg.Count; i++)
        {
            matesPos[i].position = heroCfg[i].modelPos;
            matesPos[i].localEulerAngles = new Vector3(0,heroCfg[i].modelRot,0);
        }
        for (int i = 0; i < monsterCfg.Count; i++)
        {
            var spawn = CreateSpawn(this.transform, monsterCfg[i]);
            spawn.autos = GetNaviPoints(monsterCfg[i]);
        }

        for (int i = 0; i < wildMonsterCfg.Count; i++)
        {
            var spawn = CreateWildSpawn(this.transform, wildMonsterCfg[i]);
        }

        for (int i = 0; i < wildMonsterCfgRed.Count; i++)
        {
            var spawn = CreateWildSpawn(this.transform, wildMonsterCfgRed[i]);
        }
        if (block != null)
        {
            // CreateBlock();
        }
        for (int i = 0; i < wildBossCfg.Count; i++)
        {
            var spawn = CreateWildSpawn(this.transform, wildBossCfg[i]);
        }
    }

    public GameObject[] blueLeft, blueRight, redLeft, RedRight;
    GameObject[] GetNaviPoints(Moba3SceneConfigNode node)
    {
        GameObject[] naviPoints = null;
        if (node != null)
        {
            if (node.groupIndex == 1)
            {
                if (node.route == 1)
                {
                    naviPoints = blueLeft;
                }
                else if (node.route == 2)
                {
                    naviPoints = blueRight;
                }
            }
            else if (node.groupIndex == 0)
            {
                if (node.route == 1)
                {
                    naviPoints = redLeft;
                }
                else if (node.route == 2)
                {
                    naviPoints = RedRight;
                }
            }
        }
        return naviPoints;

    }

    void InitAllTowers()
    {
        for (int i = 0; i < towersBlueLeft.Count; i++)
        {
            InitTower(towersBlueLeft[i], 210000100, 1, Modestatus.Tower);
            InitTower(towersBlueRight[i], 210000100, 1, Modestatus.Tower);
            InitTower(towersRedLeft[i], 210000100, 0, Modestatus.Tower);
            InitTower(towersRedRight[i], 210000100, 0, Modestatus.Tower);
        }
        InitTower(CampBlue, 210000200, 1, Modestatus.Tower);
        InitTower(CampRed, 210000200, 0, Modestatus.Tower);
    }

    void OnMonsterCreated(GameObject go, CharacterData cd)
    {
        CharacterState cs = go.GetComponent<CharacterState>();
        InitMonsterHpAndAttack(cs);
        if (MobaMiniMap.instance != null)
        {
            MobaMiniMap.instance.AddMapIconByType(cs);
        }

        AddCs(cs);
        cs.OnDead += ChangeMorale;
    }

    void AddHeroAIs(CharacterState cs)
    {
        BasePlayerAI ai = AddHeroAIBySceneType(cs);
        if (ai != null)
        {
            if (cs.groupIndex == 1 && cs.CharData.memberId == 0)
            {
                ai.enabled = false;
            }
            else
            {
                ai.enabled = true;
                cs.pm.isAutoMode = true;
            }
        }
    }

    void InitHeroData(SceneType type)
    {
        switch (type)
        {
            case SceneType.NONE:
                break;
            case SceneType.KM:
                break;
            case SceneType.TP:
                break;
            case SceneType.KV:
                break;
            case SceneType.ES:
                break;
            case SceneType.TD:
                break;
            case SceneType.BO:
                break;
            case SceneType.ACT_GOLD:
                break;
            case SceneType.ACT_EXP:
                break;
            case SceneType.ACT_POWER:
                break;
            case SceneType.MB1:
                break;
            case SceneType.MB3:
                HeroData data = Globe.moba3v3MyTeam1[0];
                data.groupIndex = 1;
                data.state = Modestatus.Player;
                data.memberId = 0;
                data.fakeMobaPlayerName = playerData.GetInstance().selfData.playeName;
                herosDataBlue.Add(data);
                herosDataBlue.Add(GetHeroData(Globe.moba3v3MyTeam2, 1, Modestatus.NpcPlayer, 1));
                herosDataBlue.Add(GetHeroData(Globe.moba3v3MyTeam3, 1, Modestatus.NpcPlayer, 2));
                herosDataRed.Add(GetHeroData(Globe.moba3v3EnemyTeam1, 0, Modestatus.NpcPlayer, 0));
                herosDataRed.Add(GetHeroData(Globe.moba3v3EnemyTeam2, 0, Modestatus.NpcPlayer, 1));
                herosDataRed.Add(GetHeroData(Globe.moba3v3EnemyTeam3, 0, Modestatus.NpcPlayer, 2));
                break;
            case SceneType.MB5:
                break;
            case SceneType.PVP3:
                break;
            case SceneType.FD_LG:
                break;
            default:
                break;
        }
    }

    HeroData GetHeroData(HeroData[] hd, int groupindex, Modestatus status, int memberId)
    {
        HeroData data = hd[0];
        data.groupIndex = (uint)groupindex;
        data.state = status;
        data.memberId = memberId;
        data.fakeMobaPlayerName = UICreateRole.GetRandomName();
        return data;
    }

    void CreatAllHeros(SceneType type)
    {
        switch (type)
        {
            case SceneType.NONE:
                break;
            case SceneType.KM:
                break;
            case SceneType.TP:
                break;
            case SceneType.KV:
                break;
            case SceneType.ES:
                break;
            case SceneType.TD:
                break;
            case SceneType.BO:
                break;
            case SceneType.ACT_GOLD:
                break;
            case SceneType.ACT_EXP:
                break;
            case SceneType.ACT_POWER:
                break;
            case SceneType.MB1:
                break;
            case SceneType.MB3:
                for (int i = 0; i < 3; i++)
                {
                    herosBlue.Add(this.CreatHero(herosDataBlue[i], matesPos[i].gameObject, this.getAtkLineById(i)));
                }
                for (int i = 0; i < 3; i++)
                {
                    herosRed.Add(this.CreatHero(herosDataRed[i], matesPos[i + 3].gameObject, this.getAtkLineById(i)));
                }
                break;
            case SceneType.MB5:
                break;
            case SceneType.PVP3:
                break;
            case SceneType.FD_LG:
                break;
            default:
                break;
        }
    }

    HeroAttackLine getAtkLineById(int id)
    {
        HeroAttackLine atkLine = HeroAttackLine.none;
        int type = id % 2;
        if (type == 0)
        {
            atkLine = HeroAttackLine.leftLine;
        }
        else if (type == 1)
        {
            atkLine = HeroAttackLine.RightLine;
        }

        return atkLine;
    }

    bool isVirgin = true;
    CharacterState CreatHero(HeroData data, GameObject parGo, HeroAttackLine atkLine)
    {
        CharacterState cs = null;
        data.useServerAttr = false;
        data.RefreshAttr();
        if (data.groupIndex == 1)
        {
            if (data.state == Modestatus.Player)
            {
                cs = CreateMainHero(data).GetComponent<CharacterState>();
            }
            else if (data.state == Modestatus.NpcPlayer)
            {
                cs = CreateBattleHero(data, parGo).GetComponent<CharacterState>();
            }
            cs.AddHpBar(true);
            cs.pm.moveAreaMask = 1 | 1 << (int)NavAreas.BlueHeroPass;
        }
        else if (data.groupIndex == 0)
        {
            cs = CreateBattleHero(data, parGo).GetComponent<CharacterState>();
            cs.CharData.groupIndex = 0;
            cs.AddHpBar(true);
            cs.gameObject.tag = "EnemyPlayer";
            cs.pm.moveAreaMask = 1 | 1 << (int)NavAreas.RedHeroPass;
        }
        if (cs != null)
        {
            BasePlayerAI ai = AddHeroAIBySceneType(cs);
            if (cs.groupIndex == 1 && cs.CharData.memberId == 0)
            {
                ai.enabled = false;

            }
            else
                ai.enabled = true;
        }
        cs.OnHelp += TowerHelp;
        cs.OnDead += OnHeroDead;
        cs.OnDead += ChangeMorale;
        cs.OnDamageBy += (CharacterState mcs, CharacterState attackerCs) =>
        {
            if(BattleUtil.IsHeroTarget(attackerCs))
            {
                RefreshAttackedHeroRecords(mcs.CharData, attackerCs.CharData);
            }
            else if(attackerCs.state == Modestatus.SummonHero && attackerCs.Master != null)
            {
                RefreshAttackedHeroRecords(mcs.CharData, attackerCs.Master.CharData);
            }
        };
        cs.moveSpeed = cs.moveInitSpeed = 0.75f * cs.moveInitSpeed;
        cs.InitHp(cs.maxHp);
        cs.atkLine = atkLine;

        if (MobaMiniMap.instance != null)
        {
            MobaMiniMap.instance.AddMapIconByType(cs);
        }

        return cs;
    }

    bool win = false;
    void ShowResult(bool isWin)
    {
        win = isWin;
        isFinish = true;
        ThirdCamera.instance._MainPlayer = isWin ? CampRed.transform : CampBlue.transform;
        spwanList.AddRange(SpawnMonsters);
        DisableComponents();
        OverResult();
        Invoke("ShowResultPanel", 3f);
    }

    void ShowResultPanel()
    {
        Control.ShowGUI(UIPanleID.UITheBattlePanel, EnumOpenUIType.DefaultUIOrSecond, false, new object[] { true, win });
        AudioController.Instance.PlayBackgroundMusic(win ? "win" : "lose", false);
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
            SceneUIManager.instance.pnReborn.ShowRebornCD(RebornCD);
            ThirdCamera.instance._MainPlayer = null;
        }

        int mid = cs.CharData.memberId;
        int pos = GetPos(cs);
        UILabel laReborn = rebornThumbs[pos].GetComponentInChildren<UILabel>();
        CDTimer.CD cd = CDTimer.GetInstance().AddCD(1f, ( int count, long id ) =>
        {
            if(isFinish)
                return;
            ChangeColorGray.Instance.ChangeSpriteColor(rebornThumbs[pos].SpHeadIcon, false);
            rebornThumbs[pos].LaRebornCD.text = (count - 1).ToString();
            if(cs.groupIndex != CharacterManager.playerGroupIndex)
                rebornThumbs[pos].gameObject.SetActive(true);
        }, RebornCD);
        cd.OnRemove += ( int count, long id ) =>
        {
            ChangeColorGray.Instance.ChangeSpriteColor(rebornThumbs[pos].SpHeadIcon, true);
            rebornThumbs[pos].LaRebornCD.text = "";
            if(cs.groupIndex != CharacterManager.playerGroupIndex)
                rebornThumbs[pos].gameObject.SetActive(false);
            HeroAttackLine randLine = UnityEngine.Random.Range(0, 2) > 0 ? HeroAttackLine.RightLine : HeroAttackLine.leftLine;
            CreatHero(cs.groupIndex == 1 ? herosDataBlue[mid] : herosDataRed[mid],
                   cs.groupIndex == 1 ? matesPos[mid].gameObject : matesPos[mid + 3].gameObject,
                   randLine);
        };
    }

    protected override void RefreshInfo()
    {
        if (herosDataBlue!=null&& herosDataRed!=null & herosDataBlue.Count>0 && herosDataRed.Count>0)
        {
            SceneUIManager.instance.fightTouch.mobaStatic.Refresh(herosDataBlue[0], ScoreBlue, ScoreRed);
            //BattleCDandScore.instance.LaVS.text = herosDataBlue[0].mobaKillCount + "/" + herosDataRed[0].mobaMorale + "% " + ScoreBlue + "vs" + ScoreRed;
        }
    }

    protected override void OnSceneDestroy ()
    {
        base.OnSceneDestroy();
            ClearCsData(Globe.moba3v3MyTeam1);
        GameLibrary.isMoba = false;
    }
}



