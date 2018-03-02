using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class UIResultLottery : GUIBase
{
    public GameObject oKBtnLabel;

    #region  variable
    public static UIResultLottery instance;
    private Transform view;
    public GUISingleMultList multList;
    private LotteryGoldVO goldVO;
    public GUISingleButton buyBtn;
    public GUISingleButton oKBtn;

    public GUISingleSprite moneySprite;
    public GUISingleSprite icon;
    public GUISingleSprite quality;
    public GUISingleSprite effect;

    public GUISingleLabel priceTxt;
    public GUISingleLabel nameTxt;
    public GUISingleLabel countTxt;

    private UISprite sprite;
    private PlayEffect mEffect;
    private LotteryType mLotteryType;
    private int mBuyCount;
    private int mPrice;
    private string mName;
    private string mIconName;
    private string mQuality;
    private long mItemID;//额外的药水ID


    #endregion

    #region 初始化组件
    protected override void Init()
    {
        buyBtn.onClick = OnBuyClick;
        oKBtn.onClick = OnOKClick;
        view = transform.Find("Scroll View");
        //mEffect = effect.GetComponent<PlayEffect>();
        sprite = buyBtn.transform.Find("Sprite").GetComponent<UISprite>();
    }
    /// <summary>
    /// 控制暂停出动画的时候恢复初始状态
    /// </summary>
    public void ContinuePlay()
    {
        ShowItemHandle();

    }
    private GameObject modle = null;
    private GameObject go = null;
    private int _index = 0;

    private UILotteryItem[] aaa;


    //private List<string> iconNameList = new List<string>();
    /// <summary>
    /// 控制出动画的时候的播放动画
    /// </summary>
    /// <param name="go"></param>
    /// <param name="index"></param>
    public void IsShowAnima(int index)
    {
        _index = index;
        IsShow();
    }

    //protected override void OnRelease()
    //{
    //    base.OnRelease();
    //    multList.Release();
    //}
    /// <summary>
    /// 显示兑换碎片
    /// </summary>
    public void IsShowDebris(int index)
    {
        _index = index;
        ShowItemHandle();
    }
    public void ShowItemHandle()
    {
        if (aaa == null)
            aaa = multList.GetComponentsInChildren<UILotteryItem>(true);
        if (_index >= aaa.Length - 1)
        {
            go = null;
            aaa = null;
            _index = 0;
            return;
        }
        else
        {
            _index += 1;
        }
        DelayTime();
    }
    private void ShowInfo()
    {
        aaa[_index].gameObject.SetActive(true);
    }
    public void DelayTime()
    {
        Invoke("ShowInfo", 1f / 5f);

    }
    public UIResultLottery()
    {
        instance = this;
    }
    #endregion
    private void OnOKClick()
    {
        if (GameLibrary.isShow)
        {
            return;
        }
        multList.transform.parent = view.parent;
        multList.gameObject.SetActive(true);
        Control.HideGUI();
        Control.HideGUI(UIPanleID.UILottryEffect);

        sprite.transform.gameObject.SetActive(false);
        HeroPosEmbattle.instance.HideLottryAnimaEffect();
    }
    public override UIPanleID GetUIKey()
    {
        return UIPanleID.UIResultLottery;
    }
    /// <summary>
    /// 购买数量，货币类型1金币2钻石，价格，购买药剂名称,图标名，物品品质，药水ID
    /// </summary>
    /// <param name="buyCount"></param>
    /// <param name="moneyType"></param>
    /// <param name="price"></param>
    protected override void SetUI(params object[] uiParams)
    {
        if (uiParams.Length > 0)
        {
            mBuyCount = (int)uiParams[0];
            mLotteryType = (LotteryType)uiParams[1];
            mPrice = int.Parse(uiParams[2].ToString());
            mName = uiParams[3].ToString();
            mIconName = uiParams[4].ToString();
            mQuality = uiParams[5].ToString();
            mItemID = long.Parse(uiParams[6].ToString());
        }     
        base.SetUI(uiParams);
    }

    protected override void OnLoadData()
    {
        Singleton<Notification>.Instance.RegistMessageID(MessageID.common_lucky_gamble_ret, this.GetUIKey());
        base.OnLoadData();
    }

    public override void ReceiveData(uint messageID)
    {
        Show();
        base.ReceiveData(messageID);
    }

    private void OnBuyClick()
    {
        if (GameLibrary.isShow)
        {
            return;
        }


        if (mLotteryType == LotteryType.GoldLottery)
        {
            if (playerData.GetInstance().baginfo.gold > mPrice)
            {
                GameLibrary.isShow = true;
                HeroPosEmbattle.instance.ShowLottryAnimaEffect(PosType.Lorry, mLotteryType, mBuyCount, CostType.Cost);
                Control.HideGUI(UIPanleID.UIMoney);
                Control.ShowGUI(UIPanleID.UILottryEffect, EnumOpenUIType.DefaultUIOrSecond);
                object[] obj = new object[7] { mBuyCount, mLotteryType, mPrice, mName, mIconName, mQuality, mItemID };
                Control.ShowGUI(UIPanleID.UIResultLottery, EnumOpenUIType.OpenNewCloseOld, false, obj);
            }
            else
            {
                Control.ShowGUI(UIPanleID.UIGoldHand, EnumOpenUIType.DefaultUIOrSecond);
            }
        }
        else
        if (mLotteryType == LotteryType.DiamondLottery)
        {
            if (playerData.GetInstance().baginfo.diamond > mPrice)
            {
                GameLibrary.isShow = true;
                HeroPosEmbattle.instance.ShowLottryAnimaEffect(PosType.Lorry, mLotteryType, mBuyCount, CostType.Cost);
                Control.HideGUI(UIPanleID.UIMoney);
                Control.ShowGUI(UIPanleID.UILottryEffect, EnumOpenUIType.DefaultUIOrSecond);
                object[] obj = new object[7] { mBuyCount, mLotteryType, mPrice, mName, mIconName, mQuality, mItemID };
                Control.ShowGUI(UIPanleID.UIResultLottery, EnumOpenUIType.OpenNewCloseOld, false, obj);
            }
            else
            {
                //UIPromptBox.Instance.ShowLabel("您的钻石不足请充值");
                Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, "您的钻石不足请充值");
            }
        }
        else if (mLotteryType == LotteryType.LotterySoul)
        {
            if (playerData.GetInstance().baginfo.diamond > mPrice)
            {
                GameLibrary.isShow = true;
                HeroPosEmbattle.instance.ShowLottryAnimaEffect(PosType.Lorry, mLotteryType, mBuyCount, CostType.Cost);
                Control.HideGUI(UIPanleID.UIMoney);
                Control.ShowGUI(UIPanleID.UILottryEffect, EnumOpenUIType.DefaultUIOrSecond);
                object[] obj = new object[7] { mBuyCount, mLotteryType, mPrice, mName, mIconName, mQuality, mItemID };
                Control.ShowGUI(UIPanleID.UIResultLottery, EnumOpenUIType.OpenNewCloseOld, false, obj);
            }
            else
            {
                //UIPromptBox.Instance.ShowLabel("您的钻石不足请充值");
                Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, "您的钻石不足请充值");
            }
        }
    }
    /// <summary>
    /// 接收服务器数据
    /// </summary>
    public object[] InitItemData()
    {
        GameLibrary.lotteryCount = playerData.GetInstance().lotteryInfo.itemList.Count;
        return playerData.GetInstance().lotteryInfo.itemList.ToArray();
    }
    #region 初始化UI显示
    protected override void ShowHandler()
    {
       // Control.HideGUI(UIPanleID.UILottryEffect);
        sprite.transform.gameObject.SetActive(false);
        priceTxt.text = mPrice.ToString();
        nameTxt.text = mName;
        PlayEffect.mActive = true;
        //mEffect.Name = "6_000";
        countTxt.text = mBuyCount.ToString();
        buyBtn.text = "购买" + mBuyCount + "个";
        if (mBuyCount == 10)
        {
            sprite.transform.gameObject.SetActive(true);
        }
        if (mLotteryType == LotteryType.GoldLottery)
        {
            moneySprite.spriteName = "jinbi";
            icon.spriteName = mIconName;
            quality.spriteName = mQuality;
        }
        else if (mLotteryType == LotteryType.DiamondLottery)
        {
            moneySprite.spriteName = "zuanshi";
            icon.spriteName = mIconName;
            quality.spriteName = mQuality;
        }
        else if (mLotteryType == LotteryType.LotterySoul)
        {
            moneySprite.spriteName = "zuanshi";
            icon.spriteName = mIconName;
            quality.spriteName = mQuality;
        }
        CreatItemData();
    }
    #endregion
    /// <summary>
    /// 生成抽奖物品
    /// </summary>
    public void CreatItemData()
    {
        if (mBuyCount == 1)
        {
            multList.transform.GetChild(0).localPosition = new Vector3(360, -42, 0);
            multList.InSize(InitItemData().Length, 5);
            multList.Info(InitItemData());
        }
        else if (mBuyCount == 10)
        {
            multList.transform.GetChild(0).localPosition = new Vector3(47, -42, 0);
            multList.InSize(InitItemData().Length, 5);
            multList.Info(InitItemData());
        }
        else if (InitItemData().Length <= 0)
        {
            GameLibrary.isShow = false;
            Hide();
        }
        multList.ScrollView = view;
    }

    protected override void RegisterComponent()
    {
        base.RegisterComponent();
        RegisterComponentID(39, 38, oKBtnLabel);
    }
    protected override void RegisterIsOver()
    {
        base.RegisterIsOver();
    }
}
