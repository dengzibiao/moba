using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Tianyu;

/// <summary>
/// 英雄列表中英雄卡片
/// </summary>
public class ItemHeroList : GUISingleItemList
{

    public GUISingleButton icon;        //英雄原画
    public UISprite border;             //边框
    public UILabel levelLabel;          //等级
    public UILabel nameLabel;           //名字
    public UISprite play;               //出战
    UISprite mask;                      //遮罩
    UISprite typeS;                     //英雄类型
    UISprite suo;                       //锁
    UILabel souStoneL;                  //魂石数
    UIButton summonBtn;                 //召唤按钮

    //星级List
    List<UISprite> startList = new List<UISprite>();

    HeroNode heroNode;
    UIHeroList herolist;
    HeroData hd = null;             //英雄信息
    StarUpGradeNode starUpNode;
    ItemData item;

    int currentSoul = 0;            //当前的魂石

    /// <summary>
    /// item初始化函数
    /// </summary>
    protected override void InitItem()
    {
        icon = transform.Find("Icon").GetComponent<GUISingleButton>();
        icon.onClick = OnIconClick;
        border = transform.Find("Border").GetComponent<UISprite>();
        levelLabel = transform.Find("LevelLabel").GetComponent<UILabel>();
        nameLabel = transform.Find("NameLabel").GetComponent<UILabel>();
        play = transform.FindComponent<UISprite>("Play");
        mask = UnityUtil.FindCtrl<UISprite>(gameObject, "Mask");
        typeS = UnityUtil.FindCtrl<UISprite>(gameObject, "Type");
        suo = UnityUtil.FindCtrl<UISprite>(gameObject, "Suo");
        souStoneL = UnityUtil.FindCtrl<UILabel>(gameObject, "SouStoneL");
        summonBtn = UnityUtil.FindCtrl<UIButton>(gameObject, "SummonBtn");

        for (int i = 1; i <= 5; i++)
        {
            startList.Add(transform.Find("Star" + i).GetComponent<UISprite>());
        }

        GetComponent<BoxCollider>().enabled = false;
        play.enabled = false;
        herolist = UIHeroList.instance;
        EventDelegate.Set(summonBtn.onClick, OnSummonBtnClick);
    }

    /// <summary>
    /// 信息赋值
    /// </summary>
    /// <param name="obj"></param>
    public override void Info(object obj)
    {

        heroNode = (HeroNode)obj;
        hd = playerData.GetInstance().GetHeroDataByID(heroNode.hero_id);
        if (!Globe.allHeroDic.ContainsKey(heroNode.hero_id))
            Globe.allHeroDic.Add(heroNode.hero_id, heroNode);
        for (int i = 0; i < playerData.GetInstance().herodataList.Count; i++)
        {
            if (heroNode.hero_id == playerData.GetInstance().herodataList[i].id)
            {
                heroNode.isHas = true;
            }
        }

        //获取背包中的魂石
        item = playerData.GetInstance().GetItemDatatByID(heroNode.soul_gem);
        currentSoul = null == item ? 0 : item.Count;

        if (heroNode.isHas == false)
        {
            icon.GetComponent<UISprite>().color = new Color(0, 0, 0);
            //icon.enabled = false;
            suo.enabled = true;
            mask.enabled = true;
            mask.GetComponent<BoxCollider>().enabled = false;
            mask.alpha = 0.1f;
            for (int i = 0; i < startList.Count; i++)
            {
                startList[i].spriteName = i < heroNode.init_star ? "xing" : "xing-hui";
            }
            starUpNode = FSDataNodeTable<StarUpGradeNode>.GetSingleton().FindDataByType(heroNode.init_star);
            if (currentSoul >= starUpNode.call_stone_num)
                summonBtn.gameObject.SetActive(true);
            souStoneL.text = "[3BE6FFFF]" + currentSoul + "[-]/" + starUpNode.call_stone_num;
        }
        else
        {
            mask.enabled = false;
            int star = hd.star + 1;
            if (star > 5)
                star = 5;
            starUpNode = FSDataNodeTable<StarUpGradeNode>.GetSingleton().FindDataByType(star);
            for (int i = 0; i < 5; i++)
            {
                startList[i].spriteName = i < hd.star ? "xing" : "xing-hui";
            }
            icon.GetComponent<UISprite>().color = new Color(1, 1, 1);
            icon.enabled = true;
            souStoneL.text = "[3BE6FFFF]" + currentSoul + "[-]/" + starUpNode.evolve_stone_num;
        }
        if (hd != null)
        {
            border.spriteName = GoodsDataOperation.GetInstance().GetHeroGrameByHeroGrade(hd.grade);
        }
        else
        {
            border.spriteName = "baikuang";
        }
        levelLabel.text = !heroNode.isHas ? "" : hd.lvl + "级";
        nameLabel.text = heroNode.name;
        icon.spriteName = heroNode.original_painting;
       

        ShowType(heroNode.attribute);

        Globe.isC = true;
    }

