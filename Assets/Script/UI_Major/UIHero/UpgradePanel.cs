using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

/// <summary>
/// 英雄升级面板
/// </summary>

public class UpgradePanel : GUIBase
{

    public delegate void DeleUpgrade(UpgradeItem item, bool state);
    public delegate void DeleUpgradeOne(UpgradeItem item);

    //DeleUpgrade m_DeleUpgrade = null;
    //DeleUpgradeOne m_DeleUpgradeOne = null;

    public GUISingleLabel eXPLabel;

    #region MyRegion
    public static UpgradePanel instance;
    //private UIButton BtnClose;
    private UISprite SprIcon;
    private UILabel LabName;
    private UILabel LabLv;
    private UISlider DerExp;

    private List<UpgradeItem> LisUpgradeItem = new List<UpgradeItem>();

    HeroNode vo;
    PlayerInfo play;

    bool isOne = false;

    #endregion



    protected override void Init()
    {
        base.Init();
        instance = this;

        //BtnClose = UnityUtil.FindCtrl<UIButton>(gameObject, "CloseBtn");

        //EventDelegate.Set(BtnClose.onClick, this.OnCloseBtn);

        SprIcon = UnityUtil.FindCtrl<UISprite>(gameObject, "Icon");
        LabName = UnityUtil.FindCtrl<UILabel>(gameObject, "Name");
        LabLv = UnityUtil.FindCtrl<UILabel>(gameObject, "Lv");

        DerExp = UnityUtil.FindCtrl<UISlider>(gameObject, "ExpBar");

        Globe.allHeroDic.TryGetValue(Globe.selectHero.hero_id, out vo);
        Globe.heroInfoDic.TryGetValue(Globe.selectHero.hero_id, out play);

        GameObject BottleMultList = UnityUtil.FindObjectRecursively(gameObject, "BottleMultList");

        for (int i = 0; i < 6; i++)
        {
            GameObject go = UnityUtil.FindObjectRecursively(BottleMultList, "item" + i.ToString());
            UpgradeItem item = new UpgradeItem();
            item.goItem = go;
            item.SprBg = go.GetComponent<UISprite>();
            item.SprIcon = UnityUtil.FindCtrl<UISprite>(go, "MailBtn");
            item.LabCount = UnityUtil.FindCtrl<UILabel>(go, "Count");
            item.LabExpNumber = UnityUtil.FindCtrl<UILabel>(go, "EXPLabel");

            LisUpgradeItem.Add(item);
        }

        SprIcon.spriteName = vo.icon_name;

        //TODO dele
        HeroData1 data = new HeroData1();
        data.Name = vo.name;
        data.Lv = play.Level;
        data.ExpMax = 1000 * play.Level;
        data.ExpNow = play.CurrentExp;
        Open(data);

        isOne = true;
    }
    /// <summary>
    /// 临时数据 玩家英雄数据
    /// </summary>
    HeroData1 m_HeroData = null;


    protected override void ShowHandler()
    {
        base.ShowHandler();

        if (isOne)
        {
            Globe.allHeroDic.TryGetValue(Globe.selectHero.hero_id, out vo);
            Globe.heroInfoDic.TryGetValue(Globe.selectHero.hero_id, out play);

            SprIcon.spriteName = vo.icon_name;

            HeroData1 data = new HeroData1();
            data.Name = vo.name;
            data.Lv = play.Level;
            data.ExpMax = 1000 * play.Level;
            data.ExpNow = play.CurrentExp;
            Open(data);

        }

    }


    /// <summary>
    /// 初始化接口
    /// </summary>
    /// <param name="data"></param>
    public void Open(HeroData1 data)
    {
        m_HeroData = data;
        if (GoodsDataManager.Instance.GetGoodsList().Count != LisUpgradeItem.Count) return;
        RefreshHeroInfo(data);

        for (int i = 0; i < LisUpgradeItem.Count; i++)
        {
            LisUpgradeItem[i].Init(GoodsDataManager.Instance.GetGoodsList()[i], UpgradeItemOneCallBack, UpgradeItemCallBack);
        }
    }

    /// <summary>
    /// 吃药瓶 （one time）回调
    /// </summary>
    /// <param name="item"></param>
    public void UpgradeItemOneCallBack(UpgradeItem item)
    {
        if (m_UpItem.m_Count <= 0) return;
        m_UpItem = item;
        AddExp();
    }

    /// <summary>
    /// 消耗品对象（GameObject）
    /// </summary>
    UpgradeItem m_UpItem = null;
    bool isState = false;

