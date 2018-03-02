using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tianyu;
using UnityEngine.SceneManagement;


/// <summary>
/// 禁止 Ctrl K + D !!!
/// 禁止 Ctrl K + D !!!
/// 禁止 Ctrl K + D !!!
/// 禁止 Ctrl K + D !!!
/// 禁止 Ctrl K + D !!!
/// 禁止 Ctrl K + D !!!
/// 禁止 Ctrl K + D !!!
/// 禁止 Ctrl K + D !!!
/// </summary>
public class ItemNodeState
{
    public short[] propertylist = new short[18];
    public long props_id;//道具id
    public string name;
    public int types;
    public string describe;
    public int grade;
    public long next_grade;
    public int[] cprice;//买入价格 [金币,钻石,龙鳞硬币,角斗士硬币,兄弟会币]
    public int sprice;
    public short power;
    public short intelligence;
    public short agility;
    public short hp;
    public short attack;
    public short armor;
    public short magic_resist;
    public short critical;
    public short dodge;
    public short hit_ratio;
    public short armor_penetration;
    public short magic_penetration;
    public short suck_blood;
    public short tenacity;
    public short movement_speed;
    public short attack_speed;
    public short striking_distance;
    public short hp_regain; //生命恢复
    public short skill_point;
    public short exp_gain;
    public short power_add;
    public long[] be_equip;
    public long[] be_synth;
    public long[,] syn_condition;
    public int syn_cost;
    public int[] drop_fb;
    public string icon_atlas;
    public string icon_name;
    public int released;
   // public int lv_limit;
    public int piles;
}
public class GameLibrary
{
    public static bool serverInit = false;
    public static bool clientInitRet = false;
    public static bool heroListReq = false;//英雄列表已经申请
    public static long scripid;
    public static short typeid;
    public static short stepid;
    public static GameLibrary mSingleton;
    public static GameLibrary Instance()
    {
        if (mSingleton == null)
            mSingleton = new GameLibrary();
        return mSingleton;
    }

    public static bool isSkipingScene = false;//通过传送门进行跳转场景时先修改这个状态
    public bool isLoadOtherPepole = true;
    public byte PackedCount = 0;//为了解决网络包体过大，代理函数会调用2次的问题添加的控制变量
    //角色
    public static long player = 201000500;
    public static long emeny = 201001900;
    //昵称
    public static string nickName = "";
    public int scenceid;
    public static byte playerstate = 0;//0正常游戏，1跳转场景状态，2界面控制状态加血敌方受击 主角受击
    public static bool isNetworkVersion = false;
    public static bool isPracticeMoba = false;
    public Dictionary<long, ItemNodeState> ItemStateList = new Dictionary<long, ItemNodeState>();//物品数据表

    private static Dictionary<long, SkillNode> _skillNodeList;
    public static Dictionary<long, SkillNode> skillNodeList
    {
        get
        {
            if (_skillNodeList == null)
            {
                _skillNodeList = FSDataNodeTable<SkillNode>.GetSingleton().DataNodeList;
            }
            return _skillNodeList;
        }
    }

    private static List<SkillBuffNode> _skillBuffList;
    public static List<SkillBuffNode> skillBuffList
    {
        get
        {
            if (_skillBuffList == null)
            {
                _skillBuffList = new List<SkillBuffNode>(FSDataNodeTable<SkillBuffNode>.GetSingleton().DataNodeList.Values);
            }
            return _skillBuffList;
        }
    }

    private static Dictionary<long, UIMountNode> _mountNodeList;
    public static Dictionary<long, UIMountNode> mountNodeList
    {
        get
        {
            if (_mountNodeList == null)
            {
                _mountNodeList = FSDataNodeTable<UIMountNode>.GetSingleton().DataNodeList;
            }
            return _mountNodeList;
        }
    }

    private static Dictionary<long, PetNode> _PetNodeList;
    public static Dictionary<long, PetNode> PetNodeList
    {
        get
        {
            if (_PetNodeList == null)
            {
                _PetNodeList = FSDataNodeTable<PetNode>.GetSingleton().DataNodeList;
            }
            return _PetNodeList;
        }
    }

    //英雄队列
    public static List<string> heroTeam;//= new List<string>();

    //英雄队列
    public static List<long> playHero = new List<long>();

    public const float DIE_DESTROY_DELAY = 2f;
    public const int mPlayDelay = 3, mTowerDelay = 5, mMonsterDelay = 7;

    public const string UI_Login = "UILogin";
    private static Texture dissolveTexture;
    //上一个场景
    public static string LastScene = "";
    //登录，主城
    public const string Scene_Login = "UI_Login";
    public const string UI_Major = "UI_MajorCity01";
    public const string Scene_XinShou = "Xinshou";

