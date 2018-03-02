using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Tianyu;

public class UIAbattiorList : GUIBase
{
    public static UIAbattiorList instance;

    public UIButton backBtn;
    public GUISingleButton rankBtn;
    public GUISingleButton shopBtn;
    //public GUISingleButton logBtn;
    public GUISingleLabel goldLabel;

    public UIButton ReplacetBtn;

    public List<UIPVPPlayer> playerHero = new List<UIPVPPlayer>();
    public UIPVPPlayer selfHero;


    public UILabel WaitTimer;
    public UILabel DareNum;

    public UIAbattiorPro uiAbattiorPro;

    public List<int> roomList = new List<int>();

    string waitMinute;
    string waitSecond;

    [HideInInspector]
    public bool isCanDare = true;

    bool isCD = false;
    DateTime currentDate;
    DateTime nextDate;
    TimeSpan timeSpan;
    bool isRefCDorNum = false;

    public UIAbattiorList()
    {
        instance = this;
    }

    protected override void Init()
    {
        base.Init();

        EventDelegate.Set(backBtn.onClick, OnBackClick);
        EventDelegate.Set(ReplacetBtn.onClick, OnReplacetBtnClick);

        rankBtn.onClick = OnRankBtn;
        shopBtn.onClick = OnShopBtn;
        //logBtn.onClick = OnLogBtn;

        selfHero.isSelf = true;
        //RefreshPlayerInfo();

    }
    public override UIPanleID GetUIKey()
    {
        return UIPanleID.UIAbattiorList;
    }

    protected override void SetUI(params object[] uiParams)
    {
        
        base.SetUI(uiParams);
    }
    protected override void OnLoadData()
    {
        base.OnLoadData();

        Singleton<Notification>.Instance.RegistMessageID(MessageID.pve_arena_reload_cd_ret, this.GetUIKey());
        Singleton<Notification>.Instance.RegistMessageID(MessageID.pve_arena_reload_cd_ret, UIPanleID.UIAbattiorList);

        if (Globe.arenaHero.Count <= 0)
        {
            Singleton<Notification>.Instance.RegistMessageID(MessageID.pve_query_arena_list_ret, this.GetUIKey());
            Singleton<Notification>.Instance.RegistMessageID(MessageID.pve_query_arena_list_ret, UIPanleID.UIAbattiorList);
            Singleton<Notification>.Instance.Send(MessageID.pve_query_arena_list_req, C2SMessageType.ActiveWait);
        }
        else
        {
            if (IsShow())
                RefreshPlayerInfo();
            else
                Show();
        }

    }
    public override void ReceiveData(uint messageID)
    {
        base.ReceiveData(messageID);

        if (messageID == MessageID.pve_query_arena_list_ret)
        {
            if (IsShow())
                RefreshPlayerInfo();
            else
                Show();
        }
        else if (messageID == MessageID.pve_arena_reload_cd_ret)
        {
            ReloadCDResult();
        }
        
    }
    protected override void ShowHandler()
    {
        selfHero.RefreshUI(Globe.defendTeam);
        OnReplacetBtnClick();
        RefreshPVPCoin();
        CheckDareTime();
    }

    void FixedUpdate()
    {
        if (isCD)
        {
            RefreshDareTime();
        }
    }

    public void RefreshPVPCoin()
    {
        goldLabel.text = playerData.GetInstance().baginfo.pvpCoin + "";
    }


    void CheckDareTime()
    {
        string currentymd = TimeManager.Instance.ConvertIntDateTime(Auxiliary.GetNowTime()).ToString("yyMMdd");
        if (TimeManager.Instance.CheckTimeIsNowadays(currentymd, true, true) > 0)
        {
            GameLibrary.ArenaNumber[0] = currentymd;
            GameLibrary.ArenaNumber[1] = "000000";
            GameLibrary.ArenaNumber[2] = "00";
            GameLibrary.ArenaNumber[3] = "5";
        }
        DareNum.text = "今日剩余挑战次数: [FFFFFF]" + GameLibrary.ArenaNumber[3] + "[-]/" + playerData.GetInstance().MaxArenaDare;
        if (TimeManager.Instance.CheckTimeIsNowadays((string)GameLibrary.ArenaNumber[0] + GameLibrary.ArenaNumber[1], false) > 0)
        {
            isCD = true;
            nextDate = DateTime.ParseExact((string)GameLibrary.ArenaNumber[0] + GameLibrary.ArenaNumber[1], "yyMMddHHmmss", new System.Globalization.CultureInfo("zh-CN", true));
            WaitTimer.enabled = true;
        }
    }

