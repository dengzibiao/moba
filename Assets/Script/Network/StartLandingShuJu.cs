using UnityEngine;
using System.Collections;
using Pathfinding.Serialization.JsonFx;
using System.Collections.Generic;
using Tianyu;
using UnityEngine.SceneManagement;
using System;

public class StartLandingShuJu : MonoBehaviour
{

    public int SceneID;
    public string currentScene;

    private static StartLandingShuJu instance;
    public static StartLandingShuJu GetInstance()
    {
        return instance;
    }

    private JsonReaderSettings readerSettings;
    private WWW mWWW;

    void OnLevelWasLoaded(int level)
    {
        currentScene = SceneManager.GetActiveScene().name;

        if (currentScene != "Loding")
        {
            //背景音乐
            PlayBgMusic();
            if (instance == this)
            {
                CreateInitGo();
                CreateMainPlayer();
            }
            else
            {
                enabled = false;
            }
            BackMajorOpenPanel();
        }
           
    }

    void BackMajorOpenPanel()
    {
        if (currentScene == GameLibrary.UI_Major && null != Globe.backPanelParameter)
        {
            switch ((UIPanleID)Globe.backPanelParameter[0])// int.Parse(Globe.backPanelParameter[0].ToString())
            {
                case UIPanleID.UIActivity:
                    UI_Setting.GetInstance().OnEnchantBtnClick();
                    break;
                case UIPanleID.UIPvP:
                    UI_Setting.GetInstance().OnArenaABtnClick();
                    break;
                case UIPanleID.EquipDevelop:
                    UI_Setting.GetInstance().OnEquipBtn();
                    Globe.backPanelParameter = null;
                    break;
                case UIPanleID.UILevel:
                    object[] openParams = new object[] { OpenLevelType.ByIDOpen, (int)Globe.backPanelParameter[1] };
                    Control.ShowGUI(UIPanleID.UILevel, EnumOpenUIType.OpenNewCloseOld, false, openParams);
                    break;
                default:
                    //Control.ShowGUI(Globe.backPanelParameter[0].ToString());
                    Control.ShowGUI((UIPanleID)(int.Parse(Globe.backPanelParameter[0].ToString())), EnumOpenUIType.OpenNewCloseOld, false);
                    Globe.backPanelParameter = null;
                    break;
            }
        }
    }

    public void PlayBgMusic()
    {
        string mCurSceneName = currentScene;
        switch (mCurSceneName)
        {
            case GameLibrary.Scene_Login:
                AudioController.Instance.PlayBackgroundMusic("login", true);
                break;
            case GameLibrary.UI_Major:
                AudioController.Instance.PlayBackgroundMusic("major", true);
                break;
            case GameLibrary.PVP_Zuidui:
                AudioController.Instance.PlayBackgroundMusic("jdc", true);
                break;
            default:
                AudioController.Instance.PlayBackgroundMusic("copy", true);
                break;
        }
    }

    void Awake()
    {
        currentScene = SceneManager.GetActiveScene().name;
    }

    void Start()
    {
        if(currentScene!= "Loding")
        {
            PlayBgMusic();
            serverMgr.GetInstance().gamedata = new GameDataSave();
            serverMgr.GetInstance().gamedata.startXml();
            serverMgr.GetInstance().gamedata.Load();

            if (instance == null)
            {
                if (instance != null)
                    Destroy(instance);
                instance = this;
                if (currentScene == GameLibrary.Scene_Login)
                {
                    GameLibrary.isNetworkVersion = true;
                    DontDestroyOnLoad(gameObject);

                }
                Localization.language = "Chinese";
                loadJson();

                CreateInitGo();
                CreateMainPlayer();
            }
            AudioController.Instance.Mute(serverMgr.GetInstance().GetGameMusic());
            AudioController.Instance.SoundMute(serverMgr.GetInstance().GetGameSoundEffect());
            //设置屏幕正方向在Home键右边
            //  Screen.orientation = ScreenOrientation.LandscapeRight;

            ////设置屏幕自动旋转， 并置支持的方向
            //Screen.orientation = ScreenOrientation.AutoRotation;
            //Screen.autorotateToLandscapeLeft = true;
            //Screen.autorotateToLandscapeRight = true;
            //Screen.autorotateToPortrait = false;
            //Screen.autorotateToPortraitUpsideDown = false;

        }
        Application.targetFrameRate = 30;
    }

