using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Tianyu;
using System;

/// <summary>
/// 英雄列表
/// </summary>
public class UIHeroList : GUIBase
{
    //新手引导位置挂点
    public GameObject HeroButton;
    public override UIPanleID GetUIKey()
    {
        return UIPanleID.UIHeroList;
    }

    #region 字段属性

    public static UIHeroList instance;

    public delegate void OnClosePanel();
    public OnClosePanel OnClose;

    public GUISingleButton backBtn;                     //返回按钮
    public GUISingleCheckBoxGroup typeHeroTab;          //英雄类型标签
    public GUISingleMultList allofMultList;             //所有英雄
    public GUISingleMultList powerMultList;             //力量
    public GUISingleMultList agileMultList;             //敏捷
    public GUISingleMultList intelligenceMultList;      //智力

    long soul_gem;
    int currentSoul;
    int chooseIndex = 0;                                //默认标签选择

    Dictionary<long, object> heroList;                  //所有英雄字典
   // List<GameObject> tabLis = new List<GameObject>();  //类型列表
    GUISingleMultList[] tabList;
    public object[] obj;                                //所有英雄
    public Dictionary<long, HeroNode> objDic;           //全部英雄列表

    [HideInInspector]
    public List<long> summonList = new List<long>();    //可召唤英雄列表
    [HideInInspector]
    public GetHeroPanel getHeroPanel;                   //置换英雄

    public HeroData[] heroPlayList = new HeroData[6];
    public HeroData[] playReserved = new HeroData[6];

    public bool isLineup { get; set; }                 //是否出战
    public bool isUpdateHero { get; set; }            //是否刷新英雄
    public long summmonHero { get; set; }            //召唤英雄ID

    #endregion
    protected override void Init()
    {
        base.Init();

        instance = this;

        getHeroPanel = transform.Find("GetHeroPanel").GetComponent<GetHeroPanel>();

        allofMultList = transform.Find("HeroListScrollView/AllofMultList").GetComponent<GUISingleMultList>();
        powerMultList = transform.Find("HeroListScrollView/PowerMultList").GetComponent<GUISingleMultList>();
        agileMultList = transform.Find("HeroListScrollView/AgileMultList").GetComponent<GUISingleMultList>();
        intelligenceMultList = transform.Find("HeroListScrollView/IntelligenceMultList").GetComponent<GUISingleMultList>();
        tabList= transform.Find("HeroListScrollView").GetComponentsInChildren<GUISingleMultList>();
        //foreach (Transform item in transform.Find("HeroListScrollView"))
        //{
        //    tabList.Add(item.gameObject);
        //}

        backBtn.onClick = OnBackBtnClick;
        typeHeroTab.onClick = OnTypeHeroTabClick;

        //获取全部英雄
        objDic = FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList;

        int heroCount = 0;
        foreach (object hn in objDic.Values)
        {
            if (((HeroNode)hn).released == 1)
                heroCount++;
        }

        obj = new object[heroCount];

        int index = 0;
        foreach (object hn in objDic.Values)
        {
            if (((HeroNode)hn).released == 1)
            {
                if (index < obj.Length)
                {
                    obj[index] = hn;
                    index++;
                }
            }
        }

    }

    protected override void OnLoadData()
    {
        base.OnLoadData();
        Singleton<Notification>.Instance.RegistMessageID(MessageID.common_player_hero_info_ret, UIPanleID.UIHeroList);
        Show();
    }
    public override void ReceiveData(UInt32 messageID)
    {
        base.ReceiveData(messageID);
        switch (messageID)
        {
            case MessageID.common_player_hero_info_ret:
                if (Globe.isDetails)
                {
                    Control.HideGUI();
                    Globe.isDetails = false;
                }
                break;
        }
    }
    protected override void ShowHandler()
    {
        base.ShowHandler();

        InitListData();
    }

