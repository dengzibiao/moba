using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Tianyu;

/// <summary>
/// 英雄列表
/// </summary>

public class UIHeroEmbattle : GUIBase
{

    public static UIHeroEmbattle instance;

    public GUISingleButton backBtn;
    public GUISingleCheckBoxGroup typeHeroTab;
    public GUISingleMultList allofMultList;
    public GUISingleMultList powerMultList;
    public GUISingleMultList agileMultList;
    public GUISingleMultList intelligenceMultList;

    public GUISingleButton mainHero;            //主英雄
    public GUISingleButton fuHero1;             //辅助英雄1
    public GUISingleButton fuHero2;             //辅助英雄2
    public GUISingleButton fuHero3;             //辅助英雄3
    public GUISingleButton sureBtn;           //布阵/确定按钮

    UILabel label;                              //布阵按钮Label

    public Dictionary<long, HeroNode> objDic;    //全部英雄列表

    Transform heroListSV;

    object[] obj = new object[14];                                //所有英雄
    Dictionary<long, object> heroList;
    List<GameObject> tabList = new List<GameObject>();  //类型列表
    int chooseIndex = 0;                                //默认标签选择


    //可召唤的英雄列表
    public List<long> summonList = new List<long>();
    public override UIPanleID GetUIKey()
    {
        return UIPanleID.none;
    }

    protected override void Init()
    {
        base.Init();

        instance = this;

        allofMultList = transform.Find("HeroListScrollView/AllofMultList").GetComponent<GUISingleMultList>();
        powerMultList = transform.Find("HeroListScrollView/PowerMultList").GetComponent<GUISingleMultList>();
        agileMultList = transform.Find("HeroListScrollView/AgileMultList").GetComponent<GUISingleMultList>();
        intelligenceMultList = transform.Find("HeroListScrollView/IntelligenceMultList").GetComponent<GUISingleMultList>();

        foreach (Transform item in transform.Find("HeroListScrollView"))
        {
            tabList.Add(item.gameObject);
        }

        heroListSV = transform.Find("HeroListScrollView").transform;

        label = sureBtn.transform.FindComponent<UILabel>("Label");

        mainSprite = mainHero.GetComponent<UISprite>();
        fuSprite1 = fuHero1.GetComponent<UISprite>();
        fuSprite2 = fuHero2.GetComponent<UISprite>();
        fuSprite3 = fuHero3.GetComponent<UISprite>();

        mainSprite.enabled = false;
        fuSprite1.enabled = false;
        fuSprite2.enabled = false;
        fuSprite3.enabled = false;

        mainHero.onClick = OnMainHeroClick;
        fuHero1.onClick = OnFuHero1Click;
        fuHero2.onClick = OnFuHero2Click;
        fuHero3.onClick = OnFuHero3Click;

        sureBtn.onClick = OnSureBtnClick;
        backBtn.onClick = OnBackBtnClick;

        typeHeroTab.onClick = OnTypeHeroTabClick;

        //获取全部英雄
        objDic = FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList;

        //获取全部英雄
        int index = 0;
        foreach (object hn in objDic.Values)
        {
            if (index < obj.Length)
            {
                obj[index] = hn;
                index++;
            }
        }

        //Globe.mainVO = FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList[201000200];//Manager.Instance().GetCSV<HeroCSV>("Hero").GetVO(201000200);
        //Globe.gMainNode = FSDataNodeTable<HeroNode>.GetSingleton().FindDataByType(FightTouch.GetHeroById(GameLibrary.player));

        //Globe.playHeroList[0] = Globe.mainVO;
        //mainSprite.spriteName = Globe.mainVO.icon_name;

        //BtnIsClick(false);

        GetSaveHeroList();

    }

    /// <summary>
    /// 读取英雄数据
    /// </summary>
    public void InitListData()
    {

        InitHeroList(obj);


    }

