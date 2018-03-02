using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class equipHeroItem : GUISingleItemList
{

    // Use this for initialization
    //void Start () {

    //}
    public GUISingleButton icon;        //英雄原画
    public UISprite border;             //边框
    public UILabel levelLabel;          //等级
    public UILabel nameLabel;           //名字
    public UISprite[] start;            //星级列表


    //星级List
   // List<UISprite> startList = new List<UISprite>();

    HeroNode heroNode;
    UIHeroList herolist;
    HeroData hd = null;             //英雄信息
    StarUpGradeNode starUpNode;
    ItemData item;



    /// <summary>
    /// item初始化函数
    /// </summary>
    protected override void InitItem()
    {
        icon.onClick = OnIconClick;
       
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

        for (int i = 0; i < start.Length; i++)
        {
            if(i<hd.star)
            {
                //start[i].spriteName = "xing";// : "xing-hui";
                start[i].gameObject.SetActive(true);
            }
            else
            {
                //start[i].spriteName = "xing-hui";
                start[i].gameObject.SetActive(false);
            }
        }

        border.spriteName = UISign_in.GetHeroGradeName(hd.grade);
        levelLabel.text = !heroNode.isHas ? "" : hd.lvl + "级";
      //  nameLabel.text = heroNode.name;
        icon.spriteName = heroNode.icon_name;


       // ShowType(heroNode.attribute);

       // Globe.isC = true;
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
        if (heroNode.hero_id == playerData.GetInstance().selectHeroDetail.id)
        {
            EquipDevelop.GetSingolton().HeroPanel.gameObject.SetActive(false);
            return;
        }
        else
        {
            ClientSendDataMgr.GetSingle().GetHeroSend().SendGetHeroInfo(heroNode.hero_id, C2SMessageType.ActiveWait);                  //获取英雄信息
            playerData.GetInstance().isEquipDevelop = true;
            EquipDevelop.GetSingolton().HeroPanel.gameObject.SetActive(false);
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
    //void ShowType(int type)
    //{
    //    switch (type)
    //    {
    //        case 1:
    //            typeS.spriteName = "li";
    //            break;
    //        case 2:
    //            typeS.spriteName = "zhi";
    //            break;
    //        case 3:
    //            typeS.spriteName = "min";
    //            break;
    //        default:
    //            break;
    //    }
    //}

    // Update is called once per frame
    void Update () {
	
	}
}
