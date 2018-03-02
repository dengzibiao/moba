using UnityEngine;
using System.Collections.Generic;
using Tianyu;
using System.Text;
using System;

/// <summary>
/// 装备升级面板
/// </summary>
public class EquipInfoPanel : GUIBase
{

    #region 字段

    public static EquipInfoPanel instance;

    public GUISingleSprite border;          //装备边框
    public GUISingleSprite icon;            //装备图标
    public GUISingleLabel levelL;           //装备等级
    public GUISingleLabel powerL;           //战斗力
    public GUISingleLabel addPowerL;        //增加战斗力
    public GUISingleLabel hitL;             //命中
    public GUISingleLabel addHitL;          //增加命中
    public GUISingleButton maskBtn;         //遮罩按钮
    public GUISingleLabel prompt;           //提示消息
    public GUISingleMultList eMatMultList;  //上方材料列表
    //public GUISingleCheckBoxGroup detailsTab;   //侧标签

    public GUISingleSprite matBorder;       //材料边框
    public GUISingleSprite matIcon;         //材料Icon
    public GUISingleLabel matCount;         //材料个数

    UILabel allUpgradeL;                    //一键升级
    UILabel allUpGoldL;                     //一键升级金币
    UILabel UpgradeL;                       //升级
    UILabel UpGoldL;                        //升级金币
    GUISingleButton allUpgradeBtn;          //一键升级按钮
    GUISingleButton upgradeBtn;             //升级按钮

    GUISingleMultList matMultList;          //材料
    GUISingleButton evolutionBtn;           //进化/合成按钮
    UILabel maxLabel;

    GameObject upgradeObj;                  //升级面板
    GameObject evolutionObj;                //进化面板

    int site = 0;                    //装备类型

    List<UISprite> starList = new List<UISprite>();

    ItemEquip item;

    public ItemNodeState itemNode;           //需显示的装备信息

    UInt32 currentMoney = 0;           //当前等级

    public int matCountM = 0;

    HeroData hd;

    EquipData ed;

    EquipUpgradeNode equipUpNode;

    public Dictionary<long, int> needMatDic = new Dictionary<long, int>();    //需要装备

    public List<ItemMatList> itemmatl = new List<ItemMatList>();       //材料信息

    EquipUpgradeNode equipUpNodeL;  //装备升级信息

    public ItemNodeState lastItem;         //记录上次item

    int upGradelvl = 0;             //所升级数

    int currentlvl = 0;             //当前等级

    int needGolds = 0;              //升级需要金币

    int needGold = 0;

    GameObject go;

    ItemNodeState itemMat;

    object[] obj;
    List<ItemNodeState> itemlist = new List<ItemNodeState>();

    int index = 0;

    Dictionary<ItemNodeState, int> itemDic = new Dictionary<ItemNodeState, int>();

    int ind = 0;

    ItemNodeState itemNeed;

    bool isCom = false;

    public bool isNood { get; set; }

    #endregion

    public override UIPanleID GetUIKey()
    {
        return UIPanleID.EquipInfoPanel;
    }

