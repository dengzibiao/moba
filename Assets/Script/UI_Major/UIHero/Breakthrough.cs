using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tianyu;


public class Breakthrough : GUIBase
{
    HeroData hd;
    public UISprite HeroImg;
    public UISprite icon;
    public UIButton RisingStarBtn;
    public UISlider jingyancao;
    public UISprite[] StarList;
    public UILabel ExpLbl;

    //public UILabel strengthGrowthLabel;
    //public UILabel intelligenceGrowthLabel;
    //public UILabel agileGrowthLabel;
    //public UISprite PowerProgress;
    //public UISprite IntelligenceProgress;
    //public UISprite AgilityProgress;
    private Transform shengXingEffect;
    public Transform[] xingxingEffect;
    ItemData soulItem;
    int currentSoul = 0;
    int soul_gem = 0;
    int userGlod = 0;
    StarUpGradeNode starUpHero;
    public UILabel Gold;
    public static Breakthrough instance;
    public Breakthrough()
    {
        instance = this;
    }

    public override UIPanleID GetUIKey()
    {
        return UIPanleID.none;
    }

    /// <summary>
    /// 初始化
    /// </summary>
    protected override void Init()
    {
        //PowerProgress.width = 0;
        //PowerProgress.pivot = UIWidget.Pivot.Left;
        //IntelligenceProgress.width = 0;
        //IntelligenceProgress.pivot = UIWidget.Pivot.Left;
        //AgilityProgress.width = 0;
        //AgilityProgress.pivot = UIWidget.Pivot.Left;
        EventDelegate.Set(RisingStarBtn.onClick, OnRisingStarBtnClick);
        shengXingEffect = transform.Find("UI_YXShengXing_01");
    }
    protected override void ShowHandler()
    {
        //strengthGrowthLabel.text = "" + UI_HeroDetail.hd.node.GetStarGrowUpRate(0, UI_HeroDetail.hd.star);
        //PowerProgress.width = 60;
        //intelligenceGrowthLabel.text = "" + UI_HeroDetail.hd.node.GetStarGrowUpRate(1, UI_HeroDetail.hd.star);
        //IntelligenceProgress.width = HeroDescription.Instance().GetProgressLength(UI_HeroDetail.hd.node.GetStarGrowUpRate(1, UI_HeroDetail.hd.star));
        //agileGrowthLabel.text = "" + UI_HeroDetail.hd.node.GetStarGrowUpRate(2, UI_HeroDetail.hd.star);
        //AgilityProgress.width = HeroDescription.Instance().GetProgressLength(UI_HeroDetail.hd.node.GetStarGrowUpRate(2, UI_HeroDetail.hd.star));

    }
    public void RefreshUI(HeroData hd)
    {
        this.hd = hd;
        soulItem = playerData.GetInstance().GetItemDatatByID(hd.node.soul_gem);
        HeroImg.spriteName = hd.node.icon_name;
        icon.spriteName = GoodsDataOperation.GetInstance().GetSmallHeroGrameByHeroGrade(hd.grade);
        if (null == soulItem)
        {
            currentSoul = 0;
        }
        else
        {
            currentSoul = soulItem.Count;
        }
        for (int i = 0; i < StarList.Length; i++)
        {
            StarList[i].spriteName = i < hd.star ? "xing" : "xing-hui";
        }
        if (hd.star < 5)
        {
            int star = hd.star;
            RisingStarBtn.gameObject.SetActive(true);
            Gold.gameObject.SetActive(true);
            starUpHero = FSDataNodeTable<StarUpGradeNode>.GetSingleton().FindDataByType(++star);
            //更换英雄魂石图标 Gold 魂石条

            Gold.text = "" + starUpHero.evolve_cost;

            //魂石条赋值
            jingyancao.value = currentSoul / (float)starUpHero.evolve_stone_num;
            jingyancao.transform.Find("Label").GetComponent<UILabel>().text = currentSoul + "/" + starUpHero.evolve_stone_num;
        }
        else
        {
            Gold.text = "";
            jingyancao.value = 1 / 1;
            //jingyancao.transform.Find("Label").GetComponent<UILabel>().text = currentSoul + " 英雄星级已达到满级";
            jingyancao.transform.Find("Label").GetComponent<UILabel>().text = " 英雄星级已达到满级";
            RisingStarBtn.gameObject.SetActive(false);
            Gold.gameObject.SetActive(false);
        }
        if (shengXingEffect!=null)
        {
            if (shengXingEffect.gameObject.activeSelf)
            {
                shengXingEffect.gameObject.SetActive(false);
            }
        }
        for (int i = 0; i < xingxingEffect.Length; i++)
        {
            xingxingEffect[i].gameObject.SetActive(false);
        }
    }
    public void PlayerShengXingEffect(int from,int to)
    {
        shengXingEffect.gameObject.SetActive(true);
        if (xingxingEffect.Length >= to)
        {
            xingxingEffect[to - 1].gameObject.SetActive(true);
        }
        StartCoroutine(HideEffect());
    }
    IEnumerator HideEffect()
    {
        yield return new WaitForSeconds(1f);
        shengXingEffect.gameObject.SetActive(false);
        for (int i=0;i<xingxingEffect.Length;i++)
        {
            xingxingEffect[i].gameObject.SetActive(false);
        }
    }
    private void OnRisingStarBtnClick()
    {

        if (hd.star >= 5)
        {

            starUpHero = FSDataNodeTable<StarUpGradeNode>.GetSingleton().FindDataByType(hd.star);
            Gold.text = "" + starUpHero.evolve_cost;

            //获取物品
            //soulItem = playerData.GetInstance().GetItemDatatByID(Globe.selectHero.soul_gem);

            if (null == soulItem)
            {
                currentSoul = 0;
            }
            else
            {
                currentSoul = soulItem.Count;
            }

            jingyancao.value = 1;
            jingyancao.transform.Find("Label").GetComponent<UILabel>().text = currentSoul + "/" + starUpHero.evolve_stone_num;

            return;
        }

        //金币
        if (playerData.GetInstance().baginfo.gold < starUpHero.evolve_cost)
        {
            Control.ShowGUI(UIPanleID.UIGoldHand, EnumOpenUIType.DefaultUIOrSecond);
            return;
        }

        if (currentSoul >= starUpHero.evolve_stone_num)
        {
            soul_gem = starUpHero.evolve_stone_num;
            userGlod = starUpHero.evolve_cost;

            int star = hd.star;
            HeroAndEquipNodeData.HD = hd;
            ClientSendDataMgr.GetSingle().GetHeroSend().SendUpgradeHeroStar(Globe.selectHero.hero_id, star+1, Globe.selectHero.soul_gem, starUpHero.evolve_stone_num);
        }
        else
        {
            //UIGoodsGetWayPanel.Instance.SetID(hd.node.soul_gem);
            //if (UILevel.instance.GetLevelData())
            //{
            //    Control.ShowGUI(GameLibrary.UIGoodsGetWayPanel);
            //}
            Control.ShowGUI(UIPanleID.UIGoodsGetWayPanel, EnumOpenUIType.DefaultUIOrSecond, false, (long)hd.node.soul_gem);
            return;
        }

    }
}