    void CreateInitGo()
    {
        //如果是主城或龙骨荒原
        if (currentScene == GameLibrary.UI_Major || currentScene == GameLibrary.Scene_Login)
            return;
        //场景id不为0，副本id为0，同步场景id
        if (SceneID != 0 && GameLibrary.dungeonId == 0)
        {
            GameLibrary.dungeonId = SceneID;
        }
        //活动副本是30000以上的id
        if (GameLibrary.dungeonId >= 30000 || !CreateScenePrefab(GameLibrary.dungeonId))
        {
            //获取场景预制体名称
            string prefabName = null;
            if (FSDataNodeTable<SceneNode>.GetSingleton().DataNodeList.ContainsKey(GameLibrary.dungeonId))
            {
                prefabName = FSDataNodeTable<SceneNode>.GetSingleton().DataNodeList[GameLibrary.dungeonId].Config;
            }
            else if (currentScene == GameLibrary.LGhuangyuan)
            {
                prefabName = "SceneCtrl_LG";
            }
            //实例化
            if (!string.IsNullOrEmpty(prefabName) && (!GameObject.Find(prefabName) || !GameObject.Find(prefabName).activeSelf))
            {
                GameObject Scenectrl = Resources.Load<GameObject>("Prefab/SceneCtrls/" + prefabName) as GameObject;
                if (null == Scenectrl)
                {
                    Debug.Log("Scenectrl is null.");
                }
                else
                {
                    GameObject.Instantiate<GameObject>(Scenectrl);
                }
            }

            prefabName = null;
        }

        if (FindObjectOfType<UIRoot>() != null && currentScene != GameLibrary.LGhuangyuan)
            FindObjectOfType<UIRoot>().gameObject.SetActive(false);
        if (FindObjectOfType<UIRoot>() == null)
            Instantiate(Resources.Load<GameObject>("Prefab/UI Root"));
        Camera mainCam = Camera.main;
        if (mainCam != null && !mainCam.orthographic)
        {
            UnityUtil.AddComponetIfNull<ThirdCamera>(mainCam.gameObject);
            //DisableSyncWalkMsg();
            //UnityUtil.AddComponetIfNull<CameraMover>(mainCam.gameObject);
        }

    }

    bool CreateScenePrefab(int sceneID)
    {
        if (sceneID == 0)
        {
            Debug.Log("SceneID is 0!");
            return false;
        }

        SceneBaseManager smBase = GameObject.FindObjectOfType<SceneBaseManager>();
        if (null != smBase) smBase.gameObject.SetActive(false);

        GameObject game = new GameObject("SceneCtrl");
        SceneBaseManager scene = null;
        SceneNode node = FSDataNodeTable<SceneNode>.GetSingleton().FindDataByType(sceneID);
        if (null == node) return false;
        int type = node.game_type;
        if (type == 6) return false;
        switch (type)
        {
            case 1:
                if (sceneID == 10000)
                {
                    if (!GameLibrary.isNetworkVersion)
                        Globe.isFightGuide = true;
                    scene = game.AddComponent<SceneNewbieGuide>();
                }
                else
                    scene = game.AddComponent<SceneDungeons>();
                scene.sceneType = SceneType.KM; break;
            case 2:
                scene = game.AddComponent<SceneMoba1>();
                scene.sceneType = SceneType.Dungeons_MB1; break;
            case 3:
                scene = game.AddComponent<SceneTotalPoints>();
                scene.sceneType = SceneType.TP; break;
            case 4:
                scene = game.AddComponent<SceneEscort>();
                scene.sceneType = SceneType.KV; break;
            case 5:
                scene = game.AddComponent<SceneTowerDefence>();
                scene.sceneType = SceneType.TD; break;
        }
        scene.CreateScenePrefab(game);
        return true;
    }

    void CreateMainPlayer()
    {
        // Globe.UseServerBattleAttrs = GameLibrary.isNetworkVersion;
        if ((currentScene == GameLibrary.UI_Major || currentScene == GameLibrary.LGhuangyuan) || currentScene == GameLibrary.PVP_1V1 && CharacterManager.instance != null)
        {
            CharacterManager.instance.CreateTownPlayer();
            if (MountAndPetNodeData.Instance().godefPetID != 0)
                CharacterManager.playerCS.CreatePet(MountAndPetNodeData.Instance().godefPetID);
            if (MountAndPetNodeData.Instance().goMountID > 0)
                CharacterManager.playerCS.pm.Ride(true, MountAndPetNodeData.Instance().goMountID, false);
        }
        if (FindObjectOfType<SceneBaseManager>() != null)
        {
            FindObjectOfType<SceneBaseManager>().StartCD();
            FindObjectOfType<SceneBaseManager>().InitScene();
        }
    }