    protected override void Init()
    {
        base.Init();

        instance = this;

        eMatMultList.gameObject.SetActive(false);

        upgradeObj = transform.Find("Upgrade").gameObject;
        evolutionObj = transform.Find("Evolution").gameObject;

        allUpgradeL = UnityUtil.FindCtrl<UILabel>(upgradeObj, "AllUpgradeL");
        allUpGoldL = UnityUtil.FindCtrl<UILabel>(upgradeObj, "AllUpGoldL");
        UpgradeL = UnityUtil.FindCtrl<UILabel>(upgradeObj, "UpgradeL");
        UpGoldL = UnityUtil.FindCtrl<UILabel>(upgradeObj, "UpGoldL");
        allUpgradeBtn = UnityUtil.FindCtrl<GUISingleButton>(upgradeObj, "AllUpgradeBtn");
        upgradeBtn = UnityUtil.FindCtrl<GUISingleButton>(upgradeObj, "UpgradeBtn");

        go = evolutionObj.transform.Find("GameObject").gameObject;
        matMultList = evolutionObj.transform.Find("GameObject/MatMultList").GetComponent<GUISingleMultList>();
        evolutionBtn = evolutionObj.transform.Find("GameObject/EvolutionBtn").GetComponent<GUISingleButton>();
        maxLabel = evolutionObj.transform.Find("Label").GetComponent<UILabel>();

        for (int i = 1; i < 6; i++)
        {
            starList.Add(UnityUtil.FindCtrl<UISprite>(icon.gameObject, "Star" + i));
        }

        allUpgradeBtn.onClick = OnAllUpgradeBtnClick;
        upgradeBtn.onClick = OnUpgradeBtnClick;
        maskBtn.onClick = OnMaskBtnClick;

        evolutionBtn.onClick = OnEvolutionBtnClick;

        //detailsTab.onClick = OnDetailsTabClick;

        UIEventListener.Get(icon.gameObject).onClick = OnIconClick;

    }

    protected override void ShowHandler()
    {

        //detailsTab.DefauleIndex = 0;

        //装备
        border.gameObject.SetActive(true);
        icon.gameObject.SetActive(true);
        levelL.gameObject.SetActive(true);
        powerL.gameObject.SetActive(true);
        addPowerL.gameObject.SetActive(true);
        hitL.gameObject.SetActive(true);
        addHitL.gameObject.SetActive(true);

        //材料
        matBorder.GetComponent<UISprite>().enabled = false;
        matIcon.GetComponent<UISprite>().enabled = false;
        matCount.GetComponent<UILabel>().enabled = false;

        OnUpdateEquipInfo();

    }