    void RefreshDareTime()
    {
        if (TimeManager.Instance.CheckTimeIsNowadays((string)GameLibrary.ArenaNumber[0] + GameLibrary.ArenaNumber[1], false) > 0)
        {
            isCanDare = false;
            currentDate = TimeManager.Instance.ConvertIntDateTime(Auxiliary.GetNowTime());
            timeSpan = nextDate.Subtract(currentDate);
            waitMinute = timeSpan.Minutes > 9 ? "" + timeSpan.Minutes : "0" + timeSpan.Minutes;
            waitSecond = timeSpan.Seconds > 9 ? "" + timeSpan.Seconds : "0" + timeSpan.Seconds;
            WaitTimer.text = "[FF0000]" + waitMinute + ":" + waitSecond + "[-]后可再次进行挑战";
        }
        else
        {
            ClearOnCD();
        }
    }

    //int CheckCDIsCanDare()
    //{
    //    currentDate = TimeManager.Instance.ConvertIntDateTime(Auxiliary.GetNowTime());
    //    nextDate = DateTime.ParseExact(nextTime, "yyMMddHHmmss", new System.Globalization.CultureInfo("zh-CN", true));
    //    return nextDate.CompareTo(currentDate);
    //}

    int diamond;

    public bool CanDare()
    {

        if (Convert.ToInt32(GameLibrary.ArenaNumber[3]) <= 0)
        {
            isRefCDorNum = false;
            //uiAbattiorPro.RefreshPrompt(isRefCDorNum);

            //VIP表
            ResetLaterNode node = FSDataNodeTable<ResetLaterNode>.GetSingleton().FindDataByType(1);
            if (Convert.ToInt32(GameLibrary.ArenaNumber[2]) >= node.buy_abattoir.Length - 1)
                diamond = node.buy_abattoir[node.buy_abattoir.Length - 1];
            else
                diamond = node.buy_abattoir[Convert.ToInt32(GameLibrary.ArenaNumber[2])];

            ShowUIPopUp("获得挑战次数", diamond + "钻石", UIPopupType.EnSure);

            return false;
        }

        if (isCanDare)
        {
            return true;
        }
        else
        {
            isRefCDorNum = true;
            diamond = 50;
            ShowUIPopUp("立即开始挑战", diamond + "钻石", UIPopupType.EnSure);

            //uiAbattiorPro.RefreshPrompt(isRefCDorNum);
            return false;
        }
    }

    void ShowUIPopUp(string str1, string str2, UIPopupType type)
    {
        object[] obj = new object[5] { str1, str2, type, this.gameObject, "OnDefineBtnClick" };
        Control.ShowGUI(UIPanleID.UIPopUp, EnumOpenUIType.DefaultUIOrSecond, false, obj);
    }

    public void OnDefineBtnClick()
    {
        if (playerData.GetInstance().baginfo.diamond >= diamond)
        {
            //if (isRef)
            //{
            //    uiAbattior.ClearOnCD();
            //}
            //else
            //{
            //    uiAbattior.AddDareNum();
            //}
            //ClientSendDataMgr.GetSingle().GetBattleSend().SendArenaReloadCD(isRef ? 1 : 2);

            Dictionary<string, object> newpacket = new Dictionary<string, object>();
            newpacket.Add("arg1", isRefCDorNum ? 1 : 2);
            Singleton<Notification>.Instance.Send(MessageID.pve_arena_reload_cd_req, newpacket, C2SMessageType.ActiveWait);

        }
        else
        {
            //UIBuyEnergyVitality.Instance.SetInfo(ActionPointType.Energy);
            //Control.ShowGUI(GameLibrary.UIBuyEnergyVitality);
            Control.ShowGUI(UIPanleID.UITooltips, EnumOpenUIType.DefaultUIOrSecond, false, "当前钻石不足");
            //gameObject.SetActive(false);
        }
    }

    public void DareLater(string prompt)
    {
        OnReplacetBtnClick();
        //uiAbattiorPro.RefresLaterPrompt(prompt);
        object[] obj = new object[5] { prompt, "", UIPopupType.OnlyShow, this.gameObject, "" };
        Control.ShowGUI(UIPanleID.UIPopUp, EnumOpenUIType.DefaultUIOrSecond, false, obj);
    }
    
    public void ReloadCDResult()
    {
        if (isRefCDorNum)
        {
            ClearOnCD();
        }
        else
        {
            AddDareNum();
        }
    }

    void ClearOnCD()
    {
        isCD = false;
        isCanDare = true;
        WaitTimer.enabled = false;
        GameLibrary.ArenaNumber[1] = "000000";
    }

