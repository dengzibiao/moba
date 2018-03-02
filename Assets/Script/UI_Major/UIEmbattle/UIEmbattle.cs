using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using Tianyu;

public enum OpenSourceType
{
    Dungeons = 1,   //副本
    ArenaDefen,     //角斗场防守
    actGold,        //活动金币
    actExpe,        //活动经验
    actPower,       //活动力量
    actAgile,       //活动敏捷
    actIntel,       //活动智力
    City,           //主城 4人 做记录
    Moba,           //不做记录，为空
    Arena,          //角斗场进攻 
    Moba3V3,
    Moba5V5,

}

public class UIEmbattle : GUIBase
{
    //新手引导位置挂点
    public GameObject HeroButton1;
    public GameObject HeroButton2;

    public static UIEmbattle instance;

    public delegate void OnConfirmDel(bool isCon);
    public OnConfirmDel OnConfirm;
    public bool isMoba = false;
    public static OpenSourceType sourceType = OpenSourceType.City;

    public UIButton backBtn;
    public GUISingleButton ConfirmBtn;
    public GUISingleCheckBoxGroup TypeHeroTab;
    public UIGrid itemEmbattleGrid;
    public ItemHeroLineUp[] itemHeroLineUps;
    public ItemEmbattleList heroModelDefault;
    public ItemEmbattleList[] itemHeroModul;
    public GameObject[] modelPos;
    public UISprite[] tabSprite;
    public UIPanel Matching;
    public UILabel fcLabel;
    public UILabel PromptLabel;
    public TweenAlpha tips;

    public GUISingleButton twoBtn;
    public GUISingleButton threeBtn;
    public GameObject arenaProm;

    GameObject itemEmbattle;

    HeroData[] heroPlayList = new HeroData[6];
    Dictionary<long, ItemEmbattleList> itemEmbattles = new Dictionary<long, ItemEmbattleList>();
    List<ItemHeroLineUp> ups = new List<ItemHeroLineUp>();

    List<HeroData> herodatalist;

    int PreviousFc = 0;
    int TotalFc = 0;
    int Difference = 0;

    bool ThreeState = true;

    public UIEmbattle()
    {
        instance = this;
    }

    public override UIPanleID GetUIKey()
    {
        return UIPanleID.UIEmbattle;
    }

    protected override void SetUI(params object[] uiParams)
    {
        if (uiParams.Length > 0)
            sourceType = (OpenSourceType)uiParams[0];
        base.SetUI(uiParams);
    }

    protected override void OnLoadData()
    {
        base.OnLoadData();
        Show();
        Singleton<Notification>.Instance.RegistMessageID(MessageID.pve_search_moba_list_ret, this.GetUIKey());
        Singleton<Notification>.Instance.RegistMessageID(MessageID.pve_search_moba_list_ret, UIPanleID.UIEmbattle);
    }
    public override void ReceiveData(uint messageID)
    {
        base.ReceiveData(messageID);

        if (messageID == MessageID.pve_search_moba_list_ret)
            MobaMatchedAndSwitch(sourceType);
    }

    protected override void Init()
    {
        base.Init();

        itemEmbattle = Resources.Load(GameLibrary.PATH_UIPrefab + "ItemEmbattle") as GameObject;

        EventDelegate.Set(backBtn.onClick, OnBackBtnClick);
        ConfirmBtn.onClick = OnConfirmBtnClick;
        TypeHeroTab.onClick = OnTypeHeroTab;

        twoBtn.onClick += OnTwoBtn;
        threeBtn.onClick += OnThreeBtn;

        RefreshItemHeroLineUps();
    }

    protected override void ShowHandler()
    {
        if (!InitItem()) return;
        RefreshIEGrid(herodatalist, 0);
        TypeHeroTab.DefauleIndex = 0;
        if (CheckStateIsArena())
        {
            ThreeState = sourceType == OpenSourceType.Arena ? Globe.arenaFightHero[5] == 1 : Globe.adFightHero[5] == 1;
            ArenaSetBtnAplha(!ThreeState);
            itemHeroLineUps[0].transform.localPosition = new Vector3(-169.5f, -308.1f, 0);
            arenaProm.SetActive(true);
        }
        else
        {
            itemHeroLineUps[0].transform.localPosition = new Vector3(21.1f, -308.1f, 0);
            arenaProm.SetActive(false);
        }
        for (int i = 0; i < modelPos.Length; i++)
        {
            modelPos[i].SetActive(CheckStateIsArena());
        }
        heroModelDefault.gameObject.SetActive(!CheckStateIsArena());
        twoBtn.gameObject.SetActive(CheckStateIsArena());
        threeBtn.gameObject.SetActive(CheckStateIsArena());
        InitHeroPlay();
    }

