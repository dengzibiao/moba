using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// 升星面板
/// </summary>

public class RisingStarPanel : GUIBase
{

    //public delegate void OnRisingStarChange();
    //public event OnRisingStarChange OnChangeStar;

    public static RisingStarPanel instance;

    public GUISingleButton closeBtn;      //关闭按钮
    public GUISingleSprite bGFront;       //升星前头像底框
    public GUISingleSprite bGPost;        //升星后头像底框
    public GUISingleSprite iconF;         //人物头像
    public GUISingleSprite iconR;         //人物头像
    public GUISingleLabel powerF;         //升星前力量
    public GUISingleLabel powerR;         //升星后力量
    public GUISingleLabel powerAdd;       //升星增加
    public GUISingleLabel agileF;         //升星前智力
    public GUISingleLabel agileR;         //升星后智力
    public GUISingleLabel agileAdd;       //升星增加
    public GUISingleLabel intelF;         //升星前敏捷
    public GUISingleLabel intelR;         //升星后敏捷
    public GUISingleLabel intelAdd;       //升星增加

    HeroVO vo;

    public override UIPanleID GetUIKey()
    {
        return UIPanleID.none;
    }
    protected override void Init()
    {
        base.Init();

        instance = this;

        closeBtn.onClick = OnCloseBtn;

    }

    protected override void ShowHandler()
    {
        base.ShowHandler();

        RisingStar();

    }

    /// <summary>
    /// 升星
    /// </summary>
    public void RisingStar()
    {

        //Globe.allHeroDic.TryGetValue(Globe.selectHero.hero_id, out vo);

        iconF.spriteName = vo.original_painting;
        iconR.spriteName = vo.original_painting;

        if (vo.init_star < 5)
        {
            UpdateBorder(bGFront);

            vo.init_star++;

            UpdateBorder(bGPost);

            //if (OnChangeStar != null)
            //{
            //    OnChangeStar();
            //}

            //UI_HeroDetail.instance.UpdateStar();
        }

    }

    /// <summary>
    /// 关闭按钮
    /// </summary>
    /// <param name="go"></param>
    private void OnCloseBtn()
    {
       // Control.Hide(GameLibrary.UIRisingStarPanel);
    }

    /// <summary>
    /// 更换底框
    /// </summary>
    /// <param name="border"></param>
    void UpdateBorder(GUISingleSprite border)
    {
        switch (vo.init_star)
        {
            case 0:
                border.spriteName = "baiyingxiongkuang";
                break;
            case 1:
                border.spriteName = "lvyingxiongkuang";
                UpdateStar(border, 1);
                break;
            case 2:
                border.spriteName = "lanyingxiongkuang";
                UpdateStar(border, 2);
                break;
            case 3:
                border.spriteName = "ziyingxiongkuang";
                UpdateStar(border, 3);
                break;
            case 4:
                border.spriteName = "chengyingxiongkuang";
                UpdateStar(border, 4);
                break;
            case 5:
                border.spriteName = "hongyingxiongkuang";
                UpdateStar(border, 5);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 更新星星
    /// </summary>
    /// <param name="border"></param>
    /// <param name="star"></param>
    void UpdateStar(GUISingleSprite border, int star)
    {
        for (int i = 1; i <= star; i++)
        {
            border.transform.Find("Star" + i).gameObject.SetActive(true);
        }
        for (int i = star + 1; i <= 5; i++)
        {
            border.transform.Find("Star" + i).gameObject.SetActive(false);
        }
    }

}