using UnityEngine;
using System.Collections;

public class SceneToEscape : SceneBaseManager
{
    public new static SceneToEscape instance;

    public ToEscapeTrigger teTigger;
    public UnityEngine.AI.NavMeshObstacle bossBlocker;
    public SpawnMonster bossSmStar;
    public SpawnMonster bossSmEnd;

    public float playerSpeed = 1;
    public float bossSpeed = 1;

    GameObject StarBoss;

    int glod = 0;

    public override void InitScene ()
    {
        sceneType = SceneType.ES;
        instance = this;
        ReadTask();
        CreateMainHero();
        escortNPC = player;

        FightTouch._instance.SetAllBtnLockStatus(true);

        bossSmStar.OnCreatMonster += (GameObject go, CharacterData cd ) =>
        {
            StarBoss = go;
            go.GetComponent<CharacterState>().moveSpeed = bossSpeed;
            go.GetComponent<CharacterState>().Invincible = true;
            cd.attrNode.field_distance *= 100;
            RemoveCs(StarBoss.GetComponent<CharacterState>());
        };

        teTigger.OnExitT += OnExit;

        bossSmEnd.OnCreatMonster += (GameObject go, CharacterData cd ) =>
        {
            StarBoss = null;
            bossSmStar.gameObject.SetActive(false);
            bossBlocker.gameObject.SetActive(true);

            CDTimer.GetInstance().AddCD(1f, (int count, long id) => UnityUtil.AddComponetIfNull<Boss_AI>(go));
            //GameLibrary.bossBlood.IsShow();
            go.GetComponent<CharacterState>().OnDead += (CharacterState Cs) =>
            {
                isFinish = true;
                DungeonOverHandle(null);
            };

            player.GetComponent<Player_AutoAI>().isAttack = true;
            FightTouch._instance.SetAllBtnLockStatus(false);
        };

        player.OnDead += (CharacterState Cs) => WinCondition(false);
        player.OnBeAttack += (CharacterState CS) =>
        {
            if (null != StarBoss && CS.gameObject == StarBoss)
            {
                playerbeAtt11++;
                playerbeAtt7++;
                SceneUIManager.instance.gamePrompt.SwitchBloodScreen(true);
            }
        };

        //player.moveSpeed = playerSpeed;
        //player.CharData.attrNode.movement_speed = playerSpeed;
        player.moveSpeed = player.moveInitSpeed = playerSpeed;

        base.InitScene();
    }

    public void CollectItems(int count)
    {
        glod += count;
    }

    public void OnGoldBuffPoint()
    {
        ClientSendDataMgr.GetSingle().GetBattleSend().SendFightSettlement(GameLibrary.mapId, GameLibrary.dungeonId, dungeonTypes, GameLibrary.star, 1);
    }

    void OnExit()
    {
        if (null != StarBoss)
        {
            RemoveCs(StarBoss.GetComponent<CharacterState>());
            Destroy(StarBoss);
        }
        bossSmEnd.enabled = true;
        CharacterManager.playerCS.SetAttackTargetTo(null);
    }
}
