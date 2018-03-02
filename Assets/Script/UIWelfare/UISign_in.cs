using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tianyu;
using System;

public class UISign_inData
{

    private static UISign_inData mSingleton;
    public static UISign_inData Instance()
    {
        if (mSingleton == null)
            mSingleton = new UISign_inData();
        return mSingleton;
    }
    public bool GetRedPoint;
    public long DoodsID;
    public int DoodsNum;
    public long[] GoodsNumList;
    public int _index = 0;
    public ItemNodeState ItemNode;
    public bool goodsType;
    public long ID;

}
public enum AccumulateNum
{
    ThreeDay = 3,
    SevenDay = 7,
    FifteenDay = 15,
    EightDay = 28
}
/// <summary>
/// 签到
/// </summary>
public class UISign_in : GUIBase
{
    public GUISingleButton accumulateSign_ThreeDay;
    public GUISingleButton accumulateSign_SevenDay;
    public GUISingleButton accumulateSign_FifteenDay;
    public GUISingleButton accumulateSign_Twenty_EightDay;
    public UISprite ThreeDayOK;
    public UISprite SevenDayOK;
    public UISprite FifteenDayOK;
    public UISprite EightDayOK;

    public UISprite Sprite;
    public UISprite Sprite1;
    public UISprite Sprite2;
    public UISprite Sprite3;

    public UISprite ThreeDay;
    public UISprite SevenDay;
    public UISprite FifteenDay;
    public UISprite eightDay;

    public GUISingleLabel sign_inReplenishMax;//根据当前VIP显示当前最大的补签次数
    public GUISingleLabel sign_inReplenishNum;
    public GUISingleLabel memberLevle;//vip等级
    public GUISingleButton sign_inReplenishBtn;
    public static GUISingleMultList goodsMultList;
    public static GUISingleSprite progressbar;
    public static GUISingleLabel accumulateSign_inNum;
    public UILabel Title;
    public static List<UISign_inNode> itemRankList = new List<UISign_inNode>();

