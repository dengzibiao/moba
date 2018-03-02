/*
文件名（File Name）:   EnumType.cs

作者（Author）:    #高#

创建时间（CreateTime）:  2016-6-29 14:6:6
*/
using UnityEngine;
using System.Collections;

/// <summary>
/// 客户端向服务器发送消息的类型
/// </summary>
public enum C2SMessageType : int
{
    ActiveWait = 0,//主动发送不影响其他消息发送且转圈
    Active, //主动发送不影响其他消息发送且不转圈
    PASVWait,//点击按钮发送消息,转圈且不会重复发送数据

}
/// <summary>
/// 对象当前状态 
/// </summary>
public enum EnumObjectState
{
    None,
    Initial,//初始化
    Loading,//加载
    Ready,//加载完成
    Disabled,//
    Closing//关闭
}
/// <summary>
/// 打开界面的类型
/// </summary>
public enum EnumOpenUIType:int
{
    DefaultUIOrSecond,//打开默认界面或打开二级界面
  OpenNewCloseOld //关闭当前界面打开一下个界面
}
/// <summary>
/// 此处定义红点的大类类型
/// </summary>
public enum EnumRedPoint : int
{
    //---------------------------------------------//
    // 子项的类型自己定义，定义后需要告知服务器
    //---------------------------------------------//
    RP_WELFARE = 1,  //福利
    RP_NEW_MAIL=2, //邮件
    RP_EVENT_DUNGEON=3,//试炼
    RP_DRAW = 4,  //祭坛
    RD_FRIEND = 5,  //好友申请列表1
    RD_Level=6,//冒险精英副本1
    RD_UIPVP=7,    //对战 角斗场1，
    RD_Guild=8,  //公会 申请列表1
    RD_Collect=9, //收藏 坐骑1，宠物2
    RD_ACTIVITY=10, //活动
    //英雄==11
}
public enum Friends
{
    Agree = 1,//同意
    Refuse = 2,//拒绝
    Apply = 1,//申请列表
    Blacklist = 2,//黑名单列表
    Delete = 1,//从好友列表删除
    Remove = 2,//从黑名单列表移除
    DeleteEnemy = 3,//从仇人列表删除,或申请仇人列表类型
    DeleteLast = 4//从最近联系列表删除
}
public enum MoneyType : int
{
    // 1金币，2钻石，3角斗场，4竞技场币，5远征币，6悬赏币
    Gold = 1, Diamond, PVPcoin, AreanCoin, PVEcion, RewardCoin
}
/// <summary>
/// 物品品质白，绿，蓝，紫，橙，红
/// </summary>
public enum GradeType
{
    Gray = 1, Green = 2, Green1 = 3, Blue = 4, Blue1 = 5, Blue2 = 6, Purple = 7, Purple1 = 8, Purple2 = 9, Purple3 = 10, Orange = 11, Orange1 = 12, Orange2 = 13, Orange3 = 14, Orange4 = 15, Red = 16, Red1 = 17, Red2 = 18, Red3 = 19, Red4 = 20, Red5 = 21
}


/// <summary>
/// 抽奖类型1=Gold，2=Diamond，3=魂匣
/// </summary>
public enum LotteryType : int
{
    GoldLottery = 1, DiamondLottery, LotterySoul, None
}
/// <summary>
/// 获得新英雄展示动画类型用来区分调用来源
/// </summary>
public enum ShowHeroEffectType : int
{
    //1抽奖，2英雄列表，3签到，4世界地图评星宝箱，5开服回馈
    None = 0, Lottry, HeroList, signIn, StarEvaluationChest, NewPlayerRewards,
}
/// <summary>
/// 消耗类型0=免费，1=消耗
/// </summary>
public enum CostType : int
{
    Free, Cost, None
}
/// <summary>
/// 道具类型 1：装备，2：材料，3：材料碎片，4：金币道具，5：经验道具，6：英雄灵魂石，7：英雄整卡，8：符文，9：体力恢复道具，10：扫荡卷，11：宝箱。(4,5,9,10,11,13为消耗品)，12：任务收集品；13：喇叭；14：货币 15：坐骑卡；16：宠物卡
/// </summary>
public enum ItemType : int
{
    Equip = 1, Material, MaterialChip, GoldProp, ExpProp, SoulStone, HeroCard, Rune, RecoverProp, SweptVolume, JewelBox, TaskGoods, Horn, Money, Mount, Pet
}
/// <summary>
/// Scene名字
/// </summary>
/// <summary>
/// 邮件附件物品类型 1 : 道具 2：Gold 3：Diamond 4:体力5:
/// </summary>
public enum MailGoodsType : int
{
    ItemType = 1, GoldType = 101, DiamomdType = 10101, PowerType, ExE = 1010101, HeroExp = 102, XuanshangGold = 103,
}

