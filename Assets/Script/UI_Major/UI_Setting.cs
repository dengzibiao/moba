using UnityEngine;
using System.Collections.Generic;
using System;
using Tianyu;

public enum RefHeroListType
{
    HeroBtnOpen,//英雄按钮请求
    EquipHeroListOpen,//装备面板请求
    embattle,//布阵按钮请求

}
public class UI_Setting : GUIBase
{
    private static UI_Setting instance;
    public static UI_Setting GetInstance()
    {
        return instance;
    }
    public GUISingleButton shrinkBtn;//展开按钮
    public GUISingleButton shrinkTopBtn;//顶端展开按钮
    public GUISingleButton shopBtn;//商店
    public GUISingleButton bagBtn;//背包
    public GUISingleButton heroBtn;//英雄
    public GUISingleButton altarBtn;//祭坛
    public GUISingleButton ectypeBtn;//冒险按钮(副本）
    public GUISingleButton taskBtn;//活动按钮
    public GUISingleButton eventDungBtn;
    public GUISingleSprite[] redPoint;
    public GUISingleButton enchantBtn;//试炼按钮
    public GUISingleButton rankListBtn;
    public GUISingleButton friendBtn;//好友
    public GUISingleButton embattle;//布阵按钮
    public GUISingleButton arenaABtn;//对战按钮
    public GUISingleButton welfareBtn;//福利
    public GUISingleButton mailBtn;//邮箱
    public GUISingleButton societyBtn;//公会
    public GUISingleButton shoucangBtn;//收藏
    public GUISingleButton systemBtn;//系统设置展开
    public GUISingleButton yinyueSwitch;//音乐
    public GUISingleButton yinxiaoSwitch;//音效
    public GUISingleButton EquipBtn;
    public GUISingleProgressBar expBar;
    private Dictionary<int, List<int>> redp;
    public UISprite mark;//设置按钮中遮罩
    private List<UITweener> tweens = new List<UITweener>();

    private List<UITweener> tweensTop = new List<UITweener>();
    public Transform select;
    public GUISingleButton[] buttonarr = new GUISingleButton[9];
    int selectindex = 0;

