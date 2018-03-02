using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Tianyu;
/// <summary>
///福利--功能 
/// </summary>
public class UIWelfare : GUIBase
{
    private Transform getEnergyPanel;//获取体力
    private Transform uiSign_in;//每日签到
    private Transform upgradegiftbag;//等级礼包面板
    private Transform onlinegiftbag;//在线礼包面板
    private Transform Sign_inScrollView;//在线礼包面板
    private Transform newPlayerRewards;//在线礼包面板
    private Transform activationCode;//激活码
    public GUISingleLabel titleLab;
    public GUISingleCheckBoxGroup checkBoxs;
    private int _index = 0;
    private long id = -1;//存储英雄ID
    public GUISingleButton backBtn;
    private Color c;
    public GUISingleSprite redPoint_2;//红点显示
    public GUISingleSprite redPoint_1;//红点显示
    public GUISingleSprite redPoint_3;//升级大礼红点
    public GUISingleSprite redPoint_4;//在线奖励红点
    public GUISingleSprite redPoint_5;
    private GameObject[] obj = new GameObject[6];
    DateTime time;

    long times;
    long timer;
    string a;
    long aa;
    string Date;
    public Dictionary<long, MealAttrNode> nodeDic;
    public static UIWelfare _instance;
    public List<ItemData> ItemDatas = new List<ItemData>();
    public bool isShoucang = false;//是从收藏界面 进入的福利界面
    public int openSelectIndex = -1;//从收藏界面进入要打开的相应页签
    public UIWelfare()
    {
        _instance = this;
    }

    public override UIPanleID GetUIKey()
    {
        return UIPanleID.UIWelfare;
    }
    private float onlinetimer = 0;
    protected override void Init()
    {
        base.Init();
        _instance = this;
        backBtn.onClick = OnBackBtn;
        checkBoxs.onClick = OnCheckBoxClick;
        uiSign_in = transform.Find("UISign_in");
        getEnergyPanel = transform.Find("GetEnergyPanel").GetComponent<Transform>();
        upgradegiftbag = transform.Find("PlayerUpgradegiftbag");
        onlinegiftbag = transform.Find("PlayerOnlinegiftbag");
        newPlayerRewards = transform.Find("NewPlayerRewards");
        activationCode = transform.Find("ActivationCode");
        redPoint_1 = transform.Find("RedPoint_1").GetComponent<GUISingleSprite>();
        redPoint_2 = transform.Find("RedPoint_2").GetComponent<GUISingleSprite>();
        redPoint_3 = transform.Find("RedPoint_3").GetComponent<GUISingleSprite>();
        redPoint_4 = transform.Find("RedPoint_4").GetComponent<GUISingleSprite>();
        redPoint_5 = transform.FindChild("RedPoint_5").GetComponent<GUISingleSprite>();
        time = TimeManager.Instance.ConvertIntDateTime(Auxiliary.GetNowTime());
        obj[0] = uiSign_in.gameObject;
        obj[1] = getEnergyPanel.gameObject;
        obj[2] = upgradegiftbag.gameObject;
        obj[3] = onlinegiftbag.gameObject;
        obj[4] = newPlayerRewards.gameObject;
        obj[5] = activationCode.gameObject;
        //ClientSendDataMgr.GetSingle().GetEnergySend().SendGetEnergy();

        if (FSDataNodeTable<MealAttrNode>.GetSingleton().DataNodeList.Count > 0)
        {

            nodeDic = FSDataNodeTable<MealAttrNode>.GetSingleton().DataNodeList;
            foreach (var mealAttr in nodeDic)
            {
                if (mealAttr.Value.releassed == 1)
                {

                    playerData.GetInstance().getEnergyData.mealList.Add(mealAttr.Value);
                }
            }
        }
        InitGiveBackData();

    }
    private void ShowRedPoint(Dictionary<int, List<int>> redlist)
    {
        redPoint_1.ShowOrHide(Singleton<RedPointManager>.Instance.GetChildList(EnumRedPoint.RP_WELFARE, 1));
        redPoint_2.ShowOrHide(Singleton<RedPointManager>.Instance.GetChildList(EnumRedPoint.RP_WELFARE, 2));
        redPoint_3.ShowOrHide(Singleton<RedPointManager>.Instance.GetChildList(EnumRedPoint.RP_WELFARE, 3));
        redPoint_4.ShowOrHide(Singleton<RedPointManager>.Instance.GetChildList(EnumRedPoint.RP_WELFARE, 4));
        redPoint_5.ShowOrHide(Singleton<RedPointManager>.Instance.GetChildList(EnumRedPoint.RP_WELFARE, 5));
    }
    /// <summary>
    /// 返回按钮事件
    /// </summary>
    private void OnBackBtn()
    {
        _index = 0;
        ScrollViewResetPosition();
        Hide();
        Control.HideGUI();
        HeroPosEmbattle.instance.HideModel();
        //if (isShoucang)
        //{
        //    Control.ShowGUI(UIPanleID.UIMountAndPet);
        //    isShoucang = false;
        //}

    }
    /// <summary>
    /// 临时存储领取奖励的英雄ID
    /// </summary>
    /// <param name="id"></param>
    public void HeroID(long id)
    {
        this.id = id;
    }

