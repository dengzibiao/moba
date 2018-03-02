using System.Collections.Generic;
using UnityEngine;
using Tianyu;
using System;

public class UISceneEntry : GUIBase
{
    public UILabel BtnBattleLabel;//引导挂点

    public static UISceneEntry instance;

    public GUISingleButton BackBtn;
    public GUISingleButton BtnBattle;
    public GUISingleButton BtnLineup;
    public GUISingleButton BtnCleanout;
    public GUISingleButton BtnCleanout5;
    public UILabel LaSceneName;
    public UISprite SpFCArrow;
    public UILabel LaBossName;

    public ItemHeroLineUp[] itemHeroPlay;
    public WdStarCondition[] WdStarConditions;
    public UILabel LaCleanoutTokenNum;
    public UILabel LaRemainTimes;
    public UILabel LaAdvisePower;
    public UILabel LaCurrentPower;
    public UILabel LaCostVigor;
    public UILabel LaNeedLevel;

    public UIScrollView SvMayReward;
    public UIGrid SvMayRewardGrid;
    public GameObject MaybeGainbg;

    public GoodsTips goodsTips;
    public UIRaids raids;
    public SpinWithMouse spinDrug;
    public UIBuySweepVoucher bugSweepVoucher;



    public OpenSourceType type = OpenSourceType.Dungeons;
    public GameObject prompt;

    int cleanoutCount = 0;
    public int CleanoutCount
    {
        get
        {
            return cleanoutCount;
        }
    }

    GameObject go;
    UIEmbattle embattle;

    public SceneNode scene;

    int SweepVoucher = 0;
    int BtnCleanout5Number = 10;

    Dictionary<int, int[]> Dungeons = new Dictionary<int, int[]>();
    List<int[]> eventStar = new List<int[]>();

    protected override void Init()
    {
        base.Init();
        instance = this;
        BackBtn.onClick = OnBackBtn;
        BtnBattle.onClick = OnBattleBtnClick;
        BtnLineup.onClick = OnLineupBtnClick;
        BtnCleanout.onClick = OnCleanoutBtnClick;
        BtnCleanout5.onClick = OnCleanout5BtnClick;
    }

    public override UIPanleID GetUIKey()
    {
        return UIPanleID.SceneEntry;
    }

    protected override void SetUI(params object[] uiParams)
    {
        if (uiParams.Length > 0)
        {
            scene = (SceneNode)uiParams[0];
            if (uiParams.Length > 1)
            {
                type = (OpenSourceType)uiParams[1];
            }
        }
        if (type == OpenSourceType.Dungeons)
            Globe.openSceenID = 0;
        base.SetUI(uiParams);
    }

    protected override void OnLoadData()
    {
        base.OnLoadData();
        if (null != scene)
        {
            Singleton<Notification>.Instance.RegistMessageID(MessageID.pve_reset_elite_dungeon_ret, this.GetUIKey());
            Singleton<Notification>.Instance.RegistMessageID(MessageID.pve_reset_elite_dungeon_ret, UIPanleID.SceneEntry);
            Singleton<Notification>.Instance.RegistMessageID(MessageID.common_buy_someone_ret, this.GetUIKey());
            Singleton<Notification>.Instance.RegistMessageID(MessageID.common_buy_someone_ret, UIPanleID.SceneEntry);
            Show();
            RefreshUI();
        }
    }

    public override void ReceiveData(uint messageID)
    {
        base.ReceiveData(messageID);

        if (messageID == MessageID.pve_reset_elite_dungeon_ret)
        {
            ResetEliteDungeon(2);
        }
        else if (messageID == MessageID.common_buy_someone_ret)
        {
            BuySweepVoucher();
        }
    }

