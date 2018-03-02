using UnityEngine;
using Tianyu;
using System.Collections.Generic;
using System.Collections;
using System;

public class SceneMobaBase : SceneBaseManager
{
    protected List<Tower_AI> TowerList = new List<Tower_AI>();
    float ratio = 0.05f;
    //int rebornTime = 12;
    int RedKillCount;
    int BlueKillCount;

    List<HeroData> heroDatas = new List<HeroData>();
    public bool isDungeons = false;

    Dictionary<CharacterData, Dictionary<CharacterData, float>> AttackedHeroRecords = new Dictionary<CharacterData, Dictionary<CharacterData, float>>();
    float maxCheckTime = 10f;

    protected void HandleKillInfo ( CharacterState cs )
    {
        CharacterData charData = cs.CharData;
        CharacterData killerData = GetKiller(charData);
        if(killerData != null)
        {
            killerData.mobaMorale += 5;
            AddMoraleAttrs(killerData, 5);
            killerData.mobaKillCount++;
            killerData.mobaSerialKillCount++;
            if (killerData.mobaMultiKillCD != null)
                CDTimer.GetInstance().RemoveCD(killerData.mobaMultiKillCD);
            killerData.mobaMultiKillCount++;
            killerData.mobaMultiKillCD = CDTimer.GetInstance().AddCD(5f, (int c, long id) => killerData.mobaMultiKillCount = 0);
            killerData.mobaMultiKillCD.OnRemove += (int c, long id) => killerData.mobaMultiKillCD = null;
        }
        else
        {
            AddKoInfo(Localization.Get("MobaKillCount1"),"jisha", charData, cs.LastAttackBy.CharData, new List<CharacterData>());
        }
        List<CharacterData> aidDatas = GetAids(charData);
        if(aidDatas.Contains(killerData))
        {
            aidDatas.Remove(killerData);
        }
        for(int i = 0; i < aidDatas.Count; i++)
        {
            aidDatas[i].mobaAidCount++;
            aidDatas[i].mobaMorale += 2;
            AddMoraleAttrs(aidDatas[i], 2);
        }

        if(killerData != null)
        {
            if(killerData.mobaMultiKillCount > 1)
            {
                string preStr = killerData.mobaMultiKillCount == 2 ? "MobaKillNum" : "ChineseNum";
                string info = string.Format(Localization.Get("MobaMultiKill"), Localization.Get(preStr + killerData.mobaMultiKillCount));
                AddKoInfo(info, GetMultiKillSpName(killerData.mobaMultiKillCount), charData, killerData, aidDatas, MobaKOInfoType.multiKill);
            }
            else
            {
                if(BlueKillCount + RedKillCount == 0)
                    AddKoInfo(Localization.Get("MobaFirstKill"), "yixue", charData, killerData, aidDatas, MobaKOInfoType.firstKill);
                else
                    AddKoInfo(GetSerialKillDesc(killerData.mobaSerialKillCount), GetSerialKillSpName(killerData.mobaSerialKillCount), charData, killerData, aidDatas);
                if(killerData.mobaSerialKillCount == 7)
                    AddKoInfo(Localization.Get("MobaKillGodLike"), "chaoshen", charData, killerData, aidDatas, MobaKOInfoType.chaoshenKill);
                if(charData.mobaSerialKillCount > 2)
                    AddKoInfo(Localization.Get("MobaEndKill"), "zhongjie", charData, killerData, aidDatas, MobaKOInfoType.endKill);
            }
        }
        if (sceneType != SceneType.MB1 && sceneType != SceneType.Dungeons_MB1)
            CheckIfAllDead(cs);

        charData.mobaDeathCount++;
        if(charData.groupIndex == 0)
            BlueKillCount++;
        else
            RedKillCount++;
        charData.mobaSerialKillCount = 0;
    }

    MobaKOInfo AddKoInfo (string desc, string spName, CharacterData dead, CharacterData killer, List<CharacterData> aids, MobaKOInfoType type = MobaKOInfoType.serialKill)
    {
        MobaKOInfo koInfo = new MobaKOInfo();
        koInfo.type = type;
        koInfo.ShowAid = aids.Count > 0;
        koInfo.killDesc = desc;
        koInfo.killSpName = spName;
        koInfo.decedent = dead;
        koInfo.killer = killer;
        koInfo.aids = aids;
        koInfo.SerialKillCount = killer.mobaSerialKillCount;
        SceneUIManager.instance.MobaKoInfo.KoInfosToPlay.Add(koInfo);
        return koInfo;
    }