    void InitHeroPlay()
    {
        SetTabState(100);
        InitHeroState();
        if (sourceType == OpenSourceType.Arena)
        {
            CheckHerodata(Globe.arenaFightHero, false);
            SaveHeroList(Globe.challengeTeam, ref heroPlayList);
            ConfirmBtn.text = "进入游戏";
            InitArenaHero();
        }
        else if (sourceType == OpenSourceType.ArenaDefen)
        {
            CheckHerodata(Globe.adFightHero, false);
            SaveHeroList(Globe.defendTeam, ref heroPlayList);
            ConfirmBtn.text = "确定";
            InitArenaHero();
        }
        else
        {
            ConfirmBtn.text = "确定";
            if (sourceType == OpenSourceType.Moba || sourceType == OpenSourceType.Moba3V3 || sourceType == OpenSourceType.Moba5V5)
            {
                HeroData[] mobaData = new HeroData[6];
                SaveHeroList(mobaData, ref heroPlayList);
                ConfirmBtn.text = "进入游戏";
                if (null == heroPlayList[0] || heroPlayList[0].id == 0)
                    SetConfirmBtnState(false);
                List<long> itemKey = new List<long>(itemEmbattles.Keys);
                for (int i = 0; i < itemEmbattles.Count; i++)
                {
                    itemEmbattles[itemKey[i]].SetPlay(false);
                }
            }
            else
            {
                SetConfirmBtnState(true);
                switch (sourceType)
                {
                    case OpenSourceType.City:
                    case OpenSourceType.Dungeons:
                        CheckHerodata(Globe.fightHero);
                        SaveHeroList(Globe.playHeroList, ref heroPlayList);
                        break;
                    case OpenSourceType.actGold:
                        SetTabState(0);
                        CheckHerodata(Globe.ed1FightHero);
                        SaveHeroList(Globe.actGold, ref heroPlayList);
                        break;
                    case OpenSourceType.actExpe:
                        SetTabState(0);
                        CheckHerodata(Globe.ed2FightHero);
                        SaveHeroList(Globe.actExpe, ref heroPlayList);
                        break;
                    case OpenSourceType.actPower:
                        SetTabState(1);
                        CheckHerodata(Globe.ed3FightHero);
                        SaveHeroList(Globe.actPower, ref heroPlayList);
                        break;
                    case OpenSourceType.actAgile:
                        SetTabState(2);
                        CheckHerodata(Globe.ed4FightHero);
                        SaveHeroList(Globe.actAgile, ref heroPlayList);
                        break;
                    case OpenSourceType.actIntel:
                        SetTabState(3);
                        CheckHerodata(Globe.ed5FightHero);
                        SaveHeroList(Globe.actIntel, ref heroPlayList);
                        break;
                }
            }
            DefaultData();
            PlayType(true);
        }
        PreviousFc = RefreshFighting(heroPlayList);
        fcLabel.text = "" + PreviousFc;
    }

    void CheckHerodata(int[] herodata, bool isFour = true)
    {
        for (int i = 0; i < herodata.Length; i++)
        {
            if (isFour)
            {
                if (i > 3)
                    herodata[i] = 0;
            }
            else
            {
                //if (i == 1 || i == 2 || i == 3)
                //    herodata[i] = 0;
            }
        }
    }

    void InitHeroState()
    {

        for (int i = 1; i < 3; i++)
        {
            itemHeroLineUps[i].gameObject.SetActive(CheckStateIsArena());
        }

        for (int i = 0; i < itemHeroLineUps.Length; i++)
        {
            itemHeroLineUps[i].SetLock(false);
        }
    }

    void SaveHeroList(HeroData[] last, ref HeroData[] copyTo)
    {
        for (int i = 0; i < last.Length; i++)
        {
            if (copyTo.Length <= i) continue;
            copyTo[i] = last[i];
        }
    }

    void SaveHeroList(int[] last, ref int[] copyTo)
    {
        for (int i = 0; i < last.Length; i++)
        {
            copyTo[i] = last[i];
        }
        if (CheckStateIsArena())
        {
            if (!ThreeState)// && !CheckIsFlag(copyTo[5])
            {
                for (int i = 2; i < 5; i++)
                {
                    copyTo[i] = copyTo[i + 1];
                }
            }
            copyTo[5] = ThreeState ? 1 : 2;
        }
        else
        {
            for (int i = 1; i < copyTo.Length; i++)
            {
                if (i > 3)
                {
                    copyTo[i] = 0;
                }
                else
                {
                    copyTo[i] = copyTo[i + 2];
                }
            }
        }
    }

