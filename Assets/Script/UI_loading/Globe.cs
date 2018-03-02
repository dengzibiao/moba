using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tianyu;
using UnityEngine.SceneManagement;
/// <summary>
/// 公共数据
/// </summary>

public delegate void CallBack();
public delegate void ChangeServer(ServeData data);

#region 委托
/// <summary>
/// 用于object状态改变时回调
/// </summary>
/// <param name="sender"></param>
/// <param name="newState"></param>
/// <param name="oldState"></param>
public delegate void StateChangedHandler(object sender, EnumObjectState newState, EnumObjectState oldState);

#endregion
public class Globe
{
	public static float lastNetTime = 0;
    public static bool isInitHero = false;
    public static string account;
    public static string password;
    public static string playerId = "12345";//
    public static int mapId = 0;
    public static string LoadScenceName = "Anmaodao_KM2";
    public static ServeData SelectedServer;
    public static float LoadTime = 0;
    public static bool isOnce = false;
    public static CallBack callBack = null;
    public static CallBack completed = null;
    public static bool IsvrState = false;
    public static bool isInCity = false;
    public static int currentDepth;
    // public static bool UseServerBattleAttrs = true;//true 用server的战斗属性

    public static bool isUpgrade = false;//是否升级  用于是否显示升级面板
    public static bool isMainCityUpgrade = false;
    public static bool isDungeonsUpgrade = false;
    public static bool isSaoDang = false;//是扫荡造成的升级
    public  static int autoScenceCount = 1;
    //已有英雄列表
    public static List<int> alreadyHeroList = new List<int>();

    //所有英雄列表
    public static Dictionary<long, HeroNode> allHeroDic = new Dictionary<long, HeroNode>();
    //public static Dictionary<long, HeroNode> allHeroNodeDic = new Dictionary<long, HeroNode>();

    public static bool isUpdate = false;

    public static int LastMapID = 0;
    public static int LastMapType = 1;

    //选择到的英雄卡牌
    public static HeroNode selectHero;

    //选择出战的英雄
    public static Dictionary<long, bool> playDic = new Dictionary<long, bool>();

    // Moba 1v1
    public static HeroData[] mobaMyTeam = new HeroData[4];//moba自己的英雄数据
    public static CharacterData[] mobaEnemyTeam = new CharacterData[4];//敌方的英雄数据
    // Moba 3v3
    public static HeroData[] moba3v3MyTeam1 = new HeroData[4];
    public static HeroData[] moba3v3MyTeam2 = new HeroData[4];
    public static HeroData[] moba3v3MyTeam3 = new HeroData[4];
    public static HeroData[] moba3v3EnemyTeam1 = new HeroData[4];
    public static HeroData[] moba3v3EnemyTeam2 = new HeroData[4];
    public static HeroData[] moba3v3EnemyTeam3 = new HeroData[4];

    public static HeroData[] playHeroList
    {
        get { return GetDatasFromLocalDict(fightHero); }
    }
    public static HeroData[] defendTeam
    {
        get { return GetDatasFromLocalDict(adFightHero); }
    }
    public static HeroData[] actGold
    {
        get { return GetDatasFromLocalDict(ed1FightHero); }
    }
    public static HeroData[] actExpe
    {
        get { return GetDatasFromLocalDict(ed2FightHero); }
    }
    public static HeroData[] actPower
    {
        get { return GetDatasFromLocalDict(ed3FightHero); }
    }
    public static HeroData[] actAgile
    {
        get { return GetDatasFromLocalDict(ed4FightHero); }
    }
    public static HeroData[] actIntel
    {
        get { return GetDatasFromLocalDict(ed5FightHero); }
    }
    public static HeroData[] challengeTeam
    {
        get { return GetDatasFromLocalDict(arenaFightHero); }
    }

    static HeroData[] GetDatasFromLocalDict(int[] ids)
    {
        HeroData[] ret = new HeroData[ids.Length];
        for (int i = 0; i < ids.Length; i++)
        {
            if (ids[i] != 0)
            {
                ret[i] = playerData.GetInstance().GetHeroDataByID(ids[i]);
            }
        }
        return ret;
    }

