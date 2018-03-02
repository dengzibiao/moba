////////////////////////////////////////////////////////////////////////////////
//存放玩家基础数据

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Tianyu;
using UnityEngine.SceneManagement;

public class Roledata
{
    public uint[] infodata = new uint[1024];
    public UInt32 accountId = 0;
    public UInt32 playerId = 0;
    public string playeName = "";
    //public string playerTitleName = "ch_005";
    //public int playerTitleId = 5;//玩家当前携带的称号id(默认有一个 并且必须带一个)
    //public Dictionary<int, string> playerTitleDic = new Dictionary<int, string>() { { 5,"永久"}, {1,"2天" } };//玩家已激活的称号列表  id -- 有效期
    public string playerTitleName = "";
    public int playerTitleId = 0;//玩家当前携带的称号id(默认有一个 并且必须带一个)
    public Dictionary<int, string> playerTitleDic = new Dictionary<int, string>();//玩家已激活的称号列表  id -- 有效期
    public UInt32 heroId = 0;
    Vector3 pos = new Vector3(35.92f, 0.18f, 39.71f);
    public Vector3 rotation;
    public GameObject model;
    public int heroLveCap;
    public string mobelNumb = "";
    public string password = "";
    public void SetPos(Vector3 valu)
    {
        pos.x = valu.x;
        pos.y = valu.y;
        pos.z = valu.z;
    }
    public Vector3 GetPos()
    {
        return pos;
    }
    // public double sysTime;//系统时间

    //显示数据
    public string icon;//头像   
    public int exprience;//经验
    public int level = 1;//等级
    public int FightLv;//战斗力
    public int maxExprience;//最大经验
    public int currentFriends;///当前好友
    public int rank;//角斗场排行
    public long expPool;//经验池经验
    public long maxExpPool;//经验池最大值

    public int iconFram;//头像框
    public int changeCount;//改名次数
    public uint keyId;
    public int mapID = 20100;
    public int oldMapID = -1;
    public int vip = 0;//vip等级
    public int hp;
    public int maxhp;
    public string mapName;

    //public List<IconData> IconList = new List<IconData>();
    //public List<IconBorderData> IconBorderList = new List<IconBorderData>();
    public List<PlayerNameNode> nameList = new List<PlayerNameNode>();//名字库列表
}
public class PlayerTitleData
{
    public int id;//称号id
    public string endTiem;//称号结束时间
}
public class IconData
{
    public long icon_id;//= 10001;         //人物头像ID
    public string icon_name;       //人物头像名
}
public class IcnFramData
{
    public long iconFrame_id; ///人物头像框ID
    public string iconFrame_name; //
}
public class GameSet
{
    public bool isPlay = false; //默认音乐不静音
    public bool isPlaySoundEffect = false; //音效默认不静音
}
/// <summary>
/// 玩家排行榜信息
/// </summary>
public class PlayerRankData
{
    public int playerId;//玩家自己Id
    //public int currentValue;//当前数值
    public int currentRank;//当前排名
    //public int yesterdayRank;//昨日排名
    public string guildName;//公会名称
    public RankListType rankListType;//类型
    /// <summary>数据一天更新一次，将每个数据进行单独储存---当前数值 /// </summary>
    public int fight;//总战力
    public int startSum;//星级总数
    public int lv;//等级
    public int yestterday;//昨日排名
    public int realTime;//实时排名
    ///// <summary>数据一天更新一次，将每个数据进行单独储存---当前排名 /// </summary>
    public int fightCurrentRank;
    public int starttSumCurrentRank;
    public int lvCurrentRank;
    public int yestterdayCurrentRank;
    public int realTimeCurrentRank;
    public int bestfourCurrentRank;
    ///// <summary>数据一天更新一次，将每个数据进行单独储存---昨日排名 /// </summary>
    public int fightYesterdayRank;
    public int starttSumYesterday;
    public int lvYesterday;
    public int yestterday_yes;
    public int realTimeYesterday;
    public int bestfourYesterday;
    //public List<RankListData> itemRankList = new List<RankListData>();
    /// <summary>数据一天更新一次，将每个数据列表进行单独储存 /// </summary>
    public List<RankListData> fightList = new List<RankListData>();//战力
    public List<RankListData> starSumList = new List<RankListData>();//星级
    public List<RankListData> diamondUserList = new List<RankListData>();//钻石
    public List<RankListData> bestFourPersonsList = new List<RankListData>();//四强                                                           
    public List<RankListData> fortuneList = new List<RankListData>();//金币
    public List<RankListData> levelList = new List<RankListData>();//等级
    public List<RankListData> realTimeRankList = new List<RankListData>();//角斗场
    public List<RankListData> yesterdayList = new List<RankListData>(); //昨日排名

    public long fightLog = 0;
    public long starSumLog = 0;
    public long bestFourLog = 0;
    public long levelLog = 0;
    public int page = 0;

}
/// <summary>
/// 每日签到
/// </summary>
public class SingnData
{
    public int types;//1签到，22所有
                     //签到的补充这里

    public string Signed;//签到的时间戳
    public string dinning;//定时进餐
    public string lvReward;//升级
    public string onlineReward;//奖励
    public long logintime;//loginTime 秒
    public int onLineTime;//玩家上次在线时长累计（用于在线奖励倒计时） 单位：分钟
    public long getRewardTime;//玩家在线过程中 领取奖励 将此值设置为领取奖励的时间 用于记录本次奖励的在线时长
    public long onLineRewardTime;//在线奖励可领取时间
    public Dictionary<int, int> alreadylevelRewardDic = new Dictionary<int, int>();//已领取的等级奖励id字典
    public bool isCanGetOnlineReward = false;
    public int onlineAlreadyGetCount = 0;//在线已经领取次数
    public bool onlineIsRefresh = false;
}
/// <summary>
/// 定时进餐
/// </summary>
public class GetEnergyData
{
    public int resolt;
    public List<MealAttrNode> mealList = new List<MealAttrNode>();
}
/// <summary>
/// 其他玩家的排行信息
/// </summary>
public class RankListData
{
    public int currentRank;//玩家当前排名
    public int playerId;//玩家id
    public string name;//玩家名字
    public string nameTitle;//玩家称号
    public long iconId;//玩家头像Id
    public long iconBoxId;//玩家头像框id
    public string guildName;//公会名称
    public int rankValue;//入榜数值
    public int playerLv;//角色等级

    public List<RankHeroList> arenaHeroList = new List<RankHeroList>();//获取三英雄
    public string[] bestFour = new string[5];//最强四人--长度4
    /// <summary> //1头像id,2等级,3战力,4星星,5品阶/// </summary>
    public string[] arenaArr = new string[5];
}
/// <summary>
/// 角斗场三个防守英雄
/// </summary>
public class RankHeroList
{
    public long iconId1;//英雄id
    public int lv;//等级
    public int star;//星星
    public int iconIdFrame;//品质
}
/// <summary>
/// 跑马灯
/// </summary>
public class MarqueeData
{
    public int opt1;
    public int opt2;
    public int opt3;
    public string user1;
    public string user2;
    public string user3;
}

public class NewPlayerRewardList
{//新手登录奖励初始化表数据
    public List<ItemData> rewardList = new List<ItemData>();
    //服务器返回的时间列表
    public List<int> timeList = new List<int>();
}
/// <summary>
/// 好友列表
/// </summary>
public class FriendListData
{
    public List<FriendData> RecommendfriendList = new List<FriendData>();//推荐好友
    public List<FriendData> searchList = new List<FriendData>();//搜索好友列表
    public List<FriendData> applyforList = new List<FriendData>();//好友申请列表
    public List<FriendData> friendList = new List<FriendData>();//好友列表
}
/// <summary>
/// 好友数据
/// </summary>
public class FriendData
{ //角色账号，角色ID,角色昵称,角色等级,角色头像,战斗力,称号
    private long _acountPlayerId;
    private long _playerId;
    private string _name;
    private int _level;
    private int _heroLevel;
    private string _playerIcon;
    private string _playerFrame;
    private int _fighting;
    private string _title;

    public long PlayerId
    {
        get { return _playerId; }
        set { _playerId = value; }
    }

    public string Name
    {
        get { return _name; }
        set { _name = value; }
    }

    public int Level
    {
        get { return _level; }
        set { _level = value; }
    }

    public string PlayerIcon
    {
        get { return _playerIcon; }
        set { _playerIcon = value; }
    }

    public int Fighting
    {
        get { return _fighting; }
        set { _fighting = value; }
    }

    public string Title
    {
        get { return _title; }
        set { _title = value; }
    }

    public string PlayerFrame
    {
        get { return _playerFrame; }
        set { _playerFrame = value; }
    }

    public long AcountPlayerId
    {
        get { return _acountPlayerId; }
        set { _acountPlayerId = value; }
    }

    public int HeroLevel
    {
        get { return _heroLevel; }
        set { _heroLevel = value; }
    }
}
public class BagData
{
    public long gold;//金钱
    public long diamond;//Diamond
    public int strength; //体力
    public int vitality; //活力

    public long areanCoin;//竞技场币
    public long pveCoin;//--龙鳞币
    public long pvpCoin;//角斗场币
    public long rewardCoin;//悬赏币
    public int openGridNumb;//开放格子数
    public int todayBuyStrengthCount;// 今日购买体力次数
    public int todayBuyVitalityCount;//今日购买活力次数
    //Dictionary<int,int>
    public Dictionary<long, ItemData> ItemDic = new Dictionary<long, ItemData>();//用于物品的查找
    public List<ItemData> itemlist = new List<ItemData>();//用于物品的排序 插入
}

