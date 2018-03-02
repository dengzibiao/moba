using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Tianyu;
public class UIRankList : GUIBase
{
    public GUISingleButton backBtn; //返回按钮
    //public UIButton[] TableBtn;
    //public UISprite[] MarkSp;
    //public UILabel[] ColorLab;
    //private Color _c;
    //public GUISingleLabel currentRankLab;//当前排名
    //public GUISingleSprite icon;//头像
    //public GUISingleSprite iconBorder;//头像框
    //public GUISingleLabel playerLv;//战队等级数值
    //public GUISingleLabel lvLab;//玩家等级
    //public GUISingleLabel playerName;//玩家名字
    //public GUISingleLabel currentValueLab;//当前数值
    //public GUISingleSprite fightSp;//战斗力
    public GUISingleMultList goodsMultList;//排行列表
    //public UIWidget goodMyIconWgt;
    //public UISprite[] myHeroIconss=new UISprite[3];//自己的三个英雄信息

    //public UIWidget PlayerBestWight;//玩家四强显示
    //public UISprite[] iconSp;//四强头像
    //public UISprite[] iconBoerderSp;//四强头像框
    //public UILabel[] bestFour_levelLab;//四强等级
    //public UILabel sumFight;//总战力
    //public UISprite[] bestFour_star1;
    //public UISprite[] bestFour_star2;
    //public UISprite[] bestFour_star3;
    //public UISprite[] bestFour_star4;
    //Dictionary<long, string> heroList;//所有英雄字典
    //public GUISingleSprite sumFightSP;//三个英雄总战力
    //public UISprite[] stars1=new UISprite[5];//星星显示
    //public UISprite[] stars2 = new UISprite[5];//星星显示//星星显示
    //public UISprite[] stars3 = new UISprite[5];//星星显示//星星显示
    //public UILabel[] lvLabss=new UILabel[3];//三个英雄的等级
    //public UISprite[] iconframe = new UISprite[3];//三个英雄的品阶框

    public object[] objs;

    private RankListData rankListData = new RankListData();
    public GUISingleButton lastBtn;//上一页
    public GUISingleButton nextBtn;//下一页
    public int currentPage = 0;//当前页数记录
    public GUISingleCheckBoxGroup checkBoxs;//页签
    public int index = 0;//标签下表
    public bool isRank;
    private RankListType type;
    public int count = 0;//记录数据条
    public static UIRankList _instance;
    public UIRankList()
    {
        _instance = this;
    }