    //服务器取得玩家保存的队列信息
    public static int[] fightHero = new int[6];
    public static int[] adFightHero = new int[6];
    public static int[] ed1FightHero = new int[6];
    public static int[] ed2FightHero = new int[6];
    public static int[] ed3FightHero = new int[6];
    public static int[] ed4FightHero = new int[6];
    public static int[] ed5FightHero = new int[6];
    public static int[] arenaFightHero = new int[6];


    // 根据场景选择出最终的出战队列
    public static HeroData[] Heros()
    {
        if (StartLandingShuJu.GetInstance().currentScene == GameLibrary.LGhuangyuan)
        {
            SceneBaseManager.instance.sceneType = SceneType.FD_LG;
        }
	
		if (SceneBaseManager.instance != null)
        {
            switch (SceneBaseManager.instance.sceneType)
            {
                case SceneType.ACT_GOLD:
                    return actGold;
                case SceneType.ACT_EXP:
                    return actExpe;
                case SceneType.ACT_POWER:
                    return actPower;
                case SceneType.ACT_INTEL:
                    return actIntel;
                case SceneType.ACT_AGILE:
                    return actAgile;
                case SceneType.MB1:
                    return mobaMyTeam;
                case SceneType.MB3:
                    return moba3v3MyTeam1;
                case SceneType.MB5:
                    return moba3v3MyTeam1;
                case SceneType.PVP3:
                    return challengeTeam;
                default:
                    if (null == playHeroList[0] || playHeroList[0].id == 0)
                        playHeroList[0] = new HeroData(GameLibrary.player);
                    return playHeroList;
            }
        }
        else
        {
            if (null == playHeroList[0] || playHeroList[0].id == 0)
                playHeroList[0] = new HeroData(GameLibrary.player);
            return playHeroList;
        }
    }

    //public static HeroData[] PlayerHeros = new HeroData[6];
    //public static HeroAttrNode[] SkillHeroList = new HeroAttrNode[3];

    //选择出战的英雄
    public static List<HeroNode> heroPlay = new List<HeroNode>();

    public static Dictionary<long, List<SkillData>> Obj = new Dictionary<long, List<SkillData>>();

    public static bool isC = false;
    public static bool isDetails = false;
    public static byte heroEntrance = 0;//英雄信息入口 1： 点击英雄卡牌 2：英雄装备培养

    public static bool isFB = false;
    public static bool isLoadOutCity = false;//是否是从野外回主城
	//public static bool isEnterScence = false;
    //卡牌信息
    public static Dictionary<long, PlayerInfo> heroInfoDic = new Dictionary<long, PlayerInfo>();

    //背包
    public static int seletIndex = 0;
    public static int seletTagIndex = 0;
    public static bool isUpdateBag = false;
    public static bool isRefresh = false;
    public static List<BackpackEquipVO> alreadyGoodSList = new List<BackpackEquipVO>();
    public static object[] allObj;

    //聊天
    public static ChatType selectChatChannel = ChatType.WorldChat;//当前选择的聊天频道
    public static bool playerCanSpeak = false;//玩家是否可以发言
    public static bool isHaveTeam = false;//玩家是否在队伍中
    public static bool isHaveSociety = false;//玩家是否加入公会
    public static bool isHavePrivateTarget = false;//玩家是否选择私聊对象
    public static string chatingPlayerNickName = "";//私聊对象的名称
    public static int worldChatUnReadCount = 0;
    public static int societyChatUnReadCount = 0;
    public static int privateChatUnReadCount = 0;
    public static int nearbyChatUnReadCount = 0;
    public static int troopsChatUnReadCount = 0;
    public static int systemChatUnReadCount = 0;
    public static long privateChatPlayerId = 0;//私聊玩家ID  
    public static long privateChatPlayerAId = 0;
    public static List<long> troopsPlayerIdList = new List<long>();//队伍玩家ID列表
    public static List<long> societyPlayerIdList = new List<long>();//公会玩家ID列表
    public static List<long> nearbyPlayerIdList = new List<long>();//附近玩家ID列表