    public void RefreshUI()
    {
        LaSceneName.text = scene.SceneName;
        BtnBattle.GetComponent<BoxCollider>().enabled = true;

        CharacterAttrNode model = null;
        if (FSDataNodeTable<MonsterAttrNode>.GetSingleton().DataNodeList.ContainsKey(scene.boss))
            model = FSDataNodeTable<MonsterAttrNode>.GetSingleton().DataNodeList[scene.boss];
        else
            model = FSDataNodeTable<HeroAttrNode>.GetSingleton().DataNodeList[scene.boss];
        go = HeroPosEmbattle.instance.CreatModelByModelID(model.model, PosType.DetailPos, spinDrug, MountAndPet.Null);
        //go = HeroPosEmbattle.instance.CreatModel(model.icon_name, PosType.DetailPos, spinDrug);
        go.transform.localScale = Vector3.one * scene.model_size;

        if (scene.Type == 2)
        {
            CheckSystemTime();
            LaNeedLevel.enabled = false;
        }
        else
        {
            LaNeedLevel.enabled = true;
        }

        LaBossName.text = scene.animationName;
        LaAdvisePower.text = "推荐战力：" + scene.fighting_capacity;
        LaCostVigor.text = "" + scene.power_cost;
        LaNeedLevel.text = "进入等级：[00FF21FF]" + scene.pass_lv + "级[-]";

        for (int i = SvMayRewardGrid.transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(SvMayRewardGrid.transform.GetChild(i).gameObject);
        }
        GameObject goMay;
        for (int i = 0; i < scene.drop.Length; i++)
        {
            if (null != scene.drop[i] && scene.drop[i] is object[]) continue;
            goMay = NGUITools.AddChild(SvMayRewardGrid.gameObject, MaybeGainbg);
            goMay.GetComponent<MaybeGain>().Init(scene.drop[i]);
        }

        SvMayRewardGrid.Reposition();
        SvMayReward.ResetPosition();

        BuySweepVoucher();

        ResetEliteDungeon();

        //刷新箭头，战斗力，出战英雄
        OnConfirmLineup(true);

        SetCleanoutBtnState();

        for (int i = 0; i < WdStarConditions.Length; i++)
        {
            if (type == OpenSourceType.Dungeons)
                WdStarConditions[i].RefreshUI(scene.star_describe[i], !Dungeons.ContainsKey(scene.SceneId) || Dungeons[scene.SceneId][i] > 0);
            else
                WdStarConditions[i].RefreshUI(scene.star_describe[i], eventStar.Count < ((scene.SceneId - scene.bigmap_id) - 1) || eventStar[(scene.SceneId - scene.bigmap_id) - 1][i] > 0);
        }

    }

    void SetCleanoutBtnState()
    {
        if (type == OpenSourceType.Dungeons)
        {
            //BtnCleanout5State();

            if (!GameLibrary.mapOrdinary.TryGetValue(scene.bigmap_id, out Dungeons)) return;
            if (!Dungeons.ContainsKey(scene.SceneId)) return;

            if (Globe.GetStar(Dungeons[scene.SceneId]) >= 3)
                SetBtnCleanoutState(true);
            else
                SetBtnCleanoutState(false);
        }
        else
        {
            int count = GameLibrary.eventdList[(scene.bigmap_id - 30000) / 100];

            eventStar = GameLibrary.eventsList[(scene.bigmap_id - 30000) / 100];

            if (count <= 0)
            {
                SetBtnCleanoutState(false);
            }
            else
            {
                if (Globe.GetStar(eventStar[(scene.SceneId - scene.bigmap_id) - 1]) >= 3)
                {
                    SetBtnCleanoutState(true);
                }
                else
                {
                    SetBtnCleanoutState(false);
                }
            }
            //BtnCleanout5.text = string.Format(Localization.Get("UILevelBtnCleanout5"), count == 0 ? 2 : count);
            //BtnCleanout5Number = count;
        }
    }