    /// <summary>
    /// 英雄列表数据显示
    /// </summary>
    /// <param name="allHero"></param>
    void InitHeroList(object[] allHero)
    {

        for (int i = 0; i < tabList.Count; i++)
        {
            tabList[i].SetActive(true);
        }

        int powerIndex = 0;
        int agileIndex = 0;
        int intelligenceIndex = 0;


        //取出所有的英雄
        for (int i = 0; i < allHero.Length; i++)
        {

            //HeroNode hn = (HeroNode)allHero[i];

            switch (((HeroNode)allHero[i]).attribute)
            {
                case 1:
                    powerIndex++;
                    break;
                case 2:
                    agileIndex++;
                    break;
                case 3:
                    intelligenceIndex++;
                    break;
                default:
                    break;
            }

        }

        object[] powerObj = new object[powerIndex];
        object[] agileObj = new object[agileIndex];
        object[] intelligenceObj = new object[intelligenceIndex];

        int powerI = 0;
        int agileI = 0;
        int intelligenceI = 0;

        for (int i = 0; i < allHero.Length; i++)
        {
            HeroNode heroVOo = (HeroNode)allHero[i];

            switch (heroVOo.attribute)
            {
                case 1:
                    powerObj[powerI] = allHero[i];
                    powerI++;
                    break;
                case 2:
                    agileObj[agileI] = allHero[i];
                    agileI++;
                    break;
                case 3:
                    intelligenceObj[intelligenceI] = allHero[i];
                    intelligenceI++;
                    break;
                default:
                    break;
            }

        }

        //全部英雄
        allofMultList.InSize(allHero.Length, 5);
        allofMultList.Info(allHero);

        //力量
        powerMultList.InSize(powerObj.Length, 5);
        powerMultList.Info(powerObj);

        //敏捷
        agileMultList.InSize(agileObj.Length, 5);
        agileMultList.Info(agileObj);

        //智力
        intelligenceMultList.InSize(intelligenceObj.Length, 5);
        intelligenceMultList.Info(intelligenceObj);

        for (int i = 0; i < tabList.Count; i++)
        {
            if (i == chooseIndex) tabList[i].SetActive(true);
            else tabList[i].SetActive(false);
        }

    }

    /// <summary>
    /// 设置主英雄和保存队列
    /// </summary>
    void GetSaveHeroList()
    {

        //Globe.playHeroList[0] = Globe.gMainNode;

        //mainSprite.spriteName = Globe.gMainNode.icon_name;

        //string heroList = PlayerPrefs.GetString("HeroList" + playerData.GetInstance().selfData.accountId);

        //string[] hList = heroList.Split('|');

        //for (int i = 1; i < hList.Length; i++)
        //{
        //    if (hList[i] != "0")
        //        Globe.playHeroList[i] = FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList[long.Parse(hList[i])];
        //}

        //for (int i = 0; i < Globe.playHeroList.Length; i++)
        //{
        //    Globe.playReserved[i] = Globe.playHeroList[i];
        //}

    }


    protected override void ShowHandler()
    {
        base.ShowHandler();



        //if (Globe.heroPlay.Count > 0)
        //{
        //    mainSprite.enabled = true;
        //    mainSprite.spriteName = Globe.mainVO.icon_name;
        //    Globe.mainS = SubStringName(Globe.mainVO.icon_name);
        //    GameLibrary.player = Globe.mainS;
        //}

        InitListData();

        if (GameLibrary.isShowPlay|| GameLibrary.isShowPlayToFB)
        {
            OpenSource(false, true);
        }
        else
        {
            OpenSource(true, false);
        }

    }