    public override UIPanleID GetUIKey()
    {
        return UIPanleID.UIRankList;
    }
    protected override void Init()
    {
        base.Init();
        goodsMultList = transform.FindComponent<GUISingleMultList>("RankListScrollView/GoodsMultList");
        //goodMyIconWgt = transform.FindComponent<UIWidget>("GoodMyIconWgt");
        backBtn.onClick = OnBackBtnOnClick;
        ////查找三个英雄头像sp
        //for (int g = 0; g < myHeroIconss.Length;g++ )
        //{
        //    myHeroIconss[g] = transform.Find("GoodMyIconWgt/Icon" + (g + 1)).GetComponent<UISprite>();
        //    lvLabss[g] = transform.Find("GoodMyIconWgt/Icon" + (g + 1) + "/LevelLab").GetComponent<UILabel>();
        //    iconframe[g] = transform.Find("GoodMyIconWgt/Icon" + (g + 1) + "/IconFrame").GetComponent<UISprite>();
        //}
        /////查找三个英雄的星星
        //for (int i = 0; i < stars1.Length; i++)
        //{
        //    stars1[i] = transform.Find("GoodMyIconWgt/Icon1/Stars/Star" + (i + 1)).GetComponent<UISprite>();
        //    stars2[i] = transform.Find("GoodMyIconWgt/Icon2/Stars/Star" + (i + 1)).GetComponent<UISprite>();
        //    stars3[i] = transform.Find("GoodMyIconWgt/Icon3/Stars/Star" + (i + 1)).GetComponent<UISprite>();

        //}

        checkBoxs.onClick = OnCheckBoxClick;
        lastBtn.onClick = OnLastClick;
        nextBtn.onClick = OnNextClick;
        // checkBoxs.DefauleIndex = isRankss;
    }
    /// <summary>
    /// 切换页签的时候关闭掉页签碰撞，等数据接受完再打开
    /// </summary>
    /// <param name="index"></param>
    public void HideCheckBox(int index)
    {
        for (int i = 0; i < checkBoxs.GetBoxList().Length; i++)
        {
            if (index == i)
            {
                checkBoxs.GetBoxList()[i].GetComponent<BoxCollider>().enabled = true;
                checkBoxs.GetBoxList()[i].isEnabled = true;
            }
            else
            {
                checkBoxs.GetBoxList()[i].isEnabled = false;
                checkBoxs.GetBoxList()[i].GetComponent<BoxCollider>().enabled = false;
            }
        }
    }
    /// <summary>
    /// 打开页签的碰撞
    /// </summary>
    public void OpenCheckBox()
    {
        for (int i = 0; i < checkBoxs.GetBoxList().Length; i++)
        {
            checkBoxs.GetBoxList()[i].GetComponent<BoxCollider>().enabled = true;
            checkBoxs.GetBoxList()[i].isEnabled = true;
        }
    }
    protected override void ShowHandler()
    {
        base.ShowHandler();
        //icon.spriteName = playerData.GetInstance().iconData.icon_name + "_head";
        ////iconBorder.spriteName = playerData.GetInstance().iconFrameData.iconFrame_name;
        //playerName.text = playerData.GetInstance().selfData.playeName;
        //lvLab.text = playerData.GetInstance().selfData.level.ToString();

        //int fcSums = 0;
        //for (int i = 0; i < playerData.GetInstance().herodataList.Count; i++)
        //{
        //    fcSums += playerData.GetInstance().herodataList[i].fc;
        //}
        //playerData.GetInstance().selfData.FightLv = fcSums;  ///所有英雄的总战力
        Debug.Log(this.index + "showhandle");



        checkBoxs.setMaskState(this.index);

        Debug.Log(this.index + "showha");

        Debug.Log("222");
        TypeHandle(type);

    }
    //public void RefrashData()
    //{
    //    //icon.spriteName = playerData.GetInstance().iconData.icon_name + "_head";
    //    ////iconBorder.spriteName = playerData.GetInstance().iconFrameData.iconFrame_name;
    //    //playerName.text = playerData.GetInstance().selfData.playeName;
    //    if ((playerData.GetInstance().playerRankData.levelList != null && playerData.GetInstance().playerRankData.levelList.Count > 0) || (playerData.GetInstance().playerRankData.realTimeRankList != null && playerData.GetInstance().playerRankData.realTimeRankList.Count > 0))
    //    {
    //        TypeHandle();
    //    }
    //}
    public void CreatIconData()
    {
        isPageBtn();//按钮显示与否--没执行
        if (PageData() != null)//加判断如果是空 就不能执行
        {

            if (goodsMultList != null)
            {
                goodsMultList.InSize(4, 1);
                //Debug.LogError(PageData().Length);
                goodsMultList.Info(PageData());
            }

        }

    }
    private object[] OtherRankListData()
    {
        switch (this.type)
        {
            case RankListType.Fight:
                return playerData.GetInstance().playerRankData.fightList.ToArray();
            case RankListType.StarSum:
                return playerData.GetInstance().playerRankData.starSumList.ToArray();
            case RankListType.DiamondUser:
                return playerData.GetInstance().playerRankData.diamondUserList.ToArray();
            case RankListType.BestFourPersons:
                return playerData.GetInstance().playerRankData.bestFourPersonsList.ToArray();
            case RankListType.Fortune:
                return playerData.GetInstance().playerRankData.fortuneList.ToArray();
            case RankListType.Level:
                return playerData.GetInstance().playerRankData.levelList.ToArray();
            case RankListType.RealTimeRank:
                return playerData.GetInstance().playerRankData.realTimeRankList.ToArray();
            case RankListType.YesterdayRank:
                return playerData.GetInstance().playerRankData.yesterdayList.ToArray();
            default:
                return null;
        }
    }
    private void OnCheckBoxClick(int index, bool boo)
    {
        if (boo)
        {
            this.index = index;
            switch (index)
            {
                case 0:
                    //切换页签时 关闭掉页签的碰撞，等消息接受完再打开
                    currentPage = 0;
                    if (Globe.isOpenLevelSend)//判断1小时刷新一次
                        SendInfo(0);
                    else
                    {
                        this.type = RankListType.Level;
                        playerData.GetInstance().playerRankData.rankListType = RankListType.Level;
                        Show();
                    }
                    break;
                case 1:
                    //切换页签时 关闭掉页签的碰撞，等消息接受完再打开
                    currentPage = 0;
                    if (Globe.isOpenSend)
                    {
                        SendInfo(1);

                    }
                    else
                    {
                        playerData.GetInstance().playerRankData.rankListType = RankListType.RealTimeRank;
                        this.type = RankListType.RealTimeRank;
                        Show();
                        //    OpenCheckBox();
                    }
                    break;
                default:
                    break;
            }
        }
    }
    