    #region 英雄列表赋值
    /// <summary>
    /// 读取英雄数据
    /// </summary>
    public void InitListData()
    {
        //全部英雄的字典
        heroList = new Dictionary<long, object>();

        //所有英雄的键值
        List<long> count = new List<long>();

        //排序后已英雄
        if (obj.Length <= 0)
            return;
        object[] objs = new object[obj.Length];

        List<object> notown = new List<object>();

        int index = 0;

        HeroNode heroNode;

        //卡牌排序可召唤英雄，拥有卡牌，未拥有卡牌
        if (playerData.GetInstance().herodataList.Count > 0)
        {
            //存储所有英雄
            for (int i = 0; i < obj.Length; i++)
            {
                heroNode = (HeroNode)obj[i];

                heroList.Add(heroNode.hero_id, obj[i]);

                count.Add(heroNode.hero_id);
            }

            //通过从背包中寻找魂石判断可召唤的英雄
            HeroNode hn;
            ItemData items;
            for (int i = 0; i < obj.Length; i++)
            {
                hn = (HeroNode)obj[i];

                if (playerData.GetInstance().GetHeroDataByID(hn.hero_id) != null) continue;

                items = playerData.GetInstance().GetItemDatatByID(hn.soul_gem);

                StarUpGradeNode starUpNode = FSDataNodeTable<StarUpGradeNode>.GetSingleton().FindDataByType(hn.init_star);

                if (null != items)
                {
                    if (items.Count >= starUpNode.call_stone_num)
                    {
                        if (!summonList.Contains(hn.hero_id))
                        {
                            summonList.Add(hn.hero_id);
                        }
                    }
                }

            }

            object obje = null;

            //可召唤的英雄在前
            if (summonList.Count > 0)
            {
                //可召唤英雄大于1时进行排序
                if (summonList.Count > 1) NotownSort(summonList);

                for (int i = 0; i < summonList.Count; i++)
                {
                    if (heroList.TryGetValue(summonList[i], out obje))
                    {
                        objs[index] = obje;
                        count.Remove(summonList[i]);
                        index++;
                        heroList.Remove(summonList[i]);
                    }
                }
            }

            //拥有英雄个数大于1进行排序
            if (playerData.GetInstance().herodataList.Count > 1)
            {
                HeroSort();
                //playerData.GetInstance().herodataList.Sort(new HeroDataComparer());
            }

            //已经拥有的英雄
            for (int i = 0; i < playerData.GetInstance().herodataList.Count; i++)
            {

                if (heroList.TryGetValue(playerData.GetInstance().herodataList[i].id, out obje))
                {
                    objs[index] = obje;
                    count.Remove(playerData.GetInstance().herodataList[i].id);
                    index++;
                    heroList.Remove(playerData.GetInstance().herodataList[i].id);
                }
            }

            //剩余未拥有英雄
            for (int i = 0; i < heroList.Count; i++)
            {
                if (heroList.TryGetValue(count[i], out obje))
                {
                    notown.Add(obje);
                }
            }

            //未拥有的英雄
            if (notown.Count > 0)
            {
                for (int i = 0; i < notown.Count; i++)
                {
                    objs[index] = notown[i];
                    index++;
                }
            }

            //进行显示
            InitHeroList(objs);
        }
        else//玩家未拥有卡牌
        {
            //进行显示
            InitHeroList(obj);
        }

    }

    /// <summary>
    /// 可召唤英雄排序
    /// </summary>
    /// <param name="sumHeroList"></param>
    /// <returns></returns>
    void NotownSort(List<long> sumHeroList)
    {
        long temp = 0;
        for (int i = 0; i < sumHeroList.Count - 1; i++)
        {
            for (int j = i + 1; j < sumHeroList.Count; j++)
            {
                if (sumHeroList[i] > sumHeroList[j])
                {
                    temp = sumHeroList[i];
                    sumHeroList[i] = sumHeroList[j];
                    sumHeroList[j] = temp;
                }
            }
        }
    }

    void HeroSort()
    {

        HeroData mainHero = null;

        List<HeroData> fuHero = new List<HeroData>();
        List<HeroData> Surplus = new List<HeroData>();

        mainHero = playerData.GetInstance().GetHeroDataByID(Globe.playHeroList[0].id);
        playerData.GetInstance().herodataList.Remove(Globe.playHeroList[0]);
        ExtPlayedHero(Globe.playHeroList[1], fuHero);
        ExtPlayedHero(Globe.playHeroList[2], fuHero);
        ExtPlayedHero(Globe.playHeroList[3], fuHero);

        for (int i = 0; i < playerData.GetInstance().herodataList.Count; i++)
        {
            Surplus.Add(playerData.GetInstance().herodataList[i]);
        }

        fuHero.Sort(new HeroDataComparer());
        Surplus.Sort(new HeroDataComparer());

        playerData.GetInstance().herodataList.Clear();

        playerData.GetInstance().herodataList.Add(mainHero);

        for (int i = 0; i < fuHero.Count; i++)
        {
            if (fuHero[i].id != Globe.playHeroList[0].id)
                playerData.GetInstance().herodataList.Add(fuHero[i]);
        }

        for (int i = 0; i < Surplus.Count; i++)
        {
            if (Surplus[i].id != Globe.playHeroList[0].id)
                playerData.GetInstance().herodataList.Add(Surplus[i]);
        }
    }

    void ExtPlayedHero(HeroData hd, List<HeroData> heroData)
    {
        if (null == hd) return;
        for (int i = 0; i < playerData.GetInstance().herodataList.Count; i++)
        {
            if (null != playerData.GetInstance().herodataList[i] && playerData.GetInstance().herodataList[i].id == hd.id)
            {
                heroData.Add(hd);
                playerData.GetInstance().herodataList.RemoveAt(i);
                return;
            }
        }
    }