    void BtnCleanout5State()
    {
        if (scene.Type == 1)
        {
            BtnCleanout5Number = 10;
        }
        else if (scene.Type == 2)
        {
            BtnCleanout5Number = GameLibrary.mapElite[scene.SceneId][0];
            if (BtnCleanout5Number < 0)
            {
                BtnCleanout5Number = 0;
                GameLibrary.mapElite[scene.SceneId][0] = 0;
            }
        }
        else
        {
            BtnCleanout5Number = GameLibrary.eventdList[(scene.bigmap_id - 30000) / 100];
        }

        if (scene.Type == 2)
        {
            BtnCleanout5.text = BtnCleanout5Number == 0 ? "重置" : "扫荡" + BtnCleanout5Number + "次";
        }
        else
        {
            BtnCleanout5.text = string.Format(Localization.Get("UILevelBtnCleanout5"), BtnCleanout5Number == 0 ? 2 : BtnCleanout5Number);
        }
    }

    void SetFCArrow()
    {
        if (GetHeroListFC() >= scene.fighting_capacity)
            SpFCArrow.spriteName = "jiantou01";
        else
            SpFCArrow.spriteName = "jiantou-hong";
    }

    public void OnCleanoutResult(object[] item, object[] other = null)
    {
        if (!raids.gameObject.activeSelf)
        {
            raids.gameObject.SetActive(true);
        }
        raids.AddRaidsItem(item, other, scene);
        ResetEliteDungeon(scene.Type == 2 ? 1 : 0, item.Length);
    }

    void SetBtnCleanoutState(bool isCanClick)
    {
        UnityUtil.SetBtnState(BtnCleanout.gameObject, isCanClick);
        UnityUtil.SetBtnState(BtnCleanout5.gameObject, isCanClick);
    }

    HeroData[] SumFC()
    {
        switch (type)
        {
            case OpenSourceType.actGold:
                return Globe.actGold;
            case OpenSourceType.actExpe:
                return Globe.actExpe;
            case OpenSourceType.actPower:
                return Globe.actPower;
            case OpenSourceType.actAgile:
                return Globe.actAgile;
            case OpenSourceType.actIntel:
                return Globe.actIntel;
            default: return Globe.playHeroList;
        }
    }

    public void RefreshHeroPlay()
    {
        for (int i = 0; i < itemHeroPlay.Length; i++)
        {
            itemHeroPlay[i].RefreshUI(SumFC()[i], false);
        }
    }

    void OnCleanoutBtnClick()
    {
        CleanoutDungeons(1);
    }

    void OnCleanout5BtnClick()
    {
        CleanoutDungeons(BtnCleanout5Number);
    }

    public void CleanoutDungeons(int count)
    {
        if (IsStrengthEnough(count, false))
        {
            cleanoutCount = count;
            Globe.autoScenceCount = count;
            if (type == OpenSourceType.Dungeons)
            {
                ClientSendDataMgr.GetSingle().GetBattleSend().SendFlashDungeonFight(scene.bigmap_id, scene.SceneId, scene.Type, cleanoutCount);
                Globe.isSaoDang = true;
            }
            else
            {
                Dictionary<string, long> hero = new Dictionary<string, long>();

                for (int i = 0; i < SumFC().Length; i++)
                {
                    if (!hero.ContainsKey((i + 1).ToString()))
                        hero.Add((i + 1).ToString(), null == SumFC()[i] ? 0 : SumFC()[i].id);
                }

                if (SumFC().Length < 6)
                {
                    for (int i = SumFC().Length + 1; i <= 6; i++)
                    {
                        hero.Add((i).ToString(), 0);
                    }
                }
                ClientSendDataMgr.GetSingle().GetBattleSend().SendEventFlashDungeonFight(scene.bigmap_id, scene.SceneId, cleanoutCount, hero);
                Globe.isSaoDang = true;
            }
        }
    }

