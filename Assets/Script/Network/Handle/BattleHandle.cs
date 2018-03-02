using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;
using System.Collections.Generic;
using Tianyu;

public class CBattleHandle : CHandleBase
{

    public CBattleHandle(CHandleMgr mgr) : base(mgr)
    {

    }

    public override void RegistAllHandle()
    {
        //角斗场
        RegistHandle(MessageID.pve_query_arena_list_ret, GetQueryArenaListResultHandle);
        RegistHandle(MessageID.pve_init_arena_fight_ret, GetInitArenaFightResultHandle);
        RegistHandle(MessageID.pve_start_arena_fight_ret, GetStarArenaFightResultHandle);
        RegistHandle(MessageID.pve_arena_settlement_ret, GetArenaSettlementResultHandle);
        RegistHandle(MessageID.pve_arena_reload_cd_ret, GetArenaReloadCDResultHandle);

        //副本
        RegistHandle(MessageID.pve_worldmap_list_ret, GetQueryWorldMapResultHandle);
        RegistHandle(MessageID.pve_dungeon_list_ret, GetQueryDungeonListResultHandle);
        RegistHandle(MessageID.pve_into_dungeon_ret, GetIntoDungeonResultHandle);
        RegistHandle(MessageID.pve_start_dungeon_fight_ret, GetStartFightResultHandle);
        RegistHandle(MessageID.pve_dungeon_settlement_ret, GetFightSettlementResultHandle);
        RegistHandle(MessageID.pve_draw_dungeon_box_reward_ret, GetDrawDungeonBoxRewardResultHandle);
        RegistHandle(MessageID.pve_flash_dungeon_ret, GetFlashDungeonFightResultHandle);
        RegistHandle(MessageID.pve_reset_elite_dungeon_ret, GetResetEliteDungeonResultHandle);
        RegistHandle(MessageID.common_buy_someone_ret, GetCommonBuySomeoneResultHandle);

        //活动
        RegistHandle(MessageID.pve_eventdungeon_list_ret, GetQueryEventListResultHandle);
        RegistHandle(MessageID.pve_into_eventdungeon_ret, GetIntoEventDungeonResultHandle);
        RegistHandle(MessageID.pve_start_eventdungeon_ret, GetStartEventFightResultHandle);
        RegistHandle(MessageID.pve_eventdungeon_settlement_ret, GetEventFightSettlementResultHandle);
        RegistHandle(MessageID.pve_eventdungeon_flash_ret, GetEventFlashDungeonFightResultHandle);

        RegistHandle(MessageID.common_specified_hero_attrib_ret, GetHerosAttrResultHandle);
        //新手指引
        //    RegistHandle(MessageID.c_player_guide_info_ret, GuidInfoRet);
        //    RegistHandle(MessageID.c_player_manipulate_specified_UI, OpenUI);

        //pvp
        RegistHandle(MessageID.pvp_application_fight_ret, PvPReq);
        RegistHandle(MessageID.pve_init_moba_fight_ret, PvERet);
        RegistHandle(MessageID.player_relay_ret, PlayerRelay);
    }
    bool PlayerRelay(CReadPacket packet)
    {
        return true;
    }
    bool PvERet(CReadPacket packet)
    {
        if (1 == Singleton<SceneManage>.Instance.mobaltype)
        {
            //UI_Loading.LoadScene (GameLibrary.PVP_Moba, 3);
            GameLibrary.LastScene = SceneManager.GetActiveScene().name;//记录前一个场景名
            StartLandingShuJu.GetInstance().GetLoadingData(GameLibrary.PVP_Moba, 3);
            SceneManager.LoadScene("Loding");
        }
        else if (3 == Singleton<SceneManage>.Instance.mobaltype)
        {
            //UI_Loading.LoadScene (GameLibrary.PVP_Moba3v3, 3);
            GameLibrary.LastScene = SceneManager.GetActiveScene().name;//记录前一个场景名
            StartLandingShuJu.GetInstance().GetLoadingData(GameLibrary.PVP_Moba3v3, 3);
            SceneManager.LoadScene("Loding");
        }
        playerData.GetInstance().selfData.SetPos(CharacterManager.player.transform.localPosition);
        return true;
    }
    bool GetHerosAttrResultHandle(CReadPacket packet)
    {
        Dictionary<string, object> data = packet.data;
        int resolt = int.Parse(data["ret"].ToString());
        if (resolt == 0)
        {
            Dictionary<string, object> heroDicts = (Dictionary<string, object>)data["mh"];
            foreach (string k in heroDicts.Keys)
            {
                Dictionary<string, object> heroDict = (Dictionary<string, object>)heroDicts[k];
                long heroId = long.Parse(heroDict["id"].ToString());
                object[] attrs = (object[])heroDict["attr"];
                HeroData heroData = playerData.GetInstance().GetHeroDataByID(heroId);
                for (int j = 0; j < attrs.Length; j++)
                {
                    Formula.SetAttrTo(ref heroData.serverAttrs, (AttrType)j, float.Parse(attrs[j].ToString()));
                }
            }
            if (data.ContainsKey("rk"))
            {
                playerData.GetInstance().selfData.rank = Convert.ToInt32(data["rk"]);
            }
            return true;
        }
        return false;
    }

