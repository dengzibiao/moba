/*
文件名（File Name）:   UIPanleID.cs

作者（Author）:    #高#

创建时间（CreateTime）:  #CreateTime#
*/


public enum UIPanleID : int
{
    none = -1,
    /// <summary>连接中UI</summary>
    theConnectUI = 0,
    /// <summary>打开界面等待界面</summary>
    theOpenInterfaceWaitUI = 1,
    /// <summary>登陆界面</summary>
    UILogin = 2,
    UIHeroShow = 7,
    /// <summary>选服界面</summary>
    UI_SelectServer = 15,
    /// <summary>布阵</summary>
    //UI_Embattle = 16,//
    /// <summary>服务器列表界面</summary>
    UI_ServerList = 17,
    /// <summary>创建角色</summary>
    UI_CreateRole = 18,
    /// <summary>所有功能按钮界面</summary>
    UISetting = 19,
    /// <summary>玩家头像面板</summary>
    UIRole = 20,
    /// <summary>玩家信息</summary>
    UIRoleInfo = 21,
    /// <summary>改头像</summary>
    ChangeIcon = 22,
    /// <summary>改头像框</summary>
    ChangeIconBorder = 23,
    /// <summary>改名字</summary>
    ChangeName = 24,
    /// <summary>货币界面</summary>
    UIMoney = 25,
    /// <summary>商城界面</summary>
    UIShopPanel = 26,
    /// <summary>购买二级界面</summary>
    UIPopBuy = 28,
    /// <summary>出售二级界面</summary>
    UIShopSell = 29,
    /// <summary>遮罩</summary>
    UIMask = 30,
    /// <summary>抽奖界面</summary>
    UILottery = 31,
    UISoulLottery = 36,//--------------
    /// <summary>恭喜获得--钻石</summary>
    UIResultLottery = 37,
    /// <summary>抽奖特效</summary>
    UILottryEffect = 39,
    /// <summary>抽奖提示</summary>
    UIPopLottery = 44,
    UIBloodScreen = 45,//-----
    UI_PreviewEquip = 46,//-----------
    /// <summary>加载圈</summary>
    UIWaitForSever = 47,
    /// <summary>任务信息界面</summary>
    UITaskInfoPanel = 50,
    /// <summary>好友界面</summary>
    UIFriends = 51,
    /// <summary>任务奖励--恭喜获得界面</summary>
    UITaskRewardPanel = 52,
    /// <summary>暂停界面</summary>
    UIPause = 53,
    /// <summary>背包界面</summary>
    UIKnapsack = 54,
    //UIBattleWin = 55,//胜利界面
    /// <summary>玩家升级界面</summary>
    UIUpgradePanel = 56,
    //UIRisingStarPanel = 57,
    /// <summary>获得途径 </summary>
    UIGetWayPanel = 58,
    /// <summary>技能点不足提示</summary>
    UISkillAndGoldHintPanel = 59,
    /// <summary>装备信息界面</summary>
    UIEquipInfoPanel = 60,
    /// <summary>福利-签到</summary>
    UIPromptBox = 61,
    /// <summary>福利-补签</summary>
    UISign_intBox = 62,
    /// <summary>PromptPanel提示面板</summary>
    UIPromptPanel = 63,
    /// <summary>出售面板</summary>
    UISalePanel = 64,
    /// <summary></summary>
    UIGoodsDetials = 65,//-----没有
    /// <summary>获取体力</summary>
    UIBuyEnergyVitality = 66,
    /// <summary>点击手-按下界面</summary>
    UICountdownPanel = 67,
    /// <summary>玩家升级界面</summary>
    Upgrade = 68,
    /// <summary>点金手</summary>
    UIGoldHand = 69,
    /// <summary>-胜利-失败面板</summary>
    TheBattlePanel = 70,
    /// <summary>pvp</summary>
    UIPvP = 71,
    /// <summary>角斗场</summary>
    UIAbattiorList = 72,
    /// <summary>日常任务</summary>
    UIActivities = 73,
    /// <summary>试炼面板</summary>
    UIActivityPanel = 74,
    /// <summary></summary>
    UI_TaskTracker = 75,
    /// <summary>注册界面</summary>
    UIRegister = 78,
    /// <summary>----背包使用面 此面板暂时不用</summary>
    UIHeroUseExp = 79,
    /// <summary>排行榜界面</summary>
    UIRankList = 80,
    /// <summary>跑马灯界面</summary>
    UIMarquee = 81,
    /// <summary>邮箱界面</summary>
    UIMailPanel = 82,
    /// <summary>聊天界面</summary>
    UIChatPanel = 83,
    /// <summary>//邮箱按钮</summary>
    UIMail = 84,
    /// <summary>聊天首界面</summary>
    UIChat = 85,
    /// <summary>私聊加好友</summary>
    UIPlayerInteractionPort = 86,
    /// <summary>玩家称号面板</summary>
    UIPlayerTitlePanel = 87,
    /// <summary>福利总界面</summary>
    UIWelfare = 88,
    /// <summary>升级礼包</summary>
    UIUpgradeGiftBag = 89,
    /// <summary>提示面板</summary>
    UITooltips = 90,
    /// <summary>嗜血神灵公告面板</summary>
    UIGameAffiche = 91,
    /// <summary>副本</summary>
    UILevel = 92,
    /// <summary>英雄列表</summary>
    UIHeroList = 93,
    /// <summary>英雄详情</summary>
    UIHeroDetail = 94,
    /// <summary>装备信息面板</summary>
    EquipInfoPanel = 95,
    /// <summary>英雄上阵</summary>
    UIHeroPlay = 96,
    /// <summary>布阵</summary>
    UIEmbattle = 97,
    /// <summary>任务-任务列表</summary>
    UITaskList = 98,
    /// <summary>任务-对话</summary>
    UIDialogue = 99,
    /// <summary>采集</summary>
    UITaskCollectPanel = 100,
    /// <summary>获得途径</summary>
    UIGoodsGetWayPanel = 101,
    /// <summary>任务-使用道具面板</summary>
    UITaskUseItemPanel = 102,
    /// <summary>好友提示面板</summary>
    UIFriendTip = 103,
    /// <summary>副本界面</summary>
    SceneEntry = 104,
    /// <summary>福利界面tips</summary>
    UIgoodstips = 105,
    /// <summary>左侧任务面板</summary>
    UITaskTracker = 106,
    /// <summary>副本中任务对话界面</summary>
    UIFubenTaskDialogue = 107,
    /// <summary>主城经验条</summary>
    UIExpBar = 108,
    /// <summary>任务点箱子tips</summary>
    UIBoxGoodsTip = 109,
    /// <summary>竞技场</summary>
    UIArenaModePanel = 110,
    /// <summary>公共刷新二级界面</summary>
    UIPopUp = 111,
    /// <summary>获取途径</summary>
    UIExptips = 112,
    /// <summary>背包使用经验药水界面</summary>
    UIUseExpVialPanel = 113,
    /// <summary>公会列表</summary>
    UINotJoinSocietyPanel = 114,
    /// <summary>加入公会面板</summary>
    UIHaveJoinSocietyPanel = 115,
    /// <summary>公会详情</summary>
    UISocietyInfoPanel = 116,
    /// <summary>公会--</summary>
    UISocietyInteractionPort = 117,
    /// <summary>背包使用经验药水界面</summary>
    UIMountAndPet = 119,
    /// <summary>死亡复活面板</summary>
    UIDeadToReborn = 120,
    /// <summary>加载界面</summary>
    UILoading = 121,
    /// <summary></summary>
    EditSocietyInfoPanel = 122,
    /// <summary>祭坛抽奖提示</summary>
    UILottryHeroEffect=123,
    /// <summary>背包使用面板</summary>
    UIUseEnergyItemPanel = 124,
    /// <summary>新手引导--小手</summary>
    UIGuidePanel = 125,
    /// <summary>新手引导面板</summary>
    NextGuidePanel = 126,
    /// <summary>公会图标</summary>
    UISocietyIconPanel = 127,
    /// <summary>装备养成</summary>
    EquipDevelop = 128,
    /// <summary>装备详情</summary>
    UIEquipDetailPanel = 129,
    /// <summary>注入经验</summary>
    UIExpPropPanel = 130,
    /// <summary>摇杆</summary>
    EasyTouchPanel=131,
    /// <summary>任务自动寻路面板</summary>
    UITaskEffectPanel = 132,

