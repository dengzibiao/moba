using UnityEngine;
using System.Collections;
using Tianyu;
using System;
using System.Collections.Generic;

public class HeroPreview : GUIBase
{
    public UILabel Money;
    public GUISingleMultList HeroPreviewList;
    public static List<HeroAttrNode> itemRankList = new List<HeroAttrNode>();

    public UISprite HeroUpFront;
    public UISprite HeroUpFrontIcon;
    public UILabel HeroUpFrontName;

    public UISprite HeroUpQueen;
    public UISprite HeroUpQueenIcon;
    public UILabel HeroUpQueenName;

    public GUISingleLabel hintLabel;


    public GUISingleLabel attrValue;
    public GUISingleLabel attrName;
    public GUISingleLabel nextAttrValue;
    public GUISingleLabel advancedLevelLabel;
    public UIScrollView heroPreView;
    /// <summary>
    /// 单例
    /// </summary>
    private static HeroPreview mSingleton;
    public static HeroPreview Instance()
    {
        if (mSingleton == null)
            mSingleton = new HeroPreview();
        return mSingleton;
    }
    public HeroPreview()
    {
        mSingleton = this;
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
    }
    protected override void ShowHandler()
    {

        RefreshData();
    }
    public void RefreshData()
    {
        HeroUpFrontName.text = "[" + GoodsDataOperation.GetInstance().GetNameColourByHeroGrade(UI_HeroDetail.hd.grade) + "]" + UI_HeroDetail.hd.node.name + "[-]";
        HeroUpQueenName.text = "[" + GoodsDataOperation.GetInstance().GetNameColourByHeroGrade(UI_HeroDetail.hd.grade + 1) + "]" + UI_HeroDetail.hd.node.name + "[-]";
        HeroUpFrontIcon.spriteName = UI_HeroDetail.hd.node.icon_name;
        HeroUpQueenIcon.spriteName = UI_HeroDetail.hd.node.icon_name;
        HeroUpFront.spriteName = UISign_in.GetHeroGradeName(UI_HeroDetail.hd.grade);
        HeroUpQueen.spriteName = UISign_in.GetHeroGradeName(UI_HeroDetail.hd.grade + 1);
     
        foreach (var item in FSDataNodeTable<HeroAttrNode>.GetSingleton().DataNodeList.Values)
        {
            if (item.grade == UI_HeroDetail.hd.grade && item.id == UI_HeroDetail.hd.id || item.grade == UI_HeroDetail.hd.grade + 1 && item.id == UI_HeroDetail.hd.id)
            {
                itemRankList.Add(item);
            }
            if (item.grade == UI_HeroDetail.hd.grade && item.id == UI_HeroDetail.hd.id)
            {
                Money.text = item.break_gold.ToString();
            }
            if (item.grade == UI_HeroDetail.hd.grade && item.name == UI_HeroDetail.hd.node.name)
            {
                advancedLevelLabel.text = "进阶下一级英雄需要达到"+item.break_lv.ToString();
            }
            
        }
        GetHeroArr();
        //HeroPreviewList.InSize(18, 1);
        //HeroPreviewList.Info(itemRankList.ToArray());
        SetLabelHint(UI_HeroDetail.hd.grade);
    }
    private void GetHeroArr()
    {
        string name = "";
        string currentattrstr = "";
        string nextlvattrstr = "";
        playerData.GetInstance().selectHeroDetail.RefreshAttr();
        float[] itemarr = playerData.GetInstance().selectHeroDetail.charAttrs;
        HeroData myHd = playerData.GetInstance().selectHeroDetail;
        HeroData nextGradeHd = new HeroData(myHd.id, myHd.lvl, myHd.grade + 1, myHd.star);
        nextGradeHd.equipSite = myHd.equipSite;
        nextGradeHd.RefreshAttr();
        float[] nextgradearr = nextGradeHd.charAttrs;
        for (int i = 0; i < itemarr.Length; i++)
        {
            if (itemarr[i]>0)
            {
                name += propertyname[i] + "\n";
                currentattrstr += ((int)itemarr[i]).ToString() + "\n";
                nextlvattrstr += ((int)nextgradearr[i]).ToString() + "\n";
            }
        }
        nextGradeHd = null;
        heroPreView = transform.Find("HeroPreview").GetComponent<UIScrollView>();
        attrName = transform.Find("HeroPreview/AttrName").GetComponent<GUISingleLabel>();
        attrValue = transform.Find("HeroPreview/AttrValue").GetComponent<GUISingleLabel>();
        nextAttrValue = transform.Find("HeroPreview/NextAttrValue").GetComponent<GUISingleLabel>();
        if (attrName!=null)
        {
            attrName.text = name;
            UIWidget widget = attrName.GetComponent<UIWidget>();
            if (widget != null)
            {
                bool vis = widget.isVisible;

                if (!vis) this.gameObject.SetActive(true);

                this.gameObject.SetActive(vis);
            }
            NGUITools.AddWidgetCollider(attrName.gameObject, true);
        }
        if (attrValue!=null)
        {
            attrValue.text = currentattrstr;
        }
        if (nextAttrValue!=null)
        {
            nextAttrValue.text = nextlvattrstr;
        }
        if (heroPreView!=null)
        {
            heroPreView.ResetPosition();
        }
       
    }
    string[] propertyname = { "力量","智力","敏捷","生命","攻击","护甲","魔抗","暴击","闪避","命中",
        "护甲穿透","魔法穿透","吸血","韧性","移动速度","攻击速度","攻击距离","生命恢复"};
    //英雄品级 1：白，2：绿，3：蓝，4：紫，5：橙:，6：红；
    private void SetLabelHint(int grade)
    {
        switch (grade)
        {
            case 1:
                hintLabel.text = "装备均达到品质[" + GoodsDataOperation.GetInstance().GetNameColourByHeroGrade(UI_HeroDetail.hd.grade+1) + "]绿色品质[-]以上，英雄方可进阶";
                break;
            case 2:
                hintLabel.text = "装备均达到品质[" + GoodsDataOperation.GetInstance().GetNameColourByHeroGrade(UI_HeroDetail.hd.grade+1) + "]蓝色品质[-]以上，英雄方可进阶";
                break;
            case 3:
                hintLabel.text = "装备均达到品质[" + GoodsDataOperation.GetInstance().GetNameColourByHeroGrade(UI_HeroDetail.hd.grade+1) + "]紫色品质[-]以上，英雄方可进阶";
                break;
            case 4:
                hintLabel.text = "装备均达到品质[" + GoodsDataOperation.GetInstance().GetNameColourByHeroGrade(UI_HeroDetail.hd.grade+1) + "]橙色品质[-]以上，英雄方可进阶";
                break;
            case 5:
                hintLabel.text = "装备均达到品质[" + GoodsDataOperation.GetInstance().GetNameColourByHeroGrade(UI_HeroDetail.hd.grade+1) + "]红色品质[-]以上，英雄方可进阶";
                break;
            case 6:
                hintLabel.text = "英雄已达到最大品质";
                break;
            default:
                break;
        }
        
    }
}