    protected override void SetUI(params object[] uiParams)
    {
        if(uiParams!=null && uiParams.Length>0)
         _index = int.Parse(uiParams[0].ToString());
        base.SetUI(uiParams);
    }

    protected override void OnLoadData()
    {
        Singleton<Notification>.Instance.RegistMessageID(MessageID.common_newbie_reward_ret, this.GetUIKey());
        Singleton<Notification>.Instance.RegistMessageID(MessageID.common_everyday_sign_list_ret, this.GetUIKey());
        Singleton<Notification>.Instance.RegistMessageID(MessageID.common_player_level_reward_ret, this.GetUIKey());
        Singleton<Notification>.Instance.RegistMessageID(MessageID.common_timeing_dining_ret, this.GetUIKey());
        Singleton<Notification>.Instance.RegistMessageID(MessageID.common_draw_online_reward_ret, this.GetUIKey());       
        ClientSendDataMgr.GetSingle().GetUISign_inSend().SendGetUISign_inList(C2SMessageType.Active, 2);//发送每日签到列表  
        base.OnLoadData();
    }

    public override void ReceiveData(uint messageID)
    {
        switch (messageID)
        {
            case MessageID.common_timeing_dining_ret: Show(); break;
            case MessageID.common_player_level_reward_ret: Show(); break;
            case MessageID.common_everyday_sign_list_ret: Show();break;
            case MessageID.common_draw_online_reward_ret:break;
            case MessageID.common_newbie_reward_ret:
                if (this.id != -1)
                {
                    if (playerData.GetInstance().AddHeroToList(id))
                    {
                        object[] ob = new object[4] { id, ShowHeroEffectType.NewPlayerRewards, HeroOrSoul.Hero, 0 };
                        Control.ShowGUI(UIPanleID.UILottryHeroEffect, EnumOpenUIType.DefaultUIOrSecond, false, ob);
                        id = -1;
                    }
                }
                Show();
                break;
        }
        base.ReceiveData(messageID);
    }