    string GetSerialKillSpName ( int serialKillCount = 0 )
    {
        if(serialKillCount == 3)
            return "dasha";
        else if(serialKillCount == 4)
            return "sharem";
        else if(serialKillCount == 5)
            return "wuren";
        else if(serialKillCount == 6)
            return "hengsao";
        else if(serialKillCount >= 7)
            return "tianxia";
        else
            return "jisha";
    }

    string GetMultiKillSpName ( int multiKillCount = 0 )
    {
        if(multiKillCount == 2)
            return "shuangsha";
        else if(multiKillCount == 3)
            return "sansha";
        else if(multiKillCount == 4)
            return "sisha";
        else if(multiKillCount == 5)
            return "wusha";
        else
            return "jisha";
    }

    string GetSerialKillDesc (int multiKillCount = 0)
    {
        if(multiKillCount < 3)
            return Localization.Get("MobaKillCount1");
        else if(multiKillCount == 3)
            return Localization.Get("MobaKillCount3");
        else if(multiKillCount == 4)
            return Localization.Get("MobaKillCount4");
        else if(multiKillCount == 5)
            return Localization.Get("MobaKillCount5");
        else if(multiKillCount == 6)
            return Localization.Get("MobaKillCount6");
        else if(multiKillCount>=7)
            return Localization.Get("MobaKillCount7");
        else
            return "";
    }

    protected void RefreshAttackedHeroRecords ( CharacterData hit, CharacterData attacker )
    {
        if(AttackedHeroRecords.ContainsKey(hit))
        {
            if(AttackedHeroRecords[hit].ContainsKey(attacker))
            {
                AttackedHeroRecords[hit][attacker] = maxCheckTime;
            }
            else
            {
                AttackedHeroRecords[hit].Add(attacker, maxCheckTime);
            }
        }
        else
        {
            Dictionary<CharacterData, float> attackerDict = new Dictionary<CharacterData, float>();
            attackerDict.Add(attacker, maxCheckTime);
            AttackedHeroRecords.Add(hit, attackerDict);
        }
    }

    void Update ()
    {
        List<CharacterData> keyList = new List<CharacterData>(AttackedHeroRecords.Keys);
        for(int i = 0; i<keyList.Count; i++)
        {
            List<CharacterData> attackerKeys = new List<CharacterData>(AttackedHeroRecords[keyList[i]].Keys);
            for(int j = 0; j< attackerKeys.Count; j++)
            {
                AttackedHeroRecords[keyList[i]][attackerKeys[j]] -= Time.deltaTime;
            }
        }
    }

    CharacterData GetKiller ( CharacterData charData )
    {
        CharacterData killer = null;
        float latestAttackTime = 0f;
        if(AttackedHeroRecords.ContainsKey(charData))
        {
            foreach(CharacterData kcd in AttackedHeroRecords[charData].Keys)
            {
                if(AttackedHeroRecords[charData][kcd] > latestAttackTime)
                {
                    latestAttackTime = AttackedHeroRecords[charData][kcd];
                    killer = kcd;
                }
            }
        }
        return killer;
    }

    List<CharacterData> GetAids ( CharacterData charData )
    {
        List<CharacterData> aids = new List<CharacterData>();
        if(AttackedHeroRecords.ContainsKey(charData))
        {
            foreach(CharacterData kcd in AttackedHeroRecords[charData].Keys)
            {
                if(AttackedHeroRecords[charData][kcd] > 0f)
                {
                    aids.Add(kcd);
                }
            }
        }
        return aids;
    }

    protected void InitTower (CharacterState towerCS, UInt32 id, UInt32 groupIndex, Modestatus state, int lv = 1)
    {
        MonsterData towerData = new MonsterData(id, lv);
        towerData.groupIndex = groupIndex;
        towerData.state = Modestatus.Tower;
        InitTowerInfo(towerCS, towerData);
    }