    //野外副本PVP
    public const string LGhuangyuan = "LGhuangyuan";
    public const string Anmaodao_ES1 = "Anmaodao_ES1";
    public const string Anmaodao_KM1 = "Anmaodao_KM1";
    public const string Anmaodao_KM2 = "Anmaodao_KM2";
    public const string Anmaodao_TD1 = "Anmaodao_TD1";
    public const string Shiguangsenlin_KM1 = "Shiguangsenlin_KM1";
    public const string DaYing_KM1 = "DaYing_KM1";
    public const string Dixialongmu_KM1 = "Dixialongmu_KM1";
    public const string Dixialongmu_KM2 = "Dixialongmu_KM2";
    public const string Dixialongmu_KM3 = "Dixialongmu_KM3";
    public const string Ltya_ES1 = "Ltya_ES1";
    public const string Ltya_KM1 = "Ltya_KM1";
    public const string Ltya_KM2 = "Ltya_KM2";
    public const string Ltya_TD1 = "Ltya_TD1";
    public const string Moba1V1_TP1 = "PVP_xue";
    public const string NorthCity_KM1 = "NorthCity_KM1";
    public const string NorthCity_KV1 = "NorthCity_KV1";
    public const string PVP_Zuidui = "Juedouchang";
    public const string PVP_Moba = "PVP_xue";
    public const string PVP_1V1 = "PVP_1V1";
    public const string PVP_Moba3v3 = "Moba3V3_2";
    public const string PVP_Moba5v5 = "Moba5V5";
    public const string ACT_Bingfenggu = "ACT_Bingfenggu";
    public const string ACT_Dixialongcheng = "ACT_Dixialongcheng";
    public const string ACT_Yegesl = "ACT_Yegesl";

    public static bool isMoba = false;
    public static bool isPVP3 = false;


