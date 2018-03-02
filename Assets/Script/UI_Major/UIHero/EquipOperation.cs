using UnityEngine;
using System.Collections;
using Tianyu;


//EvolveBtn  OneEvolvesBtn
public class EquipOperation : GUIBase
{
    public  GUISingleButton OneEvolvesBtn;//一键进化按钮
    public  GUISingleButton EquipEvolveBtn;//装备进化按钮

    public  GUISingleButton CompoundBtn;//合成材料按钮
    public  GUISingleButton CancelBtn;//取消合成按钮

    public  UISprite EquipEvolves;//进化所需材料
    public  UISprite MaterialCompounds;//合成所需材料
    public  MaterialItsm[] MI;//进化材料
    public  MaterialItsm[] MI2;//合成材料
    public  GUISingleButton[] GBItem;//进化材料按钮
    public  GUISingleButton[] GBItem2;//合成材料按钮
    public  GUISingleButton IntensifyBtn;
    public  GUISingleButton EvolveBtn;
    public  UISprite IntensifyImg;
    public  UISprite EvolveImg;
    static ItemEquip IE;
    //site 1武器 2头盔 3胸甲 4腿甲 5护手 6鞋子
    static int sites = 0;
    //强化属性
    public  GUISingleMultList equlistMultList;//强化属性列表
    public  UISprite EquipIntensifyImg;//强化装备图片
    public  UISprite Intensifyicon;//装备强化品质框
    public  UILabel IntensifyName;//装备强化名称
    public  UILabel lvFront;//等级
    public  UILabel LvQueen;//下一级的强化等级
    public  UILabel ONE;//强化一级所需
    public  UILabel TEN;//强化十级所需
    public  GUISingleButton ALLIntensify;//强化全部按钮
    public  GUISingleButton OneBtnIntensify;//强化一级按钮
    public  UILabel IntensifyLV;//强化等级
    public  UISprite EquipIntensify;//一键强化到10级的lab
    public  UISprite EquipEvolve;//装备进化按钮


    //进化属性
    public  GUISingleMultList EquipEvolveMultList;//进化属性
    public  UISprite EquipFrontImg;//当前装备图标
    public  UISprite EquipQueenImg;//进化后的图标
    public  UISprite EquipFrontIcon;//当前的品质框
    public  UISprite EquipQueenIcon;//进化后的品质框
    public  UILabel EquipFrontName;//当前的装备名字
    public  UILabel EquipQueenName;//进化后的装备名称

    public  UISprite Evolve0;//进化所需材料
    public  UISprite Evolve1;
    public  UISprite Evolve2;

    public  UISprite material0;//材料1
    public  UISprite material1;
    public  UISprite material2;
    public  UISprite material3;
    public  GUISingleButton CompoundBox;//合成按钮
    public  UISprite CompoundName;//合成名字
    public  UISprite CompoundIcon;//合成装备的品质框
    public long propid;//材料id
   


    /// <summary>
    /// 单例
    /// </summary>
    private static EquipOperation mSingleton;
    public static EquipOperation Instance()
    {
    //    if (mSingleton == null)
         //   mSingleton = new EquipOperation();
        return mSingleton;
    }

    public override UIPanleID GetUIKey()
    {
        return UIPanleID.none;
    }