    protected override void ShowHandler()
    {
        for (int i = 0; i < obj.Length; i++)
        {
            if (i != _index)
            {
                obj[i].SetActive(false);
            }
        }
        if (!obj[_index].activeInHierarchy)
        {
            obj[_index].SetActive(true);
        }
        checkBoxs.setMaskState(_index);
        if (_index != 0)
        {
            HeroPosEmbattle.instance.HideModel(PosType.uisign);
        }
        ShowEnrgyData();
        ShowRedTag();
        Rfresh();
        SetTitleString(_index);
        //ShowRedPoint(Singleton<RedPointManager>.Instance.GetRedList());
    }
    private void ShowEnrgyData()
    {
        Date = TimeManager.Instance.ConvertIntDateTime(Auxiliary.GetNowTime()).ToString("yyyyMMdd");
        times = long.Parse(TimeManager.Instance.ConvertIntDateTime(Auxiliary.GetNowTime()).ToString("HH"));
        timer = long.Parse(TimeManager.Instance.ConvertIntDateTime(Auxiliary.GetNowTime()).ToString("HHmmss"));
        a = playerData.GetInstance().singnData.dinning;

        if ((times >= playerData.GetInstance().getEnergyData.mealList[0].startTime && timer <= 10000 * playerData.GetInstance().getEnergyData.mealList[0].endTime))//12,14
        {
            int type = playerData.GetInstance().getEnergyData.mealList[0].id > 3 ? playerData.GetInstance().getEnergyData.mealList[0].id - 3 : playerData.GetInstance().getEnergyData.mealList[0].id;
            aa = long.Parse(Date + type);


            if (aa > int.Parse(a))//现在的大于上次的
            {
                redPoint_2.gameObject.SetActive(true);//能吃的情况
                OperateRedFlag(2, true);
                UIGetEnergyPanel._instance.InitData(true, "我做的烧鸡可是大补哦，快来吃吧！吃烧鸡可以补充体力哦！", true, true);
            }
            else
            {
                //写吃过的情况
                redPoint_2.gameObject.SetActive(false);
                OperateRedFlag(2, false);
                UIGetEnergyPanel._instance.InitData(false, "烹饪中~", false, false);
            }

        }
        else if ((times >= playerData.GetInstance().getEnergyData.mealList[1].startTime && timer <= 10000 * playerData.GetInstance().getEnergyData.mealList[1].endTime))
        {
            int type = playerData.GetInstance().getEnergyData.mealList[1].id > 3 ? playerData.GetInstance().getEnergyData.mealList[1].id - 3 : playerData.GetInstance().getEnergyData.mealList[1].id;
            long aa = long.Parse(Date + type);
            if (aa > int.Parse(a))
            {
                redPoint_2.gameObject.SetActive(true);//能吃的情况
                OperateRedFlag(2, true);
                UIGetEnergyPanel._instance.InitData(true, "我做的烧鸡可是大补哦，快来吃吧！吃烧鸡可以补充体力哦！", true, true);
            }
            else
            {
                //写吃过的情况
                redPoint_2.gameObject.SetActive(false);//能吃的情况
                OperateRedFlag(2, false);
                UIGetEnergyPanel._instance.InitData(false, "烹饪中~", false, false);
            }

        }
        else if ((times >= playerData.GetInstance().getEnergyData.mealList[2].startTime && timer <= 10000 * playerData.GetInstance().getEnergyData.mealList[2].endTime))//21,23
        {
            int type = playerData.GetInstance().getEnergyData.mealList[2].id > 3 ? playerData.GetInstance().getEnergyData.mealList[2].id - 3 : playerData.GetInstance().getEnergyData.mealList[2].id;
            long aa = long.Parse(Date + type);

            if (aa > int.Parse(a))
            {
                redPoint_2.gameObject.SetActive(true);//能吃的情况
                OperateRedFlag(2, true);
                UIGetEnergyPanel._instance.InitData(true, "我做的烧鸡可是大补哦，快来吃吧！吃烧鸡可以补充体力哦！", true, true);
            }
            else
            {
                //写吃过的情况
                redPoint_2.gameObject.SetActive(false);
                OperateRedFlag(2, false);
                UIGetEnergyPanel._instance.InitData(false, "烹饪中~", false, false);
            }
        }
        else
        {
            redPoint_2.gameObject.SetActive(false);
            OperateRedFlag(2, false);
            UIGetEnergyPanel._instance.InitData(false, "烹饪中~", false, false);
        }
    }
    private void OperateRedFlag(int index, bool isAdd)
    {
        if (index == 1)
        {
            if (isAdd)
            {
                Singleton<RedPointManager>.Instance.AddChildFlag(EnumRedPoint.RP_WELFARE, 1);
            }
            else
            {
                Singleton<RedPointManager>.Instance.DeletChildFlag(EnumRedPoint.RP_WELFARE, 1);
            }
        }
        if (index == 2)
        {
            if (isAdd)
            {
                Singleton<RedPointManager>.Instance.AddChildFlag(EnumRedPoint.RP_WELFARE, 2);
            }
            else
            {
                Singleton<RedPointManager>.Instance.DeletChildFlag(EnumRedPoint.RP_WELFARE, 2);
            }
        }
        if (index == 3)
        {
            if (isAdd)
            {
                Singleton<RedPointManager>.Instance.AddChildFlag(EnumRedPoint.RP_WELFARE, 3);
            }
            else
            {
                Singleton<RedPointManager>.Instance.DeletChildFlag(EnumRedPoint.RP_WELFARE, 3);
            }
        }
        if (index == 4)
        {
            if (isAdd)
            {
                Singleton<RedPointManager>.Instance.AddChildFlag(EnumRedPoint.RP_WELFARE, 4);
            }
            else
            {
                Singleton<RedPointManager>.Instance.DeletChildFlag(EnumRedPoint.RP_WELFARE, 4);
            }
        }
        if (index == 5)
        {
            if (isAdd)
            {
                Singleton<RedPointManager>.Instance.AddChildFlag(EnumRedPoint.RP_WELFARE, 5);
            }
            else
            {
                Singleton<RedPointManager>.Instance.DeletChildFlag(EnumRedPoint.RP_WELFARE, 5);
            }
        }
    }
    void Update()
    {
        ShowEnrgyData();
        onlinetimer += Time.deltaTime;
        if (playerData.GetInstance().singnData.onLineRewardTime > 0)
        {
            if (onlinetimer > 1)
            {
                playerData.GetInstance().singnData.onLineRewardTime--;
                onlinetimer = 0;
            }
        }

    }
    private void OnCheckBoxClick(int index, bool boo)
    {
        if (boo)
        {
            this._index = index;
            ScrollViewResetPosition();
            switch (index)
            {
                case 0:
                    ClientSendDataMgr.GetSingle().GetUISign_inSend().SendGetUISign_inList(C2SMessageType.Active, 2);
                    break;
                case 1:
                    Show();

                    break;
                case 2:
                    Show();
                    break;
                case 3:
                    Show();
                    break;
                case 4:
                    Show();
                    break;
                case 5:
                    Show();
                    break;
                default:
                    break;
            }
        }
    }
    private void SetTitleString(int index)
    {
        switch (index)
        {
            case 0:
                int month = 0;
                month = PropertyManager.ConvertIntDateTime(Auxiliary.GetNowTime()).Month;
                titleLab.text = month + "月签到";
                break;
            case 1:
                titleLab.text = "补充体力";
                break;
            case 2:
                titleLab.text = "升级大礼";
                break;
            case 3:
                titleLab.text = "在线奖励";
                break;
            case 4:
                titleLab.text = "登录礼包";
                break;
            case 5:
                titleLab.text = "激活码";
                break;
            default:
                titleLab.text = "福利";
                break;
        }
    }
    /// <summary>
    /// ScrillView位置还原
    /// </summary>
    private void ScrollViewResetPosition()
    {
        upgradegiftbag.Find("GiftbagScrollView").GetComponent<UIScrollView>().ResetPosition();
        onlinegiftbag.Find("GiftbagScrollView").GetComponent<UIScrollView>().ResetPosition();
        uiSign_in.Find("Sign_inScrollView").GetComponent<UIScrollView>().ResetPosition();
    }
    /// <summary>
    /// 展示在线奖励红点和升级大礼红点
    /// </summary>
    public void ShowRedTag()
    {
        //先计算一下当前等级可领取奖励个数。
        //然后获取已经领取的奖励个数
        //相减得到未领取的个数。未领取的个数大于0 有红点
        int canGetCount = 0;
        int alreadyGetCount = 0;
        int residueGetCount = 0;
        if (FSDataNodeTable<LevelRewardNode>.GetSingleton().DataNodeList.Count > 0)
        {
            foreach (LevelRewardNode node in FSDataNodeTable<LevelRewardNode>.GetSingleton().DataNodeList.Values)
            {
                if (node.need_lv <= playerData.GetInstance().selfData.level)
                {
                    canGetCount++;
                }
            }
        }
        alreadyGetCount = playerData.GetInstance().singnData.alreadylevelRewardDic.Count;
        if (canGetCount >= alreadyGetCount)
        {
            residueGetCount = canGetCount - alreadyGetCount;
        }

        redPoint_3.gameObject.SetActive(residueGetCount > 0 ? true : false);
        OperateRedFlag(3, residueGetCount > 0 ? true : false);

        //if (FSDataNodeTable<OnlineRewardNode>.GetSingleton().DataNodeList.Count > 0)
        //{
        //    foreach (OnlineRewardNode node in FSDataNodeTable<OnlineRewardNode>.GetSingleton().DataNodeList.Values)
        //    {
        //        if ((Auxiliary.GetNowTime() - playerData.GetInstance().singnData.getRewardTime + playerData.GetInstance().singnData.onLineTime * 60) >= node.online_time * 60 && UIOnlineGiftBag.Instance.AlreadyGetCount + 1 == node.id)
        //        {
        //            playerData.GetInstance().singnData.isCanGetOnlineReward = true;
        //        }
        //    }
        //}
        if (playerData.GetInstance().singnData.onlineAlreadyGetCount < FSDataNodeTable<OnlineRewardNode>.GetSingleton().DataNodeList.Count)
        {
            if (playerData.GetInstance().singnData.onLineRewardTime <= 0)
            {
                playerData.GetInstance().singnData.isCanGetOnlineReward = true;
            }
        }
        redPoint_4.gameObject.SetActive(playerData.GetInstance().singnData.isCanGetOnlineReward);
        OperateRedFlag(4, playerData.GetInstance().singnData.isCanGetOnlineReward);
        if (int.Parse(playerData.GetInstance().singnData.Signed.Substring(4, 2)) < time.Day)
        {
            redPoint_1.transform.gameObject.SetActive(true);
            OperateRedFlag(1, true);
        }
        else
        {
            redPoint_1.transform.gameObject.SetActive(false);
            OperateRedFlag(1, false);
        }

    }
    /// <summary>
    /// 初始化回馈奖励数据
    /// </summary>
    private void InitGiveBackData()
    {
        if (FSDataNodeTable<NewPlayerRewardNode>.GetSingleton().DataNodeList.Count > 0)
        {
            playerData.GetInstance().newPlayerRewardList.rewardList.Clear();
            foreach (NewPlayerRewardNode node in FSDataNodeTable<NewPlayerRewardNode>.GetSingleton().DataNodeList.Values)
            {
                ItemData info = new ItemData();
                info.Id = node.id;
                info.goodsItem = node.goodsItem;
                info.Diamond = node.diamond;
                info.Gold = node.gold;
                playerData.GetInstance().newPlayerRewardList.rewardList.Add(info);
            }
        }
        Rfresh();
    }