    //UI界面
    public static string UIHeroShow = "UIHeroShow";
    public const string UI_SelectServer = "UISelectServer";
    public const string UI_Embattle = "UIEmbattle";//布阵
    public const string UI_ArenaMode = "UIArenaModePanel";     //竞技场模式选择
    public const string UI_ServerList = "UIServerList";
    public const string UI_CreateRole = "UICreateRole";
    public const string UISetting = "UI_Setting";                //主城设置
    public const string UIRole = "UIRole";                      //角色头像
    public const string UIRoleInfo = "UIRoleInfo";               //角色信息面板
    public const string ChangeIcon = "ChangeIcon";               //头像
    public const string ChangeIconBorder = "ChangeIconBorder";   //头像框
    public const string ChangeName = "ChangeName";               //改名
    public const string UIShop = "UIShopPanel";                      //商店界面
    public const string UIPopRefresh = "UIPopRefresh";          //商城刷新框
    public const string UIPopBuy = "UIPopBuy";                  //商城购买框
    public const string UIShopSell = "UIShopSell";              //商城出售金币道具框
    public const string UIMask = "UIMask";                      //遮罩
    public const string UILottery = "UILottery";                //抽奖界面
    public const string UI_PopBuy = "UIPopBuy";                  //商城购买框
    public const string UI_ShopSell = "UIShopSell";              //商城出售金币道具框
    public const string UI_Mask = "UIMask";                      //遮罩
    public const string UI_Lottery = "UILottery";                //抽奖界面
    public const string UISoulLottery = "UISoulLottery";        //魂匣
    public const string UIResultLottery = "UIResultLottery";    //恭喜获得界面
    public const string UIResultSoul = "UIResultSoul";          //魂匣恭喜获得界面
    public const string UILottryEffect = "UILottryEffect";    //抽奖过场动画
    public const string UIHeroList = "UIHeroList";             //英雄列表界面
    public const string UI_HeroDetail = "UIHeroDetail";         //英雄信息界面
    public const string UI_HeroPlay = "UIHeroPlay";             //英雄出战界面
    public const string UILevel = "UILevel";                   //英雄出战界面
    public const string UIPopLottery = "UIPopLottery";          //恭喜获得弹出界面
    public const string UIBloodScreen = "UIBloodScreen";        //血红闪屏
    public const string UI_PreviewEquip = "UIPreviewEquip";     //装备一览
    public const string UIWaitForSever = "UIWaitForSever";     //等待服务器消息读条界面
    public const string UI_TaskList = "UITaskList";     //任务列表界面
    public const string UI_Dialogue = "UIDialogue";     //NPC对话界面
    public const string UITaskInfoPanel = "UITaskInfoPanel";//任务查看界面
    public const string UIFriends = "UIFriends";//好友界面
    public const string UIFriendTip= "UIFriendTip";//搜索好友
    public const string UITaskRewardPanel = "UITaskRewardPanel";//任务查看界面
    public const string UIPause = "UIPause";                    //游戏暂停界面
    public const string UIKnapsack = "UIKnapsack";              //物品背包界面
    public const string UIBattleWin = "UIBattleWin";            //胜利界面
    public const string UIUpgradePanel = "UpgradePanelX";        //升级界面
    public const string UIRisingStarPanel = "RisingStarPanel";  //升星界面
    public const string UIGetWayPanel = "GetWayPanel";          //获取界面
    public const string UISkillAndGoldHintPanel = "UISkillAndGoldHintPanel";      //购买点数提示面板
    public const string UIEquipInfoPanel = "EquipInfoPanel";    //装备信息面板
    public const string UIPromptBox = "UIPromptBox";            //提示框
    public const string UISign_intBox = "UISign_intBox";            //提示框   
    public const string UIPromptPanel = "PromptPanel";          //养成系统提示框
    public const string UISalePanel = "UISalePanel";            //背包出售物品界面
    public const string UIGoodsDetials = "GoodsDetials";
    public const string UIBuyEnergyVitality = "UIBuyEnergyVitality";//购买体力和活力提示面板
    public const string UICountdownPanel = "UICountdownPanel";//体力和活力倒计时信息
    public const string Upgrade = "Upgrade";                //角色升级界面
    public const string UIGoldHand = "UIGoldHand";            //点金手界面
    public const string UITheBattlePanel = "TheBattlePanel";  //战斗结束
    public const string UIPvP = "UIPVP";
    public const string UIAbattiorLis = "UIAbattiorList";
    public const string UIActivities = "UIActivities";//活动界面
    public const string UIActivity = "UIActivity";
    public const string UI_TaskTracker = "UITaskTracker";        //任务追踪
    public const string UI_Money = "UIMoney";                   //主城货币
    public const string UI_Shop = "UIShop";                      //商店界面
    public const string UI_Register = "UIRegister";
    public const string UIHeroUseExp = "UIHeroUseExp";      //使用药水升级英雄
    public const string UIRankList = "UIRankList";          //排行榜界面
    public const string UIMarquee = "UIMarquee";            //跑马灯
    public const string UIMailPanel = "UIMailPanel";        //邮箱界面
    public const string UIChatPanel = "UIChatPanel";        //聊天界面
    public const string UIMail = "UIMail";                  //主城邮箱按钮
    public const string UIChat = "UIChat";                  //主城聊天按钮
    public const string UIPlayerInteractionPort = "UIPlayerInteractionPort";//玩家交互接口界面
    public const string UIPlayerTitlePanel = "UIPlayerTitlePanel";//玩家称号面板
    public const string UIWelfare = "UIWelfare";
    public const string UITooltips = "UITooltips";
    public const string UIgoodstips = "UIgoodstips";
    public const string UIUpgradeGiftBag = "UIUpgradeGiftBag";//等级奖励面板
    public const string UITaskCollectPanel = "UITaskCollectPanel";//采集进度条面板
    public const string UITaskTracker = "UITaskTracker";
    public const string UIGoodsGetWayPanel = "UIGoodsGetWayPanel";//物品获取路径面板
    public const string UITaskUseItemPanel = "UITaskUseItemPanel";//使用道具任务面板
    public const string UIGameAffiche = "UIGameAffiche";//公告任务面板
    public const string SceneEntry = "SceneEntry";//副本界面
    public const string UIFubenTaskDialogue = "UIFubenTaskDialogue";//副本任务对话框
    public const string UIBoxGoodsTip = "UIBoxGoodsTip";//宝箱物品展示
    public const string UIDeadToReborn = "UIDeadToReborn";
    public const string NpcUIControl = "NpcUIControl";//npcUI管理面板
    public const string UIUseExpVialPanel = "UIUseExpVialPanel";//背包使用经验药水面板
    public const string UINotJoinSocietyPanel = "UINotJoinSocietyPanel";
    public const string UIHaveJoinSocietyPanel = "UIHaveJoinSocietyPanel";
    public const string UISocietyInfoPanel = "UISocietyInfoPanel";
    public const string EditSocietyInfoPanel = "EditSocietyInfoPanel";
    public const string UIMountAndPet = "UIMountAndPet";//宠物坐骑界面
    public const string UILoading = "UILoading";//加载界面
    public const string UIUseEnergyItemPanel = "UIUseEnergyItemPanel";
    public const string UIGuidePanel = "UIGuidePanel";
    public const string NextGuidePanel = "NextGuidePanel";
    public const string UISocietyIconPanel = "UISocietyIconPanel";
    public const string EquipDevelop = "EquipDevelop";
    public const string EquipStrengthePanel = "EquipStrengthePanel";
    public const string EquipHeroListPanel = "EquipHeroListPanel";
    public const string UIExpPropPanel = "UIExpPropPanel";
    public const string UIEquipDetailPanel = "UIEquipDetailPanel";
    public static string UICreateName = "UICreateName";
    public const string UITaskEffectPanel = "UITaskEffectPanel";


    public const string UIFlopCardPanel = "FlopPanel";
    public const string UIArenaWinPanel = "ArenaWinPanel";
    public const string UIGamePromptPanel = "GamePrompt";
    public const string UIMobaStatInfo = "MobaStatInfo";
    public const string UIExptips = "UIExptips";
    public const string UISocietyInteractionPort = "UISocietyInteractionPort";
    public const string LoadSceneName = "";
    public static string PATH_SCENE = "Prefab/Scenes/";
    public static string PATH_UIPrefab = "Prefab/UIPanel/";