public class ActionData
{
    public long energyRecoverEndTime;//体力恢复结束时间
    public long energyTime;//一次体力倒计时毫秒
    public long vitalityTime;//一次活力倒计时毫秒
    public long allVitalityTime;//剩余所有活力倒计时毫秒
    public long allEnergyTime;//剩余所有体力倒计时毫秒

    public int maxCurrentEnerty = 999;//当前体力值最大值
    public int buyEnergyCount = 120;//购买体力数量
    public float energyTimeBucket = 6;//体力恢复时间间隔
    public int maxEnergyCount = 0;//体力上线
    public int energyBuyTimes = 0;// 体力购买次数
    public int maxEnergyBuyTimes;//体力最大购买次数
    public int EnergyNeedJewelCount = 50; //购买一次花费钻石数
    public int[] EnergyJewelArray;


    public int maxCurrentVitality = 999;//当前活力值最大值
    public int buyVitalityCount = 120;//购买活力数量
    public float vitalityTimeBucket = 6;// 活力恢复时间间隔
    public int maxVitalityCount = 0;//活力上线
    public int vitalityBuyTimes = 0;// 活力购买次数
    public int maxVitalityBuyTimes = 5;//活力最大购买次数
    public int vitalityNeedJewelCount = 50; //购买一次活力花费钻石数
    public int[] vitalityJewelArray;

}
public class ItemData
{
    private int start;//英雄星级
    private long _id;//物品ID
    private int vipID;//VIP的ID
    private int _serialNumberId;//序列ID
    private string _uuid;//物品uuid
    private int _count;//物品叠加数
    private string _name;//物品名称
    private int _types;//物品类型
    private string _describe;//物品描述
    private int _cprice;//购买价格
    private int _sprice;//出售单价
    private int _piles;//堆叠上限
    private string _iconName;//图标名称
    private MoneyType _moneyType;//金币类型
    private GradeType _gradeType;//品质
    private int diamond;//钻石
    private int gold;//金币数
    private int power;//金币数
    private bool _isBuy = false;//未购买(商店)
    public int[,] goodsItem;//奖励物品[id,数量]
    private UIAtlas _uiAtlas;
    private MailGoodsType mailGoodsType;
    private long gameTime;//游戏时间
    private long timeSign;//时间戳
    private int exe;//战队经验
    private int heroExp;//英雄经验
    private int xuanshangGold;//悬赏币
    private int taskType;//任务类型
    public MoneyType MoneyTYPE
    {
        get { return _moneyType; }
        set { _moneyType = value; }
    }

    /// <summary>
    /// 物品ID
    /// </summary>
    public long Id
    {
        get { return _id; }
        set { _id = value; }
    }
    /// <summary>
    /// 物品叠加数
    /// </summary>
    public int Count
    {
        get { return _count; }

        set { _count = value; }
    }
    /// <summary>
    /// 物品名称
    /// </summary>
    public string Name
    {
        get { return _name; }
        set { _name = value; }
    }
    /// <summary>
    /// 物品品质
    /// </summary>
    public GradeType GradeTYPE
    {
        get { return _gradeType; }
        set { _gradeType = value; }
    }

    public static string GetFrameByGradeType(GradeType gradeType)
    {
        switch (gradeType)
        {
            case GradeType.Green:
                return "lv";
            case GradeType.Green1:
                return "lv1";
            case GradeType.Blue:
                return "lan";
            case GradeType.Blue1:
                return "lan1";
            case GradeType.Blue2:
                return "lan2";
            case GradeType.Purple:
                return "zi";
            case GradeType.Purple1:
                return "zi1";
            case GradeType.Purple2:
                return "zi2";
            case GradeType.Purple3:
                return "zi3";
            case GradeType.Orange:
                return "cheng";
            case GradeType.Red:
                return "hong";
            case GradeType.Gray:
            default:
                return "hui";
        }
    }

    public static void SetAngleMarking(UISprite sprite, int type)
    {
        sprite.enabled = true;
        switch (type)
        {
            case 3:
                sprite.spriteName = "materialdebris";
                break;
            case 6:
                sprite.spriteName = "linghunshi";
                break;
            default:
                sprite.enabled = false;
                break;
        }
        sprite.MakePixelPerfect();
    }

    /// <summary>
    /// icon资源名字
    /// </summary>
    public string IconName
    {
        get { return _iconName; }
        set { _iconName = value; }
    }

    public string Uuid
    {
        get { return _uuid; }
        set { _uuid = value; }
    }
    /// <summary>
    /// 物品类型
    /// </summary>
    public int Types
    {
        get { return _types; }
        set { _types = value; }
    }
    /// <summary>
    /// 物品描述
    /// </summary>
    public string Describe
    {
        get { return _describe; }
        set { _describe = value; }
    }
    /// <summary>
    /// 系统出售价格或玩家买入价格
    /// </summary>
    public int Cprice
    {
        get { return _cprice; }
        set { _cprice = value; }
    }
    /// <summary>
    /// 玩家出售价格
    /// </summary>
    public int Sprice
    {
        get { return _sprice; }
        set { _sprice = value; }
    }
    /// <summary>
    /// 物品堆叠上限
    /// </summary>
    public int Piles
    {
        get { return _piles; }
        set { _piles = value; }
    }
    /// <summary>
    /// 商店物品是否已购买过
    /// </summary>
    public bool IsBuy
    {
        get { return _isBuy; }
        set { _isBuy = value; }
    }
    //图集
    public UIAtlas UiAtlas
    {
        get { return _uiAtlas; }
        set { _uiAtlas = value; }
    }

    public int Diamond
    {
        get { return diamond; }
        set { diamond = value; }
    }

    public int Gold
    {
        get { return gold; }
        set { gold = value; }
    }

    public int SerialNumberId
    {
        get { return _serialNumberId; }
        set { _serialNumberId = value; }
    }

    public MailGoodsType GoodsType
    {
        get { return mailGoodsType; }
        set { mailGoodsType = value; }
    }

    public long GameTime
    {
        get { return gameTime; }
        set { gameTime = value; }
    }

    public long TimeSign
    {
        get { return timeSign; }
        set { timeSign = value; }
    }

    public int Power
    {
        get { return power; }
        set { power = value; }
    }

    public int Exe
    {
        get { return exe; }
        set { exe = value; }
    }

    public int VipId
    {
        get { return vipID; }
        set { vipID = value; }
    }

    public int HeroExp
    {
        get
        {
            return heroExp;
        }

        set
        {
            heroExp = value;
        }
    }

    public int XuanshangGold
    {
        get
        {
            return xuanshangGold;
        }

        set
        {
            xuanshangGold = value;
        }
    }

    public int TaskType
    {
        get { return taskType; }
        set { taskType = value; }
    }

    public int Star
    {
        get { return start; }
        set { start = value; }
    }
}

public class MailData
{
    public int startIndex;
    /// <summary>
    /// 邮件数量
    /// </summary>
    public int currentCount;
    /// <summary>
    /// 邮件数量上限
    /// </summary>
    public int maxCount;
    public int unReadMailCount;
    public List<MailItemData> mailItemList = new List<MailItemData>();
}
public class MailItemData
{
    private long _id;
    private string _niceName;
    private string _title;
    private string _content;
    private string _mainType;
    private string _type;
    private long _creatTime;
    private long _endTime;
    private int _newMailFlag;
    private int _isHaveGetGoods;
    public List<MailAccessoryData> accessoryDataList = new List<MailAccessoryData>();
    #region GetAndSet
    /// <summary>
    /// 邮件ID
    /// </summary>
    public long Id
    {
        get
        {
            return _id;
        }

        set
        {
            _id = value;
        }
    }
    /// <summary>
    /// 发信人昵称
    /// </summary>
    public string NiceName
    {
        get
        {
            return _niceName;
        }

        set
        {
            _niceName = value;
        }
    }
    /// <summary>
    /// 邮件标题
    /// </summary>
    public string Title
    {
        get
        {
            return _title;
        }

        set
        {
            _title = value;
        }
    }
    /// <summary>
    /// 内容
    /// </summary>
    public string Content
    {
        get
        {
            return _content;
        }

        set
        {
            _content = value;
        }
    }
    /// <summary>
    /// 主类型
    /// </summary>
    public string MainType
    {
        get
        {
            return _mainType;
        }

        set
        {
            _mainType = value;
        }
    }
    /// <summary>
    /// 类型
    /// </summary>
    public string Type
    {
        get
        {
            return _type;
        }

        set
        {
            _type = value;
        }
    }
    /// <summary>
    /// 创建时间
    /// </summary>
    public long CreatTime
    {
        get
        {
            return _creatTime;
        }

        set
        {
            _creatTime = value;
        }
    }
    /// <summary>
    /// 结束时间
    /// </summary>
    public long EndTime
    {
        get
        {
            return _endTime;
        }

        set
        {
            _endTime = value;
        }
    }
    /// <summary>
    /// 新邮件标示
    /// </summary>
    public int NewMailFlag
    {
        get
        {
            return _newMailFlag;
        }

        set
        {
            _newMailFlag = value;
        }
    }
    /// <summary>
    /// 是否已经领取过邮件附件 1可领取 0 已领取
    /// </summary>
    public int IsHaveGetGoods
    {
        get
        {
            return _isHaveGetGoods;
        }

        set
        {
            _isHaveGetGoods = value;
        }
    }
    #endregion
}
/// <summary>
/// 邮件附件数据
/// </summary>
public class MailAccessoryData
{
    private int _id;//物品ID
    private long _count;//物品叠加数
    private int _gold;//Gold
    private int _diamond;//Diamond
    private MailGoodsType _type;//附件类型
    private GradeType _gradeType;
    private int itemType;
    private UIAtlas _uiAtlas;
    private bool _isHaveGet;// 是否已经被领取
    #region GetSet
    /// <summary>
    /// 物品ID
    /// </summary>
    public int Id
    {
        get
        {
            return _id;
        }

        set
        {
            _id = value;
        }
    }
    /// <summary>
    /// 物品叠加数
    /// </summary>
    public long Count
    {
        get
        {
            return _count;
        }

        set
        {
            _count = value;
        }
    }
    /// <summary>
    /// 金币数
    /// </summary>
    public int Gold
    {
        get
        {
            return _gold;
        }

        set
        {
            _gold = value;
        }
    }
    /// <summary>
    /// 钻石数
    /// </summary>
    public int Diamond
    {
        get
        {
            return _diamond;
        }

        set
        {
            _diamond = value;
        }
    }
    /// <summary>
    /// 附件类型
    /// </summary>
    public MailGoodsType Type
    {
        get
        {
            return _type;
        }

        set
        {
            _type = value;
        }
    }