    /// <summary>
    /// 初始化
    /// </summary>
    protected override void Init()
    {
        mSingleton = this;
        //MI = new MaterialItsm[6];
        //GBItem = new GUISingleButton[6];
        //MI2 = new MaterialItsm[10];
        //GBItem2 = new GUISingleButton[10];
       if (IntensifyBtn == null)
        {
            IntensifyBtn = transform.Find("IntensifyBtn").GetComponent<GUISingleButton>();
            IntensifyImg = transform.Find("IntensifyBtn/IntensifyImg").GetComponent<UISprite>();
            EvolveBtn = transform.Find("EvolveBtn").GetComponent<GUISingleButton>();
            EvolveImg = transform.Find("EvolveBtn/EvolveImg").GetComponent<UISprite>();

            EquipEvolveBtn = transform.Find("EquipEvolve/EquipEvolves/EvolveBtn").GetComponent<GUISingleButton>();
            OneEvolvesBtn = transform.Find("EquipEvolve/EquipEvolves/OneEvolvesBtn").GetComponent<GUISingleButton>();

            CompoundBtn = transform.Find("EquipEvolve/MaterialCompound/CompoundBtn").GetComponent<GUISingleButton>();
            CancelBtn = transform.Find("EquipEvolve/MaterialCompound/CancelBtn").GetComponent<GUISingleButton>();

            //强化属性
            IntensifyLV = transform.Find("EquipIntensify/IntensifyLV").GetComponent<UILabel>();
            EquipIntensifyImg = transform.Find("EquipIntensify/EquipIntensifyImg").GetComponent<UISprite>();
            Intensifyicon = transform.Find("EquipIntensify/Intensifyicon").GetComponent<UISprite>();
            IntensifyName = transform.Find("EquipIntensify/IntensifyName").GetComponent<UILabel>();
            lvFront = transform.Find("EquipIntensify/lvFront").GetComponent<UILabel>();
            LvQueen = transform.Find("EquipIntensify/LvQueen").GetComponent<UILabel>();
            ONE = transform.Find("EquipIntensify/ONE/Label").GetComponent<UILabel>();
            TEN = transform.Find("EquipIntensify/TEN/Label").GetComponent<UILabel>();
            ALLIntensify = transform.Find("EquipIntensify/ALLIntensify").GetComponent<GUISingleButton>();
            OneBtnIntensify = transform.Find("EquipIntensify/OneBtnIntensify").GetComponent<GUISingleButton>();
            equlistMultList = transform.Find("EquipIntensify/EquipIntensifys/EquipIntensifyList").GetComponent<GUISingleMultList>();
            //进化属性
            EquipEvolveMultList = transform.Find("EquipEvolve/EquipEvolveView/EquipEvolveMultList").GetComponent<GUISingleMultList>();
            EquipFrontImg = transform.Find("EquipEvolve/EquipFrontImg").GetComponent<UISprite>();
            EquipFrontIcon = transform.Find("EquipEvolve/EquipFrontIcon").GetComponent<UISprite>();
            EquipFrontName = transform.Find("EquipEvolve/EquipFrontName").GetComponent<UILabel>();

            EquipQueenImg = transform.Find("EquipEvolve/EquipQueenImg").GetComponent<UISprite>();
            EquipQueenIcon = transform.Find("EquipEvolve/EquipQueenIcon").GetComponent<UISprite>();
            EquipQueenName = transform.Find("EquipEvolve/EquipQueenName").GetComponent<UILabel>();

            Evolve0 = transform.Find("EquipEvolve/EquipEvolves/Evolve0").GetComponent<UISprite>();
            Evolve1 = transform.Find("EquipEvolve/EquipEvolves/Evolve1").GetComponent<UISprite>();
            Evolve2 = transform.Find("EquipEvolve/EquipEvolves/Evolve2").GetComponent<UISprite>();

            material0 = transform.Find("EquipEvolve/MaterialCompound/material0").GetComponent<UISprite>();
            material1 = transform.Find("EquipEvolve/MaterialCompound/material1").GetComponent<UISprite>();
            material2 = transform.Find("EquipEvolve/MaterialCompound/material2").GetComponent<UISprite>();
            material3 = transform.Find("EquipEvolve/MaterialCompound/material3").GetComponent<UISprite>();

            CompoundBox = transform.Find("EquipEvolve/MaterialCompound/Compound").GetComponent<GUISingleButton>();
            CompoundName = transform.Find("EquipEvolve/MaterialCompound/Compound/CompoundName").GetComponent<UISprite>();
            CompoundIcon = transform.Find("EquipEvolve/MaterialCompound/CompoundIcon").GetComponent<UISprite>();

            EquipEvolves = transform.Find("EquipEvolve/EquipEvolves").GetComponent<UISprite>();
            MaterialCompounds = transform.Find("EquipEvolve/MaterialCompound").GetComponent<UISprite>();

            EquipEvolve = transform.Find("EquipEvolve").GetComponent<UISprite>();
            EquipIntensify = transform.Find("EquipIntensify").GetComponent<UISprite>();

            //材料ITem属性
            for (int i = 0; i < MI.Length; i++)
            {
                if (i == 0)
                {
                    MI[i] = transform.Find("EquipEvolve/EquipEvolves/Evolve" + 0 + "/Material").GetComponent<MaterialItsm>();
                }
                else if (i >= 1 && i < 3)
                {
                    MI[i] = transform.Find("EquipEvolve/EquipEvolves/Evolve" + 1 + "/Material").GetComponent<MaterialItsm>();
                }
                else if (i >= 3 && i < 6)
                {
                    MI[i] = transform.Find("EquipEvolve/EquipEvolves/Evolve" + 2 + "/Material").GetComponent<MaterialItsm>();
                }
            }

            //材料按钮动态添加时间
            for (int i = 0; i < GBItem.Length; i++)
            {
                if (i == 0)
                {
                    GBItem[i] = transform.Find("EquipEvolve/EquipEvolves/Evolve" + 0 + "/Material/MaterialName").GetComponent<GUISingleButton>();
                    GBItem[i].index = i;
                }
                else if (i >= 1 && i < 3)
                {
                    GBItem[i] = transform.Find("EquipEvolve/EquipEvolves/Evolve" + 1 + "/Material/MaterialName").GetComponent<GUISingleButton>();
                    GBItem[i].index = i;
                }
                else if (i >= 3 && i < 6)
                {
                    GBItem[i] = transform.Find("EquipEvolve/EquipEvolves/Evolve" + 2 + "/Material/MaterialName").GetComponent<GUISingleButton>();
                    GBItem[i].index = i;
                }
            }
            //合成
            for (int i = 0; i < MI2.Length; i++)
            {
                if (i == 0)
                {
                    MI2[i] = transform.Find("EquipEvolve/MaterialCompound/material" + 0 + "/Material").GetComponent<MaterialItsm>();
                }
                else if (i >= 1 && i < 3)
                {
                    MI2[i] = transform.Find("EquipEvolve/MaterialCompound/material" + 1 + "/Material").GetComponent<MaterialItsm>();
                }
                else if (i >= 3 && i < 6)
                {
                    MI2[i] = transform.Find("EquipEvolve/MaterialCompound/material" + 2 + "/Material").GetComponent<MaterialItsm>();
                }
                else if (i >= 6)
                {
                    MI2[i] = transform.Find("EquipEvolve/MaterialCompound/material" + 3 + "/Material").GetComponent<MaterialItsm>();
                }

            }

            //材料按钮动态添加时间
            for (int i = 0; i < GBItem2.Length; i++)
            {
                if (i == 0)
                {
                    GBItem2[i] = transform.Find("EquipEvolve/MaterialCompound/material" + 0 + "/Material/MaterialName").GetComponent<GUISingleButton>();
                    GBItem2[i].index = i;
                }
                else if (i >= 1 && i < 3)
                {
                    GBItem2[i] = transform.Find("EquipEvolve/MaterialCompound/material" + 1 + "/Material/MaterialName").GetComponent<GUISingleButton>();
                    GBItem2[i].index = i;
                }
                else if (i >= 3 && i < 6)
                {
                    GBItem2[i] = transform.Find("EquipEvolve/MaterialCompound/material" + 2 + "/Material/MaterialName").GetComponent<GUISingleButton>();
                    GBItem2[i].index = i;
                }
                else if (i >= 6)
                {
                    GBItem2[i] = transform.Find("EquipEvolve/MaterialCompound/material" + 3 + "/Material/MaterialName").GetComponent<GUISingleButton>();
                    GBItem2[i].index = i;
                }
            }
        }
        for (int i = 0; i < GBItem.Length; i++)
        {
            GBItem[i].onItemClick = MIClick;
        }
        for (int i = 0; i < GBItem2.Length; i++)
        {
            GBItem2[i].onItemClick = MI2Click;
        }
        CompoundBtn.onClick = EquipCompoundOnClick;
        CancelBtn.onClick = OneCancelOnClick;

        EquipEvolveBtn.onClick = EquipEvolveOnClick;
        OneEvolvesBtn.onClick = OneEvolvesOnClick;
        CompoundBox.onClick = Reverse;
        IntensifyBtn.onClick = IntensifyBtnClick;
        EvolveBtn.onClick = EvolveBtnClick;
        ALLIntensify.onClick = ALLIntensifyBtnClick;
        OneBtnIntensify.onClick = OneBtnIntensifyBtnClick;
        IntensifyImg.gameObject.SetActive(true);
        if(EquipPanel.instance!=null&&EquipPanel.instance.ItemEquiplist[UI_HeroDetail.equipItemState]!=null)
        RefreshUI(1,EquipPanel.instance.ItemEquiplist[UI_HeroDetail.equipItemState]);
    }
    protected override void ShowHandler()
    {
        if (HeroAndEquipNodeData.TabType == 0)
        {
            IntensifyImg.gameObject.SetActive(true);
            EvolveImg.gameObject.SetActive(false);
        }
        else
        {
            IntensifyImg.gameObject.SetActive(false);
            EvolveImg.gameObject.SetActive(true);
        }
        if (HeroAndEquipNodeData.TabType == 0)
        {
            IntensifyImg.gameObject.SetActive(true);
            EvolveImg.gameObject.SetActive(false);
            SetIntensifyData(IE, sites);
            
        }
        else if (HeroAndEquipNodeData.TabType == 1)
        {
            SetEvolveData(IE, sites);
        }
        else
        {
            SetCompoundData(HeroAndEquipNodeData.MI1s,HeroAndEquipNodeData.MI1s.INS.props_id);
        }
    }
    private void SetIntensifyData(ItemEquip IE, int site)
    {
        if (EquipEvolve != null)
            EquipEvolve.gameObject.SetActive(false);

        if (EquipIntensify != null)
            EquipIntensify.gameObject.SetActive(true);

        if (EquipIntensifyImg != null && IE != null && IE.itemVO != null)
        {
            EquipIntensifyImg.spriteName = IE.itemVO.icon_name;
        }

        if (Intensifyicon != null )
            Intensifyicon.spriteName = UISign_in.GetspriteName(IE.itemVO.grade);

        if (IntensifyName != null && IE != null && IE.itemVO != null)
            IntensifyName.text = IE.itemVO.name;

        HeroData hd = playerData.GetInstance().GetHeroDataByID(Globe.selectHero.hero_id);
        EquipData ed;
        hd.equipSite.TryGetValue(site, out ed);
        if (ed != null)
        {
            lvFront.text = ed.level + "级";
            LvQueen.text = ed.level + 1 + "级";
        }
        long ALLEquipMoney = 0;
        for (int i = 0; i < EquipPanel.instance.ItemEquiplist.Length; i++)
        {
            if (EquipPanel.instance.ItemEquiplist[i] != null)
            {
                hd.equipSite.TryGetValue(i + 1, out ed);
                foreach (var item in FSDataNodeTable<EquipUpgradeNode>.GetSingleton().DataNodeList.Values)
                {
                    if (ed != null)
                    {
                        if (ed.level == item.id)
                        {
                            ALLEquipMoney += item.consume;
                        }
                    }
                }
            }
        }
        ONE.text = ALLEquipMoney.ToString();
        HeroAndEquipNodeData.Instance().GetUpOneEquipNum(sites);
        TEN.text = HeroAndEquipNodeData.Money.ToString();
        IntensifyLV.text = "一键强化到" + HeroAndEquipNodeData.lv.ToString() + "级";
        //TEN.text=
        //item
        if (UI_HeroDetail.BtnState == false)
        {
           // EquipIntensifyList.Instance().refreshUI(IE);
        }
        equlistMultList.InSize(18, 1);
        equlistMultList.Info(EquipPanel.instance.ItemEquiplist);
    }
    //进化
    private void SetEvolveData(ItemEquip IE, int site)
    {
        if (IE != null && IE.itemVO != null)
        {
            MaterialCompounds.gameObject.SetActive(false);
            EquipEvolves.gameObject.SetActive(true);
            //这是装备DATA
            EquipFrontImg.spriteName = GameLibrary.Instance().ItemStateList[IE.itemVO.props_id].icon_name;
            EquipFrontIcon.spriteName = UISign_in.GetspriteName(IE.itemVO.grade);
            EquipFrontName.text = GameLibrary.Instance().ItemStateList[IE.itemVO.props_id].name;
            EquipQueenImg.spriteName = GameLibrary.Instance().ItemStateList[IE.itemVO.next_grade].icon_name;
            EquipQueenIcon.spriteName = UISign_in.GetspriteName(GameLibrary.Instance().ItemStateList[IE.itemVO.next_grade].grade);
            EquipQueenName.text = GameLibrary.Instance().ItemStateList[IE.itemVO.next_grade].name;

         //   EquipIntensifyList.Instance().refreshUI(IE);
            EquipEvolveMultList.InSize(18, 1);
            EquipEvolveMultList.Info(EquipPanel.instance.ItemEquiplist);
            ItemNodeState ins;
            //这里把材料的二维数组除以2 这是材料DATA
            switch (GetSyn_conditionLength(GameLibrary.Instance().ItemStateList[IE.itemVO.next_grade].syn_condition.Length))
            {
                case 1:
                    ins = GameLibrary.Instance().ItemStateList[GameLibrary.Instance().ItemStateList[IE.itemVO.next_grade].syn_condition[0, 0]];
                   // HeroAndEquipNodeData.Locations = 0;
                    MI[0].RefreshUI(ins,0);
                    Evolve0.gameObject.SetActive(true);
                    Evolve1.gameObject.SetActive(false);
                    Evolve2.gameObject.SetActive(false);
                    break;
                case 2:
                    ins = GameLibrary.Instance().ItemStateList[GameLibrary.Instance().ItemStateList[IE.itemVO.next_grade].syn_condition[0, 0]];
                   
                    MI[1].RefreshUI(ins,0);
                   // HeroAndEquipNodeData.Locations = 0;
                    ins = GameLibrary.Instance().ItemStateList[GameLibrary.Instance().ItemStateList[IE.itemVO.next_grade].syn_condition[1, 0]];
                    MI[2].RefreshUI(ins,1);
                   // HeroAndEquipNodeData.Locations = 1;
                    Evolve1.gameObject.SetActive(true);
                    Evolve0.gameObject.SetActive(false);
                    Evolve2.gameObject.SetActive(false);
                   // HeroAndEquipNodeData.Locations = 0;
                    break;
                case 3:
                    ins = GameLibrary.Instance().ItemStateList[GameLibrary.Instance().ItemStateList[IE.itemVO.next_grade].syn_condition[0, 0]];
                   
                    MI[3].RefreshUI(ins,0);
                    //HeroAndEquipNodeData.Locations = 0;
                    ins = GameLibrary.Instance().ItemStateList[GameLibrary.Instance().ItemStateList[IE.itemVO.next_grade].syn_condition[1, 0]];
                  
                    MI[4].RefreshUI(ins,1);
                    //HeroAndEquipNodeData.Locations = 1;
                    ins = GameLibrary.Instance().ItemStateList[GameLibrary.Instance().ItemStateList[IE.itemVO.next_grade].syn_condition[2, 0]];
                    MI[5].RefreshUI(ins,2);
                   // HeroAndEquipNodeData.Locations = 2;
                    Evolve2.gameObject.SetActive(true);
                    Evolve1.gameObject.SetActive(false);
                    Evolve0.gameObject.SetActive(false);
                  //  HeroAndEquipNodeData.Locations = 0;
                    break;
                default:
                    break;
            }
        }
    }
    //合成
    private void SetCompoundData(MaterialItsm MIs,long id)
    {
        propid = id;
        MIs.INS = GameLibrary.Instance().ItemStateList[id];
        if (MIs.INS != null)
        {
            CompoundName.spriteName = MIs.INS.icon_name;
            CompoundIcon.spriteName = UISign_in.GetspriteName(MIs.INS.grade);
            ItemNodeState ins;
            switch (GetSyn_conditionLength(GameLibrary.Instance().ItemStateList[id].syn_condition.Length))
            {
                case 1:
                    ins = GameLibrary.Instance().ItemStateList[HeroAndEquipNodeData.Instance().GetReplaceNum(GameLibrary.Instance().ItemStateList[id].syn_condition[0, 0].ToString())];
                    // HeroAndEquipNodeData.Locations = 0;
                    MI2[0].RefreshUI(ins, 0);
                    material0.gameObject.SetActive(true);
                    material1.gameObject.SetActive(false);
                    material2.gameObject.SetActive(false);
                    material3.gameObject.SetActive(false);
                    break;
                case 2:
                    ins = GameLibrary.Instance().ItemStateList[HeroAndEquipNodeData.Instance().GetReplaceNum(GameLibrary.Instance().ItemStateList[id].syn_condition[0, 0].ToString())];
                    MI2[1].RefreshUI(ins, 0);
                    //  HeroAndEquipNodeData.Locations = 0;
                    ins = GameLibrary.Instance().ItemStateList[HeroAndEquipNodeData.Instance().GetReplaceNum(GameLibrary.Instance().ItemStateList[id].syn_condition[1, 0].ToString())];
                    MI2[2].RefreshUI(ins, 1);
                    // HeroAndEquipNodeData.Locations = 1;
                    material0.gameObject.SetActive(false);
                    material1.gameObject.SetActive(true);
                    material2.gameObject.SetActive(false);
                    material3.gameObject.SetActive(false);
                    // HeroAndEquipNodeData.Locations = 0;
                    break;
                case 3:
                    ins = GameLibrary.Instance().ItemStateList[HeroAndEquipNodeData.Instance().GetReplaceNum(GameLibrary.Instance().ItemStateList[MIs.INS.props_id].syn_condition[0, 0].ToString())];

                    MI2[3].RefreshUI(ins, 0);
                    //  HeroAndEquipNodeData.Locations = 0;
                    ins = GameLibrary.Instance().ItemStateList[HeroAndEquipNodeData.Instance().GetReplaceNum(GameLibrary.Instance().ItemStateList[MIs.INS.props_id].syn_condition[1, 0].ToString())];
                    MI2[4].RefreshUI(ins, 1);
                    //  HeroAndEquipNodeData.Locations = 1;
                    ins = GameLibrary.Instance().ItemStateList[HeroAndEquipNodeData.Instance().GetReplaceNum(GameLibrary.Instance().ItemStateList[MIs.INS.props_id].syn_condition[2, 0].ToString())];
                    MI2[5].RefreshUI(ins, 2);
                    //  HeroAndEquipNodeData.Locations = 2;
                    material0.gameObject.SetActive(false);
                    material1.gameObject.SetActive(false);
                    material2.gameObject.SetActive(false);
                    material3.gameObject.SetActive(false);
                    // HeroAndEquipNodeData.Locations = 0;
                    break;
                case 4:
                    ins = GameLibrary.Instance().ItemStateList[HeroAndEquipNodeData.Instance().GetReplaceNum(GameLibrary.Instance().ItemStateList[MIs.INS.props_id].syn_condition[0, 0].ToString())];

                    MI2[6].RefreshUI(ins, 0);
                    // HeroAndEquipNodeData.Locations = 0;
                    ins = GameLibrary.Instance().ItemStateList[HeroAndEquipNodeData.Instance().GetReplaceNum(GameLibrary.Instance().ItemStateList[MIs.INS.props_id].syn_condition[1, 0].ToString())];
                    MI2[7].RefreshUI(ins, 1);
                    //HeroAndEquipNodeData.Locations = 1;
                    ins = GameLibrary.Instance().ItemStateList[HeroAndEquipNodeData.Instance().GetReplaceNum(GameLibrary.Instance().ItemStateList[MIs.INS.props_id].syn_condition[2, 0].ToString())];
                    MI2[8].RefreshUI(ins, 2);
                    // HeroAndEquipNodeData.Locations = 2;
                    ins = GameLibrary.Instance().ItemStateList[HeroAndEquipNodeData.Instance().GetReplaceNum(GameLibrary.Instance().ItemStateList[MIs.INS.props_id].syn_condition[3, 0].ToString())];
                    MI2[9].RefreshUI(ins, 3);
                    //HeroAndEquipNodeData.Locations = 3;
                    material0.gameObject.SetActive(false);
                    material1.gameObject.SetActive(false);
                    material2.gameObject.SetActive(false);
                    material3.gameObject.SetActive(true);
                    // HeroAndEquipNodeData.Locations = 0;
                    break;
                default:
                    break;
            }
            HeroAndEquipNodeData.MI.Add(MIs);
        }
    }