    //Item路径
    public const string Item = "Prefab/Item/";

    //英雄特效
    public const string Effect_Hero = "Effect/Prefabs/Heros/";

    //NPC特效
    public const string Effect_NPC = "Effect/Prefabs/NPC/";

    //怪物特效
    public const string Effect_Monster = "Effect/Prefabs/monster/";

    //buff特效
    public const string Effect_Buff = "Effect/Prefabs/Buff/";

    //UI特效
    public const string Effect_UI = "Effect/Prefabs/UI/";

    //游戏音效路径
    public const string Resource_Music = "Sound/BGM/";
    public const string Resource_Sound = "Sound/";
    public const string Resource_UISound = "Sound/UI/";
    public const string Resource_GuideSound = "Sound/Guide/";

    //刷新怪物特效
    public const string Effect_Spawn = "Effect/Prefabs/NPC/shuaxin/";

    //建筑特效
    public const string Effect_Build = "Effect/Prefabs/Build/";

    //boss出场特效
    public const string Effect_Boss = "Effect/Prefabs/Monster/Boss/";
    //boss出场
    public static bool isBossChuChang = false;

    public const string Hero_URL = "Prefab/Character/Heros/";
    public const string Monster_URL = "Prefab/Character/Monster/";
    public const string NPC_URL = "Prefab/Character/NPC/";
    public const string Pet_URL = "Prefab/Character/Pet/";
    public const string Mount_URL = "Prefab/Character/Mount/";

    public static GameObject TouchObject = null;
    public static bool AttackAction = false;


    public static bool isInitData = true;
    public static bool isShowPlay = false;
    public static bool isShowPlayToFB = false;
    public bool isInitHeroList = false;
    //选择的副本
    public static string chooseFB;

    //选择的地图ID
    public static int mapId = 1;

    //副本ID
    public static int dungeonId = 0;

    //副本类型
    public static int dungeonType = 1;

    //开始结束时间
    public static long starTime = 0;
    public static long endTime = 300;
    public static int[] star = new int[3];
    public static Dictionary<int, bool> meetStar = new Dictionary<int, bool>();
    public static Dictionary<long, int> receiveGoods = new Dictionary<long, int>();
    public static int receiveGolds = 0;

    public static Dictionary<int, int> dungeonStar = new Dictionary<int, int>();
    public static int totalStar = 0;
    public static int[][] box;
    //副本相关数据
    public static Dictionary<int, Dictionary<int, int[]>> mapOrdinary = new Dictionary<int, Dictionary<int, int[]>>();//副本  精英，副本 《大关id，小关列表》
    public static Dictionary<int,int[]> mapElite = new Dictionary<int, int[]>();//精英本
    public static Dictionary<int, int[][]> mapBox = new Dictionary<int, int[][]>();//

    public static int UseActionPoint = 0;

    public static Dictionary<int, int> eventdList = new Dictionary<int, int>();
    public static Dictionary<int, List<int[]>> eventsList = new Dictionary<int, List<int[]>>();//试练
    public static int[] eventOpen;

    public static object[] ArenaNumber;//角斗场次数信息

    public static bool SceneType ( SceneType type )
    {
        return null != SceneBaseManager.instance && SceneBaseManager.instance.sceneType == type;
    }

    public static bool IsMajorOrLogin()
    {
        if (StartLandingShuJu.GetInstance() != null)
            return StartLandingShuJu.GetInstance().currentScene == UI_Major || StartLandingShuJu.GetInstance().currentScene == Scene_Login;
        else
            return false;
    }

    public static bool IsMajorScene()
    {
        return StartLandingShuJu.GetInstance().currentScene == UI_Major;
    }

    public static bool IsLoginScene()
    {
        return StartLandingShuJu.GetInstance().currentScene == Scene_Login;
    }

    public static bool IsAppeared()
    {
        return null == HeroPosEmbattle.instance || HeroPosEmbattle.instance.type == AnimType.Appeared;
    }

    //boss血条
    public static BossBlood bossBlood;

    //自动攻击
    public static bool autoMode = false;

    public static Dictionary<int, int> skillLevel = new Dictionary<int, int>();

    public static Dictionary<long, int> skillLevelcount = new Dictionary<long, int>();

    public static List<object> saleItemList = new List<object>();
    public static Dictionary<string, string> LOGIN_TXT = new Dictionary<string, string>()
    {
        {"login_txt00001", "网络连接超时，请重新连接！"},
        {"login_txt00002", "账户未输入"},
        {"login_txt00003", "出战英雄未选择"},
        {"login_txt00004", "对不起，该昵称已经被注册，请使用其他昵称！"},
        {"login_txt00005", "对不起，该昵称内含有非法字符！"},
        {"login_txt00006", "初始化用户信息失败，请联系客服！"},
        {"login_txt00007", "加载用户信息失败，请联系客服！"},
        {"login_txt00008", "加载用户副本信息失败，请联系客服！"},
        {"login_txt00009", "操作成功！"},
        {"login_txt00010", "网络连接失败！"},
        {"login_txt00011", "获取个人信息资料失败"},
        {"login_txt00012", "登录成功！"}
    };