    public GradeType GradeType
    {
        get
        {
            return _gradeType;
        }

        set
        {
            _gradeType = value;
        }
    }

    public bool IsHaveGet
    {
        get
        {
            return _isHaveGet;
        }

        set
        {
            _isHaveGet = value;
        }
    }

    public int ItemType
    {
        get
        {
            return itemType;
        }

        set
        {
            itemType = value;
        }
    }

    public UIAtlas UiAtlas
    {
        get
        {
            return _uiAtlas;
        }

        set
        {
            _uiAtlas = value;
        }
    }
    #endregion
}
/// <summary>
/// 日常任务
/// </summary>
public class EveryTaskDataList
{
    //奖励箱子的ID
    private int id;
    private int activeIndex;//总的当前活跃度
    private int activeAll;//总的活跃度
    public int box1State;
    public int box2State;
    public int box3State;
    public int box4State;
    public DayActiveNode dailyTasks;
    public List<EveryTaskData> itemList = new List<EveryTaskData>();//日常
    public List<EveryTaskData> itList = new List<EveryTaskData>();//悬赏

    //----------------------------------------------------------------
    public int getCount;//悬赏任务领奖次数
    public int refreshCount;//刷新悬赏任务次数
    public List<int> refreshCost = new List<int>();//刷新消耗钻石

    public int ActiveAll
    {
        get { if (activeAll != 0) return activeAll; else return GetActiveAll(); }
    }

    public int ActiveIndex
    {
        get { return activeIndex; }
        set { activeIndex = value; }
    }

    public int Id
    {
        get { return id; }
        set { id = value; }
    }

    public DayActiveNode DailyTasks
    {
        get
        {
            if (dailyTasks != null) return dailyTasks;
            else
            {
                return null;
            }

        }
        set { dailyTasks = value; }
    }

    int GetActiveAll()
    {
        for (int i = 0; i < itemList.Count; i++)
        {
            activeAll += itemList[i].active;
        }
        return activeAll;
    }
}
public class IChat
{
    /// <summary>
    /// 系统聊天数据列表
    /// </summary>
    public List<ChatData> worldChatList = new List<ChatData>();
    /// <summary>
    /// 公会聊天数据列表
    /// </summary>
    public List<ChatData> societyChatList = new List<ChatData>();
    /// <summary>
    /// 私聊聊天数据列表
    /// </summary>
    public List<ChatData> privateChatList = new List<ChatData>();
    /// <summary>
    /// 附近聊天数据列表
    /// </summary>
    public List<ChatData> nearbyChatList = new List<ChatData>();
    /// <summary>
    /// 队伍聊天数据列表
    /// </summary>
    public List<ChatData> troopsChatList = new List<ChatData>();
    /// <summary>
    /// 系统聊天数据列表
    /// </summary>
    public List<ChatData> systemChatList = new List<ChatData>();
}
public class ChatData
{
    private long _id;
    private long _accountId;
    private string _nickName;
    private long _headId;
    private int _vip;
    private long _speakingTime;
    private string _time;
    private ChatType _type;
    private bool isLocalPlayer;
    private string chatContent;
    private ChatContentType _contentType;
    #region GetSet
    /// <summary>
    /// 发言者ID
    /// </summary>
    public long Id
    {
        get
        {
            return _id;
        }

        set
        {
            _id = value;
        }
    }
    /// <summary>
    /// 发言者昵称
    /// </summary>
    public string NickName
    {
        get
        {
            return _nickName;
        }

        set
        {
            _nickName = value;
        }
    }
    /// <summary>
    /// 发言者头像ID
    /// </summary>
    public long HeadId
    {
        get
        {
            return _headId;
        }

        set
        {
            _headId = value;
        }
    }
    /// <summary>
    /// 发言者vip等级
    /// </summary>
    public int Vip
    {
        get
        {
            return _vip;
        }

        set
        {
            _vip = value;
        }
    }
    /// <summary>
    /// 发言时间
    /// </summary>
    public long SpeakingTime
    {
        get
        {
            return _speakingTime;
        }

        set
        {
            _speakingTime = value;
        }
    }
    /// <summary>
    /// 是否是玩家本身
    /// </summary>
    public bool IsLocalPlayer
    {
        get
        {
            return isLocalPlayer;
        }

        set
        {
            isLocalPlayer = value;
        }
    }
    /// <summary>
    /// 聊天频道类型 1世界聊天 2公会聊天 3私聊 4附近聊天 5队伍聊天 6系统
    /// </summary>
    public ChatType Type
    {
        get
        {
            return _type;
        }

        set
        {
            _type = value;
        }
    }
    /// <summary>
    /// 内容
    /// </summary>
    public string ChatContent
    {
        get
        {
            return chatContent;
        }

        set
        {
            chatContent = value;
        }
    }

    public ChatContentType ContentType
    {
        get
        {
            return _contentType;
        }

        set
        {
            _contentType = value;
        }
    }

    public string Time
    {
        get
        {
            return _time;
        }

        set
        {
            _time = value;
        }
    }

    public long AccountId
    {
        get
        {
            return _accountId;
        }

        set
        {
            _accountId = value;
        }
    }
    #endregion
}

public abstract class CharacterData
{
    public long id;
    public int lvl = 1;
    public Modestatus state;
    public UInt32 groupIndex;      // 分组Id，一般红0蓝1
    public CharacterAttrNode attrNode;

    public bool useServerAttr = true;
    public float[] charAttrs = new float[Formula.ATTR_COUNT]; // 玩家当前属性
    public float[] battleInitAttrs = new float[Formula.ATTR_COUNT]; // 战斗初始属性
    public float[] buffAttrs = new float[Formula.ATTR_COUNT];    // 战斗Buff加成
    public float[] serverAttrs = new float[Formula.ATTR_COUNT];  // 服务器返回的属性
    public float[] adjustAttrs = new float[Formula.ATTR_COUNT];  // 属性调整
    public Dictionary<long, int> skill = new Dictionary<long, int>();   //技能等级
    public float[] skill_Damage;                                    //技能伤害

    public int mobaMorale;
    public int mobaKillCount;
    public int mobaSerialKillCount;
    public int mobaAidCount;
    public int mobaDeathCount;
    public int mobaMultiKillCount;
    public CDTimer.CD mobaMultiKillCD;
    public string fakeMobaPlayerName;
    public int memberId;   // 战斗位置（几号位）
    public int monsterAreaId;//野怪区域id，1，2，3

    public CharacterData Clone()
    {
        CharacterData mTempData = (CharacterData)this.MemberwiseClone();
        if (skill_Damage != null && skill_Damage.Length > 0)
        {
            mTempData.skill_Damage = new float[skill_Damage.Length];
            for (int i = 0; i < skill_Damage.Length; i++)
            {
                mTempData.skill_Damage[i] = skill_Damage[i];
            }
        }
        return mTempData;
    }

    protected void InitSkills()
    {
        for (int i = 0; i < attrNode.skill_id.Length; i++)
        {
            if (!skill.ContainsKey(attrNode.skill_id[i]))
                skill.Add(attrNode.skill_id[i], 0);
        }
    }
}

public class MonsterData : CharacterData
{

    public float lvlRate = 0.5f;

    public MonsterData(long id)
    {
        if (id == 0) return;
        this.id = id;
        if (FSDataNodeTable<MonsterAttrNode>.GetSingleton().DataNodeList.ContainsKey(id))
        {
            attrNode = FSDataNodeTable<MonsterAttrNode>.GetSingleton().DataNodeList[id];
            InitSkills();
        }
        else
        {
            Debug.LogError(id + " == >Monsterattr not find");
        }
    }

    public MonsterData(long id, int level) : this(id)
    {
        this.lvl = level;
    }

    public MonsterData(long id, int level, int[] skillLvs) : this(id, level)
    {
        for (int i = 0; i < attrNode.skill_id.Length; i++)
        {
            int sLv = skillLvs.Length > i ? skillLvs[i] : 0;
            if (skill.ContainsKey(attrNode.skill_id[i]))
                skill[attrNode.skill_id[i]] = sLv;
            else
                skill.Add(attrNode.skill_id[i], sLv);
        }
    }

    public MonsterData(long id, int level, int skillLv) : this(id, level, new int[1] { skillLv })
    {

    }
}

