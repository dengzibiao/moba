/*
文件名（File Name）:   UILottryHeroEffect.cs

作者（Author）:    #高#

创建时间（CreateTime）:  #CreateTime#
*/
using UnityEngine;
using System.Collections;
using Tianyu;

public class UILottryHeroEffect : GUIBase
{
    public static UILottryHeroEffect instance;
    public GUISingleButton spriteBtn;//全屏按钮
    public GUISingleSpriteGroup star;
    public GUISingleLabel nameLabel;
    public GUISingleLabel des;
    public GUISingleSprite heroType;
    private string mName;
    private ItemNodeState vo;
    private int mStar;
    private long mID;
    private int mIndex;
    private int mCount;
    private int mAttribute;
    private HeroOrSoul mTp;
    private string mDes;
    private GameObject modle = null;
    private ShowHeroEffectType mType;
    protected override void Init()
    {
        base.Init();
        spriteBtn.onClick = SpriteBtn;
    }

    public UILottryHeroEffect()
    {
        instance = this;
    }
    public override UIPanleID GetUIKey()
    {
        return UIPanleID.UILottryHeroEffect;
    }
    protected override void ShowHandler()
    {
        base.ShowHandler();
        if (mName != null && mStar != 0)
        {
            nameLabel.text = mName;
            star.IsShow(mStar);
            star.transform.localPosition = GetStarPos();
            heroType.spriteName = GetSpiteName(mAttribute);
            if (mTp == HeroOrSoul.Soul && mDes != null)
            {
                des.text = mDes;
            }
            else if (mTp == HeroOrSoul.Hero)
            {
                des.text = mDes;
            }
        }
        StartCoroutine(IsShow());
    }

    private IEnumerator IsShow()
    {
        yield return new WaitForSeconds(1f);
        isShow = true;
    }


    private string GetSpiteName(int attribute)
    {
        switch (attribute)
        {
            case 1: return "li";
            case 2: return "zhi";
            case 3: return "min";
        }
        return "";
    }

    private Vector3 GetStarPos()
    {
        switch (mStar)
        {
            case 1:
                return new Vector3(-7.1f, star.transform.localPosition.y, 0);
            case 2:
                return new Vector3(-40.83f, star.transform.localPosition.y, 0);
            case 3:
                return new Vector3(-63.2f, star.transform.localPosition.y, 0);
            case 4:
                return new Vector3(-83.9f, star.transform.localPosition.y, 0);
        }
        return star.transform.localPosition;
    }

    protected override void SetUI(params object[] uiParams)
    {
        if (uiParams[0] != null)
            mID = long.Parse(uiParams[0].ToString());
        if (uiParams[1] != null)
            mType = (ShowHeroEffectType)uiParams[1];
        if (uiParams[2] != null)
            mTp = (HeroOrSoul)uiParams[2];
        if (uiParams[3] != null)
            mIndex = int.Parse(uiParams[3].ToString());
        base.SetUI(uiParams);
    }

    protected override void OnLoadData()
    {
        if (mTp != HeroOrSoul.None) GetInfo(mTp);
        base.OnLoadData();
    }

    public void InitUI(long id, ShowHeroEffectType type, HeroOrSoul tp, int index)
    {
        mType = type;
        mID = id;
        mIndex = index;
        mTp = tp;


    }

