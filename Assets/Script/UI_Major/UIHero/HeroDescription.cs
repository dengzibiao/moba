using UnityEngine;
using System.Collections;
using Tianyu;
using System.Collections.Generic;

public class HeroDescription : GUIBase
{
   // public GameObject property;

    UI_HeroDetail heroDetail;

    public GUISingleMultList FeatureList;
    public GUISingleMultList HeroPreviewList;
    public UISprite obj;
    public UIButton AttributesBtn;
    public UIButton IntroductionBtn;
    public GameObject Introduction;
    public GameObject Attributes;
    public UISprite AttributesImg;
    public UISprite IntroductionImg;

    //public UILabel strengthGrowthLabel;
    //public UILabel intelligenceGrowthLabel;
    //public UILabel agileGrowthLabel;
    //public UISprite PowerProgress;
    //public UISprite IntelligenceProgress;
    //public UISprite AgilityProgress;
    public UILabel describe;
    public UILabel info;

    public GUISingleButton Mount;
    public GUISingleButton Baby;

    private UIGrid dingweigrid;
    private UIGrid tediangrid;
    public Transform[] dingweiT;
    public Transform[] tedianT;
    private UILabel heroInfo;
    public UILabel attrValue;
    public UILabel attrName;
    /// <summary>
    /// 单例
    /// </summary>
    private static HeroDescription mSingleton;
    public static HeroDescription Instance()
    {
        if (mSingleton == null)
            mSingleton = new HeroDescription();
        return mSingleton;
    }
    public override UIPanleID GetUIKey()
    {
        return UIPanleID.none;
    }


    protected override void Init()
    {
        dingweigrid = transform.Find("DingweiContainer/Grid").GetComponent<UIGrid>();
        tediangrid = transform.Find("TedianContainer/Grid").GetComponent<UIGrid>();
        heroInfo = transform.Find("HeroInfo").GetComponent<UILabel>();
        attrValue = transform.Find("obj/HeroPreview/AttrValue").GetComponent<UILabel>();
        attrName = transform.Find("obj/HeroPreview/AttrName").GetComponent<UILabel>();
        heroDetail = GetComponentInParent<UI_HeroDetail>();
        EventDelegate.Set(AttributesBtn.onClick, OnAttributesBtn);
        EventDelegate.Set(IntroductionBtn.onClick, OnIntroductionBtn);
        AttributesImg.gameObject.SetActive(true);
        //PowerProgress.width = 0;
        //PowerProgress.pivot = UIWidget.Pivot.Left;
        //IntelligenceProgress.width = 0;
        //IntelligenceProgress.pivot = UIWidget.Pivot.Left;
        //AgilityProgress.width = 0;
        //AgilityProgress.pivot = UIWidget.Pivot.Left;
        Mount.onClick = ShowMount;
        Baby.onClick = ShowBaby;
    }

    public void InitDesInfo()
    {
        Introduction.SetActive(false);
        Attributes.SetActive(false);
    }
    protected override void ShowHandler()
    {
        RefreshDesInfo();
        OnAttributesBtn();
        SetDingweiAndTedian();
    }
    public void RefreshDesInfo()
    {
        //strengthGrowthLabel.text = "" + UI_HeroDetail.hd.node.GetStarGrowUpRate(0, UI_HeroDetail.hd.star);
        //PowerProgress.width = 60;
        //intelligenceGrowthLabel.text = "" + UI_HeroDetail.hd.node.GetStarGrowUpRate(1, UI_HeroDetail.hd.star);
        //IntelligenceProgress.width = HeroDescription.Instance().GetProgressLength(UI_HeroDetail.hd.node.GetStarGrowUpRate(1, UI_HeroDetail.hd.star));
        //agileGrowthLabel.text = "" + UI_HeroDetail.hd.node.GetStarGrowUpRate(2, UI_HeroDetail.hd.star);
        //AgilityProgress.width = HeroDescription.Instance().GetProgressLength(UI_HeroDetail.hd.node.GetStarGrowUpRate(2, UI_HeroDetail.hd.star));
        //FeatureList.InSize(12, 12);
        if (null == UI_HeroDetail.hd) return;
        describe.text = UI_HeroDetail.hd.node.describe;
        info.GetComponent<UITextList>().Clear();
        info.GetComponent<UITextList>().Add("" + UI_HeroDetail.hd.node.info);

        foreach (var item in FSDataNodeTable<HeroAttrNode>.GetSingleton().DataNodeList.Values)
        {
            if (item.grade == UI_HeroDetail.hd.grade && item.id == UI_HeroDetail.hd.id || item.grade == UI_HeroDetail.hd.grade + 1 && item.id == UI_HeroDetail.hd.id)
            {
                HeroPreview.itemRankList.Add(item);
            }
        }
       // HeroPreviewList.InSize(18, 1);
       // HeroPreviewList.Info(HeroPreview.itemRankList.ToArray());
        GeEquipStrengthArr();
    }
    private void GeEquipStrengthArr()
    {
        playerData.GetInstance().selectHeroDetail.RefreshAttr(); ;
        float[] basepropertys = playerData.GetInstance().selectHeroDetail.charAttrs;
        if (basepropertys == null)
            return;
        string name = "";
        string currentattrstr = "";
        for (int i = 0; i < basepropertys.Length; i++)
        {
            if (basepropertys[i] > 0)
            {
                name += propertyname[i] + "\n";
                currentattrstr += ((int)basepropertys[i]).ToString() + "\n";
            }
        }
        attrValue = transform.Find("obj/HeroPreview/AttrValue").GetComponent<UILabel>();
        attrName = transform.Find("obj/HeroPreview/AttrName").GetComponent<UILabel>();
        if (attrValue!=null)
        {
            attrValue.text = currentattrstr;
        }
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
    }
   