    int GetSyn_conditionLength(int length)
    {
        switch (length)
        {
            case 2:
                return 1;
            case 4:
                return 2;
            default:
                break;
        }
        return 0;
    }
    //强化
    void IntensifyBtnClick()
    {
        HeroAndEquipNodeData.TabType = 0;
        IntensifyImg.gameObject.SetActive(true);
        SetIntensifyData(IE, HeroAndEquipNodeData.site);
        EvolveImg.gameObject.SetActive(false);
    }
    //进化
    void EvolveBtnClick()
    {
        HeroAndEquipNodeData.TabType = 1;
        EquipEvolve.gameObject.SetActive(true);
        SetEvolveData(IE, HeroAndEquipNodeData.site);
        IntensifyImg.gameObject.SetActive(false);
        EquipIntensify.gameObject.SetActive(false);
        EvolveImg.gameObject.SetActive(true);
    }
    public void RefreshUI(int site, ItemEquip ie)
    {
        sites = site;
        IE = ie;
        if (UI_HeroDetail.equipItemState == 0)
        {
            UI_HeroDetail.equipItemState = 1;
        }
        else if (UI_HeroDetail.equipItemState == 3)
        {
            ShowHandler();
        }
        else
        {
            ShowHandler();
            UI_HeroDetail.equipItemState = 0;
        }

    }
    void ALLIntensifyBtnClick()
    {
        if (HeroAndEquipNodeData.Instance().GetUpALLEquipNum().Count > 0)
        {
            ClientSendDataMgr.GetSingle().GetHeroSend().SendUpGradeHE(UI_HeroDetail.hd.id, HeroAndEquipNodeData.Instance().GetUpALLEquipNum(), HeroAndEquipNodeData.Instance().GetMoney(), C2SMessageType.Active);
        }
        else
        {
            if (HeroAndEquipNodeData.LvState == false)
            {
                Control.ShowGUI(UIPanleID.UITooltips, EnumOpenUIType.DefaultUIOrSecond, false, "请提升英雄等级");
            }
            else
            {
                Control.ShowGUI(UIPanleID.UIGoldHand, EnumOpenUIType.DefaultUIOrSecond);
            }
        }
    }