    int Upgradelvl = 0;
    bool isShrinkSystemBtn = false;
    public bool isShowHeroBtn = false;
    int id;
    TweenPosition sysTemTweenP;
    TweenAlpha sysTemTweenA;
    UISprite Shrink;
    public RefHeroListType heroListreqType;
    protected override void Init()
    {
        instance = this;
        go = GameObject.Find("UI Root").gameObject;
        tweens.AddRange(bagBtn.GetComponents<UITweener>());
        tweens.AddRange(heroBtn.GetComponents<UITweener>());
        tweens.AddRange(altarBtn.GetComponents<UITweener>());
        tweens.AddRange(ectypeBtn.GetComponents<UITweener>());
        tweens.AddRange(embattle.GetComponents<UITweener>());
        tweens.AddRange(shopBtn.GetComponents<UITweener>());
        tweens.AddRange(enchantBtn.GetComponents<UITweener>());
        tweens.AddRange(rankListBtn.GetComponents<UITweener>());
        tweens.AddRange(arenaABtn.GetComponents<UITweener>());
        tweens.AddRange(systemBtn.GetComponents<UITweener>());
        tweens.AddRange(societyBtn.GetComponents<UITweener>());
        tweens.AddRange(shoucangBtn.GetComponents<UITweener>());
        if (EquipBtn != null)
            tweens.AddRange(EquipBtn.GetComponents<UITweener>());
        //顶层按钮
        tweensTop.AddRange(taskBtn.GetComponents<UITweener>());
        tweensTop.AddRange(friendBtn.GetComponents<UITweener>());
        tweensTop.AddRange(welfareBtn.GetComponents<UITweener>());
        tweensTop.AddRange(mailBtn.GetComponents<UITweener>());

        sysTemTweenP = transform.Find("SystemSetting").GetComponent<TweenPosition>();
        sysTemTweenA = transform.Find("SystemSetting").GetComponent<TweenAlpha>();
        Shrink = transform.Find("ShrinkTopBtn").GetComponent<UISprite>();
        mark = transform.Find("SystemSetting/Mark").GetComponent<UISprite>();
        yinyueSwitch = transform.Find("SystemSetting/YinyueSwitch").GetComponent<GUISingleButton>();
        yinxiaoSwitch = transform.Find("SystemSetting/YinxiaoSwitch").GetComponent<GUISingleButton>();

        shrinkBtn.GetComponentInChildren<UIPlaySound>();
        isShrink = false;
        shrinkBtn.onClick = OnShrinkClick;
        embattle.onClick = OnEmbattle;
        bagBtn.onClick = OnBagClick;
        heroBtn.onClick = OnHeroBtnClick;
        taskBtn.onClick = OnTaskBtn;
        altarBtn.onClick = OnAltarClick;
        //expBar.state = ProgressState.STRING;
        //expBar.InValue(800f, 1000f);
        //expBar.InValue(int.Parse(playerData.GetInstance().selfData.exprience.ToString()), int.Parse(playerData.GetInstance().selfData.maxExprience.ToString()));
        //expBar.onChange = OnExpChange;
        shopBtn.onClick = OnShopBtnClick;
        ectypeBtn.onClick = OnEctypeClick;
        enchantBtn.onClick = OnEnchantBtnClick;
        rankListBtn.onClick = OnRankListClick;
        friendBtn.onClick = OnFriendClick;
        arenaABtn.onClick = OnArenaABtnClick;
        welfareBtn.onClick = OnWelfareBtnClick;
        mailBtn.onClick = OnMailOnClick;
        societyBtn.onClick = OnSocietyClick;
        systemBtn.onClick = OnSysTemClick;
        shoucangBtn.onClick = OnShouCangClick;
        yinyueSwitch.onClick = YinyueSwitchOnClick;
        yinxiaoSwitch.onClick = YinxiaoSwitchOnClick;
        shrinkTopBtn.onClick = OnShrinkTopClick;
        if (EquipBtn != null)
            EquipBtn.onClick = OnEquipBtn;

        sysTemTweenP.gameObject.SetActive(false);
        isShrinkTop = false;
        Singleton<RedPointManager>.Instance.NotifyRedChangeEvent += SetMainSettingRed;
    }
    public override UIPanleID GetUIKey()
    {
        return UIPanleID.UISetting;
    }
    private void YinxiaoSwitchOnClick()
    {
        bool p = serverMgr.GetInstance().GetGameSoundEffect();
        p = !p;
        if (p) yinxiaoSwitch.spriteName = "yingxiaoguan";
        else
        {
            yinxiaoSwitch.spriteName = "yingxiao";
        }
        AudioController.Instance.SoundMute(p);
        serverMgr.GetInstance().SetGameSoundEffect(p);
        serverMgr.GetInstance().saveData();

    }

    private void YinyueSwitchOnClick()
    {
        bool p = serverMgr.GetInstance().GetGameMusic();
        p = !p;
        if (p) yinyueSwitch.spriteName = "yinyueguan";
        else
        {
            yinyueSwitch.spriteName = "yinyue";
        }
        AudioController.Instance.Mute(p);
        serverMgr.GetInstance().SetGameMusic(p);
        serverMgr.GetInstance().saveData();
    }

