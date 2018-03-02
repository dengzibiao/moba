using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Tianyu;

public class EquipNodeData : GUIBase
{
    public static bool listType = true;
    private static int[] FrontEquipPropertyNum = new int[18];
    private static int[] QueenEquipPropertyNum = new int[18];
    static long upMoney = playerData.GetInstance().baginfo.gold;
    public static Dictionary<string, int> equipUp;
    public static bool LvState = true;
    public static bool MoneyState = true;
    public static int TanNUm = 0;
    public static int site = 1;
    public static long Money;
    public static int lv;
    public static int TabType = 0;
    public static MaterialItsm MI1s;
    public static bool DetailsTabState = true;
    public static HeroData HD;

    public long HeroId;//英雄ID
    public int Herolevel;//英雄等级
    public long exps;//英雄升级后的经验
    public long expsPool;//经验池里的经验

    public static List<MaterialItsm> MI = new List<MaterialItsm>();
    /// <summary>
    /// 单例
    /// </summary>
    private static EquipNodeData mSingleton;
    public static EquipNodeData Instance()
    {
        if (mSingleton == null)
            mSingleton = new EquipNodeData();
        return mSingleton;
    }

    public override UIPanleID GetUIKey()
    {
        return UIPanleID.none;
    }

    protected override void Init()
    {

    }
    protected override void ShowHandler()
    {

    }
    public int GetMoney()
    {
        return Convert.ToInt32(playerData.GetInstance().baginfo.gold - upMoney);
    }
    public int[] GetFrontEquipPropertyNum(ItemEquip Ie)
    {
        foreach (var item in GameLibrary.Instance().ItemStateList.Values)
        {
            if (Ie.itemVO.props_id == item.props_id)
            {
                for (int i = 0; i < FrontEquipPropertyNum.Length; i++)
                {
                    FrontEquipPropertyNum[i] = item.propertylist[i] + item.propertylist[i] * UI_HeroDetail.hd.lvl;
                }
            }
        }
        return FrontEquipPropertyNum;
    }
    public int[] GetQueenEquipPropertyNum(ItemEquip Ie)
    {

        foreach (var item in GameLibrary.Instance().ItemStateList.Values)
        {
            if (Ie.itemVO.props_id + 1 == item.props_id)
            {
                for (int i = 0; i < QueenEquipPropertyNum.Length; i++)
                {
                    QueenEquipPropertyNum[i] = item.propertylist[i];
                }
            }
        }
        return QueenEquipPropertyNum;
    }
    public void GetEquipPropertyNum(ItemEquip[] Ie)
    {

    }
    public Dictionary<string, int> GetUpALLEquipNum()
    {

        LvState = true;
        equipUp = new Dictionary<string, int>();
        for (int i = 0; i < EquipPanel.instance.ItemEquiplist.Length; i++)
        {
            if (GetWhetherMayUp(EquipPanel.instance.ItemEquiplist[i], i + 1))
            {
                equipUp.Add((i + 1).ToString(), 1);
            }
        }
        return equipUp;
    }
    public Dictionary<string, int> GetUpOneEquipNum(int site)
    {
        Money = 0;
        lv = 0;
        bool stateOK = false;
        LvState = true;
        equipUp = new Dictionary<string, int>();
        HeroData hd = playerData.GetInstance().GetHeroDataByID(Globe.selectHero.hero_id);
        EquipData ed;
        hd.equipSite.TryGetValue(site, out ed);
        if (ed != null)
        {
            if (ed.level < UI_HeroDetail.hd.lvl)
            {
                foreach (var item in FSDataNodeTable<EquipUpgradeNode>.GetSingleton().DataNodeList.Values)
                {
                    if (item.id >= ed.level && item.id < UI_HeroDetail.hd.lvl)
                    {
                        Money += item.consume;
                        lv = ed.level;
                        if (upMoney > Money)
                        {
                            stateOK = true;
                            lv = item.id + 1;
                            upMoney = upMoney - Money;
                        }
                        else
                        {
                            MoneyState = false;
                        }
                    }
                }
                if (stateOK == true)
                {
                    equipUp.Add(site.ToString(), lv - ed.level);
                }
            }
            else
            {
                LvState = false;
            }
        }
        return equipUp;
    }
    public bool GetWhetherMayUp(ItemEquip Ie, int site)
    {
        HeroData hd = playerData.GetInstance().GetHeroDataByID(Globe.selectHero.hero_id);
        EquipData ed;
        hd.equipSite.TryGetValue(site, out ed);
        if (ed.level < UI_HeroDetail.hd.lvl)
        {
            if (upMoney > FSDataNodeTable<EquipUpgradeNode>.GetSingleton().DataNodeList[ed.level].consume)
            {
                upMoney = upMoney - FSDataNodeTable<EquipUpgradeNode>.GetSingleton().DataNodeList[ed.level].consume;
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            LvState = false;
            return false;
        }

    }
    public int GetHeroLV(HeroData HD, long exp)
    {
        int lv = 0;
        int exps = 0;
        if (HD.lvl < playerData.GetInstance().selfData.level)
        {
            foreach (var item in FSDataNodeTable<HeroUpGradeNode>.GetSingleton().DataNodeList.Values)
            {
                if (item.id >= HD.lvl && item.id < playerData.GetInstance().selfData.level + 1)
                {
                    exps += item.exp;
                    if (exp > exps)
                    {
                        lv = item.id;
                    }
                }
            }
        }
        return lv;
    }
    public long GetHeroExpMax(int lv)
    {
        return FSDataNodeTable<HeroUpGradeNode>.GetSingleton().DataNodeList[lv].exp;
    }
}