    void AddDareNum()
    {
        GameLibrary.ArenaNumber[3] = playerData.GetInstance().MaxArenaDare;
        DareNum.text = "今日剩余挑战次数: [FFFFFF]" + GameLibrary.ArenaNumber[3] + "[-]/" + playerData.GetInstance().MaxArenaDare;
    }

    void OnRefreshWaitLabel(float time)
    {

        if (!WaitTimer.enabled)
        {
            isCanDare = false;
            WaitTimer.enabled = true;
        }

        waitMinute = (time / 60) > 9 ? ((int)(time / 60)).ToString("0") : "0" + ((int)(time / 60)).ToString("0");
        waitSecond = (time % 60) > 9 ? (time % 60).ToString("0") : "0" + (time % 60).ToString("0");

        WaitTimer.text = "[2dd740]" + waitMinute + ":" + waitSecond + "[-]后可再次进行挑战";

        if (time <= 0)
        {
            ClearOnCD();
        }

    }

    public void RefreshPlayerInfo()
    {
        if (Globe.arenaHero.Count <= 0 && !Globe.ArenaRkisOne)
        {
            //ClientSendDataMgr.GetSingle().GetBattleSend().SendQueryArenaList(0);
            //Control.ShowGUI(UIPanleID.UIAbattiorList, EnumOpenUIType.OpenNewCloseOld, false, true);
            //Globe.IsRefreshArena = true;
            Singleton<Notification>.Instance.Send(MessageID.pve_query_arena_list_req, C2SMessageType.ActiveWait);
            return;
        }

        if (Globe.ArenaRkisOne)
        {
            for (int i = 0; i < playerHero.Count; i++)
            {
                playerHero[i].RefreshUID(null);
            }
            return;
        }

        List<ArenaHero> hero = Globe.arenaHero[0];
        for (int i = 0; i < playerHero.Count; i++)
        {
            playerHero[i].RefreshUID(i < hero.Count ? hero[i] : null);
        }

        if (Globe.arenaHero.Count > 0)
            Globe.arenaHero.RemoveAt(0);

    }

    void OnReplacetBtnClick()
    {
        //if (!CanDare())
        //{
        //    return;
        //}
        //UnityUtil.SetBtnState(ReplacetBtn.gameObject, false);
        //CDTimer.GetInstance().AddCD(2, (int count, long ci) => { UnityUtil.SetBtnState(ReplacetBtn.gameObject, true); });

        if (Globe.arenaHero.Count <= 0 && !Globe.ArenaRkisOne)
        {
            //Globe.IsRefreshArena = true;
            //ClientSendDataMgr.GetSingle().GetBattleSend().SendQueryArenaList(0);
            //Control.ShowGUI(UIPanleID.UIAbattiorList, EnumOpenUIType.OpenNewCloseOld, false, false);
            Singleton<Notification>.Instance.Send(MessageID.pve_query_arena_list_req, C2SMessageType.ActiveWait);
        }
        else
        {
            RefreshPlayerInfo();
            
        }
    }

    void OnBackClick()
    {
        for (int i = 0; i < playerHero.Count; i++)
        {
            playerHero[i].ClearCallBack();
        }
        selfHero.ClearCallBack();

        //Control.ShowGUI(UIPanleID.UIPvP, EnumOpenUIType.OpenNewCloseOld, false);
        //Control.HideGUI(UIPanleID.UIAbattiorList);

        Control.HideGUI();
    }

    void OnRankBtn()
    {
        //UIRankList._instance.IsRank(1);
        //ClientSendDataMgr.GetSingle().GetRankListSend().SendRankList((int)RankListType.YesterdayRank,0,15);
        //UIPromptBox.Instance.ShowLabel("此功能暂未开启");
        Globe.isOpenSend = true;
        Globe.isOpenLevelSend = true;
        //playerData.GetInstance().playerRankData.rankListType = RankListType.RealTimeRank;
        //UIRankList._instance.IsRank(1);
        Control.ShowGUI(UIPanleID.UIRankList, EnumOpenUIType.OpenNewCloseOld, false, 1);
        //UIRankList._instance.SendInfo(1);
        //Show();
        //Control.ShowGUI(GameLibrary.UIRankList);
    }
    void OnShopBtn()
    {
        //UIShop.Instance.IsShop(1);
        //Control.ShowGUI(GameLibrary.UIShop);
        object[] obj = new object[2] { 0, 1 };
        Control.ShowGUI(UIPanleID.UIShopPanel, EnumOpenUIType.OpenNewCloseOld, false, obj);
    }

    void OnLogBtn()
    {

    }

}

public class ArenaHero
{
    public int pid;
    public string nm;
    public int rk;
    public int lvl;
    public int fc;
    public int heroState;
    public HeroData[] herolist = new HeroData[5];
}