    public void ServerList()
    {
        if (DataDefine.isConectSocket)
        {
            StartCoroutine(LoadServerlist());
        }
    }
    public void GetServedrDate(Dictionary<string, object> serverlist)
    {
        ServeData data = new ServeData();
        if (serverlist.ContainsKey("descd"))
            data.Desc = serverlist["desc"].ToString();

        if (serverlist.ContainsKey("ip"))
            data.ip = serverlist["ip"].ToString();

        if (serverlist.ContainsKey("areaId"))
            data.areaId = int.Parse(serverlist["areaId"].ToString());

        if (serverlist.ContainsKey("playerId"))
            data.playerId = uint.Parse(serverlist["playerId"].ToString());

        if (serverlist.ContainsKey("name"))
            data.name = serverlist["name"].ToString();

        if (serverlist.ContainsKey("state"))
            data.state = byte.Parse(serverlist["state"].ToString());

        if (serverlist.ContainsKey("port"))
            data.port = int.Parse(serverlist["port"].ToString());

        Globe.SelectedServer = data;
    }
    public void SetServerList(Dictionary<string, object>[] serverarr)
    {
        if (serverarr == null)
            return;
        if (serverMgr.GetInstance().serverkeymap.Count != 0)
        {
            serverMgr.GetInstance().serverkeymap.Clear();
        }
        if (serverMgr.GetInstance().serverlist.Count != 0)
        {
            serverMgr.GetInstance().serverlist.Clear();
        }



        for (int i = 0; i < serverarr.Length; i++)
        {
            ServeData dater = new ServeData();
            dater.name = serverarr[i]["name"].ToString();
            dater.ip = serverarr[i]["ip"].ToString();
            dater.port = int.Parse(serverarr[i]["port"].ToString());
            dater.state = byte.Parse(serverarr[i]["state"].ToString());
            dater.Desc = serverarr[i]["desc"].ToString();
            dater.areaId = int.Parse(serverarr[i]["areaId"].ToString());
            dater.playerId = uint.Parse(serverarr[i]["playerId"].ToString());
            if (dater.playerId != 0)
            {
                if (serverarr[i].ContainsKey("playerName"))
                    dater.playerName = serverarr[i]["playerName"].ToString();

                if (serverarr[i].ContainsKey("heroId"))
                    dater.heroId = long.Parse(serverarr[i]["heroId"].ToString());
            }
            serverMgr.GetInstance().serverkeymap.Add(dater.areaId, dater.name);
            serverMgr.GetInstance().serverlist.Add(dater);
            if (FSDataNodeTable<UIGameAfficheNode>.GetSingleton().DataNodeList != null)
            {
                foreach (var item in FSDataNodeTable<UIGameAfficheNode>.GetSingleton().DataNodeList.Values)
                {
                    if (item.id == 1)
                    {
                        if ((serverMgr.GetInstance().GetGameAfficheStart() * 100) != (item.version * 100))
                        {
                            Control.ShowGUI(UIPanleID.UIGameAffiche, EnumOpenUIType.DefaultUIOrSecond);
                            serverMgr.GetInstance().SetGameAfficheStart(item.version);
                            serverMgr.GetInstance().saveData();
                        }
                    }
                }
            }
            // UISelectServer.Instance.InitServerList();
        }
        Control.ShowGUI(UIPanleID.UI_SelectServer, EnumOpenUIType.DefaultUIOrSecond,false);
    }
    //获取服务器列表
    public IEnumerator LoadServerlist()
    {
        long[] arg = { serverMgr.GetInstance().GetMobile(), 1 };
        string temp;// = string.Format( url , arg );
        if (DataDefine.isOutLine)
        {
            //temp = DataDefine.ServerListOutLineUrl + "?account=" + serverMgr.GetInstance().GetMobile().ToString() + "&types=" + 2.ToString() + "&cv=" + DataDefine.ClientVersion+"&mc="+1002;
            temp = DataDefine.ServerListOutLineUrl + "?account=" + serverMgr.GetInstance().GetMobile().ToString() + "&types=" + 2.ToString() + "&cv=" + DataDefine.ClientVersion + "&mc=" + 1002;
        }
        else
        {
            temp = DataDefine.ServerListUrl + "?account=" + serverMgr.GetInstance().GetMobile().ToString() + "&types=" + 2.ToString() + "&cv=" + DataDefine.ClientVersion+ "&mc=" + 1002;
        }
        this.mWWW = new WWW(temp);

        yield return mWWW;
        if (this.mWWW == null || !this.mWWW.isDone)
            yield return mWWW;
        string text = this.mWWW.data;
        if (this.mWWW.error == null)
        {
            //读json串
            Dictionary<string, object> aobj = (Dictionary<string, object>)Jsontext.ReadeData(text);
            int ret = int.Parse(aobj["ret"].ToString());
            if (ret == 0)
            {
                Dictionary<string, object> servernew = aobj["new"] as Dictionary<string, object>;
                Dictionary<string, object>[] serverarr = aobj["host"] as Dictionary<string, object>[];
                Dictionary<string, object> serverlast = aobj["last"] as Dictionary<string, object>;

                if (null != serverlast)
                {
                    GetServedrDate(serverlast);
                }
                else
                {
                    GetServedrDate(servernew);
                }

                SetServerList(serverarr);
                //  UISelectServer.Instance.InitServerList(); 
            }
        }
    }