    public bool isReconect = false;//是否为重新登陆
    public static bool isRefresh = false;
    public static bool isBack = true;//是否可以返回战场

    public static int ItemTime = 0;
    public static bool isShow = false;//抽奖显示
    public static int lotteryCount = 0;//再次可执行点击的标记
    public static bool isSendPackage = false;//发消息并等待服务器回复(默认当前没有消息发送)
    public static bool isInit = false;//UI界面是否已经加载过
    //队友列表
    public static List<HeroData> partnerList = new List<HeroData>();
    public static List<HeroData> enemyList;
    //{
    //    "yx_002","yx_003","yx_005"
    //};


    public STATUS SetIdleStatusByScene(STATUS next)
    {
        if (!CheckPlayIdel() && next == STATUS.IDLE)
        {
            next = STATUS.PREPARE;
        }
        return next;
    }

    public bool CheckPlayIdel()
    {
        return StartLandingShuJu.GetInstance().currentScene == UI_Major;
    }

    public bool CheckInCopy()
    {
        return StartLandingShuJu.GetInstance().currentScene != UI_Major;
    }

    public bool CheckSceneNeedSync()
    {
        return CheckSceneNeedSync(StartLandingShuJu.GetInstance().currentScene);
    }

    public bool CheckSceneNeedSync(string mCurSceneName)
    {
        return mCurSceneName == UI_Major || mCurSceneName == LGhuangyuan;
    }

    public bool CheckNotHeroBoss(CharacterState cs)
    {
        return cs.state == Modestatus.Boss && cs.mCurMobalId == MobaObjectID.None && cs.GetMobaName() != "gw_1006";
    }

    public bool CheckIsEliteMonster(CharacterState cs)
    {
        return cs.CharData.attrNode.types == 4;
    }

    public int CalcSkillDamage (float skillFinalVal, int damageType, int lvl, CharacterState hitState, CharacterState attackerCs = null)
    {
        int result = 0;
        if (hitState == null || hitState.isDie || hitState.Invincible)
            return result;
        bool showHud = ShouldShowHud(hitState, attackerCs);
        switch ((DamageType)damageType)
        {
            case DamageType.physics:
                float battle_armor = Formula.GetSingleAttributeRate(hitState.CharData, AttrType.armor, lvl);
                result = CheckImmune(skillFinalVal * (1f - battle_armor), hitState.RecievePhysicDamage, showHud, hitState.gameObject);
                break;
            case DamageType.magic:
                float battle_magic_resist = Formula.GetSingleAttributeRate(hitState.CharData, AttrType.magic_resist, lvl);
                result = CheckImmune(skillFinalVal * (1f - battle_magic_resist), hitState.RecieveMagicDamage, showHud, hitState.gameObject);
                break;
            case DamageType.fix:
                result = CheckImmune(skillFinalVal, hitState.RecieveFixDamage, showHud, hitState.gameObject);
                break;
            default:
                break;
        }
        return result;
    }

    public int CheckImmune(float result, bool immuneType, bool showHud, GameObject gameObject)
    {
        if (!immuneType)
        {
            if (showHud) HUDAndHPBarManager.instance.HUD(gameObject, 0, HUDType.Immune);
            return 0;
        }
        else
        {
            return Mathf.CeilToInt(result);
        }
    }
    //判断是否显示飘字
    public bool ShouldShowHud(CharacterState hitState, CharacterState attackerCs)
    {
        return hitState.state == Modestatus.Player ||
            ( attackerCs != null && attackerCs.state == Modestatus.Player && !hitState.Invisible ) || ( attackerCs != null &&
            attackerCs.state == Modestatus.SummonHero && attackerCs.Master != null && attackerCs.Master.state == Modestatus.Player );
    }

    public void SetSkillDamageCharaData(ref CharacterData characterData, SkillNode skillNode, CharacterState attackerCs)
    {
        characterData = attackerCs.CharData.Clone();
        characterData.state = attackerCs.state;
        characterData.groupIndex = attackerCs.groupIndex;
        if (skillNode.add_state != null)
        {
            characterData.skill_Damage = new float[skillNode.add_state.Length + 1];
            characterData.skill_Damage [0] = skillNode.GetSkillBattleValue(0, attackerCs.CharData);
            for (int i = 0; i < skillNode.add_state.Length; i++)
            {
                object o = skillNode.add_state[i];
                if (o != null && o is System.Array && ((System.Array)o).Length > 0)
                {
                    characterData.skill_Damage [i + 1] = skillNode.GetSkillBattleValue(i + 1, attackerCs.CharData);
                }
            }
        }
    }

