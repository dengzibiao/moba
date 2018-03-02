using UnityEngine;
using System.Collections;
using Tianyu;
using System.Collections.Generic;
using System;

public class SceneTowerDefence : SceneBaseManager
{

    public CharacterState Camp;
    public CharacterState Tower1;
    public CharacterState Tower2;

    public float Interval = 0.5f;

    TowerDefenceNode towerNode;
    TowerDefenceNode eliteNode;

    Dictionary<int, SpawnMonster> point = new Dictionary<int, SpawnMonster>();
    Dictionary<CharacterState, float> towerlvl = new Dictionary<CharacterState, float>();

	int towerID=0;
	int campID =0;

    int wellenCount = 0;
    int eliteCount = 1000;

    int currentWellen = 1;
    int currentelite = 1001;

    int tdIndex = 0;

    bool isSole = false;

    public override void CreateScenePrefab(GameObject sceneCtrl)
    {
        if (null == sceneCtrl) return;

        TDLevelConfigNode node = null;
        foreach (int id in FSDataNodeTable<TDLevelConfigNode>.GetSingleton().DataNodeList.Keys)
        {
            node = FSDataNodeTable<TDLevelConfigNode>.GetSingleton().DataNodeList[id];
            if (node.sceneid == GameLibrary.dungeonId)
            {
                tdIndex = node.tdID;
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

        CharacterManager character = CreatePlayer(levelConfig[0].modelPos, sceneCtrl.transform);
        if (null != character)
        {
            character.autos = CreateAutoPoint(((TDLevelConfigNode)levelConfig[0]).auto, character.transform);
        }

        GameObject build = null;
        for (int i = 0; i < buildingConfig.Count; i++)
        {
            build = CreateBuilding(buildingConfig[i], sceneCtrl.transform);

            if (buildingConfig[i].type == 4)
            {
                Camp = build.GetComponent<CharacterState>();
                towerlvl.Add(Camp, buildingConfig[i].modellvl);
				campID =  buildingConfig [i].modelID;
            }
            else
            {
                if (null == Tower1)
                {
                    Tower1 = build.GetComponent<CharacterState>();
                    towerlvl.Add(Tower1, buildingConfig[i].modellvl);
                }
                else
                {
                    Tower2 = build.GetComponent<CharacterState>();
                    towerlvl.Add(Tower2, buildingConfig[i].modellvl);
                }
				towerID = buildingConfig [i].modelID;
            }
        }

        SpawnMonster[] monster = CreateSpawn(sceneCtrl.transform);
        for (int i = 0; i < monster.Length; i++)
        {
            monster[i].spawnID = i + 1;
            monster[i].autos = new GameObject[] { Camp.gameObject };
        }
    }

    public override void InitScene()
    {
        sceneType = SceneType.TD;
        ReadTask();
        CreateMainHero();

        currentWellen = Convert.ToInt32(tdIndex + "" + currentWellen);
        currentelite = Convert.ToInt32(tdIndex + "" + currentelite);

        SetMaxWave(1);
        wellenCount = maxWellen - 1;

        SetMaxWave(1001);
        eliteCount = maxWellen - 1;

        SpawnMonster[] sm = GetComponentsInChildren<SpawnMonster>();
        for (int i = 0; i < sm.Length; i++)
        {
            spwanList.Add(sm[i]);
        }
        for (int i = 0; i < spwanList.Count; i++)
        {
            if (!point.ContainsKey(spwanList[i].spawnID))
                point.Add(spwanList[i].spawnID, spwanList[i]);
        }

		InitTower(Tower1, towerID !=0 ? towerID : 206000300, 1, Modestatus.Tower);
		InitTower(Tower2, towerID !=0 ? towerID : 206000300, 1, Modestatus.Tower);
		InitTower(Camp, campID != 0? campID : 206000700, 1, Modestatus.Tower);

        Camp.gameObject.layer = Tower1.gameObject.layer = Tower2.gameObject.layer = (int)GameLayer.Tower;
        Camp.OnHit += SceneUIManager.instance.TowerDefence.RefreshCampHP;
        Camp.OnDead += (CharacterState cCs) => WinCondition(false);
        player.OnDead += (CharacterState cCs) => WinCondition(false);

        Tower1.OnHit += (CharacterState cs) => { ShowBloodScreen(); };
        Tower2.OnHit += (CharacterState cs) => { ShowBloodScreen(); };
        Camp.OnHit += (CharacterState cs) => { ShowBloodScreen(false); };

        towerNode = FSDataNodeTable<TowerDefenceNode>.GetSingleton().FindDataByType(currentWellen);

        if (currentWellen <= wellenCount)
        {
            Invoke("BrushSoldiers", towerNode.Interval);
        }

        eliteNode = FSDataNodeTable<TowerDefenceNode>.GetSingleton().FindDataByType(currentelite);
        InvokeRepeating("EliteMonsters", eliteNode.Interval, eliteNode.Interval);

        SceneUIManager.instance.TowerDefence.RefreshWave(towerNode.Interval, currentWellen % tdIndex, wellenCount % tdIndex);

        base.InitScene();
    }

    TowerDefenceNode maxNode;
    int maxWellen = 1;

    void SetMaxWave(int max)
    {
        maxWellen = Convert.ToInt32(tdIndex + "" + max);
        maxNode = FSDataNodeTable<TowerDefenceNode>.GetSingleton().FindDataByType(maxWellen);
        if (null == maxNode)
            return;
        max++;
        SetMaxWave(max);
    }

    void InitTower(CharacterState cs, long id, UInt32 groupIndex, Modestatus state)
    {
        MonsterData towerData = new MonsterData(id, (int)towerlvl[cs]);
        towerData.groupIndex = groupIndex;
        towerData.lvlRate = towerlvl[cs];
        towerData.state = Modestatus.Tower;
        cs.InitData(towerData);
        cs.GetComponent<Tower_AI>().InitTowerAI();
        cs.AddHpBar();
        AddCs(cs);
    }

    public override void RemoveCs(CharacterState cs)
    {
        base.RemoveCs(cs);
        bool isRefresh = false;
        for (int i = 0; i < enemy.size; i++)
        {
            if (enemy[i].GetComponent<Boss_AI>()) return;
            if (enemy[i].GetComponent<Monster_AI>() && !enemy[i].GetComponent<Monster_AI>().IsElite)
            {
                isRefresh = true;
                return;
            }
        }

        if (!isRefresh && isSole)
        {
            NextWave();
            isSole = false;
        }
    }

    void NextWave()
    {
        if (currentWellen >= wellenCount)
        {
            CancelInvoke("EliteMonsters");
            DungeonOverHandle(null);
            return;
        }

        currentWellen++;
        towerNode = FSDataNodeTable<TowerDefenceNode>.GetSingleton().FindDataByType(currentWellen);
        SceneUIManager.instance.TowerDefence.RefreshWave(towerNode.Interval, currentWellen % tdIndex, wellenCount % tdIndex);
        Invoke("BrushSoldiers", towerNode.Interval);
    }

    void BrushSoldiers()
    {
        isSole = true;
        List<int> smID = new List<int>(point.Keys);
        for (int i = 0; i < smID.Count; i++)
        {
            if (smID[i] == 1)
                CheckMonsterList(towerNode.Monster1, point[smID[i]]);
            else if (smID[i] == 2)
                CheckMonsterList(towerNode.Monster2, point[smID[i]]);
            else if (smID[i] == 3)
                CheckMonsterList(towerNode.Monster3, point[smID[i]]);
        }
    }

    void CheckMonsterList(object[] mon, SpawnMonster sm)
    {
        if (null != mon && mon.Length > 0)
            WellenBS(mon, sm);
    }

    void WellenBS(object[] id, SpawnMonster sm)
    {
        sm.SetMonsterPoint(id, false, Interval);
    }

    void EliteMonsters()
    {
        eliteNode = FSDataNodeTable<TowerDefenceNode>.GetSingleton().FindDataByType(currentelite);

        if (currentelite >= eliteCount)
        {
            CancelInvoke("EliteMonsters");
        }

        List<int> smID = new List<int>(point.Keys);
        for (int i = 0; i < smID.Count; i++)
        {
            if (smID[i] == 1 && null != eliteNode.Monster1 && eliteNode.Monster1.Length > 0)
                point[smID[i]].SetMonsterPoint(eliteNode.Monster1, true);
            else if (smID[i] == 2 && null != eliteNode.Monster2 && eliteNode.Monster2.Length > 0)
                point[smID[i]].SetMonsterPoint(eliteNode.Monster2, true);
            else if (smID[i] == 3 && null != eliteNode.Monster3 && eliteNode.Monster3.Length > 0)
                point[smID[i]].SetMonsterPoint(eliteNode.Monster3, true);
        }

        currentelite++;

    }

    void ShowBloodScreen(bool isShowPrompt = true)
    {
        SceneUIManager.instance.gamePrompt.SwitchBloodScreen(true, isShowPrompt, sceneType);
    }

    public override void DisableComponents()
    {
        base.DisableComponents();
        CancelInvoke("EliteMonsters");
        CancelInvoke("BrushSoldiers");
    }


}