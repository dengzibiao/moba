using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

/// <summary>
/// 英雄出战面板
/// </summary>

public class UI_HeroPlay : GUIBase
{

    public static UI_HeroPlay instance;

    public GUISingleButton backBtn;
    public GUISingleCheckBoxGroup typeHeroTab;
    public GUISingleMultList allofMultList;
    public GUISingleMultList powerMultList;
    public GUISingleMultList agileMultList;
    public GUISingleMultList intelligenceMultList;
    public GUISingleButton mainHero;
    public GUISingleButton fuHero1;
    public GUISingleButton fuHero2;
    public GUISingleButton fuHero3;
    public GUISingleButton fightingBtn;

    public UISprite mainMask;
    public UISprite fuHero1Mask;
    public UISprite fuHero2Mask;
    public UISprite fuHero3Mask;
    public UILabel prompt;

    object[] objAll;

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

        mainMask = mainHero.transform.Find("mark").GetComponent<UISprite>();
        fuHero1Mask = fuHero1.transform.Find("mark").GetComponent<UISprite>();
        fuHero2Mask = fuHero2.transform.Find("mark").GetComponent<UISprite>();
        fuHero3Mask = fuHero3.transform.Find("mark").GetComponent<UISprite>();
        prompt = transform.Find("PromptLabel").GetComponent<UILabel>();

        mainMask.enabled = false;
        fuHero1Mask.enabled = false;
        fuHero2Mask.enabled = false;
        fuHero3Mask.enabled = false;

        backBtn.onClick = OnBackBtnClick;
        typeHeroTab.onClick = OnTypeHeroTabClick;
        mainHero.onClick = OnMainHero;
        fuHero1.onClick = OnFuHero1Click;
        fuHero2.onClick = OnFuHero2Click;
        fuHero3.onClick = OnFuHero3Click;
        fightingBtn.onClick = OnFightingBtnClick;

        //全部英雄
        objAll = VOManager.Instance().GetCSV<HeroTableCSV>("HeroTable").GetVoList();