    void OnBattleBtnClick()
    {

        if (!IsStrengthEnough(1)) return;

        GameLibrary.UseActionPoint = scene.power_cost;

        if (type == OpenSourceType.Dungeons)
        {
            ClientSendDataMgr.GetSingle().GetBattleSend().SendIntoDungeon(scene.bigmap_id, scene.SceneId, scene.Type, HeroList(Globe.playHeroList));
            BattleBtnEnabele();
        }
        else
        {
            if (GameLibrary.eventdList[(scene.SceneId - 30000) / 100] <= 0)
            {
                //print("次数为0");
                //prompt.transform.Find("Label").GetComponent<UILabel>().text = "征战次数为0";
                //prompt.GetComponent<TweenAlpha>().enabled = true;
                //prompt.GetComponent<TweenAlpha>().ResetToBeginning();
                //prompt.GetComponent<TweenAlpha>().PlayForward();
                Control.ShowGUI(UIPanleID.UITooltips, EnumOpenUIType.DefaultUIOrSecond, false, "征战次数为0");
                return;
            }
            switch (type)
            {
                case OpenSourceType.actGold:
                    ChechHeroList(Globe.actGold);
                    break;
                case OpenSourceType.actExpe:
                    ChechHeroList(Globe.actExpe);
                    break;
                case OpenSourceType.actPower:
                    ChechHeroList(Globe.actPower);
                    break;
                case OpenSourceType.actAgile:
                    ChechHeroList(Globe.actAgile);
                    break;
                case OpenSourceType.actIntel:
                    ChechHeroList(Globe.actIntel);
                    break;
            }
        }
    }

    Dictionary<string, long> HeroList(HeroData[] herodata)
    {
        Dictionary<string, long> hero = new Dictionary<string, long>();
        for (int i = 0; i < herodata.Length; i++)
        {
            if (null != herodata[i] && herodata[i].id != 0)
                hero.Add((i + 1).ToString(), herodata[i].id);
            else
                hero.Add((i + 1).ToString(), 0);
        }
        return hero;
    }

    void ChechHeroList(HeroData[] herodata)
    {
        if (null == herodata[0] || herodata[0].id == 0)
            OnLineupBtnClick();
        else
        {
            ClientSendDataMgr.GetSingle().GetBattleSend().SendIntoEventDungeon(scene.SceneId, HeroList(herodata));
            BattleBtnEnabele();
        }
    }

    void OnLineupBtnClick()
    {
        HeroPosEmbattle.instance.HideModel(PosType.DetailPos);
        //UIEmbattle.sourceType = type;
        //if (null == embattle)
        //    embattle = Control.ShowGUI(GameLibrary.UI_Embattle) as UIEmbattle;
        //else
        //    Control.ShowGUI(GameLibrary.UI_Embattle);
        //if (null == embattle.OnConfirm)
        //    embattle.OnConfirm += OnConfirmLineup;

        Globe.openSceenID = scene.SceneId;
        Control.HideGUI(UIPanleID.SceneEntry);
        Control.ShowGUI(UIPanleID.UIEmbattle, EnumOpenUIType.OpenNewCloseOld, false, type);


        //if (null == embattle)
        //    embattle = UIEmbattle.instance;

        //if (null != embattle && null == embattle.OnConfirm)
        //    embattle.OnConfirm += OnConfirmLineup;

    }

    void OnConfirmLineup(bool isDefine)
    {
        HeroPosEmbattle.instance.ShowModel(PosType.DetailPos);
        RefreshHeroPlay();
        LaCurrentPower.text = string.Format(Localization.Get("UILevelLaCurrentPower"), GetHeroListFC());
        SetFCArrow();
    }

    int GetHeroListFC()
    {
        int sumFC = 0;
        for (int i = 0; i < SumFC().Length; i++)
        {
            if (null != SumFC()[i] && SumFC()[i].id != 0 && SumFC()[i].fc == 0)
            {
                sumFC += playerData.GetInstance().GetHeroDataByID(SumFC()[i].id).fc;
            }
            else if (null != SumFC()[i])
            {
                sumFC += SumFC()[i].fc;
            }
        }
        return sumFC;
    }

    void OnHideMaskClick(GameObject go)
    {
        HeroPosEmbattle.instance.HideModel(PosType.DetailPos);
        //gameObject.SetActive(false);
        Control.HideGUI(UIPanleID.SceneEntry);
        //if (null != embattle && null != embattle.OnConfirm)
        //    embattle.OnConfirm = null;
    }

