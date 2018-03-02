using UnityEngine;
using System.Collections;

public class EquipOperationPanel : GUIBase {
    static EquipOperationPanel singleton;
    public EquipOperationPanel Instance()
    {
        return singleton;
    }
    // Use this for initialization
    public override UIPanleID GetUIKey()
    {
        return UIPanleID.none;
    }

    void Start () {
        singleton = this;
	
	}

    public GUISingleButton OneEvolvesBtn;//一键进化按钮
    public GUISingleButton EquipEvolveBtn;//装备进化按钮

    public GUISingleButton CompoundBtn;//合成材料按钮
    public GUISingleButton CancelBtn;//取消合成按钮

    public UISprite EquipEvolves;//进化所需材料
    public UISprite MaterialCompounds;//合成所需材料
    public MaterialItsm[] MI;//进化材料
    public MaterialItsm[] MI2;//合成材料
    public GUISingleButton[] GBItem;//进化材料按钮
    public GUISingleButton[] GBItem2;//合成材料按钮
    public GUISingleButton IntensifyBtn;
    public GUISingleButton EvolveBtn;
    public UISprite IntensifyImg;
    public UISprite EvolveImg;
    static ItemEquip IE;
    //site 1武器 2头盔 3胸甲 4腿甲 5护手 6鞋子
    static int sites = 0;
    //强化属性
    public GUISingleMultList equlistMultList;//强化属性列表
    public UISprite EquipIntensifyImg;//强化装备图片
    public UISprite Intensifyicon;//装备强化品质框
    public UILabel IntensifyName;//装备强化名称
    public UILabel lvFront;//等级
    public UILabel LvQueen;//下一级的强化等级
    public UILabel ONE;//强化一级所需
    public UILabel TEN;//强化十级所需
    public GUISingleButton ALLIntensify;//强化全部按钮
    public GUISingleButton OneBtnIntensify;//强化一级按钮
    public UILabel IntensifyLV;//强化等级
    public UISprite EquipIntensify;//一键强化到10级的lab
    public UISprite EquipEvolve;//装备进化按钮


    //进化属性
    public GUISingleMultList EquipEvolveMultList;//进化属性
    public UISprite EquipFrontImg;//当前装备图标
    public UISprite EquipQueenImg;//进化后的图标
    public UISprite EquipFrontIcon;//当前的品质框
    public UISprite EquipQueenIcon;//进化后的品质框
    public UILabel EquipFrontName;//当前的装备名字
    public UILabel EquipQueenName;//进化后的装备名称

    public UISprite Evolve0;//进化所需材料
    public UISprite Evolve1;
    public UISprite Evolve2;

