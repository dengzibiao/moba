using UnityEngine;
using System.Collections.Generic;
using Tianyu;
using System;
/// <summary>
/// 英雄详细信息面板
/// </summary>
public class UI_HeroDetail : GUIBase
{
    //public GameObject EquipTab;
    //public GameObject MailBtn;
    public GameObject Property;
    public GameObject upGradeProperty;//升级属性面板

    public UIScrollView EquipIntensifys;//装备强化
    public UIScrollView EquipEvolveView;//装备进化
    public UIScrollView HeroPreviewlist;//
    public UIScrollView HeroPreviewLists;//

    public UILabel strengthGrowthLabel;
    public UILabel intelligenceGrowthLabel;
    public UILabel agileGrowthLabel;
    public UISprite PowerProgress;
    public UISprite IntelligenceProgress;
    public UISprite AgilityProgress;

    //升级属性面板属性值
    public GUISingleLabel strength;
    public GUISingleLabel intelligence;
    public GUISingleLabel agile;
    public GUISingleLabel strengthGrowth;
    public GUISingleLabel intelligenceGrowth;
    public GUISingleLabel agileGrowth;

    int currentEXP = 0;             //当前经验值
    int countss;
    int upGradelvl = 0;             //所升级数
    int counts;                     //剩余经验值
    int exp;                        //药品经验值
    public string[] propTypeName = new string[18] { "力量", "智力", "敏捷", "生命", "攻击", "护甲", "魔抗", "暴击", "闪避", "命中", "护甲穿透", "魔法穿透", "吸血", "韧性", "移动速度", "攻击速度", "可视范围", "生命回复" };//
    int expDiffe;//经验差值
    public static bool BtnState = true;
    public static UI_HeroDetail instance;

    public GUISingleButton backBtn;              //返回按钮

    public GUISingleCheckBoxGroup detailsTab;    //标签栏

    PlayerLevelUpNode playerNode;//英雄升级数据

    public UIHeroNameArea NameArea;//英雄属性面板
    public UIUpGradeStar UpGradeStar;
    public EquipPanel Equip;//装备操作面板
    public HeroDescription heroDes;//英雄描述面板
    public UIHeroSkill HeroSkill;//英雄技能面板
    public UIRunes Runes;//符文面板
    public UIHeroDetList HeroDetList;//英雄描述信息面板
    public GameObject skillDes;//技能描述
    public SpinWithMouse dragSprite;//英雄模型操作面板
    public Breakthrough breakthrough;//突破面板
    public GameObject EquipIntensify;//装备强化进化
    public GameObject AdvancedBtn;//进阶
    public GameObject HeroPreview;//升级全部

    public static int equipItemState = 0;//所选的装备索引

    public UISprite Blickline;
    UIHeroList UIHeroList;

    public static HeroData hd
    {
        get { return playerData.GetInstance().selectHeroDetail; }
        set { playerData.GetInstance().selectHeroDetail = value; }
    }

    List<GameObject> runeList = new List<GameObject>();
    long[] heroRunes;
    GameObject go;
    public bool isSendDrug { get; set; }        //发送消耗药品升级
    public long heroIDD { get; set; }        //升级英雄ID
    public long itemIDD { get; set; }        //药品ID
    public int countD { get; set; }        //消耗个数
    public int levelD { get; set; }        //所升级数
    public int runesGlod { get; set; }

    bool isOne = false;

    bool UpGradeState = true;

    public override UIPanleID GetUIKey()
    {
        return UIPanleID.UIHeroDetail;
    }