    /// <summary>竞技场结算界面</summary>
    ArenaWinPanel=133,
    /// <summary>竞技场获得奖励</summary>
    FlopPanel=134,
    /// <summary>装备--选择强化或进化英雄面板</summary>
    HeroSelectTipsPanel=135,
    /// <summary>maba地图面板</summary>
    MobaMap = 136,
    /// <summary>战斗统计面板</summary>
    MobaStatInfo=137,
    /// <summary>试炼</summary>
    UIActivity=138,
    /// <summary>新手改名面板</summary>
    UICreateName=139,
    /// <summary>装备强化面板</summary>
    EquipStrengthePanel=140,
    EquipIntensifyPanel = 141,
    EquipEvolvePanel = 142,
    /// <summary>胜利界面</summary>
    UITheBattlePanel=143,
}
/// <summary>
/// 面板深度 规则：从1开始 两个面板隔5
/// </summary>
public enum UIPanelDepth:int
{
    UIRole = 0,
    UIRoleInfo = 1,
    UIChat = 0,
    UITaskTracker = 10,
    UISetting = 0,
    UIExpBar=5,
    UIArenaModePanel=60,
    UIActivities = 20,
    UILottery = 25,
    UIFriends = 30,
    UIMailPanel = 35,
    UIGoodsGetWayPanel = 180,
    UILevel = 190,
    UIActivityPanel = 45,
    SceneEntry=215,
    UIPVP = 50,
    Raids=53,
    Upgrade=275,
    UIKnapsack = 55,
    UIShop = 60,
    UIRankList = 65,
    
    UIHeroList = 75,

    UIChatPanel = 305,//70,
    UIMask =62,
    UIPopBuy=85,
    UIResultLottery=95,
  
    UIResultSoul = 100,
    UIHeroDetail=105,

    UIPlayerTitlePanel = 110,
    UIShopSell = 115,
    UIWelfare=120, //tips层级
    UITaskRewardPanel = 125,
    UINotJoinSocietyPanel = 130,
    UIHaveJoinSocietyPanel = 135,
    UISocietyInfoPanel = 140,
    UIMountAndPet = 150,
    UIgoodstips =200,//福利界面层级
    UISign_intBox = 205,//补签弹框面板
    UIPopLottery = 210,

    UIAbattiorList = 60,
    UIEmbattle = 265,//70
    UIPlayerInteractionPort = 270,
    UIGoldHand = 280,//点金手界面
    UIBuyEnergyVitality = 285,//购买体力界面
    UIPopup = 290,
    UIWaitForSever =295,

    UIMarquee = 310,
    UIMoney = 300,//80,
    UILottryHeroEffect = 305,
    UIPromptBox = 350,
}
