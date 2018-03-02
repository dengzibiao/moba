using UnityEngine;
using System.Collections;
using Tianyu;
using System.Collections.Generic;

public class SceneEscort : SceneBaseManager
{

    public new static SceneEscort instance;

    GameObject bullockCarts;
    //Transform autoPoint;

    public GameObject cart;

    CharacterState cartCs;

    int GlodCount = 0;

    public override void CreateScenePrefab(GameObject sceneCtrl)
    {
        base.CreateScenePrefab(sceneCtrl);

        if (transform.Find("BullockCarts"))
        {
            List<Transform> autoPoint = new List<Transform>();
            for (int i = 0; i < transform.Find("BullockCarts").childCount; i++)
            {
                autoPoint.Add(transform.Find("BullockCarts").GetChild(i));
            }
            Resource.CreatPrefabs("UI_YinDao_GuangQuan_01", autoPoint[autoPoint.Count - 1].gameObject, Vector3.zero, GameLibrary.Effect_UI);
            autoPoint.Clear();
        }
    }

    public override void InitScene ()
    {
        instance = this;

        if (GameLibrary.dungeonId != 0 && GameLibrary.dungeonId / 100 == 301)
            sceneType = SceneType.ACT_GOLD;
        else if (GameLibrary.dungeonId != 0 && GameLibrary.dungeonId / 100 == 302)
            sceneType = SceneType.ACT_EXP;
        else
            sceneType = SceneType.KV;

        Globe.isFB = true;

        bullockCarts = transform.Find("BullockCarts").gameObject;
        //autoPoint = bullockCarts.transform.Find(Tag.auto);

        CreateMainHero();

        if (sceneType == SceneType.KV)
        {
            ReadTask();
            CreatBullockCarts(curlevelConfig[0].npcID, curlevelConfig[0].npclvl);
        }
        else
        {
            string cartid = GameLibrary.dungeonId + "000";
            LevelConfigsNode level = FSDataNodeTable<LevelConfigsNode>.GetSingleton().FindDataByType(int.Parse(cartid));
            CreatBullockCarts(level.monsterID, level.monsterlvl, level.scale);
        }
        

        EffectBlock[] eb = GetComponentsInChildren<EffectBlock>();
        for (int i = 0; i < eb.Length; i++)
        {
            eb[i].LoadSpawnMonster();
            eb[i].OnCloseWall += (int num) => OnCloseWall(num);
            effectList.Add(eb[i]);
        }

        SpawnMonster[] sm = GetComponentsInChildren<SpawnMonster>();
        for (int i = 0; i < sm.Length; i++)
        {
            sm[i].isKM = false;
            spwanList.Add(sm[i]);
        }

        for (int i = 0; i < spwanList.Count; i++)
        {
            spwanList[i].enabled = false;
        }

        for (int i = 0; i < effectList.Count; i++)
        {
            effectList[i].OnCanMoveCarts += TriggerOperating;
        }

        //Destroy(player.GetComponent<BasePlayerAI>());
        //player.pm.isAutoMode = true;
        //player.gameObject.AddComponent<Player_CollectAI>();

        escortNPC = cart.GetComponent<CharacterState>();
        player.GetComponent<Player_AutoAI>().protectionCs = escortNPC;
        player.OnDead += (c) => { WinCondition(false); };

        base.InitScene();
    }

    public void CollectItems(int count)
    {
        GlodCount += count;
        SceneUIManager.instance.RefreshACTCounter(GlodCount);
    }

    void TriggerOperating(bool isMove, EffectBlock eb)
    {
        cart.GetComponent<BullockCarts>().isMove = isMove;
        eb.enabled = false;
        eb.GetComponent<UnityEngine.AI.NavMeshObstacle>().enabled = false;
        eb.GetComponent<BoxCollider>().enabled = false;
    }

    void CreatBullockCarts(int monsterid, float lvl, float scale = 1)
    {
        MonsterAttrNode node = FSDataNodeTable<MonsterAttrNode>.GetSingleton().FindDataByType(monsterid);
        if (null == node)
        {
            Debug.Log("huoche id not found.");
            return;
        }
        cart = Resource.CreatPrefabs(node.icon_name, bullockCarts, Vector3.zero, GameLibrary.NPC_URL);
        if (null == cart)
            cart = Resource.CreatPrefabs(node.icon_name, bullockCarts, Vector3.zero, GameLibrary.Monster_URL);
        if (null == cart)
            cart = Resource.CreatPrefabs(node.icon_name, bullockCarts, Vector3.zero, GameLibrary.Hero_URL);
        if (null == cart)
        {
            Debug.Log("Cart is null.");
            return;
        }
        cart.gameObject.layer = CharacterManager.player.gameObject.layer;
        cart.transform.localScale = Vector3.one * scale;
        SetBullockCarts(node.id, lvl);
    }

    void SetBullockCarts(long id, float lvl)
    {
        MonsterData md = new MonsterData(id, (int)lvl);
        md.groupIndex = 1;
        md.lvlRate = lvl;
        md.state = Modestatus.NpcPlayer;
        cartCs = BattleUtil.AddMoveComponents(cart, md.attrNode.modelNode);
        cartCs.pm.nav.obstacleAvoidanceType = UnityEngine.AI.ObstacleAvoidanceType.NoObstacleAvoidance;
        cartCs.pm.nav.avoidancePriority = 59;
        cartCs.InitData(md);
        BullockCarts bc = UnityUtil.AddComponetIfNull<BullockCarts>(cart);
        if (bc)
        {
            bc.OnArrDes += () => { DungeonOverHandle(null); };
            cartCs.OnHit += bc.BeAttacked;
        }
        cartCs.OnDead += (c) => { WinCondition(false); };
        cartCs.moveSpeed = cartCs.moveInitSpeed;
        cartCs.AddHpBar();
        AddCs(cartCs);
        if (sceneType == SceneType.KV)
        {
            GameObject prompt = new GameObject("UIPrompt");
            prompt.transform.parent = cartCs.transform;
            prompt.transform.localPosition = new Vector3(0, 1.2f, 0);
            CDTimer.GetInstance().AddCD(0.5f, (int count, long cid) => { HUDAndHPBarManager.instance.AddPrompt(prompt.transform); });
        }
        cart.tag = "Cart";
    }

    public override void RemoveCs(CharacterState cs)
    {
        base.RemoveCs(cs);
        if (enemy.size <= 0)
            cart.GetComponent<BullockCarts>().isMove = true;
    }

}
