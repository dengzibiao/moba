using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tianyu;

public class EquipPanel : MonoBehaviour
{
   
    public  ItemEquip[] ItemEquiplist ;//所选英雄的装备
    public UISprite[] hahaha ;
    public static EquipPanel instance;

    UIButton advancedBtn;//
    UIButton upgradeAllBtn;

    List<ItemEquip> equip = new List<ItemEquip>();              //6个装备位
    Dictionary<long, HeroAttrNode> objDic;

    // 1武器 2头盔 3胸甲 4腿甲 5护手 6鞋子
    Dictionary<int, ItemEquip> itemEquipDicI = new Dictionary<int, ItemEquip>();

    object[] obj;       //所有的装备

    Dictionary<long, HeroAttrNode> herodataDic = new Dictionary<long, HeroAttrNode>();

    List<HeroAttrNode> equipAllList = new List<HeroAttrNode>();

    List<ItemVO> itemList = new List<ItemVO>();         //玩家当前的装备集合
    List<ItemVO> currentIL = new List<ItemVO>();        //玩家一键升级前的装备集合
    int currentMoney = 0;                               //玩家当前的金币

    int AllMoney = 0;

    int siteUp;

    HeroData hd;

    EquipUpgradeNode equipUpNode;

    List<EquipData> upED = new List<EquipData>();

    Dictionary<string, int> equipUp;

    Transform jinjieEffect;
    public ItemNodeState item { get; set; }
    public int site { get; set; }

    #region
    //Dictionary<int, EquipData> edDic = new Dictionary<int, EquipData>();

    //ItemEquip item;

    //List<BackpackEquipVO> list = new List<BackpackEquipVO>();

    //Dictionary<int, BackpackEquipVO> equipDic = new Dictionary<int, BackpackEquipVO>();

    //Dictionary<int, BackpackEquipVO> allDic = new Dictionary<int, BackpackEquipVO>();

    //List<HeroDataVO> equipList = new List<HeroDataVO>();

    //string[] equipS;                                    //装备ID
    //int site;                                           //装备位
    #endregion

    void Awake()
    {
        instance = this;
        for (int i = 1; i <= 6; i++)
        {
            equip.Add(transform.FindChild("Equip" + i).GetComponent<ItemEquip>());
            itemEquipDicI.Add(i, transform.FindChild("Equip" + i).GetComponent<ItemEquip>());    //取装备位置
        }

        obj = new object[FSDataNodeTable<HeroAttrNode>.GetSingleton().DataNodeList.Count];
        objDic = FSDataNodeTable<HeroAttrNode>.GetSingleton().DataNodeList;

        int index = 0;
        foreach (object hn in objDic.Values)
        {
            if (index < obj.Length)
            {
                obj[index] = hn;
                index++;
            }
        }

        for (int i = 0; i < obj.Length; i++)
        {
            HeroAttrNode herodata = (HeroAttrNode)obj[i];
            if (!herodataDic.ContainsKey(herodata.id))
                herodataDic.Add(herodata.id, herodata);
            equipAllList.Add(herodata);
        }
        jinjieEffect = transform.Find("UI_YXJJ_01");
    }

