using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

public class SceneArena : SceneBaseManager
{
    public Transform[] partnerPos;
    public Transform[] myHeroTeam;
    public Transform[] dyHeroTeam;

    List<ArenaPlayer> npcList = new List<ArenaPlayer>();
    Dictionary<CharacterState, TeamIcon> heroIcon = new Dictionary<CharacterState, TeamIcon>();
    Dictionary<string, float> damageList = new Dictionary<string, float>();
    List<ArenaPlayer> damageHeroList = new List<ArenaPlayer>();
    List<CharacterState> heroCsList = new List<CharacterState>();

    //bool isRandom = true;

    //int[] mList = new int[6];
    //int[] dList = new int[6];
    List<int> mL = new List<int>();
    List<int> dL = new List<int>();

    static int round;
    float timeEclipse;
    bool TestVersion = false;

    public override void StartCD()
    {

    }

    public override void InitScene()
    {
        GameLibrary.isPVP3 = true;

        //if (ThirdCamera.instance)
        //{
        //    ThirdCamera.instance.transform.position = new Vector3(6.2f, 4.4f, 1.6f);
        //    ThirdCamera.instance.transform.rotation = Quaternion.Euler(38f, -90f, 0);
        //    //ThirdCamera.instance._flatAngle = 320;
        //}

        if (!GameLibrary.isNetworkVersion)
        {
            //if (isRandom)
            //{
            //    List<long> randIds = BattleUtil.GetRandomTeam(6);
            //    for (int i = 0; i < mList.Length; i++)
            //    {
            //        mList[i] = (int)randIds[i];
            //    }
            //    List<long> randEnemyIds = BattleUtil.GetRandomTeam(6);
            //    for (int i = 0; i < dList.Length; i++)
            //    {
            //        dList[i] = (int)randEnemyIds[i];
            //    }
            //}
            //GameLibrary.player = mList[0];

            //Globe.challengeTeam = new HeroData[] { new HeroData(201001000), new HeroData(201001900), new HeroData(201001800), new HeroData(201001400), new HeroData(201000900), new HeroData(201002200) };
            Globe.arenaFightHero = new int[] { 201001000, 201001900, 201001800, 201001400, 201000900, 1 };
            //mL.Add(mList[0]);
            //mL.Add(mList[4]);
            //mL.Add(mList[5]);
            Globe.ArenaEnemy = new HeroData[] { new HeroData(201000300), new HeroData(201001200), new HeroData(201003300), new HeroData(201001600), new HeroData(201001100), new HeroData(201001400) };
            //dL.Add(dList[0]);//new HeroData(dList[0]), new HeroData(dList[1]), new HeroData(dList[2]), new HeroData(dList[3]), new HeroData(dList[4]), new HeroData(mList[5])
            //dL.Add(dList[4]);
            //dL.Add(dList[5]);
            //Globe.InitHeroSort(Globe.challengeTeam);
        }

        //Globe.InitHeroSort(Globe.ArenaEnemy);

        //InitPosition(CharacterManager.instance.transform, partnerPos[1], partnerPos[2], Globe.challengeTeam, true);
        //InitPosition(partnerPos[3], partnerPos[4], partnerPos[5], Globe.ArenaEnemy, false);

        InitPosition(Globe.arenaFightHero[5], myHeroTeam, 0);
        InitPosition(Globe.ArenaState, dyHeroTeam, 5);

        CreateTeam();
        //InitPlayer();

        if (!TestVersion)
        {
            SceneUIManager.instance.arenaPanel.SetArenaCD(300);
            SceneUIManager.instance.arenaPanel.ArenaCD.cd.OnRemove += (int count, long id) =>
             {
                 if (!isEnd)
                 {
                     win = false;
                     GameOver();
                 }
             };
            Delay.instance.OnDelayDone += EnableAIs;
            Delay.instance.Do(3);
        }
        else
        {
            timeEclipse = Time.realtimeSinceStartup;
            EnableAIs();
        }

        if (GameLibrary.isNetworkVersion)
        {
            SceneUIManager.instance.arenaPanel.RefreshName(Globe.arenahero.lvl, Globe.arenahero.nm);
            ClientSendDataMgr.GetSingle().GetBattleSend().SendStarArenaFighting(0);
        }
        if (null != FightTouch._instance)
            FightTouch._instance.OnInit += () => FightTouch._instance.SetSkillEffect(false);
    }

