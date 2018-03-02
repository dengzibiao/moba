using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tianyu;

public enum HeroOrSoul : int
{
    None = 0,
    Hero,//英雄
    Soul//灵魂石
}
public class UILotteryItem : GUISingleItemList
{
    public GUISingleSprite icon;
    public GUISingleSprite point;
    public GUISingleLabel count;
    public GUISingleLabel nameTxt;
    private object item;
    public UISprite effect;
    private ItemData voo;
    private string _iconName;//作为当前item出现未有英雄的标记
    private long iconID = -1;//作为出现已有英雄换随便的标记
    private int mStar;
    public UIGrid grid;
    public List<GameObject> star = new List<GameObject>();
    Transform ziEffect;
    Transform chengEffect;
    protected override void InitItem()
    {
        ////初始化
        effect = transform.Find("Effect").GetComponent<UISprite>();
        ziEffect = transform.Find("UI_HDJL_01");
        chengEffect = transform.Find("UI_HDJL_02");
        ziEffect.gameObject.SetActive(false);
        chengEffect.gameObject.SetActive(false);
    }
    protected override void ShowHandler()
    {
        base.ShowHandler();
        point.gameObject.SetActive(false);
        count.text = voo.Count.ToString();
        if ((int.Parse(StringUtil.SubString(voo.Id.ToString(), 3)) == 107))
        {
            count.text = "";
            int heroId = int.Parse(201 + StringUtil.SubString(voo.Id.ToString(), 6, 3));
            iconID = heroId;
            if (FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList.ContainsKey(heroId))
            {
                mStar = FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList[heroId].init_star;
                for (int i = 0; i < mStar; i++)
                {
                    star[i].SetActive(true);
                }
                for (int i = mStar; i < star.Count; i++)
                {
                    star[i].SetActive(false);
                }
                grid.Reposition();
            }
            if (playerData.GetInstance().AddHeroToList(voo.Id))
            {
                object[] ob = new object[4] { iconID, ShowHeroEffectType.Lottry, HeroOrSoul.Hero, index + 1 };
                Control.ShowGUI(UIPanleID.UILottryHeroEffect, EnumOpenUIType.DefaultUIOrSecond, false, ob);
            }
            else
            {
                object[] ob = new object[4] { iconID, ShowHeroEffectType.Lottry, HeroOrSoul.Soul, index + 1 };
                Control.ShowGUI(UIPanleID.UILottryHeroEffect, EnumOpenUIType.DefaultUIOrSecond, false, ob);
            }

        }
        if ((int.Parse(StringUtil.SubString(voo.Id.ToString(), 3)) == 106))
        {
            point.spriteName = "linghunshi";
            point.gameObject.SetActive(true);
        }
        if ((int.Parse(StringUtil.SubString(voo.Id.ToString(), 3)) == 103))
        {
            point.spriteName = "materialdebris";
            point.gameObject.SetActive(true);
        }
        nameTxt.text = voo.Name;

        icon.uiAtlas = voo.UiAtlas;
        icon.spriteName = voo.IconName;
        switch (voo.GradeTYPE)
        {
            case GradeType.Gray:
                A_Sprite.spriteName = "hui";
                effect.spriteName = "";
                break;
            case GradeType.Green:
                A_Sprite.spriteName = "lv";
                effect.spriteName = "";
                break;
            case GradeType.Blue:
                A_Sprite.spriteName = "lan";
                effect.spriteName = "";
                break;
            case GradeType.Purple:
                A_Sprite.spriteName = "zi";
                ziEffect.gameObject.SetActive(true);
                break;
            case GradeType.Orange:
                A_Sprite.spriteName = "cheng";
                chengEffect.gameObject.SetActive(true);
                break;
            case GradeType.Red:
                A_Sprite.spriteName = "hong";
                break;
        }
        if (iconID == -1)
        {
            UIResultLottery.instance.ShowItemHandle();
            Invoke("RevertIsShow", 0.5f);
        }
        else
        {
            iconID = -1;
            mStar = -1;
            Invoke("RevertIsShow", 1);
        }


    }

    private void RevertIsShow()
    {
        if (index == GameLibrary.lotteryCount - 1)
        {
            GameLibrary.lotteryCount = 0;
            GameLibrary.isShow = false;
        }
    }
    public override void Info(object obj)
    {

        //ID 道具表
        // object _item = obj;



        if (obj == null)
        {
            icon.spriteName = "";
            count.text = "";
            nameTxt.text = "";
            effect.spriteName = "";
        }
        else
        {
            voo = (ItemData)obj;
            this.gameObject.SetActive(false);
            if (index + 1 == GameLibrary.lotteryCount)
            {
                UIResultLottery.instance.ShowItemHandle();
            }
        }

    }
}