    void PlayType(bool isFour)
    {

        List<long> itemKey = new List<long>(itemEmbattles.Keys);
        for (int i = 0; i < itemEmbattles.Count; i++)
        {
            itemEmbattles[itemKey[i]].SetPlay(false);
        }

        for (int i = 0; i < itemHeroLineUps.Length; i++)
        {
            itemHeroLineUps[i].RefreshUI(null);
        }

        if (isFour)
        {
            for (int i = 0; i < heroPlayList.Length; i++)
            {
                if (CheckArenaSite(i))
                    continue;
                HeroToBattle(heroPlayList[i], true, true);
                if (null != heroPlayList[i] && itemEmbattles.ContainsKey(heroPlayList[i].id))
                    itemEmbattles[heroPlayList[i].id].SetPlay(true);
                if (i == 0 && (null == heroPlayList[i] || heroPlayList[i].id == 0))
                    RefreshHeroModul(0, null);
            }
        }
        else
        {
            for (int i = 0; i < itemHeroModul.Length; i++)
            {
                RefreshHeroModul(i, null);
            }
            for (int i = 0; i < heroPlayList.Length; i++)
            {
                HeroToBattle(heroPlayList[i], true, true, false);
                if (null != heroPlayList[i] && itemEmbattles.ContainsKey(heroPlayList[i].id))
                    itemEmbattles[heroPlayList[i].id].SetPlay(true);
            }
        }
        for (int j = 0; j < ups.Count; j++)
        {
            ups[j].isShow = false;
        }
        ups.Clear();
    }

    public bool IsAllowPlay(HeroData hd)
    {
        if (sourceType == OpenSourceType.Arena || sourceType == OpenSourceType.ArenaDefen)
        {
            for (int i = 0; i < itemHeroLineUps.Length; i++)
            {
                if (!itemHeroLineUps[i].isShow && ((ThreeState && i != 5) || (!ThreeState && i != 2)))
                    return true;
            }
        }
        else
        {
            for (int i = 0; i < itemHeroLineUps.Length; i++)
            {
                if (!itemHeroLineUps[i].isShow && itemHeroLineUps[i].gameObject.activeSelf)
                    return true;
            }
        }
        return false;
    }

    bool InitItem()
    {
        GameObject item = null;
        ItemEmbattleList itemEmba = null;
        herodatalist = new List<HeroData>();
        if (playerData.GetInstance().herodataList.Count <= 0)
        {
            Debug.Log("herodataList count is 0");
            return false;
        }
        if (itemEmbattleGrid.transform.childCount > 0)
        {
            for (int i = itemEmbattleGrid.transform.childCount - 1; i >= 0; i--)
            {
                if (itemEmbattleGrid.GetChild(i) != null)
                    DestroyImmediate(itemEmbattleGrid.GetChild(i).gameObject);
            }
            itemEmbattles.Clear();
        }
        herodatalist = playerData.GetInstance().herodataList;
        herodatalist.Sort(new HeroDataComparer());
        for (int i = 0; i < herodatalist.Count; i++)
        {
            item = NGUITools.AddChild(itemEmbattleGrid.gameObject, itemEmbattle);
            if (item.GetComponent<ItemEmbattleList>())
            {
                itemEmba = item.GetComponent<ItemEmbattleList>();
                if (itemEmbattles.ContainsKey(herodatalist[i].id))
                    itemEmbattles.Remove(herodatalist[i].id);
                itemEmbattles.Add(herodatalist[i].id, itemEmba);

                itemEmba.RefreshItemUI(herodatalist[i]);
                if (null == itemEmba.OnClickItem)
                    itemEmba.OnClickItem = OnIECallBack;
            }
        }
        return true;
    }

    void RefreshIEGrid(List<HeroData> herolist, int type)
    {
        ItemEmbattleList itemEmba = null;
        for (int i = 0; i < herolist.Count; i++)
        {
            if (type == 0)
            {
                if (itemEmbattles.TryGetValue(herolist[i].id, out itemEmba))
                {
                    if (!itemEmba.gameObject.activeSelf)
                        itemEmba.gameObject.SetActive(true);
                }
                if (sourceType == OpenSourceType.actGold)
                {
                    if (herolist[i].node.sex == 2)
                        itemEmba.gameObject.SetActive(false);
                }
                else if (sourceType == OpenSourceType.actExpe)
                {
                    if (herolist[i].node.sex == 1)
                        itemEmba.gameObject.SetActive(false);
                }
            }
            else
            {
                if (!itemEmbattles.TryGetValue(herolist[i].id, out itemEmba)) continue;

                if (herolist[i].node.attribute != type)
                {
                    if (itemEmba.gameObject.activeSelf)
                        itemEmba.gameObject.SetActive(false);
                }
                else
                {
                    if (!itemEmba.gameObject.activeSelf)
                        itemEmba.gameObject.SetActive(true);
                }

            }
        }
        //itemEmbattleGrid.repositionNow = true;
        itemEmbattleGrid.Reposition();
        itemEmbattleGrid.transform.parent.GetComponent<UIScrollView>().ResetPosition();
    }