    //邮箱
    public static int selectMailIndex = 0;// 当前点击的邮箱索引
    public static long selectMailId = 0;//当前点击的邮件ID
    public static List<long> readMailIdList = new List<long>();//阅读邮件id列表

    //称号
    public static int selectTitleIndex = -1;//称号列表默认不选中 -1
    public static int selectTitleTypeIndex = 0;//当前选择的称号类型索引
    public static long selectTitleId = 0;

    //背包吃药水
    public static int currentCount = 0;//当前选择的药水数量
    public static bool isGetNewHero = false;
    public static bool canGetHeroInfos = false;
    public static bool isSaleSingleGood = false;
    public static bool isExternalChangeGoods = false;
    public static List<SkillData> bingNvObj = new List<SkillData>();
    public static List<SkillData> juMoObj = new List<SkillData>();
    public static List<SkillData> shengQiObj = new List<SkillData>();
    public static List<SkillData> shaWangObj = new List<SkillData>();
    public static List<SkillData> xiaoXiaoObj = new List<SkillData>();

    public static bool isInFuHero(long heroid)
    {
        for (int i = 0; i <= 3; i++)
        {
            if (playHeroList[i].id == heroid)
                return true;
        }
        return false;
    }


    //角斗场
    public static bool ArenaRkisOne = false;
    public static List<List<ArenaHero>> arenaHero = new List<List<ArenaHero>>();
    public static int ArenaPlaterID;

    //角斗场敌方阵容
    public static HeroData[] ArenaEnemy = new HeroData[6];
    public static ArenaHero arenahero;
    public static int ArenaIsWin = -1;
    public static int mrk = 0;
    public static int drk = 0;
    public static int dPhoto;
    public static int maxRk;
    public static int diamond;
    public static int ArenaState = 1;

    //返回主程打开界面
    public static object[] backPanelParameter;

    public static void InitHeroSort(HeroData[] herolist)
    {
        List<HeroData> hero = new List<HeroData>();
        for (int i = 0; i < herolist.Length; i++)
        {
            if (null != herolist[i] && herolist[i].id != 0)
                hero.Add(herolist[i]);
        }
        //hero.Sort(new ArenaHeroSort());
        HeroData hd = null;
        for (int i = 0; i < 3; i++)
        {
            hd = i < hero.Count ? hero[i] : null;
            herolist[i] = hd;
        }
    }

    public static void InitHeroSort(List<HeroData> herolist)
    {
        HeroData[] hero = new HeroData[herolist.Count];
        for (int i = 0; i < herolist.Count; i++)
        {
            hero[i] = herolist[i];
        }
        InitHeroSort(hero);
        for (int i = 0; i < hero.Length; i++)
        {
            herolist[i] = hero[i];
        }
    }

    #region 出场特效
    static Dictionary<string, GameObject> effectDic = new Dictionary<string, GameObject>();
    public static void AddAppearedEffect(string name, GameObject effect)
    {
        if (!effectDic.ContainsKey(name))
            effectDic.Add(name, effect.gameObject);
    }
    public static GameObject GetAppearedEffect(string name)
    {
        if (effectDic.ContainsKey(name))
        {
            if (null == effectDic[name])
            {
                effectDic.Remove(name);
                return null;
            }
            else
            {
                return effectDic[name];
            }
        }
        else
            return null;
    }
    public static void DestroyAppearedEffect()
    {
        foreach (GameObject effect in effectDic.Values)
        {
            if (null != effect)
                GameObject.Destroy(effect.gameObject);
        }
        effectDic.Clear();
    }
    #endregion

    public static int GetStar(int[] star)
    {
        int totalStar = -1;
        for (int i = 0; i < star.Length; i++)
        {
            if (star[i] > -1) totalStar = 0;
        }
        for (int i = 0; i < star.Length; i++)
        {
            if (star[i] != -1) totalStar += star[i];
        }
        return totalStar;
    }

    public static bool isOpenLevel = false;
	public static bool isOverBtn = true;//控制快速通关按钮


    public static bool isOpenSend = false;//排行榜记录是否请求状态
    public static bool isOpenLevelSend = false;

    public static bool isFightGuide = false;
    public static string FightGuideSceneName = "";

    public static int openSceenID = 0;
}