    private bool PvPReq(CReadPacket packet)
    {
        // Debug.Log("OpenUi");
        Dictionary<string, object> data = packet.data;

        return true;
    }
    private bool OpenUI(CReadPacket packet)
    {
        // Debug.Log("OpenUi");
        Dictionary<string, object> data = packet.data;
        byte ad = packet.ReadByte("ad");//窗口操作类型，0关闭，1打开
        byte ui = packet.ReadByte("ui");//面板id
        //if (ad == 1)
        //    Control.ShowGUI(GameLibrary.UIShop);

        //else
        //{
        //    Control.HideGUI(GameLibrary.UIShop);
        //    ClientSendDataMgr.GetSingle().GetBattleSend().SendGuidStep();

        //}
        //--"td" = typeId;
        //--"sp"= stepId;
        //--wd =widgetId


        return true;
    }
    private bool GuidInfoRet(CReadPacket packet)
    {
        // Debug.Log("GuidInfoRet");
        Dictionary<string, object> data = packet.data;
        GameLibrary.scripid = packet.GetLong("sd");
        GameLibrary.typeid = packet.GetShort("td");
        GameLibrary.stepid = packet.GetShort("sp");
        //--"sd"= scriptId;
        //--"td" = typeId;
        //--"sp"= stepId;
        //--wd =widgetId


        return true;
    }
    //角斗场
    private bool GetQueryArenaListResultHandle(CReadPacket packet)
    {
        // Debug.Log("Get Query Arena List result");

        Dictionary<string, object> data = packet.data;

        int resolt = int.Parse(data["ret"].ToString());

        if (resolt == 0)
        {
            if (data.ContainsKey("rk"))
            {
                playerData.GetInstance().selfData.rank = Convert.ToInt32(data["rk"]);
            }

            Dictionary<string, object> item = data["item"] as Dictionary<string, object>;
            object[] herodata1 = new object[0];
            object[] herodata2 = new object[0];
            object[] herodata3 = new object[0];
            //if (item.ContainsKey("1"))
            //    herodata1 = (object[])item["1"];
            //if (item.ContainsKey("2"))
            //    herodata2 = (object[])item["2"];
            //if (item.ContainsKey("3"))
            //    herodata3 = (object[])item["3"];
            if (item.ContainsKey("1"))
                SetItemData(ref herodata1, item["1"]);
            if (item.ContainsKey("2"))
                SetItemData(ref herodata2, item["2"]);
            if (item.ContainsKey("3"))
                SetItemData(ref herodata3, item["3"]);

            Globe.arenaHero.Clear();
            List<ArenaHero> hero;

            Globe.ArenaRkisOne = herodata1.Length <= 0;

            for (int i = 0; i < herodata1.Length; i++)
            {
                hero = new List<ArenaHero>();
                InitArenaData(herodata1[i], hero);
                if (herodata2.Length > i)
                    InitArenaData(herodata2[i], hero);
                if (herodata3.Length > i)
                    InitArenaData(herodata3[i], hero);
                Globe.arenaHero.Add(hero);
            }

            if (data.ContainsKey("bht"))
            {
                SaveArenaInfo(data["bht"].ToString());
            }

        }
        else
        {
            Debug.LogError(string.Format("获取角斗场列表错误：{0}", data["desc"].ToString()));
        }

        return true;

    }
    void InitArenaData(object obj, List<ArenaHero> list)
    {
        Dictionary<string, object> herodata = obj as Dictionary<string, object>;
        ArenaHero hero = new ArenaHero();
        Dictionary<string, object> heroInfo = herodata["hl"] as Dictionary<string, object>;
        Dictionary<string, object> heroData;

        HeroData hd = null;
        for (int i = 1; i <= heroInfo.Count; i++)
        {
            if (i != heroInfo.Count)
            {
                heroData = heroInfo[i.ToString()] as Dictionary<string, object>;
                hd = null;
                if (heroData.Count > 0)
                {
                    hd = new HeroData(Convert.ToInt32(heroData["id"]), Convert.ToInt32(heroData["lvl"]), Convert.ToInt32(heroData["gd"]), Convert.ToInt32(heroData["star"]));
                    hd.fc = Convert.ToInt32(heroData["fc"]);
                }
                hero.herolist[i - 1] = hd;
            }
            else
            {
                hero.heroState = Convert.ToInt32(heroInfo[i.ToString()]);
            }
        }

        hero.pid = Convert.ToInt32(herodata["pid"]);
        hero.nm = Convert.ToString(herodata["nm"]);
        hero.rk = Convert.ToInt32(herodata["rk"]);
        hero.lvl = Convert.ToInt32(herodata["lvl"]);
        hero.fc = Convert.ToInt32(herodata["fc"]);

        list.Add(hero);
    }
    void SetItemData(ref object[] herodata, object item)
    {
        if (item is object[])
        {
            object[] hero = item as object[];
            if (hero.Length > 0)
            {
                herodata = (object[])item;
            }
        }
    }