    private void GetInfo(HeroOrSoul typ)
    {
        if (FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList.ContainsKey(mID))
        {
            string iconName = FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList[mID].icon_name;
            mStar = FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList[mID].init_star;
            mName = FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList[mID].name;
            mCount = FSDataNodeTable<StarUpGradeNode>.GetSingleton().DataNodeList[mStar].convert_stone_num;
            mAttribute = FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList[mID].attribute;
            switch (mType)
            {
                case ShowHeroEffectType.Lottry:
                    switch (typ)
                    {
                        case HeroOrSoul.Hero:
                            playerData.GetInstance().RefreshHeroToList(mID, mStar, 0, 0); GetHero(mName); UIResultLottery.instance.IsShowAnima(mIndex);
                            Control.HideGUI(UIPanleID.UIMoney);
                            HeroPosEmbattle.instance.ShowLottryAnimaEffect(PosType.heroEffect, LotteryType.None, 0, CostType.None);
                            modle= HeroPosEmbattle.instance.CreatModel(iconName, PosType.heroEffect, null, AnimType.Run, 145, 0.36f);
                            Show(); break;
                        case HeroOrSoul.Soul:
                            GetSoul(); UIResultLottery.instance.IsShowAnima(mIndex);
                            Control.HideGUI(UIPanleID.UIMoney);
                            HeroPosEmbattle.instance.ShowLottryAnimaEffect(PosType.heroEffect, LotteryType.None, 0, CostType.None);
                            modle= HeroPosEmbattle.instance.CreatModel(iconName, PosType.heroEffect, null, AnimType.Run, 145, 0.36f);
                            Show(); break;
                    }
                    break;
                case ShowHeroEffectType.HeroList:
                    HeroPosEmbattle.instance.ShowLottryAnimaEffect(PosType.heroEffect, LotteryType.None, 0, CostType.None);
                    HeroPosEmbattle.instance.CreatModel(iconName, PosType.heroEffect, null, AnimType.Run, 145, 0.36f);
                    GetHero(mName);
                    Show();
                    break;
                case ShowHeroEffectType.NewPlayerRewards:
                    playerData.GetInstance().RefreshHeroToList(mID, mStar, 0, 0);
                    HeroPosEmbattle.instance.ShowLottryAnimaEffect(PosType.heroEffect, LotteryType.None, 0, CostType.None);
                    HeroPosEmbattle.instance.CreatModel(iconName, PosType.heroEffect, null, AnimType.Run, 145, 0.36f);
                    GetHero(mName);
                    Show();
                    break;
                case ShowHeroEffectType.StarEvaluationChest:
                    HeroPosEmbattle.instance.ShowLottryAnimaEffect(PosType.heroEffect, LotteryType.None, 0, CostType.None);
                    HeroPosEmbattle.instance.CreatModel(iconName, PosType.heroEffect, null, AnimType.Run, 145, 0.36f);
                    switch (typ)
                    {
                        case HeroOrSoul.Hero:
                            GetHero(mName); break;
                        case HeroOrSoul.Soul:
                            GetSoul(); break;
                    }
                    Show();
                    break;
            }
        }
    }
    private bool isShow = false;
    private void SpriteBtn()
    {
        if (isShow)
        {
            switch (mType)
            {
                case ShowHeroEffectType.Lottry:
                    HeroPosEmbattle.instance.LottryHeroEffectHandle();
                    switch (mTp)
                    {
                        case HeroOrSoul.Hero: modle.SetActive(false); modle.SetActive(false); UIResultLottery.instance.ContinuePlay(); break;
                        case HeroOrSoul.Soul:
                            modle.SetActive(false); UIResultLottery.instance.IsShowDebris(mIndex); break;
                    }
                    break;
                case ShowHeroEffectType.HeroList: HeroPosEmbattle.instance.LottryHeroEffectHandle(); HeroPosEmbattle.instance.HideLottryAnimaEffect(); break;
                case ShowHeroEffectType.StarEvaluationChest:
                    HeroPosEmbattle.instance.LottryHeroEffectHandle();
                    HeroPosEmbattle.instance.HideLottryAnimaEffect();
                    HeroPosEmbattle.instance.HideModel(PosType.DetailPos);
                    break;
                case ShowHeroEffectType.NewPlayerRewards:
                    HeroPosEmbattle.instance.LottryHeroEffectHandle();
                    HeroPosEmbattle.instance.HideLottryAnimaEffect();
                    HeroPosEmbattle.instance.HideModel(PosType.DetailPos);
                    break;
            }
            if (mType != ShowHeroEffectType.StarEvaluationChest)
                Control.ShowGUI(UIPanleID.UIMoney, EnumOpenUIType.DefaultUIOrSecond);
            Control.HideGUI(this.GetUIKey());
            isShow = false;
        }
    }

    protected override void OnRelease()
    {
        base.OnRelease();
        mStar = 0;
        mID = 0;
        mIndex = 0;
        mCount = 0;
        mAttribute = 0;
        mTp = HeroOrSoul.None;
        mDes = null;
        mType = ShowHeroEffectType.None;
        modle = null;
    }

    void GetHero(string mName)
    {
        mDes = "恭喜您获得[9c35fe]" + mName + "[-]英雄";
    }

    void GetSoul()
    {
        int id = int.Parse(106 + StringUtil.SubString(mID.ToString(), 6, 3));
        vo = GameLibrary.Instance().ItemStateList[id];
        mDes = "您已经拥有此英雄,故赠送您[2dd740]" + mCount + "个[-][9c35fe]" + vo.name;
    }
}