    void OnIECallBack(ItemEmbattleList item, bool isPlay)
    {
        HeroToBattle(item.heroData, isPlay);
    }

    public void ShowTips()
    {
        Control.ShowGUI(UIPanleID.UITooltips, EnumOpenUIType.DefaultUIOrSecond, false, "出战英雄列表已满");
    }

    void HeroToBattle(HeroData hd, bool isPlay, bool isInit = false, bool isFour = true)
    {
        if (!isPlay)
        {
            for (int i = 0; i < itemHeroLineUps.Length; i++)
            {
                if (CheckArenaSite(i)) continue;
                if (null != hd && null != itemHeroLineUps[i].heroData && itemHeroLineUps[i].heroData.id == hd.id)
                {
                    itemHeroLineUps[i].RefreshUI(null);
                    heroPlayList[i] = null;
                    CheckHeroModul(i, null);
                    RefreshFighting(heroPlayList);
                    //if (sourceType == OpenSourceType.ArenaDefen || sourceType == OpenSourceType.Arena)
                    //{
                    //    if (i != 1 && i != 2 && i != 3)
                    //    {
                    //        HeroSort();
                    //    }
                    //}
                    return;
                }
            }
            return;
        }

        for (int i = 0; i < itemHeroLineUps.Length; i++)
        {
            if (!isFour && ((ThreeState && i == 5) || (!ThreeState && i == 2)))
                itemHeroLineUps[i].heroData = null;
            if (null != hd && null != itemHeroLineUps[i] && null != itemHeroLineUps[i].heroData && itemHeroLineUps[i].heroData.id == hd.id && hd.id != 0)
                return;
        }

        if (isInit)
        {
            for (int i = 0; i < itemHeroLineUps.Length; i++)
            {
                if (CheckArenaSite(i))
                    continue;
                if (!itemHeroLineUps[i].isShow)
                {
                    itemHeroLineUps[i].RefreshUI(hd);
                    CheckHeroModul(i, hd);
                    RefreshFighting(heroPlayList);
                    if (null == hd || hd.id == 0)
                    {
                        itemHeroLineUps[i].isShow = true;
                        ups.Add(itemHeroLineUps[i]);
                    }
                    return;
                }
            }

        }
        else
        {


            for (int i = 0; i < itemHeroLineUps.Length; i++)
            {
                if (CheckStateIsArena() && ((ThreeState && i == 5) || !ThreeState && i == 2)) continue;
                if (CheckArenaSite(i)) continue;
                if (!itemHeroLineUps[i].isShow)
                {
                    itemHeroLineUps[i].RefreshUI(hd);
                    heroPlayList[i] = hd;
                    CheckHeroModul(i, hd);
                    RefreshFighting(heroPlayList);
                    //if (sourceType == OpenSourceType.ArenaDefen || sourceType == OpenSourceType.Arena)
                    //    HeroSort();
                    return;
                }
            }
            //for (int i = 1; i < 4; i++)
            //{
            //    if (!itemHeroLineUps[i].isShow)
            //    {
            //        itemHeroLineUps[i].RefreshUI(hd);
            //        heroPlayList[i] = hd;
            //        CheckHeroModul(i, hd);
            //        RefreshFighting(heroPlayList);
            //        return;
            //    }
            //}
        }
    }

    void HeroSort()
    {
        List<HeroData> item = new List<HeroData>();
        for (int i = 0; i < 3; i++)
        {
            if (itemHeroLineUps[i == 0 ? 0 : i + 3].isShow && null != itemHeroLineUps[i == 0 ? 0 : i + 3].heroData.attrNode)
                item.Add(itemHeroLineUps[i == 0 ? 0 : i + 3].heroData);
        }
        //item.Sort(new ArenaHeroSort());
        //item.Sort(new ArenaHeroSort());
        int index = 0;
        HeroData herodata = null;
        for (int i = 0; i < 3; i++)
        {
            index = i == 0 ? 0 : i + 3;
            herodata = i < item.Count ? item[i] : null;
            if (null == itemHeroLineUps[index].heroData || null == herodata || itemHeroLineUps[index].heroData.id != herodata.id)
            {
                itemHeroLineUps[index].RefreshUI(herodata);
                heroPlayList[index] = herodata;
                CheckHeroModul(index, herodata);
            }

        }
        RefreshFighting(heroPlayList);
        item.Clear();
    }