public class HeroData : CharacterData
{
    public long playerId;       //所属玩家ID
    public bool actived;        //是否已激活
    public bool isPlay;         //是否出战
    public bool isGetData;      //是否获取过数据

    public int star = 1;        //英雄星级
    int _grade = 1;
    public int grade      //英雄品阶
    {
        get
        {
            return _grade;
        }
        set
        {
            _grade = value;
            SetNodeByIdAndGrade(id, _grade);
        }
    }

    public int fc;              //英雄战斗力
    public int exps;            //当前经验值
    public int maxExps;         //最大经验值

    public HeroNode node;         //配置属性

    public Dictionary<int, EquipData> equipSite = new Dictionary<int, EquipData>();      //拥有装备
    public float[] equipTotalAttrs = new float[Formula.ATTR_COUNT];      // 装备属性加成总值
    public long[] runes = new long[4];

    public HeroData(long mId)
    {
        SetNodeByIdAndGrade(mId, grade);
        InitSkills();
    }

    public HeroData(long mId, int mLevel)
    {
        lvl = mLevel;
        SetNodeByIdAndGrade(mId, grade);
        InitSkills();
    }

    public HeroData(long mId, int mLevel, int mGrade, int mStar)
    {
        id = mId - mId % 100;
        lvl = mLevel;
        star = mStar;
        grade = mGrade;
        SetNodeByIdAndGrade(mId, grade);
        InitSkills();
    }

    public HeroData(object data)
    {
        SetData(data);
    }

    public void SetData(object data)
    {
        if (null == data)
        {
            Debug.LogError("英雄初始数据为空");
            return;
        }

        Dictionary<string, object> dict = data as Dictionary<string, object>;

        if (dict.ContainsKey("id"))
            id = Convert.ToInt64(dict["id"]);
        if (id <= 0)
            return;
        if (dict.ContainsKey("lv"))
            lvl = Convert.ToInt32(dict["lv"]);
        if (dict.ContainsKey("fc"))
            fc = Convert.ToInt32(dict["fc"]);
        if (dict.ContainsKey("grade"))
            grade = Convert.ToInt32(dict["grade"]);
        if (dict.ContainsKey("star"))
            star = Convert.ToInt32(dict["star"]);
        SetNodeByIdAndGrade(id, grade);
        InitSkills();

        if (dict.ContainsKey("exps"))
            exps = int.Parse(dict["exps"].ToString());
        if (dict.ContainsKey("maxExps"))
            maxExps = int.Parse(dict["maxExps"].ToString());
        //zctodo
        if (dict.ContainsKey("attr"))
        {
            object[] attrs = (object[])dict["attr"];
            for (int i = 0; i < Formula.ATTR_COUNT; i++)
            {
                float attrVal = attrs[i] != null ? float.Parse(attrs[i].ToString()) : 0f;
                Formula.SetAttrTo(ref serverAttrs, (AttrType)i, attrVal);
            }
        }
        else
        {
            if (dict.ContainsKey("power"))
                Formula.SetAttrTo(ref serverAttrs, AttrType.power, float.Parse(dict["power"].ToString()));
            if (dict.ContainsKey("intelligence"))
                Formula.SetAttrTo(ref serverAttrs, AttrType.intelligence, float.Parse(dict["intelligence"].ToString()));
            if (dict.ContainsKey("agility"))
                Formula.SetAttrTo(ref serverAttrs, AttrType.agility, float.Parse(dict["agility"].ToString()));
            if (dict.ContainsKey("hp"))
                Formula.SetAttrTo(ref serverAttrs, AttrType.hp, float.Parse(dict["hp"].ToString()));
            if (dict.ContainsKey("attack"))
                Formula.SetAttrTo(ref serverAttrs, AttrType.attack, float.Parse(dict["attack"].ToString()));
            if (dict.ContainsKey("dodge"))
                Formula.SetAttrTo(ref serverAttrs, AttrType.dodge, float.Parse(dict["dodge"].ToString()));
            if (dict.ContainsKey("armor"))
                Formula.SetAttrTo(ref serverAttrs, AttrType.armor, float.Parse(dict["armor"].ToString()));
            if (dict.ContainsKey("magic_resist"))
                Formula.SetAttrTo(ref serverAttrs, AttrType.magic_resist, float.Parse(dict["magic_resist"].ToString()));
            if (dict.ContainsKey("armor_penetration"))
                Formula.SetAttrTo(ref serverAttrs, AttrType.armor_penetration, float.Parse(dict["armor_penetration"].ToString()));
            if (dict.ContainsKey("suck_blood"))
                Formula.SetAttrTo(ref serverAttrs, AttrType.suck_blood, float.Parse(dict["suck_blood"].ToString()));
            if (dict.ContainsKey("tenacity"))
                Formula.SetAttrTo(ref serverAttrs, AttrType.tenacity, float.Parse(dict["tenacity"].ToString()));
            if (dict.ContainsKey("critical"))
                Formula.SetAttrTo(ref serverAttrs, AttrType.critical, float.Parse(dict["critical"].ToString()));
            if (dict.ContainsKey("hit_ratio"))
                Formula.SetAttrTo(ref serverAttrs, AttrType.hit_ratio, float.Parse(dict["hit_ratio"].ToString()));
            if (dict.ContainsKey("magic_penetration"))
                Formula.SetAttrTo(ref serverAttrs, AttrType.magic_penetration, float.Parse(dict["magic_penetration"].ToString()));
        }

        if (dict.ContainsKey("runes"))
        {
            Dictionary<string, object> runesDic = dict["runes"] as Dictionary<string, object>;
            foreach (string str in runesDic.Keys)
            {
                runes[int.Parse(str) - 1] = long.Parse(runesDic[str].ToString());
            }
        }

        if (dict.ContainsKey("skill"))
        {
            Dictionary<string, object> skillListDic = dict["skill"] as Dictionary<string, object>;
            foreach (KeyValuePair<string, object> sKvp in skillListDic)
            {
                if (skill.ContainsKey(long.Parse(sKvp.Key)))
                    skill[long.Parse(sKvp.Key)] = (int)sKvp.Value;
                else
                    skill.Add(long.Parse(sKvp.Key), (int)sKvp.Value);
            }
        }

        if (dict.ContainsKey("equip"))
        {
            Dictionary<string, object> itemDic = dict["equip"] as Dictionary<string, object>;
            //Dictionary<long, EquipData> equipID = new Dictionary<long, EquipData>();
            Dictionary<int, EquipData> equipDict = new Dictionary<int, EquipData>();
            Dictionary<string, object> itemI;

            object obj = null;

            object itemId = 0;
            object itemGrade = 0;
            object itemLv = 0;

            for (int i = 1; i <= itemDic.Count; i++)
            {

                itemDic.TryGetValue("" + i, out obj);

                itemI = (Dictionary<string, object>)obj;

                EquipData ed = new EquipData();

                itemI.TryGetValue("id", out itemId);
                itemI.TryGetValue("grade", out itemGrade);
                itemI.TryGetValue("lvl", out itemLv);

                ed.id = int.Parse(itemId.ToString()) + int.Parse(itemGrade.ToString());
                ed.grade = int.Parse(itemGrade.ToString());
                ed.level = int.Parse(itemLv.ToString());
                ed.site = i;

                //equipID.Add(ed.id, ed);
                equipDict.Add(i, ed);
            }

            //hd.equipID = equipID;
            equipSite.Clear();
            equipSite = equipDict;
            CalculateEquipsTotalAttr();
        }
    }

    void SetNodeByIdAndGrade(long mId, int mGrade)
    {
        id = mId - mId % 100;
        if (mId % 100 != 0 && mId % 100 != mGrade)
        {
            Debug.LogError("英雄品阶与ID对应错误");
        }

        if (FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList.ContainsKey(id))
        {
            node = FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList[id];
            attrNode = FSDataNodeTable<HeroAttrNode>.GetSingleton().DataNodeList[id + mGrade];
        }
        else
        {
            Debug.LogError("错误的英雄ID：" + mId);
        }
    }

    public void RefreshAttr()
    {
        if (equipSite.Count <= 0)
        {
            AddFakeEquips();
        }
        bool saveOld = useServerAttr;
        useServerAttr = false;
        CalculateEquipsTotalAttr();
        Formula.SetAttrTo(ref charAttrs, AttrType.power, Formula.GetAProperty(this, AttrType.power));
        Formula.SetAttrTo(ref charAttrs, AttrType.intelligence, Formula.GetAProperty(this, AttrType.intelligence));
        Formula.SetAttrTo(ref charAttrs, AttrType.agility, Formula.GetAProperty(this, AttrType.agility));
        for (int i = 3; i < charAttrs.Length; i++)
        {
            float newVal = Formula.GetSingleAttribute(this, (AttrType)i);
            Formula.SetAttrTo(ref charAttrs, (AttrType)i, newVal);
        }
        fc = GetFC();
        useServerAttr = saveOld;
    }

    public static HeroData[] GetHeroDatasFromDict(Dictionary<string, object> heroDict, int len)
    {
        HeroData[] heroDatas = new HeroData[len];
        int i = 1;
        foreach (string k in heroDict.Keys)
        {
            if (k == "1")
            {
                heroDatas[0] = new HeroData(heroDict[k]);
            }
            else
            {
                heroDatas[i] = new HeroData(heroDict[k]);
                i++;
            }
        }
        return heroDatas;
    }

