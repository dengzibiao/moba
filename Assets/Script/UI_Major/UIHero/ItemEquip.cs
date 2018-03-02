using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class ItemEquip : MonoBehaviour
{

   // public static ItemEquip instance;



    UISprite border;        //底框
    UIButton mailBtn;       //装备按钮
    UILabel label;          //等级
    public UISprite hahaha;          //等级
    List<UISprite> starList = new List<UISprite>();

    //site 1武器 2头盔 3胸甲 4腿甲 5护手 6鞋子
    public int site = 0;

    [HideInInspector]
    public BackpackEquipVO equipVO;

    [HideInInspector]
    public ItemNodeState itemVO;
    public EquipData equipData;

    void Awake()
    {
      //  instance = this;
        border = GetComponent<UISprite>();
        mailBtn = transform.Find("MailBtn").GetComponent<UIButton>();
        label = transform.Find("Label").GetComponent<UILabel>();

        for (int i = 1; i < 6; i++)
        {
            starList.Add(UnityUtil.FindCtrl<UISprite>(gameObject, "Star" + i));
        }
    }

    void Start()
    {
        EventDelegate.Set(mailBtn.onClick, OnMailBtnClick);
        //transform.parent.parent.GetComponent<UI_HeroDetail>().equipPosList.Add(border.GetComponent<TweenPosition>());
    }

    /// <summary>
    /// 装备信息赋值
    /// </summary>
    /// <param name="site">site 1武器 2头盔 3胸甲 4腿甲 5护手 6鞋子</param>
    /// <param name="item">装备</param>
    public void Init(int site, ItemNodeState item)
    {

        this.site = site;
        
       // itemVO = new ItemNodeState();

        itemVO = item;

        mailBtn.normalSprite = item.icon_name;

        HeroData hd = playerData.GetInstance().GetHeroDataByID(Globe.selectHero.hero_id);

        EquipData ed;

        //hd.equipID.TryGetValue(item.props_id, out ed);
        hd.equipSite.TryGetValue(site, out ed);
        equipData = ed;
        if (ed != null)
        {
            label.text = ed.level + "级";
            UpdateBorder(ed.grade);
        }
        else
        {
            label.text = 1 + "级";
            UpdateBorder(0);
        }

    }

    /// <summary>
    /// 显示装备边框
    /// </summary>
    public void ShowBorder()
    {
        switch (equipVO.qualitytype)
        {
            case 1:
                border.spriteName = "hui";
                break;
            case 2:
                border.spriteName = "lv";
                break;
            case 3:
                border.spriteName = "lan";
                break;
            case 4:
                border.spriteName = "zi";
                break;
            case 5:
                border.spriteName = "cheng";
                break;
            case 6:
                border.spriteName = "hong";
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 装备按钮
    /// </summary>
    /// <param name="go"></param>
    public void OnMailBtnClick()
    {
        if (HeroAndEquipNodeData.TabType == 2)
        {
            HeroAndEquipNodeData.TabType = 1;
        }
        //EquipPanel.instance.sethahaha(this.site);
        HeroAndEquipNodeData.site = this.site;
        if (HeroAndEquipNodeData.TanNUm == 3)
        {
            UI_HeroDetail.equipItemState = 3;
            if(EquipOperation.Instance()!=null)
            EquipOperation.Instance().RefreshUI(this.site, this);
        }
        else
        {
            UI_HeroDetail.equipItemState = 0;
            if(EquipOperation.Instance()!=null)
            EquipOperation.Instance().RefreshUI(this.site, this);
        }
        //UIEquipDetailPanel.instance.SetData(equipData);
        //Control.ShowGUI(GameLibrary.UIEquipDetailPanel);
        Control.ShowGUI(UIPanleID.UIEquipDetailPanel, EnumOpenUIType.DefaultUIOrSecond, false, equipData);
    }

    /// <summary>
    /// 更换英雄框
    /// </summary>
    void UpdateBorder(int grade)
    {
        border.spriteName = UISign_in.GetspriteName(grade);
        if (grade <= 1)
        {
            //白
           

            UpdateStar(0);

        }
        else if (grade == 2 || grade == 3)
        {
            //绿
          

            UpdateStar(grade - 2);

        }
        else if (grade == 4 || grade == 5 || grade == 6)
        {
            //蓝
          

            UpdateStar(grade - 4);

        }
        else if (grade == 7 || grade == 8 || grade == 9 || grade == 10)
        {
            //紫
          
            UpdateStar(grade - 7);
        }
        else if (grade == 11 || grade == 12 || grade == 13 || grade == 14 || grade == 15)
        {
            //橙
           
            UpdateStar(grade - 11);
        }
        else if (grade == 16 || grade == 17 || grade == 18 || grade == 19 || grade == 20 || grade == 21)
        {
            //红
           
            UpdateStar(grade - 16);
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

}