    float xDeviation = 0.6f;
    float yDeviation = 0.6f;
    void InitPosition(Transform onePos, Transform towPos, Transform threePos, HeroData[] hero, bool isPlayer)
    {
        if (null == hero) return;
        //上下左右偏差0.6f
        yDeviation = isPlayer ? 0.6f : -0.6f;

        int maxCount = 0;
        int powerCount = 0;
        int intelCount = 0;
        int agileCount = 0;
        for (int i = 0; i < hero.Length; i++)
        {
            if (i == 1 || i == 2 || i == 3 || null == hero[i] || null == hero[i].node) continue;
            maxCount++;
            if (hero[i].node.attribute == 1)
                powerCount++;
            else if (hero[i].node.attribute == 2)
                intelCount++;
            else
                agileCount++;
        }

        if (maxCount == 1)
        {
            return;
        }
        else if (maxCount == 2)
        {
            if (powerCount == 2 || intelCount == 2 || agileCount == 2)
            {
                float zPos = 0;
                if (powerCount != 2) zPos = intelCount == 2 ? -yDeviation * 2 : -yDeviation;
                onePos.localPosition += new Vector3(-xDeviation, 0, zPos);
                towPos.localPosition = onePos.localPosition + new Vector3(xDeviation * 2, 0, 0);
            }
            else
            {
                if (agileCount == 1 && powerCount != 1)
                    onePos.localPosition += new Vector3(0, 0, -yDeviation);
                towPos.localPosition = onePos.localPosition + new Vector3(0, 0, powerCount == 1 ? -yDeviation : -yDeviation * 2);
            }
        }
        else
        {
            if (intelCount == 3 || agileCount == 3)
            {
                onePos.localPosition += new Vector3(-xDeviation, 0, intelCount == 3 ? -yDeviation * 2 : -yDeviation);
                towPos.localPosition = onePos.localPosition + new Vector3(xDeviation, 0, 0);
                threePos.localPosition = onePos.localPosition + new Vector3(xDeviation * 2, 0, 0);
            }
            else if (powerCount == 1 || (agileCount == 1 && powerCount != 2))
            {
                if (agileCount == 1 && powerCount != 1)
                    onePos.localPosition += new Vector3(0, 0, -yDeviation);
                towPos.localPosition = onePos.localPosition + new Vector3(-xDeviation, 0, -yDeviation);
                threePos.localPosition = onePos.localPosition + new Vector3(xDeviation, 0, -yDeviation);
            }
            else if (powerCount >= 2 || agileCount == 2)
            {
                onePos.localPosition += new Vector3(-xDeviation, 0, agileCount == 2 ? -yDeviation : 0);
                towPos.localPosition = onePos.localPosition - new Vector3(-xDeviation * 2, 0, 0);
                threePos.localPosition = onePos.localPosition + new Vector3(xDeviation, 0, -yDeviation);
            }
        }
    }

    void InitPosition(int state, Transform[] tran, int index)
    {
        float _y = 0f;
        if (state == 1)
        {
            _y = tran[2].localPosition.z;
            for (int i = 2; i < tran.Length; i++)
            {
                tran[i].localPosition = new Vector3(tran[i].localPosition.x, tran[i].localPosition.y, tran[0].localPosition.z);
                partnerPos[index] = tran[i];
                index++;
            }
            for (int i = 0; i < 2; i++)
            {
                tran[i].localPosition = new Vector3(tran[i].localPosition.x, tran[i].localPosition.y, _y);
                partnerPos[index] = tran[i];
                index++;
            }
        }
    }