    string[] propertyname = { "力量","智力","敏捷","生命","攻击","护甲","魔抗","暴击","闪避","命中",
        "护甲穿透","魔法穿透","吸血","韧性","移动速度","攻击速度","攻击距离","生命恢复"};
    public void OnAttributesBtn()
    {
		if( UI_HeroDetail.instance!=null)
        UI_HeroDetail.instance.ScrollViewResetPosition();
        ShowOrHidePanel(false);
        obj.gameObject.SetActive(true);
        IntroductionImg.gameObject.SetActive(false);
        AttributesImg.gameObject.SetActive(true);
      //  property.gameObject.SetActive(true);
    }

    void OnIntroductionBtn()
    {
        UI_HeroDetail.instance.ScrollViewResetPosition();
        obj.gameObject.SetActive(false);
        ShowOrHidePanel(true);
        IntroductionImg.gameObject.SetActive(true);
        AttributesImg.gameObject.SetActive(false);
       // property.gameObject.SetActive(false);
    }

    public void ShowOrHidePanel(bool isShow)
    {
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);
        Introduction.SetActive(isShow);
    }
    public int GetProgressLength(float Num)
    {
        return (int)Mathf.Round(Num*14.2f);
    }
    void ShowMount()
    {
        //Control.HideGUI(UIPanleID.UIHeroDetail);
        //UIMountAndPet.Instance.SetShowType(MountAndPet.Mount,EntranceType.Hero);
        //Control.ShowGUI(UIPanleID.UIMountAndPet);
    }
    void ShowBaby()
    {
        //UIMountAndPet.Instance.SetShowType(MountAndPet.Pet,EntranceType.Hero);
        //Control.HideGUI(UIPanleID.UIHeroDetail);
        //Control.ShowGUI(UIPanleID.UIMountAndPet);
    }
    /// <summary>
    /// 设置定位 特点图标
    /// </summary>
    public void SetDingweiAndTedian()
    {
        //英雄定位 1：战士；2：肉盾；3：刺客；4：射手；5：法师；6：辅助；
        //英雄特点 0：先手；1：突进；2：爆发；3：吸血；4：团控；5：收割；6：控制；7：推进；8：消耗；9：恢复；10：增益；
        dingweigrid = transform.Find("DingweiContainer/Grid").GetComponent<UIGrid>();
        tediangrid = transform.Find("TedianContainer/Grid").GetComponent<UIGrid>();
        heroInfo = transform.Find("HeroInfo").GetComponent<UILabel>();
        if (null == UI_HeroDetail.hd) return;
        heroInfo.text = UI_HeroDetail.hd.node.info;
        int[] dingwei = UI_HeroDetail.hd.node.dingwei;
        int[] tedian = UI_HeroDetail.hd.node.characteristic;
        for (int j = 0; j < dingweiT.Length; j++)
        {
            dingweiT[j].gameObject.SetActive(false);
        }
        for (int j = 0; j < tedianT.Length; j++)
        {
            tedianT[j].gameObject.SetActive(false);
        }
        if (dingwei!=null)
        {
            for (int i=0;i<dingwei.Length;i++)
            {
                for (int j =0;j<dingweiT.Length;j++)
                {
                    if ((dingwei[i] - 1) == j)
                    {
                        dingweiT[j].gameObject.SetActive(true);
                        break;
                    }
                }
            }
        }
        if (tedian != null)
        {
            for (int i = 0; i < tedian.Length; i++)
            {
                for (int j = 0; j < tedianT.Length; j++)
                {
                    if ((tedian[i]) == j)
                    {
                        tedianT[j].gameObject.SetActive(true);
                        break;
                    }
                }
            }
        }
        dingweigrid.Reposition();
        tediangrid.Reposition();
    }
    
}
