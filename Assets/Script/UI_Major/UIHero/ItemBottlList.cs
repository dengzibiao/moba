using System.Collections;
using UnityEngine;
using Tianyu;
using System;
using System.Collections.Generic;


public class propsData
{
    public long id;
    public long num;
}

/// <summary>
/// 药品
/// </summary>
public class ItemBottlList : GUISingleItemList
{
    /// <summary>
    /// 单例
    /// </summary>
    private static ItemBottlList mSingleton;
    public static ItemBottlList Instance()
    {
        if (mSingleton == null)
            mSingleton = new ItemBottlList();
        return mSingleton;
    }
    bool isUse = false;             //是否使用药水
    #region 字段
    UI_HeroDetail hd_ins;
    public UIButton mailBtn;        //药品
    public UILabel count;           //数量
    public UILabel num;           //数量

    public HeroNode hero;

    ItemNodeState itemNode;                //药品数据
    HeroData hd;                    //英雄信息
    HeroUpGradeNode heroUpNode;     //每级所需经验
    PlayerLevelUpNode playerNode;

    public static List<propsData> propslist = new List<propsData>();
    int currentCount = 0;           //当前个数
    int useCount = 0;               //使用个数
    int upGradelvl = 0;             //所升级数
    int counts;                     //剩余经验值
    int exp;                        //药品经验值

    float userRate = 0.5f;          //药品使用速率

    int currentDrug = 0;                    //当前药品

    bool isComplete = false;
    bool isStop = false;
    int useCounts = 0;               //使用个数
    int upGradelvls = 0;             //所升级数
    int consumeCount = 0;           //消耗的个数
    int currentEXP = 0;             //当前经验值
    int countss;
    int exps;

    int expDiffe;//经验差值

    #endregion

    /// <summary>
    /// item初始化函数
    /// </summary>
    protected override void InitItem()
    {
        hd_ins = UI_HeroDetail.instance;
        //初始化
        mailBtn = transform.Find("MailBtn").GetComponent<UIButton>();

        UIEventListener.Get(mailBtn.gameObject).onClick += OnBtnClick;
        UIEventListener.Get(mailBtn.gameObject).onPress += OnPressBtnClick;

        count = transform.Find("Count").GetComponent<UILabel>();

        hd = playerData.GetInstance().GetHeroDataByID(Globe.selectHero.hero_id);
        hero = hd.node;
        playerNode = FSDataNodeTable<PlayerLevelUpNode>.GetSingleton().FindDataByType(playerData.GetInstance().selfData.level);
    }

    /// <summary>
    /// 信息赋值
    /// </summary>
    /// <param name="obj"></param>
    public override void Info(object obj)
    {

        if (obj == null)
        {
            mailBtn.gameObject.SetActive(false);
            count.enabled = false;
        }
        else
        {

            itemNode = GameLibrary.Instance().ItemStateList[((ItemData)obj).Id];

            mailBtn.GetComponent<UISprite>().spriteName = itemNode.icon_name;
            count.text = playerData.GetInstance().GetItemCountById(itemNode.props_id) + "";
            currentCount = playerData.GetInstance().GetItemCountById(itemNode.props_id);
            num.text = "+" + GameLibrary.Instance().ItemStateList[itemNode.props_id].exp_gain.ToString() + "经验";
            propsData props = new propsData();
            props.id = itemNode.props_id;
            props.num = GameLibrary.Instance().ItemStateList[itemNode.props_id].exp_gain * playerData.GetInstance().GetItemCountById(itemNode.props_id);
            propslist.Add(props);
            //propsDic.Add(itemNode.props_id, GameLibrary.Instance().ItemStateList[itemNode.props_id].exp_gain * playerData.GetInstance().GetItemCountById(itemNode.props_id));
        }
    }