    IEnumerator Restart()
    {
        for (int i = 0; i < agents.size; i++)
        {
            SkillBuffManager.GetInst().ClearBuffsFrom(agents[i]);
            agents[i].DeleteHpAndHud(0f, 0f);
        }
        npcList.Clear();
        heroIcon.Clear();
        ClearAllCs();

        Destroy(CharacterManager.player);
        for (int i = 0; i < partnerPos.Length; i++)
        {
            partnerPos[i].DestroyChildren();
        }
        yield return new WaitForSeconds(0.1f);
        InitScene();
        endBattle = false;
        round++;
    }

    void InitPlayer()
    {
        CreateMainHero();
        player.pm.isAutoMode = true;
        player.CharData.state = Modestatus.NpcPlayer;
        //player.OnDead += (CharacterState cs) => InvokeChangeCamera();
        RecordDamage(player, player, 0);
        player.AddHpBar();
        ArenaPlayer playerAr = new ArenaPlayer();
        playerAr.groupIndx = 1;
        playerAr.playerData = (HeroData)player.CharData;
        playerAr.playerIcon = SceneUIManager.instance.arenaPanel.mIcon[0];
        playerAr.playerIcon.AssignedInfo(player);
        player.OnHit += playerAr.playerIcon.RefreshHPBar;
        damageHeroList.Add(playerAr);
        //for (int i = 0; i < player.CharData.buff_Propers.Length; i++)
        //{
        //    float rato = player.CharData.battleInitAttrs[i] * 0.05f;
        //    player.CharData.buff_Propers[i] += Globe.ArenaIsWin == 1 ? rato : -rato;
        //}
        //player.CharData.buff_Propers[3] *= 5f;
        //player.Hp(0);
    }

    void EnableAIs()
    {
        //player.GetComponent<BasePlayerAI>().enabled = true;
        for (int j = 0; j < agents.size; j++)
        {
            agents[j].GetComponent<BasePlayerAI>().enabled = true;
        }
        //Time.timeScale = 3;
    }