    int GetFC()
    {
        AfcNode afc = FSDataNodeTable<AfcNode>.GetSingleton().FindDataByType(1);
        float fc = 0;
        float log = 0f;
        for (int i = 0; i < 11; i++)
        {
            log = Formula.GetSingleAttribute(this, (AttrType)i + 3);
            fc += log * afc.attrRate[i];
        }
        //技能
        List<long> skillKey = new List<long>(skill.Keys);
        if (skillKey.Count < 4)
        {
            fc += 4 * afc.skill;
        }
        else
        {
            int index = 0;
            for (int i = skillKey.Count - 1; i >= 0; i--)
            {
                fc += (skill[skillKey[i]] != 0 ? skill[skillKey[i]] : 1) * afc.skill;
                index++;
                if (index > 3) break;
            }
        }
        return Mathf.FloorToInt(fc);
    }

    public void AddFakeEquips(int lvl = 1, int grade = 1)
    {
        if (lvl <= 0)
            return;
        EquipData equip;
        for (int i = 0; i < ((HeroAttrNode)attrNode).equipment.Length; i++)
        {
            equip = new EquipData();
            equip.id = (int)((HeroAttrNode)attrNode).equipment[i] + grade;
            equip.level = lvl;
            equip.grade = grade;
            equip.site = i + 1;
            equipSite.Add(i + 1, equip);
        }
    }

    void CalculateEquipsTotalAttr()
    {
        for (int i = 0; i < equipTotalAttrs.Length; i++)
        {
            equipTotalAttrs[i] = 0f;
        }
        foreach (int k in equipSite.Keys)
        {
            EquipData ed = equipSite[k];
            ItemNodeState baseItem = GameLibrary.Instance().ItemStateList[ed.baseId];
            ItemNodeState upgradeItem = GameLibrary.Instance().ItemStateList[ed.id];
            equipTotalAttrs[0] += upgradeItem.power * ed.level + baseItem.power;
            equipTotalAttrs[1] += upgradeItem.intelligence * ed.level + baseItem.intelligence;
            equipTotalAttrs[2] += upgradeItem.agility * ed.level + baseItem.agility;
            equipTotalAttrs[3] += upgradeItem.hp * ed.level + baseItem.hp;
            equipTotalAttrs[4] += upgradeItem.attack * ed.level + baseItem.attack;
            equipTotalAttrs[5] += upgradeItem.armor * ed.level + baseItem.armor;
            equipTotalAttrs[6] += upgradeItem.magic_resist * ed.level + baseItem.magic_resist;
            equipTotalAttrs[7] += upgradeItem.critical * ed.level + baseItem.critical;
            equipTotalAttrs[8] += upgradeItem.dodge * ed.level + baseItem.dodge;
            equipTotalAttrs[9] += upgradeItem.hit_ratio * ed.level + baseItem.hit_ratio;
            equipTotalAttrs[10] += upgradeItem.armor_penetration * ed.level + baseItem.armor_penetration;
            equipTotalAttrs[11] += upgradeItem.magic_penetration * ed.level + baseItem.magic_penetration;
            equipTotalAttrs[12] += upgradeItem.suck_blood * ed.level + baseItem.suck_blood;
            equipTotalAttrs[13] += upgradeItem.tenacity * ed.level + baseItem.tenacity;
        }
    }
}

public class GuideData
{
    public Dictionary<int, int> GuideLibrary = new Dictionary<int, int>();
    public Dictionary<int, bool> guideActive = new Dictionary<int, bool>();
    public Dictionary<int, bool> guideEnd = new Dictionary<int, bool>();
    public int guideId;
    public int Unlock;
    public long guideUnlockId;
    public int scripId;
    public int typeId;
    public int stepId;
    public int uId;
    public int state;
}

public class EquipData
{
    public long id;          //装备ID
    public long baseId
    {
        get
        {
            return id - id % 100;      //装备基础ID
        }
    }
    public int grade;       //装备品阶
    public int level;       //装备等级
    public int site;        //装备位置
}
public class GoldHandData
{
    public int maxcount;//次数上线
    public int curcount;//当前可使用次数
    public int alreadyUseCount;//已经使用次数
    public int id;//当前阶段
    public int time;//本阶段点击次数

    public List<GoldHandItemData> goldHandList = new List<GoldHandItemData>();//使用点金手获得的item信息
}
public class GoldHandItemData
{
    public int jewelCount;//花费钻石数
    public int goldCount;//获得的金币数
    public bool isCrit;//是否暴击
    public int critCount;//暴击倍数
}
public class LotteryInfo
{
    public int goldDrawCount;//金币抽奖次数
    public int diamondDrawCounnt;//钻石抽奖次数
    public long goldTime;//金币下次免费抽奖时间
    public long diamondTime;//钻石下次免费抽奖时间
    public long shopTime;//商城下次刷新时间
    public int shopRefreshCount;//商城刷新次数
    public float sale;//特教打折
    public int page;//显示商店物品页数
    public List<int> refreshCost = new List<int>();//本地存储杂货铺价格
    public List<ItemData> shopItemList = new List<ItemData>();//商店物品列表
    //public List<int> jdcrefreshCostList = new List<int>();//
    //public List<int>jjcRefreshCostList=new List<int>();
    //public List<int>xsRefreshCostList=new List<int>();  
    public List<ItemData> hotList = new List<ItemData>();//每周热点
    public List<ItemData> itemList = new List<ItemData>();//金币抽奖，钻石抽奖
}
public delegate void ChangeMoney(MoneyType moneyType, long money);
public delegate void Changelv(int lv, int exp);
public delegate void ChangeActionPoint(ActionPointType type, int count);
public delegate void HaveNewMail(int count);
public delegate void HaveNewChat(ChatData chatData);

public class playerData
{
    static playerData instance;
    /// <summary>
    /// 暂定货币更改事件
    /// </summary>
    public event ChangeMoney ChangeMoneyEvent;
    public event Changelv ChangelvAndExpEvent;
    public event ChangeActionPoint ChangeActionPoint;
    public event HaveNewMail NewMailHint;
    public event HaveNewChat NewChatHint;
    public event Action ChangeActionPointCeiling;
    public HeroData selectHeroDetail;//申请英雄详细信息数据，当前只有一个
    public bool isEquipDevelop = true;
    public static playerData GetInstance()
    {
        if (instance == null)
            instance = new playerData();
        return instance;
    }
    //打包自己的数据给行走服务器
    public bool ChangeInfo()
    {
        string str = string.Format("{0},{1},{2},{3}", selfData.icon, selfData.heroId, selfData.level, selfData.playeName);
        serverMgr.chat_server_send.send(new string[] { "publish", "i" + selfData.playerId, str });//redis-cli publish broadcast online playerId
        return false;
    }
    public SingnData singnData = new SingnData();
    public GetEnergyData getEnergyData = new GetEnergyData();
    public MarqueeData marqueeData = new MarqueeData();
    public PlayerRankData playerRankData = new PlayerRankData();
    public IconData iconData = new IconData();
    public IcnFramData iconFrameData = new IcnFramData();
    public Roledata selfData = new Roledata();
    public BagData baginfo = new BagData();
    public GoldHandData goldHand = new GoldHandData();
    public LotteryInfo lotteryInfo = new LotteryInfo();
    public MailData mailData = new MailData();
    public IChat iChat = new IChat();
    public ActionData actionData = new ActionData();
    public FriendListData friendListData = new FriendListData();
    public EveryTaskDataList taskDataList = new EveryTaskDataList();
    public NewPlayerRewardList newPlayerRewardList = new NewPlayerRewardList();
    public GuideData guideData = new GuideData();
    public GameSet gameSet = new GameSet();
    public bool isInitNearList = false;
    public Dictionary<long, RoleInfo> NearRIarr = new Dictionary<long, RoleInfo>();//附近元素列表
    //  public Dictionary<string, Roledata>[] nearList;
    public void CreateNearList(object[] data)
    {
        for (int i = 0; i < data.Length; i++)
        {
            Roledata role = new Roledata();
            Dictionary<string, object> playerInfo = (Dictionary<string, object>)data[i];

            role.playerId = (UInt32)playerInfo["playerId"];
            role.heroId = (UInt32)playerInfo["heroId"];

            role.SetPos(new Vector3((int)playerInfo["mapX"], (int)playerInfo["mapY"], (int)playerInfo["mapZ"]));
            role.rotation = new Vector3((int)playerInfo["dirX"], (int)playerInfo["dirY"], (int)playerInfo["dirZ"]);
            nearList.Add(role.playerId, role);

            //  CharacterManager.instance.CreatNearList(role);
        }
        isInitNearList = true;
    }
    public Dictionary<long, Roledata> nearList = new Dictionary<long, Roledata>();//附近玩家列表

    public Dictionary<int, List<MarqueeData>> marqueeListDic = new Dictionary<int, List<MarqueeData>>();//跑马灯

    //拥有的英雄集合
    public List<HeroData> herodataList = new List<HeroData>();

    public List<int> CanEnterMap = new List<int>();
    public List<int> worldMap = new List<int>();
    public Dictionary<int, int> dungeonList = new Dictionary<int, int>();

    public int MaxArenaDare = 5;
    public int skillPoints = 20;
    public int GetHeroHp(long heroid)
    {
        //最大生命值=力量*23.5+（初始值+装备+符石）
        HeroAttrNode heroattr = FSDataNodeTable<HeroAttrNode>.GetSingleton().DataNodeList[heroid];
        if (heroattr != null)
        {
            float heroHp = 0;
            heroHp = (float)(heroattr.power * 23.5 + (heroattr.hp + heroattr.EquiplistHpadd() + RuneAddhp()));
        }
        return 0;

    }
    //当前英雄的符文增加的血量
    public float RuneAddhp()
    {
        float runhp = 0;
        return runhp;
    }