    /// <summary>
    /// 升级/进化页签
    /// </summary>
    /// <param name="index"></param>
    /// <param name="boo"></param>
    private void OnDetailsTabClick(int index, bool boo)
    {
        if (boo == false) return;

        switch (index)
        {
            case 0:
                upgradeObj.gameObject.SetActive(true);
                evolutionObj.gameObject.SetActive(false);
                prompt.text = "所需金币";
                icon.GetComponent<BoxCollider>().enabled = false;
                eMatMultList.gameObject.SetActive(false);
                powerL.gameObject.SetActive(true);
                addPowerL.gameObject.SetActive(true);
                hitL.gameObject.SetActive(true);
                addHitL.gameObject.SetActive(true);

                break;
            case 1:
                upgradeObj.gameObject.SetActive(false);
                evolutionObj.gameObject.SetActive(true);
                prompt.text = "所需材料";
                icon.GetComponent<BoxCollider>().enabled = true;
                if (isMaxlvl())
                {
                    ShowMaxLvl(true);
                    UpdateMatInfo();
                }
                else
                {
                    ShowMaxLvl(false);
                }

                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 点击头像
    /// </summary>
    /// <param name="go"></param>
    private void OnIconClick(GameObject go)
    {
        eMatMultList.gameObject.SetActive(false);
        powerL.gameObject.SetActive(true);
        addPowerL.gameObject.SetActive(true);
        hitL.gameObject.SetActive(true);
        addHitL.gameObject.SetActive(true);
        evolutionBtn.text = "进化";

        UpdateMatInfo();

        index = 0;
        itemlist.Clear();
        itemDic.Clear();

        isNood = false;

    }

    /// <summary>
    /// 更新装备信息
    /// </summary>
    /// <param name="vo"></param>
    public void OnUpdateEquipInfo()
    {

        itemNode = EquipPanel.instance.item;

        hd = playerData.GetInstance().GetHeroDataByID(Globe.selectHero.hero_id);

        hd.equipSite.TryGetValue(site, out ed);

        if (null == ed) return;

        icon.spriteName = itemNode.icon_name;
        levelL.text = "Lv." + ed.level;

        //ItemVO item = VOManager.Instance().GetCSV<ItemCSV>("Item").GetVO(itemNode.props_id);
        //ItemNodeState item;
        //GameLibrary.Instance().ItemStateList.TryGetValue(itemNode.props_id - itemNode.grade, out item);

        long initID = itemNode.props_id - itemNode.grade;

        //初始装备值
        //ItemVO initItem = VOManager.Instance().GetCSV<ItemCSV>("Item").GetVO(initID);
        ItemNodeState initItem;
        GameLibrary.Instance().ItemStateList.TryGetValue(initID, out initItem);

        powerL.text = "力量：+" + itemNode.power;

        //初始值+单阶增加生命*装备等级
        addPowerL.text = "+" + (initItem.power + itemNode.power * ed.level);

        hitL.text = "命中：+" + itemNode.hit_ratio;

        //初始值+单阶增加生命*装备等级
        addHitL.text = "+" + (initItem.hit_ratio + itemNode.hit_ratio * ed.level);

        equipUpNode = FSDataNodeTable<EquipUpgradeNode>.GetSingleton().FindDataByType(ed.level + 1);

        needGold = 0;

        if (ed.level != hd.lvl)
        {
            for (int i = ed.level + 1; i <= hd.lvl; i++)
            {
                needGold += VOManager.Instance().GetCSV<EquipUpgradeCSV>("EquipUpgrade").GetVO(i).consume;
            }
        }

        allUpgradeL.text = "升级至英雄等级";
        allUpGoldL.text = "" + needGold;

        UpgradeL.text = "升级至" + (ed.level + 1) + "级";
        UpGoldL.text = "" + equipUpNode.consume;  

        needGolds = (int)equipUpNode.consume;

        ShowMaxLvl(true);
        UpdateBorder(ed.grade);

        if (itemmatl.Count > 0)
        {
            for (int i = 0; i < itemmatl.Count; i++)
            {
                itemmatl[i].UpdateCount();
            }
        }

    }

    /// <summary>
    /// 升级按钮
    /// </summary>
    private void OnUpgradeBtnClick()
    {

       long  currentGold = playerData.GetInstance().baginfo.gold;

        if (ed.level >= hd.lvl)
        {
            PromptPanel.instance.ShowPrompt("请提升英雄等级");
            return;
        }

        upGradelvl = 0;
        equipUp = new Dictionary<string, int>();

        if (equipUpNode.consume < currentGold)
        {
            upGradelvl = 1;

            equipUp.Add(site.ToString(), 1);

            ClientSendDataMgr.GetSingle().GetHeroSend().SendUpGradeHE(hd.id, equipUp, (int)equipUpNode.consume, C2SMessageType.Active);
        }
        else
        {
            PromptPanel.instance.ShowPrompt("金币不足");
        }

    }

    Dictionary<string, int> equipUp;

    public void OnSuccess()
    {
        ed.level += upGradelvl;

        OnUpdateEquipInfo();

        EquipPanel.instance.ShowEquip(hd);
    }

    /// <summary>
    /// 一键升级按钮
    /// </summary>
    private void OnAllUpgradeBtnClick()
    {

        if (ed.level >= hd.lvl)
        {
            PromptPanel.instance.ShowPrompt("请提升英雄等级");
            return;
        }

        upGradelvl = 0;
        equipUp = new Dictionary<string, int>();

        needGolds = 0;
        currentlvl = 0;
        currentlvl = ed.level;

        EquipUpgrade();

        ClientSendDataMgr.GetSingle().GetHeroSend().SendUpGradeHE(hd.id, equipUp, needGolds, C2SMessageType.Active);

    }

    /// <summary>
    /// 装备升级
    /// </summary>
    void EquipUpgrade()
    {

        //获取装备升级消耗  参数装备等级
        equipUpNodeL = FSDataNodeTable<EquipUpgradeNode>.GetSingleton().FindDataByType(ed.level);

        currentMoney =(UInt32) playerData.GetInstance().baginfo.gold;

        ed.level++;

        if (equipUp.ContainsKey(site.ToString()))
            equipUp[site.ToString()]++;
        else
            equipUp.Add(site.ToString(), 1);

        needGolds +=(int) equipUpNodeL.consume;

        //currentMoney -= equipUpNodeL.consume;

        if (ed.level >= hd.lvl)
        {
            //Control.ShowGUI(GameLibrary.UIPromptPanel);
            //PromptPanel.instance.prm = "请提升英雄等级";
            return;
        }

        if (equipUpNodeL.consume > currentMoney)
        {
            print("金币不足，点金手面板");
            return;
        }

        EquipUpgrade();

    }

    /// <summary>
    /// 进化/合成按钮
    /// </summary>
    private void OnEvolutionBtnClick()
    {

        isCom = false;

        if (!eMatMultList.gameObject.activeSelf)
        {
            UpdateMatInfo();
        }
        else
        {
            if (null != itemMat.syn_condition)
            {
                MatComposite(itemMat);
            }
        }

        if (itemmatl.Count <= 0)
        {
            isCom = true;
        }
        else
        {
            for (int i = 0; i < itemmatl.Count; i++)
            {
                if (!itemmatl[i].isEnough)
                {
                    isCom = true;
                }
                //else
                //{
                //    isCom = false;
                //}
            }
        }

        if (evolutionBtn.text == "进化")
        {
            if (isCom)
            {
                PromptPanel.instance.ShowPrompt("材料不足");
            }
            else
            {
                if (isMaxlvl())
                {
                    ClientSendDataMgr.GetSingle().GetHeroSend().SendHeroEMon(hd.id, site);
                }
            }
        }
        else if (evolutionBtn.text == "合成")
        {

            if (isCom)
            {
                PromptPanel.instance.ShowPrompt("材料不足");
            }
            else
            {
                ClientSendDataMgr.GetSingle().GetHeroSend().SendHeroECom(itemNeed.props_id, 1);
            }

        }

    }

    public void HeroEMonHandler()
    {
        ed.grade++;
        GoodsDataOperation.GetInstance().UseGoods(needMatDic);
        OnUpdateEquipInfo();
        EquipPanel.instance.ShowEquip(hd);
    }

    public void HeroEComHandler()
    {
        GoodsDataOperation.GetInstance().UseGoods(needMatDic);
        GoodsDataOperation.GetInstance().AddGoods((int)itemNeed.props_id, 1);

        eMatMultList.Info(obj);

        if (isNood)
        {
            OnIconClick(gameObject);
        }

        OnUpdateEquipInfo();
        EquipPanel.instance.ShowEquip(hd);

    }

    StringBuilder matsb = new StringBuilder();

    /// <summary>
    /// 更新材料信息
    /// </summary>
    public void UpdateMatInfo()
    {

        itemmatl.Clear();

        needMatDic.Clear();

        //if (null == itemNode || itemNode.syn_condition[0, 0] == 0) return;

        List<long> itemList = new List<long>();

        ItemNodeState itemA = null;

        if (!GameLibrary.Instance().ItemStateList.TryGetValue(itemNode.props_id + 1, out itemA))
            return;

        //需要材料
        for (int i = 0; i < (itemA.syn_condition.Length / 2); i++)
        {
            for (int j = 0; j < 2; j++)
            {
                matsb.Append(itemA.syn_condition[i, j] + ",");
            }
        }

        string needMat = matsb.ToString();

        string[] needMats = needMat.Split(',');

        matsb.Length = 0;

        for (int i = 0; i < needMats.Length - 1; i += 2)
        {

            //当字典中不存在时加入字典
            if (!needMatDic.ContainsKey(long.Parse(needMats[i])))
            {
                needMatDic.Add(long.Parse(needMats[i]), int.Parse(needMats[i + 1]));

            }

            if (!itemList.Contains(long.Parse(needMats[i])))
            {
                itemList.Add(long.Parse(needMats[i]));
            }

        }

        object[] objMa = new object[itemList.Count];

        ItemNodeState itemNodeS;
        for (int i = 0; i < itemList.Count; i++)
        {
            GameLibrary.Instance().ItemStateList.TryGetValue(itemList[i], out itemNodeS);
            objMa[i] = itemNodeS;
        }
        
        matMultList.InSize(needMats.Length / 2, 5);
        matMultList.Info(objMa);
        evolutionBtn.text = "进化";

    }

    /// <summary>
    /// 材料合成
    /// </summary>
    void MatComposite(ItemNodeState item)
    {

        itemmatl.Clear();
        needMatDic.Clear();

        itemNeed = item;

        List<long> itemList = new List<long>();

        //需要材料
        for (int i = 0; i < (itemNeed.syn_condition.Length / 2); i++)
        {
            for (int j = 0; j < 2; j++)
            {
                matsb.Append(itemNeed.syn_condition[i, j] + ",");
            }
        }

        string needMat = matsb.ToString();
        string[] needMats = needMat.Split(',');
        matsb.Length = 0;

        for (int i = 0; i < needMats.Length - 1; i += 2)
        {
            //当字典中不存在时加入字典
            if (!needMatDic.ContainsKey(long.Parse(needMats[i])))
            {
                needMatDic.Add(long.Parse(needMats[i]), int.Parse(needMats[i + 1]));

            }

            if (!itemList.Contains(long.Parse(needMats[i])))
            {
                itemList.Add(long.Parse(needMats[i]));
            }

        }

        object[] objMat = new object[itemList.Count];

        ItemNodeState itemNodeS;
        for (int i = 0; i < itemList.Count; i++)
        {
            GameLibrary.Instance().ItemStateList.TryGetValue(itemList[i], out itemNodeS);
            objMat[i] = itemNodeS;
        }

        matMultList.InSize(needMats.Length / 2, 5);
        matMultList.Info(objMat);

        evolutionBtn.text = "合成";


    }
    
    /// <summary>
    /// 装备信息和材料信息切换
    /// </summary>
    /// <param name="isShow"></param>
    public void HideEquipInfo(bool isShow, ItemNodeState item, int count)
    {

        itemMat = item;
        matCountM = count;

        powerL.gameObject.SetActive(false);
        addPowerL.gameObject.SetActive(false);
        hitL.gameObject.SetActive(false);
        addHitL.gameObject.SetActive(false);

        if (!itemlist.Contains(item))
        {
            itemlist.Add(item);
            itemDic.Add(item, index);
            index++;
        }

        if (isShow)
        {
            MatComposite(item);
        }
        else
        {
            itemDic.TryGetValue(item, out ind);

            if (itemlist.Count > 1)
            {
                itemlist.RemoveRange(ind + 1, itemlist.Count);
            }

        }

        //材料
        eMatMultList.gameObject.SetActive(true);

        obj = new object[itemlist.Count];

        eMatMultList.InSize(itemlist.Count, 5);

        for (int i = 0; i < itemlist.Count; i++)
        {
            obj[i] = itemlist[i];
        }

        eMatMultList.Info(obj);

    }

    /// <summary>
    /// 侧标签升级按钮
    /// </summary>
    private void OnUpgradeTBtnClick()
    {
        OnTagBtnClick(true);
        eMatMultList.gameObject.SetActive(false);
    }

    /// <summary>
    /// 侧标签进化按钮
    /// </summary>
    private void OnEvolutionTBtnClick()
    {

        OnTagBtnClick(false);

        border.gameObject.SetActive(true);
        icon.gameObject.SetActive(true);
        levelL.gameObject.SetActive(true);
        powerL.gameObject.SetActive(true);
        addPowerL.gameObject.SetActive(true);
        hitL.gameObject.SetActive(true);
        addHitL.gameObject.SetActive(true);

        //材料

        matBorder.GetComponent<UISprite>().enabled = false;
        matIcon.GetComponent<UISprite>().enabled = false;
        matCount.GetComponent<UILabel>().enabled = false;

    }

    /// <summary>
    /// 升级进化切换
    /// </summary>
    /// <param name="isTag"></param>
    void OnTagBtnClick(bool isTag)
    {
        if (isTag)
        {
            prompt.text = "所需金币";
            upgradeObj.SetActive(true);
            evolutionObj.SetActive(false);
            icon.GetComponent<BoxCollider>().enabled = false;
        }
        else
        {
            prompt.text = "所需材料";
            upgradeObj.SetActive(false);
            evolutionObj.SetActive(true);
            icon.GetComponent<BoxCollider>().enabled = true;

            if (isMaxlvl())
            {
                ShowMaxLvl(true);
                UpdateMatInfo();
            }
            else
            {
                ShowMaxLvl(false);
            }
        }
    }

    /// <summary>
    /// 遮罩
    /// </summary>
    private void OnMaskBtnClick()
    {
        OnTagBtnClick(true);
        Hide();
    }

    /// <summary>
    /// 更换英雄框
    /// </summary>
    void UpdateBorder(int grade)
    {
        if (grade <= 1)
        {
            //白
            border.spriteName = "bai";

            UpdateStar(0);

        }
        else if (grade == 2 || grade == 3)
        {
            //绿
            border.spriteName = "lv";

            UpdateStar(grade - 2);

            if (grade == 3 && hd.grade == 1)
            {
                ShowMaxLvl(false);
            }

        }
        else if (grade == 4 || grade == 5 || grade == 6)
        {
            //蓝
            border.spriteName = "lan";

            UpdateStar(grade - 4);

            if (grade == 6 && hd.grade == 2)
            {
                ShowMaxLvl(false);
            }

        }
        else if (grade == 7 || grade == 8 || grade == 9 || grade == 10)
        {
            //紫
            border.spriteName = "zi";
            UpdateStar(grade - 7);
            if (grade == 10 && hd.grade == 3)
            {
                ShowMaxLvl(false);
            }
        }
        else if (grade == 11 || grade == 12 || grade == 13 || grade == 14 || grade == 15)
        {
            //橙
            border.spriteName = "cheng";
            UpdateStar(grade - 11);
            if (grade == 15 && hd.grade == 4)
            {
                ShowMaxLvl(false);
            }
        }
        else if (grade == 16 || grade == 17 || grade == 18 || grade == 19 || grade == 20 || grade == 21)
        {
            //红
            border.spriteName = "hong";
            UpdateStar(grade - 16);
            if (grade == 21 && hd.grade == 5)
            {
                ShowMaxLvl(false);
            }
        }
    }

    /// <summary>
    /// 更新星级
    /// </summary>
    void UpdateStar(int star)
    {

        for (int i = 0; i < star; i++)
        {
            starList[i].enabled = true;
        }

        for (int i = star; i < starList.Count; i++)
        {
            starList[i].enabled = false;
        }
    }

    /// <summary>
    /// 材料和Label互斥
    /// </summary>
    void ShowMaxLvl(bool isShow)
    {
        go.SetActive(isShow);
        maxLabel.enabled = !isShow;
    }

    /// <summary>
    /// 判断是否达到进化条件
    /// </summary>
    /// <returns></returns>
    bool isMaxlvl()
    {
        if (hd.grade == 1 && ed.grade < 3)
        {
            return true;
        }
        else if (hd.grade == 2 && ed.grade < 6)
        {
            return true;
        }
        else if (hd.grade == 3 && ed.grade < 10)
        {
            return true;
        }
        else if (hd.grade == 4 && ed.grade < 15)
        {
            return true;
        }
        else if (hd.grade == 5 && ed.grade < 21)
        {
            return true;
        }
        return false;
    }

    #region MyRegion


    public void ShowMatList(ItemVO item, int count)
    {

        powerL.gameObject.SetActive(false);
        addPowerL.gameObject.SetActive(false);
        hitL.gameObject.SetActive(false);
        addHitL.gameObject.SetActive(false);

        eMatMultList.gameObject.SetActive(true);

        eMatMultList.InSize(count, 5);

        eMatMultList.Info();

    }


    #endregion

}