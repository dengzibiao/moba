using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tianyu;

public class HeroUpgradeItem : GUISingleItemList
{
    public UISprite icon;        //英雄原画
    public UISprite border;             //边框
    //public UILabel qualityLabel;        //品质等级
    public UILabel levelLabel;          //等级
    public UILabel nameLabel;           //名字
    public UISlider expSlider;
    public UILabel expLabel;
                                        
    List<UISprite> startList = new List<UISprite>();//星级List
    HeroData heroData;              //英雄信息
    HeroUpgradeVO huVO;             //每级所需经验
    int useCount = 0;               //使用个数
    int upGradelvl = 0;             //所升级数
    int counts;                     //剩余经验值
    int exp;                        //药品经验值

    float userRate = 0.5f;          //药品使用速率

    HeroNode heroVo;                  //当前选择的英雄
    bool isOnClick = false;
    /// <summary>
    /// item初始化函数
    /// </summary>
    protected override void InitItem()
    {
        //初始化
        icon = transform.Find("Icon").GetComponent<UISprite>();
        UIEventListener.Get(icon.gameObject).onClick = OnIconClick;
        UIEventListener.Get(icon.gameObject).onPress = OnPressIconClick;

        border = transform.Find("Border").GetComponent<UISprite>();
        //qualityLabel = transform.Find("QualityLabel").GetComponent<UILabel>();
        levelLabel = transform.Find("LevelLabel").GetComponent<UILabel>();
        nameLabel = transform.Find("NameLabel").GetComponent<UILabel>();
        expSlider = transform.Find("expSlider").GetComponent<UISlider>();
        expLabel = transform.Find("expSlider/Label").GetComponent<UILabel>();
        for (int i = 1; i <= 5; i++)
        {
            startList.Add(transform.Find("Star" + i).GetComponent<UISprite>());
        }
        //EventDelegate.Set(mask.onClick, this.OnMackBtn);
        //Debug.LogError(Globe.selectHero.hero_id);
        //heroData = playerData.GetInstance().GetHeroDataByID(Globe.selectHero.hero_id);
        //GetComponent<BoxCollider>().enabled = false;


    }
    /// <summary>
    /// 单击吃一次药水
    /// </summary>
    /// <param name="go"></param>
    private void OnIconClick(GameObject go)
    {
        Globe.selectHero = heroVo;
        heroData = playerData.GetInstance().GetHeroDataByID(Globe.selectHero.hero_id);
        isOnClick = true;
        //Globe.selectHero.hero_id
        if (Globe.currentCount <= 0)
        {
            //UIPromptBox.Instance.ShowLabel("药品个数为零");
            Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, "药品个数为零");
            return;
        }
        if (heroData.lvl >= playerData.GetInstance().selfData.level)
        {
            //UIPromptBox.Instance.ShowLabel("请提升玩家等级");
            Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, "请提升玩家等级");
            return;
        }


        Globe.currentCount -= 1;
        useCount++;

        counts = heroData.exps;
        //exp = FSDataNodeTable<ItemNode>.GetSingleton().FindDataByType(GoodsDetials.instance.currentSItemData.Id).exp_gain;
        exp = (VOManager.Instance().GetCSV<ItemCSV>("Item").GetVO(GoodsDetials.instance.currentSItemData.Id)).exp_gain;
        //经验条滚动，每个药品使用的时间
        InvokeRepeating("OnSecondFunction", 0f, userRate / exp);


        //
        
    }
    /// <summary>
    /// 长按持续吃药水
    /// </summary>
    /// <param name="go"></param>
    /// <param name="state"></param>
    private void OnPressIconClick(GameObject go, bool state)
    {
        Globe.selectHero = heroVo;
        heroData = playerData.GetInstance().GetHeroDataByID(Globe.selectHero.hero_id);
        if (state)
        {
            //isContinued = true;
            //Invoke("OnMedicine", 1);

            InvokeRepeating("EatDrug", 1, userRate);

        }
        else
        {
            //Debug.LogError(state);
            //isContinued = false;
            //CancelInvoke("OnMedicine");
            CancelInvoke("EatDrug");
            //长按松开的时候 才向服务器发送信息
            if (useCount != 0)
            {

                ItemNodeState itemnodestate = null;
                if (GameLibrary.Instance().ItemStateList.ContainsKey(GoodsDetials.instance.currentSItemData.Id))
                {
                    itemnodestate = GameLibrary.Instance().ItemStateList[GoodsDetials.instance.currentSItemData.Id];
                    //ClientSendDataMgr.GetSingle().GetHeroSend().SendDrugUpgrade(heroData.id, FSDataNodeTable<ItemNode>.GetSingleton().FindDataByType(GoodsDetials.instance.currentSItemData.Id).props_id, useCount, upGradelvl);
                    //ClientSendDataMgr.GetSingle().GetHeroSend().SendDrugUpgrade(heroData.id, itemnodestate.props_id, useCount, upGradelvl);
                    GoodsDataOperation.GetInstance().UseGoods((int)GoodsDetials.instance.currentSItemData.Id, useCount);
                }
               
            }
            upGradelvl = 0;
            useCount = 0;
        }

    }
    /// <summary>
    /// 吃药方法
    /// </summary>
    void EatDrug()
    {
        if (Globe.currentCount <= 0)
        {
            print("药品个数为零");
            //Control.ShowGUI(GameLibrary.UIPromptBox);
            //UIPromptBox.Instance.ShowLabel("药品个数为零");
            Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, "药品个数为零");
            CancelInvoke("EatDrug");
            return;
        }

        if (heroData.lvl >= playerData.GetInstance().selfData.level)
        {
            //Control.ShowGUI(GameLibrary.UIPromptBox);
            //UIPromptBox.Instance.ShowLabel("请提升玩家等级");
            Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, "请提升玩家等级");
            CancelInvoke("EatDrug");
            return;
        }

        Globe.currentCount -= 1;
        useCount++;

        counts = heroData.exps;
        //exp = FSDataNodeTable<ItemNode>.GetSingleton().FindDataByType(GoodsDetials.instance.currentSItemData.Id).exp_gain;
        exp = (VOManager.Instance().GetCSV<ItemCSV>("Item").GetVO(GoodsDetials.instance.currentSItemData.Id)).exp_gain;
        //经验条滚动，每个药品使用的时间
        InvokeRepeating("OnSecondFunction", 0f, userRate / (VOManager.Instance().GetCSV<ItemCSV>("Item").GetVO(GoodsDetials.instance.currentSItemData.Id)).exp_gain);
        //InvokeRepeating("OnSecondFunction", 0f, userRate / FSDataNodeTable<ItemNode>.GetSingleton().FindDataByType(GoodsDetials.instance.currentSItemData.Id).exp_gain);
    }

    /// <summary>
    /// 经验条滚动
    /// </summary>
    void OnSecondFunction()
    {

        exp -= 1;
        counts++;

        //更新经验条信息
        expSlider.value = 1f * counts / heroData.maxExps;
        expLabel.text = Mathf.CeilToInt((1f * counts / heroData.maxExps) * heroData.maxExps) + "/" + heroData.maxExps;
        levelLabel.text = "Lv." + heroData.lvl;

        if ((1f * counts / heroData.maxExps) >= 1f)
        {
            counts = 0;


            heroData.lvl++;
            upGradelvl++;
            //heroData.maxExps = FSDataNodeTable<HeroUpGradeNode>.GetSingleton().FindDataByType(heroData.lvl).exp;
            heroData.maxExps = VOManager.Instance().GetCSV<HeroUpgradeCSV>("HeroUpgrade").GetVO(heroData.lvl).exp;
            expSlider.value = 0;
            expLabel.text = exp + "/" + heroData.maxExps;
            levelLabel.text = "Lv." + heroData.lvl;

            if (heroData.lvl >= playerData.GetInstance().selfData.level)
            {
                heroData.exps = 0;

                expSlider.value = 0;
                expLabel.text = heroData.exps + "/" + heroData.maxExps;
                levelLabel.text = "Lv." + heroData.lvl;

                CancelInvoke("EatDrug");
                CancelInvoke("OnSecondFunction");
            }
        }

        if (exp < 1)
        {
            //点击 进度条走完之后再像服务器发送
            if (isOnClick)
            {
                ItemNodeState itemnodestate = null;
                if (GameLibrary.Instance().ItemStateList.ContainsKey(GoodsDetials.instance.currentSItemData.Id))
                {
                    itemnodestate = GameLibrary.Instance().ItemStateList[GoodsDetials.instance.currentSItemData.Id];
                    //ClientSendDataMgr.GetSingle().GetHeroSend().SendDrugUpgrade(heroData.id, FSDataNodeTable<ItemNode>.GetSingleton().FindDataByType(GoodsDetials.instance.currentSItemData.Id).props_id, useCount, upGradelvl);
                    //ClientSendDataMgr.GetSingle().GetHeroSend().SendDrugUpgrade(heroData.id, itemnodestate.props_id, useCount, upGradelvl);
                    GoodsDataOperation.GetInstance().UseGoods((int)GoodsDetials.instance.currentSItemData.Id, useCount);
                }
                    
                isOnClick = false;
                upGradelvl = 0;
                useCount = 0;
            }
            heroData.exps = counts;
            CancelInvoke("OnSecondFunction");
        }

    }
    /// <summary>
    /// 信息赋值
    /// </summary>
    /// <param name="obj"></param>
    public override void Info(object obj)
    {
        heroData = (HeroData)obj;
        heroVo = FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList[heroData.id];
        nameLabel.text = heroVo.name;
        levelLabel.text = "Lv." + heroData.lvl;
        icon.spriteName = heroVo.original_painting;
        for (int i = 0; i < 5; i++)
        {
            startList[i].spriteName = i < heroData.star ? "xingxing" : "xing-hui";
        }
        expSlider.value = (1f) * heroData.exps / heroData.maxExps;
        expLabel.text = heroData.exps + "/" + heroData.maxExps;

    }
}