/*
文件名（File Name）:   ItemHeroShow.cs

作者（Author）:    #高#

创建时间（CreateTime）:  #CreateTime#
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemHeroShow : GUISingleItemList
{

    public GUISingleButton icon;        //英雄原画
    public UISprite border;             //边框
    //public UILabel qualityLabel;        //品质等级
    public UILabel levelLabel;          //等级
    public UILabel nameLabel;           //名字
    UISprite mask;                      //遮罩
    UISprite typeS;                     //英雄类型
    UISprite suo;                       //锁
    UILabel souStoneL;                  //魂石数
    UIButton summonBtn;                 //召唤按钮


    //星级List
    List<UISprite> startList = new List<UISprite>();

    HeroNode heroVO;

    PlayerInfo playInfo = null;

    HeroNode vo;

    bool isUpdate = false;

    bool isPlay_M;

    bool isPlay = false;

    UIHeroList herolist;

    //HeroData hd = null;             //英雄信息

    int currentSoul = 0;            //当前的魂石
    StarUpgradeVO starUpVO;         //升级魂石

    ItemData item;


    /// <summary>
    /// item初始化函数
    /// </summary>
    protected override void InitItem()
    {
        //初始化
        icon = transform.Find("Icon").GetComponent<GUISingleButton>();

        icon.onClick = OnIconClick;

        border = transform.Find("Border").GetComponent<UISprite>();
        //qualityLabel = transform.Find("QualityLabel").GetComponent<UILabel>();
        levelLabel = transform.Find("LevelLabel").GetComponent<UILabel>();
        nameLabel = transform.Find("NameLabel").GetComponent<UILabel>();
        mask = UnityUtil.FindCtrl<UISprite>(gameObject, "Mask");
        typeS = UnityUtil.FindCtrl<UISprite>(gameObject, "Type");
        suo = UnityUtil.FindCtrl<UISprite>(gameObject, "Suo");
        souStoneL = UnityUtil.FindCtrl<UILabel>(gameObject, "SouStoneL");
        summonBtn = UnityUtil.FindCtrl<UIButton>(gameObject, "SummonBtn");

        for (int i = 1; i <= 5; i++)
        {
            startList.Add(transform.Find("Star" + i).GetComponent<UISprite>());
        }

        //EventDelegate.Set(mask.onClick, this.OnMackBtn);

        GetComponent<BoxCollider>().enabled = false;


        herolist = UIHeroList.instance;

        EventDelegate.Set(summonBtn.onClick, OnSummonBtnClick);

    }

    /// <summary>
    /// 信息赋值
    /// </summary>
    /// <param name="obj"></param>
    public override void Info(object obj)
    {

        heroVO = (HeroNode)obj;

        if (!Globe.allHeroDic.ContainsKey(heroVO.hero_id))
        {
            Globe.allHeroDic.Add(heroVO.hero_id, heroVO);
        }


        for (int i = 0; i < heroVO.init_star; i++)
        {
            startList[i].spriteName = "xingxing";
        }

    

        levelLabel.text = "Lv." + 1;

        nameLabel.text = heroVO.name;

        mask.enabled = false;

        icon.spriteName = heroVO.original_painting;         //原画

        isUpdate = true;

        ShowType(heroVO.attribute);

        Globe.isC = true;

    }

    void OnEnable()
    {


        if (Globe.isC)
        {

            ChangeState(Globe.playHeroList[0].id, true);

        }

    }

    /// <summary>
    /// 点击选择英雄卡片
    /// </summary>
    private void OnIconClick()
    {

        //Control.ShowGUI(GameLibrary.UI_HeroDetail);

        //Control.HideGUI(GameLibrary.UIHeroShow);
    }



    /// <summary>
    /// 上阵状态切换
    /// </summary>
    /// <param name="id"></param>
    /// <param name="isSave"></param>
    void ChangeState(long id, bool isSave)
    {
        if (Globe.playDic.ContainsKey(id))
        {
            // Globe.playDic[id] = false;
            Globe.playDic.Remove(id);
        }
        Globe.playDic.Add(id, isSave);
    }

    /// <summary>
    /// 更新边框
    /// </summary>
    void UpdateBorder()
    {
        if (playInfo.Quality == 0)
        {
            border.spriteName = "baikuang";
            //qualityLabel.text = "";
        }
        else if (playInfo.Quality == 1 || playInfo.Quality == 2)
        {
            border.spriteName = "lvkuang";
            //qualityLabel.text = "+" + (playInfo.Quality - 1);
        }
        else if (playInfo.Quality == 3 || playInfo.Quality == 4 || playInfo.Quality == 5)
        {
            border.spriteName = "lankuang";
            //qualityLabel.text = "+" + (playInfo.Quality - 3);
        }
        else if (playInfo.Quality == 6 || playInfo.Quality == 7 || playInfo.Quality == 8 || playInfo.Quality == 9)
        {
            border.spriteName = "zikuang";
            //qualityLabel.text = "+" + (playInfo.Quality - 6);
        }
        else if (playInfo.Quality == 10 || playInfo.Quality == 11 || playInfo.Quality == 12 || playInfo.Quality == 13)
        {
            border.spriteName = "chengkuang";
            //qualityLabel.text = "+" + (playInfo.Quality - 10);
        }
        else if (playInfo.Quality == 14 || playInfo.Quality == 15 || playInfo.Quality == 16)
        {
            border.spriteName = "hongkuang";
            //qualityLabel.text = "+" + (playInfo.Quality - 14);
        }

        if (playInfo.Quality == 1 || playInfo.Quality == 3 || playInfo.Quality == 6 || playInfo.Quality == 10 || playInfo.Quality == 14)
        {
            //qualityLabel.text = "";
        }

        levelLabel.text = "Lv." + playInfo.Level;

    }

    /// <summary>
    /// 更新星级
    /// </summary>
    void UpdateStar()
    {
        //HeroVO 类型为Class时需判断是否为空
        //if (Globe.selectHero == null) return;

        Globe.allHeroDic.TryGetValue(Globe.selectHero.hero_id, out vo);

        print(vo.name);

        for (int i = 0; i < vo.init_star; i++)
        {
            startList[i].spriteName = "xingxing";
        }

        for (int i = vo.init_star; i < startList.Count; i++)
        {
            startList[i].spriteName = "xing-hui";
        }
    }

    /// <summary>
    /// 召唤按钮
    /// </summary>
    void OnSummonBtnClick()
    {

        transform.parent.parent.parent.Find("GetHeroPanel").gameObject.SetActive(true);

        GetHeroPanel.instance.hero = heroVO;

        //读取英雄数据
        HeroVO vo = VOManager.Instance().GetCSV<HeroCSV>("Hero").GetVO(heroVO.hero_id);

        //向服务器发送消息
        if (DataDefine.isConectSocket)
        {
            ClientSendDataMgr.GetSingle().GetHeroSend().SendSoulStoneChangeHero(heroVO.hero_id, vo.init_star, vo.soul_gem, currentSoul);
        }
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
                typeS.spriteName = "min";
                break;
            case 3:
                typeS.spriteName = "zhi";
                break;
            default:
                break;
        }
    }
}