    void SaveArenaInfo(string timeInfo)
    {
        GameLibrary.ArenaNumber = new object[4];
        string arenaNumber = timeInfo;
        string ymd = arenaNumber.Substring(0, 6);
        string time = arenaNumber.Substring(6, 6);
        string buyCount = arenaNumber.Substring(12, 2);
        string dareCount = arenaNumber.Substring(14, 1);

        int currentyMd = TimeManager.Instance.CheckTimeIsNowadays(ymd, true, true);
        if (currentyMd > 0)
        {
            GameLibrary.ArenaNumber[0] = TimeManager.Instance.ConvertIntDateTime(Auxiliary.GetNowTime()).ToString("yyMMdd");
            GameLibrary.ArenaNumber[1] = "000000";
            GameLibrary.ArenaNumber[2] = "00";
            GameLibrary.ArenaNumber[3] = "5";
        }
        else
        {
            GameLibrary.ArenaNumber[0] = ymd;
            GameLibrary.ArenaNumber[1] = time;
            GameLibrary.ArenaNumber[2] = buyCount;
            GameLibrary.ArenaNumber[3] = dareCount;
        }
    }

    private bool GetInitArenaFightResultHandle(CReadPacket packet)
    {

        //Debug.Log("Get Init Arena Fight result");

        Dictionary<string, object> data = packet.data;

        int resolt = int.Parse(data["ret"].ToString());

        if (resolt == 0)
        {
            Dictionary<string, object> dh = data["dh"] as Dictionary<string, object>;

            if (null != dh)
            {
                HeroData hd = null;
                int index = 1;
                for (int i = 0; i < dh.Count - 1; i++)
                {
                    if (dh.ContainsKey("" + index))
                    {
                        hd = new HeroData(dh["" + index]);
                    }
                    Globe.ArenaEnemy[i] = hd;
                    index++;
                }
            }

            Globe.ArenaState = Convert.ToInt32(dh["6"]);

            Dictionary<string, object> mh = data["mh"] as Dictionary<string, object>;

            if (null != mh)
            {
                HeroData hd = null;
                int index = 1;
                for (int i = 0; i < mh.Count; i++)
                {
                    if (mh.ContainsKey("" + index) && i != mh.Count - 1)
                    {
                        hd = new HeroData(mh["" + index]);
                        playerData.GetInstance().ReplaceHeroData(hd.id, mh["" + index]);
                    }
                    //Globe.challengeTeam[i] = hd;
                    Globe.arenaFightHero[i] = i == mh.Count - 1 ? Convert.ToInt32(mh["" + index]) : (int)hd.id;
                    index++;
                }
            }

            if (data.ContainsKey("bht"))
                SaveArenaInfo(data["bht"].ToString());

            if (data.ContainsKey("destId"))
                Globe.ArenaPlaterID = Convert.ToInt32(data["destId"]);

            Singleton<SceneManage>.Instance.Current = EnumSceneID.Dungeons;

            Globe.backPanelParameter = new object[] { UIPanleID.UIPvP, 1 };
            Control.HideGUI(true);
            playerData.GetInstance().selfData.SetPos(CharacterManager.player.transform.localPosition);
            GameLibrary.LastScene = SceneManager.GetActiveScene().name;//记录前一个场景名
            //Control.HideGUI();
            Control.AddOrDeletFullScreenUI(UIPanleID.UIEmbattle, false);
            StartLandingShuJu.GetInstance().GetLoadingData(GameLibrary.PVP_Zuidui, 3);
            SceneManager.LoadScene("Loding");
        }
        else
        {
            HeroPosEmbattle.instance.HideModel();
            Control.HideGUI();
            UIAbattiorList.instance.DareLater(data["desc"].ToString());

            Debug.Log(string.Format("初始化角斗场战斗错误：{0}", data["desc"].ToString()));
        }

        return true;

    }

    private bool GetStarArenaFightResultHandle(CReadPacket packet)
    {

        // Debug.Log("Get Star Arena Fight result");

        Dictionary<string, object> data = packet.data;

        int resolt = int.Parse(data["ret"].ToString());

        if (resolt == 0)
        {



        }
        else
        {
            Debug.Log(string.Format("开始角斗场战斗错误：{0}", data["desc"].ToString()));
        }

        return true;

    }

