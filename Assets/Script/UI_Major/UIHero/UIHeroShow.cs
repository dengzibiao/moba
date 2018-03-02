/*
文件名（File Name）:   UIHeroShow.cs

作者（Author）:    #高#

创建时间（CreateTime）:  #CreateTime#
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tianyu;

public class UIHeroShow : GUIBase
{
    public static UIHeroShow instance;

    public GUISingleButton backBtn;
    public GUISingleCheckBoxGroup typeHeroTab;
    public GUISingleMultList allofMultList;
    public GUISingleMultList powerMultList;
    public GUISingleMultList agileMultList;
    public GUISingleMultList intelligenceMultList;
    private Transform heroListScrollView;

    public GUISingleButton sureBtn;           //布阵/确定按钮

    public Dictionary<long, HeroNode> objDic;    //全部英雄列表


    object[] obj = new object[14];                                //所有英雄
    List<GameObject> tabList = new List<GameObject>();  //类型列表
    int chooseIndex = 0;                                //默认标签选择


    //可召唤的英雄列表
    public List<long> summonList = new List<long>();
    public override UIPanleID GetUIKey()
    {
        return UIPanleID.UIHeroShow;
    }


    protected override void Init()
    {
        base.Init();

        instance = this;

        heroListScrollView = this.GetComponentInChildren<UIScrollView>().transform;

        foreach (Transform item in heroListScrollView)
        {
            tabList.Add(item.gameObject);
        }

        sureBtn.onClick = OnSureBtnClick;
        backBtn.onClick = OnBackBtnClick;

        typeHeroTab.onClick = OnTypeHeroTabClick;
        //typeHeroTab.DefauleIndex = 0;
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
        allofMultList.ScrollView = heroListScrollView;
        //力量
        powerMultList.InSize(powerObj.Length, 5);
        powerMultList.Info(powerObj);
        powerMultList.ScrollView = heroListScrollView;
        //敏捷
        agileMultList.InSize(agileObj.Length, 5);
        agileMultList.Info(agileObj);
        agileMultList.ScrollView = heroListScrollView;
        //智力
        intelligenceMultList.InSize(intelligenceObj.Length, 5);
        intelligenceMultList.Info(intelligenceObj);
        intelligenceMultList.ScrollView = heroListScrollView;
        for (int i = 0; i < tabList.Count; i++)
        {
            if (i == chooseIndex) tabList[i].SetActive(true);
            else tabList[i].SetActive(false);
        }

    }


    protected override void ShowHandler()
    {
        base.ShowHandler();



        InitListData();

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
    /// 布阵/确定按钮
    /// </summary>
    private void OnSureBtnClick()
    {
        Hide();
    }



    /// <summary>
    /// 返回按钮
    /// </summary>
    private void OnBackBtnClick()
    {

        Hide();
    }

    public bool isLineup { get; set; }            //是否出战
    public bool isUpdateHero { get; set; }


}