    private List<CharacterState> GetCsAttackTargetBySkillNode(SkillNode skill, CharacterState cs)
    {
        List<CharacterState> mCurCalTarget = new List<CharacterState>();
        if (SceneBaseManager.instance != null)
        {
            for (int i = 0; i < SceneBaseManager.instance.agents.size; i++)
            {
                CharacterState chs = SceneBaseManager.instance.agents[i];
                if (CheckHitCondition(skill, cs, chs))
                {
                    if (cs != null && chs != null)
                    {
                        float dis = Vector3.Distance(cs.transform.position, chs.transform.position);
                        if (chs != null && (skill.dist == 0 || dis < GetSkillDistBySkillAndTarget(cs, skill)) && !chs.Invincible && IsInvisiblityCanSetTarget(cs, chs))
                        {
                            if (chs == cs && !(skill.target == TargetState.None && skill.dist == 0)) continue;
                            mCurCalTarget.Add(chs);
                        }
                    }
                }
            }
        }
        return mCurCalTarget;
    }

    public void SetCsAttackTargetBySkillNode(SkillNode skill, CharacterState cs)
    {
        if (SceneBaseManager.instance == null) return;
        CharacterState target = null;
        float disFinal = float.MaxValue;
        for (int i = 0; i < SceneBaseManager.instance.agents.size; i++)
        {
            CharacterState chs = SceneBaseManager.instance.agents[i];
            if (chs != null && CheckHitCondition(skill, cs, chs))
            {
                float dis = Vector3.Distance(cs.transform.position, chs.transform.position);
                if (chs != null && dis < (CheckIsWildCs(chs) ? cs.AttackRange : cs.TargetRange) && !chs.Invincible && IsInvisiblityCanSetTarget(cs, chs))
                {
                    if (chs == cs && !(skill.target == TargetState.None && skill.dist == 0)) continue;
                    if (target == null)
                        target = chs;

                    if (disFinal > dis)
                    {
                        disFinal = dis;
                        target = chs;
                    }

                    if (BattleUtil.IsHeroTarget(chs))
                    {
                        target = chs;
                        break;
                    }
                }
            }
        }
        cs.SetAttackTargetTo(target);
    }

    public void SetCsAttackTargetByChoseTarget(SkillNode skill, CharacterState cs)
    {
        if (SceneBaseManager.instance == null) return;
        CharacterState target = null;
        List<CharacterState> mCurCalTarget = GetCsAttackTargetBySkillNode(skill, cs);
        if (mCurCalTarget.Count > 0)
        {
            mCurCalTarget.Sort((a, b) =>
            {
                float aDis = Vector3.Distance(cs.transform.position, a.transform.position);
                float bDis = Vector3.Distance(cs.transform.position, b.transform.position);
                return Mathf.FloorToInt(aDis - bDis);
            });
            switch (skill.choseTarget)
            {
                case ChoseTarget.none:
                    CharacterState mCurTarget = null;
                    if (cs.mCurMobalId == MobaObjectID.HeroShengqi && skill.site == 2)
                    {
                        mCurCalTarget.Sort((a, b) => a.currentHp - b.currentHp);
                        mCurTarget = mCurCalTarget [0];
                    }
                    else
                    {
                        mCurTarget = mCurCalTarget.Find(chs => BattleUtil.IsHeroTarget(chs));
                    }
                    target = mCurTarget == null ? mCurCalTarget[0] : mCurTarget;
                    break;
                case ChoseTarget.random:
                    target = mCurCalTarget[Random.Range(0, mCurCalTarget.Count)];
                    break;
                case ChoseTarget.farthest:
                    target = mCurCalTarget[mCurCalTarget.Count - 1];
                    break;
                default:
                    break;
            }
        }
        cs.SetAttackTargetTo(target);
        if (cs.state == Modestatus.Player)
        {
            CharacterManager.instance.ChangePlayerAttackTargetTo(target);
        }
    }

    public bool CheckHitCondition(SkillNode mCurSkillNode, CharacterState cs, CharacterState target)
    {
        bool result = false;
        if (mCurSkillNode.influence_type != null)
        {
            for (int i = 0; i < mCurSkillNode.influence_type.Length; i++)
            {
                switch ((influence_type)mCurSkillNode.influence_type[i])
                {
                    case influence_type.self:
                        result |= target.groupIndex == cs.groupIndex && target == cs;
                        break;
                    case influence_type.selfHero:
                        result |= target.groupIndex == cs.groupIndex && target != cs && BattleUtil.IsHeroTarget(target);
                        break;
                    case influence_type.selfMonster:
                        result |= target.groupIndex == cs.groupIndex && target != cs && target.state == Modestatus.Monster;
                        break;
                    case influence_type.enemyHero:
                        result |= target.groupIndex != cs.groupIndex && BattleUtil.IsHeroTarget(target);
                        break;
                    case influence_type.enemyMonster:
                        result |= target.groupIndex != cs.groupIndex && (target.state == Modestatus.Monster || target.state == Modestatus.Boss);
                        break;
                    case influence_type.enemyTower:
                        result |= target.groupIndex != cs.groupIndex && target.state == Modestatus.Tower;
                        break;
                    case influence_type.enemyBase:
                        result |= target.groupIndex != cs.groupIndex && target.state == Modestatus.Tower;
                        break;
                    default:
                        break;
                }
            }
        }
        return result;
    }

