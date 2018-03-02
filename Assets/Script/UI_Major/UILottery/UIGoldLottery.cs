using Tianyu;
using UnityEngine;
using System.Collections.Generic;

public class UIGoldLottery : GUIBase
{
    //public GameObject leftBtnLabel;//引导挂点

    public static UIGoldLottery instance;
    public GUISingleLabel nameTxt;
    private long maxTimer = 600;
    private long lostTime;
    private float lostSecTime;//倒计时秒数
    public GUISingleLabel timeTxt;//显示倒计时
    //public GUISingleLabel freeCountTxt;//免费的次数
    public int freeCount = 0;
    public int maxFreeCount = 5;//免费的总次数
    public GUISingleLabel leftGoldTxt;
    public GUISingleLabel rightGoleTxt;
    public GUISingleSprite icon;
    public GUISingleSprite quality;
    public int leftGoldPrice;
    public int rightGoldPrice;
    public GUISingleSprite point;
    public int discount;//折扣
    private bool isShowTime;
    public GUISingleButton leftBtn;
    public GUISingleButton rightBtn;
    private long id;//药品id
    protected override void Init()
    {
        instance = this;
        if (FSDataNodeTable<GoldDrawNode>.GetSingleton().DataNodeList.Count > 0)
        {
            foreach (GoldDrawNode node in FSDataNodeTable<GoldDrawNode>.GetSingleton().DataNodeList.Values)
            {
                id = node.id;
            }
        }

        leftBtn.onClick = OnLeftBtn;
        rightBtn.onClick = OnRightBtn;
        point.gameObject.SetActive(false);
        InitShow();
    }
    public override UIPanleID GetUIKey()
    {
        return UIPanleID.none;
    }
    /// <summary>
    /// 购买10次按钮
    /// </summary>
    private void OnRightBtn()
    {
        if (GameLibrary.isShow)
        {
            return;
        }
        //金币抽奖开启条件
        if (!DataDefine.isSkipFunction&&!FunctionOpenMng.GetInstance().GetFunctionOpen(33))
        {
            string text = FSDataNodeTable<UnLockFunctionNode>.GetSingleton().DataNodeList[33].limit_tip;
            //UIPromptBox.Instance.ShowLabel(text);
            Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, text);
            return;
        }
        if (playerData.GetInstance().baginfo.gold > rightGoldPrice)
        {
            GameLibrary.isShow = true;

            HeroPosEmbattle.instance.ShowLottryAnimaEffect(PosType.Lorry,LotteryType.GoldLottery, 10, CostType.Cost);
            Control.ShowGUI(UIPanleID.UILottryEffect, EnumOpenUIType.DefaultUIOrSecond);
            object[] obj = new object[7] { 10, LotteryType.GoldLottery, rightGoldPrice, nameTxt.text, icon.spriteName, quality.spriteName, id };
            Control.ShowGUI(UIPanleID.UIResultLottery, EnumOpenUIType.OpenNewCloseOld, false, obj);
            Control.HideGUI(UIPanleID.UIMoney);

        }
        else
        {
            Control.ShowGUI(UIPanleID.UIGoldHand, EnumOpenUIType.DefaultUIOrSecond);
        }
    }
    private void OnLeftBtn()
    {
        if (GameLibrary.isShow)
        {
            return;
        }
        //金币抽奖开启条件
        if (!DataDefine.isSkipFunction&&!FunctionOpenMng.GetInstance().GetFunctionOpen(33))
        {
            string text = FSDataNodeTable<UnLockFunctionNode>.GetSingleton().DataNodeList[33].limit_tip;
            //UIPromptBox.Instance.ShowLabel(text);
            Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, text);
            return;
        }
        if (leftGoldTxt.text == "免费")
        {
            GameLibrary.isShow = true;
            if (freeCount >= 1)
            {
                freeCount -= 1;
                isShowTime = true;
            }
            point.gameObject.SetActive(false);
            Singleton<RedPointManager>.Instance.DeletChildFlag(EnumRedPoint.RP_DRAW, 1);
            leftGoldTxt.text = leftGoldPrice.ToString();
            timeTxt.text = "[acd5ff]" + TimeManager.Instance.GetTimeClockText(lostTime) + "[-]后免费" + "[2dd740](" + freeCount + "/" + maxFreeCount + ")";
           // freeCountTxt.text = "[2dd740](" + freeCount + "/" + maxFreeCount + ")";
            leftBtn.text = "购买1个";
            object[] obj = new object[7] { 1, LotteryType.GoldLottery, leftGoldPrice, nameTxt.text, icon.spriteName, quality.spriteName, id };
            HeroPosEmbattle.instance.ShowLottryAnimaEffect(PosType.Lorry, LotteryType.GoldLottery, 1, CostType.Free);
            Control.ShowGUI(UIPanleID.UILottryEffect, EnumOpenUIType.DefaultUIOrSecond);
            Control.ShowGUI(UIPanleID.UIResultLottery, EnumOpenUIType.OpenNewCloseOld, false, obj);
            Control.HideGUI(UIPanleID.UIMoney);

            return;
        }
        else
        if (playerData.GetInstance().baginfo.gold > leftGoldPrice)
        {
            GameLibrary.isShow = true;
            object[] obj = new object[7] { 1, LotteryType.GoldLottery, leftGoldPrice, nameTxt.text, icon.spriteName, quality.spriteName, id };
            HeroPosEmbattle.instance.ShowLottryAnimaEffect(PosType.Lorry, LotteryType.GoldLottery, 1, CostType.Cost);
            Control.ShowGUI(UIPanleID.UILottryEffect, EnumOpenUIType.DefaultUIOrSecond);
            Control.ShowGUI(UIPanleID.UIResultLottery, EnumOpenUIType.OpenNewCloseOld, false, obj);
          
            Control.HideGUI(UIPanleID.UIMoney);
        
        }
        else
        {
            //Control.ShowGUI(GameLibrary.UIPromptBox);
            Control.HideGUI(UIPanleID.UIMask);
            //UIPromptBox.Instance.ShowLabel("您的金币不足请充值");
            Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, "您的钻石不足请充值");
        }
    }
    //初始化UI显示
    public void InitShow()
    {
        lostSecTime = 0;
        freeCount = maxFreeCount;
        leftGoldPrice = 10000;
        discount = 9;
        rightGoldPrice = leftGoldPrice * 10 * discount / 10;
        icon.spriteName = GameLibrary.Instance().ItemStateList[id].icon_name;
        nameTxt.text ="购买"+ GameLibrary.Instance().ItemStateList[id].name;
        quality.spriteName = ItemData.GetFrameByGradeType((GradeType)(GameLibrary.Instance().ItemStateList[id].grade));
        leftGoldTxt.text = leftGoldPrice.ToString();
        rightGoleTxt.text = (rightGoldPrice).ToString();

    }

    protected override void ShowHandler()
    {
        base.ShowHandler();
        Singleton<RedPointManager>.Instance.DeletType(EnumRedPoint.RP_DRAW);
        freeCount = maxFreeCount - playerData.GetInstance().lotteryInfo.goldDrawCount;
        if (freeCount >= 1)
        {
            isShowTime = true;
            timeTxt.text = "[acd5ff]" + TimeManager.Instance.GetTimeClockText(lostTime) + "[-]后免费" + "[2dd740](" + freeCount + "/" + maxFreeCount + ")";
           // freeCountTxt.text = "[2dd740](" + freeCount + "/" + maxFreeCount + ")";
        }
        else
        {
            freeCount = 0;
            timeTxt.text = "今日免费次数已用完" + "[2dd740](" + freeCount + "/" + maxFreeCount + ")";
            //freeCountTxt.text = "[2dd740](" + freeCount + "/" + maxFreeCount + ")";
        }

        if (Auxiliary.GetNowTime() - playerData.GetInstance().lotteryInfo.goldTime >= 0)
        {
            lostTime = 0;
        }
        else
        {
            lostTime = (long)Mathf.Abs(Auxiliary.GetNowTime() - playerData.GetInstance().lotteryInfo.goldTime);
        }

    }

    void Update()
    {
        if (isShowTime)
        {
            ShowTime();
        }
    }
    /// <summary>
    /// 金币免费倒计时
    /// </summary>
    private void ShowTime()
    {      
        if (freeCount <= 0)
        {
            timeTxt.text = "今日免费次数已用完"+ "[2dd740](" + freeCount + "/" + maxFreeCount + ")";
            leftGoldTxt.text = leftGoldPrice.ToString();
            isShowTime = false;        
        }
        else
        {
            if (lostTime <= 0)
            {
                isShowTime = false;
                timeTxt.text = "已可免费购买[2dd740](" + freeCount + "/" + maxFreeCount + ")";
                point.gameObject.SetActive(true);
                Singleton<RedPointManager>.Instance.AddChildFlag(EnumRedPoint.RP_DRAW, 1);
                leftGoldTxt.text = "免费";
            }
            else
            {
                leftGoldTxt.text = leftGoldPrice.ToString();
                point.gameObject.SetActive(false);
                Singleton<RedPointManager>.Instance.DeletChildFlag(EnumRedPoint.RP_DRAW, 1);
                timeTxt.text = "[acd5ff]" + TimeManager.Instance.GetTimeClockText(lostTime) + "[-]后免费"+ "[2dd740](" + freeCount + "/" + maxFreeCount + ")";
            }         
        }     
        lostSecTime += Time.deltaTime;
        if (lostSecTime >= 1)
        {
            lostTime -= 1;
            lostSecTime = 0;
        }
    }

    //protected override void RegisterComponent()
    //{
    //    base.RegisterComponent();
    //    RegisterComponentID(13, 31, leftBtnLabel.gameObject);

    //}
    //protected override void RegisterIsOver()
    //{
    //   // base.RegisterIsOver();
    //}
}
