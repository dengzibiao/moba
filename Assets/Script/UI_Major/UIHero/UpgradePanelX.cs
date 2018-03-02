using UnityEngine;
using System.Collections;
using System;
using Tianyu;

/// <summary>
/// 药水使用面板
/// </summary>
public class UpgradePanelX : GUIBase
{

    #region

    public static UpgradePanelX instance;

    public GUISingleSprite bottlIcon;
    public GUISingleSprite bottlBorder;
    public GUISingleLabel bottlCount;
    public GUISingleLabel bottlName;
    public GUISingleLabel bottlEXP;
    public GUISingleSprite heroIcon;
    public GUISingleSprite heroBorder;
    public GUISingleLabel heroName;
    public GUISingleLabel lvF;
    public GUISingleLabel lvR;
    public UISlider expBar;
    public UIPanel mask;
    public GUISingleButton upgradeBtn;
    public UIButton aKeyUpgradeBtn;
    public UISprite backBtn;



    UI_HeroDetail hd_ins;

    HeroData hd;

    PlayerLevelUpNode playerNode;

    public ItemNodeState item;

    public HeroNode hero;

    #endregion

    public UpgradePanelX()
    {
        instance = this;
    }

    public override UIPanleID GetUIKey()
    {
        return UIPanleID.none;
    }


    protected override void Init()
    {
        base.Init();

        mask.gameObject.SetActive(false);

        //upgradeBtn.onClick = OnUpgradeBtnClick;

        UIEventListener.Get(aKeyUpgradeBtn.gameObject).onClick = OnAKeyUpgradeBtnClick;

        //UIEventListener.Get(backBtn.gameObject).onClick = OnBackBtnClick;

        hd_ins = UI_HeroDetail.instance;

        playerNode = FSDataNodeTable<PlayerLevelUpNode>.GetSingleton().FindDataByType(playerData.GetInstance().selfData.level);

    }

    /// <summary>
    /// 显示信息
    /// </summary>
    public void RefreshUI(ItemNodeState itemNode, HeroData hd)
    {

        if (null == UI_HeroDetail.instance) return;
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        item = itemNode;
        hero = hd.node;
        this.hd = hd;

        bottlIcon.spriteName = item.icon_name;

        //currentDrug = playerData.GetInstance().GetItemCountById(item.props_id);

        //if (currentDrug == 0) bottlCount.text = "";
        //else bottlCount.text = currentDrug + "";

        bottlName.text = item.name;
        bottlEXP.text = "+" + item.exp_gain;

        ShowHeroInfo(hero);
    }

    /// <summary>
    /// 显示人物信息
    /// </summary>
    /// <param name="hero"></param>
    public void ShowHeroInfo(HeroNode hero)
    {

        if (null == heroIcon) return;

        this.hero = hero;

        heroIcon.spriteName = hero.icon_name;
        heroName.text = hero.name;
        hd = playerData.GetInstance().GetHeroDataByID(hero.hero_id);

        lvF.text = "Lv." + hd.lvl;
        lvR.text = "Lv." + (hd.lvl + 1);

        if (hd.exps <= 0)
        {
            expBar.value = 0;
        }
        else
        {
            expBar.value = hd.exps / (float)hd.maxExps;
        }

        expBar.transform.Find("Label").GetComponent<UILabel>().text = hd.exps + "/" + hd.maxExps;

    }