    protected void InitTower(CharacterState towerCS, Modestatus state, LevelConfigBase config)
    {
        MonsterData towerData = new MonsterData(config.modelID, (int)config.modellvl);
        towerData.groupIndex = (UInt32)config.groupIndex;
        towerData.lvlRate = config.modellvl;
        towerData.state = state;
        InitTowerInfo(towerCS, towerData);
    }

    void InitTowerInfo(CharacterState towerCS, MonsterData towerData)
    {
        towerCS.InitData(towerData);
        towerCS.OnDead += ChangeMorale;
        towerCS.OnDead += CheckIfHero;
        MobaMiniMap.instance.AddMapIconByType(towerCS);
        AddCs(towerCS);
        Tower_AI towerAI = towerCS.GetComponent<Tower_AI>();
        towerAI.InitTowerAI();
        TowerList.Add(towerAI);
        InitMonsterHpAndAttack(towerCS);
        towerCS.gameObject.AddComponent<TowerState>();
        //GameObject cylinder = CreatCylinder(towerCS.gameObject);
        //TrigerTest tt = cylinder.GetComponent<TrigerTest>();
        //if(tt.instance!=null)
        //tt.instance.GroupIndex = groupIndex;
    }

    void CheckIfHero (CharacterState cs)
    {
        if(cs.LastAttackBy != null && BattleUtil.IsHeroTarget(cs.LastAttackBy))
        {
            AddKoInfo(Localization.Get("MobaTowerKill"), "cuihui", cs.CharData, cs.LastAttackBy.CharData, new List<CharacterData>(), MobaKOInfoType.towerKill);
        }
    }

    void CheckIfAllDead ( CharacterState cs )
    {
        CharacterState[] MyHeros = Array.FindAll(agents.ToArray(), ( CharacterState c ) => BattleUtil.IsHeroTarget(c) && c.groupIndex == cs.groupIndex);
        if(MyHeros.Length == 0 || (MyHeros.Length == 1 && MyHeros[0] == cs))
        {
            AddKoInfo(Localization.Get("MobaAllKill"), "tuanmie", cs.CharData, cs.LastAttackBy.CharData, new List<CharacterData>(), MobaKOInfoType.allKill);
        }
    }

    GameObject CreatCylinder(GameObject parent)
    {
        GameObject go = Instantiate(Resources.Load("Prefab/Character/CylinderTower")) as GameObject;
        go.transform.parent = parent.transform;
        go.transform.localPosition = Vector3.zero;
        go.AddComponent<TrigerTest>();
        return go;
    }

    float hpRatio;
    float attackRatio;
    float lvRatio;
    protected void InitHpAndAttackRatio ()
    {
        CharacterState[] Heros = Array.FindAll(agents.ToArray(), ( CharacterState c ) => BattleUtil.IsHeroTarget(c));
        for(int i = 0; i<Heros.Length; i++)
        {
            hpRatio += 1f * Heros[i].maxHp / Heros.Length;
            attackRatio += 0.1f * Formula.GetSingleAttribute(Heros[i].CharData, AttrType.attack) / Heros.Length;
            lvRatio += 1f * Heros[i].CharData.lvl / Heros.Length;
        }
    }

    protected void InitMonsterHpAndAttack(CharacterState cs)
    {
        if (isDungeons)
        {
            InitTowerAIAttack(cs, Formula.GetSingleAttribute(cs.CharData, AttrType.attack));
            return;
        }
        cs.CharData.lvl = Mathf.FloorToInt(lvRatio);
        float baseAtk = hpRatio * cs.CharData.attrNode.attack + lvRatio * ((MonsterAttrNode)cs.CharData.attrNode).lv_attack;
        float mobaMaxHp = Mathf.FloorToInt(attackRatio * cs.CharData.attrNode.hp + lvRatio * ((MonsterAttrNode)cs.CharData.attrNode).lv_hp);
        Formula.SetAttrTo(ref cs.CharData.attrNode.base_Propers, AttrType.attack, baseAtk);
        Formula.SetAttrTo(ref cs.CharData.attrNode.base_Propers, AttrType.hp, mobaMaxHp);
        InitTowerAIAttack(cs, baseAtk);
    }