    public bool Gettimers(long time)
    {
        long timerss = long.Parse(TimeManager.Instance.ConvertIntDateTime(Auxiliary.GetNowTime()).ToString("HH"));
        if (timerss > time)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    /// <summary>
    /// 最强四人
    /// </summary>
    public void OnbestFourPersonsClick()
    {
        currentPage = 0;
        playerData.GetInstance().playerRankData.rankListType = RankListType.BestFourPersons;
        long timerss = long.Parse(TimeManager.Instance.ConvertIntDateTime(Auxiliary.GetNowTime()).ToString("HH"));
        if (Gettimers(playerData.GetInstance().playerRankData.bestFourLog))
        {
            playerData.GetInstance().playerRankData.bestFourLog = timerss;
        }
        else
        {
           // Control.ShowGUI(GameLibrary.UIRankList);
        }

    }
    private void TypeHandle(RankListType type)
    {
        switch (type)
        {
            //case RankListType.Fight:
            //    PlayerBestWight.gameObject.SetActive(false);
            //    fightSp.gameObject.SetActive(true);
            //    fightSp.GetComponentInChildren<GUISingleLabel>().text = playerData.GetInstance().playerRankData.fight.ToString();
            //    currentRankLab.text = playerData.GetInstance().playerRankData.fightCurrentRank.ToString();
            //    CreatIconData();
            //    break;
            //case RankListType.StarSum:
            //    PlayerBestWight.gameObject.SetActive(false);
            //    fightSp.gameObject.SetActive(false);
            //    currentRankLab.text = playerData.GetInstance().playerRankData.starttSumCurrentRank.ToString();
            //    CreatIconData();
            //    break;
            //case RankListType.DiamondUser:
            //    PlayerBestWight.gameObject.SetActive(false);
            //    CreatIconData();  
            //    break;
            //case RankListType.BestFourPersons:
            //    PlayerBestWight.gameObject.SetActive(true);
            //    sumFightSP.gameObject.SetActive(true);
            //    fightSp.gameObject.SetActive(false);
            //    currentRankLab.text = playerData.GetInstance().playerRankData.bestfourCurrentRank.ToString();
            //    List<HeroData> mCurCanPickHero = playerData.GetInstance().herodataList;
            //    for (int i = 0; i < iconSp.Length;i++ ) 
            //    {
            //        if (i > 3) break;
            //        iconSp[i].gameObject.SetActive(i < mCurCanPickHero.Count);
            //    }

            //    //总数据排序
            //    mCurCanPickHero.Sort((a, b) => b.fc - a.fc);//a、b相等于一个类型
            //    //所有英雄数据
            //    Dictionary<long, HeroNode> mAllHeroNOde = FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList;
            //    //遍历
            //    for (int i = 0; i < mCurCanPickHero.Count; i++)
            //    {
            //        if (i > 3)
            //        {
            //            break;
            //        }
            //        HeroNode hero;
            //        mAllHeroNOde.TryGetValue(mCurCanPickHero [i].id, out hero);
            //        if (hero != null)
            //        {
            //            iconSp[i].spriteName = hero.icon_name;
            //        }
            //        iconBoerderSp[i].atlas = ResourceManager.Instance().GetUIAtlas("Prop");
            //        switch (mCurCanPickHero[i].grade)//英雄品阶
            //        {
            //            case (int)GradeType.Gray:
            //                iconBoerderSp[i].spriteName = "bai";
            //                break;
            //            case (int)GradeType.Green:
            //                iconBoerderSp[i].spriteName = "lv";
            //                break;
            //            case (int)GradeType.Blue:
            //                iconBoerderSp[i].spriteName = "lan";
            //                break;
            //            case (int)GradeType.Purple:
            //                iconBoerderSp[i].spriteName = "zi";
            //                break;
            //            case (int)GradeType.Orange:
            //                iconBoerderSp[i].spriteName = "cheng";
            //                break;
            //            case (int)GradeType.Red:
            //                iconBoerderSp[i].spriteName = "hong";
            //                break;
            //            default:
            //                break;
            //        }
            //        bestFour_levelLab[i].text =mCurCanPickHero[i].lvl.ToString();
            //        int sumFc=0;
            //        sumFc += mCurCanPickHero[i].fc;
            //        sumFight.text = sumFc.ToString();

            //        for (int k = 0; k < bestFour_star1.Length; k++)
            //        {
            //            if (k < mCurCanPickHero[i].star)
            //            {
            //                bestFour_star1[k].gameObject.SetActive(true);
            //            }
            //        }
            //        for (int j = 0; j < bestFour_star2.Length; j++)
            //        {
            //            if (j < mCurCanPickHero[i].star)
            //            {
            //                bestFour_star2[j].gameObject.SetActive(true);
            //            }
            //        }
            //        for (int o = 0; o < bestFour_star3.Length; o++)
            //        {
            //            if (o < mCurCanPickHero[i].star)
            //            {
            //                bestFour_star3[o].gameObject.SetActive(true);
            //            }
            //        }
            //        for (int l = 0; l < bestFour_star4.Length; l++)
            //        {
            //            if (l < mCurCanPickHero[i].star)
            //            {
            //                bestFour_star4[l].gameObject.SetActive(true);
            //            }
            //        } 
            //        CreatIconData();
            //    } 
            //    break;
            //case RankListType.Fortune: 
            //   PlayerBestWight.gameObject.SetActive(false);
            //   CreatIconData();
            //   break;
            case RankListType.Level:
                //PlayerBestWight.gameObject.SetActive(false);
                //sumFightSP.gameObject.SetActive(false);
                //fightSp.gameObject.SetActive(true);
                //playerLv.gameObject.SetActive(true);
                //goodMyIconWgt.gameObject.SetActive(false);
                //currentRankLab.text = playerData.GetInstance().playerRankData.lvCurrentRank.ToString();//当前排名
                //playerLv.text = "战队等级：" + playerData.GetInstance().selfData.level.ToString();

                //fightSp.GetComponentInChildren<GUISingleLabel>().text = playerData.GetInstance().selfData.FightLv.ToString();
                CreatIconData();
                break;
            case RankListType.RealTimeRank:
                //PlayerBestWight.gameObject.SetActive(false);
                //fightSp.gameObject.SetActive(false);
                //playerLv.gameObject.SetActive(false);
                //sumFightSP.gameObject.SetActive(true);
                //goodMyIconWgt.gameObject.SetActive(true);
                //currentRankLab.text = playerData.GetInstance().playerRankData.realTimeCurrentRank.ToString();//当前排名

                // Dictionary<long, HeroNode> mAllHeroNOdes = FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList;
                // HeroNode heros=null;
                // HeroNode heross = null;
                // HeroNode herosss = null;
                //RankListData rankData=  playerData.GetInstance().playerRankData.realTimeRankList.Find(x => x.playerId == playerData.GetInstance().selfData.playerId);//查找表中是否有相等id
                //if (rankData != null)
                //{
                //    mAllHeroNOdes.TryGetValue(rankData.arenaHeroList[0].iconId1, out heros);
                //    myHeroIconss[0].spriteName = heros.icon_name;
                //    iconframe[0].spriteName = GoodsDataOperation.GetInstance().GetSmallHeroGrameByHeroGrade(rankData.arenaHeroList[0].iconIdFrame);


                //    mAllHeroNOdes.TryGetValue(rankData.arenaHeroList[1].iconId1, out heros);
                //    myHeroIconss[1].spriteName = heros.icon_name;
                //    iconframe[1].spriteName = GoodsDataOperation.GetInstance().GetSmallHeroGrameByHeroGrade(rankData.arenaHeroList[1].iconIdFrame);


                //    mAllHeroNOdes.TryGetValue(rankData.arenaHeroList[2].iconId1, out heros);
                //    myHeroIconss[2].spriteName = heros.icon_name;
                //    iconframe[2].spriteName = GoodsDataOperation.GetInstance().GetSmallHeroGrameByHeroGrade(rankData.arenaHeroList[2].iconIdFrame);

                //    sumFightSP.GetComponentInChildren<GUISingleLabel>().text = rankData.rankValue.ToString();//根据id取到他的战力
                //}
                //else//取本地的
                //{
                //    if (Globe.defendTeam.Length > 0)
                //    {
                //        if (Globe.defendTeam[0] != null && Globe.defendTeam[0].id != 0)
                //        {
                //            mAllHeroNOdes.TryGetValue(Globe.defendTeam[0].id, out heros);
                //            if (heros != null)
                //            {
                //                myHeroIconss[0].gameObject.SetActive(true);
                //                myHeroIconss[0].spriteName = heros.icon_name;
                //            }
                //            iconframe[0].spriteName = GoodsDataOperation.GetInstance().GetSmallHeroGrameByHeroGrade(Globe.defendTeam[0].grade);

                //            for (int i = 0; i < stars1.Length; i++)
                //            {
                //                stars1[i].gameObject.SetActive(false);
                //            }
                //            for (int j = 0; j < Globe.defendTeam[0].star; j++)
                //            {
                //                stars1[j].gameObject.SetActive(true);
                //            } 
                //            stars1[0].GetComponentInParent<UIGrid>().Reposition();
                //            lvLabss[0].text = Globe.defendTeam[0].lvl.ToString();
                //        }
                //        else
                //        {
                //            myHeroIconss[0].gameObject.SetActive(false);
                //        }
                //        if (Globe.defendTeam[4] != null && Globe.defendTeam[4].id != 0)
                //        {
                //            mAllHeroNOdes.TryGetValue(Globe.defendTeam[4].id, out heross);
                //            if (heross != null)
                //            {
                //                myHeroIconss[1].gameObject.SetActive(true);
                //                myHeroIconss[1].spriteName = heross.icon_name;
                //            }
                //            iconframe[1].spriteName = GoodsDataOperation.GetInstance().GetSmallHeroGrameByHeroGrade(Globe.defendTeam[4].grade);
                //            //显示星星
                //            for (int o = 0; o < stars2.Length; o++)
                //            {
                //                stars2[o].gameObject.SetActive(false);
                //            }
                //            for (int p = 0; p < Globe.defendTeam[4].star; p++)
                //            {
                //                stars2[p].gameObject.SetActive(true);
                //            } 
                //            stars2[1].GetComponentInParent<UIGrid>().Reposition();
                //            lvLabss[1].text = Globe.defendTeam[4].lvl.ToString();

                //        }
                //        else
                //        {
                //            myHeroIconss[1].gameObject.SetActive(false);
                //        }
                //        if (Globe.defendTeam[5] != null && Globe.defendTeam[5].id != 0)
                //        {
                //            mAllHeroNOdes.TryGetValue(Globe.defendTeam[5].id, out herosss);
                //            if (herosss != null)
                //            {
                //                myHeroIconss[2].gameObject.SetActive(true);
                //                myHeroIconss[2].spriteName = herosss.icon_name;
                //            }
                //            iconframe[2].spriteName = GoodsDataOperation.GetInstance().GetSmallHeroGrameByHeroGrade(Globe.defendTeam[5].grade);
                //            for (int a = 0; a < stars3.Length; a++)
                //            {
                //                stars3[a].gameObject.SetActive(false);
                //            }
                //            for (int c = 0; c < Globe.defendTeam[5].star; c++)
                //            {
                //                stars3[c].gameObject.SetActive(true);
                //            }
                //            stars3[2].GetComponentInParent<UIGrid>().Reposition();
                //            lvLabss[2].text = Globe.defendTeam[5].lvl.ToString();

                //        }
                //        else
                //        {
                //            myHeroIconss[2].gameObject.SetActive(false);
                //        }
                //        int sumFc = 0;
                //        for (int i = 0; i < Globe.defendTeam.Length; i++)
                //        {
                //            if(null!=Globe.defendTeam[i]&&Globe.defendTeam[i].id!=0)
                //            {
                //                sumFc += Globe.defendTeam[i].fc;
                //            }
                //        }
                //        sumFightSP.GetComponentInChildren<GUISingleLabel>().text = sumFc.ToString();
                //    }
                //}
                CreatIconData();
                break;
            case RankListType.YesterdayRank:
                //PlayerBestWight.gameObject.SetActive(false);
                //fightSp.gameObject.SetActive(false);
                //playerLv.gameObject.SetActive(false);
                //sumFightSP.gameObject.SetActive(true);
                //goodMyIconWgt.gameObject.SetActive(true);
                CreatIconData();
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// 上页按钮
    /// </summary>
    private void OnLastClick()
    {
        if (currentPage <= 0)
        {
            return;
        }
        currentPage--;
        CreatIconData();
    }
    /// <summary>
    /// 下页按钮
    /// </summary>
    private void OnNextClick()
    {
        if (currentPage >= playerData.GetInstance().playerRankData.page)
        {
            return;
        }
        currentPage++;
        CreatIconData();
    }
    /// <summary>
    /// 从列表中单独取4条数据
    /// </summary>
    /// <returns></returns>
    private object[] PageData()
    {
        int index = 0;
        //列表数量个数大于所取区间最大值正常显示，例如数据12g个 区间最大值也是12
        if (OtherRankListData().Length >= 4 * currentPage + 4)
        {
            object[] data = new object[4];//每次显示4个数据
            for (int i = 4 * currentPage; i <= 4 * currentPage + 3; i++)
            {
                data[index] = OtherRankListData()[i];
                index++;
            }
            return data;
        }
        //列表数量个数大于所取区间最小值，小于区间最大值，取最小值和列表的个数-1，例如数据6g个 区间4-7
        else if (OtherRankListData().Length >= 4 * currentPage + 1 && OtherRankListData().Length < 4 * currentPage + 4)
        {
            object[] data = new object[OtherRankListData().Length - 4 * currentPage];//显示最小区间到OtherRankListData().Length个数据
            for (int i = 4 * currentPage; i <= OtherRankListData().Length - 1; i++)
            {
                data[index] = OtherRankListData()[i];
                index++;
            }
            return data;
        }
        //列表数量个数小于所取区间最小值，取上次数据_replaceIndex-1，例如数据4g个 区间4-7
        else if (OtherRankListData().Length > 0 && OtherRankListData().Length < 4 * currentPage + 1)
        {
            object[] data = new object[OtherRankListData().Length - 4 * (currentPage - 1)];//显示上一个最小区间到OtherRankListData().Length个数据
            for (int i = 4 * (currentPage - 1); i <= OtherRankListData().Length - 1; i++)
            {
                data[index] = OtherRankListData()[i];
                index++;
            } currentPage -= 1;
            return data;
        }
        return null;
    }
    /// <summary>
    /// 上页和下页的按钮是否显示
    /// </summary>
    private void isPageBtn()
    {
        if (playerData.GetInstance().playerRankData.page > 1 && currentPage == 0)//当前页1页以上显示下一页
        {
            nextBtn.gameObject.SetActive(true);
            lastBtn.gameObject.SetActive(false);
        }
        else if (currentPage != 0 && currentPage == playerData.GetInstance().playerRankData.page - 1)
        {
            nextBtn.gameObject.SetActive(false);
            lastBtn.gameObject.SetActive(true);
        }
        else if (currentPage == 0 && currentPage == playerData.GetInstance().playerRankData.page - 1)
        {
            lastBtn.gameObject.SetActive(false);
            nextBtn.gameObject.SetActive(false);
        }
        else
        {
            lastBtn.gameObject.SetActive(true);
            nextBtn.gameObject.SetActive(true);
        }
    }

    protected override void SetUI(params object[] uiParams)
    {
        this.index = (int)uiParams[0];

        Debug.Log("idnex0");
        base.SetUI();
    }
    protected override void OnLoadData()
    {
        base.OnLoadData();
        Singleton<Notification>.Instance.RegistMessageID(MessageID.common_ranklist_ret, UIPanleID.UIRankList);//注册
        SendInfo(this.index);
    }
    public void SendInfo(int _indexx)
    {

        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        switch (_indexx)
        {
            case 0:
                newpacket.Add("typeId", (int)RankListType.Level);
                newpacket.Add("minV", 0);
                newpacket.Add("maxV", 4);
                playerData.GetInstance().playerRankData.rankListType = RankListType.Level;
                if (Globe.isOpenLevelSend)//判断1小时刷新一次
                {
                    this.index = _indexx;
                    Singleton<Notification>.Instance.Send(MessageID.common_ranklist_req, newpacket, C2SMessageType.ActiveWait);
                    Globe.isOpenLevelSend = false;
                }

                break;
            case 1:
                newpacket.Add("typeId", (int)RankListType.RealTimeRank);
                newpacket.Add("minV", 0);
                newpacket.Add("maxV", 4);
                playerData.GetInstance().playerRankData.rankListType = RankListType.RealTimeRank;
                if (Globe.isOpenSend)
                {
                    this.index = _indexx;
                    Singleton<Notification>.Instance.Send(MessageID.common_ranklist_req, newpacket, C2SMessageType.ActiveWait);
                    Globe.isOpenSend = false;
                }

                break;
        }
    }
    public override void ReceiveData(UInt32 messageID)
    {
        switch (messageID)
        {
            case MessageID.common_ranklist_ret:
                switch (this.index)
                {

                    case 0: playerData.GetInstance().playerRankData.rankListType = RankListType.Level; this.type = RankListType.Level; GetRankListData((int)RankListType.Level); break;
                    case 1: playerData.GetInstance().playerRankData.rankListType = RankListType.RealTimeRank; this.type = RankListType.RealTimeRank; GetRankListData((int)RankListType.RealTimeRank); break;
                }

                break;
        }
        base.ReceiveData(messageID);
    }
    public void GetRankListData(int type)
    {
        if (count >= 0 && count <= 4)//0/1/2
        {
            //ClientSendDataMgr.GetSingle().GetRankListSend().SendRankList(this._type, 0, 5);//0-15/16-31/32-46
            count++;
            if (count == 4)//如果是决斗场排行榜，接收到第二次数据 就显示
            {
                switch (type)
                {
                    case 5:
                        if (playerData.GetInstance().playerRankData.levelList != null && playerData.GetInstance().playerRankData.levelList.Count > 0)
                        {
                            Show(); OpenCheckBox(); count = 0;

                        }
                        break;
                    case 6:
                        if (playerData.GetInstance().playerRankData.realTimeRankList != null && playerData.GetInstance().playerRankData.realTimeRankList.Count > 0)
                        {
                            Show(); OpenCheckBox(); count = 0;
                        }
                        break;
                }

            }
        }

    }
    private void OnBackBtnOnClick()
    {
        OpenCheckBox();
        Control.HideGUI();
        // checkBoxs.setMaskState(0);
        // this.index = 0;
        Globe.isOpenSend = false;
        Globe.isOpenLevelSend = false;
    }

}