    private void Rfresh()
    {
        string a = TimeManager.Instance.ConvertIntDateTime(Auxiliary.GetNowTime()).ToString("yyyyMMdd");
        for (int i = 0; i < playerData.GetInstance().newPlayerRewardList.rewardList.Count; i++)
        {

            playerData.GetInstance().newPlayerRewardList.rewardList[i].GameTime = long.Parse(a.Substring(2));
            playerData.GetInstance().newPlayerRewardList.rewardList[i].TimeSign =
                playerData.GetInstance().newPlayerRewardList.timeList[i];
            long aa = long.Parse(playerData.GetInstance().newPlayerRewardList.rewardList[i].GameTime.ToString().Substring(0, 6));//今天日期
            long b = long.Parse(playerData.GetInstance().newPlayerRewardList.timeList[i].ToString().Substring(0, 6));//标记日期
            int c = int.Parse(TimeManager.Instance.ConvertIntDateTime(Auxiliary.GetNowTime()).ToString("HH"));
            int count = int.Parse(playerData.GetInstance().newPlayerRewardList.timeList[i].ToString().Substring(6, 2));//次数

            if (aa == b)//判断是今天
            {
                if (count == 0 && c > 5) //如果是今天注册的默认打开
                {
                    redPoint_5.gameObject.SetActive(true);
                    OperateRedFlag(5, true);
                }
                else
                {
                    redPoint_5.gameObject.SetActive(false);
                    OperateRedFlag(5, false);
                }
            }
            else if (aa > b && count < 15 && c > 5)
            {
                redPoint_5.gameObject.SetActive(true);
                OperateRedFlag(5, true);
            }
        }
    }
    /// <summary>
    /// 临时存储Item列
    /// </summary>
    /// <param name="itemData"></param>
    public void TemporaryData(List<ItemData> itemData)
    {
        ItemDatas.Clear();
        ItemDatas = itemData;
    }/// <summary>

    public void ChangeRedPointState(bool boo)
    {
        redPoint_5.gameObject.SetActive(true);
        OperateRedFlag(5, true);
    }
    public void ExternalOpenWelfare(int type, bool shoucang)
    {
        this._index = type;
        isShoucang = shoucang;

    }

}
