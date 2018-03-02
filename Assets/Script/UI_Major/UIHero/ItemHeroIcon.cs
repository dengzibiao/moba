using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemHeroIcon : MonoBehaviour
{

    public UISprite Border;
    public UIButton iconBtn;

    HeroNode heroNode;
    HeroData heroData;

    UI_HeroDetail ui_HeroDetail;

    
    void Start()
    {
        EventDelegate.Set(iconBtn.onClick, OnIconBtnClick);

        ui_HeroDetail = UI_HeroDetail.instance;
    }

    public void RefreshInfo(HeroData hd)
    {

        heroData = hd;
        heroNode = heroData.node;

        if (hd == null)
        {
            
        }
        else
        {
            iconBtn.normalSprite = heroData.node.icon_name;
            Border.spriteName = UI_HeroDetail.instance.SetBorderColor(heroData.grade);
        }

    }

    /// <summary>
    /// 点击英雄头像
    /// </summary>
    private void OnIconBtnClick()
    {

        UI_HeroDetail.instance.SendDrugUpgrade();
        //UI_HeroDetail.instance.SetSelectIndex(heroNode);

        GameLibrary.isRefresh = true;

        UIHeroList.instance.isUpdateHero = true;

        if (GameLibrary.skillLevelcount.Count > 0)
        {
            ClientSendDataMgr.GetSingle().GetHeroSkillSend().SendUpgradeMsg(Globe.selectHero.hero_id, GameLibrary.skillLevelcount);
            GameLibrary.skillLevelcount.Clear();
        }

        Globe.selectHero = heroNode;
        
        ui_HeroDetail.InsHero(heroData,false);

        if (UpgradePanelX.instance.gameObject.activeSelf)
        {
            UpgradePanelX.instance.ShowHeroInfo(heroNode);
        }

    }
}