    void InitTowerAIAttack ( CharacterState cs, float atk )
    {
        if(cs.state == Modestatus.Tower)
        {
            Tower_AI tAI = cs.GetComponent<Tower_AI>();
            tAI.baseAttack = atk;
            cs.Hp(0);
            cs.AddHpBar();
        }
    }

    public virtual void ChangeMorale (CharacterState cs)
    {
        //if (isDungeons)
        //    return;
        switch (cs.state)
        {
            case Modestatus.Monster:
                AddMorale(cs, 1);
                break;
            //case Modestatus.Player:
            //case Modestatus.NpcPlayer:
            //    AddMorale(cs, 5);
            //    break;
            case Modestatus.Tower:
                AddMorale(cs, 10);
                break;
        }
    }

   public void AddMorale (CharacterState cs, int val)
    {
        if(cs.LastAttackBy == null)
            return;
        CharacterState killer = null;
        if(BattleUtil.IsHeroTarget(cs.LastAttackBy))
            killer = cs.LastAttackBy;
        else if(cs.LastAttackBy.state == Modestatus.SummonHero)
            killer = cs.LastAttackBy.Master;

        if(killer != null)
        {
            killer.CharData.mobaMorale += val;
            AddSaveAttrsAddRefreshHpBar(killer, val);
            // Debug.LogError(killer.CharData.id + " kill " + cs.CharData.id + " changeTo " + killer.CharData.mobaMorale);
        }

        if(cs.state == Modestatus.Tower)
        {
            for(int i = 0; i<agents.size; i++)
            {
                if(BattleUtil.IsHeroTarget(agents[i]) && agents[i].groupIndex != cs.groupIndex)
                {
                    agents[i].CharData.mobaMorale += val;
                    AddSaveAttrsAddRefreshHpBar(agents[i], val);
                }
            }
        }
        RefreshInfo();
    }

    protected void AddSaveAttrsAddRefreshHpBar (CharacterState cs, int addVal)
    {
        if(cs != null && !cs.isDie)
        {
            AddMoraleAttrs(cs.CharData, addVal);
            cs.Hp(0);
        }
    }

    void AddMoraleAttrs ( CharacterData charData, int addVal)
    {
        AddAttr(charData, addVal, AttrType.power);
        AddAttr(charData, addVal, AttrType.intelligence);
        AddAttr(charData, addVal, AttrType.agility);
    }

    void AddAttr ( CharacterData charData, int addVal, AttrType attrType )
    {
        float add = ratio * addVal * charData.battleInitAttrs[(int)attrType];
        Formula.AddAttrWith(ref charData.buffAttrs, attrType, add);
    }

    protected void ClearCsData (CharacterData[] cds)
    {
        for(int i = 0; i < cds.Length; i++)
        {
            if(cds[i] != null)
            {
                cds[i].buffAttrs = new float[Formula.ATTR_COUNT];
                cds[i].mobaMorale = 0;
                cds[i].mobaKillCount = 0;
                cds[i].mobaAidCount = 0;
                cds[i].mobaDeathCount = 0;
            }
        }
    }

    protected virtual void RefreshInfo () {
    }

    protected void TowerHelp ( CharacterState cs , CharacterState attackerCs, MobaAIPlayerPriority prior)
    {
        if (cs!=null && attackerCs!=null)
        {
            for (int i = 0; i < TowerList.Count; i++)
            {
                if (TowerList[i] != null
                    && !TowerList[i].cs.isDie
                    && TowerList[i].cs.groupIndex == cs.groupIndex
                    && BattleUtil.ReachPos(TowerList[i].cs, cs, TowerList[i].cs.AttackRange)
                    && BattleUtil.ReachPos(TowerList[i].cs, attackerCs, TowerList[i].cs.AttackRange)
                    && TowerList[i].enemyHeroAttackingMyHero == null)
                {
                    TowerList[i].enemyHeroAttackingMyHero = attackerCs;
                }
            }
        }
       
    }

    protected void OverResult()
    {
        SceneUIManager.instance.pnReborn.HideRevornCD();
        FightTouch._instance.mobaStatic.StopCD();
        //SceneUIManager.instance.SwitchBloodScreen(false);
    }

}