    // 数据存在依赖关系，注意解析顺序
    void loadJson()
    {
        FSDataNodeTable<SkillNode>.GetSingleton().LoadJson(ResLoad.LoadJsonRes1("skill/skill")); //技能表
        FSDataNodeTable<SkillBuffNode>.GetSingleton().LoadJson(ResLoad.LoadJsonRes1("skill/skillBuffs")); //技能buff
        FSDataNodeTable<ModelNode>.GetSingleton().LoadJson(ResLoad.LoadJsonRes1("model/model"));//模型表
        FSDataNodeTable<HeroNode>.GetSingleton().LoadJson(ResLoad.LoadJsonRes1("hero/hero"));//英雄表
        FSDataNodeTable<HeroAttrNode>.GetSingleton().LoadJson(ResLoad.LoadJsonRes1("hero/heroAttr"));//英雄属性表
        FSDataNodeTable<RoleIconAttrNode>.GetSingleton().LoadJson(ResLoad.LoadJsonRes1("player/headicon"));//修改头像
        FSDataNodeTable<MapNode>.GetSingleton().LoadJson(ResLoad.LoadJsonRes1("map/worldMap"));//大关表
        FSDataNodeTable<SceneNode>.GetSingleton().LoadJson(ResLoad.LoadJsonRes1("map/dungeons"));//小关表
        FSDataNodeTable<UpGradeSkillConsume>.GetSingleton().LoadJson(ResLoad.LoadJsonRes1("skill/upgradeSkillConsume"));//技能消耗表        
        FSDataNodeTable<MonsterAttrNode>.GetSingleton().LoadJson(ResLoad.LoadJsonRes1("monster/monsterAttr")); // 怪物表
        //FSDataNodeTable<ItemNode>.GetSingleton().LoadJson(ResLoad.LoadJsonRes1("item/item"));
        FSDataNodeTable<StarUpGradeNode>.GetSingleton().LoadJson(ResLoad.LoadJsonRes1("hero/upgradeStarConsume"));
        FSDataNodeTable<EquipUpgradeNode>.GetSingleton().LoadJson(ResLoad.LoadJsonRes1("hero/upgradeEquipmentConsume"));
        FSDataNodeTable<HeroUpGradeNode>.GetSingleton().LoadJson(ResLoad.LoadJsonRes1("hero/heroLevelUp"));
        FSDataNodeTable<CommentsStarNode>.GetSingleton().LoadJson(ResLoad.LoadJsonRes1("scene/CommentsStar"));
        FSDataNodeTable<LevelConfigsNode>.GetSingleton().LoadJson(ResLoad.LoadJsonRes1("scene/LevelConfigs"));
        FSDataNodeTable<LevelConfigNode>.GetSingleton().LoadJson(ResLoad.LoadJsonRes1("scene/LevelConfig"));
        FSDataNodeTable<MobaLevelConfigNode>.GetSingleton().LoadJson(ResLoad.LoadJsonRes1("scene/MobaLevelConfig"));
        FSDataNodeTable<TPLevelConfigNode>.GetSingleton().LoadJson(ResLoad.LoadJsonRes1("scene/TPLevelConfig"));
        FSDataNodeTable<TDLevelConfigNode>.GetSingleton().LoadJson(ResLoad.LoadJsonRes1("scene/TDLevelConfig"));
        FSDataNodeTable<AirWallConfig>.GetSingleton().LoadJson(ResLoad.LoadJsonRes1("scene/AirWallConfig"));
        FSDataNodeTable<TowerDefenceNode>.GetSingleton().LoadJson(ResLoad.LoadJsonRes1("scene/TowerDefence"));
        FSDataNodeTable<ActivityPropsNode>.GetSingleton().LoadJson(ResLoad.LoadJsonRes1("scene/ActivityProps"));
        FSDataNodeTable<MapInfoNode>.GetSingleton().LoadJson(ResLoad.LoadJsonRes1("mapInfo/ScenceConfigureTable"));
        FSDataNodeTable<ScenceElementFileIndexTableNode>.GetSingleton().LoadJson(ResLoad.LoadJsonRes1("mapInfo/ScenceElementFileIndexTable"));
        FSDataNodeTable<TaskDataNode>.GetSingleton().LoadJson(ResLoad.LoadJsonRes1("task/taskDetails"));
        FSDataNodeTable<TaskInstructionsNode>.GetSingleton().LoadJson(ResLoad.LoadJsonRes1("task/taskinstructions"));
        FSDataNodeTable<TaskPropsNode>.GetSingleton().LoadJson(ResLoad.LoadJsonRes1("task/Useprops"));
        FSDataNodeTable<CollectNode>.GetSingleton().LoadJson(ResLoad.LoadJsonRes1("task/Collect"));
        FSDataNodeTable<PlotLinesNode>.GetSingleton().LoadJson(ResLoad.LoadJsonRes1("task/plot_lines"));
        FSDataNodeTable<DailyTasksNode>.GetSingleton().LoadJson(ResLoad.LoadJsonRes1("task/Daily_tasks"));//日常任务
        FSDataNodeTable<DayActiveNode>.GetSingleton().LoadJson(ResLoad.LoadJsonRes1("task/day_active"));//日常箱子状态
        FSDataNodeTable<RewardTaskNode>.GetSingleton().LoadJson(ResLoad.LoadJsonRes1("task/Reward_tasks"));//悬赏任务
        FSDataNodeTable<TaskRewardNode>.GetSingleton().LoadJson(ResLoad.LoadJsonRes1("task/taskReward"));//任务奖励表
        FSDataNodeTable<NPCNode>.GetSingleton().LoadJson(ResLoad.LoadJsonRes1("npc/npc"));
        FSDataNodeTable<ItemNode>.GetSingleton().LoadJson(ResLoad.LoadJsonRes1("item/item"));//物品
        FSDataNodeTable<ItemEquipNode>.GetSingleton().LoadJson(ResLoad.LoadJsonRes1("item/equipment"));//装备
        FSDataNodeTable<MaterialNode>.GetSingleton().LoadJson(ResLoad.LoadJsonRes1("item/material"));//材料
        FSDataNodeTable<RuneNode>.GetSingleton().LoadJson(ResLoad.LoadJsonRes1("item/rune"));//魂石
        FSDataNodeTable<TitleNode>.GetSingleton().LoadJson(ResLoad.LoadJsonRes1("player/title"));//称号表
        FSDataNodeTable<ResetLaterNode>.GetSingleton().LoadJson(ResLoad.LoadJsonRes1("player/buyReset"));//购买重置表
        FSDataNodeTable<PlayerLevelUpNode>.GetSingleton().LoadJson(ResLoad.LoadJsonRes1("player/playerLevelUp"));//玩家升级表
        FSDataNodeTable<UnLockFunctionNode>.GetSingleton().LoadJson(ResLoad.LoadJsonRes1("system/unlockFunction"));//解锁功能表
        FSDataNodeTable<LevelRewardNode>.GetSingleton().LoadJson(ResLoad.LoadJsonRes1("welfare/upgradeReward"));//等级奖励表
        FSDataNodeTable<OnlineRewardNode>.GetSingleton().LoadJson(ResLoad.LoadJsonRes1("welfare/onlineReward"));//在线奖励表
        FSDataNodeTable<UISign_inNode>.GetSingleton().LoadJson(ResLoad.LoadJsonRes1("welfare/signIn"));//签到
        FSDataNodeTable<NewPlayerRewardNode>.GetSingleton().LoadJson(ResLoad.LoadJsonRes1("welfare/newbieReward"));//新手15日登录礼包
        FSDataNodeTable<MealAttrNode>.GetSingleton().LoadJson(ResLoad.LoadJsonRes1("welfare/meal"));//定时进餐
        FSDataNodeTable<GoldDrawNode>.GetSingleton().LoadJson(ResLoad.LoadJsonRes1("activity/goldDraw"));//金币抽奖
        FSDataNodeTable<DiamondDrawNode>.GetSingleton().LoadJson(ResLoad.LoadJsonRes1("activity/diamondDraw"));//钻石抽奖
        FSDataNodeTable<SoulDrawNode>.GetSingleton().LoadJson(ResLoad.LoadJsonRes1("activity/soulDraw"));//魂匣抽奖
        FSDataNodeTable<GoldHandNode>.GetSingleton().LoadJson(ResLoad.LoadJsonRes1("activity/luckyDraw"));//点金手表
        FSDataNodeTable<PlayerNameNode>.GetSingleton().LoadJson(ResLoad.LoadJsonRes1("player/playerName"));//改名字库表       
        FSDataNodeTable<NpcTableNode>.GetSingleton().LoadJson(ResLoad.LoadJsonRes1("mapInfo/NpcTable"));
        FSDataNodeTable<TransferTableNode>.GetSingleton().LoadJson(ResLoad.LoadJsonRes1("mapInfo/TransferTable"));
        FSDataNodeTable<RouteNode>.GetSingleton().LoadJson(ResLoad.LoadJsonRes1("mapInfo/RouteTable"));
        FSDataNodeTable<VipNode>.GetSingleton().LoadJson(ResLoad.LoadJsonRes1("vip/vip"));//vip表
        FSDataNodeTable<UIGameAfficheNode>.GetSingleton().LoadJson(ResLoad.LoadJsonRes1("UIGameAffiche/operate_notice"));
        FSDataNodeTable<GuideNode>.GetSingleton().LoadJson(ResLoad.LoadJsonRes1("guide/guide"));//新手引导表
        FSDataNodeTable<UIMountNode>.GetSingleton().LoadJson(ResLoad.LoadJsonRes1("mount/mount"));//坐骑表
        FSDataNodeTable<PetNode>.GetSingleton().LoadJson(ResLoad.LoadJsonRes1("pet/pet"));//宠物表

        FSDataNodeTable<UIPetNode>.GetSingleton().LoadJson(ResLoad.LoadJsonRes1("Pet/pet"));//宠物表

        FSDataNodeTable<Archaeology_rewardNode>.GetSingleton().LoadJson(ResLoad.LoadJsonRes1("activity/Archaeology_reward"));//活动-考古
        FSDataNodeTable<ShopNode>.GetSingleton().LoadJson(ResLoad.LoadJsonRes1("shop/shop"));//商店表
        FSDataNodeTable<Moba3SceneConfigNode>.GetSingleton().LoadJson(ResLoad.LoadJsonRes1("Moba/Moba3v3ConfigTable"));//moba3v3
        FSDataNodeTable<Moba3v3NaviNode>.GetSingleton().LoadJson(ResLoad.LoadJsonRes1("Moba/Moba3v3NaviNode"));//moba3v3
        FSDataNodeTable<MobaRobotNode>.GetSingleton().LoadJson(ResLoad.LoadJsonRes1("Moba/arena_robot"));
        FSDataNodeTable<AfcNode>.GetSingleton().LoadJson(ResLoad.LoadJsonRes1("hero/afc"));//战斗力表

        //LevelConfigNode
        foreach (MapInfoNode tempMIN in FSDataNodeTable<MapInfoNode>.GetSingleton().DataNodeList.Values)
        {
            foreach (ScenceElementFileIndexTableNode tempSEFITN in FSDataNodeTable<ScenceElementFileIndexTableNode>.GetSingleton().DataNodeList.Values)
            {
                if (tempSEFITN.key == tempMIN.map_info)
                {
                    string name = tempSEFITN.filename;
                    FSDataNodeTable<SceneMapNode>.GetSingleton().LoadJsons(ResLoad.LoadJsonRes1("mapInfo/" + name), name);//野外地图表
                }
            }
        }

        InitItemList();

    }
    /// <summary>
    /// 初始化物品列表
    /// </summary>
    void InitItemList()
    {
        foreach (ItemNode item in FSDataNodeTable<ItemNode>.GetSingleton().DataNodeList.Values)
        {
            ItemNodeState itemstate = new ItemNodeState();
            item.setData(ref itemstate);
            AddToStateDict(itemstate);
        }

        FSDataNodeTable<ItemNode>.GetSingleton().DataNodeList.Clear();

        foreach (MaterialNode item in FSDataNodeTable<MaterialNode>.GetSingleton().DataNodeList.Values)
        {
            ItemNodeState itemstate = new ItemNodeState();
            item.setData(ref itemstate);
            AddToStateDict(itemstate);
        }

        FSDataNodeTable<MaterialNode>.GetSingleton().DataNodeList.Clear();

        foreach (ItemEquipNode item in FSDataNodeTable<ItemEquipNode>.GetSingleton().DataNodeList.Values)
        {
            ItemNodeState itemstate = new ItemNodeState();
            item.setData(ref itemstate);
            AddToStateDict(itemstate);
        }

        FSDataNodeTable<ItemEquipNode>.GetSingleton().DataNodeList.Clear();

        foreach (RuneNode item in FSDataNodeTable<RuneNode>.GetSingleton().DataNodeList.Values)
        {
            ItemNodeState itemstate = new ItemNodeState();
            item.setData(ref itemstate);
            AddToStateDict(itemstate);
        }
        FSDataNodeTable<RuneNode>.GetSingleton().DataNodeList.Clear();
    }

