using UnityEngine;
using System.Collections;
using Tianyu;
using System.Collections.Generic;

public enum ActivityType
{
    power,
    agile,
    intelligence
}

public enum DifficultyType
{
    simple,
    ordinary,
    difficult,
    great,
    nightmare,
    abyss
}

public class SceneActivityProps : SceneBaseManager
{

    public static int POWERCOUNT = 1000;
    public static int AGILECOUNT = 2000;
    public static int INTELCOUNT = 3000;

    public ActivityType state = ActivityType.power;
    public DifficultyType difficulty = DifficultyType.simple;

    public SpawnMonster Spawan;

    //Dictionary<int, bool> starDic = new Dictionary<int, bool>();

    ActivityPropsNode propsNode;
    SceneNode sn;

    Vector3 pos;

    int maxWave = 0;
    int currentWave;
    int rate;
    //float randomRot = 0;
    object[] monster;

    bool isCreatMonster = false;

    public override void StartCD()
    {

    }

    public override void InitScene()
    {
        base.InitScene();



        Globe.isFB = true;

        sn = FSDataNodeTable<SceneNode>.GetSingleton().FindDataByType(GameLibrary.dungeonId);
        if (sn != null)
        {
            switch (sn.bigmap_id)
            {
                case 30300: state = ActivityType.power; break;
                case 30400: state = ActivityType.agile; break;
                case 30500: state = ActivityType.intelligence; break;
            }

            switch (state)
            {
                case ActivityType.power: sceneType = SceneType.ACT_POWER; break;
                case ActivityType.agile: sceneType = SceneType.ACT_AGILE; break;
                case ActivityType.intelligence: sceneType = SceneType.ACT_INTEL; break;
            }

            switch (GameLibrary.dungeonId - sn.bigmap_id)
            {
                case 1: difficulty = DifficultyType.simple; break;
                case 2: difficulty = DifficultyType.ordinary; break;
                case 3: difficulty = DifficultyType.difficult; break;
                case 4: difficulty = DifficultyType.great; break;
                case 5: difficulty = DifficultyType.nightmare; break;
                case 6: difficulty = DifficultyType.abyss; break;
            }
        }

        GetTypeData(state);
        SetMaxWave(maxWave);

        SceneUIManager.instance.TowerDefence.ShowUI(false);
        CreateMainHero();
        escortNPC = player;

        ThirdCamera.instance._flatAngle = 270;

        propsNode = FSDataNodeTable<ActivityPropsNode>.GetSingleton().FindDataByType(currentWave);

        if (null == propsNode)
        {
            Debug.LogError("Loading jsonData is null.");
            return;
        }

        GetMonsterType(difficulty);

        Invoke("CreatMonster", propsNode.interval);

        SceneUIManager.instance.TowerDefence.RefreshWave(propsNode.interval, currentWave - rate, maxWave - rate);

        player.OnDead += (CharacterState cs) =>
        {
            WinCondition(false);
            playerDeadNum++;
        };

        SceneNode sceneNode = FSDataNodeTable<SceneNode>.GetSingleton().FindDataByType(GameLibrary.dungeonId);
        BattleCDandScore.instance.StartCD(null != sceneNode && sceneNode.time_limit != 0 ? (int)sceneNode.time_limit : 300);
        BattleCDandScore.instance.cd.OnRemove += (int count, long id) =>
        {
            if (player.isDie && BattleCDandScore.instance.cd.Id != id) return;
            WinCondition(false);
        };

    }

    public override void SendEndMessage(bool isWin, bool isBackMajor = false)
    {
        base.SendEndMessage(isWin, isBackMajor);
        CancelInvoke("CreatMonster");
    }

    public override void RemoveCs(CharacterState cs)
    {
        base.RemoveCs(cs);

        if (enemy.size <= 0 && !isCreatMonster)
        {
            isCreatMonster = true;
            NextMonster();
        }
    }

    void NextMonster()
    {

        if (currentWave >= maxWave)
        {
            WinCondition(true);
            return;
        }

        currentWave++;
        propsNode = FSDataNodeTable<ActivityPropsNode>.GetSingleton().FindDataByType(currentWave);
        GetMonsterType(difficulty);
        SceneUIManager.instance.TowerDefence.RefreshWave(propsNode.interval, currentWave - rate, maxWave - rate);
        Invoke("CreatMonster", propsNode.interval);

    }