    protected override void Init()
    {
        base.Init();

        instance = this;
        HeroDetList.gameObject.SetActive(true);

        foreach (Transform rune in transform.Find("RunesSeat"))
        {
            runeList.Add(rune.gameObject);
        }

        backBtn.onClick = OnBackBtnClick;
        detailsTab.onClick = OnDetailsTabClick;

        isOne = true;

        UIHeroList = UIHeroList.instance;

        PowerProgress.width = 0;
        PowerProgress.pivot = UIWidget.Pivot.Left;
        IntelligenceProgress.width = 0;
        IntelligenceProgress.pivot = UIWidget.Pivot.Left;
        AgilityProgress.width = 0;
        AgilityProgress.pivot = UIWidget.Pivot.Left;

    }
    bool isShow = false;//是否直接打开英雄详情界面
    protected override void SetUI(params object[] uiParams)
    {
        if (uiParams!=null&& uiParams.Length>0)
        {
            isShow = (bool)uiParams[0];
        }
        base.SetUI();
    }
    protected override void OnLoadData()
    {
        base.OnLoadData();
        Singleton<Notification>.Instance.RegistMessageID(MessageID.common_player_hero_info_ret, UIPanleID.UIHeroDetail);
        Singleton<Notification>.Instance.RegistMessageID(MessageID.common_upgrade_hero_level_ret, UIPanleID.UIHeroDetail);
        Singleton<Notification>.Instance.RegistMessageID(MessageID.common_upgrade_hero_star_ret, UIPanleID.UIHeroDetail);
        Singleton<Notification>.Instance.RegistMessageID(MessageID.common_upgrade_hero_grade_ret, UIPanleID.UIHeroDetail);
        if (isShow)
        {
            Show();
        }
        else
        {
            ClientSendDataMgr.GetSingle().GetHeroSend().SendGetHeroInfo(Globe.selectHero.hero_id, C2SMessageType.ActiveWait);
        }
    }
    public override void ReceiveData(UInt32 messageID)
    {
        base.ReceiveData(messageID);
        switch (messageID)
        {
            case MessageID.common_player_hero_info_ret:
                Show();
                break;
            case MessageID.common_upgrade_hero_level_ret://英雄升级
                SwitchHero();
                Control.ShowGUI(UIPanleID.UITooltips, EnumOpenUIType.DefaultUIOrSecond, false, "英雄升级成功");
                UIHeroNameArea.Instance.PlayShengjiEffect();//英雄升级成功播放升级特效
                if (UIRole.Instance!=null)
                {
                    UIRole.Instance.SetMainHeroLevel();
                }
                break;
            case MessageID.common_upgrade_hero_star_ret://英雄升星
                SwitchHero();
                Control.ShowGUI(UIPanleID.UITooltips, EnumOpenUIType.DefaultUIOrSecond, false, "英雄升星成功");
                Breakthrough.instance.PlayerShengXingEffect(HeroAndEquipNodeData.HD.star - 1, HeroAndEquipNodeData.HD.star);//英雄升星成功播放晋级特效
                break;
            case MessageID.common_upgrade_hero_grade_ret://英雄进阶
                EquipPanel.instance.OnAdvancedSuccess();
                Control.ShowGUI(UIPanleID.UITooltips, EnumOpenUIType.DefaultUIOrSecond, false, "英雄进阶成功");
                EquipPanel.instance.PlayjinjieEffect();
                break;
        }
    }
    protected override void ShowHandler()
    {
        base.ShowHandler();

        if (!isOne) return;
        // OnDetailsTabClick(HeroAndEquipNodeData.TanNUm, true);
        detailsTab.DefauleIndex = HeroAndEquipNodeData.TanNUm;
        UpGradeState = true;
        if (playerData.GetInstance().GetHeroDataByID(Globe.selectHero.hero_id) != null)
        {
            InsHero(playerData.GetInstance().GetHeroDataByID(Globe.selectHero.hero_id), true);
            UpdateInfo();
        }
        else
        {
            HeroData data = new HeroData(Globe.selectHero.hero_id);
            InsHero(data, true);
            HeroSkill.RefreshSkill(data);
            HeroSkill.gameObject.SetActive(false);
            HeroSkill.gameObject.SetActive(true);
        }


    }
    public void SwitchHero()
    {
        if (!isOne) return;
        // OnDetailsTabClick(HeroAndEquipNodeData.TanNUm, true);
        detailsTab.DefauleIndex = HeroAndEquipNodeData.TanNUm;
        UpGradeState = true;
        if (playerData.GetInstance().GetHeroDataByID(Globe.selectHero.hero_id) != null)
        {
            InsHero(playerData.GetInstance().GetHeroDataByID(Globe.selectHero.hero_id), true);
            UpdateInfo();
        }
        else
        {
            HeroData data = new HeroData(Globe.selectHero.hero_id);
            InsHero(data, true);
            HeroSkill.RefreshSkill(data);
            HeroSkill.gameObject.SetActive(false);
            HeroSkill.gameObject.SetActive(true);
        }
    }
    public long HeroID = 0;//用于比较当前界面是否是相同的英雄，不同则刷新
    /// <summary>
    /// 实例化展示英雄
    /// </summary>
    public void InsHero(HeroData insHero, bool show)
    {

        if (hd != null)
        {
            if (!show)
            {
                return;
            }
            else
            {
                if (hd != null)
                {
                    if (HeroID / 100 == insHero.id / 100)
                        return;
                }
            }

        }
        HeroID = insHero.id;
        hd = insHero;
        if (playerData.GetInstance().GetHeroDataByID(Globe.selectHero.hero_id) != null)
        {
            if (!hd.isGetData)
            {
                playerData.GetInstance().isEquipDevelop = false;
                Globe.isDetails = true;
                ClientSendDataMgr.GetSingle().GetHeroSend().SendGetHeroInfo(hd.id, C2SMessageType.Active);
            }
            else
            {
                HeroData temp = playerData.GetInstance().GetHeroDataByID((Globe.selectHero.hero_id));
                playerData.GetInstance().selectHeroDetail = temp;
                UpdateInfo();
            }

        }
        else
        {
            HeroSkill.RefreshSkill(hd);
            HeroSkill.gameObject.SetActive(false);
            HeroSkill.gameObject.SetActive(true);

        }

        HeroPosEmbattle.instance.CreatModel(insHero.node.icon_name + "_show", PosType.TitlePos, dragSprite, AnimType.Appeared);
    }