    void OnBackBtn()
    {
        HeroPosEmbattle.instance.HideModel();
        OnHideMaskClick(gameObject);
    }

    public bool IsStrengthEnough(int count, bool isBattle = true)
    {
        //精英副本检查时间
        if (scene.Type == 2)
            CheckSystemTime();

        //体力是否满足
        if (scene.power_cost * count > playerData.GetInstance().baginfo.strength)
        {
            Control.ShowGUI(UIPanleID.UIBuyEnergyVitality, EnumOpenUIType.DefaultUIOrSecond, false, ActionPointType.Energy);
            return false;
        }

        //征战次数是否满足-精英
        if (scene.Type == 2)
        {
            if (GameLibrary.mapElite[scene.SceneId][0] <= 0)
            {
                //购买征战次数
                //bugSweepVoucher.RefreshBuyUI(PromptType.WarTimes, scene);

                ResetLaterNode node = FSDataNodeTable<ResetLaterNode>.GetSingleton().FindDataByType(1);//GameLibrary.mapElite[sn.SceneId][1]
                int reset = GameLibrary.mapElite[scene.SceneId][1] >= node.resetStage.Length - 1 ? node.resetStage.Length - 1 : GameLibrary.mapElite[scene.SceneId][1];
                resetDiamond = node.resetStage[reset];

                ShowUIPopUp("花费" + resetDiamond + "钻石购买" + 3 + "次征战", "", UIPopupType.EnSure, PromptType.WarTimes);

                return false;
            }
        }

        if (isBattle) return true;

        //扫荡卷是否足够
        if (count > SweepVoucher)
        {
            //购买扫荡卷
            //bugSweepVoucher.RefreshBuyUI(PromptType.Buy, scene);

            ItemNodeState item = GameLibrary.Instance().ItemStateList[110000100];

            for (int i = 0; i < item.cprice.Length; i++)
            {
                if (item.cprice[i] != 0)
                {
                    needCount = item.cprice[i];
                    break;
                }
            }

            ShowUIPopUp("花费" + (buyCount * needCount) + "钻石购买" + buyCount + "个扫荡卷", "", UIPopupType.EnSure, PromptType.Buy);

            return false;
        }

        return true;
    }

    /// <summary>
    /// 刷新购买扫荡卷
    /// </summary>
    public void BuySweepVoucher()
    {
        if (bugSweepVoucher.gameObject.activeSelf)
            bugSweepVoucher.gameObject.SetActive(false);
        SweepVoucher = GoodsDataOperation.GetInstance().GetItemCountById(110000100);
        LaCleanoutTokenNum.text = "" + SweepVoucher;
    }

    public void ResetEliteDungeon(int type = 0, int count = 0)
    {
        if (bugSweepVoucher.gameObject.activeSelf)
            bugSweepVoucher.gameObject.SetActive(false);
        if (type == 1)
        {
            if (GameLibrary.mapElite.ContainsKey(scene.SceneId))
                GameLibrary.mapElite[scene.SceneId][0] -= count == 0 ? 1 : count;
            if (GameLibrary.mapElite[scene.SceneId][0] < 0)
                GameLibrary.mapElite[scene.SceneId][0] = 0;
        }
        else if (type == 2)
        {
            if (GameLibrary.mapElite.ContainsKey(scene.SceneId))
            {
                GameLibrary.mapElite[scene.SceneId][0] = 3;
                GameLibrary.mapElite[scene.SceneId][1]++;
            }
        }

        BtnCleanout5State();

        if (scene.SceneId >= 30100)
        {
            LaRemainTimes.text = string.Format(Localization.Get("UILevelLaRemainTimes"), GameLibrary.eventdList[(scene.bigmap_id - 30000) / 100], 2);
        }
        else
        {
            if (scene.Type == 1)
            {
                LaRemainTimes.text = "[bbbbbb]次数[-]：无限";
            }
            else
            {
                LaRemainTimes.text = string.Format(Localization.Get("UILevelLaRemainTimes"), GameLibrary.mapElite[scene.SceneId][0], 3);
            }
        }
    }