    public UITexture Hero;
    public GUISingleLabel heroName;
    public GUISingleSpriteGroup star;
    public UILabel[] num;
    public GUISingleSprite heroType;
    private Vector3 heroShowPos = new Vector3(450, -193, -300);//-363
    /// <summary>
    /// 单例
    /// </summary>
    private static UISign_in mSingleton;
    public static UISign_in Instance()
    {
        if (mSingleton == null)
            mSingleton = new UISign_in();
        return mSingleton;
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

        foreach (var item in FSDataNodeTable<UISign_inNode>.GetSingleton().DataNodeList.Values)
        {
            if (item.day == 40)
            {
                accumulateSign_ThreeDay.spriteName = GameLibrary.Instance().ItemStateList[item.reward_prop[0]].icon_name.ToString();
                Sprite.spriteName = GetspriteName(GameLibrary.Instance().ItemStateList[item.reward_prop[0]].grade);
                num[0].text = item.reward_prop[1].ToString();
            }
            else if (item.day == 41)
            {
                accumulateSign_SevenDay.spriteName = GameLibrary.Instance().ItemStateList[item.reward_prop[0]].icon_name.ToString();
                Sprite1.spriteName = GetspriteName(GameLibrary.Instance().ItemStateList[item.reward_prop[0]].grade);
                num[1].text = item.reward_prop[1].ToString();
            }
            else if (item.day == 42)
            {
                if (item.reward_prop != null && item.reward_prop[0] != 0 && GameLibrary.Instance().ItemStateList.ContainsKey(item.reward_prop[0]))
                {
                    accumulateSign_FifteenDay.spriteName = GameLibrary.Instance().ItemStateList[item.reward_prop[0]].icon_name.ToString();
                    Sprite2.spriteName = GetspriteName(GameLibrary.Instance().ItemStateList[item.reward_prop[0]].grade);
                    num[2].text = item.reward_prop[1].ToString();
                }
                else {
                    Debug.Log(item.reward_prop);
                }
            }
            else if (item.day == 43)
            {
                accumulateSign_Twenty_EightDay.spriteName = GameLibrary.Instance().ItemStateList[item.reward_prop[0]].icon_name.ToString();
                Sprite3.spriteName = GetspriteName(GameLibrary.Instance().ItemStateList[item.reward_prop[0]].grade);
                num[3].text = item.reward_prop[1].ToString();
            }
            itemRankList.Add(item);
        }
        progressbar.pivot = UIWidget.Pivot.Left;
        progressbar.width = 0;
        goodsMultList = transform.FindComponent<GUISingleMultList>("Sign_inScrollView/Sign_inList");
        sign_inReplenishBtn.onClick = OnSign_inReplenishBtnClick;
        accumulateSign_ThreeDay.onClick = ThreeDayBtnClick;
        accumulateSign_SevenDay.onClick = SevenDayBtnClick;
        accumulateSign_FifteenDay.onClick = FifteenDayBtnClick;
        accumulateSign_Twenty_EightDay.onClick = EightDayBtnClick;

        CcumulateSign_inInit();
    }
    protected override void ShowHandler()
    {
        InitSignInItemData();
    }
    /// <summary>
    /// 动态生成Item
    /// </summary>
    public void InitSignInItemData()
    {
        int month = 0;
        int year = 0;
        month = PropertyManager.ConvertIntDateTime(Auxiliary.GetNowTime()).Month;
        year = PropertyManager.ConvertIntDateTime(Auxiliary.GetNowTime()).Year;

        if ((year % 4 == 0 && year % 100 != 0) || year % 400 == 0)
        {
            goodsMultList.InSize(GetYearDay(month), 5);
            goodsMultList.Info(itemRankList.ToArray());
        }
        else
        {
            if (month == 2)
            {
                goodsMultList.InSize(28, 5);
                goodsMultList.Info(itemRankList.ToArray());
            }
        }
        if (FSDataNodeTable<VipNode>.GetSingleton().DataNodeList.ContainsKey(playerData.GetInstance().selfData.vip))
        {
            sign_inReplenishMax.text = FSDataNodeTable<VipNode>.GetSingleton().DataNodeList[playerData.GetInstance().selfData.vip].retroactive_limit.ToString();
        }
        memberLevle.text = playerData.GetInstance().selfData.vip.ToString();
        accumulateSign_inNum.text = int.Parse(playerData.GetInstance().singnData.Signed.Substring(6, 2)) + "天";
        Debug.Log(playerData.GetInstance().singnData.Signed);
        if (int.Parse(playerData.GetInstance().singnData.Signed.Substring(6, 2)) >= 3)
        {
            if (int.Parse(playerData.GetInstance().singnData.Signed.Substring(8, 1)) == 0)
            {
                ThreeDay.transform.gameObject.SetActive(true);
            }
            else
            {
                ThreeDay.transform.gameObject.SetActive(false);
            }
        }
        if (int.Parse(playerData.GetInstance().singnData.Signed.Substring(6, 2)) >= 7)
        {
            if (int.Parse(playerData.GetInstance().singnData.Signed.Substring(9, 1)) == 0)
            {
                SevenDay.transform.gameObject.SetActive(true);
            }
            else
            {
                SevenDay.transform.gameObject.SetActive(false);

            }
        }
        if (int.Parse(playerData.GetInstance().singnData.Signed.Substring(6, 2)) >= 15)
        {
            if (int.Parse(playerData.GetInstance().singnData.Signed.Substring(10, 1)) == 0)
            {
                FifteenDay.transform.gameObject.SetActive(true);
            }
            else
            {
                FifteenDay.transform.gameObject.SetActive(false);
            }
        }
        if (int.Parse(playerData.GetInstance().singnData.Signed.Substring(6, 2)) >= 28)
        {
            if (int.Parse(playerData.GetInstance().singnData.Signed.Substring(11, 1)) == 0)
            {
                eightDay.transform.gameObject.SetActive(true);
            }
            else
            {
                eightDay.transform.gameObject.SetActive(false);

            }
        }
        if (int.Parse(playerData.GetInstance().singnData.Signed.Substring(8, 1)) != 0)
        {
            ThreeDayOK.transform.gameObject.SetActive(true);

        }
        if (int.Parse(playerData.GetInstance().singnData.Signed.Substring(9, 1)) != 0)
        {
            SevenDayOK.transform.gameObject.SetActive(true);

        }
        if (int.Parse(playerData.GetInstance().singnData.Signed.Substring(10, 1)) != 0)
        {
            FifteenDayOK.transform.gameObject.SetActive(true);

        }
        if (int.Parse(playerData.GetInstance().singnData.Signed.Substring(11, 1)) != 0)
        {
            EightDayOK.transform.gameObject.SetActive(true);

        }
        foreach (var item in FSDataNodeTable<UISign_inNode>.GetSingleton().DataNodeList.Values)
        {
            if (item.reward_prop != null)
            {
                string goodsID = item.reward_prop[0].ToString();
                int goodsIDNum = int.Parse(goodsID.Substring(0, 3));
                if (goodsIDNum == 107)
                {
                    InsHero(GameLibrary.Instance().ItemStateList[item.reward_prop[0]].icon_name);
                    heroName.text = GameLibrary.Instance().ItemStateList[item.reward_prop[0]].name.ToString();
                    long heroID = long.Parse(item.reward_prop[0].ToString().Replace("107", "201"));
                    foreach (var heroitem in FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList.Values)
                    {
                        if (heroID == heroitem.hero_id)
                        {
                            star.IsShow("xing-hui", "xing", heroitem.init_star);
                            heroType.spriteName = GetHeroType(heroitem.attribute);
                        }
                    }
                }
            }
        }
        if (FSDataNodeTable<VipNode>.GetSingleton().DataNodeList[playerData.GetInstance().selfData.vip].retroactive_limit - int.Parse(playerData.GetInstance().singnData.Signed.Substring(12, 2)) > 0)
            sign_inReplenishNum.text = (FSDataNodeTable<VipNode>.GetSingleton().DataNodeList[playerData.GetInstance().selfData.vip].retroactive_limit - int.Parse(playerData.GetInstance().singnData.Signed.Substring(12, 2))).ToString();
        else
        {
            sign_inReplenishNum.text = "0";
        }
        CcumulateSign_inInit();
    }
    //累计条
    public void CcumulateSign_inInit()
    {
        int uiSign_inNum = int.Parse(playerData.GetInstance().singnData.Signed.Substring(6, 2));
        if (uiSign_inNum <= 3)
        {
            progressbar.width = uiSign_inNum * 26;
        }
        else if (uiSign_inNum > 3 && uiSign_inNum <= 7)
        {
            progressbar.width = (uiSign_inNum - 3) * 38 + 81;
        }
        else if (uiSign_inNum > 7 && uiSign_inNum <= 15)
        {
            progressbar.width = (uiSign_inNum - 7) * 19 + 232;
        }
        else if (uiSign_inNum > 15 && uiSign_inNum <= 28)
        {
            progressbar.width = (uiSign_inNum - 15) * 12 + 384;
        }

    }
    //补签
    private void OnSign_inReplenishBtnClick()
    {
        int jewel = 0;
        foreach (var item in FSDataNodeTable<ResetLaterNode>.GetSingleton().DataNodeList.Values)
        {
            jewel = item.retroactiveBuy;
        }
        int day = Convert.ToInt32(TimeManager.Instance.ConvertIntDateTime(Auxiliary.GetNowTime()).ToString("dd"));
        if (day - int.Parse(playerData.GetInstance().singnData.Signed.Substring(12, 2)) > 0)
        {

            if (FSDataNodeTable<VipNode>.GetSingleton().DataNodeList[playerData.GetInstance().selfData.vip].retroactive_limit - int.Parse(playerData.GetInstance().singnData.Signed.Substring(12, 2)) > 0)
            {
                // UISign_intBox.Instance.ShowLabel("签到需要" + jewel.ToString() + "钻石");

                foreach (var item in FSDataNodeTable<UISign_inNode>.GetSingleton().DataNodeList.Values)
                {
                    if (item.reward_prop != null)
                    {
                        if (item.day == int.Parse(playerData.GetInstance().singnData.Signed.Substring(6, 2)) + 1)
                        {
                            UISign_inData.Instance().DoodsID = (int)item.reward_prop[0];
                            UISign_inData.Instance().DoodsNum = (int)item.reward_prop[1]; ;
                        }
                    }
                }
                object[] obj = new object[5] { null, "签到需要" + jewel.ToString() + "钻石", UIPopupType.EnSure, this.gameObject, "OnEnsureClick" };
                Control.ShowGUI(UIPanleID.UIPopUp, EnumOpenUIType.DefaultUIOrSecond, false, obj);
            }
            else
            {
                Control.ShowGUI(UIPanleID.UITooltips, EnumOpenUIType.DefaultUIOrSecond, false, "您当前补签次数是0次,可提升VIP等级,增加补签次数.");
                //Control.ShowGUI(GameLibrary.UITooltips);
            }
        }
        else
        {
            Control.ShowGUI(UIPanleID.UITooltips, EnumOpenUIType.DefaultUIOrSecond, false, "您没有可补签的日期");
            //Control.ShowGUI(GameLibrary.UITooltips);
        }
    }

