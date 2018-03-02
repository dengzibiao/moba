using UnityEngine;
using Tianyu;
using System.Collections.Generic;
using System;
using System.Collections;

public class SceneDungeons : SceneBaseManager
{

    int indexSpawn = 0;
    int endSpawn = 0;

    bool isBrushStrange = true;
    bool isHaveBoss = false;

    public override void InitScene()
    {
        sceneType = SceneType.KM;

        ReadTask();

        EffectBlock[] eb = GetComponentsInChildren<EffectBlock>();
        for (int i = 0; i < eb.Length; i++)
        {
            eb[i].OnCloseWall += (int num) => OnCloseWall(num);
            effectList.Add(eb[i]);
        }

        SpawnMonster[] sm = GetComponentsInChildren<SpawnMonster>();
        for (int i = 0; i < sm.Length; i++)
        {
            sm[i].isKM = false;
            sm[i].OnCreatMonster += (GameObject go, CharacterData cd) =>
            {
                if (BattleUtil.IsBoss(cd))
                {
                    bossCs = go.GetComponent<CharacterState>();
                    bossCs.OnDead += (CharacterState bCs) => DungeonOverHandle(bCs);
                }
            };
            spwanList.Add(sm[i]);
            if (sm[i].tag == Tag.boss)
                isHaveBoss = true;
            if (sm[i].distance > endSpawn)
                endSpawn = (int)sm[i].distance;
        }

        CreateMainHero();
        escortNPC = player;

        ThirdCamera.instance._flatAngle = GameLibrary.dungeonId > 0 ? FSDataNodeTable<SceneNode>.GetSingleton().FindDataByType(GameLibrary.dungeonId).flat_angle : 270;

        if (null != CharacterManager.instance.autos && CharacterManager.instance.autos.Length > 0)
            player.transform.LookAt(CharacterManager.instance.autos[0].transform);

        player.OnDead += (CharacterState pCs) =>
        {
            WinCondition(false);
            playerDeadNum++;
        };

        BrushStrange();

        base.InitScene();
    }

    void FixedUpdate()
    {
        if (enemy.size <= 0 && isBrushStrange)
        {
            BrushStrange();
        }
    }

    void BrushStrange()
    {
        indexSpawn++;
        if (!isHaveBoss && indexSpawn == endSpawn)
        {
            isBrushStrange = false;

            if (null != bossChuChang && null != bossChuChang.bossobj)
            {
                bossChuChang.TriggerBoss();
            }
            else
            {
                SceneUIManager.instance.bossWarning.ShowWarning(1.5f);
                Invoke("ShowBossWinning", 1.5f);
            }
            return;
        }
        int count = 0;
        for (int i = 0; i < spwanList.Count; i++)
        {
            if (spwanList[i].distance == indexSpawn && spwanList[i].tag != Tag.boss)
            {
                spwanList[i].StartSpawn();
                count++;
            }
        }
        if (count <= 0)
            isBrushStrange = false;
    }

    void ShowBossWinning()
    {
        for (int i = 0; i < spwanList.Count; i++)
        {
            if (spwanList[i].distance == indexSpawn)
            {
                spwanList[i].StartSpawn();
            }
        }
    }

}