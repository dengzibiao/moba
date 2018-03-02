using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tianyu;




enum expType
{
    yaogao = 105000100,
    yaoji = 105000200,
    miyao = 105000300,
    chaojijingyan = 105000400,
    jiujijingyan = 105000500,
}
public class UIUpGradeStar : MonoBehaviour
{

    public GUISingleMultList EXPList;
    public UISprite Border;
    public UISprite Exp;
    public UILabel Upgradenumlab;
    public UILabel Explbl;
    public UIButton SoulStoneIcon;
    public UIButton RisingStarBtn;
    public GUISingleButton OneBtnUpgrade;
    public GUISingleButton BtnUpgrade;
    public UISlider SoulStoneBar;
    public UILabel Gold;
    public UpgradePanelX UpGradePanel;
    public UIPanel BottlPanel;
    public Dictionary<long, long> propsDic = new Dictionary<long, long>();
    public Dictionary<long, int> drugDic = new Dictionary<long, int>();
    public static UIUpGradeStar instance;



    ItemData soulItem;
    StarUpGradeNode starUpHero;
    HeroData hd;
    int currentSoul = 0;
    int soul_gem = 0;
    int userGlod = 0;

    long Recordid = 0;
    int Recordlvl = 0;
    int RecordCurE = 0;
    int RecordMaxE = 0;
    long DrugID = 0;
    int DrugCount = 0;

    void Start()
    {
        instance = this;
        OneBtnUpgrade.onClick = OneBtnUpgradeClick;
        BtnUpgrade.onClick = BtnUpgradeClick;
        EventDelegate.Set(SoulStoneIcon.onClick, OnsoulStoneIconClick);

    }


    public void RefreshDrug(HeroData hd)
    {

        this.hd = hd;

        List<ItemData> expItem = GoodsDataOperation.GetInstance().GetItemListByItmeType(ItemType.ExpProp);

        object[] drug = new object[12];

        for (int i = 0; i < expItem.Count; i++)
        {
            drug[i] = expItem[i];
        }
        //Debug.Log(GoodsDataOperation.GetInstance().GetItemListByItmeType(ItemType.ExpProp).Count);
        //经验药水集合
        //EXPList.InSize(GoodsDataOperation.GetInstance().GetItemListByItmeType(ItemType.ExpProp).Count, 3);
        //EXPList.Info(drug);

    }

    public void RefreshSoulStone(HeroData hd)
    {

        this.hd = hd;
        soulItem = playerData.GetInstance().GetItemDatatByID(hd.node.soul_gem);
        if (null == soulItem)
        {
            currentSoul = 0;
        }
        else
        {
            currentSoul = soulItem.Count;
        }

        SoulStoneIcon.GetComponent<UIButton>().normalSprite = Globe.selectHero.icon_name;

        if (hd.star < 5)
        {
            int star = hd.star;
            //RisingStarBtn.gameObject.SetActive(true);
            //Gold.gameObject.SetActive(true);
            starUpHero = FSDataNodeTable<StarUpGradeNode>.GetSingleton().FindDataByType(++star);
            //更换英雄魂石图标 Gold 魂石条

            //Gold.text = "" + starUpHero.evolve_cost;

            //魂石条赋值
            SoulStoneBar.value = currentSoul / (float)starUpHero.call_stone_num;
            SoulStoneBar.transform.Find("Label").GetComponent<UILabel>().text = currentSoul + "/" + starUpHero.call_stone_num;
        }
        else
        {
            SoulStoneBar.value = 1 / 1;
            SoulStoneBar.transform.Find("Label").GetComponent<UILabel>().text = currentSoul + "";
            //RisingStarBtn.gameObject.SetActive(false);
            //Gold.gameObject.SetActive(false);
        }
        //if (playerData.GetInstance().selfData.expPool > 0)
        //{
        //    Exp.gameObject.SetActive(true);
        //}
        //else
        //{
        //    Exp.gameObject.SetActive(false);
        //}
        Explbl.text = playerData.GetInstance().selfData.expPool.ToString();
        Upgradenumlab.text = "提升" + HeroAndEquipNodeData.Instance().GetHeroLV(hd,playerData.GetInstance().selfData.expPool).ToString() + "级";
    }

    public void SetMaskPanel(bool isShow)
    {
        BottlPanel.gameObject.SetActive(isShow);
    }

    public void OpenUpGradePanel(ItemNodeState item, HeroData hd)
    {
        UpGradePanel.RefreshUI(item, hd);
    }
    private void OnsoulStoneIconClick()
    {
        Control.Show(UIPanleID.UIGetWayPanel);
    }

    public void RecordUseDrug(HeroData hd, ItemNodeState item)
    {
        Recordid = hd.id;
        Recordlvl = hd.lvl;
        RecordCurE = hd.exps;
        RecordMaxE = hd.maxExps;
        DrugID = item.props_id;
        DrugCount = playerData.GetInstance().GetItemCountById(item.props_id);
    }

    public void UpdateInfo()
    {
        HeroData hd = playerData.GetInstance().GetHeroDataByID(Recordid);
        hd.lvl = Recordlvl;
        hd.exps = RecordCurE;
        hd.maxExps = RecordMaxE;

        int count = playerData.GetInstance().GetItemCountById(DrugID);
        GoodsDataOperation.GetInstance().AddGoods(DrugID, DrugCount - count);

        UI_HeroDetail.instance.NameArea.RefreshUI(hd);
        RefreshDrug(hd);
    }
    //一键升级
    void OneBtnUpgradeClick()
    {
        Debug.Log(hd.id);
        if (hd.lvl < playerData.GetInstance().selfData.level)
        {
            Debug.Log(FSDataNodeTable<HeroUpGradeNode>.GetSingleton().DataNodeList[hd.lvl].exp);
            if (playerData.GetInstance().selfData.expPool > FSDataNodeTable<HeroUpGradeNode>.GetSingleton().DataNodeList[hd.lvl].exp)
            {
                //发送升级的请求
                ClientSendDataMgr.GetSingle().GetHeroSend().SendDrugUpgrade(hd.id, 2);
            }
            else
            {
                //弹出链接位置
                //Control.ShowGUI(GameLibrary.UIExptips);
                //经验不足
                //Control.ShowGUI(GameLibrary.UIExpPropPanel);
                Control.ShowGUI(UIPanleID.UIExpPropPanel, EnumOpenUIType.DefaultUIOrSecond);
            }
        }
        else
        {
            Control.ShowGUI(UIPanleID.UITooltips, EnumOpenUIType.DefaultUIOrSecond, false, "请提升战队等级");
        }
    }
    void BtnUpgradeClick()
    {
        Debug.Log(hd.id);
        if (hd.lvl < playerData.GetInstance().selfData.level)
        {
            if (playerData.GetInstance().selfData .expPool> FSDataNodeTable<HeroUpGradeNode>.GetSingleton().DataNodeList[hd.lvl].exp)
            {
                //发送升级的请求
                ClientSendDataMgr.GetSingle().GetHeroSend().SendDrugUpgrade(hd.id, 1);
            }
            else
            {
                //弹出链接位置
                //Control.ShowGUI(GameLibrary.UIExptips);
                //经验不足
                //Control.ShowGUI(GameLibrary.UIExpPropPanel);
                Control.ShowGUI(UIPanleID.UIExpPropPanel, EnumOpenUIType.DefaultUIOrSecond);
            }
        }
        else
        {
            Control.ShowGUI(UIPanleID.UITooltips, EnumOpenUIType.DefaultUIOrSecond, false, "请提升战队等级");
        }
    }
}