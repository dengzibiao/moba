using UnityEngine;
using System.Collections;
using System;
using Tianyu;
using System.Text;

public class GoodsDetials : GUIBase {

    public UISprite goods;
    public UISprite iconS;
    public GUISingleSprite icon;
    public UILabel goodsName;
    public UILabel goodsCount;
    public UILabel goodsDescribe;
    public UILabel saleUnitPrice;
    public UILabel details;

    public GUISingleButton saleBtn;
    public GUISingleButton detailsBtn;
    private Transform debris;
    private object item;
    ItemData equipItem;
    public static GoodsDetials instance;
    private bool isUse = false;
    public ItemData currentSItemData;

    public GoodsDetials()
    {
        instance = this;
    }

    public override UIPanleID GetUIKey()
    {
        return UIPanleID.none;
    }


    protected override void Init()
    {

        goods = transform.Find("Goods").GetComponent<UISprite>();
        iconS = transform.Find("Goods/Icon").GetComponent<UISprite>();
        goodsName = transform.Find("GoodsName").GetComponent<UILabel>();
        goodsCount = transform.Find("GoodsCount").GetComponent<UILabel>();
        goodsDescribe = transform.Find("GoodsDescribe").GetComponent<UILabel>();
        saleUnitPrice = transform.Find("SaleUnitPrice").GetComponent<UILabel>();

        saleBtn = transform.Find("SaleBtn").GetComponent<GUISingleButton>();
        detailsBtn = transform.Find("DetailsBtn").GetComponent<GUISingleButton>();
        details = transform.Find("DetailsBtn/Label").GetComponent<UILabel>();
        debris = transform.Find("Goods/Debris");
        icon = transform.Find("Goods/Icon").GetComponent<GUISingleSprite>();
        saleBtn.onClick = OnSaleBtnClick;
        detailsBtn.onClick = OnDetailsBtnClick;
    }
    protected override void ShowHandler()
    {
        if (UIKnapsack.Instance.Objs == null)
        {
            return;
        }

        if(UIKnapsack.Instance!=null&& UIKnapsack.Instance.Objs!= null&&UIKnapsack.Instance.Objs.Length>0)
        SetData(UIKnapsack.Instance.Objs[0]);

    }
    public void SetData(object obj)
    {
        int count = 0;
        item = obj;
        equipItem = (ItemData)obj;
        for (int i = 0; i < playerData.GetInstance().baginfo.itemlist.Count; i++)
        {
            if (equipItem.Id == playerData.GetInstance().baginfo.itemlist[i].Id)
            {
                count = playerData.GetInstance().baginfo.itemlist[i].Count;
            }
        }

        iconS.spriteName = equipItem.IconName;
        goodsName.text = GoodsDataOperation.GetInstance().JointNameColour(equipItem.Name, equipItem.GradeTYPE);
        switch (equipItem.GradeTYPE)
        {
            case GradeType.Gray:
                goods.spriteName = "hui";
                break;
            case GradeType.Green:
                goods.spriteName = "lv";
                break;
            case GradeType.Blue:
                goods.spriteName = "lan";
                break;
            case GradeType.Purple:
                goods.spriteName = "zi";
                break;
            case GradeType.Orange:
                goods.spriteName = "cheng";
                break;
            case GradeType.Red:
                goods.spriteName = "hong";
                break;
            default:
                break;
        }
        if (equipItem.Types == (int)ItemType.ExpProp || equipItem.Types == (int)ItemType.RecoverProp|| equipItem.Types == (int)ItemType.JewelBox)
        {
            details.text = "使用";
            //isUse = true;
        }
        else
        {
            details.text = "详情";
            //isUse = false;
        }
        icon.uiAtlas = equipItem.UiAtlas;
        if (equipItem.Types == 6|| equipItem.Types == 3)
        {
            debris.gameObject.SetActive(true);
        }
        else
        {
            debris.gameObject.SetActive(false);
        }
        if (equipItem.Types == 3)
        {
            debris.GetComponent<UISprite>().spriteName = "materialdebris";
            debris.GetComponent<UISprite>().MakePixelPerfect();
        }
        else if (equipItem.Types == 6)
        {
            debris.GetComponent<UISprite>().spriteName = "linghunshi";
            debris.GetComponent<UISprite>().MakePixelPerfect();
        }
        //goodsCount.text = equipItem.conut.ToString();
        goodsCount.text = count + "";
        goodsDescribe.text = ConvertGoodsDes(equipItem);
        //goodsDescribe.text = equipItem.Describe;
        saleUnitPrice.text = equipItem.Sprice + "";
    }
    /// <summary>
    /// 转换item描述文字
    /// </summary>
    /// <param name="itemdata"></param>
    /// <returns></returns>
    public string ConvertGoodsDes(ItemData itemdata)
    {
        ItemNodeState itemnodestate = null;
        //道具类型 1：装备，2：材料，3：材料碎片，4：金币道具，5：经验道具，6：英雄灵魂石，7：英雄整卡，8：符文，9：体力恢复道具，10：扫荡卷，11：宝箱。(4,5,9,10,11,13为消耗品)，12：任务收集品；13：喇叭；14：货币
        if (itemdata.Types == (int)ItemType.SoulStone)
        {
            int heroid = int.Parse(StringUtil.StrReplace((itemdata.Id).ToString(), "201", 0, 3));
            if (FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList.ContainsKey(heroid))
            {
                int initStar = FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList[heroid].init_star;
                int count = FSDataNodeTable<StarUpGradeNode>.GetSingleton().DataNodeList[initStar].call_stone_num;
                return itemdata.Describe.Replace("[N]", count.ToString());
            }
        }
        else if (itemdata.Types == (int)ItemType.Rune)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(itemdata.Describe);

            if (GameLibrary.Instance().ItemStateList.ContainsKey(itemdata.Id))
            {
                itemnodestate = GameLibrary.Instance().ItemStateList[itemdata.Id];
                if (itemnodestate.power > 0) sb.Append("力量" + itemnodestate.power.ToString());
                if (itemnodestate.intelligence > 0) sb.Append("智力" + itemnodestate.intelligence.ToString());
                if (itemnodestate.agility > 0) sb.Append("敏捷" + itemnodestate.agility.ToString());
                if (itemnodestate.hp > 0) sb.Append("生命值" + itemnodestate.hp.ToString());
                if (itemnodestate.attack > 0) sb.Append("攻击强度" + itemnodestate.attack.ToString());
                if (itemnodestate.armor > 0) sb.Append("护甲" + itemnodestate.armor.ToString());
                if (itemnodestate.magic_resist > 0) sb.Append("魔抗" + itemnodestate.magic_resist.ToString());
                if (itemnodestate.critical > 0) sb.Append("暴击" + itemnodestate.critical.ToString());
                if (itemnodestate.dodge > 0) sb.Append("闪避" + itemnodestate.dodge.ToString());
                if (itemnodestate.hit_ratio > 0) sb.Append("命中" + itemnodestate.hit_ratio.ToString());
                if (itemnodestate.armor_penetration > 0) sb.Append("护甲穿透" + itemnodestate.armor_penetration.ToString());
                if (itemnodestate.magic_penetration > 0) sb.Append("魔法穿透" + itemnodestate.magic_penetration.ToString());
                //缺少吸血 和 韧性
                if (itemnodestate.movement_speed > 0) sb.Append("移动速度" + itemnodestate.movement_speed.ToString());
                if (itemnodestate.attack_speed > 0) sb.Append("攻击速度" + itemnodestate.attack_speed.ToString());
                if (itemnodestate.striking_distance > 0) sb.Append("攻击距离" + itemnodestate.striking_distance.ToString());
                //缺少生命恢复
            }
            return sb.ToString();
        }
        else if (itemdata.Types == (int)ItemType.ExpProp)
        {
            if (GameLibrary.Instance().ItemStateList.ContainsKey(itemdata.Id))
            {
                itemnodestate = GameLibrary.Instance().ItemStateList[itemdata.Id];
                return itemdata.Describe.Replace("[N]", itemnodestate.exp_gain.ToString());
            }
        }
        else if (itemdata.Types == (int)ItemType.GoldProp)
        {
            if (GameLibrary.Instance().ItemStateList.ContainsKey(itemdata.Id))
            {
                itemnodestate = GameLibrary.Instance().ItemStateList[itemdata.Id];
                return itemdata.Describe.Replace("[N]", itemnodestate.sprice.ToString());
            }
        }
        else if (itemdata.Types == (int)ItemType.RecoverProp)
        {
            if (GameLibrary.Instance().ItemStateList.ContainsKey(itemdata.Id))
            {
                itemnodestate = GameLibrary.Instance().ItemStateList[itemdata.Id];
                return itemdata.Describe.Replace("[N]", itemnodestate.power_add.ToString());
            }
        }
        return itemdata.Describe;
    }
    /// <summary>
    /// 详情按钮函数事件
    /// </summary>
    private void OnDetailsBtnClick()
    {
        //经验药
        if (equipItem.Types == (int)ItemType.ExpProp)
        {
            //currentSItemData = equipItem;
            //Globe.currentCount = equipItem.Count;
            //Control.ShowGUI(GameLibrary.UIHeroUseExp);
            //Control.HideGUI(GameLibrary.UI_Money);
            //Control.HideGUI(GameLibrary.UIKnapsack);
            //Control.GetGUI(GameLibrary.UIUseExpVialPanel).GetComponent<UIUseExpVialPanel>().SetData(item);
            //Control.ShowGUI(GameLibrary.UIUseExpVialPanel);
            Control.ShowGUI(UIPanleID.UIUseExpVialPanel, EnumOpenUIType.DefaultUIOrSecond, false, item);
        }
        else if (equipItem.Types == (int)ItemType.RecoverProp)
        {
            //Control.GetGUI(GameLibrary.UIUseEnergyItemPanel).GetComponent<UIUseEnergyItemPanel>().SetData(item);
            //Control.ShowGUI(GameLibrary.UIUseEnergyItemPanel);
            Control.ShowGUI(UIPanleID.UIUseEnergyItemPanel, EnumOpenUIType.DefaultUIOrSecond, false, item);
            Debug.Log("增加体力");
        }
        else if (equipItem.Types == (int)ItemType.JewelBox)
        {
            Debug.Log("打开宝箱");
        }
        else//详细 TODO
        {
            Control.ShowGUI(UIPanleID.UIGoodsGetWayPanel, EnumOpenUIType.DefaultUIOrSecond, false, equipItem.Id);
        }
    }
    /// <summary>
    /// 出售按钮函数事件
    /// </summary>
    private void OnSaleBtnClick()
    {
        //展示出售面板
        //Control.GetGUI(GameLibrary.UISalePanel).GetComponent<UISalePanel>().SetData(item);
        //Control.ShowGUI(GameLibrary.UISalePanel);
        Control.ShowGUI(UIPanleID.UISalePanel, EnumOpenUIType.DefaultUIOrSecond,false,item);
    }
    
}
