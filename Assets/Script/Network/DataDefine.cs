using System;
 
public sealed class DataDefine
{
	public DataDefine ()
	{
	}
	public const float	netTimeCancel = 600;
    public const string version =  "1.2.0209";
	public const bool isOutLine =false;
    public const bool isSkipFunction = false;
    public const string inserverip = "http://192.168.3.251/res/";
    public const string outserverip = "http://114.215.78.17/res/";
    //public const string ServerListOutLineUrl = "http://114.215.78.17:8090/ac/serverList.action?account={0}&&types={1}";http://192.168.3.251/api/servelist?account=账户&types=登录类型
    public const string ServerListOutLineUrl = "http://114.215.78.17/api/serverlist";//?account={0}&&types={1}"
	public const string ServerListUrl = "http://192.168.3.251/api/serverlist";//?account={0}&&types={1}"

    //public const string LoginOutLineUrl = "http://www.tianyuonline.cn/mp/ac/Login.php?mobile={0}&passwd={1}";http://192.168.3.251/api/login?account=账户&passwd=密码&types=登录类型
    public const string LoginOutLineUrl = "http://114.215.78.17/api/login";//?account={0}&&passwd={1}";//"http://114.215.78.17/mp/ac/Login.php?mobile={0}&passwd={1}";
    public const string LoginOutLineUrl360 = "http://114.215.78.17/api/extendaccount";
    public const string LoginUrl = "http://192.168.3.251/api/login";//?account={0}&&passwd={1}&&types={2}"

	public const string RegistOutLineUrl = "http://114.215.78.17/api/reg?mobile={0}&passwd={1}&cv={2}&udid={3}+&mc={4}";//"http://114.215.78.17/mp/ac/Reg.php?mobile={0}&passwd={1}";
    public const string RegistLineUrl = "http://192.168.3.251/api/reg?mobile={0}&passwd={1}&cv={2}&udid={3}+&mc={4}";//


    public const int mainchannel = 1000;//主渠道号  1000自己内部数据，1001 360渠道号 1002 阿里渠道号
	public const string ClientVersion = "1.2.0209";
    public const string datakey = "bloodgod20160516";
    public const byte isEFS = 0;//是否加密处理
	public const int MAX_PACKET_SIZE									= 1024*640;

	public const bool isConectSocket = true;  //否联网版本

    public const bool isLogMsg = true;  // 是否输出消息收发
    public const bool isLogMsgDetail = false;  // 是否输出消息详情
    public const bool isLogWalkMsg = false;  // 是否输出行走消息

    public static bool filterWalkMsg ( uint msgId )
    {
        return !( msgId == 8449 || msgId == 261 || msgId == 260 || msgId == 259) || isLogWalkMsg;
    }
}