    void CheckSystemTime()
    {
        if (!GameLibrary.mapElite.ContainsKey(scene.SceneId)) return;
        int currentymd = Convert.ToInt32(TimeManager.Instance.ConvertIntDateTime(Auxiliary.GetNowTime()).ToString("yyMMdd"));
        if (currentymd > GameLibrary.mapElite[scene.SceneId][2])
        {
            GameLibrary.mapElite[scene.SceneId][0] = 3;
            GameLibrary.mapElite[scene.SceneId][1] = 0;
            GameLibrary.mapElite[scene.SceneId][2] = Convert.ToInt32(TimeManager.Instance.ConvertIntDateTime(Auxiliary.GetNowTime()).ToString("yyMMdd"));
        }
    }

    void BattleBtnEnabele()
    {
        BtnBattle.isEnabled = false;
        CDTimer.GetInstance().AddCD(0.5f, (int count, long id) => { BtnBattle.isEnabled = true; });
    }

    protected override void RegisterComponent()
    {
        base.RegisterComponent();

        RegisterComponentID(120, 4, BtnBattleLabel.gameObject);

    }
    protected override void RegisterIsOver()
    {
        base.RegisterIsOver();
    }

    #region 购买扫荡卷或征战次数

    int resetDiamond;
    int buyCount = 10;
    int needCount = 0;
    PromptType promptType;

    void ShowUIPopUp(string str1, string str2, UIPopupType type, PromptType promptType)
    {
        this.promptType = promptType;
        object[] obj = new object[5] { str1, str2, type, this.gameObject, "OnBuyBtnClick" };
        Control.ShowGUI(UIPanleID.UIPopUp, EnumOpenUIType.DefaultUIOrSecond, false, obj);
    }

    public void OnBuyBtnClick()
    {
        if (promptType == PromptType.Buy)
        {
            if (playerData.GetInstance().GetMyMoneyByType(MoneyType.Diamond) >= buyCount * needCount)
            {
                //ClientSendDataMgr.GetSingle().GetBattleSend().SendBuySomeone(110000100, buyCount);
                Dictionary<string, object> newpacket = new Dictionary<string, object>();
                newpacket.Add("arg1", 110000100);
                newpacket.Add("arg2", buyCount);
                Singleton<Notification>.Instance.Send(MessageID.common_buy_someone_req, newpacket, C2SMessageType.PASVWait);
            }
            else
            {
                Control.ShowGUI(UIPanleID.UITooltips, EnumOpenUIType.DefaultUIOrSecond, false, "当前钻石不足");
            }
            //RefreshBuyUI(PromptType.Diamond, sn);
        }
        else if (promptType == PromptType.WarTimes)
        {
            if (playerData.GetInstance().GetMyMoneyByType(MoneyType.Diamond) >= resetDiamond)
            {
                //ClientSendDataMgr.GetSingle().GetBattleSend().SendResetEliteDungeon(scene.bigmap_id, scene.SceneId);
                Dictionary<string, object> newpacket = new Dictionary<string, object>();
                newpacket.Add("arg1", scene.bigmap_id);
                newpacket.Add("arg2", scene.SceneId);
                Singleton<Notification>.Instance.Send(MessageID.pve_reset_elite_dungeon_req, newpacket, C2SMessageType.ActiveWait);
            }
            else
            {
                Control.ShowGUI(UIPanleID.UITooltips, EnumOpenUIType.DefaultUIOrSecond, false, "当前钻石不足");
            }
            //RefreshBuyUI(PromptType.Diamond, sn);
        }
        else
        {
            //充值界面跳转
            //暂无充值界面
        }
    }

    #endregion

}

public enum PromptType
{
    Buy,
    Diamond,
    WarTimes
}