    /// <summary>
    /// 英雄列表数据显示
    /// </summary>
    /// <param name="allHero"></param>
    void InitHeroList(object[] allHero)
    {
        for (int i = 0; i < tabList.Length; i++)
        {
            tabList[i].gameObject.SetActive(true);
        }

        int powerIndex = 0;
        int intelligenceIndex = 0;
        int agileIndex = 0;

        HeroNode nodeAll;

        //取出所有的英雄
        for (int i = 0; i < allHero.Length; i++)
        {

            nodeAll = (HeroNode)allHero[i];

            switch (nodeAll.attribute)
            {
                case 1:
                    powerIndex++;
                    break;
                case 2:
                    intelligenceIndex++;
                    break;
                case 3:
                    agileIndex++;
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

        HeroNode node;

        for (int i = 0; i < allHero.Length; i++)
        {
            node = (HeroNode)allHero[i];

            switch (node.attribute)
            {
                case 1:
                    powerObj[powerI] = allHero[i];
                    powerI++;
                    break;
                case 2:
                    intelligenceObj[intelligenceI] = allHero[i];
                    intelligenceI++;
                    break;
                case 3:
                    agileObj[agileI] = allHero[i];
                    agileI++;
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

        for (int i = 0; i < tabList.Length; i++)
        {
            if (i == chooseIndex) tabList[i].gameObject.SetActive(true);
            else tabList[i].gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 已拥有英雄排序
    /// </summary>
    void HasHeroSort()
    {
        object[] allHero = new object[playerData.GetInstance().herodataList.Count];

        if (playerData.GetInstance().herodataList.Count > 1)
            HeroSort();

        for (int i = 0; i < allHero.Length; i++)
        {
            foreach (HeroNode node in obj)
            {
                if (node.hero_id == playerData.GetInstance().herodataList[i].id)
                {
                    allHero[i] = node;
                }
            }
        }

        InitHeroList(allHero);
    }

    #endregion

    #region 功能
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
            allofMultList.transform.parent.GetComponent<UIScrollView>().ResetPosition();
        }
    }

    public void MobaMatchedAndSwitch()
    {
        Singleton<SceneManage>.Instance.Current = EnumSceneID.Dungeons;
        //UI_Loading.LoadScene(GameLibrary.PVP_Moba, 3, DareCallBack);
        GameLibrary.LastScene = SceneManager.GetActiveScene().name;//记录前一个场景名
        StartLandingShuJu.GetInstance().GetLoadingData(GameLibrary.PVP_Moba, 3, DareCallBack);
        SceneManager.LoadScene("Loding");
        Hide();
        Control.HideGUI(UIPanleID.UIPvP);
    }

    /// <summary>
    /// 返回按钮
    /// </summary>
    private void OnBackBtnClick()
    {

        if (null != OnClose)
            OnClose();

        Globe.DestroyAppearedEffect();

        //Hide();
        Control.HideGUI();
        UIRole.Instance.SetMainHeroLevel();
        Control.PlayBGmByClose(this.GetUIKey());
    }
    #endregion

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

    public void SaveSoul(long id, int count)
    {
        soul_gem = id;
        currentSoul = count;
    }

    /// <summary>
    /// 打开获取英雄面板
    /// </summary>
    public void OpenGetHeroPanel(int afc, int lvl, int star, int grade)
    {
        //将英雄数据存入列表
        HeroData sumHD = new HeroData(summmonHero, lvl, grade, star);
        if (afc != 0)
            sumHD.fc = afc;
        else
            sumHD.RefreshAttr();
        sumHD.node.isHas = true;

        if (!playerData.GetInstance().herodataList.Contains(sumHD))
            playerData.GetInstance().herodataList.Add(sumHD);

        //召唤成功之后从可召唤影响列表移除
        if (summonList.Contains(summmonHero))
            summonList.Remove(summmonHero);

        //GoodsDataOperation.GetInstance().UseGoods((int)soul_gem, currentSoul);

        object []ob=new object[4] {summmonHero,ShowHeroEffectType.HeroList,HeroOrSoul.Hero,0 };
        Control.ShowGUI(UIPanleID.UILottryHeroEffect, EnumOpenUIType.DefaultUIOrSecond, false, ob);
        //getHeroPanel.gameObject.SetActive(true);
        //getHeroPanel.Init();
        InitListData();
    }

    void DareCallBack()
    {
        //int index = 0;
        //for (int i = 1; i < 4; i++)
        //{
        //    if (null != heroPlayList[i] && heroPlayList[i].id != 0)
        //    {
        //        Globe.SkillHeroList[index] = FSDataNodeTable<HeroAttrNode>.GetSingleton().FindDataByType((int)heroPlayList[i].id + 1);
        //        index++;
        //    }
        //}
    }

    protected override void RegisterComponent()
    {
        base.RegisterComponent();
       
        RegisterComponentID(15, 93, HeroButton);

    }
    protected override void RegisterIsOver()
    {
        base.RegisterIsOver();
    }
}

/// <summary>
/// 队列排序
/// </summary>
class HeroDataComparer : IComparer<HeroData>
{
    public int Compare(HeroData x, HeroData y)
    {
        if (x.fc > y.fc)
        {
            return -1;
        }
        else if (x.fc < y.fc)
        {
            return 1;
        }
        else
        {
            if (x.lvl > y.lvl)
            {
                return -1;
            }
            else if (x.lvl < y.lvl)
            {
                return 1;
            }
            else
            {
                if (x.id > y.id)
                {
                    return 1;
                }
                else if (x.id < y.id)
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            }
        }
    }
}