    void ShowEffect()
    {
        if (go.name.Contains("yx_001_show"))
        {
            go.transform.Find("Bip001/Bip001 Prop1/weapon").gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 更新数据显示
    /// </summary>
    public void UpdateInfo()
    {
        if (UpGradeStar.gameObject.activeSelf)
            UpGradeStar.RefreshDrug(hd);
        UpGradeStar.RefreshSoulStone(hd);
        heroDes.RefreshDesInfo();
        heroDes.SetDingweiAndTedian();
        NameArea.RefreshUI(hd);
        breakthrough.RefreshUI(hd);
        if (Runes.gameObject.activeSelf)
            Runes.RefreshRunes();
        //hd = playerData.GetInstance().selectHeroDetail;
        //if (EquipPanel.instance)
        if (HeroAndEquipNodeData.TanNUm == 3)
        {
            EquipPanel.instance.ShowEquip(hd);
        }
        if (HeroAndEquipNodeData.TanNUm == 4)
        {
            HeroSkill.RefreshSkill(hd);
        }
        //if (HeroSkill.gameObject.activeSelf)
        WearRunes();
        float length = UI_HeroDetail.hd.node.GetStarGrowUpRate(0, UI_HeroDetail.hd.star);
        strengthGrowthLabel.text = "" + length;
        PowerProgress.width = HeroDescription.Instance().GetProgressLength(length);

        length = UI_HeroDetail.hd.node.GetStarGrowUpRate(1, UI_HeroDetail.hd.star);
        intelligenceGrowthLabel.text = "" + length;
        IntelligenceProgress.width = HeroDescription.Instance().GetProgressLength(length);

        length = UI_HeroDetail.hd.node.GetStarGrowUpRate(2, UI_HeroDetail.hd.star);
        agileGrowthLabel.text = "" + length;

        AgilityProgress.width = HeroDescription.Instance().GetProgressLength(length);
        SetUpgradeProperty();

    }
    public void SetUpgradeProperty()
    {
        UI_HeroDetail.hd.RefreshAttr();
        float[] basepropertys = UI_HeroDetail.hd.charAttrs;
        if (basepropertys != null)
        {
            strength.text = basepropertys[0] + "";
            intelligence.text = basepropertys[1] + "";
            agile.text = basepropertys[2] + "";
        }


        strengthGrowth.text = UI_HeroDetail.hd.node.GetStarGrowUpRate(0, UI_HeroDetail.hd.star) + "";
        agileGrowth.text = UI_HeroDetail.hd.node.GetStarGrowUpRate(2, UI_HeroDetail.hd.star) + "";
        intelligenceGrowth.text = UI_HeroDetail.hd.node.GetStarGrowUpRate(1, UI_HeroDetail.hd.star) + "";
    }
    public void WearRunes()
    {
        heroRunes = playerData.GetInstance().GetHeroDataByID(Globe.selectHero.hero_id).runes;

        for (int i = 0; i < heroRunes.Length; i++)
        {
            runeList[i].GetComponent<ItemRunes>().ChangeRunes(heroRunes[i], i);
        }
    }

    /// <summary>
    /// 升星成功
    /// </summary>
    public void RisingStarSuccess()
    {
        StarUpGradeNode starUpHero = FSDataNodeTable<StarUpGradeNode>.GetSingleton().FindDataByType(hd.star);
        //减去金币,使用的魂石
        playerData.GetInstance().MoneyHadler(MoneyType.Gold, -starUpHero.evolve_cost);
        GoodsDataOperation.GetInstance().UseGoods(hd.node.soul_gem, starUpHero.call_stone_num);

        hd.star++;

        UpGradeStar.RefreshSoulStone(hd);
        NameArea.RefreshUI(hd);
    }

    public void OnUpGradeSuccess()
    {
        if (!UpGradeStar.gameObject.activeSelf)
        {
            UpGradeStar.gameObject.SetActive(true);
            UpGradeStar.RefreshDrug(hd);
            UpGradeStar.gameObject.SetActive(false);
        }
        else
        {
            UpGradeStar.RefreshDrug(hd);
        }

    }

    public void RunesHandler()
    {
        playerData.GetInstance().MoneyHadler(MoneyType.Gold, -runesGlod);
    }

    /// <summary>
    /// 返回按钮
    /// </summary>
    private void OnBackBtnClick()
    {
        HeroID = 0;
        HeroAndEquipNodeData.TanNUm = 0;
        HeroPosEmbattle.instance.HideModel();
        //关闭界面时，发送数据
        if (isSendDrug)
        {

            heroIDD = 0;
            itemIDD = 0;
            countD = 0;
            levelD = 0;

            isSendDrug = false;
        }

        //Control.HideGUI(GameLibrary.UI_HeroDetail);
        //Control.ShowGUI(GameLibrary.UIHeroList);
        Control.HideGUI();
        //切换场景打断声音
        AudioController.Instance.StopUISound();

        //serverMgr.GetInstance().SetCurrentPoint(skillCount);
        //serverMgr.GetInstance().SetFullTime(Auxiliary.GetNowTime() + (20 - skillCount) * 600);
        //serverMgr.GetInstance().saveData();


        if (GameLibrary.skillLevelcount.Count > 0)
        {
            ClientSendDataMgr.GetSingle().GetHeroSkillSend().SendUpgradeMsg(Globe.selectHero.hero_id, GameLibrary.skillLevelcount);
            GameLibrary.skillLevelcount.Clear();
        }
    }

    /// <summary>
    /// 发送使用药品信息
    /// </summary>
    public void SendDrugUpgrade()
    {
        if (isSendDrug)
        {

            Debug.Log(countD + "-" + levelD);

            //ClientSendDataMgr.GetSingle().GetHeroSend().SendDrugUpgrade(heroIDD, itemIDD, countD, levelD);

            heroIDD = 0;
            itemIDD = 0;
            countD = 0;
            levelD = 0;

            isSendDrug = false;
        }
    }

    /// <summary>
    /// 英雄详情标签
    /// </summary>
    /// <param name="index"></param>
    /// <param name="boo"></param>
    private void OnDetailsTabClick(int index, bool boo)
    {
        if (index == 4)
        {
            //英雄的技能开放验证

            if (!DataDefine.isSkipFunction && !FunctionOpenMng.GetInstance().GetFunctionOpen(7))
            {
                string text = FSDataNodeTable<UnLockFunctionNode>.GetSingleton().DataNodeList[7].limit_tip;
                //UIPromptBox.Instance.ShowLabel(text);
                Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, text);
                return;
            }
        }
        if (index == 3)
        {
            //英雄的装备页签开放验证

            if (!DataDefine.isSkipFunction && !FunctionOpenMng.GetInstance().GetFunctionOpen(14))
            {
                string text = FSDataNodeTable<UnLockFunctionNode>.GetSingleton().DataNodeList[14].limit_tip;
                //UIPromptBox.Instance.ShowLabel(text);
                Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, text);
                return;
            }
        }

        if (boo == false) return;

        UpGradeStar.gameObject.SetActive(false);
        Equip.gameObject.SetActive(false);
        heroDes.gameObject.SetActive(false);
        HeroSkill.gameObject.SetActive(false);
        Runes.gameObject.SetActive(false);
        breakthrough.gameObject.SetActive(false);
        switch (index)
        {
            case 0:
                if (playerData.GetInstance().GetHeroDataByID((Globe.selectHero.hero_id)) == null)
                {
                    detailsTab.GetBoxList()[0].transform.Find("Label").GetComponent<UILabel>().text = "技能";
                    SkillListShow();
                    for (int i = 1; i < detailsTab.GetBoxList().Length; i++)
                    {
                        detailsTab.GetBoxList()[i].gameObject.SetActive(false);
                    }
                }
                else
                {
                    HeroAndEquipNodeData.TabType = 0;
                    HeroAndEquipNodeData.TanNUm = 0;
                    NameArea.gameObject.SetActive(true);
                    Blickline.gameObject.SetActive(false);
                    heroDes.ShowOrHidePanel(false);
                    ScrollViewResetPosition();
                    Property.SetActive(true);
                    upGradeProperty.SetActive(false);
                    Property.transform.localPosition = new Vector3(408, -110, 0);
                    detailsTab.GetBoxList()[0].transform.Find("Label").GetComponent<UILabel>().text = "属性";
                    for (int i = 0; i < detailsTab.GetBoxList().Length; i++)
                    {
                        detailsTab.GetBoxList()[i].gameObject.SetActive(true);
                    }
                }


                //HeroDescription.Instance().OnAttributesBtn();
                //Control.ShowGUI(GameLibrary.UI_HeroDetail);
                //SkillListShow();


                break;
            case 1:
                HeroAndEquipNodeData.TabType = 0;
                HeroAndEquipNodeData.TanNUm = 1;
                NameArea.gameObject.SetActive(true);
                Blickline.gameObject.SetActive(true);
                UpGradeStar.gameObject.SetActive(true);
                if (UpGradeState)
                {
                    UpdateInfo();
                    UpGradeState = false;
                }
                ScrollViewResetPosition();
                Property.SetActive(false);
                upGradeProperty.SetActive(true);
                Property.transform.localPosition = new Vector3(408, 70, 0);
                break;
            case 2:
                HeroAndEquipNodeData.TabType = 0;
                HeroAndEquipNodeData.TanNUm = 2;
                NameArea.gameObject.SetActive(true);
                breakthrough.gameObject.SetActive(true);
                Blickline.gameObject.SetActive(false);
                Property.SetActive(true);
                upGradeProperty.SetActive(false);
                Property.transform.localPosition = new Vector3(408, 56, 0);
                ScrollViewResetPosition();
                break;
            case 3:
                //装备
                HeroAndEquipNodeData.TabType = 0;
                HeroAndEquipNodeData.TanNUm = 3;
                BtnState = true;
                AdvancedBtn.gameObject.SetActive(true);
                HeroPreview.gameObject.SetActive(true);
                EquipIntensify.gameObject.SetActive(false);
                NameArea.gameObject.SetActive(false);
                Blickline.gameObject.SetActive(false);
                Equip.gameObject.SetActive(true);
                Property.SetActive(false);
                upGradeProperty.SetActive(false);

                EquipPanel.instance.ShowEquip(hd);
                ScrollViewResetPosition();
                break;
            case 4:
                //    HeroAndEquipNodeData.TabType = 0;
                //    HeroAndEquipNodeData.TanNUm = 4;
                //    BtnState = false;
                //    AdvancedBtn.gameObject.SetActive(false);
                //    HeroPreview.gameObject.SetActive(false);
                //    EquipIntensify.gameObject.SetActive(true);
                //    NameArea.gameObject.SetActive(false);
                //    Blickline.gameObject.SetActive(false);
                //    Equip.gameObject.SetActive(true);
                //    EquipPanel.instance.ShowEquip(hd);
                //    ScrollViewResetPosition();
                //    break;
                //case 5:
                SkillListShow();
                break;
            default:
                break;
        }
    }

