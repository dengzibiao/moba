/*
文件名（File Name）:   UISoulLottery.cs

作者（Author）:    #高#

创建时间（CreateTime）:  2016-6-22 21:18:9
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tianyu;

public class UISoulLottery : GUIBase
{
    private static UISoulLottery instance;
    public GUISingleLabel nameTxt;
    public GUISingleLabel leftDiamondTxt;
    public GUISingleLabel rightDiamondTxt;
    public GUISingleMultList multList;
    private int leftDiamondPrice;
    private int rightDiamondPrice;
    private int discount;//折扣
    public GUISingleSprite icon;
    public GUISingleSprite quality;
    public GUISingleButton leftBtn;
    public GUISingleButton rightBtn;
    private long id;//药品id
    private string mQuality;


    public static UISoulLottery Instance
    {
        get { return instance; }
        set { instance = value; }
    }

    public UISoulLottery()
    {
        Instance = this;
    }
    protected override void Init()
    {
        if (FSDataNodeTable<SoulDrawNode>.GetSingleton().DataNodeList.Count > 0)
        {
            foreach (SoulDrawNode node in FSDataNodeTable<SoulDrawNode>.GetSingleton().DataNodeList.Values)
            {
                id = node.id;
            }
        }
        leftDiamondPrice = 400;
        discount = 9;
        rightDiamondPrice = leftDiamondPrice * 10 * discount / 10;
        InitShow();
        leftBtn.onClick = OnLeftBtn;
        rightBtn.onClick = OnRightBtn;
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
    /// <summary>
    /// 接收服务器数据
    /// </summary>
    public object[] InitItemData()
    {
        return playerData.GetInstance().lotteryInfo.hotList.ToArray();
    }
    private void OnRightBtn()
    {
        if (GameLibrary.isShow)
        {
            return;
        }
        //魂匣抽奖开启条件
        if (!DataDefine.isSkipFunction && !FunctionOpenMng.GetInstance().GetFunctionOpen(31))
        {
            string text = FSDataNodeTable<UnLockFunctionNode>.GetSingleton().DataNodeList[31].limit_tip;
            //UIPromptBox.Instance.ShowLabel(text);
            Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, text);
            return;
        }
        if (playerData.GetInstance().baginfo.diamond > rightDiamondPrice)
        {
            GameLibrary.isShow = true;
            Control.ShowGUI(UIPanleID.UILottryEffect, EnumOpenUIType.DefaultUIOrSecond);
            HeroPosEmbattle.instance.ShowLottryAnimaEffect(PosType.Lorry, LotteryType.LotterySoul, 10, CostType.Cost);
            object[] obj = new object[7] { 10, LotteryType.LotterySoul, rightDiamondPrice, nameTxt.text, icon.spriteName, quality.spriteName, id };
            Control.ShowGUI(UIPanleID.UIResultLottery, EnumOpenUIType.OpenNewCloseOld, false, obj);

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

    private void OnLeftBtn()
    {
        if (GameLibrary.isShow)
        {
            return;
        }
        //魂匣抽奖开启条件
        if (!FunctionOpenMng.GetInstance().GetFunctionOpen(31))
        {
            string text = FSDataNodeTable<UnLockFunctionNode>.GetSingleton().DataNodeList[31].limit_tip;
            //UIPromptBox.Instance.ShowLabel(text);
            Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, text);
            return;
        }
        if (playerData.GetInstance().baginfo.diamond > leftDiamondPrice)
        {
            GameLibrary.isShow = true;
            Control.ShowGUI(UIPanleID.UILottryEffect, EnumOpenUIType.DefaultUIOrSecond);
            HeroPosEmbattle.instance.ShowLottryAnimaEffect(PosType.Lorry, LotteryType.LotterySoul, 1, CostType.Cost);
            object[] obj = new object[7] { 1, LotteryType.LotterySoul, leftDiamondPrice, nameTxt.text, icon.spriteName, quality.spriteName, id };
            Control.ShowGUI(UIPanleID.UIResultLottery, EnumOpenUIType.OpenNewCloseOld, false, obj);
        }
        else
        {
            //Control.ShowGUI(GameLibrary.UIPromptBox);
            Control.HideGUI(UIPanleID.UIMask);
           // UIPromptBox.Instance.ShowLabel("您的钻石不足请充值");
            Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, "您的钻石不足请充值");
        }
    }
    protected override void ShowHandler()
    {
        if (playerData.GetInstance().lotteryInfo.hotList.Count >= 4)
        {
            multList.InSize(4, 4);
            multList.Info(InitItemData());
        }
    }
}