    private bool GetArenaSettlementResultHandle(CReadPacket packet)
    {

        // Debug.Log("Get Arena Settlement result");

        Dictionary<string, object> data = packet.data;

        int resolt = int.Parse(data["ret"].ToString());

        if (resolt == 0)
        {

            if (data.ContainsKey("mrk"))
                Globe.mrk = Convert.ToInt32(data["mrk"]);

            if (data.ContainsKey("drk"))
                Globe.drk = Convert.ToInt32(data["drk"]);

            if (data.ContainsKey("dPhoto"))
                Globe.dPhoto = Convert.ToInt32(data["dPhoto"]);

            if (data.ContainsKey("maxRk"))
                Globe.maxRk = Convert.ToInt32(data["maxRk"]);

            if (data.ContainsKey("diamond"))
                Globe.diamond = Convert.ToInt32(data["diamond"]);

            SceneBaseManager.instance.UIShowResult();

        }
        else
        {
            Debug.Log(string.Format("角斗场战斗结算错误：{0}", data["desc"].ToString()));
        }

        return true;

    }

    private bool GetArenaReloadCDResultHandle(CReadPacket packet)
    {

        // Debug.Log("Get Arena Reload result");

        Dictionary<string, object> data = packet.data;

        int resolt = int.Parse(data["ret"].ToString());

        if (resolt == 0)
        {

        }
        else
        {
            Debug.Log(string.Format("角斗场CD时间或次数重置错误：{0}", data["desc"].ToString()));
        }

        return true;

    }

    //副本
    private bool GetQueryWorldMapResultHandle(CReadPacket packet)
    {

        // Debug.Log("Get Query World Map result");

        Dictionary<string, object> data = packet.data;

        int resolt = int.Parse(data["ret"].ToString());

        if (resolt == 0)
        {
            string itemS = data["item"].ToString();

            itemS = itemS.Replace("[", "");
            itemS = itemS.Replace("]", "");

            string[] itemStr = itemS.Split(',');

            if (null != itemStr && itemStr.Length > 0)
            {
                for (int i = 0; i < itemStr.Length; i++)
                {
                    if (!playerData.GetInstance().worldMap.Contains(int.Parse(itemStr[i].ToString())))
                        playerData.GetInstance().worldMap.Add(int.Parse(itemStr[i].ToString()));
                    if (i == 0 && !playerData.GetInstance().CanEnterMap.Contains(int.Parse(itemStr[i].ToString())))
                        playerData.GetInstance().CanEnterMap.Add(int.Parse(itemStr[i].ToString()));
                }

                //ClientSendDataMgr.GetSingle().GetBattleSend().SendQueryDungeonList(playerData.GetInstance().worldMap, 1);
                //ClientSendDataMgr.GetSingle().GetBattleSend().SendQueryDungeonList(playerData.GetInstance().worldMap, 2);
            }
        }
        else
        {
            Debug.Log(string.Format("获取世界列表错误：{0}", data["desc"].ToString()));
        }

        return true;

    }

