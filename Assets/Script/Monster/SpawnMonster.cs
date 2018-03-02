using UnityEngine;
using System.Collections;
using Tianyu;
using UnityEngine.SceneManagement;
using System;

public class SpawnMonster : MonoBehaviour
{

    public delegate void OnCreat(GameObject go, CharacterData cd);
    public OnCreat OnCreatMonster;

    public int spawnID = 1;
    public int spawnQueue;
    public int monsterCount = 1;
    public int monsterLevel = 1;
    public float monsterScale = 1f;
    public SpawnTrigger trigger;
    public float distance;
    public float spawnTimer;
    public float interval;
    public bool spawnAfterDie;
    public UInt32 groupIndex;
    public int monsterAreaId;
    public GameObject[] autos;
    public string effectSign = "";
    [HideInInspector]
    public bool spawnOver = false;
    public float rotationY = 0;
    public MonsterData monsterData = new MonsterData(0);

    public float lvlRate = 0f;

    public bool isKM = true;

    private EffectBlock effect_Block = null;
    private BossChuChangField effect_Boss = null;

    bool isElite = false;

    public bool isTP;

    LevelConfigsNode config;
    string levelID = "";

    public void StartSpawn()
    {
        if (GameLibrary.dungeonId >= 30000)
        {
            if (spawnID > 99)
                levelID = "" + spawnID;
            else if (spawnID > 9)
                levelID = "0" + spawnID;
            else
                levelID = "00" + spawnID;

            if (!GameLibrary.SceneType(SceneType.TD) && !GameLibrary.SceneType(SceneType.MB1) && !GameLibrary.SceneType(SceneType.Dungeons_MB1))
            {
                levelID = GameLibrary.dungeonId.ToString() + levelID;
                config = FSDataNodeTable<LevelConfigsNode>.GetSingleton().FindDataByType(Convert.ToUInt32(levelID));
            }

            if (null != config)
            {
                spawnQueue = config.monsterID;
                monsterLevel = (int)config.monsterlvl;
                lvlRate = config.monsterlvl;
                monsterScale = config.scale;
            }
        }
        if (spawnQueue == 0) return;

        if (transform.parent.GetComponent<EffectBlock>())
            effect_Block = transform.parent.GetComponent<EffectBlock>();

        if (transform.parent.GetComponent<BossChuChangField>())
            effect_Boss = transform.parent.GetComponent<BossChuChangField>();

        if (trigger != null)
        {
            trigger.OnTrigger += CreatMonster;
        }
        else if (distance > 0 && isKM)
        {
            InvokeRepeating("InvokeCheckDistance", 0f, 0.2f);
        }
        else if (spawnTimer > 0)
        {
            if (isTP)
                InvokeRepeating("InvokeMonster", spawnTimer, interval);
            else
                Invoke("CreatMonster", spawnTimer);
        }
        else
        {
            CreatMonster();
        }
    }

    public void StopCreatMonster()
    {
        CancelInvoke();
        StopAllCoroutines();
    }

    void InvokeCheckDistance()
    {
        if (CharacterManager.player != null)
        {
            if (distance <= 0) return;
            float dis = Vector3.Distance(CharacterManager.player.transform.position, transform.position);
            if (dis > distance) return;
            Invoke("CreatMonster", spawnTimer);
            distance = 0;
        }
    }

    [ContextMenu("DoSpawn")]
    void CreatMonster()
    {
        if (monsterCount < 999) monsterCount -= 1;
        if (monsterCount == 0) spawnOver = true;
        if (monsterCount < 0) return;

        monsterData = new MonsterData(spawnQueue);

        if (null == monsterData || null == monsterData.attrNode)
        {
            Debug.Log("Not fond monster id " + spawnQueue);
            return;
        }

        if (!String.IsNullOrEmpty(((MonsterAttrNode)monsterData.attrNode).effect_sign))
            effectSign = ((MonsterAttrNode)monsterData.attrNode).effect_sign;
        if (effectSign != "")
        {
            if (SceneBaseManager.instance.enemyModel.ContainsKey(effectSign))
            {
                GameObject go = Instantiate(SceneBaseManager.instance.enemyModel[effectSign]);
                go.transform.localPosition = Vector3.zero;
            }

            else
                Resource.CreatPrefabs(effectSign, gameObject, Vector3.zero, GameLibrary.Effect_Spawn);

        }

        //string modelName = GameLibrary.isMoba || SceneBaseManager.instance.sceneType == SceneType.TP ? monsterData.attrNode.icon_name + "" + groupIndex : monsterData.attrNode.icon_name;
        monsterData.lvl = monsterLevel;
        monsterData.lvlRate = lvlRate;
        monsterData.groupIndex = groupIndex;
        monsterData.monsterAreaId = monsterAreaId;
        monsterData.state = Modestatus.Monster;
        if (BattleUtil.IsBoss(monsterData))
            monsterData.state = Modestatus.Boss;

        CharacterState mCs;
        //if (GameLibrary.isMoba)
        //{
        //    if (SceneMoba3.instance != null)
        //    {
        //        mCs = SceneMoba3.instance.CreateBattleMonster(monsterData, gameObject);
        //    }
        //    else
        //    {
        //        mCs = SceneBaseManager.instance.CreateBattleMonster(monsterData, gameObject);
        //    }

        //}
        //else
        //{
        //}
        mCs = SceneBaseManager.instance.CreateBattleMonster(monsterData, gameObject);

        if (mCs != null)
        {
            mCs.OnDead += Die;
            monsterScale = monsterScale > 0 ? monsterScale : 1f;
            if (null == config)
                monsterScale = ((MonsterAttrNode)monsterData.attrNode).model_size > 0 ? ((MonsterAttrNode)monsterData.attrNode).model_size : monsterScale;
            mCs.transform.localScale = Vector3.one * monsterScale;
            if (!mCs.pm.nav.isOnNavMesh)
                Debug.LogError("Error position : " + mCs.CharData.id + " " + mCs.CharData.attrNode.name);
        }
        if (monsterCount > 0)
        {
            if (!spawnAfterDie)
            {
                Invoke("CreatMonster", interval);
            }
            if (mCs != null && spawnAfterDie)
            {
                mCs.OnDead += (CharacterState cs) => Invoke("CreatMonster", GameLibrary.DIE_DESTROY_DELAY + interval);
            }
        }
        if (mCs != null)
        {
            AddMonsterAI(mCs.gameObject);
            if (mCs.groupIndex == 99 || mCs.groupIndex == 100)
            {
                Destroy(mCs.gameObject.GetComponent<MonsterAI3v3>());
                // UnityUtil.AddComponetIfNull<WildMonster_AI>(mCs.gameObject);                
                UnityUtil.AddComponetIfNull<Monster_WildAI>(mCs.gameObject);
                mCs.transform.localEulerAngles = new Vector3(0, rotationY, 0);
            }
            if (mCs.groupIndex==101)
            {
                Destroy(mCs.gameObject.GetComponent<Boss_AI>());
                UnityUtil.AddComponetIfNull<Monster_WildAI>(mCs.gameObject);
                mCs.transform.localEulerAngles = new Vector3(0, rotationY, 0);
            }
            if (CharacterManager.player != null && mCs.groupIndex != 99 && mCs.groupIndex != 100)
                mCs.transform.LookAt(CharacterManager.player.transform.position);

            if (null != OnCreatMonster)
                OnCreatMonster(mCs.gameObject, monsterData);
        }
    }