    void Start()
    {
       
        for (int i = 0; i < ItemEquiplist.Length; i++)
        {
            if (ItemEquiplist[i] == null)
                ItemEquiplist[i] = transform.FindChild("Equip" + (i + 1).ToString()).GetComponent<ItemEquip>();
        }

        advancedBtn = UnityUtil.FindCtrl<UIButton>(gameObject, "AdvancedBtn");
        upgradeAllBtn = UnityUtil.FindCtrl<UIButton>(gameObject, "UpgradeAllBtn");

        EventDelegate.Set(advancedBtn.onClick, OnAdvancedBtnClick);
        EventDelegate.Set(upgradeAllBtn.onClick, OnUpgradeAllBtnClick);
        if(EquipOperation.Instance()!=null)
        EquipOperation.Instance().RefreshUI(1, ItemEquiplist[0]);
        //EquipOperation.Instance().RefreshUI(1, ItemEquiplist[0]); 
        //for (int i = 0; i < ItemEquiplist.Length; i++)
        //{
        //    //Debug.Log(ItemEquiplist[i].itemVO.props_id);
        //}
        if (jinjieEffect != null)
        {
            if (jinjieEffect.gameObject.activeSelf)
            {
                jinjieEffect.gameObject.SetActive(false);
            }
        }
    }
    /// <summary>
    /// 播放英雄进阶特效
    /// </summary>
    public void PlayjinjieEffect()
    {
        jinjieEffect.gameObject.SetActive(false);
        jinjieEffect.gameObject.SetActive(true);
    }
    /// <summary>
    /// 进阶按钮
    /// </summary>
    private void OnAdvancedBtnClick()
    {
        EquipData ed;
        int index = 0;

        hd = playerData.GetInstance().GetHeroDataByID(Globe.selectHero.hero_id);

        for (int i = 1; i <= hd.equipSite.Count; i++)
        {

            hd.equipSite.TryGetValue(i, out ed);

            if (ed.grade >= 2 && hd.grade == 1)
            {
                index += ed.grade;
            }
            else if (ed.grade >= 4 && hd.grade == 2)
            {
                index += ed.grade;
            }
            else if (ed.grade >= 7 && hd.grade == 3)
            {
                index += ed.grade;
            }
            else if (ed.grade >= 11 && hd.grade == 4)
            {
                index += ed.grade;
            }
            else if (ed.grade >= 16 && hd.grade == 5)
            {
                index += ed.grade;
            }

        }

        if (AdvancedRules(index))
        {
            HeroAndEquipNodeData.HD = hd;
            int level = 0;
            foreach (var item in FSDataNodeTable<HeroAttrNode>.GetSingleton().DataNodeList.Values)
            {
                if (item.grade == hd.grade && item.name == hd.node.name)
                {
                    level = item.break_lv;
                }

            }
			if (level <= playerData.GetInstance().selectHeroDetail.lvl)
            {
                ClientSendDataMgr.GetSingle().GetHeroSend().SendHeroAdvanced(Globe.selectHero.hero_id);
            }
            else
            {
                Control.ShowGUI(UIPanleID.UITooltips, EnumOpenUIType.DefaultUIOrSecond, false, "进阶下一级需要达到" + level);
            }
         
        }
        else
        {
            Control.ShowGUI(UIPanleID.UITooltips, EnumOpenUIType.DefaultUIOrSecond, false, "请提升装备品质");
            //PromptPanel.instance.ShowPrompt();
        }

    }

    /// <summary>
    /// 升级全部按钮
    /// </summary>
    private void OnUpgradeAllBtnClick()
    {
        //获取玩家当前金币
        currentMoney = (int)playerData.GetInstance().baginfo.gold;

        AllMoney = 0;

        equipUp = new Dictionary<string, int>();

        currentIL = itemList;

        hd = playerData.GetInstance().GetHeroDataByID(Globe.selectHero.hero_id);

        upED.Clear();
        List<int> equiSite = new List<int>(hd.equipSite.Keys);
        for (int i = 1; i <= equiSite.Count; i++)
        {
            upED.Add(hd.equipSite[i]);
        }

        int count = 0;

        EquipData eds;

        for (int i = 1; i <= 6; i++)
        {
            hd.equipSite.TryGetValue(i, out eds);

            if (null != eds)
                if (eds.level >= hd.lvl)
                    count++;
        }

        if (count >= 6)
        {
            PromptPanel.instance.ShowPrompt("请提升英雄等级");
            return;
        }
        EquipUpgrade();

        ClientSendDataMgr.GetSingle().GetHeroSend().SendUpGradeHE(hd.id, equipUp, AllMoney, C2SMessageType.Active);

    }

    /// <summary>
    /// 装备升级
    /// </summary>
    void EquipUpgrade()
    {

        //判断装备等级是否到达英雄等级，大于从装备列表里清除
        for (int i = upED.Count - 1; i >= 0; i--)
        {
            if (upED[i].level >= hd.lvl)
            {
                upED.Remove(upED[i]);
            }
        }

        //所有装备升到满级时退出
        if (upED.Count <= 0)
        {
            return;
        }

        //计算6件装备升一级所需的金币
        for (int i = 0; i < upED.Count; i++)
        {
            //获取装备当前的等级所需消耗金币
            equipUpNode = FSDataNodeTable<EquipUpgradeNode>.GetSingleton().FindDataByType(upED[i].level);
            AllMoney += (int)equipUpNode.consume;
        }

        if (AllMoney > currentMoney)
        {
            print("金币不足，弹出点金手");
            Control.ShowGUI(UIPanleID.UIGoldHand, EnumOpenUIType.DefaultUIOrSecond);
            return;
        }

        //装备升1级
        for (int i = 0; i < upED.Count; i++)
        {
            //upED[i].level++;

            foreach (int key in hd.equipSite.Keys)
            {
                if (hd.equipSite[key] == upED[i])
                {
                    siteUp = key;
                    break;
                }
            }

            hd.equipSite[siteUp].level++;

            if (equipUp.ContainsKey(siteUp.ToString()))
                equipUp[siteUp.ToString()]++;
            else
                equipUp.Add(siteUp.ToString(), 1);
        }

        //countEQ++;

        //if (countEQ >= 100)
        //{
        //    return;
        //}

        EquipUpgrade();

    }