    private void OnShouCangClick()
    {
        if (TaskAutoTraceManager._instance.isTaskAutoPathfinding)
        {
            StopTaskAutoFindWay();
        }
        if (DataDefine.isSkipFunction || FunctionOpenMng.GetInstance().GetFunctionOpen(39))
        {
            Debug.Log("打开收藏界面");
            //UIMountAndPet.Instance.SetShowType(MountAndPet.Mount, EntranceType.Main);
            //Control.ShowGUI(GameLibrary.UIMountAndPet);
            Control.ShowGUI(UIPanleID.UIMountAndPet, EnumOpenUIType.OpenNewCloseOld,false, MountAndPet.Mount);
            Control.PlayBgmWithUI(UIPanleID.UIMountAndPet);
        }
        else
        {
            string text = FSDataNodeTable<UnLockFunctionNode>.GetSingleton().DataNodeList[39].limit_tip;
            //UIPromptBox.Instance.ShowLabel(text);
            Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, text);
        }


    }
    /// <summary>
    /// 打开界面 如果在任务自动寻路就暂停自动寻路
    /// </summary>
    /// <returns></returns>
    private void StopTaskAutoFindWay()
    {

        TaskManager.Single().isTaskAutoTraceToTransfer = false;
        //Selfplayer.GetComponent<NavMeshAgent>().enabled = Navmeshenable;
        TaskAutoTraceManager._instance.isTaskOperation = false;
        CharacterManager.instance.SwitchAutoMode(false);
        TaskAutoTraceManager._instance.isTaskAutoPathfinding = false;
        TaskAutoTraceManager._instance.PlayerAutoPathfindingStop();
        UITaskEffectPanel.instance.SetZDXLEffect(false);
    }
    private void OnSocietyClick()
    {
        if (TaskAutoTraceManager._instance.isTaskAutoPathfinding)
        {
            StopTaskAutoFindWay();
        }
        ////公会功能解锁
        if (FunctionOpenMng.GetInstance().GetFunctionOpen(20))
        {
            if (SocietyManager.Single().isJoinSociety)
                //Control.ShowGUI(GameLibrary.UIHaveJoinSocietyPanel);
                Control.ShowGUI(UIPanleID.UIHaveJoinSocietyPanel, EnumOpenUIType.OpenNewCloseOld);
            else
                //Control.ShowGUI(GameLibrary.UINotJoinSocietyPanel);
                Control.ShowGUI(UIPanleID.UINotJoinSocietyPanel, EnumOpenUIType.OpenNewCloseOld);
        }
        else
        {
            string text = FSDataNodeTable<UnLockFunctionNode>.GetSingleton().DataNodeList[20].limit_tip;
            //UIPromptBox.Instance.ShowLabel(text);
            Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, text);
        }

    }
    private void OnSysTemClick()
    {
        isShrinkSystemBtn = !isShrinkSystemBtn;
        sysTemTweenP.Play(isShrinkSystemBtn);
        sysTemTweenA.Play(isShrinkSystemBtn);
        sysTemTweenP.gameObject.SetActive(true);
        bool p = serverMgr.GetInstance().GetGameMusic();
        if (p) yinyueSwitch.spriteName = "yinyueguan";
        else
        {
            yinyueSwitch.spriteName = "yinyue";
        }
        bool s = serverMgr.GetInstance().GetGameSoundEffect();
        if (s) yinxiaoSwitch.spriteName = "yingxiaoguan";
        else
        {
            yinxiaoSwitch.spriteName = "yingxiao";
        }
        //mark.gameObject.SetActive(true);
    }

    //private void ShowE()
    //{
    //    sysTemTweenP.gameObject.SetActive(false);
    //}
    //public bool b;

    //public int RefreshExpBar(int addExp)
    //{
    //    Upgradelvl = 0;
    //    playerData.GetInstance().selfData.exprience += addExp;

    //    PlayerUpgrade();

    //    expBar.InValue(int.Parse(playerData.GetInstance().selfData.exprience.ToString()), int.Parse(playerData.GetInstance().selfData.maxExprience.ToString()));
    //    expBar.onChange = OnExpChange;

    //    if (Upgradelvl > 0)
    //    {
    //        playerData.GetInstance().ChangeActionPointCeilingHandler();
    //    }

    //    return Upgradelvl;
    //}

    //void PlayerUpgrade()
    //{
    //    if (playerData.GetInstance().selfData.exprience >= playerData.GetInstance().selfData.maxExprience)
    //    {
    //        playerData.GetInstance().selfData.exprience -= playerData.GetInstance().selfData.maxExprience;
    //        playerData.GetInstance().selfData.level++;
    //        Upgradelvl++;
    //        playerData.GetInstance().selfData.maxExprience = FSDataNodeTable<PlayerLevelUpNode>.GetSingleton().FindDataByType(playerData.GetInstance().selfData.level).exp;
    //        if (playerData.GetInstance().selfData.exprience < playerData.GetInstance().selfData.maxExprience)
    //            return;
    //    }
    //    if (playerData.GetInstance().selfData.exprience < playerData.GetInstance().selfData.maxExprience)
    //        return;
    //    PlayerUpgrade();
    //}
    public void OnEquipBtn()
    {
        if (TaskAutoTraceManager._instance.isTaskAutoPathfinding)
        {
            StopTaskAutoFindWay();
        }
        if (!DataDefine.isSkipFunction && !FunctionOpenMng.GetInstance().GetFunctionOpen(14))
        {
            string text = FSDataNodeTable<UnLockFunctionNode>.GetSingleton().DataNodeList[14].limit_tip;
            //UIPromptBox.Instance.ShowLabel(text);
            Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, text);
            return;
        }
        //ClientSendDataMgr.GetSingle().GetHeroSend().SendGetHeroInfo(Globe.fightHero[0], C2SMessageType.ActiveWait);                  //获取英雄信息
        //playerData.GetInstance().isEquipDevelop = true;
        Control.ShowGUI(UIPanleID.EquipDevelop, EnumOpenUIType.OpenNewCloseOld);
    }

    private void OnMailOnClick()
    {
        if (TaskAutoTraceManager._instance.isTaskAutoPathfinding)
        {
            StopTaskAutoFindWay();
        }
        Control.ShowGUI(UIPanleID.UIMailPanel, EnumOpenUIType.OpenNewCloseOld);
        //Control.ShowGUI(GameLibrary.UIMailPanel);
        //GetComponent<Camera>().renderingPath = RenderingPath.Forward;
        //  ClientSendDataMgr.GetSingle().GetBattleSend().SendApplicationFightReq(1);

        // ClientSendDataMgr.GetSingle().GetHeroSend().SendGetHeroInfo(Globe.fightHero[0], C2SMessageType.ActiveWait);                  //获取英雄信息
        //  playerData.GetInstance().isEquipDevelop = true;
    }
    private void OnWelfareBtnClick()
    {
        if (TaskAutoTraceManager._instance.isTaskAutoPathfinding)
        {
            StopTaskAutoFindWay();
        }
        Control.ShowGUI(UIPanleID.UIWelfare, EnumOpenUIType.OpenNewCloseOld);
    }

    //布阵
    private void OnEmbattle()
    {
        // if (!GameLibrary.heroListReq)
        //  {
        //    heroListreqType = RefHeroListType.embattle;
        //    ClientSendDataMgr.GetSingle().GetHeroSend().SendGetHero(C2SMessageType.Active);//获取英雄列表延迟发送
        // }
        // else
        {
            EmbattleRef();
        }
    }
    public void EmbattleRef()
    {
        if (TaskAutoTraceManager._instance.isTaskAutoPathfinding)
        {
            StopTaskAutoFindWay();
        }
        Control.ShowGUI(UIPanleID.UIEmbattle, EnumOpenUIType.OpenNewCloseOld, false, OpenSourceType.City);
        //UIEmbattle.sourceType = OpenSourceType.City;
        //Control.ShowGUI(GameLibrary.UI_Embattle, false);
    }

    public void nextUp()
    {
        if (!select.gameObject.activeSelf)
        {
            select.gameObject.SetActive(true);
        }

        selectindex++;
        if (selectindex > 4)
            selectindex = 0;
        select.parent = buttonarr[selectindex].transform;
    }

    /// <summary>
    /// 商店按钮
    /// </summary>
    public void OnShopBtnClick()
    {
        if (TaskAutoTraceManager._instance.isTaskAutoPathfinding)
        {
            StopTaskAutoFindWay();
        }
        ////商店解锁
        if (DataDefine.isSkipFunction || FunctionOpenMng.GetInstance().GetFunctionOpen(22))
        {
           // UIShop.Instance.IsShop(0);
           // Control.ShowGUI(GameLibrary.UIShop);
            object[]obj=new object[2]{0,0};
            Control.ShowGUI(UIPanleID.UIShopPanel,EnumOpenUIType.OpenNewCloseOld,false,obj);
            Control.PlayBgmWithUI(UIPanleID.UIShopPanel);
            //ClientSendDataMgr.GetSingle().GetCShopSend().RequestGoodsList((int)ShopType.Prop);
        }
        else
        {
            string text = FSDataNodeTable<UnLockFunctionNode>.GetSingleton().DataNodeList[22].limit_tip;
            //UIPromptBox.Instance.ShowLabel(text);
            Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, text);
        }
    }
    /// <summary>
    /// 抽奖
    /// </summary>
    public void OnAltarClick()
    {
        if (TaskAutoTraceManager._instance.isTaskAutoPathfinding)
        {
            StopTaskAutoFindWay();
        }
        ////祭坛抽奖开放条件，金币抽奖开启
        if (DataDefine.isSkipFunction || FunctionOpenMng.GetInstance().GetFunctionOpen(33))
        {
            Control.ShowGUI(UIPanleID.UILottery, EnumOpenUIType.OpenNewCloseOld);
           // ClientSendDataMgr.GetSingle().GetLotterHotSend().LotteryHotRequest();
            // 
        }
        else
        {
            string text = FSDataNodeTable<UnLockFunctionNode>.GetSingleton().DataNodeList[33].limit_tip;
            //UIPromptBox.Instance.ShowLabel(text);
            Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, text);
        }

    }
    private GameObject go;
    // Use this for initialization

    private void OnTaskBtn()
    {
        if (TaskAutoTraceManager._instance.isTaskAutoPathfinding)
        {
            StopTaskAutoFindWay();
        }
        ////日常任务开启时，活动按钮才开启
        if (DataDefine.isSkipFunction || FunctionOpenMng.GetInstance().GetFunctionOpen(37))
        {
            print("任务");
            Control.ShowGUI(UIPanleID.UIActivities, EnumOpenUIType.OpenNewCloseOld);
            // UIPromptBox.Instance.ShowLabel("暂未开启");
            //测试动态加载
            //Control.ShowGUI(GameLibrary.UIActivities);
        }
        else
        {
            string text = FSDataNodeTable<UnLockFunctionNode>.GetSingleton().DataNodeList[37].limit_tip;
            //UIPromptBox.Instance.ShowLabel(text);
            Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, text);
        }


    }

    /// <summary>
    /// 副本按钮
    /// </summary>
    private void OnEctypeClick()
    {
        if (TaskAutoTraceManager._instance.isTaskAutoPathfinding)
        {
            StopTaskAutoFindWay();
        }
        ////冒险按钮开启条件为第一章副本解锁
        if (DataDefine.isSkipFunction || FunctionOpenMng.GetInstance().GetFunctionOpen(1))
        {
            Control.ShowGUI(UIPanleID.UILevel, EnumOpenUIType.OpenNewCloseOld, false, OpenLevelType.NormalOpen);     
            //UILevel.instance.openType = OpenLevelType.NormalOpen;
            //if (null == playerData.GetInstance().worldMap || playerData.GetInstance().worldMap.Count <= 0)
            //{
            //    ClientSendDataMgr.GetSingle().GetBattleSend().SendQueryWorldMap();//获取世界副本
            //}
            //else
            //{
            //    Control.ShowGUI(GameLibrary.UILevel);
            //}
        }
        else
        {
            string text = FSDataNodeTable<UnLockFunctionNode>.GetSingleton().DataNodeList[1].limit_tip;
            //UIPromptBox.Instance.ShowLabel(text);
            Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, text);
        }

    }

    /// <summary>
    /// 
    /// </summary>
    public void OnEventDungBtnClick()
    {

    }

    private void OnExpChange(float percent)
    {

    }

    /// <summary>
    /// 英雄按钮
    /// </summary>
    public void OnHeroBtnClick()
    {
        // if (!GameLibrary.heroListReq)
        //   {
        //     heroListreqType = RefHeroListType.HeroBtnOpen;
        //     ClientSendDataMgr.GetSingle().GetHeroSend().SendGetHero(C2SMessageType.Active);//获取英雄列表延迟发送
        //  }
        //  else
        {
            HeroBtnRefresh();
        }

    }
    public void HeroBtnRefresh()
    {
        if (TaskAutoTraceManager._instance.isTaskAutoPathfinding)
        {
            StopTaskAutoFindWay();
        }
        Control.ShowGUI(UIPanleID.UIHeroList, EnumOpenUIType.OpenNewCloseOld, false);
        //Control.ShowGUI(GameLibrary.UIHeroList);//之前的布阵
        Control.PlayBgmWithUI(UIPanleID.UIHeroList);
        // Control.ShowGUI(GameLibrary.UIHeroShow);
    }

    /// <summary>
    /// 背包按钮
    /// </summary>
    public void OnBagClick()
    {
        if (TaskAutoTraceManager._instance.isTaskAutoPathfinding)
        {
            StopTaskAutoFindWay();
        }
        if (DataDefine.isSkipFunction || FunctionOpenMng.GetInstance().GetFunctionOpen(32))
        {
            //Control.ShowGUI(GameLibrary.UIKnapsack);
            Control.ShowGUI(UIPanleID.UIKnapsack, EnumOpenUIType.OpenNewCloseOld);
        }
        else
        {
            string text = FSDataNodeTable<UnLockFunctionNode>.GetSingleton().DataNodeList[0].limit_tip;
            //UIPromptBox.Instance.ShowLabel(text);
            Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, text);
        }


    }

    public void OnShrinkClick()
    {
        isShrink = !isShrink;
        for (int i = 0; i < tweens.Count; i++)
        {
            tweens[i].Play(isShrink);
        }
        if (isShrink)//true未关闭的的声音
        {
            AudioClip menuClose = Resources.Load(GameLibrary.Resource_UISound + "UI_menu_closed") as AudioClip;
            shrinkBtn.GetComponentInChildren<UIPlaySound>().audioClip = menuClose;
            isShrinkSystemBtn = false;
        }
        else
        {
            AudioClip menuOpen = Resources.Load(GameLibrary.Resource_UISound + "UI_menu_open") as AudioClip;
            shrinkBtn.GetComponentInChildren<UIPlaySound>().audioClip = menuOpen;
        }

        sysTemTweenP.Play(false);
        sysTemTweenA.Play(false);
        UIChat.Instance.SetChatPosition(isShrink);
    }

    public void OnShrinkTopClick()
    {
        isShrinkTop = !isShrinkTop;
        for (int j = 0; j < tweensTop.Count; j++)
        {
            tweensTop[j].Play(isShrinkTop);
        }
        if (isShrinkTop)
        {
            AudioClip menuClose = Resources.Load(GameLibrary.Resource_UISound + "UI_menu_closed") as AudioClip;
            shrinkTopBtn.GetComponentInChildren<UIPlaySound>().audioClip = menuClose;
            Shrink.flip = UISprite.Flip.Nothing;
        }
        else
        {
            AudioClip menuOpen = Resources.Load(GameLibrary.Resource_UISound + "UI_menu_open") as AudioClip;
            shrinkTopBtn.GetComponentInChildren<UIPlaySound>().audioClip = menuOpen;
            Shrink.flip = UISprite.Flip.Horizontally;
        }
    }
    /// <summary>
    /// 试练
    /// </summary>
    public void OnEnchantBtnClick()
    {
        if (TaskAutoTraceManager._instance.isTaskAutoPathfinding)
        {
            StopTaskAutoFindWay();
        }
        ////试炼按钮开启条件为金币副本开启
        if (DataDefine.isSkipFunction || FunctionOpenMng.GetInstance().GetFunctionOpen(13))
        {
            Control.ShowGUI(UIPanleID.UIActivity, EnumOpenUIType.OpenNewCloseOld, false);
            //ClientSendDataMgr.GetSingle().GetBattleSend().SendQueryEventList();//获取活动副本列表
        }
        else
        {
            string text = FSDataNodeTable<UnLockFunctionNode>.GetSingleton().DataNodeList[13].limit_tip;
            //UIPromptBox.Instance.ShowLabel(text);
            Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, text);
        }


    }
    /// <summary>
    /// 排行榜
    /// </summary>
    public void OnRankListClick()
    {
        if (TaskAutoTraceManager._instance.isTaskAutoPathfinding)
        {
            StopTaskAutoFindWay();
        }
        ////排行榜功能开启判断
        if (DataDefine.isSkipFunction || FunctionOpenMng.GetInstance().GetFunctionOpen(38))
        {

            Globe.isOpenSend = true;
            Globe.isOpenLevelSend = true;
           /// UIRankList._instance.IsRank(0);
           // Control.ShowGUI(GameLibrary.UIRankList);
            Control.ShowGUI(UIPanleID.UIRankList, EnumOpenUIType.OpenNewCloseOld,false,0);
            Debug.Log("kkk");
            //ClientSendDataMgr.GetSingle().GetRankListSend().SendRankList(5);
        }
        else
        {
            string text = FSDataNodeTable<UnLockFunctionNode>.GetSingleton().DataNodeList[38].limit_tip;
            //UIPromptBox.Instance.ShowLabel(text);
            Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, text);
        }                 
        // UIPromptBox.Instance.ShowLabel("此功能暂未开启");

        //ClientSendDataMgr.GetSingle().GetRankListSend().SendRankList((int)RankListType.Level,0,15);
        //  UIRankList._instance.tweenLvView();
        //UIRankList._instance.TableBtn[0].transform.GetComponent<UIPlayTween>().Play(true);//暂时注销
    }
    //好友
    public void OnFriendClick()
    {
        if (TaskAutoTraceManager._instance.isTaskAutoPathfinding)
        {
            StopTaskAutoFindWay();
        }
        ////好友功能开启
        if (DataDefine.isSkipFunction || FunctionOpenMng.GetInstance().GetFunctionOpen(12))
        {
            // ClientSendDataMgr.GetSingle().GetFriendSend().FriendsRecommendRequest();
            Control.ShowGUI(UIPanleID.UIFriends, EnumOpenUIType.OpenNewCloseOld);
        }
        else
        {
            string text = FSDataNodeTable<UnLockFunctionNode>.GetSingleton().DataNodeList[12].limit_tip;
            //UIPromptBox.Instance.ShowLabel(text);
            Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, text);
        }

    }
    protected override void ShowHandler()
    {
        RefreshUI();
        SetMainSettingRed(Singleton<RedPointManager>.Instance.GetRedList());


    }
    void RefreshUI()
    {
        GuideManager.Single().InitData();
        //商店功能开放  看看是关闭按钮还是加锁，还是加锁靠谱些 但没有资源
        // shopBtn.gameObject.SetActive(FunctionOpenMng.GetInstance().GetValu(22));

    }


    public void OnArenaABtnClick()
    {
        if (TaskAutoTraceManager._instance.isTaskAutoPathfinding)
        {
            StopTaskAutoFindWay();
        }
        //对战按钮开启条件是角斗场或竞技场之一开启就开启
        if (DataDefine.isSkipFunction || DataDefine.isSkipFunction || FunctionOpenMng.GetInstance().GetFunctionOpen(8) || FunctionOpenMng.GetInstance().GetFunctionOpen(17))
        {
            Control.ShowGUI(UIPanleID.UIPvP, EnumOpenUIType.OpenNewCloseOld, false);
            //Control.ShowGUI(GameLibrary.UIPvP);
            //Control.PlayBgmWithUI(GameLibrary.UIPvP);
        }
        else
        {
            string text = FSDataNodeTable<UnLockFunctionNode>.GetSingleton().DataNodeList[17].limit_tip;
            //UIPromptBox.Instance.ShowLabel(text);
            Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, text);
        }
    }

    public bool isShrink { set; get; }
    private int _index = 1;

    public bool isShrinkTop { set; get; }


    protected override void RegisterComponent()
    {
        base.RegisterComponent();
        RegisterComponentID(9, 19, ectypeBtn.gameObject);
        RegisterComponentID(12, 19, altarBtn.gameObject);
        RegisterComponentID(30, 19, shopBtn.gameObject);
        RegisterComponentID(23, 19, arenaABtn.gameObject);
        // RegisterComponentID(27, 19, arenaABtn.gameObject);
        RegisterComponentID(29, 19, enchantBtn.gameObject);
        RegisterComponentID(32, 19, EquipBtn.gameObject);
        RegisterComponentID(14, 19, heroBtn.gameObject);

        RegisterComponentID(41, 19, embattle.gameObject);
    }
    protected override void RegisterIsOver()
    {
        base.RegisterIsOver();
    }

    /// <summary>
    /// 处理主界面的红点显示隐藏
    /// </summary>
    /// <param name="arr"></param>
    public void SetMainSettingRed(Dictionary<int, List<int>> arr)
    {

            for (int i = 0; i < redPoint.Length; i++)
            {
                if (arr.ContainsKey(i + 1))
                {
                    redPoint[i].gameObject.SetActive(true);
                }
                else
                {
                    redPoint[i].gameObject.SetActive(false);
                }
            }
    }
}