    public void CreateTeam()
    {
        int index = 0;
        for (int i = 0; i < 5; i++)
        {
            index = i;// == 0 ? 0 : i + 3;
            if (null == Globe.challengeTeam[index] || Globe.challengeTeam[index].id == 0) continue;
            ArenaPlayer partner = new ArenaPlayer();
            partner.groupIndx = 1;
            partner.playerData = Globe.challengeTeam[index];
            partner.playerTrans = partnerPos[i];
            partner.playerIcon = SceneUIManager.instance.arenaPanel.mIcon[i];
            npcList.Add(partner);
            damageHeroList.Add(partner);
        }

        int enemyCount = 0;

        for (int i = 0; i < 5; i++)
        {
            if (null == Globe.ArenaEnemy[i] || Globe.ArenaEnemy[i].id == 0) continue;
            enemyCount++;
            ArenaPlayer enemy = new ArenaPlayer();
            enemy.groupIndx = 0;
            enemy.playerData = Globe.ArenaEnemy[i];
            enemy.playerTrans = partnerPos[i + 5];
            enemy.playerIcon = SceneUIManager.instance.arenaPanel.dIcon[i];
            npcList.Add(enemy);
            damageHeroList.Add(enemy);
        }

        List<CharacterState> csList = new List<CharacterState>();

        for (int i = 0; i < npcList.Count; i++)
        {
            if (null == npcList[i].playerData || npcList[i].playerData.id == 0) continue;

            npcList[i].playerData.state = Modestatus.NpcPlayer;
            npcList[i].playerData.groupIndex = npcList[i].groupIndx;

            CharacterState cs = CreateBattleHero(npcList[i].playerData, npcList[i].playerTrans.gameObject);
            heroIcon.Add(cs, npcList[i].playerIcon);
            cs.pm.isAutoMode = true;
            cs.AddHpBar();
            cs.OnHit += npcList[i].playerIcon.RefreshHPBar;
            cs.OnAttackDmg += RecordDamage;
            RecordDamage(cs, cs, 0);
            cs.OnDead += (CharacterState csm) =>
            {
                heroIcon[csm].RefreshHPBar(csm);
            };
            heroCsList.Add(cs);
            npcList[i].playerIcon.AssignedInfo(cs);
            AddHeroAIBySceneType(cs);
            cs.transform.GetComponent<BasePlayerAI>().enabled = false;
            Formula.SetAttrTo(ref cs.CharData.buffAttrs, AttrType.hp, cs.currentHp * 0.5f);
            csList.Add(cs);
            // Debug.LogError(cs.CharData.id + " max: " + cs.maxHp + " server: " + cs.CharData.serverAttrs[3] + " rate: " + cs.maxHp / cs.CharData.serverAttrs[3]);
            //if (cs.CharData.groupIndex == 0)
            //{
            //    for (int j = 0; j < cs.CharData.serverAttrs.Length; j++)
            //    {
            //        cs.CharData.serverAttrs[j] = csList[0].CharData.serverAttrs[j];
            //    }
            //}
            //else
            //{
            //    csList.Add(cs);
            //}

            //if (GameLibrary.isNetworkVersion)
            //{
            //for (int j = 0; j < cs.CharData.buff_Propers.Length; j++)
            //{
            //    float rato = cs.CharData.battleInitAttrs[j] * 0.1f;
            //    if (Globe.ArenaIsWin == 1)
            //        cs.CharData.buff_Propers[j] += cs.CharData.groupIndex == 0 ? -rato : rato;
            //    else
            //        cs.CharData.buff_Propers[j] += cs.CharData.groupIndex == 0 ? rato : -rato;
            //}
            //if (cs.CharData.groupIndex == 1 && Globe.ArenaIsWin == 1)
            //{
            //    cs.CharData.buff_Propers[3] += cs.currentHp * 1.5f;
            //}
            //if (Globe.ArenaIsWin == 1)
            //{
            //    cs.CharData.buff_Propers[3] += cs.currentHp * (cs.CharData.groupIndex == 1 ? 1.5f : 1.0f);
            //}
            //else
            //{
            //    cs.CharData.buff_Propers[3] += cs.currentHp * (cs.CharData.groupIndex == 0 ? 1.5f : 1.0f);
            //}

            //}
            cs.Hp(0);
        }
        //PreLoadCharEffects(csList);
        //for (int i = 0; i < npcList.Count; i++)
        //{
        //			Debug.LogError (npcList [i].playerData.id + "-" + npcList [i].playerData.node.name);
        //    for (int j = 0; j < npcList[i].playerData.battleInitAttrs.Length; j++)
        //    {
        //				Debug.Log((AttrType)j +"-"+npcList[i].playerData.battleInitAttrs[j]);
        //    }
        //}
    }

    bool endBattle;
    bool win;
    bool isEnd;
    void Update()
    {
        if (!endBattle && (friendly.size <= 0 || enemy.size <= 0))
        {
            endBattle = true;
            isEnd = true;
            win = friendly.size <= 0 ? false : true;
            if (TestVersion)
            {
                PrintDamage(true);
                Restart();
                StartCoroutine(Restart());
            }
            else
            {
                CDTimer.GetInstance().RemoveCD(SceneUIManager.instance.arenaPanel.ArenaCD.cd);
                GameOver();
            }
        }
    }

    void GameOver()
    {
        endBattle = true;
        //Time.timeScale = 1;
        DisableComponents();
        StartCoroutine(ShowWin(1.5f, win));
    }