    void OneBtnIntensifyBtnClick()
    {
        if (HeroAndEquipNodeData.Instance().GetUpOneEquipNum(HeroAndEquipNodeData.site).Count > 0)
        {
            ClientSendDataMgr.GetSingle().GetHeroSend().SendUpGradeHE(UI_HeroDetail.hd.id, HeroAndEquipNodeData.Instance().GetUpOneEquipNum(HeroAndEquipNodeData.site), (int)HeroAndEquipNodeData.Money, C2SMessageType.Active);
        }
        else
        {
            if (HeroAndEquipNodeData.LvState == false)
            {
                Control.ShowGUI(UIPanleID.UITooltips, EnumOpenUIType.DefaultUIOrSecond, false, "请提升英雄等级");
            }
            else
            {
                //金币不足演出点金手界面
                Control.ShowGUI(UIPanleID.UIGoldHand, EnumOpenUIType.DefaultUIOrSecond);
            }
        }
    }
    void MIClick(int index)
    {
        for (int i = 0; i < MI.Length; i++)
        {
            if (index == i)
            {
                if (MI[i].INS!=null&&MI[i].INS.syn_condition.Length > 0)
                {
                    HeroAndEquipNodeData.TabType = 2;
                    HeroAndEquipNodeData.MI1s = MI[i];
                    SetCompoundData(MI[i],MI[i].INS.props_id);
                    MaterialCompounds.gameObject.SetActive(true);
                    EquipEvolves.gameObject.SetActive(false);
                }
                else
                {
                    //没有合成物品的Tips
                }
            }
        }
    }
    void MI2Click(int index)
    {
        for (int i = 0; i < HeroAndEquipNodeData.INSlist.Count; i++)
        {
            if (index == i)
            {

                if (HeroAndEquipNodeData.INSlist[i].syn_condition.Length > 0)
                {
                    Debug.Log(HeroAndEquipNodeData.INSlist[i].props_id);
                    Debug.Log(GameLibrary.Instance().ItemStateList[HeroAndEquipNodeData.INSlist[i].props_id].syn_condition.Length);
                    if (GameLibrary.Instance().ItemStateList[GameLibrary.Instance().ItemStateList[HeroAndEquipNodeData.INSlist[i].props_id].syn_condition[0,0]].syn_condition.Length > 0)
                    {
                        SetCompoundData(MI2[i],MI2[i].INS.props_id);
                    }
                    else
                    {
                        Control.ShowGUI(UIPanleID.UITooltips, EnumOpenUIType.DefaultUIOrSecond, false, "没有可合成的材料.");
                    }

                }
                else
                {
                    Control.ShowGUI(UIPanleID.UITooltips, EnumOpenUIType.DefaultUIOrSecond, false, "没有可合成的材料.");
                }
            }
        }
    }
    void Reverse()
    {

    }
    //装备进化
    void EquipEvolveOnClick()
    {
        int needlv = (1 + UI_HeroDetail.hd.grade) * UI_HeroDetail.hd.grade / 2 + 1;
         if(IE.itemVO.grade+1>needlv)
        {
            Control.ShowGUI(UIPanleID.UITooltips, EnumOpenUIType.DefaultUIOrSecond, false, "请提升英雄品质");
            return;
        }
        int count = GameLibrary.Instance().ItemStateList[IE.itemVO.next_grade].syn_condition.Length / 2;
        bool isCondition = true;
        int i = 0;
        long itemid;
        int itemcount;
        for(;i<count;i++)
        {
            itemid = GameLibrary.Instance().ItemStateList[IE.itemVO.next_grade].syn_condition[i, 0];
            itemcount = (int)GameLibrary.Instance().ItemStateList[IE.itemVO.next_grade].syn_condition[i, 1];
            if (GoodsDataOperation.GetInstance().GetItemCountById(itemid) < itemcount) 
            {
                isCondition = false;
                break;
            }
        }
        if(isCondition)//条件满足
        {
            //发送通信协议
            ClientSendDataMgr.GetSingle().GetHeroSend().SendHeroEMon(UI_HeroDetail.hd.id, HeroAndEquipNodeData.site);
        }
        else
        {
            //没满足条件，弹出提示 并打开合成面板
           // (GameLibrary.Instance().ItemStateList[IE.itemVO.props_id + 1].syn_condition[i, 0])
            {
                SetCompoundData(MI[GetIndex(i,count)],MI[GetIndex(i,count)].INS.props_id);
            }

            MaterialCompounds.gameObject.SetActive(true);
            EquipEvolves.gameObject.SetActive(false);
        }
        
    }
    int GetIndex(int pos,int count)
    {
        if (count == 1)
            return 0;
        return (count - 1) * 2 + pos-1;
    }
    //进化
    void OneEvolvesOnClick()
    {
        int needlv = (1 + UI_HeroDetail.hd.grade) * UI_HeroDetail.hd.grade / 2 + 1;
        if (IE.itemVO.grade + 1 > needlv)
        {
            Control.ShowGUI(UIPanleID.UITooltips, EnumOpenUIType.DefaultUIOrSecond, false, "请提升英雄品质");
            return;
        }

        ClientSendDataMgr.GetSingle().GetHeroSend().SendHeroEMon(UI_HeroDetail.hd.id, HeroAndEquipNodeData.site);
    }
    //合成
    void EquipCompoundOnClick()
    {
        int needcount = GameLibrary.Instance().ItemStateList[propid].syn_condition.Length / 2;
        long itemid;
        int itemcount;
        bool isenable = true;
        for(int i = 0;i<needcount;i++)
        {
            itemid = GameLibrary.Instance().ItemStateList[propid].syn_condition[i, 0];
            itemcount =(int) GameLibrary.Instance().ItemStateList[propid].syn_condition[i, 1];
            if (GoodsDataOperation.GetInstance().GetItemCountById(itemid) < itemcount)
            {
                isenable = false;
                break;
            }

        }
        if(isenable)
        {
            ClientSendDataMgr.GetSingle().GetHeroSend().SendHeroECom(propid, 1);
        }
        else
        {
            Control.ShowGUI(UIPanleID.UITooltips, EnumOpenUIType.DefaultUIOrSecond, false, "材料不足.");
        }
    }
    //取消
    void OneCancelOnClick()
    {
        MaterialCompounds.gameObject.SetActive(false);
        EquipEvolves.gameObject.SetActive(true);
    }

//    protected override void RegisterComponent()
//    {
//        base.RegisterComponent();
        
//        RegisterComponentID(17, 94, OneBtnIntensify.gameObject);
//        RegisterComponentID(18, 94, EvolveBtn.gameObject);
//        RegisterComponentID(20, 94, EquipEvolveBtn.gameObject);
//}
}