public enum EnumSceneID : int
{
    Null = -1,
    UI_Login,//登陆场景
    UI_MajorCity01,//主场景
    Dungeons,//副本
    LGhuangyuan,//龙骨荒原
    BF_NorthCity,//北方城市
    Moba1v1
}
/// <summary>
/// 任务类型
/// </summary>
public enum TaskClass : int
{
    Main = 0,//主线任务
    Branch,//支线任务
    Reward//悬赏


}
/// <summary>
/// 任务状态
/// </summary>
public enum TaskProgress : int
{
    /// <summary>
    /// 不能接
    /// </summary>
    CantAccept = 0,
    /// <summary>
    /// 未接
    /// </summary>
    NoAccept = 1,
    /// <summary>
    /// 已接
    /// </summary>
    Accept = 2,
    /// <summary>
    /// 完成
    /// </summary>
    Complete = 3,
    /// <summary>
    /// 领取
    /// </summary>
    Get = 4,
    /// <summary>
    /// 已领奖励
    /// </summary>
    Reward = 7
}

public enum GetTaskItemType : int
{
    Daily = 1,
    Box
}
/// <summary>
/// 日常任务类型
/// </summary>
public enum EveryTaskProgress : int
{
    /// <summary>
    /// 1.免费扫荡卷
    /// </summary>
    Mopping = 1,
    /// <summary>
    /// //2.使用点金手
    /// </summary>
    Goldhand,
    /// <summary>
    ///  3：抽卡
    /// </summary>
    Lottery,
    /// <summary>
    /// 4：通关普通副本
    /// </summary>
    GeneralFB,
    /// <summary>
    ///   5：竞技场挑战
    /// </summary>
    Arena,
    /// <summary>
    /// 6：升级英雄技能
    /// </summary>
    UpHeroSkill,
    /// <summary>
    /// 7：通关精英副本
    /// </summary>
    EliteFB,
    /// <summary>
    /// 8：通关活动副本Ⅰ
    /// </summary>
    ActivityFB1,
    /// <summary>
    /// 9：通关活动副本Ⅱ
    /// </summary>
    ActivityFB2,
    /// <summary>
    /// 10：悬赏任务
    /// </summary>
    Reward,
    /// <summary>
    /// 11：角斗场挑战
    /// </summary>
    Abattoir,
    /// <summary>
    /// 12：购买月卡奖励
    /// </summary>
    MonthCard,
    /// <summary>
    /// 13：提升任意装备的等级
    /// </summary>
    UpEquipLevel
}

public enum TaskModifyType : int
{
    Add,
    Change,
    Remove
}

/// <summary>
/// 聊天频道 1世界聊天 2公会聊天 3私聊 4附近聊天 5队伍聊天 6系统
/// </summary>
public enum ChatType : int
{
    WorldChat = 1,
    SocietyChat = 2,
    PrivateChat = 3,
    NearbyChat = 4,
    TroopsChat = 5,
    SystemChat = 6,
}
/// <summary>
/// 1 玩家或者服务器发言内容 2 不能发言（没满足等级或者没vip） 3 不能发言太快 4 没有队伍 5 没有公会 6：世界频道发言次数限制 7 没有私聊对象 8 私聊对象不在线 9聊天内容为空
/// </summary>
public enum ChatContentType : int
{
    TextContent = 1,
    NotSpeak = 2,
    NotSpeakFast = 3,
    NotTeam = 4,
    NotSociety = 5,
    SpeakTimesLimit = 6,
    NotPrivateTarget = 7,
    PrivateTargetNotOnLine = 8,
    NoCharacter = 9,
}
/// <summary>
/// 排行榜
/// </summary>
public enum RankListType : int
{
    /// <summary> 等级-玩家等级</summary>
    Level = 5,
    /// <summary> 财富-金币消耗 </summary>
    Fortune = 4,
    /// <summary>财富-钻石消耗 </summary>
    DiamondUser = 2,
    /// <summary> 战斗力-总战斗力 </summary>
    Fight = 0,
    /// <summary>总战力--四强 </summary>
    BestFourPersons = 3,
    /// <summary>战斗力 星级总数 </summary>
    StarSum = 1,
    /// <summary> 竞技场-昨日排名 </summary>
    YesterdayRank = 7,
    /// <summary> 竞技场-实时排名 </summary>
    RealTimeRank = 6
}
/// <summary>
/// 商店类型
/// </summary>
public enum ShopType : int
{
    Prop = 1,//道具商店
    Arena = 5,//竞技场
    abattoir = 7,//角斗场
    Reward = 8//悬赏
}
/// <summary>
/// 任务类型
/// </summary>
public enum TaskType : int
{
    /// <summary>对话///</summary>
    Dialogue = 1,
    /// <summary>通关副本///</summary>
    PassCopy = 2,
    /// <summary>采集///</summary>
    Collect = 3,
    /// <summary>提升技能等级///</summary>
    UpgradeSkillLv = 4,
    /// <summary>提升英雄装备等级///</summary>
    UpgradeHeroEquipLv = 5,
    /// <summary>杀怪///</summary>
    KillMonster = 6,
    /// <summary>怪物掉落物///</summary>
    KillDropSth = 7,
    /// <summary>背包物品///</summary>
    knapsackItem = 8,
    /// <summary>指定地点，即完成NPC附近使用道具,传信///</summary>
    NamedPComplete = 9,
    /// <summary>击杀临时怪物/// </summary>
    KillTempMonster = 10,
    /// <summary>杀人/// </summary>
    KillPerson = 11
}
public enum SocietyStatus
{
    Null = 0,
    Member = 1,//普通成员
    President = 5,//会长
}
/// <summary>
/// 标识具体面板下的item
/// </summary>
public enum PanelItemFlag : int
{

}