    void UpdateNormalDungeon(Dictionary<string, object> data, int[] mapID)
    {

        if (null != mapID)
        {
            Dictionary<string, object> iteminfo;
            Dictionary<string, object> item;

            for (int i = 0; i < mapID.Length; i++)
            {
                iteminfo = data[mapID[i].ToString()] as Dictionary<string, object>;
                item = iteminfo["item"] as Dictionary<string, object>;
                UpdateStar(item, mapID[i]);
                UpdateBox(iteminfo["box"], mapID[i], 1);
            }

        }
    }
    public void UpDataEliteMap(Dictionary<string, object> data, int[] mapID)
    {
        if (null != mapID)
        {
            Dictionary<string, object> iteminfo;
            Dictionary<string, object> item;
            for (int i = 0; i < mapID.Length; i++)
            {
                iteminfo = data[mapID[i].ToString()] as Dictionary<string, object>;
                Dictionary<string, object> times = iteminfo["times"] as Dictionary<string, object>;
                item = iteminfo["item"] as Dictionary<string, object>;
                if (null != times)
                {
                    foreach (string key in times.Keys)
                    {
                        int[] campaignInfo = new int[3];
                        string keys = times[key].ToString();
                        string yearmonthday = keys.Substring(0, 6);
                        string buyCountKeys = keys.Substring(keys.Length - 3);
                        int buyCount = 0;
                        int campaign = 0;
                        int ymd = 0;
                        if (Convert.ToInt32(TimeManager.Instance.ConvertIntDateTime(Auxiliary.GetNowTime()).ToString("yyMMdd")) > Convert.ToInt32(yearmonthday))
                        {
                            buyCount = 0;
                            campaign = 3;
                            ymd = Convert.ToInt32(TimeManager.Instance.ConvertIntDateTime(Auxiliary.GetNowTime()).ToString("yyMMdd"));
                        }
                        else
                        {
                            buyCount = Convert.ToInt32(buyCountKeys.Substring(0, buyCountKeys.Length - 1));
                            campaign = Convert.ToInt32(keys.Substring(keys.Length - 1));
                            ymd = Convert.ToInt32(yearmonthday);
                        }
                        campaignInfo[0] = campaign;
                        campaignInfo[1] = buyCount;
                        campaignInfo[2] = ymd;
                        if (GameLibrary.mapElite.ContainsKey(Convert.ToInt32(key)))
                            GameLibrary.mapElite[Convert.ToInt32(key)] = campaignInfo;
                        else
                            GameLibrary.mapElite.Add(Convert.ToInt32(key), campaignInfo);
                    }
                }
                UpdateStar(item, mapID[i]);
                UpdateBox(iteminfo["box"], mapID[i], 2);
            }
        }
    }
    void UpdateMap(Dictionary<string, object> data, int[] mapID, ref bool isSetNextBtn)
    {
        int types = int.Parse(data["types"].ToString());

        if (types == 1)//普通副本数据读取
        {
            UpdateNormalDungeon(data, mapID);
        }
        else if (types == 2)
        {
            UpDataEliteMap(data, mapID);
        }

        if (null != UILevel.instance && UILevel.instance.gameObject.activeSelf && !isSetNextBtn)
            isSetNextBtn = true;
    }
    void UpdateStar(Dictionary<string, object> item, int mapid)
    {
        if (null != item && item.Count > 0)
        {
            Dictionary<int, int[]> dungeon;
            if (GameLibrary.mapOrdinary.TryGetValue(mapid, out dungeon))
                dungeon = GameLibrary.mapOrdinary[mapid];
            else
                dungeon = new Dictionary<int, int[]>();
            List<string> heroKey = new List<string>(item.Keys);
            heroKey.Sort();
            for (int j = 0; j < heroKey.Count; j++)
            {
                if (int.Parse(heroKey[j]) == 0) continue;
                if (j == (heroKey.Count - 1) && Globe.GetStar((int[])(item[heroKey[j]])) > 0)
                {
                    int nextMapId = mapid + 100;
                    if (!playerData.GetInstance().CanEnterMap.Contains(nextMapId)
                        && playerData.GetInstance().worldMap.Contains(nextMapId)
                        && FunctionOpenMng.GetInstance().GetValu(nextMapId / 100))
                        playerData.GetInstance().CanEnterMap.Add(nextMapId);
                }
                int[] stat = (int[])item[heroKey[j]];
                if (dungeon.ContainsKey(int.Parse(heroKey[j])))
                {
                    //if (Globe.GetStar(dungeon[int.Parse(heroKey[j])]) < Globe.GetStar(stat))
                    //{
                    dungeon[int.Parse(heroKey[j])] = stat;
                    //}
                }
                else
                    dungeon.Add(int.Parse(heroKey[j]), stat);
            }

            if (!GameLibrary.mapOrdinary.ContainsKey(mapid))
                GameLibrary.mapOrdinary.Add(mapid, dungeon);
            else
                GameLibrary.mapOrdinary[mapid] = dungeon;
        }
    }
    void UpdateBox(object boxInfo, int mapid, int type)
    {
        //int星数 gold金币 dia钻石 item物品id int领取状态
        int[][] box;
        box = boxInfo as int[][];
        if (null != box)
        {
            GameLibrary.box = new int[box.Length][];
            for (int k = 0; k < box.Length; k++)
            {
                GameLibrary.box[k] = new int[6];
                for (int j = 0; j < 6; j++)
                {
                    GameLibrary.box[k][j] = box[k][j];
                }
            }
            if (!GameLibrary.mapBox.ContainsKey(mapid + type))
                GameLibrary.mapBox.Add(mapid + type, box);
            else
                GameLibrary.mapBox[mapid + type] = box;
        }
    }

    private bool GetQueryDungeonListResultHandle(CReadPacket packet)
    {

        // Debug.Log("Get Query Dungeon List result");

        Dictionary<string, object> data = packet.data;

        int resolt = int.Parse(data["ret"].ToString());

        bool isSetNextBtn = false;

        int[] mapID = data["mapId"] as int[];

        if (resolt == 0)
        {
            for (int i = 0; i < mapID.Length; i++)
            {
                if (!playerData.GetInstance().worldMap.Contains(mapID[i]))
                    playerData.GetInstance().worldMap.Add(mapID[i]);
                if (mapID[i] == 100 && !playerData.GetInstance().CanEnterMap.Contains(mapID[i]))
                    playerData.GetInstance().CanEnterMap.Add(mapID[i]);
            }
            UpdateMap(data, mapID, ref isSetNextBtn);
        }
        else
        {
            for (int i = 0; i < mapID.Length; i++)
            {
                if (playerData.GetInstance().worldMap.Contains(mapID[i]))
                    playerData.GetInstance().worldMap.Remove(mapID[i]);
            }
            Debug.Log(string.Format("获取副本列表错误：{0}", data["desc"].ToString()));
        }
        if (isSetNextBtn)
            UILevel.instance.TfMapContainer.GetComponent<UIMapContainer>().SetPrevAndNextBtn();
        return true;
    }

