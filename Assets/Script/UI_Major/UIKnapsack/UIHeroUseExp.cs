using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Tianyu;

/// <summary>
/// 英雄吃药水
/// </summary>

public class UIHeroUseExp : GUIBase
{

    public static UIHeroUseExp instance;

    public GUISingleButton backBtn;
    public GUISingleButton upPageBtn;
    public GUISingleButton nextPageBtn;
    public GUISingleCheckBoxGroup typeHeroTab;
    public GUISingleMultList allofMultList;
    public GUISingleMultList powerMultList;
    public GUISingleMultList agileMultList;
    public GUISingleMultList intelligenceMultList;
    public UIScrollBar scrollBar;

    public object[] obj;

    Transform heroListSV;
    public UIScrollView scrollView;

    //可召唤的英雄列表
    public List<long> summonList = new List<long>();

    public int maxPage;//当前lab标签下scrollview最大页数
    public int currentPage = 1;//当前scrollview页数
    static int currentLab;//当前选择的lab标签
    List<GameObject> tabList = new List<GameObject>();  //类型列表
    public List<HeroData> herodataList = new List<HeroData>();//玩家拥有英雄数据列表
    object[] powerObjs;
    object[] agileObjs;
    object[] intelligenceObjs;
    private Vector3 scrollViewPos;
    private Vector3 multListPos;
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
        tabList.Add(allofMultList.gameObject);
        tabList.Add(powerMultList.gameObject);
        tabList.Add(agileMultList.gameObject);
        tabList.Add(intelligenceMultList.gameObject);
        heroListSV = transform.Find("HeroListScrollView").transform;
        scrollView = heroListSV.GetComponent<UIScrollView>();
        scrollBar = transform.Find("ScrollBar").GetComponent<UIScrollBar>();
        backBtn.onClick = OnBackBtnClick;
        upPageBtn.onClick = OnUpPageClick;
        nextPageBtn.onClick = OnNextPageClick;

        typeHeroTab.onClick = OnTypeHeroTabClick;