    void RefreshItemHeroLineUps()
    {
        for (int m = 0; m < itemHeroLineUps.Length; m++)
        {
            itemHeroLineUps[m].OnClickItemLineUp = HeroBackList;
            itemHeroLineUps[m].RefreshUI(null);
        }
    }

    void HeroBackList(HeroData hd)
    {
        if (itemEmbattles.ContainsKey(hd.id))
        {
            for (int i = 0; i < heroPlayList.Length; i++)
            {
                if (null != heroPlayList[i] && heroPlayList[i].id == hd.id)
                    heroPlayList[i] = null;
            }
            //bool isSort = false;
            for (int i = 0; i < itemHeroLineUps.Length; i++)
            {
                if (null != itemHeroLineUps[i].heroData && itemHeroLineUps[i].heroData.id == hd.id)
                {
                    itemHeroLineUps[i].RefreshUI(null);
                    CheckHeroModul(i, null);
                    //if (i != 1 && i != 2 && i != 3)
                    //    isSort = true;
                }
            }

            //if (sourceType == OpenSourceType.ArenaDefen || sourceType == OpenSourceType.Arena)
            //{
            //    if (isSort)
            //        HeroSort();
            //}
            itemEmbattles[hd.id].SetPlay(false);
            RefreshFighting(heroPlayList);
        }
    }

    void CheckHeroModul(int index, HeroData hd)
    {
        if (!CheckStateIsArena())
        {
            if (index == 0)
            {
                RefreshHeroModul(0, hd);
                if (sourceType == OpenSourceType.Moba || sourceType == OpenSourceType.Moba3V3 || sourceType == OpenSourceType.Moba5V5 || sourceType == OpenSourceType.Arena || sourceType == OpenSourceType.ArenaDefen)
                    SetConfirmBtnState(null != hd);
            }
        }
        else
        {
            if (!ThreeState && index > 2)
                index--;
            if (index < itemHeroModul.Length)
                RefreshHeroModul(index, hd);
            SetConfirmBtnState(CheckArenaHaveHero());
        }
    }

    void RefreshHeroModul(int index, HeroData hd)
    {
        if (CheckStateIsArena())
        {
            itemHeroModul[index].RefreshHeroModul(hd, index, false, ThreeState);
        }
        else
        {
            heroModelDefault.RefreshHeroModul(hd, index, true);
        }
    }

    void OnTypeHeroTab(int index, bool boo)
    {
        if (!boo) return;
        switch (index)
        {
            case 0:
                RefreshIEGrid(herodatalist, 0);
                break;
            case 1:
                RefreshIEGrid(herodatalist, 1);
                break;
            case 2:
                RefreshIEGrid(herodatalist, 3);
                break;
            case 3:
                RefreshIEGrid(herodatalist, 2);
                break;
        }
    }

    void OnBackBtnClick()
    {
        HeroPosEmbattle.instance.HideModel();
        OnCloseOperating();
        ClosePanelOperate(false);
    }

    void OnConfirmBtnClick()
    {
        if (sourceType == OpenSourceType.Moba)
        {
            for (int i = 0; i < Globe.mobaMyTeam.Length; i++)
            {
                Globe.mobaMyTeam[i] = heroPlayList[i == 0 ? i : i + 2];
            }
            Matching.gameObject.SetActive(true);
            StartCoroutine(SendMobaMatch(1));
        }
        else if (sourceType == OpenSourceType.Moba3V3)
        {
            for (int i = 0; i < Globe.moba3v3MyTeam1.Length; i++)
            {
                Globe.moba3v3MyTeam1[i] = heroPlayList[i == 0 ? i : i + 2];
            }
            Matching.gameObject.SetActive(true);
            StartCoroutine(SendMobaMatch(2));
        }
        else if (sourceType == OpenSourceType.Moba5V5)
        {
            Matching.gameObject.SetActive(true);
            StartCoroutine(SendMobaMatch(3));
        }
        else
        {
            OnCloseOperating();
        }

        ClosePanelOperate(true);

    }