    private bool GetIntoDungeonResultHandle(CReadPacket packet)
    {

        // Debug.Log("Get Into Dungeon result");

        Dictionary<string, object> data = packet.data;

        int resolt = int.Parse(data["ret"].ToString());

        if (resolt == 0)
        {

            Dictionary<string, object> hero = data["hero"] as Dictionary<string, object>;

            if (null != hero && hero.Count > 0)
            {
                List<string> heroKey = new List<string>(hero.Keys);
                long id = 0;
                for (int i = 0; i < heroKey.Count; i++)
                {
                    id = long.Parse(heroKey[i]);
                    playerData.GetInstance().ReplaceHeroData(id, hero[heroKey[i]]);
                }
            }

            //Control.HideGUI(GameLibrary.UILevel);
            Globe.isFB = true;
            Singleton<SceneManage>.Instance.Current = EnumSceneID.Null;
            GameLibrary.mapId = UISceneEntry.instance.scene.bigmap_id;
            GameLibrary.dungeonId = UISceneEntry.instance.scene.SceneId;
            GameLibrary.dungeonType = UISceneEntry.instance.scene.Type;
            GameLibrary.chooseFB = UISceneEntry.instance.scene.MapName;
            UILevel.instance.SaveMapID(GameLibrary.mapId, GameLibrary.dungeonType);
            Singleton<SceneManage>.Instance.Current = EnumSceneID.Dungeons;
            playerData.GetInstance().selfData.SetPos(CharacterManager.player.transform.localPosition);
            Control.HideGUI(true);
            //UI_Loading.LoadScene(GameLibrary.chooseFB, 3, LoadHeroData, StarFight);
            GameLibrary.LastScene = SceneManager.GetActiveScene().name;//记录前一个场景名
            StartLandingShuJu.GetInstance().GetLoadingData(GameLibrary.chooseFB, 3, LoadHeroData, StarFight);
            SceneManager.LoadScene("Loding");
        }
        else
        {
            Debug.Log(string.Format("进入副本请求错误：{0}", data["desc"].ToString()));
        }

        return true;

    }

    private bool GetStartFightResultHandle(CReadPacket packet)
    {

        // Debug.Log("Get Start Fight result");

        Dictionary<string, object> data = packet.data;

        int resolt = int.Parse(data["ret"].ToString());

        if (resolt == 0)
        {

            long st = long.Parse(data["st"].ToString());
            long et = long.Parse(data["et"].ToString());

            GameLibrary.starTime = st;
            GameLibrary.endTime = et;
            //if (SceneBaseManager.instance != null)
            //    SceneBaseManager.instance.RunTime();

        }
        else
        {
            Debug.Log(string.Format("开始副本战斗错误：{0}", data["desc"].ToString()));
        }

        return true;

    }

    private bool GetFightSettlementResultHandle(CReadPacket packet)
    {

        // Debug.Log("Get Fight Settlement result");

        Dictionary<string, object> data = packet.data;

        int resolt = int.Parse(data["ret"].ToString());

        if (resolt == 0)
        {
            if (!data.ContainsKey("item"))
                return true;

            int gold = 0;

            if (data.ContainsKey("gold"))
            {
                gold = int.Parse(data["gold"].ToString());
                GameLibrary.receiveGolds = gold;
            }

            object[] item = data["item"] as object[];

            Dictionary<long, int> receiveGoods = new Dictionary<long, int>();

            if (null != item)
            {
                for (int i = 0; i < item.Length; i++)
                {
                    int id = int.Parse((item[i] as Dictionary<string, object>)["id"].ToString());
                    int at = int.Parse((item[i] as Dictionary<string, object>)["at"].ToString());
                    receiveGoods.Add(id, at);
                }
            }

            GameLibrary.receiveGoods = receiveGoods;

        }
        else
        {
            Debug.Log(string.Format("副本战斗结算错误：{0}", data["desc"].ToString()));
        }

        return true;

    }

    private bool GetDrawDungeonBoxRewardResultHandle(CReadPacket packet)
    {

        // Debug.Log("Get Draw Dungeon Box Reward result");

        Dictionary<string, object> data = packet.data;

        int resolt = int.Parse(data["ret"].ToString());

        if (resolt == 0)
        {

            Debug.Log("领取成功");
            UILevel.instance.TfMapContainer.GetComponent<UIMapContainer>().DrawDungeonBoxReward();

        }
        else
        {
            Debug.Log(string.Format("领取副本列表箱子奖励错误：{0}", data["desc"].ToString()));
        }

        return true;

    }