        //if (Globe.heroPlay.Count == 1)
        //{
        //    mainMask.enabled = true;
        //    mainMask.spriteName = Globe.mainVO.icon_name;
        //    fuHero1Mask.enabled = false;
        //    fuHero2Mask.enabled = false;
        //    fuHero3Mask.enabled = false;
        //}
        //else if (Globe.heroPlay.Count == 2)
        //{
        //    mainMask.enabled = true;
        //    mainMask.spriteName = Globe.mainVO.icon_name;
        //    fuHero1Mask.enabled = true;
        //    fuHero1Mask.spriteName = Globe.fu1VO.icon_name;
        //    fuHero2Mask.enabled = false;
        //    fuHero3Mask.enabled = false;
        //}
        //else if (Globe.heroPlay.Count == 3)
        //{
        //    mainMask.enabled = true;
        //    mainMask.spriteName = Globe.mainVO.icon_name;
        //    fuHero1Mask.enabled = true;
        //    fuHero1Mask.spriteName = Globe.fu1VO.icon_name;
        //    fuHero2Mask.enabled = true;
        //    fuHero2Mask.spriteName = Globe.fu2VO.icon_name;
        //    fuHero3Mask.enabled = false;
        //}
        //else if (Globe.heroPlay.Count == 4)
        //{
        //    mainMask.enabled = true;
        //    mainMask.spriteName = Globe.mainVO.icon_name;
        //    fuHero1Mask.enabled = true;
        //    fuHero1Mask.spriteName = Globe.fu1VO.icon_name;
        //    fuHero2Mask.enabled = true;
        //    fuHero2Mask.spriteName = Globe.fu2VO.icon_name;
        //    fuHero3Mask.enabled = true;
        //    fuHero3Mask.spriteName = Globe.fu3VO.icon_name;
        //}

    }

    /// <summary>
    /// 读取数据
    /// </summary>
    void InitPlayData()
    {

        //全部英雄字典
        Dictionary<int, object> objDic = new Dictionary<int, object>();

        object ob;

        //已拥有的全部英雄
        object[] objHave = new object[Globe.alreadyHeroList.Count];

        object[] powerObjN;         //力量英雄
        object[] agileObjN;         //敏捷英雄
        object[] intelligenceObjN;  //智力英雄

        int powerIndex = 0;
        int agileIndex = 0;
        int intelligenceIndex = 0;

        if (Globe.alreadyHeroList.Count > 0)
        {
            //将所有英雄的数据保存在字典中
            for (int i = 0; i < objAll.Length; i++)
            {
                HeroTableVO heroVOAll = (HeroTableVO)objAll[i];
                objDic.Add(heroVOAll.id, objAll[i]);
            }

            //根据ID寻找已经拥有的英雄
            for (int i = 0; i < Globe.alreadyHeroList.Count; i++)
            {
                if (objDic.TryGetValue(Globe.alreadyHeroList[i], out ob))
                {
                    objHave[i] = ob;
                }
            }

            //遍历已拥有的英雄，设置每个类型的个数
            for (int i = 0; i < objHave.Length; i++)
            {
                HeroTableVO vo = (HeroTableVO)objHave[i];
                switch (vo.main_prop)
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

            powerObjN = new object[powerIndex];
            agileObjN = new object[agileIndex];
            intelligenceObjN = new object[intelligenceIndex];

            int powerIN = 0;
            int agileIN = 0;
            int intelligenceIN = 0;

            //便利已经拥有的所有英雄进行分类
            for (int i = 0; i < objHave.Length; i++)
            {
                HeroTableVO heroVOo = (HeroTableVO)objHave[i];

                switch (heroVOo.main_prop)
                {
                    case 1:
                        powerObjN[powerIN] = objHave[i];
                        powerIN++;
                        break;
                    case 2:
                        agileObjN[agileIN] = objHave[i];
                        agileIN++;
                        break;
                    case 3:
                        intelligenceObjN[intelligenceIN] = objHave[i];
                        intelligenceIN++;
                        break;
                    default:
                        break;
                }

            }

            //全部英雄
            allofMultList.InSize(objHave.Length, 5);
            allofMultList.Info(objHave);

            powerMultList.InSize(powerIndex, 5);
            powerMultList.Info(powerObjN);

            agileMultList.InSize(agileIndex, 5);
            agileMultList.Info(agileObjN);

            intelligenceMultList.InSize(intelligenceIndex, 5);
            intelligenceMultList.Info(intelligenceObjN);

            powerMultList.gameObject.SetActive(false);
            agileMultList.gameObject.SetActive(false);
            intelligenceMultList.gameObject.SetActive(false);

        }
    }

    protected override void ShowHandler()
    {
        base.ShowHandler();

        powerMultList.gameObject.SetActive(true);
        agileMultList.gameObject.SetActive(true);
        intelligenceMultList.gameObject.SetActive(true);

        //if (Globe.heroPlay.Count > 0)
        //{
        //    mainMask.enabled = true;
        //    mainMask.spriteName = Globe.mainVO.icon_name;
        //    Globe.mainS = SubStringName(Globe.mainVO.icon_name);
        //    GameLibrary.player = Globe.mainS;
        //}

        InitPlayData();

    }

    /// <summary>
    /// 出战按钮
    /// </summary>
    private void OnFightingBtnClick()
    {

        if (!mainMask.enabled)
        {
            ShowPrompt();
            return;
        }

        ////设置主英雄
        //Globe.mainS = SubStringName(Globe.gMainNode.icon_name);
        //GameLibrary.player = Globe.mainS;

        ////辅助英雄
        //GameLibrary.heroTeam = new List<string>();

        //SelectHeroPlay(Globe.gMainNode);

        //if (fuHero1Mask.enabled)
        //{
        //    SelectHeroPlay(Globe.gFu1Node);
        //    AddHeroTeam(Globe.fu1S);
        //}
        //else
        //{
        //    ClearData(Globe.gFu1Node, Globe.fu1S);
        //}

        //if (fuHero2Mask.enabled)
        //{
        //    SelectHeroPlay(Globe.gFu2Node);
        //    AddHeroTeam(Globe.fu2S);
        //}
        //else
        //{
        //    ClearData(Globe.gFu2Node, Globe.fu2S);
        //}

        //if (fuHero3Mask.enabled)
        //{
        //    SelectHeroPlay(Globe.gSummon1Node);
        //    AddHeroTeam(Globe.fu3S);
        //}
        //else
        //{
        //    ClearData(Globe.gSummon1Node, Globe.fu3S);
        //}

        
        //---

        int index = 0;

        for (int i = 0; i < GameLibrary.heroTeam.Count; i++)
        {
            if (GameLibrary.heroTeam[i] != "0")
            {
                index++;
            }
        }

        print("主战英雄:" + GameLibrary.player);
        print("辅助人数:" + index);

        for (int i = 0; i < GameLibrary.heroTeam.Count; i++)
        {
            if (GameLibrary.heroTeam[i] == "0")
            {
                print("辅助 " + i + " : 无");
            }
            else
            {
                print("辅助 " + i + " : " + GameLibrary.heroTeam[i]);
            }
            
        }

        //---

        AddHeroPlay();

        //跳转场景
        Control.HideGUI(UIPanleID.UIHeroPlay);
        //UI_Loading.LoadScene(GameLibrary.chooseFB, 3f);

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
    /// 选择出战的英雄
    /// </summary>
    /// <param name="vo"></param>
    void SelectHeroPlay(HeroNode vo)
    {
        //if (Globe.heroPlay.Contains(vo))
        //{
        //    Globe.heroPlay.Remove(vo);
        //}
        //Globe.heroPlay.Add(vo);
    }

    /// <summary>
    /// 清空数据
    /// </summary>
    void ClearData(HeroNode vo, string name)
    {
        //if (Globe.heroPlay.Contains(vo))
        //{
        //    Globe.heroPlay.Remove(vo);
        //}
        //if (GameLibrary.heroTeam.Contains(name))
        //{
        //    GameLibrary.heroTeam.Remove(name);
        //}
        //GameLibrary.heroTeam.Add("0");
    }

    /// <summary>
    /// 辅助英雄1
    /// </summary>
    private void OnFuHero1Click()
    {
        //fuHero1Mask.enabled = false;
        ////if (Globe.fu1VO != null)
        ////{
        //    AddPlayDic(Globe.gFu1Node.hero_id);
        ////}
    }

    /// <summary>
    /// 辅助英雄1上阵
    /// </summary>
    /// <param name="heroVO"></param>
    public void FuHero1Battle(HeroNode heroVO)
    {
        //fuHero1Mask.enabled = true;
        //fuHero1Mask.spriteName = heroVO.icon_name;
        //Globe.fu1S = SubStringName(heroVO.icon_name);
        //Globe.gFu1Node = heroVO;
        //Globe.playHeroList[1] = Globe.gFu1Node;
    }

    /// <summary>
    /// 辅助英雄2
    /// </summary>
    private void OnFuHero2Click()
    {
        //fuHero2Mask.enabled = false;
        ////if (Globe.fu2VO != null)
        ////{
        //    AddPlayDic(Globe.gFu2Node.hero_id);
        //}
    }

    /// <summary>
    /// 辅助英雄2上阵
    /// </summary>
    /// <param name="heroVO"></param>
    public void FuHero2Battle(HeroNode heroVO)
    {
        //fuHero2Mask.enabled = true;
        //fuHero2Mask.spriteName = heroVO.icon_name;
        //Globe.fu2S = SubStringName(heroVO.icon_name);
        //Globe.gFu2Node = heroVO;
        //Globe.playHeroList[2] = Globe.gFu2Node;
    }

    /// <summary>
    /// 辅助英雄3
    /// </summary>
    private void OnFuHero3Click()
    {
        //fuHero3Mask.enabled = false;
        ////if (Globe.fu3VO != null)
        ////{
        //    AddPlayDic(Globe.gSummon1Node.hero_id);
        ////}
    }

    /// <summary>
    /// 辅助英雄3上阵
    /// </summary>
    /// <param name="heroVO"></param>
    public void FuHero3Battle(HeroNode heroVO)
    {
        //fuHero3Mask.enabled = true;
        //fuHero3Mask.spriteName = heroVO.icon_name;
        //Globe.fu3S = SubStringName(heroVO.icon_name);
        //Globe.gSummon1Node = heroVO;
        //Globe.playHeroList[3] = Globe.gSummon1Node;
    }

    /// <summary>
    /// 主英雄按钮
    /// </summary>
    private void OnMainHero()
    {
        //mainMask.enabled = false;
        //AddPlayDic(Globe.gMainNode.hero_id);
    }

    /// <summary>
    /// 主英雄上阵
    /// </summary>
    /// <param name="heroVO"></param>
    public void MainHeroBattle(HeroNode heroVO)
    {
        //mainMask.enabled = true;
        //mainMask.spriteName = heroVO.icon_name;
        //Globe.mainS = SubStringName(heroVO.icon_name);
        //Globe.gMainNode = heroVO;
        //Globe.playHeroList[0] = Globe.gMainNode;
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
    /// 返回主界面按钮
    /// </summary>
    private void OnBackBtnClick()
    {

        if (!mainMask.enabled)
        {
            //ShowPrompt();
            //return;
        }
        else
        {
            //Globe.mainS = SubStringName(Globe.gMainNode.icon_name);
            //GameLibrary.player = Globe.mainS;
            //CharacterManager.instance.CreatCharacter();

            //AddHeroPlay();

        }

        if (GameLibrary.isShowPlay)
        {
            Control.Hide(UIPanleID.UIHeroPlay);
            Control.Show(UIPanleID.UILevel);
        }
        else
        {
            Control.Hide(UIPanleID.UIHeroPlay);
        }

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
    void AddPlayDic(long id)
    {
        //if (Globe.playDic.ContainsKey(id))
        //{
        //    Globe.playDic.Remove(id);
        //}

        //Globe.playDic.Add(id, false);
    }

    /// <summary>
    /// 显示提示
    /// </summary>
    void ShowPrompt()
    {
        prompt.GetComponent<TweenAlpha>().ResetToBeginning();
        prompt.GetComponent<TweenAlpha>().PlayForward();
    }

    /// <summary>
    /// 将上阵的英雄信息保留
    /// </summary>
    void AddHeroPlay()
    {
        //Globe.playHeroList[0] = Globe.mainVO;

        //HeroVO vo = new HeroVO();

        //if (fuHero1Mask.enabled)
        //{
        //    Globe.playHeroList[1] = Globe.fu1VO;
        //}
        //else
        //{
        //    Globe.playHeroList[1] = vo;
        //}

        //if (fuHero2Mask.enabled)
        //{
        //    Globe.playHeroList[2] = Globe.fu2VO;
        //}
        //else
        //{
        //    Globe.playHeroList[2] = vo;
        //}

        //if (fuHero3Mask.enabled)
        //{
        //    Globe.playHeroList[3] = Globe.fu3VO;
        //}
        //else
        //{
        //    Globe.playHeroList[3] = vo;
        //}

        //print(Globe.playHeroList.Length);
    }

}