    public UISprite material0;//材料1
    public UISprite material1;
    public UISprite material2;
    public UISprite material3;
    public GUISingleButton CompoundBox;//合成按钮
    public UISprite CompoundName;//合成名字
    public UISprite CompoundIcon;//合成装备的品质框
    public long propid;//材料id
    protected override void Init()
    {
        //mSingleton = this;
        ////MI = new MaterialItsm[6];
        ////GBItem = new GUISingleButton[6];
        ////MI2 = new MaterialItsm[10];
        ////GBItem2 = new GUISingleButton[10];
        //if (IntensifyBtn == null)
        //{
        //    IntensifyBtn = transform.Find("IntensifyBtn").GetComponent<GUISingleButton>();
        //    IntensifyImg = transform.Find("IntensifyBtn/IntensifyImg").GetComponent<UISprite>();
        //    EvolveBtn = transform.Find("EvolveBtn").GetComponent<GUISingleButton>();
        //    EvolveImg = transform.Find("EvolveBtn/EvolveImg").GetComponent<UISprite>();

        //    EquipEvolveBtn = transform.Find("EquipEvolve/EquipEvolves/EvolveBtn").GetComponent<GUISingleButton>();
        //    OneEvolvesBtn = transform.Find("EquipEvolve/EquipEvolves/OneEvolvesBtn").GetComponent<GUISingleButton>();

        //    CompoundBtn = transform.Find("EquipEvolve/MaterialCompound/CompoundBtn").GetComponent<GUISingleButton>();
        //    CancelBtn = transform.Find("EquipEvolve/MaterialCompound/CancelBtn").GetComponent<GUISingleButton>();

        //    //强化属性
        //    IntensifyLV = transform.Find("EquipIntensify/IntensifyLV").GetComponent<UILabel>();
        //    EquipIntensifyImg = transform.Find("EquipIntensify/EquipIntensifyImg").GetComponent<UISprite>();
        //    Intensifyicon = transform.Find("EquipIntensify/Intensifyicon").GetComponent<UISprite>();
        //    IntensifyName = transform.Find("EquipIntensify/IntensifyName").GetComponent<UILabel>();
        //    lvFront = transform.Find("EquipIntensify/lvFront").GetComponent<UILabel>();
        //    LvQueen = transform.Find("EquipIntensify/LvQueen").GetComponent<UILabel>();
        //    ONE = transform.Find("EquipIntensify/ONE/Label").GetComponent<UILabel>();
        //    TEN = transform.Find("EquipIntensify/TEN/Label").GetComponent<UILabel>();
        //    ALLIntensify = transform.Find("EquipIntensify/ALLIntensify").GetComponent<GUISingleButton>();
        //    OneBtnIntensify = transform.Find("EquipIntensify/OneBtnIntensify").GetComponent<GUISingleButton>();
        //    equlistMultList = transform.Find("EquipIntensify/EquipIntensifys/EquipIntensifyList").GetComponent<GUISingleMultList>();
        //    //进化属性
        //    EquipEvolveMultList = transform.Find("EquipEvolve/EquipEvolveView/EquipEvolveMultList").GetComponent<GUISingleMultList>();
        //    EquipFrontImg = transform.Find("EquipEvolve/EquipFrontImg").GetComponent<UISprite>();
        //    EquipFrontIcon = transform.Find("EquipEvolve/EquipFrontIcon").GetComponent<UISprite>();
        //    EquipFrontName = transform.Find("EquipEvolve/EquipFrontName").GetComponent<UILabel>();

        //    EquipQueenImg = transform.Find("EquipEvolve/EquipQueenImg").GetComponent<UISprite>();
        //    EquipQueenIcon = transform.Find("EquipEvolve/EquipQueenIcon").GetComponent<UISprite>();
        //    EquipQueenName = transform.Find("EquipEvolve/EquipQueenName").GetComponent<UILabel>();

        //    Evolve0 = transform.Find("EquipEvolve/EquipEvolves/Evolve0").GetComponent<UISprite>();
        //    Evolve1 = transform.Find("EquipEvolve/EquipEvolves/Evolve1").GetComponent<UISprite>();
        //    Evolve2 = transform.Find("EquipEvolve/EquipEvolves/Evolve2").GetComponent<UISprite>();

        //    material0 = transform.Find("EquipEvolve/MaterialCompound/material0").GetComponent<UISprite>();
        //    material1 = transform.Find("EquipEvolve/MaterialCompound/material1").GetComponent<UISprite>();
        //    material2 = transform.Find("EquipEvolve/MaterialCompound/material2").GetComponent<UISprite>();
        //    material3 = transform.Find("EquipEvolve/MaterialCompound/material3").GetComponent<UISprite>();

        //    CompoundBox = transform.Find("EquipEvolve/MaterialCompound/Compound").GetComponent<GUISingleButton>();
        //    CompoundName = transform.Find("EquipEvolve/MaterialCompound/Compound/CompoundName").GetComponent<UISprite>();
        //    CompoundIcon = transform.Find("EquipEvolve/MaterialCompound/CompoundIcon").GetComponent<UISprite>();

        //    EquipEvolves = transform.Find("EquipEvolve/EquipEvolves").GetComponent<UISprite>();
        //    MaterialCompounds = transform.Find("EquipEvolve/MaterialCompound").GetComponent<UISprite>();

        //    EquipEvolve = transform.Find("EquipEvolve").GetComponent<UISprite>();
        //    EquipIntensify = transform.Find("EquipIntensify").GetComponent<UISprite>();

        //    //材料ITem属性
        //    for (int i = 0; i < MI.Length; i++)
        //    {
        //        if (i == 0)
        //        {
        //            MI[i] = transform.Find("EquipEvolve/EquipEvolves/Evolve" + 0 + "/Material").GetComponent<MaterialItsm>();
        //        }
        //        else if (i >= 1 && i < 3)
        //        {
        //            MI[i] = transform.Find("EquipEvolve/EquipEvolves/Evolve" + 1 + "/Material").GetComponent<MaterialItsm>();
        //        }
        //        else if (i >= 3 && i < 6)
        //        {
        //            MI[i] = transform.Find("EquipEvolve/EquipEvolves/Evolve" + 2 + "/Material").GetComponent<MaterialItsm>();
        //        }
        //    }

        //    //材料按钮动态添加时间
        //    for (int i = 0; i < GBItem.Length; i++)
        //    {
        //        if (i == 0)
        //        {
        //            GBItem[i] = transform.Find("EquipEvolve/EquipEvolves/Evolve" + 0 + "/Material/MaterialName").GetComponent<GUISingleButton>();
        //            GBItem[i].index = i;
        //        }
        //        else if (i >= 1 && i < 3)
        //        {
        //            GBItem[i] = transform.Find("EquipEvolve/EquipEvolves/Evolve" + 1 + "/Material/MaterialName").GetComponent<GUISingleButton>();
        //            GBItem[i].index = i;
        //        }
        //        else if (i >= 3 && i < 6)
        //        {
        //            GBItem[i] = transform.Find("EquipEvolve/EquipEvolves/Evolve" + 2 + "/Material/MaterialName").GetComponent<GUISingleButton>();
        //            GBItem[i].index = i;
        //        }
        //    }
        //    //合成
        //    for (int i = 0; i < MI2.Length; i++)
        //    {
        //        if (i == 0)
        //        {
        //            MI2[i] = transform.Find("EquipEvolve/MaterialCompound/material" + 0 + "/Material").GetComponent<MaterialItsm>();
        //        }
        //        else if (i >= 1 && i < 3)
        //        {
        //            MI2[i] = transform.Find("EquipEvolve/MaterialCompound/material" + 1 + "/Material").GetComponent<MaterialItsm>();
        //        }
        //        else if (i >= 3 && i < 6)
        //        {
        //            MI2[i] = transform.Find("EquipEvolve/MaterialCompound/material" + 2 + "/Material").GetComponent<MaterialItsm>();
        //        }
        //        else if (i >= 6)
        //        {
        //            MI2[i] = transform.Find("EquipEvolve/MaterialCompound/material" + 3 + "/Material").GetComponent<MaterialItsm>();
        //        }

        //    }

        //    //材料按钮动态添加时间
        //    for (int i = 0; i < GBItem2.Length; i++)
        //    {
        //        if (i == 0)
        //        {
        //            GBItem2[i] = transform.Find("EquipEvolve/MaterialCompound/material" + 0 + "/Material/MaterialName").GetComponent<GUISingleButton>();
        //            GBItem2[i].index = i;
        //        }
        //        else if (i >= 1 && i < 3)
        //        {
        //            GBItem2[i] = transform.Find("EquipEvolve/MaterialCompound/material" + 1 + "/Material/MaterialName").GetComponent<GUISingleButton>();
        //            GBItem2[i].index = i;
        //        }
        //        else if (i >= 3 && i < 6)
        //        {
        //            GBItem2[i] = transform.Find("EquipEvolve/MaterialCompound/material" + 2 + "/Material/MaterialName").GetComponent<GUISingleButton>();
        //            GBItem2[i].index = i;
        //        }
        //        else if (i >= 6)
        //        {
        //            GBItem2[i] = transform.Find("EquipEvolve/MaterialCompound/material" + 3 + "/Material/MaterialName").GetComponent<GUISingleButton>();
        //            GBItem2[i].index = i;
        //        }
        //    }
        //}
        //for (int i = 0; i < GBItem.Length; i++)
        //{
        //    GBItem[i].onItemClick = MIClick;
        //}
        //for (int i = 0; i < GBItem2.Length; i++)
        //{
        //    GBItem2[i].onItemClick = MI2Click;
        //}
        //CompoundBtn.onClick = EquipCompoundOnClick;
        //CancelBtn.onClick = OneCancelOnClick;

        //EquipEvolveBtn.onClick = EquipEvolveOnClick;
        //OneEvolvesBtn.onClick = OneEvolvesOnClick;
        //CompoundBox.onClick = Reverse;
        //IntensifyBtn.onClick = IntensifyBtnClick;
        //EvolveBtn.onClick = EvolveBtnClick;
        //ALLIntensify.onClick = ALLIntensifyBtnClick;
        //OneBtnIntensify.onClick = OneBtnIntensifyBtnClick;
        //IntensifyImg.gameObject.SetActive(true);
        //if (EquipPanel.instance != null && EquipPanel.instance.ItemEquiplist[UI_HeroDetail.equipItemState] != null)
        //    RefreshUI(1, EquipPanel.instance.ItemEquiplist[UI_HeroDetail.equipItemState]);
    }
    // Update is called once per frame
    void Update () {
	
	}
}