    private bool GetFlashDungeonFightResultHandle(CReadPacket packet)
    {

        // Debug.Log("Get flash dungeon fight result");

        Dictionary<string, object> data = packet.data;

        int resolt = int.Parse(data["ret"].ToString());

        if (resolt == 0)
        {
            object[] item = null;
            object[] other = null;

            if (data.ContainsKey("item"))
            {
                item = data["item"] as object[];
            }
            if (data.ContainsKey("other"))
            {
                other = data["other"] as object[];
            }

            if (null == item) return false;

            UISceneEntry.instance.OnCleanoutResult(item, other);
        }
        else
        {
            Debug.Log(string.Format("副本扫荡错误：{0}", data["desc"].ToString()));
        }

        return true;

    }

    private bool GetResetEliteDungeonResultHandle(CReadPacket packet)
    {

        // Debug.Log("Get reset elite dungeon result");

        Dictionary<string, object> data = packet.data;

        int resolt = int.Parse(data["ret"].ToString());

        if (resolt == 0)
        {
            //if (null != UISceneEntry.instance && UISceneEntry.instance.gameObject.activeSelf)
            //{
            //    UISceneEntry.instance.ResetEliteDungeon(2);
            //}
            //Debug.Log("重置精英副本成功");
        }
        else
        {
            Debug.Log(string.Format("重置精英副本错误：{0}", data["desc"].ToString()));
        }

        return true;

    }

    private bool GetCommonBuySomeoneResultHandle(CReadPacket packet)
    {
        Debug.Log("Get common buy someone result");

        Dictionary<string, object> data = packet.data;

        int resolt = int.Parse(data["ret"].ToString());

        if (resolt == 0)
        {
            //if (null != UISceneEntry.instance && UISceneEntry.instance.gameObject.activeSelf)
            //{
            //    UISceneEntry.instance.BuySweepVoucher();
            //}
            //Debug.Log("非商店购买某物品成功");
        }
        else
        {
            Debug.Log(string.Format("非商店购买某物品错误：{0}", data["desc"].ToString()));
            //UIPromptBox.Instance.ShowLabel(data["desc"].ToString());
            Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, data["desc"].ToString());
        }

