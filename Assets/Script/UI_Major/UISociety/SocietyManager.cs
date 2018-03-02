using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SocietyManager
{
    private static SocietyManager single;
    public static SocietyManager Single()
    {
        if (single == null)
        {
            single = new SocietyManager();

        }
        return single;
    }
    public int currentCheckBoxIndex = 0;//未加入公会界面 选择的页签索引

    public bool isCreateSociety = false;//创建公会 用于玩家创建公会成后之后 打开加入工会的面板
    public bool isJoinSociety = false;//是否加入公会，用于判断打开不同的面板
    public long mySocityID = 0;//公会id
    public string mySocietyIcon;//公会图标
    public string societyName = "";//公会名称
    public SocietyStatus societyStatus = SocietyStatus.Null;
    public SocietyData playerSocietyData = new SocietyData();//玩家的公会详细信息
    public SocietyData otherSocietyData;//其他公会的详细信息
    public List<SocietyData> societyList = new List<SocietyData>();//已创建的公会列表 一次获得10条 刷新一次再次获得10条
    public List<SocietyData> searchSocietyList = new List<SocietyData>();//搜索的公会列表
    //public List<int> playerApplicationSocietyList = new List<int>();//已申请的公会id列表
    public int[] playerApplicationSocietyList;//已申请的公会id列表
    public List<SocietyApplicationData> SocietyApplicationList = new List<SocietyApplicationData>();//玩家申请入会数据列表

    public List<SocietyMemberData> societyMemberlist = new List<SocietyMemberData>();//公会成员列表

    public ChatData selfChatData = null;
}
public class SocietyApplicationData
{
    /// <summary>
    /// 玩家id
    /// </summary>
    public long playerId;
    /// <summary>
    /// 玩家名称
    /// </summary>
    public string playeName;
    /// <summary>
    /// 申请时间
    /// </summary>
    public string applicationTime;
}
public class SocietyMemberData
{
    /// <summary>
    /// 角色id
    /// </summary>
    public long playerId;
    /// <summary>
    /// 帐号id
    /// </summary>
    public long accountId;
    /// <summary>
    /// 成员排名
    /// </summary>
    public int ranking;
    /// <summary>
    /// 成员图标
    /// </summary>
    public string memberIcon;
    /// <summary>
    /// 成员名字
    /// </summary>
    public string memberName;
    /// <summary>
    /// 身份
    /// </summary>
    public SocietyStatus societyStatus;
    /// <summary>
    /// 贡献度
    /// </summary>
    public long contributionValue;
    /// <summary>
    /// 周贡献度
    /// </summary>
    public long weekContributionValue;
    /// <summary>
    /// 在线状态
    /// </summary>
    public bool state;
}
public class SocietyData
{
    /// <summary>
    /// 公会ID
    /// </summary>
    public long societyID;
    /// <summary>
    /// 公会图标
    /// </summary>
    public string societyIcon;
    /// <summary>
    /// 公会名称
    /// </summary>
    public string societyName;
    /// <summary>
    /// 公会等级
    /// </summary>
    public int societyLevel;
    /// <summary>
    /// 会长ID
    /// </summary>
    public long presidentId;
    /// <summary>
    /// 会长名字
    /// </summary>
    public string presidentName;
    /// <summary>
    /// 公会当前人数
    /// </summary>
    public int societyCurrentCount;
    /// <summary>
    /// 公会人数上限
    /// </summary>
    public int societyMaxCount;
    /// <summary>
    /// 入会的等级
    /// </summary>
    public int joinLevel;
    /// <summary>
    /// 公会宣言
    /// </summary>
    public string societyManifesto;
    /// <summary>
    /// 创建时间
    /// </summary>
    public string createTime;
}