    //隐身buff普攻或者释放技能隐身buff消失
    public void BrokenInvisibility(CharacterState cs)
    {
        if (cs.Invisible)
        {
            List<SkillBuff> mCurSkillBuff = SkillBuffManager.GetInst().GetSkillBuffListByCs(cs).FindAll(a => (SkillBuffType)a.id == SkillBuffType.Invisible);
            for (int i = 0; i < mCurSkillBuff.Count; i++)
            {
                SkillBuffManager.GetInst().RemoveCalculateCurTargetProp(cs, mCurSkillBuff[i]);
            }
        }
    }

    public bool CheckInvisiblityCanReveal(CharacterState cs)
    {
        if (cs.Invisible)
        {
            List<SkillBuff> mCurSkillBuff = SkillBuffManager.GetInst().GetSkillBuffListByCs(cs).FindAll(a => (SkillBuffType)a.id == SkillBuffType.Invisible);
            if (mCurSkillBuff.Count > 0)
            {
                return (mCurSkillBuff[mCurSkillBuff.Count - 1] as BF_Invisible).CanReveal;
            }
            else
            {
                return false;
            }
        }
        return false;
    }

    //隐身单位是否能当成目标选择
    public bool IsInvisiblityCanSetTarget(CharacterState cs, CharacterState target)
    {
        return !(target.groupIndex != cs.groupIndex && target.Invisible);
    }

