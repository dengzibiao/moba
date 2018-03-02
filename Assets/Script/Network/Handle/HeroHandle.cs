using System;
using UnityEngine;
using System.Collections.Generic;
using Tianyu;

public class CHeroHandle : CHandleBase
{
    public CHeroHandle(CHandleMgr mgr) : base(mgr)
    {
    }

    public override void RegistAllHandle()
    {
        RegistHandle(MessageID.common_player_hero_list_ret, GetHeroResultHandle);//获取英雄列表
        RegistHandle(MessageID.common_player_hero_info_ret, GetHeroInfoResultHandle);//获取英雄信息
        RegistHandle(MessageID.common_assign_fight_hero_ret, AssignFightHeroHandle);//指定出战英雄
        RegistHandle(MessageID.common_soulgem_exchange_hero_ret, SoulStoneChangeHeroResultHandle);//魂石置换英雄
        RegistHandle(MessageID.common_upgrade_hero_star_ret, UpgradeHeroStarResultHandle);//英雄升星
        RegistHandle(MessageID.common_hero_equipment_list_ret, GetHeroEquipListResultHandle);//获取英雄装备
        RegistHandle(MessageID.common_upgrade_hero_level_ret, GetDrugUpgradeResultHandle);//药瓶升级
        RegistHandle(MessageID.common_upgrade_hero_equipment_ret, GetUpGradeHEResultHandle);//升级英雄装备
        RegistHandle(MessageID.common_upgrade_hero_grade_ret, GetHeroAdvancedResultHandle);//英雄进阶
        RegistHandle(MessageID.common_hero_equipment_volve_ret, GetHeroEMonResultHandle);//英雄装备进化
        RegistHandle(MessageID.common_hero_equipment_compound_ret, GetHeroEComResultHandle);//英雄装备合成
        RegistHandle(MessageID.common_hero_runes_list_ret, GetRunesListResultHandle);//获取符文列表
        RegistHandle(MessageID.common_equip_hero_runes_ret, GetEquipRunesResultHandle);//装备符文
        RegistHandle(MessageID.common_compound_hero_runes_ret, GetRunesCompoundResultHandle);//合成符文
        RegistHandle(MessageID.common_upgrade_hero_skill_ret, HeroSkillUpGradeResultHandle);//技能升级

        RegistHandle(MessageID.login_force_quit_game_notify, ServerOnQuit);//强制退出游戏


    }
    public bool ServerOnQuit(CReadPacket packet)
    {
        ClientSendDataMgr.GetSingle().GetHeroSend().SendQuitGame();
        //UIPromptBox.Instance.ShowLabel("服务器维护中！请重新登录");
        //Control.ShowGUI(GameLibrary.UIPromptBox);
        Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, "服务器维护中！请重新登录");
        return true;
    }
    public bool KeepAliveHandle(CReadPacket packet)
    {
        return true;
    }

    /// <summary>
    /// 获取英雄列表          4002
    /// </summary>
    /// <param name="packet"></param>
    /// <returns></returns>
    public bool GetHeroResultHandle(CReadPacket packet)
    {
        Dictionary<string, object> data = packet.data;
        int ret = int.Parse(data["ret"].ToString());
        if (ret == 0)
        {
            if (data.ContainsKey("item"))
            {
                object[] item = data["item"] as object[];
                if (null != item)
                {
                    //playerData.GetInstance().herodataList.Clear();
                    Dictionary<string, object> dict;
                    bool isHaveHero = false;
                    for (int i = 0; i < item.Length; i++)
                    {
                        isHaveHero = false;
                        dict = item[i] as Dictionary<string, object>;
                        for (int j = 0; j < playerData.GetInstance().herodataList.Count; j++)
                        {
                            if ((int)dict["id"] == playerData.GetInstance().herodataList[j].id)
                            {
                                playerData.GetInstance().herodataList[j].SetData(item[i]);
                                isHaveHero = true;
                                break;
                            }
                        }
                        if (!isHaveHero)
                        {
                            HeroData hd = new HeroData(item[i]);
                            hd.actived = true;
                            hd.playerId = Globe.SelectedServer.playerId;
                            if (!playerData.GetInstance().herodataList.Contains(hd))
                            {
                                playerData.GetInstance().herodataList.Add(hd);
                            }
                        }
                    }
                    //  DoHeroListByReqType();
                    //  刷新英雄列表
                    if (UIHeroList.instance && UIHeroList.instance.gameObject.activeSelf)
                    {
                        UIHeroList.instance.SummonHero();
                    }
                    if (Singleton<SceneManage>.Instance.Current == EnumSceneID.UI_MajorCity01 && UIRole.Instance != null && UIRole.Instance.gameObject.activeSelf)
                    {
                        UIRole.Instance.SetMainHeroLevel();
                    }


                }
            }
            //if (UI_Setting.GetInstance().isShowHeroBtn)
            //{
            //    Control.ShowGUI(GameLibrary.UI_HeroList);
            //    UI_Setting.GetInstance().isShowHeroBtn = false;
            //}
        }
        else
        {
            Debug.Log(string.Format("获取英雄列表错误：{0}", data["desc"].ToString()));
        }
        return true;
    }
    /* void DoHeroListByReqType()
     {
         switch(UI_Setting.GetInstance().heroListreqType)
         {
             case RefHeroListType.HeroBtnOpen:
                 UI_Setting.GetInstance().HeroBtnRefresh();
                 break;
         case RefHeroListType.embattle:
             UI_Setting.GetInstance ().EmbattleRef ();
             default:
                 break;
         }
     }*/

    /// <summary>
    /// 获取英雄信息          4004
    /// </summary>
    /// <param name="packet"></param>
    /// <returns></returns>
    public bool GetHeroInfoResultHandle(CReadPacket packet)
    {



        Dictionary<string, object> data = packet.data;

        int resolt = int.Parse(data["ret"].ToString());

        if (resolt == 0)
        {

            HeroData hd = playerData.GetInstance().GetHeroDataByID(int.Parse(data["heroId"].ToString()));

            hd.isGetData = true;

            hd.SetData(data);
            hd.RefreshAttr();
            playerData.GetInstance().selectHeroDetail = hd;
            Globe.selectHero = FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList[hd.id];

            //if (data.ContainsKey("runes"))
            //{
            //    object runes = data["runes"] as object;
            //    if (null != runes)
            //    {
            //        Dictionary<string, object> runesDic = runes as Dictionary<string, object>;
            //        foreach (string str in runesDic.Keys)
            //        {
            //            hd.runes[int.Parse(str) - 1] = long.Parse(runesDic[str].ToString());
            //        }
            //    }
            //}

            //if (data.ContainsKey("skill"))
            //{
            //    Dictionary<string, object> skillListDic = data["skill"] as Dictionary<string, object>;
            //    foreach (KeyValuePair<string, object> skill in skillListDic)
            //    {
            //        if (hd.skill.ContainsKey(long.Parse(skill.Key)))
            //            hd.skill[long.Parse(skill.Key)] = (int)skill.Value;
            //        else
            //            hd.skill.Add(long.Parse(skill.Key), (int)skill.Value);
            //    }
            //}

            //if (data.ContainsKey("equip"))
            //{
            //    Dictionary<string, object> itemDic = data["equip"] as Dictionary<string, object>;
            //    //Dictionary<long, EquipData> equipID = new Dictionary<long, EquipData>();
            //    Dictionary<int, EquipData> equipSite = new Dictionary<int, EquipData>();
            //    Dictionary<string, object> itemI;

            //    object obj = null;

            //    object id = 0;
            //    object grade = 0;
            //    object lvl = 0;

            //    for (int i = 1; i <= itemDic.Count; i++)
            //    {

            //        itemDic.TryGetValue("" + i, out obj);

            //        itemI = (Dictionary<string, object>)obj;

            //        EquipData ed = new EquipData();

            //        itemI.TryGetValue("id", out id);
            //        itemI.TryGetValue("grade", out grade);
            //        itemI.TryGetValue("lvl", out lvl);

            //        ed.id = int.Parse(id.ToString()) + int.Parse(grade.ToString());
            //        ed.grade = int.Parse(grade.ToString());
            //        ed.level = int.Parse(lvl.ToString());
            //        ed.site = i;

            //        //equipID.Add(ed.id, ed);
            //        equipSite.Add(i, ed);

            //    }

            //    //hd.equipID = equipID;
            //    hd.equipSite.Clear();
            //    hd.equipSite = equipSite;
            //}

            if (Globe.isDetails)
            {
                //Control.ShowGUI(GameLibrary.UI_HeroDetail);
                //Control.HideGUI(GameLibrary.UIHeroList);
                //Control.HideGUI(UIPanleID.UIHeroList);
                //Globe.isDetails = false;
            }

            if (null != UI_HeroDetail.instance && UI_HeroDetail.instance.IsShow())
            {
                UI_HeroDetail.instance.UpdateInfo();
            }
            //if(playerData.GetInstance().isEquipDevelop)
            //{
            //    Control.ShowGUI(GameLibrary.EquipDevelop);
            //    Control.PlayBgmWithUI(GameLibrary.EquipDevelop);
            //    // if(EquipDevelop.GetSingolton())
            //    // EquipDevelop.GetSingolton().RefreshUI(hd);
            //}
            //if (null != EquipPanel.instance && EquipPanel.instance.gameObject.activeInHierarchy)
            //{
            //    EquipPanel.instance.ShowEquip(playerData.GetInstance().GetHeroDataByID(Globe.selectHero.hero_id));
            //}
            return true;

        }
        else
        {
            Debug.Log(string.Format("获取英雄信息错误：{0}", data["desc"].ToString()));
            return false;
        }

    }

    /// <summary>
    /// 指定出战英雄          4006
    /// </summary>
    /// <param name="packet"></param>
    /// <returns></returns>
    public bool AssignFightHeroHandle(CReadPacket packet)
    {

        Debug.Log("Assign fight hero result");

        Dictionary<string, object> data = packet.data;

        int resolt = int.Parse(data["ret"].ToString());

        if (resolt == 0)
        {

        }
        else
        {
            Debug.Log(string.Format("指定出战英雄错误：{0}", data["desc"].ToString()));
        }

        return true;

    }

    /// <summary>
    /// 魂石置换英雄          4008
    /// </summary>
    /// <param name="packet"></param>
    /// <returns></returns>
    public bool SoulStoneChangeHeroResultHandle(CReadPacket packet)
    {

        Debug.Log("SoulStone Change Hero result");

        Dictionary<string, object> data = packet.data;

        int resolt = int.Parse(data["ret"].ToString());

        if (resolt == 0)
        {
            int afc = 0;
            int lvl = 1;
            int star = 1;
            int grade = 1;
            if (data.ContainsKey("afc"))
                afc = Convert.ToInt32(data["afc"]);
            if (data.ContainsKey("level"))
                lvl = Convert.ToInt32(data["level"]);
            if (data.ContainsKey("star"))
                star = Convert.ToInt32(data["star"]);
            if (data.ContainsKey("grade"))
                grade = Convert.ToInt32(data["grade"]);
            UIHeroList.instance.OpenGetHeroPanel(afc, lvl, star, grade);
        }
        else
        {
            Debug.Log(string.Format("魂石置换英雄错误：{0}", data["desc"].ToString()));
        }

        return true;

    }

    /// <summary>
    /// 英雄升星              4010
    /// </summary>
    /// <param name="packet"></param>
    /// <returns></returns>
    public bool UpgradeHeroStarResultHandle(CReadPacket packet)
    {

        Debug.Log("Upgrade Hero Star result");

        Dictionary<string, object> data = packet.data;

        int resolt = int.Parse(data["ret"].ToString());

        if (resolt == 0)
        {
            int star = HeroAndEquipNodeData.HD.star;

            HeroAndEquipNodeData.HD.star = star + 1;
            playerData.GetInstance().selectHeroDetail.star = HeroAndEquipNodeData.HD.star;
            //HeroAndEquipNodeData.HD.SetNodeByIdAndGrade(HeroAndEquipNodeData.HD.id, HeroAndEquipNodeData.HD.star);
            //HeroData myHd = playerData.GetInstance().selectHeroDetail;
            //HeroData nextGradeHd = new HeroData(myHd.id, myHd.lvl, myHd.grade + 1, myHd.star);
            //nextGradeHd.equipSite = myHd.equipSite;
            HeroAndEquipNodeData.HD.RefreshAttr();

            //Control.ShowGUI(GameLibrary.UI_HeroDetail);
            //if (Singleton<SceneManage>.Instance.Current == EnumSceneID.UI_MajorCity01)
            //{
            //    UITooltips.Instance.SetBlackerBottom_Text("英雄升星成功");
            //    Breakthrough.instance.PlayerShengXingEffect(HeroAndEquipNodeData.HD.star-1, HeroAndEquipNodeData.HD.star);//英雄升星成功播放晋级特效
            //}
        }
        else
        {
            Debug.Log(string.Format("英雄升星错误：{0}", data["desc"].ToString()));
        }

        return true;

    }

    /// <summary>
    /// 升级英雄装备          4012
    /// </summary>
    /// <param name="packet"></param>
    /// <returns></returns>
    public bool GetUpGradeHEResultHandle(CReadPacket packet)
    {

        Debug.Log("Get UpGrade Hero Equip result");

        Dictionary<string, object> data = packet.data;

        int resolt = int.Parse(data["ret"].ToString());

        if (resolt == 0)
        {

            //if (data.ContainsKey("vt") && data.ContainsKey("vv"))
            //{
            //   int vt = int.Parse(data["vt"].ToString());
            //   UInt32 vv = UInt32.Parse(data["vv"].ToString());
            Dictionary<string, object> item = data["item"] as Dictionary<string, object>;
            HeroData hd = playerData.GetInstance().GetHeroDataByID(Globe.selectHero.hero_id);
            foreach (string it in item.Keys)
            {
                hd.equipSite[int.Parse(it)].level = int.Parse(item[it].ToString());
                // hd.equipSite[int.Parse(it)].level += EquipDevelop.GetSingolton().equipStrenthDlg.equipsendLvUp;

            }
            //hd.equipSite[EquipDevelop.GetSingolton().index + 1].level +=
            //       EquipDevelop.GetSingolton().equipStrenthDlg.equipsendLvUp;
            // EquipDevelop.GetSingolton().equipStrenthDlg.InitData(hd.equipSite[EquipDevelop.GetSingolton().index + 1]);
            hd.RefreshAttr();
            //EquipDevelop.GetSingolton().RefreshUI(hd);
            //if (EquipDevelop.GetSingolton() != null && EquipDevelop.GetSingolton() && EquipDevelop.GetSingolton().equipStrenthDlg.gameObject.activeSelf)
            //{
            //    hd.equipSite[EquipDevelop.GetSingolton().index + 1].level +=
            //    EquipDevelop.GetSingolton().equipStrenthDlg.GetLvByCost((UInt32)(playerData.GetInstance().baginfo.gold- vv));
            //    // EquipDevelop.GetSingolton().equipStrenthDlg.InitData(hd.equipSite[EquipDevelop.GetSingolton().index + 1]);
            //    EquipDevelop.GetSingolton().RefreshUI(hd);

            //}
            //playerData.GetInstance().RoleMoneyHadler((MoneyType)vt, vv);
            // }

            //  Control.ShowGUI(GameLibrary.UI_HeroDetail);
            //EquipStrengthePanel.instance.OpenEquipStrengEff();//显示装备强化特效
            //EquipDevelop.GetSingolton().OpenEquipEff(item);


            EquipDevelopManager.Single().item = item;
            EquipDevelopManager.Single().hd = hd;
        }
        else
        {
            Debug.Log(string.Format("装备升级错误：{0}", data["desc"].ToString()));
        }

        return true;

    }

    /// <summary>
    /// 英雄装备进化          4014
    /// </summary>
    /// <param name="packet"></param>
    /// <returns></returns>
    public bool GetHeroEMonResultHandle(CReadPacket packet)
    {

        Debug.Log("Get Hero Equip Mon result");

        Dictionary<string, object> data = packet.data;

        int resolt = int.Parse(data["ret"].ToString());

        if (resolt == 0)
        {
            //  HeroData hd = playerData.GetInstance().GetHeroDataByID(Globe.selectHero.hero_id);
            //EquipData ed;
            //hd.equipSite.TryGetValue(HeroAndEquipNodeData.site, out ed);
            //hd.ChangeEquip(HeroAndEquipNodeData.site, ed.id + 1, ed.grade + 1);
            //HeroAndEquipNodeData.equipUp.Clear();
            //HeroAndEquipNodeData.TanNUm = 4;
            //Control.ShowGUI(GameLibrary.UI_HeroDetail);

            Dictionary<string, object> item = data["item"] as Dictionary<string, object>;
            HeroData hd = playerData.GetInstance().selectHeroDetail;
            foreach (string it in item.Keys)
            {
                hd.equipSite[int.Parse(it)].grade = int.Parse(item[it].ToString());
                hd.equipSite[int.Parse(it)].id = hd.equipSite[int.Parse(it)].id + 1;
                // hd.equipSite[int.Parse(it)].level += EquipDevelop.GetSingolton().equipStrenthDlg.equipsendLvUp;

            }

            //HeroData hd = playerData.GetInstance().selectHeroDetail;

            //hd.equipSite[EquipDevelop.GetSingolton().index + 1].grade += 1;
            //hd.equipSite[EquipDevelop.GetSingolton().index + 1].id += 1;
            hd.RefreshAttr();
            //EquipDevelop.GetSingolton().RefreshUI(hd);
            //EquipIntensifyPanel.instance.OpenEquipIntensEff();//显示进化特效
            //EquipDevelop.GetSingolton().OpenEquipjinhuaEff(item);

            EquipDevelopManager.Single().item = item;
            EquipDevelopManager.Single().hd = hd;
            return true;
        }
        else
        {
            Control.ShowGUI(UIPanleID.UITooltips, EnumOpenUIType.DefaultUIOrSecond, false, "材料不足.");
            return false;
        }


    }

    /// <summary>
    /// 英雄装备材料合成      4016
    /// </summary>
    /// <param name="packet"></param>
    /// <returns></returns>
    public bool GetHeroEComResultHandle(CReadPacket packet)
    {

        Debug.Log("Get Hero Equip Com result");

        Dictionary<string, object> data = packet.data;

        int resolt = int.Parse(data["ret"].ToString());

        if (resolt == 0)
        {
            if (EquipDevelop.GetSingolton() != null && EquipDevelop.GetSingolton().equipIntensifyDlg.equipCompoundPanel.gameObject.activeSelf)
                EquipDevelop.GetSingolton().equipIntensifyDlg.equipCompoundPanel.InitData(EquipDevelop.GetSingolton().equipIntensifyDlg.equipCompoundPanel.selectMaterial);
            //刷新背包数据
            //if (null != EquipInfoPanel.instance)
            //{
            //    EquipInfoPanel.instance.HeroEComHandler();
            //}
            //EquipCompoundPanel.instance.OpenMaterialCompountEff();//播放合成特效
        }
        else
        {
            Debug.Log(string.Format("装备合成错误：{0}", data["desc"].ToString()));
        }

        return true;

    }

    /// <summary>
    /// 英雄进阶              4018
    /// </summary>
    /// <param name="packet"></param>
    /// <returns></returns>
    public bool GetHeroAdvancedResultHandle(CReadPacket packet)
    {

        Debug.Log("Get Hero Advanced result");

        Dictionary<string, object> data = packet.data;

        int resolt = int.Parse(data["ret"].ToString());

        if (resolt == 0)
        {
            //更改英雄grade+1
            HeroAndEquipNodeData.HD.grade = HeroAndEquipNodeData.HD.grade + 1;
            HeroAndEquipNodeData.HD.RefreshAttr();
            //EquipPanel.instance.OnAdvancedSuccess();
            //UITooltips.Instance.SetBlackerBottom_Text("英雄进阶成功");
            //EquipPanel.instance.PlayjinjieEffect();   
            return true;
        }
        else
        {
            Control.ShowGUI(UIPanleID.UITooltips, EnumOpenUIType.DefaultUIOrSecond, false, data["desc"].ToString());
            Debug.Log(string.Format("英雄进阶错误：{0}", data["desc"].ToString()));
            return false;
        }
    }

    /// <summary>
    /// 装备符文              4028
    /// </summary>
    /// <param name="packet"></param>
    /// <returns></returns>
    public bool GetEquipRunesResultHandle(CReadPacket packet)
    {

        Debug.Log("Get Equip Runes result");

        Dictionary<string, object> data = packet.data;

        int resolt = int.Parse(data["ret"].ToString());

        if (resolt == 0)
        {

            Debug.Log("装备符文成功");

        }
        else
        {
            Debug.Log(string.Format("装备符文错误：{0}", data["desc"].ToString()));
        }

        return true;

    }

    /// <summary>
    /// 合成符文              4030
    /// </summary>
    /// <param name="packet"></param>
    /// <returns></returns>
    public bool GetRunesCompoundResultHandle(CReadPacket packet)
    {

        Debug.Log("Get Runes Compund result");

        Dictionary<string, object> data = packet.data;

        int resolt = int.Parse(data["ret"].ToString());

        if (resolt == 0)
        {

            Debug.Log("合成符文成功");

            if (null != UI_HeroDetail.instance)
                UI_HeroDetail.instance.RunesHandler();

        }
        else
        {
            Debug.Log(string.Format("合成符文错误：{0}", data["desc"].ToString()));
        }

        return true;

    }

    /// <summary>
    /// 药水升级              4032
    /// </summary>
    /// <param name="packet"></param>
    /// <returns></returns>
    public bool GetDrugUpgradeResultHandle(CReadPacket packet)
    {

        Debug.Log("Get Dryg Upgrade result");

        Dictionary<string, object> data = packet.data;

        int resolt = int.Parse(data["ret"].ToString());

        if (resolt == 0)
        {
            //升级成功
            for (int i = 0; i < playerData.GetInstance().herodataList.Count; i++)
            {
                Debug.Log(playerData.GetInstance().herodataList[i].id);
                if (playerData.GetInstance().herodataList[i].id == int.Parse(data["heroId"].ToString()))
                {
                    playerData.GetInstance().herodataList[i].lvl = int.Parse(data["level"].ToString());
                    playerData.GetInstance().herodataList[i].exps = int.Parse(data["exps"].ToString());
                    playerData.GetInstance().herodataList[i].maxExps = FSDataNodeTable<HeroUpGradeNode>.GetSingleton().DataNodeList[int.Parse(data["level"].ToString())].exp;
                }
            }
            playerData.GetInstance().selfData.expPool = int.Parse(data["expsPool"].ToString());
            HeroAndEquipNodeData.TanNUm = 1;
            //Control.ShowGUI(GameLibrary.UI_HeroDetail);
            //if (Singleton<SceneManage>.Instance.Current == EnumSceneID.UI_MajorCity01)
            //{
            //    UITooltips.Instance.SetBlackerBottom_Text("英雄升级成功");
            //    UIHeroNameArea.Instance.PlayShengjiEffect();//英雄升级成功播放升级特效
            //    UIRole.Instance.SetMainHeroLevel();
            //}
            return true;
        }
        else
        {
            //升级失败
            Control.ShowGUI(UIPanleID.UITooltips, EnumOpenUIType.DefaultUIOrSecond, false, "经验不足");
            return false;
        }



    }

    /// <summary>
    /// 获取符文列表          4026
    /// </summary>
    /// <param name="packet"></param>
    /// <returns></returns>
    public bool GetRunesListResultHandle(CReadPacket packet)
    {

        //Debug.Log("Get Runes List result");

        //Dictionary<string, object> data = packet.data;

        //int resolt = int.Parse(data["ret"].ToString());

        //if (resolt == 0)
        //{

        //    object[] item = data["item"] as object[];

        //    if (null != item)
        //    {

        //        for (int i = 0; i < item.Length; i++)
        //        {
        //            long id = long.Parse((item[i] as Dictionary<string, object>)["id"].ToString());
        //            int count = int.Parse((item[i] as Dictionary<string, object>)["at"].ToString());
        //        }

        //    }

        //}
        //else
        //{
        //    Debug.Log(string.Format("获取符文列表错误：{0}", data["desc"].ToString()));
        //}

        return true;

    }


    /// <summary>
    /// 获取英雄装备列表      4036
    /// </summary>
    /// <param name="packet"></param>
    /// <returns></returns>
    public bool GetHeroEquipListResultHandle(CReadPacket packet)
    {

        Debug.Log("Get Hero Equip List result");

        return true;

        //Dictionary<string, object> data = packet.data;

        //int resolt = int.Parse(data["ret"].ToString());

        //if (resolt == 0)
        //{

        //HeroData hd = playerData.GetInstance().GetHeroDataByID(int.Parse(data["heroId"].ToString()));

        //Dictionary<string, object> itemDic = data["item"] as Dictionary<string, object>;

        ////List<EquipData> edList = new List<EquipData>();

        //Dictionary<long, EquipData> edDic = new Dictionary<long, EquipData>();

        //Dictionary<int, EquipData> edDicI = new Dictionary<int, EquipData>();

        //Dictionary<string, object> itemI;

        //Dictionary<long, int> equipLI = new Dictionary<long, int>();

        ////Dictionary<EquipData, int> equipDDD = new Dictionary<EquipData, int>();

        //object obj = null;

        //object id = 0;
        //object grade = 0;
        //object lvl = 0;

        //for (int i = 1; i <= itemDic.Count; i++)
        //{

        //    itemDic.TryGetValue("" + i, out obj);

        //    itemI = (Dictionary<string, object>)obj;

        //    EquipData ed = new EquipData();

        //    itemI.TryGetValue("id", out id);
        //    itemI.TryGetValue("grade", out grade);
        //    itemI.TryGetValue("lvl", out lvl);

        //    ed.id = int.Parse(id.ToString());
        //    ed.grade = int.Parse(grade.ToString());
        //    ed.level = int.Parse(lvl.ToString());
        //    ed.site = i;

        //    //edList.Add(ed);
        //    edDic.Add(ed.id, ed);
        //    edDicI.Add(i, ed);
        //    equipLI.Add(ed.id, i);
        //    //equipDDD.Add(ed, i);

        //}

        //hd.equipList = edList;
        //hd.equipID = edDic;
        //hd.equipSite.Clear();
        //hd.equipSite = edDicI;
        //hd.eqioipLDD = equipDDD;

        //EquipPanel.instance.ShowEquip(Globe.selectHero);

        //if (EquipInfoPanel.instance && EquipInfoPanel.instance.gameObject.activeInHierarchy)
        //{
        //    EquipInfoPanel.instance.OnUpdateEquipInfo();
        //}

        //}
        //else
        //{
        //    Debug.Log(string.Format("获取英雄装备错误：{0}", data["desc"].ToString()));
        //}

        //return true;

    }


    /// <summary>
    /// 英雄技能升级          4022
    /// </summary>
    /// <param name="packet"></param>
    /// <returns></returns>
    public bool HeroSkillUpGradeResultHandle(CReadPacket packet)
    {
        Debug.Log("SkillList result");
        Dictionary<string, object> data = packet.data;

        int resolt = int.Parse(data["ret"].ToString());
        if (resolt == 0)
        {
            Debug.Log("升级成功");
            UInt32 playerMoney = UInt32.Parse(data["vv"].ToString());
            int moneyType = int.Parse(data["vt"].ToString());

            playerData.GetInstance().RoleMoneyHadler((MoneyType)moneyType, playerMoney);

        }
        else
        {
            Debug.Log(string.Format("技能升级失败：{0}", data["desc"].ToString()));
        }
        return true;
    }


}