    public void OnSuccess()
    {
        ShowEquip(hd);
    }

    public void OnAdvancedSuccess()
    {
        //hd.grade++;
        hd = HeroAndEquipNodeData.HD;
        UI_HeroDetail.hd = hd;
        ShowEquip(hd);
        //UI_HeroDetail.instance.ChangeNameColor();
    }

    /// <summary>
    /// 显示装备
    /// </summary>
    public void ShowEquip(HeroData HD)
    {

        if (HD.equipSite.Count != 0)
        {
            EquipData ed;

            ItemEquip ie;

            ItemNodeState item;
            for (int i = 1; i <= itemEquipDicI.Count; i++)
            {
                HD.equipSite.TryGetValue(i, out ed);
                itemEquipDicI.TryGetValue(i, out ie);
                if (ed != null)
                {
                    if (GameLibrary.Instance().ItemStateList.TryGetValue(ed.id, out item))
                    {
                        ie.Init(i, item);
                    }
                }
            }
            if (UI_HeroDetail.equipItemState == 0)
            {
                if (null != ItemEquiplist[0])
                {
                    if (EquipOperation.Instance()!=null)
                    EquipOperation.Instance().RefreshUI(1, ItemEquiplist[0]);
                }
            }
            if (UI_HeroDetail.equipItemState == 3)
            {
                if (null != ItemEquiplist[0])
                {
                    if (EquipOperation.Instance() != null)
                        EquipOperation.Instance().RefreshUI(1, ItemEquiplist[0]);
                }
            }
        }
        //英雄进阶成功刷新一下进阶界面信息
        HeroPreview.Instance().Show();
        if (jinjieEffect != null)
        {
            if (jinjieEffect.gameObject.activeSelf)
            {
                jinjieEffect.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 装备按钮
    /// </summary>
    /// <param name="go"></param>
    public void OnEquipClick(int site, ItemEquip item)
    {
        this.site = site;
        this.item = item.itemVO;
        Control.Show(UIPanleID.UIEquipInfoPanel);
        //equip.itemVO = item.itemVO;
        //equip.site = site;

    }

    /// <summary>
    /// 进阶规则
    /// </summary>
    /// <returns></returns>
    bool AdvancedRules(int sumGrade)
    {
        if (sumGrade >= (2 * 6) && hd.grade == 1)
        {
            return true;
        }
        else if (sumGrade >= (4 * 6) && hd.grade == 2)
        {
            return true;
        }
        else if (sumGrade >= (7 * 6) && hd.grade == 3)
        {
            return true;
        }
        else if (sumGrade >= (11 * 6) && hd.grade == 4)
        {
            return true;
        }
        else if (sumGrade >= (16 * 6) && hd.grade == 5)
        {
            return true;
        }
        return false;
        //if (sumGrade == (3 * 6) && hd.grade == 1)
        //{
        //    return true;
        //}
        //else if (sumGrade == (6 * 6) && hd.grade == 2)
        //{
        //    return true;
        //}
        //else if (sumGrade == (10 * 6) && hd.grade == 3)
        //{
        //    return true;
        //}
        //else if (sumGrade == (15 * 6) && hd.grade == 4)
        //{
        //    return true;
        //}
        //else if (sumGrade == (21 * 6) && hd.grade == 5)
        //{
        //    return true;
        //}
        //return false;
    }
    public void sethahaha(int site)
    {
        for (int i = 0; i < hahaha.Length; i++)
        {
            if (site == i)
            {
                hahaha[i].gameObject.SetActive(true);
            }
            else
            {
                hahaha[i].gameObject.SetActive(false);
            }
        }
    }
}