    void AddToStateDict(ItemNodeState itemstate)
    {
        if (!GameLibrary.Instance().ItemStateList.ContainsKey(itemstate.props_id))
        {
            GameLibrary.Instance().ItemStateList.Add(itemstate.props_id, itemstate);
        }
        else
        {
            //Debug.Log(itemstate.props_id);
        }
    }


    ////主城以外移除同步消息
    //void DisableSyncWalkMsg()
    //{
    //    //if (FSDataNodeTable<MapInfoNode>.GetSingleton().DataNodeList.ContainsKey(playerData.GetInstance().selfData.mapID))

    //    //    if (FSDataNodeTable<SceneNode>.GetSingleton().DataNodeList.ContainsKey(GameLibrary.dungeonId))
    //    //    {
    //    //        MapInfoNode tempMN = FSDataNodeTable<MapInfoNode>.GetSingleton().DataNodeList[playerData.GetInstance().selfData.mapID];
    //    //        if (tempMN != null)
    //    //        {
    //    //            if (currentScene != tempMN.MapName)
    //    //            {
    //    //                //WalkSendMgr.GetSingle().GetWalkSend().SendQuit();
    //    //            }
    //    //        }
    //    //    }
    //    foreach (MapInfoNode min in FSDataNodeTable<MapInfoNode>.GetSingleton().DataNodeList.Values)
    //    {
    //        if (min.MapName == Application.loadedLevelName)
    //        {
    //            if (FSDataNodeTable<ScenceElementFileIndexTableNode>.GetSingleton().DataNodeList.ContainsKey(min.map_info))
    //            {
    //                if (FSDataNodeTable<ScenceElementFileIndexTableNode>.GetSingleton().DataNodeList[min.map_info].isHave == 1)
    //                {
    //                    MiniMap.Create(FSDataNodeTable<ScenceElementFileIndexTableNode>.GetSingleton().DataNodeList[min.map_info]);

    //                    //  MiniMap.instance.CreateTargetPos(CharacterManager.player, ShowType.player);
    //                }
    //            }
    //        }
    //    }
    //}

    public void GetLoadingData(string SceneName, float LoadTime, CallBack callBack = null, CallBack completed = null)
    {
        Globe.LoadScenceName = SceneName;
        Globe.LoadTime = LoadTime;
        Globe.callBack = callBack;
        Globe.completed = completed;
    }
}