    /// <summary>
    /// 点击药品，弹出升级面板
    /// </summary>
    /// <param name="go"></param>
    private void OnBtnClick(GameObject go)
    {
        //UpgradePanelX.instance.item = itemNode;
        //UpgradePanelX.instance.hero= Globe.selectHero;
        //Control.ShowGUI(GameLibrary.UIUpgradePanel);
        //UI_HeroDetail.instance.UpGradeStar.OpenUpGradePanel(itemNode, hd);
        OnUpgradeBtnClick();
    }
    /// <summary>
    /// 升级按钮
    /// </summary>
    private void OnUpgradeBtnClick()
    {
        //hd_ins.UpGradeStar.RecordUseDrug(hd, item);
        //upgradeBtn.GetComponent<BoxCollider>().enabled = false;
        //aKeyUpgradeBtn.GetComponent<BoxCollider>().enabled = false;
        //mask.gameObject.SetActive(true);
        currentDrug = playerData.GetInstance().GetItemCountById(itemNode.props_id);
        playerNode.heroLvLimit = 10;
        if (hd.lvl >= playerNode.heroLvLimit)
        {
            PromptPanel.instance.ShowPrompt("请提升玩家等级等级");
            return;
        }
        if (currentDrug <= 0)
        {
            PromptPanel.instance.ShowPrompt("药品个数为0");
            return;
        }
        currentDrug--;
        GoodsDataOperation.GetInstance().UseGoods((int)itemNode.props_id, 1);
        //if (currentDrug <= 0)
        //{
        //    playerData.GetInstance().drugDic.Remove(item.props_id);
        //    playerData.GetInstance().drugKeys.Remove(item.props_id);
        //}
        UI_HeroDetail.instance.UpGradeStar.RefreshDrug(hd);
        hd_ins.countD += 1;
        useCount++; //计算使用个数
        //开启向服务器发送条件，记录使用情况
        isUse = true;
        //hd_ins.isSendDrug = true;
        hd_ins.heroIDD = hero.hero_id;
        hd_ins.itemIDD = itemNode.props_id;
        hd_ins.countD++;
        //if (currentDrug <= 0) bottlCount.text = "";
        //else bottlCount.text = currentDrug.ToString();
        //currentEXP = hd.exps;
        UpGradeLevels();
        ////记录使用的药品和个数
        //if (hd_ins.conDrugDic.ContainsKey(item.props_id))
        //{
        //    hd_ins.conDrugDic.Remove(item.props_id);
        //}

        //hd_ins.conDrugDic.Add(item.props_id, useCount);
        UI_HeroDetail.instance.NameArea.RefreshUI(hd);
    }

    /// <summary>
    /// 持续点击
    /// </summary>
    /// <param name="go"></param>
    /// <param name="state"></param>
    private void OnPressBtnClick(GameObject go, bool state)
    {
        if (currentCount <= 0)
        {
            return;
        }
        isStop = state;
        if (state)
        {
            InvokeRepeating("EatDrug", 1, userRate);
            if (!UI_HeroDetail.instance.isSendDrug)
                UI_HeroDetail.instance.isSendDrug = true;
            isComplete = true;
            useCount = 0;
        }
        else
        {
            if (useCount != 0 && isComplete)
            {
                UI_HeroDetail.instance.UpGradeStar.SetMaskPanel(true);
            }
            CancelInvoke("EatDrug");
        }
    }