        scrollViewPos = scrollView.transform.localPosition;
        multListPos = allofMultList.transform.localPosition;
        //Globe.playHeroList[0] = Globe.mainVO;
    }


    protected override void ShowHandler()
    {
        base.ShowHandler();
        upPageBtn.gameObject.SetActive(false);
        nextPageBtn.gameObject.SetActive(false);
        InitListData();
        //scrollView.ResetPosition();
        OnTypeHeroTabClick(0, true);
        typeHeroTab.setMaskState(0);
        maxPage = obj.Length / 10;
        if ((obj.Length % 10) > 0)
        {
            maxPage++;
        }
        currentPage = 1;
        if (1 < currentPage)
        {
            upPageBtn.gameObject.SetActive(true);
        }
        if (currentPage<maxPage)
        {
            nextPageBtn.gameObject.SetActive(true);
        }
    }
    /// <summary>
    /// 读取英雄数据
    /// </summary>
    public void InitListData()
    {
        herodataList.Clear();
        //获取玩家拥有的英雄列表
        foreach (HeroData data in playerData.GetInstance().herodataList)
        {
            herodataList.Add(data);
        }
        obj = herodataList.ToArray();
        for (int i = 0;i<tabList.Count;i++)
        {
            tabList[i].SetActive(true);
        }
        HeroNode heroNode;

        int powerIndex = 0;
        int agileIndex = 0;
        int intelligenceIndex = 0;
        for (int i = 0; i <herodataList.Count; i++)
        {
            //if (playerData.GetInstance().herodataList[i] == null)
            //{
            //    continue;
            //}
            //heroNode = VOManager.Instance().GetCSV<HeroCSV>("Hero").GetVO(herodataList[i].id);
            heroNode = FSDataNodeTable<HeroNode>.GetSingleton().FindDataByType((int)herodataList[i].id);

            switch (heroNode.attribute)
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

        //object[] powerObj = new object[powerIndex];
        //object[] agileObj = new object[agileIndex];
        //object[] intelligenceObj = new object[intelligenceIndex];
        powerObjs = new object[powerIndex];
        agileObjs = new object[agileIndex];
        intelligenceObjs = new object[intelligenceIndex];

        int powerI = 0;
        int agileI = 0;
        int intelligenceI = 0;

        for (int i = 0; i < obj.Length; i++)
        {
            //if (obj == null)
            //{
            //    continue;
            //}
            
            HeroNode heroNodeo = FSDataNodeTable<HeroNode>.GetSingleton().FindDataByType((int)((HeroData)obj[i]).id);

            switch (heroNodeo.attribute)
            {
                case 1:
                    powerObjs[powerI] = obj[i];
                    powerI++;
                    break;
                case 2:
                    agileObjs[agileI] = obj[i];
                    agileI++;
                    break;
                case 3:
                    intelligenceObjs[intelligenceI] = obj[i];
                    intelligenceI++;
                    break;
                default:
                    break;
            }

        }
        ////obj = new object[14];
        ////全部英雄
        ////allofMultList.ScrollView = heroListSV;
        //allofMultList.InSize(obj.Length, 5);
        //allofMultList.Info(obj);

        ////力量
        ////powerMultList.ScrollView = heroListSV;
        //powerMultList.InSize(powerIndex, 5);
        //powerMultList.Info(powerObj);

        ////敏捷
        ////agileMultList.ScrollView = heroListSV;
        //agileMultList.InSize(agileIndex, 5);
        //agileMultList.Info(agileObj);

        ////智力
        ////intelligenceMultList.ScrollView = heroListSV;
        //intelligenceMultList.InSize(intelligenceIndex, 5);
        //intelligenceMultList.Info(intelligenceObj);

        for (int i = 0; i < tabList.Count; i++)
        {
            tabList[i].SetActive(false);
            //if (i == currentLab) tabList[i].SetActive(true);
            //else tabList[i].SetActive(false);
        }

    }
    /// <summary>
    /// 切换英雄类别页签 设置翻页按钮显示
    /// </summary>
    public void SetPageInfo()
    {
        upPageBtn.gameObject.SetActive(false);
        nextPageBtn.gameObject.SetActive(false);
        if (currentLab == 0)
        {
            maxPage = obj.Length / 10;
            if ((obj.Length % 10) > 0)
            {
                maxPage++;
            }
        }
        else if (currentLab == 1)
        {
            maxPage = powerObjs.Length / 10;
            if ((powerObjs.Length % 10) > 0)
            {
                maxPage++;
            }
        }
        else if (currentLab == 2)
        {
            maxPage = agileObjs.Length / 10;
            if ((agileObjs.Length % 10) > 0)
            {
                maxPage++;
            }
        }
        else if (currentLab == 3)
        {
            maxPage = intelligenceObjs.Length / 10;
            if ((intelligenceObjs.Length % 10) > 0)
            {
                maxPage++;
            }
        }
        currentPage = 1;
        if (1 < currentPage)
        {
            upPageBtn.gameObject.SetActive(true);
        }
        if (currentPage < maxPage)
        {
            nextPageBtn.gameObject.SetActive(true);
        }

    }
    /// <summary>
    /// 已拥有英雄排序 
    /// </summary>
    List<long> HaveSort(List<long> haveHeroList)
    {

        //英雄排序，战斗力->等级->ID

        HeroData fc;

        HeroData hd;

        List<HeroData> heroList = new List<HeroData>();

        //获取已拥有英雄的数据
        for (int i = 0; i < haveHeroList.Count - 1; i++)
        {
            hd = playerData.GetInstance().GetHeroDataByID(haveHeroList[i]);
            heroList.Add(hd);
        }


        for (int i = 0; i < heroList.Count - 1; i++)
        {
            for (int j = i + 1; j < heroList.Count; j++)
            {
                //根据战斗力排序
                if (heroList[i].fc < heroList[i + 1].fc)
                {
                    fc = heroList[i];
                    heroList[i] = heroList[j];
                    heroList[j] = fc;
                }
                else if (heroList[i].fc == heroList[i + 1].fc)//根据
                {
                    if (heroList[i].lvl < heroList[i + 1].lvl)
                    {
                        fc = heroList[i];
                        heroList[i] = heroList[j];
                        heroList[j] = fc;
                    }
                    else if (heroList[i].lvl == heroList[i + 1].lvl)
                    {
                        if (heroList[i].id < heroList[i + 1].id)
                        {
                            fc = heroList[i];
                            heroList[i] = heroList[j];
                            heroList[j] = fc;
                        }
                    }
                }
            }
        }

        haveHeroList.Clear();

        for (int i = 0; i < heroList.Count; i++)
        {
            haveHeroList[i] = heroList[i].id;
        }

        return haveHeroList;

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
            currentLab = index;
            switch (index)
            {
                case 0:
                    allofMultList.gameObject.SetActive(true);
                    allofMultList.InSize(obj.Length, 5);
                    allofMultList.Info(obj);
                    scrollView.transform.localPosition = scrollViewPos;
                    allofMultList.transform.localPosition = multListPos;
                    //scrollView.ResetPosition();
                    powerMultList.gameObject.SetActive(false);
                    agileMultList.gameObject.SetActive(false);
                    intelligenceMultList.gameObject.SetActive(false);
                    SetPageInfo();
                    break;
                case 1:
                    //allofMultList.gameObject.SetActive(false);
                    //powerMultList.gameObject.SetActive(true);
                    //agileMultList.gameObject.SetActive(false);
                    //intelligenceMultList.gameObject.SetActive(false);
                    //powerMultList.InSize(powerObjs.Length, 5);
                    //powerMultList.Info(powerObjs);
                    //scrollView.transform.localPosition = scrollViewPos;
                    //powerMultList.transform.localPosition = multListPos;
                    ////scrollView.ResetPosition();
                    //SetPageInfo();
                    allofMultList.gameObject.SetActive(true);
                    allofMultList.InSize(powerObjs.Length, 5);
                    allofMultList.Info(powerObjs);
                    scrollView.transform.localPosition = scrollViewPos;
                    allofMultList.transform.localPosition = multListPos;
                    //scrollView.ResetPosition();
                    powerMultList.gameObject.SetActive(false);
                    agileMultList.gameObject.SetActive(false);
                    intelligenceMultList.gameObject.SetActive(false);
                    SetPageInfo();
                    break;
                case 2:
                    //allofMultList.gameObject.SetActive(false);
                    //powerMultList.gameObject.SetActive(false);
                    //agileMultList.gameObject.SetActive(true);
                    //intelligenceMultList.gameObject.SetActive(false);
                    //agileMultList.InSize(agileObjs.Length, 5);
                    //agileMultList.Info(agileObjs);
                    //scrollView.transform.localPosition = scrollViewPos;
                    //agileMultList.transform.localPosition = multListPos;
                    ////scrollView.ResetPosition();
                    //SetPageInfo();
                    allofMultList.gameObject.SetActive(true);
                    allofMultList.InSize(agileObjs.Length, 5);
                    allofMultList.Info(agileObjs);
                    scrollView.transform.localPosition = scrollViewPos;
                    allofMultList.transform.localPosition = multListPos;
                    //scrollView.ResetPosition();
                    powerMultList.gameObject.SetActive(false);
                    agileMultList.gameObject.SetActive(false);
                    intelligenceMultList.gameObject.SetActive(false);
                    SetPageInfo();
                    break;
                case 3:
                    //allofMultList.gameObject.SetActive(false);
                    //powerMultList.gameObject.SetActive(false);
                    //agileMultList.gameObject.SetActive(false);
                    //intelligenceMultList.gameObject.SetActive(true);
                    //intelligenceMultList.InSize(intelligenceObjs.Length, 5);
                    //intelligenceMultList.Info(intelligenceObjs);
                    //scrollView.transform.localPosition = scrollViewPos;
                    //intelligenceMultList.transform.localPosition = multListPos;
                    ////scrollView.ResetPosition();
                    //SetPageInfo();
                    allofMultList.gameObject.SetActive(true);
                    allofMultList.InSize(intelligenceObjs.Length, 5);
                    allofMultList.Info(intelligenceObjs);
                    scrollView.transform.localPosition = scrollViewPos;
                    allofMultList.transform.localPosition = multListPos;
                    //scrollView.ResetPosition();
                    powerMultList.gameObject.SetActive(false);
                    agileMultList.gameObject.SetActive(false);
                    intelligenceMultList.gameObject.SetActive(false);
                    SetPageInfo();
                    break;
                default:
                    break;
            }
        }
    }

    void RefreshScrollView()
    {

    }
    /// <summary>
    /// 下一页按钮事件
    /// </summary>
    private void OnNextPageClick()
    {
        currentPage++;
        if (1 < currentPage)
        {
            upPageBtn.gameObject.SetActive(true);
        }
        if (currentPage < maxPage)
        {
            nextPageBtn.gameObject.SetActive(true);
        }
        if (currentPage >= maxPage)
        {
            nextPageBtn.gameObject.SetActive(false);
        }
        SetMultListPosition();
        //scrollView.transform.localPosition = new();
    }
    /// <summary>
    /// 设置MultList的位置
    /// </summary>
    private void SetMultListPosition()
    {
        allofMultList.transform.localPosition = new Vector3(-146f, 100 + (currentPage - 1) * 600, 0f);
        //if (currentLab == 0)
        //{
        //    allofMultList.transform.localPosition = new Vector3(-146f, 100 + (currentPage - 1) * 600, 0f);
        //}
        //else if (currentLab == 1)
        //{
        //    powerMultList.transform.localPosition = new Vector3(-146f, 100 + (currentPage - 1) * 600, 0f);
        //}
        //else if (currentLab == 2)
        //{
        //    agileMultList.transform.localPosition = new Vector3(-146f, 100 + (currentPage - 1) * 600, 0f);
        //}
        //else if (currentLab == 3)
        //{
        //    intelligenceMultList.transform.localPosition = new Vector3(-146f, 100 + (currentPage - 1) * 600, 0f);
        //}
    }
    /// <summary>
    /// 上一页按钮事件
    /// </summary>
    private void OnUpPageClick()
    {
        currentPage--;
        if (1 < currentPage)
        {
            upPageBtn.gameObject.SetActive(true);
        }
        if (currentPage < maxPage)
        {
            nextPageBtn.gameObject.SetActive(true);
        }
        if (currentPage <= 1)
        {
            upPageBtn.gameObject.SetActive(false);
        }
        SetMultListPosition();
    }
    /// <summary>
    /// 返回按钮
    /// </summary>
    private void OnBackBtnClick()
    {
        ReductionMultListPos();
        Hide();
        Control.ShowGUI(UIPanleID.UIMoney, EnumOpenUIType.DefaultUIOrSecond);
        Control.ShowGUI(UIPanleID.UIKnapsack, EnumOpenUIType.OpenNewCloseOld);
    }
    /// <summary>
    /// 还原multlist的初始位置
    /// </summary>
    private void ReductionMultListPos()
    {
        scrollView.transform.localPosition = scrollViewPos;
        allofMultList.transform.localPosition = multListPos;
        //powerMultList.transform.localPosition = multListPos;
        //agileMultList.transform.localPosition = multListPos;
        //intelligenceMultList.transform.localPosition = multListPos;
    }

}