    private void OnEnsureClick()
    {
        Control.HideGUI(UIPanleID.UIMask);
        int jewel = 0;
        foreach (var item in FSDataNodeTable<ResetLaterNode>.GetSingleton().DataNodeList.Values)
        {
            jewel = item.retroactiveBuy;
        }
        if (playerData.GetInstance().baginfo.diamond > jewel)
        {
            ClientSendDataMgr.GetSingle().GetUISign_inSend().SendGetPatchUISign_in(C2SMessageType.PASVWait);//补签
        }
        else
        {
            //UIPromptBox.Instance.ShowLabel("钻石不足");
            //Control.ShowGUI(GameLibrary.UIPromptBox);
            Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, "您的钻石不足！");
        }
    }
    //获取月份天数
    public int GetYearDay(int year)
    {
        switch (year)
        {
            case 1:
                return 31;
                break;
            case 2:
                return 29;
                break;
            case 3:
                return 31;
            case 4:
                return 30;
            case 5:
                return 31;
            case 6:
                return 30;
            case 7:
                return 31;
            case 8:
                return 31;
            case 9:
                return 30;
            case 10:
                return 31;
            case 11:
                return 30;
            case 12:
                return 31;
        }
        return 0;
    }


    private void ThreeDayBtnClick()
    {
        UISign_inData.Instance().DoodsID = (int)FSDataNodeTable<UISign_inNode>.GetSingleton().DataNodeList[40].reward_prop[0];
        UISign_inData.Instance().DoodsNum = (int)FSDataNodeTable<UISign_inNode>.GetSingleton().DataNodeList[40].reward_prop[1];
        UISign_inData.Instance().ItemNode = GameLibrary.Instance().ItemStateList[FSDataNodeTable<UISign_inNode>.GetSingleton().DataNodeList[40].reward_prop[0]];
        UISign_inData.Instance().ID = FSDataNodeTable<UISign_inNode>.GetSingleton().DataNodeList[40].reward_prop[0];
        ClientSendDataMgr.GetSingle().GetUISign_inSend().SendGetUISign_inCumulative(C2SMessageType.PASVWait, 1);//发送累计领取    
    }
    private void SevenDayBtnClick()
    {
        UISign_inData.Instance().DoodsID = (int)FSDataNodeTable<UISign_inNode>.GetSingleton().DataNodeList[41].reward_prop[0];
        UISign_inData.Instance().DoodsNum = (int)FSDataNodeTable<UISign_inNode>.GetSingleton().DataNodeList[41].reward_prop[1];
        UISign_inData.Instance().ItemNode = GameLibrary.Instance().ItemStateList[FSDataNodeTable<UISign_inNode>.GetSingleton().DataNodeList[41].reward_prop[0]];
        UISign_inData.Instance().ID = FSDataNodeTable<UISign_inNode>.GetSingleton().DataNodeList[41].reward_prop[0];
        ClientSendDataMgr.GetSingle().GetUISign_inSend().SendGetUISign_inCumulative(C2SMessageType.PASVWait, 2);//发送累计领取    
    }
    private void FifteenDayBtnClick()
    {
        UISign_inData.Instance().DoodsID = (int)FSDataNodeTable<UISign_inNode>.GetSingleton().DataNodeList[42].reward_prop[0];
        UISign_inData.Instance().DoodsNum = (int)FSDataNodeTable<UISign_inNode>.GetSingleton().DataNodeList[42].reward_prop[1];
        UISign_inData.Instance().ItemNode = GameLibrary.Instance().ItemStateList[FSDataNodeTable<UISign_inNode>.GetSingleton().DataNodeList[42].reward_prop[0]];
        UISign_inData.Instance().ID = FSDataNodeTable<UISign_inNode>.GetSingleton().DataNodeList[42].reward_prop[0];
        ClientSendDataMgr.GetSingle().GetUISign_inSend().SendGetUISign_inCumulative(C2SMessageType.PASVWait, 3);//发送累计领取    
    }
    private void EightDayBtnClick()
    {
        UISign_inData.Instance().DoodsID = (int)FSDataNodeTable<UISign_inNode>.GetSingleton().DataNodeList[43].reward_prop[0];
        UISign_inData.Instance().DoodsNum = (int)FSDataNodeTable<UISign_inNode>.GetSingleton().DataNodeList[43].reward_prop[1];
        UISign_inData.Instance().ItemNode = GameLibrary.Instance().ItemStateList[FSDataNodeTable<UISign_inNode>.GetSingleton().DataNodeList[43].reward_prop[0]];
        UISign_inData.Instance().ID = FSDataNodeTable<UISign_inNode>.GetSingleton().DataNodeList[43].reward_prop[0];
        ClientSendDataMgr.GetSingle().GetUISign_inSend().SendGetUISign_inCumulative(C2SMessageType.PASVWait, 4);//发送累计领取    
    }
    public static string GetHeroGradeName(int Gradetype)
    {
        switch (Gradetype)
        {
            case 1: return "hui";
            case 2: return "lv";
            case 3: return "lan";
            case 4: return "zi";
            case 5: return "cheng";
            case 6: return "hong";
            default:
                break;
        }
        return "hui";
    }
    public static string GetspriteName(int Gradetype)
    {
        switch (Gradetype)
        {
            case 1: return "hui";
            case 2: return "lv";
            case 3: return "lv1";
            case 4: return "lan";
            case 5: return "lan1";
            case 6: return "lan2";
            case 7: return "zi";
            case 8: return "zi1";
            case 9: return "zi2";
            case 10: return "zi3";
            case 11:
            case 12:
            case 13:
            case 14:
            case 15: return "cheng";
            case 16:
            case 17:
            case 18:
            case 19:
            case 20:
            case 21: return "hong";
            default:
                break;
        }
        return "hui";
    }
    /// <summary>
    /// 实例化英雄展示模型
    /// </summary>
    void InsHero(string insHero)
    {
        HeroPosEmbattle.instance.CreatModel(insHero, PosType.uisign, transform.Find("Hero").GetComponent<SpinWithMouse>());
    }

    public void GetHeroDebris()
    {
        for (int i = 0; i < UISign_in.itemRankList.Count; i++)
        {
            if (int.Parse(playerData.GetInstance().singnData.Signed.Substring(6, 2)) == UISign_in.itemRankList[i].day)
            {
                if (UISign_in.itemRankList[i].reward_prop != null)
                {
                    string goodsID = UISign_in.itemRankList[i].reward_prop[0].ToString();
                    if (int.Parse(StringUtil.SubString(goodsID, 3)) == 107)
                    {
                        //判断签到是英雄的时候播放特效暂时不能播放因为会 跟签到的英雄同事显示
                        //if ((int.Parse(StringUtil.SubString(goodsID, 3)) == 107))
                        //{
                        int heroId = int.Parse(201 + StringUtil.SubString(goodsID, 6, 3));
                        //    if (playerData.GetInstance().AddHeroToList(heroId))
                        //    {
                        //        UILottryHeroEffect.instance.InitUI(heroId, ShowHeroEffectType.signIn, HeroOrSoul.Hero, 0);
                        //    }
                        //    else
                        //    {
                        //        UILottryHeroEffect.instance.InitUI(heroId, ShowHeroEffectType.signIn, HeroOrSoul.Soul, 0);
                        //    }

                        //}
                        if (playerData.GetInstance().AddHeroToList(heroId))
                        {
                            if (FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList.ContainsKey(heroId))
                            {
                                int mStar = FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList[heroId].init_star;
                                playerData.GetInstance().RefreshHeroToList(heroId, mStar, 0, 0);
                            }

                        }
                        else
                        {
                            UIPopLottery.Instance.InitShow(UISign_in.itemRankList[i].reward_prop[0], -1, LotteryType.None);
                            Control.ShowGUI(UIPanleID.UIPopLottery,EnumOpenUIType.DefaultUIOrSecond);
                            long Heroid = long.Parse(goodsID.ToString().Replace("107", "201"));
                            foreach (var heroitem in FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList.Values)
                            {
                                if (Heroid == heroitem.hero_id)
                                {
                                    HeroData data = playerData.GetInstance().GetHeroDataByID(Heroid);
                                    if (data != null)
                                    {
                                        int startId = data.star;
                                        long HerodebrisID = long.Parse(goodsID.ToString().Replace("107", "106"));
                                        Debug.Log(HerodebrisID);
                                        Debug.Log(FSDataNodeTable<StarUpGradeNode>.GetSingleton().DataNodeList[startId].convert_stone_num);
                                    }
                                }
                            }
                        }
                    }
                }

            }
        }
    }
    public string GetHeroType(int starNum)
    {
        switch (starNum)
        {
            case 1:
                return "li";
                break;
            case 2:
                return "zhi";
                break;
            case 3:
                return "min";
                break;
            default:
                break;
        }
        return "0";
    }

}