    IEnumerator ShowWin(float time, bool isWin)
    {
        yield return new WaitForSeconds(time);
        if (GameLibrary.isNetworkVersion)
            ClientSendDataMgr.GetSingle().GetBattleSend().SendArenaSettlement(Globe.ArenaPlaterID, isWin ? 1 : -1, "");
        //else
        //  UIShowResult();
    }

    public override void UIShowResult()
    {
        SceneUIManager.instance.HideUI();
        SceneUIManager.instance.arenaWinPanel.RefreshUI(win, damageHeroList);
        AudioController.Instance.PlayBackgroundMusic(win ? "win" : "lose", false);
    }

    void RecordDamage(CharacterState cs, CharacterState attackCs, float damage)
    {
        string id = attackCs.groupIndex + "_" + attackCs.CharData.id.ToString();
        if (damageList.ContainsKey(id))
        {
            damageList[id] += damage;
        }
        else
        {
            damageList.Add(id, damage);
        }

        for (int i = 0; i < damageHeroList.Count; i++)
        {
            if (damageHeroList[i].playerData == attackCs.CharData)
            {
                damageHeroList[i].damage += damage;
                break;
            }
        }

        //if (damageHeroList.ContainsKey(attackCs.CharData))
        //    damageHeroList[attackCs] += damage;
        //else
        //    damageHeroList.Add(attackCs, damage);

    }


    void PrintDamage(bool writeToFile)
    {
        float mDamage = 0;
        float dDamage = 0;
        string mDetails = "";
        string dDetails = "";
        string ret = "";
        foreach (string id in damageList.Keys)
        {
            if (id.StartsWith("1"))
            {
                mDamage += damageList[id];
                mDetails += "|" + id + (win ? "_w_" : "_l_") + damageList[id];
            }
            else
            {
                dDamage += damageList[id];
                dDetails += "|" + id + (win ? "_l_" : "_w_") + damageList[id];
            }
            //Debug.Log(id + "-" + damageList[id]);
        }
        //Debug.Log("mDamage:" + mDamage);
        //Debug.Log("dDamage:" + dDamage);
        mL.Sort();
        string ms = "|" + mL[0] + "_" + mL[1] + "_" + mL[2] + (win ? "_w_" : "_l_") + mDamage;
        dL.Sort();
        string ds = "|" + dL[0] + "_" + dL[1] + "_" + dL[2] + (win ? "_l_" : "_w_") + dDamage;
        timeEclipse = Time.realtimeSinceStartup - timeEclipse;
        ret = "time:" + timeEclipse.ToString("0.0") + ms + ds + mDetails + dDetails;

        mL.Clear();
        dL.Clear();
        damageList.Clear();
        if (writeToFile)
            createORwriteConfigFile("D:", "arenaRecord.txt", ret);
        else
            Debug.Log(ret);
    }

    private void createORwriteConfigFile(string path, string name, string info)
    {
        StreamWriter sw;
        FileInfo t = new FileInfo(path + "//" + name);
        if (!t.Exists)
        {
            sw = t.CreateText();
        }
        else
        {
            sw = t.AppendText();
        }
        sw.WriteLine(info);
        sw.Close();
        sw.Dispose();
    }

    protected override void OnSceneDestroy()
    {
        base.OnSceneDestroy();
        // Globe.UseServerBattleAttrs = false;
        GameLibrary.isPVP3 = false;
        ClearHeroData();
    }

    void ClearHeroData()
    {
        for (int i = 0; i < heroCsList.Count; i++)
        {
            heroCsList[i].CharData.buffAttrs = new float[Formula.ATTR_COUNT];
        }
    }
}

public class ArenaPlayer
{
    public UInt32 groupIndx;
    public HeroData playerData;
    public Transform playerTrans;
    public TeamIcon playerIcon;
    public float damage;
}

/*
 a1 普通攻击
 a2
 a3
 s1 技能攻击
 s2
 s3
 s4
 r (0,0,0)
 s 站立
 t 攻击目标
 */