    /// <summary>
    /// 吃药方法
    /// </summary>
    void EatDrug()
    {
        if (currentCount <= 0)
        {
            PromptPanel.instance.ShowPrompt("药品个数为零");
            CancelInvoke("EatDrug");
            isComplete = false;
            //ClientSendDataMgr.GetSingle().GetHeroSend().SendDrugUpgrade(Globe.selectHero.hero_id, itemNode.props_id, useCount, upGradelvl);
            GoodsDataOperation.GetInstance().UseGoods((int)itemNode.props_id, useCount);
            UI_HeroDetail.instance.UpGradeStar.RefreshDrug(hd);
            return;
        }
        hd = playerData.GetInstance().GetHeroDataByID(Globe.selectHero.hero_id);
        if (hd.lvl >= playerNode.heroLvLimit)
        {
            PromptPanel.instance.ShowPrompt("请提升玩家等级");
            CancelInvoke("EatDrug");
            isComplete = false;
            //ClientSendDataMgr.GetSingle().GetHeroSend().SendDrugUpgrade(Globe.selectHero.hero_id, itemNode.props_id, useCount, upGradelvl);
            GoodsDataOperation.GetInstance().UseGoods((int)itemNode.props_id, useCount);
            UI_HeroDetail.instance.UpGradeStar.RefreshDrug(hd);
            return;
        }
        currentCount -= 1;
        useCount++;
        count.text = currentCount.ToString();
        counts = hd.exps;
        exp = itemNode.exp_gain;
        //经验条滚动，每个药品使用的时间
        UI_HeroDetail.instance.UpGrade(0f, userRate / itemNode.exp_gain, itemNode, playerNode);
    }
    /// <summary>
    /// 升级函数
    /// </summary>
    void UpGradeLevels()
    {
        UI_HeroDetail.instance.UpGrade(0f, 0.02f, itemNode, playerNode);

    }
    public void StopEatDrug()
    {
        if (isStop) return;
        UI_HeroDetail.instance.UpGradeStar.SetMaskPanel(false);
        if (useCount != 0)
        {
            //ClientSendDataMgr.GetSingle().GetHeroSend().SendDrugUpgrade(Globe.selectHero.hero_id, itemNode.props_id, useCount, upGradelvl);
            GoodsDataOperation.GetInstance().UseGoods((int)itemNode.props_id, useCount);
            UI_HeroDetail.instance.UpGradeStar.RefreshDrug(hd);
            upGradelvl = 0;
            useCount = 0;
        }
    }

    /// <summary>
    /// 升级函数
    /// </summary>
    void UpGradeLevel()
    {
        if (hd.exps >= hd.maxExps)
        {
            hd.exps -= hd.maxExps;

            hd.lvl++;

            upGradelvl++;

            heroUpNode = FSDataNodeTable<HeroUpGradeNode>.GetSingleton().FindDataByType(hd.lvl);

            hd.maxExps = heroUpNode.exp;

            UpGradeLevel();
        }
        else
        {
            return;
        }
    }


    /// <summary>
    /// 经验条缓动
    /// </summary>
    //void OnSecondFunctions()
    //{
    //    exp -= expDiffe;
    //    count += expDiffe;

    //    expBar.value = 1f * count / hd.maxExps;
    //    expBar.transform.Find("Label").GetComponent<UILabel>().text = Mathf.CeilToInt(expBar.value * hd.maxExps) + "/" + hd.maxExps;

    //    UI_HeroDetail.instance.NameArea.ChangeExpBar(expBar.value, Mathf.CeilToInt(expBar.value * hd.maxExps), hd.maxExps, hd.lvl);

    //    if (expBar.value >= 1f)
    //    {
    //        count = 0;
    //        hd.lvl++;
    //        hd.fc = HeroData.GetFC(hd);
    //        upGradelvl++;
    //        lvF.text = "Lv." + hd.lvl;
    //        lvR.text = "Lv." + (hd.lvl + 1);
    //        hd.maxExps = FSDataNodeTable<HeroUpGradeNode>.GetSingleton().FindDataByType(hd.lvl).exp;
    //        if (hd.lvl >= playerNode.heroLvLimit)
    //        {
    //            hd.exps = 0;
    //            expBar.value = 0;
    //            expBar.transform.Find("Label").GetComponent<UILabel>().text = hd.exps + "/" + hd.maxExps;
    //            UI_HeroDetail.instance.NameArea.ChangeExpBar(expBar.value, Mathf.CeilToInt(expBar.value * hd.maxExps), hd.maxExps, hd.lvl);
    //            CancelInvoke("OnSecondFunction");
    //            upgradeBtn.GetComponent<BoxCollider>().enabled = true;
    //            aKeyUpgradeBtn.GetComponent<BoxCollider>().enabled = true;
    //            mask.gameObject.SetActive(false);
    //        }
    //    }

    //    if (exp < 1)
    //    {
    //        hd.exps = count;
    //        aKeyUpgradeBtn.GetComponent<BoxCollider>().enabled = true;
    //        CancelInvoke("OnSecondFunction");
    //        upgradeBtn.GetComponent<BoxCollider>().enabled = true;
    //        mask.gameObject.SetActive(false);
    //    }

    //}

}