    /// <summary>
    /// 添加英雄
    /// </summary>
    /// <param name="heroID">整卡ID</param>
    public bool AddHeroToList(long heroID)
    {

        string _id = heroID.ToString();

        string _ids = _id.Substring(3, _id.Length - 3);

        long id = long.Parse("201" + _ids);

        for (int i = 0; i < herodataList.Count; i++)
        {
            if (herodataList[i].id == id)
            {
                return false;
            }
        }

        HeroData hd = new HeroData(id);
        hd.RefreshAttr();
        //hd.fc = HeroData.GetFC(hd);
        herodataList.Add(hd);

        return true;

    }

    public void RefreshHeroToList(long heroID, int star = 0, int lvl = 0, int grade = 0)
    {

        string _id = heroID.ToString();

        string _ids = _id.Substring(3, _id.Length - 3);

        long id = long.Parse("201" + _ids);

        bool isHave = false;

        for (int i = 0; i < herodataList.Count; i++)
        {
            if (herodataList[i].id == id)
            {
                herodataList[i].star = star != 0 ? star : 1;
                herodataList[i].lvl = lvl != 0 ? lvl : 1;
                herodataList[i].grade = grade != 0 ? grade : 1;
                isHave = true;
                break;
            }
        }

        if (!isHave)
        {
            HeroData hd = new HeroData(id, lvl != 0 ? lvl : 1, grade != 0 ? grade : 1, star != 0 ? star : 1);
            hd.RefreshAttr();
            //hd.fc = HeroData.GetFC(hd);
            herodataList.Add(hd);
        }

    }

    public void ReplaceHeroData(long id, object data)
    {
        for (int i = 0; i < herodataList.Count; i++)
        {
            if (herodataList[i].id == id)
            {
                herodataList[i].SetData(data);
                break;
            }
        }
    }

    /// <summary>
    /// 通过ID查找背包物品
    /// </summary>
    /// <param name="itemid">物品id</param>
    /// <returns></returns>
    public ItemData GetItemDatatByID(long itemid)
    {
        if (baginfo.ItemDic.ContainsKey(itemid))
        {
            return baginfo.ItemDic[itemid];
        }
        return null;
    }

    /// <summary>
    /// 通过物品类型查找背包物品
    /// </summary>
    /// <param name="type">物品类型</param>
    /// <returns></returns>
    public List<ItemData> GetItemListByItmeType(ItemType type)
    {
        List<ItemData> list = new List<ItemData>();
        foreach (ItemData item in baginfo.ItemDic.Values)
        {
            if ((int)type == GameLibrary.Instance().ItemStateList[item.Id].types)
            {
                list.Add(item);
            }
        }
        return list;
    }

    /// <summary>
    /// 通过物品类类型查找背包物品
    /// </summary>
    /// <param name="type">可变参数（传递多个类型）</param>
    /// <returns></returns>
    public List<ItemData> GetItemListByItmeType(params ItemType[] type)
    {
        List<ItemData> list = new List<ItemData>();
        foreach (ItemData item in baginfo.ItemDic.Values)
        {
            for (int j = 0; j < type.Length; j++)
            {
                if ((int)type[j] == GameLibrary.Instance().ItemStateList[item.Id].types)//((int)type[j] == (VOManager.Instance().GetCSV<ItemCSV>("Item").GetVO((baginfo.itemlist[i].Id)).types))
                {
                    list.Add(item);
                }
            }
        }
        return list;
    }

    /// <summary>
    /// 通过ID查找物品数量
    /// </summary>
    /// <param name="itemid">物品id</param>
    /// <returns></returns>
    public int GetItemCountById(long itemid)
    {
        if (baginfo.ItemDic.ContainsKey(itemid))
        {
            return baginfo.ItemDic[itemid].Count;
        }

        return 0;
    }

    public int beforePlayerLevel;
    public int beforeStrength;//之前的体力
    /// <summary>
    /// 通过ID查找英雄信息
    /// </summary>
    /// <param name="id">英雄ID</param>
    /// <returns></returns>
    public HeroData GetHeroDataByID(long id)
    {
        if (id <= 0)
            return null;
        for (int i = 0; i < herodataList.Count; i++)
        {
            if (herodataList[i].id == id)
                return herodataList[i];
        }
        return null;
    }

    public HeroData FindOrNewHeroDataById(long id)
    {
        HeroData hd = GetHeroDataByID(id);
        return hd == null ? new HeroData(id) : hd;
    }

    public long[] GetHeroFormationIds(OpenSourceType type = OpenSourceType.Moba)
    {
        HeroData[] hero = null;
        switch (type)
        {
            case OpenSourceType.Moba:
                hero = Globe.mobaMyTeam;
                break;
            case OpenSourceType.Moba3V3:
                hero = Globe.moba3v3MyTeam1;
                break;
            case OpenSourceType.Arena:
                hero = Globe.challengeTeam;
                break;
        }

        if (null == hero) return null;

        long[] heroIds = new long[hero.Length];
        heroIds = new long[hero.Length];
        for (int i = 0; i < hero.Length; i++)
        {
            heroIds[i] = hero[i] != null ? hero[i].id : 0;
        }

        return heroIds;
    }

    public Dictionary<string, string> GetHeroFormationIds(HeroData[] hero, bool isArena = false)
    {
        if (null == hero) return null;
        Dictionary<string, string> heroList = new Dictionary<string, string>();

        for (int i = 0; i < hero.Length; i++)
        {
            heroList.Add((i + 1).ToString(), hero[i] != null ? hero[i].id + "" : 0 + "");
        }

        if (isArena)
        {
            heroList["6"] = Globe.arenaFightHero[5].ToString();
        }

        return heroList;
    }
    /// <summary>
    /// 接收服务器返回的等级和经验并同步显示
    /// </summary>
    /// <param name="Lv"></param>
    public void RoleLvAndExpHandler(int Lv, int exp)
    {
        //beforePlayerLevel = selfData.level;
        selfData.level = Lv;
        selfData.exprience = exp;
        if (ChangelvAndExpEvent != null)
        {
            ChangelvAndExpEvent(selfData.level, selfData.exprience);
        }
    }
    /// <summary>
    /// 接收服务器返回的货币并同步显示
    /// </summary>
    /// <param name="index"></param>
    /// <param name="money"></param>
    public void RoleMoneyHadler(MoneyType moneyType, long money)
    {
        switch (moneyType)
        {
            case MoneyType.Gold:
                if (money >= 0 && money <= 2100000000)
                    playerData.GetInstance().baginfo.gold = money;
                else if (money > 2100000000)
                    playerData.GetInstance().baginfo.gold = 2100000000;//金币的上限是21亿 再多系统回收
                else if (money < 0)
                    playerData.GetInstance().baginfo.gold = 0;
                if (ChangeMoneyEvent != null)
                {

                    ChangeMoneyEvent(moneyType, playerData.GetInstance().baginfo.gold);
                }
                break;
            case MoneyType.Diamond:
                playerData.GetInstance().baginfo.diamond = money;
                if (ChangeMoneyEvent != null)
                {

                    ChangeMoneyEvent(moneyType, playerData.GetInstance().baginfo.diamond);
                }
                break;
            case MoneyType.PVPcoin:
                playerData.GetInstance().baginfo.pvpCoin = money;
                if (ChangeMoneyEvent != null)
                {
                    ChangeMoneyEvent(moneyType, playerData.GetInstance().baginfo.pvpCoin);
                }
                break;
            case MoneyType.AreanCoin:
                playerData.GetInstance().baginfo.areanCoin = money;
                if (ChangeMoneyEvent != null)
                {
                    ChangeMoneyEvent(moneyType, playerData.GetInstance().baginfo.areanCoin);
                }
                break;
            case MoneyType.PVEcion:
                playerData.GetInstance().baginfo.pveCoin = money;
                if (ChangeMoneyEvent != null)
                {
                    ChangeMoneyEvent(moneyType, playerData.GetInstance().baginfo.pveCoin);
                }
                break;
            case MoneyType.RewardCoin:
                playerData.GetInstance().baginfo.rewardCoin = money;
                if (ChangeMoneyEvent != null)
                {
                    ChangeMoneyEvent(moneyType, playerData.GetInstance().baginfo.rewardCoin);
                }
                break;
        }

    }

    /// <summary>
    /// 增加或减少货币同步显示（在发送服务器之前）
    /// </summary>
    /// <param name="moneyType"></param>
    /// <param name="money"></param>
    public void MoneyHadler(MoneyType moneyType, long money)
    {
        switch (moneyType)
        {
            case MoneyType.Gold:
                long gold = playerData.GetInstance().baginfo.gold + money;
                if (gold >= 0 && gold <= 2100000000)
                    playerData.GetInstance().baginfo.gold = gold;
                else if (gold > 2100000000)
                    playerData.GetInstance().baginfo.gold = 2100000000;//金币的上限是21亿 再多系统回收
                else if (gold < 0)
                    playerData.GetInstance().baginfo.gold = 0;
                if (ChangeMoneyEvent != null)
                {
                    ChangeMoneyEvent(moneyType, playerData.GetInstance().baginfo.gold);
                }
                break;
            case MoneyType.Diamond:
                playerData.GetInstance().baginfo.diamond += money;
                if (ChangeMoneyEvent != null)
                {
                    ChangeMoneyEvent(moneyType, playerData.GetInstance().baginfo.diamond);
                }
                break;
        }

    }