        return true;

    }

    //活动
    private bool GetQueryEventListResultHandle(CReadPacket packet)
    {

        // Debug.Log("Get Query Event List result");

        Dictionary<string, object> data = packet.data;

        int resolt = int.Parse(data["ret"].ToString());

        if (resolt == 0)
        {
            Dictionary<string, object> edi = data["edi"] as Dictionary<string, object>;

            if (null != edi)
            {
                List<string> ediKey = new List<string>(edi.Keys);
                int count = 0;
                for (int i = 0; i < ediKey.Count; i++)
                {
                    count = Convert.ToInt32(edi[ediKey[i]]) % 10;
                    if (GameLibrary.eventdList.ContainsKey(Convert.ToInt32(ediKey[i])))
                    {
                        GameLibrary.eventdList[Convert.ToInt32(ediKey[i])] = count;
                    }
                    else
                    {
                        GameLibrary.eventdList.Add(Convert.ToInt32(ediKey[i]), count);
                    }

                }
            }

            Dictionary<string, object> edl = data["edl"] as Dictionary<string, object>;

            if (null != edl)
            {
                List<string> edlKey = new List<string>(edl.Keys);
                List<int[]> star = new List<int[]>();
                int[][] eventStar;
                edlKey.Sort();
                for (int i = 0; i < edlKey.Count; i++)
                {
                    star = new List<int[]>();
                    eventStar = (int[][])edl[edlKey[i]];
                    for (int j = 0; j < eventStar.Length; j++)
                    {
                        star.Add(eventStar[j]);
                    }

                    if (GameLibrary.eventsList.ContainsKey(Convert.ToInt32(edlKey[i])))
                    {
                        GameLibrary.eventsList[Convert.ToInt32(edlKey[i])] = star;
                    }
                    else
                    {
                        GameLibrary.eventsList.Add(Convert.ToInt32(edlKey[i]), star);
                    }

                }

            }

            int[] edoOpen = null;
            if (null != data["edo"] && data["edo"] is int[])
            {
                edoOpen = (int[])data["edo"];
            }
            if (null != edoOpen)
            {
                GameLibrary.eventOpen = edoOpen;
            }
            
        }
        else
        {
            Debug.Log(string.Format("获取活动副本列表：{0}", data["desc"].ToString()));
        }

        return true;

    }

    private bool GetIntoEventDungeonResultHandle(CReadPacket packet)
    {

        // Debug.Log("Get Into Event Dungeon result");
        Dictionary<string, object> data = packet.data;
        int resolt = int.Parse(data["ret"].ToString());
        if (resolt == 0)
        {
            Dictionary<string, object> hero = data["hero"] as Dictionary<string, object>;
            if (null != hero && hero.Count > 0)
            {
                List<string> heroKey = new List<string>(hero.Keys);
                long id = 0;
                for (int i = 0; i < heroKey.Count; i++)
                {
                    id = long.Parse(heroKey[i]);
                    playerData.GetInstance().ReplaceHeroData(id, hero[heroKey[i]]);
                }
            }

            Globe.isFB = true;
            GameLibrary.mapId = UISceneEntry.instance.scene.bigmap_id;
            GameLibrary.dungeonId = UISceneEntry.instance.scene.SceneId;
            GameLibrary.chooseFB = UISceneEntry.instance.scene.MapName;
            //Globe.backPanelParameter = new object[] { GameLibrary.UIActivityPanel, UISceneEntry.instance.sn.bigmap_id };
            Singleton<SceneManage>.Instance.Current = EnumSceneID.Dungeons;
            //UI_Loading.LoadScene(GameLibrary.chooseFB, 3, null, StarEventFight);
            GameLibrary.LastScene = SceneManager.GetActiveScene().name;//记录前一个场景名
            StartLandingShuJu.GetInstance().GetLoadingData(GameLibrary.chooseFB, 3, null, StarEventFight);
            playerData.GetInstance().selfData.SetPos(CharacterManager.player.transform.localPosition);
            Control.HideGUI(true);
            SceneManager.LoadScene("Loding");
        }
        else
        {
            Debug.Log(string.Format("进入活动副本请求：{0}", data["desc"].ToString()));
        }
        return true;
    }

    private bool GetStartEventFightResultHandle(CReadPacket packet)
    {

        // Debug.Log("Get Start Event Fight result");
        Dictionary<string, object> data = packet.data;
        int resolt = int.Parse(data["ret"].ToString());
        if (resolt == 0)
        {

            long st = long.Parse(data["st"].ToString());
            long et = long.Parse(data["et"].ToString());

            GameLibrary.starTime = st;
            GameLibrary.endTime = et;
            if (SceneBaseManager.instance != null)
                SceneBaseManager.instance.RunTime();

        }
        else
        {
            Debug.Log(string.Format("开始活动副本战斗：{0}", data["desc"].ToString()));
        }
        return true;
    }

    private bool GetEventFightSettlementResultHandle(CReadPacket packet)
    {

        // Debug.Log("Get Event Fight Settlement result");
        Dictionary<string, object> data = packet.data;
        int resolt = int.Parse(data["ret"].ToString());
        if (resolt == 0)
        {

            if (!data.ContainsKey("item"))
                return true;

            int gold = 0;

            if (data.ContainsKey("gold"))
            {
                gold = int.Parse(data["gold"].ToString());
                GameLibrary.receiveGolds = gold;
            }

            object[] item = data["item"] as object[];

            Dictionary<long, int> receiveGoods = new Dictionary<long, int>();

            if (null != item)
            {

                int[] goods;

                for (int i = 0; i < item.Length; i++)
                {
                    goods = item[i] as int[];

                    if (receiveGoods.ContainsKey(goods[0]))
                    {
                        receiveGoods[goods[0]] += goods[1];
                    }
                    else
                    {
                        receiveGoods.Add(goods[0], goods[1]);
                    }

                }
            }

            GameLibrary.receiveGoods = receiveGoods;

        }
        else
        {
            Debug.Log(string.Format("活动副本战斗结算：{0}", data["desc"].ToString()));
        }
        return true;
    }

    private bool GetEventFlashDungeonFightResultHandle(CReadPacket packet)
    {

        // Debug.Log("Get evevt flash dungeon fight result");

        Dictionary<string, object> data = packet.data;

        int resolt = int.Parse(data["ret"].ToString());

        if (resolt == 0)
        {
            object[] item = null;
            object[] other = null;
            if (data.ContainsKey("item"))
            {
                item = data["item"] as object[];
            }
            if (data.ContainsKey("other"))
            {
                other = data["other"] as object[];
            }

            if (null == item) return false;

            UISceneEntry.instance.OnCleanoutResult(item, other);

            //UILevel.instance.TfMapContainer.GetComponent<UIMapContainer>().SceneEntry.OnCleanoutResult(item);

        }
        else
        {
            Debug.Log(string.Format("活动副本扫荡错误：{0}", data["desc"].ToString()));
        }

        return true;

    }



    void LoadHeroData()
    {
        //for (int i = 0; i < Globe.playHeroList.Length; i++)
        //{
        //    if (Globe.playHeroList[i].node.hero_id != 0)
        //    {
        //        ClientSendDataMgr.GetSingle().GetHeroSend().SendGetHeroInfo(Globe.playHeroList[i].node.hero_id, C2SMessageType.Active);
        //    }
        //}
    }

    void StarFight()
    {
        ClientSendDataMgr.GetSingle().GetBattleSend().SendStartFight(GameLibrary.mapId, GameLibrary.dungeonId);
    }

    void StarEventFight()
    {
        ClientSendDataMgr.GetSingle().GetBattleSend().SendStartEventFight(GameLibrary.dungeonId);
    }

}