    /// <summary>
    /// 英雄类型标签
    /// </summary>
    /// <param name="index"></param>
    /// <param name="boo"></param>
    private void OnTypeHeroTabClick(int index, bool boo)
    {
        if (boo)
        {
            chooseIndex = index;

            switch (index)
            {
                case 0:
                    Globe.isC = false;
                    allofMultList.gameObject.SetActive(true);
                    powerMultList.gameObject.SetActive(false);
                    agileMultList.gameObject.SetActive(false);
                    intelligenceMultList.gameObject.SetActive(false);
                    break;
                case 1:
                    Globe.isC = false;
                    allofMultList.gameObject.SetActive(false);
                    powerMultList.gameObject.SetActive(true);
                    agileMultList.gameObject.SetActive(false);
                    intelligenceMultList.gameObject.SetActive(false);
                    break;
                case 2:
                    Globe.isC = false;
                    allofMultList.gameObject.SetActive(false);
                    powerMultList.gameObject.SetActive(false);
                    agileMultList.gameObject.SetActive(true);
                    intelligenceMultList.gameObject.SetActive(false);
                    break;
                case 3:
                    Globe.isC = false;
                    allofMultList.gameObject.SetActive(false);
                    powerMultList.gameObject.SetActive(false);
                    agileMultList.gameObject.SetActive(false);
                    intelligenceMultList.gameObject.SetActive(true);
                    break;
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// 主英雄按钮
    /// </summary>
    private void OnMainHeroClick()
    {
        //mainSprite.enabled = false;
        //CancelPlayDic(Globe.gMainNode.hero_id);
    }

    /// <summary>
    /// 辅助英雄1按钮
    /// </summary>
    private void OnFuHero1Click()
    {
        //fuSprite1.enabled = false;
        //CancelPlayDic(Globe.gFu1Node.hero_id);
    }

    /// <summary>
    /// 辅助英雄2按钮
    /// </summary>
    private void OnFuHero2Click()
    {
        //fuSprite2.enabled = false;
        //CancelPlayDic(Globe.gFu2Node.hero_id);
    }
    /// <summary>
    /// 辅助英雄3按钮
    /// </summary>
    private void OnFuHero3Click()
    {
        //fuSprite3.enabled = false;
        //CancelPlayDic(Globe.gSummon1Node.hero_id);
    }

    /// <summary>
    /// 主英雄上阵
    /// </summary>
    /// <param name="heroVO"></param>
    public void MainHeroBattle(HeroNode heroVO)
    {
        mainSprite.enabled = true;
        //mainSprite.spriteName = heroVO.icon_name;
        ////Globe.mainS = SubStringName(heroVO.icon_name);
        //Globe.mainS = heroVO.icon_name;
        //Globe.gMainNode = heroVO;
        //Globe.playHeroList[0] = Globe.gMainNode;
    }

    /// <summary>
    /// 辅助英雄1上阵
    /// </summary>
    /// <param name="heroVO"></param>
    public void FuHero1Battle(HeroNode heroVO)
    {
        //fuSprite1.enabled = true;
        //fuSprite1.spriteName = heroVO.icon_name;
        ////Globe.fu1S = SubStringName(heroVO.icon_name);
        //Globe.fu1S = heroVO.icon_name;
        //Globe.gFu1Node = heroVO;
        //Globe.playHeroList[1] = Globe.gFu1Node;
    }

    /// <summary>
    /// 辅助英雄2上阵
    /// </summary>
    /// <param name="heroVO"></param>
    public void FuHero2Battle(HeroNode heroVO)
    {
        //fuSprite2.enabled = true;
        //fuSprite2.spriteName = heroVO.icon_name;
        ////Globe.fu2S = SubStringName(heroVO.icon_name);
        //Globe.fu2S = heroVO.icon_name;
        //Globe.gFu2Node = heroVO;
        //Globe.playHeroList[2] = Globe.gFu2Node;
    }

    /// <summary>
    /// 辅助英雄3上阵
    /// </summary>
    /// <param name="heroVO"></param>
    public void FuHero3Battle(HeroNode heroVO)
    {
        //fuSprite3.enabled = true;
        //fuSprite3.spriteName = heroVO.icon_name;
        ////Globe.fu3S = SubStringName(heroVO.icon_name);
        //Globe.fu3S = heroVO.icon_name;
        //Globe.gSummon1Node = heroVO;
        //Globe.playHeroList[3] = Globe.gSummon1Node;
    }

    /// <summary>
    /// 返回英雄名字
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    string SubStringName(string name)
    {
        return name.Substring(0, name.LastIndexOf("_"));
    }

    /// <summary>
    /// 字典中增加选择的英雄
    /// </summary>
    void CancelPlayDic(long id)
    {
        if (Globe.playDic.ContainsKey(id))
        {
            Globe.playDic.Remove(id);
        }

        Globe.playDic.Add(id, false);
    }

    /// <summary>
    /// 布阵/确定按钮
    /// </summary>
    private void OnSureBtnClick()
    {

        Hide();
        // CharacterManager

        //isLineup = !isLineup;
        //if (isLineup)
        //{
        //    label.text = "确定";
        //}
        //else
        //{
        //    label.text = "布阵";

        //若主英雄未选择时，默认选择主英雄
        //if (!mainSprite.enabled)
        //{
        //    mainSprite.enabled = true;
        //    mainSprite.spriteName = Globe.gMainNode.icon_name;

        //    if (Globe.playDic.ContainsKey(Globe.gMainNode.hero_id))
        //    {
        //        Globe.playDic.Remove(Globe.gMainNode.hero_id);
        //    }

        //    Globe.playDic.Add(Globe.gMainNode.hero_id, true);
        //}
        //else
        //{
        //    AddHeroPlay();
        //}

        //}

        //IsShowToHaveHero(isLineup);
        //BtnIsClick(isLineup);
        //GameLibrary.player = Globe.gMainNode.icon_name;
        //CharacterManager.instance.CreatCharacter();
        //UIRole.Instance.ChangeHeroHeadIcon();
    }

    /// <summary>
    /// 四位上阵英雄是否可点击
    /// </summary>
    /// <param name="isClick"></param>
    public void BtnIsClick(bool isClick)
    {
        if (mainSprite.enabled)
        {
            mainSprite.GetComponent<BoxCollider>().enabled = isClick;
        }
        if (fuSprite1.enabled)
        {
            fuSprite1.GetComponent<BoxCollider>().enabled = isClick;
        }
        if (fuSprite2.enabled)
        {
            fuSprite2.GetComponent<BoxCollider>().enabled = isClick;
        }
        if (fuSprite3.enabled)
        {
            fuSprite3.GetComponent<BoxCollider>().enabled = isClick;
        }
    }

    /// <summary>
    /// 是否显示为拥有英雄
    /// </summary>
    /// <param name="isHide"></param>
    void IsShowToHaveHero(bool isHide)
    {

        //if (summonList.Count <= 0)
        //{
        for (int j = 0; j < heroListSV.childCount; j++)
        {
            for (int i = 1; i < heroListSV.GetChild(j).transform.childCount; i++)
            {
                if (heroListSV.GetChild(j).transform.GetChild(i).transform.Find("Mask").GetComponent<UISprite>().enabled)
                {
                    heroListSV.GetChild(j).transform.GetChild(i).gameObject.SetActive(!isHide);
                }
            }
        }
        //}
        //else
        //{
        //    if (isHide)
        //    {
        //        InitListData();
        //        return;
        //    }

        //    powerMultList.gameObject.SetActive(true);
        //    agileMultList.gameObject.SetActive(true);
        //    intelligenceMultList.gameObject.SetActive(true);

        //    //object obje;

        //    ////已经拥有的英雄
        //    //for (int i = 0; i < playerData.GetInstance().heroList.Count; i++)
        //    //{
        //    //    if (heroList.TryGetValue(playerData.GetInstance().heroList[i], out obje))
        //    //    {
        //    //        objs[index] = obje;
        //    //        count.Remove(playerData.GetInstance().heroList[i]);
        //    //        index++;
        //    //        heroList.Remove(playerData.GetInstance().heroList[i]);
        //    //    }
        //    //}

        //    object[] AllObj = new object[playerData.GetInstance().heroList.Count];

        //    int powerIndex = 0;
        //    int agileIndex = 0;
        //    int intelligenceIndex = 0;

        //    int powerI = 0;
        //    int agileI = 0;
        //    int intelligenceI = 0;

        //    HeroVO hero;

        //    for (int i = 0; i < playerData.GetInstance().heroList.Count; i++)
        //    {

        //        hero = VOManager.Instance().GetCSV<HeroCSV>("Hero").GetVO(playerData.GetInstance().heroList[i]);

        //        AllObj[i] = hero;

        //        switch (hero.attribute)
        //        {
        //            case 1:
        //                powerIndex++;
        //                break;
        //            case 2:
        //                agileIndex++;
        //                break;
        //            case 3:
        //                intelligenceIndex++;
        //                break;
        //            default:
        //                break;
        //        }

        //    }

        //    object[] powerObj = new object[powerIndex];
        //    object[] agileObj = new object[agileIndex];
        //    object[] intelligenceObj = new object[intelligenceIndex];

        //    for (int i = 0; i < AllObj.Length; i++)
        //    {
        //        HeroVO heroVOo = (HeroVO)AllObj[i];

        //        switch (heroVOo.attribute)
        //        {
        //            case 1:
        //                powerObj[powerI] = AllObj[i];
        //                powerI++;
        //                break;
        //            case 2:
        //                agileObj[agileI] = AllObj[i];
        //                agileI++;
        //                break;
        //            case 3:
        //                intelligenceObj[intelligenceI] = AllObj[i];
        //                intelligenceI++;
        //                break;
        //            default:
        //                break;
        //        }

        //    }

        //    //全部
        //    allofMultList.InSize(AllObj.Length, 5);
        //    allofMultList.Info(AllObj);

        //    //力量
        //    powerMultList.InSize(powerIndex, 5);
        //    powerMultList.Info(powerObj);

        //    //敏捷
        //    agileMultList.InSize(agileIndex, 5);
        //    agileMultList.Info(agileObj);

        //    //智力
        //    intelligenceMultList.InSize(intelligenceIndex, 5);
        //    intelligenceMultList.Info(intelligenceObj);



        //    powerMultList.gameObject.SetActive(false);
        //    agileMultList.gameObject.SetActive(false);
        //    intelligenceMultList.gameObject.SetActive(false);

        //}

    }

    /// <summary>
    /// 返回按钮
    /// </summary>
    private void OnBackBtnClick()
    {

        if (!mainSprite.enabled)
        {
            //for (int i = 0; i < Globe.playReserved.Length; i++)
            //{
            //    Globe.playHeroList[i] = Globe.playReserved[i];
            //}

            //if (Globe.playHeroList[1].hero_id == 0)
            //{
            //    Globe.playDic[Globe.gFu1Node.hero_id] = false;
            //}
            //if (Globe.playHeroList[2].hero_id == 0)
            //{
            //    Globe.playDic[Globe.gFu2Node.hero_id] = false;
            //}
            //if (Globe.playHeroList[3].hero_id == 0)
            //{
            //    Globe.playDic[Globe.gSummon1Node.hero_id] = false;
            //}
        }
        else
        {

            //Globe.mainS = SubStringName(Globe.mainVO.model);
            //Globe.mainS = Globe.gMainNode.icon_name;

            //if (Globe.mainS != GameLibrary.player)
            //{
            //    GameLibrary.player = Globe.mainS;
            //    CharacterManager.instance.CreatCharacter();
            //}

            //AddHeroPlay();

        }

        //isLineup = false;
        //label.text = "布阵";



        if (GameLibrary.isShowPlay)
        {
            Control.HideGUI(UIPanleID.UIHeroList);
            //Control.ShowGUI(GameLibrary.UI_Level);
            transform.root.Find("UICreateTeam").gameObject.SetActive(true);
            GameLibrary.isShowPlay = false;
        }
        else if(GameLibrary.isShowPlayToFB)
        {
            Control.HideGUI(UIPanleID.UIHeroList);
            //Control.ShowGUI(GameLibrary.UI_Level);
            GameLibrary.isShowPlay = false;
        }
        else
        {
            Control.HideGUI(UIPanleID.UIHeroList);
        }

        UIRole.Instance.ChangeHeroHeadIcon();

        
    }

    /// <summary>
    /// 将上阵的英雄信息保留
    /// </summary>
    void AddHeroPlay()
    {
        //Globe.playHeroList[0] = Globe.gMainNode;

        //HeroNode vo = new HeroNode();

        //if (fuSprite1.enabled)
        //{
        //    Globe.playHeroList[1] = Globe.gFu1Node;
        //}
        //else
        //{
        //    Globe.playHeroList[1] = vo;
        //}

        //if (fuSprite2.enabled)
        //{
        //    Globe.playHeroList[2] = Globe.gFu2Node;
        //}
        //else
        //{
        //    Globe.playHeroList[2] = vo;
        //}

        //if (fuSprite3.enabled)
        //{
        //    Globe.playHeroList[3] = Globe.gSummon1Node;
        //}
        //else
        //{
        //    Globe.playHeroList[3] = vo;
        //}

        //for (int i = 0; i < Globe.playHeroList.Length; i++)
        //{
        //    Globe.playReserved[i] = Globe.playHeroList[i];
        //}

        //for (int i = 0; i < Globe.playHeroList.Length; i++)
        //{
        //    Debug.Log(Globe.playHeroList[i].name);
        //}

    }

    /// <summary>
    /// 出战
    /// </summary>
    public void Fighting()
    {

        //if (!mainSprite.enabled)
        //{
        //    //ShowPrompt();
        //    print("Return");
        //    return;
        //}

        //设置主英雄
        //Globe.mainS = SubStringName(Globe.gMainNode.icon_name);
        //Globe.mainS = Globe.gMainNode.icon_name;
        //GameLibrary.player = Globe.mainS;

        ////辅助英雄
        //GameLibrary.heroTeam = new List<string>();

        //SelectHeroPlay(Globe.gMainNode);

        //if (fuSprite1.enabled)
        //{
        //    SelectHeroPlay(Globe.fuHeroVO[0]);
        //    AddHeroTeam(Globe.fu1S);
        //}
        //else
        //{
        //    ClearData(Globe.fuHeroVO[0], Globe.fu1S);
        //}

        //if (fuSprite2.enabled)
        //{
        //    SelectHeroPlay(Globe.fuHeroVO[1]);
        //    AddHeroTeam(Globe.fu2S);
        //}
        //else
        //{
        //    ClearData(Globe.fuHeroVO[1], Globe.fu2S);
        //}

        //if (fuSprite3.enabled)
        //{
        //    SelectHeroPlay(Globe.fuHeroVO[2]);
        //    AddHeroTeam(Globe.fu3S);
        //}
        //else
        //{
        //    ClearData(Globe.fuHeroVO[2], Globe.fu3S);
        //}


        ////---

        //int index = 0;

        //for (int i = 0; i < GameLibrary.heroTeam.Count; i++)
        //{
        //    if (GameLibrary.heroTeam[i] != "0")
        //    {
        //        index++;
        //    }
        //}

        //print("主战英雄:" + GameLibrary.player);
        //print("辅助人数:" + index);

        //for (int i = 0; i < GameLibrary.heroTeam.Count; i++)
        //{
        //    if (GameLibrary.heroTeam[i] == "0")
        //    {
        //        print("辅助 " + i + " : 无");
        //    }
        //    else
        //    {
        //        print("辅助 " + i + " : " + GameLibrary.heroTeam[i]);
        //    }

        //}

        ////---

        //AddHeroPlay();

        //跳转场景
        //Control.HideGUI(GameLibrary.UI_HeroPlay);
        //UI_Loading.LoadScene(GameLibrary.chooseFB, 3f);

    }

    /// <summary>
    /// 选择出战的英雄
    /// </summary>
    /// <param name="vo"></param>
    void SelectHeroPlay(HeroNode vo)
    {
        if (Globe.heroPlay.Contains(vo))
        {
            Globe.heroPlay.Remove(vo);
        }
        Globe.heroPlay.Add(vo);
    }

    /// <summary>
    /// 清空数据
    /// </summary>
    void ClearData(HeroNode vo, string name)
    {
        if (Globe.heroPlay.Contains(vo))
        {
            Globe.heroPlay.Remove(vo);
        }
        if (GameLibrary.heroTeam.Contains(name))
        {
            GameLibrary.heroTeam.Remove(name);
        }
        GameLibrary.heroTeam.Add("0");
    }

    /// <summary>
    /// 添加辅助英雄
    /// </summary>
    /// <param name="name"></param>
    void AddHeroTeam(string name)
    {
        if (GameLibrary.heroTeam.Contains(name))
        {
            GameLibrary.heroTeam.Remove(name);
        }
        GameLibrary.heroTeam.Add(name);
    }

    /// <summary>
    /// 打开源
    /// </summary>
    /// <param name="lineuoBtn"></param>
    /// <param name="other"></param>
    void OpenSource(bool lineuoBtn, bool other)
    {
        sureBtn.gameObject.SetActive(lineuoBtn);
        isLineup = other;
        //BtnIsClick(other);
        IsShowToHaveHero(other);
    }

    public HeroEmbattleItem HeroEmbattleItem;

    /// <summary>
    /// 召唤英雄
    /// </summary>
    public void SummonHero()
    {

        if (isUpdateHero)
        {
            InitListData();
            isUpdateHero = false;
        }
    }

    public UISprite mainSprite { get; set; }            //主英雄
    public UISprite fuSprite1 { get; set; }            //辅助英雄1
    public UISprite fuSprite2 { get; set; }            //辅助英雄2
    public UISprite fuSprite3 { get; set; }            //辅助英雄3
    public bool isLineup { get; set; }            //是否出战
    public bool isUpdateHero { get; set; }


}