    public void SkillListShow()
    {
        HeroAndEquipNodeData.TabType = 0;
        HeroAndEquipNodeData.TanNUm = 4;
        NameArea.gameObject.SetActive(false);
        Blickline.gameObject.SetActive(true);
        Property.SetActive(false);
        upGradeProperty.SetActive(false);//隐藏升级属性
        HeroSkill.RefreshSkill(hd);
        HeroSkill.gameObject.SetActive(true);
        ScrollViewResetPosition();
    }
    public string SetBorderColor(int grade)
    {
        if (grade >= 16)
            return "hong";
        else if (grade >= 11)
            return "cheng";
        else if (grade >= 7)
            return "zi";
        else if (grade >= 4)
            return "lan";
        else if (grade >= 2)
            return "lv";
        else if (grade == 1)
            return "bai";
        else
            return "hui";
    }
    public void UpGrade(float start, float interval, ItemNodeState itemNode, PlayerLevelUpNode PlayerNode)
    {
        countss = currentEXP;
        exp = itemNode.exp_gain;
        expDiffe = Convert.ToInt32(itemNode.exp_gain / 25);
        playerNode = PlayerNode;
        InvokeRepeating("OnSecondFunction", start, interval);
    }
    /// <summary>
    /// 经验条滚动
    /// </summary>
    void OnSecondFunction()
    {

        exp -= 10;
        counts++;

        UI_HeroDetail.instance.NameArea.ChangeExpBar(1f * counts / hd.maxExps, Mathf.CeilToInt((1f * counts / hd.maxExps) * hd.maxExps), hd.maxExps, hd.lvl);

        if ((1f * counts / hd.maxExps) >= 1f)
        {
            counts = 0;

            hd.lvl++;
            hd.RefreshAttr();
            //hd.fc = HeroData.GetFC(hd);

            upGradelvl++;

            hd.maxExps = VOManager.Instance().GetCSV<HeroUpgradeCSV>("HeroUpgrade").GetVO(hd.lvl).exp;

            UI_HeroDetail.instance.NameArea.ChangeExpBar(0, exp, hd.maxExps, hd.lvl);

            if (hd.lvl >= playerNode.heroLvLimit)
            {
                ItemBottlList.Instance().StopEatDrug();
                hd.exps = 0;
                UI_HeroDetail.instance.NameArea.ChangeExpBar(0, hd.exps, hd.maxExps, hd.lvl);
                CancelInvoke("EatDrug");
                CancelInvoke("OnSecondFunction");
            }
        }

        if (exp < 1)
        {
            hd.exps = counts;
            CancelInvoke("OnSecondFunction");
            ItemBottlList.Instance().StopEatDrug();
        }

    }

    /// <summary>
    /// ScrillView位置还原
    /// </summary>
    public void ScrollViewResetPosition()
    {
        EquipIntensifys.ResetPosition();
        EquipEvolveView.ResetPosition();
        HeroPreviewlist.ResetPosition();
        HeroPreviewLists.ResetPosition();
    }

    //protected override void RegisterComponent()
    //{
    //    base.RegisterComponent();

    //    RegisterComponentID(31, 94, EquipTab);

    //    if (playerData.GetInstance().guideData.uId == 3194)
    //    {
    //        if (UIGuidePanel.Single() != null)
    //            UIGuidePanel.Single().InitGuide();
    //    }

    //}
}