    /// <summary>
    /// 接收服务器返回的行动点并同步显示
    /// </summary>
    /// <param name="type"></param>
    /// <param name="count">服务器返回来的体力或者活力总数值</param>
    public void ActionPointHandler(ActionPointType type, int count)
    {
        switch (type)
        {
            case ActionPointType.Vitality:
                baginfo.vitality = count;
                if (ChangeActionPoint != null && Singleton<SceneManage>.Instance.Current == EnumSceneID.UI_MajorCity01)
                {
                    ChangeActionPoint(type, baginfo.vitality);
                }
                break;
            case ActionPointType.Energy:
                baginfo.strength = count;
                if (ChangeActionPoint != null && Singleton<SceneManage>.Instance.Current == EnumSceneID.UI_MajorCity01)
                {
                    ChangeActionPoint(type, baginfo.strength);
                }
                break;
        }
    }

    /// <summary>
    /// 增加行动点并同步显示（用于行动点的自动恢复）
    /// </summary>
    /// <param name="moneyType"></param>
    /// <param name="money">体力或者活力改变的数值</param>
    public void ChangeActionPointHandler(ActionPointType type, int count)
    {

        switch (type)
        {
            case ActionPointType.Vitality:

                if ((baginfo.vitality + count) >= actionData.maxVitalityCount)
                {
                    baginfo.vitality = playerData.GetInstance().actionData.maxVitalityCount;
                }
                else
                {
                    baginfo.vitality += count;
                }

                if (ChangeActionPoint != null && Singleton<SceneManage>.Instance.Current == EnumSceneID.UI_MajorCity01)
                {
                    ChangeActionPoint(type, baginfo.vitality);
                }
                break;
            case ActionPointType.Energy:
                if ((baginfo.strength + count) >= playerData.GetInstance().actionData.maxEnergyCount)
                {
                    baginfo.strength = playerData.GetInstance().actionData.maxEnergyCount;
                }
                else
                {
                    baginfo.strength += count;
                }
                if (ChangeActionPoint != null && Singleton<SceneManage>.Instance.Current == EnumSceneID.UI_MajorCity01)
                {
                    ChangeActionPoint(type, baginfo.strength);
                }
                break;
        }
    }
    /// <summary>
    /// 使用行动点
    /// </summary>
    /// <param name="type"></param>
    /// <param name="count"></param>
    public void UseActionPoint(ActionPointType type, int count)
    {
        switch (type)
        {
            case ActionPointType.Vitality:

                if ((baginfo.vitality - count) < 0)
                {
                    Debug.Log("活力不足");
                    return;
                }
                else
                {
                    baginfo.vitality -= count;
                }

                if (ChangeActionPoint != null && SceneManager.GetActiveScene().name == GameLibrary.UI_Major)
                {
                    ChangeActionPoint(type, baginfo.vitality);
                }
                break;
            case ActionPointType.Energy:
                if ((baginfo.strength - count) < 0)
                {
                    Debug.Log("体力不足");
                    return;
                }
                else
                {
                    baginfo.strength -= count;
                }
                if (ChangeActionPoint != null && SceneManager.GetActiveScene().name == GameLibrary.UI_Major)
                {
                    ChangeActionPoint(type, baginfo.strength);
                }
                break;
        }
    }
    /// <summary>
    /// 新邮件通知(服务器有新邮件通知 更新界面提示)
    /// </summary>
    /// <param name="count"></param>
    public void NewMailHandler(int count)
    {
        if (NewMailHint != null)
        {
            NewMailHint(count);
        }
    }

    /// <summary>
    /// 删除邮件成功后 本地移除该邮件
    /// </summary>
    public void RemoveSingleMailItem(long mailId)
    {
        for (int i = 0; i < mailData.mailItemList.Count; i++)
        {
            if (mailData.mailItemList[i].Id == mailId)
            {
                mailData.mailItemList.RemoveAt(i);
                mailData.currentCount--;
                break;
            }
        }
        //删除邮件时候再获取一下未读邮件个数，因为可能删除未读邮件 这时候未读数量会变化
        Singleton<Notification>.Instance.Send(MessageID.common_new_mail_count_req, C2SMessageType.ActiveWait);
        //ClientSendDataMgr.GetSingle().GetMailSend().SendGetNewMailCount(C2SMessageType.Active);
    }
    /// <summary>
    /// 新聊天通知 更新主城界面单条显示
    /// </summary>
    public void NewChatHandler(ChatData chatData)
    {
        if (NewChatHint != null)
        {
            NewChatHint(chatData);
        }
    }

    /// <summary>
    /// 用于本地增加聊天提示之类的信息
    /// </summary>
    /// <param name="contentType"></param>
    public void AddChatInfo(ChatContentType contentType, string inputContent)
    {
        ChatData chatData = new ChatData();
        chatData.Type = Globe.selectChatChannel;
        chatData.IsLocalPlayer = false;
        chatData.SpeakingTime = Auxiliary.GetNowTime();
        chatData.Time = Convert.ToDateTime(PropertyManager.ConvertIntDateTime(chatData.SpeakingTime)).ToString("HH:mm");
        //聊天内容类型 1 玩家或者服务器发言内容 2 不能发言（没满足等级或者没vip） 3 不能发言太快 4 没有队伍 5 没有公会 6：世界频道发言次数限制
        switch (contentType)
        {
            case ChatContentType.TextContent:
                chatData.ContentType = ChatContentType.TextContent;
                chatData.IsLocalPlayer = true;
                chatData.ChatContent = inputContent;
                chatData.Vip = 0;
                chatData.NickName = selfData.playeName;
                chatData.HeadId = iconData.icon_id;
                break;
            case ChatContentType.NotSpeak:
                chatData.ContentType = ChatContentType.NotSpeak;
                chatData.ChatContent = "需要玩家达到11级或vip1才能发言";
                break;
            case ChatContentType.NotSpeakFast:
                chatData.ContentType = ChatContentType.NotSpeakFast;
                double dtemp = Math.Round(UIChat.Instance.speakTimer, 1);
                double itemp = Math.Round(UIChat.Instance.speakTimer, 0);
                if (itemp > 0)
                {
                    chatData.ChatContent = "发言过快，请在" + itemp + "s后重试";

                }
                else
                {
                    chatData.ChatContent = "发言过快，请在1s后重试";
                }
                break;
            case ChatContentType.NotTeam:
                chatData.ContentType = ChatContentType.NotTeam;
                chatData.ChatContent = "暂未开放";
                break;
            case ChatContentType.NotSociety:
                chatData.ContentType = ChatContentType.NotTeam;
                chatData.ChatContent = "暂未开放";
                break;
            case ChatContentType.SpeakTimesLimit:
                chatData.ContentType = ChatContentType.SpeakTimesLimit;
                chatData.ChatContent = "超过10条,请购买喇叭";
                break;
            case ChatContentType.NotPrivateTarget:
                chatData.ContentType = ChatContentType.NotPrivateTarget;
                chatData.ChatContent = "没选择私聊对象";
                break;
            case ChatContentType.PrivateTargetNotOnLine:
                chatData.ContentType = ChatContentType.PrivateTargetNotOnLine;
                chatData.ChatContent = "信息发送失败";
                break;
            case ChatContentType.NoCharacter:
                chatData.ContentType = ChatContentType.NoCharacter;
                chatData.ChatContent = "聊天内容不能为空";
                break;
        }

        AddChatInfoToList(chatData);
    }

    public void AddChatInfoToList(ChatData chatData)
    {
        //聊天频道类型 加入到相应的list中
        switch (chatData.Type)
        {
            case ChatType.WorldChat:
                iChat.worldChatList.Add(chatData);
                Globe.worldChatUnReadCount++;
                break;
            case ChatType.SocietyChat:
                iChat.societyChatList.Add(chatData);
                Globe.societyChatUnReadCount++;
                break;
            case ChatType.PrivateChat:
                iChat.privateChatList.Add(chatData);
                Globe.privateChatUnReadCount++;
                //私聊频道 并且是玩家内容 全部都需要加
                if (chatData.ContentType == ChatContentType.TextContent)
                {
                    iChat.worldChatList.Add(chatData);
                    iChat.societyChatList.Add(chatData);
                    iChat.nearbyChatList.Add(chatData);
                    iChat.troopsChatList.Add(chatData);
                    iChat.systemChatList.Add(chatData);
                    //Globe.worldChatUnReadCount++;
                    //Globe.societyChatUnReadCount++;
                    //Globe.nearbyChatUnReadCount++;
                    //Globe.troopsChatUnReadCount++;
                    //Globe.systemChatUnReadCount++;

                }
                break;
            case ChatType.NearbyChat:
                iChat.nearbyChatList.Add(chatData);
                Globe.nearbyChatUnReadCount++;
                break;
            case ChatType.TroopsChat:
                iChat.troopsChatList.Add(chatData);
                Globe.troopsChatUnReadCount++;
                break;
            case ChatType.SystemChat:
                iChat.systemChatList.Add(chatData);
                Globe.systemChatUnReadCount++;
                break;
        }
        //if (Singleton<SceneManage>.Instance.Current == EnumSceneID.UI_MajorCity01&&Control.GetGUI(GameLibrary.UIChatPanel).gameObject.activeSelf)
        //{
        //    UIChatPanel.Instance.InitChatInfoList();
        //    UIChatPanel.Instance.RefreshChatShow();
        //}
        if (chatData.ContentType!=ChatContentType.TextContent||(SocietyManager.Single().selfChatData != null && Globe.selectChatChannel == ChatType.PrivateChat))
        {
            Singleton<Notification>.Instance.ReceiveMessageList(MessageID.common_server_chat_msg_notify);
            SocietyManager.Single().selfChatData = null;
        }
        //只有在主城才刷新新消息提示
        if (chatData.ContentType == ChatContentType.TextContent && SceneManager.GetActiveScene().name == GameLibrary.UI_Major)
        {
            UIChat.Instance.SetNewChatHint(chatData);
            //NewChatHandler(chatData);
        }
    }