    public void SetCsInvisible(CharacterState cs, bool b, SkinnedMeshRenderer[] skinned)
    {
        if (b)
        {
            SkillBuffManager.GetInst().GetSkillBuffListByCs(cs).ForEach(a =>
            {
                if (a.buffGo != null)
                {
                    a.buffGo.gameObject.SetActive(false);
                }
            });
        }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                SkillBuffManager.GetInst().SetBuffActive(cs, i);
            }
        }
        for (int i = 0; i < skinned.Length; i++)
        {
            if (skinned[i] != null)
            {
                skinned[i].enabled = !b;
            }
        }
        if (cs == CharacterManager.playerCS && CharacterManager.instance.redCircle != null && CharacterManager.instance.redCircle.gameObject.activeSelf)
        {
            CharacterManager.instance.redCircle.gameObject.SetActive(!b);
        }
        if (cs.mShadowTrans != null)
        {
            cs.mShadowTrans.gameObject.SetActive(!b);
        }
        cs.SetPartEffectActive(cs, cs.mShowPart, !b);
        cs.SetHpBarEnable(!b);
    }

    public void CheckLockTargetCastSkillValid(CharacterState cs, SkillNode mCurSkillNode)
    {
        if (cs.attackTarget != null)
        {
            if (!CheckHitCondition(mCurSkillNode, cs, cs.attackTarget))
            {
                SetCsAttackTargetBySkillNode(mCurSkillNode, cs);
            }
        }
        else
        {
            SetCsAttackTargetBySkillNode(mCurSkillNode, cs);
        }
    }

    public void AfreshTargetCastSkill(CharacterState cs, SkillNode mCurSkillNode)
    {
        if (cs.attackTarget != null)
        {
            if (CheckHitCondition(mCurSkillNode, cs, cs.attackTarget))
            {
                if ((cs.mCurMobalId == MobaObjectID.HeroJiansheng && mCurSkillNode.site == 4 && cs.pm.mCurSkillTime != 0)
                     || (cs.mCurMobalId == MobaObjectID.HeroShengqi && mCurSkillNode.site == 2))
                {
                    SetCsAttackTargetByChoseTarget(mCurSkillNode, cs);
                }
                else if (mCurSkillNode.dist != 0 && Vector3.Distance(cs.transform.position, cs.attackTarget.transform.position) >= GetSkillDistBySkillAndTarget(cs, mCurSkillNode))
                {
                    SetCsAttackTargetByChoseTarget(mCurSkillNode, cs);
                }
            }
            else
            {
                SetCsAttackTargetByChoseTarget(mCurSkillNode, cs);
            }
        }
        else
        {
            SetCsAttackTargetByChoseTarget(mCurSkillNode, cs);
        }
    }

    public SkillNode GetCurrentSkillNodeByCs(CharacterState cs, int skillIndex)
    {
        SkillNode mCurSkillNode = null;
        for (int i = 0; i < cs.CharData.attrNode.skill_id.Length; i++)
        {
            long skillId = cs.CharData.attrNode.skill_id[i];
            if (skillNodeList[skillId].site == skillIndex)
            {
                mCurSkillNode = skillNodeList[skillId];
                break;
            }
        }
        return mCurSkillNode;
    }

    public bool CheckIsWildCs(CharacterState chs)
    {
        return chs.groupIndex == 99 || chs.groupIndex == 100 || chs.groupIndex == 101;
    }

    public bool CheckExistSpecialBySkillNode(SkillNode skillNode)
    {
        return skillNode.specialBuffs.Length > 0;
    }

    public static int GetHeroMobaId(string id)
    {
        switch (id)
        {
            case "yx_001":
                return (int)MobaObjectID.HeroBingnv;
            case "yx_002":
                return (int)MobaObjectID.HeroJumo;
            case "yx_003":
                return (int)MobaObjectID.HeroShengqi;
            case "yx_004":
                return (int)MobaObjectID.HeroShawang;
            case "yx_005":
                return (int)MobaObjectID.HeroXiaoxiao;
            case "yx_006":
                return (int)MobaObjectID.HeroKulouwang;
            case "yx_007":
                return (int)MobaObjectID.HeroShangjinlieren;
            case "yx_008":
                return (int)MobaObjectID.HeroTongkunvwang;
            case "yx_009":
                return (int)MobaObjectID.HeroXiaohei;
            case "yx_010":
                return (int)MobaObjectID.HeroXiongmao;
            case "yx_011":
                return (int)MobaObjectID.HeroJiansheng;
            case "yx_012":
                return (int)MobaObjectID.HeroShuiren;
            case "yx_013":
                return (int)MobaObjectID.HeroDifa;
            case "yx_014":
                return (int)MobaObjectID.HeroHuonv;
            case "yx_015":
                return (int)MobaObjectID.HeroYingci;
            case "yx_016":
                return (int)MobaObjectID.HeroChenmo;
            case "yx_017":
                return (int)MobaObjectID.HeroXiaolu;
            case "yx_018":
                return (int)MobaObjectID.HeroShenniu;
            case "yx_019":
                return (int)MobaObjectID.HeroMeidusha;
            case "yx_020":
                return (int)MobaObjectID.HeroBaihu;
            case "yx_021":
                return (int)MobaObjectID.HeroShenling;
            case "yx_022":
                return (int)MobaObjectID.HeroMori;
            case "yx_030":
                return (int)MobaObjectID.HeroHuanci;
            case "yx_033":
                return (int)MobaObjectID.HeroLuosa;
            default:
                return 0;
        }
    }

    public float GetSkillDistBySkillAndTarget(CharacterState cs, SkillNode skillNode)
    {
        float mExtendDis = skillNode.dist;
        if (cs != null && !cs.isDie && skillNode.dist != 0)
        {
            mExtendDis += GetExtendDis(cs.attackTarget);
        }
        return mExtendDis;
    }

    public float GetExtendDis(CharacterState target)
    {
        return target == null ? 0 : target.CharData.attrNode.modelNode.colliderRadius * target.transform.localScale.z;
    }

    public bool CanControlSwitch(MonsterAnimator pm)
    {
        return pm != null && pm.canControlSwitch == 0;
    }

    //神灵武士特殊技能特殊buff判断
    public bool CheckShenlingSkillIndex(CharacterState attackerCS, SkillNode skillNode, int skillIndex)
    {
        return attackerCS != null && !attackerCS.isDie && attackerCS.mCurMobalId == MobaObjectID.HeroShenling && skillNode.site == skillIndex;
    }

    //判断角色为boss_006死亡之翼
    public bool CheckModelIsBoss006(CharacterState cs)
    {
        return cs.GetMobaName().Equals("boss_006");
    }

    public static int GetAllLayer()
    {
        return 1 << (int)GameLayer.Monster | 1 << (int)GameLayer.Player | 1 << (int)GameLayer.Tower;
    }

    public static float GetMaxRayDistance()
    {
        return Camera.main.farClipPlane - Camera.main.nearClipPlane;
    }
    //品质框定义
    public static Dictionary<GradeType, string> QualityColor = new Dictionary<GradeType, string>()
    {
         { GradeType.Gray,"hui"}, {GradeType.Orange ,"cheng"},{ GradeType.Purple ,"zi"},{GradeType.Blue , "lan"},{GradeType.Green, "lv"},{GradeType.Red ,"hong"}
    };

    public static bool AddAppearedEffect(MobaObjectID mCurMobalId)
    {
        return mCurMobalId == MobaObjectID.HeroChenmo
            || mCurMobalId == MobaObjectID.HeroBingnv
            || mCurMobalId == MobaObjectID.HeroXiaolu
            || mCurMobalId == MobaObjectID.HeroHuanci
            || mCurMobalId == MobaObjectID.HeroMeidusha
            || mCurMobalId == MobaObjectID.HeroShenling;
    }

    //颜色值定义 暂时用于任务描述替换
    public const string C1 = "009944";
    public const string C2 = "e4007f";
    public const string C3 = "7d0022";
    public const string C4 = "eb6100";
    public const string C5 = "ff0000";
    public const string C6 = "00a0e9";
}
