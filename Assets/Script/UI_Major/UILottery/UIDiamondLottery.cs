using UnityEngine;
using System.Collections;
using Tianyu;

public class UIDiamondLottery : GUIBase
{
    //public GameObject leftBtnLabel; //引导挂点

    public GUISingleLabel nameTxt;
    // private long maxTimer = 43200;
    private long lostTime;
    private float lostSecTime;
    public GUISingleLabel timeTxt;
    public GUISingleLabel leftDiamondTxt;
    public GUISingleLabel rightDiamondTxt;
    public int leftDiamondPrice;
    public int rightDiamondPrice;
    private bool isShowTime = false;
    public int freeCount;
    public GUISingleSprite icon;
    public GUISingleSprite point;
    public GUISingleSprite quality;
    public int discount;//折扣
    public GUISingleButton leftBtn;
    public GUISingleButton rightBtn;
    private long id;//药品id
    protected override void Init()
    {
        if (FSDataNodeTable<DiamondDrawNode>.GetSingleton().DataNodeList.Count > 0)
        {
            foreach (DiamondDrawNode node in FSDataNodeTable<DiamondDrawNode>.GetSingleton().DataNodeList.Values)
            {
                id = node.id;
            }
        }

        lostSecTime = 0;
        leftDiamondPrice = 288;
        discount = 9;
        rightDiamondPrice = 2590;//leftDiamondPrice * 10 * discount / 10;
        InitShow();
        leftBtn.onClick = OnLeftBtn;
        rightBtn.onClick = OnRightBtn;
        point.gameObject.SetActive(false);
    }
    public override UIPanleID GetUIKey()
    {
        return UIPanleID.none;
    }
    public void InitShow()
    {
        icon.spriteName = GameLibrary.Instance().ItemStateList[id].icon_name;
        nameTxt.text = "购买" + GameLibrary.Instance().ItemStateList[id].name;
        quality.spriteName = ItemData.GetFrameByGradeType((GradeType)(GameLibrary.Instance().ItemStateList[id].grade));
        leftDiamondTxt.text = leftDiamondPrice.ToString();
        rightDiamondTxt.text = (rightDiamondPrice).ToString();
    }
    private void OnRightBtn()
    {
        if (GameLibrary.isShow)
        {
            return;
        }
        //钻石抽奖开启条件
        if (!DataDefine.isSkipFunction&&!FunctionOpenMng.GetInstance().GetFunctionOpen(35))
        {
            string text = FSDataNodeTable<UnLockFunctionNode>.GetSingleton().DataNodeList[35].limit_tip;
            //UIPromptBox.Instance.ShowLabel(text);
            Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, text);
            return;
        }
        if (playerData.GetInstance().baginfo.diamond > rightDiamondPrice)
        {
            GameLibrary.isShow = true;
            HeroPosEmbattle.instance.ShowLottryAnimaEffect(PosType.Lorry, LotteryType.DiamondLottery, 10, CostType.Cost);
            object[] obj = new object[7] {10, LotteryType.DiamondLottery , rightDiamondPrice , nameTxt.text , icon.spriteName , quality.spriteName , id };
            Control.ShowGUI(UIPanleID.UIResultLottery, EnumOpenUIType.OpenNewCloseOld, false, obj);

            Control.HideGUI(UIPanleID.UIMoney);
            Control.ShowGUI(UIPanleID.UILottryEffect, EnumOpenUIType.DefaultUIOrSecond);
        }
        else
        {
            Control.HideGUI(UIPanleID.UIMask);
            //UIPromptBox.Instance.ShowLabel("您的钻石不足请充值");
            //Control.ShowGUI(GameLibrary.UIPromptBox);
            Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, "您的钻石不足请充值");
        }
    }
    protected override void ShowHandler()
    {
        base.ShowHandler();
        Singleton<RedPointManager>.Instance.DeletType(EnumRedPoint.RP_DRAW);
        if (Auxiliary.GetNowTime() - playerData.GetInstance().lotteryInfo.diamondTime >= 0)
        {
            lostTime = 0;
            isShowTime = true;
        }
        else
        {

            lostTime = (long)Mathf.Abs(Auxiliary.GetNowTime() - playerData.GetInstance().lotteryInfo.diamondTime);
            isShowTime = true;
        }

    }
    private void OnLeftBtn()
    {
        if (GameLibrary.isShow)
        {
            return;
        }
        //钻石抽奖开启条件
        if (!FunctionOpenMng.GetInstance().GetFunctionOpen(35))
        {
            string text = FSDataNodeTable<UnLockFunctionNode>.GetSingleton().DataNodeList[35].limit_tip;
            //UIPromptBox.Instance.ShowLabel(text);
            Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, text);
            return;
        }
        if (leftDiamondTxt.text == "免费")
        {
            GameLibrary.isShow = true;
            leftDiamondTxt.text = leftDiamondPrice.ToString();
            point.gameObject.SetActive(false);
            Singleton<RedPointManager>.Instance.DeletChildFlag(EnumRedPoint.RP_DRAW, 2);
            isShowTime = true;
            Control.ShowGUI(UIPanleID.UILottryEffect, EnumOpenUIType.DefaultUIOrSecond);
            object[] obj = new object[7] { 1, LotteryType.DiamondLottery, leftDiamondPrice, nameTxt.text, icon.spriteName, quality.spriteName, id };
            Control.ShowGUI(UIPanleID.UIResultLottery, EnumOpenUIType.OpenNewCloseOld, false,obj);
            HeroPosEmbattle.instance.ShowLottryAnimaEffect(PosType.Lorry, LotteryType.DiamondLottery, 1, CostType.Free);
            Control.HideGUI(UIPanleID.UIMoney);

        }
        else if (playerData.GetInstance().baginfo.diamond > leftDiamondPrice)
        {
            GameLibrary.isShow = true;
            Control.ShowGUI(UIPanleID.UILottryEffect, EnumOpenUIType.DefaultUIOrSecond);
            object[] obj = new object[7] { 1, LotteryType.DiamondLottery, leftDiamondPrice, nameTxt.text, icon.spriteName, quality.spriteName, id }; ;
            Control.ShowGUI(UIPanleID.UIResultLottery, EnumOpenUIType.OpenNewCloseOld, false, obj);
            HeroPosEmbattle.instance.ShowLottryAnimaEffect(PosType.Lorry, LotteryType.DiamondLottery, 1, CostType.Cost);
            Control.HideGUI(UIPanleID.UIMoney);
        }
        else
        {
            //Control.ShowGUI(GameLibrary.UIPromptBox);
            Control.HideGUI(UIPanleID.UIMask);
            //UIPromptBox.Instance.ShowLabel("您的钻石不足请充值");
            Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, "您的钻石不足请充值");
        }
    }
    void Update()
    {
        if (isShowTime)
        {
            ShowTime();
        }

    }
    public void ShowTime()
    {
        if (lostTime <= 0)
        {
            isShowTime = false;
            timeTxt.text = "已可免费购买";
            leftDiamondTxt.text = "免费";
            if (FunctionOpenMng.GetInstance().GetFunctionOpen(35))
            {
                point.gameObject.SetActive(true);
                Singleton<RedPointManager>.Instance.AddChildFlag(EnumRedPoint.RP_DRAW, 2);
            }

        }
        else
        {
            leftDiamondTxt.text = leftDiamondPrice.ToString();
            timeTxt.text = "[acd5ff]" + TimeManager.Instance.GetTimeClockText(lostTime) + "[-]后免费";
            point.gameObject.SetActive(false);
            Singleton<RedPointManager>.Instance.DeletChildFlag(EnumRedPoint.RP_DRAW, 2);
        }


        lostSecTime += Time.deltaTime;
        if (lostSecTime >= 1)
        {
            lostTime -= 1;
            lostSecTime = 0;
        }

    }

}