    void CreatMonster()
    {
        isCreatMonster = false;
        if (spwanList.Count < (monster.Length))
        {
            int count = (monster.Length) - spwanList.Count;
            SpawnMonster sm = null;
            for (int i = 0; i < count; i++)
            {
                sm = Instantiate<SpawnMonster>(Spawan);
                sm.OnCreatMonster += (GameObject go, CharacterData cd) =>
                {
                    if (go.GetComponent<Monster_AI>())
                        go.GetComponent<Monster_AI>().targetCs = player;
                    if (go.GetComponent<CharacterState>())
                        go.GetComponent<CharacterState>().CharData.attrNode.field_distance *= 100;

                    //AddCs(go.GetComponent<CharacterState>());
                };
                sm.transform.parent = transform;
                if (!spwanList.Contains(sm))
                    spwanList.Add(sm);
            }
        }
        else if (spwanList.Count > (monster.Length))
        {
            int count = spwanList.Count - (monster.Length);
            int needCount = spwanList.Count - count;
            for (int i = spwanList.Count - 1; i > needCount; i--)
            {
                spwanList.Remove(spwanList[i]);
            }
        }

        object[] monsterDatas = null;
        for (int i = 0; i < (monster.Length); i++)
        {
            if (monster[i] is int[])
            {
                int[] mons = monster[i] as int[];
                monsterDatas = new object[mons.Length];
                for (int j = 0; j < mons.Length; j++)
                {
                    monsterDatas[j] = mons[j];
                }
            }
            else
            {
                monsterDatas = monster[i] as object[];
            }
            spwanList[i].transform.position = RandomPos(spwanList[i].transform);
            spwanList[i].PropCreatMonster((int)monsterDatas[0], float.Parse(monsterDatas[1].ToString()), float.Parse(monsterDatas[2].ToString()), 1);
        }
    }

    Vector3 RandomPos(Transform sm)
    {

        BattleUtil.GetRadiusRandomPos(sm, player.transform, 1.5f, 3f);
        pos = sm.position;

        UnityEngine.AI.NavMeshHit hit;
        bool isHit = UnityEngine.AI.NavMesh.SamplePosition(sm.position, out hit, 0.01f, UnityEngine.AI.NavMesh.AllAreas);
        if (!isHit)
        {
            RandomPos(sm);
        }

        for (int i = 0; i < spwanList.Count; i++)
        {
            if (sm == spwanList[i].transform) continue;
            if (Vector3.Distance(sm.position, spwanList[i].transform.position) < 1.5f / spwanList.Count)
            {
                RandomPos(sm);
            }
        }

        return pos;

    }

    void GetTypeData(ActivityType type)
    {
        switch (type)
        {
            case ActivityType.power:
                currentWave = 1001;
                rate = 1000;
                break;
            case ActivityType.agile:
                currentWave = 2001;
                rate = 2000;
                break;
            case ActivityType.intelligence:
                currentWave = 3001;
                rate = 3000;
                break;
            default:
                break;
        }
        maxWave = currentWave;
    }

    void GetMonsterType(DifficultyType type)
    {
        switch (type)
        {
            case DifficultyType.simple: GetMonsterData(propsNode.monster_simple); break;
            case DifficultyType.ordinary: GetMonsterData(propsNode.monster_ordinary); break;
            case DifficultyType.difficult: GetMonsterData(propsNode.monster_difficult); break;
            case DifficultyType.great: GetMonsterData(propsNode.monster_great); break;
            case DifficultyType.nightmare: GetMonsterData(propsNode.monster_nightmare); break;
            case DifficultyType.abyss: GetMonsterData(propsNode.monster_abyss); break;
        }
    }

    void GetMonsterData(object[] mons)
    {
        monster = mons;
    }

    void SetMaxWave(int current)
    {
        switch (state)
        {
            case ActivityType.power:
                if (current >= POWERCOUNT)
                    return;
                break;
            case ActivityType.agile:
                if (current >= AGILECOUNT)
                    return;
                break;
            case ActivityType.intelligence:
                if (current >= INTELCOUNT)
                    return;
                break;
        }

        ActivityPropsNode node = FSDataNodeTable<ActivityPropsNode>.GetSingleton().FindDataByType(current);

        if (MonsterIsNull(node))
        {
            maxWave++;
            SetMaxWave(maxWave);
        }
        else
        {
            maxWave--;
            return;
        }
    }

    bool MonsterIsNull(ActivityPropsNode node)
    {
        switch (difficulty)
        {
            case DifficultyType.simple:
                if (null == node.monster_simple || node.monster_simple.Length <= 0)
                    return false;
                else
                    return true;
            case DifficultyType.ordinary:
                if (null == node.monster_ordinary || node.monster_ordinary.Length <= 0)
                    return false;
                else
                    return true;
            case DifficultyType.difficult:
                if (null == node.monster_difficult || node.monster_difficult.Length <= 0)
                    return false;
                else
                    return true;
            case DifficultyType.great:
                if (null == node.monster_great || node.monster_great.Length <= 0)
                    return false;
                else
                    return true;
            case DifficultyType.nightmare:
                if (null == node.monster_nightmare || node.monster_nightmare.Length <= 0)
                    return false;
                else
                    return true;
            case DifficultyType.abyss:
                if (null == node.monster_abyss || node.monster_abyss.Length <= 0)
                    return false;
                else
                    return true;
        }
        return false;
    }

    public override void DisableComponents()
    {
        base.DisableComponents();
        CancelInvoke("CreatMonster");
    }

}