    IEnumerator SendMobaMatch(int type)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", type);
        newpacket.Add("arg2", playerData.GetInstance().GetHeroFormationIds(sourceType));
        Singleton<Notification>.Instance.Send(MessageID.pve_search_moba_list_req, newpacket, C2SMessageType.ActiveWait);
        yield return null;
    }

    public void MobaMatchedAndSwitch(OpenSourceType type)
    {
        switch (type)
        {
            case OpenSourceType.Dungeons:
                break;
            case OpenSourceType.ArenaDefen:
                break;
            case OpenSourceType.actGold:
                break;
            case OpenSourceType.actExpe:
                break;
            case OpenSourceType.actPower:
                break;
            case OpenSourceType.actAgile:
                break;
            case OpenSourceType.actIntel:
                break;
            case OpenSourceType.City:
                break;
            case OpenSourceType.Moba:
                Singleton<SceneManage>.Instance.Current = EnumSceneID.Dungeons;
                Singleton<SceneManage>.Instance.mobaltype = 1;
                ClientSendDataMgr.GetSingle().GetBattleSend().Sendpve_init_moba_fight_req();

                //UI_Loading.LoadScene(GameLibrary.PVP_Moba, 3);
                break;
            case OpenSourceType.Arena:
                break;
            case OpenSourceType.Moba3V3:
                Singleton<SceneManage>.Instance.Current = EnumSceneID.Dungeons;
                Singleton<SceneManage>.Instance.mobaltype = 3;
                ClientSendDataMgr.GetSingle().GetBattleSend().Sendpve_init_moba_fight_req();

                // UI_Loading.LoadScene(GameLibrary.PVP_Moba3v3, 3);
                break;
            case OpenSourceType.Moba5V5:
                Singleton<SceneManage>.Instance.Current = EnumSceneID.Dungeons;
                //UI_Loading.LoadScene(GameLibrary.PVP_Moba5v5, 3);
                GameLibrary.LastScene = SceneManager.GetActiveScene().name;//记录前一个场景名
                StartLandingShuJu.GetInstance().GetLoadingData(GameLibrary.PVP_Moba5v5, 3);
                SceneManager.LoadScene("Loding");
                break;
            default:
                break;
        }
        Control.HideGUI(true);
    }

    void OnCloseOperating()
    {
        if ((null != heroPlayList[0] && heroPlayList[0].id != 0) || CheckArenaHaveHero())
        {
            switch (sourceType)
            {
                case OpenSourceType.City:
                case OpenSourceType.Dungeons:
                    CheckToSave(heroPlayList, ref Globe.fightHero, 1);
                    if (heroPlayList[0].id != GameLibrary.player)
                    {
                        if (null != CharacterManager.playerCS.pet)
                        {
                            CharacterManager.playerCS.HidePet();
                            ClientSendDataMgr.GetSingle().GetPetSend().SendChangeMountOrPetState(C2SMessageType.ActiveWait, 1, MountAndPetNodeData.Instance().currentPetID, 0);
                        }
                        if (CharacterManager.playerCS.pm.isRiding)//当主英雄改变 如果之前是上马状态 改成下马状态（通知后端）
                        {
                            // CharacterManager.playerCS.pm.Ride(false);
                            ClientSendDataMgr.GetSingle().GetPetSend().SendChangeMountOrPetState(C2SMessageType.ActiveWait, 2, MountAndPetNodeData.Instance().currentMountID, 0);
                        }
                        GameLibrary.player = heroPlayList[0].id;
                        CharacterManager.instance.CreateTownPlayer();
                        UIRole.Instance.RefreshIconId((int)GameLibrary.player);
                    }
                    break;
                case OpenSourceType.ArenaDefen:
                    CheckToSave(heroPlayList, ref Globe.adFightHero, 2);
                    break;
                case OpenSourceType.actGold:
                    CheckToSave(heroPlayList, ref Globe.ed1FightHero, 3);
                    break;
                case OpenSourceType.actExpe:
                    CheckToSave(heroPlayList, ref Globe.ed2FightHero, 4);
                    break;
                case OpenSourceType.actPower:
                    CheckToSave(heroPlayList, ref Globe.ed3FightHero, 5);
                    break;
                case OpenSourceType.actAgile:
                    CheckToSave(heroPlayList, ref Globe.ed4FightHero, 6);
                    break;
                case OpenSourceType.actIntel:
                    CheckToSave(heroPlayList, ref Globe.ed5FightHero, 7);
                    break;
                case OpenSourceType.Arena:
                    CheckToSave(heroPlayList, ref Globe.arenaFightHero, 8);
                    break;
            }
        }
        if (sourceType != OpenSourceType.Arena)
        {
            HeroPosEmbattle.instance.HideModel();
            //Hide();
        }
    }

    void CheckToSave(HeroData[] last, ref int[] copyTo, int tyoes = -1)
    {
        bool isSave = false;
        int[] toCopy = new int[last.Length];
        for (int i = 0; i < last.Length; i++)
        {
            toCopy[i] = last[i] == null ? 0 : (int)last[i].id;
            if (copyTo[i] != toCopy[i])
            {
                isSave = true;
            }
        }
        if (isSave)
        {
            SaveHeroList(toCopy, ref copyTo);
            if (tyoes != -1)
                C2SHeroList(tyoes, copyTo);
        }
    }

    int RefreshFighting(HeroData[] heroData)
    {
        if (null == heroData || heroData.Length <= 0)
        {
            fcLabel.text = "" + 0;
            return 0;
        }
        TotalFc = 0;
        for (int i = 0; i < heroData.Length; i++)
        {
            if (null == heroData[i] || heroData[i].id == 0) continue;
            if (heroData[i].fc == 0)
            {
                heroData[i] = playerData.GetInstance().GetHeroDataByID(heroData[i].id);
            }

            if (null != heroData[i])
            {
                TotalFc += heroData[i].fc;
            }
        }
        if (PreviousFc != TotalFc)
        {
            Difference = Mathf.Abs(TotalFc - PreviousFc);
            InvokeRepeating("AddFc", 0, 0.02f);
        }
        return TotalFc;
    }

    void AddFc()
    {
        if (TotalFc > PreviousFc)
        {
            fcLabel.text = "" + PreviousFc;
            if (TotalFc <= PreviousFc)
            {
                CancelInvoke("AddFc");
                PreviousFc = TotalFc;
                return;
            }
            PreviousFc += System.Convert.ToInt32(Difference / 20f);
            if (TotalFc <= PreviousFc)
            {
                PreviousFc = TotalFc;
            }
        }
        else
        {
            fcLabel.text = "" + PreviousFc;
            if (TotalFc >= PreviousFc)
            {
                CancelInvoke("AddFc");
                PreviousFc = TotalFc;
                return;
            }
            PreviousFc -= System.Convert.ToInt32(Difference / 20f);
            if (TotalFc >= PreviousFc)
            {
                PreviousFc = TotalFc;
            }
        }
    }

    void C2SHeroList(int tyoes, int[] hd)
    {
        if (null == hd)
        {
            Debug.Log("Save failed");
            return;
        }
        Dictionary<string, long> hero = new Dictionary<string, long>();
        int index = 1;
        for (int i = 0; i < hd.Length; i++)
        {
            if (!hero.ContainsKey((index).ToString()))
            {
                hero.Add((index).ToString(), hd[i]);
                index++;
            }
        }
        if (hero.Count < 6)
        {
            for (int i = hero.Count + 1; i <= 6; i++)
            {
                if (CheckStateIsArena() && i == 6)
                {
                    hero.Add((i).ToString(), ThreeState ? 1 : 2);
                    break;
                }
                hero.Add((i).ToString(), 0);
            }
        }
        ClientSendDataMgr.GetSingle().GetHeroSend().SendAssignFightHero(tyoes, hero);
    }

    void SetTabState(int index)
    {
        if (index == 100)
        {
            TypeHeroTab.DefauleIndex = 0;
            PromptLabel.gameObject.SetActive(false);
        }
        else
        {
            TypeHeroTab.DefauleIndex = index;
            PromptLabel.gameObject.SetActive(true);

            switch (index)
            {
                case 0:
                    if (sourceType == OpenSourceType.actGold)
                        PromptLabel.text = "只能使用男性英雄";
                    else
                        PromptLabel.text = "只能使用女性英雄";
                    break;
                case 1: PromptLabel.text = "只能使用力量英雄"; break;
                case 2: PromptLabel.text = "只能使用敏捷英雄"; break;
                case 3: PromptLabel.text = "只能使用智力英雄"; break;
            }
        }

        if (index == 100)
        {
            for (int i = 0; i < tabSprite.Length; i++)
            {
                UnityUtil.SetBtnState(tabSprite[i].gameObject, true);
            }
        }
        else
        {
            for (int i = 0; i < tabSprite.Length; i++)
            {
                if (i != index)
                {
                    UnityUtil.SetBtnState(tabSprite[i].gameObject, false);
                }
            }
        }
    }

    void OnTwoBtn()
    {
        ArenaSetBtnAplha(true);
        ThreeState = false;
        InitArenaHero(false);
    }

    void OnThreeBtn()
    {
        ArenaSetBtnAplha(false);
        ThreeState = true;
        InitArenaHero(false);
    }

    void ArenaSetBtnAplha(bool isThree)
    {
        twoBtn.GetComponent<UISprite>().alpha = isThree ? 1 : 120f / 255;
        threeBtn.GetComponent<UISprite>().alpha = isThree ? 120f / 255 : 1;
        twoBtn.isEnabled = !isThree;
        threeBtn.isEnabled = isThree;
    }

    bool CheckStateIsArena()
    {
        return sourceType == OpenSourceType.Arena || sourceType == OpenSourceType.ArenaDefen;
    }

    bool CheckIsFlag(int index)
    {
        return index == 0 || index == 1 || index == 2;
    }

    bool CheckArenaSite(int index)
    {
        return !CheckStateIsArena() && (index == 1 || index == 2);
    }

    void InitArenaHero(bool isInit = true)
    {
        if (null == heroPlayList[0] || heroPlayList[0].id == 0)
            SetConfirmBtnState(false);
        //UnityUtil.SetBtnState(ConfirmBtn.gameObject, false);
        itemHeroLineUps[2].SetLock(!ThreeState);
        itemHeroLineUps[5].SetLock(ThreeState);
        SetModelIndex();
        HeroPosEmbattle.instance.SetArenaModelPos(ThreeState);
        if (ThreeState)
        {
            if (!isInit)
                ThreeToTwo();
            modelPos[1].transform.localPosition = new Vector3(0, 3, 0);
            modelPos[0].transform.localPosition = new Vector3(0, 34, 0);
        }
        else
        {
            modelPos[1].transform.localPosition = new Vector3(0, 236, 0);
            modelPos[0].transform.localPosition = new Vector3(0, -200, 0);
            TwoToThree();
        }
        PlayType(false);
    }

    void ThreeToTwo()
    {
        for (int i = 2; i < 5; i++)
        {
            heroPlayList[i] = heroPlayList[i + 1];
        }
        heroPlayList[5] = null;
    }

    void TwoToThree()
    {
        for (int i = 4; i >= 2; i--)
        {
            heroPlayList[i + 1] = heroPlayList[i];
        }
        heroPlayList[2] = null;
    }

    void DefaultData()
    {
        for (int i = 3; i >= 1; i--)
        {
            heroPlayList[i + 2] = heroPlayList[i];
        }
        heroPlayList[1] = null;
        heroPlayList[2] = null;
    }

    void SetModelIndex()
    {
        itemHeroModul = new ItemEmbattleList[5];
        if (ThreeState)
        {
            GetModelPos(modelPos[0].transform, 3);
            GetModelPos(modelPos[1].transform, 0);
        }
        else
        {
            GetModelPos(modelPos[0].transform, 0);
            GetModelPos(modelPos[1].transform, 2);
        }
    }

    void GetModelPos(Transform tran, int index)
    {
        for (int i = 0; i < tran.childCount; i++)
        {
            itemHeroModul[index] = tran.GetChild(i).GetComponent<ItemEmbattleList>();
            index++;
        }
    }

    void SetConfirmBtnState(bool isEnable = true)
    {
        ChangeColorGray.Instance.ChangeSpriteColor(ConfirmBtn.GetComponent<UISprite>(), isEnable);
        ConfirmBtn.isEnabled = isEnable;
    }

    bool CheckArenaHaveHero()
    {
        if (!CheckStateIsArena())
            return false;
        for (int i = 0; i < heroPlayList.Length; i++)
        {
            if (null != heroPlayList[i] && heroPlayList[i].id != 0)
                return true;
        }
        return false;
    }

    void ClosePanelOperate(bool isCon)
    {
        if (null != OnConfirm && sourceType != OpenSourceType.City)
            OnConfirm(isCon);

        if ((sourceType == OpenSourceType.Dungeons ||
             sourceType == OpenSourceType.actAgile ||
             sourceType == OpenSourceType.actExpe  ||
             sourceType == OpenSourceType.actGold  ||
             sourceType == OpenSourceType.actIntel ||
             sourceType == OpenSourceType.actPower) && Globe.openSceenID != 0)
        {
            Control.HideGUI();
            object[] openParams = new object[] { FSDataNodeTable<SceneNode>.GetSingleton().FindDataByType(Globe.openSceenID), sourceType };
            Control.ShowGUI(UIPanleID.SceneEntry, EnumOpenUIType.DefaultUIOrSecond, false, openParams);
        }
        else if (sourceType == OpenSourceType.City
             || (sourceType == OpenSourceType.Moba || sourceType == OpenSourceType.Moba3V3 || sourceType == OpenSourceType.Moba5V5) && !isCon
             || sourceType == OpenSourceType.ArenaDefen || (sourceType == OpenSourceType.Arena && !isCon))
        {
            Control.HideGUI();
        }

    }

    protected override void RegisterComponent()
    {
        base.RegisterComponent();
        RegisterComponentID(43, 97, ConfirmBtn.gameObject);

    }
    protected override void RegisterIsOver()
    {
        base.RegisterIsOver();
    }
}

class ArenaHeroSort : IComparer<HeroData>
{
    public int Compare(HeroData x, HeroData y)
    {
        int xAttribute = x.node.attribute;
        int yAttribute = y.node.attribute;
        if (x.node.attribute == 2)
            xAttribute = 3;
        else if (x.node.attribute == 3)
            xAttribute = 2;
        if (y.node.attribute == 2)
            yAttribute = 3;
        else if (y.node.attribute == 3)
            yAttribute = 2;
        if (xAttribute > yAttribute)
        {
            return 1;
        }
        else if (xAttribute < yAttribute)
        {
            return -1;
        }
        else
        {
            return 0;
        }
    }
}