    void AddMonsterAI(GameObject go)
    {
        if (BattleUtil.IsBoss(monsterData))
        {
            go.tag = Tag.boss;
            UnityUtil.AddComponetIfNull<Boss_AI>(go);
        }
        else
        {
            Monster_AI mAI = null;
            switch (SceneBaseManager.instance.sceneType)
            {
                case SceneType.TP:
                    mAI = UnityUtil.AddComponetIfNull<TotalPoints_AI>(go);
                    break;
                case SceneType.KV:
                case SceneType.TD:
                case SceneType.ACT_GOLD:
                case SceneType.ACT_EXP:
                    mAI = UnityUtil.AddComponetIfNull<EscoerMonster_AI>(go);
                    mAI.IsElite = isElite;
                    break;
                case SceneType.ES:
                    mAI = UnityUtil.AddComponetIfNull<ToEscape_AI>(go);
                    break;
                case SceneType.MB3:
                    mAI = UnityUtil.AddComponetIfNull<MonsterAI3v3>(go);
                    break;
                default:
                    mAI = UnityUtil.AddComponetIfNull<Monster_AI>(go);
                    break;
            }

            //switch(SceneManager.GetActiveScene().name)
            //{
            //    case GameLibrary.PVP_Moba:
            //        mAI.chaseTime = 1f;
            //        mAI.backPositon = BackPosition.chaseStartPos;
            //        break;
            //    default:
            //        mAI.isBackRestore = true;
            //        mAI.backPositon = BackPosition.bornPos;
            //        break;
            //}
            mAI.autoPoints = autos;
        }



    }

    void Die(CharacterState cs)
    {
        if (null != effect_Block)
            effect_Block.ChangeCount();
        if (GameLibrary.LGhuangyuan == SceneManager.GetActiveScene().name && monsterData.attrNode.icon_name == "gw_116")
        {
            effect_Boss.ChangeCount();
        }
    }

    void InvokeMonster()
    {
        index = 0;
        StartCoroutine(CreatMonsterQueue(new object[2] { new object[] { spawnQueue, lvlRate != 0 ? lvlRate : 1, 1 }, new object[] { spawnQueue, lvlRate != 0 ? lvlRate : 1, 1 } }));
    }

    public void SetMonsterPoint(object[] monster, bool isElite = false, float interval = 0.5f)
    {
        index = 0;
        this.isElite = isElite;
        StartCoroutine(CreatMonsterQueue(monster, interval));
    }

    int index = 0;
    IEnumerator CreatMonsterQueue(object[] monster, float interval = 0.5f)
    {
        object[] monsterDatas = null;
        if (monster[index] is int[])
        {
            int[] mons = monster[index] as int[];
            monsterDatas = new object[mons.Length];
            for (int i = 0; i < mons.Length; i++)
            {
                monsterDatas[i] = mons[i];
            }
        }
        else
        {
            monsterDatas = monster[index] as object[];
        }

        PropCreatMonster((int)monsterDatas[0], float.Parse(monsterDatas[1].ToString()), float.Parse(monsterDatas[2].ToString()), 1);

        index++;
        yield return new WaitForSeconds(interval);

        if (index < (monster.Length / 2))
            StartCoroutine(CreatMonsterQueue(monster, interval));
        else
            StopCoroutine("CreatMonsterQueue");
    }

    public void PropCreatMonster(int id, float lvl, float scale, int count, string effectSign = "")
    {
        spawnQueue = id;
        monsterLevel = (int)lvl;
        lvlRate = lvl;
        monsterScale = scale;
        monsterCount = count;
        if (effectSign != "")
            this.effectSign = effectSign;
        CreatMonster();
    }

}