    /// <summary>
    /// 吃药瓶（ 持续 ）回调
    /// </summary>
    /// <param name="item"></param>
    /// <param name="state"></param>
    public void UpgradeItemCallBack(UpgradeItem item, bool state)
    {
        m_UpItem = item;
        isState = state;

        if (state)
        {
            Invoke("CallBack", 0.1f);
        }
    }

    private void CallBack()
    {
        if (isState)
        {
            if (m_UpItem.m_Count <= 0) return;
            AddExp();
            Invoke("CallBack", 0.1f);
        }
    }

    /// <summary>
    /// 增加经验
    /// </summary>
    private void AddExp()
    {
        if (m_UpItem == null) return;
        m_UpItem.Consumption();
        m_HeroData.ExpNow += m_UpItem.m_GoodsData.ExpNumber;
        play.CurrentExp = m_HeroData.ExpNow;
        CalculationLevel();
        RefreshHeroInfo(m_HeroData);
    }

    /// <summary>
    /// 递归
    /// </summary>
    private void CalculationLevel()
    {
        if (m_HeroData.ExpNow >= m_HeroData.ExpMax)
        {
            m_HeroData.ExpNow -= m_HeroData.ExpMax;
            m_HeroData.Lv++;
            play.Level = m_HeroData.Lv;
            //UI_HeroDetail.instance.UpdateLevel();
            m_HeroData.ExpMax = 1000 * m_HeroData.Lv;
            CalculationLevel();
        }
    }

    /// <summary>
    /// 刷新英雄数据
    /// </summary>
    private void RefreshHeroInfo(HeroData1 data)
    {
        LabName.text = data.Name;
        LabLv.text = "[FFFF00]Lv.[-]" + data.Lv.ToString();
        eXPLabel.text = data.ExpNow + "/" + data.ExpMax;
        DerExp.value = data.ExpNow / (float)data.ExpMax;
    }

    /// <summary>
    /// 关闭按钮
    /// </summary>
    private void OnCloseBtn()
    {
        Control.HideGUI(UIPanleID.UIUpgradePanel);
    }

    public override UIPanleID GetUIKey()
    {
        return UIPanleID.none;
    }


    new void OnDestroy()
    {
        //m_DeleUpgrade = null;
        //m_DeleUpgradeOne = null;
        //BtnClose = null;
        //BtnClose = null;
        LabName = null;
        LabLv = null;
        DerExp = null;

        LisUpgradeItem.Clear();
        m_HeroData = null;
        m_HeroData = null;

        System.GC.Collect();
    }

}

/// <summary>
/// 物品Item数据
/// </summary>
public class UpgradeItem
{
    public GameObject goItem;
    public UISprite SprIcon;
    public UILabel LabCount;
    public UILabel LabExpNumber;
    public UISprite SprBg;


    public int m_Count = 0;
    UpgradePanel.DeleUpgrade m_DeleUpgrade = null;
    UpgradePanel.DeleUpgradeOne m_DeleUpgradeOne = null;


    public GoodsData m_GoodsData = null;

    /// <summary>
    /// 物品初始化接口
    /// </summary>
    /// <param name="data"></param>
    /// <param name="deleOne"></param>
    /// <param name="dele"></param>
    public void Init(GoodsData data, UpgradePanel.DeleUpgradeOne deleOne, UpgradePanel.DeleUpgrade dele)
    {
        m_GoodsData = data;
        m_DeleUpgrade = dele;
        m_DeleUpgradeOne = deleOne;
        UIEventListener.Get(goItem).onClick = this.BtnOnCLick;
        UIEventListener.Get(goItem).onPress = this.BtnOnPress;

        SprIcon.spriteName = data.Name;
        m_Count = data.Count;
        LabCount.text = m_Count.ToString();
        LabExpNumber.text = "+" + data.ExpNumber.ToString();
    }

    public void BtnOnCLick(GameObject go)
    {
        if (m_DeleUpgradeOne != null)
            m_DeleUpgradeOne(this);
    }

    public void BtnOnPress(GameObject go, bool state)
    {
        if (m_DeleUpgrade != null)
            m_DeleUpgrade(this, state);
    }

    /// <summary>
    /// 消耗药瓶(刷新)
    /// </summary>
    /// <param name="count"></param>
    public void Consumption()
    {
        m_Count--;
        LabCount.text = m_Count.ToString();
        if (m_Count > 0)
        {
            SprBg.color = Color.white;
        }
        else
        {
            SprBg.color = Color.red;
            m_Count = 0;
        }
    }
}