    /// <summary>
    /// 初始化行动点的参数
    /// </summary>
    public void InitActionData()
    {
        if (FSDataNodeTable<PlayerLevelUpNode>.GetSingleton().DataNodeList != null)
        {
            actionData.maxEnergyCount = FSDataNodeTable<PlayerLevelUpNode>.GetSingleton().DataNodeList[selfData.level].maxPower;
            actionData.maxVitalityCount = FSDataNodeTable<PlayerLevelUpNode>.GetSingleton().DataNodeList[selfData.level].maxVitality;
        }
        actionData.vitalityTimeBucket = 6;// 活力恢复时间间隔
        actionData.energyTimeBucket = 6;//体力恢复时间间隔
        actionData.vitalityTime = (long)(actionData.vitalityTimeBucket * 60 * 1000);
        actionData.allVitalityTime = (long)(actionData.vitalityTimeBucket * 60 * 1000 * (actionData.maxVitalityCount - baginfo.vitality));
        if (baginfo.strength < actionData.maxEnergyCount)
        {
            //actionData.energyTime = (long)(actionData.energyTimeBucket * 60 * 1000);

            if ((actionData.energyRecoverEndTime - Auxiliary.GetNowTime()) > (actionData.maxEnergyCount - baginfo.strength) * 6 * 60)
            {
                actionData.energyTime = 6 * 60 * 1000;
                actionData.allEnergyTime = (actionData.maxEnergyCount - baginfo.strength) * 6 * 60 * 1000;

            }
            else
            {
                actionData.energyTime = (actionData.energyRecoverEndTime - Auxiliary.GetNowTime()) * 1000 - (actionData.maxEnergyCount - baginfo.strength - 1) * 6 * 60 * 1000;
                actionData.allEnergyTime = (actionData.energyRecoverEndTime - Auxiliary.GetNowTime()) * 1000;
            }
        }
        else
        {
            actionData.energyTime = 0;
            actionData.allEnergyTime = 0;
        }

        //actionData.allEnergyTime = (long)(actionData.energyTimeBucket * 60 * 1000 * (actionData.maxEnergyCount - baginfo.strength));

        GetJewelArray();

        actionData.buyEnergyCount = 120;//购买体力数量
        //actionData.energyBuyTimes = baginfo.todayBuyStrengthCount;// 体力购买次数
        //体力最大购买次数(读vip表 如果vip等级变化也要更新次数)暂时没有vip 是5次
        if (FSDataNodeTable<VipNode>.GetSingleton().DataNodeList.ContainsKey(selfData.vip))
        {
            actionData.maxEnergyBuyTimes = FSDataNodeTable<VipNode>.GetSingleton().DataNodeList[selfData.vip].buy_power;
        }
        actionData.EnergyNeedJewelCount = 50; //购买一次花费钻石数


        actionData.buyVitalityCount = 50;//购买活力数量
        if (baginfo != null)
        {
            actionData.vitalityBuyTimes = baginfo.todayBuyVitalityCount;// 活力购买次数
        }
        else {
            actionData.vitalityBuyTimes = 0;
        }
        actionData.maxVitalityBuyTimes = 5;//活力最大购买次数
        actionData.vitalityNeedJewelCount = 50; //购买一次活力花费钻石数 
    }
    /// <summary>
    /// 截取处理购买重置表中的数组
    /// </summary>
    private void GetJewelArray()
    {
        actionData.EnergyJewelArray = FSDataNodeTable<ResetLaterNode>.GetSingleton().DataNodeList[1].powerBuy;
        actionData.vitalityJewelArray = FSDataNodeTable<ResetLaterNode>.GetSingleton().DataNodeList[1].vitalityBuy;
    }
    /// <summary>
    /// 玩家升级自动增长行动点上限改变事件
    /// </summary>
    public void ChangeActionPointCeilingHandler()
    {
        if (selfData.level != 0)
        {
            if (FSDataNodeTable<PlayerLevelUpNode>.GetSingleton().DataNodeList.ContainsKey(selfData.level))
            {
                actionData.maxEnergyCount = FSDataNodeTable<PlayerLevelUpNode>.GetSingleton().DataNodeList[selfData.level].maxPower;
            }
        }
        else {
            if (FSDataNodeTable<PlayerLevelUpNode>.GetSingleton().DataNodeList.ContainsKey(1))
            {
                actionData.maxEnergyCount = FSDataNodeTable<PlayerLevelUpNode>.GetSingleton().DataNodeList[1].maxPower;
            }

        }
        //actionData.maxVitalityCount = FSDataNodeTable<PlayerLevelUpNode>.GetSingleton().FindDataByType(selfData.level).maxVitality;
        //actionData.maxEnergyCount = VOManager.Instance().GetCSV<PlayerUpgradeCSV>("PlayerUpgrade").GetVO(selfData.level).max_power;
        //actionData.maxVitalityCount = VOManager.Instance().GetCSV<PlayerUpgradeCSV>("PlayerUpgrade").GetVO(selfData.level).max_vitality;
        if (ChangeActionPointCeiling != null && SceneManager.GetActiveScene().name == GameLibrary.UI_Major)
        {
            ChangeActionPointCeiling();
        }
    }
    /// <summary>
    /// 刷新行动点恢复时间
    /// </summary>
    /// <param name="type"></param>
    public void UpdateCountdownTime(ActionPointType type)
    {
        if (type == ActionPointType.Energy)
        {
            //if (baginfo.strength < actionData.maxEnergyCount)
            //{
            //    //actionData.energyTime = (long)actionData.energyTimeBucket * 60 * 1000;
            //    //actionData.allEnergyTime = (long)((actionData.maxEnergyCount - baginfo.strength) * actionData.energyTimeBucket * 60 * 1000 + actionData.energyTime);
            //    actionData.allEnergyTime = (long)((actionData.maxEnergyCount - baginfo.strength - 1) * actionData.energyTimeBucket * 60 * 1000 + actionData.energyTime);

            //}
            //else
            //{
            //    actionData.energyTime = 0;
            //    actionData.allEnergyTime = 0;
            //}


            if (baginfo.strength < actionData.maxEnergyCount)
            {
                //actionData.energyTime = (long)(actionData.energyTimeBucket * 60 * 1000);
                actionData.energyTime = (actionData.energyRecoverEndTime - Auxiliary.GetNowTime()) * 1000 - (actionData.maxEnergyCount - baginfo.strength) * 6 * 1000;
                actionData.allEnergyTime = (actionData.energyRecoverEndTime - Auxiliary.GetNowTime()) * 1000;
            }
            else
            {
                actionData.energyTime = 0;
                actionData.allEnergyTime = 0;
            }


        }
        else if (type == ActionPointType.Vitality)
        {
            if (baginfo.vitality < actionData.maxVitalityCount)
            {
                //actionData.allVitalityTime = (long)((actionData.maxVitalityCount - baginfo.vitality) * actionData.vitalityTimeBucket * 60 * 1000 + actionData.vitalityTime);
                actionData.allVitalityTime = (long)((actionData.maxVitalityCount - baginfo.vitality - 1) * actionData.vitalityTimeBucket * 60 * 1000 + actionData.vitalityTime);
            }
        }
    }
    public void RefreshTime()
    {
        //if (baginfo.strength < actionData.maxEnergyCount)
        //{
        //    actionData.energyTime = (long)actionData.energyTimeBucket * 60 * 1000;
        //    actionData.allEnergyTime = (long)((actionData.maxEnergyCount - baginfo.strength) * actionData.energyTimeBucket * 60 * 1000);
        //}
        //else
        //{
        //    actionData.energyTime = 0;
        //    actionData.allEnergyTime = 0;
        //}

        if (baginfo.strength < actionData.maxEnergyCount)
        {
            //actionData.energyTime = (long)(actionData.energyTimeBucket * 60 * 1000);
            actionData.energyTime = (actionData.energyRecoverEndTime - Auxiliary.GetNowTime()) * 1000 - (actionData.maxEnergyCount - baginfo.strength) * 6 * 1000;
            actionData.allEnergyTime = (actionData.energyRecoverEndTime - Auxiliary.GetNowTime()) * 1000;
        }
        else
        {
            actionData.energyTime = 0;
            actionData.allEnergyTime = 0;
        }
    }

    public long GetMyMoneyByType(MoneyType type)
    {
        switch (type)
        {
            case MoneyType.Diamond:
                return baginfo.diamond;
            case MoneyType.PVPcoin:
                return baginfo.pvpCoin;
            case MoneyType.AreanCoin:
                return baginfo.areanCoin;
            case MoneyType.PVEcion:
                return baginfo.pveCoin;
            case MoneyType.RewardCoin:
                return baginfo.rewardCoin;
            default:
                return baginfo.gold;
        }
    }

    public string GetHeroGrade(int grade)
    {
        switch (grade)
        {
            case 1:
                return "bai";
            case 2:
                return "lv";
            case 3:
                return "lan";
            case 4:
                return "zi";
            case 5:
                return "cheng";
            case 6:
                return "hong";
            default:
                return "hui";
        }
    }
}