    /// <summary>
    /// 一键升级
    /// </summary>
    private void OnAKeyUpgradeBtnClick(GameObject go)
    {


        //consumeCount = currentDrug;

        if (hd.lvl < playerNode.heroLvLimit)
        {

            //hd_ins.isSendDrug = true;

            hd_ins.itemIDD = item.props_id;
            hd_ins.heroIDD = hero.hero_id;

            //AUpgrade();

            //bottlCount.text = currentDrug.ToString();

            lvF.text = "Lv." + hd.lvl;
            lvR.text = "Lv." + (hd.lvl + 1);

            if (hd.lvl >= playerNode.heroLvLimit)
            {
                hd.exps = 0;
            }

            expBar.value = hd.exps / (float)hd.maxExps;
            expBar.transform.Find("Label").GetComponent<UILabel>().text = hd.exps + "/" + hd.maxExps;

            //hd_ins.ExpBarChange(hd.exps, hd.maxExps, hd.lvl);
            hd_ins.NameArea.ChangeExpBar(expBar.value, hd.exps, hd.maxExps, hd.lvl);

            ////记录使用的药品和个数
            //if (UI_HeroDetail.instance.conDrugDic.ContainsKey(item.props_id))
            //{
            //    UI_HeroDetail.instance.conDrugDic.Remove(item.props_id);
            //}

            //UI_HeroDetail.instance.conDrugDic.Add(item.props_id, useCount);

        }
        else
        {
            PromptPanel.instance.ShowPrompt("请提升玩家等级等级");
        }

    }

    /// <summary>
    /// 一键递归
    /// </summary>
    //void AUpgrade()
    //{

    //    if (currentDrug <= 0)
    //    {
    //        print("药水空");
    //        return;
    //    }

    //    currentDrug--;
    //    GoodsDataOperation.GetInstance().UseGoods((int)item.props_id, 1);
    //    UI_HeroDetail.instance.UpGradeStar.RefreshDrug(hd);

    //    if (currentDrug <= 0) bottlCount.text = "";
    //    else bottlCount.text = currentDrug.ToString();

    //    hd_ins.countD++;

    //    useCount++;

    //    hd.exps += item.exp_gain;

    //    ARecursion();

    //    if (hd.lvl >= playerNode.heroLvLimit)
    //    {
    //        print("请提升玩家等级");
    //        return;
    //    }

    //    AUpgrade();

    //}

    /// <summary>
    /// 循环吃药
    /// </summary>
    void ARecursion()
    {
        while (hd.exps >= hd.maxExps)
        {
            hd.exps -= hd.maxExps;
            hd.lvl++;
            hd.RefreshAttr();
            //hd.fc = HeroData.GetFC(hd);
            hd.maxExps = FSDataNodeTable<HeroUpGradeNode>.GetSingleton().FindDataByType(hd.lvl).exp;
        }
    }

    /// <summary>
    /// 切换英雄更换数据信息
    /// </summary>
    public void UpdateData()
    {

        if (UI_HeroDetail.instance == null) return;

        heroIcon.spriteName = Globe.selectHero.icon_name;
        heroName.text = Globe.selectHero.name;

        print(UI_HeroDetail.instance);

        //expBar.state = ProgressState.STRING;
        //expBar.InValue(UI_HeroDetail.instance.currentEXP, 1000);

    }



    /// <summary>
    /// 返回按钮
    /// </summary>
    //private void OnBackBtnClick(GameObject go)
    //{

    //    if (currentDrug <= 0)
    //    {
    //        //if (playerData.GetInstance().drugDic.ContainsKey(item.props_id))
    //        //{
    //        //    playerData.GetInstance().drugDic.Remove(item.props_id);
    //        //}
    //        //playerData.GetInstance().drugKeys.Remove(item.props_id);
    //    }
    //    else
    //    {
    //        //if (playerData.GetInstance().drugDic.ContainsKey(item.props_id))
    //        //{
    //        //    playerData.GetInstance().drugDic.Remove(item.props_id);
    //        //}

    //        //playerData.GetInstance().drugDic.Add(item.props_id, currentDrug);
    //    }

    //    if (isUse)
    //    {
    //        ClientSendDataMgr.GetSingle().GetHeroSend().SendDrugUpgrade(Globe.selectHero.hero_id, item.props_id, useCount, upGradelvl);
    //        useCount = 0;
    //        upGradelvl = 0;
    //        isUse = false;
    //    }


    //    Control.HideGUI(GameLibrary.UIUpgradePanel);
    //}

}