    void OnEnable()
    {
        if (Globe.isC)
        {
            for (int i = 1; i <= 3; i++)
            {
                if (Globe.playHeroList[i] == null)
                {
                    return;
                }
            }
        }
    }

    /// <summary>
    /// 点击选择英雄卡片
    /// </summary>
    private void OnIconClick()
    {
        //if (!heroNode.isHas) return;

        Globe.selectHero = heroNode;

        if (!heroNode.isHas)
        {
            //Control.HideGUI(GameLibrary.UIHeroList);
            //Control.ShowGUI(GameLibrary.UI_HeroDetail);
            Control.ShowGUI(UIPanleID.UIHeroDetail, EnumOpenUIType.OpenNewCloseOld, false, true);
            Control.HideGUI(UIPanleID.UIHeroList);
            Debug.Log("还没召唤过该英雄");
        }
        else
        {
            GameLibrary.isRefresh = true;

            UIHeroList.instance.isUpdateHero = false;

            if (!hd.isGetData)
            {
                playerData.GetInstance().isEquipDevelop = false;
                Globe.isDetails = true;
                //ClientSendDataMgr.GetSingle().GetHeroSend().SendGetHeroInfo(Globe.selectHero.hero_id, C2SMessageType.ActiveWait);                  //获取英雄信息
                Control.ShowGUI(UIPanleID.UIHeroDetail, EnumOpenUIType.OpenNewCloseOld, false, false);
                Debug.Log("没获得过该英雄的详细信息");
            }
            else
            {
                HeroData temp = playerData.GetInstance().GetHeroDataByID((Globe.selectHero.hero_id));
                playerData.GetInstance().selectHeroDetail = temp;
                //Control.HideGUI(GameLibrary.UIHeroList);
                //Control.ShowGUI(GameLibrary.UI_HeroDetail);
                Control.HideGUI(UIPanleID.UIHeroList);
                Control.ShowGUI(UIPanleID.UIHeroDetail, EnumOpenUIType.OpenNewCloseOld, false, true);
                Debug.Log("获得过该英雄的详细信息");
            }
        }
        
    }

    void RefreshIcon(ItemHeroLineUp itemLine, int index)
    {
        if (heroNode.icon_name == itemLine.icon.normalSprite)
        {
            itemLine.RefreshUI(null);
            herolist.heroPlayList[index].id = 0;
        }
    }

    /// <summary>
    /// 召唤按钮
    /// </summary>
    void OnSummonBtnClick()
    {
        UIHeroList.instance.getHeroPanel.hero = heroNode;

        //记录召唤英雄
        UIHeroList.instance.summmonHero = heroNode.hero_id;
        UIHeroList.instance.SaveSoul(heroNode.soul_gem, starUpNode.call_stone_num);

        ClientSendDataMgr.GetSingle().GetHeroSend().SendSoulStoneChangeHero(heroNode.hero_id, heroNode.init_star, heroNode.soul_gem, starUpNode.call_stone_num);
    }

    /// <summary>
    /// 显示类型
    /// </summary>
    /// <param name="type"></param>
    void ShowType(int type)
    {
        switch (type)
        {
            case 1:
                typeS.spriteName = "li";
                break;
            case 2:
                typeS.spriteName = "zhi";
                break;
            case 3:
                typeS.spriteName = "min";
                break;
            default:
                break;
        }
    }
    protected override void RegisterComponent()
